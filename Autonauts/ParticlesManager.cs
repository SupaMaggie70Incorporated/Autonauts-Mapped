using System.Collections.Generic;
using UnityEngine;

public class ParticlesManager : MonoBehaviour
{
	public static ParticlesManager Instance;

	private Dictionary<MyParticles, int> m_Particles;

	private List<ParticlesToDestroy> m_ParticlesToDestroy;

	public float m_PlaybackSpeed;

	private Dictionary<string, List<MyParticles>> m_OldParticles;

	private int m_IDCounter;

	private void Awake()
	{
		Instance = this;
		m_Particles = new Dictionary<MyParticles, int>();
		m_ParticlesToDestroy = new List<ParticlesToDestroy>();
		m_OldParticles = new Dictionary<string, List<MyParticles>>();
		m_PlaybackSpeed = 1f;
	}

	public MyParticles CreateParticles(string Name, Vector3 LocalPosition, Quaternion Rotation, bool AutoDestroy = false)
	{
		MyParticles myParticles;
		if (m_OldParticles.ContainsKey(Name) && m_OldParticles[Name].Count > 0)
		{
			List<MyParticles> list = m_OldParticles[Name];
			myParticles = list[list.Count - 1];
			myParticles.gameObject.SetActive(value: true);
			list.RemoveAt(list.Count - 1);
		}
		else
		{
			myParticles = Object.Instantiate((GameObject)Resources.Load("Prefabs/Particles/" + Name, typeof(GameObject)), new Vector3(0f, 0f, 0f), Quaternion.identity, MapManager.Instance.m_ParticlesRootTransform).GetComponent<ParticleSystem>().gameObject.AddComponent<MyParticles>();
			myParticles.m_PrefabName = Name;
			myParticles.m_ID = m_IDCounter;
			m_IDCounter++;
		}
		ParticleSystem particles = myParticles.m_Particles;
		particles.transform.localPosition = LocalPosition;
		particles.transform.rotation = Rotation;
		particles.Play();
		myParticles.UpdateSpeed();
		myParticles.m_DestroyWhenDone = false;
		m_Particles.Add(myParticles, 0);
		if (AutoDestroy)
		{
			DestroyParticles(myParticles, WaitUntilNoParticles: true);
		}
		return myParticles;
	}

	public void DestroyParticles(MyParticles NewParticles, bool WaitUntilNoParticles = false, bool StopSpawning = false)
	{
		if (!NewParticles)
		{
			return;
		}
		if (!WaitUntilNoParticles)
		{
			if (NewParticles.m_DestroyWhenDone)
			{
				foreach (ParticlesToDestroy item in m_ParticlesToDestroy)
				{
					if (item.m_Particles.Contains(NewParticles))
					{
						item.m_Particles.Remove(NewParticles);
						if (item.m_Particles.Count == 0)
						{
							m_ParticlesToDestroy.Remove(item);
						}
						break;
					}
				}
			}
			m_Particles.Remove(NewParticles);
			if (!m_OldParticles.ContainsKey(NewParticles.m_PrefabName))
			{
				List<MyParticles> value = new List<MyParticles>();
				m_OldParticles.Add(NewParticles.m_PrefabName, value);
			}
			NewParticles.gameObject.SetActive(value: false);
			NewParticles.transform.SetParent(MapManager.Instance.m_ParticlesRootTransform);
			m_OldParticles[NewParticles.m_PrefabName].Add(NewParticles);
		}
		else
		{
			if (StopSpawning)
			{
				NewParticles.m_Particles.Stop();
			}
			m_ParticlesToDestroy.Add(new ParticlesToDestroy(NewParticles, 0f));
			NewParticles.m_DestroyWhenDone = true;
		}
	}

	public void SetPlaybackSpeed(float Speed)
	{
		m_PlaybackSpeed = Speed;
		foreach (KeyValuePair<MyParticles, int> particle in m_Particles)
		{
			MyParticles key = particle.Key;
			if ((bool)key)
			{
				key.UpdateSpeed();
			}
		}
	}

	public void Update()
	{
		List<ParticlesToDestroy> list = new List<ParticlesToDestroy>();
		foreach (ParticlesToDestroy item in m_ParticlesToDestroy)
		{
			item.m_TimeLeft += TimeManager.Instance.m_NormalDelta;
			if (!(item.m_TimeLeft > 0.1f))
			{
				continue;
			}
			int num = 0;
			foreach (MyParticles particle in item.m_Particles)
			{
				num += particle.m_Particles.particleCount;
			}
			if (num == 0)
			{
				list.Add(item);
			}
		}
		foreach (ParticlesToDestroy item2 in list)
		{
			m_ParticlesToDestroy.Remove(item2);
			foreach (MyParticles particle2 in item2.m_Particles)
			{
				DestroyParticles(particle2);
			}
		}
	}
}
