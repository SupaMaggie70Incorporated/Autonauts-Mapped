using System.Collections.Generic;
using UnityEngine;

public class WalledAreaManager : MonoBehaviour
{
	private static bool m_Debug;

	public static WalledAreaManager Instance;

	private List<WalledArea> m_Areas;

	private List<TileCoord> m_WallsUpdated;

	private List<int> m_SeachFoundTiles;

	private int m_SearchSize = 400;

	private bool m_SearchFailed;

	private ObjectType m_SearchWallType;

	private TileCoord m_SearchTopLeft;

	private TileCoord m_SearchBottomRight;

	private bool m_CheckPennedAnimals;

	private void Awake()
	{
		Instance = this;
		m_Areas = new List<WalledArea>();
		m_WallsUpdated = new List<TileCoord>();
		Shader.SetGlobalColor("_WalledAreaColor", new Color(0f, 1f, 1f, 1f));
		m_SearchSize = VariableManager.Instance.GetVariableAsInt(ObjectType.BrickWall, "SearchArea");
	}

	private void SearchForArea(TileCoord Position)
	{
		if (Position.x < 0 || Position.x >= TileManager.Instance.m_TilesWide || Position.y < 0 || Position.y >= TileManager.Instance.m_TilesHigh)
		{
			m_SearchFailed = true;
		}
		if (m_SeachFoundTiles.Count > m_SearchSize)
		{
			m_SearchFailed = true;
		}
		if (m_SearchFailed)
		{
			return;
		}
		int index = Position.GetIndex();
		Tile tile = TileManager.Instance.m_Tiles[index];
		if (!tile.m_Checked)
		{
			if (tile.m_Building == null || !GetIsBuildingWall(tile.m_Building))
			{
				TileManager.Instance.AddTileChecked(index);
				m_SeachFoundTiles.Add(index);
				if (Position.x < m_SearchTopLeft.x)
				{
					m_SearchTopLeft.x = Position.x;
				}
				if (Position.y < m_SearchTopLeft.y)
				{
					m_SearchTopLeft.y = Position.y;
				}
				if (Position.x > m_SearchBottomRight.x)
				{
					m_SearchBottomRight.x = Position.x;
				}
				if (Position.y > m_SearchBottomRight.y)
				{
					m_SearchBottomRight.y = Position.y;
				}
				if (m_SearchBottomRight.x - m_SearchTopLeft.x > 100 || m_SearchBottomRight.y - m_SearchTopLeft.y > 100)
				{
					m_SearchFailed = true;
					return;
				}
				if (tile.m_WalledArea != null)
				{
					if (m_Debug)
					{
						Debug.Log("Destroy Area " + tile.m_WalledArea.m_Tiles.Count);
					}
					m_Areas.Remove(tile.m_WalledArea);
					tile.m_WalledArea.Destroy();
				}
				SearchForArea(Position + new TileCoord(-1, 0));
				SearchForArea(Position + new TileCoord(1, 0));
				SearchForArea(Position + new TileCoord(0, -1));
				SearchForArea(Position + new TileCoord(0, 1));
				SearchForArea(Position + new TileCoord(-1, -1));
				SearchForArea(Position + new TileCoord(1, -1));
				SearchForArea(Position + new TileCoord(-1, 1));
				SearchForArea(Position + new TileCoord(1, 1));
			}
			else if (Wall.GetIsTypeWall(tile.m_Building.m_TypeIdentifier) && m_SearchWallType == ObjectTypeList.m_Total)
			{
				m_SearchWallType = tile.m_Building.m_TypeIdentifier;
			}
		}
		else if (!m_SeachFoundTiles.Contains(index))
		{
			m_SearchFailed = true;
		}
	}

	public void WallUpdated(TileCoord Position)
	{
		if (!m_WallsUpdated.Contains(Position))
		{
			m_WallsUpdated.Add(Position);
		}
	}

	public bool GetIsBuildingWall(Building NewWall)
	{
		if (Wall.GetIsTypeWall(NewWall.m_TypeIdentifier) || Door.GetIsTypeDoorOrArch(NewWall.m_TypeIdentifier) || Window.GetIsTypeWindow(NewWall.m_TypeIdentifier))
		{
			return true;
		}
		return false;
	}

	private void StartSearchForArea(TileCoord Position, TileCoord Offset)
	{
		m_SearchFailed = false;
		m_SearchWallType = ObjectTypeList.m_Total;
		m_SeachFoundTiles = new List<int>();
		m_SearchTopLeft = Position;
		m_SearchBottomRight = Position;
		SearchForArea(Position + Offset);
		if (!m_SearchFailed && m_SeachFoundTiles.Count != 0)
		{
			if (m_Debug)
			{
				Debug.Log("Area added " + m_SeachFoundTiles.Count + " " + m_SearchWallType);
			}
			WalledArea item = new WalledArea(m_SeachFoundTiles, m_SearchWallType);
			m_Areas.Add(item);
		}
	}

	public void CheckAllAreasForCowsOrSheep(TileCoord Position)
	{
		int index = Position.GetIndex();
		foreach (WalledArea area in m_Areas)
		{
			if (area.m_Tiles.Contains(index))
			{
				area.CheckForCowsOrSheep();
				break;
			}
		}
	}

	public void CheckPennedAnimals()
	{
		m_CheckPennedAnimals = true;
	}

	private void UpdatePennedAnimals()
	{
		if (!m_CheckPennedAnimals)
		{
			return;
		}
		m_CheckPennedAnimals = false;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		foreach (WalledArea area in m_Areas)
		{
			num += area.m_PennedCows;
			num2 += area.m_PennedSheep;
			num3 += area.m_PennedChickens;
		}
		if (num > 0)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.PenCows, Bot: false, 0, null, num);
		}
		if (num2 > 0)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.PenSheep, Bot: false, 0, null, num2);
		}
		if (num3 > 0)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.PenChooks, Bot: false, 0, null, num3);
		}
	}

	private void Update()
	{
		if (SaveLoadManager.Instance.m_Loading)
		{
			return;
		}
		if (m_WallsUpdated.Count > 0)
		{
			foreach (TileCoord item in m_WallsUpdated)
			{
				TileManager.Instance.StartTileSearch(m_SearchSize * 8);
				Tile tile = TileManager.Instance.GetTile(item);
				if ((bool)tile.m_Building)
				{
					m_SearchWallType = tile.m_Building.m_TypeIdentifier;
				}
				if (item.y != 0)
				{
					StartSearchForArea(item, new TileCoord(0, -1));
				}
				if (item.y != TileManager.Instance.m_TilesHigh - 1)
				{
					StartSearchForArea(item, new TileCoord(0, 1));
				}
				if (item.x != 0)
				{
					StartSearchForArea(item, new TileCoord(-1, 0));
				}
				if (item.x != TileManager.Instance.m_TilesHigh - 1)
				{
					StartSearchForArea(item, new TileCoord(1, 0));
				}
				if (item.y != 0 && item.x != 0)
				{
					StartSearchForArea(item, new TileCoord(-1, -1));
				}
				if (item.y != 0 && item.x != TileManager.Instance.m_TilesHigh - 1)
				{
					StartSearchForArea(item, new TileCoord(1, -1));
				}
				if (item.y != TileManager.Instance.m_TilesHigh - 1 && item.x != 0)
				{
					StartSearchForArea(item, new TileCoord(-1, 1));
				}
				if (item.y != TileManager.Instance.m_TilesHigh - 1 && item.x != TileManager.Instance.m_TilesHigh - 1)
				{
					StartSearchForArea(item, new TileCoord(1, 1));
				}
				TileManager.Instance.ClearCheckedTiles();
			}
			m_WallsUpdated.Clear();
			if (m_Debug)
			{
				Debug.Log("**********");
				foreach (WalledArea area in m_Areas)
				{
					Debug.Log("Area " + area.m_Tiles.Count);
				}
			}
		}
		UpdatePennedAnimals();
	}
}
