using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using UnityEngine;

[MoonSharpUserData]
public class ModCustom
{
	protected bool DebugInfo;

	public Dictionary<ObjectType, string> ModIDOriginals;

	public Dictionary<ObjectType, string> ModModels;

	public Dictionary<ObjectType, bool> ModModelsCustom;

	public Dictionary<ObjectType, Vector3> ModModelsScale;

	public Dictionary<ObjectType, Vector3> ModModelsRotations;

	public Dictionary<ObjectType, Vector3> ModModelsTranslations;

	public Dictionary<ObjectType, bool> IsEnabled;

	protected Dictionary<ObjectType, bool> HasSetIngredients;

	protected Dictionary<ObjectType, bool> HasSetRecipe;

	public Dictionary<ObjectType, List<Material>> ModOriginalMaterials;

	public virtual void Init()
	{
		ModIDOriginals = new Dictionary<ObjectType, string>();
		ModModels = new Dictionary<ObjectType, string>();
		ModModelsCustom = new Dictionary<ObjectType, bool>();
		ModModelsScale = new Dictionary<ObjectType, Vector3>();
		ModModelsRotations = new Dictionary<ObjectType, Vector3>();
		ModModelsTranslations = new Dictionary<ObjectType, Vector3>();
		IsEnabled = new Dictionary<ObjectType, bool>();
		HasSetIngredients = new Dictionary<ObjectType, bool>();
		HasSetRecipe = new Dictionary<ObjectType, bool>();
		ModOriginalMaterials = new Dictionary<ObjectType, List<Material>>();
	}

	public virtual string GetPrefabLocation()
	{
		return "WorldObjects/Buildings/Converters/ModConverter";
	}

	public virtual ObjectSubCategory GetSubcategory()
	{
		return ObjectSubCategory.BuildingsWorkshop;
	}

	public virtual bool GetStackable()
	{
		return false;
	}

	public bool IsItCustomType(ObjectType TypeToCheck)
	{
		return ModIDOriginals.ContainsKey(TypeToCheck);
	}

	public bool GetOriginalNameFromType(ObjectType TypeToCheck, out string Name)
	{
		return ModIDOriginals.TryGetValue(TypeToCheck, out Name);
	}

	public bool IsUsingCustomModel(ObjectType TypeToCheck)
	{
		if (ModModelsCustom.ContainsKey(TypeToCheck))
		{
			return ModModelsCustom[TypeToCheck];
		}
		return false;
	}

	public bool GetModelScale(ObjectType TypeToCheck, out Vector3 Scale)
	{
		if (ModModelsScale.TryGetValue(TypeToCheck, out Scale))
		{
			return true;
		}
		Scale = new Vector3(-1f, 1f, 1f);
		return false;
	}

	public bool GetModelRotation(ObjectType TypeToCheck, out Vector3 Rot)
	{
		return ModModelsRotations.TryGetValue(TypeToCheck, out Rot);
	}

	public bool GetModelTranslation(ObjectType TypeToCheck, out Vector3 Trans)
	{
		return ModModelsTranslations.TryGetValue(TypeToCheck, out Trans);
	}

	public void UpdateModelParameters(string UniqueName, float Scale = 1f, float RotX = 0f, float RotY = 0f, float RotZ = 0f, float TransX = 0f, float TransY = 0f, float TransZ = 0f)
	{
		if (GeneralUtils.m_InGame)
		{
			return;
		}
		foreach (KeyValuePair<ObjectType, string> modIDOriginal in ModIDOriginals)
		{
			if (modIDOriginal.Value == UniqueName)
			{
				if (ModModelsScale.ContainsKey(modIDOriginal.Key))
				{
					string descriptionOverride = "Error: UpdateModelParameters'" + UniqueName + "' - Model details already added!";
					ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
					break;
				}
				ModModelsScale.Add(modIDOriginal.Key, new Vector3(0f - Scale, Scale, Scale));
				ModModelsRotations.Add(modIDOriginal.Key, new Vector3(RotX, RotY, RotZ));
				ModModelsTranslations.Add(modIDOriginal.Key, new Vector3(TransX, TransY, TransZ));
			}
		}
	}

	public void UpdateModelScale(string UniqueName, float Scale = 1f)
	{
		if (GeneralUtils.m_InGame)
		{
			return;
		}
		foreach (KeyValuePair<ObjectType, string> modIDOriginal in ModIDOriginals)
		{
			if (modIDOriginal.Value == UniqueName)
			{
				if (ModModelsScale.ContainsKey(modIDOriginal.Key))
				{
					string descriptionOverride = "Error: UpdateModelScale'" + UniqueName + "' - Model details already added!";
					ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
					break;
				}
				ModModelsScale.Add(modIDOriginal.Key, new Vector3(0f - Scale, Scale, Scale));
			}
		}
	}

	public void UpdateModelScaleSplit(string UniqueName, float ScaleX = 1f, float ScaleY = 1f, float ScaleZ = 1f)
	{
		if (GeneralUtils.m_InGame)
		{
			return;
		}
		foreach (KeyValuePair<ObjectType, string> modIDOriginal in ModIDOriginals)
		{
			if (modIDOriginal.Value == UniqueName)
			{
				if (ModModelsScale.ContainsKey(modIDOriginal.Key))
				{
					string descriptionOverride = "Error: UpdateModelScale'" + UniqueName + "' - Model details already added!";
					ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
					break;
				}
				ModModelsScale.Add(modIDOriginal.Key, new Vector3(0f - ScaleX, ScaleY, ScaleZ));
			}
		}
	}

	public void UpdateModelRotation(string UniqueName, float RotX = 0f, float RotY = 0f, float RotZ = 0f)
	{
		if (GeneralUtils.m_InGame)
		{
			return;
		}
		foreach (KeyValuePair<ObjectType, string> modIDOriginal in ModIDOriginals)
		{
			if (modIDOriginal.Value == UniqueName)
			{
				if (ModModelsRotations.ContainsKey(modIDOriginal.Key))
				{
					string descriptionOverride = "Error: UpdateModelRotation'" + UniqueName + "' - Model details already added!";
					ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
					break;
				}
				ModModelsRotations.Add(modIDOriginal.Key, new Vector3(RotX, RotY, RotZ));
			}
		}
	}

	public void UpdateModelTranslation(string UniqueName, float TransX = 0f, float TransY = 0f, float TransZ = 0f)
	{
		if (GeneralUtils.m_InGame)
		{
			return;
		}
		foreach (KeyValuePair<ObjectType, string> modIDOriginal in ModIDOriginals)
		{
			if (modIDOriginal.Value == UniqueName)
			{
				if (ModModelsTranslations.ContainsKey(modIDOriginal.Key))
				{
					string descriptionOverride = "Error: UpdateModelTranslation'" + UniqueName + "' - Model details already added!";
					ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
					break;
				}
				ModModelsTranslations.Add(modIDOriginal.Key, new Vector3(TransX, TransY, TransZ));
			}
		}
	}

	public void RegisterForCustomCallback(string UniqueName, string CallbackType, DynValue Callback)
	{
		if (GeneralUtils.m_InGame)
		{
			return;
		}
		ModManager.CallbackTypes result = ModManager.CallbackTypes.None;
		if (!Enum.TryParse<ModManager.CallbackTypes>(CallbackType, out result))
		{
			string descriptionOverride = "Error: ModCustom.RegisterForCustomCallback - Cannot find Callback Type: " + CallbackType;
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
			return;
		}
		ObjectType result2 = ObjectType.Nothing;
		if (!Enum.TryParse<ObjectType>(UniqueName, out result2))
		{
			result2 = ModManager.Instance.GetModObjectTypeFromName(UniqueName);
			if (result2 == ObjectType.Nothing)
			{
				string descriptionOverride2 = "Error: ModCustom.RegisterForCustomCallback - Object type '" + UniqueName + "' - cannot be found";
				ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride2);
				return;
			}
		}
		ModManager.CallbackData info = default(ModManager.CallbackData);
		info.CallbackFunction = Callback;
		info.CallbackType = result;
		info.Object = result2;
		info.OwnerScript = ModManager.Instance.GetLastCalledScript();
		ModManager.Instance.RegisterCustomCallback(info);
	}
}
