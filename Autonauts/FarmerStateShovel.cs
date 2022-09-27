using UnityEngine;

public class FarmerStateShovel : FarmerStateTool
{
	private BaseClass m_SoilHole;

	private float m_SoilAnimateTimer;

	private bool m_FillHole;

	private BaseClass m_Turf;

	public FarmerStateShovel()
	{
		m_ActionSoundName = "ToolShovelDig";
		m_NoToolIconName = "GenIcons/GenIconToolShovelMetal";
		m_AdjacentTile = true;
	}

	public override bool GetIsAdjacentTile(TileCoord TargetTile, BaseClass Object)
	{
		if ((bool)Object && Object.m_TypeIdentifier == ObjectType.Hedge)
		{
			return false;
		}
		return true;
	}

	public static bool GetIsToolAcceptable(ObjectType NewType)
	{
		return ToolShovel.GetIsTypeShovel(NewType);
	}

	public override bool IsToolAcceptable(Holdable NewObject)
	{
		if (NewObject == null)
		{
			return false;
		}
		return GetIsToolAcceptable(NewObject.m_TypeIdentifier);
	}

	public override void StartState()
	{
		base.StartState();
		if ((bool)GetTargetObject() && GetTargetObject().m_TypeIdentifier == ObjectType.Hedge)
		{
			m_Farmer.m_GoToTilePosition = m_Farmer.m_TileCoord;
			FaceTowardsTarget();
			m_Farmer.m_FinalPosition = GetTargetObject().GetComponent<TileCoordObject>().m_TileCoord.ToWorldPositionTileCentered();
			m_Farmer.transform.position = m_Farmer.m_FinalPosition + m_ActionDelta * Tile.m_Size * 1.35f;
		}
		else
		{
			GetFinalRotationTile();
			StandInTargetTile();
			float num = m_ToolHeight - 0.25f;
			num += m_Farmer.m_FarmerCarry.m_ToolCarryDistance;
			Vector3 vector = m_Farmer.transform.position - m_Farmer.transform.TransformPoint(new Vector3(0f, 0f, num));
			m_Farmer.transform.position -= vector;
		}
		m_Farmer.StartAnimation("FarmerShovel");
		m_SoilHole = null;
		m_Turf = null;
		m_SoilAnimateTimer = 0f;
		if (!GetTargetObject() || GetTargetObject().m_TypeIdentifier != ObjectType.Plot)
		{
			return;
		}
		Tile tile = TileManager.Instance.GetTile(m_Farmer.m_GoToTilePosition);
		Tile.TileType tileType = tile.m_TileType;
		if (tile.m_TileType == Tile.TileType.Soil || tile.m_TileType == Tile.TileType.SoilHole)
		{
			if (tile.m_AssociatedObject != null && tile.m_AssociatedObject.m_TypeIdentifier == ObjectType.TreeStump)
			{
				m_TargetObjectUID = tile.m_AssociatedObject.m_UniqueID;
				m_TargetObjectType = tile.m_AssociatedObject.m_TypeIdentifier;
			}
			else if (tile.m_TileType == Tile.TileType.Soil)
			{
				m_SoilHole = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.SoilHolePile, m_Farmer.m_FinalPosition, Quaternion.identity);
				m_SoilHole.transform.localScale = new Vector3(1f, 0.05f, 1f);
				m_FillHole = false;
			}
			else
			{
				m_SoilHole = tile.m_AssociatedObject;
				m_FillHole = true;
			}
		}
		else if (tile.m_TileType == Tile.TileType.Empty || tile.m_TileType == Tile.TileType.IronHidden || tile.m_TileType == Tile.TileType.ClayHidden || tile.m_TileType == Tile.TileType.StoneHidden || tile.m_TileType == Tile.TileType.CoalHidden)
		{
			m_Turf = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.TurfPile, m_Farmer.m_FinalPosition + new Vector3(0f, -0.4f, 0f), Quaternion.Euler(0f, -90f, 0f));
			m_Turf.transform.localScale = new Vector3(1f, 1f, 1f);
		}
		string text = "";
		if (tileType == Tile.TileType.ClaySoil)
		{
			text = "ClaySoilChance";
		}
		if (text != "")
		{
			int variableAsInt = VariableManager.Instance.GetVariableAsInt(text);
			m_NumActionCounts = (int)(100f / (float)variableAsInt * (float)m_NumActionCounts);
		}
	}

	public override void EndState()
	{
		base.EndState();
		StandInTargetTile();
		if ((bool)m_SoilHole)
		{
			m_SoilHole.transform.localScale = new Vector3(1f, 1f, 1f);
			if (!m_FillHole)
			{
				m_SoilHole.StopUsing();
			}
		}
		if ((bool)m_Turf)
		{
			m_Turf.StopUsing();
		}
	}

	protected override void SendEvents(Actionable TargetObject)
	{
		BaseClass topObject = m_Farmer.m_FarmerCarry.GetTopObject();
		if ((bool)topObject && (bool)TargetObject && TargetObject.m_TypeIdentifier == ObjectType.Plot)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.Shovel, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
			if (topObject.m_TypeIdentifier == ObjectType.ToolShovelStone)
			{
				QuestManager.Instance.AddEvent(QuestEvent.Type.ShovelWithShovelCrude, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
			}
			else
			{
				QuestManager.Instance.AddEvent(QuestEvent.Type.ShovelWithShovel, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
			}
		}
		if ((bool)TargetObject && TargetObject.m_TypeIdentifier == ObjectType.TreeStump)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.Shovel, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
		}
		if ((bool)TargetObject && TargetObject.m_TypeIdentifier == ObjectType.Weed)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.DigWeed, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
		}
		if ((bool)TargetObject && TargetObject.m_TypeIdentifier == ObjectType.Mushroom)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.DigMushroom, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
		}
		if ((bool)TargetObject && TargetObject.m_TypeIdentifier == ObjectType.CropCarrot)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.DigCarrot, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
		}
		QuestManager.Instance.AddEvent(QuestEvent.Type.Dig, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
		QuestManager.Instance.AddEvent(QuestEvent.Type.UseObject, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
		Tile.TileType tileType = TileManager.Instance.GetTile(m_Farmer.m_GoToTilePosition).m_TileType;
		if (tileType == Tile.TileType.Soil)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.DigSoil, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
		}
		if (tileType == Tile.TileType.ClaySoil || tileType == Tile.TileType.Clay)
		{
			BadgeManager.Instance.AddEvent(BadgeEvent.Type.Mining);
		}
	}

	public override void DoAnimationAction()
	{
		base.DoAnimationAction();
		m_Farmer.CreateParticles(m_Farmer.m_FinalPosition, "Dig");
		float num = GetActionPercentDone();
		if ((bool)m_SoilHole)
		{
			if (m_FillHole)
			{
				num = 1f - num;
			}
			if (num <= 0f)
			{
				num = 0.01f;
			}
			m_SoilHole.transform.localScale = new Vector3(1.25f, num, 1.25f);
			m_SoilAnimateTimer = 0.25f;
		}
		if ((bool)m_Turf)
		{
			m_Turf.transform.position = m_Farmer.m_FinalPosition + new Vector3(0f, -0.4f + 0.4f * num, 0f);
			m_SoilAnimateTimer = 0.25f;
		}
	}

	public override void UpdateState()
	{
		base.UpdateState();
		if ((bool)m_SoilHole && m_SoilAnimateTimer > 0f)
		{
			m_SoilAnimateTimer -= TimeManager.Instance.m_NormalDelta;
			if (m_SoilAnimateTimer <= 0f)
			{
				m_SoilAnimateTimer = 0f;
				float num = GetActionPercentDone();
				if (m_FillHole)
				{
					num = 1f - num;
				}
				if (num <= 0f)
				{
					num = 0.01f;
				}
				m_SoilHole.transform.localScale = new Vector3(1f, num, 1f);
			}
		}
		if (!m_Turf || !(m_SoilAnimateTimer > 0f))
		{
			return;
		}
		m_SoilAnimateTimer -= TimeManager.Instance.m_NormalDelta;
		if (m_SoilAnimateTimer <= 0f)
		{
			m_SoilAnimateTimer = 0f;
			if ((bool)m_Turf)
			{
				m_Turf.transform.position = m_Farmer.m_FinalPosition + new Vector3(0f, -0.4f, 0f);
			}
		}
	}
}
