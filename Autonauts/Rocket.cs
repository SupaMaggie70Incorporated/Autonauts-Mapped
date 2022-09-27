using System.Collections.Generic;
using UnityEngine;

public class Rocket : BaseClass
{
	private GameObject m_RocketTrailGround;

	private GameObject m_RocketTrailCollider;

	private GameObject m_RocketTrailParticles;

	private MyLight m_Light;

	public override void PostCreate()
	{
		base.PostCreate();
		base.name = "Rocket";
		m_ModelRoot.transform.Find("Aaron").gameObject.SetActive(value: false);
		m_ModelRoot.transform.Find("Gary").gameObject.SetActive(value: false);
		m_Light = LightManager.Instance.LoadLight("RocketLight", m_ModelRoot.transform, default(Vector3));
		m_Light.transform.localPosition = new Vector3(0f, 0f, 0f);
		GameObject original = (GameObject)Resources.Load("Prefabs/Particles/Rocket/RocketTrailGround", typeof(GameObject));
		m_RocketTrailGround = Object.Instantiate(original, base.transform.position, Quaternion.Euler(90f, 0f, 0f), null);
		original = (GameObject)Resources.Load("Prefabs/Particles/Rocket/RocketTrailCollider", typeof(GameObject));
		m_RocketTrailCollider = Object.Instantiate(original, base.transform.position, Quaternion.identity, null);
		original = (GameObject)Resources.Load("Prefabs/Particles/Rocket/RocketTrailParticles", typeof(GameObject));
		m_RocketTrailParticles = Object.Instantiate(original, base.transform.position, Quaternion.Euler(90f, 0f, 0f), m_ModelRoot.transform);
		m_RocketTrailParticles.transform.localPosition = new Vector3(0f, 0f, 0f) + new Vector3(0f, -3f, 0f);
		if (GeneralUtils.m_InGame)
		{
			m_RocketTrailParticles.GetComponent<MyParticles>().m_Particles.trigger.SetCollider(0, m_RocketTrailCollider.GetComponent<BoxCollider>());
		}
		else
		{
			m_RocketTrailParticles.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
		}
		m_RocketTrailParticles.GetComponent<RocketTrailParticles>().SetChildParticles(m_RocketTrailGround.GetComponent<MyParticles>());
	}

	private void OnDestroy()
	{
		LightManager.Instance.DestroyLight(m_Light);
		Object.Destroy(m_RocketTrailGround.gameObject);
		Object.Destroy(m_RocketTrailCollider.gameObject);
	}

	public override void Restart()
	{
		base.Restart();
		if ((bool)CollectionManager.Instance)
		{
			List<BaseClass> players = CollectionManager.Instance.GetPlayers();
			if (players != null && players.Count > 0)
			{
				players[0].GetComponent<Farmer>().gameObject.SetActive(value: false);
			}
		}
	}

	private void Update()
	{
		Vector3 position = base.transform.position;
		position.y = 0.25f;
		m_RocketTrailGround.transform.position = position;
		m_RocketTrailCollider.transform.position = position;
	}
}
