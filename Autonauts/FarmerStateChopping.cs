using UnityEngine;

public class FarmerStateChopping : FarmerStateTool
{
	public FarmerStateChopping()
	{
		m_ActionSoundName = "ToolAxeChop";
		m_NoToolIconName = "GenIcons/GenIconToolAxeMetal";
		m_AdjacentTile = true;
	}

	public static bool GetIsToolAcceptable(ObjectType NewType)
	{
		return ToolAxe.GetIsTypeAxe(NewType);
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
		FaceTowardsTargetTile();
		Actionable targetObject = GetTargetObject();
		if ((bool)targetObject && (bool)targetObject.GetComponent<MyTree>())
		{
			m_Farmer.StartAnimation("FarmerChopping");
		}
		else
		{
			m_Farmer.StartAnimation("FarmerChoppingVertical");
		}
	}

	public override void EndState()
	{
		base.EndState();
		StandInOldTile();
	}

	protected override void SendEvents(Actionable TargetObject)
	{
		if ((bool)TargetObject.GetComponent<MyTree>())
		{
			if ((bool)m_Tool)
			{
				if (m_Tool.m_TypeIdentifier == ObjectType.Rock)
				{
					QuestManager.Instance.AddEvent(QuestEvent.Type.ChopTreeWithRock, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
				}
				else if (m_Tool.m_TypeIdentifier == ObjectType.ToolAxeStone)
				{
					QuestManager.Instance.AddEvent(QuestEvent.Type.ChopTreeWithAxeCrude, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
				}
				else
				{
					QuestManager.Instance.AddEvent(QuestEvent.Type.ChopTreeWithWoodAxe, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
				}
				QuestManager.Instance.AddEvent(QuestEvent.Type.ChopTree, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
				if (m_Farmer.m_TypeIdentifier == ObjectType.FarmerPlayer && m_Farmer.GetComponent<FarmerPlayer>().m_Teaching)
				{
					QuestManager.Instance.AddEvent(QuestEvent.Type.TeachChopTree, Bot: false, 0, m_Farmer);
				}
			}
			BadgeManager.Instance.AddEvent(BadgeEvent.Type.TreesCut);
		}
		if (TargetObject.m_TypeIdentifier == ObjectType.Log)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.ChopLog, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
			QuestManager.Instance.AddEvent(QuestEvent.Type.ProcessWood, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
		}
		if (TargetObject.m_TypeIdentifier == ObjectType.Plank)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.ChopPlank, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
			QuestManager.Instance.AddEvent(QuestEvent.Type.ProcessWood, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
		}
		if (TargetObject.m_TypeIdentifier == ObjectType.Pole)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.ChopPole, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
			QuestManager.Instance.AddEvent(QuestEvent.Type.ProcessWood, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
		}
		QuestManager.Instance.AddEvent(QuestEvent.Type.Chop, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
		QuestManager.Instance.AddEvent(QuestEvent.Type.UseObject, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
	}

	public override void DoAnimationAction()
	{
		base.DoAnimationAction();
		Actionable targetObject = GetTargetObject();
		if (targetObject != null)
		{
			if (MyTree.GetIsTree(targetObject.m_TypeIdentifier))
			{
				float percent = (float)m_ActionCount / (float)m_NumActionCounts;
				targetObject.GetComponent<MyTree>().UpdateChops(percent);
			}
			else if (targetObject.m_TypeIdentifier == ObjectType.Log || targetObject.m_TypeIdentifier == ObjectType.Plank || targetObject.m_TypeIdentifier == ObjectType.Pole)
			{
				Vector3 localPosition = targetObject.transform.position + new Vector3(0f, 1f, 0f);
				MyParticles newParticles = ParticlesManager.Instance.CreateParticles("ChopChips", localPosition, Quaternion.Euler(-90f, 0f, 0f));
				ParticlesManager.Instance.DestroyParticles(newParticles, WaitUntilNoParticles: true);
			}
		}
	}
}
