using UnityEngine;

public class BerryPie : Pie
{
	private MyParticles m_Particles;

	public override void Restart()
	{
		base.Restart();
		m_Particles.Clear();
		m_Particles.Play();
		UpdateParticlePosition();
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		m_Particles.Stop();
		base.StopUsing(AndDestroy);
	}

	protected new void Awake()
	{
		base.Awake();
		if ((bool)ParticlesManager.Instance)
		{
			m_Particles = ParticlesManager.Instance.CreateParticles("PieSteam", default(Vector3), Quaternion.identity);
		}
	}

	protected new void OnDestroy()
	{
		if ((bool)m_Particles)
		{
			ParticlesManager.Instance.DestroyParticles(m_Particles);
		}
		base.OnDestroy();
	}

	public override void SendAction(ActionInfo Info)
	{
		switch (Info.m_Action)
		{
		case ActionType.Stowed:
			m_Particles.gameObject.SetActive(value: false);
			break;
		case ActionType.Recalled:
			m_Particles.gameObject.SetActive(value: true);
			UpdateParticlePosition();
			break;
		case ActionType.Refresh:
		case ActionType.RefreshFirst:
			m_Particles.transform.position = base.transform.position;
			break;
		}
		base.SendAction(Info);
	}

	private void UpdateParticlePosition()
	{
		m_Particles.transform.position = base.transform.position;
		m_Particles.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
	}

	private void Update()
	{
		UpdateParticlePosition();
	}
}
