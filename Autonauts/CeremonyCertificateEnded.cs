using System.Collections.Generic;
using UnityEngine;

public class CeremonyCertificateEnded : CeremonyTitleBase
{
	private enum State
	{
		ShowQuest,
		ShowSpeech,
		ShowStamp,
		Idle,
		Total
	}

	private static float m_RolloverScale = 1.5f;

	private State m_State;

	private float m_StateTimer;

	private Rollover m_QuestRollover;

	private BaseImage m_Tick;

	private bool m_TickSoundPlayed;

	private Vector3 m_RolloverStartPosition;

	private bool m_SkipBuffer;

	private CeremonySpeech m_Speech;

	private void Awake()
	{
		AudioManager.Instance.StartEvent("CeremonyQuestEnded");
		m_Speech = base.transform.Find("SpeechPanel").GetComponent<CeremonySpeech>();
		m_Speech.GetButton().SetAction(OnAcceptClicked, m_Speech.GetButton());
		m_Speech.SetActive(Active: false);
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
		Transform parent = m_QuestRollover.transform.Find("BasePanel");
		BaseButtonImage component = Object.Instantiate((GameObject)Resources.Load("Prefabs/Hud/Standard/StandardAcceptButton", typeof(GameObject)), parent).GetComponent<BaseButtonImage>();
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
		Transform obj = m_QuestRollover.transform.Find("BasePanel");
		Vector2 sizeDelta = obj.GetComponent<RectTransform>().sizeDelta;
		sizeDelta.y += 50f;
		obj.GetComponent<RectTransform>().sizeDelta = sizeDelta;
	}

	public override void SetQuest(Quest NewQuest, List<ObjectType> UnlockedObjects)
	{
		base.SetQuest(NewQuest, UnlockedObjects);
		SetState(State.ShowQuest);
		CreateQuestRollover();
	}

	protected virtual void EndPlans()
	{
		Object.Destroy(base.gameObject);
		CeremonyManager.Instance.CeremonyEnded(CheckForMoreCeremonies: false);
		if (CeremonyManager.Instance.m_GluePlaying)
		{
			CeremonyManager.Instance.DoRevealCertificate(m_Quest, m_UnlockedObjects);
		}
		else
		{
			CeremonyManager.Instance.DoCertificateAcademyEnded(m_Quest, m_UnlockedObjects);
		}
	}

	public void OnAcceptClicked(BaseGadget NewGadget)
	{
		if (NewGadget == m_Speech.GetButton())
		{
			SetState(State.ShowStamp);
		}
		else
		{
			EndPlans();
		}
	}

	private void SetState(State NewState)
	{
		State state = m_State;
		if (state == State.ShowSpeech)
		{
			m_Speech.SetActive(Active: false);
		}
		m_State = NewState;
		m_StateTimer = 0f;
		switch (m_State)
		{
		case State.ShowSpeech:
			m_Speech.SetActive(Active: true);
			m_Speech.SetSpeechFromID("CeremonyQuestCompleteSpeech");
			break;
		case State.ShowStamp:
			m_Tick.SetActive(Active: true);
			m_Tick.transform.localScale = new Vector3(4f, 4f);
			m_TickSoundPlayed = false;
			break;
		case State.Idle:
			CreateAcceptButton();
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
			SetState(State.Idle);
		}
	}

	public override void Skip()
	{
		base.Skip();
		SetState(State.Idle);
		m_Tick.SetActive(Active: true);
		m_Tick.transform.localScale = new Vector3(1f, 1f, 1f);
		if (m_UnlockedObjects.Count != 0)
		{
			m_QuestRollover.GetComponent<RectTransform>().anchoredPosition = new Vector2(-200f, 0f);
		}
	}

	public bool IsPlaying()
	{
		if (m_State == State.Idle)
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
				if (m_Quest.m_ID == Quest.ID.AcademyBasics)
				{
					SetState(State.ShowSpeech);
				}
				else
				{
					SetState(State.ShowStamp);
				}
			}
			break;
		case State.ShowStamp:
			UpdateShowStamp();
			break;
		}
		m_StateTimer += TimeManager.Instance.m_NormalDelta;
	}
}
