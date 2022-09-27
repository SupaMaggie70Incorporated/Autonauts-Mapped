using System.Collections.Generic;
using UnityEngine;

public class StorageTypeManager : MonoBehaviour
{
	public class StorageGenericInfo
	{
		public int m_Capacity;

		public StorageGenericInfo(int Capacity)
		{
			m_Capacity = Capacity;
		}
	}

	public static StorageTypeManager Instance;

	public static Dictionary<ObjectType, StorageGenericInfo> m_StorageGenericInformation;

	public static Dictionary<ObjectType, StorageGenericInfo> m_StoragePaletteInformation;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
		}
		else if (this != Instance)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		m_StorageGenericInformation = new Dictionary<ObjectType, StorageGenericInfo>();
		m_StoragePaletteInformation = new Dictionary<ObjectType, StorageGenericInfo>();
		Reset();
	}

	public void Reset()
	{
		m_StorageGenericInformation.Clear();
		m_StoragePaletteInformation.Clear();
		int capacity = 10;
		int capacity2 = 25;
		m_StorageGenericInformation.Add(ObjectType.Wheat, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.GrassCut, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.Turf, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.Straw, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.BullrushesStems, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.Flour, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.Stick, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.Rock, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.Cog, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.CogCrude, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.FixingPeg, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.FrameBox, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.FrameSquare, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.FrameTriangle, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.FrameWindow, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.FrameDoor, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.Panel, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.RockSharp, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.Axle, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WheelCrude, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.Wheel, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.IronOre, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.IronCrude, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.Coal, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.Clay, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.TreeSeed, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.MulberrySeed, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.Coconut, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.Butter, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.Dough, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.DoughGood, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.Pastry, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.NaanRaw, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.Naan, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.Manure, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.CottonThread, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.BullrushesThread, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.MilkPorridge, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.FruitPorridge, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.HoneyPorridge, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.CakeBatter, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.AppleBerryPieRaw, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.AppleBerryPie, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.PumpkinMushroomPieRaw, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.PumpkinMushroomPie, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.Charcoal, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.ToolShovel, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.ToolShovelStone, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.ToolHoe, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.ToolHoeStone, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.ToolAxe, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.ToolAxeStone, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.ToolScythe, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.ToolScytheStone, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.ToolPick, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.ToolPickStone, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.ToolFlailCrude, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.ToolFlail, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.ToolMallet, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.ToolChiselCrude, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.ToolChisel, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.ToolBucketCrude, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.ToolBucket, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.ToolShears, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.ToolWateringCan, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.ToolFishingStick, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.ToolFishingRod, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.ToolFishingRodGood, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.ToolPitchfork, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.ToolTorchCrude, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.ToolDredgerCrude, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.ToolNetCrude, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.ToolNet, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.ToolBlade, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.JarClay, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.JarClayRaw, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.PotClay, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.PotClayRaw, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.Gnome, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.Gnome2, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.Gnome3, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.Gnome4, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.Gnome5, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.Gnome6, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.GnomeRaw, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.ToolBucketMetal, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.ToolBroom, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.LargeBowlClay, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.LargeBowlClayRaw, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatChullo, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatFarmerCap, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatCap, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatFarmer, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatBeret, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatSugegasa, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatFez, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatChef, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatKnittedBeanie, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatAdventurer, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatWig01, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatMushroom, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.TopPoncho, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.TopJumper, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.TopJacket, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.TopBlazer, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.TopTuxedo, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.TopTunic, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.TopDress, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.TopShirt, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.TopShirtTie, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.TopGown, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.TopToga, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.TopRobe, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.TopCoat, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.TopCoatScarf, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.TopSuit, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.TopDungarees, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.TopLumberjack, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatLumberjack, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.TopTShirtShow, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatBaseballShow, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatCrown, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatMortarboard, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatSouwester, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatTree, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatMadHatter, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatCloche, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatAcorn, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatSanta, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatWally, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatParty, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatViking, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatBox, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatTrain, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatSailor, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatMiner, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatFox, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatDinosaur, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatAntlers, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatBunny, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatDuck, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.TopAdventurer, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.TopMac, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.TopPlumber, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.TopTree, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.TopDungareesClown, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.TopJumper02, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.TopApron, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.TopSanta, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.TopWally, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.TopTShirt02, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.TopFox, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.TopDinosaur, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.TopBunny, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.TopDuck, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.DataStorageCrude, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.Fleece, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.Wool, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.FlowerPot, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.FlowerPotRaw, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.FlowerSeeds01, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.FlowerSeeds02, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.FlowerSeeds03, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.FlowerSeeds04, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.FlowerSeeds05, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.FlowerSeeds06, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.FlowerSeeds07, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.FlowerBunch01, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.FlowerBunch02, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.FlowerBunch03, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.FlowerBunch04, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.FlowerBunch05, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.FlowerBunch06, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.FlowerBunch07, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.WeedDug, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.Sign, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.Sign2, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.Sign3, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.Billboard, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.Canoe, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.StringBall, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.FishBait, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.FishSalmon, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.FishCatFish, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.FishMahiMahi, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.FishOrangeRoughy, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.FishPerch, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.FishCarp, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.FishGar, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.FishMonkfish, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.FishMarlin, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.AnimalLeech, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.AnimalSilkworm, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.MushroomDug, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.MushroomHerb, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.MushroomSoup, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.MushroomStew, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.MushroomPieRaw, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.MushroomPie, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.MushroomPuddingRaw, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.MushroomPudding, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.MushroomBurger, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.Porridge, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.BreadCrude, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.Bread, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.BreadButtered, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.BreadPuddingRaw, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.BreadPudding, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.CreamBrioche, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.Berries, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.BerriesSpice, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.BerriesStew, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.BerriesJam, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.BerriesPieRaw, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.BerriesPie, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.BerriesCakeRaw, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.BerriesCake, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.BerryDanish, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.PumpkinSeeds, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.PumpkinRaw, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.PumpkinHerb, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.PumpkinSoup, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.PumpkinStew, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.PumpkinPieRaw, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.PumpkinPie, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.PumpkinCakeRaw, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.PumpkinCake, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.PumpkinBurger, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.Apple, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.AppleSpice, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.AppleStew, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.AppleJam, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.ApplePie, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.ApplePieRaw, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.AppleCake, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.AppleCakeRaw, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.AppleDanish, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.FishRaw, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.FishHerb, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.FishSoup, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.FishStew, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.FishPieRaw, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.FishPie, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.FishCakeRaw, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.FishCake, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.FishBurger, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.Carrot, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.CarrotSalad, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.CarrotStirFry, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.CarrotHoney, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.CarrotCurry, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.CarrotCakeRaw, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.CarrotCake, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.CarrotBurger, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.Pumpkin, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.Scarecrow, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.FlourCrude, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.FolkHeart, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.FolkHeart2, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.FolkHeart3, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.FolkHeart4, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.FolkHeart5, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.FolkHeart6, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.FolkHeart7, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.Egg, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.Fireplace, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.Chimney, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.Millstone, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.Crank, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.MetalCog, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.MetalWheel, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.MetalAxle, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.Flywheel, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.ConnectingRod, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.Piston, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.Boiler, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.Firebox, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.Rivets, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.Blanket, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.Buttons, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.Thread, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.CottonBall, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.CottonLint, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.BullrushesFibre, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.SilkRaw, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.SilkThread, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.Doll, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.JackInTheBox, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.DollHouse, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.Spaceship, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.ToyHorse, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.ToyHorseCart, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.ToyHorseCarriage, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.ToyTrain, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.MedicineLeeches, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.MedicineFlowers, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.MedicinePills, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.EducationBook1, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.EducationEncyclopedia, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.Paper, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.Ink, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.ArtPortrait, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.ArtStillLife, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.ArtAbstract, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.Canvas, new StorageGenericInfo(capacity2));
		m_StorageGenericInformation.Add(ObjectType.PaintRed, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.PaintYellow, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.PaintBlue, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerFrameMk0, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerHeadMk0, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerDriveMk0, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerFrameMk1, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerHeadMk1, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerDriveMk1, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerFrameMk1Variant1, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerHeadMk1Variant1, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerDriveMk1Variant1, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerFrameMk1Variant2, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerHeadMk1Variant2, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerDriveMk1Variant2, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerFrameMk1Variant3, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerHeadMk1Variant3, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerDriveMk1Variant3, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerFrameMk1Variant4, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerHeadMk1Variant4, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerDriveMk1Variant4, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerFrameMk2, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerHeadMk2, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerDriveMk2, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerFrameMk2Variant1, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerHeadMk2Variant1, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerDriveMk2Variant1, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerFrameMk2Variant2, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerHeadMk2Variant2, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerDriveMk2Variant2, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerFrameMk2Variant3, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerHeadMk2Variant3, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerDriveMk2Variant3, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerFrameMk2Variant4, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerHeadMk2Variant4, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerDriveMk2Variant4, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerFrameMk3, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerHeadMk3, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerDriveMk3, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerFrameMk3Variant1, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerHeadMk3Variant1, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerDriveMk3Variant1, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerFrameMk3Variant2, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerHeadMk3Variant2, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerDriveMk3Variant2, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerFrameMk3Variant3, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerHeadMk3Variant3, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerDriveMk3Variant3, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerFrameMk3Variant4, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerHeadMk3Variant4, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerDriveMk3Variant4, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.UpgradePlayerInventoryCrude, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.UpgradePlayerInventoryGood, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.UpgradePlayerInventorySuper, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.UpgradePlayerWhistleCrude, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.UpgradePlayerWhistleGood, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.UpgradePlayerWhistleSuper, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.UpgradePlayerMovementCrude, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.UpgradePlayerMovementGood, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.UpgradePlayerMovementSuper, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.UpgradeWorkerMemoryCrude, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.UpgradeWorkerMemoryGood, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.UpgradeWorkerMemorySuper, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.UpgradeWorkerSearchCrude, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.UpgradeWorkerSearchGood, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.UpgradeWorkerSearchSuper, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.UpgradeWorkerCarryCrude, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.UpgradeWorkerCarryGood, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.UpgradeWorkerCarrySuper, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.UpgradeWorkerMovementCrude, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.UpgradeWorkerMovementGood, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.UpgradeWorkerMovementSuper, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.UpgradeWorkerEnergyCrude, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.UpgradeWorkerEnergyGood, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.UpgradeWorkerEnergySuper, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.UpgradeWorkerInventoryCrude, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.UpgradeWorkerInventoryGood, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.UpgradeWorkerInventorySuper, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.Triangle, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.Castanets, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.Cowbell, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.Guiro, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.Maracas, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.JawHarp, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.Guitar, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatFrog, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatPanda, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatPenguin, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatBearskin, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatCaptain, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatCowboy, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatBoater, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatCatintheHat, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatDrumMajor, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatGat, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatFedora, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatSombrero, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatSmurf, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatTrafficCone, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.HatWig02, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.TopFrog, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.TopPanda, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.TopPenguin, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.TopRoyalGuard, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.TopSuit02, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerDriveROB, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerFrameROB, new StorageGenericInfo(capacity));
		m_StorageGenericInformation.Add(ObjectType.WorkerHeadROB, new StorageGenericInfo(capacity));
		foreach (ModCustom modCustomClass in ModManager.Instance.ModCustomClasses)
		{
			if (!modCustomClass.GetStackable())
			{
				continue;
			}
			foreach (KeyValuePair<ObjectType, string> modIDOriginal in modCustomClass.ModIDOriginals)
			{
				m_StorageGenericInformation.Add(modIDOriginal.Key, new StorageGenericInfo(capacity));
			}
		}
		m_StoragePaletteInformation.Add(ObjectType.Log, new StorageGenericInfo(capacity2));
		m_StoragePaletteInformation.Add(ObjectType.Plank, new StorageGenericInfo(capacity2));
		m_StoragePaletteInformation.Add(ObjectType.Pole, new StorageGenericInfo(capacity2));
		m_StoragePaletteInformation.Add(ObjectType.WoodenBeam, new StorageGenericInfo(capacity2));
		m_StoragePaletteInformation.Add(ObjectType.MetalGirder, new StorageGenericInfo(capacity2));
		m_StoragePaletteInformation.Add(ObjectType.MetalPoleCrude, new StorageGenericInfo(capacity2));
		m_StoragePaletteInformation.Add(ObjectType.MetalPlateCrude, new StorageGenericInfo(capacity2));
		m_StoragePaletteInformation.Add(ObjectType.StoneBlock, new StorageGenericInfo(capacity));
		m_StoragePaletteInformation.Add(ObjectType.StoneBlockCrude, new StorageGenericInfo(capacity));
		m_StoragePaletteInformation.Add(ObjectType.HayBale, new StorageGenericInfo(capacity2));
		m_StoragePaletteInformation.Add(ObjectType.BricksCrude, new StorageGenericInfo(27));
		m_StoragePaletteInformation.Add(ObjectType.BricksCrudeRaw, new StorageGenericInfo(27));
		m_StoragePaletteInformation.Add(ObjectType.RoofTiles, new StorageGenericInfo(27));
		m_StoragePaletteInformation.Add(ObjectType.RoofTilesRaw, new StorageGenericInfo(27));
		m_StoragePaletteInformation.Add(ObjectType.Blanket, new StorageGenericInfo(capacity2));
		m_StoragePaletteInformation.Add(ObjectType.CottonCloth, new StorageGenericInfo(capacity2));
		m_StoragePaletteInformation.Add(ObjectType.BullrushesCloth, new StorageGenericInfo(capacity2));
		m_StoragePaletteInformation.Add(ObjectType.SilkCloth, new StorageGenericInfo(capacity2));
	}

	public float GetFoodStored()
	{
		float num = 0f;
		foreach (KeyValuePair<BaseClass, int> item in CollectionManager.Instance.GetCollection("Storage"))
		{
			Storage component = item.Key.GetComponent<Storage>();
			if ((bool)component)
			{
				float foodEnergy = Food.GetFoodEnergy(component.m_ObjectType);
				if (foodEnergy > 0f)
				{
					int stored = component.GetStored();
					num += foodEnergy * (float)stored;
				}
			}
		}
		return num;
	}

	public int GetStored(ObjectType NewObjectType)
	{
		int num = 0;
		foreach (KeyValuePair<BaseClass, int> item in CollectionManager.Instance.GetCollection("Storage"))
		{
			Storage component = item.Key.GetComponent<Storage>();
			if ((bool)component && component.m_ObjectType == NewObjectType)
			{
				int stored = component.GetStored();
				num += stored;
			}
		}
		return num;
	}
}
