using UnityEngine;

public class Loading : MonoBehaviour
{
	private StandardProgressBar m_Slider;

	public void SetNew()
	{
		base.transform.Find("Sun/Title").GetComponent<BaseText>().SetTextFromID("NewGame");
		base.transform.Find("Sun/StandardProgressBar").GetComponent<StandardProgressBar>().SetActive(Active: false);
		SetTip();
	}

	public void SetLoading()
	{
		m_Slider = base.transform.Find("Sun/StandardProgressBar").GetComponent<StandardProgressBar>();
		SetTip();
	}

	private void SetTip()
	{
		int autosaveFrequency = (int)SettingsManager.Instance.m_AutosaveFrequency;
		float num = SettingsManager.m_AutosaveFrequencies[autosaveFrequency];
		BaseText component = base.transform.Find("TutorialPanel/SpeechBubble/Dialog").GetComponent<BaseText>();
		if (num == 0f)
		{
			component.SetTextFromID("AutosaveDisabledWarning");
			return;
		}
		string text = TextManager.Instance.Get("AutosaveWarning", ((int)num).ToString());
		component.SetText(text);
	}

	public void SetValue(float Value)
	{
		m_Slider.SetValue(Value);
	}
}
