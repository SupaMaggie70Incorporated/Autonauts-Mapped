using System.Collections.Generic;
using UnityEngine;

public class TileHelpers : MonoBehaviour
{
	public static void GetClippedTileCoordArea(TileCoord TopLeftIn, TileCoord BottomRightIn, out TileCoord TopLeft, out TileCoord BottomRight)
	{
		TopLeft = TopLeftIn;
		BottomRight = BottomRightIn;
		if (TopLeft.x < 0)
		{
			TopLeft.x = 0;
		}
		if (TopLeft.y < 0)
		{
			TopLeft.y = 0;
		}
		if (BottomRight.x >= TileManager.Instance.m_TilesWide)
		{
			BottomRight.x = TileManager.Instance.m_TilesWide - 1;
		}
		if (BottomRight.y >= TileManager.Instance.m_TilesHigh)
		{
			BottomRight.y = TileManager.Instance.m_TilesHigh - 1;
		}
	}

	public static TileCoord GetRandomEmptyTile(TileCoord OldPosition, bool CheckFloor = false, bool CheckAssociatedObject = false, bool CheckGreaterRange = false)
	{
		TileCoord tileCoord = OldPosition;
		Tile tile = null;
		int num = 0;
		bool flag = true;
		int num2 = 50;
		if (CheckGreaterRange)
		{
			num2 = 1000;
		}
		int num3 = 1;
		do
		{
			flag = true;
			int num4 = Random.Range(0, 4);
			int nx = 0;
			int ny = 0;
			switch (num4)
			{
			case 0:
				ny = -num3;
				nx = Random.Range(-num3 + 1, num3 + 1);
				break;
			case 1:
				ny = num3;
				nx = Random.Range(-num3, num3);
				break;
			}
			switch (num4)
			{
			case 2:
				nx = -num3;
				ny = Random.Range(-num3, num3);
				break;
			case 3:
				nx = num3;
				ny = Random.Range(-num3 + 1, num3 + 1);
				break;
			}
			tileCoord = OldPosition + new TileCoord(nx, ny);
			if (tileCoord.x < 0 || tileCoord.x >= TileManager.Instance.m_TilesWide || tileCoord.y < 0 || tileCoord.y >= TileManager.Instance.m_TilesHigh)
			{
				flag = false;
			}
			else
			{
				tile = TileManager.Instance.GetTile(tileCoord);
				if (tile.m_Building != null || tile.m_BuildingFootprint != null || tileCoord == OldPosition || ((bool)tile.m_AssociatedObject && (!ObjectTypeList.Instance.GetCanDropInto(tile.m_AssociatedObject.m_TypeIdentifier) || !Flora.GetIsTypeFlora(tile.m_AssociatedObject.m_TypeIdentifier))) || TileManager.Instance.GetTileSolidToPlayer(tileCoord))
				{
					flag = false;
				}
				if (CheckFloor && (bool)tile.m_Floor)
				{
					flag = false;
				}
				if (CheckAssociatedObject && (bool)tile.m_AssociatedObject)
				{
					flag = false;
				}
			}
			num++;
			if (CheckGreaterRange && num % 50 == 0)
			{
				num3++;
				Debug.Log(num3);
			}
		}
		while (!flag && num < num2);
		if (num == num2)
		{
			tileCoord = OldPosition;
		}
		return tileCoord;
	}

	public static List<TileCoord> GetNearestTileOfType(Tile.TileType NewType, TileCoord TopLeftIn, TileCoord BottomRightIn, HighInstruction.FindType NewFindType, Actionable Requester, ObjectType UseObjectType, AFO.AT NewActionType, string ActionRequirement)
	{
		List<TileCoord> list = new List<TileCoord>();
		Holdable newObject = null;
		if ((bool)Requester && (Requester.m_TypeIdentifier == ObjectType.FarmerPlayer || Requester.m_TypeIdentifier == ObjectType.Worker))
		{
			Holdable topObject = Requester.GetComponent<Farmer>().m_FarmerCarry.GetTopObject();
			if ((bool)topObject)
			{
				if (topObject.m_TypeIdentifier == UseObjectType)
				{
					newObject = topObject;
				}
				else if (MyTool.GetIsTypeTool(topObject.m_TypeIdentifier) && MyTool.GetType(topObject.m_TypeIdentifier) == MyTool.GetType(UseObjectType))
				{
					newObject = topObject;
				}
			}
		}
		Actionable.m_ReusableActionFromObject.Init(newObject, UseObjectType, Requester, NewActionType, ActionRequirement, default(TileCoord));
		GetClippedTileCoordArea(TopLeftIn, BottomRightIn, out var TopLeft, out var BottomRight);
		TileCoord tileCoord = Requester.GetComponent<TileCoordObject>().m_TileCoord;
		int num = 0;
		int num2 = 0;
		if (TopLeft.y > BottomRight.y)
		{
			return list;
		}
		int y = tileCoord.y;
		if (y < TopLeft.y)
		{
			y = TopLeft.y;
		}
		else if (y > BottomRight.y)
		{
			y = BottomRight.y;
		}
		TileCoord tileCoord2 = default(TileCoord);
		TileCoord item = default(TileCoord);
		float num3 = 1E+08f;
		bool flag = false;
		int num4 = 0;
		while (true)
		{
			int num5 = y - num4;
			int num6 = y + num4;
			if (num5 < TopLeft.y && num6 > BottomRight.y)
			{
				break;
			}
			if (num5 >= TopLeft.y)
			{
				tileCoord2.y = num5;
				int num7 = num5 - TopLeft.y;
				int num8 = 0;
				for (int i = TopLeft.x; i <= BottomRight.x; i++)
				{
					num++;
					tileCoord2.x = i;
					Plot plotAtTile = PlotManager.Instance.GetPlotAtTile(tileCoord2);
					if (plotAtTile.m_Visible && TileManager.Instance.GetTile(tileCoord2).m_TileType == NewType)
					{
						float num9 = (tileCoord - tileCoord2).MagnitudeSqr();
						if (num9 < num3)
						{
							bool flag2 = true;
							switch (NewFindType)
							{
							case HighInstruction.FindType.Even:
								if (((uint)num7 & (true ? 1u : 0u)) != 0 || ((uint)num8 & (true ? 1u : 0u)) != 0)
								{
									flag2 = false;
								}
								break;
							case HighInstruction.FindType.HStripes:
								if (((uint)num7 & (true ? 1u : 0u)) != 0)
								{
									flag2 = false;
								}
								break;
							case HighInstruction.FindType.VStripes:
								if (((uint)num8 & (true ? 1u : 0u)) != 0)
								{
									flag2 = false;
								}
								break;
							case HighInstruction.FindType.Checkers:
								if (((uint)(num7 + num8) & (true ? 1u : 0u)) != 0)
								{
									flag2 = false;
								}
								break;
							}
							if (flag2)
							{
								num2++;
								Actionable.m_ReusableActionFromObject.m_Position = tileCoord2;
								ActionType actionFromObjectSafe = plotAtTile.GetActionFromObjectSafe(Actionable.m_ReusableActionFromObject);
								if (actionFromObjectSafe != ActionType.Total && actionFromObjectSafe != ActionType.Fail && !BaggedManager.Instance.IsTileBagged(tileCoord2))
								{
									item = tileCoord2;
									flag = true;
									num3 = num9;
								}
							}
						}
					}
					num8++;
				}
			}
			if (num4 > 0 && num6 <= BottomRight.y)
			{
				tileCoord2.y = num6;
				int num10 = num6 - TopLeft.y;
				int num11 = 0;
				for (int j = TopLeft.x; j <= BottomRight.x; j++)
				{
					num++;
					tileCoord2.x = j;
					Plot plotAtTile2 = PlotManager.Instance.GetPlotAtTile(tileCoord2);
					if (plotAtTile2.m_Visible && TileManager.Instance.GetTile(tileCoord2).m_TileType == NewType)
					{
						float num12 = (tileCoord - tileCoord2).MagnitudeSqr();
						if (num12 < num3)
						{
							bool flag3 = true;
							switch (NewFindType)
							{
							case HighInstruction.FindType.Even:
								if (((uint)num10 & (true ? 1u : 0u)) != 0 || ((uint)num11 & (true ? 1u : 0u)) != 0)
								{
									flag3 = false;
								}
								break;
							case HighInstruction.FindType.HStripes:
								if (((uint)num10 & (true ? 1u : 0u)) != 0)
								{
									flag3 = false;
								}
								break;
							case HighInstruction.FindType.VStripes:
								if (((uint)num11 & (true ? 1u : 0u)) != 0)
								{
									flag3 = false;
								}
								break;
							case HighInstruction.FindType.Checkers:
								if (((uint)(num10 + num11) & (true ? 1u : 0u)) != 0)
								{
									flag3 = false;
								}
								break;
							}
							if (flag3)
							{
								num2++;
								Actionable.m_ReusableActionFromObject.m_Position = tileCoord2;
								ActionType actionFromObjectSafe2 = plotAtTile2.GetActionFromObjectSafe(Actionable.m_ReusableActionFromObject);
								if (actionFromObjectSafe2 != ActionType.Total && actionFromObjectSafe2 != ActionType.Fail && !BaggedManager.Instance.IsTileBagged(tileCoord2))
								{
									item = tileCoord2;
									flag = true;
									num3 = num12;
								}
							}
						}
					}
					num11++;
				}
			}
			num4++;
		}
		if (flag)
		{
			list.Add(item);
		}
		return list;
	}

	public static List<TileCoord> GetTilesOfType(Tile.TileType NewType, TileCoord TopLeftIn, TileCoord BottomRightIn, HighInstruction.FindType NewFindType, Actionable Requester, ObjectType UseObjectType, AFO.AT NewActionType, string ActionRequirement)
	{
		List<TileCoord> list = new List<TileCoord>();
		Holdable newObject = null;
		if ((bool)Requester && (bool)Requester.GetComponent<Farmer>())
		{
			Holdable topObject = Requester.GetComponent<Farmer>().m_FarmerCarry.GetTopObject();
			if ((bool)topObject)
			{
				if (topObject.m_TypeIdentifier == UseObjectType)
				{
					newObject = topObject;
				}
				else if (MyTool.GetIsTypeTool(topObject.m_TypeIdentifier) && MyTool.GetType(topObject.m_TypeIdentifier) == MyTool.GetType(UseObjectType))
				{
					newObject = topObject;
				}
			}
		}
		Actionable.m_ReusableActionFromObject.Init(newObject, UseObjectType, Requester, NewActionType, ActionRequirement, default(TileCoord));
		GetClippedTileCoordArea(TopLeftIn, BottomRightIn, out var TopLeft, out var BottomRight);
		_ = Requester.GetComponent<TileCoordObject>().m_TileCoord;
		TileCoord tileCoord = default(TileCoord);
		int num = 0;
		for (int i = TopLeft.y; i <= BottomRight.y; i++)
		{
			tileCoord.y = i;
			int num2 = 0;
			for (int j = TopLeft.x; j <= BottomRight.x; j++)
			{
				bool flag = true;
				switch (NewFindType)
				{
				case HighInstruction.FindType.Even:
					if (((uint)num & (true ? 1u : 0u)) != 0 || ((uint)num2 & (true ? 1u : 0u)) != 0)
					{
						flag = false;
					}
					break;
				case HighInstruction.FindType.HStripes:
					if (((uint)num & (true ? 1u : 0u)) != 0)
					{
						flag = false;
					}
					break;
				case HighInstruction.FindType.VStripes:
					if (((uint)num2 & (true ? 1u : 0u)) != 0)
					{
						flag = false;
					}
					break;
				case HighInstruction.FindType.Checkers:
					if (((uint)(num + num2) & (true ? 1u : 0u)) != 0)
					{
						flag = false;
					}
					break;
				}
				if (flag)
				{
					tileCoord.x = j;
					Plot plotAtTile = PlotManager.Instance.GetPlotAtTile(tileCoord);
					if (plotAtTile.m_Visible && TileManager.Instance.GetTile(tileCoord).m_TileType == NewType)
					{
						Actionable.m_ReusableActionFromObject.m_Position = tileCoord;
						ActionType actionFromObjectSafe = plotAtTile.GetActionFromObjectSafe(Actionable.m_ReusableActionFromObject);
						if (actionFromObjectSafe != ActionType.Total && actionFromObjectSafe != ActionType.Fail && !BaggedManager.Instance.IsTileBagged(tileCoord))
						{
							list.Add(tileCoord);
						}
					}
				}
				num2++;
			}
			num++;
		}
		return list;
	}

	public static List<TileCoord> GetNearestTileOfTypes(Dictionary<Tile.TileType, int> NewTypes, TileCoord TopLeftIn, TileCoord BottomRightIn, HighInstruction.FindType NewFindType, Actionable Requester, ObjectType UseObjectType, AFO.AT NewActionType, string ActionRequirement)
	{
		List<TileCoord> list = new List<TileCoord>();
		Holdable newObject = null;
		if ((bool)Requester && (bool)Requester.GetComponent<Farmer>())
		{
			Holdable topObject = Requester.GetComponent<Farmer>().m_FarmerCarry.GetTopObject();
			if ((bool)topObject && topObject.m_TypeIdentifier == UseObjectType)
			{
				newObject = topObject;
			}
		}
		Actionable.m_ReusableActionFromObject.Init(newObject, UseObjectType, Requester, NewActionType, ActionRequirement, default(TileCoord));
		GetClippedTileCoordArea(TopLeftIn, BottomRightIn, out var TopLeft, out var BottomRight);
		TileCoord tileCoord = Requester.GetComponent<TileCoordObject>().m_TileCoord;
		int num = (BottomRight.x + TopLeft.x) / 2;
		int num2;
		int x;
		int x2;
		if (tileCoord.x < num)
		{
			num2 = 1;
			x = TopLeft.x;
			x2 = BottomRight.x;
		}
		else
		{
			num2 = -1;
			x = BottomRight.x;
			x2 = TopLeft.x;
		}
		int num3 = (BottomRight.y + TopLeft.y) / 2;
		int num4;
		int y;
		int y2;
		if (tileCoord.y < num3)
		{
			num4 = 1;
			y = TopLeft.y;
			y2 = BottomRight.y;
		}
		else
		{
			num4 = -1;
			y = BottomRight.y;
			y2 = TopLeft.y;
		}
		x2 += num2;
		y2 += num4;
		TileCoord tileCoord2 = default(TileCoord);
		TileCoord item = default(TileCoord);
		float num5 = 1E+08f;
		bool flag = false;
		int num6 = 0;
		for (int i = y; i != y2; i += num4)
		{
			tileCoord2.y = i;
			int num7 = 0;
			for (int j = x; j != x2; j += num2)
			{
				bool flag2 = true;
				switch (NewFindType)
				{
				case HighInstruction.FindType.Even:
					if (((uint)num6 & (true ? 1u : 0u)) != 0 || ((uint)num7 & (true ? 1u : 0u)) != 0)
					{
						flag2 = false;
					}
					break;
				case HighInstruction.FindType.HStripes:
					if (((uint)num6 & (true ? 1u : 0u)) != 0)
					{
						flag2 = false;
					}
					break;
				case HighInstruction.FindType.VStripes:
					if (((uint)num7 & (true ? 1u : 0u)) != 0)
					{
						flag2 = false;
					}
					break;
				case HighInstruction.FindType.Checkers:
					if (((uint)(num6 + num7) & (true ? 1u : 0u)) != 0)
					{
						flag2 = false;
					}
					break;
				}
				if (flag2)
				{
					tileCoord2.x = j;
					Plot plotAtTile = PlotManager.Instance.GetPlotAtTile(tileCoord2);
					if (plotAtTile.m_Visible)
					{
						Tile tile = TileManager.Instance.GetTile(tileCoord2);
						if (NewTypes.ContainsKey(tile.m_TileType))
						{
							float num8 = (tileCoord - tileCoord2).MagnitudeSqr();
							if (num8 < num5)
							{
								Actionable.m_ReusableActionFromObject.m_Position = tileCoord2;
								ActionType actionFromObjectSafe = plotAtTile.GetActionFromObjectSafe(Actionable.m_ReusableActionFromObject);
								if (actionFromObjectSafe != ActionType.Total && actionFromObjectSafe != ActionType.Fail && !BaggedManager.Instance.IsTileBagged(tileCoord2))
								{
									item = tileCoord2;
									flag = true;
									num5 = num8;
								}
							}
						}
					}
				}
				num7++;
			}
			num6++;
		}
		if (flag)
		{
			list.Add(item);
		}
		return list;
	}

	public static List<TileCoord> GetTilesOfTypes(Dictionary<Tile.TileType, int> NewTypes, TileCoord TopLeftIn, TileCoord BottomRightIn, HighInstruction.FindType NewFindType, Actionable Requester, ObjectType UseObjectType, AFO.AT NewActionType, string ActionRequirement)
	{
		List<TileCoord> list = new List<TileCoord>();
		Holdable newObject = null;
		if ((bool)Requester && (bool)Requester.GetComponent<Farmer>())
		{
			Holdable topObject = Requester.GetComponent<Farmer>().m_FarmerCarry.GetTopObject();
			if ((bool)topObject && topObject.m_TypeIdentifier == UseObjectType)
			{
				newObject = topObject;
			}
		}
		Actionable.m_ReusableActionFromObject.Init(newObject, UseObjectType, Requester, NewActionType, ActionRequirement, default(TileCoord));
		GetClippedTileCoordArea(TopLeftIn, BottomRightIn, out var TopLeft, out var BottomRight);
		_ = Requester.GetComponent<TileCoordObject>().m_TileCoord;
		TileCoord tileCoord = default(TileCoord);
		int num = 0;
		for (int i = TopLeft.y; i <= BottomRight.y; i++)
		{
			tileCoord.y = i;
			int num2 = 0;
			for (int j = TopLeft.x; j <= BottomRight.x; j++)
			{
				bool flag = true;
				switch (NewFindType)
				{
				case HighInstruction.FindType.Even:
					if (((uint)num & (true ? 1u : 0u)) != 0 || ((uint)num2 & (true ? 1u : 0u)) != 0)
					{
						flag = false;
					}
					break;
				case HighInstruction.FindType.HStripes:
					if (((uint)num & (true ? 1u : 0u)) != 0)
					{
						flag = false;
					}
					break;
				case HighInstruction.FindType.VStripes:
					if (((uint)num2 & (true ? 1u : 0u)) != 0)
					{
						flag = false;
					}
					break;
				case HighInstruction.FindType.Checkers:
					if (((uint)(num + num2) & (true ? 1u : 0u)) != 0)
					{
						flag = false;
					}
					break;
				}
				if (flag)
				{
					tileCoord.x = j;
					Plot plotAtTile = PlotManager.Instance.GetPlotAtTile(tileCoord);
					if (plotAtTile.m_Visible)
					{
						Tile tile = TileManager.Instance.GetTile(tileCoord);
						if (NewTypes.ContainsKey(tile.m_TileType))
						{
							Actionable.m_ReusableActionFromObject.m_Position = tileCoord;
							ActionType actionFromObjectSafe = plotAtTile.GetActionFromObjectSafe(Actionable.m_ReusableActionFromObject);
							if (actionFromObjectSafe != ActionType.Total && actionFromObjectSafe != ActionType.Fail && !BaggedManager.Instance.IsTileBagged(tileCoord))
							{
								list.Add(tileCoord);
							}
						}
					}
				}
				num2++;
			}
			num++;
		}
		return list;
	}

	public static List<TileCoord> GetRandomTilesOfType(Tile.TileType NewType, TileCoord TopLeftIn, TileCoord BottomRightIn, Actionable Requester, ObjectType UseObjectType, AFO.AT NewActionType, string ActionRequirement, int NumTiles)
	{
		List<TileCoord> list = new List<TileCoord>();
		Holdable newObject = null;
		if ((bool)Requester && (bool)Requester.GetComponent<Farmer>())
		{
			Holdable topObject = Requester.GetComponent<Farmer>().m_FarmerCarry.GetTopObject();
			if ((bool)topObject && topObject.m_TypeIdentifier == UseObjectType)
			{
				newObject = topObject;
			}
		}
		Actionable.m_ReusableActionFromObject.Init(newObject, UseObjectType, Requester, NewActionType, ActionRequirement, default(TileCoord));
		GetClippedTileCoordArea(TopLeftIn, BottomRightIn, out var TopLeft, out var BottomRight);
		TileCoord tileCoord = default(TileCoord);
		int num = 0;
		int num2 = 50;
		do
		{
			int y = Random.Range(TopLeft.y, BottomRight.y + 1);
			int x = Random.Range(TopLeft.x, BottomRight.x + 1);
			tileCoord.y = y;
			tileCoord.x = x;
			Plot plotAtTile = PlotManager.Instance.GetPlotAtTile(tileCoord);
			if (plotAtTile.m_Visible && TileManager.Instance.GetTile(tileCoord).m_TileType == NewType)
			{
				Actionable.m_ReusableActionFromObject.m_Position = tileCoord;
				ActionType actionFromObjectSafe = plotAtTile.GetActionFromObjectSafe(Actionable.m_ReusableActionFromObject);
				if (actionFromObjectSafe != ActionType.Total && actionFromObjectSafe != ActionType.Fail && !BaggedManager.Instance.IsTileBagged(tileCoord))
				{
					list.Add(tileCoord);
					if (list.Count == NumTiles)
					{
						return list;
					}
				}
			}
			num++;
		}
		while (num < num2);
		return list;
	}

	public static List<TileCoord> GetRandomTilesOfTypeInRange(Tile.TileType NewType, TileCoord OldPosition, int Range, Actionable Requester, ObjectType UseObjectType, AFO.AT NewActionType, string ActionRequirement, int NumTiles)
	{
		GetClippedTileCoordArea(OldPosition + new TileCoord(-Range, -Range), OldPosition + new TileCoord(Range, Range), out var TopLeft, out var BottomRight);
		return GetRandomTilesOfType(NewType, TopLeft, BottomRight, Requester, UseObjectType, NewActionType, ActionRequirement, NumTiles);
	}

	public static bool GetTileWater(Tile.TileType NewType)
	{
		if (NewType == Tile.TileType.WaterDeep || NewType == Tile.TileType.SeaWaterDeep || NewType == Tile.TileType.WaterShallow || NewType == Tile.TileType.SeaWaterShallow || NewType == Tile.TileType.Swamp)
		{
			return true;
		}
		return false;
	}

	public static bool GetTileWaterDrinkable(Tile.TileType NewType)
	{
		if (NewType == Tile.TileType.WaterDeep || NewType == Tile.TileType.WaterShallow)
		{
			return true;
		}
		return false;
	}

	public static bool GetTileWaterCollectable(Tile.TileType NewType)
	{
		if (NewType == Tile.TileType.WaterShallow || NewType == Tile.TileType.SeaWaterShallow)
		{
			return true;
		}
		return false;
	}

	public static bool GetTileWaterShallow(Tile.TileType NewType)
	{
		if (NewType == Tile.TileType.WaterShallow || NewType == Tile.TileType.SeaWaterShallow)
		{
			return true;
		}
		return false;
	}

	public static bool GetTileWaterDeep(Tile.TileType NewType)
	{
		if (NewType == Tile.TileType.WaterDeep || NewType == Tile.TileType.SeaWaterDeep)
		{
			return true;
		}
		return false;
	}
}
