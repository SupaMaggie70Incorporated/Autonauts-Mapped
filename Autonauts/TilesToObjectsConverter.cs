using UnityEngine;

public class TilesToObjectsConverter : MonoBehaviour
{
	public static void RemoveObjectsAtTileCoord(TileCoord Position)
	{
		foreach (TileCoordObject item in PlotManager.Instance.GetPlotAtTile(Position).GetObjectsTypeAtTile(Position))
		{
			if (item.GetComponent<Farmer>() == null)
			{
				item.StopUsing();
			}
		}
	}

	private static void MakeBush(int x, int y, TileCoord Position)
	{
		TileManager.Instance.SetTileType(Position, Tile.TileType.Soil);
		ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Bush, Position.ToWorldPositionTileCentered(), Quaternion.identity).GetComponent<Bush>().WorldCreated();
		int num = Random.Range(0, 2);
		for (int i = 0; i < num; i++)
		{
			TileCoord cappedTileCoord = TileManager.Instance.GetCappedTileCoord(Position + new TileCoord(Random.Range(-1, 2), Random.Range(-1, 2)));
			Tile tile = TileManager.Instance.GetTile(cappedTileCoord);
			if (tile.m_TileType == Tile.TileType.Empty && tile.m_AssociatedObject == null)
			{
				TileManager.Instance.SetTileType(cappedTileCoord, Tile.TileType.Soil);
				ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Bush, cappedTileCoord.ToWorldPositionTileCentered(), Quaternion.identity).GetComponent<Bush>().WorldCreated();
			}
		}
	}

	private static void MakeMushroom(int x, int y, TileCoord Position)
	{
		TileManager.Instance.SetTileType(Position, Tile.TileType.Soil);
		ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Mushroom, Position.ToWorldPositionTileCentered(), Quaternion.identity).GetComponent<Mushroom>().WorldCreated();
		int num = Random.Range(0, 2);
		for (int i = 0; i < num; i++)
		{
			TileCoord cappedTileCoord = TileManager.Instance.GetCappedTileCoord(Position + new TileCoord(Random.Range(-1, 2), Random.Range(-1, 2)));
			Tile tile = TileManager.Instance.GetTile(cappedTileCoord);
			if (tile.m_TileType == Tile.TileType.Empty && tile.m_AssociatedObject == null)
			{
				TileManager.Instance.SetTileType(cappedTileCoord, Tile.TileType.Soil);
				ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Mushroom, cappedTileCoord.ToWorldPositionTileCentered(), Quaternion.identity).GetComponent<Mushroom>().WorldCreated();
			}
		}
	}

	private static void MakeWildflower(int x, int y, TileCoord Position)
	{
		TileManager.Instance.SetTileType(Position, Tile.TileType.Soil);
		int type = Random.Range(0, 7);
		FlowerWild component = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.FlowerWild, Position.ToWorldPositionTileCentered(), Quaternion.identity).GetComponent<FlowerWild>();
		component.WorldCreated();
		component.SetType((FlowerWild.Type)type);
		int num = Random.Range(0, 2);
		for (int i = 0; i < num; i++)
		{
			TileCoord cappedTileCoord = TileManager.Instance.GetCappedTileCoord(Position + new TileCoord(Random.Range(-1, 2), Random.Range(-1, 2)));
			Tile tile = TileManager.Instance.GetTile(cappedTileCoord);
			if (tile.m_TileType == Tile.TileType.Empty && tile.m_AssociatedObject == null)
			{
				TileManager.Instance.SetTileType(cappedTileCoord, Tile.TileType.Soil);
				FlowerWild component2 = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.FlowerWild, cappedTileCoord.ToWorldPositionTileCentered(), Quaternion.identity).GetComponent<FlowerWild>();
				component2.SetType((FlowerWild.Type)type);
				component2.WorldCreated();
			}
		}
	}

	private static void CreateObjectsOnEmptyTile(int x, int y, TileCoord Position)
	{
		if (TileManager.Instance.GetTileType(new TileCoord(x - 1, y)) == Tile.TileType.Trees || TileManager.Instance.GetTileType(new TileCoord(x + 1, y)) == Tile.TileType.Trees || TileManager.Instance.GetTileType(new TileCoord(x, y - 1)) == Tile.TileType.Trees || TileManager.Instance.GetTileType(new TileCoord(x, y + 1)) == Tile.TileType.Trees)
		{
			if (Random.Range(0, 5) == 0)
			{
				ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Stick, Position.ToWorldPositionTileCentered(), Quaternion.identity);
			}
		}
		else if (Random.Range(0, 100) == 0)
		{
			MakeBush(x, y, Position);
		}
		else if (Random.Range(0, 300) == 0)
		{
			MakeMushroom(x, y, Position);
		}
		else if (Random.Range(0, 400) == 0)
		{
			MakeWildflower(x, y, Position);
		}
	}

	public static void CreateObjectsOnCropWheat(int x, int y, TileCoord Position)
	{
		TileManager.Instance.SetTileType(Position, Tile.TileType.Soil);
		CropWheat component = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.CropWheat, Position.ToWorldPositionTileCentered(), Quaternion.identity).GetComponent<CropWheat>();
		component.SetState(Crop.State.Wild);
		component.WorldCreated();
	}

	public static void CreateObjectsOnCropCotton(int x, int y, TileCoord Position)
	{
		TileManager.Instance.SetTileType(Position, Tile.TileType.Soil);
		CropCotton component = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.CropCotton, Position.ToWorldPositionTileCentered(), Quaternion.identity).GetComponent<CropCotton>();
		component.SetState(Crop.State.Wild);
		component.WorldCreated();
	}

	private static void CreateObjectsOnGrass(int x, int y, TileCoord Position)
	{
		ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Grass, Position.ToWorldPositionTileCentered(), Quaternion.identity).GetComponent<Grass>().WorldCreated();
	}

	private static void CreateObjectsOnWeeds(int x, int y, TileCoord Position)
	{
		TileManager.Instance.SetTileType(Position, Tile.TileType.Soil);
		ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Weed, Position.ToWorldPositionTileCentered(), Quaternion.identity).GetComponent<Weed>().WorldCreated();
	}

	private static void CreateObjectsOnLumpy(int x, int y, TileCoord Position)
	{
		TallBoulder component = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.TallBoulder, Position.ToWorldPositionTileCentered(), Quaternion.identity).GetComponent<TallBoulder>();
		if (TileManager.Instance.GetTileType(Position + new TileCoord(0, -1)) == Tile.TileType.Raised && TileManager.Instance.GetTileType(Position + new TileCoord(0, 1)) == Tile.TileType.Raised && TileManager.Instance.GetTileType(Position + new TileCoord(-1, 0)) == Tile.TileType.Raised && TileManager.Instance.GetTileType(Position + new TileCoord(1, 0)) == Tile.TileType.Raised && TileManager.Instance.GetTileType(Position + new TileCoord(-1, -1)) == Tile.TileType.Raised && TileManager.Instance.GetTileType(Position + new TileCoord(1, 1)) == Tile.TileType.Raised && TileManager.Instance.GetTileType(Position + new TileCoord(-1, 1)) == Tile.TileType.Raised && TileManager.Instance.GetTileType(Position + new TileCoord(1, -1)) == Tile.TileType.Raised)
		{
			if (TileManager.Instance.GetTileType(Position + new TileCoord(0, -2)) == Tile.TileType.Raised && TileManager.Instance.GetTileType(Position + new TileCoord(0, 2)) == Tile.TileType.Raised && TileManager.Instance.GetTileType(Position + new TileCoord(-2, 0)) == Tile.TileType.Raised && TileManager.Instance.GetTileType(Position + new TileCoord(2, 0)) == Tile.TileType.Raised)
			{
				component.SetSize(2);
			}
			else
			{
				component.SetSize(1);
			}
		}
	}

	private static void CreateObjectsOnSwamp(int x, int y, TileCoord Position)
	{
		if (TileManager.Instance.GetTileType(Position + new TileCoord(0, -1)) == Tile.TileType.Empty || TileManager.Instance.GetTileType(Position + new TileCoord(0, 1)) == Tile.TileType.Empty || TileManager.Instance.GetTileType(Position + new TileCoord(-1, 0)) == Tile.TileType.Empty || TileManager.Instance.GetTileType(Position + new TileCoord(1, 0)) == Tile.TileType.Empty)
		{
			ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Bullrushes, Position.ToWorldPositionTileCentered(), Quaternion.identity).GetComponent<Bullrushes>().WorldCreated();
		}
	}

	private static void CreateObjectsOnTrees(int x, int y, TileCoord Position)
	{
		if (Random.Range(0, 6) == 0)
		{
			if (Random.Range(0, 5) == 0)
			{
				ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Log, Position.ToWorldPositionTileCentered(), Quaternion.identity);
				TileManager.Instance.SetTileType(Position, Tile.TileType.Soil);
			}
			else if (Random.Range(0, 10) == 0)
			{
				ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.TreeStump, Position.ToWorldPositionTileCentered(), Quaternion.identity);
				TileManager.Instance.SetTileType(Position, Tile.TileType.Soil);
			}
			else
			{
				ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.TreePine, Position.ToWorldPositionTileCentered(), Quaternion.identity).GetComponent<MyTree>().WorldCreated();
			}
		}
		else if (Random.Range(0, 40) == 0)
		{
			ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.TreeSeed, Position.ToWorldPositionTileCentered(), Quaternion.identity);
			TileManager.Instance.SetTileType(Position, Tile.TileType.Soil);
		}
		else if (Random.Range(0, 40) == 0)
		{
			ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Stick, Position.ToWorldPositionTileCentered(), Quaternion.identity);
			TileManager.Instance.SetTileType(Position, Tile.TileType.Soil);
		}
		else
		{
			TileManager.Instance.SetTileType(Position, Tile.TileType.Soil);
		}
	}

	private static void CreateObjectsOnStones(int x, int y, TileCoord Position)
	{
		if (Random.Range(0, 40) == 0)
		{
			ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Boulder, Position.ToWorldPositionTileCentered(), Quaternion.identity);
		}
		else if (Random.Range(0, 15) == 0)
		{
			ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Rock, Position.ToWorldPositionTileCentered(), Quaternion.identity);
		}
	}

	private static void CreateObjectsOnIron(int x, int y, TileCoord Position)
	{
		if (Random.Range(0, 25) == 0)
		{
			ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.IronOre, Position.ToWorldPositionTileCentered(), Quaternion.identity);
		}
	}

	private static void CreateObjectsOnClay(int x, int y, TileCoord Position)
	{
		if (Random.Range(0, 15) == 0)
		{
			ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Clay, Position.ToWorldPositionTileCentered(), Quaternion.identity);
		}
	}

	private static void CreateObjectsOnSand(int x, int y, TileCoord Position)
	{
		if (TileManager.Instance.GetTileType(Position + new TileCoord(1, 0)) == Tile.TileType.SeaWaterShallow || TileManager.Instance.GetTileType(Position + new TileCoord(-1, 0)) == Tile.TileType.SeaWaterShallow || TileManager.Instance.GetTileType(Position + new TileCoord(0, 1)) == Tile.TileType.SeaWaterShallow || TileManager.Instance.GetTileType(Position + new TileCoord(0, -1)) == Tile.TileType.SeaWaterShallow)
		{
			if (Random.Range(0, 15) == 0)
			{
				ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Stick, Position.ToWorldPositionTileCentered(), Quaternion.identity);
			}
			else if (Random.Range(0, 15) == 0)
			{
				ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Rock, Position.ToWorldPositionTileCentered(), Quaternion.identity);
			}
		}
	}

	private static void CreateObjectsOnShallowSeaWater(int x, int y, TileCoord Position)
	{
		if (Random.Range(0, 500) == 0)
		{
			ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Boulder, Position.ToWorldPositionTileCentered(), Quaternion.identity);
		}
	}

	public static void ConvertTempTilesToObjects()
	{
		for (int i = 0; i < TileManager.Instance.m_TilesHigh; i++)
		{
			for (int j = 0; j < TileManager.Instance.m_TilesWide; j++)
			{
				TileCoord position = new TileCoord(j, i);
				switch (TileManager.Instance.GetTile(position).m_TileType)
				{
				case Tile.TileType.Empty:
					CreateObjectsOnEmptyTile(j, i, position);
					break;
				case Tile.TileType.CropWheat:
					CreateObjectsOnCropWheat(j, i, position);
					break;
				case Tile.TileType.CropCotton:
					CreateObjectsOnCropCotton(j, i, position);
					break;
				case Tile.TileType.Grass:
					CreateObjectsOnGrass(j, i, position);
					break;
				case Tile.TileType.Weeds:
					CreateObjectsOnWeeds(j, i, position);
					break;
				case Tile.TileType.Raised:
					CreateObjectsOnLumpy(j, i, position);
					break;
				case Tile.TileType.Swamp:
					CreateObjectsOnSwamp(j, i, position);
					break;
				case Tile.TileType.Trees:
					CreateObjectsOnTrees(j, i, position);
					break;
				case Tile.TileType.StoneHidden:
				case Tile.TileType.StoneSoil:
					CreateObjectsOnStones(j, i, position);
					break;
				case Tile.TileType.IronHidden:
				case Tile.TileType.IronSoil:
					CreateObjectsOnIron(j, i, position);
					break;
				case Tile.TileType.ClayHidden:
				case Tile.TileType.ClaySoil:
					CreateObjectsOnClay(j, i, position);
					break;
				case Tile.TileType.Sand:
					CreateObjectsOnSand(j, i, position);
					break;
				case Tile.TileType.SeaWaterShallow:
					CreateObjectsOnShallowSeaWater(j, i, position);
					break;
				}
			}
		}
	}

	public static void RemoveTempTiles()
	{
		for (int i = 0; i < TileManager.Instance.m_TilesHigh; i++)
		{
			for (int j = 0; j < TileManager.Instance.m_TilesWide; j++)
			{
				TileCoord position = new TileCoord(j, i);
				Tile.TileType tileType = TileManager.Instance.GetTileType(position);
				if (tileType == Tile.TileType.Trees || tileType == Tile.TileType.CropWheat || tileType == Tile.TileType.CropCotton || tileType == Tile.TileType.Weeds || tileType == Tile.TileType.Grass || tileType == Tile.TileType.Raised)
				{
					TileManager.Instance.SetTileType(position, Tile.TileType.Soil);
				}
			}
		}
	}
}
