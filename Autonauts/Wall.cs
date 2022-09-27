public class Wall : Building
{
	public enum WallType
	{
		Straight,
		Cross,
		Tee,
		El,
		Total
	}

	private WallType m_Type;

	protected string m_CrossModelName;

	protected string m_StraightModelName;

	protected string m_TModelName;

	protected string m_LModelName;

	public static bool GetIsTypeWall(ObjectType NewType)
	{
		if (NewType == ObjectType.BlockWall || NewType == ObjectType.BrickWall || NewType == ObjectType.LogWall || NewType == ObjectType.StoneWall || NewType == ObjectType.FencePicket || NewType == ObjectType.FencePost || NewType == ObjectType.CastleWall || NewType == ObjectType.CastlePlainTower || NewType == ObjectType.CastleFancyTower || NewType == ObjectType.CastleGate)
		{
			return true;
		}
		return false;
	}

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("Wall", m_TypeIdentifier);
		ObjectTypeList.Instance.SetTypeSleep(ObjectType.BlockWall);
		ObjectTypeList.Instance.SetTypeSleep(ObjectType.BrickWall);
		ObjectTypeList.Instance.SetTypeSleep(ObjectType.CastleWall);
		ObjectTypeList.Instance.SetTypeSleep(ObjectType.LogWall);
		ObjectTypeList.Instance.SetTypeSleep(ObjectType.StoneWall);
		ObjectTypeList.Instance.SetTypeSleep(ObjectType.FencePicket);
		ObjectTypeList.Instance.SetTypeSleep(ObjectType.FencePost);
	}

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(0, 0), new TileCoord(0, 0), new TileCoord(0, 1));
		HideAccessModel();
		Sleep();
	}

	public override void Delete()
	{
		m_ParentBuilding = null;
		LoadNewModel("Models/Buildings/" + m_StraightModelName);
		base.Delete();
	}

	private bool IsJoinable(TileCoord Position)
	{
		if (Position.y < 0 || Position.y >= TileManager.Instance.m_TilesHigh || Position.x < 0 || Position.x >= TileManager.Instance.m_TilesWide)
		{
			return false;
		}
		Building building = TileManager.Instance.GetTile(Position).m_Building;
		if ((bool)building)
		{
			building = building.GetBuildingAtLevel(m_Level);
			if ((bool)building)
			{
				if (building.m_TypeIdentifier == ObjectType.ConverterFoundation)
				{
					Building newBuilding = building.GetComponent<ConverterFoundation>().m_NewBuilding;
					if (newBuilding.m_TypeIdentifier == m_TypeIdentifier || Door.GetIsTypeDoorOrArch(newBuilding.m_TypeIdentifier) || Window.GetIsTypeWindow(newBuilding.m_TypeIdentifier))
					{
						return true;
					}
				}
				if (building.m_TypeIdentifier == m_TypeIdentifier || Door.GetIsTypeDoorOrArch(building.m_TypeIdentifier) || Window.GetIsTypeWindow(building.m_TypeIdentifier))
				{
					return true;
				}
			}
		}
		return false;
	}

	public WallType CalcWallType(out bool Up, out bool Down, out bool Left, out bool Right, out int Rotation)
	{
		Up = IsJoinable(m_TileCoord + new TileCoord(0, -1));
		Down = IsJoinable(m_TileCoord + new TileCoord(0, 1));
		Left = IsJoinable(m_TileCoord + new TileCoord(-1, 0));
		Right = IsJoinable(m_TileCoord + new TileCoord(1, 0));
		WallType result;
		if (Up != Down && Left != Right)
		{
			result = WallType.El;
			if (Up)
			{
				if (Left)
				{
					Rotation = 1;
				}
				else
				{
					Rotation = 2;
				}
			}
			else if (Left)
			{
				Rotation = 0;
			}
			else
			{
				Rotation = 3;
			}
		}
		else if (((Up & Down) && Left != Right) || (Up != Down && (Left & Right)))
		{
			result = WallType.Tee;
			if (!Up)
			{
				Rotation = 0;
			}
			else if (!Down)
			{
				Rotation = 2;
			}
			else if (!Left)
			{
				Rotation = 3;
			}
			else
			{
				Rotation = 1;
			}
		}
		else if (Up & Down & Left & Right)
		{
			result = WallType.Cross;
			Rotation = 0;
		}
		else
		{
			result = WallType.Straight;
			if (Up | Down)
			{
				Rotation = 1;
			}
			else if (Left | Right)
			{
				Rotation = 0;
			}
			else
			{
				Building building = TileManager.Instance.GetTile(m_TileCoord).m_Building;
				if ((bool)building && ((building.m_TypeIdentifier != ObjectType.ConverterFoundation && building != GetComponent<Building>()) || (building.m_TypeIdentifier == ObjectType.ConverterFoundation && building.GetComponent<ConverterFoundation>().m_NewBuilding != GetComponent<Building>())))
				{
					if ((bool)building.GetBuildingAtLevel(m_Level - 1))
					{
						Rotation = building.GetBuildingAtLevel(m_Level - 1).m_Rotation;
					}
					else
					{
						Rotation = 0;
					}
				}
				else
				{
					Rotation = 0;
				}
			}
		}
		return result;
	}

	protected override void Refresh()
	{
		bool Up;
		bool Down;
		bool Left;
		bool Right;
		int Rotation;
		WallType wallType = CalcWallType(out Up, out Down, out Left, out Right, out Rotation);
		if (wallType != m_Type)
		{
			m_Type = wallType;
			string text = m_StraightModelName;
			switch (wallType)
			{
			case WallType.El:
				text = m_LModelName;
				break;
			case WallType.Tee:
				text = m_TModelName;
				break;
			case WallType.Cross:
				text = m_CrossModelName;
				break;
			}
			bool randomVariants = text == m_StraightModelName;
			LoadNewModel("Models/Buildings/" + text, randomVariants);
		}
		if (Up || Down || Left || Right)
		{
			SetRotation(Rotation);
		}
		Tile tile = TileManager.Instance.GetTile(m_TileCoord);
		if ((bool)tile.m_Floor)
		{
			tile.m_Floor.SendAction(new ActionInfo(ActionType.Refresh, default(TileCoord)));
		}
	}

	protected override void RefreshConnected()
	{
		BuildingManager.Instance.RefreshSurroundingBuildings(this);
	}
}
