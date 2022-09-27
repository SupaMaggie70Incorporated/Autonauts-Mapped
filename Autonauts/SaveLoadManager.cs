using System;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
	public static SaveLoadManager Instance;

	[HideInInspector]
	public static string m_Version = "137.14.11";

	[HideInInspector]
	public static string m_VersionName = "";

	public static bool m_MiniMap = false;

	public static float m_MiniMapCameraX = 1224.7f;

	public static float m_MiniMapCameraY = 9.236f;

	public static float m_MiniMapCameraZ = -254f;

	public static float m_MiniMapCameraRotX = 15.6f;

	public static float m_MiniMapCameraRotY = -45f;

	public static float m_MiniMapCameraRotZ = 0f;

	public static int m_MiniMapSeed = 708516452;

	public static int m_MiniMapObjSeed = 1906368667;

	public static bool m_TestBuild = true;

	public static bool m_Video = false;

	public static string m_ForceLoadFile = "";

	public static bool m_EmptyWorld = false;

	public static GameOptions.GameMode m_Mode = GameOptions.GameMode.Total;

	public static bool m_EditWorld = false;

	public static bool m_UpdateBases = false;

	public static bool m_Tutorial = true;

	public static bool m_RecordingEnabled = true;

	public static string m_AutosaveName = "Autosave_";

	[HideInInspector]
	public int m_External;

	public string m_ModSaveDirectory;

	public float m_TimeSinceLastSave;

	public float m_AutosaveTimer;

	public bool m_Loading;

	public bool m_Creating;

	private JSONNode m_RootNode;

	private string m_LoadingFileName;

	private string m_LastName;

	private SaveFile m_LoadFile;

	private bool m_DoAutosave;

	private List<Worker> m_AbortList;

	public static string GetVersion()
	{
		string text = "Version ";
		if ((bool)TextManager.Instance)
		{
			text = TextManager.Instance.Get("Version");
		}
		if (m_VersionName != "")
		{
			return text + " " + m_Version + " '" + m_VersionName + "'";
		}
		return text + " " + m_Version;
	}

	private void Awake()
	{
		Instance = this;
		m_External = 0;
		m_TimeSinceLastSave = 0f;
		m_AutosaveTimer = 0f;
		m_ModSaveDirectory = "";
		m_LastName = "";
		m_Loading = false;
	}

	public bool DoesFileExist(string FileName)
	{
		return new SaveFile().Exists(FileName);
	}

	private void ReadySave()
	{
		SpawnAnimationManager.Instance.ReadySave();
		Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("Worker");
		m_AbortList = new List<Worker>();
		if (collection == null)
		{
			return;
		}
		foreach (KeyValuePair<BaseClass, int> item in collection)
		{
			Worker component = item.Key.GetComponent<Worker>();
			if (component.m_State == Farmer.State.Adding || component.m_State == Farmer.State.Taking)
			{
				component.SpawnAbort(null);
				m_AbortList.Add(component);
			}
		}
	}

	private void SaveFinished()
	{
		SpawnAnimationManager.Instance.SaveFinished();
		foreach (Worker abort in m_AbortList)
		{
			abort.m_WorkerInterpreter.RestartLastInstruction();
		}
	}

	public bool Save(string FileName)
	{
		HudManager.Instance.SetSaveImageActive();
		m_LastName = FileName;
		ReadySave();
		SaveFile saveFile = new SaveFile();
		GameOptionsManager.Instance.m_Options.SetMapData(TileManager.Instance.m_Tiles, TileManager.Instance.m_TilesWide, TileManager.Instance.m_TilesHigh);
		bool num = saveFile.Save(FileName);
		if (num)
		{
			m_ModSaveDirectory = FileName;
			ModManager.Instance.AfterSave();
		}
		SaveFinished();
		SettingsManager.Instance.m_LastSave = FileName;
		SettingsManager.Instance.Save();
		return num;
	}

	public int CheckValidFile(string FileName)
	{
		if (!new SaveFile().Exists(FileName))
		{
			return 1;
		}
		return 0;
	}

	private static int SortFileNamesByCreationDate(SaveFile p1, SaveFile p2)
	{
		return (int)(p1.m_Summary.GetDateLong() - p2.m_Summary.GetDateLong());
	}

	public void ClearOldAutoSaves()
	{
		List<string> allSaveNames = SaveFile.GetAllSaveNames(Autosaves: true, Recordings: false);
		int num = 20;
		if (allSaveNames.Count <= num)
		{
			return;
		}
		List<SaveFile> list = new List<SaveFile>();
		foreach (string item in allSaveNames)
		{
			SaveFile saveFile = new SaveFile();
			saveFile.LoadPreview(item);
			list.Add(saveFile);
		}
		list.Sort(SortFileNamesByCreationDate);
		int num2 = 0;
		foreach (SaveFile item2 in list)
		{
			item2.Delete(item2.m_Name);
			num2++;
			if (num2 >= allSaveNames.Count - num)
			{
				break;
			}
		}
	}

	public static void InitEverything()
	{
		GameStateEdit.Init();
		BuildingPalette.InitFirst();
		Evolution.Init();
		GameStateIndustry.Init();
		ObjectsPanels.Init();
		Autopedia.Init();
	}

	public void Load(string FileName)
	{
		m_LastName = FileName;
		m_Loading = true;
		TimeManager.Instance.PauseAll();
		m_Creating = false;
		m_LoadingFileName = FileName;
		ObjectTypeList.Instance.ResetUniqueIDCounter();
		ObjectTypeList.Instance.SetLoading(Loading: true);
		InitEverything();
		m_LoadFile = new SaveFile();
		m_LoadFile.LoadStart(FileName);
	}

	public float GetLoadPercent()
	{
		return m_LoadFile.GetLoadPercent();
	}

	public void UpdateLoad()
	{
		m_LoadFile.Update();
		if (!m_LoadFile.m_Loading)
		{
			LoadCompleted();
		}
	}

	private void LoadCompleted()
	{
		m_ModSaveDirectory = m_LoadingFileName;
		m_TimeSinceLastSave = 0f;
		m_AutosaveTimer = 0f;
		m_Loading = false;
		TimeManager.Instance.UnPauseAll();
		if (GameOptionsManager.Instance.m_Options.m_RecordingEnabled)
		{
			RecordingManager.Instance.Load(m_LoadingFileName);
		}
		TutorialPanelController.Instance.PostLoad();
	}

	private void DoAutosave()
	{
		ReadySave();
		string text = m_AutosaveName + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
		if (new SaveFile().Save(text))
		{
			string modSaveDirectory = m_ModSaveDirectory;
			m_ModSaveDirectory = text;
			ModManager.Instance.AfterSave();
			m_ModSaveDirectory = modSaveDirectory;
		}
		SaveFinished();
	}

	public void QuickSave()
	{
		if (m_LastName != "")
		{
			Save(m_LastName);
		}
	}

	private void Update()
	{
		if (m_DoAutosave)
		{
			m_DoAutosave = false;
			DoAutosave();
		}
		if (!PlotManager.Instance || m_Loading || !GeneralUtils.m_InGame)
		{
			return;
		}
		m_TimeSinceLastSave += TimeManager.Instance.m_NormalDeltaUnscaled;
		int autosaveFrequency = (int)SettingsManager.Instance.m_AutosaveFrequency;
		float num = SettingsManager.m_AutosaveFrequencies[autosaveFrequency];
		if (num > 0f)
		{
			m_AutosaveTimer += TimeManager.Instance.m_NormalDeltaUnscaled;
			if ((GameStateManager.Instance.GetActualState() == GameStateManager.State.Normal || GameStateManager.Instance.GetActualState() == GameStateManager.State.Edit) && m_AutosaveTimer >= num * 60f)
			{
				m_AutosaveTimer = 0f;
				m_TimeSinceLastSave = 0f;
				ClearOldAutoSaves();
				HudManager.Instance.SetSaveImageActive();
				m_DoAutosave = true;
			}
		}
	}
}
