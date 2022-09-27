using System.Collections.Generic;
using UnityEngine;

public class ResearchLevel : BaseGadget
{
	private int m_Level;

	public ResearchLevelInfo m_LevelInfo;

	private List<ResearchTechnology> m_Technology;

	private BaseImage m_Bar;

	public void SetLevel(int Level)
	{
		m_Level = Level;
		m_LevelInfo = QuestData.Instance.m_ResearchData.GetLevelInfo(Level);
		m_Bar = base.transform.Find("Bar").GetComponent<BaseImage>();
		BaseText component = m_Bar.transform.Find("Title").GetComponent<BaseText>();
		string text = TextManager.Instance.Get("ResearchLevel", (Level + 1).ToString());
		component.SetText(text);
		CreateTechnologies();
		UpdateLocked();
	}

	private void CreateTechnologies()
	{
		List<Quest.ID> list = new List<Quest.ID>();
		foreach (ResearchInfo researchInfo in m_LevelInfo.m_ResearchInfos)
		{
			list.Add(researchInfo.m_ID);
		}
		int num = 4;
		int num2 = list.Count / num + 1;
		if (list.Count % num == 0)
		{
			num2--;
		}
		float num3 = 300f;
		float num4 = 200f;
		float x = (float)num * num3;
		float y = (float)num2 * num4 + m_Bar.GetHeight() + 30f;
		SetSize(new Vector2(x, y));
		ResearchTechnology component = base.transform.Find("ResearchTechnology").GetComponent<ResearchTechnology>();
		component.gameObject.SetActive(value: false);
		m_Technology = new List<ResearchTechnology>();
		for (int i = 0; i < list.Count; i++)
		{
			ResearchTechnology component2 = Object.Instantiate(component, base.transform).GetComponent<ResearchTechnology>();
			component2.SetActive(Active: true);
			component2.SetQuest(list[i]);
			component2.SetAction(OnTechnologyClicked, component2);
			float x2 = (float)(i % num) * num3 + component.transform.localPosition.x;
			float y2 = (float)(i / num) * (0f - num4) + component.transform.localPosition.y;
			component2.transform.localPosition = new Vector3(x2, y2, 0f);
			m_Technology.Add(component2);
		}
	}

	public void OnTechnologyClicked(BaseGadget NewGadget)
	{
		ResearchTechnology component = NewGadget.GetComponent<ResearchTechnology>();
		if (CheatManager.Instance.m_CheatMissions && Input.GetKey(KeyCode.LeftShift))
		{
			QuestManager.Instance.CheatCompleteQuest(component.m_Quest);
		}
		Research.Instance.TechnologySelected(component);
	}

	public void UpdateLocked(bool Force = false)
	{
		Transform obj = base.transform.Find("Bar");
		BaseImage component = obj.transform.Find("Lock").GetComponent<BaseImage>();
		BaseText component2 = obj.transform.Find("Required").GetComponent<BaseText>();
		BaseImage component3 = base.transform.Find("Background").GetComponent<BaseImage>();
		if (QuestData.Instance.m_ResearchData.GetLevelLocked(m_Level) || Force)
		{
			component.SetActive(Active: true);
			component2.SetActive(Active: true);
			int researchRequired = m_LevelInfo.m_ResearchRequired;
			int completed = QuestData.Instance.m_ResearchData.GetCompleted();
			string text = TextManager.Instance.Get("ResearchCompleted", completed.ToString(), researchRequired.ToString());
			component2.SetText(text);
			component3.SetColour(new Color(0.25f, 0.25f, 0.25f, 1f));
		}
		else
		{
			component.SetActive(Active: false);
			component2.SetActive(Active: false);
			component3.SetColour(new Color(1f, 1f, 1f, 1f));
		}
		foreach (ResearchTechnology item in m_Technology)
		{
			item.UpdateLocked(Force);
		}
	}

	public ResearchInfo GetResearchFromQuest(Quest.ID NewID)
	{
		foreach (ResearchInfo researchInfo in m_LevelInfo.m_ResearchInfos)
		{
			if (researchInfo.m_ID == NewID)
			{
				return researchInfo;
			}
		}
		return null;
	}

	public void SetTechnologySelected(Quest.ID NewID, bool Selected)
	{
		foreach (ResearchTechnology item in m_Technology)
		{
			if (item.m_Quest.m_ID == NewID)
			{
				item.SetSelected(Selected);
			}
			else
			{
				item.SetSelected(Selected: false);
			}
		}
	}

	public ResearchTechnology GetTechnologyFromQuest(Quest NewQuest)
	{
		foreach (ResearchTechnology item in m_Technology)
		{
			if (item.m_Quest == NewQuest)
			{
				return item;
			}
		}
		return null;
	}
}
