using System;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public class ModVariable
{
	public void SetVariable(string Name, string String)
	{
		VariableManager.Instance.SetVariable(Name, String);
	}

	public int GetVariableAsInt(string Name)
	{
		return VariableManager.Instance.GetVariableAsInt(Name, CheckValid: false);
	}

	public float GetVariableAsFloat(string Name)
	{
		return VariableManager.Instance.GetVariableAsFloat(Name, CheckValid: false);
	}

	public string GetVariableAsString(string Name)
	{
		return VariableManager.Instance.GetVariableAsString(Name, CheckValid: false);
	}

	public void SetVariable(string Name, int Int)
	{
		VariableManager.Instance.SetVariable(Name, Int);
	}

	public void SetVariable(string Name, float Float)
	{
		VariableManager.Instance.SetVariable(Name, Float);
	}

	private string GetVariableFarmerActionAsName(string ActionString, string TargetTypeString, string ToolTypeString)
	{
		try
		{
			Farmer.State action = (Farmer.State)Enum.Parse(typeof(Farmer.State), ActionString);
			ObjectType result = ObjectType.Nothing;
			if (!Enum.TryParse<ObjectType>(TargetTypeString, out result))
			{
				result = ModManager.Instance.GetModObjectTypeFromName(TargetTypeString);
			}
			ObjectType result2 = ObjectType.Nothing;
			if (!Enum.TryParse<ObjectType>(ToolTypeString, out result2))
			{
				result2 = ModManager.Instance.GetModObjectTypeFromName(ToolTypeString);
			}
			return VariableManager.Instance.GetVariableName(action, result, result2);
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModVariable.GetVariableFarmerActionAsName Error: " + ex.ToString());
		}
		return "";
	}

	public void SetVariableFarmerAction(string ActionString, string TargetTypeString, string ToolTypeString, int Int)
	{
		Farmer.State action = (Farmer.State)Enum.Parse(typeof(Farmer.State), ActionString);
		ObjectType result = ObjectType.Nothing;
		if (!Enum.TryParse<ObjectType>(TargetTypeString, out result))
		{
			result = ModManager.Instance.GetModObjectTypeFromName(TargetTypeString);
		}
		ObjectType result2 = ObjectType.Nothing;
		if (!Enum.TryParse<ObjectType>(ToolTypeString, out result2))
		{
			result2 = ModManager.Instance.GetModObjectTypeFromName(ToolTypeString);
		}
		VariableManager.Instance.SetVariable(action, result, result2, Int);
	}

	public int GetVariableFarmerActionAsInt(string ActionString, string TargetTypeString, string ToolTypeString)
	{
		try
		{
			Farmer.State action = (Farmer.State)Enum.Parse(typeof(Farmer.State), ActionString);
			ObjectType result = ObjectType.Nothing;
			if (!Enum.TryParse<ObjectType>(TargetTypeString, out result))
			{
				result = ModManager.Instance.GetModObjectTypeFromName(TargetTypeString);
			}
			ObjectType result2 = ObjectType.Nothing;
			if (!Enum.TryParse<ObjectType>(ToolTypeString, out result2))
			{
				result2 = ModManager.Instance.GetModObjectTypeFromName(ToolTypeString);
			}
			return VariableManager.Instance.GetVariableAsInt(action, result, result2, CheckValid: false);
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModVariable.GetVariableFarmerActionAsInt Error: " + ex.ToString());
		}
		return 0;
	}

	public string GetVariableFarmerActionOnTilesAsName(string ActionString, string TileTypeString, string ToolTypeString)
	{
		try
		{
			Farmer.State action = (Farmer.State)Enum.Parse(typeof(Farmer.State), ActionString);
			Tile.TileType newType = (Tile.TileType)Enum.Parse(typeof(Tile.TileType), TileTypeString);
			ObjectType result = ObjectType.Nothing;
			if (!Enum.TryParse<ObjectType>(ToolTypeString, out result))
			{
				result = ModManager.Instance.GetModObjectTypeFromName(ToolTypeString);
			}
			return VariableManager.Instance.GetVariableName(action, newType, result);
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModVariable.GetVariableFarmerActionOnTilesAsName Error: " + ex.ToString());
		}
		return "";
	}

	public void SetVariableFarmerActionOnTiles(string ActionString, string TileTypeString, string ToolTypeString, int Int)
	{
		Farmer.State action = (Farmer.State)Enum.Parse(typeof(Farmer.State), ActionString);
		Tile.TileType newType = (Tile.TileType)Enum.Parse(typeof(Tile.TileType), TileTypeString);
		ObjectType result = ObjectType.Nothing;
		if (!Enum.TryParse<ObjectType>(ToolTypeString, out result))
		{
			result = ModManager.Instance.GetModObjectTypeFromName(ToolTypeString);
		}
		VariableManager.Instance.SetVariable(action, newType, result, Int);
	}

	public int GetVariableFarmerActionOnTilesAsInt(string ActionString, string TileTypeString, string ToolTypeString)
	{
		try
		{
			Farmer.State action = (Farmer.State)Enum.Parse(typeof(Farmer.State), ActionString);
			Tile.TileType newType = (Tile.TileType)Enum.Parse(typeof(Tile.TileType), TileTypeString);
			ObjectType result = ObjectType.Nothing;
			if (!Enum.TryParse<ObjectType>(ToolTypeString, out result))
			{
				result = ModManager.Instance.GetModObjectTypeFromName(ToolTypeString);
			}
			return VariableManager.Instance.GetVariableAsInt(action, newType, result, CheckValid: false);
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModVariable.GetVariableFarmerActionOnTilesAsInt Error: " + ex.ToString());
		}
		return 0;
	}

	private string GetVariableForObjectAsName(string NewTypeString, string VariableName)
	{
		ObjectType result = ObjectType.Nothing;
		if (!Enum.TryParse<ObjectType>(NewTypeString, out result))
		{
			result = ModManager.Instance.GetModObjectTypeFromName(NewTypeString);
		}
		return VariableManager.Instance.GetVariableName(result, VariableName);
	}

	public int GetVariableForObjectAsInt(string NewTypeString, string VariableName)
	{
		try
		{
			ObjectType result = ObjectType.Nothing;
			if (!Enum.TryParse<ObjectType>(NewTypeString, out result))
			{
				result = ModManager.Instance.GetModObjectTypeFromName(NewTypeString);
			}
			return VariableManager.Instance.GetVariableAsInt(result, VariableName, CheckValid: false);
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModVariable.GetVariableForObjectAsInt Error: " + ex.ToString());
		}
		return 0;
	}

	public float GetVariableForObjectAsFloat(string NewTypeString, string VariableName)
	{
		try
		{
			ObjectType result = ObjectType.Nothing;
			if (!Enum.TryParse<ObjectType>(NewTypeString, out result))
			{
				result = ModManager.Instance.GetModObjectTypeFromName(NewTypeString);
			}
			return VariableManager.Instance.GetVariableAsFloat(result, VariableName, CheckValid: false);
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModVariable.GetVariableForObjectAsFloat Error: " + ex.ToString());
		}
		return 0f;
	}

	public string GetVariableForObjectAsString(string NewTypeString, string VariableName)
	{
		try
		{
			ObjectType result = ObjectType.Nothing;
			if (!Enum.TryParse<ObjectType>(NewTypeString, out result))
			{
				result = ModManager.Instance.GetModObjectTypeFromName(NewTypeString);
			}
			return VariableManager.Instance.GetVariableAsString(result, VariableName, CheckValid: false);
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModVariable.GetVariableForObjectAsString Error: " + ex.ToString());
		}
		return "";
	}

	public void SetVariableForObjectAsInt(string NewTypeString, string VariableName, int Int)
	{
		ObjectType result = ObjectType.Nothing;
		if (!Enum.TryParse<ObjectType>(NewTypeString, out result))
		{
			result = ModManager.Instance.GetModObjectTypeFromName(NewTypeString);
		}
		VariableManager.Instance.SetVariable(result, VariableName, Int);
	}

	public void SetVariableForObjectAsIntFromString(string NewTypeString, string VariableName, string StringInt)
	{
		ObjectType result = ObjectType.Nothing;
		if (!Enum.TryParse<ObjectType>(NewTypeString, out result))
		{
			result = ModManager.Instance.GetModObjectTypeFromName(NewTypeString);
		}
		ObjectType result2 = ObjectType.Nothing;
		if (!Enum.TryParse<ObjectType>(StringInt, out result2))
		{
			result2 = ModManager.Instance.GetModObjectTypeFromName(StringInt);
		}
		VariableManager.Instance.SetVariable(result, VariableName, (int)result2);
	}

	public void SetVariableForObjectAsFloat(string NewTypeString, string VariableName, float Float)
	{
		ObjectType result = ObjectType.Nothing;
		if (!Enum.TryParse<ObjectType>(NewTypeString, out result))
		{
			result = ModManager.Instance.GetModObjectTypeFromName(NewTypeString);
		}
		VariableManager.Instance.SetVariable(result, VariableName, Float);
	}

	public void SetVariableForObjectAsString(string NewTypeString, string VariableName, string String)
	{
		ObjectType result = ObjectType.Nothing;
		if (!Enum.TryParse<ObjectType>(NewTypeString, out result))
		{
			result = ModManager.Instance.GetModObjectTypeFromName(NewTypeString);
		}
		VariableManager.Instance.SetVariable(result, VariableName, String);
	}

	public void SetVariableForStorageAmount(string NewTypeString, int Int)
	{
		ObjectType result = ObjectType.Nothing;
		if (!Enum.TryParse<ObjectType>(NewTypeString, out result))
		{
			result = ModManager.Instance.GetModObjectTypeFromName(NewTypeString);
		}
		if (StorageTypeManager.m_StorageGenericInformation.ContainsKey(result))
		{
			StorageTypeManager.m_StorageGenericInformation.Remove(result);
			StorageTypeManager.m_StorageGenericInformation.Add(result, new StorageTypeManager.StorageGenericInfo(Int));
		}
		else if (StorageTypeManager.m_StoragePaletteInformation.ContainsKey(result))
		{
			StorageTypeManager.m_StoragePaletteInformation.Remove(result);
			StorageTypeManager.m_StoragePaletteInformation.Add(result, new StorageTypeManager.StorageGenericInfo(Int));
		}
		else
		{
			string descriptionOverride = "Error: ModVariable.SetVariableForStorageAmount '" + NewTypeString + "' - Object Type Not Found In Storage";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
		}
	}

	public void SetVariableForBuildingUpgrade(string ObjectTypeFromString, string ObjectTypeToString)
	{
		ObjectType result = ObjectType.Nothing;
		if (!Enum.TryParse<ObjectType>(ObjectTypeFromString, out result))
		{
			result = ModManager.Instance.GetModObjectTypeFromName(ObjectTypeFromString);
		}
		ObjectType result2 = ObjectType.Nothing;
		if (!Enum.TryParse<ObjectType>(ObjectTypeToString, out result2))
		{
			result2 = ModManager.Instance.GetModObjectTypeFromName(ObjectTypeToString);
		}
		if (result2 == ObjectType.Nothing || result == ObjectType.Nothing)
		{
			string descriptionOverride = "Error: ModVariable.SetVariableForBuildingUpgrade '" + ObjectTypeFromString + "' or '" + ObjectTypeToString + "' - Object Type Not Recognised";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
		}
		else
		{
			VariableManager.Instance.SetVariable(result, "UpgradeTo", (int)result2);
			VariableManager.Instance.SetVariable(result2, "UpgradeFrom", (int)result);
		}
	}

	public void SetIngredientsForRecipe(string NewObjectResultString, string[] NewIngredientsStringArr, int[] NewIngredientsAmountArr, int ResultAmount)
	{
		SetIngredientsForRecipeSpecific("Nothing", NewObjectResultString, NewIngredientsStringArr, NewIngredientsAmountArr, ResultAmount);
	}

	public void SetIngredientsForRecipeSpecific(string NewConverterString, string NewObjectResultString, string[] NewIngredientsStringArr = null, int[] NewIngredientsAmountArr = null, int ResultAmount = 1)
	{
		if (NewIngredientsStringArr == null || NewIngredientsAmountArr == null || NewIngredientsStringArr.Length != NewIngredientsAmountArr.Length)
		{
			string descriptionOverride = "Error: ModVariable.SetIngredientsForRecipe '" + NewConverterString + "' - Ingredients and Ingredient amounts not equal";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
			return;
		}
		ObjectType result = ObjectType.Nothing;
		if (!Enum.TryParse<ObjectType>(NewConverterString, out result))
		{
			result = ModManager.Instance.GetModObjectTypeFromName(NewConverterString);
			if (result == ObjectType.Nothing)
			{
				string descriptionOverride2 = "Error: ModVariable.SetIngredientsForRecipeSpecific - Converter '" + NewConverterString + "' - cannot be found";
				ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride2);
				return;
			}
		}
		ObjectType result2 = ObjectType.Nothing;
		if (!Enum.TryParse<ObjectType>(NewObjectResultString, out result2))
		{
			result2 = ModManager.Instance.GetModObjectTypeFromName(NewObjectResultString);
			if (result2 == ObjectType.Nothing)
			{
				string descriptionOverride3 = "Error: ModVariable.SetIngredientsForRecipe - Object '" + NewObjectResultString + "' - cannot be found";
				ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride3);
				return;
			}
		}
		if (NewIngredientsStringArr != null && NewIngredientsAmountArr != null)
		{
			IngredientRequirement[] array = new IngredientRequirement[NewIngredientsStringArr.Length];
			for (int i = 0; i < NewIngredientsStringArr.Length; i++)
			{
				ObjectType result3 = ObjectType.Nothing;
				if (!Enum.TryParse<ObjectType>(NewIngredientsStringArr[i], out result3))
				{
					result3 = ModManager.Instance.GetModObjectTypeFromName(NewIngredientsStringArr[i]);
					if (result3 == ObjectType.Nothing)
					{
						string descriptionOverride4 = "Error: ModVariable.SetIngredientsForRecipe - Object Ingredient '" + NewIngredientsStringArr[i] + "' - cannot be found";
						ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride4);
						return;
					}
				}
				int count = NewIngredientsAmountArr[i];
				array[i] = new IngredientRequirement(result3, count);
			}
			ObjectTypeList.Instance.ReplaceIngredients(result2, array);
		}
		VariableManager.Instance.m_DataConverters.ReplaceResults(result, result2, ResultAmount);
	}

	public void SetIngredientsForRecipeSpecificDoubleResults(string NewConverterString, string NewObjectResultString1, string NewObjectResultString2, string[] NewIngredientsStringArr, int[] NewIngredientsAmountArr, int ResultAmount1 = 1, int ResultAmount2 = 1)
	{
		if (NewIngredientsStringArr == null || NewIngredientsAmountArr == null || NewIngredientsStringArr.Length != NewIngredientsAmountArr.Length)
		{
			string descriptionOverride = "Error: ModVariable.SetIngredientsForRecipeSpecificDouble '" + NewConverterString + "' - Ingredients and Ingredient amounts not equal";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
			return;
		}
		ObjectType result = ObjectType.Nothing;
		if (!Enum.TryParse<ObjectType>(NewConverterString, out result))
		{
			result = ModManager.Instance.GetModObjectTypeFromName(NewConverterString);
			if (result == ObjectType.Nothing)
			{
				string descriptionOverride2 = "Error: ModVariable.SetIngredientsForRecipeSpecificDoubleResults - Converter '" + NewConverterString + "' - cannot be found";
				ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride2);
				return;
			}
		}
		ObjectType result2 = ObjectType.Nothing;
		if (!Enum.TryParse<ObjectType>(NewObjectResultString1, out result2))
		{
			result2 = ModManager.Instance.GetModObjectTypeFromName(NewObjectResultString1);
			if (result2 == ObjectType.Nothing)
			{
				string descriptionOverride3 = "Error: ModVariable.SetIngredientsForRecipeSpecificDouble - Object '" + NewObjectResultString1 + "' - cannot be found";
				ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride3);
				return;
			}
		}
		ObjectType result3 = ObjectType.Nothing;
		if (!Enum.TryParse<ObjectType>(NewObjectResultString2, out result3))
		{
			result3 = ModManager.Instance.GetModObjectTypeFromName(NewObjectResultString2);
			if (result3 == ObjectType.Nothing)
			{
				string descriptionOverride4 = "Error: ModVariable.SetIngredientsForRecipeSpecificDouble - Object '" + NewObjectResultString2 + "' - cannot be found";
				ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride4);
				return;
			}
		}
		if (NewIngredientsStringArr != null && NewIngredientsAmountArr != null)
		{
			IngredientRequirement[] array = new IngredientRequirement[NewIngredientsStringArr.Length];
			for (int i = 0; i < NewIngredientsStringArr.Length; i++)
			{
				ObjectType result4 = ObjectType.Nothing;
				if (!Enum.TryParse<ObjectType>(NewIngredientsStringArr[i], out result4))
				{
					result4 = ModManager.Instance.GetModObjectTypeFromName(NewIngredientsStringArr[i]);
					if (result4 == ObjectType.Nothing)
					{
						string descriptionOverride5 = "Error: ModVariable.SetIngredientsForRecipeSpecificDouble - Object Ingredient '" + NewIngredientsStringArr[i] + "' - cannot be found";
						ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride5);
						return;
					}
				}
				int count = NewIngredientsAmountArr[i];
				array[i] = new IngredientRequirement(result4, count);
			}
			ObjectTypeList.Instance.ReplaceIngredients(result2, array);
		}
		VariableManager.Instance.m_DataConverters.ReplaceResultsDouble(result, result2, ResultAmount1, result3, ResultAmount2);
	}

	public string[] GetIngredientsForRecipe(string NewObjectResultString)
	{
		try
		{
			ObjectType result = ObjectType.Nothing;
			if (!Enum.TryParse<ObjectType>(NewObjectResultString, out result))
			{
				result = ModManager.Instance.GetModObjectTypeFromName(NewObjectResultString);
				if (result == ObjectType.Nothing)
				{
					string descriptionOverride = "Error: ModVariable.GetIngredientsForRecipe - Object '" + NewObjectResultString + "' - cannot be found";
					ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
					return new string[0];
				}
			}
			IngredientRequirement[] ingredientsFromIdentifier = ObjectTypeList.Instance.GetIngredientsFromIdentifier(result);
			string[] array = new string[ingredientsFromIdentifier.Length];
			for (int i = 0; i < ingredientsFromIdentifier.Length; i++)
			{
				if (ingredientsFromIdentifier[i].m_Type >= ObjectType.Total)
				{
					foreach (ModCustom modCustomClass in ModManager.Instance.ModCustomClasses)
					{
						if (modCustomClass.IsItCustomType(ingredientsFromIdentifier[i].m_Type))
						{
							string Name = "";
							if (modCustomClass.GetOriginalNameFromType(ingredientsFromIdentifier[i].m_Type, out Name))
							{
								array[i] = Name;
							}
							break;
						}
					}
				}
				else
				{
					array[i] = ingredientsFromIdentifier[i].m_Type.ToString();
				}
			}
			return array;
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModVariable.GetIngredientsForRecipe Error: " + ex.ToString());
		}
		return new string[0];
	}

	public int[] GetIngredientsAmountForRecipe(string NewObjectResultString)
	{
		try
		{
			ObjectType result = ObjectType.Nothing;
			if (!Enum.TryParse<ObjectType>(NewObjectResultString, out result))
			{
				result = ModManager.Instance.GetModObjectTypeFromName(NewObjectResultString);
				if (result == ObjectType.Nothing)
				{
					string descriptionOverride = "Error: ModVariable.GetIngredientsForRecipe - Object '" + NewObjectResultString + "' - cannot be found";
					ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
					return new int[0];
				}
			}
			IngredientRequirement[] ingredientsFromIdentifier = ObjectTypeList.Instance.GetIngredientsFromIdentifier(result);
			int[] array = new int[ingredientsFromIdentifier.Length];
			for (int i = 0; i < ingredientsFromIdentifier.Length; i++)
			{
				array[i] = ingredientsFromIdentifier[i].m_Count;
			}
			return array;
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModVariable.GetIngredientsAmountForRecipe Error: " + ex.ToString());
		}
		return new int[0];
	}

	public void AddRecipeToConverter(string NewConverterString, string NewObjectResultString, int ResultAmount = 1)
	{
		ObjectType result = ObjectType.Nothing;
		if (!Enum.TryParse<ObjectType>(NewConverterString, out result))
		{
			result = ModManager.Instance.GetModObjectTypeFromName(NewConverterString);
			if (result == ObjectType.Nothing)
			{
				string descriptionOverride = "Error: ModVariable.AddRecipeToConverter - Converter '" + NewConverterString + "' - cannot be found";
				ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
				return;
			}
		}
		ObjectType result2 = ObjectType.Nothing;
		if (!Enum.TryParse<ObjectType>(NewObjectResultString, out result2))
		{
			result2 = ModManager.Instance.GetModObjectTypeFromName(NewObjectResultString);
			if (result2 == ObjectType.Nothing)
			{
				string descriptionOverride2 = "Error: ModVariable.AddRecipeToConverter - Object '" + NewObjectResultString + "' - cannot be found";
				ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride2);
				return;
			}
		}
		VariableManager.Instance.m_DataConverters.ReplaceResults(result, result2, ResultAmount);
	}

	public bool RemoveRecipeFromConverter(string NewConverterString, string NewObjectResultString)
	{
		try
		{
			ObjectType result = ObjectType.Nothing;
			if (!Enum.TryParse<ObjectType>(NewConverterString, out result))
			{
				result = ModManager.Instance.GetModObjectTypeFromName(NewConverterString);
				if (result == ObjectType.Nothing)
				{
					string descriptionOverride = "Error: ModVariable.RemoveRecipeFromConverter - Converter '" + NewConverterString + "' - cannot be found";
					ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
					return false;
				}
			}
			ObjectType result2 = ObjectType.Nothing;
			if (!Enum.TryParse<ObjectType>(NewObjectResultString, out result2))
			{
				result2 = ModManager.Instance.GetModObjectTypeFromName(NewObjectResultString);
				if (result2 == ObjectType.Nothing)
				{
					string descriptionOverride2 = "Error: ModVariable.RemoveRecipeFromConverter - Object '" + NewObjectResultString + "' - cannot be found";
					ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride2);
					return false;
				}
			}
			return VariableManager.Instance.m_DataConverters.RemoveResults(result, result2);
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModVariable.RemoveRecipeFromConverter Error: " + ex.ToString());
		}
		return false;
	}
}
