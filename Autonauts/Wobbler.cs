using System;
using UnityEngine;

public class Wobbler
{
	[HideInInspector]
	public bool m_Wobbling;

	public float m_WobbleTime;

	private float m_WobbleSpeed;

	private float m_Amplitude;

	[HideInInspector]
	public float m_Timer;

	[HideInInspector]
	public float m_Height;

	public bool m_WobbleWhilePaused;

	public void Restart()
	{
		m_Wobbling = false;
		m_Height = 0f;
		m_WobbleWhilePaused = false;
		m_Timer = 0f;
	}

	public void Go(float Time, float Speed, float Amplitude)
	{
		m_Wobbling = true;
		m_WobbleTime = Time;
		m_WobbleSpeed = Speed;
		m_Amplitude = Amplitude;
		m_Height = Amplitude;
		m_Timer = 0f;
	}

	public void Update()
	{
		if (!(TimeManager.Instance == null) && m_Wobbling)
		{
			if (m_WobbleWhilePaused)
			{
				m_Timer += TimeManager.Instance.m_PauseDelta;
			}
			else
			{
				m_Timer += TimeManager.Instance.m_NormalDelta;
			}
			if (m_Timer >= m_WobbleTime)
			{
				m_Timer = m_WobbleTime;
				m_Wobbling = false;
			}
			float num = (1f - m_Timer / m_WobbleTime) * m_Amplitude;
			m_Height = Mathf.Cos(m_Timer / m_WobbleTime * m_WobbleSpeed * 360f * ((float)Math.PI / 180f)) * num;
		}
	}
}
