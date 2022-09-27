using SimpleJSON;
using UnityEngine;

public class Holdable : Selectable
{
	[HideInInspector]
	public bool m_BeingHeld;

	[HideInInspector]
	public int m_Weight;

	[HideInInspector]
	public bool m_AllowMultiple;

	[HideInInspector]
	public int m_UsageCount;

	[HideInInspector]
	public int m_MaxUsageCount;

	private Layers m_OldLayer;

	[HideInInspector]
	public float m_LifeTimer;

	public static int GetWeight(ObjectType NewType)
	{
		int variableAsInt = VariableManager.Instance.GetVariableAsInt(NewType, "Weight", CheckValid: false);
		if (variableAsInt == 0)
		{
			return 1;
		}
		return variableAsInt;
	}

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("Holdable", m_TypeIdentifier);
	}

	public override void Restart()
	{
		base.Restart();
		m_UsageCount = 0;
		m_BeingHeld = false;
		m_LifeTimer = 0f;
		if ((bool)SaveLoadManager.Instance && !SaveLoadManager.Instance.m_Loading)
		{
			DespawnManager.Instance.Add(this);
		}
		UpdateTierScale();
	}

	protected new void Awake()
	{
		base.Awake();
		m_Weight = 1;
		m_AllowMultiple = true;
		m_OldLayer = Layers.Total;
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		if (m_BeingHeld)
		{
			m_BeingHeld = false;
			InstantiationManager.Instance.SetLayer(base.gameObject, m_OldLayer);
			m_OldLayer = Layers.Total;
		}
		base.StopUsing(AndDestroy);
		DespawnManager.Instance.Remove(this);
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_Weight = GetWeight(m_TypeIdentifier);
		m_AllowMultiple = VariableManager.Instance.GetVariableAsInt(m_TypeIdentifier, "NoMultiple", CheckValid: false) == 0;
		m_MaxUsageCount = VariableManager.Instance.GetVariableAsInt(m_TypeIdentifier, "MaxUsage", CheckValid: false);
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		if (m_MaxUsageCount != 0)
		{
			JSONUtils.Set(Node, "Used", m_UsageCount);
		}
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		if (m_MaxUsageCount != 0)
		{
			m_UsageCount = JSONUtils.GetAsInt(Node, "Used", 0);
		}
	}

	public override void SetHighlight(bool Highlighted)
	{
		base.SetHighlight(Highlighted);
	}

	public void UpdateTierScale()
	{
		int tierFromType = BaseClass.GetTierFromType(m_TypeIdentifier);
		if (tierFromType != 0)
		{
			float tierScale = BaseClass.GetTierScale(tierFromType);
			base.transform.localScale = new Vector3(tierScale, tierScale, tierScale);
		}
		else
		{
			base.transform.localScale = new Vector3(1f, 1f, 1f);
		}
	}

	public override bool IsSelectable()
	{
		return !m_BeingHeld;
	}

	public override void WorldCreated()
	{
		base.WorldCreated();
		DespawnManager.Instance.Remove(this);
	}

	protected virtual void ActionBeingHeld(Actionable Holder)
	{
		if (!m_BeingHeld)
		{
			SetIsSavable(IsSavable: false);
			m_BeingHeld = true;
			m_OldLayer = (Layers)base.gameObject.layer;
			InstantiationManager.Instance.SetLayer(base.gameObject, Layers.Used0);
			PlotManager.Instance.RemoveObject(this);
			DespawnManager.Instance.Remove(this);
			base.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		}
	}

	protected virtual void ActionDropped(Actionable PreviousHolder, TileCoord DropLocation)
	{
		if (!m_BeingHeld)
		{
			return;
		}
		base.transform.parent = MapManager.Instance.m_ObjectsRootTransform;
		SetPosition(DropLocation.ToWorldPositionTileCentered());
		InstantiationManager.Instance.SetLayer(base.gameObject, m_OldLayer);
		m_OldLayer = Layers.Total;
		SetIsSavable(IsSavable: true);
		m_BeingHeld = false;
		Tile tile = TileManager.Instance.GetTile(DropLocation);
		if (TileHelpers.GetTileWater(tile.m_TileType) && tile.m_Floor == null)
		{
			if ((bool)PreviousHolder)
			{
				AudioManager.Instance.StartEvent("ToolPaddleSplash", PreviousHolder.GetComponent<TileCoordObject>());
			}
			MyParticles newParticles = ParticlesManager.Instance.CreateParticles("Splash", base.transform.position, Quaternion.Euler(-90f, 0f, 0f));
			ParticlesManager.Instance.DestroyParticles(newParticles, WaitUntilNoParticles: true);
		}
	}

	protected virtual void Used()
	{
	}

	public float GetUsed()
	{
		return (float)m_UsageCount / (float)m_MaxUsageCount;
	}

	public override void SendAction(ActionInfo Info)
	{
		switch (Info.m_Action)
		{
		case ActionType.BeingHeld:
			ActionBeingHeld(Info.m_Object);
			break;
		case ActionType.Dropped:
			ActionDropped(Info.m_Object, Info.m_Position);
			break;
		}
		base.SendAction(Info);
	}

	public override bool CanDoAction(ActionInfo Info, bool RightNow)
	{
		switch (Info.m_Action)
		{
		case ActionType.BeingHeld:
			if (!m_BeingHeld)
			{
				return true;
			}
			return false;
		case ActionType.Dropped:
			if (m_BeingHeld)
			{
				return true;
			}
			return false;
		default:
			return base.CanDoAction(Info, RightNow);
		}
	}

	public override object GetActionInfo(GetActionInfo Info)
	{
		switch (Info.m_Action)
		{
		case GetAction.IsUsed:
			if (m_MaxUsageCount > 0 && m_UsageCount >= m_MaxUsageCount)
			{
				return true;
			}
			return false;
		case GetAction.IsStorable:
			return true;
		default:
			return base.GetActionInfo(Info);
		}
	}

	protected ActionType GetActionFromBroom(AFO Info)
	{
		Info.m_FarmerState = Farmer.State.Sweeping;
		if (Info.m_ObjectType != ObjectType.ToolBroom)
		{
			return ActionType.Fail;
		}
		if (!ToolBroom.GetIsObjectAcceptable(this))
		{
			return ActionType.Total;
		}
		return ActionType.UseInHands;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (Info.m_ActionType == AFO.AT.Primary)
		{
			if (Info.m_ObjectType == ObjectType.ToolBroom)
			{
				return GetActionFromBroom(Info);
			}
			if (ModCheckUseObjectOnObject(Info))
			{
				Info.m_FarmerState = Farmer.State.ModAction;
				Info.m_EndAction = ModEndAction;
				return ActionType.UseInHands;
			}
			Info.m_FarmerState = Farmer.State.PickingUp;
			if (CanDoAction(new ActionInfo(ActionType.BeingHeld, default(TileCoord), Info.m_Actioner, "", "", Info.m_ActionType, Info.m_RequirementsIn), RightNow: true))
			{
				return ActionType.Pickup;
			}
			return ActionType.Fail;
		}
		return base.GetActionFromObject(Info);
	}

	protected ActionType GetActionFromCurrentState(AFO Info, string TypeString, string StateString)
	{
		ObjectType objectType = Info.m_ObjectType;
		if (Info.m_ActionType == AFO.AT.Primary)
		{
			if (objectType == ObjectTypeList.m_Total || objectType == m_TypeIdentifier)
			{
				string text = "FNO" + TypeString + StateString;
				Info.m_FarmerState = Farmer.State.PickingUp;
				Info.m_RequirementsOut = text;
				if (Info.m_RequirementsIn == "" || Info.m_RequirementsIn == text)
				{
					return ActionType.Pickup;
				}
				if (Info.m_RequirementsIn != "")
				{
					return ActionType.Fail;
				}
				Info.m_RequirementsOut = "";
			}
			return ActionType.Total;
		}
		return base.GetActionFromObject(Info);
	}

	public void Use(int Amount = 1)
	{
		m_UsageCount += Amount;
		if (m_UsageCount >= m_MaxUsageCount)
		{
			m_UsageCount = m_MaxUsageCount;
			Used();
		}
	}

	public bool GetIsHeavyForPlayer()
	{
		if (m_Weight >= 3)
		{
			return true;
		}
		return false;
	}

	public bool GetIsVeryHeavyForPlayer()
	{
		if (m_Weight >= 4)
		{
			return true;
		}
		return false;
	}
}
