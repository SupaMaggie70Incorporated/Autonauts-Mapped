using System.Collections.Generic;
using UnityEngine;

public class FailSafeManager : MonoBehaviour
{
	private static int m_MinBeeNests = 4;

	private static int m_MinCows = 4;

	private static int m_MinSheep = 4;

	private static int m_MinChickens = 4;

	private static int m_MinBirds = 9;

	private static int m_MinPumpkins = 20;

	private static int m_MinAppleTrees = 40;

	private static int m_MinFlowers = 20;

	public static FailSafeManager Instance;

	private int[] m_VisibleObjectCounts;

	private float m_RockTimer;

	private float m_ClayTimer;

	private float m_IronOreTimer;

	private float m_TreeTimer;

	private float m_CropWheatTimer;

	private float m_CropCottonTimer;

	private float m_BullrushesTimer;

	private bool m_CropWheatRemoved;

	private List<TileCoord> m_CropWheatTiles;

	private bool m_CropCottonRemoved;

	private List<TileCoord> m_CropCottonTiles;

	private bool m_BullrushesRemoved;

	private List<TileCoord> m_BullrushesTiles;

	private bool m_TreeSeen;

	private bool m_ClaySeen;

	private bool m_IronSeen;

	private int[] m_FlowerCounts;

	private List<FlowerWild> m_NewFlowers;

	private bool m_Disabled;

	private void Awake()
	{
		Instance = this;
		m_Disabled = false;
		if ((bool)ModManager.Instance)
		{
			m_Disabled = ModManager.Instance.FailSafeDisabled;
		}
		m_VisibleObjectCounts = new int[(int)ObjectTypeList.m_Total];
		m_RockTimer = 0f;
		m_ClayTimer = 0f;
		m_IronOreTimer = 0f;
		m_TreeTimer = 0f;
		m_CropWheatTimer = 0f;
		m_CropWheatRemoved = false;
		m_CropWheatTiles = new List<TileCoord>();
		m_CropCottonRemoved = false;
		m_CropCottonTiles = new List<TileCoord>();
		m_BullrushesRemoved = false;
		m_BullrushesTiles = new List<TileCoord>();
		m_FlowerCounts = new int[7];
		m_NewFlowers = new List<FlowerWild>();
	}

	public void AddVisibleType(ObjectType NewType)
	{
		m_VisibleObjectCounts[(int)NewType]++;
	}

	public void RemoveVisibleType(ObjectType NewType)
	{
		if (m_VisibleObjectCounts[(int)NewType] > 0)
		{
			m_VisibleObjectCounts[(int)NewType]--;
		}
	}

	private void UpdateRocks()
	{
		if (m_VisibleObjectCounts[246] == 0)
		{
			m_RockTimer += TimeManager.Instance.m_NormalDelta;
			if (!(m_RockTimer > 10f))
			{
				return;
			}
			List<Boulder> list = new List<Boulder>();
			foreach (Plot visiblePlot in PlotManager.Instance.m_VisiblePlots)
			{
				foreach (KeyValuePair<int, TileCoordObject> @object in visiblePlot.m_Objects)
				{
					TileCoordObject value = @object.Value;
					if (value.m_TypeIdentifier == ObjectType.Boulder)
					{
						list.Add(value.GetComponent<Boulder>());
					}
				}
			}
			if (list.Count > 0)
			{
				list[Random.Range(0, list.Count)].CreateRock(Ore: false);
			}
		}
		else
		{
			m_RockTimer = 0f;
		}
	}

	public void NewPlotVisible(Plot NewPlot)
	{
		for (int i = NewPlot.m_TileY; i < NewPlot.m_TileY + Plot.m_PlotTilesHigh; i++)
		{
			for (int j = NewPlot.m_TileX; j < NewPlot.m_TileX + Plot.m_PlotTilesWide; j++)
			{
				TileCoord position = new TileCoord(j, i);
				Tile tile = TileManager.Instance.GetTile(position);
				if (tile.m_TileType == Tile.TileType.ClayHidden || tile.m_TileType == Tile.TileType.ClaySoil || tile.m_TileType == Tile.TileType.ClayUsed || tile.m_TileType == Tile.TileType.Clay)
				{
					m_ClaySeen = true;
				}
				else if (tile.m_TileType == Tile.TileType.IronHidden || tile.m_TileType == Tile.TileType.IronSoil || tile.m_TileType == Tile.TileType.IronSoil2 || tile.m_TileType == Tile.TileType.IronUsed || tile.m_TileType == Tile.TileType.Iron)
				{
					m_IronSeen = true;
				}
				if ((bool)tile.m_AssociatedObject && tile.m_AssociatedObject.m_TypeIdentifier == ObjectType.TreePine)
				{
					m_TreeSeen = true;
				}
			}
		}
	}

	private void UpdateClay()
	{
		if (m_VisibleObjectCounts[253] == 0 && m_ClaySeen)
		{
			m_ClayTimer += TimeManager.Instance.m_NormalDelta;
			if (!(m_ClayTimer > 10f))
			{
				return;
			}
			List<TileCoord> list = new List<TileCoord>();
			foreach (Plot visiblePlot in PlotManager.Instance.m_VisiblePlots)
			{
				for (int i = visiblePlot.m_TileY; i < visiblePlot.m_TileY + Plot.m_PlotTilesHigh; i++)
				{
					for (int j = visiblePlot.m_TileX; j < visiblePlot.m_TileX + Plot.m_PlotTilesWide; j++)
					{
						TileCoord tileCoord = new TileCoord(j, i);
						Tile tile = TileManager.Instance.GetTile(tileCoord);
						if ((tile.m_TileType == Tile.TileType.ClaySoil || tile.m_TileType == Tile.TileType.Clay) && !tile.GetContainsObject())
						{
							list.Add(tileCoord);
						}
					}
				}
			}
			if (list.Count > 0)
			{
				TileCoord tileCoord2 = list[Random.Range(0, list.Count)];
				ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Clay, tileCoord2.ToWorldPositionTileCentered(), Quaternion.identity).WorldCreated();
			}
		}
		else
		{
			m_ClayTimer = 0f;
		}
	}

	private void UpdateIronOre()
	{
		if (m_VisibleObjectCounts[232] == 0 && m_IronSeen)
		{
			m_IronOreTimer += TimeManager.Instance.m_NormalDelta;
			if (!(m_IronOreTimer > 10f))
			{
				return;
			}
			List<TileCoord> list = new List<TileCoord>();
			foreach (Plot visiblePlot in PlotManager.Instance.m_VisiblePlots)
			{
				for (int i = visiblePlot.m_TileY; i < visiblePlot.m_TileY + Plot.m_PlotTilesHigh; i++)
				{
					for (int j = visiblePlot.m_TileX; j < visiblePlot.m_TileX + Plot.m_PlotTilesWide; j++)
					{
						TileCoord tileCoord = new TileCoord(j, i);
						Tile tile = TileManager.Instance.GetTile(tileCoord);
						if ((tile.m_TileType == Tile.TileType.IronSoil || tile.m_TileType == Tile.TileType.IronSoil2 || tile.m_TileType == Tile.TileType.Iron) && !tile.GetContainsObject())
						{
							list.Add(tileCoord);
						}
					}
				}
			}
			if (list.Count > 0)
			{
				TileCoord tileCoord2 = list[Random.Range(0, list.Count)];
				ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.IronOre, tileCoord2.ToWorldPositionTileCentered(), Quaternion.identity).WorldCreated();
			}
		}
		else
		{
			m_IronOreTimer = 0f;
		}
	}

	private void UpdateTrees()
	{
		if (m_VisibleObjectCounts[185] == 0 && m_TreeSeen)
		{
			m_TreeTimer += TimeManager.Instance.m_NormalDelta;
			List<TileCoord> list = new List<TileCoord>();
			foreach (Plot visiblePlot in PlotManager.Instance.m_VisiblePlots)
			{
				for (int i = visiblePlot.m_TileY; i < visiblePlot.m_TileY + Plot.m_PlotTilesHigh; i++)
				{
					for (int j = visiblePlot.m_TileX; j < visiblePlot.m_TileX + Plot.m_PlotTilesWide; j++)
					{
						TileCoord tileCoord = new TileCoord(j, i);
						Tile tile = TileManager.Instance.GetTile(tileCoord);
						if (tile.m_TileType == Tile.TileType.Soil && !tile.GetContainsObject())
						{
							list.Add(tileCoord);
						}
					}
				}
			}
			if (list.Count > 0)
			{
				TileCoord tileCoord2 = list[Random.Range(0, list.Count)];
				BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.TreePine, tileCoord2.ToWorldPositionTileCentered(), Quaternion.identity);
				baseClass.GetComponent<MyTree>().SetGrowing();
				baseClass.WorldCreated();
			}
		}
		else
		{
			m_TreeTimer = 0f;
		}
	}

	public void CropWheatRemove(TileCoord Position)
	{
		m_CropWheatTiles.Add(Position);
		if (m_CropWheatTiles.Count > 10)
		{
			m_CropWheatTiles.RemoveAt(0);
		}
	}

	public void CropWheatCut()
	{
		m_CropWheatRemoved = true;
	}

	private void UpdateCropWheat()
	{
		if (!m_CropWheatRemoved)
		{
			return;
		}
		if (m_VisibleObjectCounts[181] == 0)
		{
			m_CropWheatTimer += TimeManager.Instance.m_NormalDelta;
			if (!(m_CropWheatTimer > 10f))
			{
				return;
			}
			TileCoord tileCoord = default(TileCoord);
			if (m_CropWheatTiles.Count > 0)
			{
				int index = Random.Range(0, m_CropWheatTiles.Count);
				tileCoord = m_CropWheatTiles[index];
				m_CropWheatTiles.RemoveAt(index);
			}
			else
			{
				List<int> soilTileIndexes = TileManager.Instance.m_SoilTileIndexes;
				if (soilTileIndexes.Count > 0)
				{
					int num = 0;
					int num2 = -1;
					do
					{
						int index2 = Random.Range(0, soilTileIndexes.Count);
						int num3 = soilTileIndexes[index2];
						if (!TileManager.Instance.m_Tiles[num3].GetContainsObject())
						{
							num2 = num3;
							break;
						}
						num++;
					}
					while (num < 100);
					if (num2 == -1)
					{
						return;
					}
					int tilesWide = TileManager.Instance.m_TilesWide;
					tileCoord = new TileCoord(num2 % tilesWide, num2 / tilesWide);
				}
			}
			ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.CropWheat, tileCoord.ToWorldPositionTileCentered(), Quaternion.identity).GetComponent<CropWheat>().GetComponent<CropWheat>()
				.SetGrowing();
		}
		else
		{
			m_CropWheatTimer = 0f;
		}
	}

	public void BullrushesRemove(TileCoord Position)
	{
		m_BullrushesTiles.Add(Position);
		if (m_BullrushesTiles.Count > 10)
		{
			m_BullrushesTiles.RemoveAt(0);
		}
		m_BullrushesRemoved = true;
	}

	private void UpdateBullrushes()
	{
		if (!m_BullrushesRemoved)
		{
			return;
		}
		if (m_VisibleObjectCounts[194] == 0)
		{
			m_BullrushesTimer += TimeManager.Instance.m_NormalDelta;
			if (!(m_BullrushesTimer > 10f))
			{
				return;
			}
			TileCoord tileCoord = default(TileCoord);
			if (m_BullrushesTiles.Count > 0)
			{
				int index = Random.Range(0, m_BullrushesTiles.Count);
				tileCoord = m_BullrushesTiles[index];
				m_BullrushesTiles.RemoveAt(index);
			}
			else
			{
				List<int> soilTileIndexes = TileManager.Instance.m_SoilTileIndexes;
				if (soilTileIndexes.Count > 0)
				{
					int num = 0;
					int num2 = -1;
					do
					{
						int index2 = Random.Range(0, soilTileIndexes.Count);
						int num3 = soilTileIndexes[index2];
						if (!TileManager.Instance.m_Tiles[num3].GetContainsObject())
						{
							num2 = num3;
							break;
						}
						num++;
					}
					while (num < 100);
					if (num2 == -1)
					{
						return;
					}
					int tilesWide = TileManager.Instance.m_TilesWide;
					tileCoord = new TileCoord(num2 % tilesWide, num2 / tilesWide);
				}
			}
			ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Bullrushes, tileCoord.ToWorldPositionTileCentered(), Quaternion.identity).GetComponent<Bullrushes>().GetComponent<Bullrushes>()
				.SetGrowing();
		}
		else
		{
			m_BullrushesTimer = 0f;
		}
	}

	public void CropCottonRemove(TileCoord Position)
	{
		m_CropCottonTiles.Add(Position);
		if (m_CropCottonTiles.Count > 10)
		{
			m_CropCottonTiles.RemoveAt(0);
		}
	}

	public void CropCottonCut()
	{
		m_CropCottonRemoved = true;
	}

	private void UpdateCropCotton()
	{
		if (!m_CropCottonRemoved)
		{
			return;
		}
		if (m_VisibleObjectCounts[182] == 0)
		{
			m_CropCottonTimer += TimeManager.Instance.m_NormalDelta;
			if (!(m_CropCottonTimer > 10f))
			{
				return;
			}
			TileCoord tileCoord = default(TileCoord);
			if (m_CropCottonTiles.Count > 0)
			{
				int index = Random.Range(0, m_CropCottonTiles.Count);
				tileCoord = m_CropCottonTiles[index];
				m_CropCottonTiles.RemoveAt(index);
			}
			else
			{
				List<int> soilTileIndexes = TileManager.Instance.m_SoilTileIndexes;
				if (soilTileIndexes.Count > 0)
				{
					int num = 0;
					int num2 = -1;
					do
					{
						int index2 = Random.Range(0, soilTileIndexes.Count);
						int num3 = soilTileIndexes[index2];
						if (!TileManager.Instance.m_Tiles[num3].GetContainsObject())
						{
							num2 = num3;
							break;
						}
						num++;
					}
					while (num < 100);
					if (num2 == -1)
					{
						return;
					}
					int tilesWide = TileManager.Instance.m_TilesWide;
					tileCoord = new TileCoord(num2 % tilesWide, num2 / tilesWide);
				}
			}
			ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.CropCotton, tileCoord.ToWorldPositionTileCentered(), Quaternion.identity).GetComponent<CropCotton>().GetComponent<CropCotton>()
				.SetGrowing();
		}
		else
		{
			m_CropCottonTimer = 0f;
		}
	}

	public void UpdateBeeNests(int MinBeeNests)
	{
		MinBeeNests = GetScaledObjectCount(MinBeeNests);
		if (MinBeeNests - ObjectTypeList.m_ObjectTypeCounts[415] <= 0)
		{
			return;
		}
		Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("Tree");
		if (collection == null || collection.Count == 0)
		{
			return;
		}
		int num = 0;
		MyTree myTree = null;
		do
		{
			int num2 = Random.Range(0, collection.Count);
			int num3 = 0;
			foreach (KeyValuePair<BaseClass, int> item in collection)
			{
				myTree = item.Key.GetComponent<MyTree>();
				if (myTree.m_State == MyTree.State.Waiting && num3 >= num2 && !myTree.m_BeesNest)
				{
					break;
				}
				num3++;
			}
			num++;
		}
		while ((myTree == null || (bool)myTree.m_BeesNest) && num < 100);
		if ((bool)myTree && !myTree.m_BeesNest && myTree.m_State == MyTree.State.Waiting)
		{
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.BeesNest, base.transform.position, base.transform.localRotation);
			baseClass.WorldCreated();
			myTree.AddBeesNest(baseClass.GetComponent<BeesNest>());
		}
	}

	private Grass GetRandomGrass()
	{
		Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("Grass");
		if (collection == null || collection.Count == 0)
		{
			return null;
		}
		int num = 0;
		Grass grass = null;
		do
		{
			int num2 = Random.Range(0, collection.Count);
			int num3 = 0;
			foreach (KeyValuePair<BaseClass, int> item in collection)
			{
				grass = item.Key.GetComponent<Grass>();
				if (num3 >= num2)
				{
					break;
				}
				num3++;
			}
			num++;
		}
		while (grass == null && num < 100);
		return grass;
	}

	public void UpdateCows(int MinCows)
	{
		MinCows = GetScaledObjectCount(MinCows);
		if (MinCows - ObjectTypeList.m_ObjectTypeCounts[195] > 0)
		{
			Grass randomGrass = GetRandomGrass();
			if ((bool)randomGrass)
			{
				ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.AnimalCow, randomGrass.m_TileCoord.ToWorldPositionTileCentered(), Quaternion.identity).WorldCreated();
			}
		}
	}

	public void UpdateSheep(int MinSheep)
	{
		MinSheep = GetScaledObjectCount(MinSheep);
		if (MinSheep - ObjectTypeList.m_ObjectTypeCounts[196] > 0)
		{
			Grass randomGrass = GetRandomGrass();
			if ((bool)randomGrass)
			{
				ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.AnimalSheep, randomGrass.m_TileCoord.ToWorldPositionTileCentered(), Quaternion.identity).WorldCreated();
			}
		}
	}

	public void UpdateChickens(int MinChickens)
	{
		MinChickens = GetScaledObjectCount(MinChickens);
		if (MinChickens - ObjectTypeList.m_ObjectTypeCounts[199] <= 0)
		{
			return;
		}
		List<int> soilTileIndexes = TileManager.Instance.m_SoilTileIndexes;
		if (soilTileIndexes.Count <= 0)
		{
			return;
		}
		int num = 0;
		int num2 = -1;
		do
		{
			int index = Random.Range(0, soilTileIndexes.Count);
			int num3 = soilTileIndexes[index];
			if (!TileManager.Instance.m_Tiles[num3].GetContainsObject())
			{
				num2 = num3;
				break;
			}
			num++;
		}
		while (num < 100);
		if (num2 != -1)
		{
			int nx = num2 % TileManager.Instance.m_TilesWide;
			int ny = num2 / TileManager.Instance.m_TilesWide;
			TileCoord tileCoord = new TileCoord(nx, ny);
			ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.AnimalChicken, tileCoord.ToWorldPositionTileCentered(), Quaternion.identity).WorldCreated();
		}
	}

	public void UpdateBirds(int MinBirds)
	{
		MinBirds = GetScaledObjectCount(MinBirds);
		if (MinBirds - ObjectTypeList.m_ObjectTypeCounts[200] <= 0)
		{
			return;
		}
		Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("Tree");
		if (collection == null || collection.Count == 0)
		{
			return;
		}
		int num = 0;
		MyTree myTree = null;
		do
		{
			int num2 = Random.Range(0, collection.Count);
			int num3 = 0;
			foreach (KeyValuePair<BaseClass, int> item in collection)
			{
				myTree = item.Key.GetComponent<MyTree>();
				if (myTree.m_State == MyTree.State.Waiting)
				{
					if (num3 >= num2)
					{
						break;
					}
					num3++;
				}
			}
			num++;
		}
		while (myTree == null && num < 100);
		if ((bool)myTree && myTree.m_State == MyTree.State.Waiting)
		{
			int num4 = Random.Range(1, 3);
			for (int i = 0; i < num4; i++)
			{
				ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.AnimalBird, myTree.transform.position, Quaternion.Euler(0f, Random.Range(0, 360), 0f)).WorldCreated();
			}
		}
	}

	public void UpdatePumpkins(int MinPumpkins)
	{
		MinPumpkins = GetScaledObjectCount(MinPumpkins);
		if (MinPumpkins - ObjectTypeList.m_ObjectTypeCounts[189] - ObjectTypeList.m_ObjectTypeCounts[363] <= 0)
		{
			return;
		}
		Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("CropWheat");
		if (collection == null || collection.Count == 0 || collection == null)
		{
			return;
		}
		int num = 0;
		CropWheat cropWheat = null;
		do
		{
			int num2 = Random.Range(0, collection.Count);
			int num3 = 0;
			foreach (KeyValuePair<BaseClass, int> item in collection)
			{
				cropWheat = item.Key.GetComponent<CropWheat>();
				if (num3 >= num2)
				{
					break;
				}
				num3++;
			}
			num++;
		}
		while (cropWheat == null && num < 100);
		if ((bool)cropWheat)
		{
			cropWheat.StopUsing();
			ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.CropPumpkin, cropWheat.transform.position, Quaternion.Euler(0f, Random.Range(0, 360), 0f)).GetComponent<CropPumpkin>().WorldCreated();
		}
	}

	public void AddFlower(FlowerWild NewFlower)
	{
		m_NewFlowers.Add(NewFlower);
	}

	public void RemoveFlower(FlowerWild NewFlower)
	{
		if (NewFlower.m_Type != FlowerWild.Type.Total)
		{
			m_FlowerCounts[(int)NewFlower.m_Type]--;
		}
	}

	private void CheckNewFlowers()
	{
		foreach (FlowerWild newFlower in m_NewFlowers)
		{
			if ((bool)newFlower && newFlower.m_Type < FlowerWild.Type.Total)
			{
				m_FlowerCounts[(int)newFlower.m_Type]++;
			}
		}
		m_NewFlowers.Clear();
	}

	public void UpdateFlowers(int MinFlowers)
	{
		if (!GeneralUtils.m_InGame)
		{
			return;
		}
		CheckNewFlowers();
		List<int> emptyTileIndexes = TileManager.Instance.m_EmptyTileIndexes;
		MinFlowers = GetScaledObjectCount(MinFlowers);
		for (int i = 0; i < 7; i++)
		{
			if (emptyTileIndexes.Count == 0)
			{
				break;
			}
			if (MinFlowers - m_FlowerCounts[i] <= 0)
			{
				continue;
			}
			int num = 0;
			int num2 = -1;
			do
			{
				int index = Random.Range(0, emptyTileIndexes.Count);
				int num3 = emptyTileIndexes[index];
				if (!TileManager.Instance.m_Tiles[num3].GetContainsObject())
				{
					num2 = num3;
					break;
				}
				num++;
			}
			while (num < 100);
			if (num2 == -1)
			{
				break;
			}
			int tilesWide = TileManager.Instance.m_TilesWide;
			TileCoord position = new TileCoord(num2 % tilesWide, num2 / tilesWide);
			TileManager.Instance.SetTileType(position, Tile.TileType.Soil);
			FlowerWild component = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.FlowerWild, position.ToWorldPositionTileCentered(), Quaternion.identity).GetComponent<FlowerWild>();
			component.WorldCreated();
			component.SetType((FlowerWild.Type)i);
		}
	}

	public void UpdateAppleTrees(int MinAppleTrees)
	{
		MinAppleTrees = GetScaledObjectCount(MinAppleTrees);
		if (MinAppleTrees - ObjectTypeList.m_ObjectTypeCounts[186] <= 0)
		{
			return;
		}
		List<int> soilTileIndexes = TileManager.Instance.m_SoilTileIndexes;
		if (soilTileIndexes.Count <= 0)
		{
			return;
		}
		int num = 0;
		int num2 = -1;
		do
		{
			int index = Random.Range(0, soilTileIndexes.Count);
			int num3 = soilTileIndexes[index];
			if (!TileManager.Instance.m_Tiles[num3].GetContainsObject())
			{
				num2 = num3;
				break;
			}
			num++;
		}
		while (num < 100);
		if (num2 != -1)
		{
			int nx = num2 % TileManager.Instance.m_TilesWide;
			int ny = num2 / TileManager.Instance.m_TilesWide;
			TileCoord tileCoord = new TileCoord(nx, ny);
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.TreeApple, tileCoord.ToWorldPositionTileCentered(), Quaternion.identity);
			baseClass.GetComponent<MyTree>().SetGrowing();
			baseClass.WorldCreated();
			baseClass.GetComponent<MyTree>().UpdatePlotVisibility();
		}
	}

	private int GetScaledObjectCount(int Count)
	{
		if (m_Disabled)
		{
			return 0;
		}
		float num = GameOptionsManager.Instance.m_Options.m_SizeScaler[(int)GameOptionsManager.Instance.m_Options.m_GameSize];
		if (!GeneralUtils.m_InGame)
		{
			num = 1f;
		}
		int num2 = (int)((float)Count * num);
		if (num2 < 2)
		{
			num2 = 2;
		}
		return num2;
	}

	public void Disable(bool Disable)
	{
		m_Disabled = Disable;
	}

	private void Update()
	{
		if (!m_Disabled && !SaveLoadManager.m_EmptyWorld && !SaveLoadManager.Instance.m_Loading)
		{
			UpdateRocks();
			UpdateClay();
			UpdateIronOre();
			UpdateTrees();
			UpdateCropWheat();
			UpdateCropCotton();
			UpdateBullrushes();
			UpdateBeeNests(m_MinBeeNests);
			UpdateCows(m_MinCows);
			UpdateSheep(m_MinSheep);
			UpdateChickens(m_MinChickens);
			UpdateBirds(m_MinBirds);
			UpdatePumpkins(m_MinPumpkins);
			UpdateAppleTrees(m_MinAppleTrees);
			UpdateFlowers(m_MinFlowers);
		}
	}
}
