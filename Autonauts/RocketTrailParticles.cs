using System.Collections.Generic;
using UnityEngine;

public class RocketTrailParticles : MonoBehaviour
{
	private MyParticles ps;

	private List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();

	private MyParticles child;

	private void OnEnable()
	{
		ps = GetComponent<MyParticles>();
	}

	public void SetChildParticles(MyParticles NewParticles)
	{
		child = NewParticles;
	}

	public void SetActive(bool Active)
	{
		if (Active)
		{
			ps.Play();
		}
		else
		{
			ps.Stop();
		}
	}

	private void OnParticleTrigger()
	{
		int triggerParticles = ps.m_Particles.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);
		if (triggerParticles > 0)
		{
			child.m_Particles.Play();
			child.m_Particles.Emit(triggerParticles);
		}
	}
}
