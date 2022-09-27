using System.Collections.Generic;

public class TutorialData
{
	public enum Lesson
	{
		First,
		Scripting,
		Scripting2,
		Berries,
		Mushrooms,
		Total
	}

	private Lesson m_CurrentLesson;

	public List<TutorialInfo> m_TutorialInfos;

	private Dictionary<Lesson, Quest.ID> m_Lessons;

	public TutorialInfo GetInfoFromQuestID(Quest.ID NewID)
	{
		foreach (TutorialInfo tutorialInfo in m_TutorialInfos)
		{
			if (tutorialInfo.m_ID == NewID)
			{
				return tutorialInfo;
			}
		}
		return null;
	}

	private bool GetLessonContainsQuest(Quest.ID ParentID, Quest.ID ChildID)
	{
		if (ParentID == ChildID)
		{
			return true;
		}
		foreach (TutorialInfo tutorialInfo in m_TutorialInfos)
		{
			if (tutorialInfo.m_ID == ParentID)
			{
				if (tutorialInfo.m_UnlockedQuests.Count != 0)
				{
					return GetLessonContainsQuest(tutorialInfo.m_UnlockedQuests[0], ChildID);
				}
				break;
			}
		}
		return false;
	}

	public Quest.ID GetFirstQuestFromQuest(Quest.ID NewID)
	{
		foreach (KeyValuePair<Lesson, Quest.ID> lesson in m_Lessons)
		{
			if (GetLessonContainsQuest(lesson.Value, NewID))
			{
				return lesson.Value;
			}
		}
		return Quest.ID.Total;
	}

	public void ConvertToQuest(int Index)
	{
		TutorialInfo tutorialInfo = m_TutorialInfos[Index];
		Quest quest = new Quest();
		quest.m_Simple = false;
		foreach (QuestEvent @event in tutorialInfo.m_Events)
		{
			quest.AddEvent(@event.m_Type, @event.m_BotOnly, @event.m_ExtraData, @event.m_Required, @event.m_Description);
		}
		foreach (ObjectType unlockedObject in tutorialInfo.m_UnlockedObjects)
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
		foreach (Quest.ID unlockedQuest in tutorialInfo.m_UnlockedQuests)
		{
			quest.AddQuestUnlocked(unlockedQuest);
		}
		string text = tutorialInfo.m_ID.ToString();
		CeremonyManager.CeremonyType newCeremonyType = CeremonyManager.CeremonyType.TutorialComplete;
		if (tutorialInfo.m_ID == Quest.ID.TutorialTools)
		{
			newCeremonyType = CeremonyManager.CeremonyType.FirstTool;
		}
		if (tutorialInfo.m_ID == Quest.ID.TutorialBerries || tutorialInfo.m_ID == Quest.ID.TutorialMushrooms)
		{
			newCeremonyType = CeremonyManager.CeremonyType.TutorialCompleteTop;
		}
		Quest.Type newType = Quest.Type.Tutorial;
		quest.SetInfo(tutorialInfo.m_ID, Quest.Category.Total, text, text, text + "Desc", null, null, newCeremonyType, newType);
		foreach (QuestEvent item in quest.m_EventsRequired)
		{
			item.m_Locked = true;
		}
		QuestData.Instance.AddQuest(tutorialInfo.m_ID, quest);
	}

	public void ConvertToQuests()
	{
		for (int i = 0; i < m_TutorialInfos.Count; i++)
		{
			ConvertToQuest(i);
		}
	}

	private TutorialInfo AddTutorial(Quest.ID NewID)
	{
		if (!m_Lessons.ContainsKey(m_CurrentLesson))
		{
			m_Lessons.Add(m_CurrentLesson, NewID);
		}
		TutorialInfo tutorialInfo = new TutorialInfo(NewID, m_CurrentLesson);
		m_TutorialInfos.Add(tutorialInfo);
		return tutorialInfo;
	}

	private void AddFirstLesson()
	{
		m_CurrentLesson = Lesson.First;
		TutorialInfo tutorialInfo = AddTutorial(Quest.ID.TutorialStart);
		tutorialInfo.AddEvent(QuestEvent.Type.MoveCamera, BotOnly: false, null, 1, "TutorialCamera");
		tutorialInfo.AddEvent(QuestEvent.Type.ZoomCamera, BotOnly: false, null, 1, "TutorialCamera2");
		tutorialInfo.AddEvent(QuestEvent.Type.RecentreCamera, BotOnly: false, null, 1, "TutorialCamera3");
		tutorialInfo.AddEvent(QuestEvent.Type.PlotUncovered, BotOnly: false, null, 3, "TutorialExplore", TutorialPointerManager.Type.Explore);
		tutorialInfo.AddUnlockedQuest(Quest.ID.TutorialBasics);
		TutorialInfo tutorialInfo2 = AddTutorial(Quest.ID.TutorialBasics);
		tutorialInfo2.AddEvent(QuestEvent.Type.Pickup, BotOnly: false, ObjectType.Rock, 1, "TutorialPickup", TutorialPointerManager.Type.PickupRock);
		tutorialInfo2.AddEvent(QuestEvent.Type.Stow, BotOnly: false, null, 1, "TutorialStow", TutorialPointerManager.Type.Stow);
		tutorialInfo2.AddEvent(QuestEvent.Type.Recall, BotOnly: false, null, 1, "TutorialRecall", TutorialPointerManager.Type.Recall);
		tutorialInfo2.AddEvent(QuestEvent.Type.AltHover, BotOnly: false, null, 3, "TutorialAltHover");
		tutorialInfo2.AddEvent(QuestEvent.Type.ChopTree, BotOnly: false, null, 1, "TutorialUse", TutorialPointerManager.Type.ChopTree);
		tutorialInfo2.AddEvent(QuestEvent.Type.DropAnything, BotOnly: false, null, 1, "TutorialDrop");
		tutorialInfo2.AddUnlockedObject(ObjectType.Workbench);
		tutorialInfo2.AddUnlockedObject(ObjectType.ToolAxeStone);
		tutorialInfo2.AddUnlockedQuest(Quest.ID.TutorialFoundation);
		TutorialInfo tutorialInfo3 = AddTutorial(Quest.ID.TutorialFoundation);
		tutorialInfo3.AddEvent(QuestEvent.Type.EditMode, BotOnly: false, null, 1, "TutorialOpenEdit", TutorialPointerManager.Type.OpenEdit);
		tutorialInfo3.AddEvent(QuestEvent.Type.SelectBlueprint, BotOnly: false, ObjectType.Workbench, 1, "TutorialSelectWorkbench", TutorialPointerManager.Type.SelectWorkbench);
		tutorialInfo3.AddEvent(QuestEvent.Type.AddBlueprint, BotOnly: false, ObjectType.Workbench, 1, "TutorialDropWorkbench");
		tutorialInfo3.AddEvent(QuestEvent.Type.EndEditMode, BotOnly: false, null, 1, "TutorialCloseEditMode", TutorialPointerManager.Type.CloseEdit);
		tutorialInfo3.AddEvent(QuestEvent.Type.Build, BotOnly: false, ObjectType.Workbench, 1, "TutorialBuildWorkbench", TutorialPointerManager.Type.BuildWorkbench);
		tutorialInfo3.AddUnlockedQuest(Quest.ID.TutorialTools);
		TutorialInfo tutorialInfo4 = AddTutorial(Quest.ID.TutorialTools);
		tutorialInfo4.AddEvent(QuestEvent.Type.EngageConverter, BotOnly: false, ObjectType.Workbench, 1, "TutorialEngageWorkbench", TutorialPointerManager.Type.EngageWorkbench);
		tutorialInfo4.AddEvent(QuestEvent.Type.ConverterSelectObject, BotOnly: false, ObjectType.ToolAxeStone, 1, "TutorialSelectAxe", TutorialPointerManager.Type.SelectAxe);
		tutorialInfo4.AddEvent(QuestEvent.Type.Make, BotOnly: false, ObjectType.ToolAxeStone, 1, "TutorialAddIngredients", TutorialPointerManager.Type.AddIngredients);
		tutorialInfo4.AddUnlockedQuest(Quest.ID.TutorialLumber);
		TutorialInfo tutorialInfo5 = AddTutorial(Quest.ID.TutorialLumber);
		tutorialInfo5.AddEvent(QuestEvent.Type.Pickup, BotOnly: false, ObjectType.ToolAxeStone, 1, "TutorialPickupAxe");
		tutorialInfo5.AddEvent(QuestEvent.Type.ChopLog, BotOnly: false, null, 1, "TutorialChopLog", TutorialPointerManager.Type.ChopLog);
		tutorialInfo5.AddEvent(QuestEvent.Type.ChopPlank, BotOnly: false, null, 1, "TutorialChopPlank", TutorialPointerManager.Type.ChopPlank);
		tutorialInfo5.AddUnlockedObject(ObjectType.WorkerAssembler);
		tutorialInfo5.AddUnlockedObject(ObjectType.WorkerFrameMk0);
		tutorialInfo5.AddUnlockedObject(ObjectType.WorkerHeadMk0);
		tutorialInfo5.AddUnlockedObject(ObjectType.WorkerDriveMk0);
		tutorialInfo5.AddUnlockedObject(ObjectType.BasicWorker);
		tutorialInfo5.AddUnlockedQuest(Quest.ID.TutorialBotWorkshop);
		TutorialInfo tutorialInfo6 = AddTutorial(Quest.ID.TutorialBotWorkshop);
		tutorialInfo6.AddEvent(QuestEvent.Type.EditMode, BotOnly: false, null, 1, "TutorialBuildWorkerWorkbench", TutorialPointerManager.Type.OpenEdit);
		tutorialInfo6.AddEvent(QuestEvent.Type.SelectBlueprint, BotOnly: false, ObjectType.WorkerAssembler, 1, "TutorialSelectAssemblyUnit", TutorialPointerManager.Type.SelectAssemblyUnit);
		tutorialInfo6.AddEvent(QuestEvent.Type.AddBlueprint, BotOnly: false, ObjectType.WorkerAssembler, 1, "TutorialDropWorkbench");
		tutorialInfo6.AddEvent(QuestEvent.Type.EndEditMode, BotOnly: false, null, 1, "TutorialCloseEditMode", TutorialPointerManager.Type.CloseEdit);
		tutorialInfo6.AddEvent(QuestEvent.Type.Build, BotOnly: false, ObjectType.WorkerAssembler, 1, "TutorialBuildAssemblyUnit", TutorialPointerManager.Type.BuildWorkerWorkbench);
		tutorialInfo6.AddUnlockedQuest(Quest.ID.TutorialRobotics);
		TutorialInfo tutorialInfo7 = AddTutorial(Quest.ID.TutorialRobotics);
		tutorialInfo7.AddEvent(QuestEvent.Type.Make, BotOnly: false, ObjectType.BasicWorker, 1, "TutorialBuildBot4", TutorialPointerManager.Type.BuildBot);
		tutorialInfo7.AddEvent(QuestEvent.Type.RechargeBot, BotOnly: false, null, 1, "TutorialRechargeBot", TutorialPointerManager.Type.RechargeBot);
		tutorialInfo7.AddUnlockedQuest(Quest.ID.TutorialTools2);
		TutorialInfo tutorialInfo8 = AddTutorial(Quest.ID.TutorialTools2);
		tutorialInfo8.AddEvent(QuestEvent.Type.Make, BotOnly: false, ObjectType.ToolAxeStone, 1, "TutorialAddIngredients2", TutorialPointerManager.Type.AddIngredients);
		tutorialInfo8.AddEvent(QuestEvent.Type.GiveBotAnything, BotOnly: false, ObjectType.ToolAxeStone, 1, "TutorialGiveAxe", TutorialPointerManager.Type.GiveAxe);
		tutorialInfo8.AddUnlockedQuest(Quest.ID.TutorialTeaching);
		TutorialInfo tutorialInfo9 = AddTutorial(Quest.ID.TutorialTeaching);
		tutorialInfo9.AddEvent(QuestEvent.Type.UseWhistle, BotOnly: false, null, 1, "TutorialUseWhistle");
		tutorialInfo9.AddEvent(QuestEvent.Type.SelectBot, BotOnly: false, null, 1, "TutorialSelectBot", TutorialPointerManager.Type.SelectBot);
		tutorialInfo9.AddEvent(QuestEvent.Type.ClickRecord, BotOnly: false, null, 1, "TutorialClickRecord", TutorialPointerManager.Type.ClickRecord);
		tutorialInfo9.AddEvent(QuestEvent.Type.ChopTree, BotOnly: false, null, 1, "TutorialChopTree", TutorialPointerManager.Type.ChopTree);
		tutorialInfo9.AddEvent(QuestEvent.Type.ClickRepeat, BotOnly: false, null, 1, "TutorialRepeat", TutorialPointerManager.Type.Repeat);
		tutorialInfo9.AddEvent(QuestEvent.Type.ClickPlay, BotOnly: false, null, 1, "TutorialPlay", TutorialPointerManager.Type.Play);
		tutorialInfo9.AddUnlockedQuest(Quest.ID.TutorialRobotics2);
		TutorialInfo tutorialInfo10 = AddTutorial(Quest.ID.TutorialRobotics2);
		tutorialInfo10.AddEvent(QuestEvent.Type.CloseBrain, BotOnly: false, null, 1, "TutorialCloseBrain");
		tutorialInfo10.AddEvent(QuestEvent.Type.Make, BotOnly: false, ObjectType.BasicWorker, 1, "TutorialBuildBot2", TutorialPointerManager.Type.BuildBot);
		tutorialInfo10.AddEvent(QuestEvent.Type.RechargeBot, BotOnly: false, null, 1, "TutorialRechargeBot", TutorialPointerManager.Type.RechargeBot);
		tutorialInfo10.AddUnlockedObject(ObjectType.ToolShovelStone);
		tutorialInfo10.AddUnlockedQuest(Quest.ID.TutorialTools3);
		TutorialInfo tutorialInfo11 = AddTutorial(Quest.ID.TutorialTools3);
		tutorialInfo11.AddEvent(QuestEvent.Type.EngageConverter, BotOnly: false, ObjectType.Workbench, 1, "TutorialEngageWorkbench2", TutorialPointerManager.Type.EngageWorkbench);
		tutorialInfo11.AddEvent(QuestEvent.Type.ConverterSelectObject, BotOnly: false, ObjectType.ToolShovelStone, 1, "TutorialSelectShovel", TutorialPointerManager.Type.SelectShovel);
		tutorialInfo11.AddEvent(QuestEvent.Type.Make, BotOnly: false, ObjectType.ToolShovelStone, 1, "TutorialAddIngredients3", TutorialPointerManager.Type.AddIngredients);
		tutorialInfo11.AddUnlockedQuest(Quest.ID.TutorialTeaching2);
		TutorialInfo tutorialInfo12 = AddTutorial(Quest.ID.TutorialTeaching2);
		tutorialInfo12.AddEvent(QuestEvent.Type.UseWhistle, BotOnly: false, null, 1, "TutorialUseWhistle2", TutorialPointerManager.Type.UseWhistle);
		tutorialInfo12.AddEvent(QuestEvent.Type.SelectBot, BotOnly: false, null, 1, "TutorialSelectBot", TutorialPointerManager.Type.SelectBot);
		tutorialInfo12.AddEvent(QuestEvent.Type.ClickRecord, BotOnly: false, null, 1, "TutorialClickRecord", TutorialPointerManager.Type.ClickRecord);
		tutorialInfo12.AddEvent(QuestEvent.Type.DigSoil, BotOnly: false, null, 1, "TutorialDig", TutorialPointerManager.Type.DigSoil);
		tutorialInfo12.AddEvent(QuestEvent.Type.EditSearchArea, BotOnly: false, null, 1, "TutorialSetSearchArea", TutorialPointerManager.Type.EditSearchArea);
		tutorialInfo12.AddEvent(QuestEvent.Type.UseMaxArea, BotOnly: false, null, 1, "TutorialUseMaxArea", TutorialPointerManager.Type.MaxSearchArea);
		tutorialInfo12.AddEvent(QuestEvent.Type.EndEditSearchArea, BotOnly: false, null, 1, "TutorialEndEditSearchArea", TutorialPointerManager.Type.EndEditSearchArea);
		tutorialInfo12.AddEvent(QuestEvent.Type.ClickRepeat, BotOnly: false, null, 1, "TutorialRepeat", TutorialPointerManager.Type.Repeat);
		tutorialInfo12.AddEvent(QuestEvent.Type.GiveBotAnything, BotOnly: false, ObjectType.ToolShovelStone, 1, "TutorialGiveShovel", TutorialPointerManager.Type.GiveShovel);
		tutorialInfo12.AddEvent(QuestEvent.Type.ClickPlay, BotOnly: false, null, 1, "TutorialPlay2", TutorialPointerManager.Type.Play);
		tutorialInfo12.AddUnlockedQuest(Quest.ID.TutorialRobotics3);
		TutorialInfo tutorialInfo13 = AddTutorial(Quest.ID.TutorialRobotics3);
		tutorialInfo13.AddEvent(QuestEvent.Type.Make, BotOnly: false, ObjectType.BasicWorker, 1, "TutorialBuildBot3", TutorialPointerManager.Type.BuildBot);
		tutorialInfo13.AddEvent(QuestEvent.Type.RechargeBot, BotOnly: false, null, 1, "TutorialRechargeBot", TutorialPointerManager.Type.RechargeBot);
		tutorialInfo13.AddUnlockedQuest(Quest.ID.TutorialTeaching3);
		TutorialInfo tutorialInfo14 = AddTutorial(Quest.ID.TutorialTeaching3);
		tutorialInfo14.AddEvent(QuestEvent.Type.UseWhistle, BotOnly: false, null, 1, "TutorialUseWhistle3", TutorialPointerManager.Type.UseWhistle);
		tutorialInfo14.AddEvent(QuestEvent.Type.SelectBot, BotOnly: false, null, 1, "TutorialSelectBot", TutorialPointerManager.Type.SelectBot);
		tutorialInfo14.AddEvent(QuestEvent.Type.ClickRecord, BotOnly: false, null, 1, "TutorialClickRecord", TutorialPointerManager.Type.ClickRecord);
		tutorialInfo14.AddEvent(QuestEvent.Type.Pickup, BotOnly: false, ObjectType.TreeSeed, 1, "TutorialGetTreeSeed", TutorialPointerManager.Type.PickupTreeSeed);
		tutorialInfo14.AddEvent(QuestEvent.Type.PlantTreeSeed, BotOnly: false, null, 1, "TutorialPlantTreeSeed", TutorialPointerManager.Type.PlantTreeSeed);
		tutorialInfo14.AddEvent(QuestEvent.Type.ClickRepeat, BotOnly: false, null, 1, "TutorialRepeat", TutorialPointerManager.Type.Repeat);
		tutorialInfo14.AddEvent(QuestEvent.Type.ClickPlay, BotOnly: false, null, 1, "TutorialPlay2", TutorialPointerManager.Type.Play);
	}

	private void AddScriptingLesson()
	{
		m_CurrentLesson = Lesson.Scripting;
		TutorialInfo tutorialInfo = AddTutorial(Quest.ID.TutorialScripting);
		tutorialInfo.AddEvent(QuestEvent.Type.Make, BotOnly: false, ObjectType.BasicWorker, 1, "TutorialBuildBot4", TutorialPointerManager.Type.BuildBot);
		tutorialInfo.AddEvent(QuestEvent.Type.Build, BotOnly: false, ObjectType.StoragePalette, 1, "TutorialBuildPalette");
		tutorialInfo.AddEvent(QuestEvent.Type.Store, BotOnly: false, ObjectType.Log, 3, "TutorialStoreLogs", TutorialPointerManager.Type.StoreLogs);
		tutorialInfo.AddEvent(QuestEvent.Type.Build, BotOnly: false, ObjectType.ChoppingBlock, 1, "TutorialBuildChoppingBlock");
		tutorialInfo.AddEvent(QuestEvent.Type.ConverterSelectObject, BotOnly: false, ObjectType.Plank, 1, "TutorialSelectOutput", TutorialPointerManager.Type.EngageChoppingBlock);
		tutorialInfo.AddUnlockedQuest(Quest.ID.TutorialScripting2);
		TutorialInfo tutorialInfo2 = AddTutorial(Quest.ID.TutorialScripting2);
		tutorialInfo2.AddEvent(QuestEvent.Type.ClickRecord, BotOnly: false, null, 1, "TutorialRecordAgain", TutorialPointerManager.Type.ClickRecord);
		tutorialInfo2.AddEvent(QuestEvent.Type.Take, BotOnly: false, ObjectType.Log, 1, "TutorialTakeLog", TutorialPointerManager.Type.TakeLogs);
		tutorialInfo2.AddEvent(QuestEvent.Type.MakePlank, BotOnly: false, null, 1, "TutorialMakePlank", TutorialPointerManager.Type.EngageChoppingBlock);
		tutorialInfo2.AddEvent(QuestEvent.Type.ClickRepeat, BotOnly: false, null, 1, "TutorialRepeatAgain", TutorialPointerManager.Type.Repeat);
		tutorialInfo2.AddEvent(QuestEvent.Type.ClickPlay, BotOnly: false, null, 1, "TutorialPlayAgain", TutorialPointerManager.Type.Play);
		tutorialInfo2.AddUnlockedQuest(Quest.ID.TutorialScripting3);
		TutorialInfo tutorialInfo3 = AddTutorial(Quest.ID.TutorialScripting3);
		tutorialInfo3.AddEvent(QuestEvent.Type.ClickStop, BotOnly: false, null, 1, "TutorialStop", TutorialPointerManager.Type.Stop);
		tutorialInfo3.AddEvent(QuestEvent.Type.ClickRecord, BotOnly: false, null, 1, "TutorialRecordAgain2", TutorialPointerManager.Type.ClickRecord);
		tutorialInfo3.AddEvent(QuestEvent.Type.UntilBuildingFullChosen, BotOnly: false, ObjectType.ChoppingBlock, 1, "TutorialSelectFull", TutorialPointerManager.Type.RepeatDropdown);
		tutorialInfo3.AddEvent(QuestEvent.Type.UseObjectArea, BotOnly: false, ObjectType.ChoppingBlock, 1, "TutorialSelectTarget", TutorialPointerManager.Type.RepeatTargetButton);
		tutorialInfo3.AddEvent(QuestEvent.Type.ObjectAreaSelect, BotOnly: false, ObjectType.ChoppingBlock, 1, "TutorialSelectTarget2", TutorialPointerManager.Type.EngageChoppingBlock);
		tutorialInfo3.AddEvent(QuestEvent.Type.ClickRepeat, BotOnly: false, null, 1, "TutorialRepeatAgain2", TutorialPointerManager.Type.Repeat);
		tutorialInfo3.AddEvent(QuestEvent.Type.ClickPlay, BotOnly: false, null, 1, "TutorialPlayAgain2", TutorialPointerManager.Type.Play);
	}

	private void AddScripting2Lesson()
	{
		m_CurrentLesson = Lesson.Scripting2;
		TutorialInfo tutorialInfo = AddTutorial(Quest.ID.TutorialScripting4);
		tutorialInfo.AddEvent(QuestEvent.Type.Make, BotOnly: false, ObjectType.Worker, 1, "TutorialMakeMiningBot", TutorialPointerManager.Type.BuildBot);
		tutorialInfo.AddEvent(QuestEvent.Type.Make, BotOnly: false, ObjectType.ToolPickStone, 2, "TutorialMakePick", TutorialPointerManager.Type.EngageWorkbench);
		tutorialInfo.AddEvent(QuestEvent.Type.Pickup, BotOnly: false, ObjectType.ToolPickStone, 1, "TutorialGetPick", TutorialPointerManager.Type.PickupPick);
		tutorialInfo.AddUnlockedQuest(Quest.ID.TutorialScripting5);
		TutorialInfo tutorialInfo2 = AddTutorial(Quest.ID.TutorialScripting5);
		tutorialInfo2.AddEvent(QuestEvent.Type.ClickRecord, BotOnly: false, null, 1, "TutorialRecordMining", TutorialPointerManager.Type.ClickRecord);
		tutorialInfo2.AddEvent(QuestEvent.Type.MineStoneDeposits, BotOnly: false, null, 1, "TutorialMineStone", TutorialPointerManager.Type.MineStone);
		tutorialInfo2.AddEvent(QuestEvent.Type.ClickRepeat, BotOnly: false, null, 1, "TutorialMiningRepeat", TutorialPointerManager.Type.Repeat);
		tutorialInfo2.AddEvent(QuestEvent.Type.UntilHandsEmptyChosen, BotOnly: false, null, 1, "TutorialSelectHandsEmpty", TutorialPointerManager.Type.RepeatDropdown);
		tutorialInfo2.AddEvent(QuestEvent.Type.GiveBotAnything, BotOnly: false, ObjectType.ToolPickStone, 1, "TutorialGivePick", TutorialPointerManager.Type.GivePick);
		tutorialInfo2.AddEvent(QuestEvent.Type.Pickup, BotOnly: false, ObjectType.ToolPickStone, 1, "TutorialTeachGetTool", TutorialPointerManager.Type.PickupPick);
		tutorialInfo2.AddEvent(QuestEvent.Type.ClickRepeat, BotOnly: false, null, 1, "TutorialMiningRepeat2", TutorialPointerManager.Type.Repeat);
		tutorialInfo2.AddEvent(QuestEvent.Type.ClickPlay, BotOnly: false, null, 1, "", TutorialPointerManager.Type.Play);
	}

	private void AddBerriesLesson()
	{
		m_CurrentLesson = Lesson.Berries;
		TutorialInfo tutorialInfo = AddTutorial(Quest.ID.TutorialBerries);
		tutorialInfo.AddEvent(QuestEvent.Type.SelectAutopedia, BotOnly: false, null, 1, "TutorialSelectAutopedia", TutorialPointerManager.Type.Autopedia);
		tutorialInfo.AddEvent(QuestEvent.Type.SelectAutopediaObjects, BotOnly: false, null, 1, "TutorialSelectAutopediaObjects", TutorialPointerManager.Type.AutopediaObjects);
		tutorialInfo.AddEvent(QuestEvent.Type.SelectAutopediaFood, BotOnly: false, null, 1, "TutorialSelectAutopediaFood", TutorialPointerManager.Type.AutopediaFood);
		tutorialInfo.AddEvent(QuestEvent.Type.SelectAutopediaObjectType, BotOnly: false, ObjectType.Berries, 1, "TutorialSelectAutopediaBerries", TutorialPointerManager.Type.AutopediaBerries);
	}

	private void AddMushroomsLesson()
	{
		m_CurrentLesson = Lesson.Mushrooms;
		TutorialInfo tutorialInfo = AddTutorial(Quest.ID.TutorialMushrooms);
		tutorialInfo.AddEvent(QuestEvent.Type.SelectAutopedia, BotOnly: false, null, 1, "TutorialSelectAutopedia", TutorialPointerManager.Type.Autopedia);
		tutorialInfo.AddEvent(QuestEvent.Type.SelectAutopediaObjects, BotOnly: false, null, 1, "TutorialSelectAutopediaObjects", TutorialPointerManager.Type.AutopediaObjects);
		tutorialInfo.AddEvent(QuestEvent.Type.SelectAutopediaFood, BotOnly: false, null, 1, "TutorialSelectAutopediaFood", TutorialPointerManager.Type.AutopediaFood);
		tutorialInfo.AddEvent(QuestEvent.Type.SelectAutopediaObjectType, BotOnly: false, ObjectType.MushroomDug, 1, "TutorialSelectAutopediaMushrooms", TutorialPointerManager.Type.AutopediaMushrooms);
	}

	public TutorialData()
	{
		m_TutorialInfos = new List<TutorialInfo>();
		m_Lessons = new Dictionary<Lesson, Quest.ID>();
		AddFirstLesson();
		AddScriptingLesson();
		AddScripting2Lesson();
		AddBerriesLesson();
		AddMushroomsLesson();
		ConvertToQuests();
	}
}
