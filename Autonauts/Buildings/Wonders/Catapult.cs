using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class Catapult : Wonder
{
	private enum State
	{
		Idle,
		StartDelay,
		Fire,
		ReleaseJump,
		Release,
		Reset,
		Total
	}

	private static int m_MaxTileDistance = 100;

	private static float m_FireDelay = 0.1f;

	private static float m_ReleaseDelay = 1f;

	private static float m_ResetDelay = 5f;

	private State m_State;

	private float m_StateTimer;

	private Transform m_IngredientsRoot;

	private TileCoord m_TargetCoord;

	private Actionable m_HeldObject;

	private Transform m_Pivot;

	private Transform m_Axis;

	private PlaySound m_RewindingSound;

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(-1, -1), new TileCoord(1, 1), new TileCoord(0, 2));
		m_HeldObject = null;
		SetState(State.Idle);
		GetStartTarget();
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_IngredientsRoot = ObjectUtils.FindDeepChild(m_ModelRoot.transform, "IngredientPoint");
		m_Pivot = ObjectUtils.FindDeepChild(m_ModelRoot.transform, "Pivot");
		m_Axis = ObjectUtils.FindDeepChild(m_ModelRoot.transform, "Axis");
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		if ((bool)m_HeldObject)
		{
			m_HeldObject.StopUsing();
			m_HeldObject = null;
		}
		base.StopUsing(AndDestroy);
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		JSONUtils.Set(Node, "TargetX", m_TargetCoord.x);
		JSONUtils.Set(Node, "TargetY", m_TargetCoord.y);
		JSONUtils.Set(Node, "State", (int)m_State);
		if ((bool)m_HeldObject)
		{
			JSONNode jSONNode = new JSONObject();
			string saveNameFromIdentifier = ObjectTypeList.Instance.GetSaveNameFromIdentifier(m_HeldObject.GetComponent<BaseClass>().m_TypeIdentifier);
			JSONUtils.Set(jSONNode, "ID", saveNameFromIdentifier);
			m_HeldObject.GetComponent<Savable>().Save(jSONNode);
			Node["Object"] = jSONNode;
		}
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		TileCoord targetTile = default(TileCoord);
		targetTile.x = JSONUtils.GetAsInt(Node, "TargetX", 0);
		targetTile.y = JSONUtils.GetAsInt(Node, "TargetY", 0);
		SetTargetTile(targetTile);
		JSONNode jSONNode = Node["Object"];
		if (jSONNode != null && !jSONNode.IsNull)
		{
			JSONNode jSONNode2 = jSONNode;
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromSaveName(jSONNode2, new Vector3(0f, 0f, 0f), Quaternion.identity);
			if ((bool)baseClass)
			{
				baseClass.GetComponent<Savable>().Load(jSONNode2);
				m_HeldObject = baseClass.GetComponent<Actionable>();
				m_HeldObject.transform.SetParent(m_IngredientsRoot.transform);
				m_HeldObject.transform.localPosition = default(Vector3);
				baseClass.GetComponent<Savable>().SetIsSavable(IsSavable: false);
				baseClass.GetComponent<Actionable>().SendAction(new ActionInfo(ActionType.BeingHeld, default(TileCoord), this));
			}
		}
		State asInt = (State)JSONUtils.GetAsInt(Node, "State", 0);
		SetState(asInt);
	}

	public TileCoord GetTargetTile()
	{
		return m_TargetCoord;
	}

	private void UpdateTarget()
	{
		TileCoord tileCoord = m_TargetCoord - (m_TileCoord + new TileCoord(0, -1));
		float num = Mathf.Atan2(tileCoord.y, tileCoord.x);
		m_ModelRoot.transform.rotation = Quaternion.Euler(0f, 57.29578f * num + 90f, 0f);
	}

	public void SetTargetTile(TileCoord NewCoord)
	{
		m_TargetCoord = NewCoord;
		UpdateTarget();
	}

	private void GetStartTarget()
	{
		List<TileCoord> adjacentTiles = GetAdjacentTiles();
		if (adjacentTiles.Count > 0)
		{
			SetTargetTile(adjacentTiles[Random.Range(0, adjacentTiles.Count)]);
		}
	}

	public override void Moved()
	{
		base.Moved();
		GetStartTarget();
	}

	private bool IsBusy()
	{
		if (m_State != 0)
		{
			return true;
		}
		return false;
	}

	public override void SendAction(ActionInfo Info)
	{
		base.SendAction(Info);
		if (IsBusy() || ((bool)m_Engager && Info.m_Action != ActionType.Disengaged))
		{
			return;
		}
		switch (Info.m_Action)
		{
		case ActionType.Engaged:
			m_Engager = Info.m_Object;
			if ((bool)m_Engager.GetComponent<FarmerPlayer>())
			{
				GameStateManager.Instance.StartCatapult(this);
			}
			break;
		case ActionType.Disengaged:
			m_Engager = null;
			break;
		}
	}

	public override bool CanDoAction(ActionInfo Info, bool RightNow)
	{
		if (IsBusy() && RightNow)
		{
			return false;
		}
		if ((bool)m_Engager && Info.m_Action != ActionType.Disengaged)
		{
			return false;
		}
		switch (Info.m_Action)
		{
		case ActionType.Engaged:
			if (GameStateManager.Instance.GetCurrentState().GetComponent<GameStateTeachWorker2>() == null && m_Engager == null)
			{
				return true;
			}
			return false;
		case ActionType.Disengaged:
			if ((bool)m_Engager)
			{
				return true;
			}
			return false;
		default:
			return false;
		}
	}

	public override object GetActionInfo(GetActionInfo Info)
	{
		switch (Info.m_Action)
		{
		case GetAction.IsMovable:
			if (IsBusy())
			{
				return false;
			}
			return !m_DoingAction;
		case GetAction.IsBusy:
			if (IsBusy())
			{
				return true;
			}
			return m_DoingAction;
		case GetAction.IsDeletable:
			if (IsBusy())
			{
				return false;
			}
			return true;
		default:
			return base.GetActionInfo(Info);
		}
	}

	private void StartAddAnything(AFO Info)
	{
		Actionable @object = Info.m_Object;
		_ = @object.m_TypeIdentifier;
		@object.GetComponent<Savable>().SetIsSavable(IsSavable: false);
		@object.transform.position = m_IngredientsRoot.transform.position;
		m_HeldObject = @object;
	}

	private void EndAddAnything(AFO Info)
	{
		Info.m_Object.SendAction(new ActionInfo(ActionType.BeingHeld, default(TileCoord), this));
		m_HeldObject.transform.SetParent(m_IngredientsRoot.transform);
		SetState(State.StartDelay);
	}

	private void AbortAddAnything(AFO Info)
	{
		Actionable @object = Info.m_Object;
		_ = @object.m_TypeIdentifier;
		m_HeldObject = null;
		@object.GetComponent<Savable>().SetIsSavable(IsSavable: true);
	}

	private bool CanAcceptObject(Actionable NewObject)
	{
		if (NewObject == null)
		{
			return false;
		}
		if (BaseClass.GetTierFromType(NewObject.m_TypeIdentifier) > 4)
		{
			return false;
		}
		return true;
	}

	protected override ActionType GetActionFromAnything(AFO Info)
	{
		Info.m_StartAction = StartAddAnything;
		Info.m_EndAction = EndAddAnything;
		Info.m_AbortAction = AbortAddAnything;
		Info.m_FarmerState = Farmer.State.Adding;
		if ((bool)m_Engager)
		{
			return ActionType.Fail;
		}
		if (IsBusy())
		{
			return ActionType.Fail;
		}
		if (!CanAcceptObject(Info.m_Object))
		{
			return ActionType.Fail;
		}
		if (!GetIsTileAcceptable(m_TargetCoord))
		{
			return ActionType.Fail;
		}
		return ActionType.AddResource;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (Info.m_ActionType == AFO.AT.Primary)
		{
			Info.m_FarmerState = Farmer.State.Engaged;
			if (IsBusy())
			{
				return ActionType.Fail;
			}
			if (GameStateManager.Instance.GetCurrentState().GetComponent<GameStateTeachWorker2>() == null && m_Engager == null)
			{
				return ActionType.EngageObject;
			}
			return ActionType.Fail;
		}
		if (Info.m_ActionType == AFO.AT.Secondary)
		{
			ActionType actionFromAnything = GetActionFromAnything(Info);
			if (actionFromAnything != ActionType.Total)
			{
				return actionFromAnything;
			}
		}
		return ActionType.Total;
	}

	public void GetArc(out float Height, out float Delay)
	{
		TileCoord tileCoord = m_TargetCoord - (m_TileCoord + new TileCoord(0, -1));
		Delay = tileCoord.Magnitude() * 0.075f;
		Height = tileCoord.Magnitude() * 0.5f + 10f;
	}

	private void Release()
	{
		if (!(m_HeldObject == null))
		{
			m_HeldObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
			m_HeldObject.SendAction(new ActionInfo(ActionType.Dropped, m_TileCoord, this));
			m_HeldObject.transform.position = m_IngredientsRoot.transform.position;
			if (Sign.GetIsTypeSign(m_HeldObject.m_TypeIdentifier))
			{
				m_HeldObject.GetComponent<Sign>().CheckCoordChanged(m_TargetCoord);
			}
			GetArc(out var Height, out var Delay);
			Vector3 endPosition = m_TargetCoord.ToWorldPositionTileCentered();
			SpawnAnimationManager.Instance.AddJump(m_HeldObject, m_HeldObject.transform.position, endPosition, Height, Delay, null, DustPuff: true);
			m_HeldObject = null;
			AudioManager.Instance.StartEvent("CatapultRelease", this);
		}
	}

	private void SetState(State NewState)
	{
		m_State = NewState;
		m_StateTimer = 0f;
		ModManager.Instance.CheckBuildingStateChangedCallback(m_UniqueID, m_State.ToString());
	}

	private void UpdateFire()
	{
		float num = m_StateTimer / m_FireDelay;
		if (num >= 1f)
		{
			num = 1f;
		}
		float x = num * -90f;
		m_Pivot.localRotation = Quaternion.Euler(x, 0f, 0f);
		if (num >= 1f)
		{
			Release();
			m_ModelRoot.transform.localPosition = new Vector3(0f, 0.5f, 0f);
			m_ModelRoot.transform.localRotation = m_ModelRoot.transform.localRotation * Quaternion.Euler(10f, 0f, 0f);
			SetState(State.ReleaseJump);
		}
	}

	private void UpdateReset()
	{
		float num = m_StateTimer / m_ResetDelay;
		if (num >= 1f)
		{
			num = 1f;
		}
		float x = (1f - num) * -90f;
		m_Pivot.localRotation = Quaternion.Euler(x, 0f, 0f);
		m_Axis.localRotation = Quaternion.Euler(-720f * TimeManager.Instance.m_NormalDelta, 0f, 0f) * m_Axis.localRotation;
		if (num >= 1f)
		{
			AudioManager.Instance.StopEvent(m_RewindingSound);
			AudioManager.Instance.StartEvent("CatapultReset", this);
			SetState(State.Idle);
		}
	}

	public bool GetIsTileAcceptable(TileCoord NewCoord)
	{
		if ((NewCoord - m_TileCoord).Magnitude() > (float)m_MaxTileDistance)
		{
			return false;
		}
		TileManager.Instance.GetTileWalkable(NewCoord, out var _, out var _, out var _, out var PlayerWalk, TestFarmer: true, out var _);
		if (!PlayerWalk)
		{
			return false;
		}
		return true;
	}

	private void Update()
	{
		switch (m_State)
		{
		case State.StartDelay:
			if (m_StateTimer > 0.25f)
			{
				SetState(State.Fire);
			}
			break;
		case State.Fire:
			UpdateFire();
			break;
		case State.ReleaseJump:
			if (m_StateTimer > 0.05f)
			{
				m_ModelRoot.transform.localPosition = new Vector3(0f, 0f, 0f);
				UpdateTarget();
				SetState(State.Release);
			}
			break;
		case State.Release:
			if (m_StateTimer > m_ReleaseDelay)
			{
				m_RewindingSound = AudioManager.Instance.StartEvent("CatapultRewinding", this, Remember: true);
				SetState(State.Reset);
			}
			break;
		case State.Reset:
			UpdateReset();
			break;
		}
		m_StateTimer += TimeManager.Instance.m_NormalDelta;
	}
}
