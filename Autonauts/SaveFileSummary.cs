using System;
using System.IO;
using SimpleJSON;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveFileSummary
{
	public enum Resource
	{
		Stone,
		Clay,
		Metal,
		Coal,
		Water,
		SeaWater,
		Swamp,
		Sand,
		Trees,
		CropWheat,
		CropCotton,
		Total
	}

	private static string m_DefaultName = "Summary.txt";

	public string m_Version;

	public int m_External;

	public GameOptions m_GameOptions;

	public int m_DateDay;

	public int m_DateTime;

	private int[] m_ResourceCounts;

	public static Tile.TileType[] m_MapTileData;

	public string GetDateString()
	{
		return default(DateTime).AddDays(m_DateDay).AddSeconds(m_DateTime).ToString("dd-MM-yyyy_HH-mm-ss");
	}

	public long GetDateLong()
	{
		return (long)m_DateDay * 24L * 60 * 60 + m_DateTime;
	}

	private void GetDate()
	{
		DateTime now = DateTime.Now;
		m_DateDay = (now - DateTime.MinValue).Days;
		m_DateTime = now.Hour * 60 * 60 + now.Minute * 60 + now.Second;
	}

	public SaveFileSummary()
	{
		m_Version = "0";
		m_External = 0;
		m_GameOptions = new GameOptions();
		GetDate();
	}

	private void GetResourceCounts()
	{
		int num = 11;
		m_ResourceCounts = new int[num];
		for (int i = 0; i < num; i++)
		{
			int Count = 0;
			Sprite NewImage = null;
			int Min = 0;
			string Rollover = "";
			Tile.TileType NewTileType = Tile.TileType.Total;
			GetResourceInfoStatic((Resource)i, out Count, out Min, out NewImage, out Rollover, out NewTileType, GameOptionsManager.Instance.m_Options.m_MapTileData);
			m_ResourceCounts[i] = Count;
		}
	}

	public void Capture()
	{
		m_Version = SaveLoadManager.m_Version;
		m_External = SaveLoadManager.Instance.m_External;
		m_GameOptions = GameOptionsManager.Instance.m_Options;
		GetDate();
		GetResourceCounts();
	}

	private static string GetFileName(string NewPath)
	{
		return NewPath + "/" + m_DefaultName;
	}

	private string GetResourceName(Resource NewType)
	{
		return "Resource" + NewType;
	}

	private void EncodeToJSON(JSONNode rootNode)
	{
		JSONUtils.Set(rootNode, "Version", m_Version);
		JSONUtils.Set(rootNode, "External", m_External);
		rootNode["GameOptions"] = new JSONObject();
		JSONNode node = rootNode["GameOptions"];
		GameOptionsManager.Instance.m_Options.Save(node);
		JSONUtils.Set(rootNode, "DateDay", m_DateDay);
		JSONUtils.Set(rootNode, "DateTime", m_DateTime);
		int num = 11;
		for (int i = 0; i < num; i++)
		{
			string resourceName = GetResourceName((Resource)i);
			JSONUtils.Set(rootNode, resourceName, m_ResourceCounts[i]);
		}
	}

	private void DecodeFromJSON(JSONNode rootNode)
	{
		m_Version = JSONUtils.GetAsString(rootNode, "Version", SaveLoadManager.m_Version);
		m_External = JSONUtils.GetAsInt(rootNode, "External", 0);
		JSONNode jSONNode = rootNode["GameOptions"];
		if (jSONNode != null && !jSONNode.IsNull)
		{
			m_GameOptions = new GameOptions();
			m_GameOptions.Load(jSONNode);
		}
		m_DateDay = JSONUtils.GetAsInt(rootNode, "DateDay", 0);
		m_DateTime = JSONUtils.GetAsInt(rootNode, "DateTime", 0);
		int num = 11;
		m_ResourceCounts = new int[num];
		for (int i = 0; i < num; i++)
		{
			string resourceName = GetResourceName((Resource)i);
			m_ResourceCounts[i] = JSONUtils.GetAsInt(rootNode, resourceName, 0);
		}
	}

	public void Save(string NewPath)
	{
		Capture();
		JSONNode jSONNode = new JSONObject();
		JSONUtils.Set(jSONNode, "AutonautsSummary", 1);
		EncodeToJSON(jSONNode);
		string finalString = jSONNode.ToString();
		string fileName = GetFileName(NewPath);
		if (!Directory.Exists(NewPath))
		{
			ErrorMessage.LogError("Summary Save - Folder doesn't exist : " + NewPath);
		}
		else
		{
			SaveFile.Save(fileName, finalString);
		}
	}

	public bool Exists(string NewPath)
	{
		return File.Exists(GetFileName(NewPath));
	}

	public void Load(string NewPath)
	{
		string fileName = GetFileName(NewPath);
		if (!File.Exists(fileName))
		{
			ErrorMessage.LogError("Summary Load - File doesn't exist : " + fileName);
			return;
		}
		string aJSON;
		try
		{
			aJSON = File.ReadAllText(fileName);
		}
		catch (UnauthorizedAccessException ex)
		{
			ErrorMessage.LogError("Summary Load - UnauthorizedAccessException : " + fileName + " " + ex.ToString());
			return;
		}
		JSONNode jSONNode = JSON.Parse(aJSON);
		if (jSONNode == null)
		{
			ErrorMessage.LogError("Summary Load - Invalid JSON file : " + fileName);
		}
		else if (JSONUtils.GetAsInt(jSONNode, "AutonautsSummary", 0) == 0)
		{
			ErrorMessage.LogError("Summary Load - Invalid Autonauts file : " + fileName);
		}
		else
		{
			DecodeFromJSON(jSONNode);
		}
	}

	public void GetResourceInfo(Resource NewResource, out int Count, out int Min, out Sprite NewImage, out string Rollover, out Tile.TileType NewTileType)
	{
		GetResourceInfoStatic(NewResource, out Count, out Min, out NewImage, out Rollover, out NewTileType, null);
		Count = m_ResourceCounts[(int)NewResource];
	}

	public static void GetResourceInfoStatic(Resource NewResource, out int Count, out int Min, out Sprite NewImage, out string Rollover, out Tile.TileType NewTileType, Tile.TileType[] MapTileData)
	{
		m_MapTileData = MapTileData;
		Count = 0;
		NewImage = null;
		Min = 0;
		Rollover = "";
		NewTileType = Tile.TileType.Total;
		switch (NewResource)
		{
		case Resource.Stone:
			NewTileType = Tile.TileType.Stone;
			Count = GetTileTypeCount(Tile.TileType.Stone, Tile.TileType.StoneHidden, Tile.TileType.StoneSoil, Tile.TileType.StoneUsed);
			NewImage = Tile.GetIcon(Tile.TileType.Stone);
			Min = 100;
			Rollover = "TileStone";
			break;
		case Resource.Clay:
			NewTileType = Tile.TileType.Clay;
			Count = GetTileTypeCount(Tile.TileType.Clay, Tile.TileType.ClayHidden, Tile.TileType.ClaySoil, Tile.TileType.ClayUsed);
			NewImage = Tile.GetIcon(Tile.TileType.Clay);
			Min = 100;
			Rollover = "TileClay";
			break;
		case Resource.Metal:
			NewTileType = Tile.TileType.Iron;
			Count = GetTileTypeCount(Tile.TileType.IronHidden, Tile.TileType.IronSoil, Tile.TileType.IronSoil2, Tile.TileType.IronUsed, Tile.TileType.Iron);
			NewImage = Tile.GetIcon(Tile.TileType.Iron);
			Min = 100;
			Rollover = "TileIron";
			break;
		case Resource.Coal:
			NewTileType = Tile.TileType.Coal;
			Count = GetTileTypeCount(Tile.TileType.Coal, Tile.TileType.CoalHidden, Tile.TileType.CoalSoil, Tile.TileType.CoalSoil2, Tile.TileType.CoalSoil3, Tile.TileType.CoalUsed);
			NewImage = Tile.GetIcon(Tile.TileType.Coal);
			Min = 100;
			Rollover = "TileCoal";
			break;
		case Resource.Water:
			NewTileType = Tile.TileType.WaterShallow;
			Count = GetTileTypeCount(Tile.TileType.WaterShallow, Tile.TileType.WaterDeep);
			NewImage = Tile.GetIcon(Tile.TileType.WaterShallow);
			Min = 100;
			Rollover = "Water";
			break;
		case Resource.SeaWater:
			NewTileType = Tile.TileType.SeaWaterShallow;
			Count = GetTileTypeCount(Tile.TileType.SeaWaterShallow, Tile.TileType.SeaWaterDeep);
			NewImage = Tile.GetIcon(Tile.TileType.SeaWaterShallow);
			Min = 100;
			Rollover = "SeaWater";
			break;
		case Resource.Swamp:
			NewTileType = Tile.TileType.Swamp;
			Count = GetTileTypeCount(Tile.TileType.Swamp);
			NewImage = Tile.GetIcon(Tile.TileType.Swamp);
			Min = 50;
			Rollover = "TileSwamp";
			break;
		case Resource.Sand:
			NewTileType = Tile.TileType.Sand;
			Count = GetTileTypeCount(Tile.TileType.Sand);
			NewImage = Tile.GetIcon(Tile.TileType.Sand);
			Min = 50;
			Rollover = "Sand";
			break;
		case Resource.Trees:
			if (SceneManager.GetActiveScene().name == "Main" || SceneManager.GetActiveScene().name == "Playback")
			{
				NewTileType = Tile.TileType.Trees;
				Count = GetObjectTypeCount(ObjectType.TreePine);
				Min = 100;
			}
			else
			{
				NewTileType = Tile.TileType.Trees;
				Count = GetTileTypeCount(Tile.TileType.Trees);
				Min = 600;
			}
			NewImage = IconManager.Instance.GetIcon(ObjectType.TreePine);
			Rollover = "TreePine";
			break;
		case Resource.CropWheat:
			NewTileType = Tile.TileType.CropWheat;
			Count = GetObjectTypeCount(ObjectType.CropWheat) + GetTileTypeCount(Tile.TileType.CropWheat);
			NewImage = IconManager.Instance.GetIcon(ObjectType.Wheat);
			Min = 80;
			Rollover = "CropWheat";
			break;
		case Resource.CropCotton:
			NewTileType = Tile.TileType.CropCotton;
			Count = GetObjectTypeCount(ObjectType.CropCotton) + GetTileTypeCount(Tile.TileType.CropCotton);
			NewImage = IconManager.Instance.GetIcon(ObjectType.CottonBall);
			Min = 80;
			Rollover = "CropCotton";
			break;
		}
	}

	private static int GetTileTypeCount(Tile.TileType NewType1, Tile.TileType NewType2 = Tile.TileType.Total, Tile.TileType NewType3 = Tile.TileType.Total, Tile.TileType NewType4 = Tile.TileType.Total, Tile.TileType NewType5 = Tile.TileType.Total, Tile.TileType NewType6 = Tile.TileType.Total)
	{
		if (m_MapTileData == null)
		{
			return 0;
		}
		int num = 0;
		Tile.TileType[] mapTileData = m_MapTileData;
		foreach (Tile.TileType tileType in mapTileData)
		{
			if (tileType == NewType1 || tileType == NewType2 || tileType == NewType3 || tileType == NewType4 || tileType == NewType5 || tileType == NewType6)
			{
				num++;
			}
		}
		return num;
	}

	private static int GetObjectTypeCount(ObjectType NewType)
	{
		return ObjectTypeList.m_ObjectTypeCounts[(int)NewType];
	}
}
