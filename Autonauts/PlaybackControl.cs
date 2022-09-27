using UnityEngine;

public class PlaybackControl : MonoBehaviour
{
	public static PlaybackControl Instance;

	public PlaybackSlider m_Slider;

	private BaseText m_TimeText;

	private BaseButtonImage m_PlayButton;

	private void Awake()
	{
		Instance = this;
		m_PlayButton = base.transform.Find("PlayButton").GetComponent<BaseButtonImage>();
		m_PlayButton.SetAction(OnPlayClicked, m_PlayButton);
		m_Slider = base.transform.Find("PlaybackSlider").GetComponent<PlaybackSlider>();
		m_Slider.SetAction(OnSliderChanged, m_Slider);
		m_Slider.SetValue(0f);
		m_TimeText = base.transform.Find("Time").GetComponent<BaseText>();
		SetPlaying(Playing: false);
	}

	public void SetPlaying(bool Playing)
	{
		if (Playing)
		{
			m_PlayButton.SetSprite("Script/ScriptPause");
		}
		else
		{
			m_PlayButton.SetSprite("Script/ScriptGo");
		}
	}

	public void OnPlayClicked(BaseGadget NewGadget)
	{
		PlaybackManager.Instance.TogglePlay();
	}

	public void OnSliderChanged(BaseGadget NewGadget)
	{
	}

	private void UpdateTimeString()
	{
		if ((bool)PlaybackManager.Instance && (bool)RecordingManager.Instance)
		{
			string text = GeneralUtils.ConvertTimeToString(PlaybackManager.Instance.m_Time);
			string text2 = GeneralUtils.ConvertTimeToString(RecordingManager.Instance.m_TotalTime);
			string text3 = text + "/" + text2;
			m_TimeText.SetText(text3);
		}
	}

	private void Update()
	{
		UpdateTimeString();
	}
}
