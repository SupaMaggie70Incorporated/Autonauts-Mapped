using System.Collections.Generic;
using UnityEngine;

public class IndustryLevelsPanel : BaseMenu
{
	private BaseScrollView m_Panel;

	private IndustryLevelButton m_DefaultButton;

	private List<IndustryLevelButton> m_IndustryLevelButtons;

	private IndustryLevelButton m_SelectedIndustryLevel;

	private BaseText m_Title;

	protected new void Awake()
	{
		base.Awake();
		m_Panel = base.transform.Find("BasePanel").Find("BaseScrollView").GetComponent<BaseScrollView>();
		m_DefaultButton = m_Panel.GetContent().transform.Find("DefaultButton").GetComponent<IndustryLevelButton>();
		m_DefaultButton.SetActive(Active: false);
		m_Title = base.transform.Find("BasePanel").Find("Title").GetComponent<BaseText>();
		m_SelectedIndustryLevel = null;
		m_IndustryLevelButtons = new List<IndustryLevelButton>();
		base.gameObject.SetActive(value: false);
	}

	public void SetCurrentIndustrySub(IndustrySub NewIndustrySub)
	{
		foreach (IndustryLevelButton industryLevelButton in m_IndustryLevelButtons)
		{
			Object.Destroy(industryLevelButton.gameObject);
		}
		m_IndustryLevelButtons.Clear();
		if (NewIndustrySub != null)
		{
			base.gameObject.SetActive(value: true);
			CreateButtons(NewIndustrySub);
			m_Title.SetTextFromID(NewIndustrySub.m_Name);
		}
		else
		{
			base.gameObject.SetActive(value: false);
			m_Title.SetText("");
		}
	}

	private void CreateButtons(IndustrySub NewIndustrySub)
	{
		Transform parent = m_Panel.GetContent().transform;
		Vector2 anchoredPosition = m_DefaultButton.GetComponent<RectTransform>().anchoredPosition;
		float num = 10f;
		float num2 = m_DefaultButton.GetComponent<RectTransform>().rect.width + num;
		foreach (IndustryLevel industryLevel in NewIndustrySub.m_IndustryLevels)
		{
			IndustryLevelButton component = Object.Instantiate(m_DefaultButton, parent).GetComponent<IndustryLevelButton>();
			component.gameObject.SetActive(value: true);
			component.Init(industryLevel);
			component.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
			RegisterGadget(component);
			AddAction(component, OnIndustryLevelClicked);
			m_IndustryLevelButtons.Add(component);
			anchoredPosition.x += num2;
		}
		m_Panel.SetScrollSize(anchoredPosition.x - num2 + num);
	}

	public void SetSelectedLevel(IndustryLevel NewLevel)
	{
		m_SelectedIndustryLevel = null;
		foreach (IndustryLevelButton industryLevelButton in m_IndustryLevelButtons)
		{
			if (industryLevelButton.m_Level == NewLevel)
			{
				industryLevelButton.SetSelected(Selected: true);
				m_SelectedIndustryLevel = industryLevelButton;
			}
			else
			{
				industryLevelButton.SetSelected(Selected: false);
			}
		}
	}

	public void OnIndustryLevelClicked(BaseGadget NewGadget)
	{
		IndustryLevelButton component = NewGadget.GetComponent<IndustryLevelButton>();
		if (m_SelectedIndustryLevel != component)
		{
			SetSelectedLevel(component.m_Level);
			IndustriesPanels.Instance.SetIndustryLevel(m_SelectedIndustryLevel.m_Level);
		}
		else
		{
			SetSelectedLevel(null);
			IndustriesPanels.Instance.SetIndustryLevel(null);
		}
	}
}
