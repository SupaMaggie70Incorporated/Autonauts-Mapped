using UnityEngine;

public class LevelHeart : BaseImage
{
	private BaseText m_Value;

	private Vector2 m_Min;

	private Vector2 m_Max;

	protected new void Awake()
	{
		base.Awake();
	}

	private void CheckValue()
	{
		if (m_Value == null)
		{
			m_Value = base.transform.Find("HeartValue").GetComponent<BaseText>();
			m_Min = m_Value.GetComponent<RectTransform>().offsetMin;
			m_Max = m_Value.GetComponent<RectTransform>().offsetMax;
		}
	}

	public void SetValue(int Value)
	{
		CheckValue();
		m_Value.SetText((Value + 1).ToString());
	}

	private void Update()
	{
		Vector2 sizeDelta = GetComponent<RectTransform>().sizeDelta;
		m_Value.GetComponent<RectTransform>().offsetMin = m_Min * (sizeDelta.x / 50f);
		m_Value.GetComponent<RectTransform>().offsetMax = m_Max * (sizeDelta.x / 50f);
	}
}
