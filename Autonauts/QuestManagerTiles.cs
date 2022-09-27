using System.Collections.Generic;
using UnityEngine;

public class QuestManagerTiles : MonoBehaviour
{
	public static QuestManagerTiles Instance;

	private static bool m_Debug = false;

	private static int m_MaxAreaSize = 100;

	private List<QuestEventTile>[] m_EventsPerTileType;

	private Tile.TileType m_CheckType;

	private bool m_Initted;

	private void Awake()
	{
		Instance = this;
		m_Initted = false;
	}

	public void Init()
	{
		int num = 71;
		m_EventsPerTileType = new List<QuestEventTile>[num];
		for (int i = 0; i < num; i++)
		{
			m_EventsPerTileType[i] = new List<QuestEventTile>();
		}
		Quest[] questData = QuestData.Instance.m_QuestData;
		foreach (Quest quest in questData)
		{
			if (quest == null)
			{
				continue;
			}
			foreach (QuestEvent item in quest.m_EventsRequired)
			{
				if (item.m_Type == QuestEvent.Type.MineArea)
				{
					Tile.TileType tileType = (Tile.TileType)item.m_ExtraData;
					m_EventsPerTileType[(int)tileType].Add(new QuestEventTile(quest, item));
					if (m_Debug)
					{
						Debug.Log(string.Concat("Event added ", tileType, ":", item.m_Required, " from quest ", quest.m_Title));
					}
				}
			}
		}
		m_Initted = true;
		if (m_Debug)
		{
			Debug.Log("QuestManagerTiles Initted");
		}
	}

	private void FloodFill(int x, int y)
	{
		if (TileManager.Instance.m_SearchDone)
		{
			return;
		}
		int num = y * TileManager.Instance.m_TilesWide + x;
		Tile tile = TileManager.Instance.m_Tiles[num];
		if (tile.m_TileType == m_CheckType && !tile.m_Checked)
		{
			TileManager.Instance.AddTileChecked(num);
			if (x > 0)
			{
				FloodFill(x - 1, y);
			}
			if (y > 0)
			{
				FloodFill(x, y - 1);
			}
			if (x < TileManager.Instance.m_TilesWide - 1)
			{
				FloodFill(x + 1, y);
			}
			if (y < TileManager.Instance.m_TilesHigh - 1)
			{
				FloodFill(x, y + 1);
			}
		}
	}

	private int GetAreaSize(TileCoord Position, Tile.TileType NewType)
	{
		m_CheckType = NewType;
		TileManager.Instance.StartTileSearch(m_MaxAreaSize);
		FloodFill(Position.x, Position.y);
		TileManager.Instance.ClearCheckedTiles();
		return TileManager.Instance.m_SearchIndex;
	}

	public void TileChanged(TileCoord Position, Tile.TileType NewType, BaseClass Actioner)
	{
		if (!m_Initted)
		{
			return;
		}
		List<QuestEventTile> list = m_EventsPerTileType[(int)NewType];
		if (list.Count == 0)
		{
			return;
		}
		if (m_Debug)
		{
			Debug.Log("Checking Tile Type " + NewType);
		}
		int areaSize = GetAreaSize(Position, NewType);
		if (m_Debug)
		{
			Debug.Log("Area = " + areaSize);
		}
		bool bot = false;
		if ((bool)Actioner && Actioner.m_TypeIdentifier == ObjectType.Worker)
		{
			bot = true;
		}
		List<QuestEventTile> list2 = new List<QuestEventTile>();
		foreach (QuestEventTile item in list)
		{
			if (areaSize >= item.m_Event.m_Required)
			{
				QuestManager.Instance.AddEvent(QuestEvent.Type.MineArea, bot, NewType, Actioner, item.m_Event.m_Required, item.m_Event);
				list2.Add(item);
				if (m_Debug)
				{
					Debug.Log("Event used");
				}
			}
			else if (areaSize >= item.m_Event.m_Progress)
			{
				item.m_Event.m_Progress = areaSize;
			}
		}
		foreach (QuestEventTile item2 in list2)
		{
			list.Remove(item2);
		}
		if (m_Debug && list2.Count != 0)
		{
			Debug.Log("Events Left = " + list.Count);
		}
	}
}
