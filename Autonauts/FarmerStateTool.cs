public class FarmerStateTool : FarmerStateBase
{
	protected float m_OldActionPercent;

	protected Holdable m_Tool;

	protected int m_ActionCount;

	protected int m_NumActionCounts;

	private float m_EndStateTimer;

	protected string m_ActionSoundName;

	protected string m_NoToolIconName;

	protected bool m_AdjacentTile;

	protected bool m_UseTarget;

	public override string GetNoToolIconName()
	{
		return m_NoToolIconName;
	}

	public override bool GetIsAdjacentTile(TileCoord TargetTile, BaseClass Object)
	{
		return m_AdjacentTile;
	}

	public override void StartState()
	{
		base.StartState();
		m_UseTarget = true;
		m_OldActionPercent = 0f;
		m_Tool = m_Farmer.m_FarmerCarry.GetTopObject();
		Actionable currentObject = m_Farmer.m_FarmerAction.m_CurrentObject;
		m_ActionCount = 0;
		if (currentObject.m_TypeIdentifier == ObjectType.Plot)
		{
			TileCoord goToTilePosition = m_Farmer.m_GoToTilePosition;
			Tile.TileType tileType = TileManager.Instance.GetTileType(goToTilePosition);
			m_NumActionCounts = VariableManager.Instance.GetVariableAsInt(m_State, tileType, m_Farmer.m_FarmerCarry.GetTopObjectType(), CheckValid: false);
			if (m_NumActionCounts == 0)
			{
				m_NumActionCounts = VariableManager.Instance.GetVariableAsInt(m_State, currentObject.m_TypeIdentifier, m_Farmer.m_FarmerCarry.GetTopObjectType(), CheckValid: false);
			}
		}
		else
		{
			ObjectType objectType = currentObject.m_TypeIdentifier;
			if (Fish.GetIsTypeFish(objectType))
			{
				objectType = ObjectType.FishAny;
			}
			if (FolkHeart.GetIsFolkHeart(objectType))
			{
				objectType = ObjectType.HeartAny;
			}
			m_NumActionCounts = VariableManager.Instance.GetVariableAsInt(m_State, objectType, m_Farmer.m_FarmerCarry.GetTopObjectType(), CheckValid: false);
		}
		m_EndStateTimer = 0f;
	}

	protected virtual void SendEvents(Actionable TargetObject)
	{
	}

	public void DoAction(Actionable TargetObject)
	{
		m_ActionCount++;
		if (m_ActionSoundName != "")
		{
			AudioManager.Instance.StartEvent(m_ActionSoundName, m_Farmer);
		}
		Holdable heldObject = GetHeldObject();
		if ((bool)TargetObject)
		{
			TargetObject.UseAction(heldObject, m_Farmer, m_Farmer.m_FarmerAction.m_CurrentInfo.m_ActionType, m_Farmer.m_FarmerAction.m_CurrentInfo.m_Position);
		}
	}

	protected virtual void ActionSuccess(Actionable TargetObject)
	{
	}

	public override void DoAnimationAction()
	{
		base.DoAnimationAction();
		Actionable targetObject = GetTargetObject();
		if (targetObject == null)
		{
			DoEndAction();
		}
		else if (m_ActionCount < m_NumActionCounts)
		{
			DoAction(targetObject);
		}
	}

	public float GetActionPercentDone()
	{
		return (float)m_ActionCount / (float)m_NumActionCounts;
	}

	public void CheckActionDone(Actionable TargetObject)
	{
		if (m_ActionCount == m_NumActionCounts)
		{
			SendEvents(TargetObject);
			ActionSuccess(TargetObject);
			m_Farmer.SetBaggedObject(null);
			m_Farmer.SetBaggedTile(new TileCoord(0, 0));
			if (m_UseTarget)
			{
				DoEndAction();
			}
			else
			{
				m_Farmer.SetState(Farmer.State.None);
			}
			if (m_State != Farmer.State.Milking)
			{
				DegradeTool();
			}
		}
	}

	public override void UpdateState()
	{
		base.UpdateState();
		if (m_NumActionCounts == 0 || m_ActionCount != m_NumActionCounts)
		{
			return;
		}
		m_EndStateTimer += TimeManager.Instance.m_NormalDelta;
		if (m_EndStateTimer >= 0.2f)
		{
			Actionable targetObject = GetTargetObject();
			if (targetObject != null)
			{
				CheckActionDone(targetObject);
			}
		}
	}
}
