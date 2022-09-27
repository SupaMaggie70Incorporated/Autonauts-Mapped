public class OK : BasePanelOptions
{
	private void Awake()
	{
		BaseGadget component = GetPanel().transform.Find("StandardAcceptButton").GetComponent<BaseGadget>();
		component.SetAction(OnClick, component);
	}

	public void SetMessage(string Type)
	{
		SetTitleTextFromID(Type);
	}

	public void OnClick(BaseGadget NewGadget)
	{
		GameStateManager.Instance.PopState();
	}
}
