public class ResearchStationCrude2 : ResearchStationCrude
{
	private static int m_ModuleIndex = 1;

	public override void Restart()
	{
		EnableModule(1);
		base.Restart();
	}

	protected override void StartModuleAnimation(int Module)
	{
		base.StartModuleAnimation(Module);
		if (Module == m_ModuleIndex)
		{
			m_Animator.Play("ImpactConvert", Module, 0f);
		}
	}

	protected override void StopModuleAnimation(int Module)
	{
		base.StopModuleAnimation(Module);
	}

	public override void DoModuleAction()
	{
		base.DoModuleAction();
		if (m_CurrentModule == m_ModuleIndex)
		{
			AudioManager.Instance.StartEvent("BuildingResearchImpact", this);
		}
	}
}
