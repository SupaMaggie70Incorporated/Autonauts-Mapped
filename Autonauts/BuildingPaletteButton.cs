public class BuildingPaletteButton : BaseButtonImage
{
	public override void SetSelected(bool Selected)
	{
		base.SetSelected(Selected);
		CheckGadgets();
		m_Border.gameObject.SetActive(Selected);
	}
}
