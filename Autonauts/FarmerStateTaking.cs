public class FarmerStateTaking : FarmerStateBase
{
	private bool m_UsingBucket;

	public override bool GetIsAdjacentTile(TileCoord TargetTile, BaseClass Object)
	{
		if ((bool)Object && (bool)Object.GetComponent<Building>())
		{
			return false;
		}
		return true;
	}

	public override void StartState()
	{
		base.StartState();
		m_UsingBucket = false;
		m_Farmer.m_SpawnEnd = false;
		if (m_Farmer.m_FarmerAction.m_CurrentInfo != null)
		{
			m_Farmer.m_FarmerAction.DoTake();
			Holdable topObject = m_Farmer.m_FarmerCarry.GetTopObject();
			if ((bool)topObject && (bool)topObject.GetComponent<ToolFillable>())
			{
				m_UsingBucket = true;
			}
		}
	}

	public override void EndState()
	{
		base.EndState();
		BaseClass targetObject = GetTargetObject();
		if ((bool)targetObject)
		{
			if (m_Farmer.m_TypeIdentifier == ObjectType.FarmerPlayer && targetObject.m_TypeIdentifier == ObjectType.Worker)
			{
				QuestManager.Instance.AddEvent(QuestEvent.Type.TakeBotAnything, Bot: false, 0, m_Farmer);
			}
			if (Storage.GetIsTypeStorage(targetObject.m_TypeIdentifier))
			{
				QuestManager.Instance.AddEvent(QuestEvent.Type.Take, Bot: false, targetObject.GetComponent<Storage>().m_ObjectType, m_Farmer);
			}
		}
	}

	public override void SpawnEnd(BaseClass NewObject)
	{
		m_Farmer.m_FarmerCarry.TryAddCarry(NewObject.GetComponent<Holdable>());
		AddAnimationManager.Instance.Add(m_Farmer, Adding: true);
	}

	public override void UpdateState()
	{
		base.UpdateState();
		Actionable targetObject = GetTargetObject();
		if ((!m_UsingBucket && m_Farmer.m_SpawnEnd) || (m_UsingBucket && m_Farmer.m_StateTimer > 0.125f * m_GeneralStateScale) || (targetObject != null && targetObject.m_TypeIdentifier == ObjectType.FolkSeedPod))
		{
			DoEndAction();
		}
	}
}
