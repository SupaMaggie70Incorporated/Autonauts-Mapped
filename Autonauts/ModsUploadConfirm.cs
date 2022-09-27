using System.IO;
using Steamworks;
using UnityEngine;

public class ModsUploadConfirm : BaseMenu
{
	private Transform m_Panel;

	private BaseText UpdateText;

	private Mod CurrentMod;

	private bool UploadClicked;

	private BaseButton UploadButton;

	private BaseButton TCButton;

	private BaseButton ExitButton;

	private BaseRawImage LoadingImage;

	private bool IsShowingError;

	protected new void Awake()
	{
		base.Awake();
		m_Panel = base.transform.Find("BasePanelOptions").Find("Panel");
	}

	protected new void Start()
	{
		base.Start();
		UploadButton = m_Panel.Find("Upload").GetComponent<BaseButton>();
		AddAction(UploadButton, OnUploadClicked);
		TCButton = m_Panel.Find("TermsServiceButton").GetComponent<BaseButton>();
		AddAction(TCButton, OnTCClicked);
		ExitButton = m_Panel.Find("BackButton").GetComponent<BaseButton>();
		CurrentMod = ModManager.Instance.MenuSelectedMod;
		m_Panel.Find("ModName").GetComponent<BaseText>().SetText(CurrentMod.Name);
		m_Panel.Find("Image").GetComponent<BaseRawImage>().SetTexture(CurrentMod.GetTexture(CurrentMod.SteamImageName));
		LoadingImage = m_Panel.Find("LoadSpinner").GetComponent<BaseRawImage>();
		LoadingImage.SetActive(Active: false);
		UpdateText = m_Panel.Find("PercentComplete").GetComponent<BaseText>();
		UpdateText.SetActive(Active: false);
	}

	public void OnUploadClicked(BaseGadget NewGadget)
	{
		if (CurrentMod.HasDevScripts())
		{
			ModManager.Instance.SetErrorSteam(ModManager.ErrorState.Error_Dev_Folders);
		}
		else if ((float)File.ReadAllBytes(CurrentMod.SteamContentImage).Length / 1024f >= 1024f)
		{
			ModManager.Instance.SetErrorSteam(ModManager.ErrorState.Error_Upload_Image);
		}
		else if (SteamManager.Initialized)
		{
			CurrentMod.UploadToSteamWorkshop();
			UploadClicked = true;
			UploadButton.SetActive(Active: false);
			TCButton.SetActive(Active: false);
			ExitButton.SetActive(Active: true);
		}
	}

	public void OnTCClicked(BaseGadget NewGadget)
	{
		SteamFriends.ActivateGameOverlayToWebPage("https://steamcommunity.com/sharedfiles/workshoplegalagreement");
	}

	public void UploadCompleted()
	{
		UploadClicked = false;
		ExitButton.SetActive(Active: true);
		LoadingImage.SetActive(Active: false);
		ulong punBytesProcessed = 0uL;
		ulong punBytesTotal = 0uL;
		SteamUGC.GetItemUpdateProgress(SteamWorkshopManager.Instance.m_UGCUpdateHandle, out punBytesProcessed, out punBytesTotal);
		UpdateText.SetText("100%");
	}

	protected new void Update()
	{
		base.Update();
		LoadingImage.transform.Rotate(0f, 0f, -5f, Space.Self);
		if (UploadClicked)
		{
			LoadingImage.SetActive(Active: true);
			ulong punBytesProcessed = 0uL;
			ulong punBytesTotal = 0uL;
			SteamUGC.GetItemUpdateProgress(SteamWorkshopManager.Instance.m_UGCUpdateHandle, out punBytesProcessed, out punBytesTotal);
			UpdateText.SetTextFromID("ModsInfoUploadWait");
			string text = UpdateText.GetText();
			UpdateText.SetText(text + "\n(" + punBytesProcessed + "/" + punBytesTotal + ")");
			UpdateText.SetActive(Active: true);
			if (punBytesProcessed >= punBytesTotal && punBytesTotal != 0L)
			{
				UploadClicked = false;
				ExitButton.SetActive(Active: true);
			}
		}
		if (!IsShowingError && ModManager.Instance.CurrentErrorState != 0)
		{
			GameStateManager.Instance.PushState(GameStateManager.State.ModsError);
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateModsError>().SetCurrentError();
			IsShowingError = true;
			ExitButton.SetActive(Active: true);
		}
	}
}
