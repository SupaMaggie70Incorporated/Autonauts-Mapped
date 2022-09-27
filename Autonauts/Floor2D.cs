public class Floor2D : Floor
{
	private enum Type
	{
		Quarter,
		Half,
		ThreeQuarters,
		Full,
		Total
	}

	private Type m_Type;

	protected string m_WholeModelName;

	protected string m_ThreeQuatersModelName;

	protected string m_HalfModelName;

	protected string m_QuaterModelName;

	public static bool GetIsTypeFloor2D(ObjectType NewType)
	{
		if (NewType == ObjectType.FlooringCrude || NewType == ObjectType.Workshop || NewType == ObjectType.FlooringBrick || NewType == ObjectType.FlooringFlagstone || NewType == ObjectType.FlooringParquet || NewType == ObjectType.FlooringChequer)
		{
			return true;
		}
		return false;
	}

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(0, 0), new TileCoord(0, 0), new TileCoord(0, 0));
		m_Type = Type.Total;
	}

	private bool IsJoinable(TileCoord Position)
	{
		if (Position.y < 0 || Position.y >= TileManager.Instance.m_TilesHigh || Position.x < 0 || Position.x >= TileManager.Instance.m_TilesWide)
		{
			return false;
		}
		if (TileManager.Instance.GetTile(m_TileCoord).m_Building == null)
		{
			return false;
		}
		Building floor = TileManager.Instance.GetTile(Position).m_Floor;
		if ((bool)floor)
		{
			if (floor.m_TypeIdentifier == ObjectType.ConverterFoundation && floor.m_TypeIdentifier == m_TypeIdentifier)
			{
				return true;
			}
			if (floor.m_TypeIdentifier == m_TypeIdentifier)
			{
				return true;
			}
		}
		return false;
	}

	private Type GetType(Wall.WallType NewWallType, bool WallUp, bool WallDown, bool WallLeft, bool WallRight, int WallRotation, out int Rotation)
	{
		Type result = Type.Full;
		Rotation = 0;
		bool flag = IsJoinable(m_TileCoord + new TileCoord(0, -1));
		bool flag2 = IsJoinable(m_TileCoord + new TileCoord(0, 1));
		bool flag3 = IsJoinable(m_TileCoord + new TileCoord(-1, 0));
		bool flag4 = IsJoinable(m_TileCoord + new TileCoord(1, 0));
		switch (NewWallType)
		{
		case Wall.WallType.El:
		{
			bool flag5 = IsJoinable(m_TileCoord + new TileCoord(-1, -1));
			bool flag6 = IsJoinable(m_TileCoord + new TileCoord(1, -1));
			bool flag7 = IsJoinable(m_TileCoord + new TileCoord(-1, 1));
			bool flag8 = IsJoinable(m_TileCoord + new TileCoord(1, 1));
			if (flag != flag2 && flag3 != flag4)
			{
				result = Type.Quarter;
				if (flag)
				{
					if (flag3)
					{
						Rotation = 0;
					}
					else
					{
						Rotation = 1;
					}
				}
				else if (flag3)
				{
					Rotation = 3;
				}
				else
				{
					Rotation = 2;
				}
			}
			else if (WallRotation == 1 && !flag5)
			{
				result = Type.ThreeQuarters;
				Rotation = 3;
			}
			else if (WallRotation == 2 && !flag6)
			{
				result = Type.ThreeQuarters;
				Rotation = 0;
			}
			else if (WallRotation == 0 && !flag7)
			{
				result = Type.ThreeQuarters;
				Rotation = 2;
			}
			else if (WallRotation == 3 && !flag8)
			{
				result = Type.ThreeQuarters;
				Rotation = 1;
			}
			break;
		}
		case Wall.WallType.Straight:
			switch (WallRotation)
			{
			case 0:
			case 2:
				if (!flag2 && flag)
				{
					result = Type.Half;
					Rotation = 0;
				}
				else if (!flag && flag2)
				{
					result = Type.Half;
					Rotation = 2;
				}
				break;
			case 1:
			case 3:
				if (!flag3 && flag4)
				{
					result = Type.Half;
					Rotation = 1;
				}
				else if (!flag4 && flag3)
				{
					result = Type.Half;
					Rotation = 3;
				}
				break;
			}
			break;
		}
		return result;
	}

	protected override void Refresh()
	{
		Building building = TileManager.Instance.GetTile(m_TileCoord).m_Building;
		if (building == null || !building.GetComponent<Wall>())
		{
			if (m_Type != Type.Full)
			{
				m_Type = Type.Full;
				LoadNewModel("Models/Buildings/Floors/" + m_WholeModelName, RandomVariants: true);
			}
			SetRotation(0);
			return;
		}
		bool Up;
		bool Down;
		bool Left;
		bool Right;
		int Rotation;
		Wall.WallType newWallType = building.GetComponent<Wall>().CalcWallType(out Up, out Down, out Left, out Right, out Rotation);
		int Rotation2;
		Type type = GetType(newWallType, Up, Down, Left, Right, Rotation, out Rotation2);
		if (type != m_Type)
		{
			m_Type = type;
			string text = m_WholeModelName;
			switch (type)
			{
			case Type.Quarter:
				text = m_QuaterModelName;
				break;
			case Type.Half:
				text = m_HalfModelName;
				break;
			case Type.ThreeQuarters:
				text = m_ThreeQuatersModelName;
				break;
			}
			bool randomVariants = text == m_WholeModelName;
			LoadNewModel("Models/Buildings/Floors/" + text, randomVariants);
		}
		SetRotation(Rotation2);
	}
}
