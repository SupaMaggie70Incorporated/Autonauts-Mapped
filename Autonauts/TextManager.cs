using System.Collections.Generic;
using System.IO;
using Rewired;
using UnityEngine;

public class TextManager : MonoBehaviour
{
	public static TextManager Instance;

	public Dictionary<string, string> m_Strings;

	public SettingsManager.Language m_Language;

	private void Awake()
	{
		base.transform.SetParent(null);
		if (Instance == null)
		{
			Instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
		}
		else if (this != Instance)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		Load();
	}

	private string ReplaceCarriageReturn(string InputString, int Line)
	{
		bool flag;
		do
		{
			flag = true;
			int num = InputString.IndexOf("\\", 0);
			if (num == -1)
			{
				flag = false;
				continue;
			}
			if (InputString.Length <= num + 2)
			{
				return InputString;
			}
			InputString = InputString.Substring(0, num) + "\n" + InputString.Substring(num + 2, InputString.Length - num - 2);
		}
		while (flag);
		return InputString;
	}

	private string GetLanguageName()
	{
		if ((bool)SettingsManager.Instance)
		{
			if (SettingsManager.Instance.m_Language == SettingsManager.Language.Total)
			{
				SettingsManager.Language language = SettingsManager.Language.English;
				if (Application.systemLanguage == SystemLanguage.German)
				{
					language = SettingsManager.Language.German;
				}
				if (Application.systemLanguage == SystemLanguage.French)
				{
					language = SettingsManager.Language.French;
				}
				if (Application.systemLanguage == SystemLanguage.Russian)
				{
					language = SettingsManager.Language.Russian;
				}
				if (Application.systemLanguage == SystemLanguage.ChineseSimplified || Application.systemLanguage == SystemLanguage.ChineseTraditional || Application.systemLanguage == SystemLanguage.Chinese)
				{
					language = SettingsManager.Language.ChineseSimplified;
				}
				if (Application.systemLanguage == SystemLanguage.Japanese)
				{
					language = SettingsManager.Language.JapaneseKana;
				}
				if (Application.systemLanguage == SystemLanguage.Portuguese)
				{
					language = SettingsManager.Language.BrazilianPortugeuse;
				}
				if (Application.systemLanguage == SystemLanguage.Spanish)
				{
					language = SettingsManager.Language.Spanish;
				}
				if (Application.systemLanguage == SystemLanguage.Polish)
				{
					language = SettingsManager.Language.Polish;
				}
				if (Application.systemLanguage == SystemLanguage.Korean)
				{
					language = SettingsManager.Language.Korean;
				}
				if (Application.systemLanguage == SystemLanguage.Turkish)
				{
					language = SettingsManager.Language.Turkish;
				}
				SettingsManager.Instance.SetLanguage(language);
				SettingsManager.Instance.Save();
			}
			return "AllText" + SettingsManager.Instance.m_Language;
		}
		return "AllText" + m_Language;
	}

	public void Load()
	{
		string languageName = GetLanguageName();
		if (File.Exists(Application.persistentDataPath + "/" + languageName + ".csv"))
		{
			TestLoad();
			return;
		}
		languageName = "Data/" + languageName;
		TextAsset textAsset = (TextAsset)Resources.Load(languageName, typeof(TextAsset));
		Load(textAsset.text);
	}

	public void TestLoad()
	{
		string languageName = GetLanguageName();
		string path = Application.persistentDataPath + "/" + languageName + ".csv";
		if (File.Exists(path))
		{
			string @string = File.ReadAllText(path);
			Load(@string);
		}
	}

	private void Load(string String)
	{
		m_Strings = new Dictionary<string, string>();
		int num = 0;
		int num2 = 0;
		while (num < String.Length)
		{
			string text = "";
			string text2 = "";
			int num3 = String.IndexOf("\"", num);
			if (num3 == -1)
			{
				break;
			}
			text2 = String.Substring(num, num3 - num);
			text2 = ReplaceCarriageReturn(text2, num2);
			num2 += text2.Split('\n').Length - 1;
			num = num3 + 1;
			int num4 = String.IndexOf("\"", num);
			text = String.Substring(num, num4 - num);
			if (m_Strings.ContainsKey(text))
			{
				ErrorMessage.LogError("Text already contains \"" + text + "\" : line " + num2);
			}
			num = num4 + 1;
			num3 = String.IndexOf("\"", num);
			if (num3 == -1)
			{
				break;
			}
			text2 = String.Substring(num, num3 - num);
			text2 = ReplaceCarriageReturn(text2, num2);
			num2 += text2.Split('\n').Length - 1;
			num = num3 + 1;
			int num5 = 0;
			int startIndex = num;
			bool flag;
			do
			{
				flag = false;
				num4 = String.IndexOf("\"", startIndex);
				if (String.Substring(num4 + 1, 1) != ",")
				{
					flag = true;
					startIndex = num4 + 1;
				}
				num5++;
			}
			while (flag && num5 < 100);
			if (num5 == 100)
			{
				ErrorMessage.LogError("Something went wrong looking for a string : Line " + num2);
			}
			text2 = String.Substring(num, num4 - num);
			num = num4 + 1;
			text2 = ReplaceCarriageReturn(text2, num2);
			m_Strings.Add(text, text2);
		}
	}

	private string GetStringInsert(string OldString, string Val1 = "", string Val2 = "", string Val3 = "", string Val4 = "")
	{
		string text = "";
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		while ((num2 = OldString.IndexOf("%s", num)) != -1)
		{
			text += OldString.Substring(num, num2 - num);
			if (num3 == 0)
			{
				text += Val1;
			}
			if (num3 == 1)
			{
				text += Val2;
			}
			if (num3 == 2)
			{
				text += Val3;
			}
			if (num3 == 3)
			{
				text += Val4;
			}
			num3++;
			num = num2 + 2;
		}
		return text + OldString.Substring(num, OldString.Length - num);
	}

	private string GetAlphaNumeric(string OldString, int Index, out int NewIndex)
	{
		NewIndex = Index;
		while (NewIndex != OldString.Length && (char.IsLetter(OldString[NewIndex]) || char.IsNumber(OldString[NewIndex])))
		{
			NewIndex++;
		}
		return OldString.Substring(Index, NewIndex - Index);
	}

	public string GetFromAem(KeyCode NewKeyCode, ModifierKey NewModifier1)
	{
		string text = NewKeyCode.ToString();
		if (DoesExist("KeyCode" + text))
		{
			text = Get("KeyCode" + text);
		}
		if (NewModifier1 != 0)
		{
			string text2 = NewModifier1.ToString();
			if (text2 == "Control")
			{
				text2 = "CTRL";
			}
			if (DoesExist("KeyCode" + text2))
			{
				text2 = Get("KeyCode" + text2);
			}
			if (text2 != "")
			{
				text = text2 + "+" + text;
			}
		}
		return text;
	}

	private string GetKeyBinding(string KeyIdentifier)
	{
		foreach (KeyValuePair<ActionElementMap, InputBinding> controlsAction in MyInputManager.Instance.m_ControlsActions)
		{
			ActionElementMap key = controlsAction.Key;
			if (ReInput.mapping.GetAction(key.actionId).name == KeyIdentifier)
			{
				return GetFromAem(key.keyCode, key.modifierKey1);
			}
		}
		return "";
	}

	private string GetKeyBindings(string OldString)
	{
		string text = "";
		int num = 0;
		int num2 = 0;
		while ((num2 = OldString.IndexOf("%k", num)) != -1)
		{
			text += OldString.Substring(num, num2 - num);
			num2 += 2;
			string alphaNumeric = GetAlphaNumeric(OldString, num2, out num2);
			string keyBinding = GetKeyBinding(alphaNumeric);
			text = text + "[" + keyBinding + "]";
			num = num2;
		}
		return text + OldString.Substring(num, OldString.Length - num);
	}

	public string Get(string Tag, string Val1 = "", string Val2 = "", string Val3 = "", string Val4 = "")
	{
		string value = "*Missing* " + Tag;
		if (m_Strings.TryGetValue(Tag, out value))
		{
			if (value.IndexOf("%s", 0) != -1)
			{
				value = GetStringInsert(value, Val1, Val2, Val3, Val4);
			}
			if (value.IndexOf("%k", 0) != -1)
			{
				value = GetKeyBindings(value);
			}
		}
		else if (ModManager.Instance.FindModStringFromValue(Tag, out value))
		{
			if (DoesExist("M_" + value))
			{
				return Get("M_" + value, Val1, Val2, Val3, Val4);
			}
			if (value.IndexOf("%s", 0) != -1)
			{
				value = GetStringInsert(value, Val1, Val2, Val3, Val4);
			}
			if (value.IndexOf("%k", 0) != -1)
			{
				value = GetKeyBindings(value);
			}
		}
		else
		{
			value = "Error: " + Tag;
		}
		return value;
	}

	public bool DoesExist(string Tag)
	{
		return m_Strings.ContainsKey(Tag);
	}

	public void Set(string Tag, string Value)
	{
		if (DoesExist(Tag))
		{
			m_Strings[Tag] = Value;
		}
		else
		{
			m_Strings.Add(Tag, Value);
		}
	}

	private void Update()
	{
		if (SaveLoadManager.m_TestBuild && MyInputManager.m_Rewired.GetButtonDown("ReloadText"))
		{
			TestLoad();
		}
	}
}
