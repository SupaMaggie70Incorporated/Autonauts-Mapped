using UnityEngine;

public class GameCompleteBadge : MonoBehaviour
{
	private static float m_LifeDelay = 1f / 3f;

	private BaseImage[] m_Rays;

	private float[] m_Rotations;

	private float[] m_RotationDeltas;

	private float[] m_RotationDeltaTargets;

	private float[] m_Alphas;

	private float[] m_Timers;

	private void Awake()
	{
		m_Rays = new BaseImage[3];
		m_Rotations = new float[3];
		m_RotationDeltas = new float[3];
		m_RotationDeltaTargets = new float[3];
		m_Alphas = new float[3];
		m_Timers = new float[3];
		for (int i = 0; i < 3; i++)
		{
			m_Rays[i] = base.transform.Find("Rays" + (i + 1)).GetComponent<BaseImage>();
			m_Rotations[i] = Random.Range(0f, 360f);
			m_RotationDeltas[i] = 0f;
			m_RotationDeltaTargets[i] = Random.Range(0.025f, 0.05f) * 57.29578f;
			m_Timers[i] = (float)(3 - i) * (1f / 3f) * m_LifeDelay;
		}
	}

	private void Update()
	{
		float normalDelta = TimeManager.Instance.m_NormalDelta;
		for (int i = 0; i < 3; i++)
		{
			float z = m_Rays[i].transform.rotation.eulerAngles.z;
			m_Timers[i] += normalDelta;
			if (m_Timers[i] >= m_LifeDelay)
			{
				m_Timers[i] -= m_LifeDelay;
				m_Rotations[i] = Random.Range(0f, 360f);
				z = m_Rotations[i];
				if (m_RotationDeltaTargets[i] != 0f)
				{
					m_RotationDeltaTargets[i] = Random.Range(0.025f, 0.05f) * 57.29578f;
					m_RotationDeltas[i] = m_RotationDeltaTargets[i];
				}
			}
			else
			{
				if (m_RotationDeltas[i] != m_RotationDeltaTargets[i])
				{
					if (m_RotationDeltas[i] < m_RotationDeltaTargets[i])
					{
						m_RotationDeltas[i] += 0.0025f * normalDelta * 60f * 57.29578f;
						if (m_RotationDeltas[i] > m_RotationDeltaTargets[i])
						{
							m_RotationDeltas[i] = m_RotationDeltaTargets[i];
						}
					}
					else
					{
						m_RotationDeltas[i] -= 0.0025f * normalDelta * 60f * 57.29578f;
						if (m_RotationDeltas[i] < m_RotationDeltaTargets[i])
						{
							m_RotationDeltas[i] = m_RotationDeltaTargets[i];
						}
					}
				}
				z += m_RotationDeltas[i] * normalDelta * 60f * 0.1f;
			}
			m_Rays[i].transform.rotation = Quaternion.Euler(0f, 0f, z);
			float num = m_Timers[i] / m_LifeDelay;
			float a = ((!(num < 0.5f)) ? (1f - (num - 0.5f) / 0.5f) : (num / 0.5f));
			m_Rays[i].SetColour(new Color(1f, 1f, 1f, a));
		}
	}
}
