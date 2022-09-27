using System.Collections.Generic;

public class ResearchData
{
	private List<ResearchLevelInfo> m_Levels;

	public ResearchLevelInfo GetLevelInfo(int Level)
	{
		if (Level >= m_Levels.Count)
		{
			Level = m_Levels.Count - 1;
		}
		return m_Levels[Level];
	}

	public ResearchLevelInfo GetLevelFromQuestID(Quest.ID NewID)
	{
		foreach (ResearchLevelInfo level in m_Levels)
		{
			if (level.m_ID == NewID)
			{
				return level;
			}
		}
		return null;
	}

	public ResearchInfo GetInfoFromQuestID(Quest.ID NewID)
	{
		foreach (ResearchLevelInfo level in m_Levels)
		{
			foreach (ResearchInfo researchInfo in level.m_ResearchInfos)
			{
				if (researchInfo.m_ID == NewID)
				{
					return researchInfo;
				}
			}
		}
		return null;
	}

	public int GetHighestLevel()
	{
		return m_Levels.Count - 1;
	}

	public int GetCompleted()
	{
		int num = 0;
		foreach (ResearchLevelInfo level in m_Levels)
		{
			foreach (ResearchInfo researchInfo in level.m_ResearchInfos)
			{
				Quest quest = QuestManager.Instance.GetQuest(researchInfo.m_ID);
				if (quest != null && quest.GetIsComplete())
				{
					num++;
				}
			}
		}
		return num;
	}

	public bool GetLevelLocked(int Level)
	{
		Quest quest = QuestManager.Instance.GetQuest(m_Levels[Level].m_ID);
		if (quest == null)
		{
			return false;
		}
		return !quest.GetIsComplete();
	}

	public void CheckQuestsUnlocked()
	{
		int completed = GetCompleted();
		for (int i = 0; i < m_Levels.Count; i++)
		{
			Quest quest = QuestManager.Instance.GetQuest(m_Levels[i].m_ID);
			if (quest != null && !quest.GetIsComplete())
			{
				QuestEvent questEvent = quest.m_EventsRequired[0];
				if (questEvent.m_Progress < completed)
				{
					questEvent.SetProgress(completed);
				}
			}
		}
		for (int j = 0; j < m_Levels.Count; j++)
		{
			if (GetLevelLocked(j))
			{
				continue;
			}
			foreach (ResearchInfo researchInfo in m_Levels[j].m_ResearchInfos)
			{
				Quest quest2 = QuestManager.Instance.GetQuest(researchInfo.m_ID);
				if (quest2 != null && !quest2.m_Started)
				{
					QuestManager.Instance.AddQuest(quest2.m_ID);
				}
			}
		}
	}

	public void ConvertToQuest(ResearchInfo Info)
	{
		Quest quest = new Quest();
		quest.m_Simple = false;
		quest.AddEvent(QuestEvent.Type.Research, BotOnly: false, null, Info.m_HeartsRequired);
		quest.m_ObjectTypeRequired = Info.m_RequiredObjects[0];
		foreach (ObjectType unlockedObject in Info.m_UnlockedObjects)
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
		string text = Info.m_ID.ToString();
		CeremonyManager.CeremonyType newCeremonyType = CeremonyManager.CeremonyType.ResearchEnded;
		if (Info.m_ID == Quest.ID.ResearchPowerFuel2)
		{
			newCeremonyType = CeremonyManager.CeremonyType.PhaseOneComplete;
		}
		quest.SetInfo(Info.m_ID, Quest.Category.Total, text, text, text + "Desc", null, null, newCeremonyType, Quest.Type.Research);
		QuestData.Instance.AddQuest(Info.m_ID, quest);
	}

	public void LevelToQuest(ResearchLevelInfo Info)
	{
		Quest quest = new Quest();
		quest.m_Simple = false;
		if (Info.m_ID == Quest.ID.ResearchLevel1)
		{
			quest.AddEvent(QuestEvent.Type.Build, BotOnly: false, ObjectType.ResearchStationCrude, 1);
		}
		else
		{
			quest.AddEvent(QuestEvent.Type.CompleteResearch, BotOnly: false, null, Info.m_ResearchRequired);
		}
		foreach (ResearchInfo researchInfo in Info.m_ResearchInfos)
		{
			quest.AddQuestUnlocked(researchInfo.m_ID);
		}
		foreach (ObjectType unlockedObject in Info.m_UnlockedObjects)
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
		string title = quest.m_Title;
		CeremonyManager.CeremonyType newCeremonyType = CeremonyManager.CeremonyType.ResearchLevelUnlocked;
		if (Info.m_ID == Quest.ID.ResearchLevel1)
		{
			newCeremonyType = CeremonyManager.CeremonyType.FirstResearch;
		}
		quest.SetInfo(Info.m_ID, Quest.Category.Total, title, title, title + "Desc", null, null, newCeremonyType, Quest.Type.Important);
		QuestData.Instance.AddQuest(Info.m_ID, quest);
	}

	public void ConvertToQuests()
	{
		foreach (ResearchLevelInfo level in m_Levels)
		{
			LevelToQuest(level);
			foreach (ResearchInfo researchInfo in level.m_ResearchInfos)
			{
				ConvertToQuest(researchInfo);
			}
		}
	}

	public void StartLevels()
	{
		foreach (ResearchLevelInfo level in m_Levels)
		{
			QuestManager.Instance.AddQuest(level.m_ID);
		}
	}

	public Quest.ID GetQuestFromUnlockedObjectType(ObjectType NewType)
	{
		foreach (ResearchLevelInfo level in m_Levels)
		{
			foreach (ResearchInfo researchInfo in level.m_ResearchInfos)
			{
				if (researchInfo.m_UnlockedObjects.Contains(NewType))
				{
					return researchInfo.m_ID;
				}
			}
		}
		return Quest.ID.Total;
	}

	private ResearchLevelInfo AddLevel(Quest.ID NewID, int UnlockRequired)
	{
		ResearchLevelInfo researchLevelInfo = new ResearchLevelInfo(m_Levels.Count, NewID, UnlockRequired);
		m_Levels.Add(researchLevelInfo);
		return researchLevelInfo;
	}

	private void SetupLevel1()
	{
		ResearchLevelInfo researchLevelInfo = AddLevel(Quest.ID.ResearchLevel1, 0);
		ResearchInfo researchInfo = researchLevelInfo.AddResearch(Quest.ID.ResearchToolsCrude, 20);
		researchInfo.AddRequiredObject(ObjectType.Rock);
		researchInfo.AddUnlockedObject(ObjectType.RockSharp);
		researchInfo.AddUnlockedObject(ObjectType.ToolMallet);
		researchInfo.AddUnlockedObject(ObjectType.ToolFishingStick);
		researchInfo.AddUnlockedObject(ObjectType.ToolNetCrude);
		ResearchInfo researchInfo2 = researchLevelInfo.AddResearch(Quest.ID.ResearchStorageCrude, 50);
		researchInfo2.AddRequiredObject(ObjectType.Plank);
		researchInfo2.AddUnlockedObject(ObjectType.WorkbenchMk2);
		researchInfo2.AddUnlockedObject(ObjectType.StorageSand);
		researchInfo2.AddUnlockedObject(ObjectType.StorageLiquid);
		researchInfo2.AddUnlockedObject(ObjectType.ToolBucketCrude);
		researchInfo2.AddUnlockedObject(ObjectType.UpgradePlayerInventoryCrude);
		ResearchInfo researchInfo3 = researchLevelInfo.AddResearch(Quest.ID.ResearchFarmingCrude, 100);
		researchInfo3.AddRequiredObject(ObjectType.RockSharp);
		researchInfo3.AddUnlockedObject(ObjectType.ToolScytheStone);
		researchInfo3.AddUnlockedObject(ObjectType.ToolHoeStone);
		researchInfo3.AddUnlockedObject(ObjectType.ToolFlailCrude);
		researchInfo3.AddUnlockedObject(ObjectType.ToolShears);
		ResearchInfo researchInfo4 = researchLevelInfo.AddResearch(Quest.ID.ResearchRobotics, 100);
		researchInfo4.AddRequiredObject(ObjectType.TreeSeed);
		researchInfo4.AddUnlockedObject(ObjectType.WorkerWorkbenchMk1);
		researchInfo4.AddUnlockedObject(ObjectType.Worker);
		researchInfo4.AddUnlockedObject(ObjectType.WorkerHeadMk1);
		researchInfo4.AddUnlockedObject(ObjectType.WorkerFrameMk1);
		researchInfo4.AddUnlockedObject(ObjectType.WorkerDriveMk1);
		researchInfo4.AddUnlockedObject(ObjectType.DataStorageCrude);
		researchInfo4.AddUnlockedObject(ObjectType.UpgradeWorkerMemoryCrude);
		researchInfo4.AddUnlockedObject(ObjectType.UpgradeWorkerSearchCrude);
		researchInfo4.AddUnlockedObject(ObjectType.UpgradeWorkerCarryCrude);
		researchInfo4.AddUnlockedObject(ObjectType.UpgradeWorkerInventoryCrude);
		researchInfo4.AddUnlockedObject(ObjectType.UpgradeWorkerEnergyCrude);
		researchInfo4.AddUnlockedObject(ObjectType.UpgradeWorkerMovementCrude);
		ResearchInfo researchInfo5 = researchLevelInfo.AddResearch(Quest.ID.ResearchShelterCrude, 150);
		researchInfo5.AddRequiredObject(ObjectType.Log);
		researchInfo5.AddUnlockedObject(ObjectType.Hut);
		ResearchInfo researchInfo6 = researchLevelInfo.AddResearch(Quest.ID.ResearchCookingCrude, 150);
		researchInfo6.AddRequiredObject(ObjectType.Berries);
		researchInfo6.AddUnlockedObject(ObjectType.PotCrude);
		researchInfo6.AddUnlockedObject(ObjectType.Porridge);
		researchInfo6.AddUnlockedObject(ObjectType.BerriesSpice);
		researchInfo6.AddUnlockedObject(ObjectType.AppleSpice);
		researchInfo6.AddUnlockedObject(ObjectType.MushroomHerb);
		researchInfo6.AddUnlockedObject(ObjectType.PumpkinHerb);
		researchInfo6.AddUnlockedObject(ObjectType.FishHerb);
		researchInfo6.AddUnlockedObject(ObjectType.CarrotSalad);
	}

	private void SetupLevel2()
	{
		ResearchLevelInfo researchLevelInfo = AddLevel(Quest.ID.ResearchLevel2, 3);
		researchLevelInfo.AddUnlockedObject(ObjectType.ResearchStationCrude2);
		ResearchInfo researchInfo = researchLevelInfo.AddResearch(Quest.ID.ResearchPowerCrude, 500);
		researchInfo.AddRequiredObject(ObjectType.Stick);
		researchInfo.AddUnlockedObject(ObjectType.ToolTorchCrude);
		ResearchInfo researchInfo2 = researchLevelInfo.AddResearch(Quest.ID.ResearchRobotics2, 1000);
		researchInfo2.AddRequiredObject(ObjectType.WorkerHeadMk1);
		researchInfo2.AddUnlockedObject(ObjectType.StorageWorker);
		researchInfo2.AddUnlockedObject(ObjectType.WorkerWorkbenchMk2);
		researchInfo2.AddUnlockedObject(ObjectType.WorkerHeadMk2);
		researchInfo2.AddUnlockedObject(ObjectType.WorkerFrameMk2);
		researchInfo2.AddUnlockedObject(ObjectType.WorkerDriveMk2);
		researchInfo2.AddUnlockedObject(ObjectType.UpgradeWorkerMemoryGood);
		researchInfo2.AddUnlockedObject(ObjectType.UpgradeWorkerSearchGood);
		researchInfo2.AddUnlockedObject(ObjectType.UpgradeWorkerCarryGood);
		researchInfo2.AddUnlockedObject(ObjectType.UpgradeWorkerInventoryGood);
		researchInfo2.AddUnlockedObject(ObjectType.UpgradeWorkerEnergyGood);
		researchInfo2.AddUnlockedObject(ObjectType.UpgradeWorkerMovementGood);
		ResearchInfo researchInfo3 = researchLevelInfo.AddResearch(Quest.ID.ResearchPlantBreedingCrude, 1000);
		researchInfo3.AddRequiredObject(ObjectType.TreeSeed);
		researchInfo3.AddUnlockedObject(ObjectType.CrudePlantBreedingStation);
		researchInfo3.AddUnlockedObject(ObjectType.Coconut);
		researchInfo3.AddUnlockedObject(ObjectType.CarrotSeed);
		researchInfo3.AddUnlockedObject(ObjectType.Carrot);
		researchInfo3.AddUnlockedObject(ObjectType.CropCarrot);
		ResearchInfo researchInfo4 = researchLevelInfo.AddResearch(Quest.ID.ResearchConstructionCrude, 1000);
		researchInfo4.AddRequiredObject(ObjectType.Sand);
		researchInfo4.AddUnlockedObject(ObjectType.FlooringCrude);
		researchInfo4.AddUnlockedObject(ObjectType.SandPath);
		ResearchInfo researchInfo5 = researchLevelInfo.AddResearch(Quest.ID.ResearchConstructionCrude2, 1500);
		researchInfo5.AddRequiredObject(ObjectType.Log);
		researchInfo5.AddRequiredObject(ObjectType.Rock);
		researchInfo5.AddUnlockedObject(ObjectType.LogWall);
		researchInfo5.AddUnlockedObject(ObjectType.LogArch);
		researchInfo5.AddUnlockedObject(ObjectType.StoneWall);
		researchInfo5.AddUnlockedObject(ObjectType.StoneArch);
		researchInfo5.AddUnlockedObject(ObjectType.StoneArchDoor);
		researchInfo5.AddUnlockedObject(ObjectType.StonePath);
		ResearchInfo researchInfo6 = researchLevelInfo.AddResearch(Quest.ID.ResearchConstructionCrude3, 1500);
		researchInfo6.AddRequiredObject(ObjectType.Pole);
		researchInfo6.AddUnlockedObject(ObjectType.WorkbenchStructural);
		researchInfo6.AddUnlockedObject(ObjectType.FixingPeg);
		researchInfo6.AddUnlockedObject(ObjectType.FrameSquare);
		researchInfo6.AddUnlockedObject(ObjectType.Panel);
		ResearchInfo researchInfo7 = researchLevelInfo.AddResearch(Quest.ID.ResearchMasonryCrude, 1500);
		researchInfo7.AddRequiredObject(ObjectType.Rock);
		researchInfo7.AddUnlockedObject(ObjectType.MasonryBench);
		researchInfo7.AddUnlockedObject(ObjectType.ToolChiselCrude);
		researchInfo7.AddUnlockedObject(ObjectType.StoneBlock);
		ResearchInfo researchInfo8 = researchLevelInfo.AddResearch(Quest.ID.ResearchFarmingLivestock, 2000);
		researchInfo8.AddRequiredObject(ObjectType.GrassCut);
		researchInfo8.AddUnlockedObject(ObjectType.ToolPitchfork);
		researchInfo8.AddUnlockedObject(ObjectType.HayBale);
		researchInfo8.AddUnlockedObject(ObjectType.Barn);
		ResearchInfo researchInfo9 = researchLevelInfo.AddResearch(Quest.ID.ResearchConstruction, 2000);
		researchInfo9.AddRequiredObject(ObjectType.Plank);
		researchInfo9.AddUnlockedObject(ObjectType.Workshop);
		researchInfo9.AddUnlockedObject(ObjectType.FencePost);
		researchInfo9.AddUnlockedObject(ObjectType.Gate);
		ResearchInfo researchInfo10 = researchLevelInfo.AddResearch(Quest.ID.ResearchShelter, 2500);
		researchInfo10.AddRequiredObject(ObjectType.Log);
		researchInfo10.AddRequiredObject(ObjectType.Straw);
		researchInfo10.AddUnlockedObject(ObjectType.LogCabin);
		ResearchInfo researchInfo11 = researchLevelInfo.AddResearch(Quest.ID.ResearchPotteryCrude, 2500);
		researchInfo11.AddRequiredObject(ObjectType.Clay);
		researchInfo11.AddUnlockedObject(ObjectType.ClayStationCrude);
		researchInfo11.AddUnlockedObject(ObjectType.KilnCrude);
		researchInfo11.AddUnlockedObject(ObjectType.PotClayRaw);
		researchInfo11.AddUnlockedObject(ObjectType.PotClay);
		researchInfo11.AddUnlockedObject(ObjectType.GnomeRaw);
		researchInfo11.AddUnlockedObject(ObjectType.Gnome);
		researchInfo11.AddUnlockedObject(ObjectType.Gnome2);
		researchInfo11.AddUnlockedObject(ObjectType.Gnome3);
		researchInfo11.AddUnlockedObject(ObjectType.Gnome4);
		researchInfo11.AddUnlockedObject(ObjectType.Gnome5);
		researchInfo11.AddUnlockedObject(ObjectType.Gnome6);
		ResearchInfo researchInfo12 = researchLevelInfo.AddResearch(Quest.ID.ResearchCookingCrude2, 2500);
		researchInfo12.AddRequiredObject(ObjectType.ToolTorchCrude);
		researchInfo12.AddUnlockedObject(ObjectType.CookingPotCrude);
		researchInfo12.AddUnlockedObject(ObjectType.BerriesStew);
		researchInfo12.AddUnlockedObject(ObjectType.AppleStew);
		researchInfo12.AddUnlockedObject(ObjectType.MushroomSoup);
		researchInfo12.AddUnlockedObject(ObjectType.PumpkinSoup);
		researchInfo12.AddUnlockedObject(ObjectType.FishSoup);
		researchInfo12.AddUnlockedObject(ObjectType.CarrotStirFry);
		researchInfo12.AddUnlockedObject(ObjectType.MilkPorridge);
		ResearchInfo researchInfo13 = researchLevelInfo.AddResearch(Quest.ID.ResearchCookingBaking, 2500);
		researchInfo13.AddRequiredObject(ObjectType.Wheat);
		researchInfo13.AddUnlockedObject(ObjectType.Quern);
		researchInfo13.AddUnlockedObject(ObjectType.OvenCrude);
		researchInfo13.AddUnlockedObject(ObjectType.KitchenTable);
		researchInfo13.AddUnlockedObject(ObjectType.FlourCrude);
		researchInfo13.AddUnlockedObject(ObjectType.Dough);
		researchInfo13.AddUnlockedObject(ObjectType.BreadCrude);
		ResearchInfo researchInfo14 = researchLevelInfo.AddResearch(Quest.ID.ResearchConstructionParts, 2500);
		researchInfo14.AddRequiredObject(ObjectType.CogCrude);
		researchInfo14.AddUnlockedObject(ObjectType.CogBench);
		researchInfo14.AddUnlockedObject(ObjectType.Axle);
		researchInfo14.AddUnlockedObject(ObjectType.Crank);
		researchInfo14.AddUnlockedObject(ObjectType.Cog);
		ResearchInfo researchInfo15 = researchLevelInfo.AddResearch(Quest.ID.ResearchTextilesCrude, 2500);
		researchInfo15.AddRequiredObject(ObjectType.CottonBall);
		researchInfo15.AddUnlockedObject(ObjectType.WheatHammer);
		researchInfo15.AddUnlockedObject(ObjectType.SpinningWheel);
		researchInfo15.AddUnlockedObject(ObjectType.RockingChair);
		researchInfo15.AddUnlockedObject(ObjectType.Wool);
		researchInfo15.AddUnlockedObject(ObjectType.BullrushesFibre);
		researchInfo15.AddUnlockedObject(ObjectType.CottonLint);
		researchInfo15.AddUnlockedObject(ObjectType.Blanket);
		researchInfo15.AddUnlockedObject(ObjectType.BullrushesCloth);
		researchInfo15.AddUnlockedObject(ObjectType.CottonCloth);
		researchInfo15.AddUnlockedObject(ObjectType.Thread);
		researchInfo15.AddUnlockedObject(ObjectType.BullrushesThread);
		researchInfo15.AddUnlockedObject(ObjectType.CottonThread);
		researchInfo15.AddUnlockedObject(ObjectType.TopPoncho);
		researchInfo15.AddUnlockedObject(ObjectType.TopTunic);
		researchInfo15.AddUnlockedObject(ObjectType.TopToga);
		researchInfo15.AddUnlockedObject(ObjectType.HatKnittedBeanie);
		researchInfo15.AddUnlockedObject(ObjectType.HatSugegasa);
	}

	private void SetupLevel3()
	{
		ResearchLevelInfo researchLevelInfo = AddLevel(Quest.ID.ResearchLevel3, 10);
		researchLevelInfo.AddUnlockedObject(ObjectType.ResearchStationCrude3);
		ResearchInfo researchInfo = researchLevelInfo.AddResearch(Quest.ID.ResearchWasteCrude, 10000);
		researchInfo.AddRequiredObject(ObjectType.Manure);
		researchInfo.AddUnlockedObject(ObjectType.StorageFertiliser);
		researchInfo.AddUnlockedObject(ObjectType.Fertiliser);
		ResearchInfo researchInfo2 = researchLevelInfo.AddResearch(Quest.ID.ResearchAnimalBreedingCrude, 10000);
		researchInfo2.AddRequiredObject(ObjectType.Coconut);
		researchInfo2.AddUnlockedObject(ObjectType.AnimalCowHighland);
		researchInfo2.AddUnlockedObject(ObjectType.AnimalAlpaca);
		researchInfo2.AddUnlockedObject(ObjectType.CrudeAnimalBreedingStation);
		ResearchInfo researchInfo3 = researchLevelInfo.AddResearch(Quest.ID.ResearchFibre, 15000);
		researchInfo3.AddRequiredObject(ObjectType.WeedDug);
		researchInfo3.AddUnlockedObject(ObjectType.StringBall);
		researchInfo3.AddUnlockedObject(ObjectType.StringWinderCrude);
		researchInfo3.AddUnlockedObject(ObjectType.ToolFishingRod);
		researchInfo3.AddUnlockedObject(ObjectType.ToolNet);
		ResearchInfo researchInfo4 = researchLevelInfo.AddResearch(Quest.ID.ResearchForestry, 15000);
		researchInfo4.AddRequiredObject(ObjectType.TreeSeed);
		researchInfo4.AddUnlockedObject(ObjectType.StorageSeedlings);
		researchInfo4.AddUnlockedObject(ObjectType.Seedling);
		ResearchInfo researchInfo5 = researchLevelInfo.AddResearch(Quest.ID.ResearchLumberCrude, 20000);
		researchInfo5.AddRequiredObject(ObjectType.ToolAxeStone);
		researchInfo5.AddUnlockedObject(ObjectType.BenchSaw);
		ResearchInfo researchInfo6 = researchLevelInfo.AddResearch(Quest.ID.ResearchFarmingPoultry, 20000);
		researchInfo6.AddRequiredObject(ObjectType.Egg);
		researchInfo6.AddUnlockedObject(ObjectType.ChickenCoop);
		ResearchInfo researchInfo7 = researchLevelInfo.AddResearch(Quest.ID.ResearchBees, 20000);
		researchInfo7.AddRequiredObject(ObjectType.BeesNest);
		researchInfo7.AddUnlockedObject(ObjectType.StorageBeehiveCrude);
		ResearchInfo researchInfo8 = researchLevelInfo.AddResearch(Quest.ID.ResearchConstruction2, 25000);
		researchInfo8.AddRequiredObject(ObjectType.Sand);
		researchInfo8.AddUnlockedObject(ObjectType.ToolBucket);
		researchInfo8.AddUnlockedObject(ObjectType.MortarMixerCrude);
		researchInfo8.AddUnlockedObject(ObjectType.BrickWall);
		researchInfo8.AddUnlockedObject(ObjectType.FlooringBrick);
		researchInfo8.AddUnlockedObject(ObjectType.BricksCrude);
		researchInfo8.AddUnlockedObject(ObjectType.BricksCrudeRaw);
		researchInfo8.AddUnlockedObject(ObjectType.Mortar);
		researchInfo8.AddUnlockedObject(ObjectType.ToolDredgerCrude);
		ResearchInfo researchInfo9 = researchLevelInfo.AddResearch(Quest.ID.ResearchTransportation, 25000);
		researchInfo9.AddRequiredObject(ObjectType.WheelCrude);
		researchInfo9.AddUnlockedObject(ObjectType.Bridge);
		researchInfo9.AddUnlockedObject(ObjectType.BridgeStone);
		researchInfo9.AddUnlockedObject(ObjectType.VehicleAssembler);
		researchInfo9.AddUnlockedObject(ObjectType.RoadCrude);
		researchInfo9.AddUnlockedObject(ObjectType.Wheel);
		researchInfo9.AddUnlockedObject(ObjectType.WheelBarrow);
		researchInfo9.AddUnlockedObject(ObjectType.Cart);
		researchInfo9.AddUnlockedObject(ObjectType.CartLiquid);
		ResearchInfo researchInfo10 = researchLevelInfo.AddResearch(Quest.ID.ResearchShelter2, 25000);
		researchInfo10.AddRequiredObject(ObjectType.Rock);
		researchInfo10.AddRequiredObject(ObjectType.Straw);
		researchInfo10.AddUnlockedObject(ObjectType.StoneCottage);
		researchInfo10.AddUnlockedObject(ObjectType.Fireplace);
		researchInfo10.AddUnlockedObject(ObjectType.Chimney);
		researchInfo10.AddUnlockedObject(ObjectType.FrameDoor);
		researchInfo10.AddUnlockedObject(ObjectType.Door);
		researchInfo10.AddUnlockedObject(ObjectType.FrameWindow);
		researchInfo10.AddUnlockedObject(ObjectType.Window);
		ResearchInfo researchInfo11 = researchLevelInfo.AddResearch(Quest.ID.ResearchCookingCrude3, 25000);
		researchInfo11.AddRequiredObject(ObjectType.ToolTorchCrude);
		researchInfo11.AddRequiredObject(ObjectType.IronCrude);
		researchInfo11.AddUnlockedObject(ObjectType.Cauldron);
		researchInfo11.AddUnlockedObject(ObjectType.BerriesJam);
		researchInfo11.AddUnlockedObject(ObjectType.AppleJam);
		researchInfo11.AddUnlockedObject(ObjectType.MushroomStew);
		researchInfo11.AddUnlockedObject(ObjectType.PumpkinStew);
		researchInfo11.AddUnlockedObject(ObjectType.FishStew);
		researchInfo11.AddUnlockedObject(ObjectType.CarrotHoney);
		researchInfo11.AddUnlockedObject(ObjectType.FruitPorridge);
		researchInfo11.AddUnlockedObject(ObjectType.HoneyPorridge);
		researchInfo11.AddUnlockedObject(ObjectType.ToolFlail);
		ResearchInfo researchInfo12 = researchLevelInfo.AddResearch(Quest.ID.ResearchToys, 30000);
		researchInfo12.AddRequiredObject(ObjectType.Stick);
		researchInfo12.AddUnlockedObject(ObjectType.ToyStationCrude);
		researchInfo12.AddUnlockedObject(ObjectType.Doll);
		researchInfo12.AddUnlockedObject(ObjectType.ToyHorse);
		researchInfo12.AddUnlockedObject(ObjectType.Buttons);
	}

	private void SetupLevel4()
	{
		ResearchLevelInfo researchLevelInfo = AddLevel(Quest.ID.ResearchLevel4, 18);
		researchLevelInfo.AddUnlockedObject(ObjectType.ResearchStationCrude4);
		ResearchInfo researchInfo = researchLevelInfo.AddResearch(Quest.ID.ResearchConstructionParts2, 100000);
		researchInfo.AddRequiredObject(ObjectType.FrameSquare);
		researchInfo.AddUnlockedObject(ObjectType.FrameTriangle);
		researchInfo.AddUnlockedObject(ObjectType.FrameBox);
		researchInfo.AddUnlockedObject(ObjectType.WoodenBeam);
		researchInfo.AddUnlockedObject(ObjectType.FlooringParquet);
		ResearchInfo researchInfo2 = researchLevelInfo.AddResearch(Quest.ID.ResearchPower, 200000);
		researchInfo2.AddRequiredObject(ObjectType.CogCrude);
		researchInfo2.AddUnlockedObject(ObjectType.Windmill);
		researchInfo2.AddUnlockedObject(ObjectType.BeltLinkage);
		ResearchInfo researchInfo3 = researchLevelInfo.AddResearch(Quest.ID.ResearchPottery, 250000);
		researchInfo3.AddRequiredObject(ObjectType.PotClayRaw);
		researchInfo3.AddUnlockedObject(ObjectType.ClayStation);
		researchInfo3.AddUnlockedObject(ObjectType.LargeBowlClay);
		researchInfo3.AddUnlockedObject(ObjectType.LargeBowlClayRaw);
		researchInfo3.AddUnlockedObject(ObjectType.FlowerPot);
		researchInfo3.AddUnlockedObject(ObjectType.FlowerPotRaw);
		researchInfo3.AddUnlockedObject(ObjectType.BricksCrude);
		researchInfo3.AddUnlockedObject(ObjectType.BricksCrudeRaw);
		researchInfo3.AddUnlockedObject(ObjectType.RoofTiles);
		researchInfo3.AddUnlockedObject(ObjectType.RoofTilesRaw);
		researchInfo3.AddUnlockedObject(ObjectType.JarClay);
		researchInfo3.AddUnlockedObject(ObjectType.JarClayRaw);
		ResearchInfo researchInfo4 = researchLevelInfo.AddResearch(Quest.ID.ResearchPowerFuel, 300000);
		researchInfo4.AddRequiredObject(ObjectType.IronOre);
		researchInfo4.AddUnlockedObject(ObjectType.Charcoal);
		researchInfo4.AddUnlockedObject(ObjectType.ClayFurnace);
		researchInfo4.AddUnlockedObject(ObjectType.BasicMetalWorkbench);
		researchInfo4.AddUnlockedObject(ObjectType.IronCrude);
		researchInfo4.AddUnlockedObject(ObjectType.MetalPoleCrude);
		researchInfo4.AddUnlockedObject(ObjectType.MetalPlateCrude);
		researchInfo4.AddUnlockedObject(ObjectType.ToolBucketMetal);
		ResearchInfo researchInfo5 = researchLevelInfo.AddResearch(Quest.ID.ResearchMetalCrude, 400000);
		researchInfo5.AddRequiredObject(ObjectType.IronCrude);
		researchInfo5.AddUnlockedObject(ObjectType.ToolAxe);
		researchInfo5.AddUnlockedObject(ObjectType.ToolShovel);
		researchInfo5.AddUnlockedObject(ObjectType.ToolPick);
		researchInfo5.AddUnlockedObject(ObjectType.ToolHoe);
		researchInfo5.AddUnlockedObject(ObjectType.ToolScythe);
		researchInfo5.AddUnlockedObject(ObjectType.ToolChisel);
		researchInfo5.AddUnlockedObject(ObjectType.ToolBlade);
		ResearchInfo researchInfo6 = researchLevelInfo.AddResearch(Quest.ID.ResearchStorage, 400000);
		researchInfo6.AddRequiredObject(ObjectType.FrameBox);
		researchInfo6.AddUnlockedObject(ObjectType.StoragePaletteMedium);
		researchInfo6.AddUnlockedObject(ObjectType.StorageGenericMedium);
		researchInfo6.AddUnlockedObject(ObjectType.StorageLiquidMedium);
		researchInfo6.AddUnlockedObject(ObjectType.StorageSandMedium);
		ResearchInfo researchInfo7 = researchLevelInfo.AddResearch(Quest.ID.ResearchShelter3, 500000);
		researchInfo7.AddRequiredObject(ObjectType.BricksCrude);
		researchInfo7.AddUnlockedObject(ObjectType.BrickHut);
		researchInfo7.AddUnlockedObject(ObjectType.RoadGood);
		researchInfo7.AddUnlockedObject(ObjectType.ToolFishingRodGood);
		ResearchInfo researchInfo8 = researchLevelInfo.AddResearch(Quest.ID.ResearchCookingBaking2, 500000);
		researchInfo8.AddRequiredObject(ObjectType.Dough);
		researchInfo8.AddUnlockedObject(ObjectType.Gristmill);
		researchInfo8.AddUnlockedObject(ObjectType.Millstone);
		researchInfo8.AddUnlockedObject(ObjectType.Oven);
		researchInfo8.AddUnlockedObject(ObjectType.Flour);
		researchInfo8.AddUnlockedObject(ObjectType.Pastry);
		researchInfo8.AddUnlockedObject(ObjectType.DoughGood);
		researchInfo8.AddUnlockedObject(ObjectType.Bread);
		researchInfo8.AddUnlockedObject(ObjectType.BerriesPieRaw);
		researchInfo8.AddUnlockedObject(ObjectType.BerriesPie);
		researchInfo8.AddUnlockedObject(ObjectType.ApplePieRaw);
		researchInfo8.AddUnlockedObject(ObjectType.ApplePie);
		researchInfo8.AddUnlockedObject(ObjectType.MushroomPieRaw);
		researchInfo8.AddUnlockedObject(ObjectType.MushroomPie);
		researchInfo8.AddUnlockedObject(ObjectType.PumpkinPieRaw);
		researchInfo8.AddUnlockedObject(ObjectType.PumpkinPie);
		researchInfo8.AddUnlockedObject(ObjectType.FishPieRaw);
		researchInfo8.AddUnlockedObject(ObjectType.FishPie);
		researchInfo8.AddUnlockedObject(ObjectType.CarrotCurry);
		researchInfo8.AddUnlockedObject(ObjectType.NaanRaw);
		researchInfo8.AddUnlockedObject(ObjectType.Naan);
		researchInfo8.AddUnlockedObject(ObjectType.BreadButtered);
		ResearchInfo researchInfo9 = researchLevelInfo.AddResearch(Quest.ID.ResearchTextiles, 500000);
		researchInfo9.AddRequiredObject(ObjectType.Blanket);
		researchInfo9.AddUnlockedObject(ObjectType.LoomCrude);
		researchInfo9.AddUnlockedObject(ObjectType.SewingStation);
		researchInfo9.AddUnlockedObject(ObjectType.HatMaker);
		researchInfo9.AddUnlockedObject(ObjectType.TopJacket);
		researchInfo9.AddUnlockedObject(ObjectType.TopShirt);
		researchInfo9.AddUnlockedObject(ObjectType.TopCoat);
		ResearchInfo researchInfo10 = researchLevelInfo.AddResearch(Quest.ID.ResearchToys2, 500000);
		researchInfo10.AddRequiredObject(ObjectType.Stick);
		researchInfo10.AddUnlockedObject(ObjectType.JackInTheBox);
		researchInfo10.AddUnlockedObject(ObjectType.ToyHorseCart);
		ResearchInfo researchInfo11 = researchLevelInfo.AddResearch(Quest.ID.ResearchHealth, 500000);
		researchInfo11.AddRequiredObject(ObjectType.AnimalLeech);
		researchInfo11.AddUnlockedObject(ObjectType.MedicineStation);
		researchInfo11.AddUnlockedObject(ObjectType.MedicineLeeches);
		ResearchInfo researchInfo12 = researchLevelInfo.AddResearch(Quest.ID.ResearchFarmingDairy, 500000);
		researchInfo12.AddRequiredObject(ObjectType.Milk);
		researchInfo12.AddUnlockedObject(ObjectType.Butter);
		researchInfo12.AddUnlockedObject(ObjectType.ButterChurn);
		researchInfo12.AddUnlockedObject(ObjectType.MilkingShedCrude);
		ResearchInfo researchInfo13 = researchLevelInfo.AddResearch(Quest.ID.ResearchFarmingSheep, 500000);
		researchInfo13.AddRequiredObject(ObjectType.Fleece);
		researchInfo13.AddUnlockedObject(ObjectType.ShearingShedCrude);
		ResearchInfo researchInfo14 = researchLevelInfo.AddResearch(Quest.ID.ResearchFarmingLivestock2, 500000);
		researchInfo14.AddRequiredObject(ObjectType.HayBale);
		researchInfo14.AddUnlockedObject(ObjectType.HayBalerCrude);
		researchInfo14.AddUnlockedObject(ObjectType.Trough);
		ResearchInfo researchInfo15 = researchLevelInfo.AddResearch(Quest.ID.ResearchLumber, 600000);
		researchInfo15.AddRequiredObject(ObjectType.ToolAxe);
		researchInfo15.AddUnlockedObject(ObjectType.BenchSaw2);
	}

	private void SetupLevel5()
	{
		ResearchLevelInfo researchLevelInfo = AddLevel(Quest.ID.ResearchLevel5, 32);
		researchLevelInfo.AddUnlockedObject(ObjectType.ResearchStationCrude5);
		ResearchInfo researchInfo = researchLevelInfo.AddResearch(Quest.ID.ResearchMetalCrude2, 800000);
		researchInfo.AddRequiredObject(ObjectType.IronCrude);
		researchInfo.AddUnlockedObject(ObjectType.Furnace);
		researchInfo.AddUnlockedObject(ObjectType.MetalWorkbench);
		researchInfo.AddUnlockedObject(ObjectType.Rivets);
		researchInfo.AddUnlockedObject(ObjectType.MetalWheel);
		researchInfo.AddUnlockedObject(ObjectType.MetalAxle);
		researchInfo.AddUnlockedObject(ObjectType.MetalCog);
		researchInfo.AddUnlockedObject(ObjectType.MetalGirder);
		ResearchInfo researchInfo2 = researchLevelInfo.AddResearch(Quest.ID.ResearchTransportation2, 1000000);
		researchInfo2.AddRequiredObject(ObjectType.MetalWheel);
		researchInfo2.AddUnlockedObject(ObjectType.VehicleAssemblerGood);
		researchInfo2.AddUnlockedObject(ObjectType.Minecart);
		researchInfo2.AddUnlockedObject(ObjectType.Carriage);
		researchInfo2.AddUnlockedObject(ObjectType.CarriageLiquid);
		researchInfo2.AddUnlockedObject(ObjectType.CraneCrude);
		researchInfo2.AddUnlockedObject(ObjectType.TrainTrack);
		researchInfo2.AddUnlockedObject(ObjectType.TrainTrackCurve);
		researchInfo2.AddUnlockedObject(ObjectType.TrainTrackPointsLeft);
		researchInfo2.AddUnlockedObject(ObjectType.TrainTrackPointsRight);
		researchInfo2.AddUnlockedObject(ObjectType.TrainTrackBridge);
		researchInfo2.AddUnlockedObject(ObjectType.TrainTrackStop);
		ResearchInfo researchInfo3 = researchLevelInfo.AddResearch(Quest.ID.ResearchPowerFuel2, 1000000);
		researchInfo3.AddRequiredObject(ObjectType.Coal);
		researchInfo3.AddUnlockedObject(ObjectType.StationaryEngine);
		researchInfo3.AddUnlockedObject(ObjectType.Firebox);
		researchInfo3.AddUnlockedObject(ObjectType.Boiler);
		researchInfo3.AddUnlockedObject(ObjectType.Flywheel);
		researchInfo3.AddUnlockedObject(ObjectType.ConnectingRod);
		researchInfo3.AddUnlockedObject(ObjectType.Piston);
		ResearchInfo researchInfo4 = researchLevelInfo.AddResearch(Quest.ID.ResearchRobotics3, 3000000);
		researchInfo4.AddRequiredObject(ObjectType.WorkerDriveMk2);
		researchInfo4.AddUnlockedObject(ObjectType.WorkerWorkbenchMk3);
		researchInfo4.AddUnlockedObject(ObjectType.WorkerHeadMk3);
		researchInfo4.AddUnlockedObject(ObjectType.WorkerFrameMk3);
		researchInfo4.AddUnlockedObject(ObjectType.WorkerDriveMk3);
		researchInfo4.AddUnlockedObject(ObjectType.UpgradeWorkerMemorySuper);
		researchInfo4.AddUnlockedObject(ObjectType.UpgradeWorkerSearchSuper);
		researchInfo4.AddUnlockedObject(ObjectType.UpgradeWorkerCarrySuper);
		researchInfo4.AddUnlockedObject(ObjectType.UpgradeWorkerInventorySuper);
		researchInfo4.AddUnlockedObject(ObjectType.UpgradeWorkerEnergySuper);
		researchInfo4.AddUnlockedObject(ObjectType.UpgradeWorkerMovementSuper);
		ResearchInfo researchInfo5 = researchLevelInfo.AddResearch(Quest.ID.ResearchShelter4, 5000000);
		researchInfo5.AddRequiredObject(ObjectType.BricksCrude);
		researchInfo5.AddUnlockedObject(ObjectType.Mansion);
		researchInfo5.AddUnlockedObject(ObjectType.MortarMixerGood);
		ResearchInfo researchInfo6 = researchLevelInfo.AddResearch(Quest.ID.ResearchCooking2, 5000000);
		researchInfo6.AddRequiredObject(ObjectType.Dough);
		researchInfo6.AddUnlockedObject(ObjectType.CakeBatter);
		researchInfo6.AddUnlockedObject(ObjectType.BerriesCakeRaw);
		researchInfo6.AddUnlockedObject(ObjectType.BerriesCake);
		researchInfo6.AddUnlockedObject(ObjectType.AppleCakeRaw);
		researchInfo6.AddUnlockedObject(ObjectType.AppleCake);
		researchInfo6.AddUnlockedObject(ObjectType.BreadPuddingRaw);
		researchInfo6.AddUnlockedObject(ObjectType.BreadPudding);
		researchInfo6.AddUnlockedObject(ObjectType.MushroomPuddingRaw);
		researchInfo6.AddUnlockedObject(ObjectType.MushroomPudding);
		researchInfo6.AddUnlockedObject(ObjectType.FishCakeRaw);
		researchInfo6.AddUnlockedObject(ObjectType.FishCake);
		researchInfo6.AddUnlockedObject(ObjectType.PumpkinCakeRaw);
		researchInfo6.AddUnlockedObject(ObjectType.PumpkinCake);
		researchInfo6.AddUnlockedObject(ObjectType.CarrotCakeRaw);
		researchInfo6.AddUnlockedObject(ObjectType.CarrotCake);
		researchInfo6.AddUnlockedObject(ObjectType.AppleBerryPieRaw);
		researchInfo6.AddUnlockedObject(ObjectType.AppleBerryPie);
		researchInfo6.AddUnlockedObject(ObjectType.PumpkinMushroomPieRaw);
		researchInfo6.AddUnlockedObject(ObjectType.PumpkinMushroomPie);
		ResearchInfo researchInfo7 = researchLevelInfo.AddResearch(Quest.ID.ResearchTextiles2, 5000000);
		researchInfo7.AddRequiredObject(ObjectType.Blanket);
		researchInfo7.AddUnlockedObject(ObjectType.LoomGood);
		researchInfo7.AddUnlockedObject(ObjectType.SpinningJenny);
		researchInfo7.AddUnlockedObject(ObjectType.TopBlazer);
		researchInfo7.AddUnlockedObject(ObjectType.TopShirtTie);
		researchInfo7.AddUnlockedObject(ObjectType.TopCoatScarf);
		ResearchInfo researchInfo8 = researchLevelInfo.AddResearch(Quest.ID.ResearchToys3, 5000000);
		researchInfo8.AddRequiredObject(ObjectType.Stick);
		researchInfo8.AddUnlockedObject(ObjectType.DollHouse);
		researchInfo8.AddUnlockedObject(ObjectType.ToyHorseCarriage);
		ResearchInfo researchInfo9 = researchLevelInfo.AddResearch(Quest.ID.ResearchHealth2, 5000000);
		researchInfo9.AddRequiredObject(ObjectType.MedicineLeeches);
		researchInfo9.AddUnlockedObject(ObjectType.MedicineFlowers);
		ResearchInfo researchInfo10 = researchLevelInfo.AddResearch(Quest.ID.ResearchCommunication, 5000000);
		researchInfo10.AddRequiredObject(ObjectType.Log);
		researchInfo10.AddUnlockedObject(ObjectType.PaperMill);
		researchInfo10.AddUnlockedObject(ObjectType.PrintingPress);
		researchInfo10.AddUnlockedObject(ObjectType.Paper);
		researchInfo10.AddUnlockedObject(ObjectType.Ink);
		researchInfo10.AddUnlockedObject(ObjectType.EducationBook1);
	}

	private void SetupLevel6()
	{
		ResearchLevelInfo researchLevelInfo = AddLevel(Quest.ID.ResearchLevel6, 40);
		researchLevelInfo.AddUnlockedObject(ObjectType.ResearchStationCrude6);
		ResearchInfo researchInfo = researchLevelInfo.AddResearch(Quest.ID.ResearchTransportation3, 10000000);
		researchInfo.AddRequiredObject(ObjectType.MetalWheel);
		researchInfo.AddUnlockedObject(ObjectType.Train);
		researchInfo.AddUnlockedObject(ObjectType.CarriageTrain);
		researchInfo.AddUnlockedObject(ObjectType.TrainRefuellingStation);
		ResearchInfo researchInfo2 = researchLevelInfo.AddResearch(Quest.ID.ResearchCooking3, 10000000);
		researchInfo2.AddRequiredObject(ObjectType.DoughGood);
		researchInfo2.AddUnlockedObject(ObjectType.MushroomBurger);
		researchInfo2.AddUnlockedObject(ObjectType.BerryDanish);
		researchInfo2.AddUnlockedObject(ObjectType.PumpkinBurger);
		researchInfo2.AddUnlockedObject(ObjectType.CreamBrioche);
		researchInfo2.AddUnlockedObject(ObjectType.FishBurger);
		researchInfo2.AddUnlockedObject(ObjectType.AppleDanish);
		researchInfo2.AddUnlockedObject(ObjectType.CarrotBurger);
		ResearchInfo researchInfo3 = researchLevelInfo.AddResearch(Quest.ID.ResearchShelter5, 10000000);
		researchInfo3.AddRequiredObject(ObjectType.StoneBlock);
		researchInfo3.AddUnlockedObject(ObjectType.Castle);
		ResearchInfo researchInfo4 = researchLevelInfo.AddResearch(Quest.ID.ResearchAnimalBreeding2, 10000000);
		researchInfo4.AddRequiredObject(ObjectType.TreeSeed);
		researchInfo4.AddUnlockedObject(ObjectType.SilkwormStation);
		researchInfo4.AddUnlockedObject(ObjectType.MulberrySeed);
		researchInfo4.AddUnlockedObject(ObjectType.AnimalSilkworm);
		ResearchInfo researchInfo5 = researchLevelInfo.AddResearch(Quest.ID.ResearchTextiles3, 10000000);
		researchInfo5.AddRequiredObject(ObjectType.AnimalSilkworm);
		researchInfo5.AddUnlockedObject(ObjectType.SilkThread);
		researchInfo5.AddUnlockedObject(ObjectType.SilkCloth);
		researchInfo5.AddUnlockedObject(ObjectType.TopTuxedo);
		researchInfo5.AddUnlockedObject(ObjectType.TopGown);
		researchInfo5.AddUnlockedObject(ObjectType.TopSuit);
		ResearchInfo researchInfo6 = researchLevelInfo.AddResearch(Quest.ID.ResearchToys4, 10000000);
		researchInfo6.AddRequiredObject(ObjectType.Stick);
		researchInfo6.AddUnlockedObject(ObjectType.Spaceship);
		researchInfo6.AddUnlockedObject(ObjectType.ToyTrain);
		ResearchInfo researchInfo7 = researchLevelInfo.AddResearch(Quest.ID.ResearchHealth3, 10000000);
		researchInfo7.AddRequiredObject(ObjectType.MedicineFlowers);
		researchInfo7.AddUnlockedObject(ObjectType.MedicinePills);
		ResearchInfo researchInfo8 = researchLevelInfo.AddResearch(Quest.ID.ResearchCommunication2, 10000000);
		researchInfo8.AddRequiredObject(ObjectType.EducationBook1);
		researchInfo8.AddUnlockedObject(ObjectType.EducationEncyclopedia);
		ResearchInfo researchInfo9 = researchLevelInfo.AddResearch(Quest.ID.ResearchCulture, 10000000);
		researchInfo9.AddRequiredObject(ObjectType.FolkSeed);
		researchInfo9.AddUnlockedObject(ObjectType.Easel);
		researchInfo9.AddUnlockedObject(ObjectType.ArtPortrait);
		researchInfo9.AddUnlockedObject(ObjectType.ArtStillLife);
		researchInfo9.AddUnlockedObject(ObjectType.ArtAbstract);
		researchInfo9.AddUnlockedObject(ObjectType.Canvas);
		researchInfo9.AddUnlockedObject(ObjectType.PaintRed);
		researchInfo9.AddUnlockedObject(ObjectType.PaintYellow);
		researchInfo9.AddUnlockedObject(ObjectType.PaintBlue);
	}

	public ResearchData()
	{
		m_Levels = new List<ResearchLevelInfo>();
		SetupLevel1();
		SetupLevel2();
		SetupLevel3();
		SetupLevel4();
		SetupLevel5();
		SetupLevel6();
		ConvertToQuests();
	}
}
