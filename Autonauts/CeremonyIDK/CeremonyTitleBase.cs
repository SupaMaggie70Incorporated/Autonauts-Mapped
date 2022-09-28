public class CeremonyTitleBase : CeremonyBase
{
	protected void ShowTitle(bool Show)
	{
		base.transform.Find("TitleStrip").gameObject.SetActive(Show);
	}
}
