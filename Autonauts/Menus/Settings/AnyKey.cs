using System;
using Rewired;
using UnityEngine;

public class AnyKey : BasePanelOptions
{
	private KeyCode m_OriginalCode;

	private ModifierKey m_OriginalModifier;

	private KeyCode m_NewCode;

	private ModifierKey m_NewModifier1;

	private ControllerBinding m_Binding;

	private BaseGadget m_AcceptButton;

	private void Awake()
	{
		GetBackButton().SetAction(OnBackButtonClicked, null);
		m_AcceptButton = base.transform.Find("StandardAcceptButton").GetComponent<BaseGadget>();
		m_AcceptButton.SetAction(OnAcceptButtonClicked, null);
		m_AcceptButton.SetInteractable(Interactable: false);
	}

	private void CheckGadgets()
	{
		if (m_Binding == null)
		{
			m_Binding = base.transform.Find("ControllerBinding").GetComponent<ControllerBinding>();
		}
	}

	public void OnBackButtonClicked(BaseGadget NewGadget)
	{
		SettingsManager.Instance.SetKeyCode(m_Binding.m_Aem, m_OriginalCode, m_OriginalModifier);
		GameStateManager.Instance.PopState();
	}

	public void OnAcceptButtonClicked(BaseGadget NewGadget)
	{
		SettingsManager.Instance.RemoveKeyCode(m_NewCode, m_NewModifier1);
		SettingsManager.Instance.SetKeyCode(m_Binding.m_Aem, m_NewCode, m_NewModifier1);
		GameStateManager.Instance.PopState();
		GameStateManager.Instance.GetCurrentState().GetComponent<GameStateSettings>().m_Menu.UpdateControllerButtons();
	}

	public void SetAem(ActionElementMap aem, Action<BaseGadget> NewAction)
	{
		m_OriginalCode = aem.keyCode;
		m_OriginalModifier = aem.modifierKey1;
		CheckGadgets();
		m_Binding.SetAem(aem, NewAction);
	}

	public void SetKeyCode(KeyCode NewCode, ModifierKey NewModifier, bool NewKey = false)
	{
		m_NewCode = NewCode;
		m_NewModifier1 = NewModifier;
		SettingsManager.Instance.SetKeyCode(m_Binding.m_Aem, NewCode, NewModifier);
		m_Binding.UpdateText();
		if (NewKey)
		{
			m_AcceptButton.SetInteractable(Interactable: true);
		}
	}
}
