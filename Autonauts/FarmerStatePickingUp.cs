using UnityEngine;

public class FarmerStatePickingUp : FarmerStateBase
{
	private bool m_Bucket;

	private Quaternion m_Rotation;

	private Vector3 m_BucketPosition;

	private bool m_Pitchfork;

	private float m_Delay;

	private int m_Count;

	private int m_ActionCount;

	private SandPile m_SandPile;

	public override bool GetIsAdjacentTile(TileCoord TargetTile, BaseClass Object)
	{
		Holdable topObject = m_Farmer.m_FarmerCarry.GetTopObject();
		if ((bool)topObject && (bool)topObject.GetComponent<ToolFillable>())
		{
			return true;
		}
		return false;
	}

	public override void StartState()
	{
		base.StartState();
		m_Bucket = false;
		m_Pitchfork = false;
		Holdable topObject = m_Farmer.m_FarmerCarry.GetTopObject();
		if ((bool)topObject && (bool)topObject.GetComponent<ToolFillable>())
		{
			m_Bucket = true;
			if (topObject.m_TypeIdentifier == ObjectType.ToolPitchfork)
			{
				topObject.GetComponent<ToolPitchfork>().StartUsing();
				m_Farmer.StartAnimation("FarmerPitchforkPickup");
				m_Pitchfork = true;
			}
			else
			{
				m_Farmer.StartAnimation("FarmerFillablePickup");
			}
			m_Count = 0;
			m_ActionCount = VariableManager.Instance.GetVariableAsInt(topObject.m_TypeIdentifier, "FillScoops");
			FaceTowardsTargetTile();
		}
		else
		{
			m_Rotation = m_Farmer.transform.rotation;
			m_Farmer.transform.rotation = m_Rotation * Quaternion.Euler(-30f, 0f, 0f);
		}
		m_Delay = 0.125f * m_GeneralStateScale;
		if (m_Bucket)
		{
			m_Delay *= 2f;
		}
		if (m_Farmer.m_TypeIdentifier == ObjectType.FarmerPlayer)
		{
			Actionable currentObject = m_Farmer.m_FarmerAction.m_CurrentObject;
			if ((bool)currentObject && (bool)currentObject.GetComponent<Holdable>())
			{
				m_Farmer.GetComponent<FarmerPlayer>();
				if (currentObject.GetComponent<Holdable>().GetIsHeavyForPlayer())
				{
					m_Delay = 0.5f;
				}
			}
		}
		Actionable targetObject = GetTargetObject();
		m_SandPile = null;
		if ((bool)targetObject && targetObject.m_TypeIdentifier == ObjectType.Plot && TileManager.Instance.GetTile(m_Farmer.m_GoToTilePosition).m_TileType == Tile.TileType.Sand)
		{
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.SandPile, m_Farmer.m_GoToTilePosition.ToWorldPositionTileCentered(), Quaternion.identity);
			m_SandPile = baseClass.GetComponent<SandPile>();
		}
		if (SaveLoadManager.m_TestBuild && SaveLoadManager.m_Video)
		{
			m_Delay = 0.01f;
		}
	}

	public override void EndState()
	{
		base.EndState();
		Holdable topObject = m_Farmer.m_FarmerCarry.GetTopObject();
		if ((bool)topObject && topObject.m_TypeIdentifier == ObjectType.ToolPitchfork)
		{
			topObject.GetComponent<ToolPitchfork>().StopUsing();
		}
		if (!m_Bucket)
		{
			m_Farmer.transform.rotation = m_Rotation;
		}
		else
		{
			StandInOldTile();
		}
		if ((bool)m_SandPile)
		{
			m_SandPile.Melt();
		}
	}

	private void EndPickup()
	{
		if (m_Farmer.m_FarmerAction.DoPickup())
		{
			PlaySound playSound = AudioManager.Instance.StartEvent("FarmerPickUp", m_Farmer, Remember: true);
			float pitch = 1f + (float)m_Farmer.m_FarmerCarry.GetCarryCount() * 0.25f;
			if (playSound != null && playSound.m_Result != null)
			{
				playSound.m_Result.ActingVariation.VarAudio.pitch = pitch;
			}
			Holdable topObject = m_Farmer.m_FarmerCarry.GetTopObject();
			if (topObject.m_TypeIdentifier == ObjectType.ToolPitchfork && topObject.GetComponent<ToolPitchfork>().GetIsFull())
			{
				TileCoord tileCoord = m_Farmer.m_FarmerAction.m_CurrentObject.GetComponent<TileCoordObject>().m_TileCoord;
				topObject.GetComponent<ToolPitchfork>().CheckFull(tileCoord, m_Farmer);
				DegradeTool();
			}
			Actionable @object = m_Farmer.m_FarmerAction.m_CurrentInfo.m_Object;
			if ((bool)@object)
			{
				ObjectType objectType = @object.m_TypeIdentifier;
				if ((bool)@object.GetComponent<ToolFillable>())
				{
					objectType = @object.GetComponent<ToolFillable>().m_LastHeldType;
				}
				QuestManager.Instance.AddEvent(QuestEvent.Type.Pickup, m_Farmer.m_TypeIdentifier == ObjectType.Worker, objectType, m_Farmer);
				QuestManager.Instance.AddEvent(QuestEvent.Type.PickupAnything, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
			}
			if (m_Bucket)
			{
				Actionable targetObject = GetTargetObject();
				if ((bool)targetObject && targetObject.m_TypeIdentifier == ObjectType.Plot && TileManager.Instance.GetTile(m_Farmer.m_GoToTilePosition).m_TileType == Tile.TileType.Sand)
				{
					BadgeManager.Instance.AddEvent(BadgeEvent.Type.Mining);
				}
				m_Farmer.SetBaggedTile(new TileCoord(0, 0));
			}
		}
		DoEndAction();
	}

	public override void DoAnimationFunction()
	{
		base.DoAnimationFunction();
		if (m_Pitchfork)
		{
			AudioManager.Instance.StartEvent("ToolPitchforkUse", m_Farmer);
		}
	}

	public override void DoAnimationAction()
	{
		base.DoAnimationAction();
		m_Count++;
		if (m_Count == m_ActionCount)
		{
			EndPickup();
		}
		if ((bool)m_SandPile)
		{
			float progress = (float)m_Count / (float)m_ActionCount;
			m_SandPile.SetProgress(progress);
		}
	}

	public override void UpdateState()
	{
		base.UpdateState();
		if (!m_Bucket && m_Farmer.m_StateTimer > m_Delay)
		{
			EndPickup();
		}
	}
}
