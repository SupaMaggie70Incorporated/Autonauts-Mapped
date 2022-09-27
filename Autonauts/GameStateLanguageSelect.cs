using UnityEngine;

public class GameStateLanguageSelect : GameStateBase
{
	private GameObject m_Menu;

	protected new void Awake()
	{
		base.Awake();
		Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Menus/LanguageSelect", typeof(GameObject));
		m_Menu = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, menusRootTransform);
	}

	protected new void OnDestroy()
	{
		base.OnDestroy();
		Object.Destroy(m_Menu.gameObject);
	}

	private void Update()
	{
		if (MyInputManager.m_Rewired.GetButtonDown("Quit"))
		{
			GameStateMainMenu.GoToMainMenu();
		}
	}
}
