using System.Collections.Generic;
using UnityEngine;

public class MissionEditor : BaseMenu
{
	private enum State
	{
		Events,
		Objects,
		Missions,
		Total
	}

	private static float m_ListHeight = 32f;

	private State m_State;

	private IndustryLevel m_Level;

	private IndustryLevel m_OriginalLevel;

	private GameObject m_DefaultObject;

	private GameObject m_DefaultEvent;

	private GameObject m_DefaultMission;

	private List<MissionEditorGadget> m_ListObjects;

	private List<QuestEvent> m_EventExlusion;

	private List<ObjectType> m_ObjectExlusion;

	private List<Quest.ID> m_MissionExlusion;

	private BaseScrollView m_ScrollView;

	private float m_ListY;

	private MissionEditorGadget m_ButtonClicked;

	protected new void Awake()
	{
		base.Awake();
		BaseGadget component = base.transform.Find("BasePanelOptions").Find("Events").GetComponent<BaseGadget>();
		AddAction(component, OnEventsClicked);
		component = base.transform.Find("BasePanelOptions").Find("Objects").GetComponent<BaseGadget>();
		AddAction(component, OnObjectsClicked);
		component = base.transform.Find("BasePanelOptions").Find("Missions").GetComponent<BaseGadget>();
		AddAction(component, OnMissionsClicked);
		component = base.transform.Find("BasePanelOptions").Find("Add").GetComponent<BaseGadget>();
		AddAction(component, OnNewClicked);
		component = base.transform.Find("BasePanelOptions").Find("Done").GetComponent<BaseGadget>();
		AddAction(component, OnDoneClicked);
		m_DefaultObject = base.transform.Find("BasePanelOptions").Find("DefaultObject").gameObject;
		m_DefaultObject.SetActive(value: false);
		m_DefaultEvent = base.transform.Find("BasePanelOptions").Find("DefaultEvent").gameObject;
		m_DefaultEvent.SetActive(value: false);
		m_DefaultMission = base.transform.Find("BasePanelOptions").Find("DefaultMission").gameObject;
		m_DefaultMission.SetActive(value: false);
		m_ScrollView = base.transform.Find("BasePanelOptions").Find("BaseScrollView").GetComponent<BaseScrollView>();
		m_EventExlusion = new List<QuestEvent>();
		m_ObjectExlusion = new List<ObjectType>();
		m_MissionExlusion = new List<Quest.ID>();
	}

	protected new void Start()
	{
		base.Start();
		m_ListObjects = new List<MissionEditorGadget>();
		SetState(State.Events);
	}

	public void SetLevel(IndustryLevel NewLevel)
	{
		m_OriginalLevel = NewLevel;
		BaseText title = base.transform.Find("BasePanelOptions").GetComponent<BasePanelOptions>().GetTitle();
		string questNameFromID = QuestData.Instance.GetQuestNameFromID(NewLevel.m_ID);
		title.SetTextFromID(questNameFromID);
		m_Level = new IndustryLevel(NewLevel.m_ID, NewLevel.m_Parent, NewLevel.m_Type, NewLevel.m_MajorNode, NewLevel.m_ShowUnlockedQuests);
		m_Level.m_Quest = NewLevel.m_Quest;
		m_Level.m_ResearchObjectType = NewLevel.m_ResearchObjectType;
		m_Level.m_ResearchType = NewLevel.m_ResearchType;
		foreach (IndustryLevelEvent @event in NewLevel.m_Events)
		{
			m_Level.AddRequirement(@event.m_Type, @event.m_BotOnly, @event.m_ExtraData, @event.m_Count);
		}
		foreach (ObjectType unlockedObject in NewLevel.m_UnlockedObjects)
		{
			m_Level.AddUnlocked(unlockedObject);
		}
		foreach (Quest.ID unlockedQuest in NewLevel.m_UnlockedQuests)
		{
			m_Level.AddMissionUnlocked(unlockedQuest);
		}
	}

	public void OnEventsClicked(BaseGadget NewGadget)
	{
		SetState(State.Events);
	}

	public void OnObjectsClicked(BaseGadget NewGadget)
	{
		SetState(State.Objects);
	}

	public void OnMissionsClicked(BaseGadget NewGadget)
	{
		SetState(State.Missions);
	}

	public void OnNewClicked(BaseGadget NewGadget)
	{
		if (m_State == State.Events)
		{
			IndustryLevelEvent item = new IndustryLevelEvent(QuestEvent.Type.Make, BotOnly: false, ObjectType.Nothing, 1);
			m_Level.m_Events.Add(item);
		}
		else if (m_State == State.Objects)
		{
			m_Level.m_UnlockedObjects.Add(ObjectType.Nothing);
		}
		Refresh();
	}

	public void OnDoneClicked(BaseGadget NewGadget)
	{
		Quest quest = m_OriginalLevel.m_Quest;
		if (m_OriginalLevel.m_Type != Quest.Type.Research)
		{
			m_OriginalLevel.m_Events.Clear();
			quest.m_EventsRequired.Clear();
			foreach (IndustryLevelEvent @event in m_Level.m_Events)
			{
				m_OriginalLevel.AddRequirement(@event.m_Type, @event.m_BotOnly, @event.m_ExtraData, @event.m_Count);
				quest.AddEvent(@event.m_Type, @event.m_BotOnly, @event.m_ExtraData, @event.m_Count);
			}
		}
		else
		{
			m_OriginalLevel.m_Events[0].m_Count = m_Level.m_Events[0].m_Count;
		}
		m_OriginalLevel.m_UnlockedObjects.Clear();
		quest.m_ObjectsUnlocked.Clear();
		quest.m_BuildingsUnlocked.Clear();
		foreach (ObjectType unlockedObject in m_Level.m_UnlockedObjects)
		{
			m_OriginalLevel.AddUnlocked(unlockedObject);
			if (ObjectTypeList.Instance.GetIsBuilding(unlockedObject))
			{
				quest.AddBuildingUnlocked(unlockedObject);
			}
			else
			{
				quest.AddObjectUnlocked(unlockedObject);
			}
		}
		m_OriginalLevel.m_UnlockedQuests.Clear();
		quest.m_QuestsUnlocked.Clear();
		foreach (Quest.ID unlockedQuest in m_Level.m_UnlockedQuests)
		{
			m_OriginalLevel.AddMissionUnlocked(unlockedQuest);
			quest.AddQuestUnlocked(unlockedQuest);
		}
		GameStateManager.Instance.PopState();
		GameStateManager.Instance.GetCurrentState().GetComponent<GameStateIndustry>().Refresh(ChangesMade: true);
		GameStateManager.Instance.GetCurrentState().GetComponent<GameStateIndustry>().ShowSave();
		QuestManagerTiles.Instance.Init();
	}

	public void OnObjectClicked(MissionEditorObject ObjectClicked)
	{
		m_ButtonClicked = ObjectClicked;
		GameStateManager.Instance.PushState(GameStateManager.State.ObjectSelect);
		GameStateManager.Instance.GetCurrentState().GetComponent<GameStateObjectSelect>().Init(BuildingsOnly: false, Everything: true);
	}

	public void OnEventClicked(MissionEditorEvent EventClicked)
	{
		m_ButtonClicked = EventClicked;
		QuestEvent questEvent = new QuestEvent(EventClicked.m_Event.m_Type, BotOnly: false, 0, 0);
		if (EventClicked.m_EditExtra && questEvent.DoesTypeNeedExtraDataObject())
		{
			GameStateManager.Instance.PushState(GameStateManager.State.ObjectSelect);
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateObjectSelect>().Init(QuestEvent.DoesTypeNeedBuildingObjects(questEvent.m_Type), Everything: false);
		}
		else if (EventClicked.m_EditExtra && questEvent.DoesTypeNeedExtraDataTileType())
		{
			GameStateManager.Instance.PushState(GameStateManager.State.MissionList);
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateMissionList>().SetMissionGadget(EventClicked, this);
		}
		else
		{
			GameStateManager.Instance.PushState(GameStateManager.State.MissionList);
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateMissionList>().SetMissionGadget(EventClicked, this);
		}
	}

	public void OnMissionClicked(MissionEditorMission MissionClicked)
	{
		m_ButtonClicked = MissionClicked;
		GameStateManager.Instance.PushState(GameStateManager.State.MissionList);
		GameStateManager.Instance.GetCurrentState().GetComponent<GameStateMissionList>().SetMissionGadget(MissionClicked, this);
	}

	private void DestroyList()
	{
		foreach (MissionEditorGadget listObject in m_ListObjects)
		{
			Object.Destroy(listObject.gameObject);
		}
		m_ListObjects.Clear();
	}

	private MissionEditorGadget CreateNewLine(GameObject DefaultObject)
	{
		MissionEditorGadget component = Object.Instantiate(DefaultObject, m_ScrollView.GetContent().transform).GetComponent<MissionEditorGadget>();
		component.transform.localPosition = new Vector3(10f, m_ListY, 0f);
		component.gameObject.SetActive(value: true);
		component.SetParent(this);
		m_ListObjects.Add(component);
		m_ListY -= m_ListHeight;
		return component;
	}

	private void CreateEvents()
	{
		foreach (IndustryLevelEvent @event in m_Level.m_Events)
		{
			CreateNewLine(m_DefaultEvent).GetComponent<MissionEditorEvent>().SetEvent(@event);
		}
	}

	private void CreateObjects()
	{
		foreach (ObjectType unlockedObject in m_Level.m_UnlockedObjects)
		{
			CreateNewLine(m_DefaultObject).GetComponent<MissionEditorObject>().SetType(unlockedObject);
		}
	}

	private void CreateMissions()
	{
		foreach (Quest.ID unlockedQuest in m_Level.m_UnlockedQuests)
		{
			CreateNewLine(m_DefaultMission).GetComponent<MissionEditorMission>().SetQuestID(unlockedQuest);
		}
	}

	private void CreateList()
	{
		m_ListY = -10f;
		if (m_State == State.Events)
		{
			CreateEvents();
		}
		else if (m_State == State.Objects)
		{
			CreateObjects();
		}
		else if (m_State == State.Missions)
		{
			CreateMissions();
		}
	}

	private void SetState(State NewState)
	{
		DestroyList();
		m_State = NewState;
		BaseGadget component = base.transform.Find("BasePanelOptions").Find("Add").GetComponent<BaseGadget>();
		if (m_State == State.Events)
		{
			component.SetInteractable(m_OriginalLevel.m_Type != Quest.Type.Research);
		}
		else
		{
			component.SetInteractable(Interactable: true);
		}
		CreateList();
	}

	public void Refresh()
	{
		DestroyList();
		CreateList();
	}

	public void Remove(MissionEditorGadget NewGadget)
	{
		if (m_State == State.Events)
		{
			if (m_OriginalLevel.m_Type != Quest.Type.Research)
			{
				m_Level.m_Events.Remove(NewGadget.GetComponent<MissionEditorEvent>().m_Event);
			}
		}
		else if (m_State == State.Objects)
		{
			m_Level.m_UnlockedObjects.Remove(NewGadget.GetComponent<MissionEditorObject>().m_Type);
		}
		else if (m_State == State.Missions)
		{
			m_Level.m_UnlockedQuests.Remove(NewGadget.GetComponent<MissionEditorMission>().m_ID);
		}
		Refresh();
	}

	public void Shift(MissionEditorGadget NewGadget, int Direction)
	{
		if (m_State == State.Events)
		{
			IndustryLevelEvent @event = NewGadget.GetComponent<MissionEditorEvent>().m_Event;
			int num = m_Level.m_Events.IndexOf(@event);
			if ((Direction == -1 && num != 0) || (Direction == 1 && num != m_Level.m_Events.Count - 1))
			{
				m_Level.m_Events.Remove(@event);
				m_Level.m_Events.Insert(num + Direction, @event);
			}
		}
		else if (m_State == State.Objects)
		{
			ObjectType type = NewGadget.GetComponent<MissionEditorObject>().m_Type;
			int num2 = m_Level.m_UnlockedObjects.IndexOf(type);
			if ((Direction == -1 && num2 != 0) || (Direction == 1 && num2 != m_Level.m_UnlockedObjects.Count - 1))
			{
				m_Level.m_UnlockedObjects.Remove(type);
				m_Level.m_UnlockedObjects.Insert(num2 + Direction, type);
			}
		}
		else if (m_State == State.Missions)
		{
			Quest.ID iD = NewGadget.GetComponent<MissionEditorMission>().m_ID;
			int num3 = m_Level.m_UnlockedQuests.IndexOf(iD);
			if ((Direction == -1 && num3 != 0) || (Direction == 1 && num3 != m_Level.m_UnlockedQuests.Count - 1))
			{
				m_Level.m_UnlockedQuests.Remove(iD);
				m_Level.m_UnlockedQuests.Insert(num3 + Direction, iD);
			}
		}
		Refresh();
	}

	public void Up(MissionEditorGadget NewGadget)
	{
		Shift(NewGadget, -1);
	}

	public void Down(MissionEditorGadget NewGadget)
	{
		Shift(NewGadget, 1);
	}

	public void ObjectTypeChanged(ObjectType NewType)
	{
		MissionEditorEvent component = m_ButtonClicked.GetComponent<MissionEditorEvent>();
		if ((bool)component)
		{
			EventExtraDataChanged(component, NewType);
		}
		else
		{
			MissionEditorObject component2 = m_ButtonClicked.GetComponent<MissionEditorObject>();
			for (int i = 0; i < m_Level.m_UnlockedObjects.Count; i++)
			{
				if (m_Level.m_UnlockedObjects[i] == component2.m_Type)
				{
					m_Level.m_UnlockedObjects[i] = NewType;
					break;
				}
			}
		}
		Refresh();
	}

	public void MissionTypeChanged(Quest.ID NewID)
	{
		MissionEditorEvent component = m_ButtonClicked.GetComponent<MissionEditorEvent>();
		if ((bool)component)
		{
			EventExtraDataChanged(component, NewID);
		}
		else
		{
			MissionEditorMission component2 = m_ButtonClicked.GetComponent<MissionEditorMission>();
			if ((bool)component2)
			{
				for (int i = 0; i < m_Level.m_UnlockedQuests.Count; i++)
				{
					if (m_Level.m_UnlockedQuests[i] == component2.m_ID)
					{
						m_Level.m_UnlockedQuests[i] = NewID;
						break;
					}
				}
			}
		}
		Refresh();
	}

	public void EventTypeChanged(QuestEvent.Type NewEvent)
	{
		if (m_OriginalLevel.m_Type == Quest.Type.Research)
		{
			return;
		}
		MissionEditorEvent component = m_ButtonClicked.GetComponent<MissionEditorEvent>();
		for (int i = 0; i < m_Level.m_Events.Count; i++)
		{
			if (m_Level.m_Events[i].m_Type == component.m_Event.m_Type && m_Level.m_Events[i].m_BotOnly == component.m_Event.m_BotOnly && m_Level.m_Events[i].m_ExtraData == component.m_Event.m_ExtraData)
			{
				m_Level.m_Events[i].m_Type = NewEvent;
				m_Level.m_Events[i].m_ExtraData = 0;
				break;
			}
		}
		Refresh();
	}

	public void TileTypeTypeChanged(Tile.TileType NewType)
	{
		MissionEditorEvent component = m_ButtonClicked.GetComponent<MissionEditorEvent>();
		if ((bool)component)
		{
			EventExtraDataChanged(component, NewType);
		}
	}

	public void EventExtraDataChanged(MissionEditorGadget NewGadget, object ExtraData)
	{
		if (m_OriginalLevel.m_Type == Quest.Type.Research)
		{
			return;
		}
		MissionEditorEvent component = NewGadget.GetComponent<MissionEditorEvent>();
		for (int i = 0; i < m_Level.m_Events.Count; i++)
		{
			if (m_Level.m_Events[i].m_Type == component.m_Event.m_Type && m_Level.m_Events[i].m_BotOnly == component.m_Event.m_BotOnly && m_Level.m_Events[i].m_ExtraData == component.m_Event.m_ExtraData)
			{
				m_Level.m_Events[i].m_ExtraData = ExtraData;
				break;
			}
		}
		Refresh();
	}
}
