using System;
using UnityEngine;

public class GameStateIndustry : GameStateBase
{
	public enum Mode
	{
		Objects,
		Industries,
		Total
	}

	private enum State
	{
		Idle,
		FirstTimeSpeech1,
		Total
	}

	public static GameStateIndustry Instance;

	public static Quest m_SelectedQuest;

	public static ObjectType m_SelectedObject = ObjectTypeList.m_Total;

	public static Mode m_Mode = Mode.Total;

	private State m_State;

	private GameObject m_Tabs;

	[HideInInspector]
	private IndustryTree m_Tree;

	private static int m_NormalDistance = 6;

	private static int m_Distance = 12;

	private static Vector3 m_TreePosition;

	private Vector3 m_OldTreePosition;

	private Vector3 m_NewTreePosition;

	private float m_TreeMoveTimer;

	private static float m_TreeMoveDelay = 0.5f;

	private bool m_MouseGrab;

	private Vector3 m_MouseStartPosition;

	private Vector3 m_TreeStartPosition;

	private static bool m_ResetPosition = true;

	private float m_Zoom;

	private Autopedia m_Autopedia;

	private IndustriesPanels m_IndustriesPanels;

	private ObjectsPanels m_ObjectsPanels;

	private CeremonySpeech m_Speech;

	private GameObject m_Blocker;

	public static void Init()
	{
		m_SelectedQuest = null;
		m_SelectedObject = ObjectTypeList.m_Total;
	}

	public static void SetSelectedQuest(Quest NewQuest)
	{
		m_SelectedQuest = NewQuest;
	}

	public static void SetSelectedObject(ObjectType NewType)
	{
		m_SelectedObject = NewType;
	}

	protected new void Awake()
	{
		Instance = this;
		base.Awake();
		ModeButton.Get(ModeButton.Type.Industry).SetNew(New: false);
		TimeManager.Instance.PauseAll();
		AudioManager.Instance.Pause(Pause: true);
		HudManager.Instance.HideRollovers();
		Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Autopedia/Tree/IndustryTree", typeof(GameObject));
		m_Tree = UnityEngine.Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, menusRootTransform).GetComponent<IndustryTree>();
		original = (GameObject)Resources.Load("Prefabs/Hud/Autopedia/Autopedia", typeof(GameObject));
		m_Autopedia = UnityEngine.Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, menusRootTransform).GetComponent<Autopedia>();
		m_Autopedia.transform.localPosition = default(Vector3);
		menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		original = (GameObject)Resources.Load("Prefabs/Hud/Autopedia/Industry/IndustriesPanels", typeof(GameObject));
		m_IndustriesPanels = UnityEngine.Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, menusRootTransform).GetComponent<IndustriesPanels>();
		m_IndustriesPanels.transform.localPosition = default(Vector3);
		original = (GameObject)Resources.Load("Prefabs/Hud/Autopedia/Objects/ObjectsPanels", typeof(GameObject));
		m_ObjectsPanels = UnityEngine.Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, menusRootTransform).GetComponent<ObjectsPanels>();
		m_ObjectsPanels.transform.localPosition = default(Vector3);
		SetMode(m_Mode);
		if (!WorldSettings.Instance.GetAutopediaSeen())
		{
			WorldSettings.Instance.SetAutopediaSeen();
			SetState(State.FirstTimeSpeech1);
		}
		else
		{
			SetState(State.Idle);
		}
		if (m_ResetPosition)
		{
			m_ResetPosition = false;
		}
		m_Tree.SetPosition(m_TreePosition);
		m_Tree.SetActiveLevel(m_SelectedQuest);
		m_MouseGrab = false;
		UpdateMouseWheelZoom();
		UpdateMousePan();
	}

	protected new void OnDestroy()
	{
		UnityEngine.Object.Destroy(m_ObjectsPanels.gameObject);
		UnityEngine.Object.Destroy(m_IndustriesPanels.gameObject);
		UnityEngine.Object.Destroy(m_Autopedia.gameObject);
		UnityEngine.Object.Destroy(m_Tree.gameObject);
		DestroySpeech();
		base.OnDestroy();
		TimeManager.Instance.UnPauseAll();
		AudioManager.Instance.Pause(Pause: false);
		HudManager.Instance.HideRollovers();
		HudManager.Instance.ActivateQuestCompleteRollover(Activate: false, null);
		HudManager.Instance.ActivateQuestRollover(Activate: false, null);
	}

	private void CreateBlocker()
	{
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Standard/StandardBlocker", typeof(GameObject));
		m_Blocker = UnityEngine.Object.Instantiate(original, m_Autopedia.transform);
	}

	private void DestroyBlocker()
	{
		if ((bool)m_Blocker)
		{
			UnityEngine.Object.Destroy(m_Blocker.gameObject);
			m_Blocker = null;
		}
	}

	private void CreateSpeech(string Speech, bool Top)
	{
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Ceremonies/SpeechPanel", typeof(GameObject));
		m_Speech = UnityEngine.Object.Instantiate(original, m_Autopedia.transform).GetComponent<CeremonySpeech>();
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
			UnityEngine.Object.Destroy(m_Speech.gameObject);
			m_Speech = null;
		}
	}

	public void OnAcceptClicked(BaseGadget NewGadget)
	{
		if (m_State == State.FirstTimeSpeech1)
		{
			SetState(State.Idle);
		}
	}

	private void SetState(State NewState)
	{
		State state = m_State;
		if (state == State.FirstTimeSpeech1)
		{
			DestroyBlocker();
			DestroySpeech();
		}
		m_State = NewState;
		state = m_State;
		if (state == State.FirstTimeSpeech1)
		{
			CreateBlocker();
			CreateSpeech("CeremonyAutopediaFirstTimeSpeech1", Top: false);
		}
	}

	public void SetMode(Mode NewMode)
	{
		m_Mode = NewMode;
		m_ObjectsPanels.gameObject.SetActive(m_Mode == Mode.Objects);
		m_IndustriesPanels.gameObject.SetActive(m_Mode == Mode.Industries);
		m_Tree.UpdateMode(NewMode);
	}

	public void IndustryTreeNodeClicked(IndustryLevel NewLevel)
	{
		m_IndustriesPanels.SetSelectedLevel(NewLevel);
		if (m_Mode == Mode.Objects)
		{
			SetMode(Mode.Total);
		}
	}

	private void MoveTo(Vector3 Position)
	{
		float num = 0f;
		if (m_ObjectsPanels.gameObject.activeSelf)
		{
			num = m_ObjectsPanels.GetHeight();
		}
		if (m_IndustriesPanels.gameObject.activeSelf)
		{
			num = m_IndustriesPanels.GetHeight();
		}
		num /= m_Zoom;
		Position.y += num / 4f;
		m_OldTreePosition = m_TreePosition;
		m_Tree.SetPosition(m_TreePosition);
		m_NewTreePosition = Position;
		m_TreeMoveTimer = m_TreeMoveDelay;
	}

	private void UpdateMoveTo()
	{
		if (m_TreeMoveTimer > 0f)
		{
			m_TreeMoveTimer -= TimeManager.Instance.m_PauseDelta;
			if (m_TreeMoveTimer < 0f)
			{
				m_TreeMoveTimer = 0f;
			}
			m_TreePosition = (Mathf.Cos(m_TreeMoveTimer / m_TreeMoveDelay * (float)Math.PI) * 0.5f + 0.5f) * (m_NewTreePosition - m_OldTreePosition) + m_OldTreePosition;
			m_Tree.SetPosition(m_TreePosition);
		}
	}

	public void IndustryLevelClicked(IndustryLevel NewLevel)
	{
		if (NewLevel != null)
		{
			m_Tree.SetActiveLevel(NewLevel);
			MoveTo(m_Tree.GetPosition());
		}
		else
		{
			m_Tree.SetActiveLevel((IndustryTreeNode)null);
		}
	}

	public void IndustryLevelClicked(Quest NewLevel)
	{
		if (NewLevel != null)
		{
			m_Tree.SetActiveLevel(NewLevel);
			MoveTo(m_Tree.GetPosition());
		}
		else
		{
			m_Tree.SetActiveLevel((Quest)null);
		}
	}

	public void ObjectClicked(ObjectType NewType)
	{
	}

	public void ShowSave()
	{
	}

	private void UpdateMouseWheelZoom()
	{
		float axis = MyInputManager.m_Rewired.GetAxis("MouseScrollWheel");
		m_Zoom = 1f;
		for (int i = 0; i < m_Distance - m_NormalDistance; i++)
		{
			m_Zoom /= 1.25f;
		}
		for (int j = 0; j < m_NormalDistance - m_Distance; j++)
		{
			m_Zoom *= 1.25f;
		}
		m_Tree.SetScale(m_Zoom);
		if (axis != 0f)
		{
			float y = 0f;
			if (axis > 0f)
			{
				y = -1f;
			}
			else if (axis < 0f)
			{
				y = 1f;
			}
			m_TreePosition += new Vector3(0f, y, 0f) * 2000f / m_Zoom * TimeManager.Instance.m_PauseDelta;
		}
		m_Tree.SetPosition(m_TreePosition);
		m_TreePosition = m_Tree.GetPosition();
	}

	private void UpdateMousePan()
	{
	}

	private void UpdateKeyboardPan()
	{
	}

	private void UpdateSelectedArea()
	{
		Vector3 mouseStartPosition = m_MouseStartPosition;
		Vector3 mousePosition = Input.mousePosition;
		if (mouseStartPosition.x > mousePosition.x)
		{
			float x = mouseStartPosition.x;
			mouseStartPosition.x = mousePosition.x;
			mousePosition.x = x;
		}
		if (mouseStartPosition.y > mousePosition.y)
		{
			float y = mouseStartPosition.y;
			mouseStartPosition.y = mousePosition.y;
			mousePosition.y = y;
		}
		m_Tree.SelectArea(mouseStartPosition, mousePosition);
	}

	private void UpdateAreaSelect()
	{
		if (Input.GetMouseButtonDown(0))
		{
			m_MouseGrab = true;
			m_MouseStartPosition = Input.mousePosition;
		}
		else if (Input.GetMouseButtonUp(0))
		{
			m_MouseGrab = false;
		}
		else if (m_MouseGrab)
		{
			UpdateSelectedArea();
		}
	}

	public override void UpdateState()
	{
		UpdateMoveTo();
		if (MyInputManager.m_Rewired.GetButtonDown("Quit") || MyInputManager.m_Rewired.GetButtonDown("Industry"))
		{
			AudioManager.Instance.StartEvent("UIUnpause");
			GameStateManager.Instance.PopState();
		}
	}

	public void Refresh(bool ChangesMade)
	{
		m_Tree.Refresh(ChangesMade);
	}

	private void Update()
	{
		State state = m_State;
		_ = 1;
	}
}
