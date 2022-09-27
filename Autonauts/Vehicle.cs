using SimpleJSON;
using UnityEngine;

public class Vehicle : GoTo
{
	public enum State
	{
		None,
		Moving,
		RequestMove,
		Listening,
		Refuelling,
		Total
	}

	[HideInInspector]
	public State m_State;

	[HideInInspector]
	public float m_StateTimer;

	protected AFO.AT m_ActionType;

	protected PlaySound m_MoveSound;

	protected string m_MoveSoundName;

	private float m_BaseMoveDelay;

	public float m_WeightPenaltyScaler;

	private bool m_ListenRequested;

	private Worker m_WorkerEngager;

	protected bool m_RequestStop;

	public static bool GetIsTypeVehicle(ObjectType NewType)
	{
		if (MobileStorage.GetIsTypeMobileStorage(NewType) || NewType == ObjectType.Canoe || Minecart.GetIsTypeMinecart(NewType) || NewType == ObjectType.CraneCrude)
		{
			return true;
		}
		return false;
	}

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("Vehicle", m_TypeIdentifier);
	}

	public override void Restart()
	{
		base.Restart();
		SetBaseDelay(0.3f);
		m_Engager = null;
		m_MoveSound = null;
		m_MoveSoundName = "";
		m_State = State.None;
		SetState(State.None);
		m_RequestStop = false;
	}

	protected new void Awake()
	{
		base.Awake();
		m_WeightPenaltyScaler = 1f;
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
	}

	private float GetMovementScale()
	{
		float result = 1f;
		if (!GetIsPowered() && (bool)m_Engager && (bool)m_Engager.GetComponent<Farmer>())
		{
			result = 1f / (m_Engager.GetComponent<Farmer>().m_ActionSpeedScale * m_Engager.GetComponent<Farmer>().m_OverallSpeedScale);
		}
		return result;
	}

	protected void UpdateMoveDelay()
	{
		float tileDelay = TileManager.Instance.GetTileDelay(m_TileCoord, Worker: true);
		m_MoveNormalDelay = (m_BaseMoveDelay + tileDelay) * m_WeightPenaltyScaler;
		m_MoveNormalDelay *= GetMovementScale();
	}

	protected void SetWeightPenalty(float Penalty)
	{
		m_WeightPenaltyScaler = Penalty;
	}

	protected void SetBaseDelay(float Delay)
	{
		m_BaseMoveDelay = Delay;
		UpdateMoveDelay();
	}

	private void StopMoveSound()
	{
		if (m_MoveSound != null)
		{
			AudioManager.Instance.StopEvent(m_MoveSound);
			m_MoveSound = null;
		}
	}

	public virtual void SetState(State NewState)
	{
		State state = m_State;
		if (state == State.Moving)
		{
			StopMoveSound();
			StopStateMoving();
		}
		m_State = NewState;
		m_StateTimer = 0f;
		state = m_State;
		if (state == State.Moving)
		{
			StopMoveSound();
		}
		if ((bool)m_Engager)
		{
			if (NewState == State.None)
			{
				m_Engager.GetComponent<Farmer>().EngagedObjectActionActive(Active: false, Success: true);
			}
			else
			{
				m_Engager.GetComponent<Farmer>().EngagedObjectActionActive(Active: true, Success: false);
			}
		}
	}

	protected virtual void StopStateMoving()
	{
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		base.transform.localPosition = new Vector3(0f, -0.35f, 0f);
		base.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
	}

	protected override void ActionDropped(Actionable PreviousHolder, TileCoord DropLocation)
	{
		base.ActionDropped(PreviousHolder, DropLocation);
	}

	public bool GetIsBusy()
	{
		if (m_State != 0 && m_State != State.Moving && m_State != State.RequestMove)
		{
			return true;
		}
		return false;
	}

	public override bool RequestGoTo(TileCoord Destination, Actionable TargetObject = null, bool LessOne = false, int Range = 0)
	{
		if (GetIsBusy())
		{
			return false;
		}
		SetState(State.RequestMove);
		return base.RequestGoTo(Destination, TargetObject, LessOne, Range);
	}

	public void CheckPlot()
	{
		if (!m_Plot.m_Visible)
		{
			AudioManager.Instance.StartEvent("PlotAppear");
			m_Plot.SetVisible(Visible: true, DoUncoverEffect: true);
			if ((bool)m_Engager && m_Engager.m_TypeIdentifier == ObjectType.FarmerPlayer)
			{
				BadgeManager.Instance.AddEvent(BadgeEvent.Type.PlotsUncovered);
				QuestManager.Instance.AddEvent(QuestEvent.Type.PlotUncovered, Bot: false, 0, this);
			}
		}
	}

	public override bool StartGoTo(TileCoord Destination, Actionable TargetObject = null, bool LessOne = false, int Range = 0)
	{
		if (GetIsBusy())
		{
			return false;
		}
		UpdateMoveDelay();
		SetState(State.Moving);
		if (!base.StartGoTo(Destination, TargetObject, LessOne, Range))
		{
			SetState(State.None);
			return false;
		}
		return true;
	}

	public override void NextGoTo()
	{
		if ((bool)m_Engager)
		{
			if (m_Engager.m_TypeIdentifier == ObjectType.FarmerPlayer)
			{
				QuestManager.Instance.AddPlayerMove(m_Engager.GetComponent<FarmerPlayer>());
			}
			else
			{
				QuestManager.Instance.AddWorkerMove(m_Engager.GetComponent<Worker>());
			}
		}
		if ((bool)m_Engager && m_Engager.m_TypeIdentifier == ObjectType.FarmerPlayer && m_Engager.GetComponent<FarmerPlayer>().m_GoToBuffered)
		{
			SetState(State.None);
			if ((bool)m_Engager)
			{
				m_Engager.GetComponent<FarmerPlayer>().DoBuffered();
			}
			else
			{
				EndGoTo();
			}
		}
		else if (m_RequestStop)
		{
			m_RequestStop = false;
			SetState(State.None);
			EndGoTo();
		}
		else
		{
			UpdateMoveDelay();
			base.NextGoTo();
			CheckPlot();
		}
	}

	public override void ObstructionEncountered()
	{
		base.ObstructionEncountered();
		StopGoTo();
		SetState(State.RequestMove);
		base.RequestGoTo(m_GoToTilePosition, m_GoToTargetObject, m_GoToLessOne);
	}

	public override void StopGoTo()
	{
		base.StopGoTo();
		StopMoveSound();
		CheckPlot();
	}

	public override void EndGoTo()
	{
		base.EndGoTo();
		StopGoTo();
		SetState(State.None);
		if (m_ActionType == AFO.AT.AltSecondary && (bool)m_Engager && m_Engager.m_TypeIdentifier == ObjectType.FarmerPlayer && (bool)GetActionInfo(new GetActionInfo(GetAction.IsDisengagable)))
		{
			m_Engager.GetComponent<FarmerPlayer>().SendAction(new ActionInfo(ActionType.DisengageObject, m_TileCoord, this, "", "", AFO.AT.AltSecondary));
		}
	}

	public override void SendAction(ActionInfo Info)
	{
		switch (Info.m_Action)
		{
		case ActionType.MoveTo:
			m_ActionType = Info.m_ActionType;
			RequestGoTo(Info.m_Position, Info.m_Object);
			break;
		case ActionType.MoveToLessOne:
			m_ActionType = Info.m_ActionType;
			RequestGoTo(Info.m_Position, Info.m_Object, LessOne: true);
			break;
		case ActionType.MoveToRange:
			m_ActionType = Info.m_ActionType;
			RequestGoTo(Info.m_Position, Info.m_Object, LessOne: false, 5);
			break;
		case ActionType.Engaged:
			m_Engager = Info.m_Object;
			m_WorkerEngager = m_Engager.GetComponent<Worker>();
			break;
		case ActionType.Disengaged:
			m_Engager = null;
			if (m_State == State.Moving || m_State == State.RequestMove)
			{
				m_RequestStop = true;
			}
			break;
		}
		base.SendAction(Info);
	}

	public override bool CanDoAction(ActionInfo Info, bool RightNow)
	{
		switch (Info.m_Action)
		{
		case ActionType.MoveTo:
			if (m_ListenRequested)
			{
				m_ListenRequested = false;
				StartListening();
				return false;
			}
			RequestGoTo(Info.m_Position, Info.m_Object);
			return m_Path != null;
		case ActionType.MoveToLessOne:
			if (m_ListenRequested)
			{
				m_ListenRequested = false;
				StartListening();
				return false;
			}
			RequestGoTo(Info.m_Position, Info.m_Object, LessOne: true);
			return m_Path != null;
		case ActionType.MoveToRange:
			if (m_ListenRequested)
			{
				m_ListenRequested = false;
				StartListening();
				return false;
			}
			RequestGoTo(Info.m_Position, Info.m_Object, LessOne: false, 5);
			return m_Path != null;
		case ActionType.Engaged:
			if (m_Engager != null)
			{
				return false;
			}
			if ((bool)Info.m_Object && (bool)Info.m_Object.GetComponent<Farmer>())
			{
				if (Info.m_Object.GetComponent<Farmer>().m_FarmerCarry.GetTopObject() != null)
				{
					return false;
				}
				if (Info.m_Object.GetComponent<Farmer>().m_EngagedObject != null)
				{
					return false;
				}
			}
			return true;
		case ActionType.Disengaged:
			return true;
		default:
			return base.CanDoAction(Info, RightNow);
		}
	}

	public override object GetActionInfo(GetActionInfo Info)
	{
		GetAction action = Info.m_Action;
		if (action == GetAction.IsBusy)
		{
			if (m_State != 0)
			{
				return true;
			}
			return false;
		}
		return base.GetActionInfo(Info);
	}

	public override ActionType GetAutoAction(ActionInfo Info)
	{
		if (Info.m_Object == null)
		{
			return ActionType.Total;
		}
		Actionable.m_ReusableActionFromObject.Init(null, Info.m_ObjectType, this, Info.m_ActionType, "", Info.m_Position);
		ActionType actionFromObjectSafe = Info.m_Object.GetActionFromObjectSafe(Actionable.m_ReusableActionFromObject);
		Info.m_ActionRequirement = Actionable.m_ReusableActionFromObject.m_RequirementsOut;
		return actionFromObjectSafe;
	}

	protected virtual void UpdateStateMoving()
	{
		if (m_MoveSound == null && m_MoveSoundName != "")
		{
			m_MoveSound = AudioManager.Instance.StartEvent(m_MoveSoundName, this, Remember: true, Follow: true);
		}
		if (m_Move)
		{
			UpdateMovement();
		}
	}

	protected ActionType GetActionFromCrane(AFO Info)
	{
		if ((bool)Info.m_Actioner.GetComponent<Crane>().m_HeldObject)
		{
			return ActionType.Fail;
		}
		if ((bool)m_Engager)
		{
			return ActionType.Fail;
		}
		if (m_DoingAction || m_State != 0)
		{
			return ActionType.Fail;
		}
		return ActionType.Pickup;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (Info.m_ActionType == AFO.AT.Primary && (bool)Info.m_Actioner.GetComponent<Crane>())
		{
			return GetActionFromCrane(Info);
		}
		if (Info.m_ActionType == AFO.AT.AltPrimary)
		{
			Info.m_FarmerState = Farmer.State.Engaged;
			if (m_Engager != null)
			{
				return ActionType.Fail;
			}
			if ((bool)Info.m_Object && (bool)Info.m_Actioner.GetComponent<Farmer>() && Info.m_Actioner.GetComponent<Farmer>().m_FarmerCarry.GetTopObject() != null)
			{
				return ActionType.Fail;
			}
			if (m_State != 0)
			{
				return ActionType.Fail;
			}
			if (m_BeingHeld)
			{
				return ActionType.Fail;
			}
			return ActionType.EngageObject;
		}
		return base.GetActionFromObject(Info);
	}

	private void StartListening()
	{
		m_WorkerEngager.StartListening();
		SetState(State.Listening);
	}

	private void StopListening()
	{
		m_WorkerEngager.StopListening();
		SetState(State.None);
	}

	public void RequestPause()
	{
		if (m_State == State.None && (m_WorkerEngager.m_WorkerInterpreter.GetCurrentScript() == null || m_WorkerEngager.m_WorkerInterpreter.m_InstructionFailed))
		{
			StartListening();
		}
		else
		{
			m_ListenRequested = true;
		}
	}

	public void Unpause()
	{
		if (m_ListenRequested)
		{
			m_ListenRequested = false;
		}
		else if (m_State == State.Listening)
		{
			StopListening();
		}
	}

	public void CheckRequestPause()
	{
		if (m_ListenRequested && m_State == State.None)
		{
			m_ListenRequested = false;
			StartListening();
		}
	}

	public virtual bool GetIsPowered()
	{
		return false;
	}

	protected void Update()
	{
		bool flag = false;
		if ((bool)m_Engager || m_RequestStop)
		{
			if (m_RequestStop || m_Engager.GetComponent<Farmer>().m_Energy > 0f)
			{
				State state = m_State;
				if (state == State.Moving)
				{
					UpdateStateMoving();
				}
				flag = true;
			}
			else
			{
				StopMoveSound();
			}
		}
		if (flag || m_State == State.Refuelling)
		{
			m_StateTimer += TimeManager.Instance.m_NormalDelta;
		}
	}
}
