using UnityEngine;

public class UpgradeWorkerCarry : Upgrade
{
	[HideInInspector]
	public int m_Capacity;

	public static bool GetIsTypeUpgradeWorkerCarry(ObjectType NewType)
	{
		if (NewType == ObjectType.UpgradeWorkerCarryCrude || NewType == ObjectType.UpgradeWorkerCarryGood || NewType == ObjectType.UpgradeWorkerCarrySuper)
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
		m_Type = Type.WorkerCarry;
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_Capacity = VariableManager.Instance.GetVariableAsInt(m_TypeIdentifier, "Capacity");
	}
}
