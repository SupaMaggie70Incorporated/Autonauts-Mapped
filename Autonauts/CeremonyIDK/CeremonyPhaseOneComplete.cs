public class CeremonyPhaseOneComplete : CeremonyGenericSpeechWithTitle
{
	private enum State
	{
		First,
		Second,
		Third,
		Fourth,
		Phase2First,
		Phase2Second,
		Phase2Third,
		Total
	}

	private State m_State;

	protected new void Awake()
	{
		base.Awake();
		SetTitle("CeremonyPhaseOneCompleteTitle");
		AudioManager.Instance.StartEvent("CeremonyFirstBot");
		SetState(State.First);
	}

	public override void OnAcceptClicked(BaseGadget NewGadget)
	{
		if (m_State == State.First)
		{
			SetState(State.Second);
		}
		else if (m_State == State.Second)
		{
			SetState(State.Third);
		}
		else if (m_State == State.Third)
		{
			SetState(State.Fourth);
		}
		else if (m_State == State.Fourth)
		{
			SetState(State.Phase2First);
		}
		else if (m_State == State.Phase2First)
		{
			SetState(State.Phase2Second);
		}
		else if (m_State == State.Phase2Second)
		{
			SetState(State.Phase2Third);
		}
		else if (m_State == State.Phase2Third)
		{
			End();
		}
	}

	private void SetState(State NewState)
	{
		m_State = NewState;
		switch (m_State)
		{
		case State.First:
			SetSpeech("CeremonyPhaseOneCompleteSpeech1");
			break;
		case State.Second:
			SetSpeech("CeremonyPhaseOneCompleteSpeech2");
			break;
		case State.Third:
			m_Speech.m_Tutor.SetImages("Ceremonies/AaronIdle", "Ceremonies/AaronTalk");
			SetSpeech("CeremonyPhaseOneCompleteSpeech3");
			break;
		case State.Fourth:
			m_Speech.m_Tutor.SetImages("Ceremonies/GaryIdle", "Ceremonies/GaryTalk");
			SetSpeech("CeremonyPhaseOneCompleteSpeech4");
			break;
		case State.Phase2First:
			m_Speech.m_Tutor.SetImages("Ceremonies/AaronIdle", "Ceremonies/AaronTalk");
			SetSpeech("CeremonyPhaseTwoStartSpeech1");
			break;
		case State.Phase2Second:
			m_Speech.m_Tutor.SetImages("Ceremonies/GaryIdle", "Ceremonies/GaryTalk");
			SetSpeech("CeremonyPhaseTwoStartSpeech2");
			break;
		case State.Phase2Third:
			m_Speech.m_Tutor.SetImages("Ceremonies/AaronIdle", "Ceremonies/AaronTalk");
			SetSpeech("CeremonyPhaseTwoStartSpeech3");
			break;
		}
	}
}
