using UnityEngine;

public class GameStateAcademy : GameStateBase
{
	public static GameStateAcademy Instance;

	[HideInInspector]
	private Academy m_Academy;

	protected new void Awake()
	{
		Instance = this;
		base.Awake();
		ModeButton.Get(ModeButton.Type.Industry).SetNew(New: false);
		TimeManager.Instance.PauseAll();
		AudioManager.Instance.Pause(Pause: true);
		HudManager.Instance.HideRollovers();
		Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Academy/Academy", typeof(GameObject));
		m_Academy = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, menusRootTransform).GetComponent<Academy>();
	}

	protected new void OnDestroy()
	{
		Object.Destroy(m_Academy.gameObject);
		base.OnDestroy();
		TimeManager.Instance.UnPauseAll();
		AudioManager.Instance.Pause(Pause: false);
		HudManager.Instance.HideRollovers();
		HudManager.Instance.ActivateQuestCompleteRollover(Activate: false, null);
		HudManager.Instance.ActivateQuestRollover(Activate: false, null);
	}

	public void CeremonyPlaying(bool Playing, Quest NewQuest)
	{
		m_Academy.CeremonyPlaying(Playing, NewQuest);
		if (Playing)
		{
			CeremonyManager.Instance.StartImmediateCeremony(CeremonyManager.CeremonyType.CertificateAcademyEnded, NewQuest, null);
		}
	}

	public override void UpdateState()
	{
		if (MyInputManager.m_Rewired.GetButtonDown("Quit") || MyInputManager.m_Rewired.GetButtonDown("Academy"))
		{
			AudioManager.Instance.StartEvent("UIUnpause");
			GameStateManager.Instance.PopState();
		}
	}
}
