using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
	private Slider m_SFXSlider;

	private float m_OldSFXVolume;

	private Slider m_MusicSlider;

	private float m_OldMusicVolume;

	private Toggle m_AmbientOcclusionToggle;

	private bool m_OldAmbientOcclusionToggleOn;

	private Toggle m_LightsToggle;

	private bool m_OldLightsToggleOn;

	private Toggle m_RolloversToggle;

	private bool m_OldRolloversToggleOn;

	private void Awake()
	{
		if ((bool)CameraManager.Instance)
		{
			CameraManager.Instance.EnableVignette(Enabled: false);
		}
		m_SFXSlider = base.transform.Find("Panel").Find("SFXSlider").GetComponent<Slider>();
		m_SFXSlider.value = SettingsManager.Instance.m_SFXVolume;
		m_OldSFXVolume = SettingsManager.Instance.m_SFXVolume;
		m_MusicSlider = base.transform.Find("Panel").Find("MusicSlider").GetComponent<Slider>();
		m_MusicSlider.value = SettingsManager.Instance.m_MusicVolume;
		m_OldMusicVolume = SettingsManager.Instance.m_MusicVolume;
		m_AmbientOcclusionToggle = base.transform.Find("Panel").Find("AmbientOcclusionToggle").GetComponent<Toggle>();
		m_AmbientOcclusionToggle.isOn = SettingsManager.Instance.m_AmbientOcclusionEnabled;
		m_OldAmbientOcclusionToggleOn = m_AmbientOcclusionToggle.isOn;
		m_LightsToggle = base.transform.Find("Panel").Find("LightsToggle").GetComponent<Toggle>();
		m_LightsToggle.isOn = SettingsManager.Instance.m_LightsEnabled;
		m_OldLightsToggleOn = m_LightsToggle.isOn;
	}

	private void OnDestroy()
	{
	}

	public void Restore()
	{
		AudioManager.Instance.SetSFXVolume(m_OldSFXVolume);
		AudioManager.Instance.SetMusicVolume(m_OldMusicVolume);
		SettingsManager.Instance.m_LightsEnabled = m_OldLightsToggleOn;
		SettingsManager.Instance.UpdateLights();
	}

	public void OnBackButton()
	{
		Restore();
		AudioManager.Instance.StartEvent("UIOptionCancelled");
		GameStateManager.Instance.PopState();
	}

	public void OnSFXSliderChanged()
	{
		AudioManager.Instance.SetSFXVolume(m_SFXSlider.value);
		AudioManager.Instance.StartEvent("UIVolumeChangedTest");
	}

	public void OnMusicSliderChanged()
	{
		AudioManager.Instance.SetMusicVolume(m_MusicSlider.value);
	}

	public void OnWavesChanged()
	{
		if ((bool)TileMapAnimationManager.Instance)
		{
			TileMapAnimationManager.Instance.ResetWaves();
		}
	}

	public void OnLightsChanged()
	{
		SettingsManager.Instance.m_LightsEnabled = m_LightsToggle.isOn;
		SettingsManager.Instance.UpdateLights();
	}

	public void OnApply()
	{
		SettingsManager.Instance.m_SFXVolume = m_SFXSlider.value;
		SettingsManager.Instance.m_MusicVolume = m_MusicSlider.value;
		SettingsManager.Instance.m_AmbientOcclusionEnabled = m_AmbientOcclusionToggle.isOn;
		SettingsManager.Instance.m_LightsEnabled = m_LightsToggle.isOn;
		SettingsManager.Instance.Save();
		AudioManager.Instance.SetSFXVolume(m_SFXSlider.value);
		AudioManager.Instance.SetMusicVolume(m_MusicSlider.value);
		AudioManager.Instance.StartEvent("UIOptionSelected");
		GameStateManager.Instance.PopState();
	}

	public void OnMouseEnter()
	{
		AudioManager.Instance.StartEvent("UIOptionIndicated");
	}
}
