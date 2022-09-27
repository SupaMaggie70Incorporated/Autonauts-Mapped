using UnityEngine;

public class ResearchStationCrude : ResearchStation
{
	private static int m_ModuleIndex;

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(-1, -1), new TileCoord(3, 0), new TileCoord(1, 0));
		RemoveTile(new TileCoord(1, 0));
	}

	protected override void GetModelParts()
	{
		base.GetModelParts();
		GameObject item = m_ModelRoot.transform.Find("ImpactApparatus").gameObject;
		m_UpgradeModels.Add(item);
		item = m_ModelRoot.transform.Find("GrinderApparatus").gameObject;
		m_UpgradeModels.Add(item);
		item = m_ModelRoot.transform.Find("HeatingApparatus").gameObject;
		m_UpgradeModels.Add(item);
		item = m_ModelRoot.transform.Find("SoakingApparatus").gameObject;
		m_UpgradeModels.Add(item);
		item = m_ModelRoot.transform.Find("DissectionApparatus").gameObject;
		m_UpgradeModels.Add(item);
	}

	protected override void StartModuleAnimation(int Module)
	{
		base.StartModuleAnimation(Module);
		_ = new string[6] { "BaseConvert", "ImpactConvert", "GrinderConvert", "HeatingConvert", "SoakingConvert", "DissectionConvert" };
		if (Module == m_ModuleIndex)
		{
			m_Animator.Play("BaseConvert", Module, 0f);
		}
	}

	protected override void StopModuleAnimation(int Module)
	{
		base.StopModuleAnimation(Module);
	}
}
