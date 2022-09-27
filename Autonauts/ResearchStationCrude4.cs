using UnityEngine;

public class ResearchStationCrude4 : ResearchStationCrude3
{
	private static int m_ModuleIndex = 3;

	private Transform m_FirePoint;

	private MyParticles m_SmokeParticles;

	private MyLight m_FireLight;

	private PlaySound m_Sound;

	public override void Restart()
	{
		EnableModule(m_ModuleIndex);
		base.Restart();
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_FirePoint = m_UpgradeModels[m_ModuleIndex].transform.Find("Fire").Find("FirePoint");
		m_FireLight = LightManager.Instance.LoadLight("GenericFire", base.transform, new Vector3(0f, 0f, 0f));
		m_FireLight.transform.position = m_FirePoint.position;
		m_FireLight.SetActive(Active: false);
	}

	protected new void OnDestroy()
	{
		LightManager.Instance.DestroyLight(m_FireLight);
		base.OnDestroy();
	}

	protected override void StartModuleAnimation(int Module)
	{
		base.StartModuleAnimation(Module);
		if (Module == m_ModuleIndex)
		{
			m_Animator.Play("HeatingConvert", Module, 0f);
			m_SmokeParticles = ParticlesManager.Instance.CreateParticles("ResearchSmoke", m_FirePoint.position, Quaternion.Euler(-60f, 90f, 0f));
			m_FireLight.SetActive(Active: true);
			m_Sound = AudioManager.Instance.StartEvent("BuildingResearchHeating", this, Remember: true);
		}
	}

	protected override void StopModuleAnimation(int Module)
	{
		base.StopModuleAnimation(Module);
		if (Module == m_ModuleIndex)
		{
			AudioManager.Instance.StopEvent(m_Sound);
			ParticlesManager.Instance.DestroyParticles(m_SmokeParticles, WaitUntilNoParticles: true, StopSpawning: true);
			m_FireLight.SetActive(Active: false);
		}
	}
}
