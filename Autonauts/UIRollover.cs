public class UIRollover : Rollover
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

	protected new void Start()
	{
		SetWidth(200f);
	}

	protected override void UpdateTarget()
	{
		m_Text.SetText(m_Target);
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
