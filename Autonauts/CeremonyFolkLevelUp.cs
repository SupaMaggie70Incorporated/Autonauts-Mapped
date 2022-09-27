using UnityEngine;

public class CeremonyFolkLevelUp : CeremonyBase
{
	private enum State
	{
		Speech,
		RevealRollover,
		RevealNextLevel,
		NextLevelSeen,
		RevealButton,
		Total
	}

	private State m_State;

	private float m_StateTimer;

	private FolkRollover m_FolkRollover;

	private FolkRollover m_FolkRollover2;

	private StandardAcceptButton m_FolkRolloverButton;

	private int m_TargetUID;

	private void Awake()
	{
		AudioManager.Instance.StartEvent("CeremonyFolkLevelUp");
		m_FolkRollover = base.transform.Find("FolkRollover").GetComponent<FolkRollover>();
		m_FolkRollover.transform.Find("BasePanel/StandardAcceptButton").gameObject.SetActive(value: false);
		m_FolkRollover2 = Object.Instantiate(m_FolkRollover, m_FolkRollover.transform.parent);
		m_FolkRolloverButton = m_FolkRollover2.transform.Find("BasePanel/StandardAcceptButton").GetComponent<StandardAcceptButton>();
		m_FolkRolloverButton.SetAction(OnAcceptClicked, m_FolkRolloverButton);
		m_FolkRolloverButton.gameObject.SetActive(value: false);
		m_FolkRollover.gameObject.SetActive(value: false);
		m_FolkRollover2.gameObject.SetActive(value: false);
		m_State = State.Speech;
	}

	public void SetTarget(int UID)
	{
		m_TargetUID = UID;
		NextState();
	}

	private void StartRevealRollover()
	{
		m_FolkRollover.gameObject.SetActive(value: true);
		m_FolkRollover.ForceOpen();
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(m_TargetUID);
		m_FolkRollover.SetTarget(objectFromUniqueID.GetComponent<Folk>(), DisableBars: true, PreviousTier: true);
		m_FolkRollover2.gameObject.SetActive(value: true);
		m_FolkRollover2.ForceOpen();
		objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(m_TargetUID);
		m_FolkRollover2.SetTarget(objectFromUniqueID.GetComponent<Folk>(), DisableBars: true);
		m_FolkRollover2.gameObject.SetActive(value: false);
		CameraManager.Instance.SetPausedDOFEffect();
	}

	private void StartRevealNextLevel()
	{
		m_FolkRollover2.gameObject.SetActive(value: true);
	}

	private void UpdateRevealNextLevel()
	{
		if ((int)(m_StateTimer * 60f) % 8 < 4)
		{
			m_FolkRollover2.gameObject.SetActive(value: true);
		}
		else
		{
			m_FolkRollover2.gameObject.SetActive(value: false);
		}
	}

	private void StartNextLevelSeen()
	{
		m_FolkRollover.gameObject.SetActive(value: false);
		m_FolkRollover2.gameObject.SetActive(value: true);
	}

	private void StartRevealButton()
	{
		m_FolkRolloverButton.gameObject.SetActive(value: true);
	}

	private void NextState()
	{
		m_State++;
		if (m_State == State.RevealRollover)
		{
			StartRevealRollover();
		}
		else if (m_State == State.RevealNextLevel)
		{
			StartRevealNextLevel();
		}
		else if (m_State == State.NextLevelSeen)
		{
			StartNextLevelSeen();
		}
		else if (m_State == State.RevealButton)
		{
			StartRevealButton();
		}
		else
		{
			End();
		}
		m_StateTimer = 0f;
	}

	public void OnAcceptClicked(BaseGadget NewGadget)
	{
		NextState();
	}

	private void End()
	{
		Object.Destroy(base.gameObject);
		CeremonyManager.Instance.CeremonyEnded(CheckForMoreCeremonies: false);
		CameraManager.Instance.RestorePausedDOFEffect();
		GameStateManager.Instance.SetState(GameStateManager.State.Evolution);
	}

	private void Update()
	{
		switch (m_State)
		{
		case State.RevealRollover:
			if (m_StateTimer >= 1f)
			{
				NextState();
			}
			break;
		case State.RevealNextLevel:
			UpdateRevealNextLevel();
			if (m_StateTimer >= 1f)
			{
				NextState();
			}
			break;
		case State.NextLevelSeen:
			if (m_StateTimer >= 0.5f)
			{
				NextState();
			}
			break;
		}
		m_StateTimer += TimeManager.Instance.m_NormalDelta;
	}
}
