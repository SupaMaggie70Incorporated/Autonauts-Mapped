using System.Collections.Generic;
using UnityEngine;

public class RainManager : MonoBehaviour
{
	public static RainManager Instance;

	public bool m_IsEnabled;

	private GameObject m_RainSplash;

	private Rain m_Rain;

	private GameObject m_RainCollider;

	private float m_TimeUntilRain;

	private float m_TimeToRain;

	private bool m_Raining;

	private float m_RainStrength;

	private float m_DesiredRainStrength;

	private float m_TimeUntilRainLight;

	private float m_TimeToRainLight;

	private float m_RainLightStrength;

	private bool m_RainingLight;

	private void Awake()
	{
		Instance = this;
		m_RainSplash = ParticlesManager.Instance.CreateParticles("Rain/RainSplash", base.transform.position, Quaternion.Euler(90f, 0f, 0f)).gameObject;
		GameObject original = (GameObject)Resources.Load("Prefabs/Particles/Rain/RainCollider", typeof(GameObject));
		m_RainCollider = Object.Instantiate(original, base.transform.position, Quaternion.identity, null);
		string text = "Rain";
		if (SettingsManager.Instance.GetIsSnowAvailable())
		{
			text = "Snow";
		}
		m_Rain = ParticlesManager.Instance.CreateParticles("Rain/" + text, base.transform.position, Quaternion.Euler(90f, 0f, 0f)).GetComponent<Rain>();
		m_Rain.GetComponent<MyParticles>().m_Particles.trigger.SetCollider(0, m_RainCollider.GetComponent<BoxCollider>());
		m_Rain.SetChildParticles(m_RainSplash.GetComponent<MyParticles>());
		m_Rain.SetStrength(0f);
		UpdatePosition();
		m_RainingLight = false;
		m_Raining = false;
		SetupNextRainLight();
		m_TimeUntilRain = m_TimeUntilRainLight + 3f;
		m_IsEnabled = true;
	}

	private void OnDestroy()
	{
		if ((bool)ParticlesManager.Instance)
		{
			if ((bool)m_RainSplash)
			{
				ParticlesManager.Instance.DestroyParticles(m_RainSplash.GetComponent<MyParticles>());
			}
			if ((bool)m_Rain)
			{
				ParticlesManager.Instance.DestroyParticles(m_Rain.GetComponent<MyParticles>());
			}
		}
	}

	private void SetupNextRainLight()
	{
		if (SettingsManager.Instance.GetIsSnowAvailable())
		{
			m_TimeUntilRainLight = Random.Range(250, 450);
		}
		else
		{
			m_TimeUntilRainLight = Random.Range(1000, 1800);
		}
	}

	private void SetupRainLightDuration()
	{
		m_TimeToRainLight = Random.Range(250, 400);
		m_DesiredRainStrength = Random.Range(0.5f, 1f);
		m_RainLightStrength = 0f;
	}

	private void SetupNextRain()
	{
		m_TimeUntilRain = m_TimeUntilRainLight + 3f;
	}

	private void SetupRainDuration()
	{
		m_TimeToRain = m_TimeToRainLight;
		m_RainStrength = 0f;
	}

	private void UpdatePosition()
	{
		Vector3 position = CameraManager.Instance.m_Camera.transform.position;
		m_Rain.transform.position = position + new Vector3(0f, 15f, 0f);
		position.y = 0f;
		m_RainSplash.transform.position = position;
		m_RainCollider.transform.position = position;
	}

	public void SetEnabled(bool Enabled)
	{
		m_IsEnabled = Enabled;
	}

	public void StartRainingSoon()
	{
		m_IsEnabled = true;
		m_TimeUntilRainLight = 10f;
		m_TimeUntilRain = m_TimeUntilRainLight + 3f;
		m_RainStrength = 0f;
	}

	public void ToggleRain()
	{
		if (!m_Raining)
		{
			StartRainingNow();
			return;
		}
		m_Raining = false;
		m_RainStrength = 0f;
		m_RainingLight = false;
		m_RainLightStrength = 0f;
		SetupNextRain();
		RouteFinding.UpdateAllTiles();
	}

	public bool GetIsRaining()
	{
		if (m_Raining && m_IsEnabled)
		{
			return true;
		}
		return false;
	}

	public void StartRainingNow()
	{
		m_Raining = true;
		m_RainingLight = true;
		SetupRainLightDuration();
		SetupRainDuration();
		m_DesiredRainStrength = 1f;
		RouteFinding.UpdateAllTiles();
	}

	private void Update()
	{
		if (!SettingsManager.Instance.m_WeatherEnabled || !m_IsEnabled || (TutorialPanelController.Instance != null && TutorialPanelController.Instance.GetActive()))
		{
			m_Rain.SetStrength(0f);
			DayNightManager.Instance.SetRainStrength(0f, 0f);
			return;
		}
		UpdatePosition();
		float num = 1f;
		if (DayNightManager.Instance.m_ToggleSpeedTime)
		{
			num = 60f;
		}
		if (!m_RainingLight)
		{
			m_TimeUntilRainLight -= TimeManager.Instance.m_NormalDelta * num;
			if (m_TimeUntilRainLight <= 0f)
			{
				m_RainingLight = true;
				SetupRainLightDuration();
			}
		}
		else
		{
			m_TimeToRainLight -= TimeManager.Instance.m_NormalDelta * num;
			if (m_TimeToRainLight <= 0f)
			{
				m_RainingLight = false;
				SetupNextRainLight();
			}
		}
		if (m_RainingLight)
		{
			if (m_RainLightStrength < m_DesiredRainStrength)
			{
				m_RainLightStrength += 0.2f * TimeManager.Instance.m_NormalDelta;
				if (m_RainLightStrength >= m_DesiredRainStrength)
				{
					m_RainLightStrength = m_DesiredRainStrength;
				}
			}
		}
		else if (m_RainLightStrength > 0f)
		{
			m_RainLightStrength -= 0.2f * TimeManager.Instance.m_NormalDelta;
			if (m_RainLightStrength <= 0f)
			{
				m_RainLightStrength = 0f;
			}
		}
		if (!m_Raining)
		{
			m_TimeUntilRain -= TimeManager.Instance.m_NormalDelta * num;
			if (m_TimeUntilRain <= 0f)
			{
				m_Raining = true;
				SetupRainDuration();
				RouteFinding.UpdateAllTiles();
			}
		}
		else
		{
			m_TimeToRain -= TimeManager.Instance.m_NormalDelta * num;
			if (m_TimeToRain <= 0f)
			{
				m_Raining = false;
				SetupNextRain();
				RouteFinding.UpdateAllTiles();
			}
		}
		if (m_Raining)
		{
			if (m_RainStrength < m_DesiredRainStrength)
			{
				m_RainStrength += 0.2f * TimeManager.Instance.m_NormalDelta;
				if (m_RainStrength >= m_DesiredRainStrength)
				{
					m_RainStrength = m_DesiredRainStrength;
					Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("Folk");
					if (collection != null && collection.Count > 0)
					{
						QuestManager.Instance.AddEvent(QuestEvent.Type.RainOnFolk, Bot: false, 0, null);
					}
				}
			}
		}
		else if (m_RainStrength > 0f)
		{
			m_RainStrength -= 0.2f * TimeManager.Instance.m_NormalDelta;
			if (m_RainStrength <= 0f)
			{
				m_RainStrength = 0f;
			}
		}
		m_Rain.SetStrength(m_RainStrength);
		DayNightManager.Instance.SetRainStrength(m_RainLightStrength, m_RainStrength);
	}
}
