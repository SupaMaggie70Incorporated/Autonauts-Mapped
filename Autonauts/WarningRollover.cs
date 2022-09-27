public class WarningRollover : Rollover
{
	private string m_Target;

	private BaseText m_Text;

	protected new void Awake()
	{
		base.Awake();
		m_Target = "";
		m_Text = m_Panel.transform.Find("Title").GetComponent<BaseText>();
		Hide();
	}

	protected override void UpdateTarget()
	{
		if (m_Target != "")
		{
			m_Text.SetTextFromID(m_Target);
		}
	}

	public void SetTarget(string Target)
	{
		if (m_Target != Target)
		{
			m_Target = Target;
			UpdateTarget();
		}
	}

	protected override bool GetTargettingSomething()
	{
		return m_Target != "";
	}
}
