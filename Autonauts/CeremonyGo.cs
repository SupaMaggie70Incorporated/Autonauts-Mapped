using UnityEngine;

public class CeremonyGo : CeremonyTitleBase
{
	private float m_StateTimer;

	private void Start()
	{
		AudioManager.Instance.StartEvent("CeremonyGo");
	}

	private void End()
	{
		Transmitter.SetTransmittingGlobal(Transmit: false);
		Object.Destroy(base.gameObject);
		CeremonyManager.Instance.CeremonyEnded();
		if (GameOptionsManager.Instance.m_Options.m_TutorialEnabled)
		{
			TutorialPanelController.Instance.StartTutorial();
		}
	}

	private void Update()
	{
		m_StateTimer += TimeManager.Instance.m_NormalDelta;
		if (m_StateTimer > 2f)
		{
			End();
		}
	}
}
