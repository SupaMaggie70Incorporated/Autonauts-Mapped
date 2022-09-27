using System;
using MoonSharp.Interpreter;
using UnityEngine;

[MoonSharpUserData]
public class ModText
{
	public string GetLanguage()
	{
		try
		{
			if (PlayerPrefs.HasKey("Language"))
			{
				return ((SettingsManager.Language)PlayerPrefs.GetInt("Language")).ToString();
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModBase.GetLanguage Error: " + ex.ToString());
		}
		return "";
	}

	public string GetText(string TextID, bool ModText = true)
	{
		string text = "";
		if (ModText)
		{
			text = "M_";
		}
		if ((bool)TextManager.Instance && TextManager.Instance.DoesExist(text + TextID))
		{
			return TextManager.Instance.Get(text + TextID);
		}
		return "";
	}

	public void SetText(string TextID, string Text, bool ModText = true)
	{
		string text = "";
		if (ModText)
		{
			text = "M_";
		}
		if ((bool)TextManager.Instance)
		{
			TextManager.Instance.Set(text + TextID, Text);
		}
	}

	public void SetDescription(string UniqueName, string Description)
	{
		if ((bool)TextManager.Instance)
		{
			TextManager.Instance.Set("D_" + UniqueName, Description);
		}
	}
}
