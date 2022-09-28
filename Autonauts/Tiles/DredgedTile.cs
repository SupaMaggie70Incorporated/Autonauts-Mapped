using SimpleJSON;

public class DredgedTile
{
	public TileCoord m_Coord;

	public bool m_Filling;

	public Tile.TileType m_FillingType;

	public TileCoord m_FillDirection;

	public DredgedTile()
	{
		m_Filling = false;
	}

	public DredgedTile(TileCoord NewCoord)
	{
		m_Coord = NewCoord;
		m_Filling = false;
	}

	public void Save(JSONNode Node)
	{
		m_Coord.Save(Node, "T");
		JSONUtils.Set(Node, "F", m_Filling);
		JSONUtils.Set(Node, "FT", (int)m_FillingType);
		m_FillDirection.Save(Node, "FD");
	}

	public void Load(JSONNode Node)
	{
		m_Coord.Load(Node, "T");
		m_Filling = JSONUtils.GetAsBool(Node, "F", DefaultValue: false);
		m_FillingType = (Tile.TileType)JSONUtils.GetAsInt(Node, "FT", 0);
		m_FillDirection.Load(Node, "FD");
	}

	public void StartFilling(Tile.TileType FillingType, TileCoord FillDirection)
	{
		m_Filling = true;
		m_FillingType = FillingType;
		m_FillDirection = FillDirection;
	}
}
