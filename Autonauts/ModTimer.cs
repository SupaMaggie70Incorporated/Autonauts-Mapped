using System;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public class ModTimer
{
	public struct IntervalCallbackData
	{
		public Script OwnerScript;

		public DynValue CallbackFunction;

		public int IntervalMS;

		public bool Repeat;
	}

	public int SetCallback(DynValue Callback, int Milliseconds, bool Repeat = false)
	{
		if (Milliseconds < 50)
		{
			Milliseconds = 50;
		}
		try
		{
			int num = ModManager.Instance.IntervalCallbackSequence++;
			ModManager.Instance.IntervalCounters.Add(num, Milliseconds);
			IntervalCallbackData value = default(IntervalCallbackData);
			value.OwnerScript = ModManager.Instance.GetLastCalledScript();
			value.IntervalMS = Milliseconds;
			value.Repeat = Repeat;
			value.CallbackFunction = Callback;
			ModManager.Instance.IntervalCallbacks.Add(num, value);
			return num;
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModTimer.SetCallback Error: " + ex.ToString());
		}
		return -1;
	}

	public void DestroyCallback(int TimerID)
	{
		if (ModManager.Instance.IntervalCallbacks.TryGetValue(TimerID, out var value) && value.OwnerScript == ModManager.Instance.GetLastCalledScript())
		{
			ModManager.Instance.IntervalCounters.Remove(TimerID);
			ModManager.Instance.IntervalCallbacks.Remove(TimerID);
		}
	}

	public void DestroyCallback(string TimerID)
	{
	}

	public bool IsGameTimePassing()
	{
		if (GeneralUtils.m_InGame && (bool)TimeManager.Instance)
		{
			return TimeManager.Instance.m_NormalTimeEnabled;
		}
		return false;
	}
}
