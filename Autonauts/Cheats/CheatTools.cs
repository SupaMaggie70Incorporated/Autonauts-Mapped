using System;
using System.Collections.Generic;
using UnityEngine;

public class CheatTools : CreativeTools
{
	private enum CheatType
	{
		ForceError,
		GetBadge,
		SkipTutorial,
		EndTutorial,
		AddTickets,
		EndColonisationThree,
		EndPhaseOne,
		EndPhaseTwo,
		EndPhaseThree,
		EndGame,
		CompleteOffworldMission,
		AllPrizes,
		CheatMissions,
		UnlockAllMissions,
		AllMissions,
		AllBuildings,
		AllObjects,
		AllPlots,
		FreeBuild,
		FastEat,
		CheapResearch,
		FillStorage,
		DrainBot,
		SlowTime,
		FastTime,
		DisableRain,
		ToggleRain,
		Materials,
		Terraform,
		CheckAll,
		DestroyHeld,
		ClearError,
		Cursor,
		TimeSlider,
		Total
	}

	private static float m_CheatsScrollViewPosition;

	private BaseScrollView m_CheatsScrollView;

	private BaseGadget[] m_CheatButtons;

	private Transform m_ContentTrans;

	private StandardButtonText m_DefaultButton;

	private BaseSlider m_DefaultSlider;

	private BaseToggle m_DefaultToggle;

	private float m_GadgetY;

	private void OnDestroy()
	{
		HudManager.Instance.ActivateUIRollover(Activate: false, "", default(Vector3));
		m_CheatsScrollViewPosition = m_CheatsScrollView.GetScrollValue();
	}

	public override void Init(bool JustBuildings, bool Everything)
	{
		base.Init(JustBuildings, Everything);
		base.transform.Find("BasePanelOptions").GetComponent<BasePanelOptions>();
		m_CheatsScrollView = base.transform.Find("BasePanelOptions").Find("CheatsScrollView").GetComponent<BaseScrollView>();
		CreateCheatButtons();
		m_CheatsScrollView.SetScrollValue(m_CheatsScrollViewPosition);
	}

	private BaseGadget CreateGadget(BaseGadget Prefab, CheatType NewType, Action<BaseGadget> NewAction)
	{
		BaseGadget baseGadget = UnityEngine.Object.Instantiate(Prefab, m_ContentTrans);
		AddAction(baseGadget, NewAction);
		baseGadget.SetActive(Active: true);
		baseGadget.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, m_GadgetY);
		m_GadgetY -= baseGadget.GetComponent<RectTransform>().sizeDelta.y + 5f;
		m_CheatButtons[(int)NewType] = baseGadget;
		return baseGadget;
	}

	private void CreateToggle(CheatType NewType, Action<BaseGadget> NewAction)
	{
		CreateGadget(m_DefaultToggle, NewType, NewAction).GetComponent<BaseToggle>().transform.Find("BaseText").GetComponent<BaseText>().SetTextFromID("ObjectSelectCheat" + NewType);
	}

	private void CreateButton(CheatType NewType, Action<BaseGadget> NewAction)
	{
		CreateGadget(m_DefaultButton, NewType, NewAction).GetComponent<StandardButtonText>().SetTextFromID("ObjectSelectCheat" + NewType);
	}

	private void CreateSlider(CheatType NewType, Action<BaseGadget> NewAction)
	{
		CreateGadget(m_DefaultSlider, NewType, NewAction).GetComponent<BaseSlider>().transform.Find("Text").GetComponent<BaseText>().SetTextFromID("ObjectSelectCheat" + NewType);
	}

	private void CreateCheatButtons()
	{
		m_ContentTrans = m_CheatsScrollView.GetContent().transform;
		m_DefaultButton = m_ContentTrans.Find("DefaultButton").GetComponent<StandardButtonText>();
		m_DefaultButton.SetActive(Active: false);
		m_DefaultSlider = m_ContentTrans.Find("DefaultSlider").GetComponent<BaseSlider>();
		m_DefaultSlider.SetActive(Active: false);
		m_DefaultToggle = m_ContentTrans.Find("DefaultToggle").GetComponent<BaseToggle>();
		m_DefaultToggle.SetActive(Active: false);
		m_GadgetY = -5f;
		m_CheatButtons = new BaseGadget[34];
		CreateButton(CheatType.ForceError, ForceErrorClicked);
		CreateButton(CheatType.GetBadge, GetBadgeClicked);
		CreateButton(CheatType.SkipTutorial, SkipTutorialClicked);
		CreateButton(CheatType.EndTutorial, EndTutorialClicked);
		CreateButton(CheatType.AddTickets, AddTicketsClicked);
		CreateButton(CheatType.EndColonisationThree, EndColonisationThreeClicked);
		CreateButton(CheatType.EndPhaseOne, EndPhaseOneClicked);
		CreateButton(CheatType.EndPhaseTwo, EndPhaseTwoClicked);
		CreateButton(CheatType.EndPhaseThree, EndPhaseThreeClicked);
		CreateButton(CheatType.EndGame, EndGameClicked);
		CreateButton(CheatType.CompleteOffworldMission, CompleteOffworldMissionsClicked);
		CreateToggle(CheatType.CheatMissions, CheatMissionsClicked);
		CreateButton(CheatType.UnlockAllMissions, UnlockAllMissionsClicked);
		CreateButton(CheatType.AllMissions, CompleteAllMissionsClicked);
		CreateButton(CheatType.AllBuildings, AllBuildingsClicked);
		CreateButton(CheatType.AllObjects, AllObjectsClicked);
		CreateButton(CheatType.AllPlots, AllPlotsClicked);
		CreateButton(CheatType.AllPrizes, AllPrizesClicked);
		CreateToggle(CheatType.FreeBuild, FreeBuildClicked);
		CreateToggle(CheatType.FastEat, FastEatClicked);
		CreateToggle(CheatType.CheapResearch, CheapResearchClicked);
		CreateToggle(CheatType.FillStorage, FillStorageClicked);
		CreateToggle(CheatType.DrainBot, DrainBotClicked);
		CreateToggle(CheatType.SlowTime, SlowTimeClicked);
		CreateToggle(CheatType.FastTime, FastTimeClicked);
		CreateToggle(CheatType.DisableRain, DisableRainClicked);
		CreateButton(CheatType.ToggleRain, ToggleRainClicked);
		CreateToggle(CheatType.Materials, MaterialsClicked);
		CreateButton(CheatType.Terraform, TerraformClicked);
		CreateToggle(CheatType.CheckAll, CheckAllClicked);
		CreateButton(CheatType.DestroyHeld, DestroyHeldClicked);
		CreateButton(CheatType.ClearError, ClearErrorClicked);
		CreateToggle(CheatType.Cursor, CursorClicked);
		CreateSlider(CheatType.TimeSlider, UpdateTimeSlider);
		m_CheatsScrollView.SetScrollSize(0f - m_GadgetY);
		UpdateButtons();
	}

	private void UpdateButtons()
	{
		m_CheatButtons[24].GetComponent<BaseToggle>().SetOn(TimeManager.Instance.GetIsFastTime());
		m_CheatButtons[23].GetComponent<BaseToggle>().SetOn(TimeManager.Instance.m_TimeScale == 0.1f);
		m_CheatButtons[18].GetComponent<BaseToggle>().SetOn(CheatManager.Instance.m_InstantBuild);
		m_CheatButtons[25].GetComponent<BaseToggle>().SetOn(!RainManager.Instance.m_IsEnabled);
		m_CheatButtons[21].GetComponent<BaseToggle>().SetOn(CheatManager.Instance.m_FillStorage);
		m_CheatButtons[22].GetComponent<BaseToggle>().SetOn(CheatManager.Instance.m_DrainBot);
		m_CheatButtons[29].GetComponent<BaseToggle>().SetOn(CheatManager.Instance.m_CheckAll);
		m_CheatButtons[12].GetComponent<BaseToggle>().SetOn(CheatManager.Instance.m_CheatMissions);
		m_CheatButtons[19].GetComponent<BaseToggle>().SetOn(CheatManager.Instance.m_FastEat);
		m_CheatButtons[32].GetComponent<BaseToggle>().SetOn(CheatManager.Instance.m_CursorClicks);
		m_CheatButtons[20].GetComponent<BaseToggle>().SetOn(CheatManager.Instance.m_CheapResearch);
		m_CheatButtons[27].GetComponent<BaseToggle>().SetOn(CheatManager.Instance.m_ShowMaterials);
	}

	public void ForceErrorClicked(BaseGadget NewGadget)
	{
		GameStateManager.Instance.PopState();
		ErrorMessage.Instance.SetMessage("This is an error message");
	}

	public void GetBadgeClicked(BaseGadget NewGadget)
	{
		CeremonyManager.Instance.AddCeremony(CeremonyManager.CeremonyType.BadgeRevealed, BadgeManager.Instance.m_BadgeData.m_Badges[0]);
		GameStateManager.Instance.PopState();
	}

	public void SkipTutorialClicked(BaseGadget NewGadget)
	{
		TutorialPanelController.Instance.SkipTutorial();
		GameStateManager.Instance.PopState();
	}

	public void EndTutorialClicked(BaseGadget NewGadget)
	{
		TutorialPanelController.Instance.EndTutorial(Complete: true);
		GameStateManager.Instance.PopState();
	}

	public void AddTicketsClicked(BaseGadget NewGadget)
	{
		OffworldMissionsManager.Instance.AwardTickets(100);
		GameStateManager.Instance.PopState();
	}

	public void EndColonisationThreeClicked(BaseGadget NewGadget)
	{
		QuestManager.Instance.AddQuest(Quest.ID.AcademyColonisation3);
		QuestManager.Instance.CheatCompleteQuest(QuestManager.Instance.GetQuest(Quest.ID.AcademyColonisation3), DoCeremony: true);
		GameStateManager.Instance.PopState();
	}

	public void EndPhaseOneClicked(BaseGadget NewGadget)
	{
		QuestManager.Instance.AddQuest(Quest.ID.ResearchPowerFuel2);
		QuestManager.Instance.DoResearch(null, Quest.ID.ResearchPowerFuel2, null, 1000000000);
		GameStateManager.Instance.PopState();
	}

	public void EndPhaseTwoClicked(BaseGadget NewGadget)
	{
		QuestManager.Instance.AddQuest(Quest.ID.AcademyColonisation6);
		QuestManager.Instance.CheatCompleteQuest(QuestManager.Instance.GetQuest(Quest.ID.AcademyColonisation6), DoCeremony: true);
		GameStateManager.Instance.PopState();
	}

	public void EndPhaseThreeClicked(BaseGadget NewGadget)
	{
		QuestManager.Instance.AddQuest(Quest.ID.AcademyColonisation7);
		QuestManager.Instance.CheatCompleteQuest(QuestManager.Instance.GetQuest(Quest.ID.AcademyColonisation7), DoCeremony: true);
		GameStateManager.Instance.PopState();
	}

	public void EndGameClicked(BaseGadget NewGadget)
	{
		QuestManager.Instance.AddQuest(Quest.ID.AcademyColonisation8);
		QuestManager.Instance.CheatCompleteQuest(QuestManager.Instance.GetQuest(Quest.ID.AcademyColonisation8), DoCeremony: true);
		GameStateManager.Instance.PopState();
	}

	public void CheatMissionsClicked(BaseGadget NewGadget)
	{
		CheatManager.Instance.m_CheatMissions = !CheatManager.Instance.m_CheatMissions;
		GameStateManager.Instance.PopState();
	}

	public void UnlockAllMissionsClicked(BaseGadget NewGadget)
	{
		QuestManager.Instance.UnlockAll();
		GameStateManager.Instance.PopState();
	}

	public void CompleteAllMissionsClicked(BaseGadget NewGadget)
	{
		QuestManager.Instance.CompleteAll();
		GameStateManager.Instance.PopState();
	}

	public void CompleteOffworldMissionsClicked(BaseGadget NewGadget)
	{
		OffworldMissionsManager.Instance.CompleteSelected();
		GameStateManager.Instance.PopState();
	}

	public void AllBuildingsClicked(BaseGadget NewGadget)
	{
		List<ObjectType> list = new List<ObjectType>();
		foreach (KeyValuePair<ObjectType, int> item in QuestManager.Instance.m_BuildingsLocked)
		{
			list.Add(item.Key);
		}
		foreach (ObjectType item2 in list)
		{
			if (ObjectTypeList.Instance.GetCategoryFromType(item2) != ObjectCategory.Prizes)
			{
				QuestManager.Instance.m_BuildingsLocked.Remove(item2);
			}
		}
		GameStateManager.Instance.PopState();
		ModeButton.Get(ModeButton.Type.BuildingPalette).Show();
		ModeButton.Get(ModeButton.Type.Evolution).Show();
	}

	public void AllObjectsClicked(BaseGadget NewGadget)
	{
		List<ObjectType> list = new List<ObjectType>();
		foreach (KeyValuePair<ObjectType, int> item in QuestManager.Instance.m_ObjectsLocked)
		{
			list.Add(item.Key);
		}
		foreach (ObjectType item2 in list)
		{
			if (ObjectTypeList.Instance.GetCategoryFromType(item2) != ObjectCategory.Prizes)
			{
				QuestManager.Instance.m_ObjectsLocked.Remove(item2);
			}
		}
		GameStateManager.Instance.PopState();
		ModeButton.Get(ModeButton.Type.BuildingPalette).Show();
	}

	public void AllPrizesClicked(BaseGadget NewGadget)
	{
		List<ObjectType> list = new List<ObjectType>();
		foreach (KeyValuePair<ObjectType, int> item in QuestManager.Instance.m_ObjectsLocked)
		{
			list.Add(item.Key);
		}
		foreach (ObjectType item2 in list)
		{
			if (ObjectTypeList.Instance.GetCategoryFromType(item2) == ObjectCategory.Prizes)
			{
				OffworldMissionsManager.Instance.BuyPrize(item2, Cheat: true);
			}
		}
		foreach (KeyValuePair<ObjectType, int> item3 in QuestManager.Instance.m_BuildingsLocked)
		{
			list.Add(item3.Key);
		}
		foreach (ObjectType item4 in list)
		{
			if (ObjectTypeList.Instance.GetCategoryFromType(item4) == ObjectCategory.Prizes)
			{
				OffworldMissionsManager.Instance.BuyPrize(item4, Cheat: true);
			}
		}
		GameStateManager.Instance.PopState();
		ModeButton.Get(ModeButton.Type.BuildingPalette).Show();
	}

	public void FreeBuildClicked(BaseGadget NewGadget)
	{
		CheatManager.Instance.m_InstantBuild = !CheatManager.Instance.m_InstantBuild;
		GameStateManager.Instance.PopState();
	}

	public void FastTimeClicked(BaseGadget NewGadget)
	{
		TimeManager.Instance.ToggleFastTime();
		GameStateManager.Instance.PopState();
	}

	public void SlowTimeClicked(BaseGadget NewGadget)
	{
		if (TimeManager.Instance.m_TimeScale != 0.1f)
		{
			TimeManager.Instance.SetTimeScale(0.1f);
		}
		else
		{
			TimeManager.Instance.SetTimeScale(1f);
		}
		GameStateManager.Instance.PopState();
	}

	public void TerraformClicked(BaseGadget NewGadget)
	{
		GameStateManager.Instance.PopState();
		GameStateManager.Instance.SetState(GameStateManager.State.Terraform);
	}

	public void UpdateTimeSlider(BaseGadget NewGadget)
	{
		BaseSlider component = m_CheatButtons[33].GetComponent<BaseSlider>();
		DayNightManager.Instance.SetTime(component.GetValue());
	}

	public void DisableRainClicked(BaseGadget NewGadget)
	{
		RainManager.Instance.SetEnabled(!RainManager.Instance.m_IsEnabled);
		GameStateManager.Instance.PopState();
	}

	public void ToggleRainClicked(BaseGadget NewGadget)
	{
		RainManager.Instance.ToggleRain();
		GameStateManager.Instance.PopState();
	}

	public void FillStorageClicked(BaseGadget NewGadget)
	{
		CheatManager.Instance.m_FillStorage = !CheatManager.Instance.m_FillStorage;
		GameStateManager.Instance.PopState();
	}

	public void DestroyHeldClicked(BaseGadget NewGadget)
	{
		CollectionManager.Instance.GetPlayers()[0].GetComponent<FarmerPlayer>().m_FarmerCarry.DestroyAllObjects();
		GameStateManager.Instance.PopState();
	}

	public void DrainBotClicked(BaseGadget NewGadget)
	{
		CheatManager.Instance.m_DrainBot = !CheatManager.Instance.m_DrainBot;
		GameStateManager.Instance.PopState();
	}

	public void ClearErrorClicked(BaseGadget NewGadget)
	{
		ErrorMessage.Instance.Clear();
	}

	public void CheckAllClicked(BaseGadget NewGadget)
	{
		CheatManager.Instance.m_CheckAll = !CheatManager.Instance.m_CheckAll;
		GameStateManager.Instance.PopState();
	}

	public void AllPlotsClicked(BaseGadget NewGadget)
	{
		for (int i = 0; i < PlotManager.Instance.m_PlotsHigh; i++)
		{
			for (int j = 0; j < PlotManager.Instance.m_PlotsWide; j++)
			{
				PlotManager.Instance.GetPlotAtPlot(j, i).SetVisible(Visible: true);
			}
		}
		GameStateManager.Instance.PopState();
	}

	public void FastEatClicked(BaseGadget NewGadget)
	{
		CheatManager.Instance.m_FastEat = !CheatManager.Instance.m_FastEat;
		GameStateManager.Instance.PopState();
	}

	public void CursorClicked(BaseGadget NewGadget)
	{
		CheatManager.Instance.m_CursorClicks = !CheatManager.Instance.m_CursorClicks;
		GameStateManager.Instance.PopState();
	}

	public void CheapResearchClicked(BaseGadget NewGadget)
	{
		CheatManager.Instance.m_CheapResearch = !CheatManager.Instance.m_CheapResearch;
		GameStateManager.Instance.PopState();
	}

	public void MaterialsClicked(BaseGadget NewGadget)
	{
		CheatManager.Instance.m_ShowMaterials = !CheatManager.Instance.m_ShowMaterials;
		MaterialManager.Instance.ToggleTest();
		GameStateManager.Instance.PopState();
	}

	protected new void Update()
	{
		base.Update();
		if ((bool)m_CheatButtons[33])
		{
			m_CheatButtons[33].GetComponent<BaseSlider>().SetValue(DayNightManager.Instance.GetTime());
		}
	}
}
