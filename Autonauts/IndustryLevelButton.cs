using UnityEngine;

public class IndustryLevelButton : BaseButtonText
{
	[HideInInspector]
	public IndustryLevel m_Level;

	public void Init(IndustryLevel NewLevel)
	{
		m_Level = NewLevel;
		SetText((NewLevel.m_Level + 1).ToString());
		UpdatePanelColour();
		if (!NewLevel.m_Quest.GetIsComplete() || m_Level.m_Quest.GetState() == Quest.State.Unavailable)
		{
			base.transform.Find("Star").gameObject.SetActive(value: false);
		}
		base.transform.Find("Pin").gameObject.SetActive(value: false);
	}

	private void UpdatePanelColour()
	{
		Color panelColour = m_Level.m_Parent.m_Parent.m_PanelColour;
		if (m_Level.m_Quest.GetState() == Quest.State.Unavailable)
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
		if (Selected)
		{
			GameStateIndustry.SetSelectedQuest(m_Level.m_Quest);
		}
		else if (GameStateIndustry.m_SelectedQuest == m_Level.m_Quest)
		{
			GameStateIndustry.SetSelectedQuest(null);
		}
		BaseSetSelected(Selected);
	}

	public override void SetIndicated(bool Indicated)
	{
		base.SetIndicated(Indicated);
		BaseSetIndicated(Indicated);
		HudManager.Instance.ActivateQuestRollover(Indicated, m_Level.m_Quest);
	}
}
