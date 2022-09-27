using UnityEngine;

public class FarmerStateShearing : FarmerStateTool
{
	public AnimalSheep m_Sheep;

	public FarmerStateShearing()
	{
		m_ActionSoundName = "AnimalSheepShearing";
		m_NoToolIconName = "GenIcons/GenIconToolShears";
		m_AdjacentTile = true;
	}

	public static bool GetIsToolAcceptable(ObjectType NewType)
	{
		return NewType == ObjectType.ToolShears;
	}

	public override bool GetIsAdjacentTile(TileCoord TargetTile, BaseClass Object)
	{
		if (AnimalSheep.GetIsTypeSheep(Object.m_TypeIdentifier))
		{
			return true;
		}
		if (Object.m_TypeIdentifier == ObjectType.Bush)
		{
			return true;
		}
		return false;
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
		if ((bool)targetObject && AnimalSheep.GetIsTypeSheep(targetObject.m_TypeIdentifier))
		{
			FaceTowardsTarget();
		}
		else
		{
			m_Farmer.m_FinalPosition = m_Farmer.transform.position;
		}
		m_Farmer.StartAnimation("FarmerShearing");
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

	protected override void SendEvents(Actionable TargetObject)
	{
		if (TargetObject.m_TypeIdentifier == ObjectType.ShearingShedCrude || TargetObject.m_TypeIdentifier == ObjectType.AnimalSheep)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.ShearSheep, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
			BadgeManager.Instance.AddEvent(BadgeEvent.Type.Wool);
		}
		if (TargetObject.m_TypeIdentifier == ObjectType.ShearingShedCrude)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.ShearSheepInShearingShed, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
		}
	}

	public override void UpdateState()
	{
		base.UpdateState();
		if (GetTargetObject() == null)
		{
			DoEndAction();
		}
	}
}
