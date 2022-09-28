public class CeremonyOffworldMissionComplete : CeremonyGenericSpeech
{
	private enum State
	{
		Start,
		MissionComplete,
		AwardCount,
		AwardCountWait,
		TotalUpdate,
		Total
	}

	private State m_State;

	private float m_StateTimer;

	private OffworldMission m_Mission;

	private int m_AwardCounter;

	private int m_AwardDelta;

	private float m_AwardDelay;

	private int m_Award;

	private int m_Total;

	private BaseImage m_MissionComplete;

	private BaseText m_AwardText;

	private BaseText m_TotalText;

	protected new void Awake()
	{
		base.Awake();
		AudioManager.Instance.StartEvent("CeremonyFirstResearch");
		base.transform.Find("Tickets/StandardAcceptButton").GetComponent<StandardAcceptButton>().SetAction(OnAcceptClicked, null);
	}

	public void SetMission(OffworldMission NewMission)
	{
		m_Mission = NewMission;
		m_Award = NewMission.GetTickets();
		m_Total = OffworldMissionsManager.Instance.m_Tickets;
		SpacePortMission component = base.transform.Find("Tickets/Mission").GetComponent<SpacePortMission>();
		component.SetMission(NewMission, null);
		base.transform.Find("Tickets/SpaceImage/Planet").GetComponent<BaseImage>().SetSprite(NewMission.GetImage());
		m_MissionComplete = component.transform.Find("Complete").GetComponent<BaseImage>();
		m_MissionComplete.SetActive(Active: false);
		m_AwardText = base.transform.Find("Tickets/Image/Text").GetComponent<BaseText>();
		m_AwardText.SetText("0");
		m_TotalText = base.transform.Find("Tickets/Total").GetComponent<BaseText>();
		string text = TextManager.Instance.Get("CeremonyOffworldMissionCompleteTotal", (m_Total - m_Award).ToString());
		m_TotalText.SetText(text);
	}

	protected override void End()
	{
		base.End();
	}

	public new void OnAcceptClicked(BaseGadget NewGadget)
	{
		if (m_State != State.Total)
		{
			Skip();
		}
		else
		{
			End();
		}
	}

	private void UpdateComplete()
	{
		m_MissionComplete.SetActive(Active: true);
	}

	private void UpdateAward()
	{
		m_AwardText.SetText(m_Award.ToString());
	}

	private void UpdateTotal()
	{
		string text = TextManager.Instance.Get("CeremonyOffworldMissionCompleteTotal", m_Total.ToString());
		m_TotalText.SetText(text);
	}

	private void SetState(State NewState)
	{
		m_State = NewState;
		m_StateTimer = 0f;
		switch (m_State)
		{
		case State.MissionComplete:
			AudioManager.Instance.StartEvent("CeremonyMissionComplete");
			UpdateComplete();
			break;
		case State.AwardCount:
			if (m_Award < VariableManager.Instance.GetVariableAsInt("DailyMissionTickets"))
			{
				m_AwardDelta = 1;
				m_AwardDelay = 0.1f;
			}
			else
			{
				m_AwardDelta = 3;
				m_AwardDelay = 0.05f;
			}
			break;
		case State.AwardCountWait:
			UpdateAward();
			break;
		case State.TotalUpdate:
			AudioManager.Instance.StartEvent("CeremonyTicketsTotal");
			UpdateTotal();
			break;
		}
	}

	private new void Skip()
	{
		AudioManager.Instance.StartEvent("CeremonyTicketsTotal");
		UpdateComplete();
		UpdateAward();
		UpdateTotal();
		SetState(State.Total);
	}

	private void Update()
	{
		switch (m_State)
		{
		case State.Start:
			if (m_StateTimer > 1f)
			{
				SetState(State.MissionComplete);
			}
			break;
		case State.MissionComplete:
			if (m_StateTimer > 0.5f)
			{
				SetState(State.AwardCount);
			}
			break;
		case State.AwardCount:
			if (m_StateTimer > m_AwardDelay)
			{
				m_StateTimer = 0f;
				m_AwardCounter += m_AwardDelta;
				if (m_AwardCounter >= m_Award)
				{
					m_AwardCounter = m_Award;
					SetState(State.AwardCountWait);
				}
				AudioManager.Instance.StartEvent("CeremonyTicketsAdd");
				m_AwardText.SetText(m_AwardCounter.ToString());
			}
			break;
		case State.AwardCountWait:
			if (m_StateTimer > 0.5f)
			{
				SetState(State.TotalUpdate);
			}
			break;
		case State.TotalUpdate:
			if (m_StateTimer > 0.5f)
			{
				SetState(State.Total);
			}
			break;
		}
		m_StateTimer += TimeManager.Instance.m_NormalDelta;
	}
}
