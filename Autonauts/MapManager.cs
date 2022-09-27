using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
	public static MapManager Instance;

	[HideInInspector]
	public Transform m_RootObject;

	[HideInInspector]
	public Transform m_ObjectsRootTransform;

	[HideInInspector]
	public Transform m_PlotRootTransform;

	[HideInInspector]
	public Transform m_MiscRootTransform;

	[HideInInspector]
	public Transform m_ParticlesRootTransform;

	[HideInInspector]
	public Transform m_AnimalsRootTransform;

	[HideInInspector]
	public Transform m_SticksRocksRootTransform;

	[HideInInspector]
	public Transform m_CropsRootTransform;

	[HideInInspector]
	public Transform m_FolksRootTransform;

	[HideInInspector]
	public Transform m_UnusedRootTransform;

	[HideInInspector]
	public Transform m_BuildingsRootTransform;

	[HideInInspector]
	public Transform m_TreesRootTransform;

	[HideInInspector]
	public Transform m_OriginalModelsRootTransform;

	private void Awake()
	{
		Instance = this;
		m_RootObject = GameObject.Find("Generated").transform;
		m_ObjectsRootTransform = m_RootObject.Find("Objects");
		m_PlotRootTransform = m_RootObject.Find("Plots");
		m_MiscRootTransform = m_RootObject.Find("Misc");
		m_ParticlesRootTransform = m_RootObject.Find("Particles");
		m_AnimalsRootTransform = m_RootObject.Find("Animals");
		m_TreesRootTransform = m_RootObject.Find("Trees");
		m_SticksRocksRootTransform = m_RootObject.Find("SticksRocks");
		m_CropsRootTransform = m_RootObject.Find("Crops");
		m_FolksRootTransform = m_RootObject.Find("Folks");
		m_UnusedRootTransform = m_RootObject.Find("Unused");
		m_BuildingsRootTransform = m_RootObject.Find("Buildings");
		m_OriginalModelsRootTransform = m_RootObject.Find("OriginalModels");
	}

	public void CreateEmpty()
	{
		int mapWidth = GameOptionsManager.Instance.m_Options.m_MapWidth;
		int mapHeight = GameOptionsManager.Instance.m_Options.m_MapHeight;
		Tile[] array = new Tile[mapWidth * mapHeight];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = new Tile();
		}
		Load(array, mapWidth, mapHeight);
		int num = PlotManager.Instance.m_PlotsWide / 2;
		int num2 = PlotManager.Instance.m_PlotsHigh / 2;
		int num3 = num * Plot.m_PlotTilesWide;
		int num4 = num2 * Plot.m_PlotTilesHigh;
		ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.FarmerPlayer, new TileCoord(num3 + 10, num4 + 6).ToWorldPositionTileCentered(), Quaternion.identity);
	}

	public void Load(Tile[] Data, int TilesWide, int TilesHigh)
	{
		OldFileUtils.CheckUsedTiles(Data, TilesWide, TilesHigh);
		PlotManager.Instance.CreatePlots(TilesWide, TilesHigh);
		TileManager.Instance.CreateTiles(Data, TilesWide, TilesHigh);
		TileManager.Instance.UpdateShading();
		PlotManager.Instance.CreateClouds();
	}

	private void MarkBuildingOnTiles(Building NewBuilding, bool Add)
	{
		if (NewBuilding == null)
		{
			return;
		}
		Building building = NewBuilding;
		if (!Add)
		{
			building = null;
		}
		bool flag = false;
		if ((bool)NewBuilding.GetComponent<Floor>())
		{
			flag = true;
		}
		if (NewBuilding.m_TypeIdentifier == ObjectType.ConverterFoundation && (bool)NewBuilding.GetComponent<ConverterFoundation>().m_NewBuilding && Floor.GetIsTypeFloor(NewBuilding.GetComponent<ConverterFoundation>().m_NewBuilding.m_TypeIdentifier))
		{
			flag = true;
		}
		TileCoord tileCoord = new TileCoord(-1, -1);
		TileCoord tileCoord2 = new TileCoord(-1, -1);
		Building building2 = NewBuilding;
		if (NewBuilding.m_TypeIdentifier == ObjectType.ConverterFoundation)
		{
			building2 = NewBuilding.GetComponent<ConverterFoundation>().m_NewBuilding;
		}
		if (building2 == null)
		{
			return;
		}
		if (building2.m_AccessModel.activeSelf)
		{
			tileCoord = building2.GetAccessPosition();
		}
		tileCoord2 = building2.GetSpawnPoint();
		if (!flag)
		{
			TileCoord tileCoord3 = building2.m_TileCoord;
			if (tileCoord3.x < 0 || tileCoord3.y < 0 || tileCoord3.x >= TileManager.Instance.m_TilesWide || tileCoord3.y >= TileManager.Instance.m_TilesHigh)
			{
				Debug.Log("Building out of map : " + NewBuilding.m_UniqueID);
			}
			int num = tileCoord3.y * TileManager.Instance.m_TilesWide + tileCoord3.x;
			Building building3 = TileManager.Instance.m_Tiles[num].m_Building;
			if ((bool)building3 && building3 != NewBuilding && building3.m_Levels != null)
			{
				if (Add)
				{
					NewBuilding.transform.position = building3.AddBuilding(NewBuilding);
				}
				else
				{
					building3.RemoveBuilding(NewBuilding);
				}
				return;
			}
		}
		foreach (TileCoord tile in building2.m_Tiles)
		{
			if (!tile.GetIsValid())
			{
				continue;
			}
			int num2 = tile.y * TileManager.Instance.m_TilesWide + tile.x;
			if (flag)
			{
				TileManager.Instance.m_Tiles[num2].m_Floor = building;
				if ((bool)TileManager.Instance.m_Tiles[num2].m_Building)
				{
					TileManager.Instance.m_Tiles[num2].m_Building.CheckWallsFloors();
				}
			}
			else
			{
				if (tile != tileCoord && tile != tileCoord2)
				{
					TileManager.Instance.m_Tiles[num2].m_Building = building;
				}
				TileManager.Instance.m_Tiles[num2].m_BuildingFootprint = building;
			}
			TileManager.Instance.UpdateTile(tile);
			RouteFinding.UpdateTileWalk(tile.x, tile.y);
			PlotManager.Instance.GetPlotAtTile(tile).StackObjectsAtTile(tile);
		}
		if ((bool)WalledAreaManager.Instance && WalledAreaManager.Instance.GetIsBuildingWall(NewBuilding))
		{
			WalledAreaManager.Instance.WallUpdated(NewBuilding.m_TileCoord);
		}
		_ = NewBuilding.m_TypeIdentifier;
		_ = 80;
		foreach (TileCoord tile2 in building2.m_Tiles)
		{
			if (tile2.GetIsValid())
			{
				int num3 = tile2.y * TileManager.Instance.m_TilesWide + tile2.x;
				Building buildingFootprint = TileManager.Instance.m_Tiles[num3].m_BuildingFootprint;
				if ((bool)buildingFootprint)
				{
					buildingFootprint.TestBuildingHeight();
				}
			}
		}
		if (!(!SaveLoadManager.Instance.m_Loading && Add))
		{
			return;
		}
		foreach (TileCoord tile3 in building2.m_Tiles)
		{
			if (tile3.GetIsValid() && !(tile3 == tileCoord) && !(tile3 == tileCoord2) && ModManager.Instance.CheckNewBuildingInAreaCallback(NewBuilding, tile3))
			{
				break;
			}
		}
	}

	public void UpdateBuildingOnTiles(Building NewBuilding)
	{
		if (NewBuilding == null)
		{
			return;
		}
		Building building = NewBuilding;
		if (NewBuilding.m_TypeIdentifier == ObjectType.ConverterFoundation)
		{
			building = NewBuilding.GetComponent<ConverterFoundation>().m_NewBuilding;
		}
		if (building == null)
		{
			return;
		}
		foreach (TileCoord tile in building.m_Tiles)
		{
			RouteFinding.UpdateTileWalk(tile.x, tile.y);
		}
	}

	public void AddBuilding(Building NewBuilding)
	{
		MarkBuildingOnTiles(NewBuilding, Add: true);
	}

	public void RemoveBuilding(Building NewBuilding)
	{
		MarkBuildingOnTiles(NewBuilding, Add: false);
	}

	public bool CheckBuildingFloorRequirement(ObjectType NewType, bool NewTypeFloor, ObjectType FloorRequired, Tile NewTile)
	{
		if (NewTypeFloor && NewTile.m_Floor != null)
		{
			return false;
		}
		if (FloorRequired == ObjectTypeList.m_Total)
		{
			return true;
		}
		if (NewTile.m_Floor != null && NewTile.m_Floor.m_TypeIdentifier == FloorRequired)
		{
			return true;
		}
		return false;
	}

	public bool CheckBuildingOutOfMap(Building NewBuilding)
	{
		foreach (TileCoord tile in NewBuilding.m_Tiles)
		{
			if (tile.x < 0 || tile.x >= TileManager.Instance.m_TilesWide || tile.y < 0 || tile.y >= TileManager.Instance.m_TilesHigh)
			{
				return true;
			}
		}
		return false;
	}

	private bool CheckException(List<BaseClass> ExtraBadException, BaseClass TestObject)
	{
		return ExtraBadException?.Contains(TestObject) ?? false;
	}

	public bool CheckBuildingIntersection(Building NewBuilding, List<BaseClass> ModelBadException, out Selectable Object, List<BaseClass> ExtraBadException = null, bool IgnoreAccessTile = false)
	{
		Object = null;
		if (CheckBuildingOutOfMap(NewBuilding))
		{
			return true;
		}
		bool flag = false;
		if (Floor.GetIsTypeFloor(NewBuilding.m_TypeIdentifier) || (NewBuilding.m_TypeIdentifier == ObjectType.ConverterFoundation && Floor.GetIsTypeFloor(NewBuilding.GetComponent<ConverterFoundation>().m_NewBuilding.m_TypeIdentifier)))
		{
			flag = true;
		}
		ObjectType typeIdentifier = NewBuilding.m_TypeIdentifier;
		if (typeIdentifier == ObjectType.ConverterFoundation)
		{
			typeIdentifier = NewBuilding.GetComponent<ConverterFoundation>().m_NewBuilding.m_TypeIdentifier;
		}
		TileCoord accessPosition = NewBuilding.GetAccessPosition();
		TileCoord spawnPoint = NewBuilding.GetSpawnPoint();
		foreach (TileCoord tile2 in NewBuilding.m_Tiles)
		{
			if (IgnoreAccessTile && (tile2 == accessPosition || tile2 == spawnPoint))
			{
				continue;
			}
			if (tile2.x < 0 || tile2.x >= TileManager.Instance.m_TilesWide || tile2.y < 0 || tile2.y >= TileManager.Instance.m_TilesHigh)
			{
				return true;
			}
			Plot plotAtTile = PlotManager.Instance.GetPlotAtTile(tile2);
			if (!plotAtTile.m_Visible)
			{
				return true;
			}
			int num = tile2.y * TileManager.Instance.m_TilesWide + tile2.x;
			Tile tile = TileManager.Instance.m_Tiles[num];
			if ((bool)tile.m_BuildingFootprint && !ModelBadException.Contains(tile.m_BuildingFootprint))
			{
				if (flag && tile.m_Floor == null)
				{
					if (NewBuilding.m_TypeIdentifier == ObjectType.ConverterFoundation || NewBuilding.m_TypeIdentifier == ObjectType.CastleDrawbridge)
					{
						return true;
					}
					if (tile.m_BuildingFootprint.m_TypeIdentifier == ObjectType.GiantWaterWheel)
					{
						return true;
					}
					if (TrainTrack.GetIsTypeTrainTrack(NewBuilding.m_TypeIdentifier))
					{
						return true;
					}
					return false;
				}
				if (tile.m_BuildingFootprint.GetNewLevelAllowed(NewBuilding) && tile.m_BuildingFootprint.GetTopBuilding().GetNewLevelAllowed(NewBuilding))
				{
					return false;
				}
				Object = tile.m_BuildingFootprint.GetComponent<Selectable>();
				return true;
			}
			if (!flag && (bool)tile.m_Farmer)
			{
				if (tile2 != accessPosition)
				{
					Object = tile.m_Farmer;
					return true;
				}
				if (NewBuilding.m_TileCoord != spawnPoint && tile2 != spawnPoint)
				{
					Object = tile.m_Farmer;
					return true;
				}
			}
			if ((bool)tile.m_AssociatedObject && !ModelBadException.Contains(tile.m_AssociatedObject))
			{
				Object = tile.m_AssociatedObject.GetComponent<Selectable>();
				return true;
			}
			if ((!flag || !ModelBadException.Contains(tile.m_Floor)) && !CheckBuildingFloorRequirement(NewBuilding.m_TypeIdentifier, flag, NewBuilding.m_FloorRequired, tile))
			{
				if ((bool)tile.m_Floor)
				{
					Object = tile.m_Floor.GetComponent<Selectable>();
				}
				return true;
			}
			if ((bool)tile.m_Floor && (bool)tile.m_Floor.GetComponent<Floor>() && !ModelBadException.Contains(tile.m_Floor) && !tile.m_Floor.GetComponent<Floor>().CanBuildOn())
			{
				return true;
			}
			if ((bool)tile.m_Floor && tile.m_Floor.m_TypeIdentifier == ObjectType.ConverterFoundation)
			{
				return true;
			}
			if (!flag)
			{
				BaseClass selectableObjectAtTile = plotAtTile.GetSelectableObjectAtTile(tile2);
				if (selectableObjectAtTile != null && !ModelBadException.Contains(selectableObjectAtTile) && !CheckException(ExtraBadException, selectableObjectAtTile))
				{
					Object = selectableObjectAtTile.GetComponent<Selectable>();
					return true;
				}
			}
			if (Tile.m_TileInfo[(int)tile.m_TileType].m_Solid && !Bridge.GetIsTypeBridge(typeIdentifier) && typeIdentifier != ObjectType.TrainTrackBridge && (flag || tile.m_Floor == null || (!Bridge.GetIsTypeBridge(tile.m_Floor.m_TypeIdentifier) && tile.m_Floor.m_TypeIdentifier != ObjectType.TrainTrackBridge)))
			{
				return true;
			}
			if (Tile.m_TileInfo[(int)tile.m_TileType].m_CanReveal)
			{
				return true;
			}
			if (TileManager.Instance.GetTileHeightIndex(tile2) != 0)
			{
				if (typeIdentifier == ObjectType.GiantWaterWheel)
				{
					if (!TileHelpers.GetTileWaterShallow(tile.m_TileType))
					{
						return true;
					}
					if ((bool)tile.m_Floor)
					{
						return true;
					}
				}
				else if (((!Bridge.GetIsTypeBridge(typeIdentifier) && typeIdentifier != ObjectType.TrainTrackBridge) || !TileHelpers.GetTileWater(tile.m_TileType)) && (flag || tile.m_Floor == null || (!Bridge.GetIsTypeBridge(tile.m_Floor.m_TypeIdentifier) && tile.m_Floor.m_TypeIdentifier != ObjectType.TrainTrackBridge)))
				{
					return true;
				}
			}
			else if (typeIdentifier == ObjectType.GiantWaterWheel)
			{
				return true;
			}
		}
		return false;
	}

	public bool CheckBuildingDelete(Building NewBuilding, out TileCoord Position)
	{
		Position = new TileCoord(0, 0);
		foreach (TileCoord tile2 in NewBuilding.m_Tiles)
		{
			if (tile2.x < 0 || tile2.x >= TileManager.Instance.m_TilesWide || tile2.y < 0 || tile2.y >= TileManager.Instance.m_TilesHigh)
			{
				return true;
			}
			if (!PlotManager.Instance.GetPlotAtTile(tile2).m_Visible)
			{
				return true;
			}
			int num = tile2.y * TileManager.Instance.m_TilesWide + tile2.x;
			Tile tile = TileManager.Instance.m_Tiles[num];
			if ((bool)tile.m_Building || (bool)tile.m_BuildingFootprint)
			{
				Position = tile2;
				return true;
			}
		}
		return false;
	}

	public Building GetBuildingAtTile(TileCoord Position)
	{
		int num = Position.y * TileManager.Instance.m_TilesWide + Position.x;
		return TileManager.Instance.m_Tiles[num].m_Building;
	}

	public void ClearArea(TileCoord TopLeft, TileCoord BottomRight, bool Buildings = true, bool StaticObjects = true, bool HoldableObjects = true, bool Tiles = true)
	{
		List<TileCoordObject> list = new List<TileCoordObject>();
		for (int i = TopLeft.y; i <= BottomRight.y; i++)
		{
			for (int j = TopLeft.x; j <= BottomRight.x; j++)
			{
				TileCoord tileCoord = new TileCoord(j, i);
				Tile tile = TileManager.Instance.GetTile(tileCoord);
				if (Buildings)
				{
					if ((bool)tile.m_Building && IsObjectSafeToDelete(tile.m_Building))
					{
						if (tile.m_Building.m_Levels != null)
						{
							for (int num = tile.m_Building.m_Levels.Count - 1; num >= 0; num--)
							{
								Building building = tile.m_Building.m_Levels[num];
								tile.m_Building.RemoveBuilding(building);
								BuildingManager.Instance.DestroyBuilding(building);
							}
						}
						BuildingManager.Instance.DestroyBuilding(tile.m_Building);
					}
					if ((bool)tile.m_Floor && IsObjectSafeToDelete(tile.m_Floor))
					{
						BuildingManager.Instance.DestroyBuilding(tile.m_Floor);
					}
				}
				if (StaticObjects && (bool)tile.m_AssociatedObject && !ObjectTypeList.Instance.GetIsBuilding(tile.m_AssociatedObject.m_TypeIdentifier) && IsObjectSafeToDelete(tile.m_AssociatedObject))
				{
					tile.m_AssociatedObject.StopUsing();
				}
				if (HoldableObjects)
				{
					List<TileCoordObject> objectsAtTile = PlotManager.Instance.GetObjectsAtTile(tileCoord);
					if (objectsAtTile != null)
					{
						foreach (TileCoordObject item in objectsAtTile)
						{
							if (!ObjectTypeList.Instance.GetIsBuilding(item.m_TypeIdentifier))
							{
								list.Add(item);
							}
						}
						foreach (TileCoordObject item2 in list)
						{
							if (IsObjectSafeToDelete(item2))
							{
								item2.StopUsing();
							}
						}
						list.Clear();
					}
				}
				if (Tiles)
				{
					TileManager.Instance.SetTileType(tileCoord, Tile.TileType.Empty);
				}
			}
		}
	}

	public bool IsObjectSafeToDelete(BaseClass NewObject)
	{
		if (NewObject.m_TypeIdentifier == ObjectType.FarmerPlayer)
		{
			return false;
		}
		if (NewObject.m_TypeIdentifier == ObjectType.BasicWorker)
		{
			return false;
		}
		if (NewObject.m_TypeIdentifier == ObjectType.Worker)
		{
			return false;
		}
		if (NewObject.m_TypeIdentifier == ObjectType.TutorBot)
		{
			return false;
		}
		if (NewObject.m_TypeIdentifier == ObjectType.Folk)
		{
			return false;
		}
		return true;
	}
}
