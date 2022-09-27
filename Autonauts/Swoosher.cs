using System;
using UnityEngine;

public class Swoosher : MonoBehaviour
{
	private Vector3 m_Start;

	private Vector3 m_End;

	private float m_StartScale;

	private float m_EndScale;

	private float m_Delay;

	private float m_YOffset;

	private Action m_EndFunction;

	private float m_Timer;

	public bool m_Active;

	private bool m_Local;

	public bool m_UpdateWhilePaused;

	private void Awake()
	{
		m_Active = false;
	}

	public void StartSwoosh(Vector3 Start, Vector3 End, float StartScale, float EndScale, float Delay, float YOffset, Action EndFunction, bool Local = false)
	{
		m_Start = Start;
		m_End = End;
		m_StartScale = StartScale;
		m_EndScale = EndScale;
		m_Delay = Delay;
		m_YOffset = YOffset;
		m_EndFunction = EndFunction;
		m_Timer = 0f;
		m_Active = true;
		m_Local = Local;
		UpdatePosition();
	}

	private void UpdatePosition()
	{
		float num = m_Timer / m_Delay;
		if (num >= 1f)
		{
			if (m_Local)
			{
				base.transform.localPosition = m_End;
			}
			else
			{
				base.transform.position = m_End;
			}
			base.transform.localScale = new Vector3(m_EndScale, m_EndScale, 1f);
			if (m_EndFunction != null)
			{
				m_EndFunction();
			}
			return;
		}
		num = (0f - Mathf.Cos(num * (float)Math.PI)) * 0.5f + 0.5f;
		Vector3 vector = (m_End - m_Start) * num + m_Start;
		vector.y += Mathf.Sin(num * (float)Math.PI) * m_YOffset;
		if (m_Local)
		{
			base.transform.localPosition = vector;
		}
		else
		{
			base.transform.position = vector;
		}
		float num2 = (m_EndScale - m_StartScale) * num + m_StartScale;
		base.transform.localScale = new Vector3(num2, num2, 1f);
	}

	private void Update()
	{
		if (m_Active)
		{
			if (m_UpdateWhilePaused)
			{
				m_Timer += TimeManager.Instance.m_PauseDelta;
			}
			else
			{
				m_Timer += TimeManager.Instance.m_NormalDelta;
			}
			UpdatePosition();
		}
	}
}
