using System.Collections.Generic;
using UnityEngine;

public class Academy : AutopediaPage
{
	public static Academy Instance;

	private BaseScrollView m_ScrollView;

	private ButtonList m_ButtonList;

	private List<Quest.ID> m_Quests;

	private void Awake()
	{
		Instance = this;
		m_ScrollView = base.transform.Find("BaseScrollView").GetComponent<BaseScrollView>();
		m_ButtonList = m_ScrollView.GetContent().transform.Find("CertificateList").GetComponent<ButtonList>();
		m_Quests = new List<Quest.ID>();
		foreach (CertificateInfo certificateInfo in QuestManager.Instance.m_Data.m_AcademyData.m_CertificateInfos)
		{
			Quest quest = QuestManager.Instance.GetQuest(certificateInfo.m_ID);
			if (quest != null && quest.m_Started)
			{
				m_Quests.Add(certificateInfo.m_ID);
			}
		}

		SetupPanels();
	}

	protected void SetupPanels()
	{
		int count = m_Quests.Count;
		int num = count / m_ButtonList.m_ButtonsPerRow + 1;
		if (count % m_ButtonList.m_ButtonsPerRow == 0)
		{
			num--;
		}
		_ = m_ButtonList.m_ButtonsPerRow;
		_ = m_ButtonList.m_HorizontalSpacing;
		float scrollSize = (float)(num - 1) * m_ButtonList.m_VerticalSpacing - m_ButtonList.transform.localPosition.y + m_ButtonList.m_Object.GetComponent<RectTransform>().sizeDelta.y / 2f;
		m_ScrollView.SetScrollSize(scrollSize);
		m_ButtonList.m_ObjectCount = count;
		m_ButtonList.m_CreateObjectCallback = OnCreateCertificate;
		m_ButtonList.CreateButtons();
		m_ScrollView.SetScrollValue(1f);
	}

	public void OnCreateCertificate(GameObject NewGadget, int Index)
	{
		NewGadget.GetComponent<Certificate>().SetQuest(m_Quests[Index]);
	}

	public Certificate GetCertificateFromQuest(Quest NewQuest)
	{
		foreach (BaseGadget button in m_ButtonList.m_Buttons)
		{
			Certificate component = button.GetComponent<Certificate>();
			if (component.m_Quest == NewQuest)
			{
				return component;
			}
		}
		return null;
	}

	public Certificate GetCertificateFromLesson(Quest NewQuest)
	{
		foreach (BaseGadget button in m_ButtonList.m_Buttons)
		{
			Certificate component = button.GetComponent<Certificate>();
			if (QuestData.Instance.m_AcademyData.GetInfoFromQuestID(component.m_Quest.m_ID).m_LessonID == NewQuest.m_ID)
			{
				return component;
			}
		}
		return null;
	}

	public void UpdateLumber()
	{
		foreach (BaseGadget button in m_ButtonList.m_Buttons)
		{
			Certificate component = button.GetComponent<Certificate>();
			if (component.m_Quest.m_ID == Quest.ID.AcademyLumber2)
			{
				component.UpdateLesson();
				break;
			}
		}
	}

	public void ScrollToQuest(Quest NewQuest)
	{
		Certificate certificateFromQuest = GetCertificateFromQuest(NewQuest);
		if ((bool)certificateFromQuest)
		{
			float num = 0f - certificateFromQuest.transform.localPosition.y - m_ButtonList.m_Object.GetComponent<RectTransform>().sizeDelta.y / 2f;
			num += m_ScrollView.verticalScrollbar.GetComponent<RectTransform>().sizeDelta.y - 2f;
			float num2 = m_ScrollView.GetScrollSize() - m_ScrollView.GetHeight();
			float scrollValue = 1f - num / num2;
			m_ScrollView.SetScrollValue(scrollValue);
		}
	}

	public override void CeremonyPlaying(bool Playing, Quest NewQuest)
	{
		if (Playing)
		{
			ScrollToQuest(NewQuest);
		}
	}

	public void SetQuestVisible(Quest.ID NewID, bool Visible)
	{
		Quest quest = QuestManager.Instance.GetQuest(NewID);
		GetCertificateFromQuest(quest).SetActive(Visible);
	}

	public void SetCertficate(Quest.ID NewID)
	{
		Quest quest = QuestManager.Instance.GetQuest(NewID);
		ScrollToQuest(quest);
		GetCertificateFromQuest(quest).FlashSelected();
	}
}
