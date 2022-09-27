using UnityEngine;

public class StandardSlider : BaseGadget
{
	private BaseText m_TitleText;

	private BaseText m_ValueText;

	private BaseSlider m_Slider;

	private float MinValue;

	private float MaxValue = 100f;

	protected new void Awake()
	{
		base.Awake();
		CheckGadgets();
	}

	private void CheckGadgets()
	{
		if (m_TitleText == null)
		{
			m_TitleText = base.transform.Find("Text").GetComponent<BaseText>();
			m_ValueText = base.transform.Find("ValueText").GetComponent<BaseText>();
			m_Slider = base.transform.Find("BaseSlider").GetComponent<BaseSlider>();
			m_Slider.SetAction(OnValueChanged, null);
		}
	}

	public override void SetInteractable(bool Interactable)
	{
		base.SetInteractable(Interactable);
		CheckGadgets();
		m_ValueText.SetInteractable(Interactable);
		m_TitleText.SetInteractable(Interactable);
		m_Slider.SetInteractable(Interactable);
		float alpha = 1f;
		if (!Interactable)
		{
			alpha = 0.5f;
		}
		GetComponent<CanvasGroup>().alpha = alpha;
	}

	private void ValueChanged()
	{
		CheckGadgets();
		int num = (int)((MaxValue - MinValue) * m_Slider.GetValue() + MinValue);
		if (num == 0 && m_Slider.GetValue() != 0f)
		{
			num = 1;
		}
		m_ValueText.SetText(num.ToString());
	}

	public void SetStartValue(float Value)
	{
		CheckGadgets();
		m_Slider.SetStartValue(Value);
		ValueChanged();
	}

	public float GetStartValue()
	{
		return m_Slider.GetStartValue();
	}

	public void OnValueChanged(BaseGadget NewGadget)
	{
		ValueChanged();
		DoAction();
	}

	public float GetValue()
	{
		return m_Slider.GetValue();
	}

	public void SetValue(float Value)
	{
		m_Slider.SetValue(Value);
		ValueChanged();
	}

	public void SetMinMaxStart(float Value, float Min, float Max)
	{
		MinValue = Min;
		MaxValue = Max;
		CheckGadgets();
		float startValue = (Value - Min) / (Max - Min);
		m_Slider.SetStartValue(startValue);
		ValueChanged();
	}

	public float GetValueScaled()
	{
		return (MaxValue - MinValue) * m_Slider.GetValue() + MinValue;
	}
}
