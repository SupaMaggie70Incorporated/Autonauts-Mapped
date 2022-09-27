using cakeslice;
using UnityEngine;

public class BaseClass : MonoBehaviour
{
	[HideInInspector]
	public ObjectType m_TypeIdentifier;

	public int m_UniqueID;

	[HideInInspector]
	public GameObject m_ModelRoot;

	[HideInInspector]
	public string m_ModelName;

	protected void Awake()
	{
		m_UniqueID = -1;
	}

	public virtual void RegisterClass()
	{
		ClassManager.Instance.RegisterClass("BaseClass", m_TypeIdentifier);
	}

	public bool DoesContainClass(string ClassName)
	{
		return ClassManager.Instance.GetObjectTypeUsesClass(ClassName, m_TypeIdentifier);
	}

	public virtual void StopUsing(bool AndDestroy = true)
	{
		if (m_UniqueID != -1)
		{
			if ((bool)CollectionManager.Instance)
			{
				CollectionManager.Instance.RemoveCollectable(this);
			}
			if ((bool)BaggedManager.Instance)
			{
				BaggedManager.Instance.Remove(this);
			}
			ObjectTypeList.Instance.RemoveActionable(this);
			if ((bool)AddAnimationManager.Instance)
			{
				AddAnimationManager.Instance.Remove(this);
			}
			if ((bool)SpawnAnimationManager.Instance)
			{
				SpawnAnimationManager.Instance.Remove(this);
			}
			if (AndDestroy)
			{
				TestObject(this);
				Delete();
			}
		}
	}

	public static bool TestSpawningObject(BaseClass TestClass, BaseClass OldOwner, BaseClass NewOwner)
	{
		if (SpawnAnimationManager.Instance.GetIsObjectSpawning(TestClass))
		{
			SpawnAnimationJump spawnAnimationJump = (SpawnAnimationJump)SpawnAnimationManager.Instance.GetAnimation(TestClass);
			TileCoord tileCoord = default(TileCoord);
			ObjectType objectType = ObjectTypeList.m_Total;
			int num = 0;
			if (NewOwner != null)
			{
				tileCoord = NewOwner.GetComponent<TileCoordObject>().m_TileCoord;
				objectType = NewOwner.m_TypeIdentifier;
				num = NewOwner.m_UniqueID;
			}
			TileCoord tileCoord2 = default(TileCoord);
			ObjectType objectType2 = ObjectTypeList.m_Total;
			int num2 = 0;
			if ((bool)spawnAnimationJump.m_Spawner)
			{
				tileCoord2 = spawnAnimationJump.m_Spawner.GetComponent<TileCoordObject>().m_TileCoord;
				num2 = spawnAnimationJump.m_Spawner.m_UniqueID;
				objectType2 = spawnAnimationJump.m_Spawner.m_TypeIdentifier;
			}
			TileCoord tileCoord3 = default(TileCoord);
			ObjectType objectType3 = ObjectTypeList.m_Total;
			int num3 = 0;
			if ((bool)spawnAnimationJump.m_Target)
			{
				tileCoord3 = spawnAnimationJump.m_Target.GetComponent<TileCoordObject>().m_TileCoord;
				num3 = spawnAnimationJump.m_Target.m_UniqueID;
				objectType3 = spawnAnimationJump.m_Target.m_TypeIdentifier;
			}
			string text = "";
			text = string.Concat("Object ", TestClass.m_TypeIdentifier, ":", TestClass.m_UniqueID);
			text = string.Concat(text, " attempting add to ", objectType, " ", tileCoord.x, ",", tileCoord.y, " ", num, " at ", TimeManager.Instance.m_Frame);
			if (OldOwner != null)
			{
				text = string.Concat(text, "\nby ", OldOwner.m_TypeIdentifier, ":", OldOwner.m_UniqueID);
			}
			text = string.Concat(text, "\nalready being added to ", objectType3, " ", tileCoord3.x, ",", tileCoord3.y, " ", num3);
			text = string.Concat(text, "\nby ", objectType2, " ", tileCoord2.x, ",", tileCoord2.y, " ", num2, " at ", spawnAnimationJump.m_Frame);
			return true;
		}
		return false;
	}

	public static void TestObject(BaseClass TestClass, BaseClass Owner = null, BaseClass NewOwner = null)
	{
	}

	public virtual void Delete()
	{
		m_UniqueID = -1;
		if (ObjectTypeList.Instance.GetReusableFromIdentifier(m_TypeIdentifier))
		{
			InstantiationManager.Instance.ReturnBaseClassObject(this);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	protected virtual void Start()
	{
	}

	public virtual void Restart()
	{
		int num = ObjectTypeList.Instance.AddActionable(this);
		if (num != -1)
		{
			m_UniqueID = num;
		}
		else if (!ObjectTypeList.m_Loading)
		{
			ErrorMessage.LogError(string.Concat(m_TypeIdentifier, " Restart UID -1"));
		}
		base.gameObject.SetActive(value: true);
	}

	public virtual void PostCreate()
	{
	}

	public virtual void WorldCreated()
	{
	}

	public virtual string GetHumanReadableName()
	{
		if (m_TypeIdentifier >= ObjectType.Total && ModManager.Instance.m_ModStrings.ContainsKey(m_TypeIdentifier))
		{
			string text = ModManager.Instance.m_ModStrings[m_TypeIdentifier];
			if ((bool)TextManager.Instance && TextManager.Instance.DoesExist("M_" + text))
			{
				return TextManager.Instance.Get("M_" + text);
			}
			return text;
		}
		string saveNameFromIdentifier = ObjectTypeList.Instance.GetSaveNameFromIdentifier(m_TypeIdentifier);
		return TextManager.Instance.Get(saveNameFromIdentifier);
	}

	public virtual Sprite GetIcon()
	{
		return IconManager.Instance.GetIcon(m_TypeIdentifier);
	}

	public virtual string GetCheatRolloverText()
	{
		return GetHumanReadableName();
	}

	public void CleanHighlight()
	{
		Outline[] componentsInChildren = GetComponentsInChildren<Outline>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Object.Destroy(componentsInChildren[i]);
		}
	}

	public static int GetTierFromType(ObjectType NewType)
	{
		if (NewType == ObjectTypeList.m_Total)
		{
			return 0;
		}
		if (FolkHeart.GetIsFolkHeart(NewType))
		{
			return FolkHeart.GetTierFromObjectType(NewType);
		}
		return ObjectTypeList.Instance.GetTier(NewType);
	}

	public static bool GetHasTierFromType(ObjectType NewType)
	{
		if (!Food.GetIsTypeFood(NewType) && !Top.GetIsTypeTop(NewType) && !FolkHeart.GetIsFolkHeart(NewType) && !Toy.GetIsTypeToy(NewType) && !Medicine.GetIsTypeMedicine(NewType) && !Education.GetIsTypeEducation(NewType) && !Art.GetIsTypeArt(NewType))
		{
			return false;
		}
		return true;
	}

	public static float GetTierScale(int Tier)
	{
		return 0.75f + (float)Tier * 0.5f;
	}
}
