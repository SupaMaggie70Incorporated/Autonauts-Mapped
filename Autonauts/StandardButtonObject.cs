public class StandardButtonObject : BaseButtonImage
{
	private ObjectType m_Type;

	private bool m_UseGadgetRollover;

	public void SetObjectType(ObjectType NewType)
	{
		m_Type = NewType;
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

	public void UseGadgetRollover()
	{
		m_UseGadgetRollover = true;
	}

	public override void SetIndicated(bool Indicated)
	{
		base.SetIndicated(Indicated);
		BaseSetIndicated(Indicated);
		if (!m_UseGadgetRollover)
		{
			HudManager.Instance.ActivateHoldableRollover(Indicated, m_Type);
		}
	}
}
