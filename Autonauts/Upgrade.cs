using UnityEngine;

public class Upgrade : Holdable
{
	public enum Target
	{
		Both,
		Player,
		Bot,
		Total
	}

	public enum Type
	{
		PlayerInventory,
		PlayerWhistle,
		PlayerMovement,
		WorkerCarry,
		WorkerEnergy,
		WorkerInventory,
		WorkerMemory,
		WorkerMovement,
		WorkerSearch,
		Total
	}

	[HideInInspector]
	public Target m_Target;

	[HideInInspector]
	public Type m_Type;

	public int m_Level;

	public static bool GetIsTypeUpgrade(ObjectType NewType)
	{
		if (UpgradeInventory.GetIsTypeUpgradeInventory(NewType) || UpgradePlayerWhistle.GetIsTypeUpgradePlayerWhistle(NewType) || UpgradePlayerMovement.GetIsTypeUpgradePlayerMovement(NewType) || GetIsTypeWorkerUpgrade(NewType))
		{
			return true;
		}
		return false;
	}

	public static bool GetIsTypeWorkerUpgrade(ObjectType NewType)
	{
		if (UpgradeWorkerInventory.GetIsTypeUpgradeWorkerInventory(NewType) || UpgradeWorkerMemory.GetIsTypeUpgradeWorkerMemory(NewType) || UpgradeWorkerSearch.GetIsTypeUpgradeWorkerSearch(NewType) || UpgradeWorkerCarry.GetIsTypeUpgradeWorkerCarry(NewType) || UpgradeWorkerMovement.GetIsTypeUpgradeWorkerMovement(NewType) || UpgradeWorkerEnergy.GetIsTypeUpgradeWorkerEnergy(NewType))
		{
			return true;
		}
		return false;
	}

	protected new void Awake()
	{
		base.Awake();
		m_Target = Target.Both;
		m_Type = Type.Total;
	}
}
