using SimpleJSON;
using UnityEngine;

public class Farmer : GoTo
{
	public enum State
	{
		None,
		Moving,
		Turning,
		Shovel,
		Hoe,
		Seed,
		Scythe,
		Chopping,
		Created,
		Milking,
		Held,
		Make,
		Dropping,
		PickingUp,
		Taking,
		Adding,
		Stowing,
		Recalling,
		Cycling,
		Swapping,
		Mining,
		StoneCutting,
		Dredging,
		DroppingSoil,
		Listening,
		WorkerSelect,
		Paused,
		Flail,
		Recharge,
		Shearing,
		Knitting,
		Play,
		NoScript,
		RequestMove,
		RequestFind,
		Fishing,
		Netting,
		Hammering,
		Sweeping,
		Emptying,
		Engaged,
		BeingEngaged,
		RocketRide,
		RocketKicked,
		CelebrateMakeInHand,
		CelebrateEraComplete,
		SpawnJump,
		JumpTurf,
		Upgrading,
		UpgradedWait,
		Upgraded,
		ModAction,
		Total
	}

	[HideInInspector]
	public float m_Energy;

	protected float m_EnergyLossPerSecond;

	[HideInInspector]
	public float m_AddEnergyTimer;

	[HideInInspector]
	public float m_AddEnergy;

	private Vector3 m_AddEnergyPosition;

	[HideInInspector]
	public FarmerCarry m_FarmerCarry;

	[HideInInspector]
	public FarmerInventory m_FarmerInventory;

	[HideInInspector]
	public FarmerUpgrades m_FarmerUpgrades;

	[HideInInspector]
	public FarmerAction m_FarmerAction;

	[HideInInspector]
	public FarmerClothes m_FarmerClothes;

	[HideInInspector]
	public State m_State;

	[HideInInspector]
	public float m_StateTimer;

	[HideInInspector]
	public State m_InterruptState;

	[HideInInspector]
	public float m_InterruptStateTimer;

	private Vector3 m_InterruptPosition;

	private Quaternion m_InterruptRotation;

	[HideInInspector]
	public float m_TestTargetCloseDistance;

	[HideInInspector]
	public FarmerStateBase[] m_States;

	[HideInInspector]
	public float m_StartActionDelay;

	[HideInInspector]
	public float m_EndActionDelay;

	public TileCoord m_ActionTile;

	private float m_MoveBaseDelay;

	public float m_MoveSpeedScale;

	public float m_ActionSpeedScale;

	public float m_OverallSpeedScale;

	[HideInInspector]
	public Actionable m_EngagedObject;

	private int m_TempEngagedID;

	private Tile m_Tile;

	[HideInInspector]
	public GoTo m_HolderObject;

	[HideInInspector]
	public TileCoordObject m_BaggedObject;

	[HideInInspector]
	public TileCoord m_BaggedTile;

	[HideInInspector]
	public bool m_Nudge;

	[HideInInspector]
	public bool m_SpawnEnd;

	[HideInInspector]
	public bool m_SpawnSuccess;

	[HideInInspector]
	public bool m_Learning;

	[HideInInspector]
	public FarmerStateLearning m_LearningState;

	[HideInInspector]
	public Farmer m_LearningTarget;

	[HideInInspector]
	public Animator m_Animator;

	protected bool m_RequestGoToFarmer;

	private TileCoord m_RequestGoToDestination;

	private Actionable m_RequestGoToTarget;

	private bool m_RequestGoToLessOne;

	private int m_RequestRange;

	public bool m_Visible;

	public static bool GetIsTypeFarmer(ObjectType NewType)
	{
		if (NewType == ObjectType.FarmerPlayer || NewType == ObjectType.Worker)
		{
			return true;
		}
		return false;
	}

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("Farmer", m_TypeIdentifier);
	}

	public override void Restart()
	{
		base.Restart();
		if (!ObjectTypeList.m_Loading)
		{
			CollectionManager.Instance.AddCollectable("Farmer", this);
		}
		DespawnManager.Instance.Remove(this);
		m_MoveBaseDelay = VariableManager.Instance.GetVariableAsFloat(ObjectType.FarmerPlayer, "BaseDelay");
		SetOverallSpeedScale(1f);
		SetMoveSpeedScale(1f);
		SetActionSpeedScale(1f);
		m_Energy = 100f;
		m_EnergyLossPerSecond = 0.1f;
		m_AddEnergyTimer = 0f;
		m_BaggedObject = null;
		m_BaggedTile = new TileCoord(0, 0);
		m_Learning = false;
		m_StartActionDelay = 0.125f;
		m_EndActionDelay = 0.25f;
		m_State = State.None;
		SetState(State.None);
		m_InterruptState = State.Total;
		m_EngagedObject = null;
		m_HolderObject = null;
		m_Nudge = false;
		m_TestTargetCloseDistance = 0f;
		m_FarmerCarry.Restart();
		m_FarmerClothes.Restart();
		m_FarmerUpgrades.Restart();
		m_Animator.Rebind();
		m_Animator.Play("FarmerIdle");
		RecordingManager.Instance.UpdateObject(this);
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		RecordingManager.Instance.RemoveObject(this);
		m_FarmerClothes.StopUsing();
		m_FarmerCarry.StopUsing();
		m_FarmerInventory.StopUsing();
		m_FarmerUpgrades.StopUsing();
		SetBaggedObject(null);
		base.StopUsing(AndDestroy);
	}

	protected new void Awake()
	{
		m_Tile = null;
		base.Awake();
		m_FarmerCarry = base.gameObject.AddComponent<FarmerCarry>();
		m_FarmerInventory = base.gameObject.AddComponent<FarmerInventory>();
		m_FarmerUpgrades = base.gameObject.AddComponent<FarmerUpgrades>();
		m_FarmerAction = base.gameObject.AddComponent<FarmerAction>();
		m_FarmerClothes = base.gameObject.AddComponent<FarmerClothes>();
		m_Animator = GetComponent<Animator>();
		SetupStates();
		m_Visible = false;
	}

	private new void OnDestroy()
	{
		base.OnDestroy();
		m_Visible = false;
	}

	private void UpdateMoveDelay()
	{
		m_MoveNormalDelay = m_MoveBaseDelay / (m_ActionSpeedScale * m_OverallSpeedScale);
	}

	public void SetMoveSpeedScale(float Speed)
	{
		m_MoveSpeedScale = Speed;
		UpdateMoveDelay();
	}

	public void SetMoveBaseDelay(float Delay)
	{
		float tileDelay = TileManager.Instance.GetTileDelay(m_TileCoord, m_TypeIdentifier == ObjectType.Worker);
		m_MoveBaseDelay = Delay + tileDelay;
		if (m_MoveBaseDelay < 0.05f)
		{
			m_MoveBaseDelay = 0.05f;
		}
		UpdateMoveDelay();
	}

	public void SetActionSpeedScale(float Speed)
	{
		m_ActionSpeedScale = Speed;
		UpdateMoveDelay();
	}

	public void SetOverallSpeedScale(float Scale)
	{
		m_OverallSpeedScale = Scale;
		UpdateMoveDelay();
	}

	private void SetupStates()
	{
		m_States = new FarmerStateBase[53];
		AddState(State.Moving, new FarmerStateMove());
		AddState(State.Turning, new FarmerStateTurn());
		AddState(State.Shovel, new FarmerStateShovel());
		AddState(State.Hoe, new FarmerStateHoe());
		AddState(State.Seed, new FarmerStateSeed());
		AddState(State.Scythe, new FarmerStateScythe());
		AddState(State.Chopping, new FarmerStateChopping());
		AddState(State.Created, new FarmerStateCreated());
		AddState(State.Milking, new FarmerStateMilking());
		AddState(State.Held, new FarmerStateHeld());
		AddState(State.Make, new FarmerStateMake());
		AddState(State.Dropping, new FarmerStateDropping());
		AddState(State.PickingUp, new FarmerStatePickingUp());
		AddState(State.Taking, new FarmerStateTaking());
		AddState(State.Adding, new FarmerStateAdding());
		AddState(State.Stowing, new FarmerStateStowing());
		AddState(State.Recalling, new FarmerStateRecalling());
		AddState(State.Cycling, new FarmerStateCycling());
		AddState(State.Swapping, new FarmerStateSwapping());
		AddState(State.Listening, new FarmerStateListening());
		AddState(State.Mining, new FarmerStateMining());
		AddState(State.StoneCutting, new FarmerStateStoneCutting());
		AddState(State.Dredging, new FarmerStateDredging());
		AddState(State.DroppingSoil, new FarmerStateDroppingSoil());
		AddState(State.WorkerSelect, new FarmerStateWorkerSelect());
		AddState(State.Paused, new FarmerStatePaused());
		AddState(State.Flail, new FarmerStateFlail());
		AddState(State.Recharge, new FarmerStateRecharge());
		AddState(State.Shearing, new FarmerStateShearing());
		AddState(State.Knitting, new FarmerStateKnitting());
		AddState(State.Play, new FarmerStatePlaying());
		AddState(State.Fishing, new FarmerStateFishing());
		AddState(State.Netting, new FarmerStateNetting());
		AddState(State.Hammering, new FarmerStateHammering());
		AddState(State.Sweeping, new FarmerStateSweeping());
		AddState(State.Emptying, new FarmerStateEmptying());
		AddState(State.Engaged, new FarmerStateEngaged());
		AddState(State.BeingEngaged, new FarmerStateBeingEngaged());
		AddState(State.RocketRide, new FarmerStateRocketRide());
		AddState(State.RocketKicked, new FarmerStateRocketKicked());
		AddState(State.CelebrateMakeInHand, new FarmerStateCelebrateMakeInHand());
		AddState(State.CelebrateEraComplete, new FarmerStateCelebrateEraComplete());
		AddState(State.SpawnJump, new FarmerStateSpawnJump());
		AddState(State.JumpTurf, new FarmerStateJumpTurf());
		AddState(State.Upgrading, new FarmerStateUpgrading());
		AddState(State.UpgradedWait, new FarmerStateUpgradedWait());
		AddState(State.Upgraded, new FarmerStateUpgraded());
		AddState(State.ModAction, new FarmerStateModAction());
		for (int i = 0; i < m_States.Length; i++)
		{
			if (m_States[i] != null)
			{
				m_States[i].SetFarmer(this);
			}
		}
		m_LearningState = new FarmerStateLearning();
		m_LearningState.SetFarmer(this);
	}

	private void AddState(State NewState, FarmerStateBase NewStateObject)
	{
		m_States[(int)NewState] = NewStateObject;
		m_States[(int)NewState].SetState(NewState);
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		Node["Carry"] = new JSONObject();
		JSONNode node = Node["Carry"];
		m_FarmerCarry.Save(node);
		Node["Inv"] = new JSONObject();
		node = Node["Inv"];
		m_FarmerInventory.Save(node);
		Node["Up"] = new JSONObject();
		node = Node["Up"];
		m_FarmerUpgrades.Save(node);
		Node["Clothes"] = new JSONObject();
		node = Node["Clothes"];
		m_FarmerClothes.Save(node);
		if ((bool)m_EngagedObject)
		{
			JSONUtils.Set(Node, "Engaged", m_EngagedObject.m_UniqueID);
		}
		JSONUtils.Set(Node, "Energy", m_Energy);
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		CollectionManager.Instance.AddCollectable("Farmer", this);
		JSONNode jSONNode = Node["Up"];
		if (jSONNode != null && !jSONNode.IsNull)
		{
			m_FarmerUpgrades.Load(jSONNode);
		}
		jSONNode = Node["Inv"];
		m_FarmerInventory.Load(jSONNode);
		jSONNode = Node["Carry"];
		m_FarmerCarry.Load(jSONNode);
		jSONNode = Node["Clothes"];
		if (jSONNode != null && !jSONNode.IsNull)
		{
			m_FarmerClothes.Load(jSONNode);
		}
		m_TempEngagedID = JSONUtils.GetAsInt(Node, "Engaged", -1);
		m_Energy = JSONUtils.GetAsFloat(Node, "Energy", 0f);
		ScaleSpeedToEnergy();
		m_GoToTilePosition = m_TileCoord;
	}

	public override void UpdateUniqueIDs()
	{
		base.UpdateUniqueIDs();
		if (m_TempEngagedID == -1)
		{
			return;
		}
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(m_TempEngagedID, ErrorCheck: false);
		if (objectFromUniqueID == null)
		{
			Debug.Log("Farmer.UpdateUniqueIDs : Couldn't find object with UID " + m_TempEngagedID);
		}
		else if ((!ObjectTypeList.Instance.GetIsBuilding(objectFromUniqueID.m_TypeIdentifier) || !(objectFromUniqueID.GetComponent<Converter>() == null)) && objectFromUniqueID.m_TypeIdentifier != ObjectType.Worker)
		{
			SendAction(new ActionInfo(ActionType.EngageObject, default(TileCoord), objectFromUniqueID.GetComponent<Actionable>()));
			if (ObjectTypeList.Instance.GetIsBuilding(objectFromUniqueID.m_TypeIdentifier))
			{
				objectFromUniqueID.GetComponent<Converter>().ResumeConversion(this);
			}
		}
	}

	public override void SendAction(ActionInfo Info)
	{
		if ((bool)m_EngagedObject && Info.m_Action != ActionType.DisengageObject && Info.m_Action != ActionType.CycleObject && Info.m_Action != ActionType.StowObject && Info.m_Action != ActionType.Shout)
		{
			m_EngagedObject.SendAction(Info);
		}
		else
		{
			m_FarmerAction.SendAction(Info);
		}
		if (Info.m_Action == ActionType.Refresh)
		{
			m_FarmerCarry.SendAction(Info);
			if (m_EngagedObject == null && !m_BeingHeld)
			{
				base.transform.position = m_TileCoord.ToWorldPositionTileCentered();
			}
		}
		base.SendAction(Info);
	}

	public override bool CanDoAction(ActionInfo Info, bool RightNow)
	{
		if ((bool)m_EngagedObject)
		{
			return m_EngagedObject.CanDoAction(Info, RightNow);
		}
		return m_FarmerAction.CanDoAction(Info, RightNow);
	}

	public override bool RequestGoTo(TileCoord Destination, Actionable TargetObject = null, bool LessOne = false, int Range = 0)
	{
		if (m_State != 0 && m_State != State.Moving && !m_Learning && m_State != State.RequestMove && m_State != State.RequestFind)
		{
			return false;
		}
		m_RequestGoToFarmer = false;
		if (m_State == State.Moving || m_State == State.RequestMove)
		{
			m_RequestGoToFarmer = true;
			m_RequestGoToDestination = Destination;
			m_RequestGoToTarget = TargetObject;
			m_RequestGoToLessOne = LessOne;
			m_RequestRange = Range;
			return true;
		}
		SetState(State.RequestMove);
		return base.RequestGoTo(Destination, TargetObject, LessOne, Range);
	}

	public override bool StartGoTo(TileCoord Destination, Actionable TargetObject = null, bool LessOne = false, int Range = 0)
	{
		m_RequestGoToFarmer = false;
		if (m_Path.Count == 0)
		{
			SetBaggedObject(null);
		}
		if (m_State != 0 && m_State != State.Moving && !m_Learning && m_State != State.RequestMove && m_State != State.RequestFind)
		{
			return false;
		}
		SetState(State.Moving);
		if (!base.StartGoTo(Destination, TargetObject, LessOne, Range))
		{
			if (m_State == State.Moving)
			{
				SetState(State.None);
			}
			return false;
		}
		return true;
	}

	public override void ObstructionEncountered()
	{
		base.ObstructionEncountered();
		StopGoTo();
		SetState(State.RequestMove);
		base.RequestGoTo(m_GoToTilePosition, m_GoToTargetObject, m_GoToLessOne);
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		m_HolderObject = Holder.GetComponent<GoTo>();
		SetState(State.Held);
		m_FarmerCarry.Disable(Disabled: true);
		base.ActionBeingHeld(Holder);
		m_FarmerCarry.Disable(Disabled: false);
		base.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		if (m_Tile != null && m_Tile.m_Farmer == this)
		{
			m_Tile.m_Farmer = null;
		}
	}

	protected override void ActionDropped(Actionable PreviousHolder, TileCoord DropLocation)
	{
		SetState(State.None);
		m_FarmerCarry.Disable(Disabled: true);
		base.ActionDropped(PreviousHolder, DropLocation);
		m_FarmerCarry.Disable(Disabled: false);
		m_FarmerCarry.FarmerMoved();
		m_Tile.m_Farmer = this;
	}

	protected void CheckNewRequest()
	{
		if (m_RequestGoToFarmer)
		{
			m_RequestGoToFarmer = false;
			SetState(State.None);
			RequestGoTo(m_RequestGoToDestination, m_RequestGoToTarget, m_RequestGoToLessOne, m_RequestRange);
		}
	}

	public override void NextGoTo()
	{
		m_FarmerCarry.FarmerMoved();
		CheckNoEnergy();
		if (m_Energy != 0f || m_Nudge)
		{
			if (m_TestTargetCloseDistance != 0f && (bool)m_GoToTargetObject && (m_GoToTargetObject.transform.position - base.transform.position).magnitude < m_TestTargetCloseDistance)
			{
				m_TestTargetCloseDistance = 0f;
				EndGoTo();
			}
			else
			{
				CheckNewRequest();
				base.NextGoTo();
			}
		}
	}

	protected override void TileCoordChanged(TileCoord Position)
	{
		if (m_Tile != null && m_Tile.m_Farmer == this)
		{
			m_Tile.m_Farmer = null;
			RouteFinding.UpdateTileWalk(m_TileCoord.x, m_TileCoord.y);
		}
		base.TileCoordChanged(Position);
		m_Tile = TileManager.Instance.GetTile(m_TileCoord);
		m_Tile.m_Farmer = this;
		RouteFinding.UpdateTileWalk(m_TileCoord.x, m_TileCoord.y);
		ModManager.Instance.CheckPlayerOrBotEnterOrExitTileCallback(Position, m_UniqueID);
	}

	public override void EndGoTo()
	{
		m_RequestGoToFarmer = false;
		base.EndGoTo();
		m_TestTargetCloseDistance = 0f;
		SetState(State.None);
		m_Nudge = false;
	}

	public void AddEnergy(float Energy)
	{
		m_AddEnergyTimer = 0.5f;
		m_AddEnergy = Energy;
		m_AddEnergyPosition = base.transform.position;
		AudioManager.Instance.StartEvent("WorkerRestarted", this);
	}

	private void UpdateEnergy()
	{
		if (m_AddEnergyTimer > 0f)
		{
			m_AddEnergyTimer -= TimeManager.Instance.m_NormalDelta;
			if (m_AddEnergyTimer <= 0f)
			{
				m_AddEnergyTimer = 0f;
				m_Energy += m_AddEnergy;
				m_AddEnergy = 0f;
				SetActionSpeedScale(1f);
				base.transform.position = m_AddEnergyPosition;
			}
			else if ((int)(m_AddEnergyTimer * 60f) % 4 < 2)
			{
				base.transform.position = m_AddEnergyPosition + new Vector3(0f, 0f, 0f);
			}
			else
			{
				base.transform.position = m_AddEnergyPosition + new Vector3(0f, 0.5f, 0f);
			}
		}
	}

	public void CheckNoEnergy()
	{
		if (m_Energy <= 0f)
		{
			m_Energy = 0f;
		}
	}

	protected bool NeedRecharged()
	{
		return m_Energy + m_AddEnergy == 0f;
	}

	protected void ScaleSpeedToEnergy()
	{
		if (m_Energy < 10f && !m_Nudge)
		{
			float actionSpeedScale = m_Energy / 10f * 0.75f + 0.25f;
			SetActionSpeedScale(actionSpeedScale);
			if (NeedRecharged())
			{
				GetComponent<Worker>().SetLayer(Layers.WorkersDead);
			}
		}
		else
		{
			SetActionSpeedScale(1f);
		}
	}

	public virtual void SetState(State NewState, bool Abort = false)
	{
		if (m_States[(int)m_State] != null)
		{
			m_States[(int)m_State].EndState();
		}
		m_State = NewState;
		m_StateTimer = 0f;
		if (m_States[(int)NewState] != null)
		{
			m_States[(int)NewState].StartState();
		}
		if (NewState == State.None)
		{
			CheckNoEnergy();
		}
	}

	public void UpdateState()
	{
		if (m_Learning)
		{
			m_LearningState.UpdateState();
			if (m_State == State.Engaged || m_State == State.Adding || m_State == State.Taking || m_State == State.SpawnJump)
			{
				m_States[(int)m_State].UpdateState();
			}
		}
		else if (m_States[(int)m_State] != null)
		{
			m_States[(int)m_State].UpdateState();
		}
		m_StateTimer += TimeManager.Instance.m_NormalDelta * m_ActionSpeedScale * m_OverallSpeedScale;
	}

	public void SetInterruptState(State NewState)
	{
		if (m_InterruptState == State.Total)
		{
			m_InterruptPosition = m_ModelRoot.transform.position;
			m_InterruptRotation = base.transform.rotation;
		}
		if (m_States[(int)m_InterruptState] != null)
		{
			m_States[(int)m_InterruptState].EndState();
		}
		State interruptState = m_InterruptState;
		m_InterruptState = NewState;
		m_InterruptStateTimer = 0f;
		if (m_States[(int)NewState] != null)
		{
			m_States[(int)NewState].StartState();
		}
		if ((m_InterruptState == State.Paused || interruptState == State.Paused) && m_TypeIdentifier == ObjectType.Worker)
		{
			GetComponent<Worker>().m_WorkerInfoPanel.UpdateStatusImage();
		}
		if (m_InterruptState == State.Total)
		{
			m_ModelRoot.transform.position = m_InterruptPosition;
			base.transform.rotation = m_InterruptRotation;
		}
	}

	public void UpdateInterruptState()
	{
		if (m_States[(int)m_InterruptState] != null)
		{
			m_States[(int)m_InterruptState].UpdateState();
		}
		m_InterruptStateTimer += TimeManager.Instance.m_NormalDelta;
	}

	protected override void TurnEnded()
	{
		SetState(State.None);
	}

	public void SetBaggedObject(TileCoordObject NewObject)
	{
		if ((bool)m_BaggedObject)
		{
			BaggedManager.Instance.Remove(m_BaggedObject);
		}
		m_BaggedObject = NewObject;
		if ((bool)NewObject)
		{
			BaggedManager.Instance.Add(m_BaggedObject, this);
		}
	}

	public void SetBaggedTile(TileCoord Position)
	{
		if (m_BaggedTile.x != 0 || m_BaggedTile.y != 0)
		{
			BaggedManager.Instance.Remove(m_BaggedTile);
		}
		m_BaggedTile = Position;
		if (m_BaggedTile.x != 0 || m_BaggedTile.y != 0)
		{
			BaggedManager.Instance.Add(m_BaggedTile, this);
		}
	}

	public void CreateParticles(Vector3 Position, string Name)
	{
		MyParticles newParticles = ParticlesManager.Instance.CreateParticles(Name, Position, Quaternion.Euler(-90f, 0f, 0f));
		ParticlesManager.Instance.DestroyParticles(newParticles, WaitUntilNoParticles: true);
	}

	public void SpawnEnd(BaseClass NewObject, bool Success)
	{
		m_SpawnEnd = true;
		m_SpawnSuccess = Success;
		if (m_States[(int)m_State] != null)
		{
			m_States[(int)m_State].SpawnEnd(NewObject);
		}
	}

	public void SpawnAbort(BaseClass NewObject)
	{
		State state = m_State;
		bool isTypeFillable = ToolFillable.GetIsTypeFillable(m_FarmerCarry.GetTopObjectType());
		if (m_States[(int)m_State] != null)
		{
			m_States[(int)m_State].SpawnAbort(NewObject);
		}
		bool isTypeFillable2 = ToolFillable.GetIsTypeFillable(m_FarmerCarry.GetTopObjectType());
		if ((isTypeFillable || !isTypeFillable2) && state == State.Adding)
		{
			if (NewObject == null)
			{
				m_FarmerAction.UndoAdd(null);
			}
			else
			{
				m_FarmerAction.UndoAdd(NewObject.GetComponent<Actionable>());
			}
		}
	}

	public bool GetIsUsingHeldObject()
	{
		if (m_State == State.PickingUp)
		{
			Holdable topObject = m_FarmerCarry.GetTopObject();
			if ((bool)topObject && ((bool)topObject.GetComponent<ToolFillable>() || (bool)topObject.GetComponent<ToolBasket>()))
			{
				return true;
			}
		}
		if (m_State != 0 && m_State != State.NoScript && m_State != State.Moving && m_State != State.RequestMove && m_State != State.Stowing && m_State != State.Recalling && m_State != State.PickingUp && m_State != State.Dropping && m_State != State.Adding && m_State != State.Taking && m_State != State.Recharge && m_State != State.RequestFind && m_State != State.Engaged)
		{
			return true;
		}
		return false;
	}

	public virtual void EngagedObjectActionActive(bool Active, bool Success)
	{
	}

	public bool GetIsDoingSomething()
	{
		if (m_State != 0)
		{
			if (m_State != State.Engaged)
			{
				return true;
			}
			if ((bool)m_EngagedObject.GetActionInfo(new GetActionInfo(GetAction.IsBusy)))
			{
				return true;
			}
		}
		return false;
	}

	public void StartActionUpgrade(AFO Info)
	{
		m_FarmerAction.m_CurrentInfo = null;
		if (m_State != State.Engaged || m_EngagedObject == null || !Vehicle.GetIsTypeVehicle(m_EngagedObject.m_TypeIdentifier))
		{
			SetState(State.UpgradedWait);
		}
	}

	private void EndAddUpgrade(AFO Info)
	{
		m_FarmerUpgrades.AttemptAdd(Info.m_Object.GetComponent<Holdable>());
		if (m_State != State.Engaged || m_EngagedObject == null || !Vehicle.GetIsTypeVehicle(m_EngagedObject.m_TypeIdentifier))
		{
			SetState(State.Upgraded);
			Info.m_Actioner.GetComponent<Farmer>().SetState(State.Upgrading);
		}
	}

	private void AbortAddUpgrade(AFO Info)
	{
		SetState(State.None);
	}

	private ActionType GetActionFromUpgrade(AFO Info)
	{
		Info.m_StartAction = StartActionUpgrade;
		Info.m_EndAction = EndAddUpgrade;
		Info.m_AbortAction = AbortAddUpgrade;
		Info.m_FarmerState = State.Adding;
		if (Info.m_Object == null)
		{
			return ActionType.Fail;
		}
		if (!m_FarmerUpgrades.CanAdd(Info.m_Object.GetComponent<Holdable>()))
		{
			return ActionType.Fail;
		}
		return ActionType.AddResource;
	}

	private void StartAddAnything(AFO Info)
	{
		m_FarmerAction.m_CurrentInfo = null;
		SetState(State.Taking);
		m_FarmerCarry.AddCarry(Info.m_Object.GetComponent<Holdable>());
	}

	private void EndAddAnything(AFO Info)
	{
		SetState(State.None);
	}

	private void AbortAddAnything(AFO Info)
	{
		m_FarmerCarry.RemoveTopObject();
	}

	private ActionType GetActionFromAnything(AFO Info)
	{
		Info.m_FarmerState = State.Adding;
		Info.m_StartAction = StartAddAnything;
		Info.m_EndAction = EndAddAnything;
		Info.m_AbortAction = AbortAddAnything;
		if (m_State != 0)
		{
			return ActionType.Fail;
		}
		if (!m_FarmerCarry.CanAddCarry(Info.m_ObjectType))
		{
			return ActionType.Fail;
		}
		if (m_Energy == 0f)
		{
			return ActionType.Fail;
		}
		if (m_TypeIdentifier == ObjectType.Worker)
		{
			Worker component = base.gameObject.GetComponent<Worker>();
			string requirementsOut = "";
			if (component.m_WorkerIndicator.m_NoTool && (bool)Info.m_Object && m_States[(int)component.m_WorkerIndicator.m_NoToolState].IsToolAcceptable(Info.m_Object.GetComponent<Holdable>()))
			{
				requirementsOut = "FNOBotBrokenTool";
			}
			Info.m_RequirementsOut = requirementsOut;
			if (Info.m_RequirementsIn != "" && Info.m_RequirementsIn != Info.m_RequirementsOut)
			{
				return ActionType.Fail;
			}
		}
		return ActionType.AddResource;
	}

	private void StartTakeNothing(AFO Info)
	{
		m_FarmerAction.m_CurrentObject = this;
		m_FarmerAction.m_CurrentInfo = null;
		SetState(State.Adding);
		Holdable holdable = m_FarmerCarry.RemoveTopObject();
		holdable.ForceHighlight(Highlighted: false);
		Info.m_Actioner.GetComponent<Farmer>().m_FarmerCarry.AddCarry(holdable);
	}

	private void EndTakeNothing(AFO Info)
	{
		SetState(State.None);
	}

	private ActionType GetActionFromNothing(AFO Info)
	{
		Info.m_FarmerState = State.Taking;
		Info.m_StartAction = StartTakeNothing;
		Info.m_EndAction = EndTakeNothing;
		if (m_State != 0)
		{
			return ActionType.Fail;
		}
		if (!Info.m_Actioner.GetComponent<Farmer>().m_FarmerCarry.CanAddCarry(m_FarmerCarry.GetTopObjectType()))
		{
			return ActionType.Fail;
		}
		return ActionType.TakeResource;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (Info.m_ActionType == AFO.AT.AltSecondary && Upgrade.GetIsTypeUpgrade(Info.m_ObjectType))
		{
			return GetActionFromUpgrade(Info);
		}
		if (Info.m_ActionType == AFO.AT.Secondary)
		{
			if (Info.m_ObjectType != ObjectTypeList.m_Total)
			{
				return GetActionFromAnything(Info);
			}
			if (m_FarmerCarry.GetTopObjectType() != ObjectTypeList.m_Total)
			{
				return GetActionFromNothing(Info);
			}
		}
		return ActionType.Total;
	}

	public void StartLearning(FarmerPlayer Target)
	{
		m_Learning = true;
		m_LearningTarget = Target;
		m_LearningState.StartState();
	}

	public void StopLearning()
	{
		m_LearningState.EndState();
		m_Learning = false;
	}

	protected bool GetStateSpecialTool(State NewState, ObjectType NewType, BaseClass Target)
	{
		if (NewState == State.Chopping)
		{
			if (Target == null)
			{
				return NewType == ObjectType.Rock;
			}
			if (Fish.GetIsTypeFish(Target.m_TypeIdentifier))
			{
				if (NewType != ObjectType.RockSharp)
				{
					return NewType == ObjectType.ToolBlade;
				}
				return true;
			}
			return NewType == ObjectType.Rock;
		}
		if ((NewState == State.Mining && NewType == ObjectType.Rock) || (NewState == State.Scythe && NewType == ObjectType.RockSharp) || (NewState == State.Flail && NewType == ObjectType.Stick))
		{
			return true;
		}
		if (NewState == State.Shovel && NewType == ObjectType.Stick && (Target == null || Target.m_TypeIdentifier != ObjectType.Bush))
		{
			return true;
		}
		return false;
	}

	public void RocketKicked(Vector3 StartPosition)
	{
		base.gameObject.SetActive(value: true);
		SpawnAnimationManager.Instance.AddJump(this, StartPosition, base.transform.position, 4f);
	}

	public void BeginAnimationBlur()
	{
		if (m_States[(int)m_State] != null)
		{
			m_States[(int)m_State].BeginAnimationBlur();
		}
	}

	public void EndAnimationBlur()
	{
		if (m_States[(int)m_State] != null)
		{
			m_States[(int)m_State].EndAnimationBlur();
		}
	}

	public void DoAnimationFunction()
	{
		if (m_States[(int)m_State] != null)
		{
			m_States[(int)m_State].DoAnimationFunction();
		}
	}

	public void DoAnimationAction()
	{
		if (m_States[(int)m_State] != null)
		{
			m_States[(int)m_State].DoAnimationAction();
		}
	}

	public void StartAnimation(string Name)
	{
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
		}
		m_Animator.Play(Name, -1, 0f);
	}

	public virtual void DestroyTopObject()
	{
		m_FarmerCarry.DestroyTopObject();
	}

	protected virtual void Update()
	{
		if (m_Energy != 0f || m_Nudge)
		{
			if (m_InterruptState != State.Total)
			{
				UpdateInterruptState();
			}
			else
			{
				UpdateState();
			}
		}
		else
		{
			UpdateEnergy();
		}
	}

	private void OnBecameVisible()
	{
		m_Visible = true;
	}

	private void OnBecameInvisible()
	{
		m_Visible = false;
	}
}
