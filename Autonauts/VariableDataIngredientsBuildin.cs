public class VariableDataIngredientsBuildings
{
	public void Init()
	{
		IngredientRequirement[] ingredients = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.FrameSquare, 2),
			new IngredientRequirement(ObjectType.Plank, 8),
			new IngredientRequirement(ObjectType.FixingPeg, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.StorageFertiliser, ingredients);
		IngredientRequirement[] ingredients2 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Plank, 10),
			new IngredientRequirement(ObjectType.Pole, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.StorageLiquid, ingredients2);
		IngredientRequirement[] ingredients3 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Pole, 4),
			new IngredientRequirement(ObjectType.Plank, 6),
			new IngredientRequirement(ObjectType.Log, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.StorageSand, ingredients3);
		IngredientRequirement[] ingredients4 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Panel, 4),
			new IngredientRequirement(ObjectType.FixingPeg, 6)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.StorageSeedlings, ingredients4);
		IngredientRequirement[] ingredients5 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Panel, 4),
			new IngredientRequirement(ObjectType.FrameSquare, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.StorageWorker, ingredients5);
		IngredientRequirement[] ingredients6 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Plank, 4),
			new IngredientRequirement(ObjectType.Pole, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.StorageGeneric, ingredients6);
		IngredientRequirement[] ingredients7 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Straw, 8)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.StorageBeehiveCrude, ingredients7);
		IngredientRequirement[] ingredients8 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.FrameSquare, 2),
			new IngredientRequirement(ObjectType.Panel, 2),
			new IngredientRequirement(ObjectType.FixingPeg, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.StorageBeehive, ingredients8);
		IngredientRequirement[] ingredients9 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.WoodenBeam, 1),
			new IngredientRequirement(ObjectType.FrameTriangle, 3),
			new IngredientRequirement(ObjectType.Plank, 18),
			new IngredientRequirement(ObjectType.FixingPeg, 10)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.StorageLiquidMedium, ingredients9);
		IngredientRequirement[] ingredients10 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.WoodenBeam, 4),
			new IngredientRequirement(ObjectType.FrameTriangle, 3),
			new IngredientRequirement(ObjectType.Plank, 18),
			new IngredientRequirement(ObjectType.FixingPeg, 10)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.StorageSandMedium, ingredients10);
		IngredientRequirement[] ingredients11 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.WoodenBeam, 2),
			new IngredientRequirement(ObjectType.FrameBox, 2),
			new IngredientRequirement(ObjectType.Plank, 18),
			new IngredientRequirement(ObjectType.FixingPeg, 10)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.StorageGenericMedium, ingredients11);
		IngredientRequirement[] ingredients12 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.WoodenBeam, 4),
			new IngredientRequirement(ObjectType.FrameSquare, 4),
			new IngredientRequirement(ObjectType.Plank, 18),
			new IngredientRequirement(ObjectType.FixingPeg, 10)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.StoragePaletteMedium, ingredients12);
		IngredientRequirement[] ingredients13 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Log, 1),
			new IngredientRequirement(ObjectType.Plank, 3),
			new IngredientRequirement(ObjectType.Pole, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.StoragePalette, ingredients13);
		IngredientRequirement[] ingredients14 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Log, 1),
			new IngredientRequirement(ObjectType.ToolAxeStone, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ChoppingBlock, ingredients14);
		IngredientRequirement[] ingredients15 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Log, 2),
			new IngredientRequirement(ObjectType.Plank, 8),
			new IngredientRequirement(ObjectType.Pole, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.CrudePlantBreedingStation, ingredients15);
		IngredientRequirement[] ingredients16 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Log, 4),
			new IngredientRequirement(ObjectType.Panel, 4),
			new IngredientRequirement(ObjectType.FrameSquare, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.CrudeAnimalBreedingStation, ingredients16);
		IngredientRequirement[] ingredients17 = new IngredientRequirement[5]
		{
			new IngredientRequirement(ObjectType.Log, 3),
			new IngredientRequirement(ObjectType.Plank, 4),
			new IngredientRequirement(ObjectType.RockSharp, 1),
			new IngredientRequirement(ObjectType.CogCrude, 1),
			new IngredientRequirement(ObjectType.Crank, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.BenchSaw, ingredients17);
		IngredientRequirement[] ingredients18 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.FrameSquare, 3),
			new IngredientRequirement(ObjectType.Plank, 4),
			new IngredientRequirement(ObjectType.MetalPlateCrude, 1),
			new IngredientRequirement(ObjectType.Cog, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.BenchSaw2, ingredients18);
		IngredientRequirement[] ingredients19 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Log, 2),
			new IngredientRequirement(ObjectType.Stick, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Workbench, ingredients19);
		IngredientRequirement[] ingredients20 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.Log, 2),
			new IngredientRequirement(ObjectType.IronCrude, 5),
			new IngredientRequirement(ObjectType.StoneBlockCrude, 1),
			new IngredientRequirement(ObjectType.BullrushesCloth, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.BasicMetalWorkbench, ingredients20);
		IngredientRequirement[] ingredients21 = new IngredientRequirement[5]
		{
			new IngredientRequirement(ObjectType.MetalPlateCrude, 8),
			new IngredientRequirement(ObjectType.MetalPoleCrude, 4),
			new IngredientRequirement(ObjectType.WoodenBeam, 4),
			new IngredientRequirement(ObjectType.StoneBlock, 1),
			new IngredientRequirement(ObjectType.Cog, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.MetalWorkbench, ingredients21);
		IngredientRequirement[] ingredients22 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Log, 2),
			new IngredientRequirement(ObjectType.Plank, 3)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerAssembler, ingredients22);
		IngredientRequirement[] ingredients23 = new IngredientRequirement[5]
		{
			new IngredientRequirement(ObjectType.Panel, 2),
			new IngredientRequirement(ObjectType.FrameSquare, 1),
			new IngredientRequirement(ObjectType.Cog, 1),
			new IngredientRequirement(ObjectType.Crank, 1),
			new IngredientRequirement(ObjectType.RockSharp, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WheatHammer, ingredients23);
		IngredientRequirement[] ingredients24 = new IngredientRequirement[5]
		{
			new IngredientRequirement(ObjectType.FrameSquare, 2),
			new IngredientRequirement(ObjectType.Pole, 2),
			new IngredientRequirement(ObjectType.Plank, 6),
			new IngredientRequirement(ObjectType.RockSharp, 1),
			new IngredientRequirement(ObjectType.Crank, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.CogBench, ingredients24);
		IngredientRequirement[] ingredients25 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.FrameBox, 4),
			new IngredientRequirement(ObjectType.Panel, 6),
			new IngredientRequirement(ObjectType.Pole, 3),
			new IngredientRequirement(ObjectType.Cog, 3)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Windmill, ingredients25);
		IngredientRequirement[] ingredients26 = new IngredientRequirement[5]
		{
			new IngredientRequirement(ObjectType.FrameBox, 2),
			new IngredientRequirement(ObjectType.Panel, 3),
			new IngredientRequirement(ObjectType.MetalPoleCrude, 3),
			new IngredientRequirement(ObjectType.Cog, 2),
			new IngredientRequirement(ObjectType.Millstone, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Gristmill, ingredients26);
		IngredientRequirement[] ingredients27 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Clay, 4),
			new IngredientRequirement(ObjectType.Stick, 4),
			new IngredientRequirement(ObjectType.ToolTorchCrude, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.CookingPotCrude, ingredients27);
		IngredientRequirement[] ingredients28 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Clay, 10)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ClayFurnace, ingredients28);
		IngredientRequirement[] ingredients29 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.StoneBlock, 4),
			new IngredientRequirement(ObjectType.Rock, 10),
			new IngredientRequirement(ObjectType.Mortar, 6)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Furnace, ingredients29);
		IngredientRequirement[] ingredients30 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.FrameSquare, 2),
			new IngredientRequirement(ObjectType.Panel, 2),
			new IngredientRequirement(ObjectType.Plank, 4),
			new IngredientRequirement(ObjectType.FixingPeg, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.HatMaker, ingredients30);
		IngredientRequirement[] ingredients31 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.FrameSquare, 2),
			new IngredientRequirement(ObjectType.Plank, 6),
			new IngredientRequirement(ObjectType.Crank, 1),
			new IngredientRequirement(ObjectType.Cog, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ButterChurn, ingredients31);
		IngredientRequirement[] ingredients32 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.Log, 4),
			new IngredientRequirement(ObjectType.FrameSquare, 2),
			new IngredientRequirement(ObjectType.Panel, 2),
			new IngredientRequirement(ObjectType.FixingPeg, 8)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.KitchenTable, ingredients32);
		IngredientRequirement[] ingredients33 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.Panel, 1),
			new IngredientRequirement(ObjectType.FrameSquare, 1),
			new IngredientRequirement(ObjectType.Pole, 6),
			new IngredientRequirement(ObjectType.RockSharp, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.SpinningWheel, ingredients33);
		IngredientRequirement[] ingredients34 = new IngredientRequirement[6]
		{
			new IngredientRequirement(ObjectType.WoodenBeam, 4),
			new IngredientRequirement(ObjectType.MetalPlateCrude, 4),
			new IngredientRequirement(ObjectType.MetalPoleCrude, 2),
			new IngredientRequirement(ObjectType.MetalAxle, 1),
			new IngredientRequirement(ObjectType.MetalCog, 2),
			new IngredientRequirement(ObjectType.Rivets, 8)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.SpinningJenny, ingredients34);
		IngredientRequirement[] ingredients35 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.FrameSquare, 2),
			new IngredientRequirement(ObjectType.Plank, 4),
			new IngredientRequirement(ObjectType.Pole, 6)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.RockingChair, ingredients35);
		IngredientRequirement[] ingredients36 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.FrameSquare, 3),
			new IngredientRequirement(ObjectType.Panel, 3),
			new IngredientRequirement(ObjectType.Pole, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.SewingStation, ingredients36);
		IngredientRequirement[] ingredients37 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.Log, 8),
			new IngredientRequirement(ObjectType.FrameSquare, 6),
			new IngredientRequirement(ObjectType.Panel, 6),
			new IngredientRequirement(ObjectType.FixingPeg, 10)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Barn, ingredients37);
		IngredientRequirement[] ingredients38 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.Log, 2),
			new IngredientRequirement(ObjectType.Panel, 4),
			new IngredientRequirement(ObjectType.FrameSquare, 2),
			new IngredientRequirement(ObjectType.FixingPeg, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ChickenCoop, ingredients38);
		IngredientRequirement[] ingredients39 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Rock, 16),
			new IngredientRequirement(ObjectType.Mortar, 8),
			new IngredientRequirement(ObjectType.Water, 8)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Aquarium, ingredients39);
		IngredientRequirement[] ingredients40 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Stick, 5),
			new IngredientRequirement(ObjectType.Rock, 5)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.OvenCrude, ingredients40);
		IngredientRequirement[] ingredients41 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.BricksCrude, 12),
			new IngredientRequirement(ObjectType.Mortar, 6),
			new IngredientRequirement(ObjectType.StoneBlock, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Oven, ingredients41);
		IngredientRequirement[] ingredients42 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Clay, 5)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.KilnCrude, ingredients42);
		IngredientRequirement[] ingredients43 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.IronOre, 4),
			new IngredientRequirement(ObjectType.Clay, 8),
			new IngredientRequirement(ObjectType.ToolTorchCrude, 2),
			new IngredientRequirement(ObjectType.Stick, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Cauldron, ingredients43);
		IngredientRequirement[] ingredients44 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Panel, 3),
			new IngredientRequirement(ObjectType.FixingPeg, 6),
			new IngredientRequirement(ObjectType.Pole, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.MortarMixerCrude, ingredients44);
		IngredientRequirement[] ingredients45 = new IngredientRequirement[5]
		{
			new IngredientRequirement(ObjectType.FrameTriangle, 4),
			new IngredientRequirement(ObjectType.Plank, 6),
			new IngredientRequirement(ObjectType.Crank, 1),
			new IngredientRequirement(ObjectType.MetalPoleCrude, 1),
			new IngredientRequirement(ObjectType.Cog, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.MortarMixerGood, ingredients45);
		IngredientRequirement[] ingredients46 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.Rock, 2),
			new IngredientRequirement(ObjectType.FrameSquare, 2),
			new IngredientRequirement(ObjectType.Pole, 4),
			new IngredientRequirement(ObjectType.FixingPeg, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Quern, ingredients46);
		IngredientRequirement[] ingredients47 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Log, 4),
			new IngredientRequirement(ObjectType.Plank, 8),
			new IngredientRequirement(ObjectType.Pole, 6)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerWorkbenchMk1, ingredients47);
		IngredientRequirement[] ingredients48 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Log, 4),
			new IngredientRequirement(ObjectType.Panel, 8),
			new IngredientRequirement(ObjectType.FrameSquare, 6)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerWorkbenchMk2, ingredients48);
		IngredientRequirement[] ingredients49 = new IngredientRequirement[5]
		{
			new IngredientRequirement(ObjectType.MetalGirder, 1),
			new IngredientRequirement(ObjectType.FrameBox, 2),
			new IngredientRequirement(ObjectType.MetalPlateCrude, 4),
			new IngredientRequirement(ObjectType.MetalPoleCrude, 4),
			new IngredientRequirement(ObjectType.Rivets, 8)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerWorkbenchMk3, ingredients49);
		IngredientRequirement[] ingredients50 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.Rock, 2),
			new IngredientRequirement(ObjectType.Pole, 2),
			new IngredientRequirement(ObjectType.FrameSquare, 2),
			new IngredientRequirement(ObjectType.Panel, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ClayStationCrude, ingredients50);
		IngredientRequirement[] ingredients51 = new IngredientRequirement[5]
		{
			new IngredientRequirement(ObjectType.StoneBlock, 1),
			new IngredientRequirement(ObjectType.Panel, 2),
			new IngredientRequirement(ObjectType.Pole, 4),
			new IngredientRequirement(ObjectType.Cog, 1),
			new IngredientRequirement(ObjectType.Crank, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ClayStation, ingredients51);
		IngredientRequirement[] ingredients52 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Plank, 4),
			new IngredientRequirement(ObjectType.Pole, 4),
			new IngredientRequirement(ObjectType.CogCrude, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.FolkSeedPod, ingredients52);
		IngredientRequirement[] ingredients53 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Plank, 4),
			new IngredientRequirement(ObjectType.Pole, 4),
			new IngredientRequirement(ObjectType.CogCrude, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.FolkSeedRehydrator, ingredients53);
		IngredientRequirement[] ingredients54 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Pole, 2),
			new IngredientRequirement(ObjectType.Plank, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.StringWinderCrude, ingredients54);
		IngredientRequirement[] ingredients55 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Log, 2),
			new IngredientRequirement(ObjectType.FrameSquare, 4),
			new IngredientRequirement(ObjectType.Panel, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.VehicleAssembler, ingredients55);
		IngredientRequirement[] ingredients56 = new IngredientRequirement[6]
		{
			new IngredientRequirement(ObjectType.MetalGirder, 4),
			new IngredientRequirement(ObjectType.WoodenBeam, 4),
			new IngredientRequirement(ObjectType.BricksCrude, 20),
			new IngredientRequirement(ObjectType.Mortar, 10),
			new IngredientRequirement(ObjectType.Panel, 4),
			new IngredientRequirement(ObjectType.RoofTiles, 8)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.VehicleAssemblerGood, ingredients56);
		IngredientRequirement[] ingredients57 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.FrameSquare, 4),
			new IngredientRequirement(ObjectType.Panel, 8),
			new IngredientRequirement(ObjectType.Plank, 8),
			new IngredientRequirement(ObjectType.FixingPeg, 8)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.MilkingShedCrude, ingredients57);
		IngredientRequirement[] ingredients58 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.FrameSquare, 4),
			new IngredientRequirement(ObjectType.Panel, 8),
			new IngredientRequirement(ObjectType.Plank, 8),
			new IngredientRequirement(ObjectType.FixingPeg, 8)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ShearingShedCrude, ingredients58);
		IngredientRequirement[] ingredients59 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Plank, 4),
			new IngredientRequirement(ObjectType.FrameBox, 1),
			new IngredientRequirement(ObjectType.Pole, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.SilkwormStation, ingredients59);
		IngredientRequirement[] ingredients60 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.Log, 4),
			new IngredientRequirement(ObjectType.Plank, 4),
			new IngredientRequirement(ObjectType.Pole, 4),
			new IngredientRequirement(ObjectType.Rock, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkbenchMk2, ingredients60);
		IngredientRequirement[] ingredients61 = new IngredientRequirement[5]
		{
			new IngredientRequirement(ObjectType.Log, 4),
			new IngredientRequirement(ObjectType.Plank, 4),
			new IngredientRequirement(ObjectType.Pole, 4),
			new IngredientRequirement(ObjectType.Rock, 2),
			new IngredientRequirement(ObjectType.FixingPeg, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkbenchStructural, ingredients61);
		IngredientRequirement[] ingredients62 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Plank, 6),
			new IngredientRequirement(ObjectType.Pole, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.PotCrude, ingredients62);
		IngredientRequirement[] ingredients63 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.FrameSquare, 2),
			new IngredientRequirement(ObjectType.StoneBlockCrude, 2),
			new IngredientRequirement(ObjectType.ToolChiselCrude, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.MasonryBench, ingredients63);
		IngredientRequirement[] ingredients64 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.FrameBox, 2),
			new IngredientRequirement(ObjectType.FrameSquare, 2),
			new IngredientRequirement(ObjectType.Panel, 2),
			new IngredientRequirement(ObjectType.Log, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.LoomCrude, ingredients64);
		IngredientRequirement[] ingredients65 = new IngredientRequirement[6]
		{
			new IngredientRequirement(ObjectType.WoodenBeam, 4),
			new IngredientRequirement(ObjectType.MetalPlateCrude, 4),
			new IngredientRequirement(ObjectType.MetalPoleCrude, 2),
			new IngredientRequirement(ObjectType.MetalAxle, 1),
			new IngredientRequirement(ObjectType.MetalCog, 2),
			new IngredientRequirement(ObjectType.Rivets, 8)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.LoomGood, ingredients65);
		IngredientRequirement[] ingredients66 = new IngredientRequirement[5]
		{
			new IngredientRequirement(ObjectType.FrameBox, 1),
			new IngredientRequirement(ObjectType.Plank, 6),
			new IngredientRequirement(ObjectType.Cog, 1),
			new IngredientRequirement(ObjectType.Crank, 1),
			new IngredientRequirement(ObjectType.StoneBlock, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.HayBalerCrude, ingredients66);
		IngredientRequirement[] ingredients67 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.FrameSquare, 4),
			new IngredientRequirement(ObjectType.Panel, 4),
			new IngredientRequirement(ObjectType.Plank, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ToyStationCrude, ingredients67);
		IngredientRequirement[] ingredients68 = new IngredientRequirement[5]
		{
			new IngredientRequirement(ObjectType.WoodenBeam, 4),
			new IngredientRequirement(ObjectType.FrameSquare, 4),
			new IngredientRequirement(ObjectType.Panel, 4),
			new IngredientRequirement(ObjectType.StoneBlock, 1),
			new IngredientRequirement(ObjectType.Cog, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.MedicineStation, ingredients68);
		IngredientRequirement[] ingredients69 = new IngredientRequirement[6]
		{
			new IngredientRequirement(ObjectType.WoodenBeam, 4),
			new IngredientRequirement(ObjectType.FrameBox, 1),
			new IngredientRequirement(ObjectType.Panel, 4),
			new IngredientRequirement(ObjectType.MetalPlateCrude, 1),
			new IngredientRequirement(ObjectType.MetalAxle, 1),
			new IngredientRequirement(ObjectType.MetalCog, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.PrintingPress, ingredients69);
		IngredientRequirement[] ingredients70 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.MetalPlateCrude, 6),
			new IngredientRequirement(ObjectType.MetalCog, 2),
			new IngredientRequirement(ObjectType.MetalPoleCrude, 2),
			new IngredientRequirement(ObjectType.Rivets, 6)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.PaperMill, ingredients70);
		IngredientRequirement[] ingredients71 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.BricksCrude, 6),
			new IngredientRequirement(ObjectType.Mortar, 4),
			new IngredientRequirement(ObjectType.FrameSquare, 1),
			new IngredientRequirement(ObjectType.BullrushesCloth, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.PaperMill, ingredients71);
		IngredientRequirement[] ingredients72 = new IngredientRequirement[5]
		{
			new IngredientRequirement(ObjectType.WoodenBeam, 2),
			new IngredientRequirement(ObjectType.Cog, 2),
			new IngredientRequirement(ObjectType.Axle, 1),
			new IngredientRequirement(ObjectType.FrameBox, 2),
			new IngredientRequirement(ObjectType.Panel, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.PaperMill, ingredients72);
		IngredientRequirement[] ingredients73 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.FrameTriangle, 3),
			new IngredientRequirement(ObjectType.Pole, 3),
			new IngredientRequirement(ObjectType.Plank, 1),
			new IngredientRequirement(ObjectType.FixingPeg, 6)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Easel, ingredients73);
		IngredientRequirement[] ingredients74 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.Log, 2),
			new IngredientRequirement(ObjectType.Plank, 8),
			new IngredientRequirement(ObjectType.Pole, 8),
			new IngredientRequirement(ObjectType.Rock, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ResearchStationCrude, ingredients74);
		IngredientRequirement[] ingredients75 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Log, 1),
			new IngredientRequirement(ObjectType.Pole, 3),
			new IngredientRequirement(ObjectType.Rock, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ResearchStationCrude2, ingredients75);
		IngredientRequirement[] ingredients76 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.Log, 1),
			new IngredientRequirement(ObjectType.Pole, 3),
			new IngredientRequirement(ObjectType.CogCrude, 1),
			new IngredientRequirement(ObjectType.StoneBlockCrude, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ResearchStationCrude3, ingredients76);
		IngredientRequirement[] ingredients77 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Stick, 8),
			new IngredientRequirement(ObjectType.Rock, 4),
			new IngredientRequirement(ObjectType.StoneBlockCrude, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ResearchStationCrude4, ingredients77);
		IngredientRequirement[] ingredients78 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.StoneBlock, 2),
			new IngredientRequirement(ObjectType.Water, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ResearchStationCrude5, ingredients78);
		IngredientRequirement[] ingredients79 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.StoneBlock, 1),
			new IngredientRequirement(ObjectType.Cog, 2),
			new IngredientRequirement(ObjectType.RockSharp, 3)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ResearchStationCrude6, ingredients79);
		IngredientRequirement[] ingredients80 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Log, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Hut, ingredients80);
		IngredientRequirement[] ingredients81 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Straw, 6)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.LogCabin, ingredients81);
		IngredientRequirement[] ingredients82 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Log, 6)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.LogCabin, ingredients82);
		IngredientRequirement[] ingredients83 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Plank, 6)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.LogCabin, ingredients83);
		IngredientRequirement[] ingredients84 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Plank, 8)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.StoneCottage, ingredients84);
		IngredientRequirement[] ingredients85 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Chimney, 1),
			new IngredientRequirement(ObjectType.Fireplace, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.StoneCottage, ingredients85);
		IngredientRequirement[] ingredients86 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Rock, 10)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.StoneCottage, ingredients86);
		IngredientRequirement[] ingredients87 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Log, 3),
			new IngredientRequirement(ObjectType.Rock, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.StoneCottage, ingredients87);
		IngredientRequirement[] ingredients88 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Straw, 8)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.StoneCottage, ingredients88);
		IngredientRequirement[] ingredients89 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.FrameDoor, 1),
			new IngredientRequirement(ObjectType.FrameWindow, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.StoneCottage, ingredients89);
		IngredientRequirement[] ingredients90 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Rock, 10),
			new IngredientRequirement(ObjectType.StoneBlockCrude, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.BrickHut, ingredients90);
		IngredientRequirement[] ingredients91 = new IngredientRequirement[5]
		{
			new IngredientRequirement(ObjectType.WoodenBeam, 4),
			new IngredientRequirement(ObjectType.Plank, 8),
			new IngredientRequirement(ObjectType.Fireplace, 1),
			new IngredientRequirement(ObjectType.BricksCrude, 6),
			new IngredientRequirement(ObjectType.Mortar, 3)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.BrickHut, ingredients91);
		IngredientRequirement[] ingredients92 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.WoodenBeam, 2),
			new IngredientRequirement(ObjectType.Plank, 8),
			new IngredientRequirement(ObjectType.BricksCrude, 16),
			new IngredientRequirement(ObjectType.Mortar, 8)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.BrickHut, ingredients92);
		IngredientRequirement[] ingredients93 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.WoodenBeam, 6),
			new IngredientRequirement(ObjectType.FixingPeg, 8),
			new IngredientRequirement(ObjectType.Plank, 16)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.BrickHut, ingredients93);
		IngredientRequirement[] ingredients94 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.WoodenBeam, 4),
			new IngredientRequirement(ObjectType.BricksCrude, 32),
			new IngredientRequirement(ObjectType.Chimney, 2),
			new IngredientRequirement(ObjectType.Mortar, 16)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.BrickHut, ingredients94);
		IngredientRequirement[] ingredients95 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Plank, 8),
			new IngredientRequirement(ObjectType.RoofTiles, 20)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.BrickHut, ingredients95);
		IngredientRequirement[] ingredients96 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.FrameWindow, 6),
			new IngredientRequirement(ObjectType.FrameDoor, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.BrickHut, ingredients96);
		IngredientRequirement[] ingredients97 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Rock, 20),
			new IngredientRequirement(ObjectType.StoneBlockCrude, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Mansion, ingredients97);
		IngredientRequirement[] ingredients98 = new IngredientRequirement[6]
		{
			new IngredientRequirement(ObjectType.WoodenBeam, 4),
			new IngredientRequirement(ObjectType.Plank, 12),
			new IngredientRequirement(ObjectType.Fireplace, 2),
			new IngredientRequirement(ObjectType.Chimney, 2),
			new IngredientRequirement(ObjectType.BricksCrude, 8),
			new IngredientRequirement(ObjectType.Mortar, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Mansion, ingredients98);
		IngredientRequirement[] ingredients99 = new IngredientRequirement[6]
		{
			new IngredientRequirement(ObjectType.BricksCrude, 16),
			new IngredientRequirement(ObjectType.Mortar, 8),
			new IngredientRequirement(ObjectType.WoodenBeam, 2),
			new IngredientRequirement(ObjectType.MetalGirder, 1),
			new IngredientRequirement(ObjectType.FixingPeg, 8),
			new IngredientRequirement(ObjectType.Plank, 12)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Mansion, ingredients99);
		IngredientRequirement[] ingredients100 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.WoodenBeam, 6),
			new IngredientRequirement(ObjectType.FixingPeg, 8),
			new IngredientRequirement(ObjectType.Plank, 16)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Mansion, ingredients100);
		IngredientRequirement[] ingredients101 = new IngredientRequirement[5]
		{
			new IngredientRequirement(ObjectType.BricksCrude, 16),
			new IngredientRequirement(ObjectType.Mortar, 8),
			new IngredientRequirement(ObjectType.WoodenBeam, 2),
			new IngredientRequirement(ObjectType.FixingPeg, 8),
			new IngredientRequirement(ObjectType.Plank, 12)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Mansion, ingredients101);
		IngredientRequirement[] ingredients102 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.WoodenBeam, 6),
			new IngredientRequirement(ObjectType.Chimney, 2),
			new IngredientRequirement(ObjectType.BricksCrude, 8),
			new IngredientRequirement(ObjectType.Mortar, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Mansion, ingredients102);
		IngredientRequirement[] ingredients103 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.FrameBox, 2),
			new IngredientRequirement(ObjectType.FrameTriangle, 4),
			new IngredientRequirement(ObjectType.Plank, 12),
			new IngredientRequirement(ObjectType.RoofTiles, 24)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Mansion, ingredients103);
		IngredientRequirement[] ingredients104 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.StoneBlock, 4),
			new IngredientRequirement(ObjectType.MetalGirder, 2),
			new IngredientRequirement(ObjectType.FrameWindow, 8),
			new IngredientRequirement(ObjectType.FrameDoor, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Mansion, ingredients104);
		IngredientRequirement[] ingredients105 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Rock, 20),
			new IngredientRequirement(ObjectType.StoneBlockCrude, 10)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Castle, ingredients105);
		IngredientRequirement[] ingredients106 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.StoneBlock, 10),
			new IngredientRequirement(ObjectType.Mortar, 20)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Castle, ingredients106);
		IngredientRequirement[] ingredients107 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.StoneBlock, 10),
			new IngredientRequirement(ObjectType.Rock, 20),
			new IngredientRequirement(ObjectType.Mortar, 20)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Castle, ingredients107);
		IngredientRequirement[] ingredients108 = new IngredientRequirement[6]
		{
			new IngredientRequirement(ObjectType.StoneBlock, 10),
			new IngredientRequirement(ObjectType.MetalGirder, 2),
			new IngredientRequirement(ObjectType.WoodenBeam, 8),
			new IngredientRequirement(ObjectType.Plank, 30),
			new IngredientRequirement(ObjectType.FixingPeg, 10),
			new IngredientRequirement(ObjectType.Mortar, 20)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Castle, ingredients108);
		IngredientRequirement[] ingredients109 = new IngredientRequirement[6]
		{
			new IngredientRequirement(ObjectType.StoneBlock, 10),
			new IngredientRequirement(ObjectType.MetalGirder, 2),
			new IngredientRequirement(ObjectType.WoodenBeam, 2),
			new IngredientRequirement(ObjectType.Plank, 8),
			new IngredientRequirement(ObjectType.FixingPeg, 10),
			new IngredientRequirement(ObjectType.Mortar, 20)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Castle, ingredients109);
		IngredientRequirement[] ingredients110 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.FrameBox, 5),
			new IngredientRequirement(ObjectType.FrameTriangle, 6),
			new IngredientRequirement(ObjectType.WoodenBeam, 6),
			new IngredientRequirement(ObjectType.RoofTiles, 24)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Castle, ingredients110);
		IngredientRequirement[] ingredients111 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.Rock, 20),
			new IngredientRequirement(ObjectType.MetalPlateCrude, 4),
			new IngredientRequirement(ObjectType.MetalPoleCrude, 20),
			new IngredientRequirement(ObjectType.Plank, 10)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Castle, ingredients111);
		IngredientRequirement[] ingredients112 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Plank, 2),
			new IngredientRequirement(ObjectType.Log, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.FencePost, ingredients112);
		IngredientRequirement[] ingredients113 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Log, 1),
			new IngredientRequirement(ObjectType.Plank, 4),
			new IngredientRequirement(ObjectType.Pole, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Gate, ingredients113);
		IngredientRequirement[] ingredients114 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Plank, 4),
			new IngredientRequirement(ObjectType.Pole, 1),
			new IngredientRequirement(ObjectType.FixingPeg, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.FencePicket, ingredients114);
		IngredientRequirement[] ingredients115 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Plank, 8),
			new IngredientRequirement(ObjectType.Pole, 1),
			new IngredientRequirement(ObjectType.FixingPeg, 8)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.GatePicket, ingredients115);
		IngredientRequirement[] ingredients116 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Rock, 5)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.StoneWall, ingredients116);
		IngredientRequirement[] ingredients117 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.BricksCrude, 3),
			new IngredientRequirement(ObjectType.Mortar, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.BrickWall, ingredients117);
		IngredientRequirement[] ingredients118 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Log, 3),
			new IngredientRequirement(ObjectType.FixingPeg, 3)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.LogWall, ingredients118);
		IngredientRequirement[] ingredients119 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Log, 3),
			new IngredientRequirement(ObjectType.FixingPeg, 3)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.LogArch, ingredients119);
		IngredientRequirement[] ingredients120 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.StoneBlock, 2),
			new IngredientRequirement(ObjectType.Mortar, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.BlockWall, ingredients120);
		IngredientRequirement[] ingredients121 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Sand, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.SandPath, ingredients121);
		IngredientRequirement[] ingredients122 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Rock, 5)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.StonePath, ingredients122);
		IngredientRequirement[] ingredients123 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Rock, 8),
			new IngredientRequirement(ObjectType.Sand, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.RoadCrude, ingredients123);
		IngredientRequirement[] ingredients124 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Straw, 3)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.FlooringCrude, ingredients124);
		IngredientRequirement[] ingredients125 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Log, 1),
			new IngredientRequirement(ObjectType.Plank, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Workshop, ingredients125);
		IngredientRequirement[] ingredients126 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.BricksCrude, 4),
			new IngredientRequirement(ObjectType.Mortar, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.RoadGood, ingredients126);
		IngredientRequirement[] ingredients127 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.BricksCrude, 4),
			new IngredientRequirement(ObjectType.Mortar, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.FlooringBrick, ingredients127);
		IngredientRequirement[] ingredients128 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.StoneBlock, 1),
			new IngredientRequirement(ObjectType.Mortar, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.FlooringFlagstone, ingredients128);
		IngredientRequirement[] ingredients129 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Plank, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.FlooringParquet, ingredients129);
		IngredientRequirement[] ingredients130 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.WoodenBeam, 1),
			new IngredientRequirement(ObjectType.MetalPoleCrude, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TrainTrack, ingredients130);
		IngredientRequirement[] ingredients131 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.WoodenBeam, 4),
			new IngredientRequirement(ObjectType.MetalPoleCrude, 8)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TrainTrackCurve, ingredients131);
		IngredientRequirement[] ingredients132 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.WoodenBeam, 4),
			new IngredientRequirement(ObjectType.MetalPoleCrude, 8)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TrainTrackPointsLeft, ingredients132);
		ObjectTypeList.Instance.SetIngredients(ObjectType.TrainTrackPointsRight, ingredients132);
		IngredientRequirement[] ingredients133 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.WoodenBeam, 3),
			new IngredientRequirement(ObjectType.FrameSquare, 3),
			new IngredientRequirement(ObjectType.Panel, 3),
			new IngredientRequirement(ObjectType.MetalPoleCrude, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TrainTrackBridge, ingredients133);
		IngredientRequirement[] ingredients134 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Log, 2),
			new IngredientRequirement(ObjectType.FrameSquare, 3),
			new IngredientRequirement(ObjectType.Panel, 3)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Bridge, ingredients134);
		IngredientRequirement[] ingredients135 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Rock, 7)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.StoneArch, ingredients135);
		IngredientRequirement[] ingredients136 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Rock, 7),
			new IngredientRequirement(ObjectType.Plank, 4),
			new IngredientRequirement(ObjectType.FrameDoor, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.StoneArchDoor, ingredients136);
		IngredientRequirement[] ingredients137 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.StoneBlock, 4),
			new IngredientRequirement(ObjectType.Plank, 8),
			new IngredientRequirement(ObjectType.FrameDoor, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.BlockDoor, ingredients137);
		IngredientRequirement[] ingredients138 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Plank, 4),
			new IngredientRequirement(ObjectType.Pole, 1),
			new IngredientRequirement(ObjectType.FrameDoor, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Door, ingredients138);
		IngredientRequirement[] ingredients139 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Plank, 4),
			new IngredientRequirement(ObjectType.Pole, 1),
			new IngredientRequirement(ObjectType.FrameWindow, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Window, ingredients139);
		IngredientRequirement[] ingredients140 = new IngredientRequirement[6]
		{
			new IngredientRequirement(ObjectType.WoodenBeam, 4),
			new IngredientRequirement(ObjectType.Flywheel, 1),
			new IngredientRequirement(ObjectType.ConnectingRod, 1),
			new IngredientRequirement(ObjectType.Piston, 1),
			new IngredientRequirement(ObjectType.Firebox, 1),
			new IngredientRequirement(ObjectType.Boiler, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.StationaryEngine, ingredients140);
		IngredientRequirement[] ingredients141 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Wheel, 2),
			new IngredientRequirement(ObjectType.StringBall, 5),
			new IngredientRequirement(ObjectType.Pole, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.BeltLinkage, ingredients141);
		IngredientRequirement[] ingredients142 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.FrameSquare, 2),
			new IngredientRequirement(ObjectType.Plank, 6)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Trough, ingredients142);
		IngredientRequirement[] ingredients143 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.StoneBlock, 4),
			new IngredientRequirement(ObjectType.FrameWindow, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WindowStone, ingredients143);
		IngredientRequirement[] ingredients144 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Rock, 10),
			new IngredientRequirement(ObjectType.Mortar, 5)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.BridgeStone, ingredients144);
		IngredientRequirement[] ingredients145 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.FrameSquare, 1),
			new IngredientRequirement(ObjectType.Panel, 1),
			new IngredientRequirement(ObjectType.Pole, 1),
			new IngredientRequirement(ObjectType.Plank, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TrainTrackStop, ingredients145);
		IngredientRequirement[] ingredients146 = new IngredientRequirement[6]
		{
			new IngredientRequirement(ObjectType.BricksCrude, 8),
			new IngredientRequirement(ObjectType.Mortar, 2),
			new IngredientRequirement(ObjectType.FrameTriangle, 6),
			new IngredientRequirement(ObjectType.Plank, 8),
			new IngredientRequirement(ObjectType.WoodenBeam, 1),
			new IngredientRequirement(ObjectType.RoofTiles, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TrainRefuellingStation, ingredients146);
		IngredientRequirement[] ingredients147 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Log, 4),
			new IngredientRequirement(ObjectType.Pole, 4),
			new IngredientRequirement(ObjectType.Plank, 8)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Wardrobe, ingredients147);
		IngredientRequirement[] ingredients148 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Rock, 5),
			new IngredientRequirement(ObjectType.Sand, 5)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.StoneHeads, ingredients148);
		IngredientRequirement[] ingredients149 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Rock, 10),
			new IngredientRequirement(ObjectType.Sand, 10)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.StoneHeads, ingredients149);
		IngredientRequirement[] ingredients150 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Rock, 6),
			new IngredientRequirement(ObjectType.Sand, 3)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.StoneHeads, ingredients150);
		IngredientRequirement[] ingredients151 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.StoneBlock, 10),
			new IngredientRequirement(ObjectType.Mortar, 5)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Ziggurat, ingredients151);
		IngredientRequirement[] ingredients152 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.BricksCrude, 20),
			new IngredientRequirement(ObjectType.Mortar, 10)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Ziggurat, ingredients152);
		IngredientRequirement[] ingredients153 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.BricksCrude, 10),
			new IngredientRequirement(ObjectType.StoneBlock, 5),
			new IngredientRequirement(ObjectType.Mortar, 10)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Ziggurat, ingredients153);
		IngredientRequirement[] ingredients154 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.BricksCrude, 20),
			new IngredientRequirement(ObjectType.Mortar, 10)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Ziggurat, ingredients154);
		IngredientRequirement[] ingredients155 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.BricksCrude, 20),
			new IngredientRequirement(ObjectType.Mortar, 10)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Ziggurat, ingredients155);
		IngredientRequirement[] ingredients156 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.FrameTriangle, 4),
			new IngredientRequirement(ObjectType.Log, 6),
			new IngredientRequirement(ObjectType.FixingPeg, 20)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.GiantWaterWheel, ingredients156);
		IngredientRequirement[] ingredients157 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.FrameTriangle, 12),
			new IngredientRequirement(ObjectType.Log, 18),
			new IngredientRequirement(ObjectType.FixingPeg, 40)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.GiantWaterWheel, ingredients157);
		IngredientRequirement[] ingredients158 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Panel, 24),
			new IngredientRequirement(ObjectType.FixingPeg, 48)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.GiantWaterWheel, ingredients158);
		IngredientRequirement[] ingredients159 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Sand, 10),
			new IngredientRequirement(ObjectType.StoneBlock, 5)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.SpacePort, ingredients159);
		IngredientRequirement[] ingredients160 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.FrameSquare, 2),
			new IngredientRequirement(ObjectType.Panel, 5)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.SpacePort, ingredients160);
		IngredientRequirement[] ingredients161 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Sand, 8),
			new IngredientRequirement(ObjectType.StoneBlockCrude, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.StoneHenge, ingredients161);
		IngredientRequirement[] ingredients162 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Soil, 8),
			new IngredientRequirement(ObjectType.Sand, 12),
			new IngredientRequirement(ObjectType.StoneBlockCrude, 6)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.StoneHenge, ingredients162);
		IngredientRequirement[] ingredients163 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.Clay, 8),
			new IngredientRequirement(ObjectType.Soil, 12),
			new IngredientRequirement(ObjectType.Sand, 16),
			new IngredientRequirement(ObjectType.StoneBlockCrude, 8)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.StoneHenge, ingredients163);
		IngredientRequirement[] ingredients164 = new IngredientRequirement[5]
		{
			new IngredientRequirement(ObjectType.Rock, 8),
			new IngredientRequirement(ObjectType.Clay, 12),
			new IngredientRequirement(ObjectType.Soil, 16),
			new IngredientRequirement(ObjectType.Sand, 20),
			new IngredientRequirement(ObjectType.StoneBlockCrude, 10)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.StoneHenge, ingredients164);
		IngredientRequirement[] ingredients165 = new IngredientRequirement[6]
		{
			new IngredientRequirement(ObjectType.IronOre, 8),
			new IngredientRequirement(ObjectType.Rock, 12),
			new IngredientRequirement(ObjectType.Clay, 16),
			new IngredientRequirement(ObjectType.Soil, 20),
			new IngredientRequirement(ObjectType.Sand, 24),
			new IngredientRequirement(ObjectType.StoneBlockCrude, 12)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.StoneHenge, ingredients165);
		IngredientRequirement[] ingredients166 = new IngredientRequirement[5]
		{
			new IngredientRequirement(ObjectType.FrameSquare, 2),
			new IngredientRequirement(ObjectType.Panel, 4),
			new IngredientRequirement(ObjectType.Millstone, 1),
			new IngredientRequirement(ObjectType.FrameTriangle, 4),
			new IngredientRequirement(ObjectType.Log, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Catapult, ingredients166);
		IngredientRequirement[] ingredients167 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.Cog, 4),
			new IngredientRequirement(ObjectType.Axle, 1),
			new IngredientRequirement(ObjectType.Crank, 1),
			new IngredientRequirement(ObjectType.StringBall, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Catapult, ingredients167);
		IngredientRequirement[] ingredients168 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Log, 2),
			new IngredientRequirement(ObjectType.StoneBlock, 1),
			new IngredientRequirement(ObjectType.Plank, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Catapult, ingredients168);
		IngredientRequirement[] ingredients169 = new IngredientRequirement[5]
		{
			new IngredientRequirement(ObjectType.FrameSquare, 4),
			new IngredientRequirement(ObjectType.Panel, 6),
			new IngredientRequirement(ObjectType.Axle, 4),
			new IngredientRequirement(ObjectType.TreeSeed, 16),
			new IngredientRequirement(ObjectType.Cog, 8)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.BotServer, ingredients169);
		IngredientRequirement[] ingredients170 = new IngredientRequirement[5]
		{
			new IngredientRequirement(ObjectType.FrameSquare, 1),
			new IngredientRequirement(ObjectType.Panel, 3),
			new IngredientRequirement(ObjectType.Wheel, 2),
			new IngredientRequirement(ObjectType.Axle, 1),
			new IngredientRequirement(ObjectType.Cog, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.BotServer, ingredients170);
		IngredientRequirement[] ingredients171 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.FrameSquare, 2),
			new IngredientRequirement(ObjectType.Panel, 3),
			new IngredientRequirement(ObjectType.Axle, 2),
			new IngredientRequirement(ObjectType.Cog, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.BotServer, ingredients171);
		IngredientRequirement[] ingredients172 = new IngredientRequirement[5]
		{
			new IngredientRequirement(ObjectType.MetalGirder, 2),
			new IngredientRequirement(ObjectType.Rivets, 2),
			new IngredientRequirement(ObjectType.MetalCog, 2),
			new IngredientRequirement(ObjectType.MetalAxle, 2),
			new IngredientRequirement(ObjectType.StoneBlock, 8)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TranscendBuilding, ingredients172);
		IngredientRequirement[] ingredients173 = new IngredientRequirement[5]
		{
			new IngredientRequirement(ObjectType.MetalGirder, 1),
			new IngredientRequirement(ObjectType.Rivets, 2),
			new IngredientRequirement(ObjectType.MetalCog, 2),
			new IngredientRequirement(ObjectType.MetalAxle, 2),
			new IngredientRequirement(ObjectType.StoneBlock, 6)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TranscendBuilding, ingredients173);
		IngredientRequirement[] ingredients174 = new IngredientRequirement[5]
		{
			new IngredientRequirement(ObjectType.MetalGirder, 1),
			new IngredientRequirement(ObjectType.Rivets, 2),
			new IngredientRequirement(ObjectType.MetalCog, 1),
			new IngredientRequirement(ObjectType.MetalAxle, 1),
			new IngredientRequirement(ObjectType.StoneBlock, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TranscendBuilding, ingredients174);
		IngredientRequirement[] ingredients175 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.MetalCog, 1),
			new IngredientRequirement(ObjectType.MetalAxle, 1),
			new IngredientRequirement(ObjectType.StoneBlock, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TranscendBuilding, ingredients175);
	}
}
