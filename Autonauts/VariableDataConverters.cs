using System.Collections.Generic;

public class VariableDataConverters
{
	public ConverterResults[] m_Results;

	protected void AddRequirements(ObjectType BuildingType, List<IngredientRequirement> Requirements, List<IngredientRequirement> Results)
	{
		if (Requirements == null)
		{
			Requirements = new List<IngredientRequirement>();
			if (Results[0].m_Type != ObjectTypeList.m_Total)
			{
				IngredientRequirement[] ingredientsFromIdentifier = ObjectTypeList.Instance.GetIngredientsFromIdentifier(Results[0].m_Type);
				for (int i = 0; i < ingredientsFromIdentifier.Length; i++)
				{
					Requirements.Add(new IngredientRequirement(ingredientsFromIdentifier[i].m_Type, ingredientsFromIdentifier[i].m_Count));
				}
			}
		}
		m_Results[(int)BuildingType].Add(Requirements, Results);
	}

	public void Add(ObjectType BuildingType, ObjectType ResultType)
	{
		if (ResultType == ObjectType.Worker || ResultType == ObjectType.BasicWorker || ObjectTypeList.Instance.GetSubCategoryFromType(ResultType) != ObjectSubCategory.Hidden)
		{
			List<IngredientRequirement> list = new List<IngredientRequirement>();
			list.Add(new IngredientRequirement(ResultType, 1));
			AddRequirements(BuildingType, null, list);
		}
	}

	public void Add(ObjectType BuildingType, ObjectType ResultType, int ResultCount)
	{
		if (ResultType == ObjectType.Worker || ObjectTypeList.Instance.GetSubCategoryFromType(ResultType) != ObjectSubCategory.Hidden)
		{
			List<IngredientRequirement> list = new List<IngredientRequirement>();
			list.Add(new IngredientRequirement(ResultType, ResultCount));
			AddRequirements(BuildingType, null, list);
		}
	}

	public void Add(ObjectType BuildingType, ObjectType IngredientType, int IngredientCount, ObjectType ResultType, int ResultCount)
	{
		if (ResultType == ObjectType.Worker || ObjectTypeList.Instance.GetSubCategoryFromType(ResultType) != ObjectSubCategory.Hidden)
		{
			List<IngredientRequirement> list = new List<IngredientRequirement>();
			list.Add(new IngredientRequirement(IngredientType, IngredientCount));
			List<IngredientRequirement> list2 = new List<IngredientRequirement>();
			list2.Add(new IngredientRequirement(ResultType, ResultCount));
			AddRequirements(BuildingType, list, list2);
		}
	}

	public void Add(ObjectType BuildingType, List<IngredientRequirement> Requirements, ObjectType ResultType, int ResultCount)
	{
		if (ResultType == ObjectType.Worker || ObjectTypeList.Instance.GetSubCategoryFromType(ResultType) != ObjectSubCategory.Hidden)
		{
			List<IngredientRequirement> list = new List<IngredientRequirement>();
			list.Add(new IngredientRequirement(ResultType, ResultCount));
			AddRequirements(BuildingType, Requirements, list);
		}
	}

	public void Add(ObjectType BuildingType, ObjectType IngredientType, int IngredientCount, ObjectType ResultType, int ResultCount, ObjectType ResultType2, int ResultCount2)
	{
		if (ObjectTypeList.Instance.GetSubCategoryFromType(ResultType) != ObjectSubCategory.Hidden)
		{
			List<IngredientRequirement> list = new List<IngredientRequirement>();
			list.Add(new IngredientRequirement(IngredientType, IngredientCount));
			List<IngredientRequirement> list2 = new List<IngredientRequirement>();
			list2.Add(new IngredientRequirement(ResultType, ResultCount));
			list2.Add(new IngredientRequirement(ResultType2, ResultCount2));
			AddRequirements(BuildingType, list, list2);
		}
	}

	private void CheckUpgradeFrom(ObjectType NewType, ConverterResults NewResults)
	{
		int variableAsInt = VariableManager.Instance.GetVariableAsInt(NewType, "UpgradeFrom", CheckValid: false);
		if (variableAsInt == 0)
		{
			return;
		}
		ObjectType newType = (ObjectType)variableAsInt;
		ConverterResults converterResults = m_Results[variableAsInt];
		for (int i = 0; i < converterResults.m_Results.Count; i++)
		{
			List<IngredientRequirement> list = converterResults.m_Results[i];
			if (!NewResults.Contains(list[0].m_Type))
			{
				NewResults.Add(converterResults.m_Requirements[i], converterResults.m_Results[i]);
			}
		}
		CheckUpgradeFrom(newType, NewResults);
	}

	public ConverterResults GetResults(ObjectType BuildingType)
	{
		ConverterResults converterResults = new ConverterResults();
		converterResults.Add(m_Results[(int)BuildingType]);
		CheckUpgradeFrom(BuildingType, converterResults);
		return converterResults;
	}

	public ObjectType GetConverterForObject(ObjectType NewType)
	{
		for (int i = 0; i < (int)ObjectTypeList.m_Total; i++)
		{
			foreach (List<IngredientRequirement> result in m_Results[i].m_Results)
			{
				foreach (IngredientRequirement item in result)
				{
					if (item.m_Type == NewType)
					{
						return (ObjectType)i;
					}
				}
			}
		}
		return ObjectTypeList.m_Total;
	}

	public void Init()
	{
		m_Results = new ConverterResults[(int)ObjectTypeList.m_Total];
		for (int i = 0; i < (int)ObjectTypeList.m_Total; i++)
		{
			m_Results[i] = new ConverterResults();
		}
		Add(ObjectType.Barn, ObjectType.AnimalCow);
		Add(ObjectType.Barn, ObjectType.AnimalSheep);
		List<IngredientRequirement> list = new List<IngredientRequirement>();
		list.Add(new IngredientRequirement(ObjectType.AnimalCowHighland, 2));
		list.Add(new IngredientRequirement(ObjectType.HayBale, 3));
		Add(ObjectType.Barn, list, ObjectType.AnimalCowHighland, 1);
		list = new List<IngredientRequirement>();
		list.Add(new IngredientRequirement(ObjectType.AnimalAlpaca, 2));
		list.Add(new IngredientRequirement(ObjectType.HayBale, 3));
		Add(ObjectType.Barn, list, ObjectType.AnimalAlpaca, 1);
		Add(ObjectType.BasicMetalWorkbench, ObjectType.MetalPoleCrude);
		Add(ObjectType.BasicMetalWorkbench, ObjectType.MetalPlateCrude);
		Add(ObjectType.BasicMetalWorkbench, ObjectType.ToolShovel);
		Add(ObjectType.BasicMetalWorkbench, ObjectType.ToolAxe);
		Add(ObjectType.BasicMetalWorkbench, ObjectType.ToolHoe);
		Add(ObjectType.BasicMetalWorkbench, ObjectType.ToolScythe);
		Add(ObjectType.BasicMetalWorkbench, ObjectType.ToolPick);
		Add(ObjectType.BasicMetalWorkbench, ObjectType.ToolChisel);
		Add(ObjectType.BasicMetalWorkbench, ObjectType.ToolBucketMetal);
		Add(ObjectType.BasicMetalWorkbench, ObjectType.ToolBlade);
		Add(ObjectType.ChoppingBlock, ObjectType.Log, 1, ObjectType.Plank, 2);
		Add(ObjectType.ChoppingBlock, ObjectType.Plank, 1, ObjectType.Pole, 2);
		Add(ObjectType.ChoppingBlock, ObjectType.Pole, 1, ObjectType.FixingPeg, 2);
		Add(ObjectType.CrudePlantBreedingStation, ObjectType.CarrotSeed);
		Add(ObjectType.CrudePlantBreedingStation, ObjectType.Coconut);
		Add(ObjectType.CrudePlantBreedingStation, ObjectType.MulberrySeed);
		Add(ObjectType.CrudeAnimalBreedingStation, ObjectType.AnimalCowHighland);
		Add(ObjectType.CrudeAnimalBreedingStation, ObjectType.AnimalAlpaca);
		Add(ObjectType.CrudeAnimalBreedingStation, ObjectType.AnimalSilkworm);
		Add(ObjectType.BenchSaw, ObjectType.Log, 1, ObjectType.Plank, 2);
		Add(ObjectType.BenchSaw, ObjectType.Plank, 1, ObjectType.Pole, 2);
		Add(ObjectType.BenchSaw, ObjectType.Pole, 1, ObjectType.FixingPeg, 2);
		Add(ObjectType.BenchSaw2, ObjectType.Log, 1, ObjectType.Plank, 3);
		Add(ObjectType.BenchSaw2, ObjectType.Plank, 1, ObjectType.Pole, 3);
		Add(ObjectType.BenchSaw2, ObjectType.Pole, 1, ObjectType.FixingPeg, 3);
		Add(ObjectType.BenchSaw2, ObjectType.WoodenBeam);
		Add(ObjectType.ButterChurn, ObjectType.Butter);
		Add(ObjectType.Cauldron, ObjectType.MushroomStew);
		Add(ObjectType.Cauldron, ObjectType.PumpkinStew);
		Add(ObjectType.Cauldron, ObjectType.BerriesJam);
		Add(ObjectType.Cauldron, ObjectType.AppleJam);
		Add(ObjectType.Cauldron, ObjectType.FishStew);
		Add(ObjectType.Cauldron, ObjectType.FruitPorridge);
		Add(ObjectType.Cauldron, ObjectType.HoneyPorridge);
		Add(ObjectType.Cauldron, ObjectType.CarrotCurry);
		Add(ObjectType.Cauldron, ObjectType.Ink);
		Add(ObjectType.ChickenCoop, ObjectType.AnimalChicken);
		Add(ObjectType.ClayFurnace, ObjectType.Charcoal);
		Add(ObjectType.ClayFurnace, ObjectType.IronCrude);
		Add(ObjectType.ClayStation, ObjectType.PotClayRaw, 2);
		Add(ObjectType.ClayStation, ObjectType.LargeBowlClayRaw);
		Add(ObjectType.ClayStation, ObjectType.BricksCrudeRaw, 2);
		Add(ObjectType.ClayStation, ObjectType.FlowerPotRaw);
		Add(ObjectType.ClayStation, ObjectType.JarClayRaw);
		Add(ObjectType.ClayStation, ObjectType.RoofTilesRaw);
		Add(ObjectType.ClayStationCrude, ObjectType.PotClayRaw);
		Add(ObjectType.ClayStationCrude, ObjectType.BricksCrudeRaw);
		Add(ObjectType.ClayStationCrude, ObjectType.GnomeRaw);
		Add(ObjectType.CogBench, ObjectType.WheelCrude);
		Add(ObjectType.CogBench, ObjectType.Wheel);
		Add(ObjectType.CogBench, ObjectType.Axle);
		Add(ObjectType.CogBench, ObjectType.Cog);
		Add(ObjectType.CogBench, ObjectType.Buttons);
		Add(ObjectType.CookingPotCrude, ObjectType.MushroomSoup);
		Add(ObjectType.CookingPotCrude, ObjectType.BerriesStew);
		Add(ObjectType.CookingPotCrude, ObjectType.AppleStew);
		Add(ObjectType.CookingPotCrude, ObjectType.FishSoup);
		Add(ObjectType.CookingPotCrude, ObjectType.PumpkinSoup);
		Add(ObjectType.CookingPotCrude, ObjectType.MilkPorridge);
		Add(ObjectType.CookingPotCrude, ObjectType.CarrotStirFry);
		Add(ObjectType.Easel, ObjectType.ArtPortrait);
		Add(ObjectType.Easel, ObjectType.ArtStillLife);
		Add(ObjectType.Easel, ObjectType.ArtAbstract);
		Add(ObjectType.FolkSeedPod, ObjectType.FolkSeed);
		Add(ObjectType.FolkSeedRehydrator, ObjectType.Folk);
		Add(ObjectType.Gristmill, ObjectType.Flour);
		Add(ObjectType.HatMaker, ObjectType.HatFarmerCap);
		Add(ObjectType.HatMaker, ObjectType.HatBeret);
		Add(ObjectType.HatMaker, ObjectType.HatCap);
		Add(ObjectType.HatMaker, ObjectType.HatLumberjack);
		Add(ObjectType.HatMaker, ObjectType.HatBaseballShow);
		Add(ObjectType.HatMaker, ObjectType.HatMushroom);
		Add(ObjectType.HatMaker, ObjectType.HatChef);
		Add(ObjectType.HatMaker, ObjectType.HatAdventurer);
		Add(ObjectType.HatMaker, ObjectType.HatCrown);
		Add(ObjectType.HatMaker, ObjectType.HatMortarboard);
		Add(ObjectType.HatMaker, ObjectType.HatSouwester);
		Add(ObjectType.HatMaker, ObjectType.HatTree);
		Add(ObjectType.HatMaker, ObjectType.HatMadHatter);
		Add(ObjectType.HatMaker, ObjectType.HatCloche);
		Add(ObjectType.HatMaker, ObjectType.HatAcorn);
		Add(ObjectType.HatMaker, ObjectType.HatFez);
		Add(ObjectType.HatMaker, ObjectType.HatSanta);
		Add(ObjectType.HatMaker, ObjectType.HatWally);
		Add(ObjectType.HatMaker, ObjectType.HatParty);
		Add(ObjectType.HatMaker, ObjectType.HatViking);
		Add(ObjectType.HatMaker, ObjectType.HatBox);
		Add(ObjectType.HatMaker, ObjectType.HatTrain);
		Add(ObjectType.HatMaker, ObjectType.HatSailor);
		Add(ObjectType.HatMaker, ObjectType.HatMiner);
		Add(ObjectType.HatMaker, ObjectType.HatFox);
		Add(ObjectType.HatMaker, ObjectType.HatDinosaur);
		Add(ObjectType.HatMaker, ObjectType.HatAntlers);
		Add(ObjectType.HatMaker, ObjectType.HatBunny);
		Add(ObjectType.HatMaker, ObjectType.HatDuck);
		Add(ObjectType.KilnCrude, ObjectType.PotClay);
		Add(ObjectType.KilnCrude, ObjectType.LargeBowlClay);
		Add(ObjectType.KilnCrude, ObjectType.FlowerPot);
		Add(ObjectType.KilnCrude, ObjectType.BricksCrude);
		Add(ObjectType.KilnCrude, ObjectType.Gnome);
		Add(ObjectType.KilnCrude, ObjectType.Gnome2);
		Add(ObjectType.KilnCrude, ObjectType.Gnome3);
		Add(ObjectType.KilnCrude, ObjectType.Gnome4);
		Add(ObjectType.KilnCrude, ObjectType.Gnome5);
		Add(ObjectType.KilnCrude, ObjectType.Gnome6);
		Add(ObjectType.KilnCrude, ObjectType.RoofTiles);
		Add(ObjectType.KilnCrude, ObjectType.JarClay);
		Add(ObjectType.KitchenTable, ObjectType.Dough);
		Add(ObjectType.KitchenTable, ObjectType.DoughGood);
		Add(ObjectType.KitchenTable, ObjectType.CakeBatter);
		Add(ObjectType.KitchenTable, ObjectType.MushroomPieRaw);
		Add(ObjectType.KitchenTable, ObjectType.BerriesPieRaw);
		Add(ObjectType.KitchenTable, ObjectType.BerriesCakeRaw);
		Add(ObjectType.KitchenTable, ObjectType.BreadButtered);
		Add(ObjectType.KitchenTable, ObjectType.BreadPuddingRaw);
		Add(ObjectType.KitchenTable, ObjectType.Pastry);
		Add(ObjectType.KitchenTable, ObjectType.ApplePieRaw);
		Add(ObjectType.KitchenTable, ObjectType.AppleCakeRaw);
		Add(ObjectType.KitchenTable, ObjectType.PumpkinPieRaw);
		Add(ObjectType.KitchenTable, ObjectType.FishPieRaw);
		Add(ObjectType.KitchenTable, ObjectType.FishRaw);
		Add(ObjectType.KitchenTable, ObjectType.NaanRaw);
		Add(ObjectType.KitchenTable, ObjectType.PumpkinCakeRaw);
		Add(ObjectType.KitchenTable, ObjectType.FishCakeRaw);
		Add(ObjectType.KitchenTable, ObjectType.CarrotCakeRaw);
		Add(ObjectType.KitchenTable, ObjectType.MushroomPuddingRaw);
		Add(ObjectType.KitchenTable, ObjectType.AppleBerryPieRaw);
		Add(ObjectType.KitchenTable, ObjectType.PumpkinMushroomPieRaw);
		Add(ObjectType.LoomCrude, ObjectType.Blanket);
		Add(ObjectType.LoomCrude, ObjectType.CottonCloth);
		Add(ObjectType.LoomCrude, ObjectType.BullrushesCloth);
		Add(ObjectType.LoomGood, ObjectType.TopCoatScarf);
		Add(ObjectType.LoomGood, ObjectType.TopShirtTie);
		Add(ObjectType.LoomGood, ObjectType.TopBlazer);
		Add(ObjectType.LoomGood, ObjectType.TopTuxedo);
		Add(ObjectType.LoomGood, ObjectType.TopGown);
		Add(ObjectType.LoomGood, ObjectType.TopSuit);
		Add(ObjectType.LoomGood, ObjectType.SilkCloth);
		Add(ObjectType.MasonryBench, ObjectType.StoneBlock);
		Add(ObjectType.MasonryBench, ObjectType.Millstone);
		Add(ObjectType.MasonryBench, ObjectType.Fireplace);
		Add(ObjectType.MasonryBench, ObjectType.Chimney);
		Add(ObjectType.MortarMixerCrude, ObjectType.Mortar);
		Add(ObjectType.MortarMixerGood, ObjectType.Mortar, 2);
		Add(ObjectType.Oven, ObjectType.Bread);
		Add(ObjectType.Oven, ObjectType.BreadPudding);
		Add(ObjectType.Oven, ObjectType.MushroomPie);
		Add(ObjectType.Oven, ObjectType.BerriesPie);
		Add(ObjectType.Oven, ObjectType.BerriesCake);
		Add(ObjectType.Oven, ObjectType.FishPie);
		Add(ObjectType.Oven, ObjectType.ApplePie);
		Add(ObjectType.Oven, ObjectType.AppleCake);
		Add(ObjectType.Oven, ObjectType.PumpkinPie);
		Add(ObjectType.Oven, ObjectType.Naan);
		Add(ObjectType.Oven, ObjectType.MushroomPudding);
		Add(ObjectType.Oven, ObjectType.PumpkinCake);
		Add(ObjectType.Oven, ObjectType.FishCake);
		Add(ObjectType.Oven, ObjectType.CarrotCake);
		Add(ObjectType.Oven, ObjectType.AppleBerryPie);
		Add(ObjectType.Oven, ObjectType.PumpkinMushroomPie);
		Add(ObjectType.Oven, ObjectType.MushroomBurger);
		Add(ObjectType.Oven, ObjectType.FishBurger);
		Add(ObjectType.Oven, ObjectType.CarrotBurger);
		Add(ObjectType.Oven, ObjectType.BerryDanish);
		Add(ObjectType.Oven, ObjectType.AppleDanish);
		Add(ObjectType.Oven, ObjectType.PumpkinBurger);
		Add(ObjectType.Oven, ObjectType.CreamBrioche);
		Add(ObjectType.OvenCrude, ObjectType.BreadCrude);
		Add(ObjectType.OvenCrude, ObjectType.CarrotHoney);
		Add(ObjectType.PotCrude, ObjectType.Porridge);
		Add(ObjectType.PotCrude, ObjectType.MushroomHerb);
		Add(ObjectType.PotCrude, ObjectType.FishHerb);
		Add(ObjectType.PotCrude, ObjectType.PumpkinHerb);
		Add(ObjectType.PotCrude, ObjectType.BerriesSpice);
		Add(ObjectType.PotCrude, ObjectType.AppleSpice);
		Add(ObjectType.PotCrude, ObjectType.CarrotSalad);
		Add(ObjectType.Quern, ObjectType.FlourCrude);
		Add(ObjectType.RockingChair, ObjectType.Blanket);
		Add(ObjectType.RockingChair, ObjectType.CottonCloth);
		Add(ObjectType.RockingChair, ObjectType.BullrushesCloth);
		Add(ObjectType.RockingChair, ObjectType.TopPoncho);
		Add(ObjectType.RockingChair, ObjectType.TopJumper);
		Add(ObjectType.RockingChair, ObjectType.TopTunic);
		Add(ObjectType.RockingChair, ObjectType.TopDress);
		Add(ObjectType.RockingChair, ObjectType.TopToga);
		Add(ObjectType.RockingChair, ObjectType.TopRobe);
		Add(ObjectType.RockingChair, ObjectType.HatChullo);
		Add(ObjectType.RockingChair, ObjectType.HatSugegasa);
		Add(ObjectType.RockingChair, ObjectType.HatFarmer);
		Add(ObjectType.RockingChair, ObjectType.HatKnittedBeanie);
		Add(ObjectType.RockingChair, ObjectType.HatWig01);
		Add(ObjectType.SewingStation, ObjectType.TopJacket);
		Add(ObjectType.SewingStation, ObjectType.TopShirt);
		Add(ObjectType.SewingStation, ObjectType.TopCoat);
		Add(ObjectType.SewingStation, ObjectType.TopMac);
		Add(ObjectType.SewingStation, ObjectType.TopTree);
		Add(ObjectType.SewingStation, ObjectType.TopLumberjack);
		Add(ObjectType.SewingStation, ObjectType.TopDungarees);
		Add(ObjectType.SewingStation, ObjectType.TopPlumber);
		Add(ObjectType.SewingStation, ObjectType.TopAdventurer);
		Add(ObjectType.SewingStation, ObjectType.TopTShirtShow);
		Add(ObjectType.SewingStation, ObjectType.TopDungareesClown);
		Add(ObjectType.SewingStation, ObjectType.TopJumper02);
		Add(ObjectType.SewingStation, ObjectType.TopApron);
		Add(ObjectType.SewingStation, ObjectType.TopSanta);
		Add(ObjectType.SewingStation, ObjectType.TopWally);
		Add(ObjectType.SewingStation, ObjectType.TopTShirt02);
		Add(ObjectType.SewingStation, ObjectType.TopFox);
		Add(ObjectType.SewingStation, ObjectType.TopDinosaur);
		Add(ObjectType.SewingStation, ObjectType.TopBunny);
		Add(ObjectType.SewingStation, ObjectType.TopDuck);
		Add(ObjectType.SpinningWheel, ObjectType.Wool);
		Add(ObjectType.SpinningWheel, ObjectType.Thread);
		Add(ObjectType.SpinningWheel, ObjectType.CottonThread);
		Add(ObjectType.SpinningWheel, ObjectType.BullrushesThread);
		Add(ObjectType.SpinningJenny, ObjectType.Wool, 2);
		Add(ObjectType.SpinningJenny, ObjectType.Thread, 2);
		Add(ObjectType.SpinningJenny, ObjectType.CottonThread, 2);
		Add(ObjectType.SpinningJenny, ObjectType.BullrushesThread, 2);
		Add(ObjectType.SpinningJenny, ObjectType.SilkThread);
		Add(ObjectType.StringWinderCrude, ObjectType.StringBall);
		Add(ObjectType.ToyStationCrude, ObjectType.Doll);
		Add(ObjectType.ToyStationCrude, ObjectType.JackInTheBox);
		Add(ObjectType.ToyStationCrude, ObjectType.DollHouse);
		Add(ObjectType.ToyStationCrude, ObjectType.Spaceship);
		Add(ObjectType.ToyStationCrude, ObjectType.ToyHorse);
		Add(ObjectType.ToyStationCrude, ObjectType.ToyHorseCart);
		Add(ObjectType.ToyStationCrude, ObjectType.ToyHorseCarriage);
		Add(ObjectType.ToyStationCrude, ObjectType.ToyTrain);
		Add(ObjectType.MedicineStation, ObjectType.MedicineLeeches);
		Add(ObjectType.MedicineStation, ObjectType.MedicineFlowers);
		Add(ObjectType.MedicineStation, ObjectType.MedicinePills);
		Add(ObjectType.MedicineStation, ObjectType.PaintRed);
		Add(ObjectType.MedicineStation, ObjectType.PaintYellow);
		Add(ObjectType.MedicineStation, ObjectType.PaintBlue);
		Add(ObjectType.PrintingPress, ObjectType.EducationBook1);
		Add(ObjectType.PrintingPress, ObjectType.EducationEncyclopedia);
		Add(ObjectType.PaperMill, ObjectType.Paper);
		Add(ObjectType.VehicleAssembler, ObjectType.WheelBarrow);
		Add(ObjectType.VehicleAssembler, ObjectType.Cart);
		Add(ObjectType.VehicleAssembler, ObjectType.CartLiquid);
		Add(ObjectType.VehicleAssembler, ObjectType.Canoe);
		Add(ObjectType.VehicleAssemblerGood, ObjectType.CraneCrude);
		Add(ObjectType.VehicleAssemblerGood, ObjectType.Minecart);
		Add(ObjectType.VehicleAssemblerGood, ObjectType.Train);
		Add(ObjectType.VehicleAssemblerGood, ObjectType.Carriage);
		Add(ObjectType.VehicleAssemblerGood, ObjectType.CarriageLiquid);
		Add(ObjectType.VehicleAssemblerGood, ObjectType.CarriageTrain);
		Add(ObjectType.WheatHammer, ObjectType.Wheat, 1, ObjectType.WheatSeed, 2, ObjectType.Straw, 1);
		Add(ObjectType.WheatHammer, ObjectType.CottonBall, 1, ObjectType.CottonLint, 1, ObjectType.CottonSeeds, 2);
		Add(ObjectType.WheatHammer, ObjectType.BullrushesFibre);
		Add(ObjectType.Workbench, ObjectType.ToolAxeStone);
		Add(ObjectType.Workbench, ObjectType.ToolShovelStone);
		Add(ObjectType.Workbench, ObjectType.ToolFlailCrude);
		Add(ObjectType.Workbench, ObjectType.ToolHoeStone);
		Add(ObjectType.Workbench, ObjectType.RockSharp);
		Add(ObjectType.Workbench, ObjectType.ToolScytheStone);
		Add(ObjectType.Workbench, ObjectType.ToolPickStone);
		Add(ObjectType.Workbench, ObjectType.ToolMallet);
		Add(ObjectType.Workbench, ObjectType.ToolFishingStick);
		Add(ObjectType.Workbench, ObjectType.ToolNetCrude);
		Add(ObjectType.Workbench, ObjectType.WheelCrude);
		Add(ObjectType.Workbench, ObjectType.CogCrude);
		Add(ObjectType.Workbench, ObjectType.UpgradePlayerInventoryCrude);
		Add(ObjectType.Workbench, ObjectType.UpgradePlayerInventoryGood);
		Add(ObjectType.Workbench, ObjectType.UpgradePlayerMovementCrude);
		Add(ObjectType.Workbench, ObjectType.UpgradePlayerMovementGood);
		Add(ObjectType.Workbench, ObjectType.DataStorageCrude);
		Add(ObjectType.WorkbenchMk2, ObjectType.ToolFlail);
		Add(ObjectType.WorkbenchMk2, ObjectType.ToolShears);
		Add(ObjectType.WorkbenchMk2, ObjectType.ToolBucketCrude);
		Add(ObjectType.WorkbenchMk2, ObjectType.ToolBucket);
		Add(ObjectType.WorkbenchMk2, ObjectType.ToolFishingRod);
		Add(ObjectType.WorkbenchMk2, ObjectType.ToolFishingRodGood);
		Add(ObjectType.WorkbenchMk2, ObjectType.Sign);
		Add(ObjectType.WorkbenchMk2, ObjectType.Sign2);
		Add(ObjectType.WorkbenchMk2, ObjectType.Sign3);
		Add(ObjectType.WorkbenchMk2, ObjectType.Billboard);
		Add(ObjectType.WorkbenchMk2, ObjectType.ToolTorchCrude);
		Add(ObjectType.WorkbenchMk2, ObjectType.ToolPitchfork);
		Add(ObjectType.WorkbenchMk2, ObjectType.ToolBroom);
		Add(ObjectType.WorkbenchMk2, ObjectType.ToolChiselCrude);
		Add(ObjectType.WorkbenchMk2, ObjectType.ToolDredgerCrude);
		Add(ObjectType.WorkbenchMk2, ObjectType.DataStorageCrude);
		Add(ObjectType.WorkbenchMk2, ObjectType.Scarecrow);
		Add(ObjectType.WorkbenchMk2, ObjectType.ToolWateringCan);
		Add(ObjectType.WorkbenchMk2, ObjectType.ToolNet);
		Add(ObjectType.WorkbenchMk2, ObjectType.UpgradePlayerInventorySuper);
		Add(ObjectType.WorkbenchMk2, ObjectType.UpgradePlayerMovementSuper);
		Add(ObjectType.WorkbenchMk2, ObjectType.Canvas);
		Add(ObjectType.WorkbenchStructural, ObjectType.FrameSquare);
		Add(ObjectType.WorkbenchStructural, ObjectType.FrameTriangle);
		Add(ObjectType.WorkbenchStructural, ObjectType.Panel);
		Add(ObjectType.WorkbenchStructural, ObjectType.FrameWindow);
		Add(ObjectType.WorkbenchStructural, ObjectType.FrameDoor);
		Add(ObjectType.WorkbenchStructural, ObjectType.FrameBox);
		Add(ObjectType.WorkbenchStructural, ObjectType.Crank);
		Add(ObjectType.WorkerAssembler, ObjectType.BasicWorker);
		Add(ObjectType.WorkerAssembler, ObjectType.Worker);
		Add(ObjectType.WorkerWorkbenchMk1, ObjectType.WorkerHeadMk1);
		Add(ObjectType.WorkerWorkbenchMk1, ObjectType.WorkerFrameMk1);
		Add(ObjectType.WorkerWorkbenchMk1, ObjectType.WorkerDriveMk1);
		Add(ObjectType.WorkerWorkbenchMk1, ObjectType.WorkerHeadMk1Variant1);
		Add(ObjectType.WorkerWorkbenchMk1, ObjectType.WorkerFrameMk1Variant1);
		Add(ObjectType.WorkerWorkbenchMk1, ObjectType.WorkerDriveMk1Variant1);
		Add(ObjectType.WorkerWorkbenchMk1, ObjectType.WorkerHeadMk1Variant2);
		Add(ObjectType.WorkerWorkbenchMk1, ObjectType.WorkerFrameMk1Variant2);
		Add(ObjectType.WorkerWorkbenchMk1, ObjectType.WorkerDriveMk1Variant2);
		Add(ObjectType.WorkerWorkbenchMk1, ObjectType.WorkerHeadMk1Variant3);
		Add(ObjectType.WorkerWorkbenchMk1, ObjectType.WorkerFrameMk1Variant3);
		Add(ObjectType.WorkerWorkbenchMk1, ObjectType.WorkerDriveMk1Variant3);
		Add(ObjectType.WorkerWorkbenchMk1, ObjectType.WorkerHeadMk1Variant4);
		Add(ObjectType.WorkerWorkbenchMk1, ObjectType.WorkerFrameMk1Variant4);
		Add(ObjectType.WorkerWorkbenchMk1, ObjectType.WorkerDriveMk1Variant4);
		Add(ObjectType.WorkerWorkbenchMk1, ObjectType.UpgradeWorkerMemoryCrude);
		Add(ObjectType.WorkerWorkbenchMk1, ObjectType.UpgradeWorkerSearchCrude);
		Add(ObjectType.WorkerWorkbenchMk1, ObjectType.UpgradeWorkerCarryCrude);
		Add(ObjectType.WorkerWorkbenchMk1, ObjectType.UpgradeWorkerMovementCrude);
		Add(ObjectType.WorkerWorkbenchMk1, ObjectType.UpgradeWorkerEnergyCrude);
		Add(ObjectType.WorkerWorkbenchMk1, ObjectType.UpgradeWorkerInventoryCrude);
		Add(ObjectType.WorkerWorkbenchMk2, ObjectType.WorkerHeadMk2);
		Add(ObjectType.WorkerWorkbenchMk2, ObjectType.WorkerFrameMk2);
		Add(ObjectType.WorkerWorkbenchMk2, ObjectType.WorkerDriveMk2);
		Add(ObjectType.WorkerWorkbenchMk2, ObjectType.WorkerHeadMk2Variant1);
		Add(ObjectType.WorkerWorkbenchMk2, ObjectType.WorkerFrameMk2Variant1);
		Add(ObjectType.WorkerWorkbenchMk2, ObjectType.WorkerDriveMk2Variant1);
		Add(ObjectType.WorkerWorkbenchMk2, ObjectType.WorkerHeadMk2Variant2);
		Add(ObjectType.WorkerWorkbenchMk2, ObjectType.WorkerFrameMk2Variant2);
		Add(ObjectType.WorkerWorkbenchMk2, ObjectType.WorkerDriveMk2Variant2);
		Add(ObjectType.WorkerWorkbenchMk2, ObjectType.WorkerHeadMk2Variant3);
		Add(ObjectType.WorkerWorkbenchMk2, ObjectType.WorkerFrameMk2Variant3);
		Add(ObjectType.WorkerWorkbenchMk2, ObjectType.WorkerDriveMk2Variant3);
		Add(ObjectType.WorkerWorkbenchMk2, ObjectType.WorkerHeadMk2Variant4);
		Add(ObjectType.WorkerWorkbenchMk2, ObjectType.WorkerFrameMk2Variant4);
		Add(ObjectType.WorkerWorkbenchMk2, ObjectType.WorkerDriveMk2Variant4);
		Add(ObjectType.WorkerWorkbenchMk2, ObjectType.UpgradeWorkerMemoryGood);
		Add(ObjectType.WorkerWorkbenchMk2, ObjectType.UpgradeWorkerSearchGood);
		Add(ObjectType.WorkerWorkbenchMk2, ObjectType.UpgradeWorkerCarryGood);
		Add(ObjectType.WorkerWorkbenchMk2, ObjectType.UpgradeWorkerMovementGood);
		Add(ObjectType.WorkerWorkbenchMk2, ObjectType.UpgradeWorkerEnergyGood);
		Add(ObjectType.WorkerWorkbenchMk2, ObjectType.UpgradeWorkerInventoryGood);
		Add(ObjectType.HayBalerCrude, ObjectType.HayBale, 2);
		Add(ObjectType.WorkerWorkbenchMk3, ObjectType.WorkerHeadMk3);
		Add(ObjectType.WorkerWorkbenchMk3, ObjectType.WorkerFrameMk3);
		Add(ObjectType.WorkerWorkbenchMk3, ObjectType.WorkerDriveMk3);
		Add(ObjectType.WorkerWorkbenchMk3, ObjectType.WorkerHeadMk3Variant1);
		Add(ObjectType.WorkerWorkbenchMk3, ObjectType.WorkerFrameMk3Variant1);
		Add(ObjectType.WorkerWorkbenchMk3, ObjectType.WorkerDriveMk3Variant1);
		Add(ObjectType.WorkerWorkbenchMk3, ObjectType.WorkerHeadMk3Variant2);
		Add(ObjectType.WorkerWorkbenchMk3, ObjectType.WorkerFrameMk3Variant2);
		Add(ObjectType.WorkerWorkbenchMk3, ObjectType.WorkerDriveMk3Variant2);
		Add(ObjectType.WorkerWorkbenchMk3, ObjectType.WorkerHeadMk3Variant3);
		Add(ObjectType.WorkerWorkbenchMk3, ObjectType.WorkerFrameMk3Variant3);
		Add(ObjectType.WorkerWorkbenchMk3, ObjectType.WorkerDriveMk3Variant3);
		Add(ObjectType.WorkerWorkbenchMk3, ObjectType.WorkerHeadMk3Variant4);
		Add(ObjectType.WorkerWorkbenchMk3, ObjectType.WorkerFrameMk3Variant4);
		Add(ObjectType.WorkerWorkbenchMk3, ObjectType.WorkerDriveMk3Variant4);
		Add(ObjectType.WorkerWorkbenchMk3, ObjectType.UpgradeWorkerMemorySuper);
		Add(ObjectType.WorkerWorkbenchMk3, ObjectType.UpgradeWorkerSearchSuper);
		Add(ObjectType.WorkerWorkbenchMk3, ObjectType.UpgradeWorkerCarrySuper);
		Add(ObjectType.WorkerWorkbenchMk3, ObjectType.UpgradeWorkerMovementSuper);
		Add(ObjectType.WorkerWorkbenchMk3, ObjectType.UpgradeWorkerEnergySuper);
		Add(ObjectType.WorkerWorkbenchMk3, ObjectType.UpgradeWorkerInventorySuper);
		Add(ObjectType.MetalWorkbench, ObjectType.MetalCog);
		Add(ObjectType.MetalWorkbench, ObjectType.MetalWheel);
		Add(ObjectType.MetalWorkbench, ObjectType.MetalAxle);
		Add(ObjectType.MetalWorkbench, ObjectType.Flywheel);
		Add(ObjectType.MetalWorkbench, ObjectType.ConnectingRod);
		Add(ObjectType.MetalWorkbench, ObjectType.Piston);
		Add(ObjectType.MetalWorkbench, ObjectType.MetalGirder);
		Add(ObjectType.MetalWorkbench, ObjectType.Boiler);
		Add(ObjectType.MetalWorkbench, ObjectType.Firebox);
		Add(ObjectType.MetalWorkbench, ObjectType.Rivets, 2);
		Add(ObjectType.Furnace, ObjectType.IronOre, 2, ObjectType.IronCrude, 1);
		Add(ObjectType.Workbench, ObjectType.UpgradePlayerWhistleCrude);
		Add(ObjectType.Workbench, ObjectType.UpgradePlayerWhistleGood);
		Add(ObjectType.Workbench, ObjectType.UpgradePlayerWhistleSuper);
		Add(ObjectType.Workbench, ObjectType.Triangle);
		Add(ObjectType.Workbench, ObjectType.Castanets);
		Add(ObjectType.Workbench, ObjectType.Cowbell);
		Add(ObjectType.Workbench, ObjectType.Guiro);
		Add(ObjectType.Workbench, ObjectType.Maracas);
		Add(ObjectType.Workbench, ObjectType.JawHarp);
		Add(ObjectType.Workbench, ObjectType.Guitar);
		Add(ObjectType.CrudeAnimalBreedingStation, ObjectType.AnimalPetDog);
		Add(ObjectType.CrudeAnimalBreedingStation, ObjectType.AnimalPetDog2);
		Add(ObjectType.HatMaker, ObjectType.HatFrog);
		Add(ObjectType.HatMaker, ObjectType.HatPanda);
		Add(ObjectType.HatMaker, ObjectType.HatPenguin);
		Add(ObjectType.HatMaker, ObjectType.HatBearskin);
		Add(ObjectType.HatMaker, ObjectType.HatCaptain);
		Add(ObjectType.HatMaker, ObjectType.HatCowboy);
		Add(ObjectType.HatMaker, ObjectType.HatBoater);
		Add(ObjectType.HatMaker, ObjectType.HatCatintheHat);
		Add(ObjectType.HatMaker, ObjectType.HatDrumMajor);
		Add(ObjectType.HatMaker, ObjectType.HatGat);
		Add(ObjectType.HatMaker, ObjectType.HatFedora);
		Add(ObjectType.HatMaker, ObjectType.HatSombrero);
		Add(ObjectType.HatMaker, ObjectType.HatSmurf);
		Add(ObjectType.HatMaker, ObjectType.HatTrafficCone);
		Add(ObjectType.HatMaker, ObjectType.HatWig02);
		Add(ObjectType.RockingChair, ObjectType.TopFrog);
		Add(ObjectType.RockingChair, ObjectType.TopPanda);
		Add(ObjectType.RockingChair, ObjectType.TopPenguin);
		Add(ObjectType.RockingChair, ObjectType.TopRoyalGuard);
		Add(ObjectType.RockingChair, ObjectType.TopSuit02);
		Add(ObjectType.VehicleAssembler, ObjectType.TrojanRabbit);
		Add(ObjectType.WorkerWorkbenchMk1, ObjectType.WorkerHeadROB);
		Add(ObjectType.WorkerWorkbenchMk1, ObjectType.WorkerFrameROB);
		Add(ObjectType.WorkerWorkbenchMk1, ObjectType.WorkerDriveROB);
	}

	public void ReplaceResults(ObjectType BuildingType, ObjectType ResultType, int ResultAmount)
	{
		List<IngredientRequirement> list = new List<IngredientRequirement>();
		list.Add(new IngredientRequirement(ResultType, ResultAmount));
		List<IngredientRequirement> list2 = new List<IngredientRequirement>();
		if (list[0].m_Type != ObjectTypeList.m_Total)
		{
			IngredientRequirement[] ingredientsFromIdentifier = ObjectTypeList.Instance.GetIngredientsFromIdentifier(list[0].m_Type);
			for (int i = 0; i < ingredientsFromIdentifier.Length; i++)
			{
				list2.Add(new IngredientRequirement(ingredientsFromIdentifier[i].m_Type, ingredientsFromIdentifier[i].m_Count));
			}
		}
		bool flag = false;
		if (BuildingType != 0)
		{
			int count = m_Results[(int)BuildingType].m_Results.Count;
			for (int j = 0; j < count; j++)
			{
				if (m_Results[(int)BuildingType].m_Results[j][0].m_Type == ResultType)
				{
					m_Results[(int)BuildingType].m_Results.Remove(m_Results[(int)BuildingType].m_Results[j]);
					m_Results[(int)BuildingType].m_Requirements.Remove(m_Results[(int)BuildingType].m_Requirements[j]);
					m_Results[(int)BuildingType].Add(list2, list);
					flag = true;
					break;
				}
			}
		}
		else
		{
			int num = m_Results.Length;
			for (int k = 0; k < num; k++)
			{
				int count2 = m_Results[k].m_Results.Count;
				for (int l = 0; l < count2; l++)
				{
					if (m_Results[k].m_Results[l][0].m_Type == ResultType)
					{
						m_Results[k].m_Results.Remove(m_Results[k].m_Results[l]);
						m_Results[k].m_Requirements.Remove(m_Results[k].m_Requirements[l]);
						m_Results[k].Add(list2, list);
						flag = true;
						break;
					}
				}
			}
		}
		if (!flag && BuildingType != 0)
		{
			m_Results[(int)BuildingType].Add(list2, list);
		}
	}

	public void ReplaceResultsDouble(ObjectType BuildingType, ObjectType ResultType1, int ResultAmount1, ObjectType ResultType2, int ResultAmount2)
	{
		List<IngredientRequirement> list = new List<IngredientRequirement>();
		list.Add(new IngredientRequirement(ResultType1, ResultAmount1));
		list.Add(new IngredientRequirement(ResultType2, ResultAmount2));
		List<IngredientRequirement> list2 = new List<IngredientRequirement>();
		IngredientRequirement ingredientRequirement = list[0];
		if (ingredientRequirement.m_Type < ObjectTypeList.m_Total)
		{
			IngredientRequirement[] ingredientsFromIdentifier = ObjectTypeList.Instance.GetIngredientsFromIdentifier(ingredientRequirement.m_Type);
			for (int i = 0; i < ingredientsFromIdentifier.Length; i++)
			{
				list2.Add(new IngredientRequirement(ingredientsFromIdentifier[i].m_Type, ingredientsFromIdentifier[i].m_Count));
			}
		}
		if (BuildingType != 0)
		{
			int count = m_Results[(int)BuildingType].m_Results.Count;
			bool flag = false;
			for (int j = 0; j < count; j++)
			{
				int count2 = m_Results[(int)BuildingType].m_Results[j].Count;
				for (int k = 0; k < count2; k++)
				{
					if (m_Results[(int)BuildingType].m_Results[j][k].m_Type == ResultType1)
					{
						m_Results[(int)BuildingType].m_Results.Remove(m_Results[(int)BuildingType].m_Results[j]);
						m_Results[(int)BuildingType].m_Requirements.Remove(m_Results[(int)BuildingType].m_Requirements[j]);
						flag = true;
						break;
					}
				}
				if (flag)
				{
					break;
				}
			}
			count = m_Results[(int)BuildingType].m_Results.Count;
			flag = false;
			for (int l = 0; l < count; l++)
			{
				int count3 = m_Results[(int)BuildingType].m_Results[l].Count;
				for (int m = 0; m < count3; m++)
				{
					if (m_Results[(int)BuildingType].m_Results[l][m].m_Type == ResultType2)
					{
						m_Results[(int)BuildingType].m_Results.Remove(m_Results[(int)BuildingType].m_Results[l]);
						m_Results[(int)BuildingType].m_Requirements.Remove(m_Results[(int)BuildingType].m_Requirements[l]);
						flag = true;
						break;
					}
				}
				if (flag)
				{
					break;
				}
			}
		}
		else
		{
			int num = m_Results.Length;
			for (int n = 0; n < num; n++)
			{
				int count4 = m_Results[(int)BuildingType].m_Results.Count;
				for (int num2 = 0; num2 < count4; num2++)
				{
					int count5 = m_Results[(int)BuildingType].m_Results[num2].Count;
					for (int num3 = 0; num3 < count5; num3++)
					{
						if (m_Results[n].m_Results[num2][num3].m_Type == ResultType1)
						{
							m_Results[n].m_Results.Remove(m_Results[n].m_Results[num2]);
							m_Results[n].m_Requirements.Remove(m_Results[n].m_Requirements[num2]);
							break;
						}
					}
				}
			}
			num = m_Results.Length;
			for (int num4 = 0; num4 < num; num4++)
			{
				int count6 = m_Results[(int)BuildingType].m_Results.Count;
				for (int num5 = 0; num5 < count6; num5++)
				{
					int count7 = m_Results[(int)BuildingType].m_Results[num5].Count;
					for (int num6 = 0; num6 < count7; num6++)
					{
						if (m_Results[num4].m_Results[num5][num6].m_Type == ResultType2)
						{
							m_Results[num4].m_Results.Remove(m_Results[num4].m_Results[num5]);
							m_Results[num4].m_Requirements.Remove(m_Results[num4].m_Requirements[num5]);
							break;
						}
					}
				}
			}
		}
		m_Results[(int)BuildingType].Add(list2, list);
	}

	public bool RemoveResults(ObjectType BuildingType, ObjectType ResultType)
	{
		int num = 0;
		bool result = false;
		if (BuildingType != 0)
		{
			num = m_Results[(int)BuildingType].m_Results.Count;
			for (int i = 0; i < num; i++)
			{
				if (m_Results[(int)BuildingType].m_Results[i][0].m_Type == ResultType)
				{
					m_Results[(int)BuildingType].m_Results.Remove(m_Results[(int)BuildingType].m_Results[i]);
					m_Results[(int)BuildingType].m_Requirements.Remove(m_Results[(int)BuildingType].m_Requirements[i]);
					result = true;
					break;
				}
			}
		}
		else
		{
			int num2 = m_Results.Length;
			for (int j = 0; j < num2; j++)
			{
				num = m_Results[j].m_Results.Count;
				for (int k = 0; k < num; k++)
				{
					if (m_Results[j].m_Results[k][0].m_Type == ResultType)
					{
						m_Results[j].m_Results.Remove(m_Results[j].m_Results[k]);
						m_Results[j].m_Requirements.Remove(m_Results[j].m_Requirements[k]);
						result = true;
						break;
					}
				}
			}
		}
		return result;
	}
}
