using UnityEngine;

public class MyParticles : MonoBehaviour
{
	public ParticleSystem m_Particles;

	public float m_Speed;

	public string m_PrefabName;

	public int m_ID;

	public bool m_DestroyWhenDone;

	private void Awake()
	{
		m_Particles = GetComponent<ParticleSystem>();
		m_Speed = 1f;
	}

	public void Clear()
	{
		m_Particles.Clear();
	}

	public void Play()
	{
		m_Particles.Play();
	}

	public bool GetIsPlaying()
	{
		return m_Particles.isPlaying;
	}

	public void Stop()
	{
		m_Particles.Stop();
	}

	public void SetSpeed(float Speed)
	{
		m_Speed = Speed;
		UpdateSpeed();
	}

	public void UpdateSpeed()
	{
		ParticleSystem.MainModule main = m_Particles.main;
		main.simulationSpeed = ParticlesManager.Instance.m_PlaybackSpeed * m_Speed;
	}
}
