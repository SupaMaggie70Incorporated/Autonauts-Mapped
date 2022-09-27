using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BaseAutoCompleteOption : BaseGadget
{
	private Image m_Background;

	[SerializeField]
	public Color m_NormalColour;

	[SerializeField]
	public Color m_HighlightedColour;

	private void CheckGadgets()
	{
		if (m_Background == null)
		{
			m_Background = base.transform.Find("Item Background").GetComponent<Image>();
		}
	}

	public override void SetIndicated(bool Indicated)
	{
		CheckGadgets();
		base.SetIndicated(Indicated);
		if (Indicated)
		{
			m_Background.color = m_HighlightedColour;
		}
		else
		{
			m_Background.color = m_NormalColour;
		}
	}

	public void SetText(string NewText)
	{
		GetComponentInChildren<TextMeshProUGUI>().text = NewText;
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown(eventData);
		DoAction();
	}
}
