public class StorageFertiliserRollover : StorageRollover
{
	private StandardProgressBar m_Slider;

	protected new void Awake()
	{
		base.Awake();
		m_Slider = m_Panel.transform.Find("ProgressBar").GetComponent<StandardProgressBar>();
	}

	protected override void UpdateTarget()
	{
		base.UpdateTarget();
		m_Slider.SetValue(m_Target.GetComponent<StorageFertiliser>().m_Fraction);
	}
}
