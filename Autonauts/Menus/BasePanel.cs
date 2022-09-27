using UnityEngine;
using UnityEngine.UI;

public class BasePanel : BaseGadget
{
	private PanelBacking m_Back;

	private Image m_Border;

	private void UpdateGadgets()
	{
		if (m_Back == null)
		{
			m_Back = base.transform.Find("Back").GetComponent<PanelBacking>();
			m_Border = base.transform.Find("Border").GetComponent<Image>();
		}
	}

	public void SetBorderVisible(bool Visible)
	{
		UpdateGadgets();
		m_Border.gameObject.SetActive(Visible);
	}

	public void SetBorderColour(Color NewColour)
	{
		UpdateGadgets();
		m_Border.color = NewColour;
	}

	public void SetBackingColour(Color NewColour)
	{
		UpdateGadgets();
		m_Back.SetColour(NewColour);
	}

	public Color GetBackingColour()
	{
		UpdateGadgets();
		return m_Back.GetColour();
	}

	public void SetBackingGradient(string Name)
	{
		UpdateGadgets();
		m_Back.SetBackingGradient(Name);
	}
}
