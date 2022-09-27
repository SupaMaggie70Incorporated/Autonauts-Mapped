using UnityEngine;

public class FarmerStateAdding : FarmerStateBase
{
	private bool m_ObjectAnimating;

	public override bool GetIsAdjacentTile(TileCoord TargetTile, BaseClass Object)
	{
		if ((bool)Object && (bool)Object.GetComponent<Building>())
		{
			return false;
		}
		return true;
	}

	public override void StartState()
	{
		base.StartState();
		AddAnimationManager.Instance.Add(m_Farmer, Adding: false);
		if (!m_Farmer.m_FarmerAction.DoAdd())
		{
			m_Farmer.SetState(Farmer.State.None);
			return;
		}
		Actionable @object = m_Farmer.m_FarmerAction.m_CurrentInfo.m_Object;
		if (Converter.GetIsTypeConverter(@object.m_TypeIdentifier) && GetHeldObject() != null)
		{
			Holdable heldObject = GetHeldObject();
			ModManager.Instance.CheckCustomCallback(ModManager.CallbackTypes.AddedToConverter, heldObject.m_TypeIdentifier, heldObject.m_TileCoord, heldObject.m_UniqueID, @object.m_UniqueID);
		}
		float num;
		if ((bool)@object.GetComponent<Building>() && @object.m_TypeIdentifier != ObjectType.ConverterFoundation)
		{
			Building component = @object.GetComponent<Building>();
			num = (float)((component.m_AccessPointRotation + component.m_Rotation + 2) % 4) * 360f / 4f;
		}
		else
		{
			Vector3 vector = @object.transform.position - m_Farmer.transform.position;
			num = (0f - Mathf.Atan2(vector.z, vector.x)) * 57.29578f;
		}
		m_Farmer.transform.rotation = Quaternion.Euler(0f, num - 90f, 0f);
		if (ObjectTypeList.Instance.GetIsAnimateAdd(GetHeldObject()))
		{
			m_Farmer.m_SpawnEnd = false;
			m_ObjectAnimating = true;
		}
		else
		{
			m_ObjectAnimating = false;
		}
	}

	public override void EndState()
	{
		base.EndState();
	}

	public override void SpawnEnd(BaseClass NewObject)
	{
		if (m_Farmer.m_SpawnSuccess)
		{
			UpdateState();
		}
	}

	public override void UpdateState()
	{
		base.UpdateState();
		float num = 0.125f * m_GeneralStateScale;
		if (SaveLoadManager.m_TestBuild && SaveLoadManager.m_Video)
		{
			num = 0.01f;
		}
		if ((!m_ObjectAnimating || !m_Farmer.m_SpawnEnd) && (m_ObjectAnimating || !(m_Farmer.m_StateTimer > num)))
		{
			return;
		}
		BaseClass targetObject = GetTargetObject();
		if ((bool)targetObject)
		{
			BaseClass heldObject = GetHeldObject();
			if ((bool)heldObject && heldObject.m_TypeIdentifier != ObjectType.ToolWateringCan)
			{
				AddAnimationManager.Instance.Add(targetObject, Adding: true);
			}
			if (StoragePalette.GetIsTypeStoragePalette(targetObject.m_TypeIdentifier))
			{
				if (m_Farmer.m_TypeIdentifier == ObjectType.FarmerPlayer)
				{
					QuestManager.Instance.AddEvent(QuestEvent.Type.AddToStoragePalette, Bot: false, 0, m_Farmer);
					if (m_Farmer.GetComponent<FarmerPlayer>().m_Teaching)
					{
						QuestManager.Instance.AddEvent(QuestEvent.Type.TeachAddToStoragePalette, Bot: false, 0, m_Farmer);
					}
				}
				else
				{
					QuestManager.Instance.AddEvent(QuestEvent.Type.AddToStoragePalette, Bot: true, 0, m_Farmer);
				}
			}
			ObjectType lastObjectType = m_Farmer.m_FarmerCarry.GetLastObjectType();
			if (StorageLiquid.GetIsTypeStorageLiquid(targetObject.m_TypeIdentifier))
			{
				QuestManager.Instance.AddEvent(QuestEvent.Type.StoreLiquid, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
			}
			if (StorageSand.GetIsTypeStorageSand(targetObject.m_TypeIdentifier))
			{
				QuestManager.Instance.AddEvent(QuestEvent.Type.StoreParticulate, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
			}
			if (Food.GetIsTypeFood(lastObjectType))
			{
				QuestManager.Instance.AddEvent(QuestEvent.Type.StoreFood, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
			}
			if (m_Farmer.m_TypeIdentifier == ObjectType.FarmerPlayer && targetObject.m_TypeIdentifier == ObjectType.Worker)
			{
				if (lastObjectType == ObjectType.Rock || lastObjectType == ObjectType.ToolAxeStone)
				{
					QuestManager.Instance.AddEvent(QuestEvent.Type.GiveAxeToBot, Bot: false, 0, m_Farmer);
				}
				QuestManager.Instance.AddEvent(QuestEvent.Type.GiveBotAnything, Bot: false, lastObjectType, m_Farmer);
			}
			if (Storage.GetIsTypeStorage(targetObject.m_TypeIdentifier))
			{
				BadgeManager.Instance.AddEvent(BadgeEvent.Type.AnythingStored);
			}
			if (lastObjectType == ObjectType.Folk && targetObject.m_TypeIdentifier == ObjectType.TranscendBuilding)
			{
				QuestManager.Instance.AddEvent(QuestEvent.Type.FolkTranscended, Bot: false, lastObjectType, m_Farmer);
			}
		}
		DoEndAction();
		Holdable topObject = m_Farmer.m_FarmerCarry.GetTopObject();
		if (!topObject)
		{
			return;
		}
		if ((bool)topObject.GetComponent<ToolFillable>())
		{
			if (topObject.GetComponent<ToolFillable>().m_HeldType == ObjectType.Water)
			{
				QuestManager.Instance.AddEvent(QuestEvent.Type.UseWater, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
			}
			if (topObject.m_TypeIdentifier == ObjectType.ToolBucketCrude)
			{
				QuestManager.Instance.AddEvent(QuestEvent.Type.UseBucketCrude, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
			}
			if (topObject.m_TypeIdentifier == ObjectType.ToolBucket)
			{
				QuestManager.Instance.AddEvent(QuestEvent.Type.UseBucket, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
			}
		}
		if ((bool)topObject.GetComponent<ToolFillable>() && ToolFillable.GetIsTypeEmptyable(topObject.m_TypeIdentifier))
		{
			DegradeTool();
		}
	}
}
