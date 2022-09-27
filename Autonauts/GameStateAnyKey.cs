using Rewired;
using UnityEngine;

public class GameStateAnyKey : GameStateBase
{
	[HideInInspector]
	public AnyKey m_AnyKey;

	protected new void Awake()
	{
		base.Awake();
		Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Settings/AnyKey", typeof(GameObject));
		m_AnyKey = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, menusRootTransform).GetComponent<AnyKey>();
		m_AnyKey.GetComponent<RectTransform>().anchoredPosition = default(Vector2);
	}

	protected new void OnDestroy()
	{
		base.OnDestroy();
		Object.Destroy(m_AnyKey.gameObject);
	}

	public void SetAem(ActionElementMap aem)
	{
		m_AnyKey.SetAem(aem, null);
	}

	public override void UpdateState()
	{
		if (MyInputManager.m_Rewired.GetButtonDown("Quit"))
		{
			AudioManager.Instance.StartEvent("UICloseWindow");
			m_AnyKey.OnBackButtonClicked(null);
			return;
		}
		for (int i = 0; i <= 296; i++)
		{
			KeyCode keyCode = (KeyCode)i;
			if (!Input.GetKeyDown(keyCode))
			{
				continue;
			}
			if (keyCode != KeyCode.F1 && keyCode != KeyCode.F2 && keyCode != KeyCode.F9 && keyCode != KeyCode.F10)
			{
				ModifierKey newModifier = ModifierKey.None;
				if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
				{
					newModifier = ModifierKey.Control;
				}
				if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
				{
					newModifier = ModifierKey.Shift;
				}
				if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
				{
					newModifier = ModifierKey.Alt;
				}
				m_AnyKey.SetKeyCode(keyCode, newModifier, NewKey: true);
			}
			break;
		}
	}
}
