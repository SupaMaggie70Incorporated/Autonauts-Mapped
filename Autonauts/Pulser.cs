using UnityEngine;

public class Pulser : MonoBehaviour
{
	private float m_Timer;

	private float m_Delay;

	private Vector3 m_StartScale;

	private bool m_CreatedPaused;

	private void Awake()
	{
		m_Timer = 0f;
		m_Delay = 0.25f;
		m_StartScale = base.transform.localScale;
		base.transform.localScale = m_StartScale * 1.2f;
		m_CreatedPaused = TimeManager.Instance.m_PauseTimeEnabled;
	}

	public void Init(float Delay, float Scale)
	{
		m_Delay = Delay;
		base.transform.localScale = m_StartScale * Scale;
	}

	private void Update()
	{
		if (m_CreatedPaused)
		{
			m_Timer += TimeManager.Instance.m_PauseDelta;
		}
		else
		{
			m_Timer += TimeManager.Instance.m_CeremonyDelta;
		}
		if (m_Timer > m_Delay)
		{
			base.transform.localScale = m_StartScale;
			Object.Destroy(this);
		}
	}
}
