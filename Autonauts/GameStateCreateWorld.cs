using UnityEngine;

public class GameStateCreateWorld : GameStateBase
{
	private Loading m_Loading;

	private int m_StartLoading;

	private int m_EndLoading;

	protected new void Awake()
	{
		base.Awake();
		Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Transitions/NewGame", typeof(GameObject));
		m_Loading = Object.Instantiate(original, menusRootTransform).GetComponent<Loading>();
		m_Loading.SetNew();
		m_StartLoading = 5;
		m_EndLoading = 7;
		ShorelineManager.Instance.gameObject.SetActive(value: false);
		DayNightManager.Instance.gameObject.SetActive(value: false);
		SpawnAnimationManager.Instance.gameObject.SetActive(value: false);
		FailSafeManager.Instance.gameObject.SetActive(value: false);
		ModManager.Instance.gameObject.SetActive(value: false);
		ParticlesManager.Instance.gameObject.SetActive(value: false);
		RainManager.Instance.gameObject.SetActive(value: false);
		QuestManager.Instance.gameObject.SetActive(value: false);
		AudioManager.Instance.StartMusic("MusicLoading");
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
		ModManager.Instance.PostCreateScripts(CreatedGame: true);
		RainManager.Instance.gameObject.SetActive(value: true);
		ParticlesManager.Instance.gameObject.SetActive(value: true);
		ModManager.Instance.gameObject.SetActive(value: true);
		FailSafeManager.Instance.gameObject.SetActive(value: true);
		SpawnAnimationManager.Instance.gameObject.SetActive(value: true);
		DayNightManager.Instance.gameObject.SetActive(value: true);
		ShorelineManager.Instance.gameObject.SetActive(value: true);
		HudManager.Instance.StartGame();
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
				GameStateManager.Instance.SetState(GameStateManager.State.Normal);
				CeremonyManager.Instance.AddCeremony(CeremonyManager.CeremonyType.RocketIntro);
				CeremonyManager.Instance.AddCeremony(CeremonyManager.CeremonyType.CommsIntro);
				CeremonyManager.Instance.AddCeremony(CeremonyManager.CeremonyType.Go);
			}
		}
	}
}
