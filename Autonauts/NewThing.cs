using UnityEngine;
using UnityEngine.UI;

public class NewThing : MonoBehaviour
{
	private float m_Timer;

	private bool m_UpdateWhilePaused;

	private Image m_Image;

	private void Awake()
	{
		m_Timer = 0f;
		m_Image = GetComponent<Image>();
	}

	public void UpdateWhilePaused()
	{
		m_UpdateWhilePaused = true;
	}

	private void Update()
	{
		if (SettingsManager.Instance.m_FlashiesEnabled)
		{
			if (m_UpdateWhilePaused)
			{
				m_Timer += TimeManager.Instance.m_PauseDelta;
			}
			else
			{
				m_Timer += TimeManager.Instance.m_NormalDelta;
			}
			bool flag = (int)(m_Timer * 60f) % 20 < 14;
			m_Image.enabled = flag;
		}
		else
		{
			m_Image.enabled = true;
		}
	}
}
