using UnityEngine;

public class Error : BasePanelOptions
{
	private BaseButton m_StopButton;

	private void Awake()
	{
		Transform transform = GetPanel().transform;
		BaseGadget component = transform.Find("StandardAcceptButton").GetComponent<BaseGadget>();
		component.SetAction(OnClick, component);
		m_StopButton = transform.Find("StopBotButton").GetComponent<BaseButton>();
		m_StopButton.SetAction(OnStopClicked, m_StopButton);
	}

	public void SetError(string Type, string Description)
	{
		Transform obj = GetPanel().transform;
		obj.Find("TitleStrip/Title").GetComponent<BaseText>().SetTextFromID(Type);
		obj.Find("Description").GetComponent<BaseText>().SetTextFromID(Description);
		m_StopButton.SetActive(Active: false);
	}

	public void SetText(string Type, string Description)
	{
		Transform obj = GetPanel().transform;
		obj.Find("TitleStrip/Title").GetComponent<BaseText>().SetText(Type);
		obj.Find("Description").GetComponent<BaseText>().SetText(Description);
	}

	public void OnClick(BaseGadget NewGadget)
	{
		GameStateManager.Instance.PopState();
	}

	public void OnStopClicked(BaseGadget NewGadget)
	{
		GameStateError.Instance.StopBot();
		GameStateManager.Instance.PopState();
	}

	public void HideStopBot()
	{
		m_StopButton.SetActive(Active: false);
	}
}
