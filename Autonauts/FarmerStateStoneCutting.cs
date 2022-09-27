using UnityEngine;

public class FarmerStateStoneCutting : FarmerStateTool
{
	private TileCoord m_TileCoord;

	public FarmerStateStoneCutting()
	{
		m_ActionSoundName = "ToolChiselHit";
		m_NoToolIconName = "GenIcons/GenIconToolChiselCrude";
	}

	public override bool GetIsAdjacentTile(TileCoord TargetTile, BaseClass Object)
	{
		if ((bool)Object.GetComponent<Plot>())
		{
			return true;
		}
		return false;
	}

	public static bool GetIsToolAcceptable(ObjectType NewType)
	{
		return ToolChisel.GetIsTypeChisel(NewType);
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
		Actionable currentObject = m_Farmer.m_FarmerAction.m_CurrentObject;
		if ((bool)currentObject && (bool)currentObject.GetComponent<Plot>())
		{
			m_TileCoord = m_Farmer.m_TileCoord;
			m_FinalPosition = m_Farmer.transform.position;
			GetFinalRotationTile();
			StandInTargetTile();
		}
		else
		{
			m_Farmer.m_GoToTilePosition = m_Farmer.m_TileCoord;
			FaceTowardsTarget();
			m_Farmer.transform.position = m_Farmer.m_FinalPosition - m_ActionDelta * Tile.m_Size * 1.75f;
		}
		m_Farmer.StartAnimation("FarmerStoneCutting");
	}

	public override void EndState()
	{
		base.EndState();
		Actionable currentObject = m_Farmer.m_FarmerAction.m_CurrentObject;
		if ((bool)currentObject && (bool)currentObject.GetComponent<Plot>())
		{
			m_Farmer.SetTilePosition(m_TileCoord);
			m_Farmer.m_FinalPosition = m_FinalPosition;
			StandInOldTile();
		}
		else
		{
			StandInTargetTile();
		}
	}

	protected override void SendEvents(Actionable TargetObject)
	{
		if (m_Tool.m_TypeIdentifier == ObjectType.ToolChiselCrude)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.ChiselWithChiselCrude, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
		}
		if ((bool)TargetObject && TargetObject.m_TypeIdentifier == ObjectType.TallBoulder)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.MineTallBoulder, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, null);
		}
	}

	public override void DoAnimationAction()
	{
		base.DoAnimationAction();
		Actionable targetObject = GetTargetObject();
		if (targetObject != null)
		{
			Vector3 position = m_Farmer.m_FinalPosition;
			if ((bool)targetObject && targetObject.GetComponent<Plot>() == null)
			{
				position = targetObject.transform.position;
			}
			position += m_ActionDelta * Tile.m_Size * 0.75f;
			m_Farmer.CreateParticles(position, "Pickaxe");
		}
	}
}
