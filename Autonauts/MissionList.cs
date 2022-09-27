using System;
using System.Collections.Generic;
using UnityEngine;

public class MissionList : BaseMenu
{
	private BasePanelOptions m_Panel;

	private ButtonList m_ButtonList;

	private BaseScrollView m_ScrollView;

	private List<ObjectType> m_Types;

	private List<Quest.ID> m_Missions;

	private List<QuestEvent.Type> m_Events;

	private List<Tile.TileType> m_TileTypes;

	private MissionEditorGadget m_MissionGadget;

	private MissionEditor m_Editor;

	private BaseText m_Title;

	protected new void Awake()
	{
		base.Awake();
	}

	private void SetListObject(BaseButton NewObject)
	{
		m_ButtonList.m_Object = NewObject.gameObject;
		m_ButtonList.m_ButtonWidth = NewObject.GetComponent<RectTransform>().rect.width;
		m_ButtonList.m_ButtonHeight = NewObject.GetComponent<RectTransform>().rect.height;
		m_ButtonList.m_HorizontalSpacing = m_ButtonList.m_ButtonWidth + 5f;
		m_ButtonList.m_VerticalSpacing = m_ButtonList.m_ButtonHeight + 5f;
	}

	private void SetListSize(int Count, Action<GameObject, int> CreateObjectCallback)
	{
		m_ButtonList.m_ObjectCount = Count;
		m_ButtonList.m_CreateObjectCallback = CreateObjectCallback;
		float width = m_ScrollView.GetComponent<RectTransform>().rect.width - 40f;
		m_ButtonList.AutoSetButtonsPerRow(width);
		m_ScrollView.SetScrollSize(m_ButtonList.GetHeight());
	}

	private void SetupObject(BaseButtonImage NewObject)
	{
		SetListObject(NewObject);
		m_Types = new List<ObjectType>();
		for (int i = 0; i < (int)ObjectTypeList.m_Total; i++)
		{
			ObjectType objectType = (ObjectType)i;
			if (IconManager.Instance.GetIcon(objectType) != null)
			{
				m_Types.Add(objectType);
			}
		}
		SetListSize(m_Types.Count, OnCreateObject);
	}

	public void OnCreateObject(GameObject NewObject, int Index)
	{
		ObjectType objectType = m_Types[Index];
		BaseButtonImage component = NewObject.GetComponent<BaseButtonImage>();
		component.SetSprite(IconManager.Instance.GetIcon(objectType));
		component.SetRolloverFromID(ObjectTypeList.Instance.GetSaveNameFromIdentifier(objectType));
		component.m_ExtraData = Index;
		AddAction(component, OnObjectClicked);
	}

	public void OnObjectClicked(BaseGadget NewGadget)
	{
		int index = (int)NewGadget.m_ExtraData;
		ObjectType newType = m_Types[index];
		m_Editor.ObjectTypeChanged(newType);
		GameStateManager.Instance.PopState();
	}

	private void SetupMission(BaseButtonText NewMission)
	{
		SetListObject(NewMission);
		m_Title.SetText("Select Mission");
		m_Missions = new List<Quest.ID>();
		for (int i = 0; i < 155; i++)
		{
			Quest.ID iD = (Quest.ID)i;
			Quest quest = QuestData.Instance.GetQuest(iD);
			if (quest != null && quest.m_Type != 0)
			{
				m_Missions.Add(iD);
			}
		}
		SetListSize(m_Missions.Count, OnCreateMission);
	}

	public void OnCreateMission(GameObject NewObject, int Index)
	{
		Quest.ID newID = m_Missions[Index];
		Quest quest = QuestData.Instance.GetQuest(newID);
		BaseButtonText component = NewObject.GetComponent<BaseButtonText>();
		component.SetTextFromID(quest.m_Title);
		component.SetRolloverFromID(quest.m_Title);
		IndustryLevel industryLevelFromQuest = QuestData.Instance.m_IndustryData.GetIndustryLevelFromQuest(quest);
		if (industryLevelFromQuest != null && industryLevelFromQuest.m_Type != Quest.Type.Research)
		{
			Color panelColour = industryLevelFromQuest.m_Parent.m_Parent.m_PanelColour;
			component.SetBackingColour(panelColour);
		}
		component.m_ExtraData = Index;
		AddAction(component, OnMissionClicked);
	}

	public void OnMissionClicked(BaseGadget NewGadget)
	{
		int index = (int)NewGadget.m_ExtraData;
		Quest.ID newID = m_Missions[index];
		m_Editor.MissionTypeChanged(newID);
		GameStateManager.Instance.PopState();
	}

	private void SetupEvent(BaseButtonText NewEvent)
	{
		SetListObject(NewEvent);
		m_Title.SetText("Select Event");
		m_Events = new List<QuestEvent.Type>();
		for (int i = 0; i < 206; i++)
		{
			QuestEvent.Type type = (QuestEvent.Type)i;
			string nameFromType = QuestEvent.GetNameFromType(type);
			if (TextManager.Instance.DoesExist(nameFromType))
			{
				m_Events.Add(type);
			}
		}
		SetListSize(m_Events.Count, OnCreateEvent);
	}

	public void OnCreateEvent(GameObject NewObject, int Index)
	{
		QuestEvent.Type newType = m_Events[Index];
		BaseButtonText component = NewObject.GetComponent<BaseButtonText>();
		string nameFromType = QuestEvent.GetNameFromType(newType);
		component.SetTextFromID(nameFromType);
		component.SetRolloverFromID(nameFromType);
		component.m_ExtraData = Index;
		AddAction(component, OnEventClicked);
	}

	public void OnEventClicked(BaseGadget NewGadget)
	{
		int index = (int)NewGadget.m_ExtraData;
		QuestEvent.Type newEvent = m_Events[index];
		m_Editor.EventTypeChanged(newEvent);
		GameStateManager.Instance.PopState();
	}

	private void SetupTileType(BaseButtonText NewEvent)
	{
		SetListObject(NewEvent);
		m_Title.SetText("Select Tile Type");
		m_TileTypes = new List<Tile.TileType>();
		for (int i = 0; i < 71; i++)
		{
			m_TileTypes.Add((Tile.TileType)i);
		}
		SetListSize(m_TileTypes.Count, OnCreateTileType);
	}

	public void OnCreateTileType(GameObject NewObject, int Index)
	{
		Tile.TileType newType = m_TileTypes[Index];
		BaseButtonText component = NewObject.GetComponent<BaseButtonText>();
		string nameFromType = Tile.GetNameFromType(newType);
		component.SetTextFromID(nameFromType);
		component.SetRolloverFromID(nameFromType);
		component.m_ExtraData = Index;
		AddAction(component, OnTileTypeClicked);
	}

	public void OnTileTypeClicked(BaseGadget NewGadget)
	{
		int index = (int)NewGadget.m_ExtraData;
		Tile.TileType newType = m_TileTypes[index];
		m_Editor.TileTypeTypeChanged(newType);
		GameStateManager.Instance.PopState();
	}

	public void SetMissionGadget(MissionEditorGadget NewGadget, MissionEditor NewEditor)
	{
		m_MissionGadget = NewGadget;
		m_Editor = NewEditor;
		m_ScrollView = base.transform.Find("BasePanelOptions").Find("BaseScrollView").GetComponent<BaseScrollView>();
		m_ButtonList = m_ScrollView.GetContent().transform.Find("ButtonList").GetComponent<ButtonList>();
		m_Title = base.transform.Find("BasePanelOptions").GetComponent<BasePanelOptions>().GetTitle();
		BaseButtonImage component = m_ButtonList.transform.Find("DefaultButtonObject").GetComponent<BaseButtonImage>();
		component.gameObject.SetActive(value: false);
		BaseButtonText component2 = m_ButtonList.transform.Find("DefaultButtonMission").GetComponent<BaseButtonText>();
		component2.gameObject.SetActive(value: false);
		BaseButtonText component3 = m_ButtonList.transform.Find("DefaultButtonEvent").GetComponent<BaseButtonText>();
		component3.gameObject.SetActive(value: false);
		BaseButtonText component4 = m_ButtonList.transform.Find("DefaultButtonTileType").GetComponent<BaseButtonText>();
		component4.gameObject.SetActive(value: false);
		if ((bool)NewGadget.GetComponent<MissionEditorObject>())
		{
			SetupObject(component);
		}
		else if ((bool)NewGadget.GetComponent<MissionEditorMission>())
		{
			SetupMission(component2);
		}
		else if ((bool)NewGadget.GetComponent<MissionEditorEvent>())
		{
			if (!NewGadget.GetComponent<MissionEditorEvent>().m_EditExtra)
			{
				SetupEvent(component3);
			}
			else
			{
				QuestEvent questEvent = new QuestEvent(NewGadget.GetComponent<MissionEditorEvent>().m_Event.m_Type, BotOnly: false, 0, 0);
				if (questEvent.DoesTypeNeedExtraDataMission())
				{
					SetupMission(component2);
				}
				else if (questEvent.DoesTypeNeedExtraDataObject())
				{
					SetupObject(component);
				}
				else if (questEvent.DoesTypeNeedExtraDataTileType())
				{
					SetupTileType(component4);
				}
			}
		}
		m_ButtonList.CreateButtons();
	}
}
