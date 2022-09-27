using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using MoonSharp.Interpreter;
using UnityEngine;

[MoonSharpUserData]
public class ModObject
{
	internal Dictionary<string, Material> m_MaterialCache;

	public void StartMoveTo(int UID, int NewX, int NewY, float Speed = 10f, float Height = 0f)
	{
		TileCoord tileCoord = new TileCoord(NewX, NewY);
		TileCoordObject tileCoordObject = null;
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
		if (objectFromUniqueID != null)
		{
			tileCoordObject = objectFromUniqueID.GetComponent<TileCoordObject>();
		}
		if (!(tileCoordObject == null) && tileCoord.GetIsValid())
		{
			tileCoordObject.m_ModSpeed = Speed;
			tileCoordObject.m_ModPosition = tileCoordObject.transform.position;
			tileCoordObject.m_ModMoveToPosition = tileCoord.ToWorldPositionTileCentered();
			tileCoordObject.m_ModMoveDistance = (tileCoordObject.m_ModMoveToPosition - tileCoordObject.m_ModPosition).magnitude;
			tileCoordObject.m_ModMoveTimer = 0f;
			tileCoordObject.m_ModHeight = Height;
			Vector3 modMoveDelta = tileCoordObject.m_ModMoveToPosition - tileCoordObject.m_ModPosition;
			modMoveDelta.Normalize();
			tileCoordObject.m_ModMoveDelta = modMoveDelta;
			float y = -90f - Mathf.Atan2(modMoveDelta.z, modMoveDelta.x) * 57.29578f;
			tileCoordObject.transform.rotation = Quaternion.Euler(0f, y, 0f);
			tileCoordObject.m_ModOldDiff = 1E+08f;
		}
	}

	public void StartMoveTo(string UID, int NewX, int NewY, float Speed = 10f, float Height = 0f)
	{
	}

	public bool UpdateMoveTo(int UID, bool Arc = false, bool Wobble = false)
	{
		try
		{
			TileCoordObject tileCoordObject = null;
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
			if (objectFromUniqueID != null)
			{
				tileCoordObject = objectFromUniqueID.GetComponent<TileCoordObject>();
			}
			if (tileCoordObject == null || tileCoordObject.m_ModSpeed <= 0f)
			{
				return true;
			}
			if (!new TileCoord(tileCoordObject.m_ModMoveToPosition).GetIsValid())
			{
				return true;
			}
			float magnitude = (tileCoordObject.m_ModMoveToPosition - tileCoordObject.m_ModPosition).magnitude;
			if (magnitude < TileCoordObject.m_ModMoveToFinishDistance || magnitude > tileCoordObject.m_ModOldDiff)
			{
				tileCoordObject.SetPosition(tileCoordObject.m_ModMoveToPosition);
				tileCoordObject.m_ModSpeed = 0f;
				return true;
			}
			tileCoordObject.m_ModOldDiff = magnitude;
			tileCoordObject.m_ModPosition += tileCoordObject.m_ModMoveDelta * tileCoordObject.m_ModSpeed * TimeManager.Instance.m_NormalDelta;
			Vector3 modPosition = tileCoordObject.m_ModPosition;
			float num = magnitude / tileCoordObject.m_ModMoveDistance;
			if (Arc)
			{
				float num2 = Mathf.Sin(num * (float)Math.PI);
				modPosition.y += num2 * 2.5f;
			}
			if (Wobble)
			{
				tileCoordObject.m_ModMoveTimer += TimeManager.Instance.m_NormalDelta;
				float num3 = Mathf.Cos(tileCoordObject.m_ModMoveTimer * (float)Math.PI * 2f) * tileCoordObject.m_ModMoveDelta.z * TileCoordObject.m_ModWobbleHeight * num;
				float num4 = Mathf.Sin(tileCoordObject.m_ModMoveTimer * (float)Math.PI * 3f) * TileCoordObject.m_ModWobbleHeight * num;
				float num5 = Mathf.Cos(tileCoordObject.m_ModMoveTimer * (float)Math.PI * 2f) * tileCoordObject.m_ModMoveDelta.x * TileCoordObject.m_ModWobbleHeight * num;
				modPosition.x += num3;
				modPosition.y = tileCoordObject.m_ModHeight + num4;
				modPosition.z += num5;
			}
			tileCoordObject.SetPosition(modPosition);
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModObject.UpdateMoveTo Error: " + ex.ToString());
			return true;
		}
		return false;
	}

	public bool UpdateMoveTo(string UID, bool Arc = false, bool Wobble = false)
	{
		return true;
	}

	private void MoveInDirection(int UID, int DirectionX, int DirectionY)
	{
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
		if (objectFromUniqueID != null)
		{
			_ = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false).m_TypeIdentifier;
			if ((bool)objectFromUniqueID.GetComponent<TileMover>())
			{
				TileCoord direction = new TileCoord(DirectionX, DirectionY);
				objectFromUniqueID.GetComponent<TileMover>().MoveDirection(direction);
			}
		}
	}

	private void MoveInDirection(string UID, int DirectionX, int DirectionY)
	{
	}

	public void MoveToInstantly(int UID, int NewX, int NewY)
	{
		TileCoordObject tileCoordObject = null;
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
		if (objectFromUniqueID != null)
		{
			tileCoordObject = objectFromUniqueID.GetComponent<TileCoordObject>();
		}
		if (tileCoordObject != null)
		{
			TileCoord position = new TileCoord(NewX, NewY);
			if (position.GetIsValid())
			{
				tileCoordObject.UpdatePositionToTilePosition(position);
			}
		}
	}

	public void MoveToInstantly(string UID, int NewX, int NewY)
	{
	}

	public Table GetObjectTileCoord(int UID)
	{
		try
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
			if (objectFromUniqueID != null)
			{
				TileCoord tileCoord = new TileCoord(objectFromUniqueID.transform.localPosition);
				return new Table(ModManager.Instance.GetLastCalledScript(), DynValue.NewNumber(tileCoord.x), DynValue.NewNumber(tileCoord.y));
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModObject.GetObjectTileCoord Error: " + ex.ToString());
		}
		return new Table(ModManager.Instance.GetLastCalledScript(), DynValue.NewNumber(-1.0), DynValue.NewNumber(-1.0));
	}

	public Table GetObjectTileCoord(string UID)
	{
		return new Table(ModManager.Instance.GetLastCalledScript(), DynValue.NewNumber(-1.0), DynValue.NewNumber(-1.0));
	}

	public bool IsValidObjectUID(int UID)
	{
		try
		{
			return ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false) != null;
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModObject.IsValidObjectUID Error: " + ex.ToString());
		}
		return false;
	}

	public bool IsValidObjectUID(string UID)
	{
		return false;
	}

	public bool DestroyObject(int UID)
	{
		try
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
			if (objectFromUniqueID != null)
			{
				if (!objectFromUniqueID.GetComponent<Worker>() && !MapManager.Instance.IsObjectSafeToDelete(objectFromUniqueID))
				{
					return false;
				}
				if (!objectFromUniqueID.GetComponent<TileCoordObject>())
				{
					return false;
				}
				TileCoord tileCoord = objectFromUniqueID.GetComponent<TileCoordObject>().m_TileCoord;
				Tile tile = TileManager.Instance.GetTile(tileCoord);
				if ((bool)objectFromUniqueID.GetComponent<Building>() && (bool)tile.m_Building)
				{
					if (tile.m_Building.m_Levels != null)
					{
						for (int num = tile.m_Building.m_Levels.Count - 1; num >= 0; num--)
						{
							Building building = tile.m_Building.m_Levels[num];
							tile.m_Building.RemoveBuilding(building);
							BuildingManager.Instance.DestroyBuilding(building);
						}
					}
					BuildingManager.Instance.DestroyBuilding(tile.m_Building);
					return true;
				}
				if ((bool)objectFromUniqueID.GetComponent<Floor>() && (bool)tile.m_Floor)
				{
					BuildingManager.Instance.DestroyBuilding(tile.m_Floor);
					return true;
				}
				if ((bool)GameStateManager.Instance.GetCurrentState().GetComponent<GameStateNormal>() && (bool)objectFromUniqueID.GetComponent<Worker>())
				{
					GameStateManager.Instance.GetCurrentState().GetComponent<GameStateNormal>().RemoveSelectedWorker(objectFromUniqueID.GetComponent<Worker>());
				}
				objectFromUniqueID.StopUsing();
				return true;
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModObject.DestroyObject Error: " + ex.ToString());
		}
		return false;
	}

	public bool DestroyObject(string UID)
	{
		return false;
	}

	public string GetObjectType(int UID)
	{
		try
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
			if (objectFromUniqueID != null)
			{
				if (objectFromUniqueID.m_TypeIdentifier >= ObjectType.Total)
				{
					return ModManager.Instance.m_ModStrings[objectFromUniqueID.m_TypeIdentifier];
				}
				return objectFromUniqueID.m_TypeIdentifier.ToString();
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModObject.GetObjectType Error: " + ex.ToString());
		}
		return "";
	}

	public string GetObjectType(string UID)
	{
		return "";
	}

	public bool IsWearingClothing(int UID, string ClothingType)
	{
		try
		{
			FarmerClothes.Type result = FarmerClothes.Type.Total;
			if (!Enum.TryParse<FarmerClothes.Type>(ClothingType, out result))
			{
				string descriptionOverride = "Error: ModObject.IsWearingClothing - Cannot find Clothing Type: " + ClothingType;
				ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
				return false;
			}
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
			if (objectFromUniqueID != null && (bool)objectFromUniqueID.GetComponent<Farmer>() && objectFromUniqueID.GetComponent<Farmer>().m_FarmerClothes.Get(result) != null)
			{
				return true;
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModObject.IsWearingClothing Error: " + ex.ToString());
		}
		return false;
	}

	public bool IsWearingClothing(string UID, string ClothingType)
	{
		return false;
	}

	public Table GetClothingTypesWorn(int UID)
	{
		Table table = new Table(ModManager.Instance.GetLastCalledScript());
		try
		{
			List<string> list = new List<string>();
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
			if (objectFromUniqueID != null && (bool)objectFromUniqueID.GetComponent<Farmer>())
			{
				FarmerClothes farmerClothes = objectFromUniqueID.GetComponent<Farmer>().m_FarmerClothes;
				for (int i = 0; i < farmerClothes.m_Clothes.Count; i++)
				{
					if ((bool)farmerClothes.m_Clothes[i].GetComponent<Hat>() || (bool)farmerClothes.m_Clothes[i].GetComponent<Top>())
					{
						if (farmerClothes.m_Clothes[i].m_TypeIdentifier >= ObjectType.Total)
						{
							list.Add(ModManager.Instance.m_ModStrings[farmerClothes.m_Clothes[i].m_TypeIdentifier]);
						}
						else
						{
							list.Add(farmerClothes.m_Clothes[i].ToString());
						}
					}
				}
			}
			foreach (string item in list)
			{
				table.Append(DynValue.NewString(item));
			}
			return table;
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModObject.GetClothingTypesWorn Error: " + ex.ToString());
			return table;
		}
	}

	public Table GetClothingTypesWorn(string UID)
	{
		return new Table(ModManager.Instance.GetLastCalledScript());
	}

	public int GetClothingUIDOnObject(int UID, string ClothingType)
	{
		try
		{
			ObjectType result = ObjectType.Nothing;
			if (!Enum.TryParse<ObjectType>(ClothingType, out result))
			{
				result = ModManager.Instance.GetModObjectTypeFromName(ClothingType);
			}
			if (result == ObjectType.Nothing)
			{
				string descriptionOverride = "Error: ModObject.GetClothingUIDOnObject '" + ClothingType + "' - Object Type Not Recognised";
				ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
				return -1;
			}
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
			if (objectFromUniqueID != null && (bool)objectFromUniqueID.GetComponent<Farmer>())
			{
				foreach (Holdable clothe in objectFromUniqueID.GetComponent<Farmer>().m_FarmerClothes.m_Clothes)
				{
					if (clothe.m_TypeIdentifier == result)
					{
						return clothe.m_UniqueID;
					}
				}
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModObject.GetClothingUIDOnObject Error: " + ex.ToString());
		}
		return -1;
	}

	public int GetClothingUIDOnObject(string UID, string ClothingType)
	{
		return -1;
	}

	public Table GetObjectProperties(int UID)
	{
		Table table = new Table(ModManager.Instance.GetLastCalledScript());
		try
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
			if (objectFromUniqueID != null)
			{
				if (objectFromUniqueID.m_TypeIdentifier >= ObjectType.Total)
				{
					table.Append(DynValue.NewString(ModManager.Instance.m_ModStrings[objectFromUniqueID.m_TypeIdentifier]));
				}
				else
				{
					table.Append(DynValue.NewString(objectFromUniqueID.m_TypeIdentifier.ToString()));
				}
				TileCoordObject component = objectFromUniqueID.GetComponent<TileCoordObject>();
				if (component != null)
				{
					table.Append(DynValue.NewNumber(component.m_TileCoord.x));
					table.Append(DynValue.NewNumber(component.m_TileCoord.y));
				}
				else
				{
					table.Append(DynValue.NewNumber(-1.0));
					table.Append(DynValue.NewNumber(-1.0));
				}
				if (objectFromUniqueID.m_TypeIdentifier == ObjectType.ConverterFoundation)
				{
					table.Append(DynValue.NewNumber(objectFromUniqueID.GetComponent<ConverterFoundation>().m_NewBuilding.m_ModelRoot.gameObject.transform.rotation.eulerAngles.y));
				}
				else
				{
					table.Append(DynValue.NewNumber(objectFromUniqueID.m_ModelRoot.gameObject.transform.rotation.eulerAngles.y));
				}
				table.Append(DynValue.NewString(objectFromUniqueID.GetHumanReadableName()));
				return table;
			}
			return table;
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModObject.GetObjectProperties Error: " + ex.ToString());
			return new Table(ModManager.Instance.GetLastCalledScript());
		}
	}

	public Table GetObjectProperties(string UID)
	{
		return new Table(ModManager.Instance.GetLastCalledScript());
	}

	public void SetObjectRotation(int UID, float RotX = 0f, float RotY = 0f, float RotZ = 0f)
	{
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
		if (objectFromUniqueID != null)
		{
			objectFromUniqueID.gameObject.transform.localRotation = Quaternion.Euler(RotX, RotY, RotZ);
		}
	}

	public void SetObjectRotation(string UID, float RotX = 0f, float RotY = 0f, float RotZ = 0f)
	{
	}

	public string GetObjectCategory(int UID)
	{
		try
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
			if (objectFromUniqueID != null)
			{
				return ObjectTypeList.Instance.GetCategoryFromType(objectFromUniqueID.m_TypeIdentifier).ToString();
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModObject.GetObjectCategory Error: " + ex.ToString());
		}
		return "";
	}

	public string GetObjectCategory(string UID)
	{
		return "";
	}

	public string GetObjectSubcategory(int UID)
	{
		try
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
			if (objectFromUniqueID != null)
			{
				return ObjectTypeList.Instance.GetSubCategoryFromType(objectFromUniqueID.m_TypeIdentifier).ToString();
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModObject.GetObjectSubcategory Error: " + ex.ToString());
		}
		return "";
	}

	public string GetObjectSubcategory(string UID)
	{
		return "";
	}

	public void SetObjectDurability(int UID, int Durability)
	{
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
		if (objectFromUniqueID != null)
		{
			Holdable component = objectFromUniqueID.GetComponent<Holdable>();
			if (component != null)
			{
				component.m_UsageCount = Durability;
			}
		}
	}

	public void SetObjectDurability(string UID, int Durability)
	{
	}

	public int GetObjectDurability(int UID)
	{
		try
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
			if (objectFromUniqueID != null)
			{
				Holdable component = objectFromUniqueID.GetComponent<Holdable>();
				if (component != null)
				{
					return component.m_UsageCount;
				}
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModObject.GetObjectDurability Error: " + ex.ToString());
		}
		return -1;
	}

	public int GetObjectDurability(string UID)
	{
		return -1;
	}

	public void SetObjectActive(int UID, bool Active)
	{
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
		if (objectFromUniqueID != null)
		{
			objectFromUniqueID.gameObject.SetActive(Active);
		}
	}

	public void SetObjectActive(string UID, bool Active)
	{
	}

	public void SetObjectVisibility(int UID, bool Visible)
	{
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
		if (objectFromUniqueID != null)
		{
			MeshRenderer[] componentsInChildren = objectFromUniqueID.GetComponentsInChildren<MeshRenderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = Visible;
			}
		}
	}

	public void SetObjectVisibility(string UID, bool Visible)
	{
	}

	public bool AddObjectToColonistHouse(int HousingUID, int ObjectUID)
	{
		try
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(ObjectUID, ErrorCheck: false);
			BaseClass objectFromUniqueID2 = ObjectTypeList.Instance.GetObjectFromUniqueID(HousingUID, ErrorCheck: false);
			if (objectFromUniqueID2 == null || !Housing.GetIsTypeHouse(objectFromUniqueID2.m_TypeIdentifier))
			{
				return false;
			}
			if (objectFromUniqueID == null)
			{
				return false;
			}
			if (!Folk.GetWillFolkAcceptObjectType(objectFromUniqueID.m_TypeIdentifier))
			{
				string descriptionOverride = "Error: ModObject.AddObjectToColonistHouse - '" + ObjectUID + "' is not of correct type to add to house!";
				ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
				return false;
			}
			if (objectFromUniqueID != null)
			{
				Actionable component = objectFromUniqueID.GetComponent<Actionable>();
				return objectFromUniqueID2.GetComponent<Housing>().ModAddObject(component);
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModObject.AddObjectToColonistHouse Error: " + ex.ToString());
		}
		return false;
	}

	public bool AddObjectToColonistHouse(string HousingUID, int ObjectUID)
	{
		return false;
	}

	public bool AddObjectToColonistHouse(int HousingUID, string ObjectUID)
	{
		return false;
	}

	public bool AddObjectToColonistHouse(string HousingUID, string ObjectUID)
	{
		return false;
	}

	public bool RepairColonistHouseWithObject(int HousingUID, int ObjectUID)
	{
		try
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(ObjectUID, ErrorCheck: false);
			BaseClass objectFromUniqueID2 = ObjectTypeList.Instance.GetObjectFromUniqueID(HousingUID, ErrorCheck: false);
			if (objectFromUniqueID2 == null || !Housing.GetIsTypeHouse(objectFromUniqueID2.m_TypeIdentifier))
			{
				return false;
			}
			if (objectFromUniqueID == null)
			{
				return false;
			}
			if (objectFromUniqueID != null)
			{
				Actionable component = objectFromUniqueID.GetComponent<Actionable>();
				return objectFromUniqueID2.GetComponent<Housing>().ModRepairHousing(component);
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModObject.RepairColonistHouseWithObject Error: " + ex.ToString());
		}
		return false;
	}

	public bool RepairColonistHouseWithObject(string HousingUID, int ObjectUID)
	{
		return false;
	}

	public bool RepairColonistHouseWithObject(int HousingUID, string ObjectUID)
	{
		return false;
	}

	public bool RepairColonistHouseWithObject(string HousingUID, string ObjectUID)
	{
		return false;
	}

	public bool AddObjectToResearchStation(int StationUID, int ObjectUID)
	{
		try
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(ObjectUID, ErrorCheck: false);
			BaseClass objectFromUniqueID2 = ObjectTypeList.Instance.GetObjectFromUniqueID(StationUID, ErrorCheck: false);
			if (objectFromUniqueID2 == null)
			{
				return false;
			}
			ResearchStation component = objectFromUniqueID2.GetComponent<ResearchStation>();
			if (component == null || !ResearchStation.GetIsTypeResearchStation(component.m_TypeIdentifier))
			{
				return false;
			}
			if (objectFromUniqueID == null)
			{
				return false;
			}
			if (objectFromUniqueID != null)
			{
				Actionable component2 = objectFromUniqueID.GetComponent<Actionable>();
				return component.ModAddObject(component2);
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModObject.AddObjectToResearchStation Error: " + ex.ToString());
		}
		return false;
	}

	public bool AddObjectToResearchStation(string StationUID, int ObjectUID)
	{
		return false;
	}

	public bool AddObjectToResearchStation(int StationUID, string ObjectUID)
	{
		return false;
	}

	public bool AddObjectToResearchStation(string StationUID, string ObjectUID)
	{
		return false;
	}

	public bool AddObjectToStoneHead(int StoneHeadUID, int ObjectUID)
	{
		try
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(ObjectUID, ErrorCheck: false);
			BaseClass objectFromUniqueID2 = ObjectTypeList.Instance.GetObjectFromUniqueID(StoneHeadUID, ErrorCheck: false);
			if (objectFromUniqueID2 == null)
			{
				return false;
			}
			StoneHeads component = objectFromUniqueID2.GetComponent<StoneHeads>();
			if (component == null)
			{
				return false;
			}
			if (objectFromUniqueID == null)
			{
				return false;
			}
			if (objectFromUniqueID != null)
			{
				Actionable component2 = objectFromUniqueID.GetComponent<Actionable>();
				return component.ModAddObject(component2);
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModObject.AddObjectToStoneHead Error: " + ex.ToString());
		}
		return false;
	}

	public bool AddObjectToStoneHead(string StoneHeadUID, int ObjectUID)
	{
		return false;
	}

	public bool AddObjectToStoneHead(int StoneHeadUID, string ObjectUID)
	{
		return false;
	}

	public bool AddObjectToStoneHead(string StoneHeadUID, string ObjectUID)
	{
		return false;
	}

	public bool AddMaterialsToCache(string FilePath)
	{
		try
		{
			Mod lastCalledMod = ModManager.Instance.GetLastCalledMod();
			FilePath = FilePath.Replace(".mtl", "").Replace(".MTL", "");
			string path = Path.Combine(Path.Combine(lastCalledMod.FolderLocation, "Models"), FilePath).ToLower() + ".mtl";
			Dictionary<string, Material> dictionary = new MTLLoader().Load(path);
			if (m_MaterialCache == null)
			{
				m_MaterialCache = new Dictionary<string, Material>();
			}
			foreach (KeyValuePair<string, Material> item in dictionary)
			{
				m_MaterialCache.Add(lastCalledMod.Name + "/" + FilePath + "/" + item.Key, item.Value);
			}
			return true;
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModObject.AddMaterialsToCache Error: " + ex.ToString());
		}
		return false;
	}

	public bool AddMaterialsToCache(int FilePath)
	{
		return false;
	}

	public bool SetNodeMaterial(int UID, string NodeName, string NewMatName, string OldMatName = "")
	{
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
		if (objectFromUniqueID != null)
		{
			Dictionary<string, Transform> dictionary = new Dictionary<string, Transform>();
			Regex regex = new Regex(NodeName);
			Regex regex2 = new Regex(OldMatName);
			Material material = null;
			MeshRenderer meshRenderer = null;
			Mod lastCalledMod = ModManager.Instance.GetLastCalledMod();
			Queue<Transform> queue = new Queue<Transform>();
			queue.Enqueue(objectFromUniqueID.transform);
			while (queue.Count > 0)
			{
				Transform transform = queue.Dequeue();
				if (regex.Match(transform.name).Success)
				{
					dictionary.Add(transform.name, transform);
				}
				foreach (Transform item in transform)
				{
					queue.Enqueue(item);
				}
			}
			foreach (KeyValuePair<string, Transform> item2 in dictionary)
			{
				int childCount = item2.Value.childCount;
				for (int i = 0; i < childCount; i++)
				{
					Transform child = item2.Value.GetChild(i);
					if (!dictionary.ContainsKey(child.name))
					{
						dictionary.Add(child.name, child);
					}
				}
			}
			foreach (KeyValuePair<string, Transform> item3 in dictionary)
			{
				meshRenderer = item3.Value.GetComponent<MeshRenderer>();
				if (meshRenderer == null)
				{
					continue;
				}
				material = null;
				if (m_MaterialCache != null)
				{
					m_MaterialCache.TryGetValue(lastCalledMod.Name + "/" + NewMatName, out material);
				}
				if (material == null)
				{
					ModManager.Instance.WriteModError("ModObject.SetNodeMaterial Error: Could not find '" + NewMatName + "'.");
					continue;
				}
				List<int> list = new List<int>();
				if (OldMatName != null && OldMatName != "")
				{
					for (int j = 0; j < meshRenderer.materials.Length; j++)
					{
						if (regex2.Match(meshRenderer.materials[j].name).Success)
						{
							list.Add(j);
						}
					}
				}
				if (list.Count == 0)
				{
					list.Add(0);
				}
				Material[] materials = meshRenderer.materials;
				foreach (int item4 in list)
				{
					materials[item4] = material;
				}
				meshRenderer.materials = materials;
			}
			return true;
		}
		ModManager.Instance.WriteModError("ModObject.SetNodeMaterial Error: Object with UID " + UID + " does not exist.");
		return false;
	}

	public bool SetNodeMaterial(int UID, int MeshName, string MatName)
	{
		return false;
	}

	public bool SetNodeMaterial(int UID, string MeshName, int MatName)
	{
		return false;
	}

	public bool SetNodeMaterial(int UID, int MeshName, int MatName)
	{
		return false;
	}

	public bool SetNodeMaterial(string UID, int MeshName, string MatName)
	{
		return false;
	}

	public bool SetNodeMaterial(string UID, string MeshName, int MatName)
	{
		return false;
	}
}
