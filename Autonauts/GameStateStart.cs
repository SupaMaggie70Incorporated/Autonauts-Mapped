using UnityEngine;

public class GameStateStart : GameStateBase
{
	public static bool m_Seen;

	[HideInInspector]
	public GameObject m_Menu;

	protected new void Awake()
	{
		base.Awake();
		Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Menus/Start", typeof(GameObject));
		m_Menu = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, menusRootTransform);
		m_Menu.GetComponent<RectTransform>().anchoredPosition = default(Vector2);
		m_Seen = true;
	}

	protected new void OnDestroy()
	{
		base.OnDestroy();
		Object.Destroy(m_Menu.gameObject);
	}

	public override void UpdateState()
	{
	}
}
