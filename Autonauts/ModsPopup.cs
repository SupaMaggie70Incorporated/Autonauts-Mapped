using UnityEngine;

public class ModsPopup : BaseMenu
{
	private Transform m_Panel;

	private BaseText TitleText;

	private BaseText DescriptionText;

	private BaseScrollView DescriptionScroll;

	protected new void Awake()
	{
		base.Awake();
		m_Panel = base.transform.Find("BasePanelOptions").Find("Panel");
	}

	private void CheckGadgets()
	{
		if (!TitleText)
		{
			TitleText = m_Panel.Find("ErrorTitle").GetComponent<BaseText>();
			DescriptionScroll = m_Panel.Find("DescriptionScroll").GetComponent<BaseScrollView>();
			GameObject content = DescriptionScroll.GetContent();
			DescriptionText = content.transform.Find("ModDescription").GetComponent<BaseText>();
			StandardAcceptButton component = m_Panel.Find("StandardAcceptButton").GetComponent<StandardAcceptButton>();
			AddAction(component, OnOKClicked);
		}
	}

	public void SetInformation(string Title, string Description)
	{
		CheckGadgets();
		TitleText.SetText(Title);
		DescriptionText.SetText(Description);
		float preferredHeight = DescriptionText.GetPreferredHeight();
		DescriptionScroll.SetScrollSize(preferredHeight * 0.04f);
	}

	public void SetInformationFromID(string TitleID, string DescriptionID)
	{
		CheckGadgets();
		TitleText.SetTextFromID(TitleID);
		DescriptionText.SetTextFromID(DescriptionID);
		float preferredHeight = DescriptionText.GetPreferredHeight();
		DescriptionScroll.SetScrollSize(preferredHeight * 0.04f);
	}

	public void OnOKClicked(BaseGadget NewGadget)
	{
		GameStateManager.Instance.PopState();
	}

	protected new void Update()
	{
		base.Update();
	}
}
