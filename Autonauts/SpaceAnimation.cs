using UnityEngine;
using UnityEngine.UI;

public class SpaceAnimation : MonoBehaviour
{
	public static SpaceAnimation Instance;

	private Animator m_SpaceAnimator;

	private bool m_Playing;

	private bool m_PlayingStart;

	private bool m_FadingOut;

	private float m_FadeTimer;

	private Image m_FadeImage;

	public GameObject m_Rocket;

	private PlaySound m_FlameSound;

	private PlaySound m_StartSound;

	private float m_CrossVolume;

	private bool m_Duck;

	private void Awake()
	{
		Instance = this;
		GameObject gameObject = GameObject.Find("Space");
		m_SpaceAnimator = gameObject.GetComponent<Animator>();
		m_SpaceAnimator.Play("RocketShake");
		m_SpaceAnimator.Play("RocketFlame");
		m_FadeImage = GameObject.Find("Fader").GetComponent<Image>();
	}

	private void Start()
	{
		m_Rocket = base.transform.Find("Rocket/Rocket").gameObject;
		m_Rocket.transform.Find("Aaron").gameObject.SetActive(value: false);
		m_Rocket.transform.Find("Gary").gameObject.SetActive(value: false);
		m_FlameSound = AudioManager.Instance.StartEventAmbient("Rocket", m_Rocket, Remember: true, Follow: true);
		SetCrossFadeValue(0f);
	}

	private void OnDestroy()
	{
		if ((bool)AudioManager.Instance)
		{
			AudioManager.Instance.StopEvent(m_FlameSound);
		}
		Instance = null;
	}

	public void UseAudioListener(bool Use)
	{
		CameraManager.Instance.m_Camera.GetComponent<AudioListener>().enabled = !Use;
		base.transform.Find("SpaceCamera").GetComponent<AudioListener>().enabled = Use;
	}

	public bool GetStartFinished()
	{
		return !m_Playing;
	}

	public void Finished()
	{
		if (m_PlayingStart)
		{
			m_SpaceAnimator.Play("RocketIdle", -1, 0f);
		}
		m_Playing = false;
	}

	public void StartRocket()
	{
		m_SpaceAnimator.Play("RocketStart", -1, 0f);
		m_Playing = true;
		m_PlayingStart = true;
		m_StartSound = AudioManager.Instance.StartEventAmbient("RocketStart", m_Rocket, Remember: true, Follow: true);
	}

	public void RocketGo()
	{
		m_SpaceAnimator.Play("RocketGo", -1, 0f);
		m_Playing = true;
		m_PlayingStart = false;
		m_StartSound = AudioManager.Instance.StartEventAmbient("RocketLaunch", m_Rocket, Remember: true, Follow: true);
	}

	public void FadeOut()
	{
		m_FadingOut = true;
	}

	public void SetFadeValue(float Value)
	{
		m_FadeImage.color = new Color(1f, 1f, 1f, Value);
	}

	private void UpdateRocketVolume()
	{
		float num = m_CrossVolume;
		if (m_Duck)
		{
			num *= 0.25f;
		}
		AudioManager.Instance.SetEventVolume(m_FlameSound, num * 0.4f);
		if (m_StartSound != null)
		{
			AudioManager.Instance.SetEventVolume(m_StartSound, num);
		}
	}

	public void DuckAudio(bool Duck)
	{
		m_Duck = Duck;
		UpdateRocketVolume();
	}

	public void SetCrossFadeValue(float Value)
	{
		m_CrossVolume = Value;
		UpdateRocketVolume();
		if ((bool)DayNightManager.Instance)
		{
			DayNightManager.Instance.SetAmbientVolume(1f - m_CrossVolume);
		}
	}

	private void Update()
	{
		if ((bool)TimeManager.Instance && m_FadingOut)
		{
			float num = 0.5f;
			m_FadeTimer += TimeManager.Instance.m_NormalDelta;
			if (m_FadeTimer >= num)
			{
				m_FadeTimer = num;
				m_FadingOut = false;
			}
			float num2 = m_FadeTimer / num;
			m_FadeImage.color = new Color(1f, 1f, 1f, num2);
			SetCrossFadeValue(1f - num2);
		}
	}
}
