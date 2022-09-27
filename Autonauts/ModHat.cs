using System;
using MoonSharp.Interpreter;
using UnityEngine;

[MoonSharpUserData]
public class ModHat : ModCustom
{
	public override void Init()
	{
		base.Init();
	}

	public override string GetPrefabLocation()
	{
		return "WorldObjects/Clothes/Hats/HatMod";
	}

	public override ObjectSubCategory GetSubcategory()
	{
		return ObjectSubCategory.ClothingHeadwear;
	}

	public override bool GetStackable()
	{
		return true;
	}

	public void CreateHat(string UniqueName, string[] NewIngredientsStringArr = null, int[] NewIngredientsAmountArr = null, string ModelName = "", bool UsingCustomModel = true)
	{
		if (UniqueName.Length == 0)
		{
			string descriptionOverride = "Error: ModHat.CreateHat '" + UniqueName + "' - Unique Name is null length";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
			return;
		}
		if (NewIngredientsStringArr != null && NewIngredientsStringArr.Length != NewIngredientsAmountArr.Length)
		{
			string descriptionOverride2 = "Error: ModHat.CreateHat '" + UniqueName + "' - Ingredients and Ingredient amounts not equal";
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
				string descriptionOverride3 = "Error: ModHat.CreateHat '" + UniqueName + "' - already used this name!";
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
				ModelName = "Models/Clothes/Hats/HatDinosaur";
			}
			ModModels.Add(objectType, ModelName);
			ModModelsCustom.Add(objectType, UsingCustomModel);
			ModManager.Instance.AddModString(objectType, UniqueName);
			if (DebugInfo)
			{
				Debug.Log("ADDED NEW HAT CALLED " + UniqueName + " (" + UniqueName + ")  ObjID " + objectType);
			}
			ModManager.Instance.CustomCreations++;
			Mod lastCalledMod = ModManager.Instance.GetLastCalledMod();
			if (lastCalledMod != null)
			{
				lastCalledMod.CustomIDs.Add(objectType);
				return;
			}
			string descriptionOverride4 = "Error: ModHat.CreateHat - Cannot find Lua Script";
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
						string descriptionOverride5 = "Error: ModHat.CreateHat - Object Ingredient '" + NewIngredientsStringArr[i] + "' - cannot be found";
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
}
