using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using Rewired;
using Steamworks;
using UnityEngine;

[MoonSharpUserData]
public class ModManager : MonoBehaviour
{
	public enum ErrorState
	{
		No_Error,
		Error_Upload_Steam,
		Error_Upload_Title,
		Error_Upload_Description,
		Error_Upload_Image,
		Error_Upload_Tags,
		Error_Restart,
		Error_FailedSubcribe,
		Error_FailedUnsubcribe,
		Error_Delete_Steam,
		Error_FailedResults,
		Error_AcceptTCs,
		Error_Lua,
		Error_Misc,
		Error_Clash,
		Error_Dev_Folders
	}

	public struct ExposedData
	{
		public string VarName;

		public DynValue VarValue;

		public DataType VarType;

		public DynValue MinValue;

		public DynValue MaxValue;

		public bool UsesLookup;

		public DynValue Callback;

		public bool IsKeybinding;

		public List<DynValue> VarValuesList;
	}

	public enum CallbackTypes
	{
		None,
		FoodConsumed,
		ClothingTopAdded,
		ClothingTopRemoved,
		ClothingHatAdded,
		ClothingHatRemoved,
		ConverterCreateItem,
		ConverterComplete,
		HoldablePickedUp,
		HoldableDroppedOnGround,
		AddedToConverter,
		BuildingRenamed,
		BuildingRepositioned
	}

	public struct CallbackData
	{
		public ObjectType Object;

		public Script OwnerScript;

		public CallbackTypes CallbackType;

		public DynValue CallbackFunction;
	}

	public struct MinimalCallbackData
	{
		public Script OwnerScript;

		public DynValue CallbackFunction;
	}

	private bool DebugInfo;

	public static ModManager Instance;

	public List<Mod> CurrentMods;

	public List<Mod> LocalMods;

	public ModBase ModBaseClass;

	public ModSound ModSoundClass;

	public ModVariable ModVariableClass;

	public ModConverter ModConverterClass;

	public ModBuilding ModBuildingClass;

	public ModDecorative ModDecorativeClass;

	public ModTiles ModTilesClass;

	public ModPlayer ModPlayerClass;

	public ModBot ModBotClass;

	public ModDebug ModDebugClass;

	public ModCamera ModCameraClass;

	public ModHoldable ModHoldableClass;

	public ModFood ModFoodClass;

	public ModObject ModObjectClass;

	public ModTool ModToolClass;

	public ModQuest ModQuestClass;

	public ModGoTo ModGoToClass;

	public ModHat ModHatClass;

	public ModTop ModTopClass;

	public ModUI ModUIClass;

	public ModStorage ModStorageClass;

	public ModMedicine ModMedicineClass;

	public ModToy ModToyClass;

	public ModEducation ModEducationClass;

	public ModSaveData ModSaveDataClass;

	public ModTimer ModTimerClass;

	public ModText ModTextClass;

	public List<ModCustom> ModCustomClasses;

	public int CustomCreations;

	public Mod MenuSelectedMod;

	private List<string> AllModsScripts;

	public Script CreationScript;

	public bool MenuForceErrorReturn;

	public string OverrideErrorMessage;

	public string OverrideErrorModName;

	private bool ShowErrorMessageWhenSafe;

	private bool ScriptClashFound;

	public string SpawnsInfo = "";

	private string SpawnsInfoUpdated = "";

	public bool InMainUpdateState;

	private bool HasReset;

	private bool ExitGameTasksDone = true;

	public bool FailSafeDisabled;

	public bool RegisteredForInputPressCallback;

	public bool RegisteredForInputMouseDownCallback;

	private bool DoneInputSetup;

	public Dictionary<Mod, Script> RegisteredInputModsKeyPress;

	public Dictionary<Mod, Script> RegisteredInputModsMouseDown;

	private List<CallbackData> ModCallbacks;

	public Dictionary<int, List<CallbackData>> StorageAddedCallbacks;

	public Dictionary<int, List<CallbackData>> StorageRemovedCallbacks;

	public Dictionary<int, List<MinimalCallbackData>> StorageItemChangedCallbacks;

	public Dictionary<int, List<MinimalCallbackData>> BuildingEditedCallbacks;

	public Dictionary<ObjectType, List<MinimalCallbackData>> BuildingTypeSpawnedCallbacks;

	public Dictionary<int, List<MinimalCallbackData>> NewBuildingInAreaCallbacks;

	public Dictionary<ObjectType, List<MinimalCallbackData>> ItemTypeSpawnedCallbacks;

	public Dictionary<int, List<MinimalCallbackData>> PlayerOrBotEnterOrExitTileCallbacks;

	public Dictionary<int, List<MinimalCallbackData>> BuildingStateChangedCallbacks;

	public Dictionary<int, Tuple<int, int>> PlayerOrBotAlreadyAtTile;

	public int IntervalCallbackSequence = 1;

	public Dictionary<int, int> IntervalCounters;

	public Dictionary<int, ModTimer.IntervalCallbackData> IntervalCallbacks;

	public int m_LuaErrorCounter;

	public const int c_LuaErrorThreshold = 3;

	public ErrorState CurrentErrorState { get; private set; }

	public EResult SteamErrorCode { get; private set; }

	public Dictionary<ObjectType, string> m_ModStrings { get; private set; }

	public GameOptions m_GameOptionsRef { get; private set; }

	public void RegisterNewMod(Mod NewMod)
	{
		CurrentMods.Add(NewMod);
		if (NewMod.IsLocal)
		{
			LocalMods.Add(NewMod);
			if (DebugInfo)
			{
				Debug.Log(NewMod);
			}
		}
	}

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
		CurrentMods = new List<Mod>();
		LocalMods = new List<Mod>();
		m_ModStrings = new Dictionary<ObjectType, string>();
		RegisteredInputModsKeyPress = new Dictionary<Mod, Script>();
		RegisteredInputModsMouseDown = new Dictionary<Mod, Script>();
		ModCallbacks = new List<CallbackData>();
		StorageAddedCallbacks = new Dictionary<int, List<CallbackData>>();
		StorageRemovedCallbacks = new Dictionary<int, List<CallbackData>>();
		StorageItemChangedCallbacks = new Dictionary<int, List<MinimalCallbackData>>();
		BuildingEditedCallbacks = new Dictionary<int, List<MinimalCallbackData>>();
		BuildingTypeSpawnedCallbacks = new Dictionary<ObjectType, List<MinimalCallbackData>>();
		NewBuildingInAreaCallbacks = new Dictionary<int, List<MinimalCallbackData>>();
		ItemTypeSpawnedCallbacks = new Dictionary<ObjectType, List<MinimalCallbackData>>();
		PlayerOrBotEnterOrExitTileCallbacks = new Dictionary<int, List<MinimalCallbackData>>();
		PlayerOrBotAlreadyAtTile = new Dictionary<int, Tuple<int, int>>();
		BuildingStateChangedCallbacks = new Dictionary<int, List<MinimalCallbackData>>();
		IntervalCallbackSequence = 1;
		IntervalCounters = new Dictionary<int, int>();
		IntervalCallbacks = new Dictionary<int, ModTimer.IntervalCallbackData>();
		UserData.RegisterAssembly();
		Script.DefaultOptions.DebugPrint = delegate(string s)
		{
			Debug.Log(s);
		};
		AllModsScripts = new List<string>();
		ModSoundClass = new ModSound();
		ModVariableClass = new ModVariable();
		ModConverterClass = new ModConverter();
		ModBuildingClass = new ModBuilding();
		ModBaseClass = new ModBase();
		ModDecorativeClass = new ModDecorative();
		ModTilesClass = new ModTiles();
		ModPlayerClass = new ModPlayer();
		ModBotClass = new ModBot();
		ModDebugClass = new ModDebug();
		ModHoldableClass = new ModHoldable();
		ModCameraClass = new ModCamera();
		ModFoodClass = new ModFood();
		ModObjectClass = new ModObject();
		ModToolClass = new ModTool();
		ModQuestClass = new ModQuest();
		ModGoToClass = new ModGoTo();
		ModHatClass = new ModHat();
		ModTopClass = new ModTop();
		ModUIClass = new ModUI();
		ModStorageClass = new ModStorage();
		ModMedicineClass = new ModMedicine();
		ModToyClass = new ModToy();
		ModEducationClass = new ModEducation();
		ModSaveDataClass = new ModSaveData();
		ModTimerClass = new ModTimer();
		ModTextClass = new ModText();
		ModCustomClasses = new List<ModCustom>();
		ModCustomClasses.Add(ModConverterClass);
		ModCustomClasses.Add(ModBuildingClass);
		ModCustomClasses.Add(ModDecorativeClass);
		ModCustomClasses.Add(ModHoldableClass);
		ModCustomClasses.Add(ModFoodClass);
		ModCustomClasses.Add(ModToolClass);
		ModCustomClasses.Add(ModGoToClass);
		ModCustomClasses.Add(ModHatClass);
		ModCustomClasses.Add(ModTopClass);
		ModCustomClasses.Add(ModEducationClass);
		ModCustomClasses.Add(ModToyClass);
		ModCustomClasses.Add(ModMedicineClass);
		foreach (ModCustom modCustomClass in ModCustomClasses)
		{
			modCustomClass.Init();
		}
		ClearLog("ModError");
		Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "ModConfig"));
		m_LuaErrorCounter = 0;
	}

	private void Start()
	{
	}

	private void DoResetBeforeLoad()
	{
		VariableManager.Instance.ReInit();
		StorageTypeManager.Instance.Reset();
		AudioManager.Instance.ResetAllModSounds();
		foreach (Mod currentMod in CurrentMods)
		{
			currentMod.StartedUp = false;
		}
		FailSafeDisabled = false;
		StorageAddedCallbacks.Clear();
		StorageRemovedCallbacks.Clear();
		StorageItemChangedCallbacks.Clear();
		BuildingEditedCallbacks.Clear();
		BuildingStateChangedCallbacks.Clear();
		BuildingTypeSpawnedCallbacks.Clear();
		NewBuildingInAreaCallbacks.Clear();
		ItemTypeSpawnedCallbacks.Clear();
		PlayerOrBotEnterOrExitTileCallbacks.Clear();
		PlayerOrBotAlreadyAtTile.Clear();
		IntervalCallbackSequence = 1;
		IntervalCounters.Clear();
		IntervalCallbacks.Clear();
		m_LuaErrorCounter = 0;
	}

	public void PostCreateScripts(bool CreatedGame)
	{
		for (int i = 0; i < CurrentMods.Count; i++)
		{
			CurrentMods[i].PostCreateSpecific(CreatedGame);
		}
		UpdateSaveSpawnsInfo();
		InMainUpdateState = true;
	}

	private void InputCallbackPressed(InputActionEventData data)
	{
		if (!GameStateManager.Instance || GameStateManager.Instance.GetCurrentState().m_BaseState != 0)
		{
			return;
		}
		foreach (Mod currentMod in CurrentMods)
		{
			if (currentMod.IsEnabled && currentMod.IsUsingKeybindings)
			{
				currentMod.UpdateKeybindingsCall(data);
			}
		}
	}

	private void Update()
	{
		if ((bool)MyInputManager.Instance && !DoneInputSetup && (bool)AudioManager.Instance)
		{
			DoneInputSetup = true;
			for (int i = 0; i < 10; i++)
			{
				MyInputManager.m_Rewired.AddInputEventDelegate(InputCallbackPressed, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, 49 + i);
			}
		}
		if (RegisteredForInputPressCallback)
		{
			for (int j = 0; j <= 296; j++)
			{
				KeyCode keyCode = (KeyCode)j;
				if (keyCode == KeyCode.F1 || keyCode == KeyCode.F2 || keyCode == KeyCode.F9 || keyCode == KeyCode.F10 || !Input.GetKeyDown(keyCode))
				{
					continue;
				}
				foreach (KeyValuePair<Mod, Script> item in RegisteredInputModsKeyPress)
				{
					if (item.Key.IsEnabled)
					{
						DynValue[] args = new DynValue[1] { DynValue.NewString(keyCode.ToString()) };
						Callback(item.Value, item.Key.InputKeyPressCallback, args);
					}
				}
				break;
			}
		}
		if (RegisteredForInputMouseDownCallback && Input.GetMouseButtonDown(0))
		{
			foreach (KeyValuePair<Mod, Script> item2 in RegisteredInputModsMouseDown)
			{
				if (item2.Key.IsEnabled && (bool)HudManager.Instance)
				{
					TileCoord NewCoord = default(TileCoord);
					Vector3 HitPosition = default(Vector3);
					int UID = -1;
					GameStateManager.Instance.GetCurrentState().GetObjectUnderMouse(TestTiles: true, TestBuildings: true, TestMisc: true, TestBots: true, out UID, out NewCoord, out HitPosition);
					DynValue[] args2 = new DynValue[3]
					{
						DynValue.NewNumber(NewCoord.x),
						DynValue.NewNumber(NewCoord.y),
						DynValue.NewNumber(UID)
					};
					Callback(item2.Value, item2.Key.InputMouseDownCallback, args2);
				}
			}
		}
		if (!ExitGameTasksDone && !GeneralUtils.m_InGame)
		{
			if (GetComponent<AudioSource>() != null)
			{
				UnityEngine.Object.Destroy(base.gameObject.GetComponent<AudioSource>());
			}
			HasReset = false;
			ExitGameTasksDone = true;
		}
		if (!HasReset && GeneralUtils.m_InGame)
		{
			DoResetBeforeLoad();
			HasReset = true;
			ExitGameTasksDone = false;
		}
		ProcessIntervalCallbacks((int)(Time.deltaTime * 1000f));
		for (int k = 0; k < CurrentMods.Count; k++)
		{
			CurrentMods[k].Update();
		}
		if (ShowErrorMessageWhenSafe && GameStateManager.Instance != null)
		{
			GameStateManager.State actualState = GameStateManager.Instance.GetActualState();
			if (actualState == GameStateManager.State.Normal || actualState == GameStateManager.State.Edit || actualState == GameStateManager.State.MainMenu || actualState == GameStateManager.State.Start)
			{
				LaunchErrorMessage();
				ShowErrorMessageWhenSafe = false;
			}
		}
		if (ScriptClashFound && GameStateManager.Instance != null && CurrentErrorState != 0 && (bool)GameStateManager.Instance && GameStateManager.Instance.GetActualState() == GameStateManager.State.MainMenu)
		{
			OutputErrorText();
			GameStateManager.Instance.PushState(GameStateManager.State.ModsError);
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateModsError>().SetCurrentError();
			ScriptClashFound = false;
		}
	}

	public void AfterSave()
	{
		for (int i = 0; i < CurrentMods.Count; i++)
		{
			CurrentMods[i].AfterSave();
		}
	}

	public AudioClip FindModAudioClip(string FileName)
	{
		Mod lastCalledMod = GetLastCalledMod();
		if (lastCalledMod != null)
		{
			AudioClip sound = lastCalledMod.GetSound(FileName.ToLower());
			if (sound != null)
			{
				return sound;
			}
		}
		if (DebugInfo)
		{
			Debug.LogError("FindModAudioClip: Didn't Find Audio, Searching all mods");
		}
		return FindModAudioClipAllMods(FileName);
	}

	public AudioClip FindModAudioClipAllMods(string FileName)
	{
		for (int i = 0; i < CurrentMods.Count; i++)
		{
			AudioClip sound = CurrentMods[i].GetSound(FileName.ToLower());
			if (sound != null)
			{
				return sound;
			}
		}
		if (DebugInfo)
		{
			Debug.LogError("FindModAudioClipAllMods: Didn't Find Audio!");
		}
		return null;
	}

	public Texture FindModTexture(string FileName)
	{
		Mod lastCalledMod = GetLastCalledMod();
		if (lastCalledMod != null)
		{
			Texture texture = lastCalledMod.GetTexture(FileName.ToLower());
			if (texture != null)
			{
				return texture;
			}
		}
		if (DebugInfo)
		{
			Debug.LogError("FindModTexture: Didn't Find Texture, Searching all mods");
		}
		return FindModTextureAllMods(FileName);
	}

	public Texture FindModTextureAllMods(string FileName)
	{
		for (int i = 0; i < CurrentMods.Count; i++)
		{
			Texture texture = CurrentMods[i].GetTexture(FileName.ToLower());
			if (texture != null)
			{
				return texture;
			}
		}
		if (DebugInfo)
		{
			Debug.LogError("FindModTextureAllMods: Didn't Find Texture!");
		}
		return null;
	}

	public List<Texture2D> GetAllModTextures()
	{
		List<Texture2D> list = new List<Texture2D>();
		for (int i = 0; i < CurrentMods.Count; i++)
		{
			foreach (Texture allTexture in CurrentMods[i].GetAllTextures())
			{
				list.Add(allTexture as Texture2D);
			}
		}
		return list;
	}

	public void AddModString(ObjectType KeyName, string ObjectName)
	{
		if (!m_ModStrings.ContainsKey(KeyName))
		{
			m_ModStrings.Add(KeyName, ObjectName);
		}
	}

	public bool FindModStringFromValue(string ValueName, out string FoundValue)
	{
		foreach (KeyValuePair<ObjectType, string> modString in m_ModStrings)
		{
			if (modString.Value.Contains(ValueName))
			{
				FoundValue = modString.Value;
				return true;
			}
		}
		FoundValue = ValueName;
		return false;
	}

	public bool GetCustomClassData(ObjectType ObjID, out string UniqueName, out string PrefabLocation, out ObjectSubCategory SubCat, out bool CanStack)
	{
		foreach (ModCustom modCustomClass in ModCustomClasses)
		{
			if (modCustomClass.ModIDOriginals.TryGetValue(ObjID, out UniqueName))
			{
				PrefabLocation = modCustomClass.GetPrefabLocation();
				SubCat = modCustomClass.GetSubcategory();
				CanStack = modCustomClass.GetStackable();
				return true;
			}
		}
		UniqueName = "";
		PrefabLocation = "";
		SubCat = ObjectSubCategory.Any;
		CanStack = false;
		return false;
	}

	public void SetErrorSteam(ErrorState InErrorState, EResult InErrorCode = EResult.k_EResultOK)
	{
		CurrentErrorState = InErrorState;
		SteamErrorCode = InErrorCode;
		MenuForceErrorReturn = true;
		OverrideErrorMessage = string.Concat(InErrorState, " ", InErrorCode);
		OutputErrorText();
	}

	public void SetErrorLua(ErrorState InErrorState, string DescriptionOverride = null, string ModNameOverride = null)
	{
		if (m_LuaErrorCounter != -1)
		{
			OverrideErrorMessage = DescriptionOverride;
			OverrideErrorModName = ModNameOverride;
			if (m_LuaErrorCounter < 3)
			{
				CurrentErrorState = InErrorState;
				MenuForceErrorReturn = true;
				ShowErrorMessageWhenSafe = true;
			}
			OutputErrorText();
		}
	}

	public void ClearError()
	{
		CurrentErrorState = ErrorState.No_Error;
		SteamErrorCode = EResult.k_EResultOK;
		OverrideErrorMessage = "";
		OverrideErrorModName = null;
		ShowErrorMessageWhenSafe = false;
	}

	public bool AllModsInitialised()
	{
		if (!ModLoaderManager.Instance.IsFullyLoaded())
		{
			return false;
		}
		int num = 0;
		for (int i = 0; i < CurrentMods.Count; i++)
		{
			num += (CurrentMods[i].MenuStartedUp ? 1 : 0);
		}
		return num == CurrentMods.Count;
	}

	public bool IsModelUsingCustomModel(ObjectType NewType, out string ModelName)
	{
		ModelName = "";
		foreach (ModCustom modCustomClass in ModCustomClasses)
		{
			if (modCustomClass.ModModels.TryGetValue(NewType, out ModelName))
			{
				return modCustomClass.IsUsingCustomModel(NewType);
			}
		}
		return false;
	}

	public GameObject GetModModel(ObjectType NewType)
	{
		string value = "";
		for (int i = 0; i < CurrentMods.Count; i++)
		{
			if (!CurrentMods[i].CustomIDs.Contains(NewType))
			{
				continue;
			}
			foreach (ModCustom modCustomClass in ModCustomClasses)
			{
				if (modCustomClass.ModModels.TryGetValue(NewType, out value))
				{
					return CurrentMods[i].GetCustomModel(value);
				}
			}
		}
		return null;
	}

	public ModCustom GetCustomModInfo(ObjectType NewType)
	{
		for (int i = 0; i < CurrentMods.Count; i++)
		{
			if (!CurrentMods[i].CustomIDs.Contains(NewType))
			{
				continue;
			}
			using List<ModCustom>.Enumerator enumerator = ModCustomClasses.GetEnumerator();
			if (enumerator.MoveNext())
			{
				return enumerator.Current;
			}
		}
		return null;
	}

	public ObjectType GetModObjectTypeFromName(string Name)
	{
		foreach (ModCustom modCustomClass in ModCustomClasses)
		{
			foreach (KeyValuePair<ObjectType, string> modIDOriginal in modCustomClass.ModIDOriginals)
			{
				if (modIDOriginal.Value.Equals(Name))
				{
					return modIDOriginal.Key;
				}
			}
		}
		return ObjectType.Nothing;
	}

	public void GetCustomModelTransform(ObjectType NewType, out Vector3 ModelTranslation, out Vector3 ModelRotation, out Vector3 ModelScale)
	{
		ModelScale = new Vector3(-1f, 1f, 1f);
		ModelRotation = new Vector3(0f, 0f, 0f);
		ModelTranslation = new Vector3(0f, 0f, 0f);
		using (List<ModCustom>.Enumerator enumerator = ModCustomClasses.GetEnumerator())
		{
			while (enumerator.MoveNext() && !enumerator.Current.GetModelScale(NewType, out ModelScale))
			{
			}
		}
		using (List<ModCustom>.Enumerator enumerator = ModCustomClasses.GetEnumerator())
		{
			while (enumerator.MoveNext() && !enumerator.Current.GetModelTranslation(NewType, out ModelTranslation))
			{
			}
		}

        using (List<ModCustom>.Enumerator enumerator = ModCustomClasses.GetEnumerator())
        {
            while (enumerator.MoveNext() && !enumerator.Current.GetModelRotation(NewType, out ModelRotation))
            {
            }
        }
    }

	public Mod GetModContainingItem(ObjectType NewType)
	{
		for (int i = 0; i < CurrentMods.Count; i++)
		{
			if (CurrentMods[i].CustomIDs.Contains(NewType))
			{
				return CurrentMods[i];
			}
		}
		return null;
	}

	private void LaunchErrorMessage()
	{
		if (CurrentErrorState != 0)
		{
			GameStateManager.Instance.PushState(GameStateManager.State.ModsError);
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateModsError>().SetCurrentError();
		}
	}

	private void OutputErrorText()
	{
		WriteModError(OverrideErrorMessage, OverrideErrorModName);
	}

	public void WriteModError(string Message, string FilePrefix = null)
	{
		DateTime now = DateTime.Now;
		string message = string.Concat(now.ToLocalTime(), " ", Message, "\n");
		if (FilePrefix != null && Regex.IsMatch(FilePrefix, "\\D") && !Directory.Exists(Path.Combine(Application.streamingAssetsPath, "Mods") + "\\" + FilePrefix))
		{
			FilePrefix = null;
		}
		if (FilePrefix == null)
		{
			Mod lastCalledMod = GetLastCalledMod();
			if (lastCalledMod != null)
			{
				FilePrefix = lastCalledMod.Name;
			}
		}
		if (!string.IsNullOrEmpty(FilePrefix))
		{
			WriteLog(FilePrefix + "_ErrorLog", message);
		}
		else
		{
			WriteLog("ModError", message);
		}
	}

	public void WriteModDebug(string Message)
	{
		Mod lastCalledMod = Instance.GetLastCalledMod();
		string text = "";
		if (lastCalledMod == null)
		{
			Script lastCalledScript = Instance.GetLastCalledScript();
			if (lastCalledScript != null)
			{
				text = lastCalledScript.Globals["_ModDisplayName"].ToString();
				if (!Directory.Exists(Path.Combine(Application.streamingAssetsPath, "Mods") + "\\" + text))
				{
					text = "";
				}
			}
		}
		else if (lastCalledMod.IsLocal)
		{
			text = lastCalledMod.Name;
		}
		if (text != "")
		{
			WriteLog(text + "_DebugLog", Message + "\n");
		}
	}

	private void WriteLog(string LogName, string Message)
	{
		string text = Path.Combine(Application.streamingAssetsPath, "Mods") + "\\" + LogName + ".txt";
		try
		{
			FileInfo fileInfo = new FileInfo(text);
			if (!fileInfo.Exists || fileInfo.Length <= 1048576)
			{
				File.AppendAllText(text, Message);
			}
		}
		catch (UnauthorizedAccessException ex)
		{
			ErrorMessage.LogError("ModManger.WriteToLog - UnauthorizedAccessException : " + text + " " + ex.ToString());
		}
	}

	public void ClearLog(string LogName)
	{
		string text = Path.Combine(Application.streamingAssetsPath, "Mods") + "\\" + LogName + ".txt";
		try
		{
			if (File.Exists(text))
			{
				File.Delete(text);
			}
		}
		catch (UnauthorizedAccessException ex)
		{
			ErrorMessage.LogError("ModManger.ClearLog - UnauthorizedAccessException : " + text + " " + ex.ToString());
		}
	}

	public void AddModScripts(string[] MainScripts)
	{
		foreach (string text in MainScripts)
		{
			if (text.Contains("StreamingAssets"))
			{
				AllModsScripts.Add(text);
			}
		}
	}

	public void CheckAllModScriptsForClash()
	{
		string[] array = new string[10] { "ModBase.Set", "ModBase.SpawnItem", "ModBuilding.CreateBuilding", "ModConverter.CreateConverter", "ModDecorative.CreateDecorative", "ModSound.ChangeSound", "ModSound.ChangeVolume", "ModSound.ChangePitch", "ModTiles.SetTile", "ModVariable.Set" };
		foreach (string checkStr in array)
		{
			CheckSingleScriptForClash(checkStr);
		}
	}

	public void CheckSingleScriptForClash(string CheckStr)
	{
	}

	public void LoadInitialSpawnsInfo()
	{
		string path = Path.Combine(Application.streamingAssetsPath, "Mods") + "\\ModSpawns.txt";
		if (File.Exists(path))
		{
			SpawnsInfo = File.ReadAllText(path);
			SpawnsInfoUpdated = SpawnsInfo;
		}
	}

	public void UpdateSaveSpawnsInfo(string Item = "Nothing", int xPos = 0, int yPos = 0, bool Save = false)
	{
		if (Save)
		{
			if (SpawnsInfoUpdated != "")
			{
				string text = Path.Combine(Application.streamingAssetsPath, "Mods") + "\\ModSpawns.txt";
				try
				{
					File.WriteAllText(text, SpawnsInfoUpdated);
				}
				catch (UnauthorizedAccessException ex)
				{
					ErrorMessage.LogError("ModManager.UpdateSaveSpawnsInfo - UnauthorizedAccessException : " + text + " " + ex.ToString());
				}
			}
		}
		else
		{
			SpawnsInfoUpdated = SpawnsInfoUpdated + Item + "-" + xPos + "-" + yPos + "-";
		}
	}

	public Mod GetLastCalledMod()
	{
		Script lastCalledScript = GetLastCalledScript();
		foreach (Mod currentMod in CurrentMods)
		{
			foreach (Script luaScript in currentMod.LuaScripts)
			{
				if (luaScript.Equals(lastCalledScript))
				{
					return currentMod;
				}
			}
		}
		return null;
	}

	public Script GetLastCalledScript()
	{
		return MethodMemberDescriptor.CurrentScript;
	}

	public void OutputAllDataTypes()
	{
		if (!TextManager.Instance)
		{
			return;
		}
		if (!Directory.Exists(Path.Combine(Application.streamingAssetsPath, "Mods\\TypesOutput")))
		{
			Directory.CreateDirectory(Path.Combine(Application.streamingAssetsPath, "Mods\\TypesOutput"));
		}
		string path = Path.Combine(Application.streamingAssetsPath, "Mods") + "\\TypesOutput\\Types-Objects.txt";
		if (File.Exists(path))
		{
			File.Delete(path);
		}
		File.AppendAllText(path, "Object Types:\n\n");
		for (int i = 0; i < 673; i++)
		{
			ObjectType objectType = (ObjectType)i;
			File.AppendAllText(path, objectType.ToString() + " (" + TextManager.Instance.Get(objectType.ToString()) + ")\n");
		}
		path = Path.Combine(Application.streamingAssetsPath, "Mods") + "\\TypesOutput\\Types-FarmerStates.txt";
		if (File.Exists(path))
		{
			File.Delete(path);
		}
		File.AppendAllText(path, "Farmer States:\n\n");
		for (int j = 0; j < 52; j++)
		{
			Farmer.State state = (Farmer.State)j;
			File.AppendAllText(path, state.ToString() + "\n");
		}
		path = Path.Combine(Application.streamingAssetsPath, "Mods") + "\\TypesOutput\\Types-Tiles.txt";
		if (File.Exists(path))
		{
			File.Delete(path);
		}
		File.AppendAllText(path, "Tile Types:\n\n");
		for (int k = 0; k < 71; k++)
		{
			Tile.TileType newType = (Tile.TileType)k;
			File.AppendAllText(path, newType.ToString() + " (" + TextManager.Instance.Get(Tile.GetNameFromType(newType)).ToString() + ")\n");
		}
		path = Path.Combine(Application.streamingAssetsPath, "Mods") + "\\TypesOutput\\Types-GameStates.txt";
		if (File.Exists(path))
		{
			File.Delete(path);
		}
		File.AppendAllText(path, "Game States:\n\n");
		for (int l = 0; l < 60; l++)
		{
			GameStateManager.State state2 = (GameStateManager.State)l;
			File.AppendAllText(path, state2.ToString() + "\n");
		}
		path = Path.Combine(Application.streamingAssetsPath, "Mods") + "\\TypesOutput\\Types-AudioEvents.txt";
		if (File.Exists(path))
		{
			File.Delete(path);
		}
		File.AppendAllText(path, "Audio Events:\n\n");
		List<AudioSource> allSounds = AudioManager.Instance.GetAllSounds();
		List<string> list = new List<string>();
		foreach (AudioSource item in allSounds)
		{
			if (item != null && item.clip != null && !list.Contains(item.clip.name.ToString()))
			{
				list.Add(item.clip.name.ToString());
			}
		}
		foreach (string item2 in list)
		{
			File.AppendAllText(path, item2 + "\n");
		}
		path = Path.Combine(Application.streamingAssetsPath, "Mods") + "\\TypesOutput\\Types-GameModels.txt";
		if (File.Exists(path))
		{
			File.Delete(path);
		}
		File.AppendAllText(path, "Models:\n\n");
		ObjectTypeInfo[] objects = ObjectTypeList.m_Objects;
		foreach (ObjectTypeInfo objectTypeInfo in objects)
		{
			if (objectTypeInfo.m_ModelName.Length > 0 && objectTypeInfo.m_ModelName.Contains("Models/"))
			{
				File.AppendAllText(path, objectTypeInfo.m_ModelName + "\n");
			}
		}
		path = Path.Combine(Application.streamingAssetsPath, "Mods") + "\\TypesOutput\\Types-Variables.txt";
		if (File.Exists(path))
		{
			File.Delete(path);
		}
		File.AppendAllText(path, "Variables:\n\n");
		foreach (KeyValuePair<string, VariableManager.Value> variable in VariableManager.Instance.m_Variables)
		{
			string text = variable.Key.Replace(".", " ");
			if (variable.Value.m_String != null && variable.Value.m_String.Length > 0)
			{
				File.AppendAllText(path, text + " Value:" + variable.Value.m_String + "\n");
			}
			else if ((float)variable.Value.m_Int != variable.Value.m_Float)
			{
				File.AppendAllText(path, text + " Value:" + variable.Value.m_Float + "\n");
			}
			else
			{
				File.AppendAllText(path, text + " Value:" + variable.Value.m_Int + "\n");
			}
		}
	}

	public void ResetBeforeLoad()
	{
		if (!SettingsManager.Instance.m_DevMode)
		{
			HasReset = false;
		}
		else
		{
			ReloadScripts();
		}
	}

	public void ReloadScripts()
	{
		AllModsScripts.Clear();
		RegisteredForInputPressCallback = false;
		RegisteredForInputMouseDownCallback = false;
		RegisteredInputModsKeyPress.Clear();
		RegisteredInputModsMouseDown.Clear();
		foreach (Mod currentMod in CurrentMods)
		{
			currentMod.ResetScriptsBefore();
			if (Directory.Exists(currentMod.FolderLocation))
			{
				currentMod.AddScripts();
				currentMod.ResetScriptsAfter();
			}
		}
		DoResetBeforeLoad();
	}

	public void SetInitialMapData(GameOptions Ref)
	{
		m_GameOptionsRef = Ref;
		for (int i = 0; i < CurrentMods.Count; i++)
		{
			CurrentMods[i].SetupInitialMapData();
		}
		m_GameOptionsRef = null;
	}

	public void Callback(Script ModScript, DynValue function, params DynValue[] args)
	{
		try
		{
			ModScript.Call(function, args);
		}
		catch (ScriptRuntimeException ex)
		{
			string text = ModScript.Globals["_ModDisplayName"].ToString();
			string text2 = ModScript.Globals["_ModScriptFile"].ToString();
			string descriptionOverride = text + "\n" + text2 + Mod.ScriptErrorMessage(ex);
			SetErrorLua(ErrorState.Error_Lua, descriptionOverride, text);
		}
		catch (Exception ex2)
		{
			Debug.LogWarning("Callback error!");
			WriteModError("Callback Error: " + ex2.ToString());
		}
	}

	public void RegisterCustomCallback(CallbackData Info)
	{
		ModCallbacks.Add(Info);
	}

	public void CheckCustomCallback(CallbackTypes Type, ObjectType Obj, TileCoord Location, int ObjectUniqueID, int PlayerUniqueID)
	{
		foreach (CallbackData modCallback in ModCallbacks)
		{
			if (modCallback.Object != Obj || modCallback.CallbackType != Type)
			{
				continue;
			}
			string str = Obj.ToString();
			foreach (ModCustom modCustomClass in ModCustomClasses)
			{
				if (modCustomClass.ModIDOriginals.TryGetValue(Obj, out var value))
				{
					str = value;
					break;
				}
			}
			DynValue[] args = new DynValue[5]
			{
				DynValue.NewString(str),
				DynValue.NewNumber(Location.x),
				DynValue.NewNumber(Location.y),
				DynValue.NewNumber(ObjectUniqueID),
				DynValue.NewNumber(PlayerUniqueID)
			};
			Callback(modCallback.OwnerScript, modCallback.CallbackFunction, args);
		}
	}

	public void CheckStorageAddedCallback(int ObjectUniqueID)
	{
		if (StorageAddedCallbacks.ContainsKey(ObjectUniqueID))
		{
			List<CallbackData> list = StorageAddedCallbacks[ObjectUniqueID];
			for (int i = 0; i < list.Count; i++)
			{
				Callback(list[i].OwnerScript, list[i].CallbackFunction, DynValue.NewNumber(ObjectUniqueID));
			}
		}
	}

	public void CheckStorageRemovedCallback(int ObjectUniqueID)
	{
		if (StorageRemovedCallbacks.ContainsKey(ObjectUniqueID))
		{
			List<CallbackData> list = StorageRemovedCallbacks[ObjectUniqueID];
			for (int i = 0; i < list.Count; i++)
			{
				Callback(list[i].OwnerScript, list[i].CallbackFunction, DynValue.NewNumber(ObjectUniqueID));
			}
		}
	}

	public void CheckStorageItemChangedCallback(int ObjectUniqueID, string NewItemTypeStored)
	{
		if (StorageItemChangedCallbacks.ContainsKey(ObjectUniqueID))
		{
			List<MinimalCallbackData> list = StorageItemChangedCallbacks[ObjectUniqueID];
			DynValue[] args = new DynValue[2]
			{
				DynValue.NewNumber(ObjectUniqueID),
				DynValue.NewString(NewItemTypeStored)
			};
			for (int i = 0; i < list.Count; i++)
			{
				Callback(list[i].OwnerScript, list[i].CallbackFunction, args);
			}
		}
	}

	public void CheckBuildingStateChangedCallback(int m_UniqueID, string NewState)
	{
		if (BuildingStateChangedCallbacks.ContainsKey(m_UniqueID))
		{
			DynValue[] args = new DynValue[2]
			{
				DynValue.NewNumber(m_UniqueID),
				DynValue.NewString(NewState)
			};
			List<MinimalCallbackData> list = BuildingStateChangedCallbacks[m_UniqueID];
			for (int i = 0; i < list.Count; i++)
			{
				list[i].OwnerScript.Call(list[i].CallbackFunction, args);
			}
		}
	}

	public void CheckBuildingStateChangedCallback(string m_UniqueID, string NewState)
	{
	}

	public void CheckBuildingStateChangedCallback(string m_UniqueID, int NewState)
	{
	}

	public void CheckBuildingEditedCallback(Building BuildingInstance, string EditType, string NewValue = null)
	{
		if (BuildingEditedCallbacks.ContainsKey(BuildingInstance.m_UniqueID))
		{
			DynValue dynValue = EditType switch
			{
				"Rotate" => DynValue.NewNumber(BuildingInstance.m_Rotation), 
				"Move" => DynValue.NewString(BuildingInstance.m_TileCoord.x + ":" + BuildingInstance.m_TileCoord.y), 
				"Rename" => DynValue.NewString(NewValue), 
				_ => DynValue.NewString(""), 
			};
			DynValue[] args = new DynValue[3]
			{
				DynValue.NewNumber(BuildingInstance.m_UniqueID),
				DynValue.NewString(EditType),
				dynValue
			};
			List<MinimalCallbackData> list = BuildingEditedCallbacks[BuildingInstance.m_UniqueID];
			for (int i = 0; i < list.Count; i++)
			{
				Callback(list[i].OwnerScript, list[i].CallbackFunction, args);
			}
		}
	}

	public void CheckBuildingTypeSpawnedCallback(Building BuildingInstance)
	{
		if (BuildingTypeSpawnedCallbacks.ContainsKey(BuildingInstance.m_TypeIdentifier))
		{
			bool v = (bool)GameStateEdit.Instance && GameStateEdit.Instance.CheckBuildingInDragModels(BuildingInstance);
			DynValue[] args = new DynValue[4]
			{
				DynValue.NewNumber(BuildingInstance.m_UniqueID),
				DynValue.NewString(BuildingInstance.m_TypeIdentifier.ToString()),
				DynValue.NewBoolean(BuildingInstance.m_Blueprint),
				DynValue.NewBoolean(v)
			};
			List<MinimalCallbackData> list = BuildingTypeSpawnedCallbacks[BuildingInstance.m_TypeIdentifier];
			for (int i = 0; i < list.Count; i++)
			{
				Callback(list[i].OwnerScript, list[i].CallbackFunction, args);
			}
		}
	}

	public bool CheckNewBuildingInAreaCallback(Building BuildingInstance, TileCoord Position)
	{
		int key = Position.y * TileManager.Instance.m_TilesWide + Position.x;
		bool result = false;
		if (NewBuildingInAreaCallbacks.ContainsKey(key))
		{
			result = true;
			bool v = (bool)GameStateEdit.Instance && GameStateEdit.Instance.CheckBuildingInDragModels(BuildingInstance);
			DynValue[] args = new DynValue[3]
			{
				DynValue.NewNumber(BuildingInstance.m_UniqueID),
				DynValue.NewBoolean(BuildingInstance.m_Blueprint),
				DynValue.NewBoolean(v)
			};
			List<MinimalCallbackData> list = NewBuildingInAreaCallbacks[key];
			for (int i = 0; i < list.Count; i++)
			{
				Callback(list[i].OwnerScript, list[i].CallbackFunction, args);
			}
		}
		return result;
	}

	public void CheckItemTypeSpawnedCallback(int ObjectUID, ObjectType OType, TileCoord Position)
	{
		if (ItemTypeSpawnedCallbacks.ContainsKey(OType))
		{
			List<MinimalCallbackData> list = ItemTypeSpawnedCallbacks[OType];
			DynValue[] args = new DynValue[4]
			{
				DynValue.NewNumber(ObjectUID),
				DynValue.NewString(OType.ToString()),
				DynValue.NewNumber(Position.x),
				DynValue.NewNumber(Position.y)
			};
			for (int i = 0; i < list.Count; i++)
			{
				Callback(list[i].OwnerScript, list[i].CallbackFunction, args);
			}
		}
	}

	public void CheckPlayerOrBotEnterOrExitTileCallback(TileCoord Position, int FarmerOrWorkerUID)
	{
		if (PlayerOrBotAlreadyAtTile.ContainsKey(FarmerOrWorkerUID))
		{
			Tuple<int, int> tuple = PlayerOrBotAlreadyAtTile[FarmerOrWorkerUID];
			int item = tuple.Item1;
			int item2 = tuple.Item2;
			PlayerOrBotAlreadyAtTile.Remove(FarmerOrWorkerUID);
			int key = item2 * TileManager.Instance.m_TilesWide + item;
			if (PlayerOrBotEnterOrExitTileCallbacks.ContainsKey(key))
			{
				List<MinimalCallbackData> list = PlayerOrBotEnterOrExitTileCallbacks[key];
				DynValue[] args = new DynValue[4]
				{
					DynValue.NewNumber(FarmerOrWorkerUID),
					DynValue.NewNumber(item),
					DynValue.NewNumber(item2),
					DynValue.NewBoolean(v: false)
				};
				for (int i = 0; i < list.Count; i++)
				{
					Callback(list[i].OwnerScript, list[i].CallbackFunction, args);
				}
			}
		}
		int key2 = Position.y * TileManager.Instance.m_TilesWide + Position.x;
		if (PlayerOrBotEnterOrExitTileCallbacks.ContainsKey(key2))
		{
			if (!PlayerOrBotAlreadyAtTile.ContainsKey(FarmerOrWorkerUID))
			{
				Tuple<int, int> value = new Tuple<int, int>(Position.x, Position.y);
				PlayerOrBotAlreadyAtTile.Add(FarmerOrWorkerUID, value);
			}
			List<MinimalCallbackData> list2 = PlayerOrBotEnterOrExitTileCallbacks[key2];
			for (int j = 0; j < list2.Count; j++)
			{
				DynValue[] args2 = new DynValue[4]
				{
					DynValue.NewNumber(FarmerOrWorkerUID),
					DynValue.NewNumber(Position.x),
					DynValue.NewNumber(Position.y),
					DynValue.NewBoolean(v: true)
				};
				Callback(list2[j].OwnerScript, list2[j].CallbackFunction, args2);
			}
		}
	}

	public void AddOrOverwriteCallbackInList(ref List<CallbackData> Dats, DynValue Callback, ObjectType NewType, CallbackTypes NewCallbackType)
	{
		Script lastCalledScript = GetLastCalledScript();
		bool flag = false;
		CallbackData value = default(CallbackData);
		for (int i = 0; i < Dats.Count; i++)
		{
			if (Dats[i].OwnerScript == lastCalledScript)
			{
				flag = true;
				value.CallbackFunction = Callback;
				value.OwnerScript = lastCalledScript;
				value.Object = NewType;
				value.CallbackType = NewCallbackType;
				Dats[i] = value;
			}
		}
		if (!flag)
		{
			CallbackData item = default(CallbackData);
			item.CallbackFunction = Callback;
			item.OwnerScript = lastCalledScript;
			item.Object = NewType;
			item.CallbackType = NewCallbackType;
			Dats.Add(item);
		}
	}

	public void AddOrOverwriteCallbackInList(ref List<MinimalCallbackData> Dats, DynValue Callback)
	{
		Script lastCalledScript = GetLastCalledScript();
		bool flag = false;
		MinimalCallbackData value = default(MinimalCallbackData);
		for (int i = 0; i < Dats.Count; i++)
		{
			if (Dats[i].OwnerScript == lastCalledScript)
			{
				flag = true;
				value.CallbackFunction = Callback;
				value.OwnerScript = lastCalledScript;
				Dats[i] = value;
			}
		}
		if (!flag)
		{
			MinimalCallbackData item = default(MinimalCallbackData);
			item.CallbackFunction = Callback;
			item.OwnerScript = lastCalledScript;
			Dats.Add(item);
		}
	}

	public void RemoveCallbackFromList(ref List<MinimalCallbackData> Dats)
	{
		Script lastCalledScript = GetLastCalledScript();
		for (int i = 0; i < Dats.Count; i++)
		{
			if (Dats[i].OwnerScript == lastCalledScript)
			{
				Dats.RemoveAt(i);
			}
		}
	}

	private void ProcessIntervalCallbacks(int DeltaMS)
	{
		if (!GeneralUtils.m_InGame || !TimeManager.Instance || !TimeManager.Instance.m_NormalTimeEnabled)
		{
			return;
		}
		foreach (int item in new List<int>(IntervalCounters.Keys))
		{
			IntervalCounters[item] -= DeltaMS;
			if (IntervalCounters[item] > 0)
			{
				continue;
			}
			ModTimer.IntervalCallbackData intervalCallbackData = IntervalCallbacks[item];
			DynValue[] args = new DynValue[2]
			{
				DynValue.NewNumber(Math.Abs(DeltaMS)),
				DynValue.NewNumber(item)
			};
			Callback(intervalCallbackData.OwnerScript, intervalCallbackData.CallbackFunction, args);
			if (!intervalCallbackData.Repeat)
			{
				IntervalCounters.Remove(item);
				IntervalCallbacks.Remove(item);
			}
			else
			{
				while (IntervalCounters[item] <= 0)
				{
					IntervalCounters[item] += intervalCallbackData.IntervalMS;
				}
			}
		}
	}
}
