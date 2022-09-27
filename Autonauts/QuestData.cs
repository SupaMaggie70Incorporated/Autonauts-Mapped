using System.Collections.Generic;

public class QuestData
{
	public static QuestData Instance;

	public Quest[] m_QuestData;

	public IndustryData m_IndustryData;

	public TutorialData m_TutorialData;

	public AcademyData m_AcademyData;

	public ResearchData m_ResearchData;

	private List<GlueInfo> m_GlueInfos;

	public void AddQuest(Quest.ID NewID, Quest NewQuest)
	{
		m_QuestData[(int)NewID] = NewQuest;
	}

	public void Init()
	{
		Instance = this;
		m_QuestData = new Quest[155];
		m_TutorialData = new TutorialData();
		m_AcademyData = new AcademyData();
		m_ResearchData = new ResearchData();
		MakeGlueMissions();
		SetQuestLinks();
		Quest[] questData = m_QuestData;
		for (int i = 0; i < questData.Length; i++)
		{
			questData[i]?.UpdateEventsCompletable();
		}
	}

	public Quest GetQuest(Quest.ID NewID)
	{
		Quest[] questData = m_QuestData;
		foreach (Quest quest in questData)
		{
			if (quest != null && quest.m_ID == NewID)
			{
				return quest;
			}
		}
		return null;
	}

	public Quest.ID GetQuestIDFromName(string Name)
	{
		for (int i = 0; i < 155; i++)
		{
			if (GetQuestNameFromID((Quest.ID)i) == Name)
			{
				return (Quest.ID)i;
			}
		}
		return Quest.ID.Total;
	}

	public string GetQuestNameFromID(Quest.ID NewID)
	{
		return NewID.ToString();
	}

	public bool DoesQuestUnlockQuest(Quest.ID NewID)
	{
		Quest[] questData = m_QuestData;
		foreach (Quest quest in questData)
		{
			if (quest != null && quest.m_QuestsUnlocked.Contains(NewID))
			{
				return true;
			}
		}
		return false;
	}

	public static bool DoesUnlockedObjectHaveCeremony(ObjectType NewType)
	{
		if (NewType == ObjectType.AnimalCow || NewType == ObjectType.AnimalSheep || NewType == ObjectType.ApplePieRaw || NewType == ObjectType.BricksCrudeRaw || NewType == ObjectType.FlowerPotRaw || NewType == ObjectType.PotClayRaw || NewType == ObjectType.LargeBowlClayRaw || NewType == ObjectType.PumpkinPieRaw || NewType == ObjectType.FishPieRaw || NewType == ObjectType.MushroomPieRaw || NewType == ObjectType.BerriesPieRaw || NewType == ObjectType.NaanRaw || NewType == ObjectType.BerriesCakeRaw || NewType == ObjectType.AppleCakeRaw || NewType == ObjectType.BreadPuddingRaw || NewType == ObjectType.RoofTilesRaw || NewType == ObjectType.JarClayRaw || NewType == ObjectType.GnomeRaw || NewType == ObjectType.FishCakeRaw || NewType == ObjectType.CarrotCakeRaw || NewType == ObjectType.PumpkinCakeRaw || NewType == ObjectType.MushroomPuddingRaw || NewType == ObjectType.AppleBerryPieRaw || NewType == ObjectType.PumpkinMushroomPieRaw)
		{
			return false;
		}
		return true;
	}

	public Quest GetQuestFromUnlockedObject(ObjectType NewType)
	{
		Quest[] questData = m_QuestData;
		foreach (Quest quest in questData)
		{
			if (quest != null && (quest.m_BuildingsUnlocked.Contains(NewType) || quest.m_ObjectsUnlocked.Contains(NewType)))
			{
				return quest;
			}
		}
		return null;
	}

	public Quest GetResearchQuestFromObject(ObjectType NewType)
	{
		Quest[] questData = m_QuestData;
		foreach (Quest quest in questData)
		{
			if (quest != null && quest.m_Type == Quest.Type.Research && quest.m_ObjectTypeRequired == NewType)
			{
				return quest;
			}
		}
		return null;
	}

	public void SetQuestLinks()
	{
		Quest[] questData = m_QuestData;
		foreach (Quest quest in questData)
		{
			if (quest == null)
			{
				continue;
			}
			foreach (Quest.ID item in quest.m_QuestsUnlocked)
			{
				GetQuest(item)?.AddReliesOn(quest.m_ID);
			}
		}
	}

	public void ConvertToQuest(int Index)
	{
		GlueInfo glueInfo = m_GlueInfos[Index];
		Quest quest = new Quest();
		quest.m_Simple = false;
		foreach (QuestEvent @event in glueInfo.m_Events)
		{
			quest.AddEvent(@event.m_Type, @event.m_BotOnly, @event.m_ExtraData, @event.m_Required);
		}
		foreach (Quest.ID requiredQuest in glueInfo.m_RequiredQuests)
		{
			quest.AddEvent(QuestEvent.Type.CompleteMission, BotOnly: false, requiredQuest, 1);
		}
		if (glueInfo.m_ID == Quest.ID.GlueBasics)
		{
			quest.AddEvent(QuestEvent.Type.CompleteTutorial, BotOnly: false, null, 1);
		}
		foreach (Quest.ID unlockedQuest in glueInfo.m_UnlockedQuests)
		{
			quest.AddQuestUnlocked(unlockedQuest);
		}
		foreach (ObjectType unlockedObject in glueInfo.m_UnlockedObjects)
		{
			quest.AddObjectUnlocked(unlockedObject);
		}
		string text = glueInfo.m_ID.ToString();
		quest.SetInfo(glueInfo.m_ID, Quest.Category.Total, text, text, text + "Desc", null, null, glueInfo.m_CeremonyType, glueInfo.m_QuestType);
		Instance.AddQuest(glueInfo.m_ID, quest);
	}

	private GlueInfo AddGlueMission(Quest.ID NewID, CeremonyManager.CeremonyType NewCeremonyType, Quest.Type NewQuestType)
	{
		GlueInfo glueInfo = new GlueInfo(NewID, NewCeremonyType, NewQuestType);
		m_GlueInfos.Add(glueInfo);
		return glueInfo;
	}

	public void ConvertToQuests()
	{
		for (int i = 0; i < m_GlueInfos.Count; i++)
		{
			ConvertToQuest(i);
		}
	}

	private void FinishGlueFinal(GlueInfo NewGlueInfo)
	{
		foreach (CertificateInfo certificateInfo in m_AcademyData.m_CertificateInfos)
		{
			if (certificateInfo.m_ID == Quest.ID.AcademyBasics || certificateInfo.m_ID == Quest.ID.AcademyColonisation8)
			{
				continue;
			}
			bool flag = false;
			foreach (GlueInfo glueInfo in m_GlueInfos)
			{
				if (glueInfo.m_UnlockedQuests.Contains(certificateInfo.m_ID))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				NewGlueInfo.AddUnlockedQuest(certificateInfo.m_ID);
			}
		}
	}

	private void MakeGlueMissions()
	{
		m_GlueInfos = new List<GlueInfo>();
		GlueInfo glueInfo = AddGlueMission(Quest.ID.GlueFirstFreeBot, CeremonyManager.CeremonyType.TutorialFreeBot, Quest.Type.FreeBot);
		glueInfo.AddRequiredQuest(Quest.ID.TutorialTeaching);
		glueInfo = AddGlueMission(Quest.ID.GlueSecondFreeBot, CeremonyManager.CeremonyType.TutorialFreeBot, Quest.Type.FreeBot);
		glueInfo.AddRequiredQuest(Quest.ID.TutorialTeaching2);
		glueInfo = AddGlueMission(Quest.ID.GlueBasics, CeremonyManager.CeremonyType.GlueComplete, Quest.Type.Glue);
		glueInfo.AddUnlockedQuest(Quest.ID.AcademyForestry);
		glueInfo = AddGlueMission(Quest.ID.GlueForestry, CeremonyManager.CeremonyType.GlueComplete, Quest.Type.Glue);
		glueInfo.AddRequiredQuest(Quest.ID.AcademyForestry);
		glueInfo.AddUnlockedQuest(Quest.ID.AcademyForestry2);
		glueInfo.AddUnlockedQuest(Quest.ID.AcademyTools);
		glueInfo.AddUnlockedQuest(Quest.ID.AcademyRobotics);
		glueInfo.AddUnlockedQuest(Quest.ID.AcademyLumber2);
		glueInfo.AddUnlockedQuest(Quest.ID.AcademyMining);
		glueInfo = AddGlueMission(Quest.ID.GlueFirstIndustries, CeremonyManager.CeremonyType.GlueComplete, Quest.Type.Glue);
		glueInfo.AddRequiredQuest(Quest.ID.AcademyForestry2);
		glueInfo.AddRequiredQuest(Quest.ID.AcademyTools);
		glueInfo.AddRequiredQuest(Quest.ID.AcademyRobotics);
		glueInfo.AddRequiredQuest(Quest.ID.AcademyLumber2);
		glueInfo.AddRequiredQuest(Quest.ID.AcademyMining);
		glueInfo.AddUnlockedQuest(Quest.ID.AcademyFarmingFruit);
		glueInfo.AddUnlockedQuest(Quest.ID.AcademyFarmingMushrooms);
		glueInfo.AddUnlockedQuest(Quest.ID.AcademyColonisation);
		glueInfo = AddGlueMission(Quest.ID.GlueFinal, CeremonyManager.CeremonyType.GlueComplete, Quest.Type.Glue);
		glueInfo.AddRequiredQuest(Quest.ID.AcademyFarmingFruit);
		glueInfo.AddRequiredQuest(Quest.ID.AcademyFarmingMushrooms);
		glueInfo.AddRequiredQuest(Quest.ID.AcademyColonisation);
		FinishGlueFinal(glueInfo);
		glueInfo = AddGlueMission(Quest.ID.GlueBotServer, CeremonyManager.CeremonyType.Total, Quest.Type.Glue);
		glueInfo.AddEvent(QuestEvent.Type.BotServerComplete, BotOnly: false, null, 1);
		glueInfo = AddGlueMission(Quest.ID.GlueSpacePort, CeremonyManager.CeremonyType.SpacePortComplete, Quest.Type.Glue);
		glueInfo.AddEvent(QuestEvent.Type.SpacePortComplete, BotOnly: false, null, 1);
		glueInfo = AddGlueMission(Quest.ID.GlueTranscendenceComplete, CeremonyManager.CeremonyType.Total, Quest.Type.Glue);
		glueInfo.AddEvent(QuestEvent.Type.Build, BotOnly: false, ObjectType.TranscendBuilding, 1);
		ConvertToQuests();
	}
}
