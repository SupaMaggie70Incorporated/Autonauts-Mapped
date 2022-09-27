using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using UnityEngine;

[MoonSharpUserData]
public class ModTool : ModCustom
{
	public struct ModToolInfo
	{
		public List<ObjectType> ObjectsToUseOn;

		public List<Tile.TileType> TilesToUseOn;

		public List<ObjectType> ObjectsToProduce;

		public List<int> ObjectsToProduceAmount;

		public float AnimationDuration;

		public DynValue Callback;

		public Script OwnerScript;

		public bool DestroyObject;
	}

	public Dictionary<ObjectType, ModToolInfo> CustomToolInfo { get; private set; }

	public Dictionary<ObjectType, MyTool.Type> ToolBaseType { get; private set; }

	public override void Init()
	{
		base.Init();
		CustomToolInfo = new Dictionary<ObjectType, ModToolInfo>();
		ToolBaseType = new Dictionary<ObjectType, MyTool.Type>();
	}

	public override string GetPrefabLocation()
	{
		return "WorldObjects/Tools/ToolMod";
	}

	public override ObjectSubCategory GetSubcategory()
	{
		return ObjectSubCategory.ToolsLevel1;
	}

	public override bool GetStackable()
	{
		return true;
	}

	public void CreateTool(string UniqueName, string[] NewIngredientsStringArr = null, int[] NewIngredientsAmountArr = null, string[] ObjectsToUseOnArr = null, string[] TilesToUseOnArr = null, string[] ObjectsToProduceArr = null, int[] ObjectsToProduceAmountArr = null, float AnimationDuration = 2f, string ModelName = "", bool UsingCustomModel = true, DynValue CallbackOnComplete = null, bool DestroyTarget = true)
	{
		if (UniqueName.Length == 0)
		{
			string descriptionOverride = "Error: ModTool.CreateTool '" + UniqueName + "' - Unique Name is null length";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
			return;
		}
		if (NewIngredientsStringArr != null && NewIngredientsStringArr.Length != NewIngredientsAmountArr.Length)
		{
			string descriptionOverride2 = "Error: ModTool.CreateTool '" + UniqueName + "' - Ingredients and Ingredient amounts not equal";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride2);
			return;
		}
		if (ObjectsToProduceArr != null && ObjectsToProduceArr.Length != ObjectsToProduceAmountArr.Length)
		{
			string descriptionOverride3 = "Error: ModTool.CreateTool '" + UniqueName + "' - Objects to create and amounts not equal";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride3);
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
				string descriptionOverride4 = "Error: ModTool.CreateTool '" + UniqueName + "' - already used this name!";
				ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride4);
				return;
			}
			ObjectType objectType = ObjectTypeList.m_Total + ModManager.Instance.CustomCreations;
			if (CustomToolInfo.ContainsKey(objectType))
			{
				string descriptionOverride5 = "Error: ModTool.CreateTool '" + UniqueName + "' - already created this tool!";
				ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride5);
				return;
			}
			ModIDOriginals.Add(objectType, UniqueName);
			IsEnabled.Add(objectType, value: true);
			HasSetIngredients.Add(objectType, value: false);
			HasSetRecipe.Add(objectType, value: false);
			if (ModelName.Length == 0)
			{
				ModelName = "Models/Tools/ToolAxe";
			}
			ModModels.Add(objectType, ModelName);
			ModModelsCustom.Add(objectType, UsingCustomModel);
			ModToolInfo value = default(ModToolInfo);
			value.ObjectsToUseOn = new List<ObjectType>();
			if (ObjectsToUseOnArr != null)
			{
				for (int i = 0; i < ObjectsToUseOnArr.Length; i++)
				{
					ObjectType result = ObjectType.Nothing;
					if (!Enum.TryParse<ObjectType>(ObjectsToUseOnArr[i], out result))
					{
						result = ModManager.Instance.GetModObjectTypeFromName(ObjectsToUseOnArr[i]);
						if (result == ObjectType.Nothing)
						{
							string descriptionOverride6 = "Error: ModTool.CreateTool - Object '" + ObjectsToUseOnArr[i] + "' - cannot be found";
							ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride6);
							return;
						}
					}
					value.ObjectsToUseOn.Add(result);
				}
			}
			value.TilesToUseOn = new List<Tile.TileType>();
			if (TilesToUseOnArr != null)
			{
				for (int j = 0; j < TilesToUseOnArr.Length; j++)
				{
					Tile.TileType result2 = Tile.TileType.Total;
					if (!Enum.TryParse<Tile.TileType>(TilesToUseOnArr[j], out result2) && result2 == Tile.TileType.Total)
					{
						string descriptionOverride7 = "Error: ModTool.CreateTool - Tile '" + TilesToUseOnArr[j] + "' - cannot be found";
						ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride7);
						return;
					}
					value.TilesToUseOn.Add(result2);
				}
			}
			value.ObjectsToProduce = new List<ObjectType>();
			value.ObjectsToProduceAmount = new List<int>();
			if (ObjectsToProduceArr != null && ObjectsToProduceAmountArr != null)
			{
				for (int k = 0; k < ObjectsToProduceArr.Length; k++)
				{
					ObjectType result3 = ObjectType.Nothing;
					if (!Enum.TryParse<ObjectType>(ObjectsToProduceArr[k], out result3))
					{
						result3 = ModManager.Instance.GetModObjectTypeFromName(ObjectsToProduceArr[k]);
						if (result3 == ObjectType.Nothing)
						{
							string descriptionOverride8 = "Error: ModTool.CreateTool - Object '" + ObjectsToProduceArr[k] + "' - cannot be found";
							ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride8);
							return;
						}
					}
					value.ObjectsToProduce.Add(result3);
					value.ObjectsToProduceAmount.Add(ObjectsToProduceAmountArr[k]);
				}
			}
			value.AnimationDuration = AnimationDuration;
			value.Callback = CallbackOnComplete;
			value.OwnerScript = ModManager.Instance.CreationScript;
			value.DestroyObject = DestroyTarget;
			CustomToolInfo.Add(objectType, value);
			ModManager.Instance.AddModString(objectType, UniqueName);
			if (DebugInfo)
			{
				Debug.Log("ADDED NEW TOOL CALLED " + UniqueName + " (" + UniqueName + ")  ObjID " + objectType);
			}
			ModManager.Instance.CustomCreations++;
			Mod lastCalledMod = ModManager.Instance.GetLastCalledMod();
			if (lastCalledMod != null)
			{
				lastCalledMod.CustomIDs.Add(objectType);
				return;
			}
			string descriptionOverride9 = "Error: ModTool.CreateTool - Cannot find Lua Script";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride9);
			return;
		}
		ObjectType modObjectTypeFromName = ModManager.Instance.GetModObjectTypeFromName(UniqueName);
		ObjectTypeList.Instance.EnableCustomItem(modObjectTypeFromName, GetSubcategory());
		if (!HasSetIngredients[modObjectTypeFromName] && NewIngredientsStringArr != null)
		{
			IngredientRequirement[] array = new IngredientRequirement[NewIngredientsStringArr.Length];
			for (int l = 0; l < NewIngredientsStringArr.Length; l++)
			{
				ObjectType result4 = ObjectType.Nothing;
				if (!Enum.TryParse<ObjectType>(NewIngredientsStringArr[l], out result4))
				{
					result4 = ModManager.Instance.GetModObjectTypeFromName(NewIngredientsStringArr[l]);
					if (result4 == ObjectType.Nothing)
					{
						string descriptionOverride10 = "Error: ModTool.CreateTool - Object Ingredient '" + NewIngredientsStringArr[l] + "' - cannot be found";
						ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride10);
						return;
					}
				}
				int count = NewIngredientsAmountArr[l];
				array[l] = new IngredientRequirement(result4, count);
			}
			ObjectTypeList.Instance.SetIngredients(modObjectTypeFromName, array);
			HasSetIngredients[modObjectTypeFromName] = true;
		}
		VariableManager.Instance.SetVariable(modObjectTypeFromName, "Unlocked", 1);
	}

	public void SetToolCategoryBase(string UniqueName, string BaseType)
	{
		MyTool.Type result = MyTool.Type.Axe;
		if (!Enum.TryParse<MyTool.Type>(BaseType, out result))
		{
			string descriptionOverride = "Error: SetToolCategoryBase'" + BaseType + "' - Unknown tool type for base!";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
			return;
		}
		foreach (KeyValuePair<ObjectType, string> modIDOriginal in ModIDOriginals)
		{
			if (modIDOriginal.Value.Contains(UniqueName) && !ToolBaseType.ContainsKey(modIDOriginal.Key))
			{
				ToolBaseType.Add(modIDOriginal.Key, result);
			}
		}
	}
}
