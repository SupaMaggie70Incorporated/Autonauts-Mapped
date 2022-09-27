using UnityEngine;

public class GameStateNewGame : GameStateBase
{
	[HideInInspector]
	public NewGameOptions m_Menu;

	private bool m_Start;

	private bool m_Go;

	private bool m_TransitionReady;

	protected new void Awake()
	{
		base.Awake();
		Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Menus/NewGameOptions", typeof(GameObject));
		m_Menu = Object.Instantiate(original, menusRootTransform).GetComponent<NewGameOptions>();
		if ((bool)SpaceAnimation.Instance)
		{
			m_Menu.gameObject.SetActive(value: false);
			m_Start = true;
		}
	}

	protected new void OnDestroy()
	{
		if ((bool)SpaceAnimation.Instance)
		{
			SpaceAnimation.Instance.DuckAudio(Duck: false);
		}
		base.OnDestroy();
		if ((bool)m_Menu)
		{
			Object.DestroyImmediate(m_Menu.gameObject);
		}
	}

	public void SetDataFromFile(string FileName, bool Load)
	{
		m_Menu.SetDataFromFile(FileName, Load);
	}

	public void SetDataFromNew()
	{
		m_Menu.SetDataFromNew();
	}

	public void StartNewGame()
	{
		AnalyticsManager.WorldCreated(GameOptionsManager.Instance.m_Options);
		SpaceAnimation.Instance.RocketGo();
		m_Go = true;
		m_Menu.gameObject.SetActive(value: false);
		SpaceAnimation.Instance.DuckAudio(Duck: false);
	}

	private void ReadyTransition()
	{
		Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		Object.Instantiate((GameObject)Resources.Load("Prefabs/Hud/Transitions/NewGame", typeof(GameObject)), menusRootTransform).GetComponent<Loading>().SetNew();
		AudioManager.Instance.StartMusic("MusicLoading");
		m_TransitionReady = true;
	}

	public override void UpdateState()
	{
		if (m_Go)
		{
			if (!m_TransitionReady)
			{
				if (SpaceAnimation.Instance.GetStartFinished())
				{
					ReadyTransition();
				}
			}
			else if (AudioManager.Instance.IsMusicPlaying())
			{
				SpaceAnimation.Instance.UseAudioListener(Use: false);
				SessionManager.Instance.LoadLevel(LoadLevel: false, "Main");
				GameStateManager.Instance.SetState(GameStateManager.State.SceneChange);
			}
		}
		else if (m_Start)
		{
			if (SpaceAnimation.Instance.GetStartFinished())
			{
				m_Menu.gameObject.SetActive(value: true);
				m_Start = false;
				SpaceAnimation.Instance.DuckAudio(Duck: true);
			}
		}
		else if (MyInputManager.m_Rewired.GetButtonDown("Quit"))
		{
			AudioManager.Instance.StartEvent("UIOptionCancelled");
			GameStateManager.Instance.PopState();
		}
	}
}
