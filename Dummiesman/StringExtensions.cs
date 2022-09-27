namespace Dummiesman;

public static class StringExtensions
{
	public static string Clean(this string str)
	{
		string text = str.Replace('\t', ' ');
		while (text.Contains("  "))
		{
			text = text.Replace("  ", " ");
		}
		return text.Trim();
	}
}
