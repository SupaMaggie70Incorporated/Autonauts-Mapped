using UnityEngine;
using UnityEngine.UI;

public class CursorClick : MonoBehaviour
{
	private float m_Timer;

	private Color m_NewColour;

	private void Awake()
	{
		m_Timer = 0f;
	}

	public void SetColor(Color NewColor)
	{
		m_NewColour = NewColor;
		UpdateColour(1f);
	}

	private void UpdateColour(float Percent)
	{
		Color newColour = m_NewColour;
		newColour.a = 1f - Percent;
		GetComponent<Image>().color = newColour;
	}

	private void Update()
	{
		m_Timer += Time.deltaTime;
		float num = 0.5f;
		float percent = m_Timer / num;
		UpdateColour(percent);
		if (m_Timer > num)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
