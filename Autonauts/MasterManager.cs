using System.Collections.Generic;
using UnityEngine;

public class MasterManager : MonoBehaviour
{
	public static MasterManager Instance;

	private bool m_Creating;

	private List<GameObject> m_Managers;

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
		if (SaveLoadManager.m_TestBuild && SaveLoadManager.m_ForceLoadFile != "")
		{
			SessionManager.Instance.m_LoadFileName = SaveLoadManager.m_ForceLoadFile;
			SessionManager.Instance.m_LoadLevel = true;
		}
		m_Managers = new List<GameObject>();
		GameObject item = Object.Instantiate((GameObject)Resources.Load("Prefabs/Managers/TimeManager", typeof(GameObject)), new Vector3(0f, 0f, 0f), Quaternion.identity, parent);
		m_Managers.Add(item);
		Object.Instantiate((GameObject)Resources.Load("Prefabs/Managers/SaveLoadManager", typeof(GameObject)), new Vector3(0f, 0f, 0f), Quaternion.identity, parent);
		if (SessionManager.Instance.m_LoadLevel)
		{
			SaveLoadManager.Instance.Load(SessionManager.Instance.m_LoadFileName);
		}
		else
		{
			SaveLoadManager.InitEverything();
		}
		string[] array = new string[48]
		{
			"DespawnManager", "CollectionManager", "PlotManager", "PlotMeshBuilder", "PlotMeshBuilderWater", "ResourceManager", "MapManager", "WorldGeneratorManager", "WorkerScriptManager", "ShorelineManager",
			"TileManager", "HudManager", "CheatManager", "SpawnAnimationManager", "StorageTypeManager", "ObjectCountManager", "BaggedManager", "WorldSettings", "BuildingManager", "TileMapAnimationManager",
			"QuestManager", "QuestManagerTiles", "FailSafeManager", "FolkManager", "DayNightManager", "NewIconManager", "ParticlesManager", "AreaIndicatorManager", "RainManager", "AddAnimationManager",
			"LinkedSystemManager", "RefreshManager", "WaterManager", "SoilManager", "TileUseManager", "TrailManager", "WorkerGroupManager", "StatsManager", "WalledAreaManager", "RecordingManager",
			"CameraRecordingManager", "PlotObjectMergerManager", "LightManager", "GameStateManager", "WardrobeManager", "PlanningManager", "BuildingReferenceManager", "OffworldMissionsManager"
		};
		for (int i = 0; i < array.Length; i++)
		{
			item = Object.Instantiate((GameObject)Resources.Load("Prefabs/Managers/" + array[i], typeof(GameObject)), new Vector3(0f, 0f, 0f), Quaternion.identity, parent);
			if ((bool)item)
			{
				if ((bool)item.GetComponent<HudManager>())
				{
					item.GetComponent<HudManager>().InitGame();
				}
				if ((bool)item.GetComponent<TileManager>())
				{
					TileManager.Instance = item.GetComponent<TileManager>();
				}
			}
			m_Managers.Add(item);
		}
		GameStateManager.Instance.SetState(GameStateManager.State.Normal);
		QuestManager.Instance.LockQuests();
		ObjectTypeList.Instance.SetupParents();
		ObjectTypeList.Instance.Reset();
		Object.Instantiate((GameObject)Resources.Load("Prefabs/Cursor", typeof(GameObject)), new Vector3(0f, 0f, 0f), Quaternion.identity, null);
		if (SessionManager.Instance.m_LoadLevel)
		{
			GameStateManager.Instance.SetState(GameStateManager.State.Loading);
		}
		else
		{
			GameStateManager.Instance.SetState(GameStateManager.State.CreateWorld);
		}
		SettingsManager.Instance.Apply();
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
