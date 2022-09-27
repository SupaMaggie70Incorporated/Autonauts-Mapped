using UnityEngine;

public class GameStateBadges : GameStateBase
{
	public static GameStateBadges Instance;

	[HideInInspector]
	private Badges m_Badges;

	protected new void Awake()
	{
		Instance = this;
		base.Awake();
		HudManager.Instance.HideRollovers();
		Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Badges/Badges", typeof(GameObject));
		m_Badges = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, menusRootTransform).GetComponent<Badges>();
	}

	protected new void OnDestroy()
	{
		Object.Destroy(m_Badges.gameObject);
		base.OnDestroy();
		HudManager.Instance.HideRollovers();
	}

	public override void UpdateState()
	{
		if (MyInputManager.m_Rewired.GetButtonDown("Quit"))
		{
			AudioManager.Instance.StartEvent("UIUnpause");
			GameStateManager.Instance.PopState();
		}
	}
}
