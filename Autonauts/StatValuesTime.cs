public class StatValuesTime : StatValues
{
	private int m_Time;

	public StatValuesTime(StatsManager.Stat ID)
		: base(ID, Type.Time)
	{
		m_Time = 0;
	}

	public void SetTime(int Time)
	{
		m_Time = Time;
	}

	public override string GetAsString()
	{
		return GeneralUtils.ConvertTimeToString(m_Time);
	}
}
