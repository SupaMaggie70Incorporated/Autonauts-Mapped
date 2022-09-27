using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateText : MonoBehaviour
{
	private int m_TextIndex;

	private BaseText m_Text;

	private SettingsManager.Language m_Language;

	private List<string> m_Strings;

	private List<string> m_IDs;

	private int m_NumText = 100;

	private List<BaseText> m_Texts;

	private void Load()
	{
		TextManager.Instance.m_Language = m_Language;
		TextManager.Instance.Load();
		Debug.Log(TextManager.Instance.Get("StartDenki2"));
		if (m_Texts != null)
		{
			foreach (BaseText text2 in m_Texts)
			{
				Object.Destroy(text2.gameObject);
			}
		}
		string text = "BaseText";
		text = ((TextManager.Instance.m_Language == SettingsManager.Language.ChineseSimplified) ? (text + "Chinese") : ((TextManager.Instance.m_Language == SettingsManager.Language.JapaneseKana) ? (text + "Japanese") : ((TextManager.Instance.m_Language != SettingsManager.Language.Korean) ? (text + "Others") : (text + "Korean"))));
		m_Text = GameObject.Find(text).GetComponent<BaseText>();
		m_Text.CheckText();
		m_Text.m_Text.font.atlasPopulationMode = AtlasPopulationMode.Dynamic;
		m_Texts = new List<BaseText>();
		for (int i = 0; i < m_NumText; i++)
		{
			BaseText item = Object.Instantiate(m_Text, m_Text.transform.parent);
			m_Texts.Add(item);
		}
		m_Strings = new List<string>();
		m_IDs = new List<string>();
		foreach (KeyValuePair<string, string> @string in TextManager.Instance.m_Strings)
		{
			m_Strings.Add(@string.Value);
			m_IDs.Add(@string.Key);
		}
		m_TextIndex = 0;
	}

	private void Awake()
	{
		Object.Instantiate((GameObject)Resources.Load("Prefabs/Managers/TextManager", typeof(GameObject)), new Vector3(0f, 0f, 0f), Quaternion.identity, null);
		Load();
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (m_Language == SettingsManager.Language.Total)
		{
			return;
		}
		for (int i = 0; i < m_NumText; i++)
		{
			string text = m_Strings[m_TextIndex];
			m_Texts[i].SetText(text);
			m_TextIndex++;
			if (m_TextIndex == m_Strings.Count)
			{
				break;
			}
		}
		if (m_TextIndex == m_Strings.Count)
		{
			m_Text.m_Text.font.atlasPopulationMode = AtlasPopulationMode.Static;
			m_Language++;
			if (m_Language != SettingsManager.Language.Total)
			{
				Load();
			}
		}
	}
}
