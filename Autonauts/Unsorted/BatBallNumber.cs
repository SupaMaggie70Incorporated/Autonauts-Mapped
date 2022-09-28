using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatBallNumber : MonoBehaviour
{
	private List<Image> m_Digits;

	private int m_Value;

	public void SetDigits(int Digits)
	{
		Image component = base.transform.Find("Digit").GetComponent<Image>();
		m_Digits = new List<Image>();
		for (int i = 0; i < Digits; i++)
		{
			Image image = Object.Instantiate(component, base.transform);
			image.GetComponent<RectTransform>().anchoredPosition = new Vector2(i * 46, 0f);
			m_Digits.Add(image);
		}
		component.gameObject.SetActive(value: false);
		SetValue(0);
	}

	public void SetValue(int Value)
	{
		m_Value = Value;
		for (int num = m_Digits.Count - 1; num >= 0; num--)
		{
			int num2 = Value % 10;
			Value /= 10;
			string text = "Arcade/BatBall" + num2;
			Sprite sprite = (Sprite)Resources.Load("Textures/Hud/" + text, typeof(Sprite));
			m_Digits[num].sprite = sprite;
		}
	}

	public int GetValue()
	{
		return m_Value;
	}
}
