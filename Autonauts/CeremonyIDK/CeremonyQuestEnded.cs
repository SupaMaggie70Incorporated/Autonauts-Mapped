using System.Collections.Generic;
using UnityEngine;

public class CeremonyQuestEnded : CeremonyTitleBase
{
	private enum State
	{
		ShowQuest,
		ShowStamp,
		QuestMove,
		RevealBlueprints,
		Idle,
		Total
	}

	private static float m_RolloverScale = 1.5f;

	private State m_State;

	private float m_StateTimer;

	private Rollover m_QuestRollover;

	private BaseImage m_Tick;

	private List<CeremonyBlueprint> m_Blueprints;

	private NewThing m_New;

	private int m_CurrentBlueprint;

	private bool m_TickSoundPlayed;

	private Vector3 m_RolloverStartPosition;

	private bool m_SkipBuffer;

	private void Awake()
	{
		AudioManager.Instance.StartEvent("CeremonyQuestEnded");
	}

	private void Start()
	{
	}

	private void CreateAcceptButton()
	{
		Transform obj = base.transform.Find("RolloverAnchor");
		Vector3 localPosition = obj.transform.localPosition;
		localPosition.x = 0f;
		obj.transform.localPosition = localPosition;
		Transform transform = m_QuestRollover.transform.Find("BasePanel");
		Vector2 sizeDelta = transform.GetComponent<RectTransform>().sizeDelta;
		sizeDelta.y += 50f;
		transform.GetComponent<RectTransform>().sizeDelta = sizeDelta;
		BaseButtonImage component = Object.Instantiate((GameObject)Resources.Load("Prefabs/Hud/Standard/StandardAcceptButton", typeof(GameObject)), transform).GetComponent<BaseButtonImage>();
		component.SetAction(OnAcceptClicked, component);
		RectTransform component2 = component.GetComponent<RectTransform>();
		component2.anchorMin = new Vector2(1f, 0f);
		component2.anchorMax = new Vector2(1f, 0f);
		component2.pivot = new Vector2(1f, 0f);
		component2.anchoredPosition = new Vector2(-10f, 10f);
		float num = 2f / 3f;
		component2.transform.localScale = new Vector3(num, num, num);
	}

	private void CreateQuestRollover()
	{
		string text = "QuestRollover";
		if (m_Quest.m_Type == Quest.Type.Research)
		{
			text = "ResearchRollover";
		}
		Transform parent = base.transform;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Rollovers/" + text, typeof(GameObject));
		m_QuestRollover = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, parent).GetComponent<Rollover>();
		m_Tick = m_QuestRollover.transform.Find("BasePanel/ApprovedTick").GetComponent<BaseImage>();
		RectTransform component = m_QuestRollover.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0.5f, 0.5f);
		component.anchorMax = new Vector2(0.5f, 0.5f);
		component.pivot = new Vector2(0.5f, 0.5f);
		component.localScale = new Vector3(m_RolloverScale, m_RolloverScale);
		m_QuestRollover.ForceOpen();
		if (m_Quest.m_Type != Quest.Type.Research)
		{
			m_QuestRollover.GetComponent<QuestRollover>().SetTarget(m_Quest);
			m_QuestRollover.GetComponent<QuestRollover>().SetComplete();
		}
		else
		{
			m_QuestRollover.GetComponent<ResearchRollover>().SetResearchTarget(m_Quest);
		}
		component.anchoredPosition = new Vector2(0f, 0f);
		m_Tick.SetActive(Active: false);
		if (m_UnlockedObjects.Count == 0)
		{
			CreateAcceptButton();
		}
	}

	protected void CreateBlueprints()
	{
		CeremonyBlueprint component = base.transform.Find("CeremonyBlueprint").GetComponent<CeremonyBlueprint>();
		component.gameObject.SetActive(value: false);
		Vector2 anchoredPosition = component.GetComponent<RectTransform>().anchoredPosition;
		Transform parent = base.transform;
		List<ObjectType> unlockedObjects = m_UnlockedObjects;
		float num = 20f;
		float num2 = 20f;
		if (unlockedObjects.Count > 8)
		{
			num = 15f;
			num2 = 15f;
		}
		float num3 = anchoredPosition.x;
		float num4 = anchoredPosition.y + (float)unlockedObjects.Count * num2 / 2f;
		m_Blueprints = new List<CeremonyBlueprint>();
		foreach (ObjectType item in unlockedObjects)
		{
			CeremonyBlueprint ceremonyBlueprint = Object.Instantiate(component, parent);
			ceremonyBlueprint.gameObject.SetActive(value: true);
			ceremonyBlueprint.GetComponent<RectTransform>().anchoredPosition = new Vector2(num3, num4);
			ceremonyBlueprint.Init(item, OnAcceptClicked);
			ceremonyBlueprint.gameObject.SetActive(value: false);
			ceremonyBlueprint.m_Button.SetActive(Active: false);
			m_Blueprints.Add(ceremonyBlueprint);
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
		CeremonyBlueprint ceremonyBlueprint = m_Blueprints[m_Blueprints.Count - 1];
		m_New.transform.SetParent(ceremonyBlueprint.transform.Find("BasePanel"));
		RectTransform component = m_New.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0f, 1f);
		component.anchorMax = new Vector2(0f, 1f);
		component.pivot = new Vector2(0f, 1f);
		component.anchoredPosition = new Vector2(-10f, 10f);
	}

	public override void SetQuest(Quest NewQuest, List<ObjectType> UnlockedObjects)
	{
		base.SetQuest(NewQuest, UnlockedObjects);
		SetState(State.ShowQuest);
		CreateBlueprints();
		CreateQuestRollover();
		if (m_UnlockedObjects.Count > 0)
		{
			CreateNew();
			AttachNewToTopBlueprint();
		}
	}

	protected virtual void EndPlans()
	{
		Object.Destroy(base.gameObject);
		CeremonyManager.Instance.CeremonyEnded();
	}

	public void OnAcceptClicked(BaseGadget NewGadget)
	{
		if (m_Blueprints.Count != 0)
		{
			m_New.transform.SetParent(base.transform);
			CeremonyBlueprint ceremonyBlueprint = m_Blueprints[m_Blueprints.Count - 1];
			m_Blueprints.Remove(ceremonyBlueprint);
			Object.Destroy(ceremonyBlueprint.gameObject);
		}
		if (m_Blueprints.Count == 0)
		{
			EndPlans();
		}
		else
		{
			AttachNewToTopBlueprint();
		}
	}

	private void SetState(State NewState)
	{
		m_State = NewState;
		m_StateTimer = 0f;
		switch (m_State)
		{
		case State.QuestMove:
		{
			Swoosher swoosher = m_QuestRollover.gameObject.AddComponent<Swoosher>();
			Vector3 localPosition = m_QuestRollover.transform.localPosition;
			swoosher.StartSwoosh(localPosition, localPosition + new Vector3(-200f, 0f), m_RolloverScale, m_RolloverScale, 0.1f, 0f, null, Local: true);
			AudioManager.Instance.StartEvent("CeremonyBlueprintSwoosh");
			break;
		}
		case State.ShowStamp:
			m_Tick.SetActive(Active: true);
			m_Tick.transform.localScale = new Vector3(4f, 4f);
			m_TickSoundPlayed = false;
			break;
		}
	}

	private void UpdateShowStamp()
	{
		float num = m_StateTimer / 0.1f;
		if (num > 1f)
		{
			num = 1f;
			if (!m_TickSoundPlayed)
			{
				m_TickSoundPlayed = true;
				AudioManager.Instance.StartEvent("CeremonyTick");
			}
		}
		float num2 = (1f - num) * 3f + 1f;
		m_Tick.transform.localScale = new Vector3(num2, num2);
		if (m_StateTimer > 0.5f)
		{
			if (m_UnlockedObjects.Count == 0)
			{
				SetState(State.Idle);
			}
			else
			{
				SetState(State.QuestMove);
			}
		}
	}

	private void UpdateRevealBlueprints()
	{
		if (m_StateTimer > 0.1f)
		{
			m_StateTimer = 0f;
			if (m_CurrentBlueprint >= m_Blueprints.Count)
			{
				SetState(State.Idle);
				return;
			}
			CeremonyBlueprint ceremonyBlueprint = m_Blueprints[m_CurrentBlueprint];
			ceremonyBlueprint.gameObject.SetActive(value: true);
			ceremonyBlueprint.m_Button.SetActive(Active: true);
			Swoosher swoosher = ceremonyBlueprint.gameObject.AddComponent<Swoosher>();
			Vector3 localPosition = m_QuestRollover.transform.localPosition;
			Vector3 localPosition2 = ceremonyBlueprint.transform.localPosition;
			swoosher.StartSwoosh(localPosition, localPosition2, 0.5f, 1f, 0.1f, 0f, null, Local: true);
			AudioManager.Instance.StartEvent("CeremonyBlueprintSwoosh");
			m_CurrentBlueprint++;
		}
	}

	public override void Skip()
	{
		base.Skip();
		SetState(State.Idle);
		foreach (CeremonyBlueprint blueprint in m_Blueprints)
		{
			blueprint.gameObject.SetActive(value: true);
			blueprint.m_Button.SetActive(Active: true);
		}
		m_Tick.SetActive(Active: true);
		m_Tick.transform.localScale = new Vector3(1f, 1f, 1f);
		if (m_UnlockedObjects.Count != 0)
		{
			m_QuestRollover.GetComponent<RectTransform>().anchoredPosition = new Vector2(-200f, 0f);
		}
	}

	public bool IsPlaying()
	{
		if (m_State == State.Idle || m_State == State.RevealBlueprints)
		{
			return false;
		}
		return true;
	}

	private void Update()
	{
		if (IsPlaying())
		{
			if (m_SkipBuffer)
			{
				Skip();
				m_SkipBuffer = false;
			}
			else if (MyInputManager.m_Rewired.GetButtonDown("Accept"))
			{
				m_SkipBuffer = true;
			}
		}
		switch (m_State)
		{
		case State.ShowQuest:
			if (m_StateTimer > 0.5f)
			{
				SetState(State.ShowStamp);
			}
			break;
		case State.ShowStamp:
			UpdateShowStamp();
			break;
		case State.QuestMove:
			if (m_StateTimer > 0.25f)
			{
				SetState(State.RevealBlueprints);
			}
			break;
		case State.RevealBlueprints:
			UpdateRevealBlueprints();
			break;
		}
		m_StateTimer += TimeManager.Instance.m_NormalDelta;
	}
}
