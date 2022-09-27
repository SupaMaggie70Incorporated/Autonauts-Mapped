public class PowerValue : BaseImage
{
	private BaseText m_Value;

	protected new void Awake()
	{
		base.Awake();
	}

	private void CheckValue()
	{
		if (m_Value == null)
		{
			m_Value = base.transform.Find("BaseText").GetComponent<BaseText>();
		}
	}

	public void SetValue(int Value)
	{
		CheckValue();
		m_Value.SetText(Value.ToString());
	}
}
