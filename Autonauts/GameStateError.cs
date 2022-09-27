using UnityEngine;

public class GameStateError : GameStateBase
{
	public static GameStateError Instance;

	[HideInInspector]
	public Error m_Error;

	public Worker m_Bot;

	protected new void Awake()
	{
		Instance = this;
		base.Awake();
		Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Error", typeof(GameObject));
		m_Error = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, menusRootTransform).GetComponent<Error>();
		m_Error.GetComponent<RectTransform>().anchoredPosition = default(Vector2);
	}

	protected new void OnDestroy()
	{
		base.OnDestroy();
		Object.Destroy(m_Error.gameObject);
	}

	public void SetError(string Type, string Description)
	{
		m_Error.SetError(Type, Description);
	}

	public void SetText(string Type, string Description, Worker NewBot)
	{
		m_Bot = NewBot;
		m_Error.SetText(Type, Description);
	}

	public void StopBot()
	{
		if ((bool)m_Bot)
		{
			m_Bot.StopAllScripts();
		}
	}

	public override void UpdateState()
	{
		if ((bool)m_Bot && m_Bot.m_WorkerInterpreter.GetCurrentScript() == null)
		{
			m_Error.HideStopBot();
		}
		if (MyInputManager.m_Rewired.GetButtonDown("Quit"))
		{
			AudioManager.Instance.StartEvent("UICloseWindow");
			GameStateManager.Instance.PopState();
		}
	}
}
