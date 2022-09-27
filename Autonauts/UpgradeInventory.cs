using UnityEngine;

public class UpgradeInventory : Upgrade
{
	[HideInInspector]
	public int m_Capacity;

	public static bool GetIsTypeUpgradeInventory(ObjectType NewType)
	{
		if (UpgradePlayerInventory.GetIsTypeUpgradePlayerInventory(NewType) || UpgradeWorkerInventory.GetIsTypeUpgradeWorkerInventory(NewType))
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
		m_Target = Target.Both;
		m_Capacity = 1;
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_Capacity = VariableManager.Instance.GetVariableAsInt(m_TypeIdentifier, "Capacity");
	}
}
