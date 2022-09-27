using UnityEngine;

public class ToolTorchCrude : ToolLight
{
	private MyParticles m_Particles;

	private float m_FireTimer;

	public override void Restart()
	{
		base.Restart();
		m_Particles.Clear();
		m_Particles.Play();
		UpdateParticlePosition();
	}

	protected new void Awake()
	{
		base.Awake();
		if ((bool)ParticlesManager.Instance)
		{
			m_Particles = ParticlesManager.Instance.CreateParticles("TorchCrudeSmoke", default(Vector3), Quaternion.identity);
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

	public override void StopUsing(bool AndDestroy = true)
	{
		if ((bool)m_Particles)
		{
			m_Particles.Stop();
		}
		base.StopUsing(AndDestroy);
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
		m_Particles.transform.position = base.transform.position + new Vector3(0f, 1f, 0f);
		m_Particles.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
	}

	private void Update()
	{
		UpdateParticlePosition();
		m_FireTimer += TimeManager.Instance.m_NormalDelta;
		if (m_FireTimer >= 0.05f)
		{
			m_FireTimer = 0f;
			m_Light.SetIntensity(Random.Range(0.95f, 1.05f));
		}
	}
}
