using System;

public class SafeFileDateTime
{
	public string m_Name;

	public DateTime m_DateTime;

	public SafeFileDateTime(string Name, DateTime NewDateTime)
	{
		m_Name = Name;
		m_DateTime = NewDateTime;
	}
}
