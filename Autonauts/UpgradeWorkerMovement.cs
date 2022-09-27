using UnityEngine;

public class UpgradeWorkerMovement : Upgrade
{
	[HideInInspector]
	public int m_InitialDelay;

	[HideInInspector]
	public float m_MoveScale;

	public static bool GetIsTypeUpgradeWorkerMovement(ObjectType NewType)
	{
		if (NewType == ObjectType.UpgradeWorkerMovementCrude || NewType == ObjectType.UpgradeWorkerMovementGood || NewType == ObjectType.UpgradeWorkerMovementSuper)
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
		m_Type = Type.WorkerMovement;
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_InitialDelay = VariableManager.Instance.GetVariableAsInt(m_TypeIdentifier, "InitialDelay");
		m_MoveScale = VariableManager.Instance.GetVariableAsFloat(m_TypeIdentifier, "MoveScale");
	}
}
