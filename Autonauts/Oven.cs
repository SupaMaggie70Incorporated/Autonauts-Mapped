public class Oven : OvenCrude
{
	public override void Restart()
	{
		base.Restart();
		m_Capacity = 240f;
	}
}
