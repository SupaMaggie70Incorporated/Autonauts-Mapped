using System.Collections.Generic;
using UnityEngine;

public class PlaybackMasterManager : MonoBehaviour
{
	public static PlaybackMasterManager Instance;

	public CameraManager m_Camera;

	private bool m_Creating;

	private List<GameObject> m_Managers;

	private void Awake()
	{
		if (Boot.CheckBooted())
		{
			Instance = this;
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
		if (SaveLoadManager.m_TestBuild && SaveLoadManager.m_ForceLoadFile != "")
		{
			SessionManager.Instance.m_LoadFileName = SaveLoadManager.m_ForceLoadFile;
			SessionManager.Instance.m_LoadLevel = true;
		}
		string[] array = new string[25]
		{
			"DespawnManager", "CollectionManager", "PlotManager", "PlotMeshBuilder", "PlotMeshBuilderWater", "ResourceManager", "MapManager", "ShorelineManager", "GameStateManager", "HudManager",
			"TileManager", "StorageTypeManager", "ParticlesManager", "ObjectCountManager", "WorldSettings", "BuildingManager", "TileMapAnimationManager", "LinkedSystemManager", "DayNightManager", "RecordingManager",
			"CameraRecordingManager", "PlaybackManager", "PlotObjectMergerManager", "CheatManager", "LightManager"
		};
		for (int i = 0; i < array.Length; i++)
		{
			item = Object.Instantiate((GameObject)Resources.Load("Prefabs/Managers/" + array[i], typeof(GameObject)), new Vector3(0f, 0f, 0f), Quaternion.identity, parent);
			if ((bool)item)
			{
				if ((bool)item.GetComponent<HudManager>())
				{
					item.GetComponent<HudManager>().InitPlayback();
				}
				if ((bool)item.GetComponent<TileManager>())
				{
					TileManager.Instance = item.GetComponent<TileManager>();
				}
			}
			m_Managers.Add(item);
		}
		GameStateManager.Instance.SetState(GameStateManager.State.PlaybackLoading);
		ObjectTypeList.Instance.SetupParents();
		ObjectTypeList.Instance.Reset();
		Object.Instantiate((GameObject)Resources.Load("Prefabs/Cursor", typeof(GameObject)), new Vector3(0f, 0f, 0f), Quaternion.identity, null);
		SettingsManager.Instance.Apply();
	}

	public void ShutDown()
	{
		InstantiationManager.Instance.Clear();
		Object.Destroy(GameObject.Find("AudioManager"));
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
