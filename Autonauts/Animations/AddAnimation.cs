using UnityEngine;

public class AddAnimation
{
	public float m_Timer;

	public bool m_Adding;

	private Vector3 m_StartScale;

	private BaseClass m_Object;

	public AddAnimation(BaseClass NewObject, bool Adding)
	{
		m_Timer = 0.075f;
		m_Adding = Adding;
		m_Object = NewObject;
		m_StartScale = NewObject.transform.localScale;
	}

	public bool Update()
	{
		m_Timer -= TimeManager.Instance.m_NormalDelta;
		if (m_Timer < 0f)
		{
			m_Timer = 0f;
			m_Object.transform.localScale = m_StartScale;
			return false;
		}
		Vector3 vector = ((!m_Adding) ? new Vector3(0.9f, 1.1f, 0.9f) : new Vector3(1.1f, 0.9f, 1.1f));
		Vector3 startScale = m_StartScale;
		startScale.x *= vector.x;
		startScale.y *= vector.y;
		startScale.z *= vector.z;
		m_Object.transform.localScale = startScale;
		return true;
	}
}
