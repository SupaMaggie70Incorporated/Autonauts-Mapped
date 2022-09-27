public class WorkerDriveInfo
{
	public string m_MoveSoundName;

	public string m_MoveStoneSoundName;

	public string m_MoveClaySoundName;

	public float m_SpeedScale;

	public int m_MoveInitialDelay;

	public float m_RechargeDelay;

	public float m_Energy;

	public float m_Scale;

	public WorkerDriveInfo()
	{
	}

	public WorkerDriveInfo(string MoveSoundName, string MoveStoneSoundName, string MoveClaySoundName, float SpeedScale, int MoveInitialDelay, float RechargeDelay, float Energy)
	{
		m_MoveSoundName = MoveSoundName;
		m_MoveStoneSoundName = MoveStoneSoundName;
		m_MoveClaySoundName = MoveClaySoundName;
		m_SpeedScale = SpeedScale;
		m_MoveInitialDelay = MoveInitialDelay;
		m_RechargeDelay = RechargeDelay;
		m_Energy = Energy;
		m_Scale = 1f;
	}
}
