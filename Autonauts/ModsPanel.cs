using System.Collections.Generic;
using System.IO;
using Steamworks;
using UnityEngine;

public class ModsPanel : BaseMenu
{
	private Transform m_Panel;

	private List<StandardButtonText> m_InstalledItems;

	private List<StandardButtonText> m_LocalItems;

	private BaseText LocalModSelectedText;

	private BaseImage LocalModEnabled;

	private BaseImage LocalModDisabled;

	private BaseText LocalModEnabledText;

	private BaseText WorkshopItemTitleText;

	private BaseImage WorkshopModEnabled;

	private BaseImage WorkshopModDisabled;

	private BaseText WorkshopModEnabledText;

	private BaseButton InstalledDeleteButton;

	private BaseButton InstalledOptionsButton;

	private BaseButton LocalUploadButton;

	private BaseButton LocalOptionsButton;

	private BaseButton LocalExploreButton;

	private BaseButton WorkshopExploreButton;

	private BaseButton ExportObjectListButton;

	private BaseButton ReloadScriptsButton;

	private int LocalIndex;

	private int WorkshopIndex;

	private bool CreateWorkshopList;

	private Mod CurrentLocalMod;

	private Mod CurrentWorkshopMod;

	private EResult LastError = EResult.k_EResultRevoked;

	protected new void Awake()
	{
		base.Awake();
		m_Panel = base.transform.Find("BasePanelOptions").Find("Panel");
		m_Panel.Find("InstalledList").GetComponent<BaseScrollView>().GetContent()
			.transform.Find("ModItemPrefab").GetComponent<StandardButtonText>().SetActive(Active: false);
		m_Panel.Find("LocalList").GetComponent<BaseScrollView>().GetContent()
			.transform.Find("ModItemLocalPrefab").GetComponent<StandardButtonText>().SetActive(Active: false);
		if (SteamManager.Initialized)
		{
			SteamWorkshopManager.Instance.GetSubscribedItems();
		}
	}

	private void SetupList()
	{
		m_InstalledItems = new List<StandardButtonText>();
		WorkshopIndex = -1;
		LocalIndex = -1;
		BaseScrollView component = m_Panel.Find("LocalList").GetComponent<BaseScrollView>();
		StandardButtonText component2 = component.GetContent().transform.Find("ModItemLocalPrefab").GetComponent<StandardButtonText>();
		component2.SetActive(Active: false);
		float height = component2.GetHeight();
		Transform parent = component2.transform.parent;
		int count = ModManager.Instance.LocalMods.Count;
		if (count > 0)
		{
			float scrollSize = height * (float)count;
			component.SetScrollSize(scrollSize);
			m_LocalItems = new List<StandardButtonText>();
			float num = component2.GetPosition().y;
			float x = component2.GetPosition().x;
			for (int i = 0; i < count; i++)
			{
				StandardButtonText standardButtonText = Object.Instantiate(component2, parent);
				standardButtonText.SetPosition(x, num);
				standardButtonText.SetActive(Active: true);
				standardButtonText.SetText(ModManager.Instance.LocalMods[i].Name);
				standardButtonText.SetAction(OnLocalItemClicked, standardButtonText);
				m_LocalItems.Add(standardButtonText);
				num -= height;
			}
			LocalIndex = 0;
		}
	}

	protected new void Start()
	{
		base.Start();
		SetupList();
		InstalledDeleteButton = m_Panel.Find("InstalledDelete").GetComponent<BaseButton>();
		AddAction(InstalledDeleteButton, OnDeleteClicked);
		InstalledDeleteButton.SetInteractable(Interactable: false);
		InstalledOptionsButton = m_Panel.Find("InstalledOptions").GetComponent<BaseButton>();
		AddAction(InstalledOptionsButton, OnInstalledOptionsClicked);
		InstalledOptionsButton.SetInteractable(Interactable: false);
		LocalUploadButton = m_Panel.Find("Upload").GetComponent<BaseButton>();
		AddAction(LocalUploadButton, OnUploadClicked);
		LocalUploadButton.SetInteractable(Interactable: false);
		LocalOptionsButton = m_Panel.Find("LocalModOptions").GetComponent<BaseButton>();
		AddAction(LocalOptionsButton, OnLocalOptionsClicked);
		LocalOptionsButton.SetInteractable(Interactable: false);
		LocalExploreButton = m_Panel.Find("ShowLocalMods").GetComponent<BaseButton>();
		AddAction(LocalExploreButton, OnOpenExplorerClicked);
		LocalExploreButton.SetInteractable(Interactable: true);
		WorkshopExploreButton = m_Panel.Find("ShowWorkshopMods").GetComponent<BaseButton>();
		AddAction(WorkshopExploreButton, OnOpenWorkshopExplorerClicked);
		WorkshopExploreButton.SetInteractable(Interactable: true);
		ExportObjectListButton = m_Panel.Find("ExportTypes").GetComponent<BaseButton>();
		AddAction(ExportObjectListButton, OnExportListClicked);
		ExportObjectListButton.SetInteractable(Interactable: true);
		BaseToggle component = m_Panel.Find("DevModeToggle").GetComponent<BaseToggle>();
		component.SetStartOn(SettingsManager.Instance.m_DevMode);
		AddAction(component, OnDevModeToggle);
		BaseButton component2 = m_Panel.Find("DownloadMoreButton").GetComponent<BaseButton>();
		AddAction(component2, OnWorkshopButton);
		WorkshopModEnabled = m_Panel.Find("WorkshopEnabled").Find("EnabledImage").GetComponent<BaseImage>();
		WorkshopModDisabled = m_Panel.Find("WorkshopEnabled").Find("DisabledImage").GetComponent<BaseImage>();
		WorkshopModEnabledText = m_Panel.Find("WorkshopEnabled").GetComponent<BaseText>();
		WorkshopModEnabled.SetActive(Active: false);
		WorkshopModDisabled.SetActive(Active: false);
		WorkshopModEnabledText.SetActive(Active: false);
		WorkshopItemTitleText = m_Panel.Find("WorkshopItemName").GetComponent<BaseText>();
		WorkshopItemTitleText.SetText("-");
		LocalModSelectedText = m_Panel.Find("LocalModSelected").GetComponent<BaseText>();
		LocalModEnabled = m_Panel.Find("LocalModEnabled").Find("EnabledImage").GetComponent<BaseImage>();
		LocalModDisabled = m_Panel.Find("LocalModEnabled").Find("DisabledImage").GetComponent<BaseImage>();
		LocalModEnabledText = m_Panel.Find("LocalModEnabled").GetComponent<BaseText>();
		if (LocalIndex >= 0)
		{
			CurrentLocalMod = ModManager.Instance.LocalMods[LocalIndex];
			LocalModSelectedText.SetText(CurrentLocalMod.Name);
			LocalModEnabled.SetActive(CurrentLocalMod.IsEnabled);
			LocalModDisabled.SetActive(!CurrentLocalMod.IsEnabled);
			LocalOptionsButton.SetInteractable(Interactable: true);
			LocalUploadButton.SetInteractable(Interactable: true);
			LocalModEnabledText.SetTextFromID(CurrentLocalMod.IsEnabled ? "ModsEnabledTitle" : "ModsDisabledTitle");
		}
		if (!SteamManager.Initialized)
		{
			component2.SetInteractable(Interactable: false);
			LocalUploadButton.SetInteractable(Interactable: false);
		}
	}

	public void OnWorkshopButton(BaseGadget NewGadget)
	{
		if (SteamManager.Initialized)
		{
			SteamWorkshopManager.Instance.ShowSteamWorkshopOverlay();
		}
	}

	public void OnInstalledItemClicked(BaseGadget NewGadget)
	{
		if (!SteamManager.Initialized)
		{
			return;
		}
		WorkshopIndex = m_InstalledItems.IndexOf(NewGadget.GetComponent<StandardButtonText>());
		SteamUGCDetails_t steamUGCDetails_t = SteamWorkshopManager.Instance.Details[WorkshopIndex];
		foreach (Mod currentMod in ModManager.Instance.CurrentMods)
		{
			if (currentMod.m_PublishedFileId == steamUGCDetails_t.m_nPublishedFileId)
			{
				CurrentWorkshopMod = currentMod;
			}
		}
		ModManager.Instance.MenuSelectedMod = CurrentWorkshopMod;
		WorkshopItemTitleText.SetText(CurrentWorkshopMod.SteamTitle);
		WorkshopModEnabled.SetActive(CurrentWorkshopMod.IsEnabled);
		WorkshopModDisabled.SetActive(!CurrentWorkshopMod.IsEnabled);
		WorkshopModEnabledText.SetTextFromID(CurrentWorkshopMod.IsEnabled ? "ModsEnabledTitle" : "ModsDisabledTitle");
		WorkshopModEnabledText.SetActive(Active: true);
	}

	public void OnDeleteClicked(BaseGadget NewGadget)
	{
		if (SteamManager.Initialized)
		{
			SteamWorkshopManager.Instance.UnsubscribeWorkshopItem(SteamWorkshopManager.Instance.Details[WorkshopIndex].m_nPublishedFileId);
			SteamWorkshopManager.Instance.GetSubscribedItems();
		}
	}

	public void OnLocalItemClicked(BaseGadget NewGadget)
	{
		LocalIndex = m_LocalItems.IndexOf(NewGadget.GetComponent<StandardButtonText>());
		CurrentLocalMod = ModManager.Instance.LocalMods[LocalIndex];
		ModManager.Instance.MenuSelectedMod = CurrentLocalMod;
		LocalModSelectedText.SetText(CurrentLocalMod.Name);
		LocalModEnabled.SetActive(CurrentLocalMod.IsEnabled);
		LocalModDisabled.SetActive(!CurrentLocalMod.IsEnabled);
		LocalModEnabledText.SetTextFromID(CurrentLocalMod.IsEnabled ? "ModsEnabledTitle" : "ModsDisabledTitle");
	}

	public void OnUploadClicked(BaseGadget NewGadget)
	{
		if (SteamManager.Initialized)
		{
			ModManager.Instance.MenuSelectedMod = CurrentLocalMod;
			GameStateManager.Instance.PushState(GameStateManager.State.ModsUploadConfirm);
		}
	}

	public void OnOpenExplorerClicked(BaseGadget NewGadget)
	{
		Application.OpenURL("file://" + Path.Combine(Application.streamingAssetsPath, "Mods"));
	}

	public void OnOpenWorkshopExplorerClicked(BaseGadget NewGadget)
	{
		if (Directory.Exists(PlayerPrefs.GetString("Steam")))
		{
			Application.OpenURL("file://" + PlayerPrefs.GetString("Steam"));
		}
	}

	public void OnExportListClicked(BaseGadget NewGadget)
	{
		ModManager.Instance.OutputAllDataTypes();
		Application.OpenURL("file://" + Path.Combine(Application.streamingAssetsPath, "Mods\\TypesOutput"));
	}

	public void OnDevModeToggle(BaseGadget NewGadget)
	{
		BaseToggle component = NewGadget.GetComponent<BaseToggle>();
		SettingsManager.Instance.m_DevMode = component.GetOn();
		if (SettingsManager.Instance.m_DevMode)
		{
			GameStateManager.Instance.PushState(GameStateManager.State.ModsPopup);
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateModsPopup>().SetInformationFromID("ModsPanelScriptsReloaded", "ModsPanelReloadScriptsWarning");
		}
		SettingsManager.Instance.Save();
	}

	public void OnInstalledOptionsClicked(BaseGadget NewGadget)
	{
		if (SteamManager.Initialized)
		{
			ModManager.Instance.MenuSelectedMod = CurrentWorkshopMod;
			GameStateManager.Instance.PushState(GameStateManager.State.ModsOptions);
		}
	}

	public void OnLocalOptionsClicked(BaseGadget NewGadget)
	{
		ModManager.Instance.MenuSelectedMod = CurrentLocalMod;
		GameStateManager.Instance.PushState(GameStateManager.State.ModsOptions);
	}

	public void Refresh()
	{
		if (LocalIndex >= 0)
		{
			LocalModEnabled.SetActive(CurrentLocalMod.IsEnabled);
			LocalModDisabled.SetActive(!CurrentLocalMod.IsEnabled);
			LocalModEnabledText.SetTextFromID(CurrentLocalMod.IsEnabled ? "ModsEnabledTitle" : "ModsDisabledTitle");
		}
		if (SteamManager.Initialized)
		{
			SteamWorkshopManager.Instance.GetSubscribedItems();
		}
		InstalledDeleteButton.SetInteractable(Interactable: false);
		InstalledOptionsButton.SetInteractable(Interactable: false);
		WorkshopModEnabled.SetActive(Active: false);
		WorkshopModDisabled.SetActive(Active: false);
		WorkshopItemTitleText.SetText("-");
		WorkshopModEnabledText.SetActive(Active: false);
		foreach (StandardButtonText installedItem in m_InstalledItems)
		{
			Object.Destroy(installedItem.gameObject);
		}
		m_InstalledItems.Clear();
		CreateWorkshopList = false;
	}

	protected new void Update()
	{
		base.Update();
		if (SteamManager.Initialized && SteamWorkshopManager.Instance.NeedsMenuRefresh)
		{
			SteamWorkshopManager.Instance.NeedsMenuRefresh = false;
			Refresh();
		}
		else if (ModManager.Instance.CurrentErrorState != 0 && LastError != ModManager.Instance.SteamErrorCode)
		{
			LastError = ModManager.Instance.SteamErrorCode;
			GameStateManager.Instance.PushState(GameStateManager.State.ModsError);
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateModsError>().SetCurrentError();
		}
		else
		{
			if (!SteamManager.Initialized || ModManager.Instance.CurrentErrorState != 0 || CreateWorkshopList || SteamWorkshopManager.Instance.SearchStateSubscribedItems != SteamWorkshopManager.WorkshopSearchState.Search_Subscribed_Items_Complete)
			{
				return;
			}
			BaseScrollView component = m_Panel.Find("InstalledList").GetComponent<BaseScrollView>();
			StandardButtonText component2 = component.GetContent().transform.Find("ModItemPrefab").GetComponent<StandardButtonText>();
			component2.SetActive(Active: false);
			float height = component2.GetHeight();
			Transform parent = component2.transform.parent;
			if (SteamWorkshopManager.Instance.Details == null)
			{
				Refresh();
				return;
			}
			int count = SteamWorkshopManager.Instance.Details.Count;
			if (count <= 0)
			{
				CreateWorkshopList = true;
				return;
			}
			for (int i = 0; i < count; i++)
			{
				if (!SteamWorkshopManager.Instance.IsSubscribedItemDownloaded(SteamWorkshopManager.Instance.Details[i].m_nPublishedFileId))
				{
					WorkshopItemTitleText.SetTextFromID("ModsDownloadTitle");
					return;
				}
			}
			foreach (StandardButtonText installedItem in m_InstalledItems)
			{
				Object.Destroy(installedItem.gameObject);
			}
			m_InstalledItems.Clear();
			float scrollSize = height * (float)count;
			component.SetScrollSize(scrollSize);
			float num = component2.GetPosition().y;
			float x = component2.GetPosition().x;
			int num2 = -1;
			for (int j = 0; j < count; j++)
			{
				if (!SteamWorkshopManager.Instance.GetSubscribedItemComplete(SteamWorkshopManager.Instance.Details[j].m_nPublishedFileId))
				{
					continue;
				}
				string subscribedItemFolderLocation = SteamWorkshopManager.Instance.GetSubscribedItemFolderLocation(SteamWorkshopManager.Instance.Details[j].m_nPublishedFileId);
				if (subscribedItemFolderLocation.Length != 0)
				{
					if (num2 == -1)
					{
						num2 = j;
					}
					StandardButtonText standardButtonText = Object.Instantiate(component2, parent);
					standardButtonText.SetPosition(x, num);
					standardButtonText.SetActive(Active: true);
					standardButtonText.SetText(SteamWorkshopManager.Instance.Details[j].m_rgchTitle);
					standardButtonText.SetAction(OnInstalledItemClicked, standardButtonText);
					m_InstalledItems.Add(standardButtonText);
					int num3 = subscribedItemFolderLocation.LastIndexOf("\\");
					if (num3 > 0)
					{
						subscribedItemFolderLocation = subscribedItemFolderLocation.Substring(0, num3);
						PlayerPrefs.SetString("Steam", subscribedItemFolderLocation);
					}
					num -= height;
				}
			}
			if (num2 == -1)
			{
				return;
			}
			WorkshopIndex = 0;
			SteamUGCDetails_t steamUGCDetails_t = SteamWorkshopManager.Instance.Details[num2];
			foreach (Mod currentMod in ModManager.Instance.CurrentMods)
			{
				if (currentMod != null && !currentMod.IsLocal && currentMod.m_PublishedFileId == steamUGCDetails_t.m_nPublishedFileId)
				{
					CurrentWorkshopMod = currentMod;
				}
			}
			if (CurrentWorkshopMod != null)
			{
				WorkshopItemTitleText.SetText(CurrentWorkshopMod.SteamTitle);
				WorkshopModEnabled.SetActive(CurrentWorkshopMod.IsEnabled);
				WorkshopModDisabled.SetActive(!CurrentWorkshopMod.IsEnabled);
				WorkshopModEnabledText.SetTextFromID(CurrentWorkshopMod.IsEnabled ? "ModsEnabledTitle" : "ModsDisabledTitle");
				WorkshopModEnabledText.SetActive(Active: true);
				InstalledDeleteButton.SetInteractable(Interactable: true);
				InstalledOptionsButton.SetInteractable(Interactable: true);
				CreateWorkshopList = true;
			}
		}
	}
}
