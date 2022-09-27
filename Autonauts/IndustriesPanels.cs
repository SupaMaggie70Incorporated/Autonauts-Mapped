using UnityEngine;

public class IndustriesPanels : MonoBehaviour
{
	public static IndustriesPanels Instance;

	private IndustriesPanel m_Industries;

	private IndustrySubsPanel m_SubIndustries;

	private IndustryLevelsPanel m_Levels;

	private void Awake()
	{
		Instance = this;
		GetDefaultGadgets();
	}

	private void GetDefaultGadgets()
	{
		m_Industries = base.transform.Find("IndustryList").GetComponent<IndustriesPanel>();
		m_SubIndustries = base.transform.Find("SubIndustryList").GetComponent<IndustrySubsPanel>();
		m_Levels = base.transform.Find("LevelList").GetComponent<IndustryLevelsPanel>();
	}

	private void Start()
	{
		IndustryLevel industryLevelFromQuest = QuestData.Instance.m_IndustryData.GetIndustryLevelFromQuest(GameStateIndustry.m_SelectedQuest);
		SetSelectedLevel(industryLevelFromQuest);
	}

	public void SetIndustry(Industry NewIndustry)
	{
		m_SubIndustries.SetCurrentIndustry(NewIndustry);
	}

	public void SetIndustrySub(IndustrySub NewIndustrySub)
	{
		m_Levels.SetCurrentIndustrySub(NewIndustrySub);
	}

	public void SetIndustryLevel(IndustryLevel NewLevel)
	{
	}

	public void SetSelectedLevel(IndustryLevel NewLevel)
	{
		if (NewLevel != null && NewLevel.m_Type != Quest.Type.Research)
		{
			m_Industries.SetSelectedIndustry(NewLevel.m_Parent.m_Parent);
			m_SubIndustries.SetCurrentIndustry(NewLevel.m_Parent.m_Parent);
			m_SubIndustries.SetSelectedIndustrySub(NewLevel.m_Parent);
			m_Levels.SetCurrentIndustrySub(NewLevel.m_Parent);
			m_Levels.SetSelectedLevel(NewLevel);
		}
	}

	public float GetHeight()
	{
		return m_Levels.transform.localPosition.y - m_Levels.transform.Find("BasePanel").GetComponent<RectTransform>().rect.yMin;
	}
}
