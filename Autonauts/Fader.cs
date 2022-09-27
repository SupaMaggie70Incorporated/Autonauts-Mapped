using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
	private bool m_Fading;

	private bool m_FadeUp;

	private float m_FadeDelay;

	private float m_Timer;

	private Color m_FadeColour;

	private Image m_Image;

	private void Awake()
	{
		m_Fading = false;
		m_Image = GetComponent<Image>();
	}

	public void StartFade(bool Up, float Delay, Color FadeColour)
	{
		m_Fading = true;
		m_FadeUp = Up;
		m_FadeDelay = Delay;
		m_FadeColour = FadeColour;
		UpdateFade();
	}

	private void UpdateFade()
	{
		float num = m_Timer / m_FadeDelay;
		Color fadeColour = m_FadeColour;
		if (m_FadeUp)
		{
			fadeColour.a = num;
		}
		else
		{
			fadeColour.a = 1f - num;
		}
		if ((bool)m_Image)
		{
			m_Image.color = fadeColour;
		}
	}

	private void Update()
	{
		if (m_Fading)
		{
			m_Timer += TimeManager.Instance.m_NormalDelta;
			if (m_Timer >= m_FadeDelay)
			{
				m_Timer = m_FadeDelay;
				m_Fading = false;
			}
			UpdateFade();
		}
	}
}
