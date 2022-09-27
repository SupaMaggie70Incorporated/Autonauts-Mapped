using System.Collections.Generic;
using System.IO;
using SimpleJSON;
using UnityEngine;

public class RecordingManager : MonoBehaviour
{
	private static int m_Version = 1;

	private int m_CurrentVersion;

	private static bool m_Log = false;

	public static RecordingManager Instance;

	public Dictionary<int, ObjectRecording> m_Objects;

	public Dictionary<int, PlotRecording> m_Plots;

	public List<TileRecording> m_Tiles;

	public int m_Frame;

	private int m_ChangeIndex;

	private float m_Time;

	public float m_RecordFrequency;

	public float m_TotalTime;

	private Dictionary<int, ChangedObject> m_ChangedObjects;

	private bool m_RecordingActive;

	public int[] m_StartingTiles;

	public int m_TilesWidth;

	public int m_TilesHeight;

	private int m_PlotsWide;

	private void Awake()
	{
		Instance = this;
		m_Frame = 0;
		m_ChangeIndex = 0;
		m_Time = 0f;
		m_RecordFrequency = 5f;
		m_Objects = new Dictionary<int, ObjectRecording>();
		m_ChangedObjects = new Dictionary<int, ChangedObject>();
		m_Plots = new Dictionary<int, PlotRecording>();
		m_Tiles = new List<TileRecording>();
		if (SaveLoadManager.m_RecordingEnabled)
		{
			m_RecordingActive = GeneralUtils.m_InGame;
		}
	}

	private string GetFilename(string Name)
	{
		bool autosave = false;
		if (Name.Contains(SaveLoadManager.m_AutosaveName))
		{
			autosave = true;
		}
		return SaveFile.GetBasePath(autosave) + "/" + Name + "/Recording.txt";
	}

	public static bool Exists(string NewPath)
	{
		if (!File.Exists(NewPath + "/Recording.txt"))
		{
			return false;
		}
		return true;
	}

	private void SetStartingTiles()
	{
		m_TilesWidth = TileManager.Instance.m_TilesWide;
		m_TilesHeight = TileManager.Instance.m_TilesHigh;
		m_StartingTiles = new int[m_TilesWidth * m_TilesHeight];
		m_PlotsWide = m_TilesWidth / Plot.m_PlotTilesWide;
		Tile[] tiles = TileManager.Instance.m_Tiles;
		for (int i = 0; i < m_TilesHeight; i++)
		{
			for (int j = 0; j < m_TilesWidth; j++)
			{
				int num = i * m_TilesWidth + j;
				m_StartingTiles[num] = (int)tiles[num].m_TileType;
			}
		}
		m_Tiles.Clear();
	}

	private void GetStartingObjects()
	{
		m_Objects.Clear();
		Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("Savable");
		if (collection == null)
		{
			return;
		}
		foreach (KeyValuePair<BaseClass, int> item in collection)
		{
			if (item.Key != null && item.Key.GetComponent<Savable>().GetIsSavable())
			{
				UpdateObject(item.Key.GetComponent<TileCoordObject>());
			}
		}
	}

	public void NewLevelStarted()
	{
		if (SaveLoadManager.m_RecordingEnabled)
		{
			SetStartingTiles();
			GetStartingObjects();
			DoFrame();
		}
	}

	private void SaveStartingTiles(JSONNode rootNode)
	{
		JSONArray jSONArray = new JSONArray();
		JSONUtils.Set(rootNode, "TilesWide", m_TilesWidth);
		JSONUtils.Set(rootNode, "TilesHigh", m_TilesHeight);
		rootNode["TileTypes"] = jSONArray;
		for (int i = 0; i < m_TilesHeight; i++)
		{
			for (int j = 0; j < m_TilesWidth; j++)
			{
				int num = i * m_TilesWidth + j;
				jSONArray[num] = m_StartingTiles[num];
			}
		}
	}

	private void LoadStartingTiles(JSONNode rootNode)
	{
		m_TilesWidth = JSONUtils.GetAsInt(rootNode, "TilesWide", 0);
		m_TilesHeight = JSONUtils.GetAsInt(rootNode, "TilesHigh", 0);
		m_PlotsWide = m_TilesWidth / Plot.m_PlotTilesWide;
		m_StartingTiles = new int[m_TilesWidth * m_TilesHeight];
		JSONArray asArray = rootNode["TileTypes"].AsArray;
		for (int i = 0; i < m_TilesHeight; i++)
		{
			for (int j = 0; j < m_TilesWidth; j++)
			{
				int num = i * m_TilesWidth + j;
				m_StartingTiles[num] = asArray[num].AsInt;
			}
		}
	}

	private void SaveObjects(JSONNode rootNode)
	{
		JSONUtils.Set(rootNode, "Frame", m_Frame);
		JSONUtils.Set(rootNode, "Time", m_Time);
		JSONArray jSONArray = (JSONArray)(rootNode["Objects"] = new JSONArray());
		int num = 0;
		foreach (KeyValuePair<int, ObjectRecording> @object in m_Objects)
		{
			JSONNode jSONNode2 = new JSONObject();
			ObjectRecording value = @object.Value;
			JSONUtils.Set(jSONNode2, "UID", @object.Key);
			JSONUtils.Set(jSONNode2, "Type", value.m_ObjectType);
			JSONArray jSONArray2 = (JSONArray)(jSONNode2["Values"] = new JSONArray());
			int num2 = 0;
			foreach (RecordingStamp stamp in value.m_Stamps)
			{
				jSONArray2[num2] = stamp.f;
				num2++;
				jSONArray2[num2] = stamp.i;
				num2++;
				jSONArray2[num2] = stamp.x;
				num2++;
				if (RecordingStamp.GetIsSpecial(stamp.x))
				{
					jSONArray2[num2] = stamp.GetSpecialData1AsString();
					num2++;
					jSONArray2[num2] = stamp.GetSpecialData2AsString();
					num2++;
				}
				else
				{
					jSONArray2[num2] = stamp.y;
					num2++;
					jSONArray2[num2] = stamp.r;
					num2++;
				}
			}
			jSONArray[num] = jSONNode2;
			num++;
		}
	}

	private void SavePlots(JSONNode rootNode)
	{
		JSONArray jSONArray = (JSONArray)(rootNode["Plots"] = new JSONArray());
		int num = 0;
		foreach (KeyValuePair<int, PlotRecording> plot in m_Plots)
		{
			PlotRecording value = plot.Value;
			jSONArray[num] = value.f;
			num++;
			jSONArray[num] = value.x;
			num++;
			jSONArray[num] = value.y;
			num++;
		}
	}

	private void SaveTiles(JSONNode rootNode)
	{
		JSONArray jSONArray = (JSONArray)(rootNode["Tiles"] = new JSONArray());
		int num = 0;
		foreach (TileRecording tile in m_Tiles)
		{
			jSONArray[num] = tile.f;
			num++;
			jSONArray[num] = tile.x;
			num++;
			jSONArray[num] = tile.y;
			num++;
			jSONArray[num] = (int)tile.m_Type;
			num++;
		}
	}

	private void LoadObjects(JSONNode rootNode)
	{
		m_Frame = JSONUtils.GetAsInt(rootNode, "Frame", 0);
		m_Time = JSONUtils.GetAsFloat(rootNode, "Time", 0f);
		m_TotalTime = (float)(m_Frame - 1) * m_RecordFrequency;
		JSONArray asArray = rootNode["Objects"].AsArray;
		for (int i = 0; i < asArray.Count; i++)
		{
			JSONObject asObject = asArray[i].AsObject;
			int asInt = JSONUtils.GetAsInt(asObject, "UID", 0);
			ObjectRecording objectRecording = new ObjectRecording(JSONUtils.GetAsString(asObject, "Type", ""));
			m_Objects.Add(asInt, objectRecording);
			JSONArray asArray2 = asObject["Values"].AsArray;
			int num = 0;
			while (num < asArray2.Count)
			{
				int asInt2 = asArray2[num].AsInt;
				num++;
				int i2 = 0;
				if (m_CurrentVersion != 0)
				{
					i2 = asArray2[num].AsInt;
					num++;
				}
				int asInt3 = asArray2[num].AsInt;
				num++;
				if (RecordingStamp.GetIsSpecial(asInt3))
				{
					string data = asArray2[num];
					num++;
					string data2 = asArray2[num];
					num++;
					objectRecording.AddStamp(asInt2, i2, (RecordingStamp.SpecialMessage)asInt3, data, data2);
				}
				else
				{
					int asInt4 = asArray2[num].AsInt;
					num++;
					int asInt5 = asArray2[num].AsInt;
					num++;
					objectRecording.AddStamp(asInt2, i2, asInt3, asInt4, asInt5);
				}
			}
		}
	}

	private void LoadPlots(JSONNode rootNode)
	{
		JSONArray asArray = rootNode["Plots"].AsArray;
		int num = 0;
		while (num < asArray.Count)
		{
			int asInt = asArray[num].AsInt;
			num++;
			int asInt2 = asArray[num].AsInt;
			num++;
			int asInt3 = asArray[num].AsInt;
			num++;
			int key = asInt3 * m_PlotsWide + asInt2;
			m_Plots.Add(key, new PlotRecording(asInt, asInt2, asInt3));
		}
	}

	private void LoadTiles(JSONNode rootNode)
	{
		JSONArray asArray = rootNode["Tiles"].AsArray;
		int num = 0;
		while (num < asArray.Count)
		{
			int asInt = asArray[num].AsInt;
			num++;
			int asInt2 = asArray[num].AsInt;
			num++;
			int asInt3 = asArray[num].AsInt;
			num++;
			Tile.TileType asInt4 = (Tile.TileType)asArray[num].AsInt;
			num++;
			m_Tiles.Add(new TileRecording(asInt, asInt2, asInt3, asInt4));
		}
	}

	public void Save(string Name)
	{
		Debug.Log("Save");
		if (SaveLoadManager.m_RecordingEnabled)
		{
			string filename = GetFilename(Name);
			JSONNode jSONNode = new JSONObject();
			JSONUtils.Set(jSONNode, "Version", m_Version);
			SaveStartingTiles(jSONNode);
			SaveTiles(jSONNode);
			SaveObjects(jSONNode);
			SavePlots(jSONNode);
			string finalString = jSONNode.ToString();
			SaveFile.Save(filename, finalString);
		}
	}

	public void Load(string Name)
	{
		Debug.Log("Load");
		if (SaveLoadManager.m_RecordingEnabled)
		{
			string filename = GetFilename(Name);
			if (!File.Exists(filename))
			{
				NewLevelStarted();
				return;
			}
			JSONNode jSONNode = JSON.Parse(File.ReadAllText(filename));
			m_CurrentVersion = JSONUtils.GetAsInt(jSONNode, "Version", 0);
			LoadStartingTiles(jSONNode);
			LoadTiles(jSONNode);
			LoadObjects(jSONNode);
			LoadPlots(jSONNode);
		}
	}

	public void RemoveObject(TileCoordObject NewObject)
	{
		if (!m_RecordingActive || NewObject.m_UniqueID == -1)
		{
			return;
		}
		int uniqueID = NewObject.m_UniqueID;
		if (!m_ChangedObjects.ContainsKey(uniqueID))
		{
			if (m_Objects.ContainsKey(uniqueID))
			{
				m_Objects[uniqueID].AddStamp(m_Frame, m_ChangeIndex, 0, 0, 0);
				m_ChangeIndex++;
			}
		}
		else if (!m_Objects.ContainsKey(uniqueID))
		{
			m_ChangedObjects.Remove(uniqueID);
		}
		else
		{
			m_Objects[uniqueID].AddStamp(m_Frame, m_ChangeIndex, 0, 0, 0);
			m_ChangeIndex++;
			m_ChangedObjects.Remove(uniqueID);
		}
	}

	public void IgnoreObject(TileCoordObject NewObject)
	{
		if (m_RecordingActive && !SaveLoadManager.Instance.m_Loading && NewObject.m_UniqueID != -1)
		{
			if (m_Log)
			{
				Debug.Log(string.Concat("Ignore ", NewObject.m_TypeIdentifier, " ", NewObject.m_UniqueID));
			}
			int uniqueID = NewObject.m_UniqueID;
			if (m_Objects.ContainsKey(uniqueID))
			{
				m_Objects.Remove(uniqueID);
			}
			if (m_ChangedObjects.ContainsKey(uniqueID))
			{
				m_ChangedObjects.Remove(uniqueID);
			}
		}
	}

	public ChangedObject UpdateObject(TileCoordObject NewObject)
	{
		if (!m_RecordingActive)
		{
			return null;
		}
		if (SaveLoadManager.Instance.m_Loading)
		{
			return null;
		}
		if (NewObject.m_UniqueID == -1)
		{
			return null;
		}
		ObjectType typeIdentifier = NewObject.m_TypeIdentifier;
		if (!Flora.GetIsTypeFlora(typeIdentifier) && !ObjectTypeList.Instance.GetIsBuilding(typeIdentifier) && typeIdentifier != ObjectType.Worker && typeIdentifier != ObjectType.FarmerPlayer && typeIdentifier != ObjectType.Boulder && typeIdentifier != ObjectType.TallBoulder)
		{
			return null;
		}
		if (typeIdentifier == ObjectType.ConverterFoundation)
		{
			return null;
		}
		if (m_Log)
		{
			Debug.Log(string.Concat("Update ", NewObject.m_TypeIdentifier, " ", NewObject.m_UniqueID));
		}
		int uniqueID = NewObject.m_UniqueID;
		if (!m_ChangedObjects.ContainsKey(uniqueID))
		{
			ChangedObject changedObject = new ChangedObject(NewObject);
			m_ChangedObjects.Add(uniqueID, changedObject);
			return changedObject;
		}
		return m_ChangedObjects[uniqueID];
	}

	public void SpecialMessage(TileCoordObject NewObject, RecordingStamp.SpecialMessage NewMessage, object Data1, object Data2)
	{
		if (m_RecordingActive && !SaveLoadManager.Instance.m_Loading && NewObject.m_UniqueID != -1)
		{
			UpdateObject(NewObject).AddSpecialMessage(NewMessage, Data1, Data2);
		}
	}

	public void ShowPlot(Plot NewPlot)
	{
		if (m_RecordingActive && (!SaveLoadManager.Instance.m_Loading || SaveLoadManager.Instance.m_Creating))
		{
			int key = NewPlot.m_PlotY * m_PlotsWide + NewPlot.m_PlotX;
			if (!m_Plots.ContainsKey(key))
			{
				m_Plots.Add(key, new PlotRecording(m_Frame, NewPlot.m_PlotX, NewPlot.m_PlotY));
			}
		}
	}

	public void ChangeTile(int x, int y, Tile.TileType NewType)
	{
		if (m_RecordingActive && !SaveLoadManager.Instance.m_Loading)
		{
			m_Tiles.Add(new TileRecording(m_Frame, x, y, NewType));
		}
	}

	private void AddStamp(ObjectRecording NewRecording, TileCoordObject NewObject, ChangedObject NewChange, bool New)
	{
		if (NewChange.m_Specials.Count == 0 || New)
		{
			int r = 0;
			if (NewObject.m_TypeIdentifier == ObjectType.Worker || NewObject.m_TypeIdentifier == ObjectType.FarmerPlayer)
			{
				r = NewObject.GetComponent<Farmer>().m_Rotation;
			}
			else if (NewObject.m_TypeIdentifier == ObjectType.FlowerWild)
			{
				r = (int)NewObject.GetComponent<FlowerWild>().m_Type;
			}
			else if (ObjectTypeList.Instance.GetIsBuilding(NewObject.m_TypeIdentifier))
			{
				r = NewObject.GetComponent<Building>().m_Rotation;
			}
			NewRecording.AddStamp(m_Frame, m_ChangeIndex, NewObject.m_TileCoord.x, NewObject.m_TileCoord.y, r);
		}
		if (NewChange.m_Specials.Count == 0)
		{
			return;
		}
		foreach (ChangedObjectSpecial special in NewChange.m_Specials)
		{
			NewRecording.AddStamp(m_Frame, m_ChangeIndex, special.m_SpecialMessage, special.m_Data1, special.m_Data2);
		}
	}

	private void DoFrame()
	{
		foreach (KeyValuePair<int, ChangedObject> changedObject in m_ChangedObjects)
		{
			int key = changedObject.Key;
			ChangedObject value = changedObject.Value;
			TileCoordObject @object = value.m_Object;
			if (!m_Objects.ContainsKey(key))
			{
				ObjectRecording objectRecording = new ObjectRecording(ObjectTypeList.Instance.GetSaveNameFromIdentifier(@object.m_TypeIdentifier));
				AddStamp(objectRecording, @object, value, New: true);
				m_ChangeIndex++;
				m_Objects.Add(key, objectRecording);
			}
			else
			{
				ObjectRecording objectRecording = m_Objects[key];
				AddStamp(objectRecording, @object, value, New: false);
				m_ChangeIndex++;
			}
		}
		m_ChangedObjects.Clear();
		m_Frame++;
		m_ChangeIndex = 0;
	}

	private void Update()
	{
		if (m_RecordingActive && !SaveLoadManager.Instance.m_Loading)
		{
			m_Time += TimeManager.Instance.m_NormalDelta;
			if (m_Time >= m_RecordFrequency)
			{
				m_Time -= m_RecordFrequency;
				DoFrame();
			}
		}
	}
}
