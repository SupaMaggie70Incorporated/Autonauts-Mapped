public class PauseButton : StandardButtonImage
{
	protected new void Awake()
	{
		base.Awake();
		SetAction(OnClick, this);
	}

	public void OnClick(BaseGadget NewGadget)
	{
		GameStateManager.Instance.PushState(GameStateManager.State.Paused);
	}
}
