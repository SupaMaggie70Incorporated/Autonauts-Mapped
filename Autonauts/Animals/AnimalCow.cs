using System;
using UnityEngine;

public class AnimalCow : AnimalGrazer
{
	private GameObject m_Body;

	private Vector3 m_OriginalBodyScale;

	private GameObject[] m_LegsRoot;

	private Vector3[] m_LegsPosition;

	private GameObject[] m_Legs;

	private Vector3 m_OriginalLegScale;

	private Wobbler m_Wobbler;

	private bool m_RunningAway;

	private PlaySound m_PlaySound;

	public static bool GetIsTypeCow(ObjectType NewType)
	{
		if (NewType == ObjectType.AnimalCow || NewType == ObjectType.AnimalCowHighland)
		{
			return true;
		}
		return false;
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_Body = m_ModelRoot.transform.Find("Body").gameObject;
		m_Head = m_ModelRoot.transform.Find("HeadRoot").gameObject;
		m_Eye = m_Head.transform.Find("Head").Find("Eye.001").gameObject;
		m_HeadPosition = m_Head.transform.localPosition;
		m_OriginalBodyScale = m_Body.transform.localScale;
		m_LegsRoot = new GameObject[4];
		m_LegsPosition = new Vector3[4];
		m_Legs = new GameObject[4];
		for (int i = 0; i < 4; i++)
		{
			m_LegsRoot[i] = m_ModelRoot.transform.Find("Leg" + (i + 1)).gameObject;
			m_LegsPosition[i] = m_LegsRoot[i].transform.localPosition;
			m_Legs[i] = m_LegsRoot[i].transform.Find("Leg" + (i + 1) + "Cube").gameObject;
		}
		m_OriginalLegScale = m_LegsRoot[0].transform.localScale;
		m_EatHeadMoveSize = 1f;
	}

	public override void Restart()
	{
		base.Restart();
		m_MoveNormalDelay = 0.2f;
		m_RunningAway = false;
		m_Wobbler.Restart();
		UpdateFatScale(0f);
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		UpdateFatScale(1f);
		base.StopUsing(AndDestroy);
	}

	protected new void Awake()
	{
		base.Awake();
		m_Wobbler = new Wobbler();
	}

	public override void NextGoTo()
	{
		if (!m_RunningAway)
		{
			TileCoordObject nearbyThreat = GetNearbyThreat(m_TileCoord);
			if ((bool)nearbyThreat)
			{
				RunFromThreat(nearbyThreat);
				return;
			}
		}
		base.NextGoTo();
	}

	public override void EndGoTo()
	{
		base.EndGoTo();
		if (m_RunningAway)
		{
			m_RunningAway = false;
			m_StateTimer = -1.5f;
		}
	}

	private float GetFatPercent()
	{
		float num = m_FatCount / m_MaxFatCount;
		num = (float)(int)(num * 4f) / 4f;
		if (num > 1f)
		{
			num = 1f;
		}
		return num;
	}

	protected override void UpdateFatCount()
	{
		float fatPercent = GetFatPercent();
		if (m_OldFatPercent != fatPercent)
		{
			m_OldFatPercent = fatPercent;
			m_Wobbler.Go(0.5f, 5f, 0.2f);
			AudioManager.Instance.StartEvent("AnimalCowGrowing", this);
		}
	}

	public void Milk()
	{
		m_FatCount = 0f;
		UpdateFatCount();
		SetState(State.None);
	}

	protected override void UpdateFatScale(float Percent)
	{
		float num = 0.5f + 0.5f * Percent;
		m_Body.transform.localScale = new Vector3(m_OriginalBodyScale.x * num, m_OriginalBodyScale.y, m_OriginalBodyScale.z);
		num = 0.5f + 0.5f * Percent;
		for (int i = 0; i < 4; i++)
		{
			m_LegsRoot[i].transform.localScale = new Vector3(m_OriginalLegScale.x * num, m_OriginalLegScale.y * num, m_OriginalLegScale.z);
		}
	}

	private void UpdateFatWobble()
	{
		if (m_Wobbler.m_Wobbling)
		{
			m_Wobbler.Update();
			float fatPercent = GetFatPercent();
			UpdateFatScale(m_Wobbler.m_Height + fatPercent);
		}
	}

	protected override void EndCarry()
	{
		base.EndCarry();
		for (int i = 0; i < 4; i++)
		{
			m_LegsRoot[i].transform.localPosition = m_LegsPosition[i];
		}
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		base.transform.localPosition = new Vector3(0f, 2.5f, 0f);
	}

	private void StartBucket(AFO Info)
	{
		m_Busy = true;
	}

	private void EndBucket(AFO Info)
	{
		m_Busy = false;
		Milk();
	}

	private ActionType GetActionFromBucket(AFO Info)
	{
		Info.m_FarmerState = Farmer.State.Milking;
		Info.m_StartAction = StartBucket;
		Info.m_EndAction = EndBucket;
		if (!GetIsFull() || (m_State != State.Full && m_State != State.FindingTarget && m_State != 0))
		{
			return ActionType.Fail;
		}
		if ((bool)Info.m_Object && (!ToolFillable.GetIsTypeFillable(Info.m_Object.m_TypeIdentifier) || Info.m_Object.GetComponent<ToolFillable>().GetIsFull()))
		{
			return ActionType.Fail;
		}
		if (Info.m_RequirementsIn != "" && Info.m_RequirementsIn != Info.m_RequirementsOut)
		{
			return ActionType.Fail;
		}
		return ActionType.UseInHands;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		string requirementsOut = "FNOCowIdle";
		if (GetIsFull())
		{
			requirementsOut = "FNOCowFull";
		}
		Info.m_RequirementsOut = requirementsOut;
		if (Info.m_ActionType == AFO.AT.Primary)
		{
			if (ToolBucket.GetIsTypeBucket(Info.m_ObjectType))
			{
				return GetActionFromBucket(Info);
			}
			if (Info.m_ObjectType == ObjectTypeList.m_Total)
			{
				Info.m_FarmerState = Farmer.State.PickingUp;
				if (Info.m_RequirementsIn == "" || Info.m_RequirementsIn == Info.m_RequirementsOut)
				{
					return ActionType.Pickup;
				}
				if (Info.m_RequirementsIn != "")
				{
					return ActionType.Fail;
				}
			}
		}
		Info.m_RequirementsOut = "";
		return base.GetActionFromObject(Info);
	}

	public override void SetState(State NewState)
	{
		switch (m_State)
		{
		case State.Eat:
			AudioManager.Instance.StopEvent(m_PlaySound);
			break;
		case State.Moving:
			m_ModelRoot.transform.localPosition = new Vector3(0f, 0f, 0f);
			break;
		case State.Carry:
			EndCarry();
			break;
		case State.Sleep:
			m_ModelRoot.transform.localPosition = new Vector3(0f, 0f, 0f);
			m_Eye.SetActive(value: true);
			break;
		}
		base.SetState(NewState);
		switch (m_State)
		{
		case State.Eat:
			m_PlaySound = AudioManager.Instance.StartEvent("AnimalCowEating", this, Remember: true);
			break;
		case State.Poop:
			AudioManager.Instance.StartEvent("AnimalCowExcreting", this);
			break;
		case State.Sleep:
			m_ModelRoot.transform.localPosition = new Vector3(0f, -0.8f * m_Scale, 0f);
			m_Eye.SetActive(value: false);
			break;
		}
	}

	protected override void TestOccupiedTile()
	{
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(m_BaggedObjectUID, ErrorCheck: false);
		if ((bool)objectFromUniqueID && m_FavouriteFoodTypes.Contains(objectFromUniqueID.m_TypeIdentifier))
		{
			SetState(State.Eat);
			FaceTarget();
			return;
		}
		Tile tile = TileManager.Instance.GetTile(m_TileCoord);
		if ((bool)tile.m_AssociatedObject && (tile.m_AssociatedObject.m_TypeIdentifier == ObjectType.Grass || tile.m_AssociatedObject.m_TypeIdentifier == ObjectType.CropWheat))
		{
			SetState(State.Eat);
		}
	}

	private void RunFromThreat(TileCoordObject NearbyThreat)
	{
		AudioManager.Instance.StartEvent("AnimalCowScared", this);
		TileCoord tileCoord = m_TileCoord - NearbyThreat.GetComponent<TileCoordObject>().m_TileCoord;
		if (tileCoord.Magnitude() == 0f)
		{
			tileCoord.x -= m_ThreatRange;
		}
		Vector2 vector = new Vector2(tileCoord.x, tileCoord.y);
		float num = (float)m_ThreatRange - vector.magnitude + 2f;
		vector.Normalize();
		vector *= num;
		tileCoord.x = (int)vector.x;
		tileCoord.y = (int)vector.y;
		m_RunningAway = true;
		TileCoord destination = m_TileCoord + tileCoord;
		m_MoveNormalDelay = 0.2f;
		RequestGoTo(destination);
	}

	protected override void UpdateStateNone()
	{
		if (!(m_StateTimer > 0.5f) || m_Wobbler.m_Wobbling)
		{
			return;
		}
		if (m_EatCount >= m_MaxEatCount)
		{
			m_EatCount = 0f;
			Vector3 position = m_TileCoord.ToWorldPositionTileCentered();
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Manure, position, Quaternion.identity);
			SetState(State.Poop);
			if (!SaveLoadManager.Instance.m_Loading)
			{
				ModManager.Instance.CheckItemTypeSpawnedCallback(baseClass.m_UniqueID, baseClass.m_TypeIdentifier, baseClass.GetComponent<TileCoordObject>().m_TileCoord);
			}
			return;
		}
		if (m_FatCount >= m_MaxFatCount)
		{
			SetState(State.Full);
			return;
		}
		TileCoordObject nearbyThreat = GetNearbyThreat(m_TileCoord);
		if ((bool)nearbyThreat)
		{
			RunFromThreat(nearbyThreat);
			return;
		}
		if (m_RequestWaitCarry)
		{
			SetState(State.WaitCarry);
			return;
		}
		if (DayNightManager.Instance.GetIsNightTime())
		{
			SetState(State.Sleep);
			return;
		}
		m_MoveNormalDelay = 0.4f;
		GoToFavouriteFood();
	}

	protected override void UpdateStateMove()
	{
		base.UpdateStateMove();
		float y = 0f;
		if ((int)(m_StateTimer * 60f) % 8 < 5)
		{
			y = 0.5f;
		}
		m_ModelRoot.transform.localPosition = new Vector3(0f, y, 0f);
		UpdateMovement();
	}

	protected override void UpdateStatePoop()
	{
		float num = 0.35f;
		if (m_StateTimer < num)
		{
			float num2 = m_StateTimer / num;
			m_ModelRoot.transform.localPosition = new Vector3(0f, Mathf.Sin(num2 * (float)Math.PI) * 8f, 0f);
			m_ModelRoot.transform.localRotation = Quaternion.Euler(num2 * 360f, 0f, 0f);
		}
		else
		{
			m_ModelRoot.transform.localPosition = new Vector3(0f, 0f, 0f);
			m_ModelRoot.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			SetState(State.None);
		}
	}

	protected override void UpdateStateCarry()
	{
		if ((bool)m_Carrier && (bool)m_Carrier.GetComponent<Farmer>())
		{
			m_CarryLegsTimer += TimeManager.Instance.m_NormalDelta;
			if ((int)(m_CarryLegsTimer * 60f) % 14 < 7)
			{
				m_LegsRoot[0].transform.localPosition = m_LegsPosition[0] + new Vector3(0f, 0.25f, 0f);
				m_LegsRoot[1].transform.localPosition = m_LegsPosition[1] + new Vector3(0f, -0.25f, 0f);
				m_LegsRoot[2].transform.localPosition = m_LegsPosition[2] + new Vector3(0f, -0.25f, 0f);
				m_LegsRoot[3].transform.localPosition = m_LegsPosition[3] + new Vector3(0f, 0.25f, 0f);
			}
			else
			{
				m_LegsRoot[0].transform.localPosition = m_LegsPosition[0] + new Vector3(0f, -0.25f, 0f);
				m_LegsRoot[1].transform.localPosition = m_LegsPosition[1] + new Vector3(0f, 0.25f, 0f);
				m_LegsRoot[2].transform.localPosition = m_LegsPosition[2] + new Vector3(0f, 0.25f, 0f);
				m_LegsRoot[3].transform.localPosition = m_LegsPosition[3] + new Vector3(0f, -0.25f, 0f);
			}
		}
	}

	protected override void UpdateStateFull()
	{
		base.UpdateStateFull();
		if (GetIsFull())
		{
			GoToBuilding(ObjectType.MilkingShedCrude, m_EatRange);
		}
	}

	protected override void UpdateStateSleep()
	{
		float y = 0f;
		if ((int)(m_StateTimer * 60f) % 60 < 30)
		{
			y = -0.5f;
		}
		m_Head.transform.localPosition = m_HeadPosition + new Vector3(0f, y, 0f);
		if (!DayNightManager.Instance.GetIsNightTime())
		{
			SetState(State.None);
		}
	}

	protected override void Update()
	{
		if (m_Plot == null || m_Plot.m_Visible)
		{
			base.Update();
			UpdateFatWobble();
		}
	}
}
