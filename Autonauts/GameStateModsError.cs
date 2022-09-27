using UnityEngine;

public class GameStateModsError : GameStateBase
{
	[HideInInspector]
	private ModsErrorPanel m_Menu;

	protected new void Awake()
	{
		base.Awake();
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Mods/ModsErrorPanel", typeof(GameObject));
		m_Menu = Object.Instantiate(original, HudManager.Instance.m_MenusRootTransform).GetComponent<ModsErrorPanel>();
	}

	protected new void OnDestroy()
	{
		base.OnDestroy();
		Object.Destroy(m_Menu.gameObject);
	}

	public override void Pushed(GameStateManager.State NewState)
	{
		base.Pushed(NewState);
		m_Menu.gameObject.SetActive(value: false);
	}

	public override void Popped(GameStateManager.State NewState)
	{
		base.Popped(NewState);
		m_Menu.gameObject.SetActive(value: true);
	}

	public void SetCurrentError()
	{
		m_Menu.SetCurrentError();
	}

	public override void UpdateState()
	{
		if (MyInputManager.m_Rewired.GetButtonDown("Quit"))
		{
			AudioManager.Instance.StartEvent("UIOptionCancelled");
			GameStateManager.Instance.PopState();
		}
	}
}
