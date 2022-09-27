using UnityEngine;

public class GameStatePlayback : GameStateBase
{
	private PlaybackControl m_PlaybackControl;

	protected new void Awake()
	{
		base.Awake();
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Playback/PlaybackControl", typeof(GameObject));
		m_PlaybackControl = Object.Instantiate(original, HudManager.Instance.m_HUDRootTransform).GetComponent<PlaybackControl>();
		PlaybackManager.Instance.SetSlider(m_PlaybackControl.m_Slider);
	}

	protected new void OnDestroy()
	{
		base.OnDestroy();
		Object.Destroy(m_PlaybackControl.gameObject);
	}

	public override void Pushed(GameStateManager.State NewState)
	{
		base.Pushed(NewState);
		m_PlaybackControl.gameObject.SetActive(value: false);
	}

	public override void Popped(GameStateManager.State NewState)
	{
		base.Pushed(NewState);
		m_PlaybackControl.gameObject.SetActive(value: true);
	}

	public override void UpdateState()
	{
		if (MyInputManager.m_Rewired.GetButtonDown("Quit"))
		{
			GameStateManager.Instance.PushState(GameStateManager.State.Paused);
		}
		if (MyInputManager.m_Rewired.GetButtonDown("ToggleFreeCam"))
		{
			AudioManager.Instance.StartEvent("UIOptionSelected");
			GameStateManager.Instance.PushState(GameStateManager.State.FreeCam);
		}
	}
}
