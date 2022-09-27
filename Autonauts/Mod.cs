using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using MoonSharp.Interpreter;
using Rewired;
using Steamworks;
using UnityEngine;

public class Mod
{
	private bool DebugInfo;

	public bool StartedUp;

	public string FolderLocation;

	private List<Texture> ModTextures;

	private List<AudioClip> ModSounds;

	private List<GameObject> ModModels;

	public List<ObjectType> CustomIDs;

	public List<ModManager.ExposedData> ExposedVars;

	public string SteamTitle;

	public string SteamDescription;

	public IList<string> SteamTags;

	public string SteamContentFolder;

	public string SteamContentImage;

	public string SteamImageName;

	public DynValue InputKeyPressCallback;

	public DynValue InputMouseDownCallback;

	public bool IsUsingKeybindings;

	public bool MenuStartedUp { get; private set; }

	public string Name { get; private set; }

	public bool IsLocal { get; private set; }

	public List<Script> LuaScripts { get; private set; }

	public int ItemsToLoad { get; private set; }

	public bool IsEnabled { get; private set; }

	public PublishedFileId_t m_PublishedFileId { get; private set; }

	public Mod(string ModName, int ItemsInMod, string Folder, bool LocalMod)
	{
		if (DebugInfo)
		{
			Debug.Log("MOD CREATED - " + ModName + " - ITEMS: " + ItemsInMod);
		}
		StartedUp = false;
		Name = ModName;
		FolderLocation = Folder;
		ItemsToLoad = ItemsInMod;
		IsLocal = LocalMod;
		MenuStartedUp = false;
		LuaScripts = new List<Script>();
		ModTextures = new List<Texture>();
		ModSounds = new List<AudioClip>();
		ModModels = new List<GameObject>();
		CustomIDs = new List<ObjectType>();
		ExposedVars = new List<ModManager.ExposedData>();
		IsEnabled = true;
		InputKeyPressCallback = DynValue.NewNil();
		InputMouseDownCallback = DynValue.NewNil();
		ModManager.Instance.ClearLog(Name + "_DebugLog");
		ModManager.Instance.ClearLog(Name + "_ErrorLog");
	}

	public void AddSound(AudioClip Sound)
	{
		ModSounds.Add(Sound);
		ItemsToLoad--;
	}

	public AudioClip GetSound(string FileName)
	{
		for (int i = 0; i < ModSounds.Count; i++)
		{
			if (ModSounds[i].name == FileName)
			{
				return ModSounds[i];
			}
		}
		return null;
	}

	public void AddTexture(Texture Tex)
	{
		ModTextures.Add(Tex);
		ItemsToLoad--;
	}

	public Texture GetTexture(string FileName)
	{
		for (int i = 0; i < ModTextures.Count; i++)
		{
			if (ModTextures[i].name == FileName)
			{
				return ModTextures[i];
			}
		}
		return null;
	}

	public List<Texture> GetAllTextures()
	{
		return ModTextures;
	}

	public void AddModel(GameObject Model, string FileLoc, string Name)
	{
		ModModels.Add(Model);
		ItemsToLoad--;
	}

	private void InterpretLuaScript(string ScriptFilename)
	{
		string code = File.ReadAllText(ScriptFilename);
		string fileName = Path.GetFileName(ScriptFilename);
		Script script = new Script(CoreModules.Preset_SoftSandbox);
		if (script != null)
		{
			try
			{
				RegisterScriptGlobals(script);
				script.DoString(code);
			}
			catch (ScriptRuntimeException ex)
			{
				string descriptionOverride = "Function: " + Name + "\nError" + ScriptErrorMessage(ex);
				ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Lua, descriptionOverride);
			}
		}
		script.Globals["_ModDisplayName"] = DynValue.NewString(Name);
		script.Globals["_ModScriptFile"] = DynValue.NewString(fileName);
		LuaScripts.Add(script);
	}

	public bool HasDevScripts()
	{
		return Directory.GetDirectories(FolderLocation, "*.dev", SearchOption.TopDirectoryOnly).Length != 0;
	}

	private void AssembleDevScripts()
	{
		string[] directories = Directory.GetDirectories(FolderLocation, "*.dev", SearchOption.TopDirectoryOnly);
		Array.Sort(directories);
		string[] array = directories;
		foreach (string text in array)
		{
			try
			{
				string text2 = text.Substring(0, text.Length - 3) + "lua";
				bool flag = false;
				if (!File.Exists(text2))
				{
					flag = true;
				}
				else
				{
					FileInfo[] files = new DirectoryInfo(text).GetFiles();
					DateTime dateTime = DateTime.MinValue;
					FileInfo[] array2 = files;
					foreach (FileInfo fileInfo in array2)
					{
						if (fileInfo.LastWriteTime > dateTime)
						{
							dateTime = fileInfo.LastWriteTime;
						}
					}
					if (new FileInfo(text2).LastWriteTime < dateTime)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					continue;
				}
				string text3 = "";
				string[] files2 = Directory.GetFiles(text, "*.lua", SearchOption.TopDirectoryOnly);
				Array.Sort(files2);
				string[] array3 = files2;
				foreach (string path in array3)
				{
					text3 = text3 + File.ReadAllText(path) + "\n";
				}
				if (!(text3 != ""))
				{
					continue;
				}
				if (File.Exists(text2))
				{
					if (!Directory.Exists(text + "/Backups"))
					{
						Directory.CreateDirectory(text + "/Backups");
					}
					string destFileName = text + "/Backups/" + Name + DateTime.Now.ToString(".yyyy-MM-dd_HH-mm-ss") + ".bak";
					File.Move(text2, destFileName);
				}
				File.WriteAllText(text2, text3);
			}
			catch (Exception ex)
			{
				ModManager.Instance.WriteModError("Mod.AssembleDevScripts Error: " + ex.ToString());
			}
		}
	}

	public void AddScripts()
	{
		if (ModManager.Instance.ModDebugClass.IsDevMode())
		{
			AssembleDevScripts();
		}
		string[] files = Directory.GetFiles(FolderLocation, "*.lua", SearchOption.TopDirectoryOnly);
		Array.Sort(files);
		ModManager.Instance.AddModScripts(files);
		string[] array = files;
		foreach (string scriptFilename in array)
		{
			InterpretLuaScript(scriptFilename);
		}
	}

	public void AfterSave()
	{
		if (!GeneralUtils.m_InGame || !IsEnabled)
		{
			return;
		}
		for (int i = 0; i < LuaScripts.Count; i++)
		{
			if (LuaScripts[i].Globals["AfterSave"] != null)
			{
				CallFunction(LuaScripts[i], "AfterSave");
			}
		}
	}

	private void RegisterScriptGlobals(Script NewScript)
	{
		if (NewScript.Globals != null)
		{
			NewScript.Globals["ModSound"] = ModManager.Instance.ModSoundClass;
			NewScript.Globals["ModVariable"] = ModManager.Instance.ModVariableClass;
			NewScript.Globals["ModBase"] = ModManager.Instance.ModBaseClass;
			NewScript.Globals["ModConverter"] = ModManager.Instance.ModConverterClass;
			NewScript.Globals["ModBuilding"] = ModManager.Instance.ModBuildingClass;
			NewScript.Globals["ModDecorative"] = ModManager.Instance.ModDecorativeClass;
			NewScript.Globals["ModTiles"] = ModManager.Instance.ModTilesClass;
			NewScript.Globals["ModPlayer"] = ModManager.Instance.ModPlayerClass;
			NewScript.Globals["ModBot"] = ModManager.Instance.ModBotClass;
			NewScript.Globals["ModDebug"] = ModManager.Instance.ModDebugClass;
			NewScript.Globals["ModHoldable"] = ModManager.Instance.ModHoldableClass;
			NewScript.Globals["ModFood"] = ModManager.Instance.ModFoodClass;
			NewScript.Globals["ModObject"] = ModManager.Instance.ModObjectClass;
			NewScript.Globals["ModTool"] = ModManager.Instance.ModToolClass;
			NewScript.Globals["ModCamera"] = ModManager.Instance.ModCameraClass;
			NewScript.Globals["ModQuest"] = ModManager.Instance.ModQuestClass;
			NewScript.Globals["ModGoTo"] = ModManager.Instance.ModGoToClass;
			NewScript.Globals["ModHat"] = ModManager.Instance.ModHatClass;
			NewScript.Globals["ModTop"] = ModManager.Instance.ModTopClass;
			NewScript.Globals["ModUI"] = ModManager.Instance.ModUIClass;
			NewScript.Globals["ModStorage"] = ModManager.Instance.ModStorageClass;
			NewScript.Globals["ModMedicine"] = ModManager.Instance.ModMedicineClass;
			NewScript.Globals["ModToy"] = ModManager.Instance.ModToyClass;
			NewScript.Globals["ModEducation"] = ModManager.Instance.ModEducationClass;
			NewScript.Globals["ModSaveData"] = ModManager.Instance.ModSaveDataClass;
			NewScript.Globals["ModTimer"] = ModManager.Instance.ModTimerClass;
			NewScript.Globals["ModText"] = ModManager.Instance.ModTextClass;
		}
	}

	public void CallFunction(Script CurrScript, string FuncName, params DynValue[] args)
	{
		if ((GeneralUtils.m_InGame && (FuncName.Equals("OnUpdate") || FuncName.Equals("BeforeLoad") || FuncName.Equals("AfterSave") || FuncName.Equals("AfterLoad") || FuncName.Equals("AfterLoad_CreatedWorld") || FuncName.Equals("AfterLoad_LoadedWorld") || FuncName.Equals("Creation"))) || (!GeneralUtils.m_InGame && (FuncName.Equals("SteamDetails") || FuncName.Equals("Creation") || FuncName.Equals("AfterLoad_CreatedWorld") || FuncName.Equals("Expose"))))
		{
			if (DebugInfo)
			{
				Debug.Log(string.Concat("SCRIPT ", CurrScript, " FUNC: ", FuncName, " ARGS ", args));
			}
			try
			{
				CurrScript.Call(CurrScript.Globals[FuncName], args);
			}
			catch (ScriptRuntimeException ex)
			{
				string text = (IsLocal ? Name : (Name + "\\" + SteamTitle));
				string text2 = CurrScript.Globals["_ModScriptFile"].ToString();
				string descriptionOverride = text + "\nFunction: " + FuncName + "\n" + text2 + ScriptErrorMessage(ex);
				ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Lua, descriptionOverride);
			}
			catch (Exception ex2)
			{
				ModManager.Instance.WriteModError(FuncName + " Error: " + ex2.ToString());
			}
		}
	}

	public static string ScriptErrorMessage(ScriptRuntimeException ex)
	{
		return Regex.Replace(ex.DecoratedMessage, "^.+?:\\((\\d+),", " (line $1, col ");
	}

	private void MenuStart(bool CallCreation = true)
	{
		for (int i = 0; i < LuaScripts.Count; i++)
		{
			if (LuaScripts[i].Globals["SteamDetails"] != null)
			{
				CallFunction(LuaScripts[i], "SteamDetails");
			}
		}
		for (int j = 0; j < LuaScripts.Count; j++)
		{
			if (LuaScripts[j].Globals["Expose"] != null)
			{
				CallFunction(LuaScripts[j], "Expose");
			}
		}
		if (CallCreation)
		{
			for (int k = 0; k < LuaScripts.Count; k++)
			{
				ModManager.Instance.CreationScript = LuaScripts[k];
				if (LuaScripts[k].Globals["Creation"] != null)
				{
					CallFunction(LuaScripts[k], "Creation");
				}
			}
		}
		MenuStartedUp = true;
	}

	private void GameStart()
	{
		for (int i = 0; i < LuaScripts.Count; i++)
		{
			if (LuaScripts[i].Globals["Creation"] != null)
			{
				CallFunction(LuaScripts[i], "Creation");
			}
		}
		for (int j = 0; j < LuaScripts.Count; j++)
		{
			if (LuaScripts[j].Globals["BeforeLoad"] != null)
			{
				CallFunction(LuaScripts[j], "BeforeLoad");
			}
		}
		foreach (ObjectType customID in CustomIDs)
		{
			ObjectTypeList.Instance.UpdateBootVars(customID);
		}
	}

	public void PostCreate()
	{
		for (int i = 0; i < LuaScripts.Count; i++)
		{
			if (LuaScripts[i].Globals["AfterLoad"] != null)
			{
				CallFunction(LuaScripts[i], "AfterLoad");
			}
		}
	}

	public void PostCreateSpecific(bool Created)
	{
		if (!IsEnabled)
		{
			return;
		}
		for (int i = 0; i < LuaScripts.Count; i++)
		{
			if (Created)
			{
				if (LuaScripts[i].Globals["AfterLoad_CreatedWorld"] != null)
				{
					CallFunction(LuaScripts[i], "AfterLoad_CreatedWorld");
				}
			}
			else if (LuaScripts[i].Globals["AfterLoad_LoadedWorld"] != null)
			{
				CallFunction(LuaScripts[i], "AfterLoad_LoadedWorld");
			}
		}
		PostCreate();
	}

	public void SetupInitialMapData()
	{
		if (!IsEnabled)
		{
			return;
		}
		for (int i = 0; i < LuaScripts.Count; i++)
		{
			if (LuaScripts[i].Globals["AfterLoad_CreatedWorld"] != null)
			{
				CallFunction(LuaScripts[i], "AfterLoad_CreatedWorld");
			}
		}
	}

	public void Update()
	{
		if (!GeneralUtils.m_InGame)
		{
			if (ItemsToLoad == 0 && !MenuStartedUp)
			{
				AddExposedVariable("ModsEnabledTitle", DynValue.NewBoolean(v: true), null, null, null, UsingLookup: true);
				MenuStart();
				LoadExposedVariables();
			}
		}
		else
		{
			if (!IsEnabled)
			{
				return;
			}
			if (!StartedUp && ItemsToLoad == 0)
			{
				GameStart();
				StartedUp = true;
				return;
			}
			for (int i = 0; i < LuaScripts.Count; i++)
			{
				if (LuaScripts[i].Globals["OnUpdate"] != null)
				{
					CallFunction(LuaScripts[i], "OnUpdate", DynValue.NewNumber(Time.deltaTime));
				}
			}
		}
	}

	public void SetSteamWorkshopDetails(string Title, string Description, IList<string> Tags, string ContentImage)
	{
		SteamTitle = Title;
		SteamDescription = Description;
		SteamTags = Tags;
		SteamContentFolder = FolderLocation;
		SteamContentImage = FolderLocation + "/textures/" + ContentImage;
		SteamImageName = ContentImage.Replace("\\", "").Replace(".png", "").Replace(".jpg", "")
			.Replace(".jpeg", "")
			.ToLower();
	}

	public void UploadToSteamWorkshop()
	{
		SteamWorkshopManager.Instance.CreateWorkshopItem(this);
	}

	public void SetPublishedFieldID(PublishedFileId_t NewPublishedFileId)
	{
		m_PublishedFileId = NewPublishedFileId;
		string contents = NewPublishedFileId.ToString();
		string text = FolderLocation + "\\steamModID";
		try
		{
			File.WriteAllText(text, contents);
		}
		catch (UnauthorizedAccessException ex)
		{
			ErrorMessage.LogError("Summary Save - UnauthorizedAccessException : " + text + " " + ex.ToString());
		}
	}

	public void SetLoadedPublishedID(string[] IDs)
	{
		for (int i = 0; i < IDs.Length; i++)
		{
			string text = File.ReadAllText(IDs[i]);
			if (text.Length > 0)
			{
				ulong value = Convert.ToUInt64(text);
				m_PublishedFileId = new PublishedFileId_t(value);
				if (DebugInfo)
				{
					Debug.Log("LOADED ID " + m_PublishedFileId);
				}
			}
		}
	}

	public GameObject GetCustomModel(string Name)
	{
		foreach (GameObject modModel in ModModels)
		{
			if (modModel.name == Name)
			{
				return modModel;
			}
		}
		return null;
	}

	public void AddExposedVariable(string UniqueName, DynValue DefaultValue, DynValue Callback, DynValue Min = null, DynValue Max = null, bool UsingLookup = false)
	{
		foreach (ModManager.ExposedData exposedVar in ExposedVars)
		{
			if (exposedVar.VarName.Equals(UniqueName))
			{
				return;
			}
		}
		if (Callback != null && Callback.Type != DataType.Function)
		{
			string descriptionOverride = "Error: ModBase.ExposeVariable - Callback is not a function";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
			return;
		}
		ModManager.ExposedData item = default(ModManager.ExposedData);
		item.VarName = UniqueName;
		item.VarType = DefaultValue.Type;
		item.VarValue = DefaultValue;
		item.Callback = Callback;
		item.MinValue = Min;
		item.MaxValue = Max;
		item.UsesLookup = UsingLookup;
		item.IsKeybinding = false;
		if (item.VarType == DataType.Number && Min != null && Max != null && Min.Type == DataType.Number && Max.Type == DataType.Number && (DefaultValue.Number < Min.Number || DefaultValue.Number > Max.Number))
		{
			string descriptionOverride2 = "Error: ModBase.ExposeVariable '" + UniqueName + "' - Value is outside of Min/Max Range";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride2);
			return;
		}
		if (item.VarType == DataType.Number && Min == null)
		{
			item.MinValue = DynValue.NewNumber(0.0);
		}
		if (item.VarType == DataType.Number && Max == null)
		{
			item.MaxValue = DynValue.NewNumber(100.0);
		}
		ExposedVars.Add(item);
	}

	public void AddExposedVariableList(string UniqueName, DynValue[] DefaultOptions, int SelectedOption, DynValue Callback)
	{
		foreach (ModManager.ExposedData exposedVar in ExposedVars)
		{
			if (exposedVar.VarName.Equals(UniqueName))
			{
				return;
			}
		}
		if (Callback != null && Callback.Type != DataType.Function)
		{
			string descriptionOverride = "Error: ModBase.ExposeVariable - Callback is not a function";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
			return;
		}
		ModManager.ExposedData item = default(ModManager.ExposedData);
		item.VarName = UniqueName;
		item.VarType = DataType.Table;
		item.VarValue = DynValue.NewNumber(SelectedOption);
		item.Callback = Callback;
		item.UsesLookup = false;
		item.IsKeybinding = false;
		item.VarValuesList = new List<DynValue>();
		foreach (DynValue item2 in DefaultOptions)
		{
			item.VarValuesList.Add(item2);
		}
		ExposedVars.Add(item);
	}

	public void AddExposedKeybinding(string UniqueName, int Key, DynValue Callback)
	{
		foreach (ModManager.ExposedData exposedVar in ExposedVars)
		{
			if (exposedVar.VarName.Equals(UniqueName))
			{
				return;
			}
		}
		if (Callback != null && Callback.Type != DataType.Function)
		{
			string descriptionOverride = "Error: ModBase.AddExposedKeybinding - Callback is not a function";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
			return;
		}
		ModManager.ExposedData item = default(ModManager.ExposedData);
		item.VarName = UniqueName;
		item.VarType = DataType.Nil;
		item.VarValue = DynValue.NewNumber(Key);
		item.Callback = Callback;
		item.MinValue = DynValue.NewNil();
		item.MaxValue = DynValue.NewNil();
		item.UsesLookup = false;
		item.IsKeybinding = true;
		ExposedVars.Add(item);
		IsUsingKeybindings = true;
	}

	public void RegisterForInputPress(DynValue Callback)
	{
		InputKeyPressCallback = Callback;
		ModManager.Instance.RegisteredForInputPressCallback = true;
		foreach (Script luaScript in LuaScripts)
		{
			if (luaScript.IsOwnership(luaScript, Callback))
			{
				ModManager.Instance.RegisteredInputModsKeyPress.Add(this, luaScript);
			}
		}
	}

	public void RegisterForInputMouseDown(DynValue Callback)
	{
		InputMouseDownCallback = Callback;
		ModManager.Instance.RegisteredForInputMouseDownCallback = true;
		foreach (Script luaScript in LuaScripts)
		{
			if (luaScript.IsOwnership(luaScript, Callback))
			{
				ModManager.Instance.RegisteredInputModsMouseDown.Add(this, luaScript);
			}
		}
	}

	public void UpdateExposedVariable(string UniqueName, DynValue Value, bool Save = true)
	{
		if (UniqueName.Equals("ModsEnabledTitle"))
		{
			IsEnabled = Value.Boolean;
			foreach (ObjectType customID in CustomIDs)
			{
				foreach (ModCustom modCustomClass in ModManager.Instance.ModCustomClasses)
				{
					if (modCustomClass.IsEnabled.ContainsKey(customID))
					{
						if (!IsEnabled && ObjectTypeList.Instance != null)
						{
							ObjectTypeList.Instance.DisableCustomItem(customID);
						}
						modCustomClass.IsEnabled.Remove(customID);
						modCustomClass.IsEnabled.Add(customID, IsEnabled);
					}
				}
			}
		}
		int count = ExposedVars.Count;
		for (int i = 0; i < count; i++)
		{
			if (!ExposedVars[i].VarName.Equals(UniqueName))
			{
				continue;
			}
			ModManager.ExposedData exposedData = default(ModManager.ExposedData);
			exposedData = ExposedVars[i];
			exposedData.VarValue = Value;
			ExposedVars.RemoveAt(i);
			ExposedVars.Insert(i, exposedData);
			if (exposedData.Callback == null)
			{
				break;
			}
			foreach (Script luaScript in LuaScripts)
			{
				DynValue[] args = new DynValue[2]
				{
					exposedData.VarValue,
					DynValue.NewString(exposedData.VarName)
				};
				if (luaScript.IsOwnership(luaScript, exposedData.Callback))
				{
					ModManager.Instance.Callback(luaScript, exposedData.Callback, args);
				}
			}
			break;
		}
		if (Save)
		{
			SaveExposedVariables();
		}
	}

	public void UpdateKeybindingsCall(InputActionEventData data)
	{
		if (!IsEnabled)
		{
			return;
		}
		foreach (ModManager.ExposedData exposedVar in ExposedVars)
		{
			if (!exposedVar.IsKeybinding || (int)exposedVar.VarValue.Number != data.actionId - 49 + 1 || exposedVar.Callback == null)
			{
				continue;
			}
			foreach (Script luaScript in LuaScripts)
			{
				DynValue[] args = new DynValue[1] { DynValue.NewString(exposedVar.VarName) };
				if (luaScript.IsOwnership(luaScript, exposedVar.Callback))
				{
					ModManager.Instance.Callback(luaScript, exposedVar.Callback, args);
				}
			}
		}
	}

	private void LoadExposedVariables()
	{
		string text = Path.Combine(Application.persistentDataPath, "ModConfig") + "\\" + Name + ".txt";
		if (!File.Exists(text))
		{
			string text2 = Path.Combine(Application.streamingAssetsPath, "Mods") + "\\" + Name + "\\Config.txt";
			if (!IsLocal)
			{
				text2 = FolderLocation + "\\Config.txt";
			}
			if (File.Exists(text2))
			{
				File.Move(text2, text);
			}
		}
		if (!File.Exists(text))
		{
			return;
		}
		StreamReader streamReader = new StreamReader(text);
		string text3;
		while ((text3 = streamReader.ReadLine()) != null)
		{
			int num = text3.IndexOf('=');
			if (num == -1)
			{
				continue;
			}
			string text4 = text3.Substring(0, num);
			string text5 = text3.Substring(num + 1, text3.Length - num - 1);
			if (text4.Equals("Enabled"))
			{
				UpdateExposedVariable("ModsEnabledTitle", DynValue.NewBoolean(bool.Parse(text5)), Save: false);
				continue;
			}
			float result = 0f;
			if (text5.Equals("true") || text5.Equals("false"))
			{
				UpdateExposedVariable(text4, DynValue.NewBoolean(bool.Parse(text5)), Save: false);
			}
			else if (float.TryParse(text5, NumberStyles.Number, CultureInfo.InvariantCulture, out result))
			{
				UpdateExposedVariable(text4, DynValue.NewNumber(result), Save: false);
			}
			else
			{
				UpdateExposedVariable(text4, DynValue.NewString(text5), Save: false);
			}
		}
		streamReader.Close();
	}

	private void SaveExposedVariables()
	{
		string text = Path.Combine(Application.persistentDataPath, "ModConfig") + "\\" + Name + ".txt";
		if (File.Exists(text))
		{
			File.Delete(text);
		}
		foreach (ModManager.ExposedData exposedVar in ExposedVars)
		{
			if (exposedVar.IsKeybinding)
			{
				continue;
			}
			try
			{
				if (exposedVar.VarName.Equals("ModsEnabledTitle"))
				{
					File.AppendAllText(text, string.Concat("Enabled=", exposedVar.VarValue, "\n"));
				}
				else
				{
					File.AppendAllText(text, exposedVar.VarName + "=" + exposedVar.VarValue.ToPrintString() + "\n");
				}
			}
			catch (UnauthorizedAccessException ex)
			{
				ErrorMessage.LogError("Mod.SaveExposedVariables - UnauthorizedAccessException : " + text + " " + ex.ToString());
				break;
			}
		}
	}

	public void ResetScriptsBefore()
	{
		ExposedVars.Clear();
		LuaScripts.Clear();
		InputKeyPressCallback = null;
		InputMouseDownCallback = null;
	}

	public void ResetScriptsAfter()
	{
		AddExposedVariable("ModsEnabledTitle", DynValue.NewBoolean(v: true), null, null, null, UsingLookup: true);
		MenuStart(CallCreation: false);
		LoadExposedVariables();
	}
}
