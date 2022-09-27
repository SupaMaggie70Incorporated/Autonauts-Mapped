using System.Collections.Generic;

public class WalledArea
{
	public ObjectType m_WallType;

	public List<int> m_Tiles;

	public int m_PennedCows;

	public int m_PennedSheep;

	public int m_PennedChickens;

	public WalledArea(List<int> Tiles, ObjectType WallType)
	{
		m_Tiles = Tiles;
		m_WallType = WallType;
		foreach (int Tile in Tiles)
		{
			TileManager.Instance.m_Tiles[Tile].m_WalledArea = this;
			TileManager.Instance.UpdateTile(new TileCoord(Tile));
		}
		foreach (int Tile2 in Tiles)
		{
			Tile tile = TileManager.Instance.m_Tiles[Tile2];
			if ((bool)tile.m_Building)
			{
				tile.m_Building.CheckWallsFloors();
			}
		}
		CheckAreaCreated();
	}

	public void Destroy()
	{
		foreach (int tile2 in m_Tiles)
		{
			Tile tile = TileManager.Instance.m_Tiles[tile2];
			tile.m_WalledArea = null;
			TileManager.Instance.UpdateTile(new TileCoord(tile2));
			if ((bool)tile.m_Building)
			{
				tile.m_Building.CheckWallsFloors();
			}
		}
	}

	public void CheckForCowsOrSheep()
	{
		m_PennedCows = 0;
		m_PennedSheep = 0;
		m_PennedChickens = 0;
		foreach (int tile in m_Tiles)
		{
			TileCoord position = new TileCoord(tile);
			Plot plotAtTile = PlotManager.Instance.GetPlotAtTile(position);
			List<TileCoordObject> objectsTypeAtTile = plotAtTile.GetObjectsTypeAtTile(ObjectType.AnimalCow, position);
			m_PennedCows += objectsTypeAtTile.Count;
			objectsTypeAtTile = plotAtTile.GetObjectsTypeAtTile(ObjectType.AnimalCowHighland, position);
			m_PennedCows += objectsTypeAtTile.Count;
			objectsTypeAtTile = plotAtTile.GetObjectsTypeAtTile(ObjectType.AnimalSheep, position);
			m_PennedSheep += objectsTypeAtTile.Count;
			objectsTypeAtTile = plotAtTile.GetObjectsTypeAtTile(ObjectType.AnimalAlpaca, position);
			m_PennedSheep += objectsTypeAtTile.Count;
			objectsTypeAtTile = plotAtTile.GetObjectsTypeAtTile(ObjectType.AnimalChicken, position);
			m_PennedChickens += objectsTypeAtTile.Count;
		}
		WalledAreaManager.Instance.CheckPennedAnimals();
	}

	private void CheckAreaCreated()
	{
		CheckForCowsOrSheep();
	}
}
