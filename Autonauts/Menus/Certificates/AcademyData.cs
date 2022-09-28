using System.Collections.Generic;

public class AcademyData
{
	public List<CertificateInfo> m_CertificateInfos;

	private static int m_QuickTestCount = 1;

	public CertificateInfo GetInfoFromQuestID(Quest.ID NewID)
	{
		foreach (CertificateInfo certificateInfo in m_CertificateInfos)
		{
			if (certificateInfo.m_ID == NewID)
			{
				return certificateInfo;
			}
		}
		return null;
	}

	public Quest.ID GetQuestFromUnlockedObjectType(ObjectType NewType)
	{
		foreach (CertificateInfo certificateInfo in m_CertificateInfos)
		{
			if (certificateInfo.m_UnlockedObjects.Contains(NewType))
			{
				return certificateInfo.m_ID;
			}
		}
		return Quest.ID.Total;
	}

	public void ConvertToQuest(int Index)
	{
		CertificateInfo certificateInfo = m_CertificateInfos[Index];
		Quest quest = new Quest();
		quest.m_Simple = false;
		foreach (QuestEvent @event in certificateInfo.m_Events)
		{
			quest.AddEvent(@event.m_Type, @event.m_BotOnly, @event.m_ExtraData, @event.m_Required);
			if (@event.m_Description != "")
			{
				certificateInfo.m_Tutorial = true;
			}
		}
		foreach (ObjectType unlockedObject in certificateInfo.m_UnlockedObjects)
		{
			if (!ObjectTypeList.Instance.GetIsBuilding(unlockedObject))
			{
				quest.AddObjectUnlocked(unlockedObject);
			}
			else
			{
				quest.AddBuildingUnlocked(unlockedObject);
			}
		}
		foreach (Quest.ID unlockedQuest in certificateInfo.m_UnlockedQuests)
		{
			quest.AddQuestUnlocked(unlockedQuest);
		}
		foreach (Quest.ID researchID in certificateInfo.m_ResearchIDs)
		{
			quest.AddRequiredQuest(researchID);
		}
		string text = certificateInfo.m_ID.ToString();
		CeremonyManager.CeremonyType newCeremonyType = CeremonyManager.CeremonyType.CertificateEnded;
		quest.SetInfo(certificateInfo.m_ID, Quest.Category.Total, text, text, text + "Desc", null, null, newCeremonyType, Quest.Type.Academy);
		quest.m_IconName = "Academy/" + certificateInfo.m_ID;
		quest.m_Colour = certificateInfo.m_Colour;
		QuestData.Instance.AddQuest(certificateInfo.m_ID, quest);
	}

	public void ConvertToQuests()
	{
		for (int i = 0; i < m_CertificateInfos.Count; i++)
		{
			ConvertToQuest(i);
		}
	}

	private CertificateInfo AddCertificate(Quest.ID NewID, int HexColour)
	{
		CertificateInfo certificateInfo = new CertificateInfo(NewID, GeneralUtils.ColorFromHex(HexColour));
		m_CertificateInfos.Add(certificateInfo);
		return certificateInfo;
	}

	public AcademyData()
	{
		m_CertificateInfos = new List<CertificateInfo>();
		CertificateInfo certificateInfo = AddCertificate(Quest.ID.AcademyBasics, 16711790);
		certificateInfo.AddEvent(QuestEvent.Type.CompleteTutorial, BotOnly: false, null, 1);
		certificateInfo.AddUnlockedObject(ObjectType.ToolPickStone);
		certificateInfo.AddUnlockedObject(ObjectType.HatMortarboard);
		CertificateInfo certificateInfo2 = AddCertificate(Quest.ID.AcademyForestry, 11992832);
		certificateInfo2.AddEvent(QuestEvent.Type.ChopTree, BotOnly: false, null, 3);
		certificateInfo2.AddEvent(QuestEvent.Type.Dig, BotOnly: false, null, 3);
		certificateInfo2.AddEvent(QuestEvent.Type.PlantTreeSeed, BotOnly: false, null, 3);
		certificateInfo2.AddUnlockedObject(ObjectType.ChoppingBlock);
		certificateInfo2.AddUnlockedObject(ObjectType.HatParty);
		CertificateInfo certificateInfo3 = AddCertificate(Quest.ID.AcademyLumber2, 11305000);
		certificateInfo3.m_LessonID = Quest.ID.TutorialScripting;
		certificateInfo3.AddEvent(QuestEvent.Type.Build, BotOnly: false, ObjectType.ChoppingBlock, 2);
		certificateInfo3.AddEvent(QuestEvent.Type.Store, BotOnly: false, ObjectType.Log, 25);
		certificateInfo3.AddEvent(QuestEvent.Type.ChopLog, BotOnly: false, null, 25);
		certificateInfo3.AddEvent(QuestEvent.Type.Store, BotOnly: false, ObjectType.Plank, 25);
		certificateInfo3.AddEvent(QuestEvent.Type.ChopPlank, BotOnly: false, null, 25);
		certificateInfo3.AddEvent(QuestEvent.Type.Store, BotOnly: false, ObjectType.Pole, 25);
		certificateInfo3.AddUnlockedObject(ObjectType.HatTree);
		certificateInfo3.AddUnlockedObject(ObjectType.TopTree);
		CertificateInfo certificateInfo4 = AddCertificate(Quest.ID.AcademyForestry2, 11992832);
		certificateInfo4.AddEvent(QuestEvent.Type.ChopTree, BotOnly: false, null, 30);
		certificateInfo4.AddEvent(QuestEvent.Type.Dig, BotOnly: false, null, 30);
		certificateInfo4.AddEvent(QuestEvent.Type.PlantTreeSeed, BotOnly: false, null, 30);
		certificateInfo4.AddEvent(QuestEvent.Type.GrowTree, BotOnly: false, null, 30);
		certificateInfo4.AddUnlockedObject(ObjectType.HatLumberjack);
		certificateInfo4.AddUnlockedObject(ObjectType.TopLumberjack);
		CertificateInfo certificateInfo5 = AddCertificate(Quest.ID.AcademyMining, 12632272);
		certificateInfo5.m_LessonID = Quest.ID.TutorialScripting4;
		certificateInfo5.AddEvent(QuestEvent.Type.MineStone, BotOnly: false, null, 25);
		certificateInfo5.AddEvent(QuestEvent.Type.Store, BotOnly: false, ObjectType.Rock, 25);
		certificateInfo5.AddUnlockedObject(ObjectType.HatSouwester);
		certificateInfo5.AddUnlockedObject(ObjectType.TopMac);
		CertificateInfo certificateInfo6 = AddCertificate(Quest.ID.AcademyTools, 13805869);
		certificateInfo6.AddEvent(QuestEvent.Type.Make, BotOnly: false, ObjectType.ToolAxeStone, 10);
		certificateInfo6.AddEvent(QuestEvent.Type.Make, BotOnly: false, ObjectType.ToolShovelStone, 10);
		certificateInfo6.AddEvent(QuestEvent.Type.Make, BotOnly: false, ObjectType.ToolPickStone, 10);
		certificateInfo6.AddUnlockedObject(ObjectType.TopPlumber);
		CertificateInfo certificateInfo7 = AddCertificate(Quest.ID.AcademyRobotics, 12120304);
		certificateInfo7.AddEvent(QuestEvent.Type.Make, BotOnly: false, ObjectType.Worker, 10);
		certificateInfo7.AddEvent(QuestEvent.Type.BotTeach, BotOnly: false, null, 10);
		certificateInfo7.AddEvent(QuestEvent.Type.Group3Bots, BotOnly: false, null, 3);
		certificateInfo7.AddUnlockedObject(ObjectType.CogCrude);
		certificateInfo7.AddUnlockedObject(ObjectType.WheelCrude);
		CertificateInfo certificateInfo8 = AddCertificate(Quest.ID.AcademyFarmingFruit, 11665663);
		certificateInfo8.m_LessonID = Quest.ID.TutorialBerries;
		certificateInfo8.AddEvent(QuestEvent.Type.BashBush, BotOnly: false, null, 20);
		certificateInfo8.AddEvent(QuestEvent.Type.PlantBerries, BotOnly: false, null, 20);
		certificateInfo8.AddEvent(QuestEvent.Type.Store, BotOnly: false, ObjectType.Berries, 20);
		certificateInfo8.AddUnlockedObject(ObjectType.TopAdventurer);
		CertificateInfo certificateInfo9 = AddCertificate(Quest.ID.AcademyFarmingMushrooms, 16263264);
		certificateInfo9.m_LessonID = Quest.ID.TutorialMushrooms;
		certificateInfo9.AddEvent(QuestEvent.Type.DigMushroom, BotOnly: false, null, 20);
		certificateInfo9.AddEvent(QuestEvent.Type.PlantMushroom, BotOnly: false, null, 20);
		certificateInfo9.AddEvent(QuestEvent.Type.Store, BotOnly: false, ObjectType.MushroomDug, 20);
		certificateInfo9.AddUnlockedObject(ObjectType.HatMushroom);
		CertificateInfo certificateInfo10 = AddCertificate(Quest.ID.AcademyColonisation, 16711790);
		certificateInfo10.AddEvent(QuestEvent.Type.Build, BotOnly: false, ObjectType.FolkSeedPod, 1);
		certificateInfo10.AddEvent(QuestEvent.Type.Build, BotOnly: false, ObjectType.FolkSeedRehydrator, 1);
		certificateInfo10.AddEvent(QuestEvent.Type.Make, BotOnly: false, ObjectType.Folk, 3);
		certificateInfo10.AddEvent(QuestEvent.Type.FeedFolk, BotOnly: false, null, 6);
		certificateInfo10.AddEvent(QuestEvent.Type.Make, BotOnly: false, ObjectType.FolkHeart, 30);
		certificateInfo10.AddUnlockedObject(ObjectType.Wardrobe);
		certificateInfo10.AddUnlockedObject(ObjectType.UpgradePlayerMovementCrude);
		certificateInfo10.AddUnlockedObject(ObjectType.Sign);
		certificateInfo10.AddUnlockedObject(ObjectType.Sign2);
		certificateInfo10.AddUnlockedObject(ObjectType.HatBaseballShow);
		certificateInfo10.AddUnlockedObject(ObjectType.TopTShirtShow);
		CertificateInfo certificateInfo11 = AddCertificate(Quest.ID.AcademyScience, 10584063);
		certificateInfo11.AddEvent(QuestEvent.Type.Build, BotOnly: false, ObjectType.ResearchStationCrude, 1);
		certificateInfo11.AddEvent(QuestEvent.Type.CompleteResearch, BotOnly: false, null, 12);
		certificateInfo11.AddUnlockedObject(ObjectType.Billboard);
		certificateInfo11.AddUnlockedObject(ObjectType.HatWally);
		certificateInfo11.AddUnlockedObject(ObjectType.TopWally);
		CertificateInfo certificateInfo12 = AddCertificate(Quest.ID.AcademyFarmingFruit2, 16742263);
		certificateInfo12.AddEvent(QuestEvent.Type.BashAppleTree, BotOnly: false, null, 20);
		certificateInfo12.AddEvent(QuestEvent.Type.PlantApple, BotOnly: false, null, 20);
		certificateInfo12.AddEvent(QuestEvent.Type.Store, BotOnly: false, ObjectType.Apple, 20);
		certificateInfo12.AddUnlockedObject(ObjectType.HatMadHatter);
		certificateInfo12.AddUnlockedObject(ObjectType.TopDungareesClown);
		CertificateInfo certificateInfo13 = AddCertificate(Quest.ID.AcademyFarmingVegetables, 16760832);
		certificateInfo13.AddEvent(QuestEvent.Type.ScythePumpkin, BotOnly: false, null, 20);
		certificateInfo13.AddEvent(QuestEvent.Type.PlantPumpkinSeeds, BotOnly: false, null, 20);
		certificateInfo13.AddEvent(QuestEvent.Type.Store, BotOnly: false, ObjectType.Pumpkin, 20);
		certificateInfo13.AddUnlockedObject(ObjectType.HatCap);
		CertificateInfo certificateInfo14 = AddCertificate(Quest.ID.AcademyFarmingVegetables2, 16760832);
		certificateInfo14.AddResearchID(Quest.ID.ResearchPlantBreedingCrude);
		certificateInfo14.AddEvent(QuestEvent.Type.DigCarrot, BotOnly: false, null, 20);
		certificateInfo14.AddEvent(QuestEvent.Type.PlantCarrotSeed, BotOnly: false, null, 20);
		certificateInfo14.AddEvent(QuestEvent.Type.Store, BotOnly: false, ObjectType.Carrot, 20);
		certificateInfo14.AddUnlockedObject(ObjectType.HatViking);
		CertificateInfo certificateInfo15 = AddCertificate(Quest.ID.AcademyFarmingFruit3, 16742263);
		certificateInfo15.AddResearchID(Quest.ID.ResearchPlantBreedingCrude);
		certificateInfo15.AddEvent(QuestEvent.Type.BashCoconutTree, BotOnly: false, null, 20);
		certificateInfo15.AddEvent(QuestEvent.Type.PlantCoconut, BotOnly: false, null, 20);
		certificateInfo15.AddEvent(QuestEvent.Type.Store, BotOnly: false, ObjectType.Coconut, 20);
		certificateInfo15.AddUnlockedObject(ObjectType.HatDuck);
		certificateInfo15.AddUnlockedObject(ObjectType.TopDuck);
		CertificateInfo certificateInfo16 = AddCertificate(Quest.ID.AcademyColonisation2, 16711790);
		certificateInfo16.AddEvent(QuestEvent.Type.MakeFolkHoused, BotOnly: false, null, 10);
		certificateInfo16.AddEvent(QuestEvent.Type.Make, BotOnly: false, ObjectType.FolkHeart2, 100);
		certificateInfo16.AddResearchID(Quest.ID.ResearchShelterCrude);
		certificateInfo16.AddUnlockedObject(ObjectType.StoneHeads);
		certificateInfo16.AddUnlockedObject(ObjectType.WorkerHeadMk1Variant1);
		certificateInfo16.AddUnlockedObject(ObjectType.WorkerFrameMk1Variant1);
		certificateInfo16.AddUnlockedObject(ObjectType.WorkerDriveMk1Variant1);
		certificateInfo16.AddUnlockedObject(ObjectType.HatSanta);
		certificateInfo16.AddUnlockedObject(ObjectType.TopSanta);
		CertificateInfo certificateInfo17 = AddCertificate(Quest.ID.AcademyPottery, 13651968);
		certificateInfo17.AddEvent(QuestEvent.Type.Build, BotOnly: false, ObjectType.ClayStationCrude, 1);
		certificateInfo17.AddEvent(QuestEvent.Type.Make, BotOnly: false, ObjectType.PotClay, 10);
		certificateInfo17.AddResearchID(Quest.ID.ResearchPotteryCrude);
		certificateInfo17.AddUnlockedObject(ObjectType.HatBeret);
		CertificateInfo certificateInfo18 = AddCertificate(Quest.ID.AcademyCooking, 3188991);
		certificateInfo18.AddEvent(QuestEvent.Type.Build, BotOnly: false, ObjectType.PotCrude, 1);
		certificateInfo18.AddEvent(QuestEvent.Type.Build, BotOnly: false, ObjectType.CookingPotCrude, 1);
		certificateInfo18.AddEvent(QuestEvent.Type.ForageFood, BotOnly: false, null, 30);
		certificateInfo18.AddEvent(QuestEvent.Type.StoreFood, BotOnly: false, null, 30);
		certificateInfo18.AddResearchID(Quest.ID.ResearchCookingCrude);
		certificateInfo18.AddUnlockedObject(ObjectType.HatChef);
		certificateInfo18.AddUnlockedObject(ObjectType.TopApron);
		CertificateInfo certificateInfo19 = AddCertificate(Quest.ID.AcademyBaking, 16757375);
		certificateInfo19.AddEvent(QuestEvent.Type.Build, BotOnly: false, ObjectType.OvenCrude, 1);
		certificateInfo19.AddEvent(QuestEvent.Type.Make, BotOnly: false, ObjectType.BreadCrude, 30);
		certificateInfo19.AddEvent(QuestEvent.Type.Store, BotOnly: false, ObjectType.BreadCrude, 30);
		certificateInfo19.AddResearchID(Quest.ID.ResearchCookingBaking);
		certificateInfo19.AddUnlockedObject(ObjectType.HatWig01);
		certificateInfo19.AddUnlockedObject(ObjectType.TopTShirt02);
		CertificateInfo certificateInfo20 = AddCertificate(Quest.ID.AcademyFarmingCereal, 14735500);
		certificateInfo20.AddEvent(QuestEvent.Type.Hoe, BotOnly: false, null, 30);
		certificateInfo20.AddEvent(QuestEvent.Type.PlantWheat, BotOnly: false, null, 50);
		certificateInfo20.AddEvent(QuestEvent.Type.GrowWheat, BotOnly: false, null, 100);
		certificateInfo20.AddEvent(QuestEvent.Type.ScytheWheat, BotOnly: false, null, 30);
		certificateInfo20.AddEvent(QuestEvent.Type.ThreshWheat, BotOnly: false, null, 30);
		certificateInfo20.AddEvent(QuestEvent.Type.Store, BotOnly: false, ObjectType.WheatSeed, 50);
		certificateInfo20.AddResearchID(Quest.ID.ResearchFarmingCrude);
		certificateInfo20.AddUnlockedObject(ObjectType.Scarecrow);
		certificateInfo20.AddUnlockedObject(ObjectType.ToolWateringCan);
		certificateInfo20.AddUnlockedObject(ObjectType.TopDungarees);
		CertificateInfo certificateInfo21 = AddCertificate(Quest.ID.AcademyFishing, 5308320);
		certificateInfo21.AddEvent(QuestEvent.Type.Make, BotOnly: false, ObjectType.ToolFishingRod, 1);
		certificateInfo21.AddEvent(QuestEvent.Type.CatchFish, BotOnly: false, null, 30);
		certificateInfo21.AddResearchID(Quest.ID.ResearchFibre);
		certificateInfo21.AddUnlockedObject(ObjectType.Canoe);
		certificateInfo21.AddUnlockedObject(ObjectType.HatSailor);
		certificateInfo21.AddUnlockedObject(ObjectType.Aquarium);
		CertificateInfo certificateInfo22 = AddCertificate(Quest.ID.AcademyFarmingDairy, 16771144);
		certificateInfo22.AddEvent(QuestEvent.Type.PenCows, BotOnly: false, null, 10);
		certificateInfo22.AddEvent(QuestEvent.Type.MilkCow, BotOnly: false, null, 50);
		certificateInfo22.AddEvent(QuestEvent.Type.Store, BotOnly: false, ObjectType.Milk, 50);
		certificateInfo22.AddResearchID(Quest.ID.ResearchFarmingLivestock);
		certificateInfo22.AddUnlockedObject(ObjectType.WorkerHeadMk1Variant2);
		certificateInfo22.AddUnlockedObject(ObjectType.WorkerFrameMk1Variant2);
		certificateInfo22.AddUnlockedObject(ObjectType.WorkerDriveMk1Variant2);
		CertificateInfo certificateInfo23 = AddCertificate(Quest.ID.AcademyFarmingSheep, 16777215);
		certificateInfo23.AddEvent(QuestEvent.Type.PenSheep, BotOnly: false, null, 10);
		certificateInfo23.AddEvent(QuestEvent.Type.ShearSheep, BotOnly: false, null, 50);
		certificateInfo23.AddEvent(QuestEvent.Type.Store, BotOnly: false, ObjectType.Fleece, 50);
		certificateInfo23.AddResearchID(Quest.ID.ResearchFarmingLivestock);
		certificateInfo23.AddUnlockedObject(ObjectType.HatFarmerCap);
		certificateInfo23.AddUnlockedObject(ObjectType.WorkerHeadMk1Variant3);
		certificateInfo23.AddUnlockedObject(ObjectType.WorkerFrameMk1Variant3);
		certificateInfo23.AddUnlockedObject(ObjectType.WorkerDriveMk1Variant3);
		CertificateInfo certificateInfo24 = AddCertificate(Quest.ID.AcademyForestry3, 11992832);
		certificateInfo24.AddEvent(QuestEvent.Type.PlantSeedling, BotOnly: false, null, 30);
		certificateInfo24.AddResearchID(Quest.ID.ResearchForestry);
		certificateInfo24.AddUnlockedObject(ObjectType.HatAcorn);
		certificateInfo24.AddUnlockedObject(ObjectType.TopJumper02);
		CertificateInfo certificateInfo25 = AddCertificate(Quest.ID.AcademyFarmingPoultry, 16772829);
		certificateInfo25.AddEvent(QuestEvent.Type.PenChooks, BotOnly: false, null, 10);
		certificateInfo25.AddEvent(QuestEvent.Type.Build, BotOnly: false, ObjectType.ChickenCoop, 1);
		certificateInfo25.AddEvent(QuestEvent.Type.FeedChicken, BotOnly: false, null, 50);
		certificateInfo25.AddEvent(QuestEvent.Type.ChickenCoopMakeEgg, BotOnly: false, null, 50);
		certificateInfo25.AddEvent(QuestEvent.Type.Store, BotOnly: false, ObjectType.Egg, 50);
		certificateInfo25.AddResearchID(Quest.ID.ResearchFarmingPoultry);
		certificateInfo25.AddUnlockedObject(ObjectType.HatAdventurer);
		certificateInfo25.AddUnlockedObject(ObjectType.WorkerHeadMk1Variant4);
		certificateInfo25.AddUnlockedObject(ObjectType.WorkerFrameMk1Variant4);
		certificateInfo25.AddUnlockedObject(ObjectType.WorkerDriveMk1Variant4);
		CertificateInfo certificateInfo26 = AddCertificate(Quest.ID.AcademyBeekeeping, 16487168);
		certificateInfo26.AddEvent(QuestEvent.Type.Build, BotOnly: false, ObjectType.StorageBeehiveCrude, 5);
		certificateInfo26.AddEvent(QuestEvent.Type.BeeMakesHoney, BotOnly: false, null, 50);
		certificateInfo26.AddEvent(QuestEvent.Type.Store, BotOnly: false, ObjectType.Honey, 50);
		certificateInfo26.AddResearchID(Quest.ID.ResearchBees);
		certificateInfo26.AddUnlockedObject(ObjectType.StorageBeehive);
		certificateInfo26.AddUnlockedObject(ObjectType.HatCloche);
		CertificateInfo certificateInfo27 = AddCertificate(Quest.ID.AcademyTextiles, 4249600);
		certificateInfo27.AddEvent(QuestEvent.Type.Build, BotOnly: false, ObjectType.SpinningWheel, 1);
		certificateInfo27.AddEvent(QuestEvent.Type.Build, BotOnly: false, ObjectType.RockingChair, 1);
		certificateInfo27.AddEvent(QuestEvent.Type.MakeTop, BotOnly: false, null, 20);
		certificateInfo27.AddResearchID(Quest.ID.ResearchTextilesCrude);
		certificateInfo27.AddUnlockedObject(ObjectType.HatChullo);
		certificateInfo27.AddUnlockedObject(ObjectType.HatFarmer);
		certificateInfo27.AddUnlockedObject(ObjectType.TopJumper);
		certificateInfo27.AddUnlockedObject(ObjectType.TopDress);
		certificateInfo27.AddUnlockedObject(ObjectType.TopRobe);
		certificateInfo27.AddUnlockedObject(ObjectType.UpgradePlayerInventoryGood);
		CertificateInfo certificateInfo28 = AddCertificate(Quest.ID.AcademyColonisation3, 16711790);
		certificateInfo28.AddEvent(QuestEvent.Type.ClotheFolk, BotOnly: false, null, 20);
		certificateInfo28.AddEvent(QuestEvent.Type.Make, BotOnly: false, ObjectType.FolkHeart3, 200);
		certificateInfo28.AddResearchID(Quest.ID.ResearchTextilesCrude);
		certificateInfo28.AddUnlockedObject(ObjectType.SpacePort);
		certificateInfo28.AddUnlockedObject(ObjectType.GiantWaterWheel);
		certificateInfo28.AddUnlockedObject(ObjectType.FencePicket);
		certificateInfo28.AddUnlockedObject(ObjectType.GatePicket);
		certificateInfo28.AddUnlockedObject(ObjectType.WorkerHeadMk2Variant1);
		certificateInfo28.AddUnlockedObject(ObjectType.WorkerFrameMk2Variant1);
		certificateInfo28.AddUnlockedObject(ObjectType.WorkerDriveMk2Variant1);
		certificateInfo28.AddUnlockedObject(ObjectType.ToolBroom);
		CertificateInfo certificateInfo29 = AddCertificate(Quest.ID.AcademyConstruction, 16766976);
		certificateInfo29.AddEvent(QuestEvent.Type.Build, BotOnly: false, ObjectType.SandPath, 30);
		certificateInfo29.AddEvent(QuestEvent.Type.Build, BotOnly: false, ObjectType.StoneWall, 30);
		certificateInfo29.AddResearchID(Quest.ID.ResearchConstructionCrude);
		certificateInfo29.AddUnlockedObject(ObjectType.HatFez);
		CertificateInfo certificateInfo30 = AddCertificate(Quest.ID.AcademyMasonry, 13684960);
		certificateInfo30.AddEvent(QuestEvent.Type.Build, BotOnly: false, ObjectType.MasonryBench, 1);
		certificateInfo30.AddEvent(QuestEvent.Type.MineTallBoulder, BotOnly: false, null, 10);
		certificateInfo30.AddEvent(QuestEvent.Type.Make, BotOnly: false, ObjectType.StoneBlock, 3);
		certificateInfo30.AddResearchID(Quest.ID.ResearchMasonryCrude);
		certificateInfo30.AddUnlockedObject(ObjectType.BlockWall);
		certificateInfo30.AddUnlockedObject(ObjectType.BlockDoor);
		certificateInfo30.AddUnlockedObject(ObjectType.WindowStone);
		certificateInfo30.AddUnlockedObject(ObjectType.FlooringFlagstone);
		CertificateInfo certificateInfo31 = AddCertificate(Quest.ID.AcademyMetal, 14737600);
		certificateInfo31.AddEvent(QuestEvent.Type.Build, BotOnly: false, ObjectType.ClayFurnace, 1);
		certificateInfo31.AddEvent(QuestEvent.Type.Make, BotOnly: false, ObjectType.IronCrude, 30);
		certificateInfo31.AddEvent(QuestEvent.Type.Build, BotOnly: false, ObjectType.BasicMetalWorkbench, 1);
		certificateInfo31.AddEvent(QuestEvent.Type.MakeCrudeMetalTool, BotOnly: false, null, 30);
		certificateInfo31.AddResearchID(Quest.ID.ResearchMetalCrude);
		certificateInfo31.AddUnlockedObject(ObjectType.WorkerHeadMk2Variant2);
		certificateInfo31.AddUnlockedObject(ObjectType.WorkerFrameMk2Variant2);
		certificateInfo31.AddUnlockedObject(ObjectType.WorkerDriveMk2Variant2);
		CertificateInfo certificateInfo32 = AddCertificate(Quest.ID.AcademyTransportation, 2113791);
		certificateInfo32.AddEvent(QuestEvent.Type.Build, BotOnly: false, ObjectType.RoadCrude, 30);
		certificateInfo32.AddEvent(QuestEvent.Type.Make, BotOnly: false, ObjectType.WheelBarrow, 5);
		certificateInfo32.AddEvent(QuestEvent.Type.Make, BotOnly: false, ObjectType.Cart, 3);
		certificateInfo32.AddEvent(QuestEvent.Type.StorageUsed, BotOnly: false, ObjectType.WheelBarrow, 100);
		certificateInfo32.AddEvent(QuestEvent.Type.StorageUsed, BotOnly: false, ObjectType.Cart, 100);
		certificateInfo32.AddResearchID(Quest.ID.ResearchTransportation);
		certificateInfo32.AddUnlockedObject(ObjectType.WorkerHeadMk2Variant3);
		certificateInfo32.AddUnlockedObject(ObjectType.WorkerFrameMk2Variant3);
		certificateInfo32.AddUnlockedObject(ObjectType.WorkerDriveMk2Variant3);
		CertificateInfo certificateInfo33 = AddCertificate(Quest.ID.AcademyColonisation4, 16711790);
		certificateInfo33.AddEvent(QuestEvent.Type.ToyFolk, BotOnly: false, null, 30);
		certificateInfo33.AddEvent(QuestEvent.Type.Make, BotOnly: false, ObjectType.FolkHeart4, 300);
		certificateInfo33.AddResearchID(Quest.ID.ResearchToys);
		certificateInfo33.AddUnlockedObject(ObjectType.Catapult);
		certificateInfo33.AddUnlockedObject(ObjectType.UpgradePlayerMovementGood);
		CertificateInfo certificateInfo34 = AddCertificate(Quest.ID.AcademyPower, 16750848);
		certificateInfo34.AddEvent(QuestEvent.Type.Build, BotOnly: false, ObjectType.Windmill, 1);
		certificateInfo34.AddEvent(QuestEvent.Type.Build, BotOnly: false, ObjectType.BeltLinkage, 5);
		certificateInfo34.AddResearchID(Quest.ID.ResearchPower);
		certificateInfo34.AddUnlockedObject(ObjectType.HatCrown);
		certificateInfo34.AddUnlockedObject(ObjectType.WorkerHeadMk2Variant4);
		certificateInfo34.AddUnlockedObject(ObjectType.WorkerFrameMk2Variant4);
		certificateInfo34.AddUnlockedObject(ObjectType.WorkerDriveMk2Variant4);
		CertificateInfo certificateInfo35 = AddCertificate(Quest.ID.AcademyFishing2, 5308320);
		certificateInfo35.AddEvent(QuestEvent.Type.Make, BotOnly: false, ObjectType.ToolFishingRodGood, 3);
		certificateInfo35.AddEvent(QuestEvent.Type.CatchBait, BotOnly: false, null, 200);
		certificateInfo35.AddEvent(QuestEvent.Type.CatchFish, BotOnly: false, null, 200);
		certificateInfo35.AddResearchID(Quest.ID.ResearchShelter3);
		certificateInfo35.AddUnlockedObject(ObjectType.Sign3);
		CertificateInfo certificateInfo36 = AddCertificate(Quest.ID.AcademyRobotics2, 12120304);
		certificateInfo36.AddResearchID(Quest.ID.ResearchRobotics2);
		certificateInfo36.AddEvent(QuestEvent.Type.Make, BotOnly: false, ObjectType.Worker, 150);
		certificateInfo36.AddUnlockedObject(ObjectType.BotServer);
		CertificateInfo certificateInfo37 = AddCertificate(Quest.ID.AcademyMining2, 12632272);
		certificateInfo37.AddResearchID(Quest.ID.ResearchPowerFuel2);
		certificateInfo37.AddEvent(QuestEvent.Type.MineCoal, BotOnly: false, null, 100);
		certificateInfo37.AddUnlockedObject(ObjectType.HatMiner);
		CertificateInfo certificateInfo38 = AddCertificate(Quest.ID.AcademyLeeching, 9524224);
		certificateInfo38.AddResearchID(Quest.ID.ResearchHealth);
		certificateInfo38.AddEvent(QuestEvent.Type.LeechCaught, BotOnly: false, null, 30);
		certificateInfo38.AddEvent(QuestEvent.Type.Store, BotOnly: false, ObjectType.AnimalLeech, 30);
		certificateInfo38.AddUnlockedObject(ObjectType.HatBunny);
		certificateInfo38.AddUnlockedObject(ObjectType.TopBunny);
		CertificateInfo certificateInfo39 = AddCertificate(Quest.ID.AcademyColonisation5, 16711790);
		certificateInfo39.AddEvent(QuestEvent.Type.MedicineFolk, BotOnly: false, null, 30);
		certificateInfo39.AddEvent(QuestEvent.Type.Make, BotOnly: false, ObjectType.FolkHeart5, 500);
		certificateInfo39.AddResearchID(Quest.ID.ResearchHealth);
		certificateInfo39.AddUnlockedObject(ObjectType.UpgradePlayerInventorySuper);
		certificateInfo39.AddUnlockedObject(ObjectType.HatTrain);
		certificateInfo39.AddUnlockedObject(ObjectType.Ziggurat);
		CertificateInfo certificateInfo40 = AddCertificate(Quest.ID.AcademyFlowers, 15400846);
		certificateInfo40.AddEvent(QuestEvent.Type.GrowFlower, BotOnly: false, null, 100);
		certificateInfo40.AddEvent(QuestEvent.Type.ScytheFlower, BotOnly: false, null, 100);
		certificateInfo40.AddResearchID(Quest.ID.ResearchHealth2);
		certificateInfo40.AddUnlockedObject(ObjectType.HatBox);
		CertificateInfo certificateInfo41 = AddCertificate(Quest.ID.AcademyMetal2, 14737600);
		certificateInfo41.AddEvent(QuestEvent.Type.Build, BotOnly: false, ObjectType.Furnace, 1);
		certificateInfo41.AddEvent(QuestEvent.Type.Make, BotOnly: false, ObjectType.IronCrude, 50);
		certificateInfo41.AddEvent(QuestEvent.Type.Build, BotOnly: false, ObjectType.MetalWorkbench, 1);
		certificateInfo41.AddEvent(QuestEvent.Type.Make, BotOnly: false, ObjectType.Rivets, 30);
		certificateInfo41.AddResearchID(Quest.ID.ResearchMetalCrude2);
		certificateInfo41.AddUnlockedObject(ObjectType.WorkerHeadMk3Variant1);
		certificateInfo41.AddUnlockedObject(ObjectType.WorkerFrameMk3Variant1);
		certificateInfo41.AddUnlockedObject(ObjectType.WorkerDriveMk3Variant1);
		CertificateInfo certificateInfo42 = AddCertificate(Quest.ID.AcademyTransportation2, 2113791);
		certificateInfo42.AddEvent(QuestEvent.Type.Build, BotOnly: false, ObjectType.TrainTrack, 30);
		certificateInfo42.AddEvent(QuestEvent.Type.Make, BotOnly: false, ObjectType.Minecart, 1);
		certificateInfo42.AddEvent(QuestEvent.Type.Make, BotOnly: false, ObjectType.Carriage, 2);
		certificateInfo42.AddEvent(QuestEvent.Type.StorageUsed, BotOnly: false, ObjectType.Carriage, 100);
		certificateInfo42.AddResearchID(Quest.ID.ResearchTransportation2);
		certificateInfo42.AddUnlockedObject(ObjectType.HatFox);
		certificateInfo42.AddUnlockedObject(ObjectType.TopFox);
		certificateInfo42.AddUnlockedObject(ObjectType.HatDinosaur);
		certificateInfo42.AddUnlockedObject(ObjectType.TopDinosaur);
		CertificateInfo certificateInfo43 = AddCertificate(Quest.ID.AcademyColonisation6, 16711790);
		certificateInfo43.AddEvent(QuestEvent.Type.EducateFolk, BotOnly: false, null, 30);
		certificateInfo43.AddEvent(QuestEvent.Type.Make, BotOnly: false, ObjectType.FolkHeart6, 1000);
		certificateInfo43.AddResearchID(Quest.ID.ResearchCommunication);
		certificateInfo43.AddUnlockedObject(ObjectType.UpgradePlayerMovementSuper);
		certificateInfo43.AddUnlockedObject(ObjectType.HatAntlers);
		certificateInfo43.AddUnlockedObject(ObjectType.WorkerHeadMk3Variant2);
		certificateInfo43.AddUnlockedObject(ObjectType.WorkerFrameMk3Variant2);
		certificateInfo43.AddUnlockedObject(ObjectType.WorkerDriveMk3Variant2);
		CertificateInfo certificateInfo44 = AddCertificate(Quest.ID.AcademyPower2, 16750848);
		certificateInfo44.AddEvent(QuestEvent.Type.Build, BotOnly: false, ObjectType.StationaryEngine, 3);
		certificateInfo44.AddEvent(QuestEvent.Type.Build, BotOnly: false, ObjectType.BeltLinkage, 10);
		certificateInfo44.AddResearchID(Quest.ID.ResearchPowerFuel2);
		certificateInfo44.AddUnlockedObject(ObjectType.WorkerHeadMk3Variant3);
		certificateInfo44.AddUnlockedObject(ObjectType.WorkerFrameMk3Variant3);
		certificateInfo44.AddUnlockedObject(ObjectType.WorkerDriveMk3Variant3);
		CertificateInfo certificateInfo45 = AddCertificate(Quest.ID.AcademySilk, 5308320);
		certificateInfo45.AddEvent(QuestEvent.Type.Build, BotOnly: false, ObjectType.SilkwormStation, 1);
		certificateInfo45.AddEvent(QuestEvent.Type.Make, BotOnly: false, ObjectType.SilkRaw, 50);
		certificateInfo45.AddResearchID(Quest.ID.ResearchAnimalBreeding2);
		certificateInfo45.AddUnlockedObject(ObjectType.WorkerHeadMk3Variant4);
		certificateInfo45.AddUnlockedObject(ObjectType.WorkerFrameMk3Variant4);
		certificateInfo45.AddUnlockedObject(ObjectType.WorkerDriveMk3Variant4);
		CertificateInfo certificateInfo46 = AddCertificate(Quest.ID.AcademyColonisation7, 16711790);
		certificateInfo46.AddEvent(QuestEvent.Type.ArtFolk, BotOnly: false, null, 30);
		certificateInfo46.AddEvent(QuestEvent.Type.Make, BotOnly: false, ObjectType.FolkHeart7, 1000);
		certificateInfo46.AddResearchID(Quest.ID.ResearchCulture);
		certificateInfo46.AddUnlockedObject(ObjectType.TranscendBuilding);
		certificateInfo46.AddUnlockedQuest(Quest.ID.AcademyColonisation8);
		CertificateInfo certificateInfo47 = AddCertificate(Quest.ID.AcademyColonisation8, 16711790);
		certificateInfo47.AddEvent(QuestEvent.Type.Build, BotOnly: false, ObjectType.TranscendBuilding, 1);
		certificateInfo47.AddEvent(QuestEvent.Type.FolkTranscended, BotOnly: false, null, 1);
		certificateInfo47.AddUnlockedObject(ObjectType.StoneHenge);
		ConvertToQuests();
	}
}
