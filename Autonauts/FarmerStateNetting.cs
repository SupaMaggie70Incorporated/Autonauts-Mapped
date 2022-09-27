using UnityEngine;

public class FarmerStateNetting : FarmerStateTool
{
	public FarmerStateNetting()
	{
		m_ActionSoundName = "ToolPaddleSplash";
		m_NoToolIconName = "GenIcons/GenIconToolNetCrude";
		m_AdjacentTile = true;
	}

	public static bool GetIsToolAcceptable(ObjectType NewType)
	{
		return ToolNet.GetIsTypeNet(NewType);
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
		m_Farmer.StartAnimation("FarmerNettingSwamp");
	}

	public override void EndState()
	{
		base.EndState();
		StandInOldTile();
	}

	protected override void SendEvents(Actionable TargetObject)
	{
		switch (TileManager.Instance.GetTileType(m_Farmer.m_GoToTilePosition))
		{
		case Tile.TileType.Swamp:
			QuestManager.Instance.AddEvent(QuestEvent.Type.LeechCaught, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
			break;
		case Tile.TileType.WaterShallow:
		case Tile.TileType.SeaWaterShallow:
			QuestManager.Instance.AddEvent(QuestEvent.Type.CatchBait, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
			break;
		}
	}

	public override void DoAnimationAction()
	{
		base.DoAnimationAction();
		m_Farmer.CreateParticles(m_Farmer.m_GoToTilePosition.ToWorldPositionTileCentered(), "Splash");
	}

	private void CreateObject()
	{
		ObjectType objectType = ObjectTypeList.m_Total;
		switch (TileManager.Instance.GetTileType(m_Farmer.m_GoToTilePosition))
		{
		case Tile.TileType.Swamp:
			objectType = ObjectType.AnimalLeech;
			break;
		case Tile.TileType.WaterShallow:
		case Tile.TileType.SeaWaterShallow:
			objectType = ObjectType.FishBait;
			break;
		}
		if (objectType != ObjectTypeList.m_Total)
		{
			TileCoord randomEmptyTile = TileHelpers.GetRandomEmptyTile(m_Farmer.m_GoToTilePosition, CheckFloor: false, CheckAssociatedObject: true);
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(objectType, randomEmptyTile.ToWorldPositionTileCentered(), Quaternion.identity);
			float y = Random.Range(0, 360);
			baseClass.transform.localRotation = Quaternion.Euler(0f, y, 0f);
			SpawnAnimationManager.Instance.AddJump(baseClass, m_Farmer.m_GoToTilePosition, randomEmptyTile, 0f, randomEmptyTile.GetHeightOffGround(), 5f, 0.5f);
		}
	}

	protected override void ActionSuccess(Actionable TargetObject)
	{
		base.ActionSuccess(TargetObject);
		CreateObject();
	}
}
