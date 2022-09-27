using System;
using SimpleJSON;
using UnityEngine;

public class GameOptions
{
	public enum GameMode
	{
		ModeCampaign,
		ModeFree,
		ModeCreative,
		Total
	}

	public enum GameSize
	{
		Small,
		Medium,
		Large,
		Total
	}

	public Vector2Int[] m_Sizes = new Vector2Int[3]
	{
		new Vector2Int(210, 120),
		new Vector2Int(420, 240),
		new Vector2Int(630, 360)
	};

	public float[] m_SizeScaler = new float[3] { 0.25f, 1f, 2.25f };

	public GameMode m_GameMode;

	public GameSize m_GameSize;

	public bool m_RandomObjectsEnabled;

	public bool m_BadgeUnlocksEnabled;

	public bool m_TutorialEnabled;

	public bool m_RecordingEnabled;

	public int m_MapWidth;

	public int m_MapHeight;

	public Tile.TileType[] m_MapTileData;

	public TileCoord m_PlayerPosition;

	public TileCoord m_FlagPosition;

	public int m_MapSeed;

	public string m_MapName;

	public GameOptions()
	{
		m_MapTileData = null;
		m_GameMode = GameMode.ModeCampaign;
		SetMapSize(GameSize.Medium);
		m_PlayerPosition = new TileCoord(m_MapWidth / 2 + Plot.m_PlotTilesWide / 2, m_MapHeight / 2 + Plot.m_PlotTilesHigh / 2);
		m_RandomObjectsEnabled = true;
		m_BadgeUnlocksEnabled = true;
		m_TutorialEnabled = true;
		m_RecordingEnabled = false;
		NewMapSeed();
		m_MapName = "";
	}

	public void SetDefaults()
	{
		m_MapName = TextManager.Instance.Get("NewGameDefaultName");
	}

	public void Save(JSONNode Node)
	{
		JSONUtils.Set(Node, "GameMode", (int)m_GameMode);
		JSONUtils.Set(Node, "GameSize", (int)m_GameSize);
		JSONUtils.Set(Node, "RandomObjectsEnabled", m_RandomObjectsEnabled);
		JSONUtils.Set(Node, "BadgeUnlocksEnabled", m_BadgeUnlocksEnabled);
		JSONUtils.Set(Node, "RecordingEnabled", m_RecordingEnabled);
		JSONUtils.Set(Node, "Seed", m_MapSeed);
		JSONUtils.Set(Node, "Name", m_MapName);
	}

	public void Load(JSONNode Node)
	{
		m_GameMode = (GameMode)JSONUtils.GetAsInt(Node, "GameMode", 0);
		m_GameSize = (GameSize)JSONUtils.GetAsInt(Node, "GameSize", 1);
		SetMapSize(m_GameSize);
		m_RandomObjectsEnabled = JSONUtils.GetAsBool(Node, "RandomObjectsEnabled", DefaultValue: true);
		m_BadgeUnlocksEnabled = JSONUtils.GetAsBool(Node, "BadgeUnlocksEnabled", DefaultValue: true);
		m_RecordingEnabled = JSONUtils.GetAsBool(Node, "RecordingEnabled", DefaultValue: true);
		m_MapSeed = JSONUtils.GetAsInt(Node, "Seed", 0);
		m_MapName = JSONUtils.GetAsString(Node, "Name", "Tim");
	}

	public void SetMapData(Tile.TileType[] MapData, int MapWidth, int MapHeight, TileCoord PlayerPosition, TileCoord FlagPosition)
	{
		m_MapTileData = new Tile.TileType[MapWidth * MapHeight];
		MapData.CopyTo(m_MapTileData, 0);
		m_MapWidth = MapWidth;
		m_MapHeight = MapHeight;
		m_PlayerPosition = PlayerPosition;
		m_FlagPosition = FlagPosition;
	}

	public void SetMapData(Tile[] MapData, int MapWidth, int MapHeight)
	{
		int num = MapWidth * MapHeight;
		m_MapTileData = new Tile.TileType[MapWidth * MapHeight];
		for (int i = 0; i < num; i++)
		{
			m_MapTileData[i] = MapData[i].m_TileType;
		}
		m_MapWidth = MapWidth;
		m_MapHeight = MapHeight;
	}

	public void SetPlayerPosition(TileCoord PlayerPosition)
	{
		m_PlayerPosition = PlayerPosition;
	}

	public void SetFlagPosition(TileCoord FlagPosition)
	{
		m_FlagPosition = FlagPosition;
	}

	public void SetMapSize(GameSize NewSize)
	{
		m_GameSize = NewSize;
		m_MapWidth = m_Sizes[(int)NewSize].x;
		m_MapHeight = m_Sizes[(int)NewSize].y;
		if (SaveLoadManager.m_MiniMap)
		{
			m_MapWidth = 84;
			m_MapHeight = 96;
		}
	}

	public void NewMapSeed()
	{
		m_MapSeed = (int)DateTime.Now.Ticks;
		if (m_MapSeed < 0)
		{
			m_MapSeed = -m_MapSeed;
		}
		if (m_MapSeed > 999999999)
		{
			m_MapSeed -= 999999999;
		}
	}

	public void SetTile(TileCoord Position, Tile.TileType NewTileType)
	{
		int num = Position.y * m_MapWidth + Position.x;
		if (m_MapTileData != null && num < m_MapWidth * m_MapHeight)
		{
			m_MapTileData[num] = NewTileType;
		}
	}
}
