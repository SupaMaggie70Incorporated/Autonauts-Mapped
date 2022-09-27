public class SilkwormStationRollover : GeneralRollover
{
	private BaseText m_Text;

	private SilkwormStation m_Target;

	private ConverterRequirementFuel m_Fuel;

	private BaseText m_CurrentText;

	private BaseText m_TotalText;

	private BaseImage m_Image;

	private BaseText m_Description;

	private BaseText m_SilkText;

	protected new void Awake()
	{
		base.Awake();
		m_Target = null;
		m_Fuel = m_Panel.transform.Find("ConverterRequirementFuel").GetComponent<ConverterRequirementFuel>();
		m_CurrentText = m_Panel.transform.Find("CountCurrent").GetComponent<BaseText>();
		m_TotalText = m_Panel.transform.Find("CountTotal").GetComponent<BaseText>();
		if ((bool)m_Panel.transform.Find("Description"))
		{
			m_Description = m_Panel.transform.Find("Description").GetComponent<BaseText>();
		}
		m_SilkText = m_Panel.transform.Find("SilkCount").GetComponent<BaseText>();
		Hide();
	}

	protected override void UpdateTarget()
	{
		base.UpdateTarget();
		m_CurrentText.SetText(m_Target.GetStored().ToString());
		m_TotalText.SetText(m_Target.GetCapacity().ToString());
		m_SilkText.SetText(m_Target.GetSilkCount().ToString());
		m_Fuel.UpdateFuel(m_Target.GetFuelPercent(), m_Target.GetFuel());
	}

	public void SetTarget(SilkwormStation Target)
	{
		if (Target != m_Target)
		{
			m_Target = Target;
			if ((bool)m_Target)
			{
				m_Title.SetText(m_Target.GetHumanReadableName());
				UpdateTarget();
			}
		}
	}

	protected override bool GetTargettingSomething()
	{
		return m_Target != null;
	}
}
