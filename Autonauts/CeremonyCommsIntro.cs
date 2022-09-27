using UnityEngine;

public class CeremonyCommsIntro : CeremonyBase
{
	private enum State
	{
		Speech1,
		Speech2,
		Total
	}

	private State m_State;

	private CeremonySpeech m_Speech;

	private float m_StateTimer;

	private void Awake()
	{
		m_Speech = base.transform.Find("SpeechPanel").GetComponent<CeremonySpeech>();
		m_Speech.GetButton().SetAction(OnAcceptClicked, null);
		SetState(State.Speech1);
		if ((bool)TutorBot.Instance)
		{
			TutorBot.Instance.RocketEnd();
		}
	}

	private void SetSpeech(string TextID)
	{
		if (TextID == null)
		{
			m_Speech.SetActive(Active: false);
			return;
		}
		m_Speech.SetActive(Active: true);
		m_Speech.SetSpeechFromID(TextID);
	}

	private void SetState(State NewState)
	{
		m_State = NewState;
		switch (NewState)
		{
		case State.Speech1:
			SetSpeech("CeremonyCommsIntroSpeech1");
			break;
		case State.Speech2:
			SetSpeech("CeremonyCommsIntroSpeech2");
			AudioManager.Instance.StartEvent("CeremonyTabReveal");
			break;
		}
	}

	public void OnAcceptClicked(BaseGadget NewGadget)
	{
		if (m_State == State.Speech1)
		{
			SetState(State.Speech2);
		}
		else if (m_State == State.Speech2)
		{
			End();
		}
	}

	private void End()
	{
		Object.Destroy(base.gameObject);
		CeremonyManager.Instance.CeremonyEnded();
		CameraManager.Instance.SetState(CameraManager.State.Normal);
		CameraManager.Instance.Focus(Transmitter.Instance.transform.position);
	}
}
