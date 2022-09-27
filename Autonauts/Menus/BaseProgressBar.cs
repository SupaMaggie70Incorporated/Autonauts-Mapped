using UnityEngine;
using UnityEngine.UI;

public class BaseProgressBar : BaseGadget
{
	private Slider m_Slider;

	private Image m_Fill;

	private void CheckSlider()
	{
		if (m_Slider == null)
		{
			m_Slider = GetComponent<Slider>();
			m_Fill = m_Slider.transform.Find("Fill Area").Find("Fill").GetComponent<Image>();
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

	public void HideSlider()
	{
		CheckSlider();
		m_Slider.transform.Find("Background").gameObject.SetActive(value: false);
		m_Slider.transform.Find("Border").gameObject.SetActive(value: false);
		m_Fill.gameObject.SetActive(value: false);
	}

	public virtual void SetFillColour(Color NewColour)
	{
		CheckSlider();
		m_Fill.color = NewColour;
	}
}
