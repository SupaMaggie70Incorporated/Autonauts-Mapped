using UnityEngine;

public class StorageGeneric : Storage
{
	private Transform m_Hinge;

	public static bool GetIsTypeStorageGeneric(ObjectType NewType)
	{
		if (NewType == ObjectType.StorageGeneric || NewType == ObjectType.StorageGenericMedium)
		{
			return true;
		}
		return false;
	}

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(0, 0), new TileCoord(0, 0), new TileCoord(0, 1));
		m_ObjectType = ObjectTypeList.m_Total;
	}

	protected new void Awake()
	{
		base.Awake();
		m_MaxLevels = 3;
		m_NumLevels = 1;
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_Hinge = ObjectUtils.FindDeepChild(m_ModelRoot.transform, "Hinge");
		m_Sign = ObjectUtils.FindDeepChild(m_ModelRoot.transform, "Plane").GetComponent<MeshRenderer>();
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		base.StopUsing(AndDestroy);
		m_ObjectType = ObjectTypeList.m_Total;
	}

	public override bool GetNewLevelAllowed(Building NewBuilding)
	{
		return GetNewLevelAllowedStacked(NewBuilding);
	}

	public override void SetObjectType(ObjectType NewType)
	{
		if (!StorageTypeManager.m_StorageGenericInformation.ContainsKey(NewType))
		{
			NewType = ObjectTypeList.m_Total;
		}
		base.SetObjectType(NewType);
		if (m_ObjectType != ObjectTypeList.m_Total)
		{
			ResourceManager.Instance.UnRegisterStorage(this);
		}
		m_Capacity = 0;
		if (NewType != ObjectTypeList.m_Total)
		{
			m_Capacity = StorageTypeManager.m_StorageGenericInformation[NewType].m_Capacity;
		}
		SetSign(NewType);
	}

	protected override bool CanAcceptObject(BaseClass NewObject, ObjectType NewType)
	{
		if (!StorageTypeManager.m_StorageGenericInformation.ContainsKey(NewType))
		{
			return false;
		}
		if (m_TypeIdentifier == ObjectType.StorageGeneric && ObjectTypeList.Instance.GetTier(NewType) >= 5)
		{
			return false;
		}
		int stored = GetStored(CheckStack: true, CheckReserved: false);
		int capacity = GetCapacity();
		if (stored > 0 && stored >= capacity)
		{
			if (m_ObjectType == ObjectTypeList.m_Total)
			{
				SetObjectType(NewType);
				if (stored + 1 <= capacity + GetCapacity())
				{
					return true;
				}
			}
			return false;
		}
		if (m_ObjectType == ObjectTypeList.m_Total)
		{
			return true;
		}
		if (stored == 0)
		{
			return true;
		}
		if (NewType != m_ObjectType)
		{
			return false;
		}
		return true;
	}

	public override void SetBlueprint(bool Blueprint, bool Register = false)
	{
		base.SetBlueprint(Blueprint, Register);
		Transform transform = ObjectUtils.FindDeepChild(base.transform, "Crate");
		if ((bool)transform)
		{
			Material[] materials = transform.GetComponent<MeshRenderer>().materials;
			for (int i = 0; i < materials.Length; i++)
			{
				materials[i].renderQueue--;
			}
		}
	}

	public override void StartOpenLid()
	{
		base.StartOpenLid();
		m_Hinge.localRotation = Quaternion.Euler(-20f, 0f, 180f);
	}

	public override void CloseLid()
	{
		base.CloseLid();
		m_Hinge.localRotation = Quaternion.Euler(-90f, 0f, 180f);
	}
}
