using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BaseDropdown : BaseGadget
{
	private TMP_Dropdown m_Dropdown;

	private TextMeshProUGUI m_Text;

	private int m_StartValue;

	protected new void Awake()
	{
		base.Awake();
	}

	protected new void OnDestroy()
	{
		base.OnDestroy();
		CheckDropdown();
		m_Dropdown.Hide();
	}

	private void CheckDropdown()
	{
		if (!(m_Dropdown != null))
		{
			m_Dropdown = GetComponent<TMP_Dropdown>();
			m_Text = base.transform.Find("Label").GetComponent<TextMeshProUGUI>();
		}
	}

	public void OnValueChanged()
	{
		DoAction();
	}

	public void SetValue(int NewValue)
	{
		CheckDropdown();
		m_Dropdown.value = NewValue;
	}

	public int GetValue()
	{
		CheckDropdown();
		return m_Dropdown.value;
	}

	public void SetStartValue(int NewValue)
	{
		m_StartValue = NewValue;
		SetValue(NewValue);
	}

	public int GetStartValue()
	{
		return m_StartValue;
	}

	public void ClearOptions()
	{
		CheckDropdown();
		m_Dropdown.ClearOptions();
	}

	public void SetOptions(List<string> Options)
	{
		List<TMP_Dropdown.OptionData> list = new List<TMP_Dropdown.OptionData>();
		foreach (string Option in Options)
		{
			TMP_Dropdown.OptionData item = new TMP_Dropdown.OptionData(Option);
			list.Add(item);
		}
		CheckDropdown();
		m_Dropdown.options = list;
	}

	public void SetOption(int Index, string NewText)
	{
		CheckDropdown();
		m_Dropdown.options[Index].text = NewText;
	}

	public string GetOption(int Index)
	{
		CheckDropdown();
		return m_Dropdown.options[Index].text;
	}

	public void SetLabelText(string NewText)
	{
		base.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = NewText;
	}

	public float GetLabelWidth()
	{
		return base.transform.Find("Label").GetComponent<TextMeshProUGUI>().preferredWidth;
	}

	public override void SetInteractable(bool Interactable)
	{
		base.SetInteractable(Interactable);
		m_Dropdown.interactable = Interactable;
		Color color = new Color(0f, 0f, 0f, 1f);
		if (!Interactable)
		{
			color.a = 0.5f;
		}
		m_Text.color = color;
	}

	public void Show(bool Show)
	{
		CheckDropdown();
		if (Show)
		{
			m_Dropdown.Show();
		}
		else
		{
			m_Dropdown.Hide();
		}
	}
}
