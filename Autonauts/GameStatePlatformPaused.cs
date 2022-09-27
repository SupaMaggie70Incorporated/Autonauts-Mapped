using UnityEngine;

public class GameStatePlatformPaused : GameStateBase
{
	private bool m_AlreadyPaused;

	private GameObject m_Menu;

	protected new void Awake()
	{
		base.Awake();
		Transform saveImageRootTransform = HudManager.Instance.m_SaveImageRootTransform;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Menus/PlatformPaused", typeof(GameObject));
		m_Menu = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, saveImageRootTransform);
		m_Menu.transform.localPosition = new Vector3(HudManager.Instance.m_HalfCanvasWidth, HudManager.Instance.m_HalfCanvasHeight, 0f);
		if (TimeManager.Instance.m_NormalDelta != 0f)
		{
			TimeManager.Instance.PauseAll();
			AudioManager.Instance.Pause(Pause: true);
			m_AlreadyPaused = false;
		}
		else
		{
			m_AlreadyPaused = true;
		}
	}

	protected new void OnDestroy()
	{
		base.OnDestroy();
		Object.Destroy(m_Menu.gameObject);
		if (!m_AlreadyPaused)
		{
			if ((bool)TimeManager.Instance)
			{
				TimeManager.Instance.UnPauseAll();
			}
			AudioManager.Instance.Pause(Pause: false);
		}
	}
}
