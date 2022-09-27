using UnityEngine;

public class GameStateModsUploadConfirm : GameStateBase
{
	[HideInInspector]
	private ModsUploadConfirm m_Menu;

	protected new void Awake()
	{
		base.Awake();
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Mods/ModsUploadConfirm", typeof(GameObject));
		m_Menu = Object.Instantiate(original, HudManager.Instance.m_MenusRootTransform).GetComponent<ModsUploadConfirm>();
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
		if (ModManager.Instance.MenuForceErrorReturn)
		{
			ModManager.Instance.MenuForceErrorReturn = false;
			GameStateManager.Instance.PopState();
		}
	}

	public override void UpdateState()
	{
		if (MyInputManager.m_Rewired.GetButtonDown("Quit"))
		{
			AudioManager.Instance.StartEvent("UIOptionCancelled");
			GameStateManager.Instance.PopState();
		}
	}

	public void SetUploadComplete()
	{
		m_Menu.UploadCompleted();
	}
}
