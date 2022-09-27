public class QuestEvent
{
	public enum Type
	{
		Make,
		MakeConverter,
		Pickup,
		Store,
		Build,
		CompleteMission,
		CompleteTutorial,
		CompleteAnyMission,
		MakeAnyPorridge,
		MakeTop,
		MakeHat,
		MakeWorkerWithFrameMk1,
		MakeWorkerWithHeadMk1,
		MakeWorkerWithDriveMk1,
		MakeWorkerMk2,
		MakeCrudeTool,
		MakeCrudeMetalTool,
		MakeStructuralPart,
		MakePlank,
		UseRockingChairTop,
		UseRockingChairHat,
		PickupAnything,
		DropAnything,
		StoreLiquid,
		StoreParticulate,
		StoreFood,
		AddToStoragePalette,
		StorageUsed,
		Take,
		ScytheWheat,
		ScytheWheatWithScytheCrude,
		ScytheWheatWithScythe,
		ScytheCotton,
		ScytheBullrushes,
		ScytheGrass,
		ScythePumpkin,
		ScytheFlower,
		UseRockSharpOnCrops,
		FlailWheat,
		UseFlailCrude,
		UseFlail,
		FlailBush,
		ThreshWheat,
		ThreshWheatWithStick,
		ThreshCottonBalls,
		ThreshBullrushesStems,
		Chop,
		ChopTree,
		ChopTreeWithRock,
		ChopTreeWithAxeCrude,
		ChopTreeWithWoodAxe,
		ChopLog,
		ChopPlank,
		ChopPole,
		ProcessWood,
		Shovel,
		ShovelWithShovelCrude,
		ShovelWithShovel,
		Dig,
		DigWeed,
		DigMushroom,
		DigSoil,
		DigCarrot,
		HoeWithHoeCrude,
		HoeWithHoe,
		Hoe,
		MineWithPickaxeCrude,
		MineStone,
		MineStoneDeposits,
		MineClay,
		UsePickaxe,
		MineIron,
		MineCoal,
		MineTallBoulder,
		MineArea,
		ChiselWithChiselCrude,
		UseBucketCrude,
		UseBucket,
		FillBucket,
		FillBucketSand,
		FillBucketSandOrSoil,
		FillBucketHoney,
		DredgeWithCrudeDredger,
		LeechCaught,
		BashPumpkin,
		BashBoulderWithRock,
		BashBush,
		BashAppleTree,
		BashCoconutTree,
		PlantTreeSeed,
		PlantBerries,
		PlantWheat,
		PlantCotton,
		PlantBullrushes,
		PlantMushroom,
		PlantSeedling,
		PlantMulberrySeed,
		PlantSeedlingMulberry,
		PlantCropSeed,
		PlantManure,
		PlantPumpkinSeeds,
		PlantFertiliser,
		PlantApple,
		PlantCarrotSeed,
		PlantCoconut,
		Carry,
		Move,
		MoveWater,
		MoveCanoe,
		MoveWheelbarrow,
		MoveCart,
		UseClayStationCrude,
		UseOvenCrude,
		UseWorkbench,
		UseWater,
		CatchBait,
		CatchFish,
		ForageFood,
		MilkCow,
		MilkCowInMilkingShed,
		ShearSheep,
		ShearSheepInShearingShed,
		UpgradeBot,
		Research,
		CompleteResearch,
		Stack20Objects,
		MakeFolkHeart,
		FolkDied,
		RainOnFolk,
		FeedFolk,
		ClotheFolk,
		ToyFolk,
		MedicineFolk,
		EducateFolk,
		ArtFolk,
		FolkTranscended,
		MakeFolkHappy,
		MakeFolkHoused,
		BeeMakesHoney,
		ChickenCoopMakeEgg,
		FeedChicken,
		BirdEatCrops,
		Pen5Animals,
		PenCows,
		PenSheep,
		PenChooks,
		GrowWheat,
		GrowTree,
		GrowMushroom,
		GrowFlower,
		CreateTreeSeed,
		StowWhistleCrude,
		UseWhistle,
		GiveAxeToBot,
		GiveBotAnything,
		TakeBotAnything,
		BuildAnything,
		PlotUncovered,
		RechargeBot,
		TeachChopTree,
		TeachPickupLog,
		TeachAddToStoragePalette,
		SelectBot,
		ClickRecord,
		ClickRepeat,
		ClickPlay,
		ClickStop,
		ClickObject,
		Group3Bots,
		BotTeach,
		EditSearchArea,
		UseMaxArea,
		UseObjectArea,
		ObjectAreaSelect,
		EndEditSearchArea,
		RolloverBoulder,
		RolloverBush,
		RolloverCrops,
		BurnWood,
		Land,
		Communicate,
		UpdateStoredSticks,
		UpdateFedFolk,
		UpdateHousedFolk,
		UseObject,
		Stow,
		Recall,
		SelectBlueprint,
		AddBlueprint,
		EditMode,
		EndEditMode,
		EngageConverter,
		ConverterSelectObject,
		CloseBrain,
		SelectAutopedia,
		MoveCamera,
		ZoomCamera,
		RecentreCamera,
		UntilBuildingFullChosen,
		UntilHandsEmptyChosen,
		SelectAutopediaObjects,
		SelectAutopediaFood,
		SelectAutopediaObjectType,
		AltHover,
		BotServerComplete,
		SpacePortComplete,
		Total
	}

	private static string[] m_TypeNames;

	public Type m_Type;

	public bool m_BotOnly;

	public object m_ExtraData;

	public int m_Required;

	public int m_Progress;

	public bool m_Complete;

	public bool m_Completable;

	public ObjectType m_LockedObject;

	public string m_Description;

	public bool m_Locked;

	public QuestEvent(Type NewType, bool BotOnly, object ExtraData, int Required, string Description = "")
	{
		m_Type = NewType;
		m_BotOnly = BotOnly;
		m_ExtraData = ExtraData;
		m_Required = Required;
		m_Progress = 0;
		m_Complete = false;
		m_Locked = false;
		m_Description = Description;
	}

	public bool AddEvent(int Value)
	{
		if (m_Locked && m_Type != Type.Build)
		{
			return false;
		}
		if (m_Progress < m_Required)
		{
			int progress = m_Progress;
			if (m_Type == Type.UpdateStoredSticks)
			{
				m_Progress = StorageTypeManager.Instance.GetStored(ObjectType.Stick);
			}
			else if (m_Type == Type.UpdateFedFolk)
			{
				m_Progress = FolkManager.Instance.GetFedFolk();
			}
			else if (m_Type == Type.UpdateHousedFolk)
			{
				m_Progress = FolkManager.Instance.GetHousedFolk();
			}
			else if (m_Type == Type.MakeFolkHappy)
			{
				m_Progress = FolkManager.Instance.GetHappy();
			}
			else if (m_Type == Type.MakeFolkHoused)
			{
				m_Progress = FolkManager.Instance.GetHoused();
			}
			else if (m_Type == Type.PenCows || m_Type == Type.PenSheep || m_Type == Type.PenChooks)
			{
				m_Progress = Value;
			}
			else
			{
				m_Progress += Value;
			}
			if (m_Type == Type.Research && CheatManager.Instance.m_CheapResearch)
			{
				m_Progress = m_Required;
			}
			if (m_Progress >= m_Required)
			{
				m_Progress = m_Required;
				m_Complete = true;
			}
			else if (progress >= m_Progress)
			{
				return false;
			}
			return true;
		}
		return false;
	}

	public void SetProgress(int Progress)
	{
		m_Progress = Progress;
		if (m_Progress >= m_Required)
		{
			m_Progress = m_Required;
			m_Complete = true;
		}
	}

	public void Reset()
	{
		m_Progress = 0;
		m_Complete = false;
	}

	public string GetExtraDataAsString()
	{
		if (DoesTypeNeedExtraDataObject(m_Type))
		{
			return ObjectTypeList.Instance.GetSaveNameFromIdentifier((ObjectType)m_ExtraData);
		}
		if (DoesTypeNeedExtraDataMission(m_Type))
		{
			return QuestData.Instance.GetQuestNameFromID((Quest.ID)m_ExtraData);
		}
		if (DoesTypeNeedExtraDataTileType(m_Type))
		{
			return Tile.GetNameFromType((Tile.TileType)m_ExtraData);
		}
		return "";
	}

	public static object GetExtraDataFromString(Type TestEvent, string ExtraData)
	{
		if (DoesTypeNeedExtraDataObject(TestEvent))
		{
			return ObjectTypeList.Instance.GetIdentifierFromSaveName(ExtraData);
		}
		if (DoesTypeNeedExtraDataMission(TestEvent))
		{
			return QuestData.Instance.GetQuestIDFromName(ExtraData);
		}
		if (DoesTypeNeedExtraDataTileType(TestEvent))
		{
			return Tile.GetTypeFromName(ExtraData);
		}
		return null;
	}

	public void SetExtraDataFromString(string ExtraData)
	{
		m_ExtraData = GetExtraDataFromString(m_Type, ExtraData);
	}

	public bool DoesTypeMatch(Type TestEvent, bool BotOnly, object ExtraData)
	{
		if (m_Type != TestEvent)
		{
			return false;
		}
		if (DoesTypeNeedExtraDataObject(m_Type) && (ObjectType)ExtraData != (ObjectType)m_ExtraData)
		{
			return false;
		}
		if (DoesTypeNeedExtraDataMission(TestEvent) && (Quest.ID)ExtraData != (Quest.ID)m_ExtraData)
		{
			return false;
		}
		if (DoesTypeNeedExtraDataTileType(TestEvent) && (Tile.TileType)ExtraData != (Tile.TileType)m_ExtraData)
		{
			return false;
		}
		return true;
	}

	public string GetDisplayString()
	{
		string nameFromType = GetNameFromType(m_Type);
		return TextManager.Instance.Get(nameFromType, GetExtraDataString());
	}

	public string GetExtraDataString()
	{
		if (DoesTypeNeedExtraDataObject(m_Type))
		{
			return ObjectTypeList.Instance.GetHumanReadableNameFromIdentifier((ObjectType)m_ExtraData);
		}
		if (DoesTypeNeedExtraDataMission(m_Type))
		{
			string tag = ((Quest.ID)m_ExtraData).ToString();
			return TextManager.Instance.Get(tag);
		}
		if (DoesTypeNeedExtraDataTileType(m_Type))
		{
			string nameFromType = Tile.GetNameFromType((Tile.TileType)m_ExtraData);
			return TextManager.Instance.Get(nameFromType);
		}
		return "";
	}

	public static bool DoesTypeNeedBuildingObjects(Type NewType)
	{
		if (NewType == Type.Build || NewType == Type.EngageConverter)
		{
			return true;
		}
		return false;
	}

	public static bool DoesTypeNeedExtraDataObject(Type NewType)
	{
		if (NewType == Type.Make || NewType == Type.Pickup || NewType == Type.Build || NewType == Type.Store || NewType == Type.StorageUsed || NewType == Type.ClickObject || NewType == Type.EngageConverter || NewType == Type.ConverterSelectObject || NewType == Type.GiveBotAnything || NewType == Type.SelectBlueprint || NewType == Type.AddBlueprint || NewType == Type.ObjectAreaSelect || NewType == Type.Take || NewType == Type.MakeConverter || NewType == Type.SelectAutopediaObjectType)
		{
			return true;
		}
		return false;
	}

	public static bool DoesTypeNeedExtraDataMission(Type NewType)
	{
		if (NewType == Type.CompleteMission)
		{
			return true;
		}
		return false;
	}

	public static bool DoesTypeNeedExtraDataTileType(Type NewType)
	{
		if (NewType == Type.MineArea)
		{
			return true;
		}
		return false;
	}

	public bool DoesTypeNeedExtraDataObject()
	{
		return DoesTypeNeedExtraDataObject(m_Type);
	}

	public bool DoesTypeNeedExtraDataMission()
	{
		return DoesTypeNeedExtraDataMission(m_Type);
	}

	public bool DoesTypeNeedExtraDataTileType()
	{
		return DoesTypeNeedExtraDataTileType(m_Type);
	}

	public static void Init()
	{
		int num = 206;
		m_TypeNames = new string[num];
		for (int i = 0; i < num; i++)
		{
			string[] typeNames = m_TypeNames;
			int num2 = i;
			Type type = (Type)i;
			typeNames[num2] = "Event" + type;
		}
	}

	public static string GetNameFromType(Type NewType)
	{
		return m_TypeNames[(int)NewType];
	}

	public static Type GetTypeFromName(string Name)
	{
		for (int i = 0; i < m_TypeNames.Length; i++)
		{
			if (m_TypeNames[i] == Name)
			{
				return (Type)i;
			}
		}
		return Type.Total;
	}

	public void UpdateCanBeCompleted(Quest Parent)
	{
		m_LockedObject = ObjectTypeList.m_Total;
		m_Completable = true;
		if (DoesTypeNeedExtraDataObject())
		{
			if (m_ExtraData == null)
			{
				ErrorMessage.LogError(string.Concat("Bad data for Quest ", Parent.m_ID, " Event ", m_Type));
			}
			m_LockedObject = (ObjectType)m_ExtraData;
			if (m_LockedObject != ObjectTypeList.m_Total && m_LockedObject != 0)
			{
				if (QuestManager.Instance.GetIsObjectLocked(m_LockedObject) || QuestManager.Instance.GetIsBuildingLocked(m_LockedObject))
				{
					m_Completable = false;
				}
				else
				{
					IngredientRequirement[] ingredientsFromIdentifier = ObjectTypeList.Instance.GetIngredientsFromIdentifier(m_LockedObject);
					for (int i = 0; i < ingredientsFromIdentifier.Length; i++)
					{
						IngredientRequirement ingredientRequirement = ingredientsFromIdentifier[i];
						if (QuestManager.Instance.GetIsObjectLocked(ingredientRequirement.m_Type))
						{
							m_Completable = false;
						}
					}
				}
			}
		}
		if (m_Type != Type.Research)
		{
			return;
		}
		m_LockedObject = ObjectType.ResearchStationCrude;
		if (QuestManager.Instance.GetIsBuildingLocked(m_LockedObject))
		{
			m_Completable = false;
		}
		if (Parent.m_ObjectTypeRequired != ObjectTypeList.m_Total && Parent.m_ObjectTypeRequired != 0)
		{
			m_LockedObject = Parent.m_ObjectTypeRequired;
			if (QuestManager.Instance.GetIsObjectLocked(m_LockedObject))
			{
				m_Completable = false;
			}
		}
	}

	public bool CanBeCompleted()
	{
		return m_Completable;
	}
}
