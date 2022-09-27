using System.Text.RegularExpressions;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public class ModDebug
{
	public void Log(params object[] args)
	{
		Script lastCalledScript = ModManager.Instance.GetLastCalledScript();
		if (lastCalledScript != null && Regex.IsMatch(lastCalledScript.Globals["_ModDisplayName"].ToString(), "\\D"))
		{
			string text = "";
			DynValue[] array = new DynValue[args.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = DynValue.FromObject(lastCalledScript, args[i]);
				text += array[i].ToPrintString();
			}
			ModManager.Instance.WriteModDebug(text);
		}
	}

	public bool IsDevMode()
	{
		if (!SettingsManager.Instance)
		{
			return false;
		}
		return SettingsManager.Instance.m_DevMode;
	}

	public void ClearLog()
	{
		Mod lastCalledMod = ModManager.Instance.GetLastCalledMod();
		if (lastCalledMod != null && lastCalledMod.IsLocal)
		{
			ModManager.Instance.ClearLog(lastCalledMod.Name + "_DebugLog");
		}
	}
}
