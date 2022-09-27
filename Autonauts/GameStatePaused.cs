using UnityEngine;

public class GameStatePaused : GameStateBase
{
	[HideInInspector]
	public GameObject m_Menu;

	protected new void Awake()
	{
		base.Awake();
		Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Menus/Paused", typeof(GameObject));
		m_Menu = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, menusRootTransform);
		HudManager.Instance.RolloversEnabled(Enabled: false);
		HudManager.Instance.RolloversEnabled(Enabled: true);
		HudManager.Instance.SetHudButtonsActive(Active: false);
		AudioManager.Instance.StartEvent("UIPause");
		CameraManager.Instance.SetPausedDOFEffect();
		if ((bool)TutorialPanelController.Instance)
		{
			TutorialPanelController.Instance.SetActive(Active: false);
		}
	}

	private void Start()
	{
		TimeManager.Instance.PauseAll();
		AudioManager.Instance.Pause(Pause: true);
	}

	protected new void OnDestroy()
	{
		base.OnDestroy();
		Object.Destroy(m_Menu.gameObject);
		CameraManager.Instance.RestorePausedDOFEffect();
		HudManager.Instance.SetHudButtonsActive(Active: true);
		if ((bool)TimeManager.Instance)
		{
			TimeManager.Instance.UnPauseAll();
		}
		AudioManager.Instance.Pause(Pause: false);
		if ((bool)TutorialPanelController.Instance)
		{
			TutorialPanelController.Instance.SetActive(Active: true);
		}
	}

	public override void Pushed(GameStateManager.State NewState)
	{
		base.Pushed(NewState);
		m_Menu.GetComponent<Paused>().Pushed();
	}

	public override void Popped(GameStateManager.State NewState)
	{
		base.Popped(NewState);
		m_Menu.GetComponent<Paused>().Popped();
	}

	public override void UpdateState()
	{
		if (MyInputManager.m_Rewired.GetButtonDown("Quit"))
		{
			AudioManager.Instance.StartEvent("UIUnpause");
			GameStateManager.Instance.PopState();
		}
	}
}
