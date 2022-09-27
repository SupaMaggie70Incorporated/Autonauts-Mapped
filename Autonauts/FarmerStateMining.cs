using UnityEngine;

public class FarmerStateMining : FarmerStateTool
{
	private BlockSoil m_BlockSoil;

	private float m_OldTileHeight;

	private TileCoord m_TileCoord;

	private Tile m_TargetTile;

	private Tile.TileType m_OldTileType;

	public FarmerStateMining()
	{
		m_ActionSoundName = "ToolPickaxeHit";
		m_NoToolIconName = "GenIcons/GenIconToolPickMetal";
	}

	public override bool GetIsAdjacentTile(TileCoord TargetTile, BaseClass Object)
	{
		if ((bool)Object.GetComponent<Plot>())
		{
			return true;
		}
		return false;
	}

	public static bool GetIsToolAcceptable(ObjectType NewType)
	{
		return ToolPick.GetIsTypePick(NewType);
	}

	public override bool IsToolAcceptable(Holdable NewObject)
	{
		if (NewObject == null)
		{
			return false;
		}
		return GetIsToolAcceptable(NewObject.m_TypeIdentifier);
	}

	public static bool GetWillToolMineDown(ObjectType HeldObjectType, Tile.TileType TileType)
	{
		if (HeldObjectType == ObjectTypeList.m_Total)
		{
			return false;
		}
		if (TileType == Tile.TileType.IronSoil && HeldObjectType == ObjectType.ToolPickStone)
		{
			return false;
		}
		if (TileType == Tile.TileType.IronSoil2 && (HeldObjectType == ObjectType.ToolPickStone || HeldObjectType == ObjectType.ToolPick))
		{
			return false;
		}
		if (TileType == Tile.TileType.StoneSoil && HeldObjectType == ObjectType.ToolPickStone)
		{
			return false;
		}
		if (TileType == Tile.TileType.ClaySoil && HeldObjectType == ObjectType.ToolPickStone)
		{
			return false;
		}
		if (TileType == Tile.TileType.CoalSoil && HeldObjectType == ObjectType.ToolPickStone)
		{
			return false;
		}
		if (TileType == Tile.TileType.CoalSoil2 && (HeldObjectType == ObjectType.ToolPickStone || HeldObjectType == ObjectType.ToolPick))
		{
			return false;
		}
		return true;
	}

	public override void StartState()
	{
		base.StartState();
		m_BlockSoil = null;
		m_OldTileType = Tile.TileType.Total;
		Actionable currentObject = m_Farmer.m_FarmerAction.m_CurrentObject;
		if ((bool)currentObject && (bool)currentObject.GetComponent<Plot>())
		{
			m_TileCoord = m_Farmer.m_TileCoord;
			m_FinalPosition = m_Farmer.transform.position;
			GetFinalRotationTile();
			StandInTargetTile();
			m_TargetTile = TileManager.Instance.GetTile(m_Farmer.m_GoToTilePosition);
			Tile.TileType tileType = m_TargetTile.m_TileType;
			if (Tile.m_TileInfo[(int)tileType].m_CanReveal && (bool)GetHeldObject() && GetWillToolMineDown(GetHeldObject().m_TypeIdentifier, m_TargetTile.m_TileType))
			{
				m_OldTileHeight = TileManager.Instance.GetTileHeight(m_Farmer.m_GoToTilePosition);
				m_BlockSoil = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.BlockSoil, m_Farmer.m_GoToTilePosition.ToWorldPositionTileCentered(), Quaternion.identity).GetComponent<BlockSoil>();
				m_BlockSoil.SetTile(m_Farmer.m_GoToTilePosition);
				UpdateBlockSoil();
				m_OldTileType = tileType;
				m_Farmer.m_Plot.MineTile(m_Farmer.m_GoToTilePosition, m_Farmer);
				m_UseTarget = false;
				m_Farmer.transform.position = m_Farmer.m_FinalPosition;
			}
			string text = "";
			if (tileType == Tile.TileType.IronSoil)
			{
				text = "IronSoilChance";
			}
			if (tileType == Tile.TileType.IronSoil2)
			{
				text = "IronSoil2Chance";
			}
			if (tileType == Tile.TileType.CoalSoil)
			{
				text = "CoalSoilChance";
			}
			if (tileType == Tile.TileType.CoalSoil2)
			{
				text = "CoalSoil2Chance";
			}
			if (tileType == Tile.TileType.CoalSoil3)
			{
				text = "CoalSoil3Chance";
			}
			if (tileType == Tile.TileType.StoneSoil)
			{
				text = "StoneSoilChance";
			}
			if (text != "")
			{
				int variableAsInt = VariableManager.Instance.GetVariableAsInt(text);
				m_NumActionCounts = (int)(100f / (float)variableAsInt * (float)m_NumActionCounts);
			}
		}
		else
		{
			m_Farmer.m_GoToTilePosition = m_Farmer.m_TileCoord;
			FaceTowardsTarget();
			m_Farmer.transform.position = m_Farmer.m_FinalPosition - m_ActionDelta * Tile.m_Size * 1.75f;
		}
		m_Farmer.StartAnimation("FarmerMining");
	}

	public override void AbortAction()
	{
		base.AbortAction();
		if (m_OldTileType != Tile.TileType.Total)
		{
			TileManager.Instance.SetTileType(m_Farmer.m_GoToTilePosition, m_OldTileType, m_Farmer);
		}
	}

	public override void EndState()
	{
		base.EndState();
		Actionable currentObject = m_Farmer.m_FarmerAction.m_CurrentObject;
		if ((bool)currentObject && (bool)currentObject.GetComponent<Plot>())
		{
			m_Farmer.SetTilePosition(m_TileCoord);
			m_Farmer.m_FinalPosition = m_FinalPosition;
			StandInOldTile();
		}
		else
		{
			StandInTargetTile();
		}
		if ((bool)m_BlockSoil)
		{
			m_BlockSoil.StopUsing();
		}
	}

	protected override void SendEvents(Actionable TargetObject)
	{
		if (m_Tool.m_TypeIdentifier == ObjectType.ToolPickStone)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.MineWithPickaxeCrude, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
		}
		if (m_Tool.m_TypeIdentifier == ObjectType.ToolPick)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.UsePickaxe, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
		}
		if (m_Tool.m_TypeIdentifier == ObjectType.Rock)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.BashBoulderWithRock, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
		}
		if ((bool)TargetObject && TargetObject.m_TypeIdentifier == ObjectType.Boulder)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.MineStone, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, null);
		}
		QuestManager.Instance.AddEvent(QuestEvent.Type.UseObject, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
		BadgeManager.Instance.AddEvent(BadgeEvent.Type.Mining);
	}

	private void UpdateBlockSoil()
	{
		if ((bool)m_BlockSoil)
		{
			float num = (float)m_ActionCount / (float)m_NumActionCounts;
			Vector3 position = m_BlockSoil.transform.position;
			position.y = num * (TileManager.Instance.GetTileHeight(m_Farmer.m_GoToTilePosition) - m_OldTileHeight) + m_OldTileHeight;
			m_BlockSoil.transform.position = position;
		}
	}

	public override void DoAnimationAction()
	{
		base.DoAnimationAction();
		UpdateBlockSoil();
		Actionable targetObject = GetTargetObject();
		if (targetObject != null)
		{
			Vector3 position = m_Farmer.m_FinalPosition;
			if ((bool)targetObject && targetObject.GetComponent<Plot>() == null)
			{
				position = targetObject.transform.position;
			}
			position += m_ActionDelta * Tile.m_Size * 0.75f;
			if ((bool)m_BlockSoil)
			{
				m_Farmer.CreateParticles(position, "Dig");
			}
			else
			{
				m_Farmer.CreateParticles(position, "Pickaxe");
			}
		}
	}
}
