public class FarmerStateModAction : FarmerStateBase
{
	private float m_Timer;

	private float AnimationTime;

	public override void StartState()
	{
		base.StartState();
		ObjectType typeIdentifier = GetTargetObject().m_TypeIdentifier;
		Tile.TileType tileType = TileManager.Instance.GetTile(m_Farmer.m_GoToTilePosition).m_TileType;
		ObjectType typeIdentifier2 = GetHeldObject().m_TypeIdentifier;
		if (ModManager.Instance.ModToolClass.CustomToolInfo.ContainsKey(typeIdentifier2))
		{
			ModTool.ModToolInfo modToolInfo = ModManager.Instance.ModToolClass.CustomToolInfo[typeIdentifier2];
			if ((modToolInfo.ObjectsToUseOn.Count > 0 && modToolInfo.ObjectsToUseOn.Contains(typeIdentifier)) || (modToolInfo.TilesToUseOn.Count > 0 && modToolInfo.TilesToUseOn.Contains(tileType)))
			{
				AnimationTime = modToolInfo.AnimationDuration;
			}
		}
		m_Timer = 0f;
		m_Farmer.StartAnimation("FarmerJumpTurf");
	}

	public override void UpdateState()
	{
		base.UpdateState();
		m_Timer += TimeManager.Instance.m_NormalDelta;
		if (m_Timer > AnimationTime)
		{
			DoEndAction();
			DegradeTool();
		}
	}
}
