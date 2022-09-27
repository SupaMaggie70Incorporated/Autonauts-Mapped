using System;
using System.Collections;
using System.IO;
using Dummiesman;
using UnityEngine;
using UnityEngine.Networking;

public class ModLoaderManager : MonoBehaviour
{
	private bool DebugInfo;

	public static ModLoaderManager Instance;

	private string StreamAssetsFolder;

	public int TotalModItems;

	public int TotalModItemsLoaded;

	public bool DiscoveredAllMods;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
		else if (this != Instance)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		StreamAssetsFolder = Application.streamingAssetsPath;
		TotalModItems = 0;
		TotalModItemsLoaded = 0;
		DiscoveredAllMods = false;
	}

	public IEnumerator NowStart()
	{
		FindMods();
		yield return true;
	}

	public bool IsFullyLoaded()
	{
		if (DiscoveredAllMods)
		{
			return TotalModItems == TotalModItemsLoaded;
		}
		return false;
	}

	private void FindMods()
	{
		if (DebugInfo)
		{
			Debug.Log("Searching for Mods");
		}
		if (!Directory.Exists(Path.Combine(StreamAssetsFolder, "Mods")))
		{
			Directory.CreateDirectory(Path.Combine(StreamAssetsFolder, "Mods"));
		}
		string[] directories = Directory.GetDirectories(Path.Combine(StreamAssetsFolder, "Mods"));
		string text = PlayerPrefs.GetString("Steam");
		if (!Directory.Exists(text))
		{
			PlayerPrefs.SetString("Steam", "");
			text = "";
		}
		string[] array = ((text.Length > 0) ? Directory.GetDirectories(text) : null);
		string[] array2 = new string[directories.Length + ((array != null) ? array.Length : 0)];
		directories.CopyTo(array2, 0);
		array?.CopyTo(array2, directories.Length);
		if (array2.Length != 0)
		{
			Array.Sort(array2);
		}
		for (int i = 0; i < array2.Length; i++)
		{
			bool flag = i < directories.Length;
			string text2 = (flag ? array2[i].Replace(Path.Combine(StreamAssetsFolder, "Mods\\"), "") : array2[i].Replace(text, "").TrimStart('\\'));
			if (text2.Equals("TypesOutput"))
			{
				continue;
			}
			if (DebugInfo)
			{
				Debug.Log("Found Mod - " + text2);
			}
			string text3 = Path.Combine(array2[i], "Sounds");
			string text4 = Path.Combine(array2[i], "Models");
			string text5 = Path.Combine(array2[i], "Textures");
			int num = 0;
			if (Directory.Exists(text3))
			{
				string[] files = Directory.GetFiles(text3, "*.wav", SearchOption.AllDirectories);
				num += files.Length;
			}
			if (Directory.Exists(text5))
			{
				string[] files2 = Directory.GetFiles(text5, "*.png", SearchOption.AllDirectories);
				num += files2.Length;
			}
			if (Directory.Exists(text5))
			{
				string[] files3 = Directory.GetFiles(text5, "*.jpg", SearchOption.AllDirectories);
				num += files3.Length;
			}
			string[] files4;
			if (Directory.Exists(text4))
			{
				files4 = Directory.GetFiles(text4, "*.obj", SearchOption.AllDirectories);
				num += files4.Length;
			}
			TotalModItems += num;
			Mod mod = new Mod(text2, num, array2[i], flag);
			ModManager.Instance.RegisterNewMod(mod);
			mod.AddScripts();
			string[] files5 = Directory.GetFiles(array2[i], "steamModID", SearchOption.TopDirectoryOnly);
			mod.SetLoadedPublishedID(files5);
			if (Directory.Exists(text3))
			{
				string[] files = Directory.GetFiles(text3, "*.wav", SearchOption.AllDirectories);
				if (files != null && files.Length != 0)
				{
					for (int j = 0; j < files.Length; j++)
					{
						string text6 = files[j].Replace(text3 + "\\", "").Replace(".wav", "").ToLower();
						if (DebugInfo)
						{
							Debug.Log("Discovered Audio File - " + text6);
						}
						StartCoroutine(LoadAudioFromFile(files[j], mod, text6));
					}
				}
			}
			if (Directory.Exists(text5))
			{
				string[] files2 = Directory.GetFiles(text5, "*.png", SearchOption.AllDirectories);
				string[] files3 = Directory.GetFiles(text5, "*.jpg", SearchOption.AllDirectories);
				if (files2 != null && files2.Length != 0)
				{
					for (int k = 0; k < files2.Length; k++)
					{
						string text7 = files2[k].Replace(text5 + "\\", "").Replace(".png", "").ToLower();
						if (DebugInfo)
						{
							Debug.Log("Discovered PNG Texture File - " + text7);
						}
						StartCoroutine(LoadTextureFromFile(files2[k], mod, text7));
					}
				}
				if (files3 != null && files3.Length != 0)
				{
					for (int l = 0; l < files3.Length; l++)
					{
						string text8 = files3[l].Replace(text5 + "\\", "").Replace(".jpg", "").ToLower();
						if (DebugInfo)
						{
							Debug.Log("Discovered JPG Texture File - " + text8);
						}
						StartCoroutine(LoadTextureFromFile(files3[l], mod, text8));
					}
				}
			}
			if (!Directory.Exists(text4))
			{
				continue;
			}
			files4 = Directory.GetFiles(text4, "*.obj", SearchOption.AllDirectories);
			if (files4 == null || files4.Length == 0)
			{
				continue;
			}
			for (int m = 0; m < files4.Length; m++)
			{
				string text9 = files4[m].Replace(text4 + "\\", "").Replace(".obj", "").ToLower();
				if (DebugInfo)
				{
					Debug.Log("Discovered Model File - " + text9);
				}
				LoadModelFromFile(files4[m], mod, text9);
			}
		}
		DiscoveredAllMods = true;
		ModManager.Instance.LoadInitialSpawnsInfo();
		ModManager.Instance.CheckAllModScriptsForClash();
	}

	private IEnumerator LoadAudioFromFile(string FileLoc, Mod Owner, string Name)
	{
		using UnityWebRequest UWRLoader = UnityWebRequestMultimedia.GetAudioClip("file://" + FileLoc, AudioType.WAV);
		yield return UWRLoader.SendWebRequest();
		if (string.IsNullOrEmpty(UWRLoader.error))
		{
			if (DebugInfo)
			{
				Debug.Log("LOADED AUDIO: " + FileLoc);
			}
			AudioClip content = DownloadHandlerAudioClip.GetContent(UWRLoader);
			content.name = Name;
			Owner.AddSound(content);
			TotalModItemsLoaded++;
		}
		else
		{
			string descriptionOverride = "Error: Loading Audio File Failed '" + Name + "' - " + UWRLoader.error;
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
		}
	}

	private IEnumerator LoadTextureFromFile(string FileLoc, Mod Owner, string Name)
	{
		using UnityWebRequest UWRLoader = UnityWebRequestTexture.GetTexture("file://" + FileLoc);
		yield return UWRLoader.SendWebRequest();
		if (string.IsNullOrEmpty(UWRLoader.error))
		{
			if (DebugInfo)
			{
				Debug.Log("LOADED TEXTURE: " + FileLoc);
			}
			Texture content = DownloadHandlerTexture.GetContent(UWRLoader);
			content.name = Name;
			Owner.AddTexture(content);
			TotalModItemsLoaded++;
		}
		else
		{
			string descriptionOverride = "Error: Loading Texture File Failed '" + Name + "' - " + UWRLoader.error;
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
		}
	}

	private void LoadModelFromFile(string FileLoc, Mod Owner, string Name)
	{
		GameObject gameObject = null;
		if (File.Exists(FileLoc))
		{
			gameObject = new OBJLoader().Load(FileLoc, FileLoc.Replace(".obj", ".mtl"));
			gameObject.transform.SetParent(base.transform);
			gameObject.gameObject.SetActive(value: false);
			if (DebugInfo)
			{
				Debug.Log("LOADED MODEL: " + FileLoc);
			}
			gameObject.name = Name;
			Owner.AddModel(gameObject, FileLoc, Name);
			TotalModItemsLoaded++;
		}
	}
}
