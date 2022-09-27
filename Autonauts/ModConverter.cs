using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using UnityEngine;

[MoonSharpUserData]
public class ModConverter : ModCustom
{
	public Dictionary<ObjectType, Vector2Int> ModCoordsTL;

	public Dictionary<ObjectType, Vector2Int> ModCoordsBR;

	public Dictionary<ObjectType, Vector2Int> ModCoordsAccess;

	public Dictionary<ObjectType, Vector2Int> ModCoordsSpawn;

	public override void Init()
	{
		base.Init();
		ModCoordsTL = new Dictionary<ObjectType, Vector2Int>();
		ModCoordsBR = new Dictionary<ObjectType, Vector2Int>();
		ModCoordsAccess = new Dictionary<ObjectType, Vector2Int>();
		ModCoordsSpawn = new Dictionary<ObjectType, Vector2Int>();
	}

	public override string GetPrefabLocation()
	{
		return "WorldObjects/Buildings/Converters/ModConverter";
	}

	public override ObjectSubCategory GetSubcategory()
	{
		return ObjectSubCategory.BuildingsWorkshop;
	}

	public void CreateConverter(string UniqueName, string[] RecipeStringArr, string[] NewIngredientsStringArr, int[] NewIngredientsAmountArr, string ModelName = "", int[] TL = null, int[] BR = null, int[] Access = null, int[] Spawn = null, bool UsingCustomModel = true)
	{
		if (UniqueName.Length == 0)
		{
			string descriptionOverride = "Error: ModConverter.CreateConverter '" + UniqueName + "' - Unique Name is null length";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
			return;
		}
		if (NewIngredientsStringArr == null || NewIngredientsAmountArr == null || NewIngredientsStringArr.Length != NewIngredientsAmountArr.Length)
		{
			string descriptionOverride2 = "Error: ModConverter.CreateConverter '" + UniqueName + "' - Ingredients and Ingredient amounts not equal";
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
				string descriptionOverride3 = "Error: ModConverter.CreateConverter '" + UniqueName + "' - already used this name!";
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
				ModelName = "Models/Buildings/Converters/Workbench";
			}
			ModModels.Add(objectType, ModelName);
			ModModelsCustom.Add(objectType, UsingCustomModel);
			ModManager.Instance.AddModString(objectType, UniqueName);
			ModCoordsTL.Add(objectType, (TL != null && TL.Length > 1) ? new Vector2Int(TL[0], TL[1]) : new Vector2Int(0, -1));
			ModCoordsBR.Add(objectType, (BR != null && BR.Length > 1) ? new Vector2Int(BR[0], BR[1]) : new Vector2Int(1, 0));
			ModCoordsAccess.Add(objectType, (Access != null && Access.Length > 1) ? new Vector2Int(Access[0], Access[1]) : new Vector2Int(-1, 0));
			ModCoordsSpawn.Add(objectType, (Spawn != null && Spawn.Length > 1) ? new Vector2Int(Spawn[0], Spawn[1]) : new Vector2Int(2, 0));
			if (DebugInfo)
			{
				Debug.Log("ADDED NEW CONVERTER CALLED " + UniqueName + " (" + UniqueName + ")  ObjID " + objectType);
			}
			ModManager.Instance.CustomCreations++;
			Mod lastCalledMod = ModManager.Instance.GetLastCalledMod();
			if (lastCalledMod != null)
			{
				lastCalledMod.CustomIDs.Add(objectType);
				return;
			}
			string descriptionOverride4 = "Error: ModConverter.CreateConverter - Cannot find Lua Script";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride4);
			return;
		}
		ObjectType modObjectTypeFromName = ModManager.Instance.GetModObjectTypeFromName(UniqueName);
		ObjectTypeList.Instance.EnableCustomItem(modObjectTypeFromName, GetSubcategory());
		if (!HasSetIngredients[modObjectTypeFromName])
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
						string descriptionOverride5 = "Error: ModConverter.CreateConverter - Object Ingredient '" + NewIngredientsStringArr[i] + "' - cannot be found";
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
		if (!HasSetRecipe[modObjectTypeFromName])
		{
			for (int j = 0; j < RecipeStringArr.Length; j++)
			{
				ObjectType result2 = ObjectType.Nothing;
				if (Enum.TryParse<ObjectType>(RecipeStringArr[j], ignoreCase: true, out result2))
				{
					VariableManager.Instance.m_DataConverters.Add(modObjectTypeFromName, result2);
					HasSetRecipe[modObjectTypeFromName] = true;
				}
				else
				{
					result2 = ModManager.Instance.GetModObjectTypeFromName(RecipeStringArr[j]);
					if (result2 != 0)
					{
						VariableManager.Instance.m_DataConverters.Add(modObjectTypeFromName, result2);
						HasSetRecipe[modObjectTypeFromName] = true;
					}
				}
				if (result2 == ObjectType.Nothing)
				{
					string descriptionOverride6 = "Error: ModConverter.CreateConverter '" + RecipeStringArr[j] + "' - Cannot find this object to make. Check spelling and load order.";
					ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride6);
					return;
				}
			}
		}
		VariableManager.Instance.SetVariable(modObjectTypeFromName, "Unlocked", 1);
		VariableManager.Instance.SetVariable(modObjectTypeFromName, "ConversionDelay", 2f);
		VariableManager.Instance.SetVariable(modObjectTypeFromName, "BuildDelay", 1f);
	}

	public bool AddIngredientToSpecifiedConverter(int UID, string IngredientString)
	{
		try
		{
			ObjectType result = ObjectType.Nothing;
			if (!Enum.TryParse<ObjectType>(IngredientString, out result))
			{
				result = ModManager.Instance.GetModObjectTypeFromName(IngredientString);
			}
			if (result == ObjectType.Nothing)
			{
				string descriptionOverride = "Error: ModConverter.AddIngredientToSpecifiedConverter - Ingredient '" + IngredientString + "' - cannot be found";
				ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
				return false;
			}
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
			if (objectFromUniqueID != null && Converter.GetIsTypeConverter(objectFromUniqueID.m_TypeIdentifier))
			{
				Converter component = objectFromUniqueID.GetComponent<Converter>();
				if (component != null && component.m_ResultsToCreate != 0 && component.CanAcceptIngredient(result) && component.GetAreRequirementsMet())
				{
					Holdable component2 = ObjectTypeList.Instance.CreateObjectFromIdentifier(result, new TileCoord(0, 0).ToWorldPosition(), Quaternion.identity).GetComponent<Holdable>();
					if (component2 != null)
					{
						component2.UpdatePositionToTilePosition(component.m_TileCoord);
						component2.GetComponent<Savable>().SetIsSavable(IsSavable: false);
						component.AddIngredient(component2);
						component2.SendAction(new ActionInfo(ActionType.BeingHeld, default(TileCoord), component));
						component.UpdateModIngredients();
						if (component.m_State == Converter.State.Idle && component.AreRequrementsMet())
						{
							component.StartConversion(null);
						}
						return true;
					}
				}
			}
			if (objectFromUniqueID == null)
			{
				return false;
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModConverter.AddIngredientToSpecifiedConverter Error: " + ex.ToString());
		}
		return false;
	}

	public bool AddIngredientToSpecifiedConverter(string UID, string IngredientString)
	{
		return false;
	}

	public bool AddFuelToSpecifiedConverter(int UID, float Fuel)
	{
		try
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
			if (objectFromUniqueID != null && Converter.GetIsTypeConverter(objectFromUniqueID.m_TypeIdentifier) && Fueler.GetIsTypeFueler(objectFromUniqueID.m_TypeIdentifier))
			{
				Fueler component = objectFromUniqueID.GetComponent<Fueler>();
				if (component != null)
				{
					if (component.m_Fuel + Fuel > component.m_Capacity)
					{
						return false;
					}
					component.SetFuel(component.m_Fuel + Fuel);
					component.ModFuelChanged();
					if (component.m_State == Converter.State.Idle && component.AreRequrementsMet())
					{
						component.StartConverting();
						component.m_State = Converter.State.Converting;
					}
					return true;
				}
			}
			if (objectFromUniqueID == null)
			{
				return false;
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModConverter.AddFuelToSpecifiedConverter Error: " + ex.ToString());
		}
		return false;
	}

	public bool AddFuelToSpecifiedConverter(string UID, float Fuel)
	{
		return false;
	}

	public float GetFuelQuantity(int UID)
	{
		try
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
			if (objectFromUniqueID != null && Converter.GetIsTypeConverter(objectFromUniqueID.m_TypeIdentifier) && Fueler.GetIsTypeFueler(objectFromUniqueID.m_TypeIdentifier))
			{
				Fueler component = objectFromUniqueID.GetComponent<Fueler>();
				if (component != null)
				{
					return component.m_Fuel;
				}
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModConverter.GetFuelQuantity Error: " + ex.ToString());
		}
		return -1f;
	}

	public float GetFuelQuantity(string UID)
	{
		return 0f;
	}

	public float GetFuelMaxCapacity(int UID)
	{
		try
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
			if (objectFromUniqueID != null && Converter.GetIsTypeConverter(objectFromUniqueID.m_TypeIdentifier) && Fueler.GetIsTypeFueler(objectFromUniqueID.m_TypeIdentifier))
			{
				Fueler component = objectFromUniqueID.GetComponent<Fueler>();
				if (component != null)
				{
					return component.m_Capacity;
				}
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModConverter.GetFuelMaxCapacity Error: " + ex.ToString());
		}
		return -1f;
	}

	public float GetFuelMaxCapacity(string UID)
	{
		return 0f;
	}

	public bool AreConverterRequrementsMet(int UID)
	{
		try
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
			if (objectFromUniqueID != null && Converter.GetIsTypeConverter(objectFromUniqueID.m_TypeIdentifier))
			{
				Converter component = objectFromUniqueID.GetComponent<Converter>();
				if (component != null)
				{
					return component.m_State == Converter.State.Idle && component.AreRequrementsMet();
				}
			}
			if (objectFromUniqueID == null)
			{
				return false;
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModConverter.AreConverterRequrementsMet Error: " + ex.ToString());
		}
		return false;
	}

	public bool AreConverterRequrementsMet(string UID)
	{
		return false;
	}

	public Table GetConverterProperties(int UID)
	{
		Table table = new Table(ModManager.Instance.GetLastCalledScript());
		try
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
			if (objectFromUniqueID != null)
			{
				if (Converter.GetIsTypeConverter(objectFromUniqueID.m_TypeIdentifier))
				{
					Converter component = objectFromUniqueID.GetComponent<Converter>();
					if (component != null)
					{
						table.Append(DynValue.NewString(component.m_State.ToString()));
						table.Append(DynValue.NewNumber(component.m_TileCoord.x));
						table.Append(DynValue.NewNumber(component.m_TileCoord.y));
						if (objectFromUniqueID.m_TypeIdentifier == ObjectType.ConverterFoundation)
						{
							table.Append(DynValue.NewNumber(objectFromUniqueID.GetComponent<ConverterFoundation>().m_NewBuilding.m_ModelRoot.gameObject.transform.rotation.eulerAngles.y));
						}
						else
						{
							table.Append(DynValue.NewNumber(objectFromUniqueID.m_ModelRoot.gameObject.transform.rotation.eulerAngles.y));
						}
						table.Append(DynValue.NewString(objectFromUniqueID.GetHumanReadableName()));
						table.Append(DynValue.NewBoolean(component.m_State == Converter.State.Idle && component.AreRequrementsMet()));
						table.Append(DynValue.NewNumber(component.m_TileCoord.x + component.m_SpawnPoint.x));
						table.Append(DynValue.NewNumber(component.m_TileCoord.y + component.m_SpawnPoint.y));
						table.Append(DynValue.NewNumber(component.m_TileCoord.x + component.m_AccessPoint.x));
						table.Append(DynValue.NewNumber(component.m_TileCoord.y + component.m_AccessPoint.y));
						if ((bool)component.m_LastAddedIngredient)
						{
							if (component.m_LastAddedIngredient.m_TypeIdentifier >= ObjectType.Total)
							{
								table.Append(DynValue.NewString(ModManager.Instance.m_ModStrings[component.m_LastAddedIngredient.m_TypeIdentifier]));
							}
							else
							{
								table.Append(DynValue.NewString(component.m_LastAddedIngredient.m_TypeIdentifier.ToString()));
							}
						}
						else
						{
							table.Append(DynValue.NewNumber(-1.0));
						}
						table.Append(DynValue.NewNumber(GetFuelQuantity(UID)));
						table.Append(DynValue.NewNumber(GetFuelMaxCapacity(UID)));
						return table;
					}
					return table;
				}
				return table;
			}
			return table;
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModConverter.GetConverterProperties Error: " + ex.ToString());
			return new Table(ModManager.Instance.GetLastCalledScript());
		}
	}

	public Table GetConverterProperties(string UID)
	{
		return new Table(ModManager.Instance.GetLastCalledScript());
	}
}
