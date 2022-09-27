using UnityEngine;

public class ResearchStationAnimControl : MonoBehaviour
{
	public static float m_SamplePlateDepth = -0.4f;

	private ResearchStation m_Station;

	private Transform m_SamplePlate;

	private Vector3 m_SamplePlatePosition;

	private MeshRenderer m_SampleBulb;

	private MeshRenderer m_WuvBulb;

	private GameObject m_Funnel;

	private Wobbler m_FunnelWobbler;

	private MyParticles m_RayParticlesActive;

	private MyParticles m_RayParticlesActive2;

	private bool m_First;

	private bool m_Researching;

	private PlaySound m_PlaySound;

	private float m_SampleTimer;

	private Vector3 m_SampleScale;

	public void Restart()
	{
		m_First = false;
	}

	public void GetModelParts()
	{
		m_Station = GetComponent<ResearchStation>();
		_ = m_Station.m_ModelRoot.transform;
		m_SamplePlate = m_Station.m_UpgradeModels[0].transform.Find("SamplePlate");
		m_SamplePlatePosition = m_SamplePlate.transform.localPosition;
		m_SampleBulb = m_Station.m_UpgradeModels[0].transform.Find("SampleBulb").GetComponent<MeshRenderer>();
		m_WuvBulb = m_Station.m_UpgradeModels[0].transform.Find("WuvBulb").GetComponent<MeshRenderer>();
		m_Funnel = m_Station.m_UpgradeModels[0].transform.Find("WuvFunnel").gameObject;
		m_Funnel.SetActive(value: false);
		m_FunnelWobbler = new Wobbler();
		if ((bool)ParticlesManager.Instance && m_RayParticlesActive == null)
		{
			m_RayParticlesActive = ParticlesManager.Instance.CreateParticles("ResearchActive", default(Vector3), Quaternion.Euler(0f, 0f, 0f));
			m_RayParticlesActive.transform.SetParent(base.transform);
			m_RayParticlesActive.transform.position = m_Station.m_UpgradeModels[0].transform.Find("SparkPointA").position;
			m_RayParticlesActive.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
			m_RayParticlesActive.Play();
			m_RayParticlesActive2 = ParticlesManager.Instance.CreateParticles("ResearchActive", default(Vector3), Quaternion.Euler(0f, 0f, 0f));
			m_RayParticlesActive2.transform.SetParent(base.transform);
			m_RayParticlesActive2.transform.position = m_Station.m_UpgradeModels[0].transform.Find("SparkPointB").position;
			m_RayParticlesActive2.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
			m_RayParticlesActive2.Play();
		}
		SetSampleBulbOn(On: false);
		SetWuvBulbOn(On: false);
		SetRayParticlesOn(On: false);
		SetSoundActive(On: false);
	}

	private void OnDestroy()
	{
		if ((bool)ParticlesManager.Instance && (bool)m_RayParticlesActive)
		{
			ParticlesManager.Instance.DestroyParticles(m_RayParticlesActive);
		}
		if ((bool)ParticlesManager.Instance && (bool)m_RayParticlesActive2)
		{
			ParticlesManager.Instance.DestroyParticles(m_RayParticlesActive2);
		}
		if (m_PlaySound != null)
		{
			AudioManager.Instance.StopEvent(m_PlaySound);
		}
	}

	private void SetSampleBulbOn(bool On)
	{
		if (!m_Station.m_Blueprint)
		{
			string text = "SharedRedGlow";
			if (On)
			{
				text = "SharedGreenGlow";
			}
			Material material = (Material)Resources.Load("Models/Materials/" + text, typeof(Material));
			m_SampleBulb.material = material;
		}
	}

	private void SetSamplePlateOn(bool On)
	{
		float z = 0f;
		if (On)
		{
			z = m_SamplePlateDepth;
		}
		m_SamplePlate.transform.localPosition = m_SamplePlatePosition + new Vector3(0f, 0f, z);
	}

	private void SetFunnelOn(bool On)
	{
		m_Funnel.SetActive(On);
		if (On)
		{
			m_FunnelWobbler.Go(0.25f, 5f, 0.5f);
		}
	}

	private void UpdateFunnel()
	{
		m_FunnelWobbler.Update();
		float num = 0.5f - m_FunnelWobbler.m_Height + 0.5f;
		m_Funnel.transform.localScale = new Vector3(num, num, num);
	}

	private void SetWuvBulbOn(bool On)
	{
		if (!m_Station.m_Blueprint)
		{
			string text = "SharedRedGlow";
			if (On)
			{
				text = "SharedGreenGlow";
			}
			Material material = (Material)Resources.Load("Models/Materials/" + text, typeof(Material));
			m_WuvBulb.material = material;
		}
	}

	private void SetRayParticlesOn(bool On)
	{
		if ((bool)m_RayParticlesActive)
		{
			if (On)
			{
				m_RayParticlesActive.SetSpeed(1f);
				m_RayParticlesActive2.SetSpeed(1f);
			}
			else
			{
				m_RayParticlesActive.SetSpeed(0.2f);
				m_RayParticlesActive2.SetSpeed(0.2f);
			}
		}
	}

	private void SetSoundActive(bool On)
	{
		if (m_PlaySound != null)
		{
			AudioManager.Instance.StopEvent(m_PlaySound);
		}
		if (!m_Station.m_Blueprint)
		{
			if (On)
			{
				m_PlaySound = AudioManager.Instance.StartEvent("BuildingResearchMaking", m_Station, Remember: true);
			}
			else
			{
				m_PlaySound = AudioManager.Instance.StartEvent("BuildingResearchIdle", m_Station, Remember: true);
			}
		}
	}

	public void UpdateSample(bool ForceOff = false)
	{
		if (m_Station.m_CurrentResearchObject != null && !ForceOff)
		{
			SetSampleBulbOn(On: true);
			SetSamplePlateOn(On: true);
			SetFunnelOn(On: true);
		}
		else
		{
			SetSampleBulbOn(On: false);
			SetSamplePlateOn(On: false);
			SetFunnelOn(On: false);
		}
	}

	public void UpdateState(bool Force = false)
	{
		if (m_Station == null)
		{
			return;
		}
		bool flag = m_Station.m_State == ResearchStation.State.Researching;
		if (!(flag != m_Researching || Force))
		{
			return;
		}
		if (flag)
		{
			SetWuvBulbOn(On: true);
			SetRayParticlesOn(On: true);
			SetSoundActive(On: true);
			if ((bool)m_Station.m_CurrentResearchObject)
			{
				m_SampleScale = m_Station.m_CurrentResearchObject.transform.localScale;
			}
			else
			{
				m_SampleScale = new Vector3(1f, 1f, 1f);
			}
		}
		else
		{
			SetWuvBulbOn(On: false);
			SetRayParticlesOn(On: false);
			SetSoundActive(On: false);
			if ((bool)m_Station.m_CurrentResearchObject && m_Researching)
			{
				m_Station.m_CurrentResearchObject.transform.localScale = m_SampleScale;
			}
		}
		m_Researching = flag;
	}

	public void SetBlueprint(bool Blueprint)
	{
		m_RayParticlesActive.gameObject.SetActive(!Blueprint);
		m_RayParticlesActive2.gameObject.SetActive(!Blueprint);
		if (Blueprint)
		{
			Material material = (Material)Resources.Load("Models/Materials/SharedWhite", typeof(Material));
			m_WuvBulb.material = material;
			m_SampleBulb.material = material;
		}
		UpdateSample();
		UpdateState(Force: true);
	}

	private void UpdateHeartShrink()
	{
		if (m_Station.m_Heart == null)
		{
			return;
		}
		float totalResearchDelay = m_Station.GetTotalResearchDelay();
		float currentResearchTimer = m_Station.GetCurrentResearchTimer();
		float num = 1f - currentResearchTimer / totalResearchDelay;
		if (num > 0f)
		{
			float tierScale = BaseClass.GetTierScale(FolkHeart.GetTierFromObjectType(m_Station.m_Heart.GetComponent<FolkHeart>().m_TypeIdentifier));
			num *= tierScale;
			if ((int)(currentResearchTimer * 60f) % 10 < 5)
			{
				num *= 1.1f;
			}
			m_Station.m_Heart.transform.localScale = new Vector3(num, num, num);
		}
		else
		{
			m_Station.m_Heart.transform.localScale = new Vector3(0f, 0f, 0f);
		}
	}

	private void UpdateSampleAnim()
	{
		m_SampleTimer += TimeManager.Instance.m_NormalDelta;
		float num = 1f;
		if ((int)(m_SampleTimer * 60f) % 10 < 5)
		{
			num = 1.2f;
		}
		if ((bool)m_Station.m_CurrentResearchObject)
		{
			m_Station.m_CurrentResearchObject.transform.localScale = m_SampleScale * num;
		}
	}

	private void Update()
	{
		if (!m_First)
		{
			m_First = true;
			UpdateState(Force: true);
		}
		if (!m_RayParticlesActive.m_Particles.isPlaying)
		{
			m_RayParticlesActive.Play();
			m_RayParticlesActive2.Play();
		}
		UpdateFunnel();
		if (m_Researching)
		{
			UpdateHeartShrink();
			UpdateSampleAnim();
		}
	}
}
