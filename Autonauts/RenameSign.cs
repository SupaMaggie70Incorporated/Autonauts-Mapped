public class RenameSign : BasePanelOptions
{
	public static RenameSign Instance;

	private BaseInputField m_InputField;

	private BaseToggle m_AreaLinkedToggle;

	private Sign m_Sign;

	private void Awake()
	{
		Instance = this;
		m_InputField = base.transform.Find("BaseInputField").GetComponent<BaseInputField>();
		m_AreaLinkedToggle = base.transform.Find("AreaLinkedToggle").GetComponent<BaseToggle>();
		GetBackButton().SetAction(OnCancelClicked, GetBackButton());
		BaseGadget component = base.transform.Find("StandardAcceptButton").GetComponent<BaseGadget>();
		component.SetAction(OnOkClicked, component);
		component = base.transform.Find("EditAreaButton").GetComponent<BaseGadget>();
		component.SetAction(OnClickEditArea, component);
		m_Sign = null;
	}

	private void OnDestroy()
	{
		if ((bool)m_Sign && (bool)m_Sign.m_Engager)
		{
			m_Sign.m_Engager.SendAction(new ActionInfo(ActionType.DisengageObject, default(TileCoord)));
		}
	}

	public void SetSign(Sign NewSign)
	{
		m_Sign = NewSign;
		string signName = m_Sign.m_SignName;
		m_InputField.SetText(signName);
		m_InputField.SetPlaceholderText(signName);
		m_AreaLinkedToggle.SetOn(m_Sign.m_AreaLinkedToPosition);
	}

	public void OnOkClicked(BaseGadget NewGadget)
	{
		string text = m_InputField.GetText();
		m_Sign.SetName(text);
		m_Sign.m_AreaLinkedToPosition = m_AreaLinkedToggle.m_On;
		m_Sign.m_Engager.SendAction(new ActionInfo(ActionType.DisengageObject, default(TileCoord)));
		GameStateManager.Instance.PopState();
	}

	public void OnCancelClicked(BaseGadget NewGadget)
	{
		m_Sign.m_Engager.SendAction(new ActionInfo(ActionType.DisengageObject, default(TileCoord)));
		GameStateManager.Instance.PopState();
	}

	public void OnClickEditArea(BaseGadget NewGadget)
	{
		GameStateManager.Instance.PushState(GameStateManager.State.EditArea);
		GameStateManager.Instance.GetCurrentState().GetComponent<GameStateEditArea>().SetIndicator(m_Sign.m_Indicator, m_Sign.m_MaxSize, null);
	}
}
