using UnityEngine;

public class Door : Building
{
	private GameObject m_Hinge;

	private bool m_Openable;

	private bool m_Open;

	public static bool GetIsTypeDoor(ObjectType NewType)
	{
		if (NewType == ObjectType.Gate || NewType == ObjectType.Door || NewType == ObjectType.GatePicket || NewType == ObjectType.StoneArchDoor || NewType == ObjectType.BlockDoor || NewType == ObjectType.BrickArchwayDoor)
		{
			return true;
		}
		return false;
	}

	public static bool GetIsTypeDoorOrArch(ObjectType NewType)
	{
		if (GetIsTypeDoor(NewType) || NewType == ObjectType.LogArch || NewType == ObjectType.StoneArch || ModManager.Instance.ModBuildingClass.IsItWalkable(NewType) || NewType == ObjectType.BrickArchway || NewType == ObjectType.BrickArchwayDoor || NewType == ObjectType.HedgeArchway)
		{
			return true;
		}
		return false;
	}

	public override void Restart()
	{
		base.Restart();
		m_Open = false;
		SetDimensions(new TileCoord(0, 0), new TileCoord(0, 0), new TileCoord(0, 1));
		if ((bool)m_ModelRoot.transform.Find("Hinge"))
		{
			m_Hinge = m_ModelRoot.transform.Find("Hinge").gameObject;
		}
		HideAccessModel();
		m_Openable = false;
		if ((bool)m_Hinge)
		{
			m_Openable = true;
		}
	}

	public new void Awake()
	{
		base.Awake();
		m_MaxLevels = 5;
		m_NumLevels = 3;
	}

	public override bool GetNewLevelAllowed(Building NewBuilding)
	{
		if (m_TotalLevels + NewBuilding.m_TotalLevels > m_MaxLevels)
		{
			return false;
		}
		ObjectType typeIdentifier = NewBuilding.m_TypeIdentifier;
		if (typeIdentifier == ObjectType.ConverterFoundation)
		{
			typeIdentifier = NewBuilding.GetComponent<ConverterFoundation>().m_NewBuilding.m_TypeIdentifier;
		}
		if (m_TypeIdentifier == ObjectType.StoneArch || m_TypeIdentifier == ObjectType.StoneArchDoor || m_TypeIdentifier == ObjectType.LogArch || m_TypeIdentifier == ObjectType.Gate || m_TypeIdentifier == ObjectType.GatePicket)
		{
			return false;
		}
		if (typeIdentifier != ObjectType.BrickWall && typeIdentifier != ObjectType.LogWall && typeIdentifier != ObjectType.BlockWall && !Window.GetIsTypeWindow(typeIdentifier))
		{
			return false;
		}
		return true;
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
					if (newBuilding.m_TypeIdentifier == m_TypeIdentifier || GetIsTypeDoor(newBuilding.m_TypeIdentifier) || Window.GetIsTypeWindow(newBuilding.m_TypeIdentifier))
					{
						return true;
					}
				}
				if (Wall.GetIsTypeWall(building.m_TypeIdentifier) || GetIsTypeDoor(building.m_TypeIdentifier) || Window.GetIsTypeWindow(building.m_TypeIdentifier))
				{
					return true;
				}
			}
		}
		return false;
	}

	protected override void Refresh()
	{
		bool num = IsJoinable(m_TileCoord + new TileCoord(0, -1));
		bool flag = IsJoinable(m_TileCoord + new TileCoord(0, 1));
		if ((num || flag) && m_Rotation != 1 && m_Rotation != 3)
		{
			SetRotation(1);
		}
		Tile tile = TileManager.Instance.GetTile(m_TileCoord);
		if ((bool)tile.m_Floor)
		{
			tile.m_Floor.SendAction(new ActionInfo(ActionType.Refresh, default(TileCoord)));
		}
	}

	private void Update()
	{
		if (!GetIsSavable() || !m_Openable)
		{
			return;
		}
		bool flag = false;
		if ((bool)TileManager.Instance.GetTile(m_TileCoord).m_Farmer)
		{
			flag = true;
		}
		if (flag != m_Open)
		{
			m_Open = flag;
			if (m_Open)
			{
				AudioManager.Instance.StartEvent("BuildingGateOpen", this);
				m_Hinge.transform.localRotation = Quaternion.Euler(0f, 90f, 0f) * ObjectUtils.m_ModelRotator;
			}
			else
			{
				AudioManager.Instance.StartEvent("BuildingGateClose", this);
				m_Hinge.transform.localRotation = Quaternion.Euler(0f, 0f, 0f) * ObjectUtils.m_ModelRotator;
			}
		}
	}
}
