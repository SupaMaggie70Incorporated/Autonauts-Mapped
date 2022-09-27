using System.Collections.Generic;

public class GameStateCeremony : GameStateBase
{
	private bool m_Blur;

	protected new void Awake()
	{
		base.Awake();
		HighlightObject(null);
		HudManager.Instance.RolloversEnabled(Enabled: false);
		HudManager.Instance.SetHudButtonsActive(Active: false);
		CameraManager.Instance.EnableVignette(Enabled: true);
		Cursor.Instance.NoTarget();
		TutorialPanelController.Instance.CeremonyActive(Active: true);
		HudManager.Instance.SetIndicatorsVisible(Visible: false);
	}

	protected new void OnDestroy()
	{
		HudManager.Instance.SetIndicatorsVisible(Visible: true);
		TutorialPanelController.Instance.CeremonyActive(Active: false);
		HudManager.Instance.RolloversEnabled(Enabled: true);
		HudManager.Instance.SetHudButtonsActive(Active: true);
		if (m_Blur)
		{
			CameraManager.Instance.RestorePausedDOFEffect();
		}
		else
		{
			CameraManager.Instance.EnableVignette(Enabled: false);
		}
		base.OnDestroy();
	}

	public void SetBlur()
	{
		m_Blur = true;
		CameraManager.Instance.SetPausedDOFEffect();
	}

	public override void UpdateState()
	{
		if (!MyInputManager.m_Rewired.GetButtonDown("Quit"))
		{
			return;
		}
		CeremonyManager.CeremonyType type = CeremonyManager.Instance.m_CurrentCeremonyData.m_Type;
		if (type != CeremonyManager.CeremonyType.RocketIntro && type != CeremonyManager.CeremonyType.CommsIntro && type != CeremonyManager.CeremonyType.Go && type != CeremonyManager.CeremonyType.QuestEnded)
		{
			return;
		}
		if (type == CeremonyManager.CeremonyType.RocketIntro || type == CeremonyManager.CeremonyType.CommsIntro || type == CeremonyManager.CeremonyType.Go)
		{
			if (GameOptionsManager.Instance.m_Options.m_TutorialEnabled)
			{
				TutorialPanelController.Instance.StartTutorial();
			}
			List<BaseClass> players = CollectionManager.Instance.GetPlayers();
			if (players != null && players[0] != null)
			{
				CameraManager.Instance.Focus(players[0].transform.position);
				CameraManager.Instance.SetDistance(13f);
			}
		}
		CeremonyManager.Instance.SkipCeremony();
	}
}
