using UnityEngine;

public class UpgradePlayerWhistle : Upgrade
{
	[HideInInspector]
	public string[] m_WhistleSounds;

	[HideInInspector]
	public static bool GetIsTypeUpgradePlayerWhistle(ObjectType NewType)
	{
		if (NewType == ObjectType.UpgradePlayerWhistleCrude || NewType == ObjectType.UpgradePlayerWhistleGood || NewType == ObjectType.UpgradePlayerWhistleSuper)
		{
			return true;
		}
		return false;
	}

	protected new void Awake()
	{
		base.Awake();
		m_Target = Target.Player;
		m_Type = Type.PlayerWhistle;
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_WhistleSounds = new string[4];
		m_WhistleSounds[0] = VariableManager.Instance.GetVariableAsString(m_TypeIdentifier, "CallSound");
		m_WhistleSounds[1] = VariableManager.Instance.GetVariableAsString(m_TypeIdentifier, "CancelSound");
		m_WhistleSounds[2] = VariableManager.Instance.GetVariableAsString(m_TypeIdentifier, "DropAllSound");
		m_WhistleSounds[3] = VariableManager.Instance.GetVariableAsString(m_TypeIdentifier, "ToMeSound");
	}
}
