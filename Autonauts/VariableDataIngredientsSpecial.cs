public class VariableDataIngredientsSpecial
{
	private IngredientRequirement[][] m_Ingredients;

	private ObjectType[] m_Converters;

	public IngredientRequirement[] GetIngredients(ObjectType NewType)
	{
		return m_Ingredients[(int)NewType];
	}

	public ObjectType GetConverterType(ObjectType NewType)
	{
		return m_Converters[(int)NewType];
	}

	private void SetIngredients(ObjectType NewType, IngredientRequirement[] NewIngredients)
	{
		m_Ingredients[(int)NewType] = NewIngredients;
	}

	private void SetConverter(ObjectType NewType, ObjectType NewConverter)
	{
		m_Converters[(int)NewType] = NewConverter;
	}

	private void CreateArrays()
	{
		int total = (int)ObjectTypeList.m_Total;
		m_Ingredients = new IngredientRequirement[total][];
		m_Converters = new ObjectType[total];
		for (int i = 0; i < total; i++)
		{
			m_Ingredients[i] = new IngredientRequirement[0];
			m_Converters[i] = ObjectTypeList.m_Total;
		}
	}

	private ObjectType GetTileObject(Tile.TileType NewType)
	{
		return (ObjectType)((int)NewType + (int)ObjectTypeList.m_Total);
	}

	public void Init()
	{
		CreateArrays();
		IngredientRequirement[] newIngredients = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.TreePine, 1)
		};
		SetIngredients(ObjectType.Log, newIngredients);
		SetConverter(ObjectType.Log, ObjectType.ToolAxeStone);
		IngredientRequirement[] newIngredients2 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Log, 1)
		};
		SetIngredients(ObjectType.Plank, newIngredients2);
		SetConverter(ObjectType.Plank, ObjectType.ToolAxeStone);
		IngredientRequirement[] newIngredients3 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Plank, 1)
		};
		SetIngredients(ObjectType.Pole, newIngredients3);
		SetConverter(ObjectType.Pole, ObjectType.ToolAxeStone);
		IngredientRequirement[] newIngredients4 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.TreePine, 1)
		};
		SetIngredients(ObjectType.Stick, newIngredients4);
		SetConverter(ObjectType.Stick, ObjectType.ToolMallet);
		IngredientRequirement[] newIngredients5 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.TreeApple, 1)
		};
		SetIngredients(ObjectType.Apple, newIngredients5);
		SetConverter(ObjectType.Apple, ObjectType.ToolMallet);
		IngredientRequirement[] newIngredients6 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.TreePine, 1)
		};
		SetIngredients(ObjectType.TreeSeed, newIngredients6);
		SetConverter(ObjectType.TreeSeed, ObjectType.ToolMallet);
		IngredientRequirement[] newIngredients7 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.TreeSeed, 1),
			new IngredientRequirement(ObjectType.Fertiliser, 0)
		};
		SetIngredients(ObjectType.Seedling, newIngredients7);
		SetConverter(ObjectType.Seedling, ObjectType.StorageSeedlings);
		IngredientRequirement[] newIngredients8 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.MulberrySeed, 1),
			new IngredientRequirement(ObjectType.Fertiliser, 0)
		};
		SetIngredients(ObjectType.SeedlingMulberry, newIngredients8);
		SetConverter(ObjectType.SeedlingMulberry, ObjectType.StorageSeedlings);
		IngredientRequirement[] newIngredients9 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Manure, 1)
		};
		SetIngredients(ObjectType.Fertiliser, newIngredients9);
		SetConverter(ObjectType.Fertiliser, ObjectType.StorageFertiliser);
		IngredientRequirement[] newIngredients10 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Pole, 1)
		};
		SetIngredients(ObjectType.FixingPeg, newIngredients10);
		SetConverter(ObjectType.FixingPeg, ObjectType.ToolAxeStone);
		IngredientRequirement[] newIngredients11 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.TreePine, 1)
		};
		SetIngredients(ObjectType.BeesNest, newIngredients11);
		SetConverter(ObjectType.BeesNest, ObjectType.ToolMallet);
		IngredientRequirement[] newIngredients12 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.CropPumpkin, 1)
		};
		SetIngredients(ObjectType.Pumpkin, newIngredients12);
		SetConverter(ObjectType.Pumpkin, ObjectType.ToolScytheStone);
		IngredientRequirement[] newIngredients13 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Pumpkin, 1)
		};
		SetIngredients(ObjectType.PumpkinSeeds, newIngredients13);
		SetConverter(ObjectType.PumpkinSeeds, ObjectType.ToolMallet);
		IngredientRequirement[] newIngredients14 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Pumpkin, 1)
		};
		SetIngredients(ObjectType.PumpkinRaw, newIngredients14);
		SetConverter(ObjectType.PumpkinRaw, ObjectType.ToolMallet);
		IngredientRequirement[] newIngredients15 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Bush, 1)
		};
		SetIngredients(ObjectType.Berries, newIngredients15);
		SetConverter(ObjectType.Berries, ObjectType.Stick);
		IngredientRequirement[] newIngredients16 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Grass, 1)
		};
		SetIngredients(ObjectType.GrassCut, newIngredients16);
		SetConverter(ObjectType.GrassCut, ObjectType.RockSharp);
		IngredientRequirement[] newIngredients17 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.BeesNest, 1)
		};
		SetIngredients(ObjectType.AnimalBee, newIngredients17);
		SetConverter(ObjectType.AnimalBee, ObjectType.StorageBeehiveCrude);
		IngredientRequirement[] newIngredients18 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.ToolFishingStick, 1),
			new IngredientRequirement(GetTileObject(Tile.TileType.SeaWaterShallow), 1)
		};
		SetIngredients(ObjectType.FishSalmon, newIngredients18);
		IngredientRequirement[] newIngredients19 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.ToolFishingRod, 1),
			new IngredientRequirement(ObjectType.FishBait, 1),
			new IngredientRequirement(GetTileObject(Tile.TileType.WaterDeep), 1)
		};
		SetIngredients(ObjectType.FishCatFish, newIngredients19);
		IngredientRequirement[] newIngredients20 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.ToolFishingRodGood, 1),
			new IngredientRequirement(ObjectType.FishBait, 1),
			new IngredientRequirement(GetTileObject(Tile.TileType.SeaWaterDeep), 1)
		};
		SetIngredients(ObjectType.FishMahiMahi, newIngredients20);
		IngredientRequirement[] newIngredients21 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.ToolFishingRod, 1),
			new IngredientRequirement(ObjectType.FishBait, 1),
			new IngredientRequirement(GetTileObject(Tile.TileType.SeaWaterDeep), 1)
		};
		SetIngredients(ObjectType.FishOrangeRoughy, newIngredients21);
		IngredientRequirement[] newIngredients22 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.ToolFishingRodGood, 1),
			new IngredientRequirement(ObjectType.FishBait, 1),
			new IngredientRequirement(GetTileObject(Tile.TileType.WaterDeep), 1)
		};
		SetIngredients(ObjectType.FishPerch, newIngredients22);
		IngredientRequirement[] newIngredients23 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.ToolFishingRod, 1),
			new IngredientRequirement(ObjectType.FishBait, 1),
			new IngredientRequirement(GetTileObject(Tile.TileType.WaterDeep), 1)
		};
		SetIngredients(ObjectType.FishCarp, newIngredients23);
		IngredientRequirement[] newIngredients24 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.ToolFishingRodGood, 1),
			new IngredientRequirement(ObjectType.FishBait, 1),
			new IngredientRequirement(GetTileObject(Tile.TileType.WaterDeep), 1)
		};
		SetIngredients(ObjectType.FishGar, newIngredients24);
		IngredientRequirement[] newIngredients25 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.ToolFishingRod, 1),
			new IngredientRequirement(ObjectType.FishBait, 1),
			new IngredientRequirement(GetTileObject(Tile.TileType.SeaWaterDeep), 1)
		};
		SetIngredients(ObjectType.FishMonkfish, newIngredients25);
		IngredientRequirement[] newIngredients26 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.ToolFishingRodGood, 1),
			new IngredientRequirement(ObjectType.FishBait, 1),
			new IngredientRequirement(GetTileObject(Tile.TileType.SeaWaterDeep), 1)
		};
		SetIngredients(ObjectType.FishMarlin, newIngredients26);
		IngredientRequirement[] newIngredients27 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.ToolNetCrude, 1),
			new IngredientRequirement(GetTileObject(Tile.TileType.SeaWaterShallow), 1)
		};
		SetIngredients(ObjectType.FishBait, newIngredients27);
		IngredientRequirement[] newIngredients28 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.ToolNetCrude, 1),
			new IngredientRequirement(GetTileObject(Tile.TileType.Swamp), 1)
		};
		SetIngredients(ObjectType.AnimalLeech, newIngredients28);
		IngredientRequirement[] newIngredients29 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.AnimalSilkworm, 1),
			new IngredientRequirement(ObjectType.MulberrySeed, 1)
		};
		SetIngredients(ObjectType.AnimalSilkmoth, newIngredients29);
		SetConverter(ObjectType.AnimalSilkmoth, ObjectType.SilkwormStation);
		IngredientRequirement[] newIngredients30 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.AnimalSilkworm, 1),
			new IngredientRequirement(ObjectType.MulberrySeed, 1)
		};
		SetIngredients(ObjectType.SilkRaw, newIngredients30);
		SetConverter(ObjectType.SilkRaw, ObjectType.SilkwormStation);
		SetConverter(ObjectType.Egg, ObjectType.AnimalChicken);
		IngredientRequirement[] newIngredients31 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Mushroom, 1)
		};
		SetIngredients(ObjectType.MushroomDug, newIngredients31);
		SetConverter(ObjectType.MushroomDug, ObjectType.ToolShovelStone);
		IngredientRequirement[] newIngredients32 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.AnimalCow, 1)
		};
		SetIngredients(ObjectType.Milk, newIngredients32);
		SetConverter(ObjectType.Milk, ObjectType.ToolBucketCrude);
		SetConverter(ObjectType.Manure, ObjectType.AnimalCow);
		IngredientRequirement[] newIngredients33 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Wheat, 1)
		};
		SetIngredients(ObjectType.WheatSeed, newIngredients33);
		SetConverter(ObjectType.WheatSeed, ObjectType.ToolFlailCrude);
		IngredientRequirement[] newIngredients34 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Wheat, 1)
		};
		SetIngredients(ObjectType.Straw, newIngredients34);
		SetConverter(ObjectType.Straw, ObjectType.ToolFlailCrude);
		IngredientRequirement[] newIngredients35 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.CropWheat, 1)
		};
		SetIngredients(ObjectType.Wheat, newIngredients35);
		SetConverter(ObjectType.Wheat, ObjectType.ToolScytheStone);
		IngredientRequirement[] newIngredients36 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.CropCarrot, 1)
		};
		SetIngredients(ObjectType.Carrot, newIngredients36);
		SetConverter(ObjectType.Carrot, ObjectType.ToolShovelStone);
		IngredientRequirement[] newIngredients37 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.CropCotton, 1)
		};
		SetIngredients(ObjectType.CottonBall, newIngredients37);
		SetConverter(ObjectType.CottonBall, ObjectType.ToolScytheStone);
		IngredientRequirement[] newIngredients38 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Weed, 1)
		};
		SetIngredients(ObjectType.WeedDug, newIngredients38);
		SetConverter(ObjectType.WeedDug, ObjectType.ToolShovelStone);
		IngredientRequirement[] newIngredients39 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.FlowerWild, 1)
		};
		SetIngredients(ObjectType.FlowerSeeds01, newIngredients39);
		SetConverter(ObjectType.FlowerSeeds01, ObjectType.ToolScytheStone);
		SetIngredients(ObjectType.FlowerSeeds02, newIngredients39);
		SetConverter(ObjectType.FlowerSeeds02, ObjectType.ToolScytheStone);
		SetIngredients(ObjectType.FlowerSeeds03, newIngredients39);
		SetConverter(ObjectType.FlowerSeeds03, ObjectType.ToolScytheStone);
		SetIngredients(ObjectType.FlowerSeeds04, newIngredients39);
		SetConverter(ObjectType.FlowerSeeds04, ObjectType.ToolScytheStone);
		SetIngredients(ObjectType.FlowerSeeds05, newIngredients39);
		SetConverter(ObjectType.FlowerSeeds05, ObjectType.ToolScytheStone);
		SetIngredients(ObjectType.FlowerSeeds06, newIngredients39);
		SetConverter(ObjectType.FlowerSeeds06, ObjectType.ToolScytheStone);
		SetIngredients(ObjectType.FlowerSeeds07, newIngredients39);
		SetConverter(ObjectType.FlowerSeeds07, ObjectType.ToolScytheStone);
		IngredientRequirement[] newIngredients40 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.FlowerWild, 1)
		};
		SetIngredients(ObjectType.FlowerBunch01, newIngredients40);
		SetConverter(ObjectType.FlowerBunch01, ObjectType.ToolScytheStone);
		SetIngredients(ObjectType.FlowerBunch02, newIngredients40);
		SetConverter(ObjectType.FlowerBunch02, ObjectType.ToolScytheStone);
		SetIngredients(ObjectType.FlowerBunch03, newIngredients40);
		SetConverter(ObjectType.FlowerBunch03, ObjectType.ToolScytheStone);
		SetIngredients(ObjectType.FlowerBunch04, newIngredients40);
		SetConverter(ObjectType.FlowerBunch04, ObjectType.ToolScytheStone);
		SetIngredients(ObjectType.FlowerBunch05, newIngredients40);
		SetConverter(ObjectType.FlowerBunch05, ObjectType.ToolScytheStone);
		SetIngredients(ObjectType.FlowerBunch06, newIngredients40);
		SetConverter(ObjectType.FlowerBunch06, ObjectType.ToolScytheStone);
		SetIngredients(ObjectType.FlowerBunch07, newIngredients40);
		SetConverter(ObjectType.FlowerBunch07, ObjectType.ToolScytheStone);
		SetConverter(ObjectType.FolkHeart, ObjectType.Folk);
		SetConverter(ObjectType.FolkHeart2, ObjectType.Folk);
		SetConverter(ObjectType.FolkHeart3, ObjectType.Folk);
		SetConverter(ObjectType.FolkHeart4, ObjectType.Folk);
		SetConverter(ObjectType.FolkHeart5, ObjectType.Folk);
		SetConverter(ObjectType.FolkHeart6, ObjectType.Folk);
		SetConverter(ObjectType.FolkHeart7, ObjectType.Folk);
		IngredientRequirement[] newIngredients41 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.TallBoulder, 1)
		};
		SetIngredients(ObjectType.StoneBlockCrude, newIngredients41);
		SetConverter(ObjectType.StoneBlockCrude, ObjectType.ToolChiselCrude);
		IngredientRequirement[] newIngredients42 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.AnimalSheep, 1)
		};
		SetIngredients(ObjectType.Fleece, newIngredients42);
		SetConverter(ObjectType.Fleece, ObjectType.ToolShears);
		IngredientRequirement[] newIngredients43 = new IngredientRequirement[2]
		{
			new IngredientRequirement(GetTileObject(Tile.TileType.Soil), 1),
			new IngredientRequirement(ObjectType.WeedDug, 1)
		};
		SetIngredients(ObjectType.Weed, newIngredients43);
		IngredientRequirement[] newIngredients44 = new IngredientRequirement[2]
		{
			new IngredientRequirement(GetTileObject(Tile.TileType.Soil), 1),
			new IngredientRequirement(ObjectType.GrassCut, 1)
		};
		SetIngredients(ObjectType.Grass, newIngredients44);
		IngredientRequirement[] newIngredients45 = new IngredientRequirement[2]
		{
			new IngredientRequirement(GetTileObject(Tile.TileType.Soil), 1),
			new IngredientRequirement(ObjectType.WheatSeed, 1)
		};
		SetIngredients(ObjectType.CropWheat, newIngredients45);
		IngredientRequirement[] newIngredients46 = new IngredientRequirement[2]
		{
			new IngredientRequirement(GetTileObject(Tile.TileType.Soil), 1),
			new IngredientRequirement(ObjectType.CottonSeeds, 1)
		};
		SetIngredients(ObjectType.CropCotton, newIngredients46);
		IngredientRequirement[] newIngredients47 = new IngredientRequirement[2]
		{
			new IngredientRequirement(GetTileObject(Tile.TileType.Soil), 1),
			new IngredientRequirement(ObjectType.CarrotSeed, 1)
		};
		SetIngredients(ObjectType.CropCarrot, newIngredients47);
		IngredientRequirement[] newIngredients48 = new IngredientRequirement[2]
		{
			new IngredientRequirement(GetTileObject(Tile.TileType.Swamp), 1),
			new IngredientRequirement(ObjectType.BullrushesSeeds, 1)
		};
		SetIngredients(ObjectType.Bullrushes, newIngredients48);
		IngredientRequirement[] newIngredients49 = new IngredientRequirement[2]
		{
			new IngredientRequirement(GetTileObject(Tile.TileType.SoilHole), 1),
			new IngredientRequirement(ObjectType.TreeSeed, 1)
		};
		SetIngredients(ObjectType.TreePine, newIngredients49);
		IngredientRequirement[] newIngredients50 = new IngredientRequirement[2]
		{
			new IngredientRequirement(GetTileObject(Tile.TileType.SoilHole), 1),
			new IngredientRequirement(ObjectType.Coconut, 1)
		};
		SetIngredients(ObjectType.TreeCoconut, newIngredients50);
		IngredientRequirement[] newIngredients51 = new IngredientRequirement[2]
		{
			new IngredientRequirement(GetTileObject(Tile.TileType.SoilHole), 1),
			new IngredientRequirement(ObjectType.Apple, 1)
		};
		SetIngredients(ObjectType.TreeApple, newIngredients51);
		IngredientRequirement[] newIngredients52 = new IngredientRequirement[2]
		{
			new IngredientRequirement(GetTileObject(Tile.TileType.SoilHole), 1),
			new IngredientRequirement(ObjectType.SeedlingMulberry, 1)
		};
		SetIngredients(ObjectType.TreeMulberry, newIngredients52);
		IngredientRequirement[] newIngredients53 = new IngredientRequirement[2]
		{
			new IngredientRequirement(GetTileObject(Tile.TileType.SoilHole), 1),
			new IngredientRequirement(ObjectType.PumpkinSeeds, 1)
		};
		SetIngredients(ObjectType.CropPumpkin, newIngredients53);
		IngredientRequirement[] newIngredients54 = new IngredientRequirement[2]
		{
			new IngredientRequirement(GetTileObject(Tile.TileType.SoilHole), 1),
			new IngredientRequirement(ObjectType.Berries, 1)
		};
		SetIngredients(ObjectType.Bush, newIngredients54);
		IngredientRequirement[] newIngredients55 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Bush, 1)
		};
		SetIngredients(ObjectType.Hedge, newIngredients55);
		SetConverter(ObjectType.Hedge, ObjectType.ToolShears);
		IngredientRequirement[] newIngredients56 = new IngredientRequirement[2]
		{
			new IngredientRequirement(GetTileObject(Tile.TileType.SoilHole), 1),
			new IngredientRequirement(ObjectType.MushroomDug, 1)
		};
		SetIngredients(ObjectType.Mushroom, newIngredients56);
		IngredientRequirement[] newIngredients57 = new IngredientRequirement[2]
		{
			new IngredientRequirement(GetTileObject(Tile.TileType.SoilTilled), 1),
			new IngredientRequirement(ObjectType.FlowerSeeds01, 1)
		};
		SetIngredients(ObjectType.FlowerWild, newIngredients57);
		IngredientRequirement[] newIngredients58 = new IngredientRequirement[1]
		{
			new IngredientRequirement(GetTileObject(Tile.TileType.StoneSoil), 1)
		};
		SetIngredients(ObjectType.Rock, newIngredients58);
		SetConverter(ObjectType.Rock, ObjectType.ToolPickStone);
		IngredientRequirement[] newIngredients59 = new IngredientRequirement[1]
		{
			new IngredientRequirement(GetTileObject(Tile.TileType.ClaySoil), 1)
		};
		SetIngredients(ObjectType.Clay, newIngredients59);
		SetConverter(ObjectType.Clay, ObjectType.ToolShovelStone);
		IngredientRequirement[] newIngredients60 = new IngredientRequirement[1]
		{
			new IngredientRequirement(GetTileObject(Tile.TileType.IronSoil), 1)
		};
		SetIngredients(ObjectType.IronOre, newIngredients60);
		SetConverter(ObjectType.IronOre, ObjectType.ToolPickStone);
		IngredientRequirement[] newIngredients61 = new IngredientRequirement[1]
		{
			new IngredientRequirement(GetTileObject(Tile.TileType.CoalSoil), 1)
		};
		SetIngredients(ObjectType.Coal, newIngredients61);
		SetConverter(ObjectType.Coal, ObjectType.ToolPickStone);
		IngredientRequirement[] newIngredients62 = new IngredientRequirement[1]
		{
			new IngredientRequirement(GetTileObject(Tile.TileType.WaterShallow), 1)
		};
		SetIngredients(ObjectType.Water, newIngredients62);
		SetConverter(ObjectType.Water, ObjectType.ToolBucketCrude);
		IngredientRequirement[] newIngredients63 = new IngredientRequirement[1]
		{
			new IngredientRequirement(GetTileObject(Tile.TileType.SeaWaterShallow), 1)
		};
		SetIngredients(ObjectType.SeaWater, newIngredients63);
		SetConverter(ObjectType.SeaWater, ObjectType.ToolBucketCrude);
		IngredientRequirement[] newIngredients64 = new IngredientRequirement[1]
		{
			new IngredientRequirement(GetTileObject(Tile.TileType.Sand), 1)
		};
		SetIngredients(ObjectType.Sand, newIngredients64);
		SetConverter(ObjectType.Sand, ObjectType.ToolBucketCrude);
		IngredientRequirement[] newIngredients65 = new IngredientRequirement[1]
		{
			new IngredientRequirement(GetTileObject(Tile.TileType.Soil), 1)
		};
		SetIngredients(ObjectType.Soil, newIngredients65);
		SetConverter(ObjectType.Soil, ObjectType.ToolBucketCrude);
		IngredientRequirement[] newIngredients66 = new IngredientRequirement[1]
		{
			new IngredientRequirement(GetTileObject(Tile.TileType.Empty), 1)
		};
		SetIngredients(ObjectType.Turf, newIngredients66);
		SetConverter(ObjectType.Turf, ObjectType.ToolShovelStone);
		IngredientRequirement[] newIngredients67 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.StorageBeehiveCrude, 1)
		};
		SetIngredients(ObjectType.Honey, newIngredients67);
		SetConverter(ObjectType.Honey, ObjectType.ToolBucketCrude);
		IngredientRequirement[] newIngredients68 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.FishAny, 1)
		};
		SetIngredients(ObjectType.FishRaw, newIngredients68);
		SetConverter(ObjectType.FishRaw, ObjectType.RockSharp);
		IngredientRequirement[] newIngredients69 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.CottonBall, 1)
		};
		SetIngredients(ObjectType.CottonSeeds, newIngredients69);
		SetConverter(ObjectType.CottonSeeds, ObjectType.WheatHammer);
		SetIngredients(ObjectType.CottonLint, newIngredients69);
		SetConverter(ObjectType.CottonLint, ObjectType.WheatHammer);
		IngredientRequirement[] newIngredients70 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Bullrushes, 1)
		};
		SetIngredients(ObjectType.BullrushesSeeds, newIngredients70);
		SetConverter(ObjectType.BullrushesSeeds, ObjectType.ToolScytheStone);
		SetIngredients(ObjectType.BullrushesStems, newIngredients70);
		SetConverter(ObjectType.BullrushesStems, ObjectType.ToolScytheStone);
		IngredientRequirement[] newIngredients71 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.GrassCut, 5)
		};
		SetIngredients(ObjectType.HayBale, newIngredients71);
		SetConverter(ObjectType.HayBale, ObjectType.ToolPitchfork);
		SetIngredients(ObjectType.Blanket, ObjectTypeList.Instance.GetIngredientsFromIdentifier(ObjectType.Blanket));
		SetConverter(ObjectType.Blanket, ObjectType.RockingChair);
		SetIngredients(ObjectType.CottonCloth, ObjectTypeList.Instance.GetIngredientsFromIdentifier(ObjectType.CottonCloth));
		SetConverter(ObjectType.CottonCloth, ObjectType.RockingChair);
		SetIngredients(ObjectType.BullrushesCloth, ObjectTypeList.Instance.GetIngredientsFromIdentifier(ObjectType.BullrushesCloth));
		SetConverter(ObjectType.BullrushesCloth, ObjectType.RockingChair);
		for (int i = 0; i < (int)ObjectTypeList.m_Total; i++)
		{
			ObjectType objectType = (ObjectType)i;
			int variableAsInt = VariableManager.Instance.GetVariableAsInt(objectType, "UpgradeFrom", CheckValid: false);
			if (variableAsInt != 0)
			{
				ObjectType objectType2 = (ObjectType)variableAsInt;
				IngredientRequirement[] ingredientsFromIdentifier = ObjectTypeList.Instance.GetIngredientsFromIdentifier(objectType);
				IngredientRequirement[] array = new IngredientRequirement[ingredientsFromIdentifier.Length + 1];
				array[0].m_Type = objectType2;
				if (Storage.GetIsTypeStorage(objectType2))
				{
					array[0].m_Count = 3;
				}
				else
				{
					array[0].m_Count = 1;
				}
				for (int j = 0; j < ingredientsFromIdentifier.Length; j++)
				{
					array[j + 1].m_Count = ingredientsFromIdentifier[j].m_Count;
					array[j + 1].m_Type = ingredientsFromIdentifier[j].m_Type;
				}
				SetIngredients(objectType, array);
			}
		}
	}
}
