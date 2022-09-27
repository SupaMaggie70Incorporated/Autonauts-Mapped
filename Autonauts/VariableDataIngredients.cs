public class VariableDataIngredients
{
	public void Init()
	{
		IngredientRequirement[] ingredients = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.Log, 1),
			new IngredientRequirement(ObjectType.Plank, 3),
			new IngredientRequirement(ObjectType.Pole, 1),
			new IngredientRequirement(ObjectType.TreeSeed, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.BasicWorker, ingredients);
		IngredientRequirement[] ingredients2 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.WorkerHeadAny, 1),
			new IngredientRequirement(ObjectType.WorkerFrameAny, 1),
			new IngredientRequirement(ObjectType.WorkerDriveAny, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Worker, ingredients2);
		IngredientRequirement[] ingredients3 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.FolkSeed, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Folk, ingredients3);
		IngredientRequirement[] ingredients4 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Rock, 1),
			new IngredientRequirement(ObjectType.Stick, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ToolShovelStone, ingredients4);
		IngredientRequirement[] ingredients5 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.RockSharp, 1),
			new IngredientRequirement(ObjectType.Stick, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ToolHoeStone, ingredients5);
		IngredientRequirement[] ingredients6 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Rock, 1),
			new IngredientRequirement(ObjectType.Stick, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ToolAxeStone, ingredients6);
		IngredientRequirement[] ingredients7 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.RockSharp, 1),
			new IngredientRequirement(ObjectType.Stick, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ToolScytheStone, ingredients7);
		IngredientRequirement[] ingredients8 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Rock, 1),
			new IngredientRequirement(ObjectType.Stick, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ToolPickStone, ingredients8);
		IngredientRequirement[] ingredients9 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Plank, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ToolBucketCrude, ingredients9);
		IngredientRequirement[] ingredients10 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Stick, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ToolFlailCrude, ingredients10);
		IngredientRequirement[] ingredients11 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.RockSharp, 1),
			new IngredientRequirement(ObjectType.Pole, 1),
			new IngredientRequirement(ObjectType.ToolMallet, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ToolChiselCrude, ingredients11);
		IngredientRequirement[] ingredients12 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.IronCrude, 1),
			new IngredientRequirement(ObjectType.Pole, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ToolShovel, ingredients12);
		IngredientRequirement[] ingredients13 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.IronCrude, 1),
			new IngredientRequirement(ObjectType.Pole, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ToolHoe, ingredients13);
		IngredientRequirement[] ingredients14 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.IronCrude, 1),
			new IngredientRequirement(ObjectType.Pole, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ToolAxe, ingredients14);
		IngredientRequirement[] ingredients15 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.IronCrude, 1),
			new IngredientRequirement(ObjectType.Pole, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ToolScythe, ingredients15);
		IngredientRequirement[] ingredients16 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.IronCrude, 1),
			new IngredientRequirement(ObjectType.Pole, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ToolPick, ingredients16);
		IngredientRequirement[] ingredients17 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Plank, 6),
			new IngredientRequirement(ObjectType.Pole, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ToolBucket, ingredients17);
		IngredientRequirement[] ingredients18 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Pole, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ToolFlail, ingredients18);
		IngredientRequirement[] ingredients19 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.IronCrude, 1),
			new IngredientRequirement(ObjectType.Pole, 1),
			new IngredientRequirement(ObjectType.ToolMallet, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ToolChisel, ingredients19);
		IngredientRequirement[] ingredients20 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.MetalPlateCrude, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ToolBucketMetal, ingredients20);
		IngredientRequirement[] ingredients21 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Log, 1),
			new IngredientRequirement(ObjectType.Pole, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ToolMallet, ingredients21);
		IngredientRequirement[] ingredients22 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.RockSharp, 2),
			new IngredientRequirement(ObjectType.Pole, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ToolShears, ingredients22);
		IngredientRequirement[] ingredients23 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Plank, 4),
			new IngredientRequirement(ObjectType.FixingPeg, 8),
			new IngredientRequirement(ObjectType.Pole, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ToolWateringCan, ingredients23);
		IngredientRequirement[] ingredients24 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Pole, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ToolFishingStick, ingredients24);
		IngredientRequirement[] ingredients25 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Pole, 2),
			new IngredientRequirement(ObjectType.StringBall, 3)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ToolFishingRod, ingredients25);
		IngredientRequirement[] ingredients26 = new IngredientRequirement[5]
		{
			new IngredientRequirement(ObjectType.Pole, 4),
			new IngredientRequirement(ObjectType.Axle, 1),
			new IngredientRequirement(ObjectType.Crank, 1),
			new IngredientRequirement(ObjectType.Cog, 1),
			new IngredientRequirement(ObjectType.StringBall, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ToolFishingRodGood, ingredients26);
		IngredientRequirement[] ingredients27 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Pole, 1),
			new IngredientRequirement(ObjectType.Stick, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ToolBroom, ingredients27);
		IngredientRequirement[] ingredients28 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Pole, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ToolPitchfork, ingredients28);
		IngredientRequirement[] ingredients29 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Stick, 1),
			new IngredientRequirement(ObjectType.Straw, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ToolTorchCrude, ingredients29);
		IngredientRequirement[] ingredients30 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.ToolShovelStone, 1),
			new IngredientRequirement(ObjectType.ToolPickStone, 1),
			new IngredientRequirement(ObjectType.RockSharp, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ToolDredgerCrude, ingredients30);
		IngredientRequirement[] ingredients31 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Pole, 1),
			new IngredientRequirement(ObjectType.Plank, 5)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ToolNetCrude, ingredients31);
		IngredientRequirement[] ingredients32 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Pole, 1),
			new IngredientRequirement(ObjectType.FrameSquare, 1),
			new IngredientRequirement(ObjectType.StringBall, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ToolNet, ingredients32);
		IngredientRequirement[] ingredients33 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.MetalPlateCrude, 1),
			new IngredientRequirement(ObjectType.Pole, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ToolBlade, ingredients33);
		IngredientRequirement[] ingredients34 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Log, 1),
			new IngredientRequirement(ObjectType.RockSharp, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Canoe, ingredients34);
		IngredientRequirement[] ingredients35 = new IngredientRequirement[5]
		{
			new IngredientRequirement(ObjectType.FrameBox, 1),
			new IngredientRequirement(ObjectType.Panel, 4),
			new IngredientRequirement(ObjectType.MetalPlateCrude, 1),
			new IngredientRequirement(ObjectType.MetalWheel, 2),
			new IngredientRequirement(ObjectType.MetalAxle, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Carriage, ingredients35);
		IngredientRequirement[] ingredients36 = new IngredientRequirement[6]
		{
			new IngredientRequirement(ObjectType.FrameTriangle, 2),
			new IngredientRequirement(ObjectType.Panel, 2),
			new IngredientRequirement(ObjectType.MetalPlateCrude, 2),
			new IngredientRequirement(ObjectType.Rivets, 1),
			new IngredientRequirement(ObjectType.MetalWheel, 2),
			new IngredientRequirement(ObjectType.MetalAxle, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.CarriageLiquid, ingredients36);
		IngredientRequirement[] ingredients37 = new IngredientRequirement[6]
		{
			new IngredientRequirement(ObjectType.FrameBox, 2),
			new IngredientRequirement(ObjectType.Panel, 4),
			new IngredientRequirement(ObjectType.MetalPlateCrude, 2),
			new IngredientRequirement(ObjectType.Rivets, 1),
			new IngredientRequirement(ObjectType.MetalWheel, 4),
			new IngredientRequirement(ObjectType.MetalAxle, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.CarriageTrain, ingredients37);
		IngredientRequirement[] ingredients38 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.FrameSquare, 2),
			new IngredientRequirement(ObjectType.WheelCrude, 1),
			new IngredientRequirement(ObjectType.Axle, 1),
			new IngredientRequirement(ObjectType.Pole, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WheelBarrow, ingredients38);
		IngredientRequirement[] ingredients39 = new IngredientRequirement[5]
		{
			new IngredientRequirement(ObjectType.FrameBox, 1),
			new IngredientRequirement(ObjectType.Panel, 2),
			new IngredientRequirement(ObjectType.WheelCrude, 2),
			new IngredientRequirement(ObjectType.Axle, 1),
			new IngredientRequirement(ObjectType.Pole, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Cart, ingredients39);
		IngredientRequirement[] ingredients40 = new IngredientRequirement[5]
		{
			new IngredientRequirement(ObjectType.FrameBox, 1),
			new IngredientRequirement(ObjectType.Panel, 2),
			new IngredientRequirement(ObjectType.WheelCrude, 2),
			new IngredientRequirement(ObjectType.Axle, 1),
			new IngredientRequirement(ObjectType.Pole, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.CartLiquid, ingredients40);
		IngredientRequirement[] ingredients41 = new IngredientRequirement[6]
		{
			new IngredientRequirement(ObjectType.FrameSquare, 2),
			new IngredientRequirement(ObjectType.Panel, 2),
			new IngredientRequirement(ObjectType.MetalWheel, 2),
			new IngredientRequirement(ObjectType.MetalAxle, 1),
			new IngredientRequirement(ObjectType.MetalCog, 1),
			new IngredientRequirement(ObjectType.Pole, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Minecart, ingredients41);
		IngredientRequirement[] ingredients42 = new IngredientRequirement[6]
		{
			new IngredientRequirement(ObjectType.Firebox, 1),
			new IngredientRequirement(ObjectType.Boiler, 1),
			new IngredientRequirement(ObjectType.Piston, 2),
			new IngredientRequirement(ObjectType.ConnectingRod, 2),
			new IngredientRequirement(ObjectType.MetalWheel, 4),
			new IngredientRequirement(ObjectType.MetalAxle, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Train, ingredients42);
		IngredientRequirement[] ingredients43 = new IngredientRequirement[6]
		{
			new IngredientRequirement(ObjectType.FrameBox, 2),
			new IngredientRequirement(ObjectType.Wheel, 4),
			new IngredientRequirement(ObjectType.StringBall, 4),
			new IngredientRequirement(ObjectType.Cog, 2),
			new IngredientRequirement(ObjectType.Axle, 2),
			new IngredientRequirement(ObjectType.WoodenBeam, 6)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.CraneCrude, ingredients43);
		IngredientRequirement[] ingredients44 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.AnimalCow, 2),
			new IngredientRequirement(ObjectType.HayBale, 3)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.AnimalCow, ingredients44);
		IngredientRequirement[] ingredients45 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.AnimalSheep, 2),
			new IngredientRequirement(ObjectType.HayBale, 3)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.AnimalSheep, ingredients45);
		IngredientRequirement[] ingredients46 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.AnimalChicken, 2),
			new IngredientRequirement(ObjectType.WheatSeed, 10)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.AnimalChicken, ingredients46);
		IngredientRequirement[] ingredients47 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.AnimalCow, 1),
			new IngredientRequirement(ObjectType.HayBale, 3),
			new IngredientRequirement(ObjectType.BullrushesCloth, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.AnimalCowHighland, ingredients47);
		IngredientRequirement[] ingredients48 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.AnimalSheep, 1),
			new IngredientRequirement(ObjectType.HayBale, 3),
			new IngredientRequirement(ObjectType.Blanket, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.AnimalAlpaca, ingredients48);
		IngredientRequirement[] ingredients49 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.AnimalLeech, 1),
			new IngredientRequirement(ObjectType.StringBall, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.AnimalSilkworm, ingredients49);
		IngredientRequirement[] ingredients50 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Panel, 1),
			new IngredientRequirement(ObjectType.FixingPeg, 6)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Cog, ingredients50);
		IngredientRequirement[] ingredients51 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Plank, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.CogCrude, ingredients51);
		IngredientRequirement[] ingredients52 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Log, 1),
			new IngredientRequirement(ObjectType.Pole, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Axle, ingredients52);
		IngredientRequirement[] ingredients53 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Plank, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WheelCrude, ingredients53);
		IngredientRequirement[] ingredients54 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Plank, 3),
			new IngredientRequirement(ObjectType.Pole, 3),
			new IngredientRequirement(ObjectType.FixingPeg, 3)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Wheel, ingredients54);
		IngredientRequirement[] ingredients55 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Pole, 2),
			new IngredientRequirement(ObjectType.FixingPeg, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Crank, ingredients55);
		IngredientRequirement[] ingredients56 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.FrameSquare, 2),
			new IngredientRequirement(ObjectType.Pole, 4),
			new IngredientRequirement(ObjectType.FixingPeg, 8)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.FrameBox, ingredients56);
		IngredientRequirement[] ingredients57 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Pole, 4),
			new IngredientRequirement(ObjectType.FixingPeg, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.FrameSquare, ingredients57);
		IngredientRequirement[] ingredients58 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Pole, 3),
			new IngredientRequirement(ObjectType.FixingPeg, 3)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.FrameTriangle, ingredients58);
		IngredientRequirement[] ingredients59 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.FrameSquare, 1),
			new IngredientRequirement(ObjectType.Panel, 2),
			new IngredientRequirement(ObjectType.Pole, 2),
			new IngredientRequirement(ObjectType.FixingPeg, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.FrameWindow, ingredients59);
		IngredientRequirement[] ingredients60 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.FrameSquare, 2),
			new IngredientRequirement(ObjectType.Panel, 2),
			new IngredientRequirement(ObjectType.Log, 2),
			new IngredientRequirement(ObjectType.FixingPeg, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.FrameDoor, ingredients60);
		IngredientRequirement[] ingredients61 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.FrameSquare, 1),
			new IngredientRequirement(ObjectType.Plank, 4),
			new IngredientRequirement(ObjectType.FixingPeg, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Panel, ingredients61);
		IngredientRequirement[] ingredients62 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Log, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WoodenBeam, ingredients62);
		IngredientRequirement[] ingredients63 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.IronOre, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.IronCrude, ingredients63);
		IngredientRequirement[] ingredients64 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.IronCrude, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.MetalPoleCrude, ingredients64);
		IngredientRequirement[] ingredients65 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.IronCrude, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.MetalPlateCrude, ingredients65);
		IngredientRequirement[] ingredients66 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.MetalPlateCrude, 2),
			new IngredientRequirement(ObjectType.Rivets, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.MetalCog, ingredients66);
		IngredientRequirement[] ingredients67 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.MetalPlateCrude, 2),
			new IngredientRequirement(ObjectType.Rivets, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.MetalWheel, ingredients67);
		IngredientRequirement[] ingredients68 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.MetalPoleCrude, 2),
			new IngredientRequirement(ObjectType.Rivets, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.MetalAxle, ingredients68);
		IngredientRequirement[] ingredients69 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.MetalPlateCrude, 8),
			new IngredientRequirement(ObjectType.MetalPoleCrude, 6),
			new IngredientRequirement(ObjectType.Rivets, 6)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Flywheel, ingredients69);
		IngredientRequirement[] ingredients70 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.MetalPoleCrude, 4),
			new IngredientRequirement(ObjectType.Rivets, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ConnectingRod, ingredients70);
		IngredientRequirement[] ingredients71 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.MetalPoleCrude, 6),
			new IngredientRequirement(ObjectType.Rivets, 3)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Piston, ingredients71);
		IngredientRequirement[] ingredients72 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.MetalPlateCrude, 8),
			new IngredientRequirement(ObjectType.Rivets, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.MetalGirder, ingredients72);
		IngredientRequirement[] ingredients73 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.MetalPlateCrude, 8),
			new IngredientRequirement(ObjectType.Rivets, 8)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Boiler, ingredients73);
		IngredientRequirement[] ingredients74 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.MetalPlateCrude, 8),
			new IngredientRequirement(ObjectType.Rivets, 8)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Firebox, ingredients74);
		IngredientRequirement[] ingredients75 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.MetalPoleCrude, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Rivets, ingredients75);
		IngredientRequirement[] ingredients76 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Rock, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.RockSharp, ingredients76);
		IngredientRequirement[] ingredients77 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.StoneBlockCrude, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.StoneBlock, ingredients77);
		IngredientRequirement[] ingredients78 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Clay, 1),
			new IngredientRequirement(ObjectType.Sand, 1),
			new IngredientRequirement(ObjectType.Water, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Mortar, ingredients78);
		IngredientRequirement[] ingredients79 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Clay, 1),
			new IngredientRequirement(ObjectType.Straw, 1),
			new IngredientRequirement(ObjectType.Water, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.BricksCrudeRaw, ingredients79);
		IngredientRequirement[] ingredients80 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.BricksCrudeRaw, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.BricksCrude, ingredients80);
		IngredientRequirement[] ingredients81 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Clay, 1),
			new IngredientRequirement(ObjectType.Straw, 1),
			new IngredientRequirement(ObjectType.Water, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.RoofTilesRaw, ingredients81);
		IngredientRequirement[] ingredients82 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.RoofTilesRaw, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.RoofTiles, ingredients82);
		IngredientRequirement[] ingredients83 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Rock, 10),
			new IngredientRequirement(ObjectType.Mortar, 5)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Fireplace, ingredients83);
		IngredientRequirement[] ingredients84 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Rock, 10),
			new IngredientRequirement(ObjectType.Mortar, 5)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Chimney, ingredients84);
		IngredientRequirement[] ingredients85 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.StoneBlockCrude, 3)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Millstone, ingredients85);
		IngredientRequirement[] ingredients86 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Wool, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Blanket, ingredients86);
		IngredientRequirement[] ingredients87 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.FixingPeg, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Buttons, ingredients87);
		IngredientRequirement[] ingredients88 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.StringBall, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Thread, ingredients88);
		IngredientRequirement[] ingredients89 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.CottonLint, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.CottonThread, ingredients89);
		IngredientRequirement[] ingredients90 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.CottonThread, 3)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.CottonCloth, ingredients90);
		IngredientRequirement[] ingredients91 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.BullrushesStems, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.BullrushesFibre, ingredients91);
		IngredientRequirement[] ingredients92 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.BullrushesFibre, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.BullrushesThread, ingredients92);
		IngredientRequirement[] ingredients93 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.BullrushesThread, 3)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.BullrushesCloth, ingredients93);
		IngredientRequirement[] ingredients94 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.SilkRaw, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.SilkThread, ingredients94);
		IngredientRequirement[] ingredients95 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.SilkThread, 6)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.SilkCloth, ingredients95);
		IngredientRequirement[] ingredients96 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.MushroomDug, 4),
			new IngredientRequirement(ObjectType.WeedDug, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.MushroomHerb, ingredients96);
		IngredientRequirement[] ingredients97 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.MushroomHerb, 2),
			new IngredientRequirement(ObjectType.Water, 2),
			new IngredientRequirement(ObjectType.PotClay, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.MushroomSoup, ingredients97);
		IngredientRequirement[] ingredients98 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.MushroomSoup, 2),
			new IngredientRequirement(ObjectType.Dough, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.MushroomStew, ingredients98);
		IngredientRequirement[] ingredients99 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.MushroomStew, 3),
			new IngredientRequirement(ObjectType.Pastry, 3)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.MushroomPieRaw, ingredients99);
		IngredientRequirement[] ingredients100 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.MushroomPieRaw, 1),
			new IngredientRequirement(ObjectType.LargeBowlClay, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.MushroomPie, ingredients100);
		IngredientRequirement[] ingredients101 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.MushroomStew, 3),
			new IngredientRequirement(ObjectType.CakeBatter, 3)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.MushroomPuddingRaw, ingredients101);
		IngredientRequirement[] ingredients102 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.MushroomPuddingRaw, 1),
			new IngredientRequirement(ObjectType.LargeBowlClay, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.MushroomPudding, ingredients102);
		IngredientRequirement[] ingredients103 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.MushroomPudding, 1),
			new IngredientRequirement(ObjectType.Egg, 1),
			new IngredientRequirement(ObjectType.DoughGood, 1),
			new IngredientRequirement(ObjectType.Butter, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.MushroomBurger, ingredients103);
		IngredientRequirement[] ingredients104 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Berries, 4),
			new IngredientRequirement(ObjectType.WeedDug, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.BerriesSpice, ingredients104);
		IngredientRequirement[] ingredients105 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.BerriesSpice, 2),
			new IngredientRequirement(ObjectType.Water, 2),
			new IngredientRequirement(ObjectType.PotClay, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.BerriesStew, ingredients105);
		IngredientRequirement[] ingredients106 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.BerriesStew, 2),
			new IngredientRequirement(ObjectType.Honey, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.BerriesJam, ingredients106);
		IngredientRequirement[] ingredients107 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.BerriesJam, 3),
			new IngredientRequirement(ObjectType.Pastry, 3)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.BerriesPieRaw, ingredients107);
		IngredientRequirement[] ingredients108 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.BerriesPieRaw, 1),
			new IngredientRequirement(ObjectType.LargeBowlClay, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.BerriesPie, ingredients108);
		IngredientRequirement[] ingredients109 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.BerriesJam, 3),
			new IngredientRequirement(ObjectType.CakeBatter, 3)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.BerriesCakeRaw, ingredients109);
		IngredientRequirement[] ingredients110 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.BerriesCakeRaw, 1),
			new IngredientRequirement(ObjectType.LargeBowlClay, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.BerriesCake, ingredients110);
		IngredientRequirement[] ingredients111 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.BerriesCake, 1),
			new IngredientRequirement(ObjectType.Egg, 1),
			new IngredientRequirement(ObjectType.DoughGood, 1),
			new IngredientRequirement(ObjectType.Butter, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.BerryDanish, ingredients111);
		IngredientRequirement[] ingredients112 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.WheatSeed, 2),
			new IngredientRequirement(ObjectType.Water, 2),
			new IngredientRequirement(ObjectType.PotClay, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Porridge, ingredients112);
		IngredientRequirement[] ingredients113 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.FlourCrude, 1),
			new IngredientRequirement(ObjectType.Dough, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.BreadCrude, ingredients113);
		IngredientRequirement[] ingredients114 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Dough, 2),
			new IngredientRequirement(ObjectType.DoughGood, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Bread, ingredients114);
		IngredientRequirement[] ingredients115 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Bread, 1),
			new IngredientRequirement(ObjectType.Butter, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.BreadButtered, ingredients115);
		IngredientRequirement[] ingredients116 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.BreadButtered, 2),
			new IngredientRequirement(ObjectType.Milk, 3),
			new IngredientRequirement(ObjectType.Egg, 3),
			new IngredientRequirement(ObjectType.Honey, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.BreadPuddingRaw, ingredients116);
		IngredientRequirement[] ingredients117 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.BreadPuddingRaw, 1),
			new IngredientRequirement(ObjectType.LargeBowlClay, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.BreadPudding, ingredients117);
		IngredientRequirement[] ingredients118 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.BreadPudding, 1),
			new IngredientRequirement(ObjectType.Egg, 3),
			new IngredientRequirement(ObjectType.BerriesJam, 1),
			new IngredientRequirement(ObjectType.Butter, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.CreamBrioche, ingredients118);
		IngredientRequirement[] ingredients119 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.PumpkinRaw, 4),
			new IngredientRequirement(ObjectType.WeedDug, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.PumpkinHerb, ingredients119);
		IngredientRequirement[] ingredients120 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.PumpkinHerb, 1),
			new IngredientRequirement(ObjectType.Water, 2),
			new IngredientRequirement(ObjectType.PotClay, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.PumpkinSoup, ingredients120);
		IngredientRequirement[] ingredients121 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.PumpkinSoup, 1),
			new IngredientRequirement(ObjectType.Dough, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.PumpkinStew, ingredients121);
		IngredientRequirement[] ingredients122 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.PumpkinStew, 2),
			new IngredientRequirement(ObjectType.Pastry, 3)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.PumpkinPieRaw, ingredients122);
		IngredientRequirement[] ingredients123 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.PumpkinPieRaw, 1),
			new IngredientRequirement(ObjectType.LargeBowlClay, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.PumpkinPie, ingredients123);
		IngredientRequirement[] ingredients124 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.PumpkinStew, 3),
			new IngredientRequirement(ObjectType.CakeBatter, 3)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.PumpkinCakeRaw, ingredients124);
		IngredientRequirement[] ingredients125 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.PumpkinCakeRaw, 1),
			new IngredientRequirement(ObjectType.LargeBowlClay, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.PumpkinCake, ingredients125);
		IngredientRequirement[] ingredients126 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.PumpkinCake, 1),
			new IngredientRequirement(ObjectType.Egg, 1),
			new IngredientRequirement(ObjectType.DoughGood, 1),
			new IngredientRequirement(ObjectType.Butter, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.PumpkinBurger, ingredients126);
		IngredientRequirement[] ingredients127 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Apple, 4),
			new IngredientRequirement(ObjectType.WeedDug, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.AppleSpice, ingredients127);
		IngredientRequirement[] ingredients128 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.AppleSpice, 2),
			new IngredientRequirement(ObjectType.Water, 2),
			new IngredientRequirement(ObjectType.PotClay, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.AppleStew, ingredients128);
		IngredientRequirement[] ingredients129 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.AppleStew, 2),
			new IngredientRequirement(ObjectType.Honey, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.AppleJam, ingredients129);
		IngredientRequirement[] ingredients130 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.AppleJam, 3),
			new IngredientRequirement(ObjectType.Pastry, 3)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ApplePieRaw, ingredients130);
		IngredientRequirement[] ingredients131 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.ApplePieRaw, 1),
			new IngredientRequirement(ObjectType.LargeBowlClay, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ApplePie, ingredients131);
		IngredientRequirement[] ingredients132 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.AppleJam, 3),
			new IngredientRequirement(ObjectType.CakeBatter, 3)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.AppleCakeRaw, ingredients132);
		IngredientRequirement[] ingredients133 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.AppleCakeRaw, 1),
			new IngredientRequirement(ObjectType.LargeBowlClay, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.AppleCake, ingredients133);
		IngredientRequirement[] ingredients134 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.AppleCake, 1),
			new IngredientRequirement(ObjectType.Egg, 1),
			new IngredientRequirement(ObjectType.DoughGood, 1),
			new IngredientRequirement(ObjectType.Butter, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.AppleDanish, ingredients134);
		IngredientRequirement[] ingredients135 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.FishAny, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.FishRaw, ingredients135);
		IngredientRequirement[] ingredients136 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.FishRaw, 4),
			new IngredientRequirement(ObjectType.WeedDug, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.FishHerb, ingredients136);
		IngredientRequirement[] ingredients137 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.FishHerb, 2),
			new IngredientRequirement(ObjectType.Water, 2),
			new IngredientRequirement(ObjectType.PotClay, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.FishSoup, ingredients137);
		IngredientRequirement[] ingredients138 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.FishSoup, 2),
			new IngredientRequirement(ObjectType.Dough, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.FishStew, ingredients138);
		IngredientRequirement[] ingredients139 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.FishStew, 3),
			new IngredientRequirement(ObjectType.Pastry, 3)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.FishPieRaw, ingredients139);
		IngredientRequirement[] ingredients140 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.FishPieRaw, 1),
			new IngredientRequirement(ObjectType.LargeBowlClay, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.FishPie, ingredients140);
		IngredientRequirement[] ingredients141 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.FishStew, 3),
			new IngredientRequirement(ObjectType.CakeBatter, 3)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.FishCakeRaw, ingredients141);
		IngredientRequirement[] ingredients142 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.FishCakeRaw, 1),
			new IngredientRequirement(ObjectType.LargeBowlClay, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.FishCake, ingredients142);
		IngredientRequirement[] ingredients143 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.FishCake, 1),
			new IngredientRequirement(ObjectType.Egg, 1),
			new IngredientRequirement(ObjectType.DoughGood, 1),
			new IngredientRequirement(ObjectType.Butter, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.FishBurger, ingredients143);
		IngredientRequirement[] ingredients144 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Carrot, 4),
			new IngredientRequirement(ObjectType.WeedDug, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.CarrotSalad, ingredients144);
		IngredientRequirement[] ingredients145 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.CarrotSalad, 2),
			new IngredientRequirement(ObjectType.PumpkinSeeds, 2),
			new IngredientRequirement(ObjectType.PotClay, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.CarrotStirFry, ingredients145);
		IngredientRequirement[] ingredients146 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.CarrotStirFry, 2),
			new IngredientRequirement(ObjectType.Honey, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.CarrotHoney, ingredients146);
		IngredientRequirement[] ingredients147 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.CarrotHoney, 3),
			new IngredientRequirement(ObjectType.Water, 1),
			new IngredientRequirement(ObjectType.Naan, 1),
			new IngredientRequirement(ObjectType.LargeBowlClay, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.CarrotCurry, ingredients147);
		IngredientRequirement[] ingredients148 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.CarrotHoney, 3),
			new IngredientRequirement(ObjectType.CakeBatter, 3)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.CarrotCakeRaw, ingredients148);
		IngredientRequirement[] ingredients149 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.CarrotCakeRaw, 1),
			new IngredientRequirement(ObjectType.LargeBowlClay, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.CarrotCake, ingredients149);
		IngredientRequirement[] ingredients150 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.CarrotCake, 1),
			new IngredientRequirement(ObjectType.Egg, 1),
			new IngredientRequirement(ObjectType.DoughGood, 1),
			new IngredientRequirement(ObjectType.Butter, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.CarrotBurger, ingredients150);
		IngredientRequirement[] ingredients151 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Porridge, 2),
			new IngredientRequirement(ObjectType.Milk, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.MilkPorridge, ingredients151);
		IngredientRequirement[] ingredients152 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.MilkPorridge, 2),
			new IngredientRequirement(ObjectType.Berries, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.FruitPorridge, ingredients152);
		IngredientRequirement[] ingredients153 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.MilkPorridge, 2),
			new IngredientRequirement(ObjectType.Honey, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.HoneyPorridge, ingredients153);
		IngredientRequirement[] ingredients154 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.AppleJam, 3),
			new IngredientRequirement(ObjectType.BerriesSpice, 3),
			new IngredientRequirement(ObjectType.Pastry, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.AppleBerryPieRaw, ingredients154);
		IngredientRequirement[] ingredients155 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.AppleBerryPieRaw, 1),
			new IngredientRequirement(ObjectType.LargeBowlClay, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.AppleBerryPie, ingredients155);
		IngredientRequirement[] ingredients156 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.PumpkinStew, 3),
			new IngredientRequirement(ObjectType.MushroomHerb, 3),
			new IngredientRequirement(ObjectType.Pastry, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.PumpkinMushroomPieRaw, ingredients156);
		IngredientRequirement[] ingredients157 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.PumpkinMushroomPieRaw, 1),
			new IngredientRequirement(ObjectType.LargeBowlClay, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.PumpkinMushroomPie, ingredients157);
		IngredientRequirement[] ingredients158 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.WheatSeed, 6)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Flour, ingredients158);
		IngredientRequirement[] ingredients159 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.WheatSeed, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.FlourCrude, ingredients159);
		IngredientRequirement[] ingredients160 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Log, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Charcoal, ingredients160);
		IngredientRequirement[] ingredients161 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.PotClayRaw, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.PotClay, ingredients161);
		IngredientRequirement[] ingredients162 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Clay, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.PotClayRaw, ingredients162);
		IngredientRequirement[] ingredients163 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.LargeBowlClayRaw, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.LargeBowlClay, ingredients163);
		IngredientRequirement[] ingredients164 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Clay, 8)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.LargeBowlClayRaw, ingredients164);
		IngredientRequirement[] ingredients165 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.JarClayRaw, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.JarClay, ingredients165);
		IngredientRequirement[] ingredients166 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Clay, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.JarClayRaw, ingredients166);
		IngredientRequirement[] ingredients167 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Milk, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Butter, ingredients167);
		IngredientRequirement[] ingredients168 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Butter, 2),
			new IngredientRequirement(ObjectType.Water, 2),
			new IngredientRequirement(ObjectType.Flour, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Pastry, ingredients168);
		IngredientRequirement[] ingredients169 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Porridge, 2),
			new IngredientRequirement(ObjectType.FlourCrude, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Dough, ingredients169);
		IngredientRequirement[] ingredients170 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Dough, 1),
			new IngredientRequirement(ObjectType.Flour, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.DoughGood, ingredients170);
		IngredientRequirement[] ingredients171 = new IngredientRequirement[5]
		{
			new IngredientRequirement(ObjectType.Flour, 1),
			new IngredientRequirement(ObjectType.Butter, 1),
			new IngredientRequirement(ObjectType.Egg, 1),
			new IngredientRequirement(ObjectType.Honey, 1),
			new IngredientRequirement(ObjectType.LargeBowlClay, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.CakeBatter, ingredients171);
		IngredientRequirement[] ingredients172 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Butter, 2),
			new IngredientRequirement(ObjectType.Water, 2),
			new IngredientRequirement(ObjectType.Flour, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.NaanRaw, ingredients172);
		IngredientRequirement[] ingredients173 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.NaanRaw, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Naan, ingredients173);
		IngredientRequirement[] ingredients174 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.WeedDug, 2),
			new IngredientRequirement(ObjectType.PumpkinSeeds, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.CarrotSeed, ingredients174);
		IngredientRequirement[] ingredients175 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.TreeSeed, 8),
			new IngredientRequirement(ObjectType.BullrushesSeeds, 8)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Coconut, ingredients175);
		IngredientRequirement[] ingredients176 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Berries, 8),
			new IngredientRequirement(ObjectType.CottonSeeds, 8)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.MulberrySeed, ingredients176);
		IngredientRequirement[] ingredients177 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Fleece, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Wool, ingredients177);
		IngredientRequirement[] ingredients178 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.FlowerPotRaw, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.FlowerPot, ingredients178);
		IngredientRequirement[] ingredients179 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Clay, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.FlowerPotRaw, ingredients179);
		IngredientRequirement[] ingredients180 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.GnomeRaw, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Gnome, ingredients180);
		IngredientRequirement[] ingredients181 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.GnomeRaw, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Gnome2, ingredients181);
		IngredientRequirement[] ingredients182 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.GnomeRaw, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Gnome3, ingredients182);
		IngredientRequirement[] ingredients183 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.GnomeRaw, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Gnome4, ingredients183);
		IngredientRequirement[] ingredients184 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.GnomeRaw, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Gnome5, ingredients184);
		IngredientRequirement[] ingredients185 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.GnomeRaw, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Gnome6, ingredients185);
		IngredientRequirement[] ingredients186 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Clay, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.GnomeRaw, ingredients186);
		IngredientRequirement[] ingredients187 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Plank, 2),
			new IngredientRequirement(ObjectType.Pole, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Sign, ingredients187);
		IngredientRequirement[] ingredients188 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Plank, 2),
			new IngredientRequirement(ObjectType.Pole, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Sign2, ingredients188);
		IngredientRequirement[] ingredients189 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Plank, 2),
			new IngredientRequirement(ObjectType.Pole, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Sign3, ingredients189);
		IngredientRequirement[] ingredients190 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Log, 1),
			new IngredientRequirement(ObjectType.Plank, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Billboard, ingredients190);
		IngredientRequirement[] ingredients191 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.WeedDug, 5)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.StringBall, ingredients191);
		IngredientRequirement[] ingredients192 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.GrassCut, 5)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.HayBale, ingredients192);
		IngredientRequirement[] ingredients193 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Straw, 6),
			new IngredientRequirement(ObjectType.Stick, 3),
			new IngredientRequirement(ObjectType.Pumpkin, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Scarecrow, ingredients193);
		IngredientRequirement[] ingredients194 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Plank, 2),
			new IngredientRequirement(ObjectType.Pole, 2),
			new IngredientRequirement(ObjectType.TreeSeed, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.DataStorageCrude, ingredients194);
		IngredientRequirement[] ingredients195 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Plank, 5),
			new IngredientRequirement(ObjectType.Pole, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.UpgradePlayerInventoryCrude, ingredients195);
		IngredientRequirement[] ingredients196 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.FrameBox, 1),
			new IngredientRequirement(ObjectType.Plank, 5)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.UpgradePlayerInventoryGood, ingredients196);
		IngredientRequirement[] ingredients197 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.FrameBox, 1),
			new IngredientRequirement(ObjectType.Panel, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.UpgradePlayerInventorySuper, ingredients197);
		IngredientRequirement[] ingredients198 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Plank, 1),
			new IngredientRequirement(ObjectType.Pole, 3),
			new IngredientRequirement(ObjectType.WheelCrude, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.UpgradePlayerMovementCrude, ingredients198);
		IngredientRequirement[] ingredients199 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Plank, 1),
			new IngredientRequirement(ObjectType.WheelCrude, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.UpgradePlayerMovementGood, ingredients199);
		IngredientRequirement[] ingredients200 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.Plank, 1),
			new IngredientRequirement(ObjectType.Pole, 1),
			new IngredientRequirement(ObjectType.Axle, 1),
			new IngredientRequirement(ObjectType.Wheel, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.UpgradePlayerMovementSuper, ingredients200);
		IngredientRequirement[] ingredients201 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Plank, 3),
			new IngredientRequirement(ObjectType.Pole, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.UpgradeWorkerInventoryCrude, ingredients201);
		IngredientRequirement[] ingredients202 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.FrameSquare, 1),
			new IngredientRequirement(ObjectType.Plank, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.UpgradeWorkerInventoryGood, ingredients202);
		IngredientRequirement[] ingredients203 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.FrameBox, 1),
			new IngredientRequirement(ObjectType.Panel, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.UpgradeWorkerInventorySuper, ingredients203);
		IngredientRequirement[] ingredients204 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Pole, 2),
			new IngredientRequirement(ObjectType.TreeSeed, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.UpgradeWorkerMemoryCrude, ingredients204);
		IngredientRequirement[] ingredients205 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Pole, 8),
			new IngredientRequirement(ObjectType.TreeSeed, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.UpgradeWorkerMemoryGood, ingredients205);
		IngredientRequirement[] ingredients206 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.MetalPoleCrude, 1),
			new IngredientRequirement(ObjectType.Coconut, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.UpgradeWorkerMemorySuper, ingredients206);
		IngredientRequirement[] ingredients207 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Plank, 4),
			new IngredientRequirement(ObjectType.Pole, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.UpgradeWorkerSearchCrude, ingredients207);
		IngredientRequirement[] ingredients208 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Panel, 1),
			new IngredientRequirement(ObjectType.Pole, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.UpgradeWorkerSearchGood, ingredients208);
		IngredientRequirement[] ingredients209 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.MetalPlateCrude, 1),
			new IngredientRequirement(ObjectType.MetalPoleCrude, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.UpgradeWorkerSearchSuper, ingredients209);
		IngredientRequirement[] ingredients210 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.CogCrude, 2),
			new IngredientRequirement(ObjectType.Pole, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.UpgradeWorkerCarryCrude, ingredients210);
		IngredientRequirement[] ingredients211 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.CogCrude, 4),
			new IngredientRequirement(ObjectType.Pole, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.UpgradeWorkerCarryGood, ingredients211);
		IngredientRequirement[] ingredients212 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.MetalCog, 1),
			new IngredientRequirement(ObjectType.MetalAxle, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.UpgradeWorkerCarrySuper, ingredients212);
		IngredientRequirement[] ingredients213 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.CogCrude, 4),
			new IngredientRequirement(ObjectType.Pole, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.UpgradeWorkerMovementCrude, ingredients213);
		IngredientRequirement[] ingredients214 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Cog, 4),
			new IngredientRequirement(ObjectType.Axle, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.UpgradeWorkerMovementGood, ingredients214);
		IngredientRequirement[] ingredients215 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.MetalCog, 1),
			new IngredientRequirement(ObjectType.MetalAxle, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.UpgradeWorkerMovementSuper, ingredients215);
		IngredientRequirement[] ingredients216 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.CogCrude, 4),
			new IngredientRequirement(ObjectType.Pole, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.UpgradeWorkerEnergyCrude, ingredients216);
		IngredientRequirement[] ingredients217 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Cog, 4),
			new IngredientRequirement(ObjectType.Axle, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.UpgradeWorkerEnergyGood, ingredients217);
		IngredientRequirement[] ingredients218 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.MetalCog, 1),
			new IngredientRequirement(ObjectType.MetalAxle, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.UpgradeWorkerEnergySuper, ingredients218);
		IngredientRequirement[] ingredients219 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Wool, 3)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.HatChullo, ingredients219);
		IngredientRequirement[] ingredients220 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Wool, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.HatFarmerCap, ingredients220);
		IngredientRequirement[] ingredients221 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Wool, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.HatCap, ingredients221);
		IngredientRequirement[] ingredients222 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Straw, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.HatFarmer, ingredients222);
		IngredientRequirement[] ingredients223 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Wool, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.HatBeret, ingredients223);
		IngredientRequirement[] ingredients224 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Straw, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.HatSugegasa, ingredients224);
		IngredientRequirement[] ingredients225 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Wool, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.HatFez, ingredients225);
		IngredientRequirement[] ingredients226 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Wool, 3)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.HatKnittedBeanie, ingredients226);
		IngredientRequirement[] ingredients227 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.BullrushesCloth, 2),
			new IngredientRequirement(ObjectType.BullrushesThread, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.HatAdventurer, ingredients227);
		IngredientRequirement[] ingredients228 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Wool, 8)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.HatWig01, ingredients228);
		IngredientRequirement[] ingredients229 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.CottonCloth, 2),
			new IngredientRequirement(ObjectType.CottonThread, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.HatChef, ingredients229);
		IngredientRequirement[] ingredients230 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Blanket, 2),
			new IngredientRequirement(ObjectType.Thread, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.HatMushroom, ingredients230);
		IngredientRequirement[] ingredients231 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.BullrushesCloth, 2),
			new IngredientRequirement(ObjectType.Wool, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.HatLumberjack, ingredients231);
		IngredientRequirement[] ingredients232 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.BullrushesCloth, 2),
			new IngredientRequirement(ObjectType.BullrushesThread, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.HatBaseballShow, ingredients232);
		IngredientRequirement[] ingredients233 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.BullrushesCloth, 2),
			new IngredientRequirement(ObjectType.BullrushesThread, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.HatTree, ingredients233);
		IngredientRequirement[] ingredients234 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.FrameSquare, 1),
			new IngredientRequirement(ObjectType.Plank, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.HatCrown, ingredients234);
		IngredientRequirement[] ingredients235 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.FrameSquare, 1),
			new IngredientRequirement(ObjectType.Plank, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.HatMortarboard, ingredients235);
		IngredientRequirement[] ingredients236 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.BullrushesCloth, 2),
			new IngredientRequirement(ObjectType.BullrushesThread, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.HatSouwester, ingredients236);
		IngredientRequirement[] ingredients237 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Wool, 6)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.HatMadHatter, ingredients237);
		IngredientRequirement[] ingredients238 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.CottonCloth, 2),
			new IngredientRequirement(ObjectType.CottonThread, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.HatCloche, ingredients238);
		IngredientRequirement[] ingredients239 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.BullrushesCloth, 2),
			new IngredientRequirement(ObjectType.BullrushesThread, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.HatAcorn, ingredients239);
		IngredientRequirement[] ingredients240 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.BullrushesCloth, 2),
			new IngredientRequirement(ObjectType.BullrushesThread, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.HatSanta, ingredients240);
		IngredientRequirement[] ingredients241 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Blanket, 2),
			new IngredientRequirement(ObjectType.Thread, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.HatWally, ingredients241);
		IngredientRequirement[] ingredients242 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.CottonCloth, 2),
			new IngredientRequirement(ObjectType.CottonThread, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.HatParty, ingredients242);
		IngredientRequirement[] ingredients243 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Pole, 4),
			new IngredientRequirement(ObjectType.FixingPeg, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.HatViking, ingredients243);
		IngredientRequirement[] ingredients244 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Paper, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.HatBox, ingredients244);
		IngredientRequirement[] ingredients245 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.BullrushesCloth, 2),
			new IngredientRequirement(ObjectType.BullrushesThread, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.HatTrain, ingredients245);
		IngredientRequirement[] ingredients246 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.CottonCloth, 2),
			new IngredientRequirement(ObjectType.CottonThread, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.HatSailor, ingredients246);
		IngredientRequirement[] ingredients247 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.MetalPlateCrude, 2),
			new IngredientRequirement(ObjectType.ToolTorchCrude, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.HatMiner, ingredients247);
		IngredientRequirement[] ingredients248 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Blanket, 1),
			new IngredientRequirement(ObjectType.CottonCloth, 1),
			new IngredientRequirement(ObjectType.Thread, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.HatFox, ingredients248);
		IngredientRequirement[] ingredients249 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Blanket, 1),
			new IngredientRequirement(ObjectType.BullrushesCloth, 1),
			new IngredientRequirement(ObjectType.Thread, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.HatDinosaur, ingredients249);
		IngredientRequirement[] ingredients250 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Plank, 2),
			new IngredientRequirement(ObjectType.Pole, 4),
			new IngredientRequirement(ObjectType.FixingPeg, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.HatAntlers, ingredients250);
		IngredientRequirement[] ingredients251 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Blanket, 1),
			new IngredientRequirement(ObjectType.CottonCloth, 1),
			new IngredientRequirement(ObjectType.Thread, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.HatBunny, ingredients251);
		IngredientRequirement[] ingredients252 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Blanket, 1),
			new IngredientRequirement(ObjectType.BullrushesCloth, 1),
			new IngredientRequirement(ObjectType.Thread, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.HatDuck, ingredients252);
		IngredientRequirement[] ingredients253 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.Blanket, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TopPoncho, ingredients253);
		IngredientRequirement[] ingredients254 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.TopPoncho, 1),
			new IngredientRequirement(ObjectType.Blanket, 1),
			new IngredientRequirement(ObjectType.StringBall, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TopJumper, ingredients254);
		IngredientRequirement[] ingredients255 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.TopJumper, 1),
			new IngredientRequirement(ObjectType.Blanket, 1),
			new IngredientRequirement(ObjectType.Buttons, 1),
			new IngredientRequirement(ObjectType.Thread, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TopJacket, ingredients255);
		IngredientRequirement[] ingredients256 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.TopJacket, 1),
			new IngredientRequirement(ObjectType.CottonThread, 1),
			new IngredientRequirement(ObjectType.CottonCloth, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TopBlazer, ingredients256);
		IngredientRequirement[] ingredients257 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.TopBlazer, 1),
			new IngredientRequirement(ObjectType.SilkThread, 4),
			new IngredientRequirement(ObjectType.SilkCloth, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TopTuxedo, ingredients257);
		IngredientRequirement[] ingredients258 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.CottonCloth, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TopTunic, ingredients258);
		IngredientRequirement[] ingredients259 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.TopTunic, 1),
			new IngredientRequirement(ObjectType.CottonCloth, 1),
			new IngredientRequirement(ObjectType.StringBall, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TopDress, ingredients259);
		IngredientRequirement[] ingredients260 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.TopDress, 1),
			new IngredientRequirement(ObjectType.CottonCloth, 1),
			new IngredientRequirement(ObjectType.Buttons, 1),
			new IngredientRequirement(ObjectType.CottonThread, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TopShirt, ingredients260);
		IngredientRequirement[] ingredients261 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.TopShirt, 1),
			new IngredientRequirement(ObjectType.BullrushesThread, 1),
			new IngredientRequirement(ObjectType.BullrushesCloth, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TopShirtTie, ingredients261);
		IngredientRequirement[] ingredients262 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.TopShirtTie, 1),
			new IngredientRequirement(ObjectType.SilkThread, 4),
			new IngredientRequirement(ObjectType.SilkCloth, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TopGown, ingredients262);
		IngredientRequirement[] ingredients263 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.TopDress, 1),
			new IngredientRequirement(ObjectType.CottonCloth, 1),
			new IngredientRequirement(ObjectType.Buttons, 1),
			new IngredientRequirement(ObjectType.CottonThread, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TopDungarees, ingredients263);
		IngredientRequirement[] ingredients264 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.TopDress, 1),
			new IngredientRequirement(ObjectType.CottonCloth, 1),
			new IngredientRequirement(ObjectType.Buttons, 1),
			new IngredientRequirement(ObjectType.CottonThread, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TopPlumber, ingredients264);
		IngredientRequirement[] ingredients265 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.TopDress, 1),
			new IngredientRequirement(ObjectType.CottonCloth, 1),
			new IngredientRequirement(ObjectType.Buttons, 1),
			new IngredientRequirement(ObjectType.CottonThread, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TopDungareesClown, ingredients265);
		IngredientRequirement[] ingredients266 = new IngredientRequirement[1]
		{
			new IngredientRequirement(ObjectType.BullrushesCloth, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TopToga, ingredients266);
		IngredientRequirement[] ingredients267 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.TopToga, 1),
			new IngredientRequirement(ObjectType.BullrushesCloth, 1),
			new IngredientRequirement(ObjectType.StringBall, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TopRobe, ingredients267);
		IngredientRequirement[] ingredients268 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.TopRobe, 1),
			new IngredientRequirement(ObjectType.BullrushesCloth, 1),
			new IngredientRequirement(ObjectType.Buttons, 1),
			new IngredientRequirement(ObjectType.BullrushesThread, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TopCoat, ingredients268);
		IngredientRequirement[] ingredients269 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.TopCoat, 1),
			new IngredientRequirement(ObjectType.Thread, 1),
			new IngredientRequirement(ObjectType.Blanket, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TopCoatScarf, ingredients269);
		IngredientRequirement[] ingredients270 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.TopCoatScarf, 1),
			new IngredientRequirement(ObjectType.SilkThread, 4),
			new IngredientRequirement(ObjectType.SilkCloth, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TopSuit, ingredients270);
		IngredientRequirement[] ingredients271 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.TopTunic, 1),
			new IngredientRequirement(ObjectType.CottonCloth, 1),
			new IngredientRequirement(ObjectType.CottonThread, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TopTShirtShow, ingredients271);
		IngredientRequirement[] ingredients272 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.TopJumper, 1),
			new IngredientRequirement(ObjectType.Blanket, 1),
			new IngredientRequirement(ObjectType.Buttons, 1),
			new IngredientRequirement(ObjectType.Thread, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TopLumberjack, ingredients272);
		IngredientRequirement[] ingredients273 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.TopTunic, 1),
			new IngredientRequirement(ObjectType.CottonCloth, 1),
			new IngredientRequirement(ObjectType.CottonThread, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TopAdventurer, ingredients273);
		IngredientRequirement[] ingredients274 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.TopRobe, 1),
			new IngredientRequirement(ObjectType.BullrushesCloth, 1),
			new IngredientRequirement(ObjectType.Buttons, 1),
			new IngredientRequirement(ObjectType.BullrushesThread, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TopMac, ingredients274);
		IngredientRequirement[] ingredients275 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.TopRobe, 1),
			new IngredientRequirement(ObjectType.BullrushesCloth, 1),
			new IngredientRequirement(ObjectType.Buttons, 1),
			new IngredientRequirement(ObjectType.BullrushesThread, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TopTree, ingredients275);
		IngredientRequirement[] ingredients276 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.TopPoncho, 1),
			new IngredientRequirement(ObjectType.Blanket, 1),
			new IngredientRequirement(ObjectType.StringBall, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TopJumper02, ingredients276);
		IngredientRequirement[] ingredients277 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.TopToga, 1),
			new IngredientRequirement(ObjectType.BullrushesCloth, 1),
			new IngredientRequirement(ObjectType.StringBall, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TopApron, ingredients277);
		IngredientRequirement[] ingredients278 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.TopRobe, 1),
			new IngredientRequirement(ObjectType.BullrushesCloth, 1),
			new IngredientRequirement(ObjectType.Buttons, 1),
			new IngredientRequirement(ObjectType.BullrushesThread, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TopSanta, ingredients278);
		IngredientRequirement[] ingredients279 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.TopPoncho, 1),
			new IngredientRequirement(ObjectType.Blanket, 1),
			new IngredientRequirement(ObjectType.StringBall, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TopWally, ingredients279);
		IngredientRequirement[] ingredients280 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.TopTunic, 1),
			new IngredientRequirement(ObjectType.CottonCloth, 1),
			new IngredientRequirement(ObjectType.CottonThread, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TopTShirt02, ingredients280);
		IngredientRequirement[] ingredients281 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.TopJumper, 1),
			new IngredientRequirement(ObjectType.CottonThread, 1),
			new IngredientRequirement(ObjectType.CottonCloth, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TopFox, ingredients281);
		IngredientRequirement[] ingredients282 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.TopJumper, 1),
			new IngredientRequirement(ObjectType.BullrushesThread, 1),
			new IngredientRequirement(ObjectType.BullrushesCloth, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TopDinosaur, ingredients282);
		IngredientRequirement[] ingredients283 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.TopJumper, 1),
			new IngredientRequirement(ObjectType.BullrushesThread, 1),
			new IngredientRequirement(ObjectType.BullrushesCloth, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TopBunny, ingredients283);
		IngredientRequirement[] ingredients284 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.TopJumper, 1),
			new IngredientRequirement(ObjectType.BullrushesThread, 1),
			new IngredientRequirement(ObjectType.BullrushesCloth, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.TopDuck, ingredients284);
		IngredientRequirement[] ingredients285 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.Pole, 2),
			new IngredientRequirement(ObjectType.Rock, 1),
			new IngredientRequirement(ObjectType.Blanket, 1),
			new IngredientRequirement(ObjectType.Buttons, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Doll, ingredients285);
		IngredientRequirement[] ingredients286 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Doll, 1),
			new IngredientRequirement(ObjectType.FrameBox, 2),
			new IngredientRequirement(ObjectType.Panel, 3)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.JackInTheBox, ingredients286);
		IngredientRequirement[] ingredients287 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.JackInTheBox, 1),
			new IngredientRequirement(ObjectType.Doll, 1),
			new IngredientRequirement(ObjectType.FrameTriangle, 2),
			new IngredientRequirement(ObjectType.Panel, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.DollHouse, ingredients287);
		IngredientRequirement[] ingredients288 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.DollHouse, 1),
			new IngredientRequirement(ObjectType.MetalPlateCrude, 1),
			new IngredientRequirement(ObjectType.MetalPoleCrude, 1),
			new IngredientRequirement(ObjectType.Rivets, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Spaceship, ingredients288);
		IngredientRequirement[] ingredients289 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.Blanket, 2),
			new IngredientRequirement(ObjectType.Thread, 1),
			new IngredientRequirement(ObjectType.Buttons, 2),
			new IngredientRequirement(ObjectType.Straw, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ToyHorse, ingredients289);
		IngredientRequirement[] ingredients290 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.ToyHorse, 1),
			new IngredientRequirement(ObjectType.Wheel, 2),
			new IngredientRequirement(ObjectType.FrameBox, 1),
			new IngredientRequirement(ObjectType.Panel, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ToyHorseCart, ingredients290);
		IngredientRequirement[] ingredients291 = new IngredientRequirement[5]
		{
			new IngredientRequirement(ObjectType.ToyHorseCart, 1),
			new IngredientRequirement(ObjectType.ToyHorse, 1),
			new IngredientRequirement(ObjectType.Wheel, 2),
			new IngredientRequirement(ObjectType.FrameBox, 1),
			new IngredientRequirement(ObjectType.Panel, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ToyHorseCarriage, ingredients291);
		IngredientRequirement[] ingredients292 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.ToyHorseCarriage, 1),
			new IngredientRequirement(ObjectType.MetalPlateCrude, 1),
			new IngredientRequirement(ObjectType.MetalPoleCrude, 1),
			new IngredientRequirement(ObjectType.Rivets, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ToyTrain, ingredients292);
		IngredientRequirement[] ingredients293 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.AnimalLeech, 4),
			new IngredientRequirement(ObjectType.Water, 1),
			new IngredientRequirement(ObjectType.JarClay, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.MedicineLeeches, ingredients293);
		IngredientRequirement[] ingredients294 = new IngredientRequirement[5]
		{
			new IngredientRequirement(ObjectType.FlowerBunch01, 4),
			new IngredientRequirement(ObjectType.FlowerBunch04, 4),
			new IngredientRequirement(ObjectType.FlowerBunch07, 4),
			new IngredientRequirement(ObjectType.MedicineLeeches, 1),
			new IngredientRequirement(ObjectType.Water, 10)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.MedicineFlowers, ingredients294);
		IngredientRequirement[] ingredients295 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.MedicineFlowers, 1),
			new IngredientRequirement(ObjectType.Rock, 10),
			new IngredientRequirement(ObjectType.LargeBowlClay, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.MedicinePills, ingredients295);
		IngredientRequirement[] ingredients296 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Paper, 4),
			new IngredientRequirement(ObjectType.Ink, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.EducationBook1, ingredients296);
		IngredientRequirement[] ingredients297 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.EducationBook1, 2),
			new IngredientRequirement(ObjectType.Paper, 2),
			new IngredientRequirement(ObjectType.Ink, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.EducationEncyclopedia, ingredients297);
		IngredientRequirement[] ingredients298 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Log, 4),
			new IngredientRequirement(ObjectType.Water, 10)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Paper, ingredients298);
		IngredientRequirement[] ingredients299 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Charcoal, 4),
			new IngredientRequirement(ObjectType.Water, 4),
			new IngredientRequirement(ObjectType.JarClay, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Ink, ingredients299);
		IngredientRequirement[] ingredients300 = new IngredientRequirement[5]
		{
			new IngredientRequirement(ObjectType.Canvas, 1),
			new IngredientRequirement(ObjectType.PaintRed, 5),
			new IngredientRequirement(ObjectType.PaintYellow, 3),
			new IngredientRequirement(ObjectType.PaintBlue, 1),
			new IngredientRequirement(ObjectType.HeartAny, 3)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ArtPortrait, ingredients300);
		IngredientRequirement[] ingredients301 = new IngredientRequirement[5]
		{
			new IngredientRequirement(ObjectType.Canvas, 1),
			new IngredientRequirement(ObjectType.PaintRed, 1),
			new IngredientRequirement(ObjectType.PaintYellow, 5),
			new IngredientRequirement(ObjectType.PaintBlue, 3),
			new IngredientRequirement(ObjectType.HeartAny, 3)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ArtStillLife, ingredients301);
		IngredientRequirement[] ingredients302 = new IngredientRequirement[5]
		{
			new IngredientRequirement(ObjectType.Canvas, 1),
			new IngredientRequirement(ObjectType.PaintRed, 3),
			new IngredientRequirement(ObjectType.PaintYellow, 1),
			new IngredientRequirement(ObjectType.PaintBlue, 5),
			new IngredientRequirement(ObjectType.HeartAny, 3)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.ArtAbstract, ingredients302);
		IngredientRequirement[] ingredients303 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.FrameSquare, 1),
			new IngredientRequirement(ObjectType.BullrushesCloth, 4)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.Canvas, ingredients303);
		IngredientRequirement[] ingredients304 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.ToolBucketMetal, 1),
			new IngredientRequirement(ObjectType.FlowerBunch05, 10),
			new IngredientRequirement(ObjectType.Water, 5),
			new IngredientRequirement(ObjectType.Egg, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.PaintRed, ingredients304);
		IngredientRequirement[] ingredients305 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.ToolBucketMetal, 1),
			new IngredientRequirement(ObjectType.FlowerBunch06, 10),
			new IngredientRequirement(ObjectType.Water, 5),
			new IngredientRequirement(ObjectType.Egg, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.PaintYellow, ingredients305);
		IngredientRequirement[] ingredients306 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.ToolBucketMetal, 1),
			new IngredientRequirement(ObjectType.FlowerBunch03, 10),
			new IngredientRequirement(ObjectType.Water, 5),
			new IngredientRequirement(ObjectType.Egg, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.PaintBlue, ingredients306);
		IngredientRequirement[] ingredients307 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Plank, 2),
			new IngredientRequirement(ObjectType.Pole, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerFrameMk1, ingredients307);
		IngredientRequirement[] ingredients308 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Plank, 1),
			new IngredientRequirement(ObjectType.TreeSeed, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerHeadMk1, ingredients308);
		IngredientRequirement[] ingredients309 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Log, 1),
			new IngredientRequirement(ObjectType.Plank, 1),
			new IngredientRequirement(ObjectType.Pole, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerDriveMk1, ingredients309);
		IngredientRequirement[] ingredients310 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Plank, 2),
			new IngredientRequirement(ObjectType.Pole, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerFrameMk1Variant1, ingredients310);
		IngredientRequirement[] ingredients311 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Plank, 1),
			new IngredientRequirement(ObjectType.TreeSeed, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerHeadMk1Variant1, ingredients311);
		IngredientRequirement[] ingredients312 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Log, 1),
			new IngredientRequirement(ObjectType.Plank, 1),
			new IngredientRequirement(ObjectType.Pole, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerDriveMk1Variant1, ingredients312);
		IngredientRequirement[] ingredients313 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Plank, 2),
			new IngredientRequirement(ObjectType.Pole, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerFrameMk1Variant2, ingredients313);
		IngredientRequirement[] ingredients314 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Plank, 1),
			new IngredientRequirement(ObjectType.TreeSeed, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerHeadMk1Variant2, ingredients314);
		IngredientRequirement[] ingredients315 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Log, 1),
			new IngredientRequirement(ObjectType.Plank, 1),
			new IngredientRequirement(ObjectType.Pole, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerDriveMk1Variant2, ingredients315);
		IngredientRequirement[] ingredients316 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Plank, 2),
			new IngredientRequirement(ObjectType.Pole, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerFrameMk1Variant3, ingredients316);
		IngredientRequirement[] ingredients317 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Plank, 1),
			new IngredientRequirement(ObjectType.TreeSeed, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerHeadMk1Variant3, ingredients317);
		IngredientRequirement[] ingredients318 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Log, 1),
			new IngredientRequirement(ObjectType.Plank, 1),
			new IngredientRequirement(ObjectType.Pole, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerDriveMk1Variant3, ingredients318);
		IngredientRequirement[] ingredients319 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Plank, 2),
			new IngredientRequirement(ObjectType.Pole, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerFrameMk1Variant4, ingredients319);
		IngredientRequirement[] ingredients320 = new IngredientRequirement[2]
		{
			new IngredientRequirement(ObjectType.Plank, 1),
			new IngredientRequirement(ObjectType.TreeSeed, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerHeadMk1Variant4, ingredients320);
		IngredientRequirement[] ingredients321 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Log, 1),
			new IngredientRequirement(ObjectType.Plank, 1),
			new IngredientRequirement(ObjectType.Pole, 1)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerDriveMk1Variant4, ingredients321);
		IngredientRequirement[] ingredients322 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Panel, 2),
			new IngredientRequirement(ObjectType.Pole, 2),
			new IngredientRequirement(ObjectType.FixingPeg, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerFrameMk2, ingredients322);
		IngredientRequirement[] ingredients323 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Plank, 2),
			new IngredientRequirement(ObjectType.Pole, 2),
			new IngredientRequirement(ObjectType.TreeSeed, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerHeadMk2, ingredients323);
		IngredientRequirement[] ingredients324 = new IngredientRequirement[5]
		{
			new IngredientRequirement(ObjectType.Pole, 1),
			new IngredientRequirement(ObjectType.Panel, 1),
			new IngredientRequirement(ObjectType.FixingPeg, 1),
			new IngredientRequirement(ObjectType.Cog, 1),
			new IngredientRequirement(ObjectType.WheelCrude, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerDriveMk2, ingredients324);
		IngredientRequirement[] ingredients325 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Panel, 2),
			new IngredientRequirement(ObjectType.Pole, 2),
			new IngredientRequirement(ObjectType.FixingPeg, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerFrameMk2Variant1, ingredients325);
		IngredientRequirement[] ingredients326 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Plank, 2),
			new IngredientRequirement(ObjectType.Pole, 2),
			new IngredientRequirement(ObjectType.TreeSeed, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerHeadMk2Variant1, ingredients326);
		IngredientRequirement[] ingredients327 = new IngredientRequirement[5]
		{
			new IngredientRequirement(ObjectType.Pole, 1),
			new IngredientRequirement(ObjectType.Panel, 1),
			new IngredientRequirement(ObjectType.FixingPeg, 1),
			new IngredientRequirement(ObjectType.Cog, 1),
			new IngredientRequirement(ObjectType.WheelCrude, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerDriveMk2Variant1, ingredients327);
		IngredientRequirement[] ingredients328 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Panel, 2),
			new IngredientRequirement(ObjectType.Pole, 2),
			new IngredientRequirement(ObjectType.FixingPeg, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerFrameMk2Variant2, ingredients328);
		IngredientRequirement[] ingredients329 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Plank, 2),
			new IngredientRequirement(ObjectType.Pole, 2),
			new IngredientRequirement(ObjectType.TreeSeed, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerHeadMk2Variant2, ingredients329);
		IngredientRequirement[] ingredients330 = new IngredientRequirement[5]
		{
			new IngredientRequirement(ObjectType.Pole, 1),
			new IngredientRequirement(ObjectType.Panel, 1),
			new IngredientRequirement(ObjectType.FixingPeg, 1),
			new IngredientRequirement(ObjectType.Cog, 1),
			new IngredientRequirement(ObjectType.WheelCrude, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerDriveMk2Variant2, ingredients330);
		IngredientRequirement[] ingredients331 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Panel, 2),
			new IngredientRequirement(ObjectType.Pole, 2),
			new IngredientRequirement(ObjectType.FixingPeg, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerFrameMk2Variant3, ingredients331);
		IngredientRequirement[] ingredients332 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Plank, 2),
			new IngredientRequirement(ObjectType.Pole, 2),
			new IngredientRequirement(ObjectType.TreeSeed, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerHeadMk2Variant3, ingredients332);
		IngredientRequirement[] ingredients333 = new IngredientRequirement[5]
		{
			new IngredientRequirement(ObjectType.Pole, 1),
			new IngredientRequirement(ObjectType.Panel, 1),
			new IngredientRequirement(ObjectType.FixingPeg, 1),
			new IngredientRequirement(ObjectType.Cog, 1),
			new IngredientRequirement(ObjectType.WheelCrude, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerDriveMk2Variant3, ingredients333);
		IngredientRequirement[] ingredients334 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Panel, 2),
			new IngredientRequirement(ObjectType.Pole, 2),
			new IngredientRequirement(ObjectType.FixingPeg, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerFrameMk2Variant4, ingredients334);
		IngredientRequirement[] ingredients335 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.Plank, 2),
			new IngredientRequirement(ObjectType.Pole, 2),
			new IngredientRequirement(ObjectType.TreeSeed, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerHeadMk2Variant4, ingredients335);
		IngredientRequirement[] ingredients336 = new IngredientRequirement[5]
		{
			new IngredientRequirement(ObjectType.Pole, 1),
			new IngredientRequirement(ObjectType.Panel, 1),
			new IngredientRequirement(ObjectType.FixingPeg, 1),
			new IngredientRequirement(ObjectType.Cog, 1),
			new IngredientRequirement(ObjectType.WheelCrude, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerDriveMk2Variant4, ingredients336);
		IngredientRequirement[] ingredients337 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.MetalPlateCrude, 2),
			new IngredientRequirement(ObjectType.MetalPoleCrude, 2),
			new IngredientRequirement(ObjectType.Rivets, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerFrameMk3, ingredients337);
		IngredientRequirement[] ingredients338 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.MetalPlateCrude, 1),
			new IngredientRequirement(ObjectType.MetalPoleCrude, 1),
			new IngredientRequirement(ObjectType.Coconut, 1),
			new IngredientRequirement(ObjectType.Rivets, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerHeadMk3, ingredients338);
		IngredientRequirement[] ingredients339 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.MetalPlateCrude, 2),
			new IngredientRequirement(ObjectType.MetalAxle, 2),
			new IngredientRequirement(ObjectType.MetalCog, 2),
			new IngredientRequirement(ObjectType.Rivets, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerDriveMk3, ingredients339);
		IngredientRequirement[] ingredients340 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.MetalPlateCrude, 2),
			new IngredientRequirement(ObjectType.MetalPoleCrude, 2),
			new IngredientRequirement(ObjectType.Rivets, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerFrameMk3Variant1, ingredients340);
		IngredientRequirement[] ingredients341 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.MetalPlateCrude, 1),
			new IngredientRequirement(ObjectType.MetalPoleCrude, 1),
			new IngredientRequirement(ObjectType.Coconut, 1),
			new IngredientRequirement(ObjectType.Rivets, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerHeadMk3Variant1, ingredients341);
		IngredientRequirement[] ingredients342 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.MetalPlateCrude, 2),
			new IngredientRequirement(ObjectType.MetalAxle, 2),
			new IngredientRequirement(ObjectType.MetalCog, 2),
			new IngredientRequirement(ObjectType.Rivets, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerDriveMk3Variant1, ingredients342);
		IngredientRequirement[] ingredients343 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.MetalPlateCrude, 2),
			new IngredientRequirement(ObjectType.MetalPoleCrude, 2),
			new IngredientRequirement(ObjectType.Rivets, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerFrameMk3Variant2, ingredients343);
		IngredientRequirement[] ingredients344 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.MetalPlateCrude, 1),
			new IngredientRequirement(ObjectType.MetalPoleCrude, 1),
			new IngredientRequirement(ObjectType.Coconut, 1),
			new IngredientRequirement(ObjectType.Rivets, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerHeadMk3Variant2, ingredients344);
		IngredientRequirement[] ingredients345 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.MetalPlateCrude, 2),
			new IngredientRequirement(ObjectType.MetalAxle, 2),
			new IngredientRequirement(ObjectType.MetalCog, 2),
			new IngredientRequirement(ObjectType.Rivets, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerDriveMk3Variant2, ingredients345);
		IngredientRequirement[] ingredients346 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.MetalPlateCrude, 2),
			new IngredientRequirement(ObjectType.MetalPoleCrude, 2),
			new IngredientRequirement(ObjectType.Rivets, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerFrameMk3Variant3, ingredients346);
		IngredientRequirement[] ingredients347 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.MetalPlateCrude, 1),
			new IngredientRequirement(ObjectType.MetalPoleCrude, 1),
			new IngredientRequirement(ObjectType.Coconut, 1),
			new IngredientRequirement(ObjectType.Rivets, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerHeadMk3Variant3, ingredients347);
		IngredientRequirement[] ingredients348 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.MetalPlateCrude, 2),
			new IngredientRequirement(ObjectType.MetalAxle, 2),
			new IngredientRequirement(ObjectType.MetalCog, 2),
			new IngredientRequirement(ObjectType.Rivets, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerDriveMk3Variant3, ingredients348);
		IngredientRequirement[] ingredients349 = new IngredientRequirement[3]
		{
			new IngredientRequirement(ObjectType.MetalPlateCrude, 2),
			new IngredientRequirement(ObjectType.MetalPoleCrude, 2),
			new IngredientRequirement(ObjectType.Rivets, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerFrameMk3Variant4, ingredients349);
		IngredientRequirement[] ingredients350 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.MetalPlateCrude, 1),
			new IngredientRequirement(ObjectType.MetalPoleCrude, 1),
			new IngredientRequirement(ObjectType.Coconut, 1),
			new IngredientRequirement(ObjectType.Rivets, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerHeadMk3Variant4, ingredients350);
		IngredientRequirement[] ingredients351 = new IngredientRequirement[4]
		{
			new IngredientRequirement(ObjectType.MetalPlateCrude, 2),
			new IngredientRequirement(ObjectType.MetalAxle, 2),
			new IngredientRequirement(ObjectType.MetalCog, 2),
			new IngredientRequirement(ObjectType.Rivets, 2)
		};
		ObjectTypeList.Instance.SetIngredients(ObjectType.WorkerDriveMk3Variant4, ingredients351);
	}
}
