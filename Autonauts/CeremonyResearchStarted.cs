using System.Collections.Generic;
using UnityEngine;

public class CeremonyResearchStarted : CeremonyTitleBase
{
	private List<ResearchRollover> m_Rollovers;

	private NewThing m_New;

	private void Awake()
	{
		AudioManager.Instance.StartEvent("CeremonyQuestEnded");
	}

	private void Start()
	{
	}

	private void CreateRollovers()
	{
		ResearchRollover component = base.transform.Find("ResearchRollover").GetComponent<ResearchRollover>();
		component.gameObject.SetActive(value: false);
		List<Quest> list = new List<Quest>();
		foreach (Quest.ID item in m_Quest.m_QuestsUnlocked)
		{
			Quest quest = QuestManager.Instance.GetQuest(item);
			if (quest.m_Type == Quest.Type.Research)
			{
				list.Add(quest);
			}
		}
		Vector2 anchoredPosition = component.GetComponent<RectTransform>().anchoredPosition;
		Transform parent = base.transform;
		float num = 20f;
		float num2 = 20f;
		float num3 = anchoredPosition.x;
		float num4 = anchoredPosition.y + (float)list.Count * num2 / 2f;
		m_Rollovers = new List<ResearchRollover>();
		foreach (Quest item2 in list)
		{
			ResearchRollover researchRollover = Object.Instantiate(component, parent);
			researchRollover.gameObject.SetActive(value: true);
			researchRollover.ForceOpen();
			researchRollover.SetResearchTarget(item2);
			researchRollover.GetComponent<RectTransform>().anchoredPosition = new Vector2(num3, num4);
			BaseButtonImage component2 = researchRollover.transform.Find("BasePanel").Find("StandardAcceptButton").GetComponent<BaseButtonImage>();
			component2.SetAction(OnAcceptClicked, component2);
			m_Rollovers.Add(researchRollover);
			num3 += num;
			num4 -= num2;
		}
	}

	protected void CreateNew()
	{
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/NewThing", typeof(GameObject));
		m_New = Object.Instantiate(original, default(Vector3), Quaternion.identity, base.transform).GetComponent<NewThing>();
		m_New.transform.localScale = new Vector3(3f, 3f, 3f);
	}

	protected void AttachNewToTopBlueprint()
	{
		ResearchRollover researchRollover = m_Rollovers[m_Rollovers.Count - 1];
		m_New.transform.SetParent(researchRollover.transform.Find("BasePanel"));
		RectTransform component = m_New.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0f, 1f);
		component.anchorMax = new Vector2(0f, 1f);
		component.pivot = new Vector2(0f, 1f);
		component.anchoredPosition = new Vector2(-10f, 10f);
	}

	public override void SetQuest(Quest NewQuest, List<ObjectType> UnlockedObjects)
	{
		base.SetQuest(NewQuest, UnlockedObjects);
		CreateRollovers();
		CreateNew();
		AttachNewToTopBlueprint();
	}

	private void End()
	{
		Object.Destroy(base.gameObject);
		CeremonyManager.Instance.CeremonyEnded();
	}

	public void OnAcceptClicked(BaseGadget NewGadget)
	{
		if (m_Rollovers.Count != 0)
		{
			m_New.transform.SetParent(base.transform);
			ResearchRollover researchRollover = m_Rollovers[m_Rollovers.Count - 1];
			m_Rollovers.Remove(researchRollover);
			Object.Destroy(researchRollover.gameObject);
		}
		if (m_Rollovers.Count == 0)
		{
			End();
		}
		else
		{
			AttachNewToTopBlueprint();
		}
	}

	private void Update()
	{
	}
}
