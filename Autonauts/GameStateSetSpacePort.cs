using UnityEngine;

public class GameStateSetSpacePort : GameStateBase
{
	private SpacePortSelect m_Menu;

	protected new void Awake()
	{
		base.Awake();
		Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/BuildingSelect/SpacePortSelect", typeof(GameObject));
		m_Menu = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, menusRootTransform).GetComponent<SpacePortSelect>();
		m_Menu.GetComponent<RectTransform>().anchoredPosition = default(Vector2);
		HudManager.Instance.SetHudButtonsActive(Active: false);
	}

	protected new void OnDestroy()
	{
		base.OnDestroy();
		Object.Destroy(m_Menu.gameObject);
		HudManager.Instance.SetHudButtonsActive(Active: true);
	}

	public void SetInfo(FarmerPlayer NewPlayer, SpacePort NewSpacePort)
	{
		m_Menu.SetBuilding(NewSpacePort);
	}

	public override void UpdateState()
	{
		if (MyInputManager.m_Rewired.GetButtonDown("Quit"))
		{
			GameStateManager.Instance.PopState();
		}
	}
}
