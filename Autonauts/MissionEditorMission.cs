using UnityEngine;

public class MissionEditorMission : MissionEditorGadget
{
	public Quest.ID m_ID;

	public void SetQuestID(Quest.ID NewID)
	{
		m_ID = NewID;
		BaseButtonText component = base.transform.Find("BaseButtonText").GetComponent<BaseButtonText>();
		component.SetTextFromID(QuestData.Instance.GetQuest(NewID).m_Title);
		component.SetAction(OnClicked, component);
		Quest quest = QuestData.Instance.GetQuest(NewID);
		IndustryLevel industryLevelFromQuest = QuestData.Instance.m_IndustryData.GetIndustryLevelFromQuest(quest);
		if (industryLevelFromQuest != null && industryLevelFromQuest.m_Type != Quest.Type.Research)
		{
			Color panelColour = industryLevelFromQuest.m_Parent.m_Parent.m_PanelColour;
			component.SetBackingColour(panelColour);
		}
	}

	public void OnClicked(BaseGadget NewGadget)
	{
		m_Parent.OnMissionClicked(this);
	}
}
