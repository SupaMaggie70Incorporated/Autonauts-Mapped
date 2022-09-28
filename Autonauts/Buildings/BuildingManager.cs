using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
	public static BuildingManager Instance;

	private void Awake()
	{
		Instance = this;
	}

	public void RemoveBuilding(Building NewBuilding)
	{
		MapManager.Instance.RemoveBuilding(NewBuilding);
		RefreshBuilding(NewBuilding);
	}

	private void PatchUpWorkers(Building NewBuilding, TileCoord AccessPosition)
	{
		Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("Worker");
		if (collection != null)
		{
			foreach (KeyValuePair<BaseClass, int> item in collection)
			{
				foreach (HighInstruction item2 in item.Key.GetComponent<Worker>().m_WorkerInterpreter.m_HighInstructions.m_List)
				{
					bool found = false;
					item2.ChangeUIDLocation(NewBuilding.m_UniqueID, AccessPosition, found);
				}
				item.Key.GetComponent<Worker>().m_WorkerInterpreter.UpdateCurrentScript();
			}
		}
		WorkerScriptManager.Instance.ChangeBuildingLocation(NewBuilding.m_UniqueID, AccessPosition);
	}

	public void AddBuilding(Building NewBuilding, TileCoord NewPosition, int NewRotation)
	{
		NewBuilding.SetTilePosition(NewPosition);
		Vector3 position = NewPosition.ToWorldPositionTileCentered();
		if (Bridge.GetIsTypeBridge(NewBuilding.m_TypeIdentifier) || NewBuilding.m_TypeIdentifier == ObjectType.TrainTrackBridge)
		{
			position.y = 0f;
		}
		NewBuilding.SetPosition(position);
		NewBuilding.SetRotation(NewRotation);
		MapManager.Instance.AddBuilding(NewBuilding);
		RefreshBuilding(NewBuilding);
		TileCoord accessPosition = NewBuilding.GetAccessPosition();
		PatchUpWorkers(NewBuilding, accessPosition);
		if ((bool)GameStateManager.Instance.GetCurrentState().GetComponent<GameStateTeachWorker2>())
		{
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateTeachWorker2>().m_ScriptEdit.UpdateBuildingMoved(NewBuilding.m_UniqueID, NewPosition);
		}
		if ((bool)NewBuilding.GetComponent<Housing>())
		{
			NewBuilding.GetComponent<Housing>().Moved();
		}
	}

	public void MoveBuildings(List<MovingBuilding> MovingList)
	{
		foreach (MovingBuilding Moving in MovingList)
		{
			Moving.GetOutputObjects();
			Moving.RemoveOutputObjects();
			Moving.m_Building.RemoveAllBuildings();
		}
		for (int num = MovingList.Count - 1; num >= 0; num--)
		{
			Building building = MovingList[num].m_Building;
			MapManager.Instance.RemoveBuilding(building);
			RefreshBuilding(building);
		}
		foreach (MovingBuilding Moving2 in MovingList)
		{
			Building building2 = Moving2.m_Building;
			building2.SetTilePosition(Moving2.m_NewPosition);
			Vector3 position = Moving2.m_NewPosition.ToWorldPositionTileCentered();
			if (Bridge.GetIsTypeBridge(building2.m_TypeIdentifier) || building2.m_TypeIdentifier == ObjectType.TrainTrackBridge)
			{
				position.y = 0f;
			}
			building2.SetPosition(position);
			building2.SetRotation(Moving2.m_NewRotation);
			MapManager.Instance.AddBuilding(building2);
			RefreshBuilding(building2);
			TileCoord accessPosition = building2.GetAccessPosition();
			PatchUpWorkers(building2, accessPosition);
			if ((bool)GameStateManager.Instance && (bool)GameStateManager.Instance.GetCurrentState().GetComponent<GameStateTeachWorker2>())
			{
				GameStateManager.Instance.GetCurrentState().GetComponent<GameStateTeachWorker2>().m_ScriptEdit.UpdateBuildingMoved(building2.m_UniqueID, accessPosition);
			}
			building2.Moved();
		}
		foreach (MovingBuilding Moving3 in MovingList)
		{
			Moving3.MoveOutputObjects();
		}
		foreach (MovingBuilding Moving4 in MovingList)
		{
			if (Moving4.GetType().GetProperty("m_ParentBuilding") != null)
			{
				if (Moving4.m_Building.m_ParentBuilding.GetIsSavable())
				{
					ModManager.Instance.CheckBuildingEditedCallback(Moving4.m_Building.m_ParentBuilding, "Move");
				}
			}
			else if (Moving4.m_Building.GetIsSavable())
			{
				ModManager.Instance.CheckBuildingEditedCallback(Moving4.m_Building, "Move");
			}
		}
	}

	public void MoveBuilding(Building NewBuilding, TileCoord NewPosition, int NewRotation)
	{
		MovingBuilding item = new MovingBuilding(0, NewBuilding, NewPosition, NewRotation);
		List<MovingBuilding> list = new List<MovingBuilding>();
		list.Add(item);
		MoveBuildings(list);
	}

	private void Update()
	{
	}

	public void RefreshBuilding(Building NewBuilding)
	{
		NewBuilding.SendAction(new ActionInfo(ActionType.RefreshFirst, default(TileCoord)));
		if ((bool)NewBuilding.GetComponent<Wall>() || Door.GetIsTypeDoor(NewBuilding.m_TypeIdentifier) || TrainTrack.GetIsTypeTrainTrack(NewBuilding.m_TypeIdentifier))
		{
			RefreshSurroundingBuildings(NewBuilding);
		}
	}

	public void RefreshBuilding(TileCoord Position)
	{
		Tile tile = TileManager.Instance.GetTile(Position);
		if ((bool)tile.m_Building)
		{
			tile.m_Building.SendAction(new ActionInfo(ActionType.Refresh, default(TileCoord)));
		}
		if ((bool)tile.m_Floor)
		{
			tile.m_Floor.SendAction(new ActionInfo(ActionType.Refresh, default(TileCoord)));
		}
	}

	public void RefreshSurroundingBuildings(Building NewBuilding)
	{
		TileCoord tileCoord = NewBuilding.m_TileCoord;
		if (tileCoord.y > 0)
		{
			RefreshBuilding(tileCoord + new TileCoord(0, -1));
		}
		if (tileCoord.y < TileManager.Instance.m_TilesHigh - 1)
		{
			RefreshBuilding(tileCoord + new TileCoord(0, 1));
		}
		if (tileCoord.x > 0)
		{
			RefreshBuilding(tileCoord + new TileCoord(-1, 0));
		}
		if (tileCoord.x < TileManager.Instance.m_TilesWide - 1)
		{
			RefreshBuilding(tileCoord + new TileCoord(1, 0));
		}
		if (!NewBuilding.GetComponent<Floor>())
		{
			return;
		}
		if (tileCoord.x > 0)
		{
			if (tileCoord.y > 0)
			{
				RefreshBuilding(tileCoord + new TileCoord(-1, -1));
			}
			if (tileCoord.y < TileManager.Instance.m_TilesHigh - 1)
			{
				RefreshBuilding(tileCoord + new TileCoord(-1, 1));
			}
		}
		if (tileCoord.x < TileManager.Instance.m_TilesWide - 1)
		{
			if (tileCoord.y > 0)
			{
				RefreshBuilding(tileCoord + new TileCoord(1, -1));
			}
			if (tileCoord.y < TileManager.Instance.m_TilesHigh - 1)
			{
				RefreshBuilding(tileCoord + new TileCoord(1, 1));
			}
		}
	}

	public bool GetIsInstantBuild()
	{
		if (GameOptionsManager.Instance.m_Options.m_GameMode != GameOptions.GameMode.ModeCreative && !CheatManager.Instance.m_InstantBuild)
		{
			return false;
		}
		return true;
	}

	public bool DestroyBuilding(Actionable TargetObject)
	{
		Building component = TargetObject.GetComponent<Building>();
		component.SetHighlight(Highlighted: false);
		if (component.m_Levels != null)
		{
			foreach (Building level in component.m_Levels)
			{
				level.RemoveAllBuildings();
			}
		}
		MapManager.Instance.RemoveBuilding(component);
		RefreshBuilding(component);
		if (!GetIsInstantBuild() && component.m_TypeIdentifier != ObjectType.ConverterFoundation)
		{
			ResourceManager.Instance.AddResource(component.m_TypeIdentifier, 1);
		}
		component.gameObject.SetActive(value: false);
		component.StopUsing(AndDestroy: false);
		ModManager.Instance.CheckBuildingEditedCallback(component, "Destroy");
		return true;
	}

	public Building AddBuilding(TileCoord TilePosition, ObjectType NewType, int Rotation, Building OriginalBuilding = null, bool Instant = false, bool ForceBlueprint = false, bool IncludeUpgrades = true)
	{
		Vector3 position = TilePosition.ToWorldPositionTileCentered();
		if (Bridge.GetIsTypeBridge(NewType) || NewType == ObjectType.TrainTrackBridge)
		{
			position.y = 0f;
		}
		Tile tile = TileManager.Instance.GetTile(TilePosition);
		if ((bool)tile.m_Building && !Floor.GetIsTypeFloor(NewType))
		{
			position.y += tile.m_Building.GetBuildingHeightOffset();
		}
		Building building = null;
		ObjectType objectType = ObjectTypeList.m_Total;
		if ((!GetIsInstantBuild() && !Instant) || ForceBlueprint)
		{
			int resource = ResourceManager.Instance.GetResource(NewType, IncludeUpgrades);
			Building building2 = tile.m_Building;
			if (resource == 0 || (building2 != null && building2.GetTopBuilding().m_TypeIdentifier == ObjectType.ConverterFoundation) || ForceBlueprint)
			{
				ConverterFoundation component = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.ConverterFoundation, position, Quaternion.identity).GetComponent<ConverterFoundation>();
				component.SetNewBuilding(NewType);
				building = component;
				building.SetRotation(Rotation);
			}
			else
			{
				objectType = ResourceManager.Instance.ReleaseResource(NewType, IncludeUpgrades);
				building = ObjectTypeList.Instance.CreateObjectFromIdentifier(objectType, position, Quaternion.identity).GetComponent<Building>();
				building.SetRotation(Rotation);
			}
		}
		else
		{
			building = ObjectTypeList.Instance.CreateObjectFromIdentifier(NewType, position, Quaternion.identity).GetComponent<Building>();
			building.SetRotation(Rotation);
			if (NewType == ObjectType.ResearchStationCrude && CheatManager.Instance.m_InstantBuild)
			{
				QuestManager.Instance.AddEvent(QuestEvent.Type.Build, Bot: false, ObjectType.ResearchStationCrude, null);
			}
		}
		foreach (TileCoord tile2 in building.m_Tiles)
		{
			if (!tile2.GetIsValid())
			{
				building.StopUsing();
				if (objectType != ObjectTypeList.m_Total)
				{
					ResourceManager.Instance.AddResource(objectType, 1);
				}
				return null;
			}
		}
		MapManager.Instance.AddBuilding(building);
		if ((bool)OriginalBuilding)
		{
			building.CopyFrom(OriginalBuilding);
		}
		if ((bool)building)
		{
			RefreshBuilding(building);
		}
		if (!SaveLoadManager.Instance.m_Loading)
		{
			ModManager.Instance.CheckBuildingTypeSpawnedCallback(building);
		}
		return building;
	}
}
