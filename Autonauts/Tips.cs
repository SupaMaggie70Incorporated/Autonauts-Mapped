using System.Collections.Generic;
using UnityEngine;

public class Tips : AutopediaPage
{
	private enum Type
	{
		Tip12,
		Tip1,
		Tip4,
		Tip14,
		Tip8,
		Tip9,
		Tip10,
		Tip6,
		Tip7,
		Tip11,
		Tip2,
		Tip3,
		Tip13,
		Total
	}

	private BaseScrollView m_ScrollView;

	private List<BaseText> m_Texts;

	private List<BaseText> m_Dots;

	private void Awake()
	{
		m_ScrollView = base.transform.Find("BaseScrollView").GetComponent<BaseScrollView>();
		BaseText component = m_ScrollView.GetContent().transform.Find("DefaultTipText").GetComponent<BaseText>();
		component.SetActive(Active: false);
		BaseText component2 = m_ScrollView.GetContent().transform.Find("DefaultTipDot").GetComponent<BaseText>();
		component2.SetActive(Active: false);
		m_Texts = new List<BaseText>();
		m_Dots = new List<BaseText>();
		for (int i = 0; i < 13; i++)
		{
			BaseText baseText = Object.Instantiate(component, m_ScrollView.GetContent().transform);
			baseText.GetComponent<RectTransform>().offsetMin = new Vector2(10f, 0f);
			baseText.GetComponent<RectTransform>().offsetMax = new Vector2(-10f, 0f);
			Type type = (Type)i;
			string newText = "Tips" + type;
			baseText.SetTextFromID(newText);
			m_Texts.Add(baseText);
			BaseText baseText2 = Object.Instantiate(component2, m_ScrollView.GetContent().transform);
			baseText2.SetActive(Active: true);
			m_Dots.Add(baseText2);
		}
	}

	private void UpdateText()
	{
		float num = 15f;
		float num2 = 0f;
		foreach (BaseText text in m_Texts)
		{
			text.SetActive(Active: true);
			text.m_Text.ForceMeshUpdate(ignoreActiveState: true);
			num2 += text.GetPreferredHeight() + num;
		}
		m_ScrollView.SetScrollSize(num2);
		float num3 = -10f;
		for (int i = 0; i < m_Texts.Count; i++)
		{
			BaseText baseText = m_Texts[i];
			baseText.GetComponent<RectTransform>().offsetMin = new Vector2(35f, num3 - baseText.GetPreferredHeight());
			baseText.GetComponent<RectTransform>().offsetMax = new Vector2(-10f, num3);
			m_Dots[i].transform.localPosition = new Vector3(10f, num3);
			num3 -= baseText.GetPreferredHeight() + num;
		}
	}

	private void Update()
	{
		UpdateText();
	}
}
