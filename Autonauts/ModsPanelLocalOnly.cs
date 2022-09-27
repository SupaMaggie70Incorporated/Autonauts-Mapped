using System.Collections.Generic;
using System.IO;
using Steamworks;
using UnityEngine;

public class ModsPanelLocalOnly : BaseMenu
{
	private Transform m_Panel;

	private List<StandardButtonText> m_LocalItems;

	private BaseText LocalModSelectedText;

	private BaseImage LocalModEnabled;

	private BaseImage LocalModDisabled;

	private BaseText LocalModEnabledText;

	private BaseButton LocalOptionsButton;

	private BaseButton LocalExploreButton;

	private BaseButton ExportObjectListButton;

	private BaseButton ReloadScriptsButton;

	private int LocalIndex;

	private Mod CurrentLocalMod;

	private EResult LastError = EResult.k_EResultRevoked;

	protected new void Awake()
	{
		base.Awake();
		m_Panel = base.transform.Find("BasePanelOptions").Find("Panel");
		m_Panel.Find("LocalList").GetComponent<BaseScrollView>().GetContent()
			.transform.Find("ModItemLocalPrefab").GetComponent<StandardButtonText>().SetActive(Active: false);
	}

	private void SetupList()
	{
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
		LocalOptionsButton = m_Panel.Find("LocalModOptions").GetComponent<BaseButton>();
		AddAction(LocalOptionsButton, OnLocalOptionsClicked);
		LocalOptionsButton.SetInteractable(Interactable: false);
		LocalExploreButton = m_Panel.Find("ShowLocalMods").GetComponent<BaseButton>();
		AddAction(LocalExploreButton, OnOpenExplorerClicked);
		LocalExploreButton.SetInteractable(Interactable: true);
		ExportObjectListButton = m_Panel.Find("ExportTypes").GetComponent<BaseButton>();
		AddAction(ExportObjectListButton, OnExportListClicked);
		ExportObjectListButton.SetInteractable(Interactable: true);
		BaseToggle component = m_Panel.Find("DevModeToggle").GetComponent<BaseToggle>();
		component.SetStartOn(SettingsManager.Instance.m_DevMode);
		AddAction(component, OnDevModeToggle);
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
			LocalModEnabledText.SetTextFromID(CurrentLocalMod.IsEnabled ? "ModsEnabledTitle" : "ModsDisabledTitle");
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

	public void OnOpenExplorerClicked(BaseGadget NewGadget)
	{
		Application.OpenURL("file://" + Path.Combine(Application.streamingAssetsPath, "Mods"));
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
	}

	protected new void Update()
	{
		base.Update();
		if (ModManager.Instance.CurrentErrorState != 0 && LastError != ModManager.Instance.SteamErrorCode)
		{
			LastError = ModManager.Instance.SteamErrorCode;
			GameStateManager.Instance.PushState(GameStateManager.State.ModsError);
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateModsError>().SetCurrentError();
		}
	}
}
