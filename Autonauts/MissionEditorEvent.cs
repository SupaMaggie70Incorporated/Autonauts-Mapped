using UnityEngine;

public class MissionEditorEvent : MissionEditorGadget
{
	public IndustryLevelEvent m_Event;

	private BaseInputField m_Input;

	public bool m_EditExtra;

	public void SetEvent(IndustryLevelEvent NewEvent)
	{
		m_Event = NewEvent;
		BaseButtonText component = base.transform.Find("Event").GetComponent<BaseButtonText>();
		component.SetTextFromID(QuestEvent.GetNameFromType(NewEvent.m_Type));
		component.SetAction(OnClickedEvent, component);
		BaseToggle component2 = base.transform.Find("Bot").GetComponent<BaseToggle>();
		component2.SetStartOn(NewEvent.m_BotOnly);
		component2.SetAction(OnClickedBot, component2);
		QuestEvent questEvent = new QuestEvent(NewEvent.m_Type, NewEvent.m_BotOnly, NewEvent.m_ExtraData, 0);
		component = base.transform.Find("Extra").GetComponent<BaseButtonText>();
		component.SetText(questEvent.GetExtraDataString());
		component.SetAction(OnClickedExtra, component);
		if (questEvent.DoesTypeNeedExtraDataMission())
		{
			Quest.ID newID = (Quest.ID)questEvent.m_ExtraData;
			Quest quest = QuestData.Instance.GetQuest(newID);
			IndustryLevel industryLevelFromQuest = QuestData.Instance.m_IndustryData.GetIndustryLevelFromQuest(quest);
			if (industryLevelFromQuest != null && industryLevelFromQuest.m_Type != Quest.Type.Research)
			{
				Color panelColour = industryLevelFromQuest.m_Parent.m_Parent.m_PanelColour;
				component.SetBackingColour(panelColour);
			}
		}
		m_Input = base.transform.Find("BaseInputField").GetComponent<BaseInputField>();
		string text = NewEvent.m_Count.ToString();
		m_Input.SetPlaceholderText(text);
		m_Input.SetText(text);
		m_Input.SetAction(OnValueChanged, component);
	}

	public void OnClickedEvent(BaseGadget NewGadget)
	{
		m_EditExtra = false;
		m_Parent.OnEventClicked(this);
	}

	public void OnClickedBot(BaseGadget NewGadget)
	{
		m_Event.m_BotOnly = NewGadget.GetComponent<BaseToggle>().GetOn();
	}

	public void OnClickedExtra(BaseGadget NewGadget)
	{
		QuestEvent questEvent = new QuestEvent(m_Event.m_Type, m_Event.m_BotOnly, m_Event.m_ExtraData, 0);
		if (questEvent.DoesTypeNeedExtraDataObject() || questEvent.DoesTypeNeedExtraDataMission() || questEvent.DoesTypeNeedExtraDataTileType())
		{
			m_EditExtra = true;
			m_Parent.OnEventClicked(this);
		}
	}

	public void OnValueChanged(BaseGadget NewGadget)
	{
		if (int.TryParse(m_Input.GetText(), out var result))
		{
			m_Event.m_Count = result;
			return;
		}
		m_Event.m_Count = 0;
		m_Input.SetText("0");
	}
}
