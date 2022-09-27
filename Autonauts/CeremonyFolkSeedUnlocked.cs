using System.Collections.Generic;
using UnityEngine;

public class CeremonyFolkSeedUnlocked : CeremonyGenericSpeechWithTitle
{
	private FolkSeedPod m_Pod;

	private Animator m_FolkSeedPodTop;

	private MyParticles m_Particles;

	protected new void Awake()
	{
		base.Awake();
		SetTitle("CeremonyFolkSeedUnlocked");
		ShowSpeech(Show: false);
		AudioManager.Instance.StartEvent("CeremonyFolkSeedUnlocked");
		CreateSeedPodAnimation();
	}

	private void CreateSeedPodAnimation()
	{
		Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("Converter");
		if (collection != null)
		{
			foreach (KeyValuePair<BaseClass, int> item in collection)
			{
				Converter component = item.Key.GetComponent<Converter>();
				if (component.m_TypeIdentifier == ObjectType.FolkSeedPod)
				{
					m_Pod = component.GetComponent<FolkSeedPod>();
					break;
				}
			}
		}
		if ((bool)m_Pod)
		{
			m_Pod.ShowTop(Show: false);
			GameObject original = (GameObject)Resources.Load("Models/Animations/Buildings/Converters/FolkSeedPodTop/FolkSeedPodTop", typeof(GameObject));
			m_FolkSeedPodTop = Object.Instantiate(original, m_Pod.m_ModelRoot.transform.Find("FolkSeedPodTop").position, Quaternion.identity, null).GetComponent<Animator>();
			m_FolkSeedPodTop.Play(0);
			PanTo(m_Pod, new Vector3(0f, 10f, 0f), 20f, 1f);
			m_Particles = ParticlesManager.Instance.CreateParticles("Transmitter", default(Vector3), Quaternion.Euler(90f, 0f, 0f));
			m_Particles.transform.position = m_Pod.transform.position + new Vector3(0f, 20f, 0f);
			m_Particles.Play();
		}
	}

	private void Update()
	{
		if ((bool)m_FolkSeedPodTop && m_FolkSeedPodTop.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && !m_FolkSeedPodTop.IsInTransition(0))
		{
			if ((bool)m_Pod)
			{
				m_Pod.ShowTop(Show: true);
			}
			Object.Destroy(m_FolkSeedPodTop.gameObject);
			ParticlesManager.Instance.DestroyParticles(m_Particles, WaitUntilNoParticles: true, StopSpawning: true);
			SetSpeech("CeremonyFolkSeedUnlockedSpeech1");
			ShowSpeech(Show: true);
		}
	}

	protected override void End()
	{
		base.End();
		if ((bool)m_Pod)
		{
			ReturnPanTo(1f);
		}
	}
}
