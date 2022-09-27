public class FarmerStateEmptying : FarmerStateBase
{
	private TileCoord m_EmptyTile;

	private int m_Count;

	public override void StartState()
	{
		base.StartState();
		m_Farmer.StartAnimation("FarmerFillableEmpty");
		m_Count = 0;
	}

	public override void EndState()
	{
		base.EndState();
	}

	public override void DoAnimationAction()
	{
		base.DoAnimationAction();
		m_Count++;
		if (m_Count == 2)
		{
			DoEndAction();
		}
	}
}
