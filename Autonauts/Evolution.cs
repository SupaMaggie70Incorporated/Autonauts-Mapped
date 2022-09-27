using UnityEngine;

public class Evolution : MonoBehaviour
{
	private enum State
	{
		Idle,
		FirstTimeStart,
		FirstTimeRevealBars,
		FirstTimeSpeech1,
		FirstTimeSpeech2,
		LevelUpStart,
		LevelUpRevealBar,
		LevelUpRevealInfo,
		LevelUpSpeech,
		Total
	}

	private static int m_LastLevelSeen = -1;

	private EvolutionLevel[] m_Levels = new EvolutionLevel[8]
	{
		new EvolutionLevel(16737894),
		new EvolutionLevel(16167531),
		new EvolutionLevel(16777096),
		new EvolutionLevel(8978312),
		new EvolutionLevel(8978431),
		new EvolutionLevel(6711039),
		new EvolutionLevel(10053375),
		new EvolutionLevel(16777215)
	};

	private EvolutionTriangleBand[] m_Bands;

	private BaseText[] m_CountText;

	private State m_State;

	private float m_StateTimer;

	private int m_StateCount;

	private BaseButtonBack m_BackButton;

	private CeremonySpeech m_Speech;

	private bool m_FistTime;

	public static void Init()
	{
		m_LastLevelSeen = -1;
	}

	public static void SetLastLevelSeen(int Level)
	{
		if (Level > m_LastLevelSeen)
		{
			m_LastLevelSeen = Level;
		}
	}

	private void Awake()
	{
		m_BackButton = base.transform.Find("TitleBar/StandardCancelButton").GetComponent<BaseButtonBack>();
		m_BackButton.SetAction(OnContinue, m_BackButton);
		m_BackButton.SetActive(Active: false);
		CreateBands();
		SetStartingBands();
		m_FistTime = false;
		if (m_LastLevelSeen == -1)
		{
			m_FistTime = true;
			SetState(State.FirstTimeStart);
		}
		else if (m_LastLevelSeen < FolkManager.Instance.m_HighestActiveFolk)
		{
			SetState(State.LevelUpStart);
		}
		else
		{
			SetState(State.Idle);
		}
	}

	private void OnDestroy()
	{
		DestroySpeech();
	}

	private void CreateBands()
	{
		int num = m_Levels.Length;
		m_Bands = new EvolutionTriangleBand[m_Levels.Length];
		m_CountText = new BaseText[m_Levels.Length];
		Transform transform = base.transform.Find("Bands");
		EvolutionTriangleBand component = transform.Find("TriangleBand").GetComponent<EvolutionTriangleBand>();
		component.gameObject.SetActive(value: false);
		BaseText component2 = transform.Find("FolkCount").GetComponent<BaseText>();
		component2.SetActive(Active: false);
		float y = component.GetComponent<RectTransform>().sizeDelta.y;
		float num2 = y + 10f;
		float num3 = num2 * (float)(num - 1) / 2f - 50f;
		float num4 = y * EvolutionTriangleBand.m_Slope * 2f;
		for (int i = 0; i < num; i++)
		{
			int num5 = num - 1 - i;
			EvolutionTriangleBand component3 = Object.Instantiate(component, transform).GetComponent<EvolutionTriangleBand>();
			component3.gameObject.SetActive(value: true);
			component3.GetComponent<RectTransform>().sizeDelta = new Vector2(num4, y);
			component3.transform.localPosition = new Vector3(0f, num3, 0f);
			component3.SetTitleVisible(Visible: false);
			m_Bands[num5] = component3;
			SetBandActive(num5, Active: false);
			BaseText component4 = Object.Instantiate(component2, transform).GetComponent<BaseText>();
			component4.SetActive(Active: false);
			component4.GetComponent<RectTransform>().anchoredPosition = new Vector2(-50f, num3);
			m_CountText[num5] = component4;
			num3 -= num2;
			num4 += num2 * EvolutionTriangleBand.m_Slope * 2f;
		}
	}

	private void SetStartingBands()
	{
		for (int i = 0; i < m_Levels.Length; i++)
		{
			if (i <= m_LastLevelSeen || CheatManager.Instance.m_CheatMissions)
			{
				SetBandActive(i, Active: true);
				ShowBandTitle(i);
				int num = FolkManager.Instance.m_FolkTierCounts[i];
				if (num > 0)
				{
					ShowBandCount(i, num);
				}
			}
		}
	}

	private void SetBandVisible(int Band, bool Visible)
	{
		m_Bands[Band].gameObject.SetActive(Visible);
	}

	private void SetBandAppear(int Band)
	{
		m_Bands[Band].StartAppear();
		AudioManager.Instance.StartEvent("EvolutionBandAppear");
	}

	private void SetBandActive(int Band, bool Active)
	{
		EvolutionTriangleBand obj = m_Bands[Band];
		float num = 1f;
		if (!Active)
		{
			num = 0.25f;
		}
		Color baseColour = m_Levels[Band].m_BaseColour;
		baseColour.a *= num;
		obj.color = baseColour;
		baseColour = m_Levels[Band].m_FadedColour;
		baseColour.a *= num;
		obj.m_Colour2 = baseColour;
	}

	private void ShowBandTitle(int Band)
	{
		EvolutionTriangleBand obj = m_Bands[Band];
		obj.SetTitleVisible(Visible: true);
		string val = TextManager.Instance.Get("EvolutionLevelTitle" + Band);
		string title = TextManager.Instance.Get("EvolutionLevelTitle", (Band + 1).ToString(), val);
		obj.SetTitle(title);
		string image = "Evolution/EvolutionLevel" + Band;
		obj.SetImage(image);
	}

	private void ShowBandCount(int Band, int FolkCount)
	{
		BaseText obj = m_CountText[Band];
		obj.SetActive(Active: true);
		obj.SetText(FolkCount.ToString());
	}

	public void OnContinue(BaseGadget NewGadget)
	{
		AudioManager.Instance.StartEvent("UIUnpause");
		GameStateManager.Instance.PopState();
	}

	public bool GetIsPlayingCeremony()
	{
		return m_State != State.Idle;
	}

	private void CreateSpeech(string Speech, bool Top, bool Big)
	{
		string text = "SpeechPanel";
		if (Big)
		{
			text = "EvolutionSpeechPanel";
		}
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Ceremonies/" + text, typeof(GameObject));
		m_Speech = Object.Instantiate(original, base.transform).GetComponent<CeremonySpeech>();
		m_Speech.GetButton().SetAction(OnAcceptClicked, null);
		m_Speech.SetSpeechFromID(Speech);
		RectTransform component = m_Speech.GetComponent<RectTransform>();
		if (!Top)
		{
			component.anchorMin = new Vector2(0f, 0f);
			component.anchorMax = new Vector2(1f, 0f);
			component.pivot = new Vector2(0.5f, 0f);
		}
		else
		{
			component.anchorMin = new Vector2(0f, 1f);
			component.anchorMax = new Vector2(1f, 1f);
			component.pivot = new Vector2(0.5f, 1f);
		}
		component.anchoredPosition = default(Vector2);
	}

	private void DestroySpeech()
	{
		if ((bool)m_Speech)
		{
			Object.Destroy(m_Speech.gameObject);
			m_Speech = null;
		}
	}

	public void OnAcceptClicked(BaseGadget NewGadget)
	{
		if (m_State == State.FirstTimeSpeech1)
		{
			SetState(State.FirstTimeSpeech2);
		}
		else if (m_State == State.FirstTimeSpeech2)
		{
			SetState(State.Idle);
		}
		else if (m_State == State.LevelUpSpeech)
		{
			SetState(State.Idle);
		}
	}

	private void SetState(State NewState)
	{
		switch (m_State)
		{
		case State.FirstTimeSpeech1:
		case State.LevelUpSpeech:
			DestroySpeech();
			break;
		case State.FirstTimeSpeech2:
			m_Bands[m_Bands.Length - 1].gameObject.SetActive(value: true);
			DestroySpeech();
			break;
		}
		m_State = NewState;
		switch (m_State)
		{
		case State.Idle:
			m_BackButton.SetActive(Active: true);
			m_LastLevelSeen = FolkManager.Instance.m_HighestActiveFolk;
			break;
		case State.FirstTimeStart:
		{
			for (int k = 0; k < m_Bands.Length; k++)
			{
				SetBandVisible(k, Visible: false);
			}
			m_StateTimer = 1f;
			break;
		}
		case State.FirstTimeRevealBars:
			m_StateCount = 0;
			SetBandVisible(0, Visible: true);
			SetBandAppear(0);
			m_StateTimer = 0.08f;
			break;
		case State.FirstTimeSpeech1:
			CreateSpeech("CeremonyFirstFolkSpeech2", Top: true, Big: false);
			break;
		case State.FirstTimeSpeech2:
			CreateSpeech("CeremonyFirstFolkSpeech3", Top: false, Big: false);
			m_StateTimer = 0f;
			break;
		case State.LevelUpStart:
			m_StateTimer = 1f;
			break;
		case State.LevelUpRevealBar:
		{
			for (int j = m_LastLevelSeen + 1; j <= FolkManager.Instance.m_HighestActiveFolk; j++)
			{
				SetBandActive(j, Active: true);
			}
			m_StateTimer = 1f;
			AudioManager.Instance.StartEvent("EvolutionBandUnlocked");
			break;
		}
		case State.LevelUpRevealInfo:
		{
			for (int i = m_LastLevelSeen + 1; i <= FolkManager.Instance.m_HighestActiveFolk; i++)
			{
				ShowBandTitle(i);
				int num = FolkManager.Instance.m_FolkTierCounts[i];
				if (num > 0)
				{
					ShowBandCount(i, num);
				}
			}
			AudioManager.Instance.StartEvent("EvolutionInfoAppear");
			m_StateTimer = 1f;
			break;
		}
		case State.LevelUpSpeech:
			CreateSpeech("CeremonyFolkLevelUpSpeech" + (FolkManager.Instance.m_HighestActiveFolk + 1), Top: false, Big: true);
			m_Speech.GetComponent<EvolutionSpeechPanel>().SetFolkLevel(FolkManager.Instance.m_HighestActiveFolk);
			break;
		}
	}

	private void Update()
	{
		switch (m_State)
		{
		case State.FirstTimeStart:
			if (m_StateTimer <= 0f)
			{
				SetState(State.FirstTimeRevealBars);
			}
			break;
		case State.FirstTimeRevealBars:
			if (!(m_StateTimer <= 0f))
			{
				break;
			}
			m_StateCount++;
			if (m_StateCount == m_Bands.Length)
			{
				SetState(State.LevelUpRevealBar);
				break;
			}
			SetBandVisible(m_StateCount, Visible: true);
			SetBandAppear(m_StateCount);
			if (m_StateCount != m_Bands.Length - 1)
			{
				m_StateTimer = 0.08f;
			}
			else
			{
				m_StateTimer = 1f;
			}
			break;
		case State.FirstTimeSpeech2:
			if ((int)((0f - m_StateTimer) * 60f) % 20 < 10)
			{
				m_Bands[m_Bands.Length - 1].gameObject.SetActive(value: true);
			}
			else
			{
				m_Bands[m_Bands.Length - 1].gameObject.SetActive(value: false);
			}
			break;
		case State.LevelUpStart:
			if (m_StateTimer <= 0f)
			{
				SetState(State.LevelUpRevealBar);
			}
			break;
		case State.LevelUpRevealBar:
			if (m_StateTimer <= 0f)
			{
				SetState(State.LevelUpRevealInfo);
			}
			break;
		case State.LevelUpRevealInfo:
			if (m_StateTimer <= 0f)
			{
				if (m_FistTime)
				{
					SetState(State.FirstTimeSpeech1);
				}
				else
				{
					SetState(State.LevelUpSpeech);
				}
			}
			break;
		}
		m_StateTimer -= TimeManager.Instance.m_PauseDelta;
	}
}
