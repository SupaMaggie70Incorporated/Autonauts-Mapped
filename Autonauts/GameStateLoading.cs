using UnityEngine;
using UnityEngine.UI;

public class GameStateLoading : GameStateBase
{
	public static GameStateLoading Instance;

	private Loading m_Loading;

	private int m_StartLoading;

	private int m_EndLoading;

	protected new void Awake()
	{
		Instance = this;
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

	public void SetText(string Name)
	{
		m_Loading.GetComponentInChildren<Text>().text = Name;
	}

	private void StartGame()
	{
		ModManager.Instance.PostCreateScripts(CreatedGame: false);
		QuestManager.Instance.gameObject.SetActive(value: true);
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
			return;
		}
		if (SaveLoadManager.Instance.m_Loading)
		{
			SaveLoadManager.Instance.UpdateLoad();
		}
		if (!SaveLoadManager.Instance.m_Loading)
		{
			if (m_EndLoading == 2)
			{
				StartGame();
			}
			m_EndLoading--;
			if (m_EndLoading == 0)
			{
				GameStateManager.Instance.SetState(GameStateManager.State.Normal);
			}
		}
		else
		{
			m_Loading.SetValue(SaveLoadManager.Instance.GetLoadPercent());
		}
	}
}
