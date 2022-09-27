using System.Collections.Generic;
using UnityEngine;

public class Rain : MonoBehaviour
{
	protected MyParticles ps;

	protected List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();

	protected MyParticles m_ChildParticles;

	protected int m_EmitFrequency;

	private ParticleSystem.Particle[] m_TempParticles;

	private Vector3 m_OldPosition;

	protected void Awake()
	{
		m_EmitFrequency = 800;
		m_TempParticles = new ParticleSystem.Particle[10000];
	}

	private void OnEnable()
	{
		ps = GetComponent<MyParticles>();
	}

	public void SetChildParticles(MyParticles NewParticles)
	{
		m_ChildParticles = NewParticles;
	}

	public void SetStrength(float Strength)
	{
		ParticleSystem.EmissionModule emission = ps.m_Particles.emission;
		emission.rateOverTime = Strength * (float)m_EmitFrequency;
		if (Strength > 0f || ps.m_Particles.particleCount > 0)
		{
			base.gameObject.SetActive(value: true);
		}
	}

	private void OnParticleTrigger()
	{
		if (ps.m_Particles.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter) <= 0)
		{
			return;
		}
		m_ChildParticles.m_Particles.Play();
		ParticleSystem.EmitParams emitParams = default(ParticleSystem.EmitParams);
		foreach (ParticleSystem.Particle item in enter)
		{
			Vector3 position = item.position;
			position.y = 0f;
			emitParams.position = position;
			m_ChildParticles.m_Particles.Emit(emitParams, 1);
		}
	}

	private void OnPreRender()
	{
		if (!(m_OldPosition != base.transform.position))
		{
			return;
		}
		float num = 50f;
		float num2 = 50f;
		float num3 = 50f;
		float num4 = base.transform.position.x - num / 2f;
		float num5 = base.transform.position.x + num / 2f;
		float num6 = base.transform.position.z + num2 / 2f;
		float num7 = base.transform.position.z - num2 / 2f;
		float num8 = base.transform.position.y + num3 * 0.25f;
		float num9 = base.transform.position.y - num3 * 0.75f;
		int particleCount = ps.m_Particles.particleCount;
		ps.m_Particles.GetParticles(m_TempParticles);
		for (int i = 0; i < particleCount; i++)
		{
			Vector3 position = m_TempParticles[i].position;
			if (position.x < num4)
			{
				position.x += num;
			}
			if (position.x > num5)
			{
				position.x -= num;
			}
			if (position.z > num6)
			{
				position.z -= num2;
			}
			if (position.z < num7)
			{
				position.z += num2;
			}
			if (position.y > num8)
			{
				position.y -= num3;
			}
			if (position.y < num9)
			{
				position.y += num3;
			}
			m_TempParticles[i].position = position;
		}
		ps.m_Particles.SetParticles(m_TempParticles);
		m_OldPosition = base.transform.position;
	}

	private void Update()
	{
		if (ps.m_Particles.particleCount == 0)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
