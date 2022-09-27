using System;
using System.Collections.Generic;
using Rewired;
using UnityEngine;

public class MenuSettings : BaseMenu
{
	private enum State
	{
		Preferences,
		Graphics,
		Controls,
		Total
	}

	private static float m_SettingSpacing = 45f;

	private static float m_DividerSpacing = 10f;

	private static State m_State = State.Preferences;

	private Transform m_Panel;

	private BaseButtonText m_ApplyButton;

	private BaseButtonImage[] m_PageButtons;

	private BaseScrollView[] m_Pages;

	private StandardSlider m_SFXGadget;

	private StandardSlider m_MusicGadget;

	private BaseToggle m_FlashiesGadget;

	private BaseToggle m_WeatherGadget;

	private BaseToggle m_DayNightGadget;

	private BaseDropdown m_AutosaveFrequencyGadget;

	private int m_AutosaveFrequency;

	private SettingsManager.AutosaveFrequency m_OldAutosaveFrequency;

	private int m_Resolution;

	private bool m_FullScreen;

	private BaseDropdown m_ResolutionGadget;

	private BaseDropdown m_QualityGadget;

	private BaseToggle m_FullScreenGadget;

	private BaseToggle m_AmbientOcclusionGadget;

	private BaseToggle m_LightsGadget;

	private BaseToggle m_ShadowsGadget;

	private BaseToggle m_BloomGadget;

	private BaseToggle m_AntialiasingGadget;

	private BaseButtonText m_DefaultsButton;

	private List<ControllerBinding> m_ControllerButtons;

	private List<BaseText> m_ControllerInfo;

	private ControllerBinding m_CurrentControllerButton;

	private Dictionary<ActionElementMap, InputBinding> m_ControllerKeys;

	private GameObject m_DefaultToggle;

	private GameObject m_DefaultSlider;

	private GameObject m_DefaultDropdown;

	private GameObject m_DefaultButton;

	private GameObject m_DefaultDivider;

	private float m_GadgetYOffset;

	private int m_OldScreenWidth;

	private int m_OldScreenHeight;

	private BaseButtonImage m_LanguageButton;

	private PlaySound m_SoundTest;

	protected new void Awake()
	{
		base.Awake();
	}

	protected new void Start()
	{
		base.Start();
		m_Panel = base.transform.Find("BasePanelOptions");
		BaseButtonImage component = m_Panel.Find("ApplyButton").GetComponent<BaseButtonImage>();
		AddAction(component, OnApplyPressed);
		CreatePages();
		GetDefaults();
		CreatePreferences();
		CreateGraphics();
		CreateControls();
		HideDefaults();
		SetState(m_State);
		m_PageButtons[(int)m_State].SetSelected(Selected: true);
	}

	private void GetDefaults()
	{
		m_DefaultToggle = m_Panel.Find("DefaultToggleOption").gameObject;
		m_DefaultSlider = m_Panel.Find("DefaultSliderOption").gameObject;
		m_DefaultDropdown = m_Panel.Find("DefaultDropdownOption").gameObject;
		m_DefaultButton = m_Panel.Find("ControllerBinding").gameObject;
		m_DefaultDivider = m_Panel.Find("DefaultDivider").gameObject;
	}

	private void HideDefaults()
	{
		m_DefaultToggle.gameObject.SetActive(value: false);
		m_DefaultSlider.gameObject.SetActive(value: false);
		m_DefaultDropdown.gameObject.SetActive(value: false);
		m_DefaultButton.gameObject.SetActive(value: false);
		m_DefaultDivider.gameObject.SetActive(value: false);
	}

	private void CreatePages()
	{
		BaseScrollView component = m_Panel.Find("DefaultScrollView").GetComponent<BaseScrollView>();
		m_PageButtons = new BaseButtonImage[3];
		for (int i = 0; i < 3; i++)
		{
			BaseButtonImage[] pageButtons = m_PageButtons;
			int num = i;
			Transform panel = m_Panel;
			State state = (State)i;
			pageButtons[num] = panel.Find(state.ToString() + "Button").GetComponent<BaseButtonImage>();
			m_PageButtons[i].SetAction(OnPageButtonClicked, m_PageButtons[i]);
		}
		m_Pages = new BaseScrollView[3];
		for (int j = 0; j < 3; j++)
		{
			State state = (State)j;
			string text = state.ToString();
			BaseScrollView baseScrollView = UnityEngine.Object.Instantiate(component, m_Panel);
			baseScrollView.name = text;
			baseScrollView.gameObject.SetActive(value: false);
			m_Pages[j] = baseScrollView;
		}
		component.gameObject.SetActive(value: false);
	}

	private void CreateDivider(BaseScrollView NewPage)
	{
		Transform parent = NewPage.GetContent().transform;
		Vector2 vector = new Vector2(0f, m_GadgetYOffset - m_DividerSpacing);
		m_GadgetYOffset -= m_DividerSpacing;
		UnityEngine.Object.Instantiate(m_DefaultDivider, parent).transform.localPosition = vector;
	}

	private GameObject CreateGadget(GameObject NewPrefab, string NewName, BaseScrollView NewPage, Action<BaseGadget> NewAction)
	{
		Transform parent = NewPage.GetContent().transform;
		Vector2 vector = new Vector2(0f, m_GadgetYOffset);
		m_GadgetYOffset -= m_SettingSpacing;
		GameObject obj = UnityEngine.Object.Instantiate(NewPrefab, parent);
		obj.name = NewName;
		obj.transform.localPosition = vector;
		obj.transform.Find("Text").GetComponent<BaseText>().SetTextFromID(NewName);
		return obj;
	}

	private StandardSlider CreateSlider(string NewName, float DefaultValue, BaseScrollView NewPage, Action<BaseGadget> NewAction)
	{
		StandardSlider component = CreateGadget(m_DefaultSlider, NewName, NewPage, NewAction).GetComponent<StandardSlider>();
		component.SetStartValue(DefaultValue);
		component.SetAction(NewAction, component);
		return component;
	}

	private BaseToggle CreateToggle(string NewName, bool DefaultValue, BaseScrollView NewPage, Action<BaseGadget> NewAction)
	{
		BaseToggle component = CreateGadget(m_DefaultToggle, NewName, NewPage, NewAction).transform.Find("BaseToggle").GetComponent<BaseToggle>();
		component.SetStartOn(DefaultValue);
		RegisterGadget(component);
		AddAction(component, NewAction);
		return component;
	}

	private BaseDropdown CreateDropdown(string NewName, int DefaultValue, BaseScrollView NewPage, Action<BaseGadget> NewAction, List<string> Options)
	{
		BaseDropdown component = CreateGadget(m_DefaultDropdown, NewName, NewPage, NewAction).transform.Find("BaseDropdown").GetComponent<BaseDropdown>();
		component.SetOptions(Options);
		component.SetStartValue(DefaultValue);
		RegisterGadget(component);
		AddAction(component, NewAction);
		return component;
	}

	private ControllerBinding CreateButtonText(string NewName, ActionElementMap aem, BaseScrollView NewPage, Action<BaseGadget> NewAction)
	{
		ControllerBinding component = CreateGadget(m_DefaultButton, NewName, NewPage, NewAction).GetComponent<ControllerBinding>();
		component.SetAem(aem, NewAction);
		return component;
	}

	private List<string> GetAutosaveFrequencies(out int CurrentAutosaveFrequency)
	{
		List<string> list = new List<string>();
		CurrentAutosaveFrequency = 0;
		for (int i = 0; i < 4; i++)
		{
			SettingsManager.AutosaveFrequency autosaveFrequency = (SettingsManager.AutosaveFrequency)i;
			string item = TextManager.Instance.Get("SettingsPreferencesAutosave" + autosaveFrequency);
			list.Add(item);
			if (SettingsManager.Instance.m_AutosaveFrequency == autosaveFrequency)
			{
				CurrentAutosaveFrequency = i;
			}
		}
		return list;
	}

	private void CreatePreferences()
	{
		m_GadgetYOffset = -10f;
		m_SFXGadget = CreateSlider("SettingsPreferencesSFX", SettingsManager.Instance.m_SFXVolume, m_Pages[0], OnPreferencesClicked);
		m_MusicGadget = CreateSlider("SettingsPreferencesMusic", SettingsManager.Instance.m_MusicVolume, m_Pages[0], OnPreferencesClicked);
		m_FlashiesGadget = CreateToggle("SettingsPreferencesFlashies", SettingsManager.Instance.m_FlashiesEnabled, m_Pages[0], OnPreferencesClicked);
		m_WeatherGadget = CreateToggle("SettingsPreferencesWeather", SettingsManager.Instance.m_WeatherEnabled, m_Pages[0], OnPreferencesClicked);
		m_DayNightGadget = CreateToggle("SettingsPreferencesDayNight", SettingsManager.Instance.m_DayNightEnabled, m_Pages[0], OnPreferencesClicked);
		List<string> autosaveFrequencies = GetAutosaveFrequencies(out m_AutosaveFrequency);
		m_AutosaveFrequencyGadget = CreateDropdown("SettingsPreferencesAutosaveFrequency", m_AutosaveFrequency, m_Pages[0], OnPreferencesClicked, autosaveFrequencies);
		m_OldAutosaveFrequency = SettingsManager.Instance.m_AutosaveFrequency;
		m_Pages[0].SetScrollSize(0f - m_GadgetYOffset);
	}

	private List<string> GetScreenResolutions(out int ResolutionValue)
	{
		int screenWidth = SettingsManager.Instance.GetScreenWidth();
		int screenHeight = SettingsManager.Instance.GetScreenHeight();
		List<string> list = new List<string>();
		ResolutionValue = 0;
		int num = 0;
		Resolution[] possibleResolutions = SettingsManager.Instance.GetPossibleResolutions();
		for (int i = 0; i < possibleResolutions.Length; i++)
		{
			Resolution resolution = possibleResolutions[i];
			list.Add(resolution.width + " x " + resolution.height);
			if (resolution.width == screenWidth && resolution.height == screenHeight)
			{
				ResolutionValue = num;
			}
			num++;
		}
		return list;
	}

	private List<string> GetQualities(out int CurrentQuality)
	{
		List<string> list = new List<string>();
		string[] names = QualitySettings.names;
		foreach (string text in names)
		{
			string item = TextManager.Instance.Get("SettingsGraphicsQuality" + text);
			list.Add(item);
		}
		CurrentQuality = QualitySettings.GetQualityLevel();
		return list;
	}

	private void CreateGraphics()
	{
		int CurrentQuality;
		List<string> qualities = GetQualities(out CurrentQuality);
		m_GadgetYOffset = -10f;
		int ResolutionValue;
		List<string> screenResolutions = GetScreenResolutions(out ResolutionValue);
		m_OldScreenWidth = SettingsManager.Instance.GetScreenWidth();
		m_OldScreenHeight = SettingsManager.Instance.GetScreenHeight();
		m_Resolution = ResolutionValue;
		m_ResolutionGadget = CreateDropdown("SettingsGraphicsResolution", ResolutionValue, m_Pages[1], OnGraphicsClicked, screenResolutions);
		m_FullScreenGadget = CreateToggle("SettingsGraphicsFullScreen", SettingsManager.Instance.GetFullScreen(), m_Pages[1], OnGraphicsClicked);
		CreateDivider(m_Pages[1]);
		m_QualityGadget = CreateDropdown("SettingsGraphicsQuality", CurrentQuality, m_Pages[1], OnGraphicsClicked, qualities);
		m_AmbientOcclusionGadget = CreateToggle("SettingsGraphicsAmbientOcclusion", SettingsManager.Instance.m_AmbientOcclusionEnabled, m_Pages[1], OnGraphicsClicked);
		m_LightsGadget = CreateToggle("SettingsGraphicsLights", SettingsManager.Instance.m_LightsEnabled, m_Pages[1], OnGraphicsClicked);
		m_ShadowsGadget = CreateToggle("SettingsGraphicsShadows", SettingsManager.Instance.m_ShadowsEnabled, m_Pages[1], OnGraphicsClicked);
		m_BloomGadget = CreateToggle("SettingsGraphicsBloom", SettingsManager.Instance.m_BloomEnabled, m_Pages[1], OnGraphicsClicked);
		m_AntialiasingGadget = CreateToggle("SettingsGraphicsAntialiasing", SettingsManager.Instance.m_AntialiasingEnabled, m_Pages[1], OnGraphicsClicked);
		m_Pages[1].SetScrollSize(0f - m_GadgetYOffset);
		UpdateAutoDOF();
		UpdateShadowButton();
	}

	private void CreateControls()
	{
		m_DefaultsButton = m_Panel.Find("DefaultsButton").GetComponent<BaseButtonText>();
		m_DefaultsButton.gameObject.SetActive(value: false);
		AddAction(m_DefaultsButton, OnDefaultsPressed);
		m_ControllerInfo = new List<BaseText>();
		m_ControllerButtons = new List<ControllerBinding>();
		m_ControllerKeys = new Dictionary<ActionElementMap, InputBinding>();
		m_GadgetYOffset = -10f;
		foreach (KeyValuePair<ActionElementMap, InputBinding> controlsAction in MyInputManager.Instance.m_ControlsActions)
		{
			ActionElementMap key = controlsAction.Key;
			InputAction action = ReInput.mapping.GetAction(key.actionId);
			if (action.categoryId == 0)
			{
				ControllerBinding item = CreateButtonText("RewiredAction" + action.name, key, m_Pages[2], OnControlsClicked);
				m_ControllerButtons.Add(item);
				InputBinding value = new InputBinding(key.keyCode, key.modifierKey1);
				m_ControllerKeys.Add(key, value);
			}
		}
		m_Pages[2].SetScrollSize(0f - m_GadgetYOffset);
		UpdateControllerButtons();
	}

	public void UpdateControllerButtons()
	{
		foreach (ControllerBinding controllerButton in m_ControllerButtons)
		{
			controllerButton.UpdateText();
		}
	}

	private void ControllerButtonPressed(ControllerBinding NewButton)
	{
		GameStateManager.Instance.PushState(GameStateManager.State.AnyKey);
		m_CurrentControllerButton = NewButton;
		GameStateManager.Instance.GetCurrentState().GetComponent<GameStateAnyKey>().SetAem(NewButton.m_Aem);
	}

	private void SetState(State NewState)
	{
		m_Pages[(int)m_State].gameObject.SetActive(value: false);
		m_State = NewState;
		m_Pages[(int)m_State].gameObject.SetActive(value: true);
		m_DefaultsButton.gameObject.SetActive(m_State == State.Controls);
	}

	public void CancelChanges()
	{
		SettingsManager.Instance.SetSFXVolume(m_SFXGadget.GetStartValue());
		SettingsManager.Instance.SetMusicVolume(m_MusicGadget.GetStartValue());
		SettingsManager.Instance.SetFlashiesEnabled(m_FlashiesGadget.GetStartOn());
		SettingsManager.Instance.SetWeatherEnabled(m_WeatherGadget.GetStartOn());
		SettingsManager.Instance.SetDayNightEnabled(m_DayNightGadget.GetStartOn());
		SettingsManager.AutosaveFrequency startValue = (SettingsManager.AutosaveFrequency)m_AutosaveFrequencyGadget.GetStartValue();
		SettingsManager.Instance.SetAutosaveFrequency(startValue);
		SettingsManager.Instance.SetScreen(m_OldScreenWidth, m_OldScreenHeight, m_FullScreenGadget.GetStartOn(), SetResolution: true);
		SettingsManager.Instance.SetQuality(m_QualityGadget.GetStartValue());
		SettingsManager.Instance.SetAmbientOcclusion(m_AmbientOcclusionGadget.GetStartOn());
		SettingsManager.Instance.SetLights(m_LightsGadget.GetStartOn());
		SettingsManager.Instance.SetShadows(m_ShadowsGadget.GetStartOn());
		SettingsManager.Instance.SetBloom(m_BloomGadget.GetStartOn());
		SettingsManager.Instance.SetAntialiasing(m_AntialiasingGadget.GetStartOn());
		foreach (KeyValuePair<ActionElementMap, InputBinding> controllerKey in m_ControllerKeys)
		{
			SettingsManager.Instance.SetKeyCode(controllerKey.Key, controllerKey.Value.m_Code, controllerKey.Value.m_ModifierKey);
		}
	}

	private void ApplyChanges()
	{
		SettingsManager.Instance.Save();
	}

	public void OnPageButtonClicked(BaseGadget NewButton)
	{
		for (int i = 0; i < m_PageButtons.Length; i++)
		{
			if (m_PageButtons[i] == NewButton)
			{
				m_PageButtons[i].SetSelected(Selected: true);
				SetState((State)i);
			}
			else
			{
				m_PageButtons[i].SetSelected(Selected: false);
			}
		}
	}

	public void OnDefaultsPressed(BaseGadget NewGadget)
	{
		SettingsManager.Instance.DefaultKeyCodes();
		UpdateControllerButtons();
	}

	public void ConfirmQuit()
	{
		ApplyChanges();
		GameStateManager.Instance.PopState();
	}

	public void OnApplyPressed(BaseGadget NewGadget)
	{
		if (!SettingsManager.Instance.CheckKeyCodesValid())
		{
			GameStateManager.Instance.PushState(GameStateManager.State.Confirm);
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateConfirm>().SetConfirm(ConfirmQuit, "ConfirmAssignKeys", "ConfirmAssignKeysDescription");
		}
		else
		{
			ConfirmQuit();
		}
	}

	public override void OnBackClicked(BaseGadget NewGadget)
	{
		CancelChanges();
		base.OnBackClicked(NewGadget);
	}

	private void UpdateAutoDOF()
	{
	}

	public void ConfirmStopAutosaves()
	{
		SettingsManager.Instance.SetAutosaveFrequency(SettingsManager.AutosaveFrequency.None);
	}

	public void CancelStopAutosaves()
	{
		SettingsManager.AutosaveFrequency startValue = (SettingsManager.AutosaveFrequency)m_AutosaveFrequencyGadget.GetStartValue();
		SettingsManager.Instance.SetAutosaveFrequency(startValue);
		m_AutosaveFrequencyGadget.SetValue((int)m_OldAutosaveFrequency);
	}

	public void OnPreferencesClicked(BaseGadget NewGadget)
	{
		if (NewGadget == m_SFXGadget)
		{
			SettingsManager.Instance.SetSFXVolume(m_SFXGadget.GetValue());
			if (m_SoundTest == null || !m_SoundTest.m_Result.ActingVariation.IsPlaying)
			{
				m_SoundTest = AudioManager.Instance.StartEvent("UIVolumeChangedTest", null, Remember: true);
			}
		}
		else if (NewGadget == m_MusicGadget)
		{
			SettingsManager.Instance.SetMusicVolume(m_MusicGadget.GetValue());
		}
		else if (NewGadget == m_FlashiesGadget)
		{
			SettingsManager.Instance.SetFlashiesEnabled(m_FlashiesGadget.GetOn());
		}
		else if (NewGadget == m_WeatherGadget)
		{
			SettingsManager.Instance.SetWeatherEnabled(m_WeatherGadget.GetOn());
		}
		else if (NewGadget == m_DayNightGadget)
		{
			SettingsManager.Instance.SetDayNightEnabled(m_DayNightGadget.GetOn());
		}
		if (NewGadget == m_AutosaveFrequencyGadget)
		{
			SettingsManager.AutosaveFrequency value = (SettingsManager.AutosaveFrequency)m_AutosaveFrequencyGadget.GetValue();
			if (value == SettingsManager.AutosaveFrequency.None)
			{
				GameStateManager.Instance.PushState(GameStateManager.State.Confirm);
				GameStateManager.Instance.GetCurrentState().GetComponent<GameStateConfirm>().SetConfirm(ConfirmStopAutosaves, "ConfirmStopAutosaves", "ConfirmStopAutosavesDescription", CancelStopAutosaves);
			}
			else
			{
				m_OldAutosaveFrequency = value;
				SettingsManager.Instance.SetAutosaveFrequency(value);
			}
		}
	}

	private void UpdateResolution()
	{
		SettingsManager.Instance.GetScreenResolution(m_Resolution, out var DesiredWidth, out var DesiredHeight);
		SettingsManager.Instance.SetScreen(DesiredWidth, DesiredHeight, m_FullScreen, SetResolution: true);
	}

	private void UpdateShadowButton()
	{
		if (m_QualityGadget.GetValue() == 0 || m_QualityGadget.GetValue() == 1)
		{
			m_ShadowsGadget.SetInteractable(Interactable: false);
		}
		else
		{
			m_ShadowsGadget.SetInteractable(Interactable: true);
		}
	}

	public void OnGraphicsClicked(BaseGadget NewGadget)
	{
		if (NewGadget == m_ResolutionGadget)
		{
			m_Resolution = m_ResolutionGadget.GetValue();
			UpdateResolution();
		}
		else if (NewGadget == m_FullScreenGadget)
		{
			m_FullScreen = m_FullScreenGadget.GetOn();
			UpdateResolution();
		}
		else if (NewGadget == m_QualityGadget)
		{
			UpdateShadowButton();
			SettingsManager.Instance.SetQuality(m_QualityGadget.GetValue());
		}
		else if (NewGadget == m_AmbientOcclusionGadget)
		{
			SettingsManager.Instance.SetAmbientOcclusion(m_AmbientOcclusionGadget.GetOn());
		}
		else if (NewGadget == m_LightsGadget)
		{
			SettingsManager.Instance.SetLights(m_LightsGadget.GetOn());
		}
		else if (NewGadget == m_ShadowsGadget)
		{
			SettingsManager.Instance.SetShadows(m_ShadowsGadget.GetOn());
		}
		else if (NewGadget == m_BloomGadget)
		{
			SettingsManager.Instance.SetBloom(m_BloomGadget.GetOn());
		}
		else if (NewGadget == m_AntialiasingGadget)
		{
			SettingsManager.Instance.SetAntialiasing(m_AntialiasingGadget.GetOn());
		}
	}

	public void OnControlsClicked(BaseGadget NewGadget)
	{
		ControllerBinding component = NewGadget.GetComponent<ControllerBinding>();
		GameStateManager.Instance.PushState(GameStateManager.State.AnyKey);
		m_CurrentControllerButton = component;
		GameStateManager.Instance.GetCurrentState().GetComponent<GameStateAnyKey>().SetAem(component.m_Aem);
	}

	private void CheckFullScreenToggle()
	{
		if (m_FullScreenGadget.GetOn() != SettingsManager.Instance.GetFullScreen())
		{
			m_FullScreenGadget.SetOn(SettingsManager.Instance.GetFullScreen());
		}
	}

	protected new void Update()
	{
		base.Update();
		CheckFullScreenToggle();
	}
}
