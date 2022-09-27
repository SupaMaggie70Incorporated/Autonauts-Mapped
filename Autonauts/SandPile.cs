using UnityEngine;

public class SandPile : BaseClass
{
	private bool m_Melting;

	private float m_MeltTimer;

	public override void Restart()
	{
		base.Restart();
		m_Melting = false;
		m_MeltTimer = 0f;
		SetProgress(0f);
	}

	public void SetProgress(float Progress)
	{
		base.transform.localScale = new Vector3(1f, Progress + 0.01f, 1f);
	}

	public void Melt()
	{
		m_Melting = true;
	}

	private void Update()
	{
		if (!(TimeManager.Instance == null) && m_Melting)
		{
			m_MeltTimer += TimeManager.Instance.m_NormalDelta;
			float num = 1f;
			float num2 = m_MeltTimer / num;
			base.transform.localScale = new Vector3(1f, 1f - num2, 1f);
			if (m_MeltTimer > num)
			{
				StopUsing();
			}
		}
	}
}
