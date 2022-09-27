using UnityEngine;
using UnityEngine.SceneManagement;

public class GeneralUtils
{
	public static bool m_InGame;

	public static void Init()
	{
		m_InGame = false;
		if (SceneManager.GetActiveScene().name == "Main")
		{
			m_InGame = true;
		}
	}

	public static string ConvertTimeToString(float Time)
	{
		int num = (int)Time;
		return string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(num / 3600 + ":", (num / 600 % 6).ToString()), (num / 60 % 10).ToString()), ":"), (num / 10 % 6).ToString()), (num % 10).ToString());
	}

	public static int IntParseFast(string value)
	{
		int num = 0;
		foreach (char c in value)
		{
			num = 10 * num + (c - 48);
		}
		return num;
	}

	public static Color ColorFromHex(int Colour)
	{
		byte r = (byte)((Colour >> 16) & 0xFF);
		byte g = (byte)((uint)(Colour >> 8) & 0xFFu);
		byte b = (byte)((uint)Colour & 0xFFu);
		return new Color32(r, g, b, byte.MaxValue);
	}

	public static string GetMapSeedString(int Seed)
	{
		string text = "";
		for (int i = 0; i < 9; i++)
		{
			text = Seed % 10 + text;
			Seed /= 10;
			if (i == 2 || i == 5)
			{
				text = "-" + text;
			}
		}
		return text;
	}

	public static Color GetIndicatorColour()
	{
		if (SettingsManager.Instance.GetIsSnowAvailable())
		{
			return ColorFromHex(16711850);
		}
		return new Color(1f, 1f, 1f, 1f);
	}

	public static string FormatBigInt(int Value)
	{
		int num = 0;
		int num2 = Value;
		while (num2 > 0 && num2 % 10 == 0)
		{
			num2 /= 10;
			num++;
		}
		if (Value >= 1000000)
		{
			if (num >= 6)
			{
				return Value / 1000000 + TextManager.Instance.Get("ResearchRolloverMillion");
			}
			switch (num)
			{
			case 5:
				return Value / 1000000 + "." + Value / 100000 % 10 + TextManager.Instance.Get("ResearchRolloverMillion");
			case 4:
				return Value / 1000000 + "." + Value / 100000 % 10 + Value / 10000 % 10 + TextManager.Instance.Get("ResearchRolloverMillion");
			}
		}
		if (Value >= 1000)
		{
			if (num >= 3)
			{
				return Value / 1000 + TextManager.Instance.Get("ResearchRolloverThousand");
			}
			if (num == 2)
			{
				return Value / 1000 + "." + Value / 100 % 10 + TextManager.Instance.Get("ResearchRolloverThousand");
			}
		}
		return Value.ToString();
	}
}
