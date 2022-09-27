using UnityEngine;

public class UpgradeWorkerSearch : Upgrade
{
	[HideInInspector]
	public int m_InitialDelay;

	[HideInInspector]
	public int m_Range;

	public static bool GetIsTypeUpgradeWorkerSearch(ObjectType NewType)
	{
		if (NewType == ObjectType.UpgradeWorkerSearchCrude || NewType == ObjectType.UpgradeWorkerSearchGood || NewType == ObjectType.UpgradeWorkerSearchSuper)
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
		m_Type = Type.WorkerSearch;
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_InitialDelay = VariableManager.Instance.GetVariableAsInt(m_TypeIdentifier, "InitialDelay");
		m_Range = VariableManager.Instance.GetVariableAsInt(m_TypeIdentifier, "Range");
	}
}
