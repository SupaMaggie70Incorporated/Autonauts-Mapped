public class Snow : Rain
{
	protected new void Awake()
	{
		base.Awake();
		m_EmitFrequency = 800;
	}

	private void OnParticleTrigger()
	{
	}
}
