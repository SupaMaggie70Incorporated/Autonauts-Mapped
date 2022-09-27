using System;
using System.IO;
using SimpleJSON;

public class SaveFileWorldData
{
	private static string m_DefaultName = "World.txt";

	private static string m_ValidString = "AutonautsWorld";

	private static string GetFileName(string NewPath)
	{
		return NewPath + "/" + m_DefaultName;
	}

	public void Save(string NewPath)
	{
		JSONObject jSONObject = new JSONObject();
		JSONUtils.Set(jSONObject, m_ValidString, 1);
		SaveJSON.Capture(jSONObject);
		string finalString = jSONObject.ToString();
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

	public void StartLoad(string NewPath)
	{
		string fileName = GetFileName(NewPath);
		if (!File.Exists(fileName))
		{
			ErrorMessage.LogError("Summary Load - File doesn't exist : " + fileName);
		}
		else
		{
			LoadJSON.StartLoad(fileName);
		}
	}

	public static bool Exists(string NewPath)
	{
		if (!File.Exists(GetFileName(NewPath)))
		{
			return false;
		}
		return true;
	}

	public static DateTime GetSaveDateTime(string NewPath)
	{
		return File.GetLastWriteTime(NewPath + "/World.txt");
	}

	public void Update()
	{
		if (LoadJSON.m_Loading)
		{
			LoadJSON.Update();
		}
	}
}
