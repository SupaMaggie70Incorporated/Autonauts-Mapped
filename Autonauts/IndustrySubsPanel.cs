using System.Collections.Generic;
using UnityEngine;

public class IndustrySubsPanel : BaseMenu
{
	private BaseScrollView m_Panel;

	private IndustrySubButton m_DefaultButton;

	private List<IndustrySubButton> m_IndustrySubButtons;

	private IndustrySubButton m_SelectedIndustrySub;

	private BaseText m_Title;

	protected new void Awake()
	{
		base.Awake();
		m_Panel = base.transform.Find("BasePanel").Find("BaseScrollView").GetComponent<BaseScrollView>();
		m_DefaultButton = m_Panel.GetContent().transform.Find("DefaultButton").GetComponent<IndustrySubButton>();
		m_DefaultButton.SetActive(Active: false);
		m_Title = base.transform.Find("BasePanel").Find("Title").GetComponent<BaseText>();
		m_SelectedIndustrySub = null;
		m_IndustrySubButtons = new List<IndustrySubButton>();
		base.gameObject.SetActive(value: false);
	}

	public void SetCurrentIndustry(Industry NewIndustry)
	{
		foreach (IndustrySubButton industrySubButton in m_IndustrySubButtons)
		{
			Object.Destroy(industrySubButton.gameObject);
		}
		m_IndustrySubButtons.Clear();
		if (NewIndustry != null)
		{
			base.gameObject.SetActive(value: true);
			CreateButtons(NewIndustry);
			m_Title.SetTextFromID(NewIndustry.m_Name);
		}
		else
		{
			base.gameObject.SetActive(value: false);
			m_Title.SetText("");
		}
		IndustriesPanels.Instance.SetIndustrySub(null);
	}

	private void CreateButtons(Industry CurrentIndustry)
	{
		Transform parent = m_Panel.GetContent().transform;
		Vector2 anchoredPosition = m_DefaultButton.GetComponent<RectTransform>().anchoredPosition;
		float num = m_DefaultButton.GetComponent<RectTransform>().rect.width + 10f;
		foreach (IndustrySub subIndustry in CurrentIndustry.m_SubIndustries)
		{
			IndustrySubButton component = Object.Instantiate(m_DefaultButton, parent).GetComponent<IndustrySubButton>();
			component.gameObject.SetActive(value: true);
			component.Init(subIndustry);
			component.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
			RegisterGadget(component);
			AddAction(component, OnIndustrySubClicked);
			m_IndustrySubButtons.Add(component);
			anchoredPosition.x += num;
		}
	}

	public void SetSelectedIndustrySub(IndustrySub NewSub)
	{
		m_SelectedIndustrySub = null;
		foreach (IndustrySubButton industrySubButton in m_IndustrySubButtons)
		{
			if (industrySubButton.m_IndustrySub == NewSub)
			{
				industrySubButton.SetSelected(Selected: true);
				m_SelectedIndustrySub = industrySubButton;
			}
			else
			{
				industrySubButton.SetSelected(Selected: false);
			}
		}
	}

	public void OnIndustrySubClicked(BaseGadget NewGadget)
	{
		IndustrySubButton component = NewGadget.GetComponent<IndustrySubButton>();
		if (m_SelectedIndustrySub != component)
		{
			SetSelectedIndustrySub(component.m_IndustrySub);
			IndustriesPanels.Instance.SetIndustrySub(m_SelectedIndustrySub.m_IndustrySub);
		}
		else
		{
			SetSelectedIndustrySub(null);
			IndustriesPanels.Instance.SetIndustrySub(null);
		}
	}
}
