using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaseInputField : BaseGadget
{
	private TMP_InputField m_Input;

	private Image m_Image;

	[SerializeField]
	private string m_DefaultInputString = "";

	[SerializeField]
	private Color m_SelectedColour = new Color(1f, 1f, 1f);

	[SerializeField]
	private Color m_UnSelectedColour = new Color(1f, 1f, 1f);

	[HideInInspector]
	public Action<BaseGadget> m_OnValueChangedEvent;

	[HideInInspector]
	public Action<BaseGadget> m_OnEditEndEvent;

	public string m_StartText = "";

	protected new void Awake()
	{
		base.Awake();
		if (m_DefaultInputString != null && m_DefaultInputString != "")
		{
			SetPlaceholderTextFromID(m_DefaultInputString);
		}
	}

	public void OnValueChanged()
	{
		if (m_OnValueChangedEvent != null)
		{
			m_OnValueChangedEvent(this);
		}
	}

	public void OnEditEnd()
	{
		DoAction();
		OnDeSelect();
		if (m_OnEditEndEvent != null)
		{
			m_OnEditEndEvent(this);
		}
	}

	private void CheckGadgets()
	{
		if (m_Input == null)
		{
			m_Input = GetComponent<TMP_InputField>();
			m_Image = GetComponent<Image>();
			m_Image.color = m_UnSelectedColour;
		}
	}

	public string GetText()
	{
		CheckGadgets();
		return m_Input.text;
	}

	public void SetText(string Text)
	{
		CheckGadgets();
		m_Input.text = Text;
	}

	public void SetStartText(string Text)
	{
		m_StartText = Text;
		SetText(Text);
	}

	public override void SetInteractable(bool Interactable)
	{
		base.SetInteractable(Interactable);
		CheckGadgets();
		m_Input.interactable = Interactable;
		Color color = m_Input.textComponent.color;
		if (Interactable)
		{
			color.a = 1f;
		}
		else
		{
			color.a = 0.5f;
		}
		m_Input.textComponent.color = color;
	}

	public void ReadOnly(bool Read)
	{
		CheckGadgets();
		m_Input.readOnly = Read;
	}

	public void SetPlaceholderText(string Text)
	{
		CheckGadgets();
		m_Input.placeholder.GetComponent<TextMeshProUGUI>().text = Text;
	}

	public void SetPlaceholderTextFromID(string Text)
	{
		CheckGadgets();
		m_Input.placeholder.GetComponent<TextMeshProUGUI>().text = TextManager.Instance.Get(Text);
	}

	public void OnSelect()
	{
		CheckGadgets();
		m_Image.color = m_SelectedColour;
	}

	public void OnDeSelect()
	{
		CheckGadgets();
		m_Image.color = m_UnSelectedColour;
	}

	public void ForceFocus()
	{
		m_Input.ActivateInputField();
		m_Input.Select();
	}
}
