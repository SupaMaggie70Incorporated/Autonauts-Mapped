using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class SaveJSON : MonoBehaviour
{
	public static void Capture(JSONNode rootNode)
	{
		rootNode["Version"] = SaveLoadManager.m_Version;
		rootNode["External"] = SaveLoadManager.Instance.m_External;
		rootNode["GameOptions"] = new JSONObject();
		JSONNode node = rootNode["GameOptions"];
		GameOptionsManager.Instance.m_Options.Save(node);
		rootNode["Time"] = new JSONObject();
		JSONNode node2 = rootNode["Time"];
		TimeManager.Instance.Save(node2);
		rootNode["DayNight"] = new JSONObject();
		JSONNode node3 = rootNode["DayNight"];
		DayNightManager.Instance.Save(node3);
		JSONUtils.Set(rootNode, "CameraDistance", CameraManager.Instance.m_Distance);
		JSONUtils.Set(rootNode, "CameraX", CameraManager.Instance.m_CameraPosition.x);
		JSONUtils.Set(rootNode, "CameraZ", CameraManager.Instance.m_CameraPosition.z);
		rootNode["Tiles"] = new JSONObject();
		JSONNode node4 = rootNode["Tiles"];
		TileManager.Instance.Save(node4);
		rootNode["Plots"] = new JSONObject();
		JSONNode node5 = rootNode["Plots"];
		PlotManager.Instance.Save(node5);
		rootNode["Scripts"] = new JSONObject();
		JSONNode node6 = rootNode["Scripts"];
		WorkerScriptManager.Instance.Save(node6);
		rootNode["Resources"] = new JSONObject();
		JSONNode node7 = rootNode["Resources"];
		ResourceManager.Instance.Save(node7);
		rootNode["ObjectTypes"] = new JSONObject();
		JSONNode node8 = rootNode["ObjectTypes"];
		ObjectTypeList.Instance.Save(node8);
		rootNode["SpawnAnimations"] = new JSONObject();
		JSONNode node9 = rootNode["SpawnAnimations"];
		SpawnAnimationManager.Instance.Save(node9);
		rootNode["WorldSettings"] = new JSONObject();
		JSONNode node10 = rootNode["WorldSettings"];
		WorldSettings.Instance.Save(node10);
		rootNode["Water"] = new JSONObject();
		JSONNode node11 = rootNode["Water"];
		WaterManager.Instance.Save(node11);
		rootNode["Soil"] = new JSONObject();
		JSONNode node12 = rootNode["Soil"];
		SoilManager.Instance.Save(node12);
		rootNode["Quest"] = new JSONObject();
		JSONNode node13 = rootNode["Quest"];
		QuestManager.Instance.Save(node13);
		rootNode["Stats"] = new JSONObject();
		JSONNode node14 = rootNode["Stats"];
		StatsManager.Instance.Save(node14);
		rootNode["Folk"] = new JSONObject();
		JSONNode node15 = rootNode["Folk"];
		FolkManager.Instance.Save(node15);
		rootNode["Wardrobe"] = new JSONObject();
		JSONNode node16 = rootNode["Wardrobe"];
		WardrobeManager.Instance.Save(node16);
		rootNode["CameraSequence"] = new JSONObject();
		JSONNode node17 = rootNode["CameraSequence"];
		CameraSequence.Instance.Save(node17);
		rootNode["Planning"] = new JSONObject();
		JSONNode parentNode = rootNode["Planning"];
		PlanningManager.Instance.Save(parentNode);
		rootNode["OffworldMissions"] = new JSONObject();
		JSONNode rootNode2 = rootNode["OffworldMissions"];
		OffworldMissionsManager.Instance.Save(rootNode2);
		JSONArray jSONArray = (JSONArray)(rootNode["Objects"] = new JSONArray());
		Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("Savable");
		int num = 0;
		foreach (KeyValuePair<BaseClass, int> item in collection)
		{
			if (item.Key != null && item.Key.GetComponent<Savable>().GetIsSavable())
			{
				JSONNode jSONNode2 = new JSONObject();
				string saveNameFromIdentifier = ObjectTypeList.Instance.GetSaveNameFromIdentifier(item.Key.GetComponent<BaseClass>().m_TypeIdentifier);
				jSONNode2["ID"] = saveNameFromIdentifier;
				item.Key.GetComponent<Savable>().Save(jSONNode2);
				jSONArray[num] = jSONNode2;
				num++;
			}
		}
		rootNode["DespawnManager"] = new JSONObject();
		JSONNode node18 = rootNode["DespawnManager"];
		DespawnManager.Instance.Save(node18);
		rootNode["BaggedManager"] = new JSONObject();
		JSONNode node19 = rootNode["BaggedManager"];
		BaggedManager.Instance.Save(node19);
		rootNode["WorkerGroupManager"] = new JSONObject();
		JSONNode node20 = rootNode["WorkerGroupManager"];
		WorkerGroupManager.Instance.Save(node20);
	}
}
