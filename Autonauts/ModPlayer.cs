using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public class ModPlayer
{
	public void MoveTo(int x, int y)
	{
		FarmerPlayer component = CollectionManager.Instance.GetPlayers()[0].GetComponent<FarmerPlayer>();
		if (new TileCoord(x, y).GetIsValid() && component.m_State == Farmer.State.None)
		{
			component.SendAction(new ActionInfo(ActionType.MoveTo, new TileCoord(x, y)));
		}
	}

	public bool IsBusy()
	{
		try
		{
			if (CollectionManager.Instance.GetPlayers()[0].GetComponent<FarmerPlayer>().m_State != 0)
			{
				return true;
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModPlayer.IsBusy Error: " + ex.ToString());
		}
		return false;
	}

	public void SetStartLocation(int StartX, int StartY)
	{
		if (ModManager.Instance.m_GameOptionsRef != null)
		{
			if (StartX >= 0 && StartY >= 0 && StartX < ModManager.Instance.m_GameOptionsRef.m_MapWidth && StartY < ModManager.Instance.m_GameOptionsRef.m_MapHeight)
			{
				TileCoord playerPosition = new TileCoord(StartX, StartY);
				if ((bool)GameOptionsManager.Instance)
				{
					GameOptionsManager.Instance.m_Options.SetPlayerPosition(playerPosition);
				}
			}
		}
		else if (StartX >= 0 && StartY >= 0 && StartX < TileManager.Instance.m_TilesWide && StartY < TileManager.Instance.m_TilesHigh)
		{
			TileCoord playerPosition2 = new TileCoord(StartX, StartY);
			if ((bool)GameOptionsManager.Instance)
			{
				GameOptionsManager.Instance.m_Options.SetPlayerPosition(playerPosition2);
			}
		}
	}

	public void SetPlayerStartLocation(int StartX, int StartY)
	{
		SetStartLocation(StartX, StartY);
	}

	public Table GetLocation()
	{
		try
		{
			if (CollectionManager.Instance != null)
			{
				List<BaseClass> players = CollectionManager.Instance.GetPlayers();
				if (players.Count > 0)
				{
					FarmerPlayer component = players[0].GetComponent<FarmerPlayer>();
					if (component != null)
					{
						TileCoord tileCoord = new TileCoord(component.transform.localPosition);
						return new Table(ModManager.Instance.GetLastCalledScript(), DynValue.NewNumber(tileCoord.x), DynValue.NewNumber(tileCoord.y));
					}
				}
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModPlayer.GetLocation Error: " + ex.ToString());
		}
		return new Table(ModManager.Instance.GetLastCalledScript(), DynValue.NewNumber(-1.0), DynValue.NewNumber(-1.0));
	}

	public Table GetPlayerLocation()
	{
		return GetLocation();
	}

	public Table GetHeldObjectUIDs()
	{
		try
		{
			FarmerPlayer component = CollectionManager.Instance.GetPlayers()[0].GetComponent<FarmerPlayer>();
			Table table = new Table(ModManager.Instance.GetLastCalledScript());
			if (component.m_FarmerCarry.m_CarryObject.Count > 0)
			{
				foreach (Holdable item in component.m_FarmerCarry.m_CarryObject)
				{
					if ((bool)item)
					{
						table.Append(DynValue.NewNumber(item.m_UniqueID));
					}
				}
				return table;
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModPlayer.GetHeldObjectUIDs Error: " + ex.ToString());
		}
		return new Table(ModManager.Instance.GetLastCalledScript());
	}

	public Table GetAllPlayerHeldObjectsUIDs()
	{
		return GetHeldObjectUIDs();
	}

	public string GetHeldObjectType()
	{
		try
		{
			ObjectType typeIdentifier = CollectionManager.Instance.GetPlayers()[0].GetComponent<FarmerPlayer>().m_FarmerCarry.GetTopObject().m_TypeIdentifier;
			if (typeIdentifier >= ObjectType.Total)
			{
				return ModManager.Instance.m_ModStrings[typeIdentifier];
			}
			return typeIdentifier.ToString();
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModPlayer.GetHeldObjectType Error: " + ex.ToString());
		}
		return "";
	}

	public string GetState()
	{
		try
		{
			return CollectionManager.Instance.GetPlayers()[0].GetComponent<FarmerPlayer>().m_State.ToString();
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModPlayer.GetState Error: " + ex.ToString());
		}
		return "";
	}

	public string GetPlayerState()
	{
		return GetState();
	}

	public int GetUID()
	{
		try
		{
			return CollectionManager.Instance.GetPlayers()[0].GetComponent<FarmerPlayer>().m_UniqueID;
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModPlayer.GetUID Error: " + ex.ToString());
		}
		return -1;
	}

	public void DropAllHeldObjects()
	{
		FarmerPlayer component = CollectionManager.Instance.GetPlayers()[0].GetComponent<FarmerPlayer>();
		if ((bool)component.m_FarmerCarry && component.m_FarmerCarry.m_CarryObject.Count > 0)
		{
			component.m_FarmerCarry.DropAllObjects();
		}
	}

	public void DropAllUpgrades()
	{
		FarmerPlayer component = CollectionManager.Instance.GetPlayers()[0].GetComponent<FarmerPlayer>();
		if ((bool)component.m_FarmerUpgrades)
		{
			component.m_FarmerUpgrades.DropAllObjects();
		}
	}
}
