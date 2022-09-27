public class GeneralRollover : Rollover
{
	protected BaseText m_Title;

	protected new void Awake()
	{
		base.Awake();
		m_Title = m_Panel.transform.Find("Title").GetComponent<BaseText>();
	}
}
