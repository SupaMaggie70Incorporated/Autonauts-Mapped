using UnityEngine;

public class FarmerStateDredging : FarmerStateTool
{
	public FarmerStateDredging()
	{
		m_ActionSoundName = "ToolDredgerDig";
		m_NoToolIconName = "GenIcons/GenIconToolDredgerCrude";
		m_AdjacentTile = true;
	}

	public static bool GetIsToolAcceptable(ObjectType NewType)
	{
		return NewType == ObjectType.ToolDredgerCrude;
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
		GetFinalRotationTile();
		StandInTargetTile();
		m_Farmer.StartAnimation("FarmerDredging");
	}

	public override void EndState()
	{
		base.EndState();
		StandInTargetTile();
	}

	protected override void SendEvents(Actionable TargetObject)
	{
		QuestManager.Instance.AddEvent(QuestEvent.Type.DredgeWithCrudeDredger, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
	}

	public override void DoAnimationAction()
	{
		base.DoAnimationAction();
		m_Farmer.CreateParticles(m_Farmer.m_FinalPosition, "Dig");
		ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.DigShadow, m_Farmer.m_FinalPosition + new Vector3(0f, 0.1f, 0f), Quaternion.identity);
	}
}
