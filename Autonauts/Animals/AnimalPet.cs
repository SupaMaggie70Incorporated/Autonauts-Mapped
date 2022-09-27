using SimpleJSON;
using UnityEngine;

public class AnimalPet : Animal
{
	[HideInInspector]
	public GameObject m_HatParent;

	[HideInInspector]
	public Hat m_CurrentHat;

	public static bool GetIsTypePet(ObjectType NewType)
	{
		if (AnimalPetDog.GetIsTypeAnimalPetDog(NewType))
		{
			return true;
		}
		return false;
	}

	public override void Restart()
	{
		base.Restart();
		m_MoveNormalDelay = 0.2f;
		m_CurrentHat = null;
	}

	public override void PostCreate()
	{
		base.PostCreate();
		if ((bool)m_ModelRoot)
		{
			m_HatParent = ObjectUtils.FindDeepChild(m_ModelRoot.transform, "HatPoint").gameObject;
			string aName = "HeadRoot";
			Transform transform = ObjectUtils.FindDeepChild(m_ModelRoot.transform, aName);
			if ((bool)m_HatParent && (bool)transform)
			{
				Quaternion localRotation = Quaternion.Inverse(transform.localRotation) * m_HatParent.transform.localRotation;
				m_HatParent.transform.localRotation = localRotation;
			}
		}
		if ((bool)CollectionManager.Instance)
		{
			CollectionManager.Instance.AddCollectable("Pet", this);
		}
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		SetBaggedObject(null);
		if (m_CurrentHat != null)
		{
			m_CurrentHat.StopUsing();
			m_CurrentHat = null;
		}
		base.StopUsing(AndDestroy);
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		if (m_CurrentHat != null)
		{
			JSONNode jSONNode = new JSONObject();
			string saveNameFromIdentifier = ObjectTypeList.Instance.GetSaveNameFromIdentifier(m_CurrentHat.m_TypeIdentifier);
			JSONUtils.Set(jSONNode, "ID", saveNameFromIdentifier);
			m_CurrentHat.Save(jSONNode);
			Node["Hat"] = jSONNode;
		}
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		JSONNode jSONNode = Node["Hat"];
		if (jSONNode != null && !jSONNode.IsNull)
		{
			JSONNode asObject = jSONNode.AsObject;
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromSaveName(asObject, new Vector3(0f, 0f, 0f), Quaternion.identity);
			if ((bool)baseClass)
			{
				baseClass.GetComponent<Savable>().Load(asObject);
				ApplyHat(baseClass.GetComponent<Hat>());
			}
		}
	}

	public override object GetActionInfo(GetActionInfo Info)
	{
		if (Info.m_Action == GetAction.GetObjectType)
		{
			if (Info.m_Value == AFO.AT.AltPrimary.ToString() && m_CurrentHat != null)
			{
				return m_CurrentHat.m_TypeIdentifier;
			}
			return ObjectTypeList.m_Total;
		}
		return base.GetActionInfo(Info);
	}

	private void StartAddHat(AFO Info)
	{
		Actionable @object = Info.m_Object;
		AudioManager.Instance.StartEvent("ClothingApplied", this);
		ApplyHat(@object.GetComponent<Hat>());
		ModManager.Instance.CheckCustomCallback(ModManager.CallbackTypes.ClothingHatAdded, @object.m_TypeIdentifier, m_TileCoord, @object.m_UniqueID, m_UniqueID);
	}

	private void EndAddHat(AFO Info)
	{
		Actionable @object = Info.m_Object;
		((SpawnAnimationJump)SpawnAnimationManager.Instance.GetAnimation(@object)).m_EndPosition = m_HatParent.transform.position;
	}

	private void AbortAddHat(AFO Info)
	{
		RemoveHat(Jump: false);
	}

	private ActionType GetActionFromHat(AFO Info)
	{
		Info.m_FarmerState = Farmer.State.Adding;
		Info.m_StartAction = StartAddHat;
		Info.m_EndAction = EndAddHat;
		Info.m_AbortAction = AbortAddHat;
		if (m_CurrentHat != null && m_CurrentHat.m_TypeIdentifier == Info.m_ObjectType)
		{
			return ActionType.Fail;
		}
		return ActionType.AddResource;
	}

	public void StartActionNothingAlt(AFO Info)
	{
		Holdable newHoldable = RemoveHat(Jump: false);
		Info.m_Actioner.GetComponent<Farmer>().m_FarmerCarry.TryAddCarry(newHoldable);
	}

	private void AbortAddNothingAlt(AFO Info)
	{
		Holdable lastObject = Info.m_Actioner.GetComponent<Farmer>().m_FarmerCarry.GetLastObject();
		ApplyHat(lastObject.GetComponent<Hat>());
	}

	private ActionType GetActionFromNothingAlt(AFO Info)
	{
		Info.m_StartAction = StartActionNothingAlt;
		Info.m_AbortAction = AbortAddNothingAlt;
		Info.m_FarmerState = Farmer.State.Taking;
		if (m_CurrentHat == null)
		{
			return ActionType.Fail;
		}
		return ActionType.TakeResource;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (Info.m_ActionType == AFO.AT.AltSecondary && Hat.GetIsTypeHat(Info.m_ObjectType))
		{
			return GetActionFromHat(Info);
		}
		if (Info.m_ActionType == AFO.AT.AltPrimary && Info.m_ObjectType == ObjectTypeList.m_Total && (bool)m_CurrentHat)
		{
			return GetActionFromNothingAlt(Info);
		}
		return base.GetActionFromObject(Info);
	}

	private Hat RemoveHat(bool Jump)
	{
		if (m_CurrentHat != null)
		{
			ModManager.Instance.CheckCustomCallback(ModManager.CallbackTypes.ClothingHatRemoved, m_CurrentHat.m_TypeIdentifier, m_TileCoord, m_CurrentHat.m_UniqueID, m_UniqueID);
			m_CurrentHat.Remove();
			TileCoord randomEmptyTile = TileHelpers.GetRandomEmptyTile(m_TileCoord);
			m_CurrentHat.ForceHighlight(Highlighted: false);
			m_CurrentHat.SendAction(new ActionInfo(ActionType.Dropped, randomEmptyTile, this));
			if (Jump)
			{
				float yOffset = ObjectTypeList.Instance.GetYOffset(m_CurrentHat.m_TypeIdentifier);
				SpawnAnimationManager.Instance.AddJump(m_CurrentHat, m_TileCoord, randomEmptyTile, 0f, yOffset, 4f);
			}
			Hat currentHat = m_CurrentHat;
			m_CurrentHat = null;
			return currentHat;
		}
		return null;
	}

	private void ApplyHat(Hat NewHat)
	{
		RemoveHat(Jump: true);
		if ((bool)NewHat)
		{
			m_CurrentHat = NewHat;
			m_CurrentHat.SendAction(new ActionInfo(ActionType.BeingHeld, default(TileCoord), this));
			if ((bool)m_HatParent)
			{
				m_CurrentHat.Wear(base.gameObject, m_HatParent.transform);
			}
			else
			{
				m_CurrentHat.Wear(base.gameObject, base.transform);
			}
		}
	}

	protected FarmerPlayer GetPlayer()
	{
		return CollectionManager.Instance.GetPlayers()[0].GetComponent<FarmerPlayer>();
	}

	protected float GetPlayerRange()
	{
		return (GetPlayer().m_TileCoord - m_TileCoord).Magnitude() * Tile.m_Size;
	}

	protected TileCoord GetTileNearPlayer(float Range)
	{
		TileCoord tileCoord = GetPlayer().m_TileCoord;
		TileCoord result = tileCoord;
		float num = 1E+08f;
		int num2 = (int)(Range / Tile.m_Size);
		for (int i = -num2; i <= num2; i++)
		{
			for (int j = -num2; j <= num2; j++)
			{
				TileCoord tileCoord2 = new TileCoord(j, i) + tileCoord;
				if (tileCoord2.GetIsValid() && !TileManager.Instance.GetTileSolidToPlayer(tileCoord2))
				{
					float num3 = (m_TileCoord - tileCoord2).Magnitude();
					if (num3 < num)
					{
						num = num3;
						result = tileCoord2;
					}
				}
			}
		}
		return result;
	}

	public virtual void PlayerCall()
	{
	}

	protected void FaceTarget(TileCoord TargetTilePosition)
	{
		if (m_TileCoord != TargetTilePosition)
		{
			Vector3 vector = TargetTilePosition.ToWorldPositionTileCentered() - base.transform.position;
			float num = 0f - Mathf.Atan2(vector.z, vector.x);
			base.transform.localRotation = Quaternion.Euler(0f, num * 57.29578f - 90f, 0f);
		}
	}

	protected void FaceTarget(BaseClass Target)
	{
		FaceTarget(Target.GetComponent<TileCoordObject>().m_TileCoord);
	}

	protected void FaceTarget(int ObjectUID)
	{
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(ObjectUID, ErrorCheck: false);
		if ((bool)objectFromUniqueID)
		{
			FaceTarget(objectFromUniqueID.GetComponent<TileCoordObject>().m_TileCoord);
		}
	}
}
