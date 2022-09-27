using UnityEngine;
using UnityEngine.UI;

public class WorkerStatusIndicator : Indicator
{
	[HideInInspector]
	public enum State
	{
		Off,
		Busy,
		Question,
		LowEnergy,
		NoTool,
		NoEnergy,
		TryingToListen,
		Copying,
		Restoring,
		Total
	}

	[HideInInspector]
	public State m_State;

	private float m_StateTimer;

	private static string[] m_StateNames;

	private static string[] m_StateImageNames;

	private Image m_Image;

	private Image m_ToolImage;

	private Farmer m_Farmer;

	private bool m_NoEnergy;

	private bool m_LowEnergy;

	[HideInInspector]
	public bool m_NoTool;

	[HideInInspector]
	public Farmer.State m_NoToolState;

	private ObjectType m_NoToolType;

	private bool m_Busy;

	private bool m_Question;

	private bool m_TryingToListen;

	private bool m_Copying;

	private bool m_Restoring;

	private float m_Height;

	private bool m_Enabled;

	private float m_NormalScale;

	private string m_SpriteName;

	public static void Clear()
	{
		m_StateNames = null;
	}

	public void Init()
	{
		int num = 9;
		m_StateNames = new string[num];
		m_StateImageNames = new string[num];
		for (int i = 0; i < num; i++)
		{
			State state = (State)i;
			string val = TextManager.Instance.Get("TabWorkersState" + state);
			string text = TextManager.Instance.Get("TabWorkersStateBot", val);
			m_StateNames[i] = text;
			string[] stateImageNames = m_StateImageNames;
			int num2 = i;
			State state2 = (State)i;
			stateImageNames[num2] = "Worker" + state2;
		}
	}

	public void Restart()
	{
		m_NoEnergy = false;
		m_LowEnergy = false;
		m_NoTool = false;
		m_Busy = false;
		m_Question = false;
		m_TryingToListen = false;
		m_Copying = false;
		m_Restoring = false;
		m_SpriteName = "";
		UpdateState();
		if (m_StateNames == null)
		{
			Init();
		}
	}

	protected new void Awake()
	{
		base.Awake();
		m_Offset = 50f;
	}

	public void SetFarmer(Farmer NewFarmer)
	{
		m_Image = GetComponent<Image>();
		m_ToolImage = base.transform.Find("Image").GetComponent<Image>();
		m_Farmer = NewFarmer;
		SetState(State.Off);
	}

	public void UpdateHeight()
	{
		m_Height = ObjectUtils.ObjectBounds(m_Farmer.gameObject).size.y;
	}

	public string GetStateImageName()
	{
		if (m_StateImageNames == null)
		{
			Init();
		}
		return m_StateImageNames[(int)m_State];
	}

	public string GetStateRolloverName()
	{
		return m_StateNames[(int)m_State];
	}

	public void SetState(State NewState)
	{
		State state = m_State;
		m_State = NewState;
		if (m_State == State.Off)
		{
			base.gameObject.SetActive(value: false);
		}
		else
		{
			base.gameObject.SetActive(value: true);
		}
		string stateImageName = GetStateImageName();
		if (stateImageName != "")
		{
			Sprite sprite = (Sprite)Resources.Load("Textures/Hud/Indicators/" + stateImageName, typeof(Sprite));
			m_Image.sprite = sprite;
		}
		m_ToolImage.gameObject.SetActive(value: false);
		if (NewState == State.NoTool)
		{
			Sprite icon = IconManager.Instance.GetIcon(m_NoToolType);
			m_ToolImage.sprite = icon;
		}
		if (state != m_State)
		{
			m_StateTimer = 0f;
			if (m_Farmer.m_TypeIdentifier == ObjectType.Worker)
			{
				m_Farmer.GetComponent<Worker>().IndicatorStateChanged();
			}
		}
		if (m_State == State.NoTool || m_State == State.Question || m_State == State.TryingToListen || m_State == State.Copying || m_State == State.Restoring)
		{
			m_NormalScale = 2f;
		}
		else
		{
			m_NormalScale = 1f;
		}
		if (m_State == State.Question)
		{
			m_StateTimer = Random.Range(0, 60);
		}
		SetImageEnabled(m_State != State.Off);
	}

	private void UpdateState()
	{
		if (m_Copying)
		{
			SetState(State.Copying);
		}
		else if (m_Restoring)
		{
			SetState(State.Restoring);
		}
		else if (m_NoEnergy)
		{
			SetState(State.NoEnergy);
		}
		else if (m_NoTool)
		{
			SetState(State.NoTool);
		}
		else if (m_LowEnergy)
		{
			SetState(State.LowEnergy);
		}
		else if (m_Question)
		{
			SetState(State.Question);
		}
		else if (m_Busy)
		{
			SetState(State.Busy);
		}
		else if (m_TryingToListen)
		{
			SetState(State.TryingToListen);
		}
		else
		{
			SetState(State.Off);
		}
	}

	public void SetNoTool(bool NoTool, Farmer.State NoToolState, ObjectType NewType)
	{
		if (m_NoTool != NoTool)
		{
			m_NoTool = NoTool;
			m_NoToolState = NoToolState;
			m_NoToolType = NewType;
			UpdateState();
		}
	}

	public void SetNoEnergy(bool NoEnergy)
	{
		if (m_NoEnergy != NoEnergy)
		{
			m_NoEnergy = NoEnergy;
			if (m_NoEnergy)
			{
				AudioManager.Instance.StartEvent("WorkerNoEnergy", m_Farmer);
			}
			UpdateState();
		}
	}

	public void SetLowEnergy(bool LowEnergy)
	{
		if (m_LowEnergy != LowEnergy)
		{
			m_LowEnergy = LowEnergy;
			if (m_LowEnergy)
			{
				AudioManager.Instance.StartEvent("WorkerLowEnergy", m_Farmer);
			}
			UpdateState();
		}
	}

	public void SetBusy(bool Busy)
	{
		if (m_Busy != Busy)
		{
			m_Busy = Busy;
			if (m_Busy)
			{
				AudioManager.Instance.StartEvent("WorkerStuck", m_Farmer);
			}
			UpdateState();
		}
	}

	public void SetTryingToListen(bool Listen)
	{
		if (m_TryingToListen != Listen)
		{
			m_TryingToListen = Listen;
			UpdateState();
		}
	}

	public void SetQuestion(bool Question)
	{
		if (m_Question != Question)
		{
			m_Question = Question;
			if (m_Question)
			{
				AudioManager.Instance.StartEvent("WorkerStuck", m_Farmer);
			}
			UpdateState();
		}
	}

	public void SetCopying(bool Copying)
	{
		if (m_Copying != Copying)
		{
			m_Copying = Copying;
			UpdateState();
		}
	}

	public void SetRestoring(bool Restoring)
	{
		if (m_Restoring != Restoring)
		{
			m_Restoring = Restoring;
			UpdateState();
		}
	}

	private void SetImage(string Name)
	{
		if (m_SpriteName != Name)
		{
			m_SpriteName = Name;
			Sprite sprite = (Sprite)Resources.Load("Textures/Hud/Indicators/" + Name, typeof(Sprite));
			m_Image.sprite = sprite;
		}
	}

	private void UpdateQuestion()
	{
		if (TabManager.Instance.m_ActiveTabType == TabManager.TabType.Workers)
		{
			SetImageEnabled(Enabled: true);
			string image = "WorkerQuestion";
			if ((int)(m_StateTimer * 60f) % 30 > 15)
			{
				image = "WorkerQuestion2";
			}
			SetImage(image);
		}
		else
		{
			SetImageEnabled(Enabled: false);
		}
	}

	private void SetImageEnabled(bool Enabled)
	{
		m_Enabled = Enabled;
	}

	public void UpdateIndicator()
	{
		if (m_Image == null)
		{
			m_Image = GetComponent<Image>();
			m_Image.enabled = true;
		}
		if (m_ToolImage == null)
		{
			m_ToolImage = base.transform.Find("Image").GetComponent<Image>();
			m_ToolImage.enabled = true;
		}
		if (!m_Farmer.m_Visible)
		{
			m_Image.gameObject.SetActive(value: false);
			m_ToolImage.gameObject.SetActive(value: false);
			return;
		}
		m_Image.gameObject.SetActive(value: true);
		switch (m_State)
		{
		case State.LowEnergy:
			if ((int)(m_StateTimer * 60f) % 40 > 30)
			{
				SetImageEnabled(Enabled: false);
			}
			else
			{
				SetImageEnabled(Enabled: true);
			}
			break;
		case State.NoEnergy:
			if ((int)(m_StateTimer * 60f) % 16 > 8)
			{
				SetImageEnabled(Enabled: false);
			}
			else
			{
				SetImageEnabled(Enabled: true);
			}
			break;
		case State.NoTool:
			if ((int)(m_StateTimer * 60f) % 32 > 16)
			{
				if (!m_ToolImage.gameObject.activeSelf)
				{
					m_ToolImage.gameObject.SetActive(value: true);
					Sprite sprite = (Sprite)Resources.Load("Textures/Hud/Indicators/WorkerNoTool2", typeof(Sprite));
					m_Image.sprite = sprite;
				}
			}
			else if (m_ToolImage.gameObject.activeSelf)
			{
				m_ToolImage.gameObject.SetActive(value: false);
				Sprite sprite2 = (Sprite)Resources.Load("Textures/Hud/Indicators/WorkerNoTool", typeof(Sprite));
				m_Image.sprite = sprite2;
				AudioManager.Instance.StartEvent("WorkerNoTool", m_Farmer);
			}
			break;
		case State.Question:
			UpdateQuestion();
			break;
		}
		if (m_Enabled)
		{
			m_Scale = m_NormalScale;
			UpdateTransform(m_Farmer.transform.position + new Vector3(0f, m_Height, 0f));
		}
		else if (m_Scale != 0f)
		{
			m_Scale = 0f;
			UpdateTransform(m_Farmer.transform.position + new Vector3(0f, m_Height, 0f));
		}
		m_StateTimer += TimeManager.Instance.m_NormalDelta;
	}
}
