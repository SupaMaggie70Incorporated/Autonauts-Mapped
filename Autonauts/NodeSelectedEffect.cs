using UnityEngine;
using UnityEngine.UI;

public class NodeSelectedEffect : MonoBehaviour
{
	private static float m_Delay = 0.25f;

	private float m_Timer;

	private Image m_Image;

	private void Awake()
	{
		m_Image = GetComponent<Image>();
		UpdateColour();
		UpdateSize();
	}

	private void UpdateColour()
	{
		float a = 1f;
		float num = m_Delay * 0.75f;
		if (m_Timer > num)
		{
			a = 1f - (m_Timer - num) / (m_Delay - num);
		}
		Color color = m_Image.color;
		color.a = a;
		m_Image.color = color;
	}

	private void UpdateSize()
	{
		float num = m_Timer / m_Delay * 200f + 150f;
		GetComponent<RectTransform>().sizeDelta = new Vector2(num, num);
	}

	private void Update()
	{
		m_Timer += TimeManager.Instance.m_PauseDelta;
		if (m_Timer >= m_Delay)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		UpdateColour();
		UpdateSize();
	}
}
