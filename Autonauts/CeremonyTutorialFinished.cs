using UnityEngine;

public class CeremonyTutorialFinished : CeremonyGenericSpeechWithTitle
{
	private enum State
	{
		First,
		Second,
		Total
	}

	private State m_State;

	private MyParticles m_Particles;

	protected new void Awake()
	{
		base.Awake();
		SetTitle("CeremonyTutorialFinishedTitle");
		SetSpeech("CeremonyTutorialFinishedSpeech1");
		AudioManager.Instance.StartEvent("CeremonyFirstBot");
		m_State = State.First;
		TutorBot.Instance.SetState(TutorBot.State.TutorialFinished);
	}

	private void Start()
	{
		PanTo(TutorBot.Instance, new Vector3(0f, 2f, 0f), 10f, 1f);
	}

	public override void OnAcceptClicked(BaseGadget NewGadget)
	{
		if (m_State == State.First)
		{
			m_State = State.Second;
			SetSpeech("CeremonyTutorialFinishedSpeech2");
			TutorBot.Instance.SetState(TutorBot.State.Away);
			m_Particles = ParticlesManager.Instance.CreateParticles("Transmitter", default(Vector3), Quaternion.Euler(90f, 0f, 0f));
			m_Particles.transform.position = TutorBot.Instance.transform.position + new Vector3(0f, 20f, 0f);
			m_Particles.Play();
		}
		else
		{
			TutorBot.Instance.StopUsing();
			ParticlesManager.Instance.DestroyParticles(m_Particles, WaitUntilNoParticles: true, StopSpawning: true);
			End();
		}
	}

	protected override void End()
	{
		base.End();
		ReturnPanTo(1f);
	}
}
