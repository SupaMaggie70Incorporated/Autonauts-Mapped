public class UpgradePlayerInventory : UpgradeInventory
{
	public static bool GetIsTypeUpgradePlayerInventory(ObjectType NewType)
	{
		if (NewType == ObjectType.UpgradePlayerInventoryCrude || NewType == ObjectType.UpgradePlayerInventoryGood || NewType == ObjectType.UpgradePlayerInventorySuper)
		{
			return true;
		}
		return false;
	}

	protected new void Awake()
	{
		base.Awake();
		m_Target = Target.Player;
		m_Type = Type.PlayerInventory;
	}
}
