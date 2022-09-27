using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class ErrorMessage : MonoBehaviour
{
	public static ErrorMessage Instance;

	private static bool m_ShowError = true;

	private bool m_Active;

	private GameObject m_Title;

	private BaseText m_Message;

	private Text m_Version;

	private BaseToggle m_ShowToggle;

	private static string m_Stack;

	private void Awake()
	{
		Instance = this;
		Application.logMessageReceived += HandleLog;
		m_Active = false;
		m_Title = base.transform.Find("TitleBar").gameObject;
		m_Title.SetActive(value: false);
		BaseButton component = m_Title.transform.Find("StandardAcceptButton").GetComponent<StandardAcceptButton>();
		component.SetAction(OnAcceptClick, component);
		m_Message = base.transform.Find("ErrorMessage").GetComponent<BaseText>();
		m_Message.SetActive(Active: false);
		string text = SaveLoadManager.GetVersion();
		if ((bool)ModManager.Instance)
		{
			int count = ModManager.Instance.CurrentMods.Count;
			if (count > 0)
			{
				text = text + " - " + count + " Mods installed.\n";
			}
		}
		m_Version = base.transform.Find("Version").GetComponent<Text>();
		m_Version.text = text;
		m_Version.enabled = false;
		m_ShowToggle = base.transform.Find("ShowToggle").GetComponent<BaseToggle>();
		m_ShowToggle.SetStartOn(On: true);
		m_ShowToggle.SetActive(Active: false);
		m_ShowToggle.SetAction(OnShowClick, m_ShowToggle);
	}

	private void OnDestroy()
	{
		Application.logMessageReceived -= HandleLog;
	}

	public void OnAcceptClick(BaseGadget NewGadget)
	{
		Clear();
	}

	public void OnShowClick(BaseGadget NewGadget)
	{
		m_ShowError = m_ShowToggle.GetOn();
	}

	public void SetMessage(string Message)
	{
		if (m_ShowError)
		{
			m_Message.gameObject.SetActive(value: true);
			m_Message.SetActive(Active: true);
			m_Message.SetText(Message);
			m_Active = true;
			m_Version.enabled = true;
			m_Title.SetActive(value: true);
			m_ShowToggle.SetActive(Active: true);
		}
	}

	private void DoLog(string logString, string stackTrace, LogType type)
	{
		if (!m_Active && type != LogType.Log && type != LogType.Warning && !logString.Contains("<RI.Hid>"))
		{
			if (stackTrace == "")
			{
				stackTrace = m_Stack;
			}
			SetMessage(logString + "\n" + stackTrace);
		}
	}

	private void HandleLog(string logString, string stackTrace, LogType type)
	{
		if (!Application.isEditor)
		{
			DoLog(logString, stackTrace, type);
		}
	}

	public void DebugMessage(string logString)
	{
		SetMessage(logString);
	}

	public void Clear()
	{
		m_Message.SetActive(Active: false);
		m_Version.enabled = false;
		m_Active = false;
		m_Title.SetActive(value: false);
		m_ShowToggle.SetActive(Active: false);
	}

	public void HidePanel(bool Hide)
	{
		if (m_Active)
		{
			m_Title.SetActive(!Hide);
		}
	}

	public static void LogError(string Error)
	{
		m_Stack = new StackTrace().ToString();
		UnityEngine.Debug.LogError(Error);
	}
}
