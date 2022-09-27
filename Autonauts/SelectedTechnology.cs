using System.Collections.Generic;
using UnityEngine;

public class SelectedTechnology : BasePanel
{
	private Quest m_Quest;

	private StandardProgressBar m_ProgressBar;

	private List<SelectedUnlockedObject> m_UnlockedObjects;

	private List<SelectedUnlockedObject> m_RequiredObjects;

	private float m_StartingHeight;

	protected new void Awake()
	{
		base.Awake();
		m_UnlockedObjects = new List<SelectedUnlockedObject>();
		m_RequiredObjects = new List<SelectedUnlockedObject>();
		m_StartingHeight = GetHeight();
	}

	public void SetQuest(Quest.ID NewID)
	{
		Quest quest = (m_Quest = QuestManager.Instance.GetQuest(NewID));
		base.transform.Find("TitleStrip/Title").GetComponent<BaseText>().SetTextFromID(quest.m_Title);
		base.transform.Find("TitleStrip/Description").GetComponent<BaseText>().SetTextFromID(quest.m_Description);
		BaseText component = base.transform.Find("HeartText").GetComponent<BaseText>();
		string text = GeneralUtils.FormatBigInt(quest.m_EventsRequired[0].m_Required);
		component.SetText(text);
		m_ProgressBar = base.transform.Find("StandardProgressBar").GetComponent<StandardProgressBar>();
		CreateUnlockedObjects();
		CreateRequiredObjects();
		UpdateLocked();
		UpdateProgressBar();
		UpdateImportant();
		BaseButtonBack component2 = base.transform.Find("StandardCancelButton").GetComponent<BaseButtonBack>();
		component2.SetAction(OnBackClicked, component2);
	}

	private void DestroyObjects()
	{
		foreach (SelectedUnlockedObject unlockedObject in m_UnlockedObjects)
		{
			Object.Destroy(unlockedObject.gameObject);
		}
		m_UnlockedObjects.Clear();
		foreach (SelectedUnlockedObject requiredObject in m_RequiredObjects)
		{
			Object.Destroy(requiredObject.gameObject);
		}
		m_RequiredObjects.Clear();
	}

	public void OnBackClicked(BaseGadget NewGadget)
	{
		DestroyObjects();
		Research.Instance.TechnologySelected(null);
	}

	private void UpdateLocked()
	{
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

	private void UpdateImportant()
	{
		GameObject gameObject = base.transform.Find("Important").gameObject;
		bool active = false;
		foreach (ObjectType item in m_Quest.m_BuildingsUnlocked)
		{
			if (ResearchTechnology.GetIsObjecTypeImportant(item))
			{
				active = true;
			}
		}
		foreach (ObjectType item2 in m_Quest.m_ObjectsUnlocked)
		{
			if (ResearchTechnology.GetIsObjecTypeImportant(item2))
			{
				active = true;
			}
		}
		gameObject.SetActive(active);
	}

	private void CreateUnlockedObjects()
	{
		GameObject gameObject = base.transform.Find("UnlockedObject").gameObject;
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
		float x = gameObject.GetComponent<RectTransform>().sizeDelta.x;
		float num = x + 10f;
		int num2 = 4;
		float num3 = 450f;
		if (list.Count > 12)
		{
			num2++;
			num3 += x;
			if (list.Count > 15)
			{
				num2++;
				num3 += x;
			}
		}
		SetWidth(num3);
		float num4 = m_StartingHeight;
		if (list.Count > num2)
		{
			num4 += (float)((list.Count - 1) / num2) * num;
		}
		SetHeight(num4);
		float num5 = 0f - (float)(num2 - 1) * num / 2f;
		if (list.Count < num2)
		{
			num5 = 0f - (float)(list.Count - 1) * num / 2f;
		}
		float y = gameObject.GetComponent<RectTransform>().anchoredPosition.y;
		int num6 = 0;
		foreach (ObjectType item3 in list)
		{
			SelectedUnlockedObject component = Object.Instantiate(gameObject, base.transform).GetComponent<SelectedUnlockedObject>();
			component.SetActive(Active: true);
			component.SetObjectType(item3);
			float x2 = num5 + (float)(num6 % num2) * num;
			float y2 = y - (float)(num6 / num2) * num;
			component.GetComponent<RectTransform>().anchoredPosition = new Vector3(x2, y2);
			m_UnlockedObjects.Add(component);
			num6++;
		}
	}

	private void CreateRequiredObjects()
	{
		GameObject gameObject = base.transform.Find("RequiredObject").gameObject;
		gameObject.SetActive(value: false);
		List<ObjectType> list = new List<ObjectType>();
		list.Add(m_Quest.m_ObjectTypeRequired);
		float num = gameObject.GetComponent<RectTransform>().sizeDelta.x + 10f;
		float num2 = gameObject.GetComponent<RectTransform>().anchoredPosition.x;
		float y = gameObject.GetComponent<RectTransform>().anchoredPosition.y;
		foreach (ObjectType item in list)
		{
			SelectedUnlockedObject component = Object.Instantiate(gameObject, base.transform).GetComponent<SelectedUnlockedObject>();
			component.SetActive(Active: true);
			component.SetObjectType(item);
			component.GetComponent<RectTransform>().anchoredPosition = new Vector3(num2, y);
			num2 -= num;
			m_RequiredObjects.Add(component);
		}
	}
}
