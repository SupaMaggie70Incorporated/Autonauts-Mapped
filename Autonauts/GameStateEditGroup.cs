using UnityEngine;

public class GameStateEditGroup : GameStateBase
{
	[HideInInspector]
	public EditGroup m_Menu;

	protected new void Awake()
	{
		base.Awake();
		Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Tabs/EditGroup", typeof(GameObject));
		m_Menu = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, menusRootTransform).GetComponent<EditGroup>();
		m_Menu.transform.localPosition = new Vector3(HudManager.Instance.m_HalfCanvasWidth, HudManager.Instance.m_HalfCanvasHeight, 0f);
		HudManager.Instance.ActivateUIRollover(Activate: false, "", default(Vector3));
		HudManager.Instance.HideRollovers();
		HudManager.Instance.SetHudButtonsActive(Active: false);
	}

	protected new void OnDestroy()
	{
		base.OnDestroy();
		Object.Destroy(m_Menu.gameObject);
		HudManager.Instance.SetHudButtonsActive(Active: true);
	}

	public override void UpdateState()
	{
		if (MyInputManager.m_Rewired.GetButtonDown("Quit"))
		{
			AudioManager.Instance.StartEvent("UIOptionCancelled");
			m_Menu.OnBackClicked(null);
		}
	}
}
