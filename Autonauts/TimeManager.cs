using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
	public static TimeManager Instance;

	public bool m_NormalTimeEnabled;

	private bool m_OldNormalTimeEnabled;

	public float m_NormalDelta;

	public float m_NormalDeltaUnscaled;

	public bool m_CeremonyTimeEnabled;

	private bool m_OldCeremonyTimeEnabled;

	public float m_CeremonyDelta;

	public bool m_PauseTimeEnabled;

	private bool m_OldPauseTimeEnabled;

	public float m_PauseDelta;

	private List<Animator> m_Animations;

	public float m_TimeScale;

	public float m_TotalTime;

	public float m_TotalRealTime;

	public int m_Frame;

	public static float m_FastTime = 3f;

	private void Awake()
	{
		Instance = this;
		m_NormalTimeEnabled = true;
		m_CeremonyTimeEnabled = false;
		m_PauseTimeEnabled = false;
		m_TimeScale = 1f;
		m_TotalTime = 0f;
		m_TotalRealTime = 0f;
		m_Frame = 0;
		m_Animations = new List<Animator>();
	}

	private void OnDestroy()
	{
		Instance = null;
	}

	public void Save(JSONNode Node)
	{
		JSONUtils.Set(Node, "Time", m_TotalTime);
		JSONUtils.Set(Node, "RealTime", m_TotalRealTime);
	}

	public void Load(JSONNode Node)
	{
		m_TotalTime = JSONUtils.GetAsFloat(Node, "Time", 0f);
		m_TotalRealTime = JSONUtils.GetAsFloat(Node, "RealTime", 0f);
	}

	public void EnableNormalTime(bool Enabled)
	{
		m_NormalTimeEnabled = Enabled;
		UpdateAnimators();
		UpdateParticles();
	}

	public void SetTimeScale(float Scale)
	{
		m_TimeScale = Scale;
		UpdateParticles();
	}

	public void ToggleFastTime()
	{
		if (!GetIsFastTime())
		{
			Instance.SetTimeScale(m_FastTime);
		}
		else
		{
			Instance.SetTimeScale(1f);
		}
	}

	public bool GetIsFastTime()
	{
		return Instance.m_TimeScale == m_FastTime;
	}

	public void EnableCeremonyTime(bool Enabled)
	{
		m_CeremonyTimeEnabled = Enabled;
	}

	public void PauseAll()
	{
		m_OldNormalTimeEnabled = m_NormalTimeEnabled;
		EnableNormalTime(Enabled: false);
		m_OldCeremonyTimeEnabled = m_CeremonyTimeEnabled;
		EnableCeremonyTime(Enabled: false);
		m_PauseTimeEnabled = true;
	}

	public void UnPauseAll()
	{
		m_PauseTimeEnabled = false;
		EnableNormalTime(m_OldNormalTimeEnabled);
		EnableCeremonyTime(m_OldCeremonyTimeEnabled);
	}

	public void RegisterAnimator(Animator NewAnimator)
	{
		m_Animations.Add(NewAnimator);
	}

	public void UnRegisterAnimator(Animator NewAnimator)
	{
		m_Animations.Remove(NewAnimator);
	}

	private void UpdateAnimators()
	{
		foreach (Animator animation in m_Animations)
		{
			if ((bool)animation)
			{
				animation.enabled = m_NormalTimeEnabled;
			}
		}
	}

	private void UpdateParticles()
	{
		float playbackSpeed = 0f;
		if (m_NormalTimeEnabled)
		{
			playbackSpeed = m_TimeScale;
		}
		if ((bool)ParticlesManager.Instance)
		{
			ParticlesManager.Instance.SetPlaybackSpeed(playbackSpeed);
		}
	}

	private void UpdateFastTimeAvailable()
	{
		if (HudManager.Instance.m_FastTimeButton == null)
		{
			return;
		}
		if (CollectionManager.Instance.GetAnyInBuildingCollectionActive("StoneHenge"))
		{
			HudManager.Instance.m_FastTimeButton.SetActive(Active: true);
		}
		else if (HudManager.Instance.m_FastTimeButton.GetActive())
		{
			if (GetIsFastTime())
			{
				ToggleFastTime();
			}
			HudManager.Instance.m_FastTimeButton.SetActive(Active: false);
		}
	}

	private void Update()
	{
		float num = Time.deltaTime;
		float num2 = 0.25f;
		if (num > num2)
		{
			num = num2;
		}
		if (m_NormalTimeEnabled)
		{
			m_NormalDelta = num * m_TimeScale;
			m_TotalRealTime += num;
		}
		else
		{
			m_NormalDelta = 0f;
		}
		if (m_NormalTimeEnabled)
		{
			m_NormalDeltaUnscaled = num;
		}
		else
		{
			m_NormalDeltaUnscaled = 0f;
		}
		if (m_CeremonyTimeEnabled)
		{
			m_CeremonyDelta = num * m_TimeScale;
		}
		else
		{
			m_CeremonyDelta = 0f;
		}
		if (m_PauseTimeEnabled)
		{
			m_PauseDelta = num;
		}
		else
		{
			m_PauseDelta = 0f;
		}
		m_TotalTime += m_NormalDelta;
		m_Frame++;
		UpdateFastTimeAvailable();
	}
}
