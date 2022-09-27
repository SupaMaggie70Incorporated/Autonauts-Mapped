public class TutorialTutor : BaseImage
{
	private enum State
	{
		Idle,
		Talking,
		Total
	}

	private State m_State;

	private float m_StateTimer;

	private string m_IdleImage = "Tutorial/TutorIdle";

	private string m_TalkImage = "Tutorial/TutorTalk";

	protected new void Awake()
	{
		base.Awake();
		SetState(State.Idle);
	}

	public void SetImages(string Idle, string Talk)
	{
		m_IdleImage = Idle;
		m_TalkImage = Talk;
	}

	public void StartTalking()
	{
		SetState(State.Talking);
	}

	public void StopTalking()
	{
		SetState(State.Idle);
	}

	public bool GetIsTalking()
	{
		return m_State == State.Talking;
	}

	private void SetState(State NewState)
	{
		m_State = NewState;
		m_StateTimer = 0f;
		switch (m_State)
		{
		case State.Idle:
			SetSprite(m_IdleImage);
			break;
		case State.Talking:
			SetSprite(m_IdleImage);
			break;
		}
	}

	private void Update()
	{
		State state = m_State;
		if (state == State.Talking)
		{
			string sprite = m_IdleImage;
			if ((int)(m_StateTimer * 60f) % 14 < 7)
			{
				sprite = m_TalkImage;
			}
			SetSprite(sprite);
		}
		if ((bool)TimeManager.Instance)
		{
			if (TimeManager.Instance.m_PauseTimeEnabled)
			{
				m_StateTimer += TimeManager.Instance.m_PauseDelta;
			}
			else
			{
				m_StateTimer += TimeManager.Instance.m_NormalDelta;
			}
		}
	}
}
