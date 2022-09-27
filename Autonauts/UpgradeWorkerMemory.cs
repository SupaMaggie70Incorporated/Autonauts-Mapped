using UnityEngine;

public class UpgradeWorkerMemory : Upgrade
{
	[HideInInspector]
	public int m_Size;

	public static bool GetIsTypeUpgradeWorkerMemory(ObjectType NewType)
	{
		if (NewType == ObjectType.UpgradeWorkerMemoryCrude || NewType == ObjectType.UpgradeWorkerMemoryGood || NewType == ObjectType.UpgradeWorkerMemorySuper)
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
		m_Type = Type.WorkerMemory;
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_Size = VariableManager.Instance.GetVariableAsInt(m_TypeIdentifier, "Size");
	}
}
