using System;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;
using UnityEngine;

public class LoadJSON
{
	private enum Stage
	{
		LoadFromDisk,
		JSONParse,
		InitialSetup,
		LoadTiles,
		LoadPlots,
		LoadManagers,
		PrepareToLoadObjects,
		LoadObjects,
		EndLoad,
		MergePlots,
		Total
	}

	private static string m_ValidString = "AutonautsWorld";

	private static bool m_Log = false;

	public static bool m_Loading = false;

	private static string m_FileName = "";

	private static Stage m_Stage = Stage.Total;

	private static string m_FinalString = "";

	private static JSONNode m_RootNode;

	public static int m_LoadingVersion;

	public static int m_LoadingVersionFraction;

	public static string m_LoadingString;

	private static int m_External;

	private static JSONArray m_LoadingArray;

	private static int m_LoadingIndex;

	private static List<BaseClass> m_DisabledObjects;

	public static void StartLoad(string FileName)
	{
		m_FileName = FileName;
		m_Stage = Stage.LoadFromDisk;
		m_FinalString = "";
		m_Loading = true;
		m_DisabledObjects = new List<BaseClass>();
		if (m_Log)
		{
			Debug.Log("LoadJSON Start Load " + FileName);
		}
	}

	private static void LoadFromDisk()
	{
		try
		{
			m_FinalString = File.ReadAllText(m_FileName);
		}
		catch (UnauthorizedAccessException ex)
		{
			ErrorMessage.LogError("World Load - UnauthorizedAccessException : " + m_FileName + " " + ex.ToString());
		}
		catch (IOException ex2)
		{
			ErrorMessage.LogError("World Load - IOException : " + m_FileName + " " + ex2.ToString());
		}
	}

	private static void JSONParse()
	{
		JSONNode jSONNode = JSON.Parse(m_FinalString);
		m_FinalString = "";
		if (jSONNode == null)
		{
			ErrorMessage.LogError("World Load - Invalid JSON file : " + m_FileName);
			return;
		}
		if (JSONUtils.GetAsInt(jSONNode, m_ValidString, 0) == 0)
		{
			ErrorMessage.LogError("World Load - Invalid Autonauts file : " + m_FileName);
			return;
		}
		JSONNode jSONNode2 = jSONNode["GameOptions"];
		if (jSONNode2 == null || jSONNode2.IsNull)
		{
			ErrorMessage.LogError("World Load - Old Autonauts file : " + m_FileName);
		}
		else
		{
			m_RootNode = jSONNode;
		}
	}

	private static void InitialSetup()
	{
		string text = m_RootNode["Version"];
		string[] array = text.Split('.');
		m_LoadingVersion = 0;
		if (array.Length != 0)
		{
			int.TryParse(array[0], out m_LoadingVersion);
		}
		if (m_LoadingVersion == 0)
		{
			m_LoadingVersion = 1000000;
		}
		m_LoadingVersionFraction = 0;
		if (array.Length > 1)
		{
			int.TryParse(array[1], out m_LoadingVersionFraction);
		}
		Debug.Log(m_LoadingVersion + " " + m_LoadingVersionFraction);
		m_LoadingString = text;
		JSONNode jSONNode = m_RootNode["External"];
		if (jSONNode != null && !jSONNode.IsNull)
		{
			m_External = jSONNode.AsInt;
		}
		else
		{
			m_External = 0;
		}
		JSONNode jSONNode2 = m_RootNode["GameOptions"];
		if (jSONNode2 != null && !jSONNode2.IsNull)
		{
			GameOptionsManager.Instance.m_Options.Load(jSONNode2);
		}
		if (m_External == 1)
		{
			HudManager.Instance.SetExternalLevel();
		}
		JSONNode jSONNode3 = m_RootNode["Time"];
		if (jSONNode3 != null && !jSONNode3.IsNull)
		{
			TimeManager.Instance.Load(jSONNode3);
		}
		JSONNode jSONNode4 = m_RootNode["DayNight"];
		if (jSONNode4 != null && !jSONNode4.IsNull)
		{
			DayNightManager.Instance.Load(jSONNode4);
		}
		float asFloat = JSONUtils.GetAsFloat(m_RootNode, "CameraDistance", 0f);
		float asFloat2 = JSONUtils.GetAsFloat(m_RootNode, "CameraX", 0f);
		float asFloat3 = JSONUtils.GetAsFloat(m_RootNode, "CameraZ", 0f);
		CameraManager.Instance.SetDistance(asFloat);
		CameraManager.Instance.Focus(new Vector3(asFloat2, 0f, asFloat3));
	}

	private static void LoadTiles()
	{
		JSONNode jSONNode = m_RootNode["Tiles"];
		if (jSONNode != null && !jSONNode.IsNull)
		{
			TileManager.Instance.Load(jSONNode);
		}
	}

	private static void LoadPlots()
	{
		JSONNode jSONNode = m_RootNode["Plots"];
		if (jSONNode != null && !jSONNode.IsNull)
		{
			PlotManager.Instance.Load(jSONNode);
		}
	}

	private static void LoadManagers()
	{
		JSONNode jSONNode = m_RootNode["Scripts"];
		if (jSONNode != null && !jSONNode.IsNull)
		{
			WorkerScriptManager.Instance.Load(jSONNode);
		}
		JSONNode jSONNode2 = m_RootNode["Resources"];
		if (jSONNode2 != null && !jSONNode2.IsNull)
		{
			ResourceManager.Instance.Load(jSONNode2);
		}
		JSONNode jSONNode3 = m_RootNode["ObjectTypes"];
		if (jSONNode3 != null && !jSONNode3.IsNull)
		{
			ObjectTypeList.Instance.Load(jSONNode3);
		}
		JSONNode jSONNode4 = m_RootNode["Water"];
		if (jSONNode4 != null && !jSONNode4.IsNull)
		{
			WaterManager.Instance.Load(jSONNode4);
		}
		JSONNode jSONNode5 = m_RootNode["Soil"];
		if (jSONNode5 != null && !jSONNode5.IsNull)
		{
			SoilManager.Instance.Load(jSONNode5);
		}
		JSONNode jSONNode6 = m_RootNode["Quest"];
		if (jSONNode6 != null && !jSONNode6.IsNull)
		{
			QuestManager.Instance.Load(jSONNode6);
		}
		JSONNode jSONNode7 = m_RootNode["Stats"];
		if (jSONNode7 != null && !jSONNode7.IsNull)
		{
			StatsManager.Instance.Load(jSONNode7);
		}
		JSONNode jSONNode8 = m_RootNode["Folk"];
		if (jSONNode8 != null && !jSONNode8.IsNull)
		{
			FolkManager.Instance.Load(jSONNode8);
		}
		JSONNode jSONNode9 = m_RootNode["Wardrobe"];
		if (jSONNode9 != null && !jSONNode9.IsNull)
		{
			WardrobeManager.Instance.Load(jSONNode9);
		}
		JSONNode jSONNode10 = m_RootNode["CameraSequence"];
		if (jSONNode10 != null && !jSONNode10.IsNull)
		{
			CameraSequence.Instance.Load(jSONNode10);
		}
		JSONNode jSONNode11 = m_RootNode["Planning"];
		if (jSONNode11 != null && !jSONNode11.IsNull)
		{
			PlanningManager.Instance.Load(jSONNode11);
		}
		JSONNode jSONNode12 = m_RootNode["OffworldMissions"];
		if (jSONNode12 != null && !jSONNode12.IsNull)
		{
			OffworldMissionsManager.Instance.Load(jSONNode12);
		}
	}

	private static void PrepareToLoadObjects()
	{
		m_LoadingArray = m_RootNode["Objects"].AsArray;
		m_LoadingIndex = 0;
		Plot[] plots = PlotManager.Instance.m_Plots;
		foreach (Plot plot in plots)
		{
			plot.gameObject.SetActive(value: false);
			m_DisabledObjects.Add(plot);
		}
	}

	private static bool LoadObjects()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		int num = 0;
		do
		{
			JSONNode asObject = m_LoadingArray[m_LoadingIndex].AsObject;
			int asInt = JSONUtils.GetAsInt(asObject, "UID", -1);
			if (ObjectTypeList.Instance.GetObjectFromUniqueID(asInt, ErrorCheck: false) == null)
			{
				BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromSaveName(asObject, new Vector3(0f, 0f, 0f), Quaternion.identity);
				ObjectType identifierFromSaveName = ObjectTypeList.Instance.GetIdentifierFromSaveName(JSONUtils.GetAsString(asObject, "New", ""), Check: false);
				if ((bool)baseClass && baseClass.m_TypeIdentifier == ObjectType.ConverterFoundation && identifierFromSaveName == ObjectTypeList.m_Total)
				{
					baseClass.StopUsing();
					baseClass = null;
				}
				if ((bool)baseClass)
				{
					baseClass.GetComponent<Savable>().Load(asObject);
					if ((bool)baseClass.GetComponent<Building>())
					{
						baseClass.GetComponent<Building>().PostLoad(asObject);
					}
					baseClass.gameObject.SetActive(value: false);
					m_DisabledObjects.Add(baseClass);
				}
			}
			m_LoadingIndex++;
			if (m_LoadingIndex == m_LoadingArray.Count)
			{
				break;
			}
			num++;
		}
		while (!(Time.realtimeSinceStartup - realtimeSinceStartup > 0.1f));
		if (m_LoadingIndex >= m_LoadingArray.Count)
		{
			return true;
		}
		return false;
	}

	private static void EndLoad()
	{
		JSONNode jSONNode = m_RootNode["SpawnAnimations"];
		if (jSONNode != null && !jSONNode.IsNull)
		{
			SpawnAnimationManager.Instance.Load(jSONNode);
		}
		JSONNode jSONNode2 = m_RootNode["DespawnManager"];
		if (jSONNode2 != null && !jSONNode2.IsNull)
		{
			DespawnManager.Instance.Load(jSONNode2);
		}
		JSONNode jSONNode3 = m_RootNode["WorkerGroupManager"];
		if (jSONNode3 != null && !jSONNode3.IsNull)
		{
			WorkerGroupManager.Instance.Load(jSONNode3);
		}
		JSONNode jSONNode4 = m_RootNode["BaggedManager"];
		if (jSONNode4 != null && !jSONNode4.IsNull)
		{
			BaggedManager.Instance.Load(jSONNode4);
		}
		JSONNode jSONNode5 = m_RootNode["WorldSettings"];
		if (jSONNode5 != null && !jSONNode5.IsNull)
		{
			WorldSettings.Instance.Load(jSONNode5);
		}
		ObjectTypeList.Instance.SetLoading(Loading: false);
		Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("Savable");
		foreach (KeyValuePair<BaseClass, int> item in collection)
		{
			item.Key.GetComponent<Savable>().PostLoad();
		}
		foreach (KeyValuePair<BaseClass, int> item2 in collection)
		{
			BaseClass key = item2.Key;
			if (key.GetComponent<Savable>().GetIsSavable())
			{
				key.GetComponent<Actionable>().SendAction(new ActionInfo(ActionType.Refresh, default(TileCoord)));
			}
		}
		SpawnAnimationManager.Instance.PostLoad();
		JSONNode jSONNode6 = m_RootNode["RebuildScripts"];
		if (jSONNode6 != null && !jSONNode6.IsNull && jSONNode6.AsInt == 1)
		{
			foreach (KeyValuePair<BaseClass, int> item3 in CollectionManager.Instance.GetCollection("Worker"))
			{
				item3.Key.GetComponent<Worker>().m_WorkerInterpreter.UpdateCurrentScript();
			}
		}
		foreach (BaseClass disabledObject in m_DisabledObjects)
		{
			if ((bool)disabledObject && disabledObject.m_UniqueID != -1)
			{
				disabledObject.gameObject.SetActive(value: true);
			}
		}
		TabManager.Instance.PostLoad();
		FolkManager.Instance.UpdateFolkTiers();
		ModeButton.Get(ModeButton.Type.Evolution).UpdateFolkLevel();
		OldFileUtils.CheckPostLoad();
	}

	private static void MergePlots()
	{
		PlotManager.Instance.FinishMerge();
		PlotObjectMergerManager.Instance.FinishAllDirty();
	}

	public static void Update()
	{
		if (!m_Loading)
		{
			return;
		}
		bool flag = true;
		switch (m_Stage)
		{
		case Stage.LoadFromDisk:
			LoadFromDisk();
			break;
		case Stage.JSONParse:
			JSONParse();
			break;
		case Stage.InitialSetup:
			InitialSetup();
			break;
		case Stage.LoadTiles:
			LoadTiles();
			break;
		case Stage.LoadPlots:
			LoadPlots();
			break;
		case Stage.LoadManagers:
			LoadManagers();
			break;
		case Stage.LoadObjects:
			flag = LoadObjects();
			break;
		case Stage.EndLoad:
			EndLoad();
			break;
		case Stage.MergePlots:
			MergePlots();
			break;
		}
		if (flag)
		{
			m_Stage++;
			if (m_Log)
			{
				Debug.Log("LoadJSON Stage " + m_Stage);
			}
			if (m_Stage == Stage.LoadObjects)
			{
				PrepareToLoadObjects();
			}
			else if (m_Stage == Stage.Total)
			{
				m_Loading = false;
				m_RootNode = null;
				m_LoadingArray = null;
				m_DisabledObjects = null;
			}
		}
	}

	public static float GetLoadPercent()
	{
		float num = 0f;
		float num2 = 0.5f;
		float num3 = 1f - num2;
		if (m_Stage >= Stage.LoadObjects)
		{
			num = (float)(m_Stage - 1) / 10f * num3;
			return num + (float)m_LoadingIndex / (float)m_LoadingArray.Count * num2;
		}
		return (float)m_Stage / 10f * num3;
	}
}
