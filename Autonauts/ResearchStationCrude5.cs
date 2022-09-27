using UnityEngine;

public class ResearchStationCrude5 : ResearchStationCrude4
{
	private static int m_ModuleIndex = 4;

	private Transform m_BubblesPoint;

	private MyParticles m_BubblesParticles;

	private PlaySound m_Sound;

	public override void Restart()
	{
		EnableModule(m_ModuleIndex);
		base.Restart();
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_BubblesPoint = m_UpgradeModels[m_ModuleIndex].transform.Find("Bath").Find("BubblePoint");
	}

	protected override void StartModuleAnimation(int Module)
	{
		base.StartModuleAnimation(Module);
		if (Module == m_ModuleIndex)
		{
			m_Animator.Play("SoakingConvert", Module, 0f);
			m_BubblesParticles = ParticlesManager.Instance.CreateParticles("ResearchSoaking", m_BubblesPoint.position, Quaternion.Euler(-90f, 0f, 0f));
			m_Sound = AudioManager.Instance.StartEvent("BuildingResearchSoaking", this, Remember: true);
		}
	}

	protected override void StopModuleAnimation(int Module)
	{
		base.StopModuleAnimation(Module);
		if (Module == m_ModuleIndex)
		{
			AudioManager.Instance.StopEvent(m_Sound);
			ParticlesManager.Instance.DestroyParticles(m_BubblesParticles, WaitUntilNoParticles: true, StopSpawning: true);
		}
	}
}
