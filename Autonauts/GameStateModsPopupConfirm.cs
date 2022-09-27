using MoonSharp.Interpreter;
using UnityEngine;

public class GameStateModsPopupConfirm : GameStateBase
{
	[HideInInspector]
	private ModsPopupConfirm m_Menu;

	protected new void Awake()
	{
		base.Awake();
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Mods/ModsPopupConfirm", typeof(GameObject));
		m_Menu = Object.Instantiate(original, HudManager.Instance.m_MenusRootTransform).GetComponent<ModsPopupConfirm>();
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

	public void SetInformation(string Title, string Description, DynValue CallbackOK, DynValue CallbackCancel, Script Owner)
	{
		m_Menu.SetInformation(Title, Description, CallbackOK, CallbackCancel, Owner);
	}

	public void SetInformationFromID(string TitleID, string DescriptionID)
	{
		m_Menu.SetInformationFromID(TitleID, DescriptionID);
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
