using System.Collections.Generic;
using Rewired;
using TMPro;
using UnityEngine;
using UnityEngine.PostProcessing;

public class SettingsManager : MonoBehaviour
{
	public enum Language
	{
		English,
		German,
		French,
		Russian,
		ChineseSimplified,
		JapaneseKana,
		BrazilianPortugeuse,
		Spanish,
		Polish,
		Korean,
		Turkish,
		Total
	}

	public enum AutosaveFrequency
	{
		None,
		Ten,
		Thirty,
		Hour,
		Total
	}

	public static SettingsManager Instance;

	public Language m_Language;

	public AutosaveFrequency m_AutosaveFrequency;

	public static float[] m_AutosaveFrequencies = new float[4] { 0f, 10f, 30f, 60f };

	public float m_SFXVolume;

	public float m_MusicVolume;

	public bool m_TutorialEnabled;

	public bool m_FlashiesEnabled;

	public bool m_WeatherEnabled;

	public bool m_DayNightEnabled;

	public int m_Quality;

	public bool m_AmbientOcclusionEnabled;

	public bool m_LightsEnabled;

	public bool m_ShadowsEnabled;

	public bool m_BloomEnabled;

	public bool m_AntialiasingEnabled;

	public bool m_DOFEnabled;

	public bool m_AutoDOFEnabled;

	public float m_DOFFocalDistance;

	public float m_DOFFocalLength;

	public float m_DOFAperture;

	public bool m_HotbarEnabled;

	public TMP_FontAsset m_MainFont;

	public string m_LastSave;

	public bool m_DevMode;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
		}
		else if (this != Instance)
		{
			Object.DestroyImmediate(base.gameObject);
			return;
		}
		m_Language = Language.Total;
		m_SFXVolume = 0.9f;
		m_MusicVolume = 0.1f;
		m_TutorialEnabled = true;
		m_FlashiesEnabled = true;
		m_WeatherEnabled = true;
		m_DayNightEnabled = true;
		if (Screen.width < 640 || Screen.height < 480)
		{
			SetScreen(640, 480, FullScreen: false, SetResolution: true);
		}
		UpdateScreen();
		Debug.Log("CurrentScreen " + Screen.width + " " + Screen.height + " " + Screen.fullScreen.ToString());
		m_Quality = QualitySettings.names.Length - 1;
		m_AmbientOcclusionEnabled = false;
		m_LightsEnabled = true;
		m_ShadowsEnabled = true;
		m_BloomEnabled = true;
		m_AntialiasingEnabled = true;
		m_DOFEnabled = false;
		m_AutoDOFEnabled = true;
		m_DOFFocalDistance = 0.1f;
		m_DOFFocalLength = 0.67f;
		m_DOFAperture = 0.1f;
		m_HotbarEnabled = true;
		m_AutosaveFrequency = AutosaveFrequency.Ten;
		m_LastSave = "";
		m_DevMode = false;
		Load();
	}

	private string GetActionName(ActionElementMap aem)
	{
		InputAction action = ReInput.mapping.GetAction(aem.actionId);
		return "Action" + action.name;
	}

	public void Save()
	{
		PlayerPrefs.SetInt("Language", (int)m_Language);
		PlayerPrefs.SetFloat("SFXVolume", m_SFXVolume);
		PlayerPrefs.SetFloat("MusicVolume", m_MusicVolume);
		if (m_TutorialEnabled)
		{
			PlayerPrefs.SetInt("TutorialEnabled", 1);
		}
		else
		{
			PlayerPrefs.SetInt("TutorialEnabled", 0);
		}
		if (m_FlashiesEnabled)
		{
			PlayerPrefs.SetInt("FlashiesEnabled", 1);
		}
		else
		{
			PlayerPrefs.SetInt("FlashiesEnabled", 0);
		}
		if (m_WeatherEnabled)
		{
			PlayerPrefs.SetInt("WeatherEnabled", 1);
		}
		else
		{
			PlayerPrefs.SetInt("WeatherEnabled", 0);
		}
		if (m_DayNightEnabled)
		{
			PlayerPrefs.SetInt("DayNightEnabled", 1);
		}
		else
		{
			PlayerPrefs.SetInt("DayNightEnabled", 0);
		}
		PlayerPrefs.SetInt("Quality", m_Quality);
		if (m_AmbientOcclusionEnabled)
		{
			PlayerPrefs.SetInt("AmbientOcclusionEnabled", 1);
		}
		else
		{
			PlayerPrefs.SetInt("AmbientOcclusionEnabled", 0);
		}
		if (m_LightsEnabled)
		{
			PlayerPrefs.SetInt("LightsEnabled", 1);
		}
		else
		{
			PlayerPrefs.SetInt("LightsEnabled", 0);
		}
		if (m_ShadowsEnabled)
		{
			PlayerPrefs.SetInt("ShadowsEnabled", 1);
		}
		else
		{
			PlayerPrefs.SetInt("ShadowsEnabled", 0);
		}
		if (m_BloomEnabled)
		{
			PlayerPrefs.SetInt("BloomEnabled", 1);
		}
		else
		{
			PlayerPrefs.SetInt("BloomEnabled", 0);
		}
		if (m_AntialiasingEnabled)
		{
			PlayerPrefs.SetInt("AntialiasingEnabled", 1);
		}
		else
		{
			PlayerPrefs.SetInt("AntialiasingEnabled", 0);
		}
		if (m_HotbarEnabled)
		{
			PlayerPrefs.SetInt("HotbarEnabled", 1);
		}
		else
		{
			PlayerPrefs.SetInt("HotbarEnabled", 0);
		}
		if (m_DOFEnabled)
		{
			PlayerPrefs.SetInt("DOFEnabled", 1);
		}
		else
		{
			PlayerPrefs.SetInt("DOFEnabled", 0);
		}
		if (m_AutoDOFEnabled)
		{
			PlayerPrefs.SetInt("AutoDOFEnabled", 1);
		}
		else
		{
			PlayerPrefs.SetInt("AutoDOFEnabled", 0);
		}
		PlayerPrefs.SetFloat("DOFFocalDistance", m_DOFFocalDistance);
		PlayerPrefs.SetFloat("DOFFocalLength", m_DOFFocalLength);
		PlayerPrefs.SetFloat("DOFAperture", m_DOFAperture);
		PlayerPrefs.SetInt("AutosaveFrequency", (int)m_AutosaveFrequency);
		PlayerPrefs.SetString("LastSave", m_LastSave);
		if (m_DevMode)
		{
			PlayerPrefs.SetInt("DevModeEnabled", 1);
		}
		else
		{
			PlayerPrefs.SetInt("DevModeEnabled", 0);
		}
		foreach (KeyValuePair<ActionElementMap, InputBinding> controlsAction in MyInputManager.Instance.m_ControlsActions)
		{
			ActionElementMap key = controlsAction.Key;
			string actionName = GetActionName(key);
			string key2 = actionName + "Mod1";
			InputBinding value = controlsAction.Value;
			if (key.keyCode != value.m_Code || key.modifierKey1 != value.m_ModifierKey)
			{
				PlayerPrefs.SetInt(actionName, (int)key.keyCode);
				if (key.modifierKey1 != 0)
				{
					PlayerPrefs.SetInt(key2, (int)key.modifierKey1);
				}
				else if (PlayerPrefs.HasKey(key2))
				{
					PlayerPrefs.DeleteKey(key2);
				}
			}
			else if (PlayerPrefs.HasKey(actionName))
			{
				PlayerPrefs.DeleteKey(actionName);
			}
		}
	}

	private void Load()
	{
		if (PlayerPrefs.HasKey("Language"))
		{
			m_Language = (Language)PlayerPrefs.GetInt("Language");
		}
		if (PlayerPrefs.HasKey("SFXVolume"))
		{
			m_SFXVolume = PlayerPrefs.GetFloat("SFXVolume");
		}
		if (PlayerPrefs.HasKey("MusicVolume"))
		{
			m_MusicVolume = PlayerPrefs.GetFloat("MusicVolume");
		}
		if (PlayerPrefs.HasKey("TutorialEnabled"))
		{
			m_TutorialEnabled = PlayerPrefs.GetInt("TutorialEnabled") == 1;
		}
		if (PlayerPrefs.HasKey("FlashiesEnabled"))
		{
			m_FlashiesEnabled = PlayerPrefs.GetInt("FlashiesEnabled") == 1;
		}
		if (PlayerPrefs.HasKey("WeatherEnabled"))
		{
			m_WeatherEnabled = PlayerPrefs.GetInt("WeatherEnabled") == 1;
		}
		if (PlayerPrefs.HasKey("DayNightEnabled"))
		{
			m_DayNightEnabled = PlayerPrefs.GetInt("DayNightEnabled") == 1;
		}
		if (PlayerPrefs.HasKey("Quality"))
		{
			m_Quality = PlayerPrefs.GetInt("Quality");
		}
		if (PlayerPrefs.HasKey("AmbientOcclusionEnabled"))
		{
			m_AmbientOcclusionEnabled = PlayerPrefs.GetInt("AmbientOcclusionEnabled") == 1;
		}
		if (PlayerPrefs.HasKey("LightsEnabled"))
		{
			m_LightsEnabled = PlayerPrefs.GetInt("LightsEnabled") == 1;
		}
		if (PlayerPrefs.HasKey("ShadowsEnabled"))
		{
			m_ShadowsEnabled = PlayerPrefs.GetInt("ShadowsEnabled") == 1;
		}
		if (PlayerPrefs.HasKey("BloomEnabled"))
		{
			m_BloomEnabled = PlayerPrefs.GetInt("BloomEnabled") == 1;
		}
		if (PlayerPrefs.HasKey("AntialiasingEnabled"))
		{
			m_AntialiasingEnabled = PlayerPrefs.GetInt("AntialiasingEnabled") == 1;
		}
		if (PlayerPrefs.HasKey("HotbarEnabled"))
		{
			m_HotbarEnabled = PlayerPrefs.GetInt("HotbarEnabled") == 1;
		}
		if (PlayerPrefs.HasKey("DOFEnabled"))
		{
			m_DOFEnabled = PlayerPrefs.GetInt("DOFEnabled") == 1;
		}
		if (PlayerPrefs.HasKey("AutoDOFEnabled"))
		{
			m_AutoDOFEnabled = PlayerPrefs.GetInt("AutoDOFEnabled") == 1;
		}
		if (PlayerPrefs.HasKey("DOFFocalDistance"))
		{
			m_DOFFocalDistance = PlayerPrefs.GetFloat("DOFFocalDistance");
		}
		if (PlayerPrefs.HasKey("DOFFocalLength"))
		{
			m_DOFFocalLength = PlayerPrefs.GetFloat("DOFFocalLength");
		}
		if (PlayerPrefs.HasKey("DOFAperture"))
		{
			m_DOFAperture = PlayerPrefs.GetFloat("DOFAperture");
		}
		if (PlayerPrefs.HasKey("AutosaveFrequency"))
		{
			m_AutosaveFrequency = (AutosaveFrequency)PlayerPrefs.GetInt("AutosaveFrequency");
		}
		m_LastSave = PlayerPrefs.GetString("LastSave");
		if (PlayerPrefs.HasKey("DevModeEnabled"))
		{
			m_DevMode = PlayerPrefs.GetInt("DevModeEnabled") == 1;
		}
		if (!MyInputManager.Instance)
		{
			return;
		}
		foreach (KeyValuePair<ActionElementMap, InputBinding> controlsAction in MyInputManager.Instance.m_ControlsActions)
		{
			ActionElementMap key = controlsAction.Key;
			string actionName = GetActionName(key);
			string key2 = actionName + "Mod1";
			if (PlayerPrefs.HasKey(actionName))
			{
				ModifierKey mod = ModifierKey.None;
				if (PlayerPrefs.HasKey(key2))
				{
					mod = (ModifierKey)PlayerPrefs.GetInt(key2);
				}
				SetKeyCode(key, (KeyCode)PlayerPrefs.GetInt(actionName), mod);
			}
		}
	}

	public void ApplySound()
	{
		SetSFXVolume(m_SFXVolume);
		SetMusicVolume(m_MusicVolume);
	}

	public void Apply()
	{
		SetLanguage(m_Language);
		SetSFXVolume(m_SFXVolume);
		SetMusicVolume(m_MusicVolume);
		SetTutorialEnabled(m_TutorialEnabled);
		SetFlashiesEnabled(m_FlashiesEnabled);
		SetWeatherEnabled(m_WeatherEnabled);
		SetDayNightEnabled(m_DayNightEnabled);
		SetQuality(m_Quality);
		SetAmbientOcclusion(m_AmbientOcclusionEnabled);
		SetLights(m_LightsEnabled);
		SetShadows(m_ShadowsEnabled);
		SetBloom(m_BloomEnabled);
		SetAntialiasing(m_AntialiasingEnabled);
		SetAutosaveFrequency(m_AutosaveFrequency);
		if (m_HotbarEnabled && (bool)HudManager.Instance)
		{
			HudManager.Instance.SetHotBarEnabled(Enabled: true);
		}
		SetDOF(On: false);
	}

	public void UpdateLights()
	{
		if ((bool)LightManager.Instance)
		{
			LightManager.Instance.UpdateLightsEnabled();
		}
	}

	public void SetLanguage(Language NewLanguage)
	{
		m_Language = NewLanguage;
		TMP_FontAsset original = (TMP_FontAsset)Resources.Load("Fonts/MainFont SDF", typeof(TMP_FontAsset));
		m_MainFont = Object.Instantiate(original);
		if (m_Language == Language.ChineseSimplified)
		{
			List<TMP_FontAsset> fallbackFontAssetTable = m_MainFont.fallbackFontAssetTable;
			TMP_FontAsset value = fallbackFontAssetTable[2];
			fallbackFontAssetTable[2] = fallbackFontAssetTable[1];
			fallbackFontAssetTable[1] = value;
			m_MainFont.fallbackFontAssetTable = fallbackFontAssetTable;
		}
	}

	public void SetSFXVolume(float Volume)
	{
		m_SFXVolume = Volume;
		AudioManager.Instance.SetSFXVolume(Volume);
	}

	public void SetMusicVolume(float Volume)
	{
		m_MusicVolume = Volume;
		AudioManager.Instance.SetMusicVolume(Volume);
	}

	public void SetTutorialEnabled(bool Enabled)
	{
		m_TutorialEnabled = Enabled;
	}

	public void SetFlashiesEnabled(bool Enabled)
	{
		m_FlashiesEnabled = Enabled;
	}

	public void SetWeatherEnabled(bool Enabled)
	{
		m_WeatherEnabled = Enabled;
	}

	public void SetDayNightEnabled(bool Enabled)
	{
		m_DayNightEnabled = Enabled;
	}

	public int GetScreenWidth()
	{
		return Screen.width;
	}

	public int GetScreenHeight()
	{
		return Screen.height;
	}

	public bool GetFullScreen()
	{
		return Screen.fullScreen;
	}

	public Resolution[] GetPossibleResolutions()
	{
		List<Resolution> list = new List<Resolution>();
		Resolution[] resolutions = Screen.resolutions;
		for (int i = 0; i < resolutions.Length; i++)
		{
			Resolution resolution = resolutions[i];
			if (resolution.height < 720)
			{
				continue;
			}
			bool flag = true;
			for (int j = 0; j < list.Count; j++)
			{
				Resolution resolution2 = list[j];
				if (resolution2.width == resolution.width && resolution2.height == resolution.height)
				{
					if (resolution.refreshRate == 60)
					{
						list[j] = resolution;
					}
					flag = false;
					break;
				}
			}
			if (flag)
			{
				list.Add(resolution);
			}
		}
		return list.ToArray();
	}

	public int GetScreenResolutionIndex(int DesiredWidth, int DesiredHeight)
	{
		int result = 0;
		int num = 0;
		Resolution[] possibleResolutions = GetPossibleResolutions();
		for (int i = 0; i < possibleResolutions.Length; i++)
		{
			Resolution resolution = possibleResolutions[i];
			if (resolution.width == DesiredWidth && resolution.height == DesiredHeight)
			{
				result = num;
			}
			num++;
		}
		return result;
	}

	public void GetScreenResolution(int Index, out int DesiredWidth, out int DesiredHeight)
	{
		Resolution[] possibleResolutions = GetPossibleResolutions();
		if (Index < possibleResolutions.Length)
		{
			DesiredWidth = possibleResolutions[Index].width;
			DesiredHeight = possibleResolutions[Index].height;
		}
		else
		{
			DesiredWidth = 640;
			DesiredHeight = 480;
		}
	}

	public void UpdateScreen()
	{
		GameObject.Find("Canvas").GetComponent<CanvasScaleMatch>().UpdateMatch();
	}

	public void SetScreen(int ScreenWidth, int ScreenHeight, bool FullScreen, bool SetResolution)
	{
		Debug.Log("SetScreen " + ScreenWidth + " " + ScreenHeight + " " + FullScreen.ToString() + " " + SetResolution.ToString());
		if ((Screen.width != ScreenWidth || Screen.height != ScreenHeight || Screen.fullScreen != FullScreen) && SetResolution)
		{
			Screen.SetResolution(ScreenWidth, ScreenHeight, FullScreen);
			UpdateScreen();
		}
	}

	public void SetQuality(int Quality)
	{
		if (Quality >= QualitySettings.names.Length)
		{
			Quality = QualitySettings.names.Length - 1;
		}
		m_Quality = Quality;
		QualitySettings.SetQualityLevel(Quality);
	}

	public void SetAmbientOcclusion(bool On)
	{
		m_AmbientOcclusionEnabled = On;
		if ((bool)CameraManager.Instance)
		{
			CameraManager.Instance.m_Camera.GetComponent<PostProcessingBehaviour>().profile.ambientOcclusion.enabled = On;
		}
	}

	public void SetLights(bool On)
	{
		m_LightsEnabled = On;
		UpdateLights();
	}

	public void SetShadows(bool On)
	{
		m_ShadowsEnabled = On;
		if ((bool)DayNightManager.Instance)
		{
			DayNightManager.Instance.SetShadows(On);
		}
	}

	public void SetBloom(bool On)
	{
		m_BloomEnabled = On;
		if ((bool)CameraManager.Instance)
		{
			CameraManager.Instance.m_Camera.GetComponent<PostProcessingBehaviour>().profile.bloom.enabled = On;
		}
	}

	public void SetAntialiasing(bool On)
	{
		m_AntialiasingEnabled = On;
		if ((bool)CameraManager.Instance)
		{
			CameraManager.Instance.m_Camera.GetComponent<PostProcessingBehaviour>().profile.antialiasing.enabled = On;
		}
	}

	public void SetDOF(bool On)
	{
		m_DOFEnabled = On;
		if ((bool)CameraManager.Instance)
		{
			CameraManager.Instance.SetDOFEnabled(m_DOFEnabled);
		}
	}

	public void SetAutoDOF(bool On)
	{
		m_AutoDOFEnabled = On;
		if ((bool)CameraManager.Instance)
		{
			CameraManager.Instance.SetAutoDOFEnabled(m_AutoDOFEnabled);
		}
	}

	public void UpdateDOF()
	{
		if ((bool)CameraManager.Instance)
		{
			CameraManager.Instance.SetDOFEnabled(m_DOFEnabled);
			CameraManager.Instance.SetAutoDOFEnabled(m_AutoDOFEnabled);
			CameraManager.Instance.SetDOF(m_DOFFocalDistance * 150f, m_DOFFocalLength * 300f, m_DOFAperture * 16f);
		}
	}

	public void SetDOFFocalDistance(float Value)
	{
		m_DOFFocalDistance = Value;
	}

	public void SetDOFFocalLength(float Value)
	{
		m_DOFFocalLength = Value;
	}

	public void SetDOFAperture(float Value)
	{
		m_DOFAperture = Value;
	}

	public void SetAutosaveFrequency(AutosaveFrequency NewAutosaveFrequency)
	{
		m_AutosaveFrequency = NewAutosaveFrequency;
	}

	public void RemoveKeyCode(KeyCode NewCode, ModifierKey NewMod1)
	{
		foreach (KeyValuePair<ActionElementMap, InputBinding> controlsAction in MyInputManager.Instance.m_ControlsActions)
		{
			ActionElementMap key = controlsAction.Key;
			if (key.keyCode == NewCode && key.modifierKey1 == NewMod1)
			{
				key.keyCode = KeyCode.None;
				key.modifierKey1 = ModifierKey.None;
			}
		}
	}

	public void SetKeyCode(ActionElementMap aem, KeyCode NewCode, ModifierKey Mod1)
	{
		aem.keyCode = NewCode;
		aem.modifierKey1 = Mod1;
	}

	public void DefaultKeyCodes()
	{
		foreach (KeyValuePair<ActionElementMap, InputBinding> controlsAction in MyInputManager.Instance.m_ControlsActions)
		{
			ActionElementMap key = controlsAction.Key;
			SetKeyCode(key, controlsAction.Value.m_Code, controlsAction.Value.m_ModifierKey);
		}
	}

	public bool CheckKeyCodesValid()
	{
		foreach (KeyValuePair<ActionElementMap, InputBinding> controlsAction in MyInputManager.Instance.m_ControlsActions)
		{
			if (controlsAction.Key.keyCode == KeyCode.None)
			{
				return false;
			}
		}
		return true;
	}

	public bool GetIsSnowAvailable()
	{
		return false;
	}

	private void Update()
	{
		if (m_AutoDOFEnabled && (bool)CameraManager.Instance)
		{
			m_DOFFocalDistance = CameraManager.Instance.GetDOFFocalDistance() / 150f;
		}
	}
}
