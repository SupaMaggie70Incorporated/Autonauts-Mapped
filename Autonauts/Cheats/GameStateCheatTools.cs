using UnityEngine;

public class GameStateCheatTools : GameStateBase
{
	private static ObjectCategory m_LastCategory = ObjectCategory.Parts;

	[HideInInspector]
	public GameObject m_Menu;

	protected new void Awake()
	{
		base.Awake();
		Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/ObjectSelect/CheatTools", typeof(GameObject));
		m_Menu = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, menusRootTransform);
		m_Menu.GetComponent<RectTransform>().anchoredPosition = default(Vector2);
		m_Menu.GetComponent<CheatTools>().Init(JustBuildings: false, Everything: false);
		m_Menu.GetComponent<CheatTools>().SetCategory(m_LastCategory);
		HudManager.Instance.HideRollovers();
	}

	protected new void OnDestroy()
	{
		m_LastCategory = m_Menu.GetComponent<CheatTools>().m_CurrentCategory;
		Object.DestroyImmediate(m_Menu.gameObject);
		base.OnDestroy();
	}

	public override void UpdateState()
	{
		if (MyInputManager.m_Rewired.GetButtonDown("Quit") || MyInputManager.m_Rewired.GetButtonDown("Cheats"))
		{
			GameStateManager.Instance.PopState();
		}
	}
}
