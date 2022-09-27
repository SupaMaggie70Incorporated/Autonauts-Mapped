using UnityEngine;

public class IndustryButton : BaseButtonImage
{
	[HideInInspector]
	public Industry m_Industry;

	public void Init(Industry NewIndustry)
	{
		m_Industry = NewIndustry;
		SetSprite(NewIndustry.GetIcon());
		SetRolloverFromID(NewIndustry.m_Name);
		UpdatePanelColour();
		base.transform.Find("Pin").gameObject.SetActive(value: false);
	}

	private void UpdatePanelColour()
	{
		Color panelColour = m_Industry.m_PanelColour;
		if (!m_Industry.GetAnyLevelsUnlocked())
		{
			float num = 0.35f;
			panelColour.r *= num;
			panelColour.g *= num;
			panelColour.b *= num;
		}
		SetBackingColour(panelColour);
	}

	public override void SetInteractable(bool Interactable)
	{
		base.SetInteractable(Interactable);
		BaseSetInteractable(Interactable);
	}

	public override void SetSelected(bool Selected)
	{
		base.SetSelected(Selected);
		BaseSetSelected(Selected);
	}

	public override void SetIndicated(bool Indicated)
	{
		base.SetIndicated(Indicated);
		BaseSetIndicated(Indicated);
	}
}
