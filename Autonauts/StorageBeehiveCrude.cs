public class StorageBeehiveCrude : StorageBeehive
{
	public override void Restart()
	{
		base.Restart();
		m_MaxBees = 2;
	}
}
