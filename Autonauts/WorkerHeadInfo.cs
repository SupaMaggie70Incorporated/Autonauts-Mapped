public class WorkerHeadInfo
{
	public int m_MaxInstructions;

	public string m_SerialPrefix;

	public int m_FindNearestRange;

	public int m_FindNearestDelay;

	public float m_Scale;

	public WorkerHeadInfo()
	{
	}

	public WorkerHeadInfo(int MaxInstructions, string SerialPrefix, int FindNearestRange, int FindNearestDelay)
	{
		m_MaxInstructions = MaxInstructions;
		m_SerialPrefix = SerialPrefix;
		m_FindNearestRange = FindNearestRange;
		m_FindNearestDelay = FindNearestDelay;
		m_Scale = 1f;
	}
}
