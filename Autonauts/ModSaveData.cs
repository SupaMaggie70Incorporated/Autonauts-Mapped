using System;
using System.IO;
using MoonSharp.Interpreter;
using SimpleJSON;
using UnityEngine;

[MoonSharpUserData]
public class ModSaveData
{
	private JSONNode GetLoadData(string SaveFileName)
	{
		if (!File.Exists(SaveFileName))
		{
			return null;
		}
		string aJSON;
		try
		{
			aJSON = File.ReadAllText(SaveFileName);
		}
		catch (UnauthorizedAccessException ex)
		{
			ErrorMessage.LogError("ModSaveData.GetLoadData - UnauthorizedAccessException : " + SaveFileName + " " + ex.ToString());
			return null;
		}
		JSONNode jSONNode = JSON.Parse(aJSON);
		if (jSONNode == null)
		{
			return null;
		}
		return jSONNode;
	}

	private string GetModSaveFilename(bool CreatePath = true)
	{
		string text = "";
		string text2 = SaveLoadManager.Instance.m_ModSaveDirectory;
		if (!CreatePath && text2 == "")
		{
			text2 = SessionManager.Instance.m_LoadFileName;
		}
		Mod lastCalledMod = ModManager.Instance.GetLastCalledMod();
		if (lastCalledMod == null || string.IsNullOrEmpty(text2))
		{
			return "";
		}
		try
		{
			text = (text2.StartsWith(SaveLoadManager.m_AutosaveName) ? Path.Combine(Application.persistentDataPath, "ModAutosaves") : Path.Combine(Application.persistentDataPath, "ModSaves"));
			if (CreatePath && !Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			text = Path.Combine(text, text2);
			if (CreatePath && !Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return Path.Combine(text, lastCalledMod.Name.Trim('\\') + ".txt");
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModSaveData.GetModSaveFilename Error: " + ex.ToString());
		}
		return "";
	}

	public bool SaveValue(string Name, string Variable)
	{
		try
		{
			if (Name == null || Variable == null)
			{
				return false;
			}
			string modSaveFilename = GetModSaveFilename();
			if (modSaveFilename == "")
			{
				return false;
			}
			JSONNode jSONNode = GetLoadData(modSaveFilename);
			if (jSONNode == null)
			{
				jSONNode = new JSONObject();
			}
			JSONUtils.Set(jSONNode, Name, Variable);
			string contents = jSONNode.ToString();
			try
			{
				File.WriteAllText(modSaveFilename, contents);
			}
			catch (UnauthorizedAccessException ex)
			{
				ErrorMessage.LogError("ModSaveData.SaveValue - UnauthorizedAccessException : " + modSaveFilename + " " + ex.ToString());
				return false;
			}
			return true;
		}
		catch (Exception ex2)
		{
			ModManager.Instance.WriteModError("ModSaveData.SaveValue Error: " + ex2.ToString());
		}
		return false;
	}

	public string LoadValue(string Name, string Default)
	{
		try
		{
			if (Name == null)
			{
				return Default;
			}
			string modSaveFilename = GetModSaveFilename(CreatePath: false);
			if (modSaveFilename == "")
			{
				return Default;
			}
			JSONNode loadData = GetLoadData(modSaveFilename);
			if (loadData == null)
			{
				return Default;
			}
			return JSONUtils.GetAsString(loadData, Name, Default);
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModSaveData.LoadValue Error: " + ex.ToString());
			return Default;
		}
	}

	public bool SaveValueInGroup(string Group, string Name, string Variable)
	{
		try
		{
			if (Group == null || Name == null || Variable == null)
			{
				return false;
			}
			string modSaveFilename = GetModSaveFilename();
			if (modSaveFilename == "")
			{
				return false;
			}
			JSONNode jSONNode = GetLoadData(modSaveFilename);
			if (jSONNode == null)
			{
				jSONNode = new JSONObject();
			}
			JSONNode jSONNode2 = jSONNode[Group];
			if (jSONNode2 == null || jSONNode2.IsNull)
			{
				jSONNode[Group] = new JSONObject();
			}
			JSONUtils.Set(jSONNode[Group], Name, Variable);
			string contents = jSONNode.ToString();
			try
			{
				File.WriteAllText(modSaveFilename, contents);
			}
			catch (UnauthorizedAccessException ex)
			{
				ErrorMessage.LogError("ModSaveData.SaveValueInGroup - UnauthorizedAccessException : " + modSaveFilename + " " + ex.ToString());
				return false;
			}
			return true;
		}
		catch (Exception ex2)
		{
			ModManager.Instance.WriteModError("ModSaveData.SaveValueInGroup Error: " + ex2.ToString());
		}
		return false;
	}

	public string LoadValueInGroup(string Group, string Name, string Default)
	{
		try
		{
			if (Group == null || Name == null)
			{
				return Default;
			}
			string modSaveFilename = GetModSaveFilename(CreatePath: false);
			if (modSaveFilename == "")
			{
				return Default;
			}
			JSONNode loadData = GetLoadData(modSaveFilename);
			if (loadData == null || loadData.IsNull)
			{
				return Default;
			}
			JSONNode jSONNode = loadData[Group];
			if (jSONNode == null || jSONNode.IsNull)
			{
				return Default;
			}
			return JSONUtils.GetAsString(jSONNode, Name, Default);
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModSaveData.LoadValueInGroup Error: " + ex.ToString());
			return Default;
		}
	}
}
