using UnityEngine;

public class BaseButtonText : BaseButton
{
	private string m_StartText = "";

	private BaseText m_Text;

	protected new void Awake()
	{
		base.Awake();
	}

	private void CheckText()
	{
		if (m_Text == null)
		{
			m_Text = base.transform.Find("Text").GetComponent<BaseText>();
		}
	}

	public string GetText()
	{
		CheckText();
		return m_Text.GetText();
	}

	public void SetText(string NewText)
	{
		CheckText();
		m_Text.SetText(NewText);
	}

	public void SetTextFromID(string NewText)
	{
		CheckText();
		m_Text.SetTextFromID(NewText);
	}

	public void SetStartText(string NewText)
	{
		m_StartText = NewText;
		SetText(NewText);
	}

	public void SetStartTextFromID(string NewText)
	{
		m_StartText = NewText;
		SetTextFromID(NewText);
	}

	public string GetStartText()
	{
		return m_StartText;
	}

	public override void BaseSetInteractable(bool Interactable)
	{
		base.BaseSetInteractable(Interactable);
		CheckText();
		Color interactableColour = GetInteractableColour(Interactable, m_Text.GetColour());
		m_Text.SetColour(interactableColour);
	}

	public override void BaseSetIndicated(bool Indicated)
	{
		base.BaseSetIndicated(Indicated);
		CheckText();
		float indicatedScale = GetIndicatedScale();
		m_Text.transform.localScale = new Vector3(indicatedScale, indicatedScale, indicatedScale);
	}
}
