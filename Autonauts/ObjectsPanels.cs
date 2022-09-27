using System.Collections.Generic;
using UnityEngine;

public class ObjectsPanels : MonoBehaviour
{
	public static ObjectsPanels Instance;

	private ObjectCategoryPanel m_ObjectCategories;

	private ObjectSubCategoryPanel m_ObjectSubCatergories;

	private ObjectTypesPanel m_ObjectTypes;

	public ObjectRecipe m_ObjectRecipe;

	private List<ObjectType> m_NavigationStack;

	public static List<ObjectType> m_NewUnlocked;

	public static void AddNewUnlocked(ObjectType NewType)
	{
		m_NewUnlocked.Add(NewType);
	}

	public static void Init()
	{
		m_NewUnlocked = new List<ObjectType>();
	}

	private void Awake()
	{
		Instance = this;
		m_NavigationStack = new List<ObjectType>();
	}

	private void GetDefaultGadgets()
	{
		if (m_ObjectCategories == null)
		{
			m_ObjectCategories = base.transform.Find("ObjectCategoryList").GetComponent<ObjectCategoryPanel>();
			m_ObjectSubCatergories = base.transform.Find("ObjectSubCategoryList").GetComponent<ObjectSubCategoryPanel>();
			m_ObjectTypes = base.transform.Find("ObjectList").GetComponent<ObjectTypesPanel>();
			Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
			GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Autopedia/Objects/ObjectRecipe", typeof(GameObject));
			m_ObjectRecipe = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, menusRootTransform).GetComponent<ObjectRecipe>();
		}
	}

	private void Start()
	{
		GetDefaultGadgets();
		SetSelectedObjectType(GameStateIndustry.m_SelectedObject);
	}

	private void OnDestroy()
	{
		if ((bool)m_ObjectRecipe)
		{
			Object.DestroyImmediate(m_ObjectRecipe.gameObject);
		}
	}

	public void SetObjectCategory(ObjectCategory NewObjectCategory)
	{
		m_ObjectSubCatergories.SetCurrentCategory(NewObjectCategory);
	}

	public void SetObjectSubCategory(ObjectSubCategory NewObjectSubCategory)
	{
		m_ObjectTypes.SetCurrentIndustrySub(NewObjectSubCategory);
	}

	public void SetObjectType(ObjectType NewObjectType, bool UpdateMenus)
	{
		GetDefaultGadgets();
		GameStateIndustry.SetSelectedObject(NewObjectType);
		if (UpdateMenus)
		{
			SetSelectedObjectType(NewObjectType);
		}
		m_ObjectRecipe.SetObjectType(NewObjectType);
		if (NewObjectType != ObjectTypeList.m_Total)
		{
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateIndustry>().ObjectClicked(NewObjectType);
		}
		else
		{
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateIndustry>().ObjectClicked(ObjectTypeList.m_Total);
		}
		Quest questFromUnlockedObject = QuestData.Instance.GetQuestFromUnlockedObject(NewObjectType);
		GameStateIndustry.SetSelectedQuest(questFromUnlockedObject);
		if (questFromUnlockedObject != null)
		{
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateIndustry>().IndustryLevelClicked(questFromUnlockedObject);
		}
		else
		{
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateIndustry>().IndustryLevelClicked((Quest)null);
		}
	}

	public void SetSelectedObjectType(ObjectType NewObjectType)
	{
		GetDefaultGadgets();
		if (NewObjectType != ObjectTypeList.m_Total)
		{
			ObjectSubCategory subCategoryFromType = ObjectTypeList.Instance.GetSubCategoryFromType(NewObjectType);
			ObjectCategory categoryFromSubCategory = ObjectTypeList.Instance.GetCategoryFromSubCategory(subCategoryFromType);
			m_ObjectCategories.SetSelectedObjectCategory(categoryFromSubCategory);
			m_ObjectSubCatergories.SetCurrentCategory(categoryFromSubCategory);
			m_ObjectSubCatergories.SetSelectedIndustrySub(subCategoryFromType);
			m_ObjectTypes.SetCurrentIndustrySub(subCategoryFromType);
			m_ObjectTypes.SetSelectedObjectType(NewObjectType);
		}
	}

	public void HideRecipe()
	{
	}

	public bool IsNavigationBackAvailable()
	{
		return m_NavigationStack.Count != 0;
	}

	public void NavigateDestroy()
	{
		m_NavigationStack.Clear();
	}

	public void NavigateBack()
	{
		ObjectType newObjectType = m_NavigationStack[m_NavigationStack.Count - 1];
		m_NavigationStack.RemoveAt(m_NavigationStack.Count - 1);
		SetObjectType(newObjectType, UpdateMenus: true);
	}

	public void NavigateForward(ObjectType NewType)
	{
		m_NavigationStack.Add(NewType);
	}

	public bool GetCategoryNew(ObjectCategory NewObjectCategory)
	{
		for (int i = 0; i < 49; i++)
		{
			ObjectSubCategory objectSubCategory = (ObjectSubCategory)i;
			if (ObjectTypeList.Instance.GetCategoryFromSubCategory(objectSubCategory) == NewObjectCategory && GetSubCategoryNew(objectSubCategory))
			{
				return true;
			}
		}
		return false;
	}

	public bool GetSubCategoryNew(ObjectSubCategory NewObjectSubCategory)
	{
		for (int i = 0; i < (int)ObjectTypeList.m_Total; i++)
		{
			ObjectType newType = (ObjectType)i;
			if (ObjectTypeList.Instance.GetSubCategoryFromType(newType) == NewObjectSubCategory && GetObjectNew(newType))
			{
				return true;
			}
		}
		return false;
	}

	public bool GetObjectNew(ObjectType NewType)
	{
		return m_NewUnlocked.Contains(NewType);
	}

	public void ObjectSeen(ObjectType NewType)
	{
		if (m_NewUnlocked.Contains(NewType))
		{
			m_NewUnlocked.Remove(NewType);
		}
		m_ObjectTypes.RemoveNew(NewType);
		ObjectSubCategory subCategoryFromType = ObjectTypeList.Instance.GetSubCategoryFromType(NewType);
		if (!GetSubCategoryNew(subCategoryFromType))
		{
			m_ObjectSubCatergories.RemoveNew(subCategoryFromType);
		}
		ObjectCategory categoryFromType = ObjectTypeList.Instance.GetCategoryFromType(NewType);
		if (!GetCategoryNew(categoryFromType))
		{
			m_ObjectCategories.RemoveNew(categoryFromType);
		}
	}

	public float GetHeight()
	{
		return m_ObjectTypes.transform.localPosition.y - m_ObjectTypes.transform.Find("BasePanel").GetComponent<RectTransform>().rect.yMin;
	}
}
