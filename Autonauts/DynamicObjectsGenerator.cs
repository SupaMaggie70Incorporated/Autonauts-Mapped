using UnityEngine;

public class DynamicObjectsGenerator : MonoBehaviour
{
	private static void AddObjectsNearPlayer(TileCoord PlayerStart, ObjectType NewType, int Count)
	{
		bool flag = false;
		int num = 0;
		for (int i = 0; i < Count; i++)
		{
			do
			{
				flag = true;
				TileCoord position = new TileCoord(Random.Range(PlayerStart.x - 5, PlayerStart.x + 5), Random.Range(PlayerStart.y - 5, PlayerStart.y + 5));
				if (position.x >= 0 && position.x < TileManager.Instance.m_TilesWide && position.y >= 0 && position.y < TileManager.Instance.m_TilesHigh)
				{
					Tile tile = TileManager.Instance.GetTile(position);
					if (!Tile.m_TileInfo[(int)tile.m_TileType].m_Solid && tile.m_AssociatedObject == null)
					{
						ObjectTypeList.Instance.CreateObjectFromIdentifier(NewType, position.ToWorldPositionTileCentered(), Quaternion.identity);
						flag = false;
					}
				}
				num++;
			}
			while (flag && num < 100);
		}
	}

	private static TileCoord GetBoulderLocationNearby(TileCoord PlayerStart)
	{
		bool flag = false;
		int num = 0;
		do
		{
			flag = true;
			TileCoord tileCoord = new TileCoord(Random.Range(10, 15), Random.Range(7, 10));
			if (Random.Range(0, 2) == 0)
			{
				tileCoord.x = -tileCoord.x;
			}
			if (Random.Range(0, 2) == 0)
			{
				tileCoord.y = -tileCoord.y;
			}
			tileCoord += PlayerStart;
			if (tileCoord.x >= 0 && tileCoord.x < TileManager.Instance.m_TilesWide && tileCoord.y >= 0 && tileCoord.y < TileManager.Instance.m_TilesHigh && !Tile.m_TileInfo[(int)TileManager.Instance.GetTileType(tileCoord)].m_Solid)
			{
				return tileCoord;
			}
			num++;
		}
		while (flag && num < 100);
		return default(TileCoord);
	}

	private static void AddBoulderNearPlayer(TileCoord PlayerStart)
	{
		TileCoord position = TileResourceGenerator.m_NearbyStonesPosition;
		if (position.x == 0 && position.y == 0)
		{
			position = GetBoulderLocationNearby(PlayerStart);
		}
		if (position.x != 0 || position.y != 0)
		{
			TilesToObjectsConverter.RemoveObjectsAtTileCoord(position);
			ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Boulder, position.ToWorldPositionTileCentered(), Quaternion.identity);
		}
	}

	private static void AddGuaranteedObjects(TileCoord PlayerStart)
	{
		AddObjectsNearPlayer(PlayerStart, ObjectType.Stick, 5);
		AddObjectsNearPlayer(PlayerStart, ObjectType.Rock, 10);
		AddBoulderNearPlayer(PlayerStart);
	}

	private static void AddPlayer(TileCoord PlayerStart)
	{
		ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.FarmerPlayer, PlayerStart.ToWorldPositionTileCentered(), Quaternion.identity).GetComponent<Farmer>();
		TilesToObjectsConverter.RemoveObjectsAtTileCoord(PlayerStart);
	}

	private static void AddRocket(TileCoord RocketPosition)
	{
		Plot plotAtTile = PlotManager.Instance.GetPlotAtTile(RocketPosition);
		TilesToObjectsConverter.RemoveObjectsAtTileCoord(RocketPosition);
		TilesToObjectsConverter.RemoveObjectsAtTileCoord(RocketPosition + new TileCoord(1, 0));
		TilesToObjectsConverter.RemoveObjectsAtTileCoord(RocketPosition + new TileCoord(0, 1));
		TileManager.Instance.SetTileType(RocketPosition, Tile.TileType.Empty);
		plotAtTile.SetVisible(Visible: true);
	}

	public static void AddDynamicObjects()
	{
		if (SaveLoadManager.m_MiniMap)
		{
			for (int i = 0; i < PlotManager.Instance.m_PlotsHigh; i++)
			{
				for (int j = 0; j < PlotManager.Instance.m_PlotsWide; j++)
				{
					PlotManager.Instance.GetPlotAtPlot(j, i).SetVisible(Visible: true);
				}
			}
			for (int k = 0; k < 4; k++)
			{
				FailSafeManager.Instance.UpdateCows(4);
			}
			for (int l = 0; l < 4; l++)
			{
				FailSafeManager.Instance.UpdateSheep(4);
			}
			AddPlayer(new TileCoord(TileManager.Instance.m_TilesWide - 1, TileManager.Instance.m_TilesHigh - 1));
			return;
		}
		for (int m = 0; m < 4; m++)
		{
			FailSafeManager.Instance.UpdateBeeNests(4);
		}
		for (int n = 0; n < 10; n++)
		{
			FailSafeManager.Instance.UpdateCows(10);
		}
		for (int num = 0; num < 10; num++)
		{
			FailSafeManager.Instance.UpdateSheep(10);
		}
		for (int num2 = 0; num2 < 10; num2++)
		{
			FailSafeManager.Instance.UpdateChickens(10);
		}
		TileCoord playerPosition = GameOptionsManager.Instance.m_Options.m_PlayerPosition;
		AddGuaranteedObjects(playerPosition);
		PlotManager.Instance.GetPlotAtTile(playerPosition.x, playerPosition.y).SetVisible(Visible: true);
		AddPlayer(playerPosition);
		TileCoord rocketPosition = playerPosition + Transmitter.m_StartingOffsetFromPlayer;
		AddRocket(rocketPosition);
		Transmitter component = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Transmitter, rocketPosition.ToWorldPositionTileCentered(), Quaternion.identity).GetComponent<Transmitter>();
		MapManager.Instance.AddBuilding(component);
		if (GameOptionsManager.Instance.m_Options.m_TutorialEnabled)
		{
			TileCoord tileCoord = playerPosition + new TileCoord(2, 0);
			ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.TutorBot, tileCoord.ToWorldPositionTileCentered(), Quaternion.identity).gameObject.SetActive(value: false);
		}
	}
}
