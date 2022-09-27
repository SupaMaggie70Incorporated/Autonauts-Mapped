using UnityEngine;

public class ModsErrorPanel : BaseMenu
{
	private Transform m_Panel;

	private BaseText TitleText;

	private BaseText DescriptionText;

	protected new void Awake()
	{
		base.Awake();
		m_Panel = base.transform.Find("BasePanelOptions").Find("Panel");
	}

	private void CheckGadgets()
	{
		if (!TitleText)
		{
			TitleText = m_Panel.Find("ErrorTitle").GetComponent<BaseText>();
			DescriptionText = m_Panel.Find("ErrorInfo").GetComponent<BaseText>();
			StandardAcceptButton component = m_Panel.Find("StandardAcceptButton").GetComponent<StandardAcceptButton>();
			AddAction(component, OnOKClicked);
			BaseButton component2 = m_Panel.Find("QuitGame").GetComponent<BaseButton>();
			AddAction(component2, OnQuit);
			if (GeneralUtils.m_InGame)
			{
				component2.SetActive(Active: true);
			}
			else
			{
				component2.SetActive(Active: false);
			}
		}
	}

	private void ModErrorLoopAvoidance(string TitleID, string ErrorText)
	{
		if (ModManager.Instance.m_LuaErrorCounter == -1)
		{
			GameStateManager.Instance.PopState();
			return;
		}
		if (ModManager.Instance.m_LuaErrorCounter >= 3)
		{
			GameStateManager.Instance.PopState();
			return;
		}
		TitleText.SetTextFromID(TitleID);
		ModManager.Instance.m_LuaErrorCounter++;
		if (ModManager.Instance.m_LuaErrorCounter == 3)
		{
			DescriptionText.SetText(ErrorText + "\n\nCheck ModError.txt for additional Lua errors.");
		}
		else
		{
			DescriptionText.SetText(ErrorText);
		}
	}

	public void SetCurrentError()
	{
		CheckGadgets();
		if (ModManager.Instance.CurrentErrorState != 0)
		{
			switch (ModManager.Instance.CurrentErrorState)
			{
			case ModManager.ErrorState.Error_Restart:
				TitleText.SetTextFromID("ModsInfoRestart");
				DescriptionText.SetTextFromID("ModsInfoRestartDescription");
				break;
			case ModManager.ErrorState.Error_Upload_Description:
				TitleText.SetTextFromID("ModsInfoUploadFailed");
				DescriptionText.SetTextFromID("ModsInfoUploadFailedDescription");
				break;
			case ModManager.ErrorState.Error_Upload_Image:
				TitleText.SetTextFromID("ModsInfoUploadFailed");
				DescriptionText.SetTextFromID("ModsInfoUploadFailedImage");
				break;
			case ModManager.ErrorState.Error_Upload_Tags:
				TitleText.SetTextFromID("ModsInfoUploadFailed");
				DescriptionText.SetTextFromID("ModsInfoUploadFailedTags");
				break;
			case ModManager.ErrorState.Error_Upload_Title:
				TitleText.SetTextFromID("ModsInfoUploadFailed");
				DescriptionText.SetTextFromID("ModsInfoUploadFailedTitle");
				break;
			case ModManager.ErrorState.Error_FailedSubcribe:
				TitleText.SetTextFromID("ModsInfoSubscribeFailedTitle");
				DescriptionText.SetText("Error Code: " + ModManager.Instance.SteamErrorCode);
				break;
			case ModManager.ErrorState.Error_FailedUnsubcribe:
				TitleText.SetTextFromID("ModsInfoUnsubscribeFailedTitle");
				DescriptionText.SetText("Error Code: " + ModManager.Instance.SteamErrorCode);
				break;
			case ModManager.ErrorState.Error_Upload_Steam:
				TitleText.SetTextFromID("ModsInfoUploadFailed");
				DescriptionText.SetText("Error Code: " + ModManager.Instance.SteamErrorCode);
				break;
			case ModManager.ErrorState.Error_Delete_Steam:
				TitleText.SetTextFromID("ModsInfoDeleteFailedTitle");
				DescriptionText.SetText("Error Code: " + ModManager.Instance.SteamErrorCode);
				break;
			case ModManager.ErrorState.Error_FailedResults:
				TitleText.SetTextFromID("ModsInfoResultsFailedTitle");
				DescriptionText.SetText("Error Code: " + ModManager.Instance.SteamErrorCode);
				break;
			case ModManager.ErrorState.Error_AcceptTCs:
				TitleText.SetTextFromID("ModsInfoUploadFailed");
				DescriptionText.SetTextFromID("ModsInfoResultsFailedTCs");
				break;
			case ModManager.ErrorState.Error_Lua:
				ModErrorLoopAvoidance("ModsInfoLuaErrorTitle", ModManager.Instance.OverrideErrorMessage);
				break;
			case ModManager.ErrorState.Error_Misc:
				ModErrorLoopAvoidance("ModsInfoStandardErrorTitle", ModManager.Instance.OverrideErrorMessage);
				break;
			case ModManager.ErrorState.Error_Clash:
				TitleText.SetTextFromID("ModsInfoClashTitle");
				DescriptionText.SetText(ModManager.Instance.OverrideErrorMessage);
				break;
			case ModManager.ErrorState.Error_Dev_Folders:
				TitleText.SetTextFromID("ModsInfoStandardErrorTitle");
				DescriptionText.SetText("Please remove .dev folders before uploading");
				break;
			}
		}
		else
		{
			GameStateManager.Instance.PopState();
		}
		ModManager.Instance.ClearError();
	}

	public void SetInformation(string TitleID, string DescriptionID)
	{
		CheckGadgets();
		TitleText.SetTextFromID(TitleID);
		DescriptionText.SetText(DescriptionID);
	}

	public void OnOKClicked(BaseGadget NewGadget)
	{
		GameStateManager.Instance.PopState();
	}

	public void OnQuit(BaseGadget NewGadget)
	{
		if (!GeneralUtils.m_InGame)
		{
			ConfirmQuit();
			return;
		}
		AudioManager.Instance.StartEvent("UIOptionSelected");
		GameStateManager.Instance.PushState(GameStateManager.State.Confirm);
		GameStateManager.Instance.GetCurrentState().GetComponent<GameStateConfirm>().SetConfirm(ConfirmQuit, "ConfirmQuitGame");
	}

	public void ConfirmQuit()
	{
		base.gameObject.SetActive(value: false);
		_ = HudManager.Instance.m_MenusRootTransform;
		SessionManager.Instance.LoadLevel(LoadLevel: false, "MainMenu");
		GameStateManager.Instance.SetState(GameStateManager.State.SceneChange);
		ModManager.Instance.ClearError();
		ModManager.Instance.m_LuaErrorCounter = 3;
	}

	protected new void Update()
	{
		base.Update();
	}
}
