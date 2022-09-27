using SimpleJSON;
using UnityEngine;

public class FarmerStateBase
{
	private static AFO m_ReusableActionFromObject;

	protected Farmer.State m_State;

	protected Farmer m_Farmer;

	protected Vector3 m_ActionDelta;

	protected Vector3 m_FinalPosition;

	protected Quaternion m_FinalRotation;

	protected float m_GeneralStateScale;

	protected int m_HeldObjectUID;

	public int m_TargetObjectUID;

	protected ObjectType m_TargetObjectType;

	protected AFO m_ActionInfo;

	private TrailMaker m_TrailMaker;

	protected float m_ToolHeight;

	private float m_SafetyTimer;

	protected float m_SafetyDelay = 100f;

	public void SetState(Farmer.State NewState)
	{
		m_State = NewState;
	}

	public Farmer.State GetState()
	{
		return m_State;
	}

	public virtual void Save(JSONNode Node)
	{
	}

	public virtual void Load(JSONNode Node)
	{
	}

	public void SetFarmer(Farmer NewFarmer)
	{
		m_Farmer = NewFarmer;
		m_GeneralStateScale = 1f;
		if ((bool)m_Farmer.GetComponent<Worker>())
		{
			m_GeneralStateScale = 2f;
		}
	}

	public virtual bool GetIsAdjacentTile(TileCoord TargetTile, BaseClass Object)
	{
		return false;
	}

	public virtual string GetNoToolIconName()
	{
		return "Icons/IconEmpty";
	}

	public virtual bool IsToolAcceptable(Holdable NewObject)
	{
		return true;
	}

	public virtual void StartState()
	{
		m_SafetyTimer = 0f;
		Actionable currentObject = m_Farmer.m_FarmerAction.m_CurrentObject;
		m_TargetObjectUID = -1;
		if ((bool)currentObject)
		{
			m_TargetObjectUID = currentObject.m_UniqueID;
			m_TargetObjectType = currentObject.m_TypeIdentifier;
		}
		Holdable topObject = m_Farmer.m_FarmerCarry.GetTopObject();
		if (topObject != null)
		{
			m_HeldObjectUID = topObject.m_UniqueID;
			m_ToolHeight = ObjectTypeList.Instance.GetHeight(topObject.m_TypeIdentifier);
		}
		else
		{
			m_HeldObjectUID = 0;
		}
		if ((bool)currentObject)
		{
			ObjectType newObjectType = ObjectTypeList.m_Total;
			if ((bool)topObject)
			{
				newObjectType = topObject.m_TypeIdentifier;
			}
			AFO.AT newActionType = AFO.AT.Total;
			TileCoord position = m_Farmer.m_TileCoord;
			if (m_Farmer.m_FarmerAction.m_CurrentInfo != null)
			{
				newActionType = m_Farmer.m_FarmerAction.m_CurrentInfo.m_ActionType;
				position = m_Farmer.m_FarmerAction.m_CurrentInfo.m_Position;
			}
			if (m_ReusableActionFromObject == null)
			{
				m_ReusableActionFromObject = new AFO();
			}
			m_ReusableActionFromObject.Init(topObject, newObjectType, m_Farmer, newActionType, "", position);
			currentObject.GetActionFromObject(m_ReusableActionFromObject);
			m_ActionInfo = new AFO(m_ReusableActionFromObject);
		}
		else
		{
			m_ActionInfo = null;
		}
		m_TrailMaker = null;
	}

	protected void FaceTowardsTargetTile()
	{
		m_Farmer.m_FinalPosition = m_Farmer.m_GoToTilePosition.ToWorldPositionTileCentered();
		GetFinalRotationTile();
		Vector3 vector = m_Farmer.m_FinalPosition + m_ActionDelta * Tile.m_Size;
		m_Farmer.m_FinalPosition = vector;
		m_Farmer.transform.position = vector;
		m_Farmer.transform.rotation = m_Farmer.m_FinalRotation;
	}

	protected void FaceTowardsTarget()
	{
		m_Farmer.m_FinalPosition = m_Farmer.m_GoToTilePosition.ToWorldPositionTileCentered();
		GetFinalRotationObject();
		Vector3 vector = m_Farmer.m_FinalPosition + m_ActionDelta * Tile.m_Size;
		m_Farmer.m_FinalPosition = vector;
		m_Farmer.transform.position = vector;
		m_Farmer.transform.rotation = m_Farmer.m_FinalRotation;
	}

	protected void GetFinalRotationTile()
	{
		Vector3 vector = m_Farmer.m_GoToTilePosition.ToWorldPositionTileCentered() - m_Farmer.transform.position;
		float num = 0f - Mathf.Atan2(vector.z, vector.x);
		m_ActionDelta.x = 0f - Mathf.Cos(num);
		m_ActionDelta.y = 0f;
		m_ActionDelta.z = Mathf.Sin(num);
		m_Farmer.m_FinalRotation = Quaternion.Euler(0f, num * 57.29578f - 90f, 0f);
	}

	protected void GetFinalRotationObject()
	{
		Vector3 vector = ObjectTypeList.Instance.GetObjectFromUniqueID(m_TargetObjectUID, ErrorCheck: false).transform.position - m_Farmer.transform.position;
		float num = 0f - Mathf.Atan2(vector.z, vector.x);
		m_ActionDelta.x = 0f - Mathf.Cos(num);
		m_ActionDelta.y = 0f;
		m_ActionDelta.z = Mathf.Sin(num);
		m_Farmer.m_FinalRotation = Quaternion.Euler(0f, num * 57.29578f - 90f, 0f);
	}

	protected void StandInTargetTile()
	{
		m_Farmer.SetTilePosition(m_Farmer.m_GoToTilePosition);
		m_Farmer.m_FinalPosition = m_Farmer.m_TileCoord.ToWorldPositionTileCentered();
		EndLocationRotation();
	}

	protected void StandInOldTile()
	{
		m_Farmer.SetTilePosition(m_Farmer.m_TileCoord);
		m_Farmer.m_FinalPosition = m_Farmer.m_TileCoord.ToWorldPositionTileCentered();
		EndLocationRotation();
	}

	protected void EndLocationRotation()
	{
		m_Farmer.transform.position = m_Farmer.m_FinalPosition;
		m_Farmer.transform.rotation = m_Farmer.m_FinalRotation;
	}

	protected void ResetRotation()
	{
		m_Farmer.transform.rotation = m_Farmer.m_FinalRotation;
	}

	protected void GetCurrentRotation()
	{
		m_Farmer.m_FinalRotation = m_Farmer.transform.rotation;
	}

	public virtual void SpawnEnd(BaseClass NewObject)
	{
	}

	public void SpawnAbort(BaseClass NewObject)
	{
		m_Farmer.SetState(Farmer.State.None, Abort: true);
		Actionable targetObject = GetTargetObject();
		if ((bool)targetObject)
		{
			targetObject.AbortAction(m_ActionInfo);
		}
	}

	public virtual void EndState()
	{
		m_Farmer.StartAnimation("FarmerIdle");
		EndAnimationBlur();
	}

	public virtual void AbortAction()
	{
		m_Farmer.SetState(Farmer.State.None);
		Actionable targetObject = GetTargetObject();
		if ((bool)targetObject)
		{
			targetObject.AbortAction(m_ActionInfo);
		}
	}

	public void DoEndAction()
	{
		m_Farmer.SetState(Farmer.State.None);
		Actionable targetObject = GetTargetObject();
		if ((bool)targetObject)
		{
			targetObject.EndAction(m_ActionInfo);
		}
	}

	public virtual void UpdateState()
	{
		if (m_SafetyDelay != 0f)
		{
			UpdateSafety();
		}
	}

	public Actionable GetTargetObject()
	{
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(m_TargetObjectUID, ErrorCheck: false);
		if (objectFromUniqueID == null)
		{
			return null;
		}
		Actionable component = objectFromUniqueID.GetComponent<Actionable>();
		if (component == null || ((bool)component.GetComponent<Holdable>() && component.GetComponent<Holdable>().m_BeingHeld))
		{
			return null;
		}
		return component;
	}

	public Holdable GetHeldObject()
	{
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(m_HeldObjectUID, ErrorCheck: false);
		if (objectFromUniqueID == null)
		{
			return null;
		}
		return objectFromUniqueID.GetComponent<Holdable>();
	}

	protected void DegradeTool()
	{
		Holdable topObject = m_Farmer.m_FarmerCarry.GetTopObject();
		if ((bool)topObject)
		{
			topObject.Use();
			if ((bool)topObject.GetActionInfo(new GetActionInfo(GetAction.IsUsed)))
			{
				m_Farmer.DestroyTopObject();
			}
			HudManager.Instance.FarmerToolUsed(m_Farmer);
		}
	}

	public void BeginAnimationBlur()
	{
		string n = "Model/ToolCarryPoint";
		if ((bool)m_Farmer.GetComponent<Worker>())
		{
			n = "Model/Frame/ToolCarryPoint";
		}
		m_TrailMaker = null;
		if ((bool)m_Farmer.m_FarmerCarry.GetTopObject())
		{
			m_TrailMaker = TrailManager.Instance.StartTrail(m_Farmer.transform.Find(n), new Vector3(0f, 0f, 0f), new Vector3(0f, m_ToolHeight * 1f, 0f));
		}
	}

	public void EndAnimationBlur()
	{
		if ((bool)m_TrailMaker)
		{
			TrailManager.Instance.StopTrail(m_TrailMaker);
			m_TrailMaker = null;
		}
	}

	public virtual void DoAnimationFunction()
	{
	}

	public virtual void DoAnimationAction()
	{
		if ((bool)m_TrailMaker)
		{
			EndAnimationBlur();
		}
	}

	protected void UpdateSafety()
	{
		m_SafetyTimer += TimeManager.Instance.m_NormalDelta;
		if (m_SafetyTimer > m_SafetyDelay)
		{
			if (m_Farmer.m_TypeIdentifier == ObjectType.Worker)
			{
				Debug.Log("*** Farmer State Safety triggered : " + m_Farmer.GetComponent<Worker>().m_WorkerName + " " + m_State);
			}
			else
			{
				Debug.Log("*** Farmer State Safety triggered : Player " + m_State);
			}
			DoAnimationAction();
		}
	}

	public virtual void StartWorking()
	{
	}

	public virtual void StopWorking()
	{
	}
}
