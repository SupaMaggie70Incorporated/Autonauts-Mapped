using System.Collections.Generic;
using UnityEngine;

public class CeremonyResearchLevelUnlocked : CeremonyBase
{
	private enum State
	{
		FirstTime,
		Unlock,
		Wait,
		Total
	}

	private State m_State;

	private float m_StateTimer;

	private ResearchLevel m_ResearchLevel;

	private CeremonySpeech m_Speech;

	private void Awake()
	{
		m_Speech = base.transform.Find("SpeechPanel").GetComponent<CeremonySpeech>();
		m_Speech.GetButton().SetAction(OnAcceptClicked, null);
		AudioManager.Instance.StartEvent("CeremonyResearchLevelUnlocked");
	}

	public override void SetQuest(Quest NewQuest, List<ObjectType> UnlockedObjects)
	{
		base.SetQuest(NewQuest, UnlockedObjects);
		m_ResearchLevel = Research.Instance.GetLevelFromQuest(NewQuest);
		m_ResearchLevel.UpdateLocked(Force: true);
		if (m_Quest == null)
		{
			SetState(State.FirstTime);
		}
		else
		{
			SetState(State.Unlock);
		}
	}

	private void End()
	{
		GameStateAutopedia.Instance.CeremonyPlaying(Playing: false, null);
		SetState(State.Total);
		Object.Destroy(base.gameObject);
		CeremonyManager.Instance.CeremonyEnded(CheckForMoreCeremonies: false);
	}

	private void SetState(State NewState)
	{
		if (m_State == State.FirstTime)
		{
			TutorialPointerManager.Instance.CeremonyActive(Active: true);
			TutorialPointerManager.Instance.SetType(TutorialPointerManager.Type.Total);
		}
		m_State = NewState;
		m_StateTimer = 0f;
		switch (m_State)
		{
		case State.FirstTime:
			m_Speech.SetSpeechFromID("CeremonyResearchLevelUnlockedFirstTime");
			TutorialPointerManager.Instance.CeremonyActive(Active: false);
			TutorialPointerManager.Instance.SetType(TutorialPointerManager.Type.AutopediaResearch);
			break;
		case State.Wait:
			m_ResearchLevel.UpdateLocked();
			if (m_Quest == null)
			{
				m_Speech.SetSpeechFromID("CeremonyResearchLevelUnlockedFirstSpeech");
			}
			else
			{
				m_Speech.SetSpeechFromID("CeremonyResearchLevelUnlockedSpeech");
			}
			break;
		}
	}

	public void OnAcceptClicked(BaseGadget NewGadget)
	{
		if (m_State == State.FirstTime)
		{
			SetState(State.Wait);
		}
		else if (m_State == State.Wait)
		{
			End();
		}
	}

	private void Update()
	{
		State state = m_State;
		if (state == State.Unlock && m_StateTimer > 1f)
		{
			SetState(State.Wait);
		}
		m_StateTimer += TimeManager.Instance.m_PauseDelta;
	}
}
