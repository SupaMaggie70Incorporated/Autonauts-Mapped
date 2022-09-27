using System.Collections.Generic;
using UnityEngine;

public class QuestCompleteRollover : Rollover
{
	private Quest m_Quest;

	private QuestRollover m_QuestRollover;

	private float m_UnlockY;

	private BaseText m_Title;

	private QuestObjectUnlocked m_DefaultObject;

	private List<QuestObjectUnlocked> m_Unlocks;

	protected new void Awake()
	{
		base.Awake();
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Rollovers/QuestRollover", typeof(GameObject));
		m_QuestRollover = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, m_Panel.transform).GetComponent<QuestRollover>();
		m_QuestRollover.transform.localPosition = default(Vector3);
		m_Title = m_Panel.transform.Find("Title").GetComponent<BaseText>();
		m_DefaultObject = m_Panel.transform.Find("QuestObjectUnlocked").GetComponent<QuestObjectUnlocked>();
		m_DefaultObject.gameObject.SetActive(value: false);
	}

	private void OnDestroy()
	{
		Object.Destroy(m_QuestRollover.gameObject);
		m_QuestRollover = null;
	}

	private void AddUnlockedObject(ObjectType NewType, float Spacing)
	{
		Transform parent = m_Panel.transform;
		QuestObjectUnlocked component = Object.Instantiate(m_DefaultObject, new Vector3(0f, 0f, 0f), Quaternion.identity, parent).GetComponent<QuestObjectUnlocked>();
		component.gameObject.SetActive(value: true);
		component.transform.localPosition = new Vector3(0f, m_UnlockY, 0f);
		component.SetIngredient(NewType, WhiteText: false);
		m_Unlocks.Add(component);
		m_UnlockY -= Spacing;
	}

	private void SetupQuest()
	{
		if (m_Unlocks != null)
		{
			foreach (QuestObjectUnlocked unlock in m_Unlocks)
			{
				Object.Destroy(unlock.gameObject);
			}
		}
		m_Unlocks = new List<QuestObjectUnlocked>();
		float num = 25f;
		Vector2 sizeDelta = m_Panel.GetComponent<RectTransform>().sizeDelta;
		m_UnlockY = sizeDelta.y / 2f - 22f;
		int num2 = 0;
		if (m_Quest.m_BuildingsUnlocked != null)
		{
			foreach (ObjectType item in m_Quest.m_BuildingsUnlocked)
			{
				AddUnlockedObject(item, num);
			}
			num2 += m_Quest.m_BuildingsUnlocked.Count;
		}
		if (m_Quest.m_ObjectsUnlocked != null)
		{
			foreach (ObjectType item2 in m_Quest.m_ObjectsUnlocked)
			{
				AddUnlockedObject(item2, num);
			}
			num2 += m_Quest.m_ObjectsUnlocked.Count;
		}
		m_Title.SetActive(num2 == 0);
		if (num2 == 0)
		{
			sizeDelta.y = 50f;
		}
		else
		{
			sizeDelta.y = (float)num2 * num + 20f;
		}
		m_Panel.GetComponent<RectTransform>().sizeDelta = sizeDelta;
	}

	public void SetTarget(Quest Target)
	{
		m_Quest = Target;
		m_QuestRollover.UpdateWhilePaused(m_Wobbler.m_WobbleWhilePaused);
		m_QuestRollover.SetTarget(m_Quest);
		m_QuestRollover.ForceOpen();
		Vector2 sizeDelta = m_QuestRollover.m_Panel.GetComponent<RectTransform>().sizeDelta;
		RectTransform component = m_QuestRollover.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(1f, 1f);
		component.anchorMax = new Vector2(1f, 1f);
		component.anchoredPosition = new Vector3((0f - sizeDelta.x) / 2f, sizeDelta.y / 2f + 10f);
		if (m_Quest != null)
		{
			SetupQuest();
		}
	}

	protected override bool GetTargettingSomething()
	{
		return m_Quest != null;
	}
}
