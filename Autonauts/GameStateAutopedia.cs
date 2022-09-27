using UnityEngine;

public class GameStateAutopedia : GameStateBase
{
	public static GameStateAutopedia Instance;

	[HideInInspector]
	private Autopedia m_Autopedia;

	protected new void Awake()
	{
		Instance = this;
		base.Awake();
		ModeButton.Get(ModeButton.Type.Autopedia).SetNew(New: false);
		TimeManager.Instance.PauseAll();
		AudioManager.Instance.Pause(Pause: true);
		HudManager.Instance.HideRollovers();
		Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Autopedia/Autopedia", typeof(GameObject));
		m_Autopedia = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, menusRootTransform).GetComponent<Autopedia>();
		QuestManager.Instance.AddEvent(QuestEvent.Type.SelectAutopedia, Bot: false, null, null);
	}

	protected new void OnDestroy()
	{
		Object.Destroy(m_Autopedia.gameObject);
		base.OnDestroy();
		TimeManager.Instance.UnPauseAll();
		AudioManager.Instance.Pause(Pause: false);
		HudManager.Instance.HideRollovers();
		HudManager.Instance.ActivateQuestCompleteRollover(Activate: false, null);
		HudManager.Instance.ActivateQuestRollover(Activate: false, null);
	}

	public void CeremonyPlaying(bool Playing, Quest NewQuest)
	{
		m_Autopedia.CeremonyPlaying(Playing, NewQuest);
	}

	public override void UpdateState()
	{
		if (!m_Autopedia.m_CeremonyPlaying)
		{
			if (MyInputManager.m_Rewired.GetButtonDown("Quit"))
			{
				m_Autopedia.EscPressed();
			}
			if (MyInputManager.m_Rewired.GetButtonDown("Autopedia"))
			{
				AudioManager.Instance.StartEvent("UIUnpause");
				GameStateManager.Instance.PopState();
			}
		}
	}
}
