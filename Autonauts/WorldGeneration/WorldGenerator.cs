using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
	public static WorldGenerator Instance;

	public bool m_Generating;

	private int m_Width;

	private int m_Height;

	private int m_Seed;

	private bool m_CreateObjects;

	private Action m_Finished;

	private void Awake()
	{
		Instance = this;
		m_Generating = false;
	}

	public void StartNewMap(Action Finished, bool CreateObjects)
	{
		m_Finished = Finished;
		m_CreateObjects = CreateObjects;
		m_Seed = GameOptionsManager.Instance.m_Options.m_MapSeed;
		m_Width = GameOptionsManager.Instance.m_Options.m_MapWidth;
		m_Height = GameOptionsManager.Instance.m_Options.m_MapHeight;
		UnityEngine.Random.InitState(m_Seed);
		TilemapGenerator.Instance.StartNewMap(m_Seed, m_Width, m_Height);
		m_Generating = true;
	}

	public TileCoord GetPlayerStartPosition(Tile.TileType[] NewData)
	{
		TileCoord tileCoord = ((!SaveLoadManager.m_EmptyWorld && !SaveLoadManager.m_MiniMap) ? GameOptionsManager.Instance.m_Options.m_PlayerPosition : new TileCoord(m_Width / 2 + Plot.m_PlotTilesWide / 2, m_Height / 2 + Plot.m_PlotTilesHigh / 2));
		if (NewData[tileCoord.y * m_Width + tileCoord.x] != 0)
		{
			tileCoord = BestStartPosition.FindNearestEmptyTile(NewData, m_Width, m_Height, tileCoord);
		}
		return tileCoord;
	}

	private void FinishMap(Tile.TileType[] NewData)
	{
		for (int i = 1; i < m_Height - 1; i++)
		{
			for (int j = 1; j < m_Width - 1; j++)
			{
				Tile.TileType tileType = NewData[i * m_Width + j];
				if (Tile.m_TileInfo[(int)tileType].m_CanReveal && (TileHelpers.GetTileWater(NewData[i * m_Width + j + 1]) || TileHelpers.GetTileWater(NewData[i * m_Width + j - 1]) || TileHelpers.GetTileWater(NewData[(i + 1) * m_Width + j]) || TileHelpers.GetTileWater(NewData[(i - 1) * m_Width + j])))
				{
					NewData[i * m_Width + j] = Tile.TileType.Empty;
				}
				if ((tileType == Tile.TileType.WaterShallow || tileType == Tile.TileType.WaterDeep) && ((j > 0 && NewData[i * m_Width + j - 1] == Tile.TileType.Sand) || (j < m_Width - 1 && NewData[i * m_Width + j + 1] == Tile.TileType.Sand) || (i > 0 && NewData[(i - 1) * m_Width + j] == Tile.TileType.Sand) || (i < m_Height - 1 && NewData[(i + 1) * m_Width + j] == Tile.TileType.Sand)))
				{
					NewData[i * m_Width + j] = Tile.TileType.Empty;
				}
			}
		}
	}

	private void ClearLandingArea()
	{
		TileCoord playerPosition = GameOptionsManager.Instance.m_Options.m_PlayerPosition;
		playerPosition.y++;
		for (int i = -5; i <= 3; i++)
		{
			for (int j = -3; j <= 3; j++)
			{
				TileCoord position = new TileCoord(j, i) + playerPosition;
				Tile tile = TileManager.Instance.GetTile(position);
				if (tile != null && (bool)tile.m_AssociatedObject)
				{
					tile.m_AssociatedObject.StopUsing();
				}
			}
		}
	}

	public void GenerateTileDataDone()
	{
		Tile.TileType[] map = TilemapGenerator.Instance.m_Map;
		FinishMap(map);
		TileCoord playerStartPosition = GetPlayerStartPosition(map);
		TileResourceGenerator.AddResources(m_Seed, map, m_Width, m_Height, playerStartPosition);
		GameOptionsManager.Instance.m_Options.SetMapData(map, m_Width, m_Height, playerStartPosition, playerStartPosition);
	}

	public void StartCreateNew()
	{
		SaveLoadManager.Instance.m_Loading = true;
		SaveLoadManager.Instance.m_Creating = true;
		TimeManager.Instance.PauseAll();
		if (GameOptionsManager.Instance.m_Options.m_MapTileData == null || SaveLoadManager.m_MiniMap)
		{
			StartNewMap(null, CreateObjects: true);
		}
		else
		{
			CreateObjects();
		}
	}

	private void CreateObjects()
	{
		if (GameOptionsManager.Instance.m_Options.m_RandomObjectsEnabled)
		{
			int num = (int)DateTime.Now.Ticks;
			Debug.Log("Seed = " + num);
			UnityEngine.Random.InitState(num);
		}
		Tile.TileType[] mapTileData = GameOptionsManager.Instance.m_Options.m_MapTileData;
		int mapWidth = GameOptionsManager.Instance.m_Options.m_MapWidth;
		int mapHeight = GameOptionsManager.Instance.m_Options.m_MapHeight;
		Tile[] array = new Tile[mapWidth * mapHeight];
		for (int i = 0; i < mapHeight * mapWidth; i++)
		{
			array[i] = new Tile();
		}
		MapManager.Instance.Load(array, mapWidth, mapHeight);
		SaveLoadManager.Instance.m_Loading = false;
		TimeManager.Instance.UnPauseAll();
		if (SaveLoadManager.m_EmptyWorld)
		{
			TileCoord playerPosition = GameOptionsManager.Instance.m_Options.m_PlayerPosition;
			ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.FarmerPlayer, playerPosition.ToWorldPositionTileCentered(), Quaternion.identity).GetComponent<Farmer>();
			PlotManager.Instance.GetPlotAtTile(playerPosition.x, playerPosition.y).SetVisible(Visible: true);
			TileCoord tileCoord = GameOptionsManager.Instance.m_Options.m_PlayerPosition + Transmitter.m_StartingOffsetFromPlayer;
			Transmitter component = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Transmitter, tileCoord.ToWorldPositionTileCentered(), Quaternion.identity).GetComponent<Transmitter>();
			MapManager.Instance.AddBuilding(component);
			for (int j = 0; j < 1; j++)
			{
				for (int k = 0; k < 1; k++)
				{
					new TileCoord(k + 15, j + 8);
				}
			}
			ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.TallBoulder, new TileCoord(10, 10).ToWorldPositionTileCentered(), Quaternion.identity);
		}
		else
		{
			for (int l = 0; l < mapHeight; l++)
			{
				for (int m = 0; m < mapWidth; m++)
				{
					TileManager.Instance.SetTileType(new TileCoord(m, l), mapTileData[l * mapWidth + m]);
				}
			}
			TilesToObjectsConverter.ConvertTempTilesToObjects();
			TilesToObjectsConverter.RemoveTempTiles();
			ClearLandingArea();
			DynamicObjectsGenerator.AddDynamicObjects();
			foreach (KeyValuePair<BaseClass, int> item in CollectionManager.Instance.GetCollection("Savable"))
			{
				item.Key.WorldCreated();
			}
		}
		if (!SaveLoadManager.m_MiniMap)
		{
			List<BaseClass> players = CollectionManager.Instance.GetPlayers();
			CameraManager.Instance.Focus(players[0].transform.position);
			CameraManager.Instance.SetDistance(20f);
			RecordingManager.Instance.NewLevelStarted();
		}
		PlotManager.Instance.FinishMerge();
		PlotObjectMergerManager.Instance.FinishAllDirty();
		Debug.Log("*** Done Generating ***");
	}

	private void Update()
	{
		if (m_Generating && !TilemapGenerator.Instance.m_Generating)
		{
			m_Generating = false;
			GenerateTileDataDone();
			if (m_CreateObjects)
			{
				CreateObjects();
			}
			if (m_Finished != null && !m_Finished.Target.Equals(null))
			{
				m_Finished();
			}
		}
	}
}
