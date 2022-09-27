using UnityEngine;

public class GameStateMainMenuCreate : GameStateBase
{
	private GameObject m_Loading;

	private int m_StartLoading;

	private int m_EndLoading;

	protected new void Awake()
	{
		base.Awake();
		Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/SceneChange", typeof(GameObject));
		m_Loading = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, menusRootTransform);
		m_Loading.GetComponent<RectTransform>().anchoredPosition = default(Vector2);
		m_StartLoading = 5;
		m_EndLoading = 7;
		ShorelineManager.Instance.gameObject.SetActive(value: false);
		DayNightManager.Instance.gameObject.SetActive(value: false);
		SpawnAnimationManager.Instance.gameObject.SetActive(value: false);
		FailSafeManager.Instance.gameObject.SetActive(value: false);
		ModManager.Instance.gameObject.SetActive(value: false);
		ParticlesManager.Instance.gameObject.SetActive(value: false);
		RainManager.Instance.gameObject.SetActive(value: false);
	}

	protected new void OnDestroy()
	{
		base.OnDestroy();
		if ((bool)m_Loading)
		{
			Object.Destroy(m_Loading.gameObject);
		}
	}

	private void StartGame()
	{
		RainManager.Instance.gameObject.SetActive(value: true);
		ParticlesManager.Instance.gameObject.SetActive(value: true);
		ModManager.Instance.gameObject.SetActive(value: true);
		FailSafeManager.Instance.gameObject.SetActive(value: true);
		SpawnAnimationManager.Instance.gameObject.SetActive(value: true);
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
				WorldGenerator.Instance.StartCreateNew();
			}
		}
		else if (!SaveLoadManager.Instance.m_Loading)
		{
			if (m_EndLoading == 5)
			{
				StartGame();
			}
			m_EndLoading--;
			if (m_EndLoading == 0)
			{
				SaveLoadManager.m_MiniMap = false;
				GameOptionsManager.Instance.m_Options.m_MapSeed = 0;
				GameOptionsManager.Instance.m_Options.SetMapSize(GameOptions.GameSize.Medium);
				GameStateMainMenu.GoToMainMenu();
				CameraManager.Instance.SetState(CameraManager.State.MainMenu);
			}
		}
	}
}
