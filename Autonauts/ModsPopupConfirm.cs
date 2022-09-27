using MoonSharp.Interpreter;
using UnityEngine;

public class ModsPopupConfirm : BaseMenu
{
	private Transform m_Panel;

	private BaseText TitleText;

	private BaseText DescriptionText;

	private BaseScrollView DescriptionScroll;

	private DynValue MainCallbackOK;

	private DynValue MainCallbackCancel;

	private Script MainOwner;

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
			StandardAcceptButton component2 = m_Panel.Find("StandardCancelButton").GetComponent<StandardAcceptButton>();
			AddAction(component2, OnCancelClicked);
		}
	}

	public void SetInformation(string Title, string Description, DynValue CallbackOK, DynValue CallbackCancel, Script Owner)
	{
		CheckGadgets();
		TitleText.SetText(Title);
		DescriptionText.SetText(Description);
		float preferredHeight = DescriptionText.GetPreferredHeight();
		DescriptionScroll.SetScrollSize(preferredHeight * 0.04f);
		MainCallbackOK = CallbackOK;
		MainCallbackCancel = CallbackCancel;
		MainOwner = Owner;
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
		DynValue[] args = new DynValue[0];
		ModManager.Instance.Callback(MainOwner, MainCallbackOK, args);
	}

	public void OnCancelClicked(BaseGadget NewGadget)
	{
		GameStateManager.Instance.PopState();
		DynValue[] args = new DynValue[0];
		ModManager.Instance.Callback(MainOwner, MainCallbackCancel, args);
	}

	protected new void Update()
	{
		base.Update();
	}
}
