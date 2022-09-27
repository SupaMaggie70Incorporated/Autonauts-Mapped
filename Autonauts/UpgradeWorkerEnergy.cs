using UnityEngine;

public class UpgradeWorkerEnergy : Upgrade
{
	[HideInInspector]
	public float m_Energy;

	public static bool GetIsTypeUpgradeWorkerEnergy(ObjectType NewType)
	{
		if (NewType == ObjectType.UpgradeWorkerEnergyCrude || NewType == ObjectType.UpgradeWorkerEnergyGood || NewType == ObjectType.UpgradeWorkerEnergySuper)
		{
			return true;
		}
		return false;
	}

	public override void Restart()
	{
		base.Restart();
	}

	protected new void Awake()
	{
		base.Awake();
		m_Target = Target.Bot;
		m_Type = Type.WorkerEnergy;
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_Energy = VariableManager.Instance.GetVariableAsFloat(m_TypeIdentifier, "DriveEnergy");
	}
}
