using SimpleJSON;
using UnityEngine;
using UnityEngine.PostProcessing;

public class DayNightManager : MonoBehaviour
{
	public static DayNightManager Instance;

	public float m_LightAdjuster;

	private Light m_SunLight;

	private float m_TimeOfDay;

	private float m_LastMoonActive;

	private float m_Speed;

	private float m_SunRiseDelay = 180f;

	private float m_DayTimeDelay = 720f;

	private float m_SunSetDelay = 180f;

	private float m_NightTimeDelay = 360f;

	private float m_CockCrowDelay = 27f;

	private static float m_LightTransitionSpeed = 0.25f;

	private float m_TotalDayDelay;

	private float m_TotalDelay;

	private PlaySound m_AmbienceDayTime;

	private PlaySound m_AmbienceNightTime;

	private PlaySound m_AmbienceWind;

	private PlaySound m_AmbienceRain;

	private float m_AmbientVolume;

	private float m_SunBrightness;

	private bool m_CockCrow;

	public bool m_ToggleSpeedTime;

	private float m_RainLightStrength;

	private float m_RainSoundStrength;

	private Material m_SkyBoxMaterial;

	private float m_LightningTimer;

	private bool m_Thunder;

	private bool m_UpdateLighting;

	private bool m_DesaturationActive;

	private float m_AmbientUpdateTimer;

	private float m_OldCameraY;

	private float ModAmbienceDayVolume = 1f;

	private float ModAmbienceNightVolume = 1f;

	private float ModAmbienceWindVolume = 1f;

	private float ModAmbienceRainVolume = 1f;

	public float m_MoonActive { get; private set; }

	private void Awake()
	{
		Instance = this;
		m_SunLight = GameObject.Find("Directional Light").GetComponent<Light>();
		m_TimeOfDay = m_SunRiseDelay + m_DayTimeDelay / 2f;
		m_TimeOfDay = m_SunRiseDelay;
		m_MoonActive = 0f;
		m_LastMoonActive = -1f;
		m_LightAdjuster = 1f;
		m_CockCrow = true;
		m_AmbienceDayTime = AudioManager.Instance.StartEventAmbient("AmbienceDayTime", base.gameObject, Remember: true);
		m_AmbienceDayTime.m_Result.ActingVariation.AdjustVolume(0f);
		m_AmbienceNightTime = AudioManager.Instance.StartEventAmbient("AmbienceNightTime", base.gameObject, Remember: true);
		m_AmbienceNightTime.m_Result.ActingVariation.AdjustVolume(0f);
		m_AmbienceWind = AudioManager.Instance.StartEventAmbient("AmbienceWind", base.gameObject, Remember: true);
		m_AmbienceWind.m_Result.ActingVariation.AdjustVolume(0f);
		m_AmbienceRain = AudioManager.Instance.StartEventAmbient("AmbienceRain", base.gameObject, Remember: true);
		m_AmbienceRain.m_Result.ActingVariation.AdjustVolume(0f);
		m_AmbientVolume = 1f;
		Material original = (Material)Resources.Load("Materials/GameSkyBox", typeof(Material));
		m_SkyBoxMaterial = Object.Instantiate(original);
		RenderSettings.skybox = m_SkyBoxMaterial;
		m_ToggleSpeedTime = false;
		m_RainLightStrength = 0f;
		m_RainSoundStrength = 0f;
		m_LightningTimer = 0f;
		m_SunRiseDelay = VariableManager.Instance.GetVariableAsFloat("SunRiseDelay") * 60f;
		m_DayTimeDelay = VariableManager.Instance.GetVariableAsFloat("DayTimeDelay") * 60f;
		m_SunSetDelay = VariableManager.Instance.GetVariableAsFloat("SunSetDelay") * 60f;
		m_NightTimeDelay = VariableManager.Instance.GetVariableAsFloat("NightTimeDelay") * 60f;
		m_TotalDayDelay = m_SunRiseDelay + m_DayTimeDelay + m_SunSetDelay;
		m_TotalDelay = m_TotalDayDelay + m_NightTimeDelay;
	}

	public void Save(JSONNode Node)
	{
		JSONUtils.Set(Node, "TimeOfDay", m_TimeOfDay * 10f);
	}

	public void Load(JSONNode Node)
	{
		m_TimeOfDay = JSONUtils.GetAsFloat(Node, "TimeOfDay", 0f) / 10f;
		UpdateSun(InstantMoon: true);
	}

	private void SetMoonActive(float Value)
	{
		if (m_MoonActive != Value)
		{
			m_MoonActive = Value;
			m_UpdateLighting = true;
		}
	}

	public void SetDesaturationActive(bool Active)
	{
		m_DesaturationActive = Active;
	}

	private void UpdateSun(bool InstantMoon = false)
	{
		float num = 0f;
		float num2 = 0f;
		Color color = new Color(1f, 1f, 1f);
		Color color2 = new Color(0.45f, 0.45f, 0.45f);
		float num3 = 40f;
		float num4 = 60f;
		Color color3 = new Color(0.36f, 0.6f, 1f);
		Color color4 = new Color(0f, 0f, 0f);
		if (m_TimeOfDay < m_SunRiseDelay)
		{
			float num5 = m_TimeOfDay / m_SunRiseDelay;
			num = num5 * 60f;
			num2 = -30f;
			m_SunBrightness = num5 + 0.2f;
			if (m_SunBrightness > 1f)
			{
				m_SunBrightness = 1f;
			}
			if (m_TimeOfDay > m_CockCrowDelay && !m_CockCrow)
			{
				m_CockCrow = true;
				AudioManager.Instance.StartEvent("AmbienceCockCrow");
			}
		}
		else if (m_TimeOfDay < m_SunRiseDelay + m_DayTimeDelay)
		{
			num = 60f;
			num2 = -30f;
			m_SunBrightness = 1f;
		}
		else if (m_TimeOfDay < m_SunRiseDelay + m_DayTimeDelay + m_SunSetDelay)
		{
			float num6 = 1f - (m_TimeOfDay - m_SunRiseDelay - m_DayTimeDelay) / m_SunSetDelay;
			num = num6 * 60f;
			num2 = -30f;
			m_SunBrightness = num6 + 0.2f;
			if (m_SunBrightness > 1f)
			{
				m_SunBrightness = 1f;
			}
		}
		else
		{
			num = 0f;
			num2 = -30f;
			m_SunBrightness = 0.2f;
		}
		if (m_SunBrightness > 0.3f)
		{
			if (InstantMoon)
			{
				m_MoonActive = 0f;
			}
			else if (m_MoonActive > 0f)
			{
				SetMoonActive(m_MoonActive - m_LightTransitionSpeed * TimeManager.Instance.m_NormalDelta);
				if (m_MoonActive < 0f)
				{
					SetMoonActive(0f);
				}
			}
		}
		else if (InstantMoon)
		{
			m_MoonActive = 1f;
		}
		else if (m_MoonActive < 1f)
		{
			if (m_CockCrow)
			{
				m_CockCrow = false;
				AudioManager.Instance.StartEvent("AmbienceOwlHoot");
			}
			SetMoonActive(m_MoonActive + m_LightTransitionSpeed * TimeManager.Instance.m_NormalDelta);
			if (m_MoonActive > 1f)
			{
				SetMoonActive(1f);
			}
		}
		float num8;
		float num9;
		float num10;
		float num7;
		if (!m_DesaturationActive)
		{
			num7 = ((m_MoonActive > 0.75f) ? ((m_MoonActive - 0.75f) / 0.25f) : ((!(m_MoonActive > 0.25f)) ? (1f - m_MoonActive / 0.25f) : 0f));
			num8 = m_RainLightStrength;
			num9 = m_SunBrightness;
			num10 = m_MoonActive;
		}
		else
		{
			num7 = 1f;
			num8 = 0f;
			num9 = 1f;
			num10 = 0f;
			if (m_MoonActive > 0f)
			{
				num = num3;
				num2 = num4;
			}
		}
		num7 = num7 * 0.5f + num7 * 0.5f * (1f - num8);
		m_SunLight.shadowStrength = num7;
		float num11 = num10 / 1f;
		if (num11 < 0f)
		{
			num11 = 0f;
		}
		if (num11 > 1f)
		{
			num11 = 1f;
		}
		m_SunLight.color = (color3 - color) * num11 + color;
		float num12 = (1f - num9) * num11 + num9;
		num12 = num12 * 0.5f + num12 * 0.5f * (1f - num8);
		m_SunLight.intensity = num12;
		Color color5 = (color4 - color2) * num11 + color2;
		Color color6 = m_SunLight.color * num12 - color5;
		color5 += color6 * 0.5f * num8;
		RenderSettings.ambientSkyColor = color5;
		num11 = (num10 - 0.25f) / 0.5f;
		if (num11 < 0f)
		{
			num11 = 0f;
		}
		if (num11 > 1f)
		{
			num11 = 1f;
		}
		Quaternion a = Quaternion.Euler(num, num2, 0f);
		Quaternion b = Quaternion.Euler(num3, num4, 0f);
		m_SunLight.transform.localRotation = Quaternion.Lerp(a, b, num11);
		m_LightAdjuster = 1f - num10;
		PostProcessingBehaviour component = CameraManager.Instance.m_Camera.GetComponent<PostProcessingBehaviour>();
		ColorGradingModel.Settings settings = component.profile.colorGrading.settings;
		settings.basic.saturation = 1f - 0.25f * num8;
		component.profile.colorGrading.settings = settings;
		component.profile.bloom.enabled = SettingsManager.Instance.m_BloomEnabled;
		UpdateAmbientSounds();
		UpdateSkyBox();
		UpdateFog();
		UpdateClouds();
		UpdateMusic();
		if (m_UpdateLighting)
		{
			m_UpdateLighting = false;
			LightManager.Instance.UpdateLightsIntensity();
		}
	}

	private void UpdateLightning()
	{
		if (!SettingsManager.Instance.m_FlashiesEnabled || SettingsManager.Instance.GetIsSnowAvailable())
		{
			return;
		}
		if (m_RainLightStrength > 0.8f)
		{
			m_LightningTimer -= TimeManager.Instance.m_NormalDelta;
			if (m_LightningTimer < 0f)
			{
				m_LightningTimer = Random.Range(5, 15);
				m_Thunder = false;
			}
			else if (m_LightningTimer < 0.08f || (m_LightningTimer > 0.12f && m_LightningTimer < 0.2f))
			{
				if (!m_Thunder)
				{
					m_Thunder = true;
					AudioManager.Instance.StartEvent("AmbienceThunder");
				}
				m_SunLight.shadowStrength = 1f;
				m_SunLight.color = new Color(1f, 1f, 1f);
				m_SunLight.intensity = 1000f;
				m_SunLight.transform.localRotation = Quaternion.Euler(30f, 130f, 0f);
				RenderSettings.ambientSkyColor = new Color(0f, 0f, 0f);
				CameraManager.Instance.m_Camera.GetComponent<PostProcessingBehaviour>().profile.bloom.enabled = false;
			}
		}
		else
		{
			m_LightningTimer = 0f;
		}
	}

	private void UpdateAmbientSounds()
	{
		GameStateManager.State actualState = GameStateManager.Instance.GetActualState();
		if (actualState == GameStateManager.State.Loading || actualState == GameStateManager.State.CreateWorld || actualState == GameStateManager.State.Playback || actualState == GameStateManager.State.PlaybackLoading)
		{
			return;
		}
		float y = CameraManager.Instance.m_Camera.transform.position.y;
		m_AmbientUpdateTimer += TimeManager.Instance.m_NormalDelta;
		if (!(m_AmbientUpdateTimer < 1f) || m_OldCameraY != y)
		{
			m_AmbientUpdateTimer = 0f;
			m_OldCameraY = y;
			float num = (y - 20f) / 50f;
			if (num < 0f)
			{
				num = 0f;
			}
			if (num > 1f)
			{
				num = 1f;
			}
			float num2 = 1f - num;
			num2 *= 1f - m_RainLightStrength;
			m_AmbienceDayTime.m_Result.ActingVariation.AdjustVolume((1f - m_MoonActive) * num2 * m_AmbientVolume * ModAmbienceDayVolume);
			m_AmbienceNightTime.m_Result.ActingVariation.AdjustVolume(m_MoonActive * num2 * m_AmbientVolume * ModAmbienceNightVolume);
			float num3 = 0f;
			if (num > 0.5f)
			{
				num3 = (num - 0.5f) / 0.5f;
			}
			num3 *= 1f - m_RainLightStrength;
			m_AmbienceWind.m_Result.ActingVariation.AdjustVolume(num3 * m_AmbientVolume * ModAmbienceWindVolume);
			m_AmbienceRain.m_Result.ActingVariation.AdjustVolume(m_RainSoundStrength * m_AmbientVolume * ModAmbienceRainVolume);
		}
	}

	public void SetAmbientVolume(float NewVolume)
	{
		m_AmbientVolume = NewVolume;
	}

	private void UpdateMusic()
	{
		switch (GameStateManager.Instance.GetActualState())
		{
		case GameStateManager.State.Loading:
		case GameStateManager.State.CreateWorld:
			return;
		case GameStateManager.State.Ceremony:
			if (CeremonyManager.Instance.m_CurrentCeremonyType == CeremonyManager.CeremonyType.GameComplete)
			{
				return;
			}
			break;
		}
		if (!GeneralUtils.m_InGame || m_MoonActive == m_LastMoonActive)
		{
			return;
		}
		m_LastMoonActive = m_MoonActive;
		float num = 1f;
		num = ((m_SunBrightness > 0.3f) ? ((!(m_MoonActive > 0.5f)) ? 1f : ((m_MoonActive - 0.5f) / 0.5f)) : ((!(m_MoonActive < 0.5f)) ? 1f : (1f - m_MoonActive / 0.5f)));
		if (num > 1f)
		{
			num = 1f;
		}
		if (num < 0f)
		{
			num = 0f;
		}
		if (AudioManager.Instance.AllowDayNightChange)
		{
			string text = "MusicGame";
			if (m_MoonActive > 0.5f)
			{
				text = "MusicGameNight";
			}
			if (AudioManager.Instance.m_MusicName != text)
			{
				AudioManager.Instance.StartMusic(text);
			}
			AudioManager.Instance.SetCurrentMusicVolume(num);
		}
	}

	private void UpdateSkyBox()
	{
		float value = 1f - (m_SunBrightness - 0.2f) / 0.8f;
		m_SkyBoxMaterial.SetFloat("_Blend", value);
	}

	private void UpdateFog()
	{
		float num = 1f - (m_SunBrightness - 0.2f) / 0.8f;
		Color color = new Color(0.768627465f, 1f, 1f);
		RenderSettings.fogColor = (new Color(0f, 0f, 0f) - color) * num + color;
	}

	private void UpdateClouds()
	{
		float num = 1f - (m_SunBrightness - 0.2f) / 0.8f;
		Color color = new Color(1f, 1f, 1f, 1f);
		Color color2 = (new Color(1f, 1f, 1f, 0.2f) - color) * num + color;
		MaterialManager.Instance.m_CloudMaterial.color = color2;
	}

	public void SetRainStrength(float LightStrength, float SoundStrength)
	{
		m_RainLightStrength = LightStrength;
		m_RainSoundStrength = SoundStrength;
		if (SettingsManager.Instance.GetIsSnowAvailable())
		{
			m_RainLightStrength *= 0.5f;
			m_RainSoundStrength = 0f;
		}
	}

	public void ToggleSpeedTime()
	{
		m_ToggleSpeedTime = !m_ToggleSpeedTime;
	}

	private void Update()
	{
		if (SettingsManager.Instance.m_DayNightEnabled)
		{
			if (TutorialPanelController.Instance == null || !TutorialPanelController.Instance.GetActive())
			{
				float speed = 1f;
				if (m_ToggleSpeedTime)
				{
					speed = 60f;
				}
				m_Speed = speed;
				m_TimeOfDay += TimeManager.Instance.m_NormalDelta * m_Speed;
				if (m_TimeOfDay > m_TotalDelay)
				{
					m_TimeOfDay = 0f;
				}
			}
		}
		else
		{
			m_TimeOfDay = m_SunRiseDelay;
		}
		UpdateSun();
		UpdateLightning();
	}

	public bool GetIsNightTime()
	{
		if (m_TimeOfDay > m_TotalDayDelay || m_TimeOfDay < m_CockCrowDelay)
		{
			return true;
		}
		return false;
	}

	public void SetShadows(bool Shadows)
	{
		if (Shadows)
		{
			m_SunLight.shadows = LightShadows.Soft;
		}
		else
		{
			m_SunLight.shadows = LightShadows.None;
		}
	}

	public void SetTime(float Time)
	{
		m_TimeOfDay = Time * m_TotalDelay;
	}

	public float GetTime()
	{
		return m_TimeOfDay / m_TotalDelay;
	}

	public void Mod_SetAmbienceDayTimeVolume(float Volume)
	{
		ModAmbienceDayVolume = Volume;
	}

	public void Mod_SetAmbienceNightTimeVolume(float Volume)
	{
		ModAmbienceNightVolume = Volume;
	}

	public void Mod_SetAmbienceRainVolume(float Volume)
	{
		ModAmbienceRainVolume = Volume;
	}

	public void Mod_SetAmbienceWindVolume(float Volume)
	{
		ModAmbienceWindVolume = Volume;
	}
}
