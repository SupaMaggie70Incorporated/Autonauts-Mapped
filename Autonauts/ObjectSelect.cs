using System.Collections.Generic;
using UnityEngine;

public class ObjectSelect : BaseMenu
{
	private List<ObjectCategory> m_Categories;

	protected List<ObjectInfo> m_ObjectList;

	public ObjectCategory m_CurrentCategory;

	public ButtonList m_CategoryButtonList;

	private BaseScrollView m_ScrollView;

	private ButtonList m_ObjectButtonList;

	private List<BaseButtonImage> m_CategoryButtons;

	protected List<BaseButtonImage> m_ObjectButtons;

	private bool m_Everything;

	private float m_FlashTimer;

	private int m_FlashIndex;

	private ObjectType m_ScrollToObject;

	private bool GetIsCategoryVisible(ObjectCategory NewCategory, bool JustBuildings, bool Everything)
	{
		if (!JustBuildings || Everything)
		{
			switch (NewCategory)
			{
			case ObjectCategory.Hidden:
			case ObjectCategory.Any:
			case ObjectCategory.Effects:
				return false;
			case ObjectCategory.Buildings:
				if (!Everything)
				{
					return false;
				}
				break;
			}
		}
		else if (NewCategory != ObjectCategory.Buildings)
		{
			return false;
		}
		if (NewCategory == ObjectCategory.Prizes && !QuestManager.Instance.GetQuestComplete(Quest.ID.GlueSpacePort))
		{
			return false;
		}
		return true;
	}

	protected new void Awake()
	{
		base.Awake();
		m_CurrentCategory = ObjectCategory.Total;
		CheckGadgets();
		UpdateHeight();
	}

	protected new void Start()
	{
		base.Start();
		CheckGadgets();
		UpdateHeight();
	}

	private void CheckGadgets()
	{
		m_CategoryButtonList = base.transform.Find("BasePanelOptions").Find("CategoryButtonList").GetComponent<ButtonList>();
		m_ScrollView = base.transform.Find("BasePanelOptions").Find("BaseScrollView").GetComponent<BaseScrollView>();
		m_ObjectButtonList = m_ScrollView.GetContent().transform.Find("ButtonList").GetComponent<ButtonList>();
	}

	public virtual void Init(bool JustBuildings, bool Everything)
	{
		m_Everything = Everything;
		Init();
		CheckGadgets();
		m_CategoryButtons = new List<BaseButtonImage>();
		m_ObjectButtons = new List<BaseButtonImage>();
		m_Categories = new List<ObjectCategory>();
		for (int i = 0; i < 17; i++)
		{
			ObjectCategory objectCategory = (ObjectCategory)i;
			if (GetIsCategoryVisible(objectCategory, JustBuildings, Everything))
			{
				m_Categories.Add(objectCategory);
			}
		}
		m_CategoryButtonList.m_CreateObjectCallback = OnCreateCategoryButton;
		m_CategoryButtonList.m_ObjectCount = m_Categories.Count;
		m_CategoryButtonList.CreateButtons();
		SetCategory(m_Categories[0]);
	}

	public void OnCreateCategoryButton(GameObject NewObject, int Index)
	{
		ObjectCategory newCategory = m_Categories[Index];
		string categorySprite = ObjectTypeList.Instance.GetCategorySprite(newCategory);
		BaseButtonImage component = NewObject.GetComponent<BaseButtonImage>();
		component.SetSprite(categorySprite);
		string categoryName = ObjectTypeList.Instance.GetCategoryName(newCategory);
		component.SetRolloverFromID(categoryName);
		m_CategoryButtons.Add(component);
		RegisterGadget(component);
		AddAction(component, OnCategoryClicked);
	}

	public virtual void OnCategoryClicked(BaseGadget NewGadget)
	{
		int index = m_CategoryButtons.IndexOf(NewGadget.GetComponent<BaseButtonImage>());
		ObjectCategory objectCategory = m_Categories[index];
		SetCategory(objectCategory);
		if (objectCategory == ObjectCategory.Food)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.SelectAutopediaFood, Bot: false, null, null);
		}
		if ((bool)Objects.Instance)
		{
			Objects.Instance.UpdateCategory();
		}
	}

	protected virtual List<ObjectInfo> GetObjectsInCategory(ObjectCategory NewCategory)
	{
		List<ObjectInfo> list = new List<ObjectInfo>();
		for (int i = 0; i < (int)ObjectTypeList.m_Total; i++)
		{
			ObjectType objectType = (ObjectType)i;
			if (ObjectTypeList.Instance.GetCategoryFromType(objectType) == NewCategory)
			{
				if (m_Everything)
				{
					list.Add(new ObjectInfo(ObjectInfo.Type.Normal, objectType, ObjectTypeList.m_Total));
				}
				else if (ObjectTypeList.Instance.GetIsHoldable(objectType) && !ToolFillable.GetIsTypeLiquid(objectType) && objectType != ObjectType.AnimalBird && objectType != ObjectType.AnimalBee)
				{
					list.Add(new ObjectInfo(ObjectInfo.Type.Normal, objectType, ObjectTypeList.m_Total));
				}
			}
		}
		return list;
	}

	public void SetObject(ObjectType NewType, bool Flash = false, bool ScrollTo = false)
	{
		foreach (BaseButtonImage objectButton in m_ObjectButtons)
		{
			objectButton.SetSelected(Selected: false);
		}
		ObjectCategory categoryFromType = ObjectTypeList.Instance.GetCategoryFromType(NewType);
		SetCategory(categoryFromType);
		int num = -1;
		for (int i = 0; i < m_ObjectList.Count; i++)
		{
			if (m_ObjectList[i].m_ObjectType == NewType)
			{
				m_ObjectButtons[i].SetSelected(Selected: true);
				num = i;
				break;
			}
		}
		if (Flash && num != -1)
		{
			m_FlashTimer = 1.5f;
			m_FlashIndex = num;
			m_ScrollToObject = NewType;
		}
		else
		{
			m_FlashTimer = 0f;
		}
		if (ScrollTo)
		{
			m_ScrollToObject = NewType;
		}
	}

	public void SetCategory(ObjectCategory NewCategory)
	{
		if (m_CurrentCategory == NewCategory)
		{
			return;
		}
		m_CurrentCategory = NewCategory;
		m_FlashTimer = 0f;
		for (int i = 0; i < m_CategoryButtons.Count; i++)
		{
			if (NewCategory == m_Categories[i])
			{
				m_CategoryButtons[i].SetSelected(Selected: true);
			}
			else
			{
				m_CategoryButtons[i].SetSelected(Selected: false);
			}
		}
		m_ObjectButtonList.m_CreateObjectCallback = OnCreateObjectButton;
		m_ObjectList = GetObjectsInCategory(NewCategory);
		m_ObjectButtonList.m_ObjectCount = m_ObjectList.Count;
		float width = m_ScrollView.GetWidth() - 10f;
		m_ObjectButtonList.AutoSetButtonsPerRow(width);
		foreach (BaseButtonImage objectButton in m_ObjectButtons)
		{
			RemoveAction(objectButton);
		}
		m_ObjectButtons.Clear();
		m_ObjectButtonList.CreateButtons();
		UpdateHeight();
	}

	public void UpdateHeight()
	{
		float scrollSize = 0f;
		if ((bool)m_ObjectButtonList)
		{
			scrollSize = m_ObjectButtonList.GetHeight();
		}
		m_ScrollView.SetScrollSize(scrollSize);
		m_ScrollView.SetScrollValue(1f);
	}

	protected virtual void InitButtonWithObjectInfo(StandardButtonObject NewButton, ObjectInfo NewInfo)
	{
		ObjectType objectType = NewInfo.m_ObjectType;
		NewButton.SetSprite(IconManager.Instance.GetIcon(objectType));
		NewButton.SetObjectType(objectType);
		if ((bool)NewButton.transform.Find("Upgrade"))
		{
			BaseImage component = NewButton.transform.Find("Upgrade").GetComponent<BaseImage>();
			int variableAsInt = VariableManager.Instance.GetVariableAsInt(objectType, "UpgradeFrom", CheckValid: false);
			ObjectType objectType2 = ObjectTypeList.m_Total;
			if (variableAsInt != 0)
			{
				objectType2 = (ObjectType)variableAsInt;
			}
			component.SetActive(objectType2 != ObjectTypeList.m_Total);
		}
	}

	private bool GetIsObjectUnlocked(ObjectType NewObjectType)
	{
		if (NewObjectType < ObjectTypeList.m_Total)
		{
			if (QuestManager.Instance.m_BuildingsLocked.ContainsKey(NewObjectType) || QuestManager.Instance.m_ObjectsLocked.ContainsKey(NewObjectType))
			{
				return false;
			}
			return true;
		}
		return true;
	}

	public void OnCreateObjectButton(GameObject NewObject, int Index)
	{
		StandardButtonObject component = NewObject.GetComponent<StandardButtonObject>();
		InitButtonWithObjectInfo(component, m_ObjectList[Index]);
		component.SetLocked(!GetIsObjectUnlocked(m_ObjectList[Index].m_ObjectType));
		m_ObjectButtons.Add(component);
		RegisterGadget(component);
		AddAction(component, OnObjectClicked);
	}

	public virtual void OnObjectClicked(BaseGadget NewGadget)
	{
		int index = m_ObjectButtons.IndexOf(NewGadget.GetComponent<BaseButtonImage>());
		ObjectType objectType = m_ObjectList[index].m_ObjectType;
		GameStateManager.Instance.PopState();
		if (GameStateManager.Instance.GetCurrentState().m_BaseState == GameStateManager.State.MissionEditor)
		{
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateMissionEditor>().m_Editor.ObjectTypeChanged(objectType);
		}
		if (GameStateManager.Instance.GetCurrentState().m_BaseState == GameStateManager.State.Industry)
		{
			Quest questFromUnlockedObject = QuestData.Instance.GetQuestFromUnlockedObject(objectType);
			GameStateIndustry.SetSelectedQuest(questFromUnlockedObject);
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateIndustry>().IndustryLevelClicked(questFromUnlockedObject);
		}
	}

	public BaseButtonImage GetButtonFromObjectType(ObjectType NewType)
	{
		int num = 0;
		foreach (ObjectInfo @object in m_ObjectList)
		{
			if (NewType == @object.m_ObjectType)
			{
				return m_ObjectButtons[num];
			}
			num++;
		}
		return null;
	}

	private void ScrollTo(ObjectType NewType)
	{
		BaseButtonImage buttonFromObjectType = GetButtonFromObjectType(NewType);
		if (buttonFromObjectType != null)
		{
			float num = 0f - buttonFromObjectType.transform.localPosition.y + m_ObjectButtonList.m_Object.transform.localPosition.y;
			float y = m_ScrollView.GetContent().GetComponent<RectTransform>().sizeDelta.y;
			m_ScrollView.GetHeight();
			float num2 = y;
			float scrollValue = 1f - num / num2;
			m_ScrollView.SetScrollValue(scrollValue);
		}
	}

	private void UpdateSelectedObjectFlash()
	{
		if (m_FlashTimer > 0f)
		{
			if (TimeManager.Instance.m_PauseTimeEnabled)
			{
				m_FlashTimer -= TimeManager.Instance.m_PauseDelta;
			}
			else
			{
				m_FlashTimer -= TimeManager.Instance.m_NormalDelta;
			}
			if (m_FlashTimer < 0f)
			{
				m_ObjectButtons[m_FlashIndex].SetActive(Active: true);
			}
			else if ((int)(m_FlashTimer * 60f) % 20 < 10)
			{
				m_ObjectButtons[m_FlashIndex].SetActive(Active: true);
			}
			else
			{
				m_ObjectButtons[m_FlashIndex].SetActive(Active: false);
			}
		}
	}

	public void UpdateLockedObjects()
	{
		int num = 0;
		foreach (BaseButton button in m_ObjectButtonList.m_Buttons)
		{
			button.GetComponent<StandardButtonObject>().SetLocked(!GetIsObjectUnlocked(m_ObjectList[num].m_ObjectType));
			num++;
		}
	}

	protected new void Update()
	{
		base.Update();
		UpdateSelectedObjectFlash();
		if (m_ScrollToObject != ObjectTypeList.m_Total)
		{
			ScrollTo(m_ScrollToObject);
			m_ScrollToObject = ObjectTypeList.m_Total;
		}
	}
}
