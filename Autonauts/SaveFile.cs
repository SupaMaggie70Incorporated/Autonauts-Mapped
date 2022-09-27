using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveFile
{
	public string m_Name;

	public bool m_PreviewLoaded;

	public SaveFileThumbnail m_Thumbnail;

	public SaveFileSummary m_Summary;

	private SaveFileWorldData m_World;

	public bool m_Loading;

	private bool m_Autosave;

	private static bool m_DiskFull;

	public static string GetBasePath(bool Autosave)
	{
		if (Autosave)
		{
			return Application.persistentDataPath + "/Autosaves/";
		}
		return Application.persistentDataPath + "/Saves/";
	}

	private static int SortFilesByDate(SafeFileDateTime p1, SafeFileDateTime p2)
	{
		DateTime dateTime = p1.m_DateTime;
		DateTime dateTime2 = p2.m_DateTime;
		if (dateTime < dateTime2)
		{
			return 1;
		}
		if (dateTime > dateTime2)
		{
			return -1;
		}
		return 0;
	}

	public static bool IsDiskFull(Exception ex)
	{
		int num;
		if (ex.HResult != -2147024857)
		{
			num = ((ex.HResult == -2147024784) ? 1 : 0);
			if (num == 0)
			{
				goto IL_0026;
			}
		}
		else
		{
			num = 1;
		}
		m_DiskFull = true;
		goto IL_0026;
		IL_0026:
		return (byte)num != 0;
	}

	public static bool CheckDiskFull()
	{
		if (m_DiskFull)
		{
			m_DiskFull = false;
			return true;
		}
		return false;
	}

	public static void Save(string FileName, string FinalString)
	{
		try
		{
			File.WriteAllText(FileName, FinalString);
		}
		catch (UnauthorizedAccessException ex)
		{
			Debug.Log("UnauthorizedAccessException : " + FileName + " " + ex.ToString());
		}
		catch (IOException ex2)
		{
			if (!IsDiskFull(ex2))
			{
				ErrorMessage.LogError("Save - IOException : " + FileName + " " + ex2.ToString());
			}
		}
	}

	public static void Save(string FileName, byte[] Bytes)
	{
		try
		{
			File.WriteAllBytes(FileName, Bytes);
		}
		catch (UnauthorizedAccessException ex)
		{
			ErrorMessage.LogError("UnauthorizedAccessException : " + FileName + " " + ex.ToString());
		}
		catch (IOException ex2)
		{
			if (!IsDiskFull(ex2))
			{
				ErrorMessage.LogError("Save - IOException : " + FileName + " " + ex2.ToString());
			}
		}
	}

	public static List<string> GetAllSaveNames(bool Autosaves, bool Recordings)
	{
		List<SafeFileDateTime> list = new List<SafeFileDateTime>();
		string basePath = GetBasePath(Autosaves);
		if (!Directory.Exists(basePath))
		{
			Directory.CreateDirectory(basePath);
		}
		string[] directories = Directory.GetDirectories(basePath);
		foreach (string text in directories)
		{
			bool flag = false;
			if (!Recordings && SaveFileWorldData.Exists(text))
			{
				flag = true;
			}
			if (Recordings && RecordingManager.Exists(text))
			{
				flag = true;
			}
			if (flag)
			{
				string[] array = text.Split('/');
				string name = array[array.Length - 1];
				list.Add(new SafeFileDateTime(name, SaveFileWorldData.GetSaveDateTime(text)));
			}
		}
		list.Sort(SortFilesByDate);
		List<string> list2 = new List<string>();
		foreach (SafeFileDateTime item in list)
		{
			list2.Add(item.m_Name);
		}
		return list2;
	}

	private void CheckAutosave()
	{
		m_Autosave = false;
		if (m_Name.Contains(SaveLoadManager.m_AutosaveName))
		{
			m_Autosave = true;
		}
	}

	public bool Save(string NewName)
	{
		m_Name = NewName;
		CheckAutosave();
		string text = GetBasePath(m_Autosave) + NewName;
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		new SaveFileThumbnail().Save(text);
		new SaveFileSummary().Save(text);
		new SaveFileWorldData().Save(text);
		if (GameOptionsManager.Instance.m_Options.m_RecordingEnabled)
		{
			RecordingManager.Instance.Save(NewName);
		}
		if (m_DiskFull)
		{
			return false;
		}
		return true;
	}

	private void SetupSummary()
	{
		m_Summary = new SaveFileSummary();
		m_Summary.Capture();
		m_PreviewLoaded = false;
	}

	public void LoadPreview(string NewName)
	{
		m_Name = NewName;
		CheckAutosave();
		if (m_Name == "")
		{
			SetupSummary();
			return;
		}
		string newPath = GetBasePath(m_Autosave) + NewName;
		m_Thumbnail = new SaveFileThumbnail();
		if (m_Thumbnail.Exists(newPath))
		{
			m_Thumbnail.Load(newPath);
		}
		m_Summary = new SaveFileSummary();
		if (m_Summary.Exists(newPath))
		{
			m_Summary.Load(newPath);
		}
		m_PreviewLoaded = true;
	}

	public void LoadStart(string NewName)
	{
		m_Name = NewName;
		CheckAutosave();
		string newPath = GetBasePath(m_Autosave) + NewName;
		m_World = new SaveFileWorldData();
		m_World.StartLoad(newPath);
		m_Loading = true;
	}

	public bool Delete(string NewName)
	{
		m_Name = NewName;
		CheckAutosave();
		string text = GetBasePath(m_Autosave) + NewName;
		if (Directory.Exists(text))
		{
			Directory.Delete(text, recursive: true);
			text = text.Replace(Application.persistentDataPath + "/", Application.persistentDataPath + "/Mod");
			if (Directory.Exists(text))
			{
				Directory.Delete(text, recursive: true);
			}
			return true;
		}
		return false;
	}

	public bool Exists(string NewName)
	{
		m_Name = NewName;
		CheckAutosave();
		return SaveFileWorldData.Exists(GetBasePath(m_Autosave) + NewName);
	}

	public float GetLoadPercent()
	{
		return LoadJSON.GetLoadPercent();
	}

	public void Update()
	{
		if (m_Loading)
		{
			m_World.Update();
			if (!LoadJSON.m_Loading)
			{
				m_Loading = false;
			}
		}
	}
}
