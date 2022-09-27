using UnityEngine.UI;

public class BaseSlider : BaseGadget
{
	private Slider m_Slider;

	private float m_StartValue;

	protected new void Awake()
	{
		base.Awake();
	}

	public void OnValueChanged()
	{
		DoAction();
	}

	private void CheckSlider()
	{
		if (m_Slider == null)
		{
			m_Slider = GetComponent<Slider>();
		}
	}

	public float GetValue()
	{
		CheckSlider();
		return m_Slider.value;
	}

	public void SetValue(float Value)
	{
		CheckSlider();
		m_Slider.value = Value;
	}

	public virtual void SetStartValue(float Value)
	{
		m_StartValue = Value;
		SetValue(Value);
	}

	public float GetStartValue()
	{
		return m_StartValue;
	}

	public override void SetInteractable(bool Interactable)
	{
		base.SetInteractable(Interactable);
		m_Slider.interactable = Interactable;
	}
}
