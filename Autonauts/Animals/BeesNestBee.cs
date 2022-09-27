using System;
using UnityEngine;

public class BeesNestBee
{
	private static float m_Speed = 15f;

	private static float m_WobbleHeight = 1.5f;

	public GameObject m_Bee;

	private Vector3 m_Position;

	private Vector3 m_CurrentPosition;

	private BeesNest m_Nest;

	private float m_Timer;

	public BeesNestBee(GameObject NewBee, Vector3 Position, BeesNest Nest)
	{
		m_Bee = NewBee;
		m_Position = Position;
		m_Nest = Nest;
		m_Timer = UnityEngine.Random.Range(0f, 2f);
		m_Bee.transform.position = m_Nest.transform.position + m_Position;
		m_CurrentPosition = m_Bee.transform.position;
	}

	public void UpdateNest()
	{
		m_Bee.transform.position = m_Nest.transform.position + m_Position;
		m_CurrentPosition = m_Bee.transform.position;
	}

	public void Update()
	{
		if (TimeManager.Instance.m_NormalDelta > 0f)
		{
			Vector3 vector = m_Nest.transform.position + m_Position;
			Vector3 vector2 = vector - m_CurrentPosition;
			if (vector2.magnitude > m_Speed * TimeManager.Instance.m_NormalDelta)
			{
				vector2.Normalize();
				m_CurrentPosition += vector2 * TimeManager.Instance.m_NormalDelta * m_Speed;
			}
			vector2 = vector - m_Bee.transform.position;
			vector2.Normalize();
			float y = (0f - Mathf.Atan2(vector2.z, vector2.x)) * 57.29578f;
			m_Bee.transform.rotation = Quaternion.Euler(0f, y, 0f);
			Vector3 currentPosition = m_CurrentPosition;
			m_Timer += TimeManager.Instance.m_NormalDelta;
			float num = Mathf.Cos(m_Timer * (float)Math.PI * 2f) * m_WobbleHeight;
			float num2 = Mathf.Sin(m_Timer * (float)Math.PI * 3f) * m_WobbleHeight;
			float num3 = Mathf.Sin(m_Timer * (float)Math.PI * 2f) * m_WobbleHeight;
			currentPosition.x += num;
			currentPosition.y += num2;
			currentPosition.z += num3;
			if (currentPosition.y < 0f)
			{
				currentPosition.y = 0f;
			}
			m_Bee.transform.position = currentPosition;
		}
	}
}
