using UnityEngine;

public class Bridge : Floor
{
	[HideInInspector]
	public bool m_OrientationSet;

	[HideInInspector]
	public bool m_Vertical;

	private bool m_End;

	protected string m_ModelNormal;

	protected string m_ModelEnd;

	protected string m_ModelCross;

	public static bool GetIsTypeBridge(ObjectType NewType)
	{
		if (NewType == ObjectType.Bridge || NewType == ObjectType.BridgeStone || NewType == ObjectType.CastleDrawbridge)
		{
			return true;
		}
		return false;
	}

	public override void RegisterClass()
	{
		base.RegisterClass();
		ModelManager.Instance.AddModel("Models/Buildings/Floors/BridgeCross", ObjectType.Bridge);
		ModelManager.Instance.AddModel("Models/Buildings/Floors/BridgeEnd", ObjectType.Bridge);
	}

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(0, 0), new TileCoord(0, 0), new TileCoord(0, 0));
		HideAccessModel();
		m_OrientationSet = false;
		m_Vertical = false;
		m_End = false;
		m_ModelNormal = "Bridge";
		m_ModelEnd = "BridgeEnd";
		m_ModelCross = "BridgeCross";
	}

	public override void Delete()
	{
		m_ParentBuilding = null;
		LoadNewModel("Models/Buildings/Floors/" + m_ModelNormal);
		base.Delete();
	}

	public override object GetActionInfo(GetActionInfo Info)
	{
		GetAction action = Info.m_Action;
		if ((uint)(action - 3) <= 1u)
		{
			Tile tile = TileManager.Instance.GetTile(m_TileCoord);
			if ((bool)tile.m_BuildingFootprint)
			{
				return false;
			}
			if ((bool)tile.m_Farmer)
			{
				return false;
			}
			if ((bool)PlotManager.Instance.GetSelectableObjectAtTile(m_TileCoord))
			{
				return false;
			}
		}
		return base.GetActionInfo(Info);
	}

	protected override void TileCoordChanged(TileCoord Position)
	{
		base.TileCoordChanged(Position);
		Vector3 position = base.transform.position;
		if (position.y < 0f)
		{
			position.y = 0f;
			base.transform.position = position;
		}
	}

	public void SetVertical(bool Vertical)
	{
		m_Vertical = Vertical;
		m_OrientationSet = true;
	}

	public override float GetHeight()
	{
		float num = 0f;
		Tile.TileType tileType = TileManager.Instance.GetTileType(m_TileCoord);
		if (m_End && !TileHelpers.GetTileWater(tileType))
		{
			return 0.42f;
		}
		return 1.04f;
	}

	private bool IsJoinable(TileCoord Position)
	{
		Building floor = TileManager.Instance.GetTile(Position).m_Floor;
		if ((bool)floor && GetIsTypeBridge(floor.m_TypeIdentifier))
		{
			return true;
		}
		floor = TileManager.Instance.GetTile(Position).m_Floor;
		if ((bool)floor && floor.m_TypeIdentifier == ObjectType.ConverterFoundation && GetIsTypeBridge(floor.GetComponent<ConverterFoundation>().m_NewBuilding.m_TypeIdentifier))
		{
			return true;
		}
		return false;
	}

	private bool IsJoinableOrientation(TileCoord Position)
	{
		Building floor = TileManager.Instance.GetTile(Position).m_Floor;
		if ((bool)floor && GetIsTypeBridge(floor.m_TypeIdentifier))
		{
			if (floor.GetComponent<Bridge>() == null)
			{
				return false;
			}
			if (!m_OrientationSet && !floor.GetComponent<Bridge>().m_OrientationSet)
			{
				return true;
			}
			if (m_Vertical == floor.GetComponent<Bridge>().m_Vertical)
			{
				return true;
			}
			return false;
		}
		floor = TileManager.Instance.GetTile(Position).m_Building;
		if ((bool)floor && floor.m_TypeIdentifier == ObjectType.ConverterFoundation)
		{
			Building newBuilding = floor.GetComponent<ConverterFoundation>().m_NewBuilding;
			if (GetIsTypeBridge(newBuilding.m_TypeIdentifier))
			{
				if (newBuilding.GetComponent<Bridge>() == null)
				{
					return false;
				}
				if (!m_OrientationSet && !newBuilding.GetComponent<Bridge>().m_OrientationSet)
				{
					return true;
				}
				if (m_Vertical == newBuilding.GetComponent<Bridge>().m_Vertical)
				{
					return true;
				}
				return false;
			}
		}
		return false;
	}

	private bool GetStandingInWater()
	{
		return TileHelpers.GetTileWater(TileManager.Instance.GetTileType(m_TileCoord));
	}

	protected override void Refresh()
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		if (m_TileCoord.y > 0 && IsJoinable(m_TileCoord + new TileCoord(0, -1)))
		{
			flag = true;
		}
		if (m_TileCoord.y < TileManager.Instance.m_TilesHigh - 1 && IsJoinable(m_TileCoord + new TileCoord(0, 1)))
		{
			flag2 = true;
		}
		if (m_TileCoord.x > 0 && IsJoinable(m_TileCoord + new TileCoord(-1, 0)))
		{
			flag3 = true;
		}
		if (m_TileCoord.x < TileManager.Instance.m_TilesWide - 1 && IsJoinable(m_TileCoord + new TileCoord(1, 0)))
		{
			flag4 = true;
		}
		if (!m_OrientationSet)
		{
			if (flag || flag2)
			{
				m_Vertical = true;
			}
			else if (flag3 || flag4)
			{
				m_Vertical = false;
			}
		}
		if (m_Vertical)
		{
			if (m_TileCoord.y > 0 && IsJoinableOrientation(m_TileCoord + new TileCoord(0, -1)))
			{
				flag = true;
			}
			if (m_TileCoord.y < TileManager.Instance.m_TilesHigh - 1 && IsJoinableOrientation(m_TileCoord + new TileCoord(0, 1)))
			{
				flag2 = true;
			}
		}
		else
		{
			if (m_TileCoord.x > 0 && IsJoinableOrientation(m_TileCoord + new TileCoord(-1, 0)))
			{
				flag3 = true;
			}
			if (m_TileCoord.x < TileManager.Instance.m_TilesWide - 1 && IsJoinableOrientation(m_TileCoord + new TileCoord(1, 0)))
			{
				flag4 = true;
			}
		}
		if ((flag || flag2) && (flag3 || flag4))
		{
			m_End = false;
			if (m_Vertical)
			{
				SetRotation(1);
			}
			else
			{
				SetRotation(0);
			}
			LoadNewModel("Models/Buildings/Floors/" + m_ModelCross);
			return;
		}
		if ((m_Vertical && flag && flag2) || (!m_Vertical && flag3 && flag4))
		{
			m_End = false;
			if (m_Vertical)
			{
				SetRotation(1);
			}
			else
			{
				SetRotation(0);
			}
			LoadNewModel("Models/Buildings/Floors/" + m_ModelNormal);
			return;
		}
		m_End = true;
		if (m_Vertical)
		{
			if (flag)
			{
				SetRotation(1);
			}
			else
			{
				SetRotation(3);
			}
		}
		else if (flag3)
		{
			SetRotation(0);
		}
		else
		{
			SetRotation(2);
		}
		if (GetStandingInWater())
		{
			LoadNewModel("Models/Buildings/Floors/" + m_ModelNormal);
		}
		else
		{
			LoadNewModel("Models/Buildings/Floors/" + m_ModelEnd);
		}
	}

	public override bool CanBuildOn()
	{
		if (m_End && !GetStandingInWater())
		{
			return false;
		}
		return true;
	}
}
