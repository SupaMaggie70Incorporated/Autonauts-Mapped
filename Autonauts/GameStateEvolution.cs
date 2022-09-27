using UnityEngine;

public class GameStateEvolution : GameStateBase
{
	private Evolution m_Evolution;

	protected new void Awake()
	{
		base.Awake();
		TimeManager.Instance.PauseAll();
		AudioManager.Instance.Pause(Pause: true);
		Transform scaledHUDRootTransform = HudManager.Instance.m_ScaledHUDRootTransform;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Evolution", typeof(GameObject));
		m_Evolution = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, scaledHUDRootTransform).GetComponent<Evolution>();
		m_Evolution.GetComponent<RectTransform>().anchoredPosition = default(Vector2);
		HudManager.Instance.HideRollovers();
		HudManager.Instance.SetHudButtonsActive(Active: false);
		CameraManager.Instance.SetPausedDOFEffect();
	}

	protected new void OnDestroy()
	{
		Object.Destroy(m_Evolution.gameObject);
		base.OnDestroy();
		TimeManager.Instance.UnPauseAll();
		AudioManager.Instance.Pause(Pause: false);
		HudManager.Instance.SetHudButtonsActive(Active: true);
		HudManager.Instance.HideRollovers();
		HudManager.Instance.ActivateQuestCompleteRollover(Activate: false, null);
		HudManager.Instance.ActivateQuestRollover(Activate: false, null);
		CameraManager.Instance.RestorePausedDOFEffect();
	}

	public override void UpdateState()
	{
		if (!m_Evolution.GetIsPlayingCeremony() && MyInputManager.m_Rewired.GetButtonDown("Quit"))
		{
			AudioManager.Instance.StartEvent("UIUnpause");
			GameStateManager.Instance.PopState();
		}
	}
}
