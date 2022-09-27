using UnityEngine;

public class GameStateStats : GameStateBase
{
	private Stats m_Stats;

	protected new void Awake()
	{
		base.Awake();
		Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Stats/Stats", typeof(GameObject));
		m_Stats = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, menusRootTransform).GetComponent<Stats>();
		m_Stats.transform.localPosition = new Vector3(HudManager.Instance.m_HalfScaledWidth, HudManager.Instance.m_HalfScaledHeight, 0f);
	}

	protected new void OnDestroy()
	{
		base.OnDestroy();
		Object.Destroy(m_Stats.gameObject);
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
