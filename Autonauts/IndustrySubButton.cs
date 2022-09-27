using UnityEngine;

public class IndustrySubButton : BaseButtonImage
{
	[HideInInspector]
	public IndustrySub m_IndustrySub;

	public void Init(IndustrySub NewIndustrySub)
	{
		m_IndustrySub = NewIndustrySub;
		SetSprite(NewIndustrySub.GetIcon());
		SetRolloverFromID(NewIndustrySub.m_Name);
		UpdatePanelColour();
		base.transform.Find("Star").gameObject.gameObject.SetActive(NewIndustrySub.GetAllQuestsCompleted());
		base.transform.Find("Pin").gameObject.SetActive(value: false);
	}

	private void UpdatePanelColour()
	{
		Color panelColour = m_IndustrySub.m_Parent.m_PanelColour;
		if (!m_IndustrySub.GetAnyLevelsUnlocked())
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
