using System.Collections.Generic;
using UnityEngine;

public class Badges : MonoBehaviour
{
	public static Badges Instance;

	private BaseScrollView m_ScrollView;

	private ButtonList m_ButtonList;

	private List<Badge.ID> m_Badges;

	private void Awake()
	{
		Instance = this;
		BaseButton component = base.transform.Find("BackButton").GetComponent<BaseButton>();
		component.SetAction(OnBackButtonClicked, component);
		m_ScrollView = base.transform.Find("BaseScrollView").GetComponent<BaseScrollView>();
		m_ButtonList = m_ScrollView.GetContent().transform.Find("BadgeList").GetComponent<ButtonList>();
		component = base.transform.Find("Clear").GetComponent<BaseButton>();
		component.SetAction(OnClearButtonClicked, component);
		if (!CheatManager.Instance.m_CheatsEnabled)
		{
			component.SetActive(Active: false);
		}
		SetupPanels();
	}

	public void OnClearButtonClicked(BaseGadget NewGadget)
	{
		BadgeManager.Instance.Clear();
		AudioManager.Instance.StartEvent("UIUnpause");
		GameStateManager.Instance.PopState();
	}

	public void OnBackButtonClicked(BaseGadget NewGadget)
	{
		GameStateManager.Instance.PopState();
	}

	protected void SetupPanels()
	{
		int num = 21;
		int num2 = num / m_ButtonList.m_ButtonsPerRow + 1;
		if (num % m_ButtonList.m_ButtonsPerRow == 0)
		{
			num2--;
		}
		_ = m_ButtonList.m_ButtonsPerRow;
		_ = m_ButtonList.m_HorizontalSpacing;
		float scrollSize = (float)(num2 - 1) * m_ButtonList.m_VerticalSpacing - m_ButtonList.transform.localPosition.y + m_ButtonList.m_Object.GetComponent<RectTransform>().sizeDelta.y / 2f;
		m_ScrollView.SetScrollSize(scrollSize);
		m_ButtonList.m_ObjectCount = num;
		m_ButtonList.m_CreateObjectCallback = OnCreateCertificate;
		m_ButtonList.CreateButtons();
		m_ScrollView.SetScrollValue(1f);
	}

	public void OnCreateCertificate(GameObject NewGadget, int Index)
	{
		NewGadget.GetComponent<BadgeHud>().SetBadge(BadgeManager.Instance.m_BadgeData.m_Badges[Index]);
	}
}
