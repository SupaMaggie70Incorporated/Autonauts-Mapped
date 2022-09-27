public class FarmerStateDropping : FarmerStateBase
{
	public override void StartState()
	{
		base.StartState();
		AddAnimationManager.Instance.Add(m_Farmer, Adding: true);
		if (m_Farmer.m_FarmerCarry.GetIsCarryingSomething())
		{
			AudioManager.Instance.StartEvent("FarmerDrop", m_Farmer);
		}
	}

	public override void EndState()
	{
		base.EndState();
		if (GetHeldObject() != null)
		{
			Holdable heldObject = GetHeldObject();
			ModManager.Instance.CheckCustomCallback(ModManager.CallbackTypes.HoldableDroppedOnGround, heldObject.m_TypeIdentifier, heldObject.m_TileCoord, heldObject.m_UniqueID, m_Farmer.m_UniqueID);
		}
		if (m_Farmer.m_TypeIdentifier == ObjectType.FarmerPlayer && m_Farmer.m_FarmerCarry.GetLastObjectType() == ObjectType.Worker)
		{
			GameStateBase currentState = GameStateManager.Instance.GetCurrentState();
			if (currentState.m_BaseState == GameStateManager.State.Normal && (bool)m_Farmer.m_FarmerCarry.m_LastObject && currentState.GetComponent<GameStateNormal>().m_SelectedWorkers.Count == 0)
			{
				Worker component = m_Farmer.m_FarmerCarry.m_LastObject.GetComponent<Worker>();
				currentState.GetComponent<GameStateNormal>().AddSelectedWorker(component);
			}
		}
		QuestManager.Instance.AddEvent(QuestEvent.Type.DropAnything, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
	}

	public override void UpdateState()
	{
		base.UpdateState();
		if (m_Farmer.m_StateTimer > 0.125f * m_GeneralStateScale)
		{
			m_Farmer.m_FarmerAction.DoDrop();
			DoEndAction();
		}
	}
}
