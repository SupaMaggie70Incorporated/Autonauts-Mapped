public class ResearchStationCrude3 : ResearchStationCrude2
{
	private static int m_ModuleIndex = 2;

	private PlaySound m_Sound;

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
			m_Animator.Play("GrinderConvert", Module, 0f);
			m_Sound = AudioManager.Instance.StartEvent("BuildingResearchGrinding", this, Remember: true);
		}
	}

	protected override void StopModuleAnimation(int Module)
	{
		base.StopModuleAnimation(Module);
		if (Module == m_ModuleIndex)
		{
			AudioManager.Instance.StopEvent(m_Sound);
		}
	}
}
