public class ResearchStationCrude6 : ResearchStationCrude5
{
	private static int m_ModuleIndex = 5;

	public override void Restart()
	{
		EnableModule(m_ModuleIndex);
		base.Restart();
	}

	protected override void StartModuleAnimation(int Module)
	{
		base.StartModuleAnimation(Module);
		if (Module == m_ModuleIndex)
		{
			m_Animator.Play("DissectionConvert", Module, 0f);
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
			AudioManager.Instance.StartEvent("BuildingResearchDissection", this);
		}
	}
}
