using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
	public static MainMenuManager Instance;

	private static List<GameObject> m_Managers;

	private bool m_Creating;

	private void Awake()
	{
		if (Boot.CheckBooted())
		{
			Instance = this;
			GeneralUtils.Init();
			CameraManager.Instance.Restart();
			m_Creating = false;
		}
	}

	private void Create()
	{
		Transform parent = GameObject.Find("Managers").transform;
		m_Managers = new List<GameObject>();
		GameObject item = Object.Instantiate((GameObject)Resources.Load("Prefabs/Managers/TimeManager", typeof(GameObject)), new Vector3(0f, 0f, 0f), Quaternion.identity, parent);
		m_Managers.Add(item);
		Object.Instantiate((GameObject)Resources.Load("Prefabs/Managers/SaveLoadManager", typeof(GameObject)), new Vector3(0f, 0f, 0f), Quaternion.identity, parent);
		SaveLoadManager.m_MiniMap = true;
		GameOptionsManager.Instance.m_Options.m_MapSeed = SaveLoadManager.m_MiniMapSeed;
		GameOptionsManager.Instance.m_Options.SetMapSize(GameOptions.GameSize.Small);
		string[] array = new string[43]
		{
			"DespawnManager", "CollectionManager", "PlotManager", "PlotMeshBuilder", "PlotMeshBuilderWater", "ResourceManager", "MapManager", "WorldGeneratorManager", "WorkerScriptManager", "ShorelineManager",
			"TileManager", "HudManager", "CheatManager", "SpawnAnimationManager", "StorageTypeManager", "ObjectCountManager", "BaggedManager", "WorldSettings", "BuildingManager", "TileMapAnimationManager",
			"QuestManagerTiles", "FailSafeManager", "FolkManager", "DayNightManager", "NewIconManager", "ParticlesManager", "AreaIndicatorManager", "RainManager", "AddAnimationManager", "LinkedSystemManager",
			"RefreshManager", "WaterManager", "SoilManager", "TileUseManager", "TrailManager", "WorkerGroupManager", "StatsManager", "WalledAreaManager", "RecordingManager", "CameraRecordingManager",
			"PlotObjectMergerManager", "LightManager", "GameStateManager"
		};
		for (int i = 0; i < array.Length; i++)
		{
			item = Object.Instantiate((GameObject)Resources.Load("Prefabs/Managers/" + array[i], typeof(GameObject)), new Vector3(0f, 0f, 0f), Quaternion.identity, parent);
			if ((bool)item)
			{
				if ((bool)item.GetComponent<HudManager>())
				{
					item.GetComponent<HudManager>().InitMainMenu();
				}
				if ((bool)item.GetComponent<TileManager>())
				{
					TileManager.Instance = item.GetComponent<TileManager>();
				}
			}
			m_Managers.Add(item);
		}
		GameStateManager.Instance.SetState(GameStateManager.State.MainMenuCreate);
		ObjectTypeList.Instance.SetupParents();
		ObjectTypeList.Instance.Reset();
		SessionManager.Instance.m_FromMenu = true;
	}

	public void ShutDown()
	{
		InstantiationManager.Instance.Clear();
		while (m_Managers.Count > 0)
		{
			GameObject obj = m_Managers[m_Managers.Count - 1];
			m_Managers.RemoveAt(m_Managers.Count - 1);
			Object.DestroyImmediate(obj);
		}
	}

	private void Update()
	{
		if (!m_Creating)
		{
			m_Creating = true;
			Create();
		}
	}
}
