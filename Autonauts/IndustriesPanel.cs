using System.Collections.Generic;
using UnityEngine;

public class IndustriesPanel : BaseMenu
{
	private BaseScrollView m_Panel;

	private IndustryButton m_DefaultButton;

	private List<IndustryButton> m_IndustryButtons;

	private IndustryButton m_SelectedIndustry;

	protected new void Awake()
	{
		base.Awake();
		m_Panel = base.transform.Find("BasePanel").Find("BaseScrollView").GetComponent<BaseScrollView>();
		m_DefaultButton = m_Panel.GetContent().transform.Find("DefaultButton").GetComponent<IndustryButton>();
		m_DefaultButton.SetActive(Active: false);
		CreateTabs();
		m_SelectedIndustry = null;
	}

	protected new void Start()
	{
		base.Start();
		BaseButtonBack component = base.transform.Find("BasePanel/StandardCancelButton").GetComponent<BaseButtonBack>();
		RemoveAction(component);
		component.SetAction(OnMyBackClicked, component);
	}

	public void OnMyBackClicked(BaseGadget NewGadget)
	{
		GameStateManager.Instance.GetCurrentState().GetComponent<GameStateIndustry>().SetMode(GameStateIndustry.Mode.Total);
	}

	private void CreateTabs()
	{
		Transform parent = m_Panel.GetContent().transform;
		Vector2 anchoredPosition = m_DefaultButton.GetComponent<RectTransform>().anchoredPosition;
		float num = m_DefaultButton.GetComponent<RectTransform>().rect.width + 10f;
		m_IndustryButtons = new List<IndustryButton>();
		foreach (Industry industry in QuestData.Instance.m_IndustryData.m_Industries)
		{
			IndustryButton component = Object.Instantiate(m_DefaultButton, parent).GetComponent<IndustryButton>();
			component.gameObject.SetActive(value: true);
			component.Init(industry);
			component.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
			RegisterGadget(component);
			AddAction(component, OnIndustryClicked);
			m_IndustryButtons.Add(component);
			anchoredPosition.x += num;
		}
	}

	public void SetSelectedIndustry(Industry NewIndustry)
	{
		m_SelectedIndustry = null;
		foreach (IndustryButton industryButton in m_IndustryButtons)
		{
			if (industryButton.m_Industry == NewIndustry)
			{
				industryButton.SetSelected(Selected: true);
				m_SelectedIndustry = industryButton;
			}
			else
			{
				industryButton.SetSelected(Selected: false);
			}
		}
	}

	public void OnIndustryClicked(BaseGadget NewGadget)
	{
		IndustryButton component = NewGadget.GetComponent<IndustryButton>();
		if (m_SelectedIndustry != component)
		{
			SetSelectedIndustry(component.m_Industry);
			IndustriesPanels.Instance.SetIndustry(m_SelectedIndustry.m_Industry);
		}
		else
		{
			SetSelectedIndustry(null);
			IndustriesPanels.Instance.SetIndustry(null);
		}
	}
}
