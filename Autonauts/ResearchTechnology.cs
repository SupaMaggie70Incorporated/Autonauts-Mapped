using System.Collections.Generic;
using UnityEngine;

public class ResearchTechnology : BasePanel
{
	public Quest m_Quest;

	private StandardProgressBar m_ProgressBar;

	private BaseImage m_Complete;

	private float m_SelectedTimer;

	private GameObject m_LockedPanel;

	public void SetQuest(Quest.ID NewID)
	{
		Quest quest = (m_Quest = QuestManager.Instance.GetQuest(NewID));
		base.transform.Find("TitleStrip/Title").GetComponent<BaseText>().SetTextFromID(quest.m_Title);
		BaseText component = base.transform.Find("HeartText").GetComponent<BaseText>();
		string text = GeneralUtils.FormatBigInt(quest.m_EventsRequired[0].m_Required);
		component.SetText(text);
		base.transform.Find("Description").GetComponent<BaseText>().SetTextFromID(quest.m_Description);
		m_ProgressBar = base.transform.Find("StandardProgressBar").GetComponent<StandardProgressBar>();
		m_Complete = base.transform.Find("Complete").GetComponent<BaseImage>();
		m_LockedPanel = base.transform.Find("LockedPanel").gameObject;
		UpdateLocked();
		UpdateProgressBar();
		CreateUnlockedObjects();
		UpdateComplete();
		UpdateImportant();
	}

	public void UpdateLocked(bool Force = false)
	{
		if (!m_Quest.m_Started || Force)
		{
			m_LockedPanel.SetActive(value: true);
		}
		else
		{
			m_LockedPanel.SetActive(value: false);
		}
	}

	private void UpdateProgressBar()
	{
		if (m_Quest.m_Started)
		{
			if (m_Quest.GetCompletePercent() != 0f)
			{
				m_ProgressBar.SetActive(Active: true);
				m_ProgressBar.SetValue(m_Quest.GetCompletePercent());
			}
			else
			{
				m_ProgressBar.SetActive(Active: false);
			}
		}
		else
		{
			m_ProgressBar.SetActive(Active: false);
		}
	}

	private void CreateUnlockedObjects()
	{
		GameObject gameObject = base.transform.Find("UnlockedObjects/UnlockedObject").gameObject;
		gameObject.SetActive(value: false);
		List<ObjectType> list = new List<ObjectType>();
		foreach (ObjectType item in m_Quest.m_BuildingsUnlocked)
		{
			list.Add(item);
		}
		foreach (ObjectType item2 in m_Quest.m_ObjectsUnlocked)
		{
			if (QuestData.DoesUnlockedObjectHaveCeremony(item2))
			{
				list.Add(item2);
			}
		}
		float num = gameObject.GetComponent<RectTransform>().sizeDelta.x + 10f;
		if (list.Count > 3)
		{
			list.RemoveRange(3, list.Count - 3);
		}
		float num2 = 0f - (float)(list.Count - 1) * num / 2f;
		float y = gameObject.GetComponent<RectTransform>().anchoredPosition.y;
		Transform parent = base.transform.Find("UnlockedObjects");
		foreach (ObjectType item3 in list)
		{
			BaseImage component = Object.Instantiate(gameObject, parent).GetComponent<BaseImage>();
			component.SetActive(Active: true);
			Sprite icon = IconManager.Instance.GetIcon(item3);
			component.SetSprite(icon);
			component.GetComponent<RectTransform>().anchoredPosition = new Vector3(num2, y);
			num2 += num;
		}
	}

	private void UpdateComplete()
	{
		m_Complete.SetActive(m_Quest.GetIsComplete());
	}

	public void ForceComplete(bool Complete)
	{
		m_Complete.SetActive(Complete);
	}

	private void UpdateImportant()
	{
		BaseImage component = base.transform.Find("Important").GetComponent<BaseImage>();
		bool active = false;
		foreach (ObjectType item in m_Quest.m_BuildingsUnlocked)
		{
			if (GetIsObjecTypeImportant(item))
			{
				active = true;
			}
		}
		foreach (ObjectType item2 in m_Quest.m_ObjectsUnlocked)
		{
			if (GetIsObjecTypeImportant(item2))
			{
				active = true;
			}
		}
		component.SetActive(active);
		if (m_Quest.m_Started)
		{
			component.SetColour(new Color(1f, 1f, 1f, 1f));
		}
		else
		{
			component.SetColour(new Color(0.5f, 0.5f, 0.5f, 1f));
		}
	}

	public static bool GetIsObjecTypeImportant(ObjectType NewType)
	{
		if (Housing.GetIsTypeHouse(NewType) || Food.GetIsTypeFood(NewType) || Clothing.GetIsTypeClothing(NewType) || Toy.GetIsTypeToy(NewType) || Medicine.GetIsTypeMedicine(NewType) || Education.GetIsTypeEducation(NewType) || Art.GetIsTypeArt(NewType))
		{
			return true;
		}
		return false;
	}

	public override void SetIndicated(bool Indicated)
	{
		base.SetIndicated(Indicated);
		if (Indicated)
		{
			HudManager.Instance.ActivateResearchRollover(Activate: true, m_Quest);
		}
		else
		{
			HudManager.Instance.ActivateResearchRollover(Activate: false, (Quest)null);
		}
	}

	public override void SetSelected(bool Selected)
	{
		base.SetSelected(Selected);
		if (Selected)
		{
			m_SelectedTimer = 1.5f;
			return;
		}
		m_SelectedTimer = 0f;
		UpdateLocked();
	}

	private void Update()
	{
		if (!(m_SelectedTimer > 0f))
		{
			return;
		}
		m_SelectedTimer -= TimeManager.Instance.m_PauseDelta;
		if (m_SelectedTimer <= 0f)
		{
			m_SelectedTimer = 0f;
			UpdateLocked();
			return;
		}
		bool active = true;
		if ((int)(m_SelectedTimer * 60f) % 12 < 6)
		{
			active = false;
		}
		m_LockedPanel.SetActive(active);
	}
}
