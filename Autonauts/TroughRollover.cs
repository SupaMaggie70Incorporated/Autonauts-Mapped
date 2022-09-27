using UnityEngine;

public class TroughRollover : Rollover
{
	[HideInInspector]
	public Trough m_Target;

	private BaseProgressBar m_Hay;

	private BaseText m_Title;

	protected new void Awake()
	{
		base.Awake();
		m_Target = null;
		m_Title = m_Panel.transform.Find("Title").GetComponent<BaseText>();
		m_Hay = m_Panel.transform.Find("HayProgressBar").GetComponent<BaseProgressBar>();
		Hide();
	}

	protected override void UpdateTarget()
	{
		m_Hay.SetValue(m_Target.GetHayPercent());
	}

	public void SetTarget(Trough Target)
	{
		if (Target != m_Target)
		{
			m_Target = Target;
			if ((bool)m_Target)
			{
				string humanReadableNameFromIdentifier = ObjectTypeList.Instance.GetHumanReadableNameFromIdentifier(m_Target.m_TypeIdentifier);
				m_Title.SetText(humanReadableNameFromIdentifier);
				UpdateTarget();
			}
		}
	}

	protected override bool GetTargettingSomething()
	{
		return m_Target != null;
	}
}
