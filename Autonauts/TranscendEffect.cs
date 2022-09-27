using UnityEngine;

public class TranscendEffect : BaseClass
{
	private MyParticles m_Particles;

	private Animator m_Animator;

	private PlaySound m_Sound;

	public GameObject m_Heart;

	public override void PostCreate()
	{
		base.PostCreate();
		if ((bool)ParticlesManager.Instance)
		{
			m_Particles = ParticlesManager.Instance.CreateParticles("FolkTranscend", base.transform.position, Quaternion.identity);
			m_Particles.transform.SetParent(base.transform);
			m_Particles.transform.localPosition = default(Vector3);
			m_Particles.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
			m_Particles.Stop();
		}
		m_Heart = base.transform.Find("FolkHeart8").gameObject;
		m_Heart.gameObject.SetActive(value: false);
		m_Animator = GetComponent<Animator>();
		m_Animator.StopPlayback();
		m_Animator.Rebind();
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		if ((bool)m_Particles)
		{
			ParticlesManager.Instance.DestroyParticles(m_Particles, WaitUntilNoParticles: false, StopSpawning: true);
		}
		base.StopUsing(AndDestroy);
	}

	public void StartEffect()
	{
		m_Particles.Play();
		m_Animator.Play("FolkTranscendAnimation", -1, 0f);
		m_Sound = AudioManager.Instance.StartEventAmbient("FolkTranscend", m_Heart, Remember: true, Follow: true);
		AudioManager.Instance.SetEventVolume(m_Sound, 0f);
	}

	public void StopEffect()
	{
		m_Particles.Stop();
		AudioManager.Instance.StopEvent(m_Sound);
	}

	private void Update()
	{
		if ((bool)TimeManager.Instance && TimeManager.Instance.m_NormalTimeEnabled)
		{
			m_Animator.speed = TimeManager.Instance.m_TimeScale;
		}
		else
		{
			m_Animator.speed = 0f;
		}
		if (m_Sound != null)
		{
			float x = m_Heart.transform.localScale.x;
			AudioManager.Instance.SetEventVolume(m_Sound, x);
		}
	}
}
