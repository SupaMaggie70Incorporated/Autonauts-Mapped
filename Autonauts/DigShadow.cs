using UnityEngine;

public class DigShadow : BaseClass
{
	private float m_Timer;

	public override void Restart()
	{
		base.Restart();
		m_Timer = 0f;
		base.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
	}

	private void Update()
	{
		if (!(TimeManager.Instance == null))
		{
			m_Timer += TimeManager.Instance.m_NormalDelta;
			if (m_Timer > 0.25f)
			{
				StopUsing();
			}
		}
	}
}
