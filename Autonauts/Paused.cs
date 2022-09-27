using System;
using UnityEngine;

public class Paused : BaseMenu
{
	private bool m_Recordings;

	private BaseText m_NumberText;

	private BaseText m_VersionText;

	protected new void Awake()
	{
		base.Awake();
		m_Recordings = !GeneralUtils.m_InGame;
		string[] array = new string[10] { "ContinueButton", "LoadButton", "SaveButton", "SettingsButton", "AboutButton", "QuitButton", "BadgesButton", "StatsButton", "DiscordButton", "WikiButton" };
		Action<BaseGadget>[] array2 = new Action<BaseGadget>[10] { OnContinue, OnLoad, OnSave, OnSettings, OnAbout, OnQuit, OnBadges, OnStats, OnDiscord, OnWiki };
		for (int i = 0; i < array.Length; i++)
		{
			BaseButton component = base.transform.Find(array[i]).GetComponent<BaseButton>();
			if (array2[i] == new Action<BaseGadget>(OnBadges) && GameOptionsManager.Instance.m_Options.m_GameMode != 0)
			{
				component.SetActive(Active: false);
			}
			AddAction(component, array2[i]);
		}
		m_VersionText = base.transform.Find("Version").GetComponent<BaseText>();
		string version = SaveLoadManager.GetVersion();
		m_VersionText.SetText(version);
		NewGameOptions component2 = base.transform.Find("NewGameOptions").GetComponent<NewGameOptions>();
		component2.SetDataFromNew();
		component2.transform.Find("BasePanelOptions/Panel/BackButton").gameObject.SetActive(value: false);
		component2.transform.Find("BasePanelOptions/StartButton").gameObject.SetActive(value: false);
	}

	protected new void Start()
	{
		base.Start();
		if (m_Recordings)
		{
			base.transform.Find("SaveButton").gameObject.SetActive(value: false);
		}
	}

	public void Pushed()
	{
		base.gameObject.SetActive(value: false);
	}

	public void Popped()
	{
		base.gameObject.SetActive(value: true);
	}

	public void OnDiscord(BaseGadget NewGadget)
	{
		AudioManager.Instance.StartEvent("UIOptionSelected");
		Application.OpenURL("https://discordapp.com/invite/xXRfjsc");
	}

	public void OnWiki(BaseGadget NewGadget)
	{
		AudioManager.Instance.StartEvent("UIOptionSelected");
		Application.OpenURL("https://autonauts.gamepedia.com/Autonauts_Wiki");
	}

	public void OnContinue(BaseGadget NewGadget)
	{
		AudioManager.Instance.StartEvent("UIUnpause");
		GameStateManager.Instance.PopState();
	}

	public void OnSave(BaseGadget NewGadget)
	{
		AudioManager.Instance.StartEvent("UIOptionSelected");
		GameStateManager.Instance.PushState(GameStateManager.State.Save);
	}

	public void OnLoad(BaseGadget NewGadget)
	{
		AudioManager.Instance.StartEvent("UIOptionSelected");
		GameStateManager.Instance.PushState(GameStateManager.State.Load);
		GameStateManager.Instance.GetCurrentState().GetComponent<GameStateLoad>().Init(m_Recordings);
	}

	public void ConfirmQuit()
	{
		base.gameObject.SetActive(value: false);
		_ = HudManager.Instance.m_MenusRootTransform;
		SessionManager.Instance.LoadLevel(LoadLevel: false, "MainMenu");
		GameStateManager.Instance.SetState(GameStateManager.State.SceneChange);
	}

	public void OnSettings(BaseGadget NewGadget)
	{
		AudioManager.Instance.StartEvent("UIOptionSelected");
		GameStateManager.Instance.PushState(GameStateManager.State.Settings);
	}

	public void OnOptions(BaseGadget NewGadget)
	{
		AudioManager.Instance.StartEvent("UIOptionSelected");
		GameStateManager.Instance.PushState(GameStateManager.State.NewGame);
		GameStateManager.Instance.GetCurrentState().GetComponent<GameStateNewGame>().SetDataFromNew();
	}

	public void OnAbout(BaseGadget NewGadget)
	{
		AudioManager.Instance.StartEvent("UIOptionSelected");
		GameStateManager.Instance.PushState(GameStateManager.State.About);
	}

	public void OnQuit(BaseGadget NewGadget)
	{
		AudioManager.Instance.StartEvent("UIOptionSelected");
		GameStateManager.Instance.PushState(GameStateManager.State.Confirm);
		if (SaveLoadManager.Instance.m_TimeSinceLastSave > 30f)
		{
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateConfirm>().SetConfirm(ConfirmQuit, "ConfirmQuitGame", "ConfirmQuitGameNoSave");
		}
		else
		{
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateConfirm>().SetConfirm(ConfirmQuit, "ConfirmQuitGame");
		}
	}

	public void OnBadges(BaseGadget NewGadget)
	{
		AudioManager.Instance.StartEvent("UIOptionSelected");
		GameStateManager.Instance.PushState(GameStateManager.State.Badges);
	}

	public void OnStats(BaseGadget NewGadget)
	{
		AudioManager.Instance.StartEvent("UIOptionSelected");
		GameStateManager.Instance.PushState(GameStateManager.State.Stats);
	}
}
