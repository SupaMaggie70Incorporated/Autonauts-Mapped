using System;
using MoonSharp.Interpreter;
using UnityEngine;

[MoonSharpUserData]
public class ModGoTo : ModCustom
{
	public override void Init()
	{
		base.Init();
	}

	public override string GetPrefabLocation()
	{
		return "WorldObjects/Special/GoToMod";
	}

	public override ObjectSubCategory GetSubcategory()
	{
		return ObjectSubCategory.Misc;
	}

	public override bool GetStackable()
	{
		return true;
	}

	public void CreateGoTo(string UniqueName, string[] NewIngredientsStringArr = null, int[] NewIngredientsAmountArr = null, string ModelName = "", bool UsingCustomModel = true)
	{
		if (UniqueName.Length == 0)
		{
			string descriptionOverride = "Error: ModGoTo.CreateGoTo '" + UniqueName + "' - Unique Name is null length";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
			return;
		}
		if (NewIngredientsStringArr != null && NewIngredientsStringArr.Length != NewIngredientsAmountArr.Length)
		{
			string descriptionOverride2 = "Error: ModGoTo.CreateGoTo '" + UniqueName + "' - Ingredients and Ingredient amounts not equal";
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
				string descriptionOverride3 = "Error: ModGoTo.CreateGoTo '" + UniqueName + "' - already used this name!";
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
				ModelName = "Models/Vehicles/Minecart";
			}
			ModModels.Add(objectType, ModelName);
			ModModelsCustom.Add(objectType, UsingCustomModel);
			ModManager.Instance.AddModString(objectType, UniqueName);
			if (DebugInfo)
			{
				Debug.Log("ADDED NEW GOTO CALLED " + UniqueName + " (" + UniqueName + ")  ObjID " + objectType);
			}
			ModManager.Instance.CustomCreations++;
			Mod lastCalledMod = ModManager.Instance.GetLastCalledMod();
			if (lastCalledMod != null)
			{
				lastCalledMod.CustomIDs.Add(objectType);
				return;
			}
			string descriptionOverride4 = "Error: ModGoTo.CreateGoTo - Cannot find Lua Script";
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
						string descriptionOverride5 = "Error: ModGoTo.CreateGoTo - Object Ingredient '" + NewIngredientsStringArr[i] + "' - cannot be found";
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
	}

	public void MoveTo(int UID, int x, int y)
	{
		GoTo goTo = null;
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
		if (objectFromUniqueID != null)
		{
			goTo = objectFromUniqueID.GetComponent<GoTo>();
		}
		if (goTo != null && new TileCoord(x, y).GetIsValid())
		{
			goTo.RequestGoTo(new TileCoord(x, y));
		}
	}

	public void MoveTo(string UID, int x, int y)
	{
	}
}
