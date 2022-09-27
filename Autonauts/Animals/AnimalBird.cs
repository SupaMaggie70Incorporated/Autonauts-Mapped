using System;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class AnimalBird : Animal
{
	[HideInInspector]
	public enum State
	{
		None,
		Moving,
		Eat,
		RequestMove,
		FindingTarget,
		Carry,
		Flying,
		InTree,
		Total
	}

	private static float m_FlyUpSpeed = 7f;

	private static float m_FlyForwardSpeed = 15f;

	private static int m_VisibleRange = 15;

	private static int m_ThreatRange = 7;

	private static float m_MaxHeight = 15f;

	[HideInInspector]
	public State m_State;

	public float m_StateTimer;

	protected float m_EatDelay;

	protected List<ObjectType> m_FavouriteFoodTypes;

	private float m_OldEatPercent;

	private bool m_FlyToTarget;

	private BaseClass m_FlyTarget;

	private Vector3 m_FlyTargetPosition;

	private bool m_FlyOutOfWorld;

	private int m_TempTreeID;

	private MyTree m_CurrentTree;

	private GameObject m_Body;

	private GameObject m_LeftWing;

	private GameObject m_RightWing;

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("AnimalBird", m_TypeIdentifier);
		ModelManager.Instance.AddModel("Models/Animals/AnimalBird2", ObjectType.AnimalBird, RandomVariants: true);
		ModelManager.Instance.AddModel("Models/Animals/AnimalBird3", ObjectType.AnimalBird, RandomVariants: true);
	}

	public override void Restart()
	{
		base.Restart();
		m_MoveNormalDelay = 0.2f;
		m_EatDelay = 3f;
		m_OldEatPercent = 0f;
		m_Body = m_ModelRoot.transform.Find("Body").gameObject;
		m_LeftWing = m_Body.transform.Find("Wing1").gameObject;
		m_RightWing = m_Body.transform.Find("Wing2").gameObject;
		m_State = State.None;
		SetState(State.None);
		m_StateTimer = UnityEngine.Random.Range(-4f, 0f);
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_FavouriteFoodTypes = new List<ObjectType>();
		m_FavouriteFoodTypes.Add(ObjectType.Wheat);
		m_FavouriteFoodTypes.Add(ObjectType.WheatSeed);
		m_FavouriteFoodTypes.Add(ObjectType.CarrotSeed);
		m_FavouriteFoodTypes.Add(ObjectType.GrassCut);
		m_FavouriteFoodTypes.Add(ObjectType.Berries);
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		SetBaggedObject(null);
		base.StopUsing(AndDestroy);
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		State state = m_State;
		if (state == State.FindingTarget)
		{
			state = State.None;
		}
		JSONUtils.Set(Node, "ST", (int)state);
		JSONUtils.Set(Node, "STT", m_StateTimer);
		if (m_CurrentTree != null)
		{
			JSONUtils.Set(Node, "Tree", m_CurrentTree.m_UniqueID);
		}
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		SetState((State)JSONUtils.GetAsInt(Node, "ST", 0));
		m_StateTimer = JSONUtils.GetAsFloat(Node, "STT", 0f);
		m_TempTreeID = JSONUtils.GetAsInt(Node, "Tree", 0);
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		base.transform.localRotation = Quaternion.Euler(180f, 90f, 0f);
		SetBaggedObject(null);
		SetState(State.Carry);
	}

	protected override void ActionDropped(Actionable PreviousHolder, TileCoord DropLocation)
	{
		base.ActionDropped(PreviousHolder, DropLocation);
		base.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		SetState(State.None);
	}

	public override void SendAction(ActionInfo Info)
	{
		base.SendAction(Info);
		ActionType action = Info.m_Action;
		if ((uint)(action - 41) <= 1u && m_TempTreeID != 0)
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(m_TempTreeID, ErrorCheck: false);
			if ((bool)objectFromUniqueID)
			{
				m_CurrentTree = objectFromUniqueID.GetComponent<MyTree>();
			}
		}
	}

	public override bool CanDoAction(ActionInfo Info, bool RightNow)
	{
		ActionType action = Info.m_Action;
		if (action == ActionType.BeingHeld)
		{
			return false;
		}
		return base.CanDoAction(Info, RightNow);
	}

	public override bool RequestGoTo(TileCoord Destination, Actionable TargetObject = null, bool LessOne = false, int Range = 0)
	{
		if (m_State != 0 && m_State != State.Moving && m_State != State.RequestMove && m_State != State.FindingTarget)
		{
			return false;
		}
		SetState(State.RequestMove);
		return base.RequestGoTo(Destination, TargetObject, LessOne, Range);
	}

	public override bool StartGoTo(TileCoord Destination, Actionable TargetObject = null, bool LessOne = false, int Range = 0)
	{
		if (m_State != 0 && m_State != State.Moving && m_State != State.RequestMove && m_State != State.FindingTarget)
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

	public override void NextGoTo()
	{
		base.NextGoTo();
	}

	public override void EndGoTo()
	{
		base.EndGoTo();
		SetState(State.None);
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(m_BaggedObjectUID, ErrorCheck: false);
		if ((bool)objectFromUniqueID && m_FavouriteFoodTypes.Contains(objectFromUniqueID.m_TypeIdentifier))
		{
			SetState(State.Eat);
		}
	}

	private void RunFromThreat(TileCoordObject NearbyThreat)
	{
	}

	public TileCoordObject GetNearbyThreat(TileCoord NewTileCoord)
	{
		return null;
	}

	protected bool GoToFavouriteFood()
	{
		SetBaggedObject(null);
		List<TileCoordObject> objectsOfTypes = PlotManager.Instance.GetObjectsOfTypes(m_FavouriteFoodTypes, m_TileCoord, m_VisibleRange, this, ObjectTypeList.m_Total, AFO.AT.Primary, "");
		List<TileCoordObject> list = new List<TileCoordObject>();
		foreach (TileCoordObject item in objectsOfTypes)
		{
			if (!FindThreat(item.m_TileCoord))
			{
				list.Add(item);
			}
		}
		if (list.Count > 0)
		{
			RequestFind(list);
			SetState(State.FindingTarget);
			return true;
		}
		return false;
	}

	private void SetState(State NewState)
	{
		switch (m_State)
		{
		case State.Moving:
			m_ModelRoot.transform.localPosition = new Vector3(0f, 0f, 0f);
			break;
		case State.Eat:
			m_ModelRoot.transform.localRotation = Quaternion.identity;
			break;
		case State.Flying:
			m_LeftWing.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			m_RightWing.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			break;
		}
		m_State = NewState;
		m_StateTimer = 0f;
		State state = m_State;
		if (state != State.Moving)
		{
			_ = 2;
		}
	}

	private void UpdateStateNone()
	{
		if (m_StateTimer > 1f && !CheckFlyFromThreat() && !GoToFavouriteFood())
		{
			m_FlyToTarget = false;
			SetState(State.Flying);
		}
	}

	private void UpdateStateMove()
	{
		float y = 0f;
		if ((int)(m_StateTimer * 60f) % 6 < 3)
		{
			y = 0.4f;
		}
		m_ModelRoot.transform.localPosition = new Vector3(0f, y, 0f);
		UpdateMovement();
	}

	private TileCoordObject FindThreat(TileCoord Position)
	{
		List<BaseClass> players = CollectionManager.Instance.GetPlayers();
		if ((players[0].GetComponent<FarmerPlayer>().m_TileCoord - Position).Magnitude() < (float)m_ThreatRange)
		{
			return players[0].GetComponent<TileCoordObject>();
		}
		TileHelpers.GetClippedTileCoordArea(Position + new TileCoord(-m_ThreatRange, -m_ThreatRange), Position + new TileCoord(m_ThreatRange, m_ThreatRange), out var TopLeft, out var BottomRight);
		List<TileCoordObject> objectsOfType = PlotManager.Instance.GetObjectsOfType(ObjectType.Scarecrow, TopLeft, BottomRight, this, ObjectTypeList.m_Total, AFO.AT.Primary, "IsScarecrow", IgnorePlotVisibility: false, CheckBagged: false);
		if (objectsOfType.Count > 0)
		{
			return objectsOfType[0];
		}
		return null;
	}

	private bool CheckFlyFromThreat()
	{
		TileCoordObject tileCoordObject = FindThreat(m_TileCoord);
		if (tileCoordObject != null)
		{
			m_Body.transform.localRotation = Quaternion.Euler(-90f, -180f, 0f);
			SetBaggedObject(null);
			if (base.transform.position != tileCoordObject.transform.position)
			{
				Vector3 vector = base.transform.position - tileCoordObject.transform.position;
				vector.Normalize();
				Quaternion rotation = default(Quaternion);
				rotation.SetLookRotation(-vector, new Vector3(0f, 1f, 0f));
				base.transform.rotation = rotation;
			}
			AudioManager.Instance.StartEvent("AnimalBirdScared", this);
			m_FlyToTarget = false;
			SetState(State.Flying);
			return true;
		}
		return false;
	}

	private void UpdateStateEat()
	{
		if (CheckFlyFromThreat())
		{
			return;
		}
		float num = 0f;
		if ((int)(m_StateTimer * 60f) % 8 < 4)
		{
			num = 1f;
		}
		if (num == 1f && m_OldEatPercent == 0f)
		{
			AudioManager.Instance.StartEvent("AnimalBirdEating", this, Remember: true);
		}
		m_OldEatPercent = num;
		m_Body.transform.localRotation = Quaternion.Euler(-90f + 60f * num, -180f, 0f);
		if (!(m_StateTimer > m_EatDelay))
		{
			return;
		}
		m_Body.transform.localRotation = Quaternion.Euler(-90f, -180f, 0f);
		SetBaggedObject(null);
		TileCoordObject objectTypesAtTile = m_Plot.GetObjectTypesAtTile(m_FavouriteFoodTypes, m_TileCoord);
		if ((bool)objectTypesAtTile)
		{
			if (m_FavouriteFoodTypes.Contains(objectTypesAtTile.m_TypeIdentifier))
			{
				QuestManager.Instance.AddEvent(QuestEvent.Type.BirdEatCrops, Bot: false, 0, this);
			}
			objectTypesAtTile.StopUsing();
		}
		SetState(State.None);
	}

	private void UpdateStateFindingTarget()
	{
		if (m_FindFinished)
		{
			if (m_FoundObjects != null && m_FoundObjects.Count > 0 && !BaggedManager.Instance.IsObjectBagged(m_FoundObjects[0]))
			{
				TileCoordObject tileCoordObject = m_FoundObjects[0];
				SetBaggedObject(tileCoordObject);
				RequestGoTo(tileCoordObject.m_TileCoord);
			}
			else
			{
				m_FlyToTarget = false;
				SetState(State.Flying);
			}
		}
	}

	private void LookForTree()
	{
		List<TileCoordObject> objectsOfTypes = PlotManager.Instance.GetObjectsOfTypes(MyTree.m_TreeTypes, m_TileCoord, m_VisibleRange, this, ObjectTypeList.m_Total, AFO.AT.Primary, "", IgnorePlotVisibility: true);
		if (objectsOfTypes.Count <= 0)
		{
			return;
		}
		if (objectsOfTypes.Count > 1 && m_CurrentTree != null)
		{
			TileCoordObject component = m_CurrentTree.GetComponent<TileCoordObject>();
			foreach (TileCoordObject item in objectsOfTypes)
			{
				if (item == component)
				{
					objectsOfTypes.Remove(item);
					m_CurrentTree = null;
					break;
				}
			}
		}
		TileCoord target = new TileCoord(base.transform.position);
		m_FlyTarget = ObjectUtils.FindNearestObject(objectsOfTypes, target);
		float num = (float)UnityEngine.Random.Range(0, 3) + 0.5f;
		float num2 = num * 0.5f;
		float f = UnityEngine.Random.Range(0, 360);
		Vector3 position = default(Vector3);
		position.x = Mathf.Cos(f) * num2;
		position.z = Mathf.Sin(f) * num2;
		position.y = 7f - num;
		m_FlyTargetPosition = m_FlyTarget.m_ModelRoot.transform.TransformPoint(position);
		m_FlyToTarget = true;
	}

	private void AnimateWings()
	{
		if ((int)(m_StateTimer * 60f) % 12 < 6)
		{
			m_LeftWing.transform.localRotation = Quaternion.Euler(90f, 0f, 20f);
			m_RightWing.transform.localRotation = Quaternion.Euler(90f, 0f, -20f);
		}
		else
		{
			m_LeftWing.transform.localRotation = Quaternion.Euler(90f, 0f, 140f);
			m_RightWing.transform.localRotation = Quaternion.Euler(90f, 0f, -140f);
		}
	}

	public void FlyOutOfWorld()
	{
		SetState(State.Flying);
		m_FlyToTarget = false;
		m_FlyOutOfWorld = true;
		SetIsSavable(IsSavable: false);
	}

	private void UpdateStateFlying()
	{
		Vector3 position = base.transform.position;
		if (m_FlyToTarget)
		{
			Vector3 vector = m_FlyTargetPosition - base.transform.position;
			float magnitude = vector.magnitude;
			if (magnitude <= m_FlyForwardSpeed * 1.5f * TimeManager.Instance.m_NormalDelta)
			{
				SetPosition(m_FlyTargetPosition);
				if (m_FlyTarget != null)
				{
					base.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0, 360), 0f);
					m_CurrentTree = m_FlyTarget.GetComponent<MyTree>();
					SetState(State.InTree);
				}
				else
				{
					SetState(State.None);
				}
				return;
			}
			vector.Normalize();
			Quaternion quaternion = default(Quaternion);
			quaternion.SetLookRotation(-vector, new Vector3(0f, 1f, 0f));
			if (magnitude < 1f)
			{
				base.transform.rotation = quaternion;
			}
			else
			{
				float num = 1.5f;
				if (magnitude < 6f)
				{
					num = (6f - magnitude) / 6f * 12f * num + num;
				}
				Quaternion rotation = Quaternion.Slerp(base.transform.rotation, quaternion, TimeManager.Instance.m_NormalDelta * num);
				base.transform.rotation = rotation;
			}
			if (m_FlyTarget != null)
			{
				if (position.y < m_FlyTargetPosition.y)
				{
					position.y += m_FlyUpSpeed * TimeManager.Instance.m_NormalDelta;
				}
				else if (position.y > m_FlyTargetPosition.y)
				{
					position.y -= m_FlyUpSpeed * TimeManager.Instance.m_NormalDelta;
				}
			}
			else if (magnitude < 9f && position.y > 0f)
			{
				position.y -= m_FlyUpSpeed * TimeManager.Instance.m_NormalDelta;
				if (position.y < 0f)
				{
					position.y = 0f;
				}
			}
		}
		else if (position.y < m_MaxHeight)
		{
			position.y += m_FlyUpSpeed * TimeManager.Instance.m_NormalDelta;
		}
		Vector3 eulerAngles = base.transform.rotation.eulerAngles;
		eulerAngles.x = 0f;
		eulerAngles.z = 0f;
		base.transform.rotation = Quaternion.Euler(eulerAngles);
		float f = (0f - base.transform.rotation.eulerAngles.y - 90f) * ((float)Math.PI / 180f);
		Vector3 vector2 = default(Vector3);
		vector2.x = Mathf.Cos(f) * m_FlyForwardSpeed * TimeManager.Instance.m_NormalDelta;
		vector2.z = Mathf.Sin(f) * m_FlyForwardSpeed * TimeManager.Instance.m_NormalDelta;
		vector2.y = 0f;
		position += vector2;
		base.transform.position = position;
		if (!m_FlyToTarget && !m_FlyOutOfWorld && m_StateTimer > 0.5f)
		{
			m_StateTimer = 0f;
			LookForTree();
		}
		AnimateWings();
		TileCoord tileCoord = new TileCoord(position);
		if (tileCoord.x < 0 || tileCoord.y < 0 || tileCoord.x >= TileManager.Instance.m_TilesWide || tileCoord.y >= TileManager.Instance.m_TilesHigh)
		{
			StopUsing();
		}
		else
		{
			SetPosition(position);
		}
	}

	private void UpdateStateInTree()
	{
		if ((bool)m_CurrentTree && m_CurrentTree.m_WobbleTimer > 0f)
		{
			m_FlyToTarget = false;
			SetState(State.Flying);
		}
		if (!(m_StateTimer > 2f))
		{
			return;
		}
		m_StateTimer = 0f;
		List<TileCoordObject> objectsOfTypes = PlotManager.Instance.GetObjectsOfTypes(m_FavouriteFoodTypes, m_TileCoord, m_VisibleRange, this, ObjectTypeList.m_Total, AFO.AT.Primary, "");
		List<TileCoordObject> list = new List<TileCoordObject>();
		foreach (TileCoordObject item in objectsOfTypes)
		{
			if (!FindThreat(item.m_TileCoord))
			{
				list.Add(item);
			}
		}
		if (list.Count > 0)
		{
			m_FlyToTarget = true;
			m_FlyTarget = null;
			m_FlyTargetPosition = list[0].m_TileCoord.ToWorldPositionTileCentered();
			m_CurrentTree = null;
			SetState(State.Flying);
		}
	}

	private void Update()
	{
		switch (m_State)
		{
		case State.None:
			UpdateStateNone();
			break;
		case State.Moving:
			UpdateStateMove();
			break;
		case State.Eat:
			UpdateStateEat();
			break;
		case State.FindingTarget:
			UpdateStateFindingTarget();
			break;
		case State.Flying:
			UpdateStateFlying();
			break;
		case State.InTree:
			UpdateStateInTree();
			break;
		}
		m_StateTimer += TimeManager.Instance.m_NormalDelta;
	}
}
