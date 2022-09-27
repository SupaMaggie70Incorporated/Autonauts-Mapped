using System.Collections;
using System.IO;
using SimpleJSON;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class Boot : MonoBehaviour
{
	public static Boot Instance;

	private int m_Stage;

	private bool DoneInitialSetup;

	private string[] m_Names = new string[16]
	{
		"InputManager", "SettingsManager", "VariableManager", "AudioManager", "SessionManager", "GameOptionsManager", "TextManager", "InstantiationManager", "ModelManager", "TextureAtlasManager",
		"MaterialManager", "IconManager", "LightManager", "BadgeManager", "StorageTypeManager", "SteamManager"
	};

	private static string m_OriginalScene = "";

	private Image m_Background;

	private BaseImage m_Logo;

	private VideoPlayer m_Video;

	private bool m_VideoStarted;

	private bool m_SkipVideo;

	public static bool CheckBooted()
	{
		if (Instance == null)
		{
			m_OriginalScene = SceneManager.GetActiveScene().name;
			SceneManager.LoadScene("Boot");
			return false;
		}
		return true;
	}

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
		}
		else if (this != Instance)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		if (!Application.isEditor)
		{
			Application.runInBackground = true;
		}
		m_Background = GameObject.Find("Background").GetComponent<Image>();
		m_Logo = GameObject.Find("Logo").GetComponent<BaseImage>();
		m_Logo.gameObject.SetActive(value: false);
		m_Video = GameObject.Find("Video").GetComponent<VideoPlayer>();
		LoadOverrides();
		m_VideoStarted = false;
	}

	private void LoadOverrides()
	{
		if (!Application.isEditor)
		{
			return;
		}
		string path = Application.persistentDataPath + "/Overrides.txt";
		if (File.Exists(path))
		{
			JSONNode jSONNode = JSON.Parse(File.ReadAllText(path));
			string text = jSONNode["Autoload"];
			if (text != null && text != "")
			{
				SaveLoadManager.m_ForceLoadFile = text;
			}
			if (jSONNode["EmptyWorld"].AsBool)
			{
				SaveLoadManager.m_EmptyWorld = true;
			}
		}
	}

	private void VideoPlayer_errorReceived(VideoPlayer source, string message)
	{
		Debug.Log(message);
		m_SkipVideo = true;
		m_Background.color = new Color(1f, 1f, 1f);
		m_Logo.SetSprite("LogoPublisher");
		m_Logo.gameObject.SetActive(value: true);
	}

	private IEnumerator PlayVideo()
	{
		m_SkipVideo = false;
		m_Video.errorReceived += VideoPlayer_errorReceived;
		m_Video.Prepare();
		m_Video.SetDirectAudioVolume(0, SettingsManager.Instance.m_SFXVolume);
		WaitForSeconds NewSeconds = new WaitForSeconds(1f);
		while (!m_Video.isPrepared && !m_SkipVideo)
		{
			yield return NewSeconds;
		}
		if (!m_SkipVideo)
		{
			m_Video.Play();
			m_VideoStarted = true;
		}
	}

	private void Start()
	{
		new ClassManager();
		GeneralUtils.Init();
		Resources.Load("");
		InitStatics();
		m_Stage = 0;
		LoadManager("ModLoaderManager");
		LoadManager("ModManager");
		StartCoroutine(ModLoaderManager.Instance.NowStart());
	}

	private void InitStatics()
	{
		Converter.Init();
		Storage.Init();
		GameStateEdit.Init();
		BuildingPalette.InitFirst();
		TilePalette.Init();
		Actionable.Init();
		Tile.Init();
		BurnableFuel.Init();
		WorkerGroup.Init();
		WorkerInstruction.Init();
		FolkHeart.InitFolkHeart();
		ObjectsPanels.Init();
	}

	private void LoadManager(string Name)
	{
		if (Object.Instantiate((GameObject)Resources.Load("Prefabs/Managers/" + Name, typeof(GameObject)), new Vector3(0f, 0f, 0f), Quaternion.identity, null) == null)
		{
			ErrorMessage.LogError("Couldn't load " + Name);
		}
	}

	private void AllLoaded()
	{
		GameOptionsManager.Instance.m_Options.SetDefaults();
		ModelManager.Instance.MakeModelList();
		ObjectTypeList.Instance.RegisterClasses();
		SettingsManager.Instance.Apply();
		InstantiationManager.Instance.Clear();
		ModelManager.Instance.Init();
		IconManager.Instance.Init();
		HighInstruction.Init();
		if (m_OriginalScene != "")
		{
			SceneManager.LoadScene(m_OriginalScene);
		}
	}

	private void Update()
	{
		if (!ModLoaderManager.Instance.IsFullyLoaded() || !ModManager.Instance.AllModsInitialised())
		{
			return;
		}
		if (!DoneInitialSetup)
		{
			LoadManager(m_Names[m_Stage]);
			m_Stage++;
			LoadManager(m_Names[m_Stage]);
			m_Stage++;
			new ObjectTypeList();
			LoadManager(m_Names[m_Stage]);
			m_Stage++;
			StartCoroutine(PlayVideo());
			DoneInitialSetup = true;
			return;
		}
		if (!m_SkipVideo)
		{
			if (!m_VideoStarted)
			{
				return;
			}
			if (MyInputManager.m_Rewired != null && MyInputManager.m_Rewired.GetButtonDown("Quit"))
			{
				m_Video.Stop();
			}
			if (m_Video.isPlaying)
			{
				return;
			}
			m_SkipVideo = true;
		}
		if (m_Stage != m_Names.Length)
		{
			LoadManager(m_Names[m_Stage]);
			m_Stage++;
			if (m_Stage == 5)
			{
				m_Video.gameObject.SetActive(value: false);
				m_Background.color = GeneralUtils.ColorFromHex(9360895);
				m_Logo.gameObject.SetActive(value: true);
				m_Logo.SetSprite("LogoDenki");
				SettingsManager.Instance.ApplySound();
				AudioManager.Instance.StartEvent("Denki");
			}
			if (m_Stage == m_Names.Length)
			{
				AllLoaded();
			}
		}
	}

	private void OnApplicationQuit()
	{
		Debug.Log("Shutting Down");
		if ((bool)SessionManager.Instance)
		{
			SessionManager.Instance.ShutdownOldScene();
		}
		Debug.Log("Done");
	}
}
