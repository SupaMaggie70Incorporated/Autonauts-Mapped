using UnityEngine;

public class GameStatePlaybackLoading : GameStateBase
{
	private Loading m_Loading;

	private int m_StartLoading;

	private int m_EndLoading;

	protected new void Awake()
	{
		base.Awake();
		Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Transitions/Loading", typeof(GameObject));
		m_Loading = Object.Instantiate(original, menusRootTransform).GetComponent<Loading>();
		m_Loading.SetLoading();
		m_Loading.SetValue(0f);
		m_StartLoading = 5;
		m_EndLoading = 5;
		ShorelineManager.Instance.gameObject.SetActive(value: false);
		DayNightManager.Instance.gameObject.SetActive(value: false);
		AudioManager.Instance.StopMusic();
		AudioManager.Instance.StartMusic("MusicLoading");
	}

	protected new void OnDestroy()
	{
		base.OnDestroy();
		if ((bool)m_Loading)
		{
			Object.Destroy(m_Loading.gameObject);
		}
		AudioManager.Instance.StartMusic("MusicRecordings");
	}

	private void StartGame()
	{
		DayNightManager.Instance.gameObject.SetActive(value: true);
		ShorelineManager.Instance.gameObject.SetActive(value: true);
	}

	private void Update()
	{
		if (m_StartLoading > 0)
		{
			m_StartLoading--;
			if (m_StartLoading == 0)
			{
				PlaybackManager.Instance.Init();
			}
		}
		else if (!PlaybackManager.Instance.m_Loading)
		{
			if (m_EndLoading == 2)
			{
				StartGame();
			}
			m_EndLoading--;
			if (m_EndLoading == 0)
			{
				GameStateManager.Instance.SetState(GameStateManager.State.Playback);
			}
		}
		else
		{
			m_Loading.SetValue(PlaybackManager.Instance.GetLoadPercent());
		}
	}
}
