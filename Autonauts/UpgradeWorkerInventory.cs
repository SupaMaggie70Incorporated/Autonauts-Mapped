public class UpgradeWorkerInventory : UpgradeInventory
{
	public static bool GetIsTypeUpgradeWorkerInventory(ObjectType NewType)
	{
		if (NewType == ObjectType.UpgradeWorkerInventoryCrude || NewType == ObjectType.UpgradeWorkerInventoryGood || NewType == ObjectType.UpgradeWorkerInventorySuper)
		{
			return true;
		}
		return false;
	}

	protected new void Awake()
	{
		base.Awake();
		m_Target = Target.Bot;
		m_Type = Type.WorkerInventory;
	}
}
