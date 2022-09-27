using SimpleJSON;
using UnityEngine;

public struct TileCoord
{
	public int x;

	public int y;

	public TileCoord(int nx = 0, int ny = 0)
	{
		x = nx;
		y = ny;
	}

	public TileCoord(Vector2 vec)
	{
		x = (int)vec.x;
		y = (int)vec.y;
	}

	public TileCoord(Vector3 WorldPosition)
	{
		x = (int)(WorldPosition.x / Tile.m_Size);
		y = (int)((0f - WorldPosition.z) / Tile.m_Size);
	}

	public TileCoord(int Index)
	{
		x = 0;
		y = 0;
		SetIndex(Index);
	}

	public void FromWorldPosition(Vector3 WorldPosition)
	{
		x = (int)(WorldPosition.x / Tile.m_Size);
		y = (int)((0f - WorldPosition.z) / Tile.m_Size);
	}

	public Vector3 ToWorldPosition()
	{
		return new Vector3((float)x * Tile.m_Size, 0f, 0f - (float)y * Tile.m_Size);
	}

	public float GetHeightOffGround(bool IgnoreWater = false)
	{
		if (TileManager.Instance == null)
		{
			return 0f;
		}
		float result = TileManager.Instance.GetTileHeight(this, IgnoreWater);
		Tile tile = TileManager.Instance.GetTile(this);
		if (tile != null && (bool)tile.m_Floor)
		{
			float num = 0f;
			num = ((tile.m_Floor.m_TypeIdentifier != ObjectType.ConverterFoundation) ? tile.m_Floor.GetComponent<Floor>().GetHeight() : tile.m_Floor.GetComponent<ConverterFoundation>().m_NewBuilding.GetComponent<Floor>().GetHeight());
			result = num;
		}
		return result;
	}

	public Vector3 ToWorldPositionTileCentered(bool IgnoreWater = false)
	{
		Vector3 result = new Vector3((float)x * Tile.m_Size, 0f, 0f - (float)y * Tile.m_Size);
		result += new Vector3(Tile.m_Size * 0.5f, 0f, (0f - Tile.m_Size) * 0.5f);
		result.y = GetHeightOffGround(IgnoreWater);
		return result;
	}

	public Vector3 ToWorldPositionTileCenteredWithoutHeight()
	{
		return new Vector3(((float)x + 0.5f) * Tile.m_Size, 0f, 0f - ((float)y + 0.5f) * Tile.m_Size);
	}

	public void Rotate(int Rotation)
	{
		switch (Rotation)
		{
		case 1:
		{
			int num = y;
			y = x;
			x = -num;
			break;
		}
		case 2:
			y = -y;
			x = -x;
			break;
		case 3:
		{
			int num = y;
			y = -x;
			x = num;
			break;
		}
		}
	}

	public void Copy(TileCoord c)
	{
		x = c.x;
		y = c.y;
	}

	public override bool Equals(object obj)
	{
		if (((TileCoord)obj).x == x)
		{
			return ((TileCoord)obj).y == y;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return 0;
	}

	public static bool operator ==(TileCoord c1, TileCoord c2)
	{
		if (c1.x == c2.x)
		{
			return c1.y == c2.y;
		}
		return false;
	}

	public static bool operator !=(TileCoord c1, TileCoord c2)
	{
		if (c1.x == c2.x)
		{
			return c1.y != c2.y;
		}
		return true;
	}

	public static TileCoord operator +(TileCoord c1, TileCoord c2)
	{
		return new TileCoord(c1.x + c2.x, c1.y + c2.y);
	}

	public static TileCoord operator -(TileCoord c1, TileCoord c2)
	{
		return new TileCoord(c1.x - c2.x, c1.y - c2.y);
	}

	public static TileCoord operator -(TileCoord c1)
	{
		return new TileCoord(-c1.x, -c1.y);
	}

	public static TileCoord operator *(TileCoord c1, TileCoord c2)
	{
		return new TileCoord(c1.x * c2.x, c1.y * c2.y);
	}

	public static TileCoord operator /(TileCoord c1, TileCoord c2)
	{
		return new TileCoord(c1.x / c2.x, c1.y / c2.y);
	}

	public static TileCoord operator *(TileCoord c1, int val)
	{
		return new TileCoord(c1.x * val, c1.y * val);
	}

	public static Vector2 operator *(TileCoord c1, float val)
	{
		return new Vector2((float)c1.x * val, (float)c1.y * val);
	}

	public JSONNode Save(JSONNode Node, string BaseString)
	{
		JSONUtils.Set(Node, BaseString + "X", x);
		JSONUtils.Set(Node, BaseString + "Y", y);
		return Node;
	}

	public void Load(JSONNode Node, string BaseString)
	{
		x = JSONUtils.GetAsInt(Node, BaseString + "X", 0);
		y = JSONUtils.GetAsInt(Node, BaseString + "Y", 0);
	}

	public float Magnitude()
	{
		return Mathf.Sqrt(x * x + y * y);
	}

	public float MagnitudeSqr()
	{
		return x * x + y * y;
	}

	public int GetIndex()
	{
		return y * TileManager.Instance.m_TilesWide + x;
	}

	public void SetIndex(int NewIndex)
	{
		x = NewIndex % TileManager.Instance.m_TilesWide;
		y = NewIndex / TileManager.Instance.m_TilesWide;
	}

	public bool GetIsValid()
	{
		if (x < 0 || y < 0 || x >= TileManager.Instance.m_TilesWide || y >= TileManager.Instance.m_TilesHigh)
		{
			return false;
		}
		return true;
	}
}
