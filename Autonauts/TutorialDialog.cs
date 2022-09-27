using UnityEngine;

public class TutorialDialog : BaseText
{
	private static float m_CharsPerSecond = 100f;

	private bool m_Talking;

	private float m_TalkingTimer;

	private string m_CurrentSpeech;

	private PlaySound m_TalkingSound;

	private float m_SoundTimer;

	protected new void OnDestroy()
	{
		base.OnDestroy();
		StopTalking();
	}

	private void OnDisable()
	{
		StopTalking();
	}

	public void StartSpeech(string Description, bool Ceremony)
	{
		StopTalking();
		m_Talking = true;
		m_TalkingTimer = 0f;
		m_CurrentSpeech = Description;
		SetText("");
		m_SoundTimer = 0f;
		m_TalkingSound = AudioManager.Instance.StartEvent("TutorTalk", null, Remember: true);
		if (Ceremony)
		{
			AudioManager.Instance.SetEventVolume(m_TalkingSound, 1f);
		}
		else
		{
			AudioManager.Instance.SetEventVolume(m_TalkingSound, 0.5f);
		}
		if ((bool)TutorBot.Instance)
		{
			TutorBot.Instance.StartTalking();
		}
	}

	public void StartSpeechFromID(string DescriptionID, bool Ceremony)
	{
		StartSpeech(TextManager.Instance.Get(DescriptionID), Ceremony);
	}

	public void StopTalking(bool Ended = false)
	{
		if (m_Talking)
		{
			SetText(m_CurrentSpeech);
		}
		m_Talking = false;
		if (m_TalkingSound != null)
		{
			AudioManager.Instance.SetEventVolume(m_TalkingSound, 0f);
			AudioManager.Instance.StopEvent(m_TalkingSound);
			m_TalkingSound = null;
		}
		if ((bool)TutorBot.Instance)
		{
			TutorBot.Instance.StopTalking(Ended);
		}
	}

	public bool GetIsTalking()
	{
		return m_Talking;
	}

	private void Update()
	{
		if (m_Talking)
		{
			if (TimeManager.Instance.m_PauseTimeEnabled)
			{
				m_TalkingTimer += TimeManager.Instance.m_PauseDelta;
				m_SoundTimer += TimeManager.Instance.m_PauseDelta;
			}
			else
			{
				m_TalkingTimer += TimeManager.Instance.m_NormalDelta;
				m_SoundTimer += TimeManager.Instance.m_NormalDelta;
			}
			if ((double)m_SoundTimer > 0.15)
			{
				float pitch = Random.Range(0.8f, 1.2f);
				AudioManager.Instance.GlideEventPitch(m_TalkingSound, pitch, 0.01f);
				m_SoundTimer = 0f;
			}
			int num = (int)(m_TalkingTimer * m_CharsPerSecond);
			if (num >= m_CurrentSpeech.Length)
			{
				num = m_CurrentSpeech.Length;
				StopTalking(Ended: true);
			}
			SetText(m_CurrentSpeech.Substring(0, num));
		}
	}
}
