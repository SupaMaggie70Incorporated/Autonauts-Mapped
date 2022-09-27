using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using UnityEngine;

[MoonSharpUserData]
public class ModBuilding : ModCustom
{
	public Dictionary<ObjectType, Vector2Int> ModCoordsTL;

	public Dictionary<ObjectType, Vector2Int> ModCoordsBR;

	public Dictionary<ObjectType, Vector2Int> ModCoordsAccess;

	public Dictionary<ObjectType, bool> ModAccessNotHidden;

	protected Dictionary<ObjectType, bool> IsWalkable;

	public override void Init()
	{
		base.Init();
		ModCoordsTL = new Dictionary<ObjectType, Vector2Int>();
		ModCoordsBR = new Dictionary<ObjectType, Vector2Int>();
		ModCoordsAccess = new Dictionary<ObjectType, Vector2Int>();
		IsWalkable = new Dictionary<ObjectType, bool>();
		ModAccessNotHidden = new Dictionary<ObjectType, bool>();
	}

	public override string GetPrefabLocation()
	{
		return "WorldObjects/Buildings/ModBuilding";
	}

	public override ObjectSubCategory GetSubcategory()
	{
		return ObjectSubCategory.BuildingsMisc;
	}

	public bool IsItWalkable(ObjectType TypeToCheck)
	{
		return IsWalkable.ContainsKey(TypeToCheck);
	}

	public void CreateBuilding(string UniqueName, string[] NewIngredientsStringArr, int[] NewIngredientsAmountArr, string ModelName = "", int[] TL = null, int[] BR = null, int[] Access = null, bool UsingCustomModel = true)
	{
		if (UniqueName.Length == 0)
		{
			string descriptionOverride = "Error: ModBuilding.CreateBuilding '" + UniqueName + "' - Unique Name is null length";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
			return;
		}
		if (NewIngredientsStringArr != null && NewIngredientsStringArr.Length != NewIngredientsAmountArr.Length)
		{
			string descriptionOverride2 = "Error: ModBuilding.CreateBuilding '" + UniqueName + "' - Ingredients and Ingredient amounts not equal";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride2);
			return;
		}
		if (ModelName.Length == 0)
		{
			UsingCustomModel = false;
		}
		if (UsingCustomModel)
		{
			ModelName = ModelName.Replace("\\", "\\").Replace("/", "\\").ToLower();
		}
		if (!GeneralUtils.m_InGame)
		{
			if (ModManager.Instance.GetModObjectTypeFromName(UniqueName) != 0)
			{
				string descriptionOverride3 = "Error: ModBuilding.CreateBuilding '" + UniqueName + "' - already used this name!";
				ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride3);
				return;
			}
			ObjectType objectType = ObjectTypeList.m_Total + ModManager.Instance.CustomCreations;
			ModIDOriginals.Add(objectType, UniqueName);
			IsEnabled.Add(objectType, value: true);
			HasSetIngredients.Add(objectType, value: false);
			HasSetRecipe.Add(objectType, value: false);
			if (ModelName.Length == 0)
			{
				ModelName = "Models/Buildings/BlockWall";
			}
			ModModels.Add(objectType, ModelName);
			ModModelsCustom.Add(objectType, UsingCustomModel);
			ModManager.Instance.AddModString(objectType, UniqueName);
			ModCoordsTL.Add(objectType, (TL != null && TL.Length > 1) ? new Vector2Int(TL[0], TL[1]) : new Vector2Int(0, -1));
			ModCoordsBR.Add(objectType, (BR != null && BR.Length > 1) ? new Vector2Int(BR[0], BR[1]) : new Vector2Int(1, 0));
			ModCoordsAccess.Add(objectType, (Access != null && Access.Length > 1) ? new Vector2Int(Access[0], Access[1]) : new Vector2Int(-1, 0));
			if (DebugInfo)
			{
				Debug.Log("ADDED NEW BUILDING CALLED " + UniqueName + " (" + UniqueName + ")  ObjID " + objectType);
			}
			ModManager.Instance.CustomCreations++;
			Mod lastCalledMod = ModManager.Instance.GetLastCalledMod();
			if (lastCalledMod != null)
			{
				lastCalledMod.CustomIDs.Add(objectType);
				return;
			}
			string descriptionOverride4 = "Error: ModBuilding.CreateBuilding - Cannot find Lua Script";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride4);
			return;
		}
		ObjectType modObjectTypeFromName = ModManager.Instance.GetModObjectTypeFromName(UniqueName);
		ObjectTypeList.Instance.EnableCustomItem(modObjectTypeFromName, GetSubcategory());
		if (!HasSetIngredients[modObjectTypeFromName] && NewIngredientsStringArr != null)
		{
			IngredientRequirement[] array = new IngredientRequirement[NewIngredientsStringArr.Length];
			for (int i = 0; i < NewIngredientsStringArr.Length; i++)
			{
				ObjectType result = ObjectType.Nothing;
				if (!Enum.TryParse<ObjectType>(NewIngredientsStringArr[i], out result))
				{
					result = ModManager.Instance.GetModObjectTypeFromName(NewIngredientsStringArr[i]);
					if (result == ObjectType.Nothing)
					{
						string descriptionOverride5 = "Error: ModBuilding.CreateBuilding - Object Ingredient '" + NewIngredientsStringArr[i] + "' - cannot be found";
						ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride5);
						return;
					}
				}
				int count = NewIngredientsAmountArr[i];
				array[i] = new IngredientRequirement(result, count);
			}
			ObjectTypeList.Instance.SetIngredients(modObjectTypeFromName, array);
			HasSetIngredients[modObjectTypeFromName] = true;
		}
		VariableManager.Instance.SetVariable(modObjectTypeFromName, "Unlocked", 1);
		VariableManager.Instance.SetVariable(modObjectTypeFromName, "ConversionDelay", 2f);
		VariableManager.Instance.SetVariable(modObjectTypeFromName, "BuildDelay", 1f);
	}

	public bool IsBuildingActuallyFlooring(int UID)
	{
		try
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
			if (objectFromUniqueID != null)
			{
				return Floor.GetIsTypeFloor(objectFromUniqueID.m_TypeIdentifier);
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModBuilding.IsBuildingActuallyFlooring Error: " + ex.ToString());
		}
		return false;
	}

	public bool IsBuildingActuallyFlooring(string UID)
	{
		return false;
	}

	public int[] GetBuildingUIDsOfType(string NewTypeString, int StartX, int StartY, int EndX, int EndY)
	{
		try
		{
			if (StartX < 0 || StartY < 0 || StartX >= TileManager.Instance.m_TilesWide || StartY >= TileManager.Instance.m_TilesHigh)
			{
				return new int[0];
			}
			if (EndX < 0 || EndY < 0 || EndX >= TileManager.Instance.m_TilesWide || EndY >= TileManager.Instance.m_TilesHigh)
			{
				return new int[0];
			}
			TileCoord topLeftTile = new TileCoord(StartX, StartY);
			TileCoord bottomRightTile = new TileCoord(EndX, EndY);
			ObjectType result = ObjectType.Nothing;
			if (!Enum.TryParse<ObjectType>(NewTypeString, out result))
			{
				result = ModManager.Instance.GetModObjectTypeFromName(NewTypeString);
			}
			if (ObjectTypeList.Instance.GetIsBuilding(result))
			{
				List<TileCoordObject> buildingsInArea = PlotManager.Instance.GetBuildingsInArea(result, topLeftTile, bottomRightTile);
				int[] array = new int[buildingsInArea.Count];
				int num = 0;
				foreach (TileCoordObject item in buildingsInArea)
				{
					array[num++] = item.m_UniqueID;
				}
				return array;
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModBuilding.GetBuildingUIDsOfType Error: " + ex.ToString());
		}
		return new int[0];
	}

	public int[] GetAllBuildingsUIDsOfType(string NewTypeString, int StartX, int StartY, int EndX, int EndY)
	{
		if (StartX < 0 || StartY < 0 || StartX >= TileManager.Instance.m_TilesWide || StartY >= TileManager.Instance.m_TilesHigh)
		{
			return null;
		}
		if (EndX < 0 || EndY < 0 || EndX >= TileManager.Instance.m_TilesWide || EndY >= TileManager.Instance.m_TilesHigh)
		{
			return null;
		}
		int[] array = null;
		ObjectType result = ObjectType.Nothing;
		if (!Enum.TryParse<ObjectType>(NewTypeString, out result))
		{
			result = ModManager.Instance.GetModObjectTypeFromName(NewTypeString);
		}
		return ObjectTypeList.Instance.GetIsBuilding(result) ? GetBuildingUIDsOfType(NewTypeString, StartX, StartY, EndX, EndY) : new int[1] { -1 };
	}

	public int[] GetBuildingsUIDsRequiringIngredientInArea(string IngredientString, int StartX, int StartY, int EndX, int EndY)
	{
		List<int> list = new List<int>();
		try
		{
			if (StartX < 0 || StartY < 0 || StartX >= TileManager.Instance.m_TilesWide || StartY >= TileManager.Instance.m_TilesHigh)
			{
				return new int[0];
			}
			if (EndX < 0 || EndY < 0 || EndX >= TileManager.Instance.m_TilesWide || EndY >= TileManager.Instance.m_TilesHigh)
			{
				return new int[0];
			}
			TileCoord topLeftTile = new TileCoord(StartX, StartY);
			TileCoord bottomRightTile = new TileCoord(EndX, EndY);
			ObjectType result = ObjectType.Nothing;
			if (!Enum.TryParse<ObjectType>(IngredientString, out result))
			{
				result = ModManager.Instance.GetModObjectTypeFromName(IngredientString);
			}
			if (result == ObjectType.Nothing)
			{
				string descriptionOverride = "Error: ModBuilding.GetBuildingsUIDsRequiringIngredientInArea - Ingredient '" + IngredientString + "' - cannot be found";
				ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
				return new int[0];
			}
			for (int i = 0; i < (int)ObjectTypeList.m_Total; i++)
			{
				ObjectType objectType = (ObjectType)i;
				if (!Converter.GetIsTypeConverter(objectType) || !ObjectTypeList.Instance.GetIsBuilding(objectType))
				{
					continue;
				}
				foreach (TileCoordObject item in PlotManager.Instance.GetBuildingsInArea(objectType, topLeftTile, bottomRightTile))
				{
					Converter component = item.GetComponent<Converter>();
					if (!(component != null) || component.m_ResultsToCreate == 0)
					{
						continue;
					}
					foreach (IngredientRequirement item2 in component.m_Requirements[component.m_ResultsToCreate])
					{
						if (item2.m_Type == result)
						{
							list.Add(item.m_UniqueID);
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModBuilding.GetBuildingsUIDsRequiringIngredientInArea Error: " + ex.ToString());
		}
		int[] array = new int[list.Count];
		int num = 0;
		foreach (int item3 in list)
		{
			array[num++] = item3;
		}
		return array;
	}

	public void SetBuildingWalkable(string NewTypeString, bool CanBeWalkedThrough)
	{
		ObjectType result = ObjectType.Nothing;
		if (!Enum.TryParse<ObjectType>(NewTypeString, out result))
		{
			result = ModManager.Instance.GetModObjectTypeFromName(NewTypeString);
		}
		if (result == ObjectType.Nothing)
		{
			string descriptionOverride = "Error: ModBuilding.SetBuildingWalkable '" + NewTypeString + "' - Not Found";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
			return;
		}
		if (IsWalkable.ContainsKey(result))
		{
			IsWalkable.Remove(result);
		}
		if (CanBeWalkedThrough)
		{
			IsWalkable.Add(result, value: true);
		}
	}

	public int GetBuildingCoveringTile(int PosX, int PosY, bool AllowFlooring = false, bool AllowWalls = false, bool AllowFootprintTiles = false)
	{
		try
		{
			TileCoord position = new TileCoord(PosX, PosY);
			if (!position.GetIsValid())
			{
				return -1;
			}
			Tile tile = TileManager.Instance.GetTile(position);
			if (tile.m_Building != null)
			{
				if (!AllowFlooring && tile.m_Building.GetComponent<Floor>() != null)
				{
					return -1;
				}
				if (!AllowWalls && tile.m_Building.GetComponent<Wall>() != null)
				{
					return -1;
				}
				return tile.m_Building.m_UniqueID;
			}
			if (AllowFootprintTiles && tile.m_BuildingFootprint != null)
			{
				if (!AllowFlooring && tile.m_BuildingFootprint.GetComponent<Floor>() != null)
				{
					return -1;
				}
				if (!AllowWalls && tile.m_BuildingFootprint.GetComponent<Wall>() != null)
				{
					return -1;
				}
				return tile.m_BuildingFootprint.m_UniqueID;
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModBuilding.GetBuildingCoveringTile Error: " + ex.ToString());
		}
		return -1;
	}

	public Table GetBuildingUIDsByName(string DesiredName)
	{
		Table table = new Table(ModManager.Instance.GetLastCalledScript());
		try
		{
			foreach (KeyValuePair<BaseClass, int> item in CollectionManager.Instance.GetCollection("Building"))
			{
				Building component = item.Key.GetComponent<Building>();
				if (component.GetHumanReadableName().Equals(DesiredName))
				{
					table.Append(DynValue.NewNumber(component.m_UniqueID));
				}
			}
			return table;
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModBuilding.GetBuildingUIDsByName Error: " + ex.ToString());
			return table;
		}
	}

	public Table GetAllBuildingsUIDsFromName(string DesiredName)
	{
		Table buildingUIDsByName = GetBuildingUIDsByName(DesiredName);
		if (buildingUIDsByName.Length == 0)
		{
			return new Table(ModManager.Instance.GetLastCalledScript(), DynValue.NewNumber(-1.0));
		}
		return buildingUIDsByName;
	}

	public bool AddEnergy(int UID, float EnergyAmount, bool SetToMax = false)
	{
		try
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
			if (objectFromUniqueID != null && LinkedSystemEngine.GetIsTypeLinkedSystemEngine(objectFromUniqueID.m_TypeIdentifier))
			{
				LinkedSystemEngine component = objectFromUniqueID.GetComponent<LinkedSystemEngine>();
				if (component != null)
				{
					float energy = component.GetEnergy();
					float num = component.m_EnergyCapacity;
					if (energy >= num)
					{
						return false;
					}
					if (EnergyAmount + energy > num || SetToMax)
					{
						component.AddEnergy((int)(num - energy));
					}
					else
					{
						component.AddEnergy((int)EnergyAmount);
					}
					return true;
				}
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModBuilding.AddEnergy Error: " + ex.ToString());
		}
		return false;
	}

	public bool AddEnergy(string UID, float EnergyAmount, bool SetToMax = false)
	{
		return false;
	}

	public bool AddWater(int UID, float WaterAmount, bool SetToMax = false)
	{
		try
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
			if (objectFromUniqueID != null)
			{
				TrainRefuellingStation component = objectFromUniqueID.GetComponent<TrainRefuellingStation>();
				if (component != null)
				{
					float water = component.m_Water;
					float maxWater = TrainRefuellingStation.m_MaxWater;
					if (water >= maxWater)
					{
						return false;
					}
					if (WaterAmount + water > maxWater || SetToMax)
					{
						component.AddWater((int)(maxWater - water));
					}
					else
					{
						component.AddWater((int)WaterAmount);
					}
					return true;
				}
				StationaryEngine component2 = objectFromUniqueID.GetComponent<StationaryEngine>();
				if (component2 != null)
				{
					float water2 = component2.m_Water;
					float waterCapacity = component2.m_WaterCapacity;
					if (water2 >= waterCapacity)
					{
						return false;
					}
					if (WaterAmount + water2 > waterCapacity || SetToMax)
					{
						component2.AddWater((int)(waterCapacity - water2));
					}
					else
					{
						component2.AddWater((int)WaterAmount);
					}
					return true;
				}
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModBuilding.AddWater Error: " + ex.ToString());
		}
		return false;
	}

	public bool AddWater(string UID, float WaterAmount, bool SetToMax = false)
	{
		return false;
	}

	public bool AddHay(int UID, float HayAmount, bool SetToMax = false)
	{
		try
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
			if (objectFromUniqueID != null)
			{
				Trough component = objectFromUniqueID.GetComponent<Trough>();
				if (component != null)
				{
					float hay = component.m_Hay;
					float capacity = component.m_Capacity;
					if (hay >= capacity)
					{
						return false;
					}
					if (HayAmount + hay > capacity || SetToMax)
					{
						component.ModAddHay((int)(capacity - hay));
					}
					else
					{
						component.ModAddHay((int)HayAmount);
					}
					return true;
				}
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModBuilding.AddHay Error: " + ex.ToString());
		}
		return false;
	}

	public bool AddHay(string UID, float WaterAmount, bool SetToMax = false)
	{
		return false;
	}

	public bool AddFuel(int UID, float FuelAmount, bool SetToMax = false)
	{
		try
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
			if (objectFromUniqueID != null)
			{
				TrainRefuellingStation component = objectFromUniqueID.GetComponent<TrainRefuellingStation>();
				if (component != null)
				{
					float fuel = component.m_Fuel;
					float maxFuel = TrainRefuellingStation.m_MaxFuel;
					if (fuel >= maxFuel)
					{
						return false;
					}
					if (FuelAmount + fuel > maxFuel || SetToMax)
					{
						component.AddFuel((int)(maxFuel - fuel));
					}
					else
					{
						component.AddFuel((int)FuelAmount);
					}
					return true;
				}
				StationaryEngine component2 = objectFromUniqueID.GetComponent<StationaryEngine>();
				if (component2 != null)
				{
					float num = component2.m_Energy;
					float num2 = component2.m_EnergyCapacity;
					if (num >= num2)
					{
						return false;
					}
					if (FuelAmount + num > num2 || SetToMax)
					{
						component2.AddEnergy((int)(num2 - num));
					}
					else
					{
						component2.AddEnergy((int)FuelAmount);
					}
					return true;
				}
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModBuilding.AddFuel Error: " + ex.ToString());
		}
		return false;
	}

	public bool AddFuel(string UID, float FuelAmount, bool SetToMax = false)
	{
		return false;
	}

	public float GetFuelMaxCapacity(int UID)
	{
		try
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
			if (objectFromUniqueID != null)
			{
				if (objectFromUniqueID.GetComponent<TrainRefuellingStation>() != null)
				{
					return TrainRefuellingStation.m_MaxFuel;
				}
				StationaryEngine component = objectFromUniqueID.GetComponent<StationaryEngine>();
				if (component != null)
				{
					return component.m_EnergyCapacity;
				}
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModBuilding.GetFuelMaxCapacity Error: " + ex.ToString());
		}
		return 0f;
	}

	public float GetFuelMaxCapacity(string UID)
	{
		return 0f;
	}

	public void ShowBuildingAccessPoint(string NewTypeString, bool EnableAccessPoint)
	{
		ObjectType result = ObjectType.Nothing;
		if (!Enum.TryParse<ObjectType>(NewTypeString, out result))
		{
			result = ModManager.Instance.GetModObjectTypeFromName(NewTypeString);
		}
		if (result == ObjectType.Nothing)
		{
			string descriptionOverride = "Error: ModBuilding.ShowBuildingAccessPoint '" + NewTypeString + "' - Not Found";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
			return;
		}
		if (ModAccessNotHidden.ContainsKey(result))
		{
			ModAccessNotHidden.Remove(result);
		}
		if (EnableAccessPoint)
		{
			ModAccessNotHidden.Add(result, value: true);
		}
	}

	private Building GetBuildingFromUID(int UID)
	{
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
		if (objectFromUniqueID != null)
		{
			Building component = objectFromUniqueID.GetComponent<Building>();
			if (component != null)
			{
				return component;
			}
		}
		return null;
	}

	public bool SetBuildingName(int UID, string BuildingName)
	{
		try
		{
			Building buildingFromUID = GetBuildingFromUID(UID);
			if (buildingFromUID != null)
			{
				buildingFromUID.SetName(BuildingName);
				return true;
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModBuilding.SetBuildingName Error: " + ex.ToString());
		}
		return false;
	}

	public bool SetBuildingName(string UID, string BuildingName)
	{
		return false;
	}

	public bool SetRotation(int UID, int Rotation)
	{
		try
		{
			Building buildingFromUID = GetBuildingFromUID(UID);
			if (buildingFromUID != null)
			{
				MapManager.Instance.RemoveBuilding(buildingFromUID);
				buildingFromUID.SetRotation(Rotation % 4);
				MapManager.Instance.AddBuilding(buildingFromUID);
				return true;
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModBuilding.SetRotation Error: " + ex.ToString());
		}
		return false;
	}

	public bool SetRotation(string UID, int Rotation)
	{
		return false;
	}

	public int GetRotation(int UID)
	{
		try
		{
			Building buildingFromUID = GetBuildingFromUID(UID);
			if (buildingFromUID != null)
			{
				return buildingFromUID.m_Rotation;
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModBuilding.GetRotation Error: " + ex.ToString());
		}
		return -1;
	}

	public int GetRotation(string UID)
	{
		return -1;
	}

	public bool IsBuildingSaveable(int UID)
	{
		try
		{
			Building buildingFromUID = GetBuildingFromUID(UID);
			if (buildingFromUID == null)
			{
				return false;
			}
			if (buildingFromUID.GetType().GetProperty("m_ParentBuilding") != null)
			{
				return buildingFromUID.m_ParentBuilding.GetIsSavable();
			}
			return buildingFromUID.GetIsSavable();
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModBuilding.IsBuildingSaveable Error: " + ex.ToString());
		}
		return false;
	}

	public bool IsBuildingSaveable(string UID)
	{
		return false;
	}

	public void RegisterForBuildingEditedCallback(int BuildingUID, DynValue Callback)
	{
		if (!(GetBuildingFromUID(BuildingUID) == null))
		{
			if (!ModManager.Instance.BuildingEditedCallbacks.ContainsKey(BuildingUID))
			{
				ModManager.Instance.BuildingEditedCallbacks.Add(BuildingUID, new List<ModManager.MinimalCallbackData>());
			}
			List<ModManager.MinimalCallbackData> Dats = ModManager.Instance.BuildingEditedCallbacks[BuildingUID];
			ModManager.Instance.AddOrOverwriteCallbackInList(ref Dats, Callback);
		}
	}

	public void RegisterForBuildingEditedCallback(string BuildingUID, DynValue Callback)
	{
	}

	public void UnregisterForBuildingEditedCallback(int BuildingUID)
	{
		if (ModManager.Instance.BuildingEditedCallbacks.ContainsKey(BuildingUID))
		{
			ModManager.Instance.BuildingEditedCallbacks.Remove(BuildingUID);
		}
	}

	public void UnregisterForBuildingEditedCallback(string BuildingUID)
	{
	}

	public void RegisterForNewBuildingInAreaCallback(int StartX, int StartY, int EndX, int EndY, DynValue Callback)
	{
		if (StartX < 0 || StartY < 0 || StartX >= TileManager.Instance.m_TilesWide || StartY >= TileManager.Instance.m_TilesHigh || EndX < 0 || EndY < 0 || EndX >= TileManager.Instance.m_TilesWide || EndY >= TileManager.Instance.m_TilesHigh)
		{
			return;
		}
		for (int i = StartX; i <= EndX; i++)
		{
			for (int j = StartY; j <= EndY; j++)
			{
				int key = j * TileManager.Instance.m_TilesWide + i;
				if (!ModManager.Instance.NewBuildingInAreaCallbacks.ContainsKey(key))
				{
					ModManager.Instance.NewBuildingInAreaCallbacks.Add(key, new List<ModManager.MinimalCallbackData>());
				}
				List<ModManager.MinimalCallbackData> Dats = ModManager.Instance.NewBuildingInAreaCallbacks[key];
				ModManager.Instance.AddOrOverwriteCallbackInList(ref Dats, Callback);
			}
		}
	}

	public void UnregisterForNewBuildingInAreaCallback(int StartX, int StartY, int EndX, int EndY)
	{
		for (int i = StartX; i < EndX; i++)
		{
			for (int j = StartY; j < EndY; j++)
			{
				int key = j * TileManager.Instance.m_TilesWide + i;
				if (ModManager.Instance.NewBuildingInAreaCallbacks.ContainsKey(key))
				{
					List<ModManager.MinimalCallbackData> Dats = ModManager.Instance.NewBuildingInAreaCallbacks[key];
					ModManager.Instance.RemoveCallbackFromList(ref Dats);
					if (Dats.Count == 0)
					{
						ModManager.Instance.NewBuildingInAreaCallbacks.Remove(key);
					}
				}
			}
		}
	}

	public void RegisterForBuildingTypeSpawnedCallback(string NewTypeString, DynValue Callback)
	{
		ObjectType result = ObjectType.Nothing;
		if (!Enum.TryParse<ObjectType>(NewTypeString, out result))
		{
			result = ModManager.Instance.GetModObjectTypeFromName(NewTypeString);
		}
		if (result == ObjectType.Nothing)
		{
			string descriptionOverride = "Error: ModBuilding.RegisterForBuildingTypeSpawnedCallback: type '" + NewTypeString + "' - Not Found";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
			return;
		}
		if (!ModManager.Instance.BuildingTypeSpawnedCallbacks.ContainsKey(result))
		{
			ModManager.Instance.BuildingTypeSpawnedCallbacks.Add(result, new List<ModManager.MinimalCallbackData>());
		}
		List<ModManager.MinimalCallbackData> Dats = ModManager.Instance.BuildingTypeSpawnedCallbacks[result];
		ModManager.Instance.AddOrOverwriteCallbackInList(ref Dats, Callback);
	}

	public DynValue[][] GetBuildingRequirements(int UID)
	{
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID);
		if (objectFromUniqueID != null)
		{
			List<DynValue[]> list = new List<DynValue[]>();
			Converter component = objectFromUniqueID.GetComponent<Converter>();
			Housing component2 = objectFromUniqueID.GetComponent<Housing>();
			TrainRefuellingStation component3 = objectFromUniqueID.GetComponent<TrainRefuellingStation>();
			StationaryEngine component4 = objectFromUniqueID.GetComponent<StationaryEngine>();
			Trough component5 = objectFromUniqueID.GetComponent<Trough>();
			Fueler component6 = objectFromUniqueID.GetComponent<Fueler>();
			ResearchStation component7 = objectFromUniqueID.GetComponent<ResearchStation>();
			StoneHeads component8 = objectFromUniqueID.GetComponent<StoneHeads>();
			SpacePort component9 = objectFromUniqueID.GetComponent<SpacePort>();
			if (component8 != null)
			{
				list.Add(new DynValue[4]
				{
					DynValue.NewString("*"),
					DynValue.NewNumber(1.0),
					DynValue.NewNumber(0.0),
					DynValue.NewString("Ingredient")
				});
				return list.ToArray();
			}
			if (component9 != null)
			{
				if (OffworldMissionsManager.Instance.m_SelectedMission != null)
				{
					List<IngredientRequirement> requirements = OffworldMissionsManager.Instance.m_SelectedMission.m_Requirements;
					List<int> progress = OffworldMissionsManager.Instance.m_SelectedMission.m_Progress;
					if (requirements != null)
					{
						for (int i = 0; i < requirements.Count; i++)
						{
							list.Add(new DynValue[4]
							{
								DynValue.NewString(requirements[i].m_Type.ToString()),
								DynValue.NewNumber(requirements[i].m_Count),
								DynValue.NewNumber(progress[i]),
								DynValue.NewString("Ingredient")
							});
						}
					}
				}
				return list.ToArray();
			}
			if (component6 != null)
			{
				foreach (KeyValuePair<ObjectType, float> fuel2 in BurnableFuel.m_Fuels)
				{
					if (component6.IsObjectTypeAcceptableFuel(fuel2.Key))
					{
						list.Add(new DynValue[4]
						{
							DynValue.NewString(fuel2.Key.ToString()),
							DynValue.NewNumber(component6.m_Capacity / fuel2.Value),
							DynValue.NewNumber(component6.m_Fuel / fuel2.Value),
							DynValue.NewString("Fuel")
						});
					}
				}
			}
			if (component2 != null)
			{
				list.Add(new DynValue[4]
				{
					DynValue.NewString(component2.GetRepairTypeRequired().ToString()),
					DynValue.NewNumber(component2.GetRepairAmountRequired()),
					DynValue.NewNumber(component2.GetRepairAmountDone()),
					DynValue.NewString("Ingredient")
				});
			}
			if (component3 != null)
			{
				list.Add(new DynValue[4]
				{
					DynValue.NewString("Water"),
					DynValue.NewNumber(TrainRefuellingStation.m_MaxWater),
					DynValue.NewNumber(component3.m_Water),
					DynValue.NewString("Water")
				});
				float maxFuel = TrainRefuellingStation.m_MaxFuel;
				float fuel = component3.m_Fuel;
				if (BurnableFuel.m_Fuels.Count == 0)
				{
					BurnableFuel.PrecacheVariables();
				}
				foreach (KeyValuePair<ObjectType, float> fuel3 in BurnableFuel.m_Fuels)
				{
					if (Train.IsObjectTypeAcceptableFuel(fuel3.Key))
					{
						float value = fuel3.Value;
						float num = fuel / value;
						float num2 = maxFuel / value;
						list.Add(new DynValue[4]
						{
							DynValue.NewString(fuel3.Key.ToString()),
							DynValue.NewNumber(num2),
							DynValue.NewNumber(num),
							DynValue.NewString("Fuel")
						});
					}
				}
			}
			if (component4 != null)
			{
				list.Add(new DynValue[4]
				{
					DynValue.NewString("Water"),
					DynValue.NewNumber(component4.m_WaterCapacity),
					DynValue.NewNumber(component4.m_Water),
					DynValue.NewString("Water")
				});
				float num3 = component4.m_EnergyCapacity;
				float num4 = component4.m_Energy;
				foreach (KeyValuePair<ObjectType, float> fuel4 in BurnableFuel.m_Fuels)
				{
					if (StationaryEngine.GetIsObjectAcceptableAsFuel(fuel4.Key))
					{
						float value2 = fuel4.Value;
						float num5 = num4 / value2;
						float num6 = num3 / value2;
						list.Add(new DynValue[4]
						{
							DynValue.NewString(fuel4.Key.ToString()),
							DynValue.NewNumber(num6),
							DynValue.NewNumber(num5),
							DynValue.NewString("Fuel")
						});
					}
				}
			}
			if (component7 != null && component7.m_CurrentResearchQuest != Quest.ID.Total)
			{
				Quest quest = QuestManager.Instance.GetQuest(component7.m_CurrentResearchQuest);
				if (component7.m_CurrentResearchObject == null)
				{
					list.Add(new DynValue[4]
					{
						DynValue.NewString(quest.m_ObjectTypeRequired.ToString()),
						DynValue.NewNumber(1.0),
						DynValue.NewNumber(0.0),
						DynValue.NewString("Ingredient")
					});
				}
				int progress2 = quest.m_EventsRequired[0].m_Progress;
				int required = quest.m_EventsRequired[0].m_Required;
				foreach (ObjectType type2 in FolkHeart.m_Types)
				{
					int variableAsInt = VariableManager.Instance.GetVariableAsInt(type2, "Value");
					list.Add(new DynValue[4]
					{
						DynValue.NewString(type2.ToString()),
						DynValue.NewNumber(required / variableAsInt),
						DynValue.NewNumber(progress2 / variableAsInt),
						DynValue.NewString("Heart")
					});
				}
			}
			if (component5 != null)
			{
				float capacity = component5.m_Capacity;
				float hay = component5.m_Hay;
				foreach (KeyValuePair<ObjectType, float> fuel5 in BurnableFuel.m_Fuels)
				{
					if (Trough.GetIsObjectAcceptable(fuel5.Key))
					{
						float value3 = fuel5.Value;
						float num7 = hay / value3;
						float num8 = capacity / value3;
						list.Add(new DynValue[4]
						{
							DynValue.NewString(fuel5.Key.ToString()),
							DynValue.NewNumber(num8),
							DynValue.NewNumber(num7),
							DynValue.NewString("Hay")
						});
					}
				}
			}
			if (component != null)
			{
				int resultsToCreate = component.m_ResultsToCreate;
				List<IngredientRequirement> list2 = component.m_Requirements[resultsToCreate];
				if (list2 != null)
				{
					for (int j = 0; j < list2.Count; j++)
					{
						ObjectType type = list2[j].m_Type;
						list.Add(new DynValue[4]
						{
							DynValue.NewString(type.ToString()),
							DynValue.NewNumber(list2[j].m_Count),
							DynValue.NewNumber(component.GetIngredientCount(type)),
							DynValue.NewString("Ingredient")
						});
					}
				}
			}
			return list.ToArray();
		}
		string descriptionOverride = "Error: ModBuilding.BuildingRequirements UID: '" + UID + "' - Not Found";
		ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
		return null;
	}

	public void RegisterForBuildingStateChangedCallback(int BuildingUID, DynValue Callback)
	{
		if (!(GetBuildingFromUID(BuildingUID) == null))
		{
			if (!ModManager.Instance.BuildingStateChangedCallbacks.ContainsKey(BuildingUID))
			{
				ModManager.Instance.BuildingStateChangedCallbacks.Add(BuildingUID, new List<ModManager.MinimalCallbackData>());
			}
			List<ModManager.MinimalCallbackData> Dats = ModManager.Instance.BuildingStateChangedCallbacks[BuildingUID];
			ModManager.Instance.AddOrOverwriteCallbackInList(ref Dats, Callback);
		}
	}
}
