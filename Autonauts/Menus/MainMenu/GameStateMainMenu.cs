using UnityEngine;

public class GameStateMainMenu : GameStateBase
{
	[HideInInspector]
	public GameObject m_Menu;

	public static void GoToMainMenu()
	{
		if (!GameStateStart.m_Seen)
		{
			GameStateManager.Instance.SetState(GameStateManager.State.Start);
		}
		else
		{
			GameStateManager.Instance.SetState(GameStateManager.State.MainMenu);
		}
	}

	protected new void Awake()
	{
		base.Awake();
		Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Menus/MainMenu", typeof(GameObject));
		m_Menu = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, menusRootTransform);
		m_Menu.transform.localPosition = default(Vector3);
	}

	protected new void OnDestroy()
	{
		base.OnDestroy();
		Object.Destroy(m_Menu.gameObject);
	}

	public override void Pushed(GameStateManager.State NewState)
	{
		base.Pushed(NewState);
		m_Menu.GetComponent<MainMenu>().Pushed();
	}

	public override void Popped(GameStateManager.State NewState)
	{
		base.Popped(NewState);
		m_Menu.GetComponent<MainMenu>().Popped();
	}

	public override void UpdateState()
	{
	}
}
