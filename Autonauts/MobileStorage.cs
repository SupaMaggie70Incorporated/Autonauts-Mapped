using SimpleJSON;
using UnityEngine;

public class MobileStorage : Vehicle
{
	[HideInInspector]
	public ObjectType m_ObjectType;

	[HideInInspector]
	public int m_WeightCapacity;

	[HideInInspector]
	public int m_Capacity;

	protected int m_Stored;

	protected int m_Reserved;

	private MeshRenderer m_Sign;

	private bool m_FullOn;

	private float m_FullTimer;

	public static bool GetIsTypeMobileStorage(ObjectType NewType)
	{
		if (MobileStorageGeneral.GetIsTypeMobileStorageGeneral(NewType) || MobileStorageGeneral.GetIsTypeCartLiquid(NewType))
		{
			return true;
		}
		return false;
	}

	public override void Restart()
	{
		base.Restart();
		m_Stored = 0;
		m_Reserved = 0;
		m_MoveSound = null;
		UpdateStored();
	}

	protected new void Awake()
	{
		base.Awake();
		m_ObjectType = ObjectTypeList.m_Total;
		m_WeightCapacity = 0;
		m_Capacity = 0;
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_WeightCapacity = VariableManager.Instance.GetVariableAsInt(m_TypeIdentifier, "WeightCapacity");
		if ((bool)m_ModelRoot.transform.Find("Plane"))
		{
			m_Sign = m_ModelRoot.transform.Find("Plane").GetComponent<MeshRenderer>();
		}
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		if (m_MoveSound != null)
		{
			AudioManager.Instance.StopEvent(m_MoveSound);
		}
		base.StopUsing(AndDestroy);
	}

	public override string GetHumanReadableName()
	{
		string result = base.GetHumanReadableName();
		if (m_ObjectType != ObjectTypeList.m_Total)
		{
			string humanReadableNameFromIdentifier = ObjectTypeList.Instance.GetHumanReadableNameFromIdentifier(m_ObjectType);
			result = TextManager.Instance.Get("StorageAny", humanReadableNameFromIdentifier);
		}
		return result;
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		if (m_ObjectType == ObjectTypeList.m_Total)
		{
			JSONUtils.Set(Node, "ObjectType", "Unknown");
		}
		else
		{
			JSONUtils.Set(Node, "ObjectType", ObjectTypeList.Instance.GetSaveNameFromIdentifier(m_ObjectType));
		}
		JSONUtils.Set(Node, "Stored", m_Stored);
	}

	public override void Load(JSONNode Node)
	{
		string asString = JSONUtils.GetAsString(Node, "ObjectType", "Unknown");
		if (asString == "Unknown")
		{
			m_ObjectType = ObjectTypeList.m_Total;
		}
		else
		{
			m_ObjectType = ObjectTypeList.Instance.GetIdentifierFromSaveName(asString, Check: false);
		}
		m_Stored = JSONUtils.GetAsInt(Node, "Stored", 0);
		SetObjectType(m_ObjectType);
		UpdateStored();
		base.Load(Node);
	}

	protected virtual void UpdateStored()
	{
		if (!m_Sign)
		{
			return;
		}
		if (GetStored() == 0 || GetStored() == GetCapacity())
		{
			if (StandardShaderUtils.GetRenderMode(m_Sign.material) != StandardShaderUtils.BlendMode.Transparent)
			{
				StandardShaderUtils.ChangeRenderMode(m_Sign.material, StandardShaderUtils.BlendMode.Transparent);
				m_Sign.material.color = new Color(0f, 0f, 0f, 1f);
			}
		}
		else if (StandardShaderUtils.GetRenderMode(m_Sign.material) != StandardShaderUtils.BlendMode.Cutout)
		{
			StandardShaderUtils.ChangeRenderMode(m_Sign.material, StandardShaderUtils.BlendMode.Cutout);
			m_Sign.material.color = new Color(1f, 1f, 1f, 1f);
		}
	}

	public int GetStored()
	{
		return m_Stored - m_Reserved;
	}

	public int GetStoredForDisplay()
	{
		return m_Stored;
	}

	public int GetCapacity()
	{
		if (m_ObjectType == ObjectType.Folk)
		{
			return 1;
		}
		return m_Capacity;
	}

	public float GetStoredPercent()
	{
		return (float)GetStored() / (float)GetCapacity();
	}

	public bool GetStoredPercentFull()
	{
		if (GetStoredPercent() >= 1f)
		{
			return true;
		}
		return false;
	}

	public virtual bool CanAcceptObject(BaseClass NewObject, ObjectType NewType)
	{
		if (NewObject == null)
		{
			return false;
		}
		ObjectType objectType = NewType;
		if (!ToolFillable.GetIsTypeFillable(NewObject.m_TypeIdentifier))
		{
			objectType = NewObject.m_TypeIdentifier;
			if (!StorageTypeManager.m_StorageGenericInformation.ContainsKey(objectType) && !StoragePalette.GetIsObjectTypeAcceptable(objectType) && !StorageSand.IsObjectTypeAcceptibleStatic(objectType) && objectType != ObjectType.Folk && objectType != ObjectType.Fertiliser && objectType != ObjectType.Seedling && objectType != ObjectType.SeedlingMulberry)
			{
				return false;
			}
			if (NewObject.GetComponent<Holdable>().m_Weight > m_WeightCapacity)
			{
				return false;
			}
		}
		int stored = GetStored();
		int capacity = GetCapacity();
		if (objectType == ObjectType.Folk && stored > 0)
		{
			return false;
		}
		if (stored > 0 && stored >= capacity)
		{
			if (m_ObjectType == ObjectTypeList.m_Total)
			{
				SetObjectType(objectType);
				if (stored + 1 <= capacity + m_Capacity)
				{
					return true;
				}
			}
			return false;
		}
		if (m_DoingAction)
		{
			return false;
		}
		if (m_ObjectType == ObjectTypeList.m_Total)
		{
			return true;
		}
		if (objectType != m_ObjectType)
		{
			return false;
		}
		return true;
	}

	protected void AddToStored(ObjectType NewType, int Amount, Actionable Actioner)
	{
		if (m_Stored != m_Capacity)
		{
			m_Stored += Amount;
			if (m_Stored > m_Capacity)
			{
				m_Stored = m_Capacity;
			}
			if (CheatManager.Instance.m_FillStorage && (bool)Actioner && Actioner.m_TypeIdentifier == ObjectType.FarmerPlayer)
			{
				m_Stored = m_Capacity;
			}
		}
	}

	protected void ReleaseStored(ObjectType NewType, Actionable Actioner, int Amount = 1)
	{
		if (m_Stored != 0)
		{
			m_Stored -= Amount;
			if (m_Stored < 0)
			{
				m_Stored = 0;
			}
			if (CheatManager.Instance.m_FillStorage && (bool)Actioner && Actioner.m_TypeIdentifier == ObjectType.FarmerPlayer)
			{
				m_Stored = 0;
			}
			if (m_Stored == 0)
			{
				SetObjectType(ObjectTypeList.m_Total);
			}
			if ((bool)Actioner)
			{
				QuestManager.Instance.AddEvent(QuestEvent.Type.StorageUsed, Actioner.m_TypeIdentifier == ObjectType.Worker, m_TypeIdentifier, this);
				BadgeManager.Instance.AddEvent(BadgeEvent.Type.MobileStorage);
			}
		}
	}

	protected void SetSign(ObjectType NewType)
	{
		if (NewType == ObjectTypeList.m_Total)
		{
			NewType = ObjectType.Empty;
		}
		Sprite icon = IconManager.Instance.GetIcon(NewType);
		if ((bool)m_Sign && (bool)icon)
		{
			m_Sign.material.SetTexture("_MainTex", icon.texture);
		}
	}

	protected virtual void SetObjectType(ObjectType NewType)
	{
		m_ObjectType = NewType;
		m_Capacity = 0;
		if (NewType != ObjectTypeList.m_Total)
		{
			m_Capacity = m_WeightCapacity / Holdable.GetWeight(NewType);
		}
		if (m_Stored > m_Capacity)
		{
			m_Stored = m_Capacity;
		}
		SetSign(NewType);
	}

	public override object GetActionInfo(GetActionInfo Info)
	{
		if (Info.m_Action == GetAction.GetObjectType)
		{
			return m_ObjectType;
		}
		return base.GetActionInfo(Info);
	}

	public override void SendAction(ActionInfo Info)
	{
		ActionType action = Info.m_Action;
		if (action == ActionType.Disengaged && m_MoveSound != null)
		{
			AudioManager.Instance.StopEvent(m_MoveSound);
			m_MoveSound = null;
		}
		base.SendAction(Info);
	}

	public override bool CanDoAction(ActionInfo Info, bool RightNow)
	{
		ActionType action = Info.m_Action;
		if (action == ActionType.BeingHeld && (Info.m_Object == null || !Crane.GetIsTypeCrane(Info.m_Object.m_TypeIdentifier)))
		{
			return false;
		}
		return base.CanDoAction(Info, RightNow);
	}

	private void UpdateFull()
	{
		if (!(GetStoredPercent() >= 1f) || GetCapacity() == 0)
		{
			return;
		}
		m_FullTimer += TimeManager.Instance.m_NormalDelta;
		bool flag = (int)(m_FullTimer * 60f) % 20 < 10;
		if (m_FullOn != flag)
		{
			m_FullOn = flag;
			Color color = new Color(1f, 1f, 1f, 1f);
			if (m_FullOn)
			{
				color = new Color(1f, 0f, 0f, 1f);
			}
			if (m_Sign != null)
			{
				m_Sign.material.color = color;
			}
		}
	}

	protected new void Update()
	{
		base.Update();
		UpdateFull();
	}
}
