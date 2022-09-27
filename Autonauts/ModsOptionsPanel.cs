using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using Rewired;
using UnityEngine;

public class ModsOptionsPanel : BaseMenu
{
	private static float m_SettingSpacing = 45f;

	private Transform m_Panel;

	private Mod CurrentMod;

	private BaseScrollView m_ScrollView;

	private BaseText m_ModTitleText;

	private BaseText m_ModDescriptionText;

	private BaseRawImage m_ModImage;

	private BaseText m_ModWarningText;

	private GameObject m_DefaultToggle;

	private GameObject m_DefaultSlider;

	private GameObject m_DefaultDropdown;

	private GameObject m_DefaultKeyButton;

	private GameObject m_DefaultStringEntry;

	private float m_GadgetYOffset;

	private List<ControllerBinding> m_ControllerButtons;

	private List<BaseText> m_ControllerInfo;

	private ControllerBinding m_CurrentControllerButton;

	private Dictionary<ActionElementMap, InputBinding> m_ControllerKeys;

	protected new void Awake()
	{
		base.Awake();
		m_Panel = base.transform.Find("BasePanelOptions").Find("Panel");
		m_ScrollView = m_Panel.Find("SettingsList").GetComponent<BaseScrollView>();
		m_DefaultToggle = m_Panel.Find("BoolOption").gameObject;
		m_DefaultSlider = m_Panel.Find("NumberOption").gameObject;
		m_DefaultDropdown = m_Panel.Find("DropdownOption").gameObject;
		m_DefaultKeyButton = m_Panel.Find("ControllerBinding").gameObject;
		m_DefaultStringEntry = m_Panel.Find("StringOption").gameObject;
	}

	protected new void Start()
	{
		base.Start();
		CurrentMod = ModManager.Instance.MenuSelectedMod;
		m_ModTitleText = m_Panel.Find("ModName").GetComponent<BaseText>();
		m_ModTitleText.SetText(CurrentMod.SteamTitle);
		BaseScrollView component = m_Panel.Find("DescriptionScroll").GetComponent<BaseScrollView>();
		GameObject content = component.GetContent();
		m_ModDescriptionText = content.transform.Find("ModDescription").GetComponent<BaseText>();
		m_ModDescriptionText.SetText(CurrentMod.SteamDescription);
		float preferredHeight = m_ModDescriptionText.GetPreferredHeight();
		component.SetScrollSize(preferredHeight * 0.04f);
		m_ModImage = m_Panel.Find("ModImage").GetComponent<BaseRawImage>();
		m_ModImage.SetTexture(CurrentMod.GetTexture(CurrentMod.SteamImageName));
		m_ModWarningText = m_Panel.Find("WarningTitle").GetComponent<BaseText>();
		m_ModWarningText.SetActive(Active: false);
		SetupList();
		m_DefaultToggle.gameObject.SetActive(value: false);
		m_DefaultSlider.gameObject.SetActive(value: false);
		m_DefaultDropdown.gameObject.SetActive(value: false);
		m_DefaultKeyButton.gameObject.SetActive(value: false);
		m_DefaultStringEntry.gameObject.SetActive(value: false);
	}

	public void Refresh()
	{
	}

	private GameObject CreateGadget(GameObject NewPrefab, string NewName, BaseScrollView NewPage, bool UseLookupText = false)
	{
		Transform parent = NewPage.GetContent().transform;
		Vector2 vector = new Vector2(0f, m_GadgetYOffset);
		m_GadgetYOffset -= m_SettingSpacing;
		GameObject obj = UnityEngine.Object.Instantiate(NewPrefab, parent);
		obj.name = NewName;
		obj.transform.localPosition = vector;
		BaseText component = obj.transform.Find("Text").GetComponent<BaseText>();
		if (UseLookupText)
		{
			component.SetTextFromID(NewName);
			return obj;
		}
		component.SetText(NewName);
		return obj;
	}

	private StandardSlider CreateSlider(string NewName, float DefaultValue, BaseScrollView NewPage, Action<BaseGadget> NewAction, float Min, float Max, bool UseLookupText = false)
	{
		StandardSlider component = CreateGadget(m_DefaultSlider, NewName, NewPage, UseLookupText).GetComponent<StandardSlider>();
		component.SetMinMaxStart(DefaultValue, Min, Max);
		component.SetAction(NewAction, component);
		return component;
	}

	private BaseToggle CreateToggle(string NewName, bool DefaultValue, BaseScrollView NewPage, Action<BaseGadget> NewAction, bool UseLookupText = false)
	{
		BaseToggle component = CreateGadget(m_DefaultToggle, NewName, NewPage, UseLookupText).transform.Find("BaseToggle").GetComponent<BaseToggle>();
		component.name = NewName;
		component.SetStartOn(DefaultValue);
		RegisterGadget(component);
		AddAction(component, NewAction);
		return component;
	}

	private BaseDropdown CreateDropdown(string NewName, int DefaultValue, BaseScrollView NewPage, Action<BaseGadget> NewAction, List<string> Options, bool UseLookupText = false)
	{
		BaseDropdown component = CreateGadget(m_DefaultDropdown, NewName, NewPage, UseLookupText).transform.Find("BaseDropdown").GetComponent<BaseDropdown>();
		component.name = NewName;
		component.SetOptions(Options);
		component.SetStartValue(DefaultValue);
		RegisterGadget(component);
		AddAction(component, NewAction);
		return component;
	}

	private ControllerBinding CreateButtonText(string NewName, ActionElementMap aem, BaseScrollView NewPage, Action<BaseGadget> NewAction, bool UseLookupText = false)
	{
		ControllerBinding component = CreateGadget(m_DefaultKeyButton, NewName, NewPage, UseLookupText).GetComponent<ControllerBinding>();
		component.SetAem(aem, NewAction, LookupText: false);
		component.name = NewName;
		return component;
	}

	private BaseInputField CreateInputField(string NewName, string DefaultValue, BaseScrollView NewPage, Action<BaseGadget> NewAction, bool UseLookupText = false)
	{
		BaseInputField component = CreateGadget(m_DefaultStringEntry, NewName, NewPage, UseLookupText).transform.Find("BaseInputField").GetComponent<BaseInputField>();
		component.name = NewName;
		component.SetStartText(DefaultValue);
		RegisterGadget(component);
		AddAction(component, NewAction);
		return component;
	}

	private void SetupList()
	{
		m_GadgetYOffset = -10f;
		List<string> list = new List<string>();
		List<int> list2 = new List<int>();
		foreach (ModManager.ExposedData exposedVar in CurrentMod.ExposedVars)
		{
			if (exposedVar.IsKeybinding)
			{
				list.Add(exposedVar.VarName);
				list2.Add((int)exposedVar.VarValue.CastToNumber().Value);
			}
			else if (exposedVar.VarValuesList != null && exposedVar.VarValuesList.Count != 0)
			{
				List<string> list3 = new List<string>();
				foreach (DynValue varValues in exposedVar.VarValuesList)
				{
					list3.Add(varValues.String);
				}
				CreateDropdown(exposedVar.VarName, (int)exposedVar.VarValue.Number, m_ScrollView, OnPreferencesClicked, list3, exposedVar.UsesLookup);
			}
			else if (exposedVar.VarType == DataType.Boolean)
			{
				CreateToggle(exposedVar.VarName, exposedVar.VarValue.Boolean, m_ScrollView, OnPreferencesClicked, exposedVar.UsesLookup);
			}
			else if (exposedVar.VarType == DataType.Number)
			{
				CreateSlider(exposedVar.VarName, (float)exposedVar.VarValue.Number, m_ScrollView, OnPreferencesClicked, (float)exposedVar.MinValue.Number, (float)exposedVar.MaxValue.Number);
			}
			else if (exposedVar.VarType == DataType.String)
			{
				CreateInputField(exposedVar.VarName, exposedVar.VarValue.ToPrintString(), m_ScrollView, OnPreferencesClicked);
			}
		}
		m_ControllerButtons = new List<ControllerBinding>();
		m_ControllerKeys = new Dictionary<ActionElementMap, InputBinding>();
		int num = 0;
		while (num != list2.Count)
		{
			foreach (KeyValuePair<ActionElementMap, InputBinding> controlsAction in MyInputManager.Instance.m_ControlsActions)
			{
				ActionElementMap key = controlsAction.Key;
				InputAction action = ReInput.mapping.GetAction(key.actionId);
				int num2 = key.actionId - 49 + 1;
				if (action.categoryId == 1 && num2 == list2[num])
				{
					ControllerBinding item = CreateButtonText(list[num++], key, m_ScrollView, OnControlsClicked);
					m_ControllerButtons.Add(item);
					InputBinding value = new InputBinding(key.keyCode, key.modifierKey1);
					m_ControllerKeys.Add(key, value);
					m_ModWarningText.SetActive(Active: true);
					break;
				}
			}
		}
		m_ScrollView.SetScrollSize(0f - m_GadgetYOffset);
		foreach (ControllerBinding controllerButton in m_ControllerButtons)
		{
			controllerButton.UpdateText(LookupText: false);
		}
	}

	public void OnPreferencesClicked(BaseGadget NewGadget)
	{
		if ((bool)NewGadget.GetComponent<BaseToggle>())
		{
			CurrentMod.UpdateExposedVariable(NewGadget.name, DynValue.NewBoolean(NewGadget.GetComponent<BaseToggle>().GetOn()));
		}
		if ((bool)NewGadget.GetComponent<StandardSlider>())
		{
			CurrentMod.UpdateExposedVariable(NewGadget.name, DynValue.NewNumber(NewGadget.GetComponent<StandardSlider>().GetValueScaled()));
		}
		if ((bool)NewGadget.GetComponent<BaseInputField>())
		{
			CurrentMod.UpdateExposedVariable(NewGadget.name, DynValue.NewString(NewGadget.GetComponent<BaseInputField>().GetText()));
		}
		if ((bool)NewGadget.GetComponent<BaseDropdown>())
		{
			CurrentMod.UpdateExposedVariable(NewGadget.name, DynValue.NewNumber(NewGadget.GetComponent<BaseDropdown>().GetValue()));
		}
	}

	public void UpdateControllerButtons()
	{
		foreach (ControllerBinding controllerButton in m_ControllerButtons)
		{
			controllerButton.UpdateText(LookupText: false);
		}
	}

	private void ControllerButtonPressed(ControllerBinding NewButton)
	{
		GameStateManager.Instance.PushState(GameStateManager.State.ModsAnyKey);
		m_CurrentControllerButton = NewButton;
		GameStateManager.Instance.GetCurrentState().GetComponent<GameStateModsAnyKey>().SetAem(NewButton.m_Aem, NewButton.name);
	}

	public void OnControlsClicked(BaseGadget NewGadget)
	{
		ControllerBinding component = NewGadget.GetComponent<ControllerBinding>();
		GameStateManager.Instance.PushState(GameStateManager.State.ModsAnyKey);
		m_CurrentControllerButton = component;
		GameStateManager.Instance.GetCurrentState().GetComponent<GameStateModsAnyKey>().SetAem(component.m_Aem, component.name);
	}

	public override void OnBackClicked(BaseGadget NewGadget)
	{
		ApplyChanges();
		base.OnBackClicked(NewGadget);
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

	public void ConfirmQuit()
	{
		ApplyChanges();
		GameStateManager.Instance.PopState();
	}

	public void CancelChanges()
	{
		foreach (KeyValuePair<ActionElementMap, InputBinding> controllerKey in m_ControllerKeys)
		{
			SettingsManager.Instance.SetKeyCode(controllerKey.Key, controllerKey.Value.m_Code, controllerKey.Value.m_ModifierKey);
		}
	}

	private void ApplyChanges()
	{
		SettingsManager.Instance.Save();
	}

	protected new void Update()
	{
		base.Update();
	}
}
