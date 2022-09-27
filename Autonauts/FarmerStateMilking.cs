using UnityEngine;

public class FarmerStateMilking : FarmerStateTool
{
	public AnimalCow m_Cow;

	public FarmerStateMilking()
	{
		m_ActionSoundName = "AnimalCowMilking";
		m_NoToolIconName = "GenIcons/GenIconToolBucket";
		m_AdjacentTile = true;
	}

	public override bool GetIsAdjacentTile(TileCoord TargetTile, BaseClass Object)
	{
		if (AnimalCow.GetIsTypeCow(Object.m_TypeIdentifier))
		{
			return true;
		}
		return false;
	}

	public static bool GetIsToolAcceptable(ObjectType NewType)
	{
		return ToolBucket.GetIsTypeBucket(NewType);
	}

	public override bool IsToolAcceptable(Holdable NewObject)
	{
		if (NewObject == null)
		{
			return false;
		}
		return GetIsToolAcceptable(NewObject.m_TypeIdentifier);
	}

	public override void StartState()
	{
		base.StartState();
		BaseClass targetObject = GetTargetObject();
		if ((bool)targetObject && AnimalCow.GetIsTypeCow(targetObject.m_TypeIdentifier))
		{
			FaceTowardsTarget();
		}
		else
		{
			m_Farmer.m_FinalPosition = m_Farmer.transform.position;
		}
		m_Farmer.m_FarmerCarry.m_ToolModel.transform.localPosition = new Vector3(1f, -0.5f, -1.5f);
	}

	public override void EndState()
	{
		base.EndState();
		ResetRotation();
		if ((bool)m_Farmer.m_FarmerCarry.m_ToolModel)
		{
			m_Farmer.m_FarmerCarry.m_ToolModel.transform.localPosition = new Vector3(0f, 0f, 0f);
		}
	}

	protected override void ActionSuccess(Actionable TargetObject)
	{
		ToolBucket component = m_Tool.GetComponent<ToolBucket>();
		int amount = 1;
		BaseClass targetObject = GetTargetObject();
		if ((bool)targetObject && !AnimalCow.GetIsTypeCow(targetObject.m_TypeIdentifier))
		{
			amount = 3;
		}
		component.Fill(ObjectType.Milk, amount);
		QuestManager.Instance.AddEvent(QuestEvent.Type.FillBucket, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
	}

	protected override void SendEvents(Actionable TargetObject)
	{
		if (AnimalCow.GetIsTypeCow(TargetObject.m_TypeIdentifier))
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.MilkCow, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
			BadgeManager.Instance.AddEvent(BadgeEvent.Type.Milk);
		}
	}

	public override void UpdateState()
	{
		base.UpdateState();
		Actionable targetObject = GetTargetObject();
		if (targetObject == null)
		{
			DoEndAction();
			return;
		}
		float num = 0f;
		if ((int)(m_Farmer.m_StateTimer * 60f) % 12 < 6)
		{
			num = 1f;
		}
		m_Farmer.transform.position = m_Farmer.m_FinalPosition + new Vector3(0f, num * 1f, 0f);
		if (num != m_OldActionPercent)
		{
			m_OldActionPercent = num;
			if (num == 1f)
			{
				DoAction(targetObject);
			}
			else
			{
				CheckActionDone(targetObject);
			}
		}
	}
}
