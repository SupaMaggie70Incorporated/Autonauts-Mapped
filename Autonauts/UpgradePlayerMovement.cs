public class UpgradePlayerMovement : Upgrade
{
	public static bool GetIsTypeUpgradePlayerMovement(ObjectType NewType)
	{
		if (NewType == ObjectType.UpgradePlayerMovementCrude || NewType == ObjectType.UpgradePlayerMovementGood || NewType == ObjectType.UpgradePlayerMovementSuper)
		{
			return true;
		}
		return false;
	}

	protected new void Awake()
	{
		base.Awake();
		m_Target = Target.Player;
		m_Type = Type.PlayerMovement;
	}
}
