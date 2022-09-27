public class BaseButtonBack : BaseButtonImage
{
	protected new void Awake()
	{
		base.Awake();
		m_OnClickSound = "UIOptionCancelled";
	}

	public override void SetInteractable(bool Interactable)
	{
		base.SetInteractable(Interactable);
		BaseSetInteractable(Interactable);
	}

	public override void SetIndicated(bool Indicated)
	{
		base.SetIndicated(Indicated);
		BaseSetIndicated(Indicated);
	}
}
