using UnityEngine;

public class GameStateSettings : GameStateBase
{
	[HideInInspector]
	public MenuSettings m_Menu;

	protected new void Awake()
	{
		base.Awake();
		Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Settings/MenuSettings", typeof(GameObject));
		m_Menu = Object.Instantiate(original, menusRootTransform).GetComponent<MenuSettings>();
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

	public override void UpdateState()
	{
		if (MyInputManager.m_Rewired.GetButtonDown("Quit"))
		{
			m_Menu.GetComponent<MenuSettings>().CancelChanges();
			AudioManager.Instance.StartEvent("UIOptionCancelled");
			GameStateManager.Instance.PopState();
		}
	}
}
