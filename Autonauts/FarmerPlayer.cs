using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class FarmerPlayer : Farmer
{
	public enum WhistleCall
	{
		Select,
		Cancel,
		DropAll,
		ToMe,
		Total
	}

	[HideInInspector]
	public bool m_GoToBuffered;

	private TileCoord m_GoToBufferPosition;

	private Actionable m_GoToBufferedObject;

	private AFO.AT m_GoToBufferedAltAction;

	[HideInInspector]
	public FarmerWhistle m_FarmerWhistle;

	private bool m_StartNewPath;

	[HideInInspector]
	public bool m_Teaching;

	private Worker m_TeachingTarget;

	[HideInInspector]
	public List<ActionInfo> m_TeachingActions;

	private AFO.AT m_AltAction;

	[HideInInspector]
	public FarmerPlayerWorkerSelect m_FarmerPlayerWorkerSelect;

	private float m_WhistleDelay;

	private FarmerPlayerTarget m_FarmerPlayerTarget;

	public GameObject m_Scooter;

	private PlaySound m_ScooterSound;

	[HideInInspector]
	public FarmerPlayerStatusIndicator m_Indicator;

	private ObjectType m_PreviousAssociatedObject;

	private bool m_Devil;

	private MyParticles m_Sweat;

	public int m_TileX;

	public int m_TileY;

	private bool m_AutoAction;

	private bool m_AutoRMBWorker;

	private float m_SweatTimer;

	public override void Restart()
	{
		base.Restart();
		m_StartNewPath = false;
		if (!ObjectTypeList.m_Loading)
		{
			CollectionManager.Instance.AddCollectable("FarmerPlayer", this);
		}
		m_GoToBuffered = false;
		m_FarmerCarry.SetCapacity(VariableManager.Instance.GetVariableAsInt(ObjectType.FarmerPlayer, "CarrySize"));
		m_FarmerInventory.SetCapacity(VariableManager.Instance.GetVariableAsInt(ObjectType.FarmerPlayer, "InventoryCapacity"));
		m_FarmerPlayerWorkerSelect.Restart();
		m_Teaching = false;
		m_TeachingActions = new List<ActionInfo>();
		m_PreviousAssociatedObject = ObjectTypeList.m_Total;
		m_AutoRMBWorker = false;
	}

	protected new void Awake()
	{
		base.Awake();
		m_FarmerWhistle = base.gameObject.AddComponent<FarmerWhistle>();
		GameObject original = (GameObject)Resources.Load("Prefabs/WorldObjects/Other/FarmerPlayerStatusIndicator", typeof(GameObject));
		Transform parent = null;
		if ((bool)HudManager.Instance)
		{
			parent = HudManager.Instance.m_IndicatorsRootTransform;
		}
		m_Indicator = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, parent).GetComponent<FarmerPlayerStatusIndicator>();
		m_Indicator.SetFarmer(this);
		original = (GameObject)Resources.Load("Prefabs/WorldObjects/Other/FarmerPlayerWorkerSelect", typeof(GameObject));
		m_FarmerPlayerWorkerSelect = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, parent).GetComponent<FarmerPlayerWorkerSelect>();
		m_FarmerPlayerWorkerSelect.SetFarmerPlayer(this);
		original = (GameObject)Resources.Load("Prefabs/WorldObjects/Other/FarmerPlayerTarget", typeof(GameObject));
		m_FarmerPlayerTarget = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, null).GetComponent<FarmerPlayerTarget>();
		m_FarmerPlayerTarget.SetActive(Active: false);
		if ((bool)ParticlesManager.Instance)
		{
			m_Sweat = ParticlesManager.Instance.CreateParticles("PlayerSweat", default(Vector3), Quaternion.identity);
			m_Sweat.transform.SetParent(base.transform);
			m_Sweat.transform.localPosition = new Vector3(0f, 2.3f, 0f);
			m_Sweat.gameObject.SetActive(value: false);
		}
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_FarmerCarry.GetNodes();
	}

	protected new void OnDestroy()
	{
		if ((bool)m_Indicator)
		{
			Object.Destroy(m_Indicator.gameObject);
		}
		if ((bool)m_FarmerPlayerWorkerSelect)
		{
			Object.Destroy(m_FarmerPlayerWorkerSelect.gameObject);
		}
		DestroyScooter();
		if ((bool)m_FarmerPlayerTarget)
		{
			Object.Destroy(m_FarmerPlayerTarget);
		}
		if ((bool)m_Sweat && (bool)ParticlesManager.Instance)
		{
			ParticlesManager.Instance.DestroyParticles(m_Sweat);
		}
		base.OnDestroy();
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		CollectionManager.Instance.AddCollectable("FarmerPlayer", this);
	}

	public override bool IsSelectable()
	{
		return false;
	}

	public void SetIsTeaching(bool Teaching, Worker Target = null)
	{
		m_Teaching = Teaching;
		if (m_Teaching)
		{
			m_TeachingActions.Clear();
		}
		m_TeachingTarget = Target;
	}

	public void SendActionTeaching(ActionInfo Info)
	{
		if (!m_Teaching || !(Info.m_Object != m_TeachingTarget) || ((Info.m_Position.x != 0 || Info.m_Position.y != 0) && !PlotManager.Instance.GetPlotAtTile(Info.m_Position).m_Visible))
		{
			return;
		}
		bool flag = false;
		if (Info.m_ActionType == AFO.AT.Secondary && Info.m_ObjectType == ObjectType.Worker)
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(Info.m_ObjectUID, ErrorCheck: false);
			if ((bool)objectFromUniqueID)
			{
				Worker component = objectFromUniqueID.GetComponent<Worker>();
				if (m_FarmerCarry.GetTopObjectType() == ObjectTypeList.m_Total && component.m_FarmerCarry.GetTopObjectType() != ObjectTypeList.m_Total)
				{
					flag = true;
				}
			}
		}
		if (!flag)
		{
			m_TeachingActions.Add(new ActionInfo(Info));
		}
	}

	public override void SendAction(ActionInfo Info)
	{
		SendActionTeaching(Info);
		base.SendAction(Info);
	}

	private void DoSwapEngaged(TileCoord Destination, Worker NewWorker)
	{
		if (m_State == State.Engaged && NewWorker.m_State == State.Engaged)
		{
			return;
		}
		if (m_State == State.Engaged)
		{
			if (!(NewWorker.m_FarmerCarry.GetTopObject() != null))
			{
				Actionable engagedObject = m_EngagedObject;
				Vector3 position = base.transform.position;
				Vector3 position2 = NewWorker.transform.position;
				engagedObject.SendAction(new ActionInfo(ActionType.Disengaged, default(TileCoord)));
				m_EngagedObject = null;
				SetBaggedObject(null);
				UpdatePositionToTilePosition(NewWorker.m_TileCoord);
				SpawnAnimationManager.Instance.AddJump(this, position, position2, 5f, 0.2f, this);
				SetState(State.SpawnJump);
				NewWorker.m_EngagedObject = engagedObject;
				NewWorker.m_EngagedObject.SendAction(new ActionInfo(ActionType.Engaged, default(TileCoord), NewWorker));
				NewWorker.SetBaggedObject(null);
				SpawnAnimationManager.Instance.AddJump(NewWorker, position2, position, 5f, 0.2f, NewWorker);
				NewWorker.SetState(State.SpawnJump);
			}
		}
		else if (!(m_FarmerCarry.GetTopObject() != null))
		{
			Actionable engagedObject2 = NewWorker.m_EngagedObject;
			Vector3 position3 = NewWorker.transform.position;
			Vector3 position4 = base.transform.position;
			engagedObject2.SendAction(new ActionInfo(ActionType.Disengaged, default(TileCoord)));
			NewWorker.SetState(State.None);
			NewWorker.m_EngagedObject = null;
			NewWorker.SetBaggedObject(null);
			NewWorker.UpdatePositionToTilePosition(m_TileCoord);
			SpawnAnimationManager.Instance.AddJump(NewWorker, position3, position4, 5f, 0.2f, NewWorker);
			NewWorker.SetState(State.SpawnJump);
			m_EngagedObject = engagedObject2;
			SetState(State.Engaged);
			m_EngagedObject.SendAction(new ActionInfo(ActionType.Engaged, default(TileCoord), this));
			SetBaggedObject(null);
			SpawnAnimationManager.Instance.AddJump(this, position4, position3, 5f, 0.2f, this);
			SetState(State.SpawnJump);
		}
	}

	public bool IsWorkerCloseEnoughToTrade(Worker NewWorker)
	{
		if ((NewWorker.transform.position - base.transform.position).magnitude < Tile.m_Size * (float)FarmerAction.m_ThrowDistanceTiles)
		{
			return true;
		}
		return false;
	}

	private bool DoRMBOnWorker(TileCoord Destination, Worker NewWorker)
	{
		if ((m_State == State.Moving || !GetIsDoingSomething()) && (NewWorker.m_State == State.Moving || NewWorker.m_Learning || !NewWorker.GetIsDoingSomething()) && IsWorkerCloseEnoughToTrade(NewWorker) && NewWorker.m_Energy > 0f && (NewWorker.m_WorkerInterpreter.GetCurrentScript() == null || NewWorker.m_WorkerIndicator.m_NoTool || NewWorker.m_WorkerIndicator.m_State == WorkerStatusIndicator.State.Busy) && (m_State == State.Engaged || NewWorker.m_State == State.Engaged))
		{
			DoSwapEngaged(Destination, NewWorker);
			return true;
		}
		return false;
	}

	private bool DoLMBOnTrainTrackStop(TrainTrackStop NewStop)
	{
		if (m_EngagedObject == null || !Minecart.GetIsTypeMinecart(m_EngagedObject.m_TypeIdentifier))
		{
			return false;
		}
		NewStop.TogglePlayerStop();
		return true;
	}

	private bool DoLMBOnTrainTrackPoints(TrainTrackPoints NewPoints)
	{
		if (m_EngagedObject == null || !Minecart.GetIsTypeMinecart(m_EngagedObject.m_TypeIdentifier))
		{
			return false;
		}
		NewPoints.TogglePlayerSwitch();
		return true;
	}

	public void GoToAndAction(TileCoord Destination, Actionable TargetObject = null, AFO.AT NewInputType = AFO.AT.Primary)
	{
		if (((bool)TargetObject && (bool)TargetObject.GetComponent<Worker>() && NewInputType == AFO.AT.Secondary && DoRMBOnWorker(Destination, TargetObject.GetComponent<Worker>())) || ((bool)TargetObject && TrainTrackStop.GetIsTypeTrainTrackStop(TargetObject.m_TypeIdentifier) && NewInputType == AFO.AT.Primary && DoLMBOnTrainTrackStop(TargetObject.GetComponent<TrainTrackStop>())) || ((bool)TargetObject && TrainTrackPoints.GetIsTypeTrainTrackPoints(TargetObject.m_TypeIdentifier) && NewInputType == AFO.AT.Primary && DoLMBOnTrainTrackPoints(TargetObject.GetComponent<TrainTrackPoints>())))
		{
			return;
		}
		m_FarmerCarry.GetTopObject();
		bool flag = false;
		if (m_State == State.Moving || m_State == State.RequestMove)
		{
			flag = true;
		}
		else if ((bool)m_EngagedObject && (bool)m_EngagedObject.GetComponent<Vehicle>())
		{
			if (m_EngagedObject.GetComponent<Vehicle>().m_State == Vehicle.State.Moving || m_EngagedObject.GetComponent<Vehicle>().m_State == Vehicle.State.RequestMove)
			{
				flag = true;
			}
		}
		else if (m_State != 0)
		{
			if (m_State == State.Taking || m_State == State.Adding || m_State == State.CelebrateEraComplete || m_State == State.Cycling || m_State == State.Dropping || m_State == State.DroppingSoil || m_State == State.Emptying || (m_State == State.PickingUp && !ToolFillable.GetIsTypeFillable(m_FarmerCarry.GetTopObjectType())) || m_State == State.Recalling || m_State == State.RequestFind || m_State == State.RequestMove || m_State == State.RocketKicked || m_State == State.RocketRide || m_State == State.Swapping || m_State == State.WorkerSelect || m_State == State.Engaged)
			{
				return;
			}
			m_States[(int)m_State].AbortAction();
		}
		if (flag)
		{
			m_GoToBuffered = true;
			m_GoToBufferPosition = Destination;
			m_GoToBufferedObject = TargetObject;
			m_GoToBufferedAltAction = NewInputType;
			m_GoToTargetObject = null;
			return;
		}
		m_GoToBuffered = false;
		ActionInfo actionInfo = new ActionInfo(ActionType.MoveTo, Destination, TargetObject, "", "", NewInputType);
		ActionType actionType = AutoAction(actionInfo, AndAction: false);
		m_StartNewPath = false;
		m_AltAction = NewInputType;
		if ((bool)TargetObject && (bool)TargetObject.GetComponent<TileCoordObject>() && actionType != ActionType.Total && actionType != ActionType.DropAll)
		{
			SetBaggedObject(TargetObject.GetComponent<TileCoordObject>());
		}
		else
		{
			SetBaggedObject(null);
		}
		if (m_Teaching && GameStateManager.Instance.GetActualState() == GameStateManager.State.TeachWorker)
		{
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateTeachWorker2>().m_ScriptEdit.AboutToAddInstructions();
		}
		if (actionType == ActionType.DisengageObject && (m_EngagedObject.GetComponent<Vehicle>() == null || Minecart.GetIsTypeMinecart(m_EngagedObject.m_TypeIdentifier)))
		{
			SendAction(new ActionInfo(actionType, Destination, TargetObject, actionInfo.m_Value, actionInfo.m_Value2, NewInputType, actionInfo.m_ActionRequirement, m_FarmerCarry.GetTopObjectType()));
		}
		else if ((actionType == ActionType.MoveForwards || actionType == ActionType.MoveBackwards) && m_EngagedObject.GetComponent<Vehicle>() != null && Minecart.GetIsTypeMinecart(m_EngagedObject.m_TypeIdentifier))
		{
			SendAction(new ActionInfo(actionType, Destination, TargetObject, actionInfo.m_Value, actionInfo.m_Value2, NewInputType, actionInfo.m_ActionRequirement, m_FarmerCarry.GetTopObjectType()));
		}
		else
		{
			m_AutoAction = PlotManager.Instance.GetPlotAtTile(Destination).m_Visible;
			if (Actionable.m_ReusableActionFromObject.m_MoveRange == 0)
			{
				if (!Actionable.m_ReusableActionFromObject.m_AdjacentTile)
				{
					SendAction(new ActionInfo(ActionType.MoveTo, Destination, TargetObject, actionInfo.m_Value, actionInfo.m_Value2, NewInputType, actionInfo.m_ActionRequirement, m_FarmerCarry.GetTopObjectType()));
				}
				else
				{
					SendAction(new ActionInfo(ActionType.MoveToLessOne, Destination, TargetObject, actionInfo.m_Value, actionInfo.m_Value2, NewInputType, actionInfo.m_ActionRequirement, m_FarmerCarry.GetTopObjectType()));
				}
			}
			else
			{
				string value = Actionable.m_ReusableActionFromObject.m_MoveRange.ToString();
				SendAction(new ActionInfo(ActionType.MoveToRange, Destination, TargetObject, actionInfo.m_Value, value, NewInputType, actionInfo.m_ActionRequirement, m_FarmerCarry.GetTopObjectType()));
			}
		}
		if ((bool)TargetObject)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.ClickObject, Bot: false, TargetObject.m_TypeIdentifier, this);
		}
	}

	public void CheckPlot()
	{
		if (!m_Plot.m_Visible)
		{
			AudioManager.Instance.StartEvent("PlotAppear");
			m_Plot.SetVisible(Visible: true, DoUncoverEffect: true);
			BadgeManager.Instance.AddEvent(BadgeEvent.Type.PlotsUncovered);
			QuestManager.Instance.AddEvent(QuestEvent.Type.PlotUncovered, Bot: false, 0, this);
		}
	}

	public void EnableSweat(bool Enable)
	{
		if (Enable)
		{
			m_Sweat.gameObject.SetActive(value: true);
			m_SweatTimer = 0f;
		}
		else
		{
			m_Sweat.gameObject.SetActive(value: false);
		}
	}

	private void UpdateSweat()
	{
		if (m_Sweat.gameObject.activeSelf)
		{
			if (!m_Sweat.m_Particles.isPlaying)
			{
				m_Sweat.Play();
			}
			m_SweatTimer += TimeManager.Instance.m_NormalDelta;
			if (m_SweatTimer > 0.5f)
			{
				m_SweatTimer = 0f;
				AudioManager.Instance.StartEvent("PlayerHeavyLift", this);
			}
		}
	}

	private void UpdateSpeed()
	{
		float num = VariableManager.Instance.GetVariableAsFloat(ObjectType.FarmerPlayer, "BaseDelay");
		if (m_FarmerUpgrades.ContainsObjectType(ObjectType.UpgradePlayerMovementCrude))
		{
			num -= VariableManager.Instance.GetVariableAsFloat(ObjectType.UpgradePlayerMovementCrude, "Delay");
		}
		else if (m_FarmerUpgrades.ContainsObjectType(ObjectType.UpgradePlayerMovementGood))
		{
			num -= VariableManager.Instance.GetVariableAsFloat(ObjectType.UpgradePlayerMovementGood, "Delay");
		}
		else if (m_FarmerUpgrades.ContainsObjectType(ObjectType.UpgradePlayerMovementSuper))
		{
			num -= VariableManager.Instance.GetVariableAsFloat(ObjectType.UpgradePlayerMovementSuper, "Delay");
		}
		Holdable topObject = m_FarmerCarry.GetTopObject();
		if (topObject != null)
		{
			int weight = topObject.m_Weight;
			if (weight == 3)
			{
				num += 0.15f;
			}
			else if (weight == 4)
			{
				num += 0.25f;
			}
			else if (weight > 4)
			{
				num += 1f;
			}
		}
		SetMoveBaseDelay(num);
	}

	private void DestroyScooter()
	{
		if (m_ScooterSound != null)
		{
			AudioManager.Instance.StopEvent(m_ScooterSound);
			m_ScooterSound = null;
		}
		if ((bool)m_Scooter)
		{
			Object.Destroy(m_Scooter);
			m_Scooter = null;
		}
	}

	private void CreateScooter()
	{
		ObjectType movementUpgrade = m_FarmerUpgrades.GetMovementUpgrade();
		GameObject original = (GameObject)Resources.Load(ObjectTypeList.Instance.GetModelNameFromIdentifier(movementUpgrade), typeof(GameObject));
		m_Scooter = Object.Instantiate(original, base.transform.position, Quaternion.identity, null);
		m_ScooterSound = AudioManager.Instance.StartEvent("ScooterMove", this, Remember: true);
	}

	public override bool StartGoTo(TileCoord Destination, Actionable TargetObject = null, bool LessOne = false, int Range = 0)
	{
		if (m_Path.Count == 0)
		{
			DestroyScooter();
		}
		UpdateSpeed();
		bool flag = base.StartGoTo(Destination, TargetObject, LessOne, Range);
		m_FarmerPlayerTarget.SetActive(Active: false);
		if (m_State != 0)
		{
			if (flag)
			{
				DestroyScooter();
				if (m_FarmerUpgrades.GetContainsUpgradePlayerMovement())
				{
					CreateScooter();
				}
				m_FarmerPlayerTarget.SetPosition(Destination);
				m_FarmerPlayerTarget.SetActionType(m_AltAction);
				m_FarmerPlayerTarget.SetActive(Active: true);
			}
			else if (Destination == m_TileCoord)
			{
				m_FarmerPlayerTarget.SetPosition(Destination);
				m_FarmerPlayerTarget.SetActionType(m_AltAction);
				m_FarmerPlayerTarget.SetActive(Active: true);
				m_FarmerPlayerTarget.Delay();
			}
		}
		return flag;
	}

	public void ShowTarget()
	{
		m_FarmerPlayerTarget.SetPosition(m_TileCoord);
		m_FarmerPlayerTarget.SetActionType(AFO.AT.Primary);
		m_FarmerPlayerTarget.SetActive(Active: true);
		m_FarmerPlayerTarget.Flash();
	}

	public void DoBuffered()
	{
		m_GoToBuffered = false;
		if (m_State == State.Moving)
		{
			SetBaggedObject(null);
			SetBaggedTile(new TileCoord(0, 0));
			SetState(State.None);
			DestroyScooter();
		}
		GoToAndAction(m_GoToBufferPosition, m_GoToBufferedObject, m_GoToBufferedAltAction);
	}

	public override void NextGoTo()
	{
		if (m_GoToBuffered)
		{
			DoBuffered();
			return;
		}
		if (m_AutoAction && m_AutoRMBWorker && IsWorkerCloseEnoughToTrade(m_GoToTargetObject.GetComponent<Worker>()))
		{
			EndGoTo();
			return;
		}
		UpdateSpeed();
		base.NextGoTo();
		CheckPlot();
	}

	public override void StopGoTo()
	{
		base.StopGoTo();
		m_FarmerPlayerTarget.SetActive(Active: false);
		DestroyScooter();
		CheckPlot();
	}

	public override void EndGoTo()
	{
		base.EndGoTo();
		StopGoTo();
		if (m_AutoAction)
		{
			m_AutoRMBWorker = false;
			AutoAction(new ActionInfo(ActionType.Total, m_GoToTilePosition, m_GoToTargetObject, "", "", m_AltAction, "", m_FarmerCarry.GetTopObjectType()), AndAction: true);
		}
		if (m_GoToBuffered)
		{
			m_GoToBuffered = false;
			GoToAndAction(m_GoToBufferPosition, m_GoToBufferedObject, m_GoToBufferedAltAction);
		}
	}

	public override void MoveDirection(TileCoord Direction)
	{
		base.MoveDirection(Direction);
		QuestManager.Instance.AddPlayerMove(this);
	}

	public override ActionType GetAutoAction(ActionInfo Info)
	{
		if (Info.m_Object == null)
		{
			return ActionType.Total;
		}
		Holdable holdable = m_FarmerCarry.GetTopObject();
		ObjectType objectType = ObjectTypeList.m_Total;
		if (holdable != null)
		{
			objectType = holdable.m_TypeIdentifier;
		}
		Actionable actioner = this;
		if (m_State == State.Engaged && Info.m_Object != m_EngagedObject)
		{
			actioner = m_EngagedObject;
			objectType = Info.m_ObjectType;
			holdable = null;
		}
		Actionable.m_ReusableActionFromObject.Init(holdable, objectType, actioner, Info.m_ActionType, "", Info.m_Position);
		ActionType actionType = Info.m_Object.GetActionFromObjectSafe(Actionable.m_ReusableActionFromObject);
		Info.m_ActionRequirement = Actionable.m_ReusableActionFromObject.m_RequirementsOut;
		State farmerState = Actionable.m_ReusableActionFromObject.m_FarmerState;
		if (farmerState != State.Total && m_States[(int)farmerState] != null)
		{
			Actionable.m_ReusableActionFromObject.m_AdjacentTile = m_States[(int)farmerState].GetIsAdjacentTile(Info.m_Position, Info.m_Object);
			int moveRange = 0;
			if (actionType == ActionType.UseInHands && (bool)holdable && farmerState == State.Fishing && holdable.m_TypeIdentifier != ObjectType.ToolFishingStick)
			{
				moveRange = VariableManager.Instance.GetVariableAsInt(holdable.m_TypeIdentifier, "UseRange", CheckValid: false);
			}
			Actionable.m_ReusableActionFromObject.m_MoveRange = moveRange;
		}
		if ((bool)holdable && GetStateSpecialTool(farmerState, objectType, Info.m_Object) && m_FarmerCarry.GetCarryCount() > 1)
		{
			actionType = ActionType.Total;
			Actionable.m_ReusableActionFromObject.m_AdjacentTile = false;
			Actionable.m_ReusableActionFromObject.m_MoveRange = 0;
		}
		return actionType;
	}

	public ActionType GetAutoAltSecondaryAction(ActionInfo Info)
	{
		if ((bool)m_EngagedObject && (bool)m_EngagedObject.GetActionInfo(new GetActionInfo(GetAction.IsDisengagable)))
		{
			if ((bool)m_EngagedObject.GetComponent<Vehicle>())
			{
				TileCoord nearestAdjacentTile = m_EngagedObject.GetComponent<Vehicle>().GetNearestAdjacentTile(Info.m_Position);
				TileManager.Instance.GetTileWalkable(nearestAdjacentTile, out var _, out var _, out var _, out var PlayerWalk, TestFarmer: true, out var _);
				if (PlayerWalk)
				{
					return ActionType.DisengageObject;
				}
			}
			else if (!m_EngagedObject.GetComponent<ResearchStation>())
			{
				return ActionType.DisengageObject;
			}
		}
		return GetAutoAction(Info);
	}

	public ActionType AutoAction(ActionInfo Info, bool AndAction)
	{
		ActionType actionType = ((Info.m_ActionType != 0 && Info.m_ActionType != AFO.AT.Secondary && Info.m_ActionType != AFO.AT.AltPrimary) ? GetAutoAltSecondaryAction(Info) : ((m_State != State.Engaged || !(Info.m_Object != m_EngagedObject)) ? GetAutoAction(Info) : m_EngagedObject.GetAutoAction(Info)));
		if (!PlotManager.Instance.GetPlotAtTile(Info.m_Position).m_Visible)
		{
			actionType = ActionType.Total;
			Actionable.m_ReusableActionFromObject.m_AdjacentTile = false;
			Actionable.m_ReusableActionFromObject.m_MoveRange = 0;
		}
		if (AndAction)
		{
			switch (actionType)
			{
			case ActionType.Fail:
				SetBaggedObject(null);
				SetBaggedTile(new TileCoord(0, 0));
				AudioManager.Instance.StartEvent("PlayerActionFail");
				break;
			default:
				if (m_State == State.Engaged)
				{
					if (actionType == ActionType.UseInHands)
					{
						SendAction(new ActionInfo(actionType, Info.m_Position, Info.m_Object, Info.m_Value, Info.m_Value2, Info.m_ActionType, Info.m_ActionRequirement, m_FarmerCarry.GetTopObjectType()));
						break;
					}
					ActionInfo info = new ActionInfo(actionType, Info.m_Position, Info.m_Object, Info.m_Value, Info.m_Value2, Info.m_ActionType, Info.m_ActionRequirement, m_FarmerCarry.GetTopObjectType());
					SendActionTeaching(info);
					m_EngagedObject.SendAction(info);
				}
				else
				{
					SendAction(new ActionInfo(actionType, Info.m_Position, Info.m_Object, Info.m_Value, Info.m_Value2, Info.m_ActionType, Info.m_ActionRequirement, m_FarmerCarry.GetTopObjectType()));
				}
				break;
			case ActionType.Total:
				break;
			}
		}
		return actionType;
	}

	protected override void TileCoordChanged(TileCoord Position)
	{
		base.TileCoordChanged(Position);
		ObjectType objectType = ObjectTypeList.m_Total;
		Tile tile = TileManager.Instance.GetTile(m_TileCoord);
		if ((bool)tile.m_AssociatedObject)
		{
			objectType = tile.m_AssociatedObject.m_TypeIdentifier;
		}
		if (objectType == ObjectType.Grass || objectType == ObjectType.Bush || Crop.GetIsTypeCrop(objectType) || ((bool)tile.m_AssociatedObject && (bool)tile.m_AssociatedObject.GetComponent<MyTree>()))
		{
			AudioManager.Instance.StartEvent("CropRuffle", this);
		}
		if (m_PreviousAssociatedObject != objectType)
		{
			m_PreviousAssociatedObject = objectType;
		}
	}

	public void Whistle(bool Start, float Delay = 0f)
	{
		m_WhistleDelay = Delay;
		m_FarmerPlayerWorkerSelect.SetActive(Start);
	}

	private void UpdateWhistle()
	{
		if (m_WhistleDelay > 0f)
		{
			m_WhistleDelay -= TimeManager.Instance.m_NormalDelta;
			if (m_WhistleDelay < 0f)
			{
				m_WhistleDelay = 0f;
				m_FarmerPlayerWorkerSelect.SetActive(Active: false);
			}
		}
	}

	public override void SetState(State NewState, bool Abort = false)
	{
		base.SetState(NewState, Abort);
	}

	public bool CanWhistle()
	{
		if (m_State == State.Knitting)
		{
			return false;
		}
		return true;
	}

	public override void DestroyTopObject()
	{
		base.DestroyTopObject();
		m_Indicator.StartNoTool();
	}

	public void CompleteIdea(string[] Frames)
	{
		m_Indicator.CompleteIdea(Frames);
	}

	public void SetDevil(bool Devil)
	{
		if (m_Devil != Devil)
		{
			m_Devil = Devil;
			m_FarmerCarry.PreRefreshPlayerModel();
			m_FarmerClothes.PreRefreshPlayerModel();
			m_FarmerUpgrades.PreRefreshPlayerModel();
			string text = "FarmerPlayer";
			if (m_Devil)
			{
				text = "FarmerPlayerDevil";
			}
			LoadNewModel("Models/Other/" + text);
			m_FarmerCarry.RefreshPlayerModel();
			m_FarmerClothes.RefreshPlayerModel();
			m_FarmerUpgrades.RefreshPlayerModel();
		}
	}

	public void UseWhistle(WhistleCall NewCall)
	{
		Holdable whistle = m_FarmerUpgrades.GetWhistle();
		if (whistle == null)
		{
			string[] array = new string[4] { "WhistleBlown1", "WhistleCancel1", "WhistleDropAll1", "WhistleToMe1" };
			AudioManager.Instance.StartEvent(array[(int)NewCall]);
		}
		else
		{
			AudioManager.Instance.StartEvent(whistle.GetComponent<UpgradePlayerWhistle>().m_WhistleSounds[(int)NewCall]);
		}
	}

	private void UpdateMovementUpgrade()
	{
		if ((bool)m_Scooter)
		{
			Vector3 position = base.transform.position;
			position.y = m_TileCoord.ToWorldPosition().y;
			m_Scooter.transform.position = position;
			m_Scooter.transform.rotation = Quaternion.Euler(0f, -90f, 0f) * base.transform.rotation;
		}
	}

	protected override void Update()
	{
		base.Update();
		m_FarmerWhistle.UpdateWhistleEmanation();
		m_FarmerWhistle.UpdateWhistleModeEffect();
		if (m_StartNewPath)
		{
			m_StartNewPath = false;
			NextGoTo();
		}
		m_TileX = m_TileCoord.x;
		m_TileY = m_TileCoord.y;
		if (TimeManager.Instance.m_NormalTimeEnabled)
		{
			m_Animator.speed = TimeManager.Instance.m_TimeScale;
		}
		else
		{
			m_Animator.speed = 0f;
		}
		UpdateSweat();
		UpdateWhistle();
		UpdateMovementUpgrade();
	}

	public void CheckCarryHeavy()
	{
		if (m_FarmerCarry.GetTopObject() != null && m_FarmerCarry.GetTopObject().GetIsHeavyForPlayer())
		{
			EnableSweat(Enable: true);
		}
		else
		{
			EnableSweat(Enable: false);
		}
	}
}
