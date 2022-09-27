using UnityEngine.UI;

public class BaseMeter : BaseGadget
{
	private Slider m_Slider;

	protected new void Awake()
	{
		m_Slider = GetComponent<Slider>();
	}

	public float GetValue()
	{
		return m_Slider.value;
	}

	public void SetValue(float Value)
	{
		m_Slider.value = Value;
	}
}
