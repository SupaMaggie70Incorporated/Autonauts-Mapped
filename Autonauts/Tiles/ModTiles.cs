using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public class ModTiles
{
	public int GetTilesWide()
	{
		if (ModManager.Instance.m_GameOptionsRef != null)
		{
			return ModManager.Instance.m_GameOptionsRef.m_MapWidth;
		}
		return TileManager.Instance.m_TilesWide;
	}

	public int GetTilesHigh()
	{
		if (ModManager.Instance.m_GameOptionsRef != null)
		{
			return ModManager.Instance.m_GameOptionsRef.m_MapHeight;
		}
		return TileManager.Instance.m_TilesHigh;
	}

	public Table GetMapLimits()
	{
		if (ModManager.Instance.m_GameOptionsRef != null)
		{
			return new Table(ModManager.Instance.GetLastCalledScript(), DynValue.NewNumber(ModManager.Instance.m_GameOptionsRef.m_MapWidth), DynValue.NewNumber(ModManager.Instance.m_GameOptionsRef.m_MapHeight));
		}
		return new Table(ModManager.Instance.GetLastCalledScript(), DynValue.NewNumber(TileManager.Instance.m_TilesWide), DynValue.NewNumber(TileManager.Instance.m_TilesHigh));
	}

	public void SetTile(int x, int y, string TileTypeString)
	{
		Tile.TileType newTileType = (Tile.TileType)Enum.Parse(typeof(Tile.TileType), TileTypeString);
		if (ModManager.Instance.m_GameOptionsRef != null)
		{
			ModManager.Instance.m_GameOptionsRef.SetTile(new TileCoord(x, y), newTileType);
		}
		else
		{
			TileManager.Instance.SetTileType(new TileCoord(x, y), newTileType);
		}
	}

	public string GetTileType(int x, int y)
	{
		try
		{
			if (x < 0 || y < 0 || x >= TileManager.Instance.m_TilesWide || y >= TileManager.Instance.m_TilesHigh)
			{
				return "";
			}
			return TileManager.Instance.GetTileType(new TileCoord(x, y)).ToString();
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModTiles.GetTileType Error: " + ex.ToString());
		}
		return "";
	}

	public void ClearEverythingInArea(int StartX, int StartY, int EndX, int EndY)
	{
		if (StartX >= 0 && StartY >= 0 && StartX < TileManager.Instance.m_TilesWide && StartY < TileManager.Instance.m_TilesHigh && EndX >= 0 && EndY >= 0 && EndX < TileManager.Instance.m_TilesWide && EndY < TileManager.Instance.m_TilesHigh)
		{
			TileCoord topLeft = new TileCoord(StartX, StartY);
			TileCoord bottomRight = new TileCoord(EndX, EndY);
			MapManager.Instance.ClearArea(topLeft, bottomRight);
		}
	}

	public void ClearEverythingOnSingleTile(int StartX, int StartY)
	{
		if (StartX >= 0 && StartY >= 0 && StartX < TileManager.Instance.m_TilesWide && StartY < TileManager.Instance.m_TilesHigh)
		{
			TileCoord tileCoord = new TileCoord(StartX, StartY);
			MapManager.Instance.ClearArea(tileCoord, tileCoord);
		}
	}

	public void ClearSpecificsInArea(int StartX, int StartY, int EndX, int EndY, bool Buildings, bool StaticObjects, bool HoldableObjects, bool Tiles)
	{
		if (StartX >= 0 && StartY >= 0 && StartX < TileManager.Instance.m_TilesWide && StartY < TileManager.Instance.m_TilesHigh && EndX >= 0 && EndY >= 0 && EndX < TileManager.Instance.m_TilesWide && EndY < TileManager.Instance.m_TilesHigh)
		{
			TileCoord topLeft = new TileCoord(StartX, StartY);
			TileCoord bottomRight = new TileCoord(EndX, EndY);
			MapManager.Instance.ClearArea(topLeft, bottomRight, Buildings, StaticObjects, HoldableObjects, Tiles);
		}
	}

	public Table GetObjectTypeOnTile(int xPos, int yPos)
	{
		try
		{
			if (xPos < 0 || yPos < 0 || xPos >= TileManager.Instance.m_TilesWide || yPos >= TileManager.Instance.m_TilesHigh)
			{
				return new Table(ModManager.Instance.GetLastCalledScript());
			}
			TileCoord tileCoord = new TileCoord(xPos, yPos);
			List<TileCoordObject> objectsAtTile = PlotManager.Instance.GetObjectsAtTile(tileCoord);
			Table table = new Table(ModManager.Instance.GetLastCalledScript());
			foreach (TileCoordObject item in objectsAtTile)
			{
				if (item.m_TypeIdentifier >= ObjectType.Total)
				{
					table.Append(DynValue.NewString(ModManager.Instance.m_ModStrings[item.m_TypeIdentifier]));
				}
				else
				{
					table.Append(DynValue.NewString(item.m_TypeIdentifier.ToString()));
				}
			}
			Tile tile = TileManager.Instance.GetTile(tileCoord);
			if (tile.m_Building != null)
			{
				bool flag = false;
				foreach (TileCoordObject item2 in objectsAtTile)
				{
					if (item2.m_UniqueID == tile.m_Building.m_UniqueID)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					if (tile.m_Building.m_TypeIdentifier >= ObjectType.Total)
					{
						table.Append(DynValue.NewString(ModManager.Instance.m_ModStrings[tile.m_Building.m_TypeIdentifier]));
					}
					else
					{
						table.Append(DynValue.NewString(tile.m_Building.m_TypeIdentifier.ToString()));
					}
				}
			}
			return table;
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModTiles.GetObjectTypeOnTile Error: " + ex.ToString());
		}
		return new Table(ModManager.Instance.GetLastCalledScript());
	}

	public Table GetObjectUIDsOnTile(int xPos, int yPos)
	{
		try
		{
			if (xPos < 0 || yPos < 0 || xPos >= TileManager.Instance.m_TilesWide || yPos >= TileManager.Instance.m_TilesHigh)
			{
				return new Table(ModManager.Instance.GetLastCalledScript());
			}
			TileCoord tileCoord = new TileCoord(xPos, yPos);
			List<TileCoordObject> objectsAtTile = PlotManager.Instance.GetObjectsAtTile(tileCoord);
			Table table = new Table(ModManager.Instance.GetLastCalledScript());
			foreach (TileCoordObject item in objectsAtTile)
			{
				table.Append(DynValue.NewNumber(item.m_UniqueID));
			}
			Tile tile = TileManager.Instance.GetTile(tileCoord);
			if (tile.m_Building != null)
			{
				bool flag = false;
				foreach (TileCoordObject item2 in objectsAtTile)
				{
					if (item2.m_UniqueID == tile.m_Building.m_UniqueID)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					table.Append(DynValue.NewNumber(tile.m_Building.m_UniqueID));
				}
			}
			return table;
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModTiles.GetObjectUIDsOnTile Error: " + ex.ToString());
		}
		return new Table(ModManager.Instance.GetLastCalledScript());
	}

	public int GetAmountObjectsOfTypeInArea(string NewTypeString, int StartX, int StartY, int EndX, int EndY)
	{
		try
		{
			if (StartX < 0 || StartY < 0 || StartX >= TileManager.Instance.m_TilesWide || StartY >= TileManager.Instance.m_TilesHigh)
			{
				return 0;
			}
			if (EndX < 0 || EndY < 0 || EndX >= TileManager.Instance.m_TilesWide || EndY >= TileManager.Instance.m_TilesHigh)
			{
				return 0;
			}
			TileCoord topLeftTile = new TileCoord(StartX, StartY);
			TileCoord bottomRightTile = new TileCoord(EndX, EndY);
			ObjectType result = ObjectType.Nothing;
			if (!Enum.TryParse<ObjectType>(NewTypeString, out result))
			{
				result = ModManager.Instance.GetModObjectTypeFromName(NewTypeString);
			}
			if (result == ObjectType.Nothing)
			{
				string descriptionOverride = "Error: ModTiles.GetAmountObjectsOfTypeInArea '" + NewTypeString + "' - Object Type Not Recognised";
				ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
				return 0;
			}
			return PlotManager.Instance.GetObjectsInArea(result, topLeftTile, bottomRightTile).Count;
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModTiles.GetObjectUIDsOnTile Error: " + ex.ToString());
		}
		return 0;
	}

	public int[] GetObjectUIDsOfType(string NewTypeString, int StartX, int StartY, int EndX, int EndY)
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
			if (result == ObjectType.FarmerPlayer)
			{
				FarmerPlayer component = CollectionManager.Instance.GetPlayers()[0].GetComponent<FarmerPlayer>();
				return new int[1] { component.m_UniqueID };
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
			List<TileCoordObject> objectsInArea = PlotManager.Instance.GetObjectsInArea(result, topLeftTile, bottomRightTile);
			int[] array2 = new int[objectsInArea.Count];
			int num2 = 0;
			foreach (TileCoordObject item2 in objectsInArea)
			{
				array2[num2++] = item2.m_UniqueID;
			}
			return array2;
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModTiles.GetObjectUIDsOfType Error: " + ex.ToString());
		}
		return new int[0];
	}

	public int[] GetObjectsOfTypeInAreaUIDs(string NewTypeString, int StartX, int StartY, int EndX, int EndY)
	{
		if (StartX < 0 || StartY < 0 || StartX >= TileManager.Instance.m_TilesWide || StartY >= TileManager.Instance.m_TilesHigh)
		{
			return null;
		}
		if (EndX < 0 || EndY < 0 || EndX >= TileManager.Instance.m_TilesWide || EndY >= TileManager.Instance.m_TilesHigh)
		{
			return null;
		}
		return GetObjectUIDsOfType(NewTypeString, StartX, StartY, EndX, EndY);
	}

	public int[] GetObjectsOfTypeInAreaIDs(string NewTypeString, int StartX, int StartY, int EndX, int EndY)
	{
		return GetObjectsOfTypeInAreaUIDs(NewTypeString, StartX, StartY, EndX, EndY);
	}

	public bool IsBuildingOnTile(int xPos, int yPos)
	{
		try
		{
			if (xPos < 0 || yPos < 0 || xPos >= TileManager.Instance.m_TilesWide || yPos >= TileManager.Instance.m_TilesHigh)
			{
				return false;
			}
			TileCoord newPosition = new TileCoord(xPos, yPos);
			foreach (TileCoordObject item in PlotManager.Instance.GetObjectsAtTile(newPosition))
			{
				if (ObjectTypeList.Instance.GetIsBuilding(item.m_TypeIdentifier))
				{
					return true;
				}
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModTiles.IsBuildingOnTile Error: " + ex.ToString());
		}
		return false;
	}

	public Table GetSelectableObjectUIDs(int xPos, int yPos)
	{
		try
		{
			if (xPos < 0 || yPos < 0 || xPos >= TileManager.Instance.m_TilesWide || yPos >= TileManager.Instance.m_TilesHigh)
			{
				return new Table(ModManager.Instance.GetLastCalledScript());
			}
			TileCoord newPosition = new TileCoord(xPos, yPos);
			List<TileCoordObject> objectsAtTile = PlotManager.Instance.GetObjectsAtTile(newPosition);
			if (objectsAtTile.Count > 0)
			{
				Table table = new Table(ModManager.Instance.GetLastCalledScript());
				foreach (TileCoordObject item in objectsAtTile)
				{
					if ((bool)item && item.GetComponent<Selectable>() != null)
					{
						table.Append(DynValue.NewNumber(item.m_UniqueID));
					}
				}
				return table;
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModTiles.GetSelectableObjectUIDs Error: " + ex.ToString());
		}
		return new Table(ModManager.Instance.GetLastCalledScript());
	}

	public Table GetSelectableObjectUIDsOnTile(int xPos, int yPos)
	{
		Table selectableObjectUIDs = GetSelectableObjectUIDs(xPos, yPos);
		if (selectableObjectUIDs.Length == 0)
		{
			selectableObjectUIDs.Append(DynValue.NewNumber(-1.0));
		}
		return selectableObjectUIDs;
	}

	public Table GetSelectableObjectTypes(int StartX, int StartY, int EndX, int EndY, bool AllowBuildings = false)
	{
		try
		{
			if (StartX < 0 || StartY < 0 || StartX >= TileManager.Instance.m_TilesWide || StartY >= TileManager.Instance.m_TilesHigh)
			{
				return new Table(ModManager.Instance.GetLastCalledScript());
			}
			if (EndX < 0 || EndY < 0 || EndX >= TileManager.Instance.m_TilesWide || EndY >= TileManager.Instance.m_TilesHigh)
			{
				return new Table(ModManager.Instance.GetLastCalledScript());
			}
			List<Selectable> list = new List<Selectable>();
			for (int i = StartX; i < EndX; i++)
			{
				for (int j = StartY; j < EndY; j++)
				{
					TileCoord newPosition = new TileCoord(i, j);
					Selectable selectableObjectAtTile = PlotManager.Instance.GetSelectableObjectAtTile(newPosition, null, !AllowBuildings);
					if (selectableObjectAtTile != null)
					{
						list.Add(selectableObjectAtTile);
					}
				}
			}
			if (list.Count > 0)
			{
				Table table = new Table(ModManager.Instance.GetLastCalledScript());
				foreach (Selectable item in list)
				{
					if ((bool)item && item.GetComponent<Selectable>() != null)
					{
						table.Append(DynValue.NewString(item.m_TypeIdentifier.ToString()));
					}
				}
				return table;
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModTiles.GetSelectableObjectTypes Error: " + ex.ToString());
		}
		return new Table(ModManager.Instance.GetLastCalledScript());
	}

	public bool IsSubcategoryOnTile(int xPos, int yPos, string Subcategory)
	{
		try
		{
			if (xPos < 0 || yPos < 0 || xPos >= TileManager.Instance.m_TilesWide || yPos >= TileManager.Instance.m_TilesHigh)
			{
				return false;
			}
			ObjectSubCategory result = ObjectSubCategory.Any;
			if (!Enum.TryParse<ObjectSubCategory>(Subcategory, out result))
			{
				string descriptionOverride = "Error: ModTiles.IsSubcategoryOnTile - '" + Subcategory + "' cannot be found";
				ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
				return false;
			}
			TileCoord newPosition = new TileCoord(xPos, yPos);
			foreach (TileCoordObject item in PlotManager.Instance.GetObjectsAtTile(newPosition))
			{
				if (ObjectTypeList.Instance.GetSubCategoryFromType(item.m_TypeIdentifier) == result)
				{
					return true;
				}
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModTiles.IsSubcategoryOnTile Error: " + ex.ToString());
		}
		return false;
	}

	public Table GetRandomEmptyTileCoordinatesNear(int xPos, int yPos)
	{
		try
		{
			if (xPos < 0 || yPos < 0 || xPos >= TileManager.Instance.m_TilesWide || yPos >= TileManager.Instance.m_TilesHigh)
			{
				return new Table(ModManager.Instance.GetLastCalledScript(), DynValue.NewNumber(-1.0), DynValue.NewNumber(-1.0));
			}
			TileCoord oldPosition = new TileCoord(xPos, yPos);
			oldPosition = TileHelpers.GetRandomEmptyTile(oldPosition);
			Table table = new Table(ModManager.Instance.GetLastCalledScript());
			table.Append(DynValue.NewNumber(oldPosition.x));
			table.Append(DynValue.NewNumber(oldPosition.y));
			return table;
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModTiles.GetRandomEmptyTileCoordinatesNear Error: " + ex.ToString());
		}
		return new Table(ModManager.Instance.GetLastCalledScript(), DynValue.NewNumber(-1.0), DynValue.NewNumber(-1.0));
	}

	public Table GetHoldableObjectUIDs(int xPos, int yPos)
	{
		try
		{
			if (xPos < 0 || yPos < 0 || xPos >= TileManager.Instance.m_TilesWide || yPos >= TileManager.Instance.m_TilesHigh)
			{
				return new Table(ModManager.Instance.GetLastCalledScript());
			}
			TileCoord newPosition = new TileCoord(xPos, yPos);
			List<TileCoordObject> objectsAtTile = PlotManager.Instance.GetObjectsAtTile(newPosition);
			Table table = new Table(ModManager.Instance.GetLastCalledScript());
			foreach (TileCoordObject item in objectsAtTile)
			{
				if ((bool)item.GetComponent<Holdable>())
				{
					table.Append(DynValue.NewNumber(item.m_UniqueID));
				}
			}
			return table;
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModTiles.GetHoldableObjectUIDs Error: " + ex.ToString());
		}
		return new Table(ModManager.Instance.GetLastCalledScript());
	}

	public Table GetHoldableObjectUIDsOnTile(int xPos, int yPos)
	{
		if (xPos < 0 || yPos < 0 || xPos >= TileManager.Instance.m_TilesWide || yPos >= TileManager.Instance.m_TilesHigh)
		{
			return null;
		}
		return GetHoldableObjectUIDs(xPos, yPos);
	}

	public bool RegisterForPlayerOrBotEnterOrExitTile(int TileX, int TileY, DynValue Callback)
	{
		try
		{
			if (TileX < 0 || TileY < 0 || TileX >= TileManager.Instance.m_TilesWide || TileY >= TileManager.Instance.m_TilesHigh)
			{
				return false;
			}
			int key = TileY * TileManager.Instance.m_TilesWide + TileX;
			if (!ModManager.Instance.PlayerOrBotEnterOrExitTileCallbacks.ContainsKey(key))
			{
				ModManager.Instance.PlayerOrBotEnterOrExitTileCallbacks.Add(key, new List<ModManager.MinimalCallbackData>());
			}
			List<ModManager.MinimalCallbackData> Dats = ModManager.Instance.PlayerOrBotEnterOrExitTileCallbacks[key];
			ModManager.Instance.AddOrOverwriteCallbackInList(ref Dats, Callback);
			return true;
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModTiles.RegisterForPlayerOrBotEnterOrExitTile Error: " + ex.ToString());
		}
		return false;
	}

	public bool UnregisterForPlayerOrBotEnterOrExitTile(int TileX, int TileY)
	{
		try
		{
			if (TileX < 0 || TileY < 0 || TileX >= TileManager.Instance.m_TilesWide || TileY >= TileManager.Instance.m_TilesHigh)
			{
				return false;
			}
			int key = TileY * TileManager.Instance.m_TilesWide + TileX;
			if (ModManager.Instance.PlayerOrBotEnterOrExitTileCallbacks.ContainsKey(key))
			{
				List<ModManager.MinimalCallbackData> Dats = ModManager.Instance.PlayerOrBotEnterOrExitTileCallbacks[key];
				ModManager.Instance.RemoveCallbackFromList(ref Dats);
				if (Dats.Count == 0)
				{
					ModManager.Instance.PlayerOrBotEnterOrExitTileCallbacks.Remove(key);
				}
			}
			return true;
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModTiles.UnregisterForPlayerOrBotEnterOrExitTile Error: " + ex.ToString());
		}
		return false;
	}
}
