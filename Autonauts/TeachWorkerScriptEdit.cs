using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeachWorkerScriptEdit : BasePanelOptions
{
	public enum State
	{
		Idle,
		Recording,
		Playing,
		PlayingWaiting,
		Paused,
		Busy,
		Dead,
		Total
	}

	public static TeachWorkerScriptEdit Instance;

	private static bool m_Expanded = false;

	public State m_State;

	private static int[] m_PanelColours = new int[7] { 5505022, 16056839, 453376, 16096007, 453376, 16096007, 8102829 };

	private static int[] m_TitleBarColours = new int[7] { 10797767, 14981272, 11851942, 15977627, 11851942, 15977627, 10797767 };

	private BaseInputField m_NameInputField;

	public BaseScrollView m_ScrollView;

	private Image m_ScrollViewGrid;

	private Image m_ScrollViewBorder;

	private Image m_ScrollViewDeadPanel;

	public GameObject m_ButtonsRoot;

	private BrainButton m_CameraButton;

	private BrainButton m_TradeButton;

	private BrainButton m_UndoButton;

	private BrainButton m_DeleteButton;

	private BrainButton m_PlayButton;

	private BrainButton m_StopButton;

	private BrainButton m_RecordButton;

	private BrainButton m_DropAllButton;

	private BrainButton m_CallButton;

	private BrainButton m_ExpandButton;

	private BrainButton m_RepeatButton;

	private BrainButton m_DatabaseButton;

	private BrainButton m_ClipboardButton;

	private GameObject m_NonEditable;

	private BaseImage m_NonEditablePanel;

	private float m_NonEditableTimer;

	private float m_ExpandTimer;

	private float m_DefaultWidth;

	private bool m_PlayActive;

	private float m_RecordingTimer;

	private bool m_BotSelect;

	private static int m_ExcessInstructions = 10;

	public Worker m_CurrentTarget;

	private HudInstruction m_RepeatInstruction;

	private GameObject m_DragParent;

	private List<HudInstruction> m_DragSelectList;

	private List<HudInstruction> m_DragInstructions;

	private Vector3 m_DragMouseOffset;

	private bool m_DragCopy;

	[HideInInspector]
	public bool m_ViewportHover;

	private HudInstruction m_HoverInstruction;

	private Transform m_CanvasParent;

	private Transform m_ContentParent;

	private GameObject m_Viewport;

	public HighInstructionList m_Instructions;

	private HighInstructionList m_UndoInstructions;

	public int Counter;

	private GameObject m_InstructionCursor;

	private bool m_Locked;

	private HighInstruction m_BestInstruction;

	private int m_BestInstructionDiff;

	private GameObject m_ExecutingCursor;

	private int m_LastDragLine;

	private HighInstruction m_LastDragParent;

	private bool m_LastChildren2;

	private GameObject m_Panel;

	private GameObject m_InstructionPanel;

	private float m_MemoryTimer;

	private float m_FlashMemoryTimer;

	private Rect m_PanelRect;

	private Rect m_InstructionsRect;

	private bool m_Followed;

	private BaseClass m_PointAtObjectSearch;

	private bool m_PlayCeremony;

	private bool m_Backup;

	private bool m_StartRepeatClick;

	private HudInstruction m_StartRepeatInstruction;

	private Vector3 m_MouseStartPosition;

	public bool m_Hover;

	private InstructionPaletteSmall m_InstructionPalette;

	private float m_FollowTimer;

	private bool m_TargetBadge;

	public bool m_RefreshInstruction;

	private HudInstruction m_NewDeleteInstruction;

	private HighInstruction m_LastExectuedInstruction;

	private bool m_UndoAvailable;

	private Wobbler m_Wobbler;

	public bool m_Scaling;

	public bool m_ScalingUp;

	private Vector2 m_ScaleEndPosition;

	private Vector2 m_ScaleStartPosition;

	private static float m_ScalingDelay = 0.2f;

	private List<HighInstruction> m_NewInstructions;

	private RenderTexture m_RenderTexture;

	public Texture2D m_FinalTexture;

	private GameObject m_InstructionShadow;

	private float m_InstructionFailTimer;

	private Vector3 m_NormalScreenPosition;

	private bool m_Pushed;

	private bool m_DraggingInstructionSelect;

	private HudInstruction m_InstructionSelectInstruction;

	private GameObject m_MouseObject;

	private void Awake()
	{
		if (!(HudManager.Instance == null))
		{
			Instance = this;
			m_NameInputField = base.transform.Find("NameInputField").GetComponent<BaseInputField>();
			m_NameInputField.SetAction(OnNameInputChanged, m_NameInputField);
			m_ButtonsRoot = base.transform.Find("Buttons").gameObject;
			GameObject root = m_ButtonsRoot.transform.Find("RecordBorder").gameObject;
			m_PlayButton = new BrainButton(root, "PlayButton", OnPlayClicked);
			m_StopButton = new BrainButton(root, "StopButton", OnStopClicked);
			m_RecordButton = new BrainButton(root, "RecordButton", OnRecordClicked);
			m_DropAllButton = new BrainButton(m_ButtonsRoot, "DropAllButton", OnDropAllClicked);
			m_CallButton = new BrainButton(m_ButtonsRoot, "CallButton", OnCallClicked);
			m_CameraButton = new BrainButton(m_ButtonsRoot, "CameraButton", OnCameraClicked);
			m_TradeButton = new BrainButton(m_ButtonsRoot, "TradeButton", OnTradeClicked);
			m_UndoButton = new BrainButton(m_ButtonsRoot, "UndoButton", OnUndoClicked);
			m_DeleteButton = new BrainButton(m_ButtonsRoot, "DeleteButton", OnDeleteClicked);
			m_RepeatButton = new BrainButton(m_ButtonsRoot, "RepeatButton", null);
			m_RepeatButton.m_Button.m_OnDownEvent = OnRepeatPointerDown;
			m_DatabaseButton = new BrainButton(m_ButtonsRoot, "DatabaseButton", OnDatabaseClicked);
			m_ScrollView = base.transform.Find("BaseScrollView").GetComponent<BaseScrollView>();
			m_ScrollViewBorder = m_ScrollView.transform.Find("Border").GetComponent<Image>();
			m_ScrollViewDeadPanel = m_ScrollView.transform.Find("Viewport").Find("DeadPanel").GetComponent<Image>();
			m_ScrollViewDeadPanel.gameObject.SetActive(value: false);
			m_ScrollViewGrid = m_ScrollView.GetContent().transform.Find("Grid").GetComponent<Image>();
			m_ExpandButton = new BrainButton(base.gameObject, "ExpandButton", OnExpandClicked);
			m_ClipboardButton = new BrainButton(base.gameObject, "ClipboardButton", OnClipboardClicked);
			m_NonEditable = base.transform.Find("NonEditable").gameObject;
			m_NonEditable.SetActive(value: false);
			m_NonEditablePanel = m_ScrollView.transform.Find("NonEditablePanel").GetComponent<BaseImage>();
			m_NonEditablePanel.SetAction(OnNonEditableClicked, m_NonEditablePanel);
			GetBackButton().SetAction(OnBackClicked, null);
			m_DefaultWidth = GetComponent<RectTransform>().sizeDelta.x;
			OldAwake();
			m_State = State.Total;
			m_NormalScreenPosition = TeachWorkerScriptController.Instance.GetComponent<RectTransform>().anchoredPosition;
			UpdateExpanded();
		}
	}

	public void OnDropAllClicked(BaseGadget NewGadget)
	{
		OnClickDropAll();
	}

	public void OnCallClicked(BaseGadget NewGadget)
	{
		OnClickCall();
	}

	public void OnCameraClicked(BaseGadget NewGadget)
	{
		OnClickFollow();
	}

	public void OnTradeClicked(BaseGadget NewGadget)
	{
		OnClickTrade();
	}

	public void OnUndoClicked(BaseGadget NewGadget)
	{
		OnClickUndo();
	}

	public void OnDeleteClicked(BaseGadget NewGadget)
	{
		OnClickClear();
	}

	public void OnPlayClicked(BaseGadget NewGadget)
	{
		OnClickGo();
	}

	public void OnStopClicked(BaseGadget NewGadget)
	{
		OnClickStop();
	}

	public void OnRecordClicked(BaseGadget NewGadget)
	{
		OnClickTeach();
	}

	public void OnExpandClicked(BaseGadget NewGadget)
	{
		m_Expanded = !m_Expanded;
		UpdateExpanded();
		UpdateContentSize();
	}

	public void OnRepeatPointerDown(BaseGadget NewGadget)
	{
		OnInstructionClick(m_RepeatInstruction);
	}

	public void OnBackClicked(BaseGadget NewGadget)
	{
		OnClickBack();
	}

	public void OnCanvasClicked()
	{
		if (!IsDragging() && !m_DraggingInstructionSelect)
		{
			DragSelectListClear();
			CreateHudInstructions();
		}
	}

	private void AddInstruction(List<HudInstruction> NewInstructions, HighInstruction NewInstruction)
	{
		NewInstructions.Add(NewInstruction.m_HudParent);
		foreach (HighInstruction child in NewInstruction.m_Children)
		{
			AddInstruction(NewInstructions, child);
		}
		foreach (HighInstruction item in NewInstruction.m_Children2)
		{
			AddInstruction(NewInstructions, item);
		}
	}

	public void OnDatabaseClicked(BaseGadget NewGadget)
	{
		if ((bool)BotServer.m_FirstBotServer)
		{
			Worker currentTarget = m_CurrentTarget;
			GameStateManager.Instance.StartSelectBuilding(BotServer.m_FirstBotServer);
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateBuildingSelect>().m_Menu.GetComponent<BotServerSelect>().SetTarget(currentTarget);
		}
	}

	public void OnClipboardClicked(BaseGadget NewGadget)
	{
		List<HudInstruction> list = new List<HudInstruction>();
		foreach (HighInstruction item in m_Instructions.m_List)
		{
			list.Insert(0, item.m_HudParent);
		}
		CreateDragInstructionShadows(list, SetShadow: false);
		ClipboardHelper.CopyToClipboard(m_FinalTexture);
	}

	public void OnNameInputChanged(BaseGadget NewGadget)
	{
		BaseInputField component = NewGadget.GetComponent<BaseInputField>();
		string text = component.GetText();
		bool flag = true;
		for (int i = 0; i < text.Length; i++)
		{
			if (text[i] != ' ')
			{
				flag = false;
				break;
			}
		}
		if (flag || text == "")
		{
			text = "---";
			component.SetText(text);
		}
		if ((bool)m_CurrentTarget)
		{
			m_CurrentTarget.SetWorkerName(text);
		}
	}

	private void SetNameEditEnabled(bool Enabled)
	{
		m_NameInputField.SetInteractable(Enabled);
	}

	public void OnNonEditableClicked(BaseGadget NewGadget)
	{
		DoNonEditable();
	}

	private void DoNonEditable()
	{
		m_NonEditable.SetActive(value: true);
		BaseText component = m_NonEditable.transform.Find("Text").GetComponent<BaseText>();
		if (m_State == State.Playing || m_State == State.PlayingWaiting)
		{
			component.SetTextFromID("ScriptNonEditableStop");
		}
		else
		{
			component.SetTextFromID("ScriptNonEditableRecord");
		}
		m_NonEditableTimer = 1.5f;
	}

	private void SetNonRecordingButtons()
	{
		m_DeleteButton.SetInteractable(Interactable: false);
		m_UndoButton.SetInteractable(Interactable: false);
		m_RepeatButton.SetInteractable(Interactable: false);
		m_RecordButton.SetInteractable(Interactable: false);
		m_CameraButton.SetInteractable(Interactable: true);
		m_TradeButton.SetInteractable(Interactable: true);
		m_StopButton.SetInteractable(Interactable: false);
	}

	private void DisableAllButtons()
	{
		m_TradeButton.SetInteractable(Interactable: false);
		m_RecordButton.SetInteractable(Interactable: false);
		m_UndoButton.SetInteractable(Interactable: false);
		m_RepeatButton.SetInteractable(Interactable: false);
		m_PlayButton.SetInteractable(Interactable: false);
		m_DeleteButton.SetInteractable(Interactable: false);
		m_StopButton.SetInteractable(Interactable: false);
	}

	private void UpdateButtons()
	{
		List<BaseClass> players = CollectionManager.Instance.GetPlayers();
		m_Locked = false;
		if ((bool)players[0])
		{
			FarmerPlayer component = players[0].GetComponent<FarmerPlayer>();
			m_Locked = component.GetIsDoingSomething();
		}
		if ((bool)m_CurrentTarget && m_CurrentTarget.m_State == Farmer.State.Held)
		{
			m_Locked = true;
		}
		m_TradeButton.SetLocked(m_Locked);
		m_RecordButton.SetLocked(m_Locked);
		m_CameraButton.SetLocked(m_Locked);
		m_UndoButton.SetLocked(m_Locked);
		m_RepeatButton.SetLocked(m_Locked);
		m_PlayButton.SetLocked(m_Locked);
		m_DeleteButton.SetLocked(m_Locked);
		m_StopButton.SetLocked(m_Locked);
		m_PlayButton.Update();
		m_ClipboardButton.m_Button.SetInteractable(m_Instructions.m_List.Count > 0);
	}

	private void SetPlayButton()
	{
		bool flag = true;
		m_RecordButton.SetInteractable(flag);
		if (m_CurrentTarget != null && m_CurrentTarget.m_WorkerInterpreter.m_HighInstructions.m_List.Count > 0)
		{
			if (flag)
			{
				UpdatePlayButton();
			}
			else
			{
				m_PlayButton.SetInteractable(Interactable: false);
			}
		}
		else
		{
			m_PlayButton.SetInteractable(Interactable: false);
		}
	}

	private void SetPlayingButtons()
	{
		m_RecordButton.SetInteractable(Interactable: false);
		m_PlayButton.SetSprite("Script/ScriptPause");
		m_PlayButton.m_Button.SetRolloverFromID("ScriptPauseButton");
		m_PlayButton.m_Button.m_OnClickSound = "ScriptingPause";
		m_PlayButton.SetInteractable(Interactable: true);
		m_StopButton.SetInteractable(Interactable: true);
		m_DropAllButton.SetInteractable(Interactable: false);
		m_CallButton.SetInteractable(Interactable: false);
	}

	private void SelectNonRecordingState()
	{
		if (m_CurrentTarget.m_WorkerInterpreter.GetCurrentScript() != null)
		{
			if (m_CurrentTarget.m_InterruptState == Farmer.State.Paused)
			{
				SetState(State.Paused);
			}
			else
			{
				SetState(State.Playing);
			}
		}
		else if (m_CurrentTarget.m_Energy == 0f)
		{
			SetState(State.Dead);
		}
		else
		{
			SetState(State.Busy);
		}
	}

	private void SetPanelColour()
	{
		Color color = GeneralUtils.ColorFromHex(m_PanelColours[(int)m_State]);
		GetPanel().SetBorderColour(color);
		m_ScrollViewBorder.color = color;
		Color color2 = GeneralUtils.ColorFromHex(m_TitleBarColours[(int)m_State]);
		SetTitleStripColour(color2);
		Color color3 = new Color(1f, 1f, 1f);
		color3 = (color3 - color2) * 0.5f + color2;
		m_ScrollViewGrid.color = color3;
	}

	private void SetStateIdle()
	{
		SetPlayButton();
	}

	private void EndStateRecording()
	{
		m_RecordButton.m_Button.SetBackingColour(new Color(1f, 1f, 1f));
		HudManager.Instance.StopPointingToTile();
		CollectionManager.Instance.GetPlayers()[0].GetComponent<FarmerPlayer>().m_FarmerPlayerWorkerSelect.SetActive(Active: false);
		m_NonEditablePanel.SetActive(Active: true);
		GetBackButton().SetRollover("");
		m_StopButton.m_Button.SetRolloverFromID("ScriptStopButton");
		m_InstructionPalette.gameObject.SetActive(value: false);
	}

	private void SetStateRecording()
	{
		m_TradeButton.SetInteractable(Interactable: false);
		m_RecordButton.SetInteractable(Interactable: false);
		m_CameraButton.SetInteractable(Interactable: false);
		m_UndoButton.SetInteractable(Interactable: false);
		m_RepeatButton.SetInteractable(Interactable: true);
		m_StopButton.SetInteractable(Interactable: true);
		m_RecordingTimer = 0f;
		CollectionManager.Instance.GetPlayers()[0].GetComponent<FarmerPlayer>().m_FarmerPlayerWorkerSelect.SetActive(Active: true);
		m_NonEditablePanel.SetActive(Active: false);
		GetBackButton().SetRolloverFromID("ScriptLoseChanges");
		m_StopButton.m_Button.SetRolloverFromID("ScriptStopButtonRecording");
		m_InstructionPalette.gameObject.SetActive(value: true);
	}

	private void EndStatePlaying()
	{
		m_PlayButton.SetSprite("Script/ScriptGo");
		m_PlayButton.m_Button.SetRolloverFromID("ScriptGoButton");
		m_PlayButton.m_Button.m_OnClickSound = "UIOptionSelected";
		m_DropAllButton.SetInteractable(Interactable: true);
		m_CallButton.SetInteractable(Interactable: true);
	}

	private void SetStatePlaying()
	{
		SetPlayingButtons();
		if (m_CurrentTarget != null)
		{
			m_Instructions.CopyLineNumbers(m_CurrentTarget.m_WorkerInterpreter.m_HighInstructions);
		}
	}

	private void SetStatePlayingWaiting()
	{
		SetPlayingButtons();
	}

	private void EndStatePaused()
	{
		m_PlayButton.SetFlashing(Flash: false);
		m_DropAllButton.SetInteractable(Interactable: true);
		m_CallButton.SetInteractable(Interactable: true);
	}

	private void SetStatePaused()
	{
		m_PlayButton.SetInteractable(Interactable: true);
		m_PlayButton.SetFlashing(Flash: true);
		m_DropAllButton.SetInteractable(Interactable: false);
		m_CallButton.SetInteractable(Interactable: false);
	}

	private void EndStateDead()
	{
		m_ScrollViewDeadPanel.gameObject.SetActive(value: false);
	}

	private void SetStateDead()
	{
		DisableAllButtons();
		m_ScrollViewDeadPanel.gameObject.SetActive(value: true);
	}

	private void SetStateBusy()
	{
		m_RecordButton.SetInteractable(Interactable: false);
		m_PlayButton.SetInteractable(Interactable: false);
		m_TradeButton.SetInteractable(Interactable: false);
	}

	private void EndStateBusy()
	{
		m_RecordButton.SetInteractable(Interactable: true);
		UpdatePlayButton();
		m_TradeButton.SetInteractable(Interactable: true);
	}

	private void SetState(State NewState)
	{
		switch (m_State)
		{
		case State.Recording:
			EndStateRecording();
			break;
		case State.Playing:
			EndStatePlaying();
			break;
		case State.PlayingWaiting:
			EndStatePlaying();
			break;
		case State.Paused:
			EndStatePaused();
			break;
		case State.Busy:
			EndStateBusy();
			break;
		case State.Dead:
			EndStateDead();
			break;
		}
		m_State = NewState;
		if (m_State != State.Recording)
		{
			SetNonRecordingButtons();
		}
		switch (m_State)
		{
		case State.Idle:
			SetStateIdle();
			break;
		case State.Recording:
			SetStateRecording();
			break;
		case State.Playing:
			SetStatePlaying();
			break;
		case State.PlayingWaiting:
			SetStatePlayingWaiting();
			break;
		case State.Paused:
			SetStatePaused();
			break;
		case State.Busy:
			SetStateBusy();
			break;
		case State.Dead:
			SetStateDead();
			break;
		}
		SetPanelColour();
	}

	private void CheckDead()
	{
		if (m_CurrentTarget.m_Energy == 0f)
		{
			SetState(State.Dead);
		}
	}

	private void UpdatePlayButton()
	{
		if (GetFreeMemory() >= 0)
		{
			m_PlayButton.SetInteractable(Interactable: true);
		}
		else
		{
			m_PlayButton.SetInteractable(Interactable: false);
		}
	}

	private void UpdateStateIdle()
	{
		UpdatePlayButton();
	}

	private void UpdateStateRecording()
	{
		CollectionManager.Instance.GetPlayers();
		if (m_Instructions.m_List.Count > 0)
		{
			m_DeleteButton.SetInteractable(Interactable: true);
		}
		else
		{
			m_DeleteButton.SetInteractable(Interactable: false);
		}
		if (m_Instructions.m_List.Count > 1 && !GetCanTakeMoreInstructions())
		{
			m_RepeatButton.SetInteractable(Interactable: false);
		}
		else
		{
			m_RepeatButton.SetInteractable(Interactable: true);
		}
		Color backingColour = new Color(1f, 1f, 1f);
		m_RecordingTimer += TimeManager.Instance.m_NormalDeltaUnscaled;
		if ((int)(m_RecordingTimer * 60f) % 20 < 10)
		{
			backingColour = new Color(1f, 0f, 0f);
		}
		m_RecordButton.m_Button.SetBackingColour(backingColour);
		UpdatePlayButton();
	}

	private void UpdateStatePlaying()
	{
		CheckDead();
		if (m_CurrentTarget.m_WorkerInterpreter.GetCurrentScript() == null)
		{
			SelectNonRecordingState();
		}
		if (m_CurrentTarget.m_WorkerIndicator.m_State == WorkerStatusIndicator.State.Question)
		{
			SetState(State.PlayingWaiting);
		}
	}

	private void UpdateStatePlayingWaiting()
	{
		CheckDead();
		if (m_CurrentTarget.m_WorkerIndicator.m_State != WorkerStatusIndicator.State.Question)
		{
			SetState(State.Playing);
		}
	}

	private void UpdateStateDead()
	{
		if (m_CurrentTarget.m_Energy != 0f)
		{
			SelectNonRecordingState();
		}
	}

	private void UpdateStateBusy()
	{
		if (m_CurrentTarget.m_State == Farmer.State.Moving || m_CurrentTarget.m_State == Farmer.State.None || m_CurrentTarget.m_State == Farmer.State.RequestMove)
		{
			SetState(State.Idle);
		}
		if (m_CurrentTarget.m_State == Farmer.State.Engaged && Vehicle.GetIsTypeVehicle(m_CurrentTarget.m_EngagedObject.m_TypeIdentifier) && !m_CurrentTarget.m_EngagedObject.GetComponent<Vehicle>().GetIsBusy())
		{
			SetState(State.Idle);
		}
	}

	private void UpdateState()
	{
		switch (m_State)
		{
		case State.Idle:
			UpdateStateIdle();
			break;
		case State.Recording:
			UpdateStateRecording();
			break;
		case State.Playing:
			UpdateStatePlaying();
			break;
		case State.PlayingWaiting:
			UpdateStatePlayingWaiting();
			break;
		case State.Busy:
			UpdateStateBusy();
			break;
		case State.Dead:
			UpdateStateDead();
			break;
		case State.Paused:
			break;
		}
	}

	private void UpdateExpanded()
	{
		float num = 0f;
		if (m_Expanded)
		{
			num = 200f;
		}
		GetComponent<RectTransform>().sizeDelta = new Vector2(m_DefaultWidth + num, GetComponent<RectTransform>().sizeDelta.y);
		string rolloverFromID = "ScriptExpandButton";
		if (m_Expanded)
		{
			rolloverFromID = "ScriptShrinkButton";
		}
		m_ExpandButton.m_Button.SetRolloverFromID(rolloverFromID);
		if (m_Expanded)
		{
			m_ExpandButton.m_Button.SetBackingSprite("Script/CollapseButtonBacking");
			m_ExpandButton.m_Button.SetBorderSprite("Script/CollapseButtonBorder");
			m_ExpandButton.m_Button.SetShadowSprite("Script/CollapseButtonShadow");
		}
		else
		{
			m_ExpandButton.m_Button.SetBackingSprite("Script/ExpandButtonBacking");
			m_ExpandButton.m_Button.SetBorderSprite("Script/ExpandButtonBorder");
			m_ExpandButton.m_Button.SetShadowSprite("Script/ExpandButtonShadow");
		}
	}

	private void UpdateName()
	{
		if (m_CurrentTarget != null)
		{
			m_NameInputField.SetText(m_CurrentTarget.GetWorkerName());
		}
	}

	private void UpdateFollow()
	{
		if (m_Followed && m_State != State.Recording)
		{
			CameraManager.Instance.TrackObject(m_CurrentTarget.gameObject);
		}
		else if (CameraManager.Instance.m_State == CameraManager.State.TrackObject)
		{
			CameraManager.Instance.SetState(CameraManager.State.Normal);
		}
	}

	private void SetUndoAvailable(bool Available)
	{
		m_UndoAvailable = Available;
		m_UndoButton.SetInteractable(Available);
	}

	private void UpdateCameraButton()
	{
		if (!m_CameraButton.m_Button.GetInteractable())
		{
			return;
		}
		Color backingColour = new Color(1f, 1f, 1f);
		if (m_Followed)
		{
			m_FollowTimer += TimeManager.Instance.m_NormalDeltaUnscaled;
			if ((int)(m_FollowTimer * 60f) % 20 < 10)
			{
				backingColour = new Color(1f, 0f, 0f);
			}
		}
		m_CameraButton.m_Button.SetBackingColour(backingColour);
	}

	private void UpdateTitle()
	{
		int freeMemory = GetFreeMemory();
		string titleText = TextManager.Instance.Get("BrainTitle", freeMemory.ToString());
		SetTitleText(titleText);
		if (freeMemory < 0 || m_FlashMemoryTimer > 0f)
		{
			m_FlashMemoryTimer -= TimeManager.Instance.m_NormalDelta;
			m_MemoryTimer += TimeManager.Instance.m_NormalDeltaUnscaled;
			if ((int)(m_MemoryTimer * 60f) % 20 < 10)
			{
				SetTitleColour(new Color(1f, 1f, 1f));
			}
			else
			{
				SetTitleColour(new Color(0.2f, 0.2f, 0.2f));
			}
		}
		else
		{
			SetTitleColour(new Color(0.2f, 0.2f, 0.2f));
		}
	}

	private void UpdateOutOfRange()
	{
	}

	private void OldAwake()
	{
		if (!(HudManager.Instance == null))
		{
			m_Panel = base.transform.Find("Panel").gameObject;
			m_CanvasParent = HudManager.Instance.m_MenusRootTransform;
			m_InstructionPanel = m_ScrollView.gameObject;
			m_ContentParent = m_ScrollView.GetContent().transform;
			m_Viewport = m_ScrollView.viewport.gameObject;
			m_MemoryTimer = 0f;
			Transform parent = base.transform.Find("Panel");
			GameObject gameObject = (GameObject)Resources.Load("Prefabs/Hud/Scripting/Instruction", typeof(GameObject));
			m_RepeatInstruction = Object.Instantiate(gameObject, new Vector3(0f, 0f, 0f), gameObject.transform.localRotation, parent).GetComponent<HudInstruction>();
			m_RepeatInstruction.transform.localPosition = new Vector3(165f, -269f, 0f);
			m_RepeatInstruction.SetParent(this);
			m_RepeatInstruction.SetStatic();
			m_RepeatInstruction.SetInstruction(new HighInstruction(HighInstruction.Type.Repeat, null));
			m_RepeatInstruction.gameObject.SetActive(value: false);
			gameObject = (GameObject)Resources.Load("Prefabs/Hud/Scripting/InstructionCursor", typeof(GameObject));
			m_InstructionCursor = Object.Instantiate(gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity, HudManager.Instance.m_MenusRootTransform);
			m_InstructionCursor.SetActive(value: false);
			gameObject = (GameObject)Resources.Load("Prefabs/Hud/Scripting/InstructionCurrentCursor", typeof(GameObject));
			m_ExecutingCursor = Object.Instantiate(gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity, HudManager.Instance.m_MenusRootTransform);
			m_ExecutingCursor.SetActive(value: false);
			gameObject = (GameObject)Resources.Load("Prefabs/Hud/Scripting/ScriptDragParent", typeof(GameObject));
			m_DragParent = Object.Instantiate(gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity, HudManager.Instance.m_RootTransform);
			m_MouseObject = base.transform.Find("Mouse").gameObject;
			m_MouseObject.transform.SetParent(HudManager.Instance.m_MenusRootTransform);
			m_InstructionShadow = base.transform.Find("InstructionShadow").gameObject;
			m_InstructionShadow.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
			m_InstructionShadow.SetActive(value: false);
			m_InstructionPalette = base.transform.Find("InstructionPaletteSmall").GetComponent<InstructionPaletteSmall>();
			m_InstructionPalette.SetParent(this);
			m_InstructionPalette.gameObject.SetActive(value: false);
			m_ViewportHover = false;
			m_HoverInstruction = null;
			m_Instructions = new HighInstructionList();
			m_UndoInstructions = new HighInstructionList();
			m_DragSelectList = new List<HudInstruction>();
			m_DragInstructions = new List<HudInstruction>();
			m_Locked = false;
			m_PointAtObjectSearch = null;
			m_PanelRect = m_Panel.GetComponent<RectTransform>().rect;
			m_InstructionsRect = m_InstructionPanel.GetComponent<RectTransform>().rect;
			m_PlayCeremony = false;
			m_Backup = false;
			m_Hover = false;
			m_RefreshInstruction = false;
			m_NewDeleteInstruction = null;
			m_NewInstructions = new List<HighInstruction>();
			m_Followed = false;
			UpdateFollow();
			m_Wobbler = new Wobbler();
			UpdateNormalPosition();
			m_Pushed = false;
			int width = 32;
			int height = 32;
			m_RenderTexture = new RenderTexture(width, height, 32);
			m_FinalTexture = new Texture2D(width, height, TextureFormat.RGBA32, mipChain: false);
		}
	}

	private void OnDestroy()
	{
		if (!(HudManager.Instance == null))
		{
			if (m_RenderTexture != null)
			{
				Object.Destroy(m_RenderTexture);
			}
			if (m_FinalTexture != null)
			{
				Object.Destroy(m_FinalTexture);
			}
			if (m_InstructionShadow != null)
			{
				Object.Destroy(m_InstructionShadow.gameObject);
			}
			if ((bool)CameraManager.Instance.m_FollowWorker)
			{
				CameraManager.Instance.m_FollowWorker.SetActive(value: false);
			}
			HudManager.Instance.StopPointingToTile();
			HudManager.Instance.ActivateUIRollover(Activate: false, "", default(Vector3));
			Object.Destroy(m_InstructionCursor.gameObject);
			Object.Destroy(m_ExecutingCursor.gameObject);
			Object.Destroy(m_DragParent.gameObject);
			m_Instructions.ScaleAreaIndicators(Up: false);
			m_UndoInstructions.ShowAreaIndicators(Show: false);
		}
	}

	public void Pushed()
	{
		if (base.gameObject.activeSelf)
		{
			if ((bool)CameraManager.Instance.m_FollowWorker)
			{
				CameraManager.Instance.m_FollowWorker.SetActive(value: false);
			}
			m_Hover = false;
			HudManager.Instance.ActivateUIRollover(Activate: false, "", default(Vector3));
			if (!m_PlayCeremony)
			{
				base.gameObject.SetActive(value: false);
			}
			DragSelectListClear();
			m_Pushed = true;
		}
	}

	public void Popped()
	{
		if (!m_Pushed)
		{
			return;
		}
		m_Pushed = false;
		if (!m_PlayCeremony)
		{
			base.gameObject.SetActive(value: true);
			UpdateName();
		}
		else
		{
			m_PlayCeremony = false;
			if (m_Backup)
			{
				DoCopy();
			}
			else
			{
				DoPaste();
			}
		}
		m_Instructions.MakeAreaIndicators();
	}

	public void RefreshInstructions()
	{
		m_RefreshInstruction = true;
	}

	public void UpdateTargetInstructions()
	{
		if (m_Instructions.m_List.Count > 0)
		{
			DestroyHudInstruction(m_Instructions.m_List);
			m_Instructions.m_List.Clear();
		}
		base.transform.localScale = new Vector2(1f, 1f);
		TeachWorkerScriptController.Instance.GetComponent<RectTransform>().anchoredPosition = m_NormalScreenPosition;
		if (m_CurrentTarget.m_WorkerInterpreter.m_HighInstructions.m_List.Count > 0)
		{
			m_Instructions.Copy(m_CurrentTarget.m_WorkerInterpreter.m_HighInstructions);
			CreateHudInstructions();
		}
		if (m_State == State.Idle)
		{
			SetPlayButton();
		}
	}

	private void SetBotSelect(bool BotSelect)
	{
		m_BotSelect = BotSelect;
		float num = 115f;
		float num2 = 660f;
		if (m_BotSelect)
		{
			DisableAllButtons();
			num2 -= num;
			num = 0f;
		}
		SetNameEditEnabled(!BotSelect);
		RectTransform component = m_ScrollView.GetComponent<RectTransform>();
		component.offsetMin = new Vector2(component.offsetMin.x, num + 25f);
		m_ButtonsRoot.gameObject.SetActive(!m_BotSelect);
		GetComponent<RectTransform>();
	}

	public void SetTarget(Worker Target, bool BotSelect = false)
	{
		m_Followed = false;
		UpdateFollow();
		if (Target == null)
		{
			if (base.gameObject.activeSelf && (!m_Scaling || m_ScalingUp))
			{
				StartScaling(Up: false);
			}
			m_CurrentTarget = null;
			CameraManager.Instance.m_FollowWorker.SetActive(value: false);
			HudManager.Instance.StopPointingToTile();
		}
		else if (!(m_CurrentTarget == Target))
		{
			Worker currentTarget = m_CurrentTarget;
			m_CurrentTarget = Target;
			if (CheatManager.Instance.m_DrainBot && m_CurrentTarget.m_Energy > 5f)
			{
				m_CurrentTarget.m_Energy = 5f;
			}
			UpdateName();
			UpdateTitle();
			UpdateTargetInstructions();
			UpdateExpanded();
			if (!base.gameObject.activeSelf || (m_Scaling && !m_ScalingUp) || currentTarget != m_CurrentTarget)
			{
				StartScaling(Up: true);
			}
			UpdateScalingTransform();
			CameraManager.Instance.m_FollowWorker.SetActive(value: true);
			CameraManager.Instance.m_FollowWorker.GetComponent<MeshRenderer>().material.color = GeneralUtils.GetIndicatorColour();
			SelectNonRecordingState();
			SetBotSelect(BotSelect);
			TutorialScriptManager.Instance.NewInstructions();
		}
	}

	public void UpdateBuildingMoved(int UID, TileCoord NewPosition)
	{
		foreach (HighInstruction item in m_Instructions.m_List)
		{
			item.ChangeUIDLocation(UID, NewPosition, Found: false);
		}
	}

	private void CopyInstructions()
	{
		m_UndoInstructions.Copy(m_Instructions);
		SetUndoAvailable(Available: true);
	}

	private void RevertInstructions()
	{
		DragSelectListClear();
		DestroyHudInstruction(m_Instructions.m_List);
		m_Instructions.Copy(m_UndoInstructions);
		SetUndoAvailable(Available: false);
		CreateHudInstructions();
		TutorialScriptManager.Instance.TeachingInstructionsChanged();
	}

	public void SetInstructions(HighInstructionList NewInstructions)
	{
		DragSelectListClear();
		DestroyHudInstruction(m_Instructions.m_List);
		m_Instructions.Copy(NewInstructions);
		SetUndoAvailable(Available: false);
		CreateHudInstructions();
	}

	public void SetTeaching(bool Teaching)
	{
		if (Teaching)
		{
			SetState(State.Recording);
		}
		else
		{
			SelectNonRecordingState();
		}
		foreach (HighInstruction item in m_Instructions.m_List)
		{
			item.m_HudParent.EnableInteraction(Teaching);
		}
	}

	public void OnClickBack()
	{
		if (GameStateManager.Instance.GetCurrentState() == null || m_CurrentTarget == null)
		{
			return;
		}
		State state = m_State;
		m_InstructionShadow.SetActive(value: false);
		foreach (HudInstruction dragInstruction in m_DragInstructions)
		{
			dragInstruction.SetDragging(Dragging: false);
			DestroyHudInstruction(dragInstruction.m_Instruction.m_Children);
			DestroyHudInstruction(dragInstruction.m_Instruction.m_Children2);
			Object.DestroyImmediate(dragInstruction.gameObject);
		}
		m_DragInstructions.Clear();
		m_InstructionCursor.SetActive(value: false);
		if (m_CurrentTarget.GetComponent<Worker>().m_Learning)
		{
			m_CurrentTarget.RequestStopLearning();
		}
		CollectionManager.Instance.GetPlayers()[0].GetComponent<FarmerPlayer>().UseWhistle(FarmerPlayer.WhistleCall.Cancel);
		if ((bool)GameStateManager.Instance.GetCurrentState().GetComponent<GameStateTeachWorker2>())
		{
			Worker currentTarget = GameStateManager.Instance.GetCurrentState().GetComponent<GameStateTeachWorker2>().m_CurrentTarget;
			GameStateTeachWorker2.Instance.Close(StartScript: false);
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateNormal>().SetSelectedWorker(currentTarget, Confirmed: false, Scale: false);
			UpdateTargetInstructions();
			m_Instructions.ShowAreaIndicators(Show: true);
		}
		else if ((bool)GameStateManager.Instance.GetCurrentState().GetComponent<GameStateNormal>())
		{
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateNormal>().ClearSelectedWorkers();
		}
		if (state == State.Recording)
		{
			TutorialScriptManager.Instance.EndTeaching();
		}
	}

	public void OnClickGo()
	{
		if (m_CurrentTarget == null)
		{
			return;
		}
		if (m_CurrentTarget.m_WorkerInterpreter.GetCurrentScript() != null)
		{
			m_CurrentTarget.TogglePauseScript();
			SelectNonRecordingState();
			return;
		}
		DragSelectListClear();
		if (m_Locked)
		{
			return;
		}
		HudManager.Instance.ActivateUIRollover(Activate: false, "", default(Vector3));
		if (m_State == State.Recording)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.BotTeach, Bot: false, 0, m_CurrentTarget);
		}
		QuestManager.Instance.AddEvent(QuestEvent.Type.ClickPlay, Bot: false, 0, m_CurrentTarget);
		if (m_State == State.Recording)
		{
			Worker currentTarget = m_CurrentTarget;
			m_CurrentTarget.RequestStopLearning();
			GameStateTeachWorker2.Instance.Close(StartScript: true);
			TutorialScriptManager.Instance.EndTeaching();
			currentTarget.NewHighScriptTaught(m_Instructions.m_List);
			AudioManager.Instance.StartEvent("ScriptingGoSelected");
		}
		else
		{
			AudioManager.Instance.StartEvent("UIOptionSelected");
			if (m_CurrentTarget.m_WorkerInterpreter.GetCurrentScript() == null)
			{
				m_CurrentTarget.NewHighScriptTaught(m_Instructions.m_List);
			}
			else
			{
				m_CurrentTarget.StopAllScripts();
			}
		}
		SetState(State.Playing);
	}

	public void OnClickRepeat()
	{
		if (!(m_CurrentTarget == null))
		{
			OnInstructionClick(m_RepeatInstruction);
		}
	}

	public void OnClickTrade()
	{
		if (!(m_CurrentTarget == null))
		{
			DragSelectListClear();
			if (!m_Locked)
			{
				m_Hover = false;
				AudioManager.Instance.StartEvent("UIOptionSelected");
				Worker currentTarget = m_CurrentTarget;
				GameStateManager.Instance.PushState(GameStateManager.State.Inventory);
				List<BaseClass> players = CollectionManager.Instance.GetPlayers();
				GameStateManager.Instance.GetCurrentState().GetComponent<GameStateInventory>().SetInfo(players[0].GetComponent<FarmerPlayer>(), currentTarget);
			}
		}
	}

	public void OnClickTeach()
	{
		DragSelectListClear();
		if (!m_Locked && !(m_CurrentTarget == null))
		{
			AudioManager.Instance.StartEvent("WorkerAcknowledgeLearn", m_CurrentTarget);
			FarmerPlayer component = CollectionManager.Instance.GetPlayers()[0].GetComponent<FarmerPlayer>();
			m_CurrentTarget.StartLearning(component);
			AudioManager.Instance.StartEvent("UIOptionSelected");
			Worker currentTarget = m_CurrentTarget;
			GameStateManager.Instance.SetState(GameStateManager.State.TeachWorker);
			if (GameStateManager.Instance.GetActualState() == GameStateManager.State.TeachWorker)
			{
				GameStateManager.Instance.GetCurrentState().GetComponent<GameStateTeachWorker2>().SetTarget(currentTarget);
				QuestManager.Instance.AddEvent(QuestEvent.Type.ClickRecord, Bot: false, 0, currentTarget);
				SetState(State.Recording);
			}
		}
	}

	public void OnClickPalette()
	{
		if (!(m_CurrentTarget == null))
		{
			DragSelectListClear();
			if (!m_Locked)
			{
				m_InstructionPalette.ToggleExpanded();
				AudioManager.Instance.StartEvent("UIOptionSelected");
			}
		}
	}

	public void OnClickClear()
	{
		if (!(m_CurrentTarget == null))
		{
			DragSelectListClear();
			if (!m_Locked)
			{
				CopyInstructions();
				DestroyHudInstruction(m_Instructions.m_List);
				m_Instructions.Clear();
				CreateHudInstructions();
				TutorialScriptManager.Instance.TeachingInstructionsChanged();
			}
		}
	}

	public void Clear()
	{
		DestroyHudInstruction(m_Instructions.m_List);
		m_Instructions.Clear();
		CreateHudInstructions();
	}

	public void OnClickFollow()
	{
		if (!(m_CurrentTarget == null))
		{
			AudioManager.Instance.StartEvent("UIOptionSelected");
			if (m_CurrentTarget.m_WorkerInterpreter.GetCurrentScript() == null)
			{
				CameraManager.Instance.Focus(m_CurrentTarget.transform.position);
				return;
			}
			m_Followed = !m_Followed;
			UpdateFollow();
		}
	}

	public void OnClickDropAll()
	{
		CollectionManager.Instance.GetPlayers()[0].GetComponent<FarmerPlayer>().UseWhistle(FarmerPlayer.WhistleCall.DropAll);
		if (!(m_CurrentTarget == null) && m_CurrentTarget.m_WorkerInterpreter.GetCurrentScript() == null && !m_CurrentTarget.GetIsDoingSomething())
		{
			m_CurrentTarget.m_FarmerCarry.DropAllObjects();
			m_CurrentTarget.m_FarmerInventory.DropAllObjects();
		}
	}

	public void OnClickCall()
	{
		CollectionManager.Instance.GetPlayers()[0].GetComponent<FarmerPlayer>().UseWhistle(FarmerPlayer.WhistleCall.ToMe);
		if (!(m_CurrentTarget == null) && (bool)m_CurrentTarget && m_CurrentTarget.m_WorkerInterpreter.GetCurrentScript() == null)
		{
			List<BaseClass> players = CollectionManager.Instance.GetPlayers();
			m_CurrentTarget.GoToPlayer(players[0].GetComponent<FarmerPlayer>());
		}
	}

	public new void SetActive(bool Active)
	{
		base.gameObject.SetActive(Active);
		m_Instructions.ShowAreaIndicators(Active);
		m_Followed = false;
		UpdateFollow();
	}

	public void DoCopy()
	{
	}

	public void OnClickCopy()
	{
		DragSelectListClear();
		if (!m_Locked)
		{
			m_PlayCeremony = true;
			m_Backup = true;
			Worker currentTarget = m_CurrentTarget;
			GameStateManager.Instance.PushState(GameStateManager.State.BackupRestore);
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateBackupRestore>().SetBackup(Backup: true);
			SetTarget(currentTarget);
		}
	}

	public void DoPaste()
	{
	}

	public void OnClickPaste()
	{
		DragSelectListClear();
		if (!m_Locked)
		{
			m_PlayCeremony = true;
			m_Backup = false;
			Worker currentTarget = m_CurrentTarget;
			GameStateManager.Instance.PushState(GameStateManager.State.BackupRestore);
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateBackupRestore>().SetBackup(Backup: false);
			SetTarget(currentTarget);
		}
	}

	public void OnClickStop()
	{
		if (m_CurrentTarget == null)
		{
			return;
		}
		if (m_State == State.Recording)
		{
			DragSelectListClear();
			m_CurrentTarget.RequestStopLearning();
			GameStateTeachWorker2.Instance.Close(StartScript: true);
			TutorialScriptManager.Instance.EndTeaching();
			if ((bool)m_CurrentTarget)
			{
				m_CurrentTarget.NewHighScriptTaught(m_Instructions.m_List, Start: false);
			}
			QuestManager.Instance.AddEvent(QuestEvent.Type.ClickStop, Bot: false, 0, m_CurrentTarget);
			QuestManager.Instance.AddEvent(QuestEvent.Type.BotTeach, Bot: false, 0, m_CurrentTarget);
			AudioManager.Instance.StartEvent("ScriptingGoSelected");
		}
		else if (m_CurrentTarget.m_WorkerInterpreter.GetCurrentScript() != null)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.ClickStop, Bot: false, 0, m_CurrentTarget);
			AudioManager.Instance.StartEvent("UIOptionSelected");
			m_CurrentTarget.StopAllScripts();
			m_Followed = false;
			UpdateFollow();
			SelectNonRecordingState();
		}
	}

	public void OnClickUndo()
	{
		if (!(m_CurrentTarget == null) && m_UndoAvailable)
		{
			RevertInstructions();
		}
	}

	public void OnMouseEnter(Button ThisButton)
	{
		AudioManager.Instance.StartEvent("UIOptionIndicated");
		ThisButton.GetComponent<Image>().color = ThisButton.colors.highlightedColor;
		Vector3 position = HudManager.Instance.ConvertUIToScreenSpace(default(Vector3), ThisButton.transform);
		HudManager.Instance.ActivateUIRollover(Activate: true, "Test", position);
	}

	public void OnMouseExit(Button ThisButton)
	{
		ThisButton.GetComponent<Image>().color = ThisButton.colors.normalColor;
		HudManager.Instance.ActivateUIRollover(Activate: false, "", ThisButton.transform.position);
	}

	public void OnMouseEnter()
	{
		AudioManager.Instance.StartEvent("UIOptionIndicated");
		m_Hover = true;
	}

	public void OnMouseExit()
	{
		AudioManager.Instance.StartEvent("UIOptionIndicated");
		if (!IsDragging())
		{
			m_Hover = false;
		}
	}

	public void OnMouseEnterContent()
	{
	}

	public void OnMouseExitContent()
	{
	}

	private void DragSelectListClear()
	{
		foreach (HudInstruction dragSelect in m_DragSelectList)
		{
			if ((bool)dragSelect)
			{
				dragSelect.SetSelected(Selected: false);
			}
		}
		m_DragSelectList.Clear();
	}

	private void DragSelectListAdd(HudInstruction AddInstruction)
	{
		for (int i = 0; i < m_DragSelectList.Count; i++)
		{
			HudInstruction hudInstruction = m_DragSelectList[i];
			if (AddInstruction.m_Instruction.m_Children.Contains(hudInstruction.m_Instruction) || AddInstruction.m_Instruction.m_Children2.Contains(hudInstruction.m_Instruction))
			{
				DragSelectListRemove(hudInstruction);
				i--;
			}
		}
		AddInstruction.SetSelected(Selected: true);
		m_DragSelectList.Add(AddInstruction);
		AudioManager.Instance.StartEvent("ScriptingInstructionAdded");
	}

	private void DragSelectListRemove(HudInstruction NewInstruction)
	{
		NewInstruction.SetSelected(Selected: false);
		m_DragSelectList.Remove(NewInstruction);
		AudioManager.Instance.StartEvent("ScriptingInstructionRemoved");
	}

	private bool DragSelectListCheckContains(HighInstruction TestInstruction, List<HighInstruction> Instructions)
	{
		foreach (HighInstruction Instruction in Instructions)
		{
			if (Instruction.m_Children.Contains(TestInstruction))
			{
				return true;
			}
			if (Instruction.m_Children2.Contains(TestInstruction))
			{
				return true;
			}
			if (DragSelectListCheckContains(TestInstruction, Instruction.m_Children))
			{
				return true;
			}
			if (DragSelectListCheckContains(TestInstruction, Instruction.m_Children2))
			{
				return true;
			}
		}
		return false;
	}

	private bool TestDragSelectListCheckContains(HudInstruction TestInstruction)
	{
		List<HighInstruction> list = new List<HighInstruction>();
		foreach (HudInstruction dragSelect in m_DragSelectList)
		{
			list.Add(dragSelect.m_Instruction);
		}
		return DragSelectListCheckContains(TestInstruction.m_Instruction, list);
	}

	public void InstructionDelete(HudInstruction NewInstruction)
	{
		m_NewDeleteInstruction = NewInstruction;
	}

	private bool GetContainsInstruction(HudInstruction NewInstruction)
	{
		if (m_DragSelectList.Contains(NewInstruction))
		{
			return true;
		}
		foreach (HudInstruction dragSelect in m_DragSelectList)
		{
			if (dragSelect.m_Instruction.Contains(NewInstruction.m_Instruction))
			{
				return true;
			}
		}
		return false;
	}

	private void UpdateInstructionDelete(HudInstruction NewInstruction)
	{
		if (GetContainsInstruction(NewInstruction))
		{
			foreach (HudInstruction dragSelect in m_DragSelectList)
			{
				m_Instructions.Remove(dragSelect.m_Instruction);
				dragSelect.m_Instruction.m_Parent = null;
				DestroyHudInstruction(dragSelect.m_Instruction.m_Children);
				DestroyHudInstruction(dragSelect.m_Instruction.m_Children2);
				Object.DestroyImmediate(dragSelect.gameObject);
			}
			m_DragSelectList.Clear();
		}
		else
		{
			List<HighInstruction> list = new List<HighInstruction>();
			foreach (HighInstruction child in NewInstruction.m_Instruction.m_Children)
			{
				list.Add(child);
			}
			foreach (HighInstruction item in NewInstruction.m_Instruction.m_Children2)
			{
				list.Add(item);
			}
			HighInstruction Parent;
			bool Children;
			int instructionInsertLine = GetInstructionInsertLine(NewInstruction, out Parent, out Children);
			DestroyHudInstruction(list);
			foreach (HighInstruction item2 in list)
			{
				m_Instructions.Remove(item2);
				item2.m_Parent = null;
			}
			m_Instructions.Remove(NewInstruction.m_Instruction);
			NewInstruction.m_Instruction.m_Parent = null;
			Object.DestroyImmediate(NewInstruction.gameObject);
			int num = instructionInsertLine;
			foreach (HighInstruction item3 in list)
			{
				if (Parent == null)
				{
					m_Instructions.Insert(num, item3);
				}
				else
				{
					m_Instructions.InsertToChild(Parent, num, item3, Children);
				}
				num++;
			}
		}
		DragSelectListClear();
		CreateHudInstructions();
		TutorialScriptManager.Instance.TeachingInstructionsChanged();
	}

	private static int SortInstructionByY(HudInstruction p1, HudInstruction p2)
	{
		return (int)((p1.transform.position.y - p2.transform.position.y) * 100f);
	}

	private static int SortInstructionByMinusY(HudInstruction p1, HudInstruction p2)
	{
		return (int)((p2.transform.position.y - p1.transform.position.y) * 100f);
	}

	private void RepeatSelectedInstructions()
	{
		if (!GetCanTakeMoreInstructions())
		{
			return;
		}
		m_DragSelectList.Sort(SortInstructionByMinusY);
		List<HighInstruction> list = new List<HighInstruction>();
		foreach (HudInstruction dragSelect in m_DragSelectList)
		{
			list.Add(dragSelect.m_Instruction);
		}
		HighInstruction highInstruction = list[0];
		HighInstruction Parent;
		bool Children;
		int instructionInsertLine = GetInstructionInsertLine(highInstruction.m_HudParent, out Parent, out Children);
		DestroyHudInstruction(m_Instructions.m_List);
		m_DragSelectList.Clear();
		foreach (HighInstruction item in list)
		{
			m_Instructions.Remove(item);
		}
		string nameFromConditionType = HighInstruction.GetNameFromConditionType(HighInstruction.Type.Repeat, HighInstruction.ConditionType.HandsFull);
		HighInstruction highInstruction2 = new HighInstruction(HighInstruction.Type.Repeat, null);
		highInstruction2.m_ActionInfo.m_Value = "1";
		highInstruction2.m_Argument = nameFromConditionType;
		if (Parent == null)
		{
			m_Instructions.Insert(instructionInsertLine, highInstruction2);
		}
		else
		{
			m_Instructions.InsertToChild(Parent, instructionInsertLine, highInstruction2, Children);
		}
		int num = 0;
		foreach (HighInstruction item2 in list)
		{
			m_Instructions.InsertToChild(highInstruction2, num, item2, Children2: false);
			num++;
		}
		CreateHudInstructions();
		TutorialScriptManager.Instance.TeachingInstructionsChanged();
		AudioManager.Instance.StartEvent("ScriptingForeverAdded");
	}

	public void ClickRepeat()
	{
		QuestManager.Instance.AddEvent(QuestEvent.Type.ClickRepeat, Bot: false, 0, m_CurrentTarget);
		if (m_DragSelectList.Count > 0)
		{
			RepeatSelectedInstructions();
			return;
		}
		string nameFromConditionType = HighInstruction.GetNameFromConditionType(HighInstruction.Type.Repeat, HighInstruction.ConditionType.Forever);
		if (m_Instructions.m_List.Count != 1 || m_Instructions.m_List[0].m_Type != HighInstruction.Type.Repeat || m_Instructions.m_List[0].m_Argument != nameFromConditionType)
		{
			if (!GetCanTakeMoreInstructions())
			{
				return;
			}
			List<HighInstruction> list = new List<HighInstruction>();
			foreach (HighInstruction item in m_Instructions.m_List)
			{
				list.Add(item);
			}
			DestroyHudInstruction(m_Instructions.m_List);
			m_Instructions.Clear();
			HighInstruction highInstruction = new HighInstruction(HighInstruction.Type.Repeat, null);
			highInstruction.m_ActionInfo.m_Value = "1";
			highInstruction.m_Argument = nameFromConditionType;
			m_Instructions.Add(highInstruction);
			foreach (HighInstruction item2 in list)
			{
				m_Instructions.AddToChild(highInstruction, item2, Children2: false);
			}
			CreateHudInstructions();
			TutorialScriptManager.Instance.TeachingInstructionsChanged();
			AudioManager.Instance.StartEvent("ScriptingForeverAdded");
			return;
		}
		List<HighInstruction> list2 = new List<HighInstruction>();
		foreach (HighInstruction child in m_Instructions.m_List[0].m_Children)
		{
			list2.Add(child);
		}
		DestroyHudInstruction(m_Instructions.m_List);
		m_Instructions.Clear();
		foreach (HighInstruction item3 in list2)
		{
			m_Instructions.Add(item3);
			item3.m_Parent = null;
		}
		CreateHudInstructions();
		TutorialScriptManager.Instance.TeachingInstructionsChanged();
		AudioManager.Instance.StartEvent("ScriptingForeverRemoved");
	}

	private void CreateDragInstructionShadows(List<HudInstruction> NewInstructions, bool SetShadow = true)
	{
		float num = 0f;
		float num2 = 0f;
		float x = NewInstructions[0].transform.localScale.x;
		List<Transform> list = new List<Transform>();
		List<Vector2> list2 = new List<Vector2>();
		foreach (HudInstruction NewInstruction in NewInstructions)
		{
			float width = NewInstruction.GetWidth();
			if (width > num2)
			{
				num2 = width;
			}
			list.Add(NewInstruction.transform.parent);
			list2.Add(NewInstruction.transform.localPosition);
			NewInstruction.transform.SetParent(HudManager.Instance.m_UIRenderCanvasRootTransform.Find("Root"));
			NewInstruction.transform.localScale = new Vector3(1f, 1f, 1f);
			num += NewInstruction.GetHeight();
		}
		float num3 = 0f;
		foreach (HudInstruction NewInstruction2 in NewInstructions)
		{
			num3 += NewInstruction2.GetHeight();
			NewInstruction2.transform.localPosition = new Vector3(0f, num3, 0f);
		}
		int num4 = (int)num;
		int num5 = (int)num2;
		Camera uIRenderCamera = HudManager.Instance.m_UIRenderCamera;
		uIRenderCamera.gameObject.SetActive(value: true);
		if ((bool)m_RenderTexture)
		{
			Object.Destroy(m_RenderTexture);
			Object.Destroy(m_FinalTexture);
		}
		uIRenderCamera.orthographicSize = num4 / 2;
		m_RenderTexture = new RenderTexture(num5, num4, 32);
		HudManager.Instance.m_UIRenderCanvasRootTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(num5, num4);
		Vector3 position = new Vector3(0f, (float)(-num4) * 0.5f - 100f, 0f);
		HudManager.Instance.m_UIRenderCanvasRootTransform.position = position;
		uIRenderCamera.transform.position = position;
		m_FinalTexture = new Texture2D(num5, num4, TextureFormat.RGBA32, mipChain: false);
		m_InstructionShadow.GetComponent<RectTransform>().sizeDelta = new Vector2(num5, num4);
		m_InstructionShadow.GetComponent<RawImage>().texture = m_FinalTexture;
		uIRenderCamera.targetTexture = m_RenderTexture;
		RenderTexture.active = m_RenderTexture;
		uIRenderCamera.Render();
		m_FinalTexture.ReadPixels(new Rect(0f, 0f, num5, num4), 0, 0);
		if (SetShadow)
		{
			Color32[] pixels = m_FinalTexture.GetPixels32();
			for (int i = 0; i < pixels.Length; i++)
			{
				if (pixels[i].r != 0 || pixels[i].g != 0 || pixels[i].b != 0)
				{
					pixels[i].a = byte.MaxValue;
				}
				else
				{
					pixels[i].a = 0;
				}
			}
			m_FinalTexture.SetPixels32(pixels);
		}
		m_FinalTexture.Apply();
		m_FinalTexture.filterMode = FilterMode.Point;
		uIRenderCamera.targetTexture = null;
		RenderTexture.active = null;
		uIRenderCamera.gameObject.SetActive(value: false);
		Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		m_InstructionShadow.transform.SetParent(menusRootTransform);
		int num6 = 0;
		foreach (HudInstruction NewInstruction3 in NewInstructions)
		{
			if (SetShadow)
			{
				NewInstruction3.transform.SetParent(menusRootTransform);
			}
			else
			{
				NewInstruction3.transform.SetParent(list[num6]);
			}
			NewInstruction3.transform.localScale = new Vector3(x, x, x);
			NewInstruction3.transform.localPosition = list2[num6];
			num6++;
		}
		if (SetShadow)
		{
			m_InstructionShadow.SetActive(value: true);
		}
	}

	private void DragRepeat()
	{
		Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		bool activeSelf = m_StartRepeatInstruction.gameObject.activeSelf;
		m_StartRepeatInstruction.gameObject.SetActive(value: true);
		HudInstruction component = Object.Instantiate(m_StartRepeatInstruction, m_StartRepeatInstruction.transform.position, Quaternion.identity, menusRootTransform).GetComponent<HudInstruction>();
		component.SetParent(this);
		component.SetStatic();
		component.SetInstruction(new HighInstruction(m_StartRepeatInstruction.m_Instruction.m_Type, null));
		component.SetDragging(Dragging: true);
		m_StartRepeatInstruction.gameObject.SetActive(activeSelf);
		m_DragInstructions.Add(component);
		m_DragMouseOffset = default(Vector3);
		CreateDragInstructionShadows(m_DragInstructions);
	}

	private bool IsInstructionInDragList(List<HudInstruction> DragSelectList, HudInstruction OldInstruction)
	{
		if (DragSelectList.Contains(OldInstruction))
		{
			return true;
		}
		foreach (HudInstruction DragSelect in DragSelectList)
		{
			if (DragSelect.m_Instruction.Contains(OldInstruction.m_Instruction))
			{
				return true;
			}
		}
		return false;
	}

	private HudInstruction CloneInstruction(HudInstruction OldInstruction, HudInstruction Parent)
	{
		HighInstruction highInstruction = new HighInstruction(OldInstruction.m_Instruction.m_Type, OldInstruction.m_Instruction.m_ActionInfo);
		highInstruction.m_Argument = OldInstruction.m_Instruction.m_Argument;
		highInstruction.m_ActionInfo.m_Value = OldInstruction.m_Instruction.m_ActionInfo.m_Value;
		Transform parent = null;
		if ((bool)Parent)
		{
			parent = Parent.transform;
			highInstruction.m_Parent = Parent.m_Instruction;
		}
		HudInstruction hudInstruction = CreateHudInstruction(highInstruction, parent, default(Vector3), 0f);
		if (Parent == null)
		{
			hudInstruction.transform.SetParent(m_DragParent.transform);
			hudInstruction.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
		}
		hudInstruction.transform.position = OldInstruction.transform.position;
		foreach (HighInstruction child in OldInstruction.m_Instruction.m_Children)
		{
			HudInstruction hudInstruction2 = CloneInstruction(child.m_HudParent, hudInstruction);
			highInstruction.m_Children.Add(hudInstruction2.m_Instruction);
		}
		foreach (HighInstruction item in OldInstruction.m_Instruction.m_Children2)
		{
			HudInstruction hudInstruction3 = CloneInstruction(item.m_HudParent, hudInstruction);
			highInstruction.m_Children2.Add(hudInstruction3.m_Instruction);
		}
		return hudInstruction;
	}

	private void TransferDragSelectListToDragList(bool Copy)
	{
		if (!Copy)
		{
			CopyInstructions();
		}
		m_DragInstructions.Clear();
		foreach (HudInstruction dragSelect in m_DragSelectList)
		{
			HudInstruction hudInstruction = null;
			if (!Copy)
			{
				hudInstruction = dragSelect;
				m_Instructions.Remove(hudInstruction.m_Instruction);
				hudInstruction.m_Instruction.m_Parent = null;
			}
			else
			{
				hudInstruction = CloneInstruction(dragSelect, null);
			}
			hudInstruction.SetDragging(Dragging: true);
			m_DragInstructions.Insert(0, hudInstruction);
		}
		m_DragSelectList.Clear();
		m_DragInstructions.Sort(SortInstructionByY);
		Vector3 vector = HudManager.Instance.ScreenToCanvas(Input.mousePosition);
		Vector3 vector2 = HudManager.Instance.ScreenToCanvas(m_DragInstructions[m_DragInstructions.Count - 1].transform.position);
		m_DragMouseOffset = vector2 - vector;
		if (Copy)
		{
			m_DragMouseOffset = default(Vector3);
		}
		List<HighInstruction> list = new List<HighInstruction>();
		foreach (HudInstruction dragInstruction in m_DragInstructions)
		{
			dragInstruction.transform.SetParent(m_DragParent.transform);
			list.Add(dragInstruction.m_Instruction);
		}
		RefreshHudInstruction(list);
		CreateDragInstructionShadows(m_DragInstructions);
		CreateHudInstructions();
		m_DragCopy = Copy;
	}

	public void OnInstructionClick(HudInstruction OldInstruction)
	{
		if (m_Locked || ((bool)m_CurrentTarget && !m_CurrentTarget.m_Learning) || IsDragging())
		{
			return;
		}
		HudManager.Instance.StopPointingToTile();
		if (OldInstruction.m_Static)
		{
			m_StartRepeatClick = true;
			m_StartRepeatInstruction = OldInstruction;
			m_MouseStartPosition = HudManager.Instance.ScreenToCanvas(Input.mousePosition);
		}
		else if (Input.GetKey(KeyCode.LeftControl))
		{
			m_DraggingInstructionSelect = true;
			m_InstructionSelectInstruction = OldInstruction;
			if (!TestDragSelectListCheckContains(OldInstruction))
			{
				if (m_DragSelectList.Contains(OldInstruction))
				{
					DragSelectListRemove(OldInstruction);
				}
				else
				{
					DragSelectListAdd(OldInstruction);
				}
			}
		}
		else
		{
			if (!IsInstructionInDragList(m_DragSelectList, OldInstruction))
			{
				DragSelectListClear();
				DragSelectListAdd(OldInstruction);
			}
			TransferDragSelectListToDragList(Copy: false);
			AudioManager.Instance.StartEvent("ScriptingInstructionRemoved");
		}
	}

	public void OnViewportMouseEnter()
	{
		m_ViewportHover = true;
		UpdateDragInstructionInvalid();
	}

	public void OnViewportMouseExit()
	{
		m_ViewportHover = false;
		UpdateDragInstructionInvalid();
	}

	public bool IsDragging()
	{
		return m_DragInstructions.Count > 0;
	}

	public bool GetHover()
	{
		if (m_Hover)
		{
			return true;
		}
		return false;
	}

	public void InstructionHover(HudInstruction HoverInstruction, bool Hover)
	{
		if (!Hover)
		{
			if (m_HoverInstruction != HoverInstruction)
			{
				return;
			}
			m_HoverInstruction = null;
		}
		else
		{
			m_HoverInstruction = HoverInstruction;
		}
		UpdateDragInstructionInvalid();
		if (m_DraggingInstructionSelect && Hover)
		{
			CheckDraggingInstruction(HoverInstruction);
		}
	}

	private void UpdateDragInstructionInvalid()
	{
	}

	private int GetLine(List<HighInstruction> Instructions, Vector3 TestPosition, out HighInstruction Parent, out bool Children2)
	{
		Parent = null;
		Children2 = false;
		for (int i = 0; i < Instructions.Count; i++)
		{
			HighInstruction highInstruction = Instructions[i];
			float y = highInstruction.m_HudParent.transform.localPosition.y;
			float y2 = highInstruction.m_HudParent.transform.parent.InverseTransformPoint(TestPosition).y;
			float num = y2 - y;
			if (i == 0 && num >= -0.001f)
			{
				Parent = highInstruction.m_Parent;
				Children2 = false;
				if (Parent != null && Parent.m_Children2.Contains(highInstruction))
				{
					Children2 = true;
				}
				return 0;
			}
			float height = highInstruction.m_HudParent.GetHeight();
			float num2 = y - height;
			if (i != Instructions.Count - 1)
			{
				num2 = Instructions[i + 1].m_HudParent.transform.localPosition.y;
			}
			if (!(y2 <= y) || !(y2 > num2))
			{
				continue;
			}
			if (highInstruction.CanTakeChildren())
			{
				if (y2 < num2 + 15f)
				{
					Parent = highInstruction.m_Parent;
					Children2 = false;
					if (Parent != null && Parent.m_Children2.Contains(highInstruction))
					{
						Children2 = true;
					}
					return i + 1;
				}
				if (y2 > y - 15f)
				{
					Parent = highInstruction.m_Parent;
					Children2 = false;
					if (Parent != null && Parent.m_Children2.Contains(highInstruction))
					{
						Children2 = true;
					}
					return i;
				}
				float height2 = highInstruction.m_HudParent.GetHeight(IncludeChildren2: false);
				float num3 = y - height2 + 15f;
				List<HighInstruction> list = highInstruction.m_Children;
				if (highInstruction.m_Type == HighInstruction.Type.IfElse && y2 < num3)
				{
					list = highInstruction.m_Children2;
				}
				if (list.Count > 0)
				{
					int line = GetLine(list, TestPosition, out Parent, out Children2);
					if (Parent != null)
					{
						return line;
					}
				}
				Children2 = false;
				if (highInstruction.m_Type == HighInstruction.Type.IfElse && y2 < num3)
				{
					Children2 = true;
				}
				Parent = highInstruction;
				return list.Count;
			}
			Parent = highInstruction.m_Parent;
			Children2 = false;
			if (Parent != null && Parent.m_Children2.Contains(highInstruction))
			{
				Children2 = true;
			}
			if (y2 > y - height / 2f)
			{
				return i;
			}
			return i + 1;
		}
		if (Instructions.Count > 0)
		{
			Parent = Instructions[0].m_Parent;
			Children2 = false;
			if (Parent != null && Parent.m_Children2.Contains(Instructions[0]))
			{
				Children2 = true;
			}
		}
		return Instructions.Count;
	}

	private int GetInstructionInsertLine(HudInstruction NewInstruction, out HighInstruction Parent, out bool Children2)
	{
		Vector3 position = NewInstruction.transform.position;
		return GetLine(m_Instructions.m_List, position, out Parent, out Children2);
	}

	private int GetInsertLine(Vector3 TestPosition, out HighInstruction Parent, out bool Children2)
	{
		return GetLine(m_Instructions.m_List, TestPosition, out Parent, out Children2);
	}

	public void InsertInstruction(HighInstruction.Type NewInstructionType, int Line, ActionInfo Info)
	{
		if (GetCanTakeMoreInstructions())
		{
			HighInstruction highInstruction = new HighInstruction(NewInstructionType, Info);
			m_Instructions.Insert(Line, highInstruction);
			m_NewInstructions.Add(highInstruction);
		}
	}

	public void AboutToAddInstructions()
	{
		CopyInstructions();
		SetUndoAvailable(Available: true);
	}

	public void DeleteInstruction(int Line)
	{
		InstructionDelete(m_Instructions.m_List[Line].m_HudParent);
	}

	public int GetFreeMemory()
	{
		if (m_CurrentTarget == null)
		{
			return 0;
		}
		int totalMaxInstuctions = m_CurrentTarget.GetTotalMaxInstuctions();
		int instructionCount = HighInstruction.GetInstructionCount(m_Instructions.m_List);
		return totalMaxInstuctions - instructionCount;
	}

	private bool GetCanTakeMoreInstructions()
	{
		if (GetFreeMemory() > -m_ExcessInstructions)
		{
			return true;
		}
		return false;
	}

	private int GetDragMemoryRequired()
	{
		int result = 0;
		if (IsDragging())
		{
			List<HighInstruction> list = new List<HighInstruction>();
			foreach (HudInstruction dragInstruction in m_DragInstructions)
			{
				list.Add(dragInstruction.m_Instruction);
			}
			result = HighInstruction.GetInstructionCount(list);
		}
		return result;
	}

	public void NewInstruction(HighInstruction.Type NewInstructionType, ActionInfo Info)
	{
		DragSelectListClear();
		if (!GetCanTakeMoreInstructions() || ((Info.m_Action == ActionType.StowObject || Info.m_Action == ActionType.RecallObject || Info.m_Action == ActionType.CycleObject || Info.m_Action == ActionType.SwapObject) && m_CurrentTarget.m_FarmerInventory.m_TotalCapacity == 0))
		{
			return;
		}
		m_NewInstructions.Clear();
		ConvertPlayerActionsToWorkers.Check(this, NewInstructionType, Info);
		AudioManager.Instance.StartEvent("WorkerConfirm", m_CurrentTarget);
		CreateHudInstructions();
		foreach (HighInstruction newInstruction in m_NewInstructions)
		{
			int totalSearchRange = m_CurrentTarget.GetTotalSearchRange();
			GameStateEditArea.MakeAreaInVisiblePlots(Info.m_Position, out var TopLeft, out var BottomRight, totalSearchRange / 2);
			HighInstruction.FindType newFindType = HighInstruction.FindType.Full;
			if (TutorialPanelController.Instance.GetIsFirstDigging())
			{
				newFindType = HighInstruction.FindType.Even;
			}
			newInstruction.SetFindNearestArea(TopLeft, BottomRight, newFindType);
			AreaIndicator areaIndicatorFromInstruction = AreaIndicatorManager.Instance.GetAreaIndicatorFromInstruction(newInstruction);
			if ((bool)areaIndicatorFromInstruction)
			{
				areaIndicatorFromInstruction.Scale(Up: true);
			}
		}
	}

	private HudInstruction CreateHudInstruction(HighInstruction NewInstruction, Transform Parent, Vector3 Offset, float y)
	{
		string text = "Instruction";
		if (NewInstruction.CanTakeChildren())
		{
			text = ((NewInstruction.m_Type != HighInstruction.Type.IfElse) ? "InstructionChildren" : "InstructionChildren2");
		}
		HudInstruction component = Object.Instantiate((GameObject)Resources.Load("Prefabs/Hud/Scripting/" + text, typeof(GameObject)), new Vector3(0f, 0f, 0f), Quaternion.identity, HudManager.Instance.m_HUDRootTransform).GetComponent<HudInstruction>();
		component.SetParent(this);
		component.SetInstruction(NewInstruction);
		component.transform.SetParent(Parent);
		component.transform.localScale = new Vector3(1f, 1f, 1f);
		if (m_State != State.Recording)
		{
			component.EnableInteraction(Interaction: false);
		}
		Vector3 vector = new Vector3(30f, y);
		component.transform.localPosition = vector + Offset;
		return component;
	}

	private void CreateHudInstructions(List<HighInstruction> NewInstructions, Transform Parent, Vector3 Offset)
	{
		float num = -30f;
		foreach (HighInstruction NewInstruction in NewInstructions)
		{
			HudInstruction hudInstruction = CreateHudInstruction(NewInstruction, Parent, Offset, num);
			if (NewInstruction.m_Children.Count > 0)
			{
				CreateHudInstructions(NewInstruction.m_Children, hudInstruction.transform, new Vector3(0f, 0f, 0f));
			}
			if (NewInstruction.m_Children2.Count > 0)
			{
				CreateHudInstructions(NewInstruction.m_Children2, hudInstruction.transform, new Vector3(0f, 0f - NewInstruction.m_HudParent.GetHeight(IncludeChildren2: false) + 30f, 0f));
			}
			num -= hudInstruction.GetHeight();
		}
	}

	private void DestroyHudInstruction(List<HighInstruction> NewInstructions, bool Immediate = true)
	{
		foreach (HighInstruction NewInstruction in NewInstructions)
		{
			if (NewInstruction.m_Children.Count > 0)
			{
				DestroyHudInstruction(NewInstruction.m_Children, Immediate);
			}
			if (NewInstruction.m_Children2.Count > 0)
			{
				DestroyHudInstruction(NewInstruction.m_Children2, Immediate);
			}
			if ((bool)NewInstruction.m_HudParent)
			{
				if (Immediate)
				{
					Object.DestroyImmediate(NewInstruction.m_HudParent.gameObject);
				}
				else
				{
					Object.Destroy(NewInstruction.m_HudParent.gameObject);
				}
				NewInstruction.m_HudParent = null;
			}
		}
	}

	private void RefreshHudInstruction(List<HighInstruction> NewInstructions)
	{
		foreach (HighInstruction NewInstruction in NewInstructions)
		{
			if (NewInstruction.m_Children.Count > 0)
			{
				RefreshHudInstruction(NewInstruction.m_Children);
			}
			if (NewInstruction.m_Children2.Count > 0)
			{
				RefreshHudInstruction(NewInstruction.m_Children2);
			}
			if ((bool)NewInstruction.m_HudParent)
			{
				NewInstruction.m_HudParent.Refresh();
			}
		}
	}

	public void UpdateContentSize()
	{
		float num = 40f;
		float num2 = 0f;
		foreach (HighInstruction item in m_Instructions.m_List)
		{
			num += item.m_HudParent.GetHeight();
			float width = item.m_HudParent.GetWidth();
			if (width > num2)
			{
				num2 = width;
			}
		}
		foreach (HighInstruction item2 in m_Instructions.m_List)
		{
			item2.m_HudParent.transform.SetParent(m_CanvasParent);
		}
		GameObject obj = m_ScrollView.GetContent().gameObject;
		num2 += 40f;
		num2 -= m_ScrollView.GetComponent<RectTransform>().rect.width;
		obj.GetComponent<RectTransform>().sizeDelta = new Vector2(num2, num);
		foreach (HighInstruction item3 in m_Instructions.m_List)
		{
			item3.m_HudParent.transform.SetParent(m_ContentParent);
		}
	}

	public void CreateHudInstructions()
	{
		DestroyHudInstruction(m_Instructions.m_List);
		CreateHudInstructions(m_Instructions.m_List, m_ContentParent, new Vector3(-15f, 15f, 0f));
		RefreshHudInstruction(m_Instructions.m_List);
		UpdateContentSize();
	}

	private Vector3 GetInstructionInsertPosition(int Line, HighInstruction Parent, bool Children2)
	{
		Vector3 vector = default(Vector3);
		if (Parent == null)
		{
			if (m_Instructions.m_List.Count != 0)
			{
				if (Line < m_Instructions.m_List.Count)
				{
					return m_Instructions.m_List[Line].m_HudParent.transform.localPosition;
				}
				HudInstruction hudParent = m_Instructions.m_List[Line - 1].m_HudParent;
				float height = hudParent.GetHeight();
				return hudParent.transform.localPosition + new Vector3(0f, 0f - height, 0f);
			}
			vector = new Vector3(15f, -15f, 0f);
		}
		else
		{
			List<HighInstruction> list = Parent.m_Children;
			if (Children2)
			{
				list = Parent.m_Children2;
			}
			if (list.Count != 0)
			{
				if (Line < list.Count)
				{
					return list[Line].m_HudParent.transform.localPosition;
				}
				HudInstruction hudParent2 = list[Line - 1].m_HudParent;
				float height2 = hudParent2.GetHeight();
				return hudParent2.transform.localPosition + new Vector3(0f, 0f - height2, 0f);
			}
			vector = ((!Children2) ? new Vector3(30f, -30f, 0f) : new Vector3(30f, 0f - Parent.m_HudParent.GetHeight(IncludeChildren2: false), 0f));
		}
		return vector;
	}

	private bool GetInstructionInsideContents(HudInstruction Instruction)
	{
		Rect rect = m_Viewport.GetComponent<RectTransform>().rect;
		Transform parent = Instruction.transform.parent;
		Instruction.transform.SetParent(m_Viewport.transform);
		Vector3 localPosition = Instruction.transform.localPosition;
		Instruction.transform.SetParent(parent);
		localPosition.x += Instruction.GetComponent<RectTransform>().rect.width / 2f;
		localPosition.y -= Instruction.GetComponent<RectTransform>().rect.height / 2f;
		if (localPosition.x >= 0f && localPosition.x <= rect.width && localPosition.y <= 0f && localPosition.y >= 0f - rect.height)
		{
			return true;
		}
		return false;
	}

	private bool GetCursorInsideContents()
	{
		Rect rect = m_Viewport.GetComponent<RectTransform>().rect;
		Vector3 vector = m_Viewport.transform.InverseTransformPoint(HudManager.Instance.GetMouseInWorldSpace());
		if (vector.x >= 0f && vector.x <= rect.width && vector.y <= 0f && vector.y >= 0f - rect.height)
		{
			return true;
		}
		return false;
	}

	private void UpdateStartRepeatClick()
	{
		if (!m_StartRepeatClick)
		{
			return;
		}
		if (!Input.GetMouseButton(0))
		{
			m_StartRepeatClick = false;
			if (m_StartRepeatInstruction.m_Instruction.m_Type == HighInstruction.Type.Repeat)
			{
				CopyInstructions();
				ClickRepeat();
			}
		}
		else if ((HudManager.Instance.ScreenToCanvas(Input.mousePosition) - m_MouseStartPosition).magnitude > 5f || m_StartRepeatInstruction.m_Instruction.m_Type != HighInstruction.Type.Repeat)
		{
			m_StartRepeatClick = false;
			if (m_RepeatButton.m_Button.GetInteractable())
			{
				DragSelectListClear();
				CopyInstructions();
				DragRepeat();
			}
		}
	}

	private bool UpdateEditCursor(Vector3 TestPosition)
	{
		m_InstructionCursor.SetActive(value: true);
		HighInstruction Parent;
		bool Children;
		int insertLine = GetInsertLine(TestPosition, out Parent, out Children);
		Vector3 instructionInsertPosition = GetInstructionInsertPosition(insertLine, Parent, Children);
		Transform contentParent = m_ContentParent;
		if (Parent != null)
		{
			contentParent = Parent.m_HudParent.transform;
		}
		m_InstructionCursor.transform.SetParent(contentParent);
		m_InstructionCursor.transform.localPosition = instructionInsertPosition;
		m_InstructionCursor.transform.SetParent(m_ContentParent);
		instructionInsertPosition = m_InstructionCursor.transform.localPosition;
		instructionInsertPosition.x += m_InstructionCursor.GetComponent<RectTransform>().rect.width / 2f * (1f / m_ContentParent.transform.localScale.x);
		m_InstructionCursor.transform.localPosition = instructionInsertPosition;
		bool result = false;
		if (insertLine != m_LastDragLine || Parent != m_LastDragParent || Children != m_LastChildren2)
		{
			m_LastDragLine = insertLine;
			m_LastDragParent = Parent;
			m_LastChildren2 = Children;
			result = true;
		}
		return result;
	}

	private void StopDrag(bool Success)
	{
		HudInstruction hudInstruction = m_DragInstructions[m_DragInstructions.Count - 1];
		int index = 0;
		HighInstruction Parent = null;
		bool Children = false;
		bool flag = hudInstruction.GetInvalid();
		if (!GetCanTakeMoreInstructions() || !Success)
		{
			flag = true;
		}
		if (m_DragCopy && !flag)
		{
			CopyInstructions();
		}
		m_InstructionShadow.SetActive(value: false);
		if (!flag)
		{
			index = GetInstructionInsertLine(hudInstruction, out Parent, out Children);
			DestroyHudInstruction(m_Instructions.m_List);
		}
		foreach (HudInstruction dragInstruction in m_DragInstructions)
		{
			dragInstruction.SetDragging(Dragging: false);
			HighInstruction instruction = dragInstruction.m_Instruction;
			DestroyHudInstruction(dragInstruction.m_Instruction.m_Children);
			DestroyHudInstruction(dragInstruction.m_Instruction.m_Children2);
			Object.DestroyImmediate(dragInstruction.gameObject);
			if (!flag)
			{
				if (Parent == null)
				{
					m_Instructions.Insert(index, instruction);
				}
				else
				{
					m_Instructions.InsertToChild(Parent, index, instruction, Children);
				}
			}
		}
		if (!flag)
		{
			CreateHudInstructions();
			TutorialScriptManager.Instance.TeachingInstructionsChanged();
			AudioManager.Instance.StartEvent("ScriptingInstructionAdded");
		}
		else
		{
			AudioManager.Instance.StartEvent("ScriptingInstructionDeleted");
			m_Hover = false;
		}
		m_DragInstructions.Clear();
		m_InstructionCursor.SetActive(value: false);
	}

	private void UpdateDragCursor(Vector3 MousePosition)
	{
		if (!GetCursorInsideContents())
		{
			return;
		}
		Vector3 vector = m_InstructionPanel.transform.InverseTransformPoint(MousePosition);
		float height = m_InstructionPanel.GetComponent<RectTransform>().rect.height;
		float height2 = m_ContentParent.GetComponent<RectTransform>().rect.height;
		float num = 30f;
		if (vector.y > height / 2f - num)
		{
			Vector3 localPosition = m_ContentParent.localPosition;
			localPosition.y -= 400f * TimeManager.Instance.m_NormalDeltaUnscaled;
			if (localPosition.y < 0f)
			{
				localPosition.y = 0f;
			}
			m_ContentParent.localPosition = localPosition;
		}
		else if (vector.y < (0f - height) / 2f + num * 2f)
		{
			Vector3 localPosition2 = m_ContentParent.localPosition;
			localPosition2.y += 400f * TimeManager.Instance.m_NormalDeltaUnscaled;
			float num2 = height2 - height;
			if (num2 < 0f)
			{
				num2 = 0f;
			}
			if (localPosition2.y > num2)
			{
				localPosition2.y = num2;
			}
			m_ContentParent.localPosition = localPosition2;
		}
	}

	public void UpdateDragInstruction()
	{
		if (!IsDragging())
		{
			return;
		}
		Vector3 vector = HudManager.Instance.ScreenToCanvas(Input.mousePosition) + m_DragMouseOffset + new Vector3(-2f, 2f, 0f);
		m_InstructionShadow.transform.localPosition = vector + new Vector3(-6f, -6f, 0f);
		for (int num = m_DragInstructions.Count - 1; num >= 0; num--)
		{
			HudInstruction hudInstruction = m_DragInstructions[num];
			hudInstruction.transform.localPosition = vector;
			vector.y -= hudInstruction.GetHeight() * hudInstruction.transform.localScale.y;
		}
		HudInstruction hudInstruction2 = m_DragInstructions[m_DragInstructions.Count - 1];
		if (!GetCursorInsideContents())
		{
			m_InstructionCursor.SetActive(value: false);
			foreach (HudInstruction dragInstruction in m_DragInstructions)
			{
				dragInstruction.SetInvalidPosition(Invalid: true);
			}
		}
		else
		{
			foreach (HudInstruction dragInstruction2 in m_DragInstructions)
			{
				dragInstruction2.SetInvalidPosition(Invalid: false);
			}
			if (UpdateEditCursor(hudInstruction2.transform.position))
			{
				AudioManager.Instance.StartEvent("ScriptingInstructionDrag");
			}
		}
		if (Input.GetMouseButtonUp(0))
		{
			StopDrag(Success: true);
		}
		else
		{
			UpdateDragCursor(hudInstruction2.transform.position);
		}
		if (MyInputManager.m_Rewired.GetButtonDown("Quit"))
		{
			StopDrag(Success: false);
		}
	}

	private void FindExecutingInstruction(List<HighInstruction> Instructions, int CurrentLine)
	{
		foreach (HighInstruction Instruction in Instructions)
		{
			int num = CurrentLine - Instruction.m_ScriptLineNumber;
			if (num >= 0 && num < m_BestInstructionDiff)
			{
				m_BestInstructionDiff = num;
				m_BestInstruction = Instruction;
			}
			if (Instruction.m_Children.Count > 0)
			{
				FindExecutingInstruction(Instruction.m_Children, CurrentLine);
			}
			if (Instruction.m_Children2.Count > 0)
			{
				FindExecutingInstruction(Instruction.m_Children2, CurrentLine);
			}
		}
	}

	private HighInstruction CalcExecutingInstruction()
	{
		if (!m_CurrentTarget)
		{
			return null;
		}
		WorkerScriptLocal currentScript = m_CurrentTarget.m_WorkerInterpreter.GetCurrentScript();
		if (currentScript == null)
		{
			return null;
		}
		int currentInstruction = currentScript.m_CurrentInstruction;
		m_BestInstruction = null;
		m_BestInstructionDiff = 1000000;
		FindExecutingInstruction(m_Instructions.m_List, currentInstruction);
		return m_BestInstruction;
	}

	private void UpdateCursor()
	{
		HighInstruction highInstruction = CalcExecutingInstruction();
		if (highInstruction != null)
		{
			string text = "CurrentInstruction";
			if (m_CurrentTarget.m_WorkerInterpreter.m_InstructionFailed)
			{
				m_InstructionFailTimer += TimeManager.Instance.m_NormalDeltaUnscaled;
				if ((int)(m_InstructionFailTimer * 60f) % 8 < 4)
				{
					text = "CurrentInstructionFail";
				}
			}
			Sprite sprite = (Sprite)Resources.Load("Textures/Hud/Script/" + text, typeof(Sprite));
			m_ExecutingCursor.GetComponent<Image>().sprite = sprite;
			m_ExecutingCursor.transform.SetParent(highInstruction.m_HudParent.transform.parent);
			Vector3 localPosition = highInstruction.m_HudParent.transform.localPosition;
			m_ExecutingCursor.transform.localPosition = localPosition + new Vector3(-20f, -25f, 0f);
			m_ExecutingCursor.transform.SetParent(m_ContentParent.transform.parent);
			m_ExecutingCursor.transform.SetParent(m_ContentParent);
			m_ExecutingCursor.transform.localScale = new Vector3(1f, 1f, 1f);
			m_ExecutingCursor.SetActive(value: true);
			if (highInstruction != m_LastExectuedInstruction)
			{
				m_LastExectuedInstruction = highInstruction;
				AudioManager.Instance.StartEvent("ScriptingInstructionExecuted");
			}
		}
		else
		{
			m_ExecutingCursor.SetActive(value: false);
		}
	}

	public void SetPointAtObjectSearch(BaseClass NewObject, bool Active)
	{
		m_PointAtObjectSearch = NewObject;
		HudManager.Instance.StopPointingToObject(NewObject);
	}

	public void StartScaling(bool Up)
	{
		if (!Up && !base.gameObject.activeSelf)
		{
			return;
		}
		if ((!Up && m_Scaling && m_ScalingUp && m_Wobbler.m_Timer == 0f) || (Up && m_Scaling && !m_ScalingUp && m_Wobbler.m_Timer == 0f))
		{
			m_Wobbler.m_Wobbling = false;
			m_Scaling = false;
			base.gameObject.SetActive(Up);
			m_Instructions.CancelScaleAreaIndicators();
			m_Instructions.ShowAreaIndicators(Up);
			return;
		}
		base.gameObject.SetActive(value: true);
		m_Scaling = true;
		m_ScalingUp = Up;
		m_ScaleEndPosition = m_NormalScreenPosition;
		if ((bool)m_CurrentTarget)
		{
			Vector3 position = CameraManager.Instance.m_Camera.WorldToScreenPoint(m_CurrentTarget.transform.position);
			m_ScaleStartPosition = HudManager.Instance.ScreenToCanvas(position);
		}
		float num = m_ScalingDelay;
		if (!Up)
		{
			num *= 0.5f;
		}
		m_Wobbler.Go(num, 0.5f, 1f);
		UpdateScalingTransform();
		m_Instructions.ScaleAreaIndicators(Up);
	}

	private void UpdateScalingTransform()
	{
		float num = 1f - m_Wobbler.m_Height;
		if (!m_ScalingUp)
		{
			num = 1f - m_Wobbler.m_Timer / m_Wobbler.m_WobbleTime;
		}
		base.transform.localScale = new Vector2(num, num);
		Vector2 anchoredPosition = (m_ScaleEndPosition - m_ScaleStartPosition) * num + m_ScaleStartPosition;
		TeachWorkerScriptController.Instance.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
	}

	public bool GetFinishedScaling()
	{
		if (m_Scaling && !m_Wobbler.m_Wobbling)
		{
			m_Scaling = false;
			return true;
		}
		return false;
	}

	private void UpdateScaling()
	{
		m_Wobbler.m_WobbleWhilePaused = TimeManager.Instance.m_PauseTimeEnabled;
		if (m_Wobbler.m_Wobbling)
		{
			m_Wobbler.Update();
			UpdateScalingTransform();
			if (!m_Wobbler.m_Wobbling && !m_ScalingUp)
			{
				base.gameObject.SetActive(value: false);
			}
		}
	}

	private void UpdateNormalPosition()
	{
	}

	private void UpdateExpandButton()
	{
		if (m_Expanded)
		{
			return;
		}
		float num = 0f;
		foreach (HighInstruction item in m_Instructions.m_List)
		{
			float num2 = item.m_HudParent.GetWidth() + item.m_HudParent.transform.localPosition.x;
			if (num2 > num)
			{
				num = num2;
			}
		}
		num *= m_ScrollView.GetContent().transform.localScale.x;
		float width = m_ScrollView.GetComponent<RectTransform>().rect.width;
		bool flag = false;
		if (num > width)
		{
			m_ExpandTimer += TimeManager.Instance.m_NormalDelta;
			if ((int)(m_ExpandTimer * 60f) % 30 < 15)
			{
				flag = true;
			}
		}
		if (flag)
		{
			m_ExpandButton.m_Button.SetBackingSprite("Script/ExpandButtonBacking2");
			m_ExpandButton.m_Button.SetBorderSprite("Script/ExpandButtonBorder2");
			m_ExpandButton.m_Button.SetShadowSprite("Script/ExpandButtonShadow2");
		}
		else
		{
			m_ExpandButton.m_Button.SetBackingSprite("Script/ExpandButtonBacking");
			m_ExpandButton.m_Button.SetBorderSprite("Script/ExpandButtonBorder");
			m_ExpandButton.m_Button.SetShadowSprite("Script/ExpandButtonShadow");
		}
	}

	private void UpdateNonEditable()
	{
		if (m_NonEditableTimer > 0f)
		{
			m_NonEditableTimer -= TimeManager.Instance.m_NormalDelta;
			if (m_NonEditableTimer <= 0f)
			{
				m_NonEditable.SetActive(value: false);
			}
		}
	}

	private void UpdateCopy()
	{
		if (m_DragSelectList.Count != 0 && MyInputManager.m_Rewired.GetButtonDown("Copy"))
		{
			TransferDragSelectListToDragList(Copy: true);
			HudManager.Instance.StopPointingToTile();
			UpdateDragInstruction();
		}
	}

	private void CheckDraggingInstruction(HudInstruction NewInstruction)
	{
		if (!(NewInstruction == m_InstructionSelectInstruction) && !TestDragSelectListCheckContains(NewInstruction) && !m_DragSelectList.Contains(NewInstruction) && !NewInstruction.m_Static)
		{
			DragSelectListAdd(NewInstruction);
		}
	}

	private void UpdateDraggingInstructionSelect()
	{
		Vector3 localPosition = HudManager.Instance.ScreenToCanvas(Input.mousePosition);
		m_MouseObject.transform.localPosition = localPosition;
		UpdateDragCursor(m_MouseObject.transform.position);
		if (!Input.GetKey(KeyCode.LeftControl) || !Input.GetMouseButton(0))
		{
			m_DraggingInstructionSelect = false;
		}
	}

	private void UpdateServerButton()
	{
		if (BotServer.m_FirstBotServer == null || ((bool)m_CurrentTarget && m_CurrentTarget.m_Learning))
		{
			m_DatabaseButton.SetInteractable(Interactable: false);
		}
		else
		{
			m_DatabaseButton.SetInteractable(Interactable: true);
		}
	}

	public void Update()
	{
		if (HudManager.Instance == null)
		{
			return;
		}
		UpdateNormalPosition();
		UpdateScaling();
		if (m_CurrentTarget == null)
		{
			return;
		}
		if ((bool)m_NewDeleteInstruction)
		{
			CopyInstructions();
			UpdateInstructionDelete(m_NewDeleteInstruction);
			m_NewDeleteInstruction = null;
		}
		UpdateStartRepeatClick();
		if (m_State == State.Recording)
		{
			if (IsDragging())
			{
				UpdateDragInstruction();
			}
			else if (m_DraggingInstructionSelect)
			{
				UpdateDraggingInstructionSelect();
			}
			else
			{
				UpdateCopy();
				if (MyInputManager.m_Rewired.GetButtonDown("Undo"))
				{
					OnClickUndo();
				}
				if (MyInputManager.m_Rewired.GetButtonDown("Repeat"))
				{
					OnClickRepeat();
				}
			}
		}
		Counter = m_Instructions.m_List.Count;
		CameraManager.Instance.m_FollowWorker.transform.position = m_CurrentTarget.transform.position + new Vector3(0f, 0.05f, 0f);
		UpdateCursor();
		UpdateTitle();
		UpdateOutOfRange();
		UpdateCameraButton();
		UpdateState();
		UpdateButtons();
		UpdateExpandButton();
		UpdateNonEditable();
		UpdateServerButton();
		if (m_RefreshInstruction)
		{
			m_RefreshInstruction = false;
			CreateHudInstructions();
		}
	}

	public void RemakeAreaIndicators()
	{
		m_Instructions.ClearAreaIndicators();
		m_Instructions.MakeAreaIndicators();
	}

	private void SetFlashEditAreas(HighInstruction NewInstruction, bool Flash)
	{
		NewInstruction.m_HudParent.SetFlashEditArea(Flash);
		foreach (HighInstruction child in NewInstruction.m_Children)
		{
			SetFlashEditAreas(child, Flash);
		}
	}

	public void FlashEditAreas(bool Flash)
	{
		foreach (HighInstruction item in m_Instructions.m_List)
		{
			SetFlashEditAreas(item, Flash);
		}
	}
}
