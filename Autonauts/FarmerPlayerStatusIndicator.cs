using UnityEngine;
using UnityEngine.UI;

public class FarmerPlayerStatusIndicator : Indicator
{
	[HideInInspector]
	public enum State
	{
		Off,
		NoTool,
		TipStart,
		IdeaStart,
		IdeaComplete,
		Total
	}

	private string[] m_ImageFileNames = new string[5] { "", "WorkerNoTool", "", "../Effects/Emoticons/EmoticonIdea", "../Effects/Emoticons/EmoticonIdea" };

	[HideInInspector]
	public State m_State;

	private float m_StateTimer;

	private Image m_Image;

	private Image m_SubImage;

	private Farmer m_Farmer;

	private string[] m_IdeaFrames;

	private bool m_SoundPlayed;

	public void Restart()
	{
	}

	protected new void Awake()
	{
		base.Awake();
		m_Offset = 150f;
		m_Image = GetComponent<Image>();
		m_SubImage = base.transform.Find("Image").GetComponent<Image>();
		m_SubImage.enabled = false;
	}

	public void SetFarmer(Farmer NewFarmer)
	{
		m_Farmer = NewFarmer;
		SetState(State.Off);
	}

	public void SetState(State NewState)
	{
		State state = m_State;
		if (state == State.IdeaComplete)
		{
			m_SubImage.enabled = false;
		}
		State state2 = m_State;
		m_State = NewState;
		if (m_State == State.Off)
		{
			base.gameObject.SetActive(value: false);
		}
		else
		{
			base.gameObject.SetActive(value: true);
		}
		if (m_ImageFileNames[(int)NewState] != "")
		{
			Sprite sprite = (Sprite)Resources.Load("Textures/Hud/Indicators/" + m_ImageFileNames[(int)NewState], typeof(Sprite));
			m_Image.sprite = sprite;
		}
		switch (m_State)
		{
		case State.NoTool:
			m_Scale = 2f;
			break;
		case State.TipStart:
			m_Scale = 1f;
			m_Image.sprite = (Sprite)Resources.Load("Textures/Hud/Quests/QuestTip", typeof(Sprite));
			break;
		case State.IdeaStart:
			m_Scale = 1f;
			m_Image.sprite = (Sprite)Resources.Load("Textures/Hud/Quests/QuestIdea", typeof(Sprite));
			break;
		case State.IdeaComplete:
			m_Scale = 1f;
			m_Image.sprite = (Sprite)Resources.Load("Textures/Hud/Quests/QuestIdea", typeof(Sprite));
			break;
		default:
			m_Scale = 1f;
			break;
		}
		if (state2 != m_State)
		{
			m_StateTimer = 0f;
		}
		m_Image.enabled = true;
		UpdateState();
	}

	public void CompleteIdea(string[] Frames)
	{
		m_IdeaFrames = Frames;
		SetState(State.IdeaComplete);
		m_SoundPlayed = false;
		AudioManager.Instance.StartEvent("CeremonyIdeaStart");
	}

	public void StartNoTool()
	{
		if (m_State != State.IdeaStart && m_State != State.IdeaComplete)
		{
			SetState(State.NoTool);
		}
	}

	private void UpdateNoTool()
	{
		if ((int)(m_StateTimer * 60f) % 32 > 16)
		{
			m_Image.enabled = false;
		}
		else if (!m_Image.enabled)
		{
			m_Image.enabled = true;
			AudioManager.Instance.StartEvent("PlayerNoTool", m_Farmer);
		}
		if (m_StateTimer > 2f)
		{
			SetState(State.Off);
		}
	}

	private void UpdateIdeaComplete()
	{
		if (m_StateTimer >= 0.5f)
		{
			if (!m_SoundPlayed)
			{
				m_SoundPlayed = true;
				AudioManager.Instance.StartEvent("CeremonyIdeaComplete");
			}
			string text = m_IdeaFrames[0];
			if ((int)(m_StateTimer * 60f) % 20 > 10)
			{
				text = m_IdeaFrames[1];
			}
			m_Scale = 2f;
			m_SubImage.enabled = true;
			Sprite sprite = (Sprite)Resources.Load("Textures/Hud/Indicators/Ideas/" + text, typeof(Sprite));
			m_SubImage.sprite = sprite;
			sprite = (Sprite)Resources.Load("Textures/Hud/Indicators/Ideas/IdeaBubble", typeof(Sprite));
			m_Image.sprite = sprite;
		}
		if (m_StateTimer > 3f)
		{
			SetState(State.Off);
			TabQuests.Instance.UpdateLists();
		}
	}

	private void UpdateState()
	{
		UpdateTransform(m_Farmer.transform.position + new Vector3(0f, 2f, 0f));
		switch (m_State)
		{
		case State.NoTool:
			UpdateNoTool();
			break;
		case State.IdeaComplete:
			UpdateIdeaComplete();
			break;
		}
	}

	private void Update()
	{
		UpdateState();
		m_StateTimer += TimeManager.Instance.m_NormalDelta;
	}
}
