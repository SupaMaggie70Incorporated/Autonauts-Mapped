using System.Collections.Generic;
using UnityEngine;

public class CeremonyRevealCertificate : CeremonyTitleBase
{
	private enum State
	{
		PanTo,
		Transmitter,
		Idle,
		Total
	}

	private State m_State;

	private float m_StateTimer;

	private CeremonySpeech m_Speech;

	private Transmitter m_Transmitter;

	private void Awake()
	{
		m_Speech = base.transform.Find("SpeechPanel").GetComponent<CeremonySpeech>();
		m_Speech.GetButton().SetAction(OnAcceptClicked, null);
		Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("Transmitter");
		if (collection == null)
		{
			return;
		}
		using Dictionary<BaseClass, int>.Enumerator enumerator = collection.GetEnumerator();
		if (enumerator.MoveNext())
		{
			m_Transmitter = enumerator.Current.Key.GetComponent<Transmitter>();
		}
	}

	private void Start()
	{
	}

	private void End()
	{
		Object.Destroy(base.gameObject);
		CeremonyManager.Instance.CeremonyEnded(CheckForMoreCeremonies: false);
		if (CeremonyManager.Instance.m_GluePlaying)
		{
			CeremonyManager.Instance.DoCertificateAcademyEnded(m_Quest, m_UnlockedObjects);
		}
		if (m_Quest.m_ID == Quest.ID.AcademyColonisation6)
		{
			CeremonyManager.Instance.StartImmediateCeremony(CeremonyManager.CeremonyType.PhaseTwoComplete, m_Quest, null);
		}
		if (m_Quest.m_ID == Quest.ID.AcademyColonisation7)
		{
			CeremonyManager.Instance.StartImmediateCeremony(CeremonyManager.CeremonyType.PhaseThreeComplete, m_Quest, null);
		}
	}

	public override void SetQuest(Quest NewQuest, List<ObjectType> UnlockedObjects)
	{
		base.SetQuest(NewQuest, UnlockedObjects);
		string val = TextManager.Instance.Get(m_Quest.m_Title);
		string speech = TextManager.Instance.Get("CeremonyRevealCertificate", val);
		m_Speech.SetSpeech(speech);
		m_Speech.GetButton().SetActive(Active: false);
		SetState(State.PanTo);
	}

	public void OnAcceptClicked(BaseGadget NewGadget)
	{
		m_Transmitter.SetReceiving(Receiving: false);
		ReturnPanTo(1f);
		End();
	}

	private void SetState(State NewState)
	{
		State state = m_State;
		if (state == State.Idle)
		{
			m_Transmitter.SetReceiving(Receiving: false);
		}
		m_State = NewState;
		m_StateTimer = 0f;
		switch (m_State)
		{
		case State.PanTo:
			PanTo(m_Transmitter, new Vector3(0f, 4f, 0f), 20f, 1f);
			m_Transmitter.SetReceiving(Receiving: true);
			break;
		case State.Idle:
			m_Speech.GetButton().SetActive(Active: true);
			break;
		case State.Transmitter:
			break;
		}
	}

	private void CreateReward()
	{
		if ((bool)m_Transmitter)
		{
			m_Transmitter.UpdateRewards(ShowCeremony: true);
		}
	}

	private void Update()
	{
		switch (m_State)
		{
		case State.PanTo:
			if (!CameraManager.Instance.IsPanning())
			{
				SetState(State.Transmitter);
			}
			break;
		case State.Transmitter:
			if (m_StateTimer > 1f)
			{
				CreateReward();
				SetState(State.Idle);
			}
			break;
		}
		m_StateTimer += TimeManager.Instance.m_NormalDelta;
	}
}
