public class CeremonyPhaseThreeComplete : CeremonyGenericSpeechWithTitle
{
	private enum State
	{
		First,
		Second,
		Third,
		Fourth,
		Fifth,
		Total
	}

	private State m_State;

	protected new void Awake()
	{
		base.Awake();
		SetTitle("CeremonyPhaseThreeCompleteTitle");
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
			SetState(State.Fifth);
		}
		else if (m_State == State.Fifth)
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
			SetSpeech("CeremonyPhaseThreeCompleteSpeech1");
			break;
		case State.Second:
			SetSpeech("CeremonyPhaseThreeCompleteSpeech2");
			break;
		case State.Third:
			m_Speech.m_Tutor.SetImages("Ceremonies/AaronIdle2", "Ceremonies/AaronTalk2");
			SetSpeech("CeremonyPhaseThreeCompleteSpeech3");
			break;
		case State.Fourth:
			m_Speech.m_Tutor.SetImages("Ceremonies/GaryIdle2", "Ceremonies/GaryTalk2");
			SetSpeech("CeremonyPhaseThreeCompleteSpeech4");
			break;
		case State.Fifth:
			m_Speech.m_Tutor.SetImages("Ceremonies/AaronIdle2", "Ceremonies/AaronTalk2");
			SetSpeech("CeremonyPhaseThreeCompleteSpeech5");
			break;
		}
	}
}
