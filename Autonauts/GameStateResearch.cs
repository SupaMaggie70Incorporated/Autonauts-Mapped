using UnityEngine;

public class GameStateResearch : GameStateBase
{
	public static GameStateResearch Instance;

	[HideInInspector]
	private Research m_Research;

	protected new void Awake()
	{
		Instance = this;
		base.Awake();
		ModeButton.Get(ModeButton.Type.Industry).SetNew(New: false);
		TimeManager.Instance.PauseAll();
		AudioManager.Instance.Pause(Pause: true);
		HudManager.Instance.HideRollovers();
		Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Research/Research", typeof(GameObject));
		m_Research = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, menusRootTransform).GetComponent<Research>();
	}

	protected new void OnDestroy()
	{
		Object.Destroy(m_Research.gameObject);
		base.OnDestroy();
		TimeManager.Instance.UnPauseAll();
		AudioManager.Instance.Pause(Pause: false);
		HudManager.Instance.HideRollovers();
		HudManager.Instance.ActivateQuestCompleteRollover(Activate: false, null);
		HudManager.Instance.ActivateQuestRollover(Activate: false, null);
	}

	public void CeremonyPlaying(bool Playing, Quest NewQuest)
	{
		m_Research.CeremonyPlaying(Playing, NewQuest);
	}

	public override void UpdateState()
	{
		if (MyInputManager.m_Rewired.GetButtonDown("Quit") || MyInputManager.m_Rewired.GetButtonDown("Research"))
		{
			AudioManager.Instance.StartEvent("UIUnpause");
			GameStateManager.Instance.PopState();
		}
	}
}
