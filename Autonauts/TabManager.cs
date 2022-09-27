using System.Collections.Generic;
using UnityEngine;

public class TabManager : MonoBehaviour
{
	public enum TabType
	{
		Quests,
		Workers,
		Total
	}

	public static TabManager Instance;

	public List<Tab> m_Tabs;

	public TabType m_ActiveTabType;

	private void Awake()
	{
		Instance = this;
		m_Tabs = new List<Tab>();
		for (int i = 0; i < 2; i++)
		{
			TabType tabType = (TabType)i;
			string text = "Tab" + tabType;
			Tab component = Object.Instantiate((GameObject)Resources.Load("Prefabs/Hud/Tabs/" + text, typeof(GameObject)), HudManager.Instance.m_TabsRootTransform).GetComponent<Tab>();
			component.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -10f);
			component.SetActive(Active: false, Open: false);
			m_Tabs.Add(component);
		}
		m_ActiveTabType = TabType.Total;
	}

	private void OnDestroy()
	{
		foreach (Tab tab in m_Tabs)
		{
			Object.DestroyImmediate(tab.gameObject);
		}
	}

	public void TabClicked(TabType NewType)
	{
		bool open = true;
		if (NewType == m_ActiveTabType)
		{
			open = false;
			NewType = TabType.Total;
		}
		foreach (Tab tab in m_Tabs)
		{
			if (tab.m_Type == NewType && NewType != m_ActiveTabType)
			{
				tab.SetActive(Active: true, open);
			}
			else
			{
				tab.SetActive(Active: false, open);
			}
		}
		m_ActiveTabType = NewType;
	}

	public void SetActive(bool Active)
	{
		foreach (Tab tab in m_Tabs)
		{
			if ((bool)tab)
			{
				tab.SetVisible(Active);
			}
		}
	}

	public void ActivateTab(TabType NewType)
	{
		m_Tabs[(int)NewType].ActivateTab(PostLoad: true);
	}

	public void PostLoad()
	{
		foreach (Tab tab in m_Tabs)
		{
			if ((bool)tab)
			{
				tab.PostLoad();
			}
		}
	}
}
