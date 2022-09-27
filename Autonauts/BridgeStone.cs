public class BridgeStone : Bridge
{
	public override void RegisterClass()
	{
		base.RegisterClass();
		ModelManager.Instance.AddModel("Models/Buildings/Floors/BridgeCrossStone", ObjectType.BridgeStone);
		ModelManager.Instance.AddModel("Models/Buildings/Floors/BridgeEndStone", ObjectType.BridgeStone);
		ModelManager.Instance.AddModel("Models/Buildings/Floors/BridgeStone", ObjectType.BridgeStone);
	}

	public override void Restart()
	{
		base.Restart();
		m_ModelNormal = "BridgeStone";
		m_ModelEnd = "BridgeEndStone";
		m_ModelCross = "BridgeCrossStone";
	}
}
