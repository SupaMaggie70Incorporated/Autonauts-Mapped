using UnityEngine;

public class MyLight : MonoBehaviour
{
	private Light m_Light;

	private bool m_Active;

	private float m_Intensity;

	public float m_DayIntensity;

	public float m_NightIntensity;

	public float m_Range;

	public Color m_Colour;

	private bool m_Desaturated;

	private void Awake()
	{
		m_Light = base.gameObject.AddComponent<Light>();
		m_Active = true;
		m_Intensity = 1f;
		SetRange(m_Range);
		SetColour(m_Colour);
		UpdateActive();
		UpdateIntensity();
	}

	public void SetRange(float Range)
	{
		m_Range = Range;
		if ((bool)m_Light)
		{
			m_Light.range = m_Range;
		}
	}

	public void SetColour(Color NewColour)
	{
		m_Colour = NewColour;
		UpdateColour();
	}

	public void UpdateColour()
	{
		Color color = m_Colour;
		if (m_Desaturated)
		{
			color = new Color(1f, 1f, 1f, 1f);
		}
		if ((bool)m_Light)
		{
			m_Light.color = color;
		}
	}

	public void SetDayNightIntensity(float DayIntensity, float NightIntensity)
	{
		m_DayIntensity = DayIntensity;
		m_NightIntensity = NightIntensity;
		UpdateIntensity();
	}

	public void SetActive(bool Active)
	{
		m_Active = Active;
		UpdateActive();
	}

	public bool GetActive()
	{
		return m_Active;
	}

	public void UpdateActive()
	{
		if ((bool)SettingsManager.Instance && (bool)m_Light)
		{
			if (m_Active && SettingsManager.Instance.m_LightsEnabled)
			{
				m_Light.enabled = true;
			}
			else
			{
				m_Light.enabled = false;
			}
		}
	}

	public void SetIntensity(float Intensity)
	{
		m_Intensity = Intensity;
		UpdateIntensity();
	}

	public void UpdateIntensity()
	{
		float num = m_Intensity;
		if ((bool)DayNightManager.Instance)
		{
			num *= m_NightIntensity + DayNightManager.Instance.m_LightAdjuster * (m_DayIntensity - m_NightIntensity);
		}
		if ((bool)m_Light)
		{
			m_Light.intensity = num;
		}
	}

	public void SetDesaturationActive(bool Desaturated)
	{
		m_Desaturated = Desaturated;
		UpdateColour();
	}
}
