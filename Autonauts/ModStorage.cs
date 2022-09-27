using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using UnityEngine;

[MoonSharpUserData]
public class ModStorage
{
	public Table GetStorageUIDsOfStorageType(string NewTypeString)
	{
		try
		{
			Table table = new Table(ModManager.Instance.GetLastCalledScript());
			TileCoord topLeftTile = new TileCoord(0, 0);
			TileCoord bottomRightTile = new TileCoord(TileManager.Instance.m_TilesWide - 1, TileManager.Instance.m_TilesHigh - 1);
			ObjectType result = ObjectType.Nothing;
			if (!Enum.TryParse<ObjectType>(NewTypeString, out result))
			{
				result = ModManager.Instance.GetModObjectTypeFromName(NewTypeString);
			}
			if (!Storage.GetIsTypeStorage(result))
			{
				string descriptionOverride = "Error: ModStorage.GetStorageUIDsOfStorageType - '" + NewTypeString + "' either isn't storage type or not found";
				ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
				return new Table(ModManager.Instance.GetLastCalledScript());
			}
			List<TileCoordObject> objectsInArea = PlotManager.Instance.GetObjectsInArea(result, topLeftTile, bottomRightTile);
			foreach (TileCoordObject item in objectsInArea)
			{
				table.Append(DynValue.NewNumber(item.m_UniqueID));
			}
			if (objectsInArea.Count > 0)
			{
				return table;
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModStorage.GetStorageUIDsOfStorageType Error: " + ex.ToString());
		}
		return new Table(ModManager.Instance.GetLastCalledScript());
	}

	public Table GetAllStorageUIDsOfStorageType(string NewTypeString)
	{
		Table storageUIDsOfStorageType = GetStorageUIDsOfStorageType(NewTypeString);
		if (storageUIDsOfStorageType.Length == 0)
		{
			return new Table(ModManager.Instance.GetLastCalledScript(), DynValue.NewNumber(-1.0));
		}
		return storageUIDsOfStorageType;
	}

	public Table GetStorageUIDsHoldingObject(string NewTypeString)
	{
		try
		{
			Table table = new Table(ModManager.Instance.GetLastCalledScript());
			TileCoord topLeftTile = new TileCoord(0, 0);
			TileCoord bottomRightTile = new TileCoord(TileManager.Instance.m_TilesWide - 1, TileManager.Instance.m_TilesHigh - 1);
			ObjectType result = ObjectType.Nothing;
			if (!Enum.TryParse<ObjectType>(NewTypeString, out result))
			{
				result = ModManager.Instance.GetModObjectTypeFromName(NewTypeString);
			}
			ObjectType storageType = ObjectTypeList.Instance.GetStorageType(result);
			List<TileCoordObject> objectsInArea = PlotManager.Instance.GetObjectsInArea(storageType, topLeftTile, bottomRightTile);
			foreach (TileCoordObject item in objectsInArea)
			{
				if ((bool)item.GetComponent<Storage>() && item.GetComponent<Storage>().m_ObjectType == result)
				{
					table.Append(DynValue.NewNumber(item.m_UniqueID));
				}
			}
			if (objectsInArea.Count > 0)
			{
				return table;
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModQuest.GetStorageUIDsHoldingObject Error: " + ex.ToString());
		}
		return new Table(ModManager.Instance.GetLastCalledScript());
	}

	public Table GetStorageInfo(int UID)
	{
		try
		{
			Table table = new Table(ModManager.Instance.GetLastCalledScript());
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
			if (objectFromUniqueID == null)
			{
				return table;
			}
			Storage component = objectFromUniqueID.GetComponent<Storage>();
			if (component != null && component.m_ObjectType != ObjectTypeList.m_Total)
			{
				if (component.m_ObjectType >= ObjectType.Total)
				{
					if (ModManager.Instance.m_ModStrings.ContainsKey(component.m_ObjectType))
					{
						table.Append(DynValue.NewString(ModManager.Instance.m_ModStrings[component.m_ObjectType]));
					}
					else
					{
						table.Append(DynValue.NewNumber(-1.0));
					}
				}
				else
				{
					table.Append(DynValue.NewString(component.m_ObjectType.ToString()));
				}
				table.Append(DynValue.NewNumber(component.GetStored()));
				table.Append(DynValue.NewNumber(component.GetCapacity()));
				if (objectFromUniqueID.m_TypeIdentifier >= ObjectType.Total)
				{
					if (ModManager.Instance.m_ModStrings.ContainsKey(objectFromUniqueID.m_TypeIdentifier))
					{
						table.Append(DynValue.NewString(ModManager.Instance.m_ModStrings[objectFromUniqueID.m_TypeIdentifier]));
					}
					else
					{
						table.Append(DynValue.NewNumber(-1.0));
					}
				}
				else
				{
					table.Append(DynValue.NewString(objectFromUniqueID.m_TypeIdentifier.ToString()));
				}
			}
			return table;
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModStorage.GetStorageInfo Error: " + ex.ToString());
		}
		return new Table(ModManager.Instance.GetLastCalledScript());
	}

	public Table GetStorageInfo(string UID)
	{
		return new Table(ModManager.Instance.GetLastCalledScript());
	}

	public Table GetStorageProperties(int UID)
	{
		Table table = new Table(ModManager.Instance.GetLastCalledScript());
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID);
		if (objectFromUniqueID == null)
		{
			string descriptionOverride = "Error: ModStorage.GetStorageProperties - Storage Object '" + UID + "' cannot be found";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
			return null;
		}
		Storage component = objectFromUniqueID.GetComponent<Storage>();
		if (component == null)
		{
			string descriptionOverride2 = "Error: ModStorage.GetStorageProperties - Storage Object '" + UID + "' is not of type storage";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride2);
			return null;
		}
		if (component != null)
		{
			if (component.m_ObjectType != ObjectTypeList.m_Total)
			{
				if (component.m_ObjectType >= ObjectType.Total)
				{
					if (ModManager.Instance.m_ModStrings.ContainsKey(component.m_ObjectType))
					{
						table.Append(DynValue.NewString(ModManager.Instance.m_ModStrings[component.m_ObjectType]));
					}
					else
					{
						table.Append(DynValue.NewNumber(-1.0));
					}
				}
				else
				{
					table.Append(DynValue.NewString(component.m_ObjectType.ToString()));
				}
				table.Append(DynValue.NewNumber(component.GetStored()));
				table.Append(DynValue.NewNumber(component.GetCapacity()));
				if (objectFromUniqueID.m_TypeIdentifier >= ObjectType.Total)
				{
					if (ModManager.Instance.m_ModStrings.ContainsKey(objectFromUniqueID.m_TypeIdentifier))
					{
						table.Append(DynValue.NewString(ModManager.Instance.m_ModStrings[objectFromUniqueID.m_TypeIdentifier]));
					}
					else
					{
						table.Append(DynValue.NewNumber(-1.0));
					}
				}
				else
				{
					table.Append(DynValue.NewString(objectFromUniqueID.m_TypeIdentifier.ToString()));
				}
			}
			else
			{
				table.Append(DynValue.NewNumber(-1.0));
			}
		}
		return table;
	}

	public bool SetStorageMaxCapacity(int UID, int MaxCapacity)
	{
		try
		{
			Storage component = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false).GetComponent<Storage>();
			if (component != null)
			{
				component.m_Capacity = MaxCapacity;
				component.UpdateStored();
				return true;
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModStorage.SetStorageMaxCapacity Error: " + ex.ToString());
		}
		return false;
	}

	public bool SetStorageMaxCapacity(string UID, int MaxCapacity)
	{
		return false;
	}

	public bool SetStorageQuantityStored(int UID, int CurrentStorage)
	{
		try
		{
			Storage component = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false).GetComponent<Storage>();
			if (component.m_TypeIdentifier == ObjectType.StorageWorker)
			{
				return false;
			}
			if (component != null)
			{
				if (component.m_Stored != CurrentStorage)
				{
					component.SetStored(CurrentStorage);
					component.UpdateStored();
				}
				return true;
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModStorage.SetStorageQuantityStored Error: " + ex.ToString());
		}
		return false;
	}

	public bool SetStorageQuantityStored(string UID, int CurrentStorage)
	{
		return false;
	}

	public bool SetStorageType(string NewTypeString, int MaxCapacity, string NewStorageType)
	{
		try
		{
			ObjectType result = ObjectType.Nothing;
			if (!Enum.TryParse<ObjectType>(NewTypeString, out result))
			{
				result = ModManager.Instance.GetModObjectTypeFromName(NewTypeString);
			}
			if (result == ObjectType.Nothing)
			{
				string descriptionOverride = "Error: ModStorage.SetStorageType '" + NewTypeString + "' - Object Type Not Recognised";
				ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
				return false;
			}
			ObjectType result2 = ObjectType.Nothing;
			if (!Enum.TryParse<ObjectType>(NewStorageType, out result2))
			{
				result2 = ModManager.Instance.GetModObjectTypeFromName(NewStorageType);
			}
			if (result2 == ObjectType.Nothing)
			{
				string descriptionOverride2 = "Error: ModStorage.SetStorageType '" + NewStorageType + "' - Storage Type Not Recognised";
				ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride2);
				return false;
			}
			if (StorageTypeManager.m_StoragePaletteInformation.ContainsKey(result))
			{
				StorageTypeManager.m_StoragePaletteInformation.Remove(result);
			}
			else if (StorageTypeManager.m_StorageGenericInformation.ContainsKey(result))
			{
				StorageTypeManager.m_StorageGenericInformation.Remove(result);
			}
			ObjectTypeList.Instance.SetStorageType(result, result2);
			switch (result2)
			{
			case ObjectType.StorageGeneric:
			case ObjectType.StorageGenericMedium:
				StorageTypeManager.m_StorageGenericInformation.Add(result, new StorageTypeManager.StorageGenericInfo(MaxCapacity));
				break;
			case ObjectType.StoragePalette:
			case ObjectType.StoragePaletteMedium:
				StorageTypeManager.m_StoragePaletteInformation.Add(result, new StorageTypeManager.StorageGenericInfo(MaxCapacity));
				break;
			}
			return true;
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModStorage.SetStorageType Error: " + ex.ToString());
		}
		return false;
	}

	public Table RemoveFromStorage(int StorageUID, int Amount, int xPos, int yPos)
	{
		try
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(StorageUID, ErrorCheck: false);
			Storage component = objectFromUniqueID.GetComponent<Storage>();
			if (component != null)
			{
				if (Amount > component.m_Stored)
				{
					Amount = component.m_Stored;
				}
				TileCoord tileCoord = new TileCoord(xPos, yPos);
				Vector3 position = tileCoord.ToWorldPositionTileCentered();
				Table table = new Table(ModManager.Instance.GetLastCalledScript());
				if (xPos < 0 || yPos < 0 || xPos >= TileManager.Instance.m_TilesWide || yPos >= TileManager.Instance.m_TilesHigh)
				{
					return table;
				}
				StorageWorker component2 = component.GetComponent<StorageWorker>();
				if (component2 != null)
				{
					for (int i = 0; i < Amount; i++)
					{
						table.Append(DynValue.NewNumber(component2.ModReleaseBot(tileCoord).m_UniqueID));
					}
					return table;
				}
				int usageCount = component.ReleaseStored(objectFromUniqueID.m_TypeIdentifier, null, Amount);
				Holdable component3 = objectFromUniqueID.GetComponent<Holdable>();
				if ((bool)component3)
				{
					component3.m_UsageCount = usageCount;
				}
				for (int j = 0; j < Amount; j++)
				{
					if (ObjectTypeList.Instance.GetIsBuilding(component.m_ObjectType))
					{
						Building building = BuildingManager.Instance.AddBuilding(tileCoord, component.m_ObjectType, 0, null, Instant: true);
						table.Append(DynValue.NewNumber(building.m_UniqueID));
						continue;
					}
					BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(component.m_ObjectType, position, Quaternion.identity);
					if (component.m_ObjectType == ObjectType.CropWheat)
					{
						baseClass.GetComponent<CropWheat>().SetState(Crop.State.Wild);
					}
					table.Append(DynValue.NewNumber(baseClass.m_UniqueID));
				}
				component.UpdateStored();
				return table;
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModStorage.RemoveFromStorage Error: " + ex.ToString());
		}
		return new Table(ModManager.Instance.GetLastCalledScript());
	}

	public Table RemoveFromStorage(string StorageUID, int Amount, int xPos, int yPos)
	{
		return new Table(ModManager.Instance.GetLastCalledScript());
	}

	public Table TakeFromStorage(int StorageUID, int Amount, int xPos, int yPos)
	{
		Table table = RemoveFromStorage(StorageUID, Amount, xPos, yPos);
		if (table.Length == 0)
		{
			return null;
		}
		return table;
	}

	public bool AddToStorage(int StorageUID, int ObjectUID)
	{
		try
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(StorageUID, ErrorCheck: false);
			if (objectFromUniqueID == null)
			{
				return false;
			}
			BaseClass objectFromUniqueID2 = ObjectTypeList.Instance.GetObjectFromUniqueID(ObjectUID, ErrorCheck: false);
			Storage component = objectFromUniqueID.GetComponent<Storage>();
			if (component != null)
			{
				Worker component2 = objectFromUniqueID2.GetComponent<Worker>();
				StorageWorker component3 = objectFromUniqueID.GetComponent<StorageWorker>();
				if (component2 != null && component3 != null)
				{
					if (component3.GetCapacity() - component3.m_Stored > 0)
					{
						component3.ModAddBot(component2);
					}
					return true;
				}
				ObjectType typeIdentifier = objectFromUniqueID2.m_TypeIdentifier;
				if (StorageLiquid.GetIsTypeStorageLiquid(objectFromUniqueID.m_TypeIdentifier) && ToolFillable.GetIsTypeFillable(typeIdentifier))
				{
					ToolFillable component4 = objectFromUniqueID2.GetComponent<ToolFillable>();
					if (component4.m_Stored > 0)
					{
						typeIdentifier = component4.m_HeldType;
						component.SetObjectType(typeIdentifier);
						component.AddToStored(typeIdentifier, component4.m_Stored, null);
						component4.Empty(component4.m_Stored);
					}
				}
				else
				{
					component.SetObjectType(objectFromUniqueID2.m_TypeIdentifier);
					component.AddToStored(objectFromUniqueID2, null);
					objectFromUniqueID2.StopUsing();
				}
				component.UpdateStored();
				return true;
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModStorage.AddToStorage Error: " + ex.ToString());
		}
		return false;
	}

	public bool AddToStorage(string StorageUID, int ObjectUID)
	{
		return false;
	}

	public bool AddToStorage(int StorageUID, string ObjectUID)
	{
		return false;
	}

	public bool AddToStorage(string StorageUID, string ObjectUID)
	{
		return false;
	}

	public void RegisterForStorageAddedCallback(int StorageUID, DynValue Callback)
	{
		if (ObjectTypeList.Instance.GetObjectFromUniqueID(StorageUID, ErrorCheck: false).GetComponent<Storage>() == null)
		{
			string descriptionOverride = "Error: ModStorage.RegisterForStorageAddedCallback '" + StorageUID + "' - is not of type Storage";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
			return;
		}
		if (!ModManager.Instance.StorageAddedCallbacks.ContainsKey(StorageUID))
		{
			ModManager.Instance.StorageAddedCallbacks.Add(StorageUID, new List<ModManager.CallbackData>());
		}
		List<ModManager.CallbackData> Dats = ModManager.Instance.StorageAddedCallbacks[StorageUID];
		ModManager.Instance.AddOrOverwriteCallbackInList(ref Dats, Callback, ObjectType.StorageGeneric, ModManager.CallbackTypes.None);
	}

	public void RegisterForStorageAddedCallback(string StorageUID, DynValue Callback)
	{
	}

	public void RegisterForStorageTakenCallback(int StorageUID, DynValue Callback)
	{
		if (ObjectTypeList.Instance.GetObjectFromUniqueID(StorageUID, ErrorCheck: false).GetComponent<Storage>() == null)
		{
			string descriptionOverride = "Error: ModStorage.RegisterForStorageTakenCallback '" + StorageUID + "' - is not of type Storage";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
			return;
		}
		if (!ModManager.Instance.StorageRemovedCallbacks.ContainsKey(StorageUID))
		{
			ModManager.Instance.StorageRemovedCallbacks.Add(StorageUID, new List<ModManager.CallbackData>());
		}
		List<ModManager.CallbackData> Dats = ModManager.Instance.StorageRemovedCallbacks[StorageUID];
		ModManager.Instance.AddOrOverwriteCallbackInList(ref Dats, Callback, ObjectType.StorageGeneric, ModManager.CallbackTypes.None);
	}

	public void RegisterForStorageTakenCallback(string StorageUID, DynValue Callback)
	{
	}

	public void RegisterForStorageItemChangedCallback(int StorageUID, DynValue Callback)
	{
		if (ObjectTypeList.Instance.GetObjectFromUniqueID(StorageUID, ErrorCheck: false).GetComponent<Storage>() == null)
		{
			string descriptionOverride = "Error: ModStorage.RegisterForStorageItemChangedCallback '" + StorageUID + "' - is not of type Storage";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
			return;
		}
		if (!ModManager.Instance.StorageItemChangedCallbacks.ContainsKey(StorageUID))
		{
			ModManager.Instance.StorageItemChangedCallbacks.Add(StorageUID, new List<ModManager.MinimalCallbackData>());
		}
		List<ModManager.MinimalCallbackData> Dats = ModManager.Instance.StorageItemChangedCallbacks[StorageUID];
		ModManager.Instance.AddOrOverwriteCallbackInList(ref Dats, Callback);
	}

	public void RegisterForStorageItemChangedCallback(string StorageUID, DynValue Callback)
	{
	}

	public bool AssignStorageItemType(int StorageUID, string NewTypeString)
	{
		try
		{
			ObjectType result = ObjectType.Nothing;
			if (!Enum.TryParse<ObjectType>(NewTypeString, out result))
			{
				result = ModManager.Instance.GetModObjectTypeFromName(NewTypeString);
			}
			if (result == ObjectType.Nothing)
			{
				string descriptionOverride = "Error: ModStorage.AssignStorageItemType '" + NewTypeString + "' - Object Type Not Recognised";
				ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
				return false;
			}
			Storage component = ObjectTypeList.Instance.GetObjectFromUniqueID(StorageUID, ErrorCheck: false).GetComponent<Storage>();
			if (component != null)
			{
				component.SetObjectType(result);
				return true;
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModStorage.AssignStorageItemType Error: " + ex.ToString());
		}
		return false;
	}

	public bool AssignStorageItemType(string StorageUID, string NewTypeString)
	{
		return false;
	}

	public bool IsStorageUIDValid(int UID)
	{
		try
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
			if (objectFromUniqueID != null && objectFromUniqueID.GetComponent<Storage>() != null)
			{
				return true;
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModStorage.IsStorageUIDValid Error: " + ex.ToString());
		}
		return false;
	}

	public bool IsStorageUIDValid(string UID)
	{
		return false;
	}

	public int TransferBetweenStorages(int SourceStorageUID, int DestinationStorageUID, int Amount)
	{
		try
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(SourceStorageUID, ErrorCheck: false);
			if (objectFromUniqueID == null)
			{
				return 0;
			}
			BaseClass objectFromUniqueID2 = ObjectTypeList.Instance.GetObjectFromUniqueID(DestinationStorageUID, ErrorCheck: false);
			if (objectFromUniqueID2 == null)
			{
				return 0;
			}
			Storage component = objectFromUniqueID.GetComponent<Storage>();
			if (component == null)
			{
				return 0;
			}
			Storage component2 = objectFromUniqueID2.GetComponent<Storage>();
			if (component2 == null)
			{
				return 0;
			}
			if ((bool)component.m_ParentBuilding)
			{
				component = component.m_ParentBuilding.GetComponent<Storage>();
			}
			if ((bool)component2.m_ParentBuilding)
			{
				component2 = component2.m_ParentBuilding.GetComponent<Storage>();
			}
			if (component.m_Stored < 0)
			{
				component.m_Stored = 0;
			}
			if (component2.m_Stored < 0)
			{
				component2.m_Stored = 0;
			}
			if (component.m_ObjectType != component2.m_ObjectType)
			{
				return 0;
			}
			if (component.m_Stored == 0)
			{
				return 0;
			}
			if (Amount > component.m_Stored)
			{
				Amount = component.m_Stored;
			}
			if (component2.m_Stored + Amount > component2.m_Capacity)
			{
				Amount = component2.m_Capacity - component2.m_Stored;
			}
			component.m_Stored -= Amount;
			component2.m_Stored += Amount;
			if (component.m_Used.Count > 0)
			{
				List<int> range = component.m_Used.GetRange(component.m_Stored, Amount);
				component.m_Used.RemoveRange(component.m_Stored, Amount);
				component2.m_Used.AddRange(range);
			}
			component.UpdateStored();
			ModManager.Instance.CheckStorageRemovedCallback(component.m_UniqueID);
			ModManager.Instance.CheckStorageRemovedCallback(component2.m_UniqueID);
			return Amount;
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModStorage.TransferBetweenStorages Error: " + ex.ToString());
		}
		return 0;
	}

	public int TransferBetweenStorages(string SourceStorageUID, int DestinationStorageUID, int Amount)
	{
		return 0;
	}

	public int TransferBetweenStorages(int SourceStorageUID, string DestinationStorageUID, int Amount)
	{
		return 0;
	}

	public int TransferBetweenStorages(string SourceStorageUID, string DestinationStorageUID, int Amount)
	{
		return 0;
	}

	public int TransferBetweenStorages(string SourceStorageUID, int DestinationStorageUID, string Amount)
	{
		return 0;
	}

	public int TransferBetweenStorages(int SourceStorageUID, string DestinationStorageUID, string Amount)
	{
		return 0;
	}

	public int TransferBetweenStorages(string SourceStorageUID, string DestinationStorageUID, string Amount)
	{
		return 0;
	}
}
