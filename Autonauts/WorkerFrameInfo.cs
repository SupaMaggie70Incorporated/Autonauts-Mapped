public class WorkerFrameInfo
{
	public string m_WorkingSoundName;

	public int m_CarrySize;

	public int m_InventorySize;

	public int m_UpgradeSize;

	public float m_Scale;

	public WorkerFrameInfo()
	{
	}

	public WorkerFrameInfo(string WorkingSoundName, int CarrySize, int InventorySize, int UpgradeSize)
	{
		m_WorkingSoundName = WorkingSoundName;
		m_CarrySize = CarrySize;
		m_InventorySize = InventorySize;
		m_UpgradeSize = UpgradeSize;
		m_Scale = 1f;
	}
}
