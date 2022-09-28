using System;
using Rewired;
using UnityEngine;

public class ControllerBinding : BaseGadget
{
	public ActionElementMap m_Aem;

	private BaseButtonText m_Button;

	public void SetAem(ActionElementMap aem, Action<BaseGadget> ClickAction, bool LookupText = true)
	{
		m_Aem = aem;
		UpdateText(LookupText);
		if (ClickAction == null)
		{
			m_Button.BaseSetIndicated(Indicated: false);
		}
		else
		{
			SetAction(ClickAction, this);
		}
	}

	private void CheckGadgets()
	{
		if (m_Button == null)
		{
			m_Button = base.transform.Find("BaseButtonText").GetComponent<BaseButtonText>();
			m_Button.SetAction(OnClick, m_Button);
		}
	}

	private string GetTitleText()
	{
		InputAction action = ReInput.mapping.GetAction(m_Aem.actionId);
		return "RewiredAction" + action.name;
	}

	public static string GetControllerText(ActionElementMap Aem)
	{
		return TextManager.Instance.GetFromAem(Aem.keyCode, Aem.modifierKey1);
	}

	private float GetButtonWidth()
	{
		string text = "ButtonMainMenuSmall";
		if (0 == 0)
		{
			text = "ButtonMainMenuLarge";
		}
		m_Button.SetBackingSprite("Buttons/" + text);
		return 110f;
	}

	public void UpdateText(bool LookupText = true)
	{
		CheckGadgets();
		string text = "";
		if (LookupText)
		{
			text = GetTitleText();
			base.transform.Find("Text").GetComponent<BaseText>().SetTextFromID(text);
		}
		text = GetControllerText(m_Aem);
		m_Button.SetText(text);
		float buttonWidth = GetButtonWidth();
		m_Button.SetWidth(buttonWidth);
		if (m_Aem.keyCode == KeyCode.None)
		{
			m_Button.SetBackingColour(new Color(1f, 0f, 0f));
		}
		else
		{
			m_Button.SetBackingColour(new Color(1f, 1f, 1f));
		}
	}

	public void OnClick(BaseGadget NewGadget)
	{
		DoAction();
	}
}
