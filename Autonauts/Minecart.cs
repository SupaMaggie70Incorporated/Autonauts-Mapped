using System;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class Minecart : Vehicle
{
	private static int m_CarriagesDistance = 2;

	[HideInInspector]
	public GameObject m_Wheels1;

	[HideInInspector]
	public GameObject m_Wheels2;

	private bool m_OnTrack;

	[HideInInspector]
	public TileCoord m_MoveDirection;

	private bool m_StartMoving;

	private bool m_MoveCurve;

	private float m_MoveCurveTimer;

	private float m_MoveCurveDelay;

	private bool m_MoveCurveStart;

	private TrainTrackCurve m_Curve;

	private bool m_EnteringCurve;

	private bool m_MovePoints;

	private float m_MovePointsTimer;

	private float m_MovePointsDelay;

	private int m_MovePointsTile;

	private List<TrainTrackPoints> m_MovePointsSwitchList;

	private TrainTrackPoints m_Points;

	private bool m_EnteringPoints;

	[HideInInspector]
	public GameObject m_DrivePoint;

	protected float m_MovementTimer;

	protected bool m_MovingForwards;

	private bool m_TempMoving;

	protected int m_MaxCarriages;

	protected List<Carriage> m_Carriages;

	private List<int> m_TempCarriageIDs;

	private GameObject m_DirectionIndicator;

	private Material m_DirectionIndicatorMaterial;

	private GameObject m_Handle;

	private bool m_HandleUp;

	private bool m_ForceStop;

	private bool m_ShowPlayerControls;

	public static bool GetIsTypeMinecart(ObjectType NewType)
	{
		if (NewType == ObjectType.Minecart || NewType == ObjectType.Train)
		{
			return true;
		}
		return false;
	}

	public override void Restart()
	{
		base.Restart();
		SetBaseDelay(VariableManager.Instance.GetVariableAsFloat(ObjectType.Minecart, "BaseDelay"));
		m_MoveCurve = false;
		m_MovePoints = false;
		m_OnTrack = false;
		m_MovingForwards = true;
		m_MovePointsSwitchList = new List<TrainTrackPoints>();
		m_MoveSoundName = "MinecartMotion";
		m_DirectionIndicator.SetActive(value: false);
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_MaxCarriages = VariableManager.Instance.GetVariableAsInt(ObjectType.Minecart, "MaxCarriages");
		m_Carriages = new List<Carriage>();
		m_TempCarriageIDs = new List<int>();
		m_Wheels1 = m_ModelRoot.transform.Find("Wheels.001").gameObject;
		m_Wheels2 = m_ModelRoot.transform.Find("Wheels.002").gameObject;
		m_DrivePoint = m_ModelRoot.transform.Find("DrivingPoint").gameObject;
		GameObject original = (GameObject)Resources.Load("Prefabs/DirectionIndicator", typeof(GameObject));
		m_DirectionIndicator = UnityEngine.Object.Instantiate(original, base.transform).gameObject;
		m_DirectionIndicator.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		m_DirectionIndicatorMaterial = m_DirectionIndicator.GetComponent<MeshRenderer>().material;
		if (m_TypeIdentifier == ObjectType.Minecart)
		{
			m_Handle = m_ModelRoot.transform.Find("Handle").gameObject;
		}
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		base.StopUsing(AndDestroy);
		m_DirectionIndicator.SetActive(value: false);
		if (AndDestroy)
		{
			UnityEngine.Object.Destroy(m_DirectionIndicator);
		}
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		if (m_State == State.Moving)
		{
			JSONUtils.Set(Node, "M", Value: true);
		}
		JSONUtils.Set(Node, "F", m_MovingForwards);
		if (m_Carriages.Count > 0)
		{
			JSONArray jSONArray = (JSONArray)(Node["CarriageIDs"] = new JSONArray());
			for (int i = 0; i < m_Carriages.Count; i++)
			{
				jSONArray[i] = m_Carriages[i].m_UniqueID;
			}
		}
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		m_TempMoving = JSONUtils.GetAsBool(Node, "M", DefaultValue: false);
		m_MovingForwards = JSONUtils.GetAsBool(Node, "F", DefaultValue: true);
		JSONArray asArray = Node["CarriageIDs"].AsArray;
		if (asArray != null && !asArray.IsNull)
		{
			m_TempCarriageIDs.Clear();
			for (int i = 0; i < asArray.Count; i++)
			{
				m_TempCarriageIDs.Add(asArray[i].AsInt);
			}
		}
	}

	private void UpdateOnTrack(bool Dropped)
	{
		Tile tile = TileManager.Instance.GetTile(m_TileCoord);
		if (tile.m_Floor == null || tile.m_Floor.GetComponent<TrainTrack>() == null)
		{
			m_OnTrack = false;
			return;
		}
		m_OnTrack = true;
		TrainTrack component = tile.m_Floor.GetComponent<TrainTrack>();
		if (!Dropped)
		{
			return;
		}
		AudioManager.Instance.StartEvent("MinecartConnect", this);
		m_Rotation = (m_Rotation + 2) % 8;
		if ((bool)component.GetComponent<TrainTrackStraight>())
		{
			if (component.GetComponent<TrainTrackStraight>().GetVertical())
			{
				if (m_Rotation == 5 || m_Rotation == 7 || m_Rotation == 4 || m_Rotation == 6)
				{
					m_Rotation = 6;
				}
				else
				{
					m_Rotation = 2;
				}
			}
			else if (m_Rotation == 7 || m_Rotation == 0 || m_Rotation == 1 || m_Rotation == 2)
			{
				m_Rotation = 0;
			}
			else
			{
				m_Rotation = 4;
			}
		}
		else if ((bool)component.GetComponent<TrainTrackCurve>())
		{
			TrainTrackCurve component2 = component.GetComponent<TrainTrackCurve>();
			if (Dropped)
			{
				TileCoord startPosition = component2.GetStartPosition();
				TileCoord endPosition = component2.GetEndPosition();
				TileCoord position = startPosition;
				if ((m_TileCoord - startPosition).Magnitude() > (m_TileCoord - endPosition).Magnitude())
				{
					position = endPosition;
				}
				UpdatePositionToTilePosition(position);
			}
			if (m_TileCoord == component2.GetStartPosition())
			{
				m_Rotation = (component2.m_Rotation + 3) * 2 % 8;
			}
			else
			{
				m_Rotation = component2.m_Rotation * 2 % 8;
			}
		}
		SetRotation(m_Rotation);
	}

	public override void SendAction(ActionInfo Info)
	{
		switch (Info.m_Action)
		{
		case ActionType.MoveTo:
			if (Info.m_ActionType == AFO.AT.AltSecondary)
			{
				if ((bool)m_Engager && m_State == State.None)
				{
					m_Engager.SendAction(new ActionInfo(ActionType.DisengageObject, default(TileCoord)));
				}
				return;
			}
			break;
		case ActionType.MoveForwards:
			m_ActionType = Info.m_ActionType;
			m_MovingForwards = true;
			RequestGoTo(m_TileCoord + GetMoveDirection(), Info.m_Object);
			break;
		case ActionType.MoveBackwards:
			m_ActionType = Info.m_ActionType;
			m_MovingForwards = false;
			RequestGoTo(m_TileCoord + GetMoveDirection(), Info.m_Object);
			break;
		}
		base.SendAction(Info);
		switch (Info.m_Action)
		{
		case ActionType.Dropped:
			UpdateOnTrack(Dropped: true);
			m_MoveDirection = GetMoveDirection();
			UpdateCarriagePosition();
			break;
		case ActionType.Refresh:
		case ActionType.RefreshFirst:
			UpdateOnTrack(Dropped: false);
			m_MoveDirection = GetMoveDirection();
			foreach (int tempCarriageID in m_TempCarriageIDs)
			{
				BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(tempCarriageID, ErrorCheck: false);
				if (Carriage.GetIsTypeCarriage(objectFromUniqueID.m_TypeIdentifier))
				{
					AddCarriage(objectFromUniqueID.GetComponent<Carriage>());
				}
			}
			m_TempCarriageIDs.Clear();
			UpdateCarriagePosition();
			break;
		case ActionType.Disengaged:
			UpdatePlayerEngaged();
			break;
		}
	}

	public override object GetActionInfo(GetActionInfo Info)
	{
		if (Info.m_Action == GetAction.GetObjectType && m_Carriages.Count > 0)
		{
			return m_Carriages[0].GetActionInfo(Info);
		}
		return base.GetActionInfo(Info);
	}

	public override bool CanDoAction(ActionInfo Info, bool RightNow)
	{
		switch (Info.m_Action)
		{
		case ActionType.MoveForwards:
			m_MovingForwards = true;
			RequestGoTo(m_TileCoord + GetMoveDirection(), Info.m_Object);
			return m_Path != null;
		case ActionType.MoveBackwards:
			m_MovingForwards = false;
			RequestGoTo(m_TileCoord + GetMoveDirection(), Info.m_Object);
			return m_Path != null;
		case ActionType.BeingHeld:
			if ((bool)Info.m_Object && !Crane.GetIsTypeCrane(Info.m_Object.m_TypeIdentifier))
			{
				return false;
			}
			break;
		case ActionType.Engaged:
			if (!m_OnTrack)
			{
				return false;
			}
			break;
		case ActionType.Disengaged:
			return true;
		}
		return base.CanDoAction(Info, RightNow);
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		UpdateTrack();
	}

	protected override void ActionDropped(Actionable PreviousHolder, TileCoord DropLocation)
	{
		base.ActionDropped(PreviousHolder, DropLocation);
		UpdateTrack();
	}

	private LinkedSystemTrack GetLinkedSystem()
	{
		Tile tile = TileManager.Instance.GetTile(m_TileCoord);
		if (tile != null && (bool)tile.m_Floor && TrainTrack.GetIsTypeTrainTrack(tile.m_Floor.m_TypeIdentifier) && tile.m_Floor.m_LinkedSystem != null)
		{
			return (LinkedSystemTrack)tile.m_Floor.m_LinkedSystem;
		}
		return null;
	}

	private void UpdateTrack()
	{
		GetLinkedSystem()?.UpdateCounts();
	}

	private void AddCarriage(Carriage NewCarriage)
	{
		if ((bool)NewCarriage)
		{
			m_Carriages.Add(NewCarriage);
			NewCarriage.SetMinecart(this);
			NewCarriage.transform.rotation = base.transform.rotation;
		}
		UpdateCarriagePosition();
	}

	public void RemoveCarriage(Carriage NewCarriage)
	{
		if (m_Carriages.Count != 0)
		{
			NewCarriage.SetMinecart(null);
			m_Carriages.Remove(NewCarriage);
			UpdateCarriagePosition();
		}
	}

	public Carriage RemoveLastCarriage()
	{
		if (m_Carriages.Count == 0)
		{
			return null;
		}
		Carriage carriage = m_Carriages[m_Carriages.Count - 1];
		RemoveCarriage(carriage);
		return carriage;
	}

	public bool GetIsEmpty()
	{
		int num = 0;
		foreach (Carriage carriage in m_Carriages)
		{
			num += carriage.GetStored();
		}
		if (num == 0)
		{
			return true;
		}
		return false;
	}

	public bool GetIsFull()
	{
		int num = 0;
		int num2 = 0;
		foreach (Carriage carriage in m_Carriages)
		{
			if (carriage.m_ObjectType == ObjectTypeList.m_Total)
			{
				return false;
			}
			num += carriage.GetStored();
			num2 += carriage.GetCapacity();
		}
		if (num2 != 0 && num == num2)
		{
			return true;
		}
		return false;
	}

	public Vector3 GetAddPoint()
	{
		if (m_Carriages.Count > 0)
		{
			return m_Carriages[0].transform.position;
		}
		return base.transform.position;
	}

	private void UpdateCarriagePosition()
	{
		if (m_Carriages.Count == 0)
		{
			return;
		}
		Tile tile = TileManager.Instance.GetTile(new TileCoord(base.transform.position));
		if (tile.m_Floor == null || !TrainTrack.GetIsTypeTrainTrack(tile.m_Floor.m_TypeIdentifier))
		{
			return;
		}
		TrainTrack component = tile.m_Floor.GetComponent<TrainTrack>();
		TileCoord tileCoord = m_MoveDirection;
		if (m_MovingForwards)
		{
			tileCoord = -tileCoord;
		}
		int num = 1;
		foreach (Carriage carriage in m_Carriages)
		{
			component.GetCarriagePosition(Tile.m_Size * (float)m_CarriagesDistance * (float)num, tileCoord, m_MovingForwards, base.transform.position, carriage.transform, out var NewPosition, out var NewRotation, m_MovePointsSwitchList, this);
			if (Quaternion.Angle(NewRotation, carriage.transform.rotation) > 90f)
			{
				NewRotation = Quaternion.Euler(0f, 180f, 0f) * NewRotation;
			}
			NewPosition.y = component.GetHeight() + TrainTrack.m_Height;
			carriage.transform.position = NewPosition;
			carriage.transform.rotation = NewRotation;
			carriage.SetTilePosition(new TileCoord(NewPosition));
			num++;
		}
	}

	public void StartActionCranePrimary(AFO Info)
	{
		if (!m_BeingHeld)
		{
			Carriage @object = RemoveLastCarriage();
			Info.m_Actioner.GetComponent<Crane>().SendAction(new ActionInfo(ActionType.Pickup, m_TileCoord, @object));
			AudioManager.Instance.StartEvent("MinecartDisconnect", this);
		}
	}

	private void EndAddCranePrimary(AFO Info)
	{
	}

	private void AbortAddCranePrimary(AFO Info)
	{
		Crane component = Info.m_Actioner.GetComponent<Crane>();
		Actionable heldObject = component.m_HeldObject;
		if (!(heldObject == null) && Carriage.GetIsTypeCarriage(heldObject.m_TypeIdentifier))
		{
			component.SendAction(new ActionInfo(ActionType.DropAll, default(TileCoord)));
			AddCarriage(heldObject.GetComponent<Carriage>());
		}
	}

	private ActionType GetActionFromCranePrimary(AFO Info)
	{
		Info.m_StartAction = StartActionCranePrimary;
		Info.m_EndAction = EndAddCranePrimary;
		Info.m_AbortAction = AbortAddCranePrimary;
		Info.m_FarmerState = Farmer.State.Taking;
		if (m_State != 0)
		{
			return ActionType.Fail;
		}
		if (Info.m_Actioner.GetComponent<Crane>().m_HeldObject != null)
		{
			return ActionType.Fail;
		}
		if (m_Carriages.Count == 0)
		{
			return ActionType.Fail;
		}
		return ActionType.TakeResource;
	}

	public void StartActionCraneSecond(AFO Info)
	{
		if (!m_BeingHeld)
		{
			Crane component = Info.m_Actioner.GetComponent<Crane>();
			Actionable heldObject = component.m_HeldObject;
			if (!(heldObject == null) && Carriage.GetIsTypeCarriage(heldObject.m_TypeIdentifier))
			{
				component.SendAction(new ActionInfo(ActionType.DropAll, default(TileCoord)));
				AddCarriage(heldObject.GetComponent<Carriage>());
				AudioManager.Instance.StartEvent("MinecartConnect", this);
			}
		}
	}

	private void EndAddCraneSecond(AFO Info)
	{
	}

	private void AbortAddCraneSecond(AFO Info)
	{
		Carriage @object = RemoveLastCarriage();
		Info.m_Actioner.GetComponent<Crane>().SendAction(new ActionInfo(ActionType.Pickup, m_TileCoord, @object));
	}

	private ActionType GetActionFromCraneSecond(AFO Info)
	{
		Info.m_StartAction = StartActionCraneSecond;
		Info.m_EndAction = EndAddCraneSecond;
		Info.m_AbortAction = AbortAddCraneSecond;
		Info.m_FarmerState = Farmer.State.Adding;
		if (!m_OnTrack)
		{
			return ActionType.Fail;
		}
		if (m_State != 0)
		{
			return ActionType.Fail;
		}
		if (m_Carriages.Count == m_MaxCarriages)
		{
			return ActionType.Fail;
		}
		if (Info.m_Actioner.GetComponent<Crane>().m_HeldObject == null)
		{
			return ActionType.Fail;
		}
		if (!Carriage.GetIsTypeCarriage(Info.m_Actioner.GetComponent<Crane>().m_HeldObject.m_TypeIdentifier))
		{
			return ActionType.Fail;
		}
		return ActionType.AddResource;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (Info.m_ActionType == AFO.AT.AltPrimary)
		{
			Info.m_FarmerState = Farmer.State.Engaged;
			if (!m_OnTrack)
			{
				return ActionType.Fail;
			}
		}
		if (Info.m_ActionType == AFO.AT.Primary && (bool)Info.m_Actioner.GetComponent<Crane>())
		{
			if (m_Carriages.Count > 0)
			{
				return GetActionFromCranePrimary(Info);
			}
			return GetActionFromCrane(Info);
		}
		if (Info.m_ActionType == AFO.AT.Secondary && (bool)Info.m_Actioner.GetComponent<Crane>())
		{
			return GetActionFromCraneSecond(Info);
		}
		return base.GetActionFromObject(Info);
	}

	public override ActionType GetAutoAction(ActionInfo Info)
	{
		if (Info.m_ActionType == AFO.AT.Primary)
		{
			return GetMoveDirectionFromDestination(Info.m_Position) switch
			{
				1 => ActionType.MoveForwards, 
				-1 => ActionType.MoveBackwards, 
				_ => ActionType.Total, 
			};
		}
		if (Info.m_ActionType == AFO.AT.Secondary)
		{
			return ActionType.Total;
		}
		return base.GetAutoAction(Info);
	}

	public int GetMoveDirectionFromDestination(TileCoord Destination)
	{
		int result = 0;
		Vector3 vector = Destination.ToWorldPositionTileCentered();
		Vector3 normalized = (vector - base.transform.position).normalized;
		Vector3 worldPosition = base.transform.InverseTransformPoint(vector + normalized * 0.01f);
		TileCoord tileCoord = new TileCoord(worldPosition);
		if (Mathf.Abs(tileCoord.y) >= Mathf.Abs(tileCoord.x))
		{
			if (tileCoord.y < 0)
			{
				result = -1;
			}
			else if (tileCoord.y > 0)
			{
				result = 1;
			}
		}
		return result;
	}

	public override bool RequestGoTo(TileCoord Destination, Actionable TargetObject = null, bool LessOne = false, int Range = 0)
	{
		Tile tile = TileManager.Instance.GetTile(m_TileCoord);
		if (tile.m_Floor == null || tile.m_Floor.GetComponent<TrainTrack>() == null)
		{
			return false;
		}
		switch (GetMoveDirectionFromDestination(Destination))
		{
		case 0:
			return false;
		case 1:
			m_MovingForwards = true;
			break;
		default:
			m_MovingForwards = false;
			break;
		}
		StartMoving();
		return false;
	}

	private TileCoord GetMoveDirection(bool CheckForwards = true)
	{
		TileCoord tileCoord = default(TileCoord);
		Tile tile = TileManager.Instance.GetTile(m_TileCoord);
		if (tile.m_Floor == null || tile.m_Floor.GetComponent<TrainTrack>() == null)
		{
			return tileCoord;
		}
		TrainTrack component = tile.m_Floor.GetComponent<TrainTrack>();
		if (component.m_TypeIdentifier == ObjectType.TrainTrack || component.m_TypeIdentifier == ObjectType.TrainTrackBridge)
		{
			if (!component.GetComponent<TrainTrackStraight>().GetVertical())
			{
				if (m_Rotation == 0)
				{
					tileCoord.x = 1;
				}
				else
				{
					tileCoord.x = -1;
				}
			}
			else if (!component.GetComponent<TrainTrackStraight>().m_Cross)
			{
				if (m_Rotation == 6)
				{
					tileCoord.y = -1;
				}
				else
				{
					tileCoord.y = 1;
				}
			}
			else if (m_Rotation == 0)
			{
				tileCoord.x = 1;
			}
			else if (m_Rotation == 4)
			{
				tileCoord.x = -1;
			}
			else if (m_Rotation == 6)
			{
				tileCoord.y = -1;
			}
			else
			{
				tileCoord.y = 1;
			}
		}
		else if (TrainTrackPoints.GetIsTypeTrainTrackPoints(component.m_TypeIdentifier))
		{
			if (component.m_Rotation == 1 || component.m_Rotation == 3)
			{
				if (m_Rotation == 0)
				{
					tileCoord.x = 1;
				}
				else
				{
					tileCoord.x = -1;
				}
			}
			else if (m_Rotation == 6)
			{
				tileCoord.y = -1;
			}
			else
			{
				tileCoord.y = 1;
			}
		}
		else if (component.m_TypeIdentifier == ObjectType.TrainTrackCurve)
		{
			TrainTrackCurve component2 = component.GetComponent<TrainTrackCurve>();
			TileCoord tileCoord2 = m_TileCoord - component2.GetStartPosition();
			TileCoord tileCoord3 = m_TileCoord - component2.GetEndPosition();
			int num = m_Rotation >> 1;
			if (tileCoord2.Magnitude() < tileCoord3.Magnitude())
			{
				tileCoord = component2.GetStartDirection();
				if ((num - 1) % 4 == component2.m_Rotation)
				{
					tileCoord = -tileCoord;
				}
			}
			else
			{
				tileCoord = component2.GetEndDirection();
				if (num == component2.m_Rotation)
				{
					tileCoord = -tileCoord;
				}
			}
		}
		if (CheckForwards && !m_MovingForwards)
		{
			tileCoord.x = -tileCoord.x;
			tileCoord.y = -tileCoord.y;
		}
		return tileCoord;
	}

	public void StartMoving(bool Force = false, bool Forwards = true)
	{
		if (m_State != 0)
		{
			return;
		}
		if (m_TypeIdentifier == ObjectType.Minecart)
		{
			float num = 1f;
			foreach (Carriage carriage in m_Carriages)
			{
				if (carriage.m_WeightPenaltyScaler > num)
				{
					num = carriage.m_WeightPenaltyScaler;
				}
			}
			SetWeightPenalty(num);
			UpdateMoveDelay();
		}
		if (Force)
		{
			m_MovingForwards = Forwards;
		}
		m_MoveDirection = GetMoveDirection();
		if (m_MoveDirection.Magnitude() != 0f && CanMove())
		{
			m_MovePointsSwitchList.Clear();
			UpdateCarriagePosition();
			m_EnteringCurve = false;
			m_EnteringPoints = false;
			m_ActionType = AFO.AT.Primary;
			SetState(State.Moving);
			m_StartMoving = true;
			m_ForceStop = false;
			NextGoTo();
		}
	}

	private void ToggleMoveDirection()
	{
		m_MovingForwards = !m_MovingForwards;
		Debug.Log(m_MovingForwards);
	}

	public bool OnTrack(TrainTrack NewTrack)
	{
		TrainTrack component = TileManager.Instance.GetTile(m_TileCoord).m_Floor.GetComponent<TrainTrack>();
		TileCoord moveDirection = -GetMoveDirection(CheckForwards: false);
		int num = m_CarriagesDistance * m_Carriages.Count;
		if (component.CheckForMinecart(Tile.m_Size * (float)num, moveDirection, component.m_TileCoord.ToWorldPositionTileCentered(), NewTrack, m_MovePointsSwitchList))
		{
			return true;
		}
		return false;
	}

	public void GetHasStop()
	{
	}

	public void ForceStop()
	{
		if (m_State == State.Moving || m_State == State.RequestMove)
		{
			m_ForceStop = true;
		}
	}

	private void ClearSwitchList()
	{
		List<TrainTrackPoints> list = new List<TrainTrackPoints>();
		foreach (TrainTrackPoints movePointsSwitch in m_MovePointsSwitchList)
		{
			bool flag = false;
			Tile tile = TileManager.Instance.GetTile(m_TileCoord);
			if ((bool)tile.m_Floor && tile.m_Floor.GetComponent<TrainTrackPoints>() == movePointsSwitch)
			{
				flag = true;
			}
			foreach (Carriage carriage in m_Carriages)
			{
				tile = TileManager.Instance.GetTile(new TileCoord(carriage.transform.position));
				if ((bool)tile.m_Floor && tile.m_Floor.GetComponent<TrainTrackPoints>() == movePointsSwitch)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				Debug.Log("Remove " + movePointsSwitch.m_UniqueID);
				list.Add(movePointsSwitch);
			}
		}
		foreach (TrainTrackPoints item in list)
		{
			m_MovePointsSwitchList.Remove(item);
		}
	}

	public override void NextGoTo()
	{
		if (m_EnteringCurve)
		{
			MoveCurve(m_Curve);
			m_EnteringCurve = false;
			return;
		}
		if (m_EnteringPoints)
		{
			MovePoints(m_Points);
			m_EnteringPoints = false;
			return;
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
			return;
		}
		if (m_RequestStop)
		{
			m_RequestStop = false;
			SetState(State.None);
			EndGoTo();
			return;
		}
		if (m_ForceStop)
		{
			SetState(State.None);
			EndGoTo();
			m_ForceStop = false;
			return;
		}
		ClearSwitchList();
		Tile tile = TileManager.Instance.GetTile(m_TileCoord);
		TrainTrack component = tile.m_Floor.GetComponent<TrainTrack>();
		TileCoord moveDirection = m_MoveDirection;
		int num = 1;
		if (!m_MovingForwards)
		{
			num += m_CarriagesDistance * m_Carriages.Count;
		}
		if (!component.CheckTrackAhead(Tile.m_Size * (float)num, moveDirection, m_TileCoord.ToWorldPositionTileCentered(), m_MovePointsSwitchList))
		{
			AudioManager.Instance.StartEvent("MinecartHitBuffer", this);
			EndGoTo();
			return;
		}
		if (!m_StartMoving && (component.m_TypeIdentifier == ObjectType.TrainTrack || component.m_TypeIdentifier == ObjectType.TrainTrackBridge))
		{
			Building stop = component.GetComponent<TrainTrackStraight>().GetStop();
			if ((bool)m_Engager && (bool)stop)
			{
				if (m_Engager.m_TypeIdentifier == ObjectType.FarmerPlayer)
				{
					if (stop.GetComponent<TrainTrackStop>().m_PlayerStop)
					{
						if (GameStateManager.Instance.GetActualState() == GameStateManager.State.TeachWorker)
						{
							ActionInfo info = new ActionInfo(ActionType.StopAt, default(TileCoord), stop);
							TeachWorkerScriptEdit.Instance.InsertInstruction(HighInstruction.Type.StopAt, TeachWorkerScriptEdit.Instance.m_Instructions.m_List.Count, info);
							TeachWorkerScriptEdit.Instance.CreateHudInstructions();
						}
						EndGoTo();
						return;
					}
				}
				else if (m_Engager.m_TypeIdentifier == ObjectType.Worker && m_Engager.GetComponent<Worker>().CheckTrackBuilding(stop))
				{
					EndGoTo();
					return;
				}
			}
		}
		m_StartMoving = false;
		bool flag = true;
		if (m_MoveDirection.x != 0)
		{
			flag = false;
		}
		component = TrainTrack.GetConnectedTrackInDirection(m_TileCoord, flag, !flag, m_MoveDirection);
		if (component == tile.m_Floor.GetComponent<TrainTrack>())
		{
			if (component.m_TypeIdentifier == ObjectType.TrainTrackCurve)
			{
				MoveCurve(component.GetComponent<TrainTrackCurve>());
			}
			else if (TrainTrackPoints.GetIsTypeTrainTrackPoints(component.m_TypeIdentifier))
			{
				MovePoints(component.GetComponent<TrainTrackPoints>());
			}
		}
		else
		{
			m_EnteringCurve = component.m_TypeIdentifier == ObjectType.TrainTrackCurve;
			m_Curve = component.GetComponent<TrainTrackCurve>();
			m_EnteringPoints = TrainTrackPoints.GetIsTypeTrainTrackPoints(component.m_TypeIdentifier);
			m_Points = component.GetComponent<TrainTrackPoints>();
			int rotation = m_Rotation;
			MoveDirection(m_MoveDirection);
			SetRotation(rotation);
		}
	}

	private void MoveCurve(Building NewTrack)
	{
		m_MoveCurve = true;
		m_MoveCurveTimer = 0f;
		m_Curve = NewTrack.GetComponent<TrainTrackCurve>();
		float curveRadius = m_Curve.GetCurveRadius();
		float num = (float)Math.PI * 2f * curveRadius * 0.25f;
		m_MoveCurveDelay = num / Tile.m_Size * m_MoveNormalDelay;
		if (m_TileCoord == m_Curve.GetStartPosition())
		{
			m_MoveCurveStart = true;
		}
		else
		{
			m_MoveCurveStart = false;
		}
	}

	private void UpdateMoveCurve()
	{
		if (!m_MoveCurve || (!(m_Engager == null) && !(m_Engager.GetComponent<Farmer>().m_Energy > 0f)))
		{
			return;
		}
		m_MoveCurveTimer += TimeManager.Instance.m_NormalDelta;
		if (m_MoveCurveTimer >= m_MoveCurveDelay)
		{
			m_MoveCurveTimer = m_MoveCurveDelay;
		}
		float num = m_MoveCurveTimer / m_MoveCurveDelay;
		if (!m_MoveCurveStart)
		{
			num = 1f - num;
		}
		Quaternion NewRotation;
		Vector3 curvePosition = m_Curve.GetCurvePosition(num, m_Rotation, out NewRotation);
		base.transform.position = curvePosition;
		base.transform.rotation = NewRotation;
		if (m_MoveCurveTimer >= m_MoveCurveDelay)
		{
			int rotation = 3;
			if (m_MoveCurveStart)
			{
				rotation = 1;
			}
			m_MoveDirection.Rotate(rotation);
			if (m_MoveCurveStart)
			{
				m_Rotation = (m_Rotation + 2) % 8;
			}
			else
			{
				m_Rotation = (m_Rotation + 6) % 8;
			}
			if (m_MoveCurveStart)
			{
				SetTilePosition(m_Curve.GetEndPosition());
			}
			else
			{
				SetTilePosition(m_Curve.GetStartPosition());
			}
			UpdatePositionToTilePosition(m_TileCoord);
			m_MoveCurve = false;
			NextGoTo();
		}
	}

	private void MovePoints(Building NewTrack)
	{
		m_MovePoints = true;
		m_MovePointsTimer = 0f;
		m_Points = NewTrack.GetComponent<TrainTrackPoints>();
		m_MovePointsDelay = 2f * m_MoveNormalDelay;
		m_MovePointsTile = m_Points.GetStartTile(m_TileCoord);
		if (!m_Engager)
		{
			return;
		}
		if (m_Engager.m_TypeIdentifier == ObjectType.FarmerPlayer && m_TileCoord == m_Points.GetStartPosition())
		{
			if (NewTrack.GetComponent<TrainTrackPoints>().m_PlayerSwitch && GameStateManager.Instance.GetActualState() == GameStateManager.State.TeachWorker)
			{
				ActionInfo info = new ActionInfo(ActionType.TurnAt, default(TileCoord), NewTrack);
				TeachWorkerScriptEdit.Instance.InsertInstruction(HighInstruction.Type.TurnAt, TeachWorkerScriptEdit.Instance.m_Instructions.m_List.Count, info);
				TeachWorkerScriptEdit.Instance.CreateHudInstructions();
			}
		}
		else if (m_Engager.m_TypeIdentifier == ObjectType.Worker)
		{
			m_Engager.GetComponent<Worker>().CheckTrackBuilding(NewTrack);
		}
	}

	private void UpdateMovePoints()
	{
		if (!m_MovePoints || (!(m_Engager == null) && !(m_Engager.GetComponent<Farmer>().m_Energy > 0f)))
		{
			return;
		}
		m_MovePointsTimer += TimeManager.Instance.m_NormalDelta;
		if (m_MovePointsTimer >= m_MovePointsDelay)
		{
			m_MovePointsTimer = m_MovePointsDelay;
		}
		float percent = m_MovePointsTimer / m_MovePointsDelay;
		Quaternion NewRotation;
		Vector3 pointsPosition = m_Points.GetPointsPosition(percent, m_MovePointsTile, m_MovePointsSwitchList, m_Rotation, out NewRotation, this);
		base.transform.position = pointsPosition;
		base.transform.rotation = NewRotation;
		if (!(m_MovePointsTimer >= m_MovePointsDelay))
		{
			return;
		}
		if (m_MovePointsTile == 0)
		{
			if (!m_MovePointsSwitchList.Contains(m_Points))
			{
				SetTilePosition(m_Points.GetEndPosition1());
			}
			else
			{
				SetTilePosition(m_Points.GetEndPosition2());
			}
		}
		else
		{
			SetTilePosition(m_Points.GetStartPosition());
		}
		UpdatePositionToTilePosition(m_TileCoord);
		m_MovePoints = false;
		NextGoTo();
	}

	protected override void StopStateMoving()
	{
		float heightOffGround = m_TileCoord.GetHeightOffGround();
		Vector3 position = base.transform.position;
		position.y = heightOffGround;
		base.transform.position = position;
	}

	private void UpdateWheels()
	{
		float num = 1800f;
		TileCoord moveDirection = m_MoveDirection;
		int rotation = m_Rotation;
		if ((rotation == 0 && moveDirection.x < 0) || (rotation == 2 && moveDirection.y < 0) || (rotation == 4 && moveDirection.x > 0) || (rotation == 6 && moveDirection.y > 0))
		{
			num = 0f - num;
		}
		float num2 = num * TimeManager.Instance.m_NormalDelta;
		Quaternion quaternion = Quaternion.Euler(num2, 0f, 0f);
		m_Wheels1.transform.localRotation = m_Wheels1.transform.localRotation * quaternion;
		m_Wheels2.transform.localRotation = m_Wheels2.transform.localRotation * quaternion;
		foreach (Carriage carriage in m_Carriages)
		{
			carriage.UpdateWheels(num2);
		}
	}

	public Vector3 GetEngagerPosition()
	{
		Vector3 position = m_DrivePoint.transform.position;
		if (m_State == State.Moving && m_HandleUp)
		{
			position += new Vector3(0f, 1f, 0f);
		}
		return position;
	}

	protected virtual void UpdateMoveAnimation()
	{
		if (m_State == State.Moving)
		{
			m_MovementTimer += TimeManager.Instance.m_NormalDelta;
			m_HandleUp = (int)(m_MovementTimer * 60f) % 12 < 6;
			float num = -75f;
			if (m_HandleUp)
			{
				num -= 30f;
			}
			m_Handle.transform.localRotation = Quaternion.Euler(num, 0f, 90f);
			UpdateWheels();
		}
	}

	protected virtual bool CanMove()
	{
		if ((bool)m_Engager && m_Engager.m_TypeIdentifier == ObjectType.Worker && m_Engager.GetComponent<Farmer>().m_Energy == 0f)
		{
			return false;
		}
		return true;
	}

	private void UpdateFarmerEngaged()
	{
		m_DirectionIndicator.SetActive(value: false);
		if (m_Engager == null || m_Engager.m_TypeIdentifier == ObjectType.Worker || m_State != 0 || (GameStateManager.Instance.GetActualState() != 0 && GameStateManager.Instance.GetActualState() != GameStateManager.State.TeachWorker))
		{
			return;
		}
		int moveDirectionFromDestination = GetMoveDirectionFromDestination(GameStateManager.Instance.GetCurrentState().GetComponent<GameStateBase>().m_CursorTilePosition);
		if (moveDirectionFromDestination != 0)
		{
			m_DirectionIndicator.SetActive(value: true);
			m_DirectionIndicator.GetComponent<MeshRenderer>().enabled = true;
			float y = 90f;
			float num = 6f;
			if (moveDirectionFromDestination == 1)
			{
				num = 0f - num;
				y = 270f;
			}
			m_DirectionIndicator.transform.localRotation = Quaternion.Euler(0f, y, 0f);
			m_DirectionIndicator.transform.localPosition = new Vector3(0f, 5f, num);
			TileManager.Instance.GetTile(m_TileCoord).m_Floor.GetComponent<TrainTrack>();
			Color color = new Color(1f, 1f, 1f, 1f);
			if (!CanMove())
			{
				color = new Color(1f, 0f, 0f, 1f);
			}
			m_DirectionIndicatorMaterial.color = color;
		}
	}

	private void UpdateHeight()
	{
		if (m_OnTrack)
		{
			TrainTrack component = TileManager.Instance.GetTile(m_TileCoord).m_Floor.GetComponent<TrainTrack>();
			Vector3 position = base.transform.position;
			position.y = component.GetHeight() + TrainTrack.m_Height;
			base.transform.position = position;
		}
	}

	private void UpdatePlayerEngaged()
	{
		bool flag = false;
		if ((bool)m_Engager && m_Engager.m_TypeIdentifier == ObjectType.FarmerPlayer && (GameStateManager.Instance.GetActualState() == GameStateManager.State.Normal || GameStateManager.Instance.GetActualState() == GameStateManager.State.TeachWorker))
		{
			flag = true;
		}
		if (flag != m_ShowPlayerControls)
		{
			GetLinkedSystem()?.ShowPlayerControls(flag);
			m_ShowPlayerControls = flag;
			m_DirectionIndicator.SetActive(flag);
		}
	}

	protected new void Update()
	{
		base.Update();
		UpdatePlayerEngaged();
		if (((bool)m_Engager || m_RequestStop) && (m_RequestStop || m_Engager.GetComponent<Farmer>().m_Energy > 0f))
		{
			State state = m_State;
			UpdateMoveCurve();
			UpdateMovePoints();
			UpdateMoveAnimation();
			if (state == State.Moving || m_State == State.Moving)
			{
				UpdateCarriagePosition();
			}
			UpdateFarmerEngaged();
			UpdateHeight();
		}
		if (m_TempMoving)
		{
			m_TempMoving = false;
			StartMoving();
		}
	}
}
