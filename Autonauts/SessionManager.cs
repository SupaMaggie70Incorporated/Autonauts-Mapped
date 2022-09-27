using UnityEngine;
using UnityEngine.SceneManagement;

public class SessionManager : MonoBehaviour
{
	public static SessionManager Instance;

	[HideInInspector]
	public bool m_LoadLevel;

	[HideInInspector]
	public string m_LoadFileName = "../Autonauts Levels/0.txt";

	[HideInInspector]
	public bool m_FromMenu;

	private string m_NewLevel;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
		}
		else if (this != Instance)
		{
			Object.DestroyImmediate(base.gameObject);
			return;
		}
		m_NewLevel = "";
	}

	public void LoadLevel(bool LoadLevel, string Name)
	{
		m_LoadLevel = LoadLevel;
		m_NewLevel = Name;
	}

	public void ShutdownOldScene()
	{
		Debug.Log("Shutdown Scene");
		AudioManager.Instance.StopSFX();
		if ((bool)MainMenuManager.Instance)
		{
			MainMenuManager.Instance.ShutDown();
		}
		if ((bool)MasterManager.Instance)
		{
			MasterManager.Instance.ShutDown();
		}
		if ((bool)PlaybackMasterManager.Instance)
		{
			PlaybackMasterManager.Instance.ShutDown();
		}
		Debug.Log("Shutdown Done");
	}

	private void ChangeLevel()
	{
		ShutdownOldScene();
		SceneManager.LoadScene(m_NewLevel);
	}

	private void Update()
	{
		if (m_NewLevel != "")
		{
			ChangeLevel();
			m_NewLevel = "";
		}
	}
}
