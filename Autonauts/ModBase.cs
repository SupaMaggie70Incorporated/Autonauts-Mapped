using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using UnityEngine;

[MoonSharpUserData]
public class ModBase
{
	public void SetSteamWorkshopDetails(string Title = "", string Description = "", string[] Tags = null, string ContentImage = "")
	{
		IList<string> list = new List<string>();
		for (int i = 0; i < Tags.Length; i++)
		{
			list.Add(Tags[i]);
		}
		Mod lastCalledMod = ModManager.Instance.GetLastCalledMod();
		if (lastCalledMod != null)
		{
			lastCalledMod.SetSteamWorkshopDetails(Title, Description, list, ContentImage);
			return;
		}
		string descriptionOverride = "Error: ModBase.SetSteamWorkshopDetails - Cannot find Lua Script";
		ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
	}

	public int SpawnItem(string Item = "Nothing", int NewX = 0, int NewY = 0, bool DoOnce = false, bool Instant = false, bool ForceBP = false)
	{
		try
		{
			if (Item.Equals("ConverterFoundation"))
			{
				return -1;
			}
			if (NewX < 0 || NewY < 0 || NewX >= TileManager.Instance.m_TilesWide || NewY >= TileManager.Instance.m_TilesHigh)
			{
				return -1;
			}
			ObjectType result = ObjectType.Nothing;
			if (!Enum.TryParse<ObjectType>(Item, out result))
			{
				result = ModManager.Instance.GetModObjectTypeFromName(Item);
			}
			if (ObjectTypeList.Instance.GetSubCategoryFromType(result) == ObjectSubCategory.Hidden && result != ObjectType.Boulder && result != ObjectType.TallBoulder && result != ObjectType.TreeStump && result != ObjectType.WorkerFrameMk0 && result != ObjectType.WorkerHeadMk0 && result != ObjectType.WorkerDriveMk0)
			{
				string descriptionOverride = "Error: ModBase.SpawnItem '" + Item + "' - Item not found";
				ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
				return -1;
			}
			if (result == ObjectType.Nothing)
			{
				string descriptionOverride2 = "Error: ModBase.SpawnItem '" + Item + "' - Item not found";
				ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride2);
				return -1;
			}
			if (DoOnce && !ModManager.Instance.InMainUpdateState && CheckForAlreadySpawned(result, NewX, NewY, Item))
			{
				return -1;
			}
			TileCoord tilePosition = new TileCoord(NewX, NewY);
			Vector3 position = tilePosition.ToWorldPositionTileCentered();
			if (ToolFillable.GetIsTypeLiquid(result))
			{
				BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.ToolBucket, position, Quaternion.identity);
				int capacity = baseClass.GetComponent<ToolFillable>().m_Capacity;
				baseClass.GetComponent<ToolFillable>().Fill(result, capacity);
				return baseClass.m_UniqueID;
			}
			if (ObjectTypeList.Instance.GetIsBuilding(result))
			{
				return BuildingManager.Instance.AddBuilding(tilePosition, result, 0, null, Instant, ForceBP).m_UniqueID;
			}
			BaseClass baseClass2 = ObjectTypeList.Instance.CreateObjectFromIdentifier(result, position, Quaternion.identity);
			if (result == ObjectType.CropWheat)
			{
				baseClass2.GetComponent<CropWheat>().SetState(Crop.State.Wild);
			}
			if (result == ObjectType.FlowerWild)
			{
				int type = UnityEngine.Random.Range(0, 7);
				baseClass2.GetComponent<FlowerWild>().SetType((FlowerWild.Type)type);
			}
			return baseClass2.m_UniqueID;
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModBase.SpawnItem Error: " + ex.ToString());
		}
		return -1;
	}

	public int SpawnLiquid(string LiquidItem, string FillableItem = "ToolBucket", int NewX = 0, int NewY = 0)
	{
		try
		{
			if (NewX < 0 || NewY < 0 || NewX >= TileManager.Instance.m_TilesWide || NewY >= TileManager.Instance.m_TilesHigh)
			{
				return -1;
			}
			ObjectType result = ObjectType.Nothing;
			if (!Enum.TryParse<ObjectType>(LiquidItem, out result))
			{
				result = ModManager.Instance.GetModObjectTypeFromName(LiquidItem);
			}
			TileCoord tileCoord = new TileCoord(NewX, NewY);
			if (!tileCoord.GetIsValid())
			{
				return -1;
			}
			Vector3 position = tileCoord.ToWorldPositionTileCentered();
			if (ToolFillable.GetIsTypeLiquid(result))
			{
				ObjectType result2 = ObjectType.ToolBucket;
				if (!Enum.TryParse<ObjectType>(FillableItem, out result2))
				{
					result2 = ModManager.Instance.GetModObjectTypeFromName(FillableItem);
				}
				if (ToolFillable.GetIsTypeFillable(result2))
				{
					BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(result2, position, Quaternion.identity);
					int capacity = baseClass.GetComponent<ToolFillable>().m_Capacity;
					baseClass.GetComponent<ToolFillable>().Fill(result, capacity);
					return baseClass.m_UniqueID;
				}
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModBase.SpawnLiquid Error: " + ex.ToString());
		}
		return -1;
	}

	public void DisableSafety(bool Disable)
	{
		ModManager.Instance.FailSafeDisabled = Disable;
	}

	public void ExposeVariable(string UniqueName, DynValue DefaultValue, DynValue Callback, DynValue Min, DynValue Max)
	{
		Mod lastCalledMod = ModManager.Instance.GetLastCalledMod();
		if (lastCalledMod != null)
		{
			lastCalledMod.AddExposedVariable(UniqueName, DefaultValue, Callback, Min, Max);
			return;
		}
		string descriptionOverride = "Error: ModBase.ExposeVariable - Cannot find Lua Script";
		ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
	}

	public void ExposeVariableList(string UniqueName, DynValue[] DefaultOptions, int DefaultSelectedOption, DynValue Callback)
	{
		Mod lastCalledMod = ModManager.Instance.GetLastCalledMod();
		if (lastCalledMod != null)
		{
			lastCalledMod.AddExposedVariableList(UniqueName, DefaultOptions, DefaultSelectedOption, Callback);
			return;
		}
		string descriptionOverride = "Error: ModBase.ExposeVariableList - Cannot find Lua Script";
		ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
	}

	public void ExposeKeybinding(string UniqueName, int Key, DynValue Callback)
	{
		if (Key == 0 || Key > 10)
		{
			string descriptionOverride = "Error: ModBase.ExposeKeybinding - Using a Key outside of limits (1-10 only)";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
			return;
		}
		Mod lastCalledMod = ModManager.Instance.GetLastCalledMod();
		if (lastCalledMod != null)
		{
			lastCalledMod.AddExposedKeybinding(UniqueName, Key, Callback);
			return;
		}
		string descriptionOverride2 = "Error: ModBase.ExposeKeybinding - Cannot find Lua Script";
		ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride2);
	}

	public void RegisterForInputPress(DynValue Callback)
	{
		Mod lastCalledMod = ModManager.Instance.GetLastCalledMod();
		if (lastCalledMod != null)
		{
			lastCalledMod.RegisterForInputPress(Callback);
			return;
		}
		string descriptionOverride = "Error: ModBase.RegisterForInputPress - Cannot find Lua Script";
		ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
	}

	public void RegisterForInputMouseButtonDown(DynValue Callback)
	{
		Mod lastCalledMod = ModManager.Instance.GetLastCalledMod();
		if (lastCalledMod != null)
		{
			lastCalledMod.RegisterForInputMouseDown(Callback);
			return;
		}
		string descriptionOverride = "Error: ModBase.RegisterForInputMouseButtonDown - Cannot find Lua Script";
		ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
	}

	public DynValue GetExposedVariable(string UniqueName)
	{
		try
		{
			Mod lastCalledMod = ModManager.Instance.GetLastCalledMod();
			if (lastCalledMod == null)
			{
				string descriptionOverride = "Error: ModBase.GetExposedVariable - Cannot find Lua Script";
				ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
				return null;
			}
			foreach (ModManager.ExposedData exposedVar in lastCalledMod.ExposedVars)
			{
				if (exposedVar.VarName.Equals(UniqueName))
				{
					return exposedVar.VarValue;
				}
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModBase.GetExposedVariable Error: " + ex.ToString());
		}
		return null;
	}

	public bool IsNightTime()
	{
		if ((bool)DayNightManager.Instance)
		{
			return DayNightManager.Instance.m_MoonActive > 0.5f;
		}
		return false;
	}

	public string GetGameState()
	{
		try
		{
			if ((bool)GameStateManager.Instance)
			{
				return GameStateManager.Instance.GetActualState().ToString();
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModBase.GetGameState Error: " + ex.ToString());
		}
		return null;
	}

	public string GetGameVersion()
	{
		return SaveLoadManager.m_Version;
	}

	public int GetGameVersionPatch()
	{
		string text = GetGameVersion().Replace(GetGameVersionMajor().ToString(), "");
		int num = text.IndexOf(".");
		int num2 = text.LastIndexOf(".");
		return int.Parse(text.Remove(num, num2 - num + 1));
	}

	public int GetGameVersionMinor()
	{
		string text = GetGameVersion().Replace(GetGameVersionMajor().ToString(), "").TrimStart('.');
		int num = text.IndexOf(".");
		if (num > 0)
		{
			return int.Parse(text.Remove(num));
		}
		return int.Parse(text);
	}

	public int GetGameVersionMajor()
	{
		string gameVersion = GetGameVersion();
		int startIndex = gameVersion.IndexOf(".");
		return int.Parse(gameVersion.Remove(startIndex));
	}

	public bool IsGameVersionGreaterThanEqualTo(string DesiredVersion)
	{
		int startIndex = DesiredVersion.IndexOf(".");
		int num = int.Parse(DesiredVersion.Remove(startIndex));
		string text = DesiredVersion.Replace(num.ToString(), "").TrimStart('.');
		int num2 = text.IndexOf(".");
		int num3 = 0;
		num3 = ((num2 <= 0) ? int.Parse(text) : int.Parse(text.Remove(num2)));
		string text2 = DesiredVersion.Replace(num.ToString(), "");
		int num4 = text2.IndexOf(".");
		int num5 = text2.LastIndexOf(".");
		int num6 = int.Parse(text2.Remove(num4, num5 - num4 + 1));
		if (GetGameVersionMajor() > num)
		{
			return true;
		}
		if (GetGameVersionMajor() == num)
		{
			if (GetGameVersionMinor() > num3)
			{
				return true;
			}
			if (GetGameVersionMinor() == num3 && GetGameVersionPatch() >= num6)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsRaining()
	{
		try
		{
			if ((bool)RainManager.Instance)
			{
				return RainManager.Instance.GetIsRaining();
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModBase.IsRaining Error: " + ex.ToString());
		}
		return false;
	}

	public bool ClassAndMethodExist(string ClassName, string MethodName)
	{
		Type type = Type.GetType(ClassName);
		if (type == null)
		{
			return false;
		}
		Activator.CreateInstance(type);
		return type.GetMethod(MethodName) != null;
	}

	public void RegisterForItemTypeSpawnedCallback(string NewTypeString, DynValue Callback)
	{
		ObjectType result = ObjectType.Nothing;
		if (!Enum.TryParse<ObjectType>(NewTypeString, out result))
		{
			result = ModManager.Instance.GetModObjectTypeFromName(NewTypeString);
		}
		if (result == ObjectType.Nothing)
		{
			string descriptionOverride = "Error: ModBase.RegisterForItemTypeSpawnedCallback: type '" + NewTypeString + "' - Not Found";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
			return;
		}
		if (!ModManager.Instance.ItemTypeSpawnedCallbacks.ContainsKey(result))
		{
			ModManager.Instance.ItemTypeSpawnedCallbacks.Add(result, new List<ModManager.MinimalCallbackData>());
		}
		List<ModManager.MinimalCallbackData> Dats = ModManager.Instance.ItemTypeSpawnedCallbacks[result];
		ModManager.Instance.AddOrOverwriteCallbackInList(ref Dats, Callback);
	}

	public void UnregisterForItemTypeSpawnedCallback(string NewTypeString)
	{
		ObjectType result = ObjectType.Nothing;
		if (!Enum.TryParse<ObjectType>(NewTypeString, out result))
		{
			result = ModManager.Instance.GetModObjectTypeFromName(NewTypeString);
		}
		if (result == ObjectType.Nothing)
		{
			string descriptionOverride = "Error: ModBase.RegisterForItemTypeSpawnedCallback: type '" + NewTypeString + "' - Not Found";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
		}
		else if (ModManager.Instance.ItemTypeSpawnedCallbacks.ContainsKey(result))
		{
			List<ModManager.MinimalCallbackData> Dats = ModManager.Instance.ItemTypeSpawnedCallbacks[result];
			ModManager.Instance.RemoveCallbackFromList(ref Dats);
			if (Dats.Count == 0)
			{
				ModManager.Instance.ItemTypeSpawnedCallbacks.Remove(result);
			}
		}
	}

	private bool CheckForAlreadySpawned(ObjectType ObjectToMake, int NewX, int NewY, string Item)
	{
		string spawnsInfo = ModManager.Instance.SpawnsInfo;
		if (spawnsInfo.Length > 0)
		{
			string text = spawnsInfo;
			int num = 0;
			ObjectType result = ObjectType.Nothing;
			int num2 = 0;
			int num3 = 0;
			while (true)
			{
				int num4 = text.IndexOf('-');
				if (num4 == -1)
				{
					break;
				}
				switch (num)
				{
				case 0:
				{
					string text2 = text.Substring(0, num4);
					num++;
					if (!Enum.TryParse<ObjectType>(text2, out result))
					{
						ObjectToMake = ModManager.Instance.GetModObjectTypeFromName(text2);
					}
					break;
				}
				case 1:
					num2 = Convert.ToInt32(text.Substring(0, num4));
					num++;
					break;
				case 2:
					num3 = Convert.ToInt32(text.Substring(0, num4));
					num = 0;
					if (ObjectToMake == result && num2 == NewX && num3 == NewY)
					{
						return true;
					}
					break;
				}
				text = text.Substring(num4 + 1);
			}
			ModManager.Instance.UpdateSaveSpawnsInfo(Item, NewX, NewY);
		}
		return false;
	}
}
