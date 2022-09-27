using UnityEngine;

public class LightFire : MyLight
{
	private float m_FireTimer;

	private void Update()
	{
		m_FireTimer += TimeManager.Instance.m_NormalDelta;
		if (m_FireTimer >= 0.05f)
		{
			m_FireTimer = 0f;
			SetIntensity(Random.Range(0.85f, 1.1f));
		}
	}
}
