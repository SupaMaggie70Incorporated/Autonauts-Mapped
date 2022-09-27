using System.Collections.Generic;

public class ParticlesToDestroy
{
	public List<MyParticles> m_Particles;

	public float m_TimeLeft;

	public ParticlesToDestroy(MyParticles NewParticles, float TimeLeft)
	{
		m_Particles = new List<MyParticles>();
		MyParticles[] componentsInChildren = NewParticles.GetComponentsInChildren<MyParticles>();
		foreach (MyParticles item in componentsInChildren)
		{
			m_Particles.Add(item);
		}
		m_TimeLeft = TimeLeft;
	}
}
