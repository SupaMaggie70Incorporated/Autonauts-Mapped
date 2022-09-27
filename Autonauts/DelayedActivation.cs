using System;

public class DelayedActivation
{
	private float m_Timer;

	private Action m_ActivateCallback;

	public DelayedActivation(float Delay, Action ActivateCallback)
	{
		m_Timer = Delay;
		m_ActivateCallback = ActivateCallback;
	}

	public bool Update()
	{
		m_Timer -= TimeManager.Instance.m_NormalDelta;
		if (m_Timer < 0f)
		{
			m_ActivateCallback();
			return false;
		}
		return true;
	}
}
