public class FarmerStateUpgrading : FarmerStateBase
{
	protected int m_ActionCount;

	protected int m_NumActionCounts;

	public override bool GetIsAdjacentTile(TileCoord TargetTile, BaseClass Object)
	{
		return true;
	}

	public override void StartState()
	{
		base.StartState();
		m_ActionCount = 0;
		m_NumActionCounts = 5;
		m_ActionInfo.m_EndAction = null;
		m_Farmer.StartAnimation("FarmerUpgrading");
		if (!GetTargetObject())
		{
			return;
		}
		Worker component = GetTargetObject().GetComponent<Worker>();
		if (m_Farmer.m_TypeIdentifier == ObjectType.FarmerPlayer)
		{
			GameStateNormal component2 = GameStateManager.Instance.GetCurrentState().GetComponent<GameStateNormal>();
			if ((bool)component2)
			{
				component2.RemoveSelectedWorker(component);
			}
		}
	}

	public override void EndState()
	{
		base.EndState();
		if ((bool)GetTargetObject())
		{
			Worker component = GetTargetObject().GetComponent<Worker>();
			if ((bool)component)
			{
				component.SetState(Farmer.State.None);
			}
			AudioManager.Instance.StartEvent("WorkerUpgradeAdded", component);
		}
	}

	public override void DoAnimationAction()
	{
		base.DoAnimationAction();
		m_ActionCount++;
		if (m_ActionCount != m_NumActionCounts)
		{
			return;
		}
		m_Farmer.SetBaggedObject(null);
		m_Farmer.SetBaggedTile(new TileCoord(0, 0));
		if ((bool)GetTargetObject())
		{
			Worker component = GetTargetObject().GetComponent<Worker>();
			if ((bool)component)
			{
				component.SetState(Farmer.State.None);
			}
		}
		DoEndAction();
	}
}
