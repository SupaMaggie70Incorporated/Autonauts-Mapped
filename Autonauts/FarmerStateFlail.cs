using UnityEngine;

public class FarmerStateFlail : FarmerStateTool
{
	private float m_OldFlailPercent;

	private int m_FlailObjectUID;

	private float m_FlailDelay;

	private int m_FlailCount;

	private bool m_AudioPlay;

	private int m_FailCountTarget;

	public FarmerStateFlail()
	{
		m_ActionSoundName = "ToolFlailUse";
		m_NoToolIconName = "GenIcons/GenIconToolFlail";
		m_AdjacentTile = true;
	}

	public static bool GetIsToolAcceptable(ObjectType NewType)
	{
		if (!ToolFlail.GetIsTypeFlail(NewType))
		{
			return NewType == ObjectType.Stick;
		}
		return true;
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
		if ((bool)m_Farmer.m_FarmerCarry.m_ToolModel && (bool)m_Farmer.m_FarmerCarry.m_ToolModel.GetComponent<ToolFlail>())
		{
			m_Farmer.m_FarmerCarry.m_ToolModel.GetComponent<ToolFlail>().m_Hinge.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
		}
		base.StartState();
		Actionable targetObject = GetTargetObject();
		if ((bool)targetObject)
		{
			m_Farmer.m_GoToTilePosition = targetObject.GetComponent<TileCoordObject>().m_TileCoord;
		}
		FaceTowardsTargetTile();
		m_Farmer.StartAnimation("FarmerFlail");
	}

	public override void EndState()
	{
		base.EndState();
		if (m_TargetObjectType == ObjectType.Bush || m_TargetObjectType == ObjectType.Pumpkin)
		{
			EndLocationRotation();
		}
		else
		{
			StandInTargetTile();
		}
		if ((bool)m_Farmer.m_FarmerCarry.m_ToolModel && (bool)m_Farmer.m_FarmerCarry.m_ToolModel.GetComponent<ToolFlail>())
		{
			m_Farmer.m_FarmerCarry.m_ToolModel.GetComponent<ToolFlail>().m_Hinge.transform.localRotation = Quaternion.Euler(-60f, 90f, 90f);
		}
		QuestManager.Instance.AddEvent(QuestEvent.Type.UseObject, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
	}

	protected override void SendEvents(Actionable TargetObject)
	{
		BaseClass topObject = m_Farmer.m_FarmerCarry.GetTopObject();
		if ((bool)TargetObject && TargetObject.m_TypeIdentifier == ObjectType.Wheat)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.FlailWheat, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
			if ((bool)topObject)
			{
				if (topObject.m_TypeIdentifier == ObjectType.Stick)
				{
					QuestManager.Instance.AddEvent(QuestEvent.Type.ThreshWheatWithStick, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
				}
				if (topObject.m_TypeIdentifier == ObjectType.ToolFlailCrude)
				{
					QuestManager.Instance.AddEvent(QuestEvent.Type.UseFlailCrude, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
				}
				if (topObject.m_TypeIdentifier == ObjectType.ToolFlail)
				{
					QuestManager.Instance.AddEvent(QuestEvent.Type.UseFlail, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
				}
			}
			QuestManager.Instance.AddEvent(QuestEvent.Type.ThreshWheat, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
		}
		if ((bool)TargetObject && TargetObject.m_TypeIdentifier == ObjectType.Bush)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.BashBush, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
		}
	}
}
