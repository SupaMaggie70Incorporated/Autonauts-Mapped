using UnityEngine;

public class GameStateAbout : GameStateBase
{
	[HideInInspector]
	public GameObject m_Menu;

	private string m_OldMusic;

	protected new void Awake()
	{
		base.Awake();
		Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Menus/About", typeof(GameObject));
		m_Menu = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, menusRootTransform);
		m_OldMusic = AudioManager.Instance.m_MusicName;
		AudioManager.Instance.StartMusic("MusicAbout");
	}

	protected new void OnDestroy()
	{
		AudioManager.Instance.StartMusic(m_OldMusic);
		base.OnDestroy();
		Object.Destroy(m_Menu.gameObject);
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
