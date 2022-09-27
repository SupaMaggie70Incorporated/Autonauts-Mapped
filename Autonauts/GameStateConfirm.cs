using UnityEngine;

public class GameStateConfirm : GameStateBase
{
	[HideInInspector]
	public Confirm m_Menu;

	private bool m_HudHidden;

	protected new void Awake()
	{
		base.Awake();
	}

	protected new void OnDestroy()
	{
		base.OnDestroy();
		if (m_HudHidden)
		{
			HudManager.Instance.RolloversEnabled(Enabled: true);
			HudManager.Instance.SetHudButtonsActive(Active: true);
			TutorialPanelController.Instance.SetActive(Active: true);
		}
		Object.Destroy(m_Menu.gameObject);
	}

	public void HideHud()
	{
		m_HudHidden = true;
		HudManager.Instance.RolloversEnabled(Enabled: false);
		HudManager.Instance.SetHudButtonsActive(Active: false);
		TutorialPanelController.Instance.SetActive(Active: false);
		Cursor.Instance.NoTarget();
	}

	public void SetConfirm(ConfirmCallback Callback, string Title, string Description = "", ConfirmCallback Cancel = null)
	{
		Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		string text = "Confirm";
		if (Description != "")
		{
			text += "Description";
		}
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/" + text, typeof(GameObject));
		m_Menu = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, menusRootTransform).GetComponent<Confirm>();
		m_Menu.SetTitle(Title);
		m_Menu.SetConfirm(Callback, Cancel);
		if (Description != "")
		{
			m_Menu.SetDescription(Description);
		}
	}

	public override void UpdateState()
	{
		if (MyInputManager.m_Rewired.GetButtonDown("Quit"))
		{
			AudioManager.Instance.StartEvent("UICloseWindow");
			m_Menu.OnNo(null);
		}
	}
}
