using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class AnimalChicken : AnimalGrazer
{
	private static int m_CoopRange = 10;

	private GameObject m_Body;

	private Vector3 m_OriginalBodyScale;

	private GameObject[] m_LegsRoot;

	private Vector3[] m_LegsPosition;

	private GameObject m_LeftWing;

	private GameObject m_RightWing;

	private Wobbler m_Wobbler;

	private bool m_RunningAway;

	private PlaySound m_PlaySound;

	public ChickenCoop m_ChickenCoop;

	private int m_TargetChickenCoopID;

	private MyParticles m_FeatherParticles;

	private float m_OldStateTimer;

	private bool m_FindCoop;

	private float m_OldEatPercent;

	private int m_TempChickenCoop;

	public override void PostCreate()
	{
		base.PostCreate();
		m_EatRange = 5;
		m_FavouriteFoodTypes = new List<ObjectType>();
		m_FavouriteFoodTypes.Add(ObjectType.Wheat);
		m_FavouriteFoodTypes.Add(ObjectType.WheatSeed);
		m_FavouriteFoodTypes.Add(ObjectType.CarrotSeed);
		m_FavouriteFoodTypes.Add(ObjectType.GrassCut);
		m_FindCoop = false;
		m_OldEatPercent = 0f;
		m_Body = m_ModelRoot.transform.Find("Body").gameObject;
		m_Head = m_Body.transform.Find("HeadRoot").gameObject;
		m_LeftWing = m_Body.transform.Find("Wing1").gameObject;
		m_RightWing = m_Body.transform.Find("Wing2").gameObject;
		m_OriginalBodyScale = m_Body.transform.localScale;
		m_LegsRoot = new GameObject[2];
		m_LegsPosition = new Vector3[2];
		for (int i = 0; i < 2; i++)
		{
			m_LegsRoot[i] = m_ModelRoot.transform.Find("Leg" + (i + 1)).gameObject;
			m_LegsPosition[i] = m_LegsRoot[i].transform.localPosition;
		}
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
		if ((bool)m_FeatherParticles)
		{
			ParticlesManager.Instance.DestroyParticles(m_FeatherParticles);
		}
		UpdateFatScale(1f);
		base.StopUsing(AndDestroy);
	}

	protected new void Awake()
	{
		base.Awake();
		m_Wobbler = new Wobbler();
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		if (m_ChickenCoop != null)
		{
			JSONUtils.Set(Node, "Coop", m_ChickenCoop.m_UniqueID);
		}
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		m_TempChickenCoop = JSONUtils.GetAsInt(Node, "Coop", 0);
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
		if (!(m_FatCount >= m_MaxFatCount) && !DayNightManager.Instance.GetIsNightTime())
		{
			return;
		}
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(m_TargetChickenCoopID, ErrorCheck: false);
		if (!objectFromUniqueID || objectFromUniqueID.GetComponent<ChickenCoop>().AnyChickenIngredients())
		{
			return;
		}
		TileCoord accessPosition = objectFromUniqueID.GetComponent<ChickenCoop>().GetAccessPosition();
		if (m_TileCoord == accessPosition)
		{
			m_ChickenCoop = objectFromUniqueID.GetComponent<ChickenCoop>();
			TileCoord tileCoord = m_TileCoord;
			if (m_ChickenCoop.m_Rotation == 0)
			{
				tileCoord.y--;
			}
			else if (m_ChickenCoop.m_Rotation == 1)
			{
				tileCoord.x++;
			}
			else if (m_ChickenCoop.m_Rotation == 2)
			{
				tileCoord.y++;
			}
			else
			{
				tileCoord.x--;
			}
			SpawnAnimationManager.Instance.AddJump(this, m_TileCoord, tileCoord, 0f, 2f, 3f, 0.4f);
			m_ChickenCoop.AddChicken(this);
			SetState(State.EnterBuilding);
			m_ModelRoot.SetActive(value: true);
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
			AudioManager.Instance.StartEvent("AnimalChickenGrowing", this);
		}
	}

	protected override void UpdateFatScale(float Percent)
	{
		float num = 0.5f + 0.5f * Percent;
		m_Body.transform.localScale = new Vector3(m_OriginalBodyScale.x * num, m_OriginalBodyScale.y, m_OriginalBodyScale.z);
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
		for (int i = 0; i < m_LegsRoot.Length; i++)
		{
			m_LegsRoot[i].transform.localPosition = m_LegsPosition[i];
		}
	}

	public void StartMilking()
	{
		m_Busy = true;
	}

	public void FinishMilking()
	{
		m_Busy = false;
		m_FatCount = 0f;
		UpdateFatCount();
		SetState(State.None);
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		if (m_ChickenCoop == null)
		{
			base.ActionBeingHeld(Holder);
		}
		else
		{
			SetIsSavable(IsSavable: false);
		}
		if ((bool)Holder && (bool)Holder.GetComponent<Farmer>())
		{
			base.transform.localPosition = new Vector3(0f, -0.5f, 0f);
			base.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		}
		else
		{
			m_ChickenCoop = Holder.GetComponent<ChickenCoop>();
			m_ModelRoot.SetActive(value: false);
			m_Indicator.SetAllOff();
		}
	}

	protected override void ActionDropped(Actionable PreviousHolder, TileCoord DropLocation)
	{
		m_ModelRoot.SetActive(value: true);
		m_ChickenCoop = null;
		base.ActionDropped(PreviousHolder, DropLocation);
		SetIsSavable(IsSavable: true);
		m_LeftWing.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		m_RightWing.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
	}

	public override void SendAction(ActionInfo Info)
	{
		ActionType action = Info.m_Action;
		if (action != 0 && action != ActionType.Show && (uint)(action - 41) <= 1u && m_TempChickenCoop != 0)
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(m_TempChickenCoop, ErrorCheck: false);
			if (objectFromUniqueID == null)
			{
				Debug.Log("Chicken.Refresh : Couldn't find object with UID " + m_TempChickenCoop);
			}
			else
			{
				m_ChickenCoop = objectFromUniqueID.GetComponent<ChickenCoop>();
			}
			m_TempChickenCoop = 0;
		}
		base.SendAction(Info);
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (Info.m_ActionType == AFO.AT.Primary && (m_State == State.EnterBuilding || m_State == State.ExitBuilding))
		{
			return ActionType.Total;
		}
		return base.GetActionFromObject(Info);
	}

	public override void SetState(State NewState)
	{
		switch (m_State)
		{
		case State.Moving:
			m_ModelRoot.transform.localPosition = new Vector3(0f, 0f, 0f);
			AudioManager.Instance.StopEvent(m_PlaySound);
			break;
		case State.Eat:
			m_Body.transform.localRotation = Quaternion.Euler(-90f, 0f, 180f);
			m_Head.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			break;
		case State.Carry:
			AudioManager.Instance.StopEvent(m_PlaySound);
			if ((bool)m_FeatherParticles)
			{
				ParticlesManager.Instance.DestroyParticles(m_FeatherParticles, WaitUntilNoParticles: true, StopSpawning: true);
			}
			EndCarry();
			break;
		case State.Sleep:
			m_ModelRoot.transform.localPosition = new Vector3(0f, 0f, 0f);
			m_Indicator.SetSleeping(Sleeping: false);
			break;
		case State.DoBusiness:
			AudioManager.Instance.StopEvent(m_PlaySound);
			break;
		case State.WaitToExitBuilding:
			if ((bool)m_ChickenCoop)
			{
				m_ModelRoot.SetActive(value: true);
			}
			break;
		case State.ExitBuilding:
			if ((bool)m_FeatherParticles)
			{
				ParticlesManager.Instance.DestroyParticles(m_FeatherParticles, WaitUntilNoParticles: true, StopSpawning: true);
			}
			break;
		}
		base.SetState(NewState);
		switch (m_State)
		{
		case State.Moving:
			m_PlaySound = AudioManager.Instance.StartEvent("AnimalChickenMoving", this, Remember: true, Follow: true);
			break;
		case State.Sleep:
			if ((bool)m_ChickenCoop)
			{
				m_ModelRoot.SetActive(value: false);
			}
			else
			{
				m_ModelRoot.transform.localPosition = new Vector3(0f, -0.8f * m_Scale, 0f);
			}
			break;
		case State.Carry:
			m_FeatherParticles = ParticlesManager.Instance.CreateParticles("ChickenFeathers", default(Vector3), Quaternion.Euler(90f, 0f, 0f));
			if (m_PlaySound != null)
			{
				AudioManager.Instance.StopEvent(m_PlaySound);
			}
			m_PlaySound = null;
			break;
		case State.EnterBuilding:
			base.enabled = true;
			base.transform.rotation = Quaternion.Euler(0f, m_ChickenCoop.GetAccessRotationInDegrees() - 90f, 0f);
			break;
		case State.DoBusiness:
			m_PlaySound = AudioManager.Instance.StartEvent("AnimalChickenLaying", this);
			if (m_ChickenCoop != null)
			{
				m_ModelRoot.SetActive(value: false);
			}
			break;
		case State.WaitToExitBuilding:
			m_StateTimer = 0f - Random.Range(0f, 2f);
			break;
		case State.ExitBuilding:
		{
			m_PlaySound = AudioManager.Instance.StartEvent("AnimalChickenSurprise", this);
			m_FeatherParticles = ParticlesManager.Instance.CreateParticles("ChickenFeathers", default(Vector3), Quaternion.Euler(90f, 0f, 0f));
			m_FeatherParticles.m_Particles.Stop();
			ParticleSystem.MainModule main = m_FeatherParticles.m_Particles.main;
			main.duration = 0.125f;
			m_FeatherParticles.m_Particles.Play();
			if (m_ChickenCoop != null)
			{
				TileCoord startPosition = new TileCoord(base.transform.position);
				SpawnAnimationManager.Instance.AddJump(this, startPosition, m_ChickenCoop.GetAccessPosition(), 2f, 0f, 3f, 0.75f);
				base.enabled = true;
				base.transform.rotation = Quaternion.Euler(0f, m_ChickenCoop.GetAccessRotationInDegrees() + 90f, 0f);
				m_ChickenCoop.RemoveChicken(this);
				m_ChickenCoop = null;
			}
			break;
		}
		}
		m_OldStateTimer = 0f;
	}

	protected override void TestOccupiedTile()
	{
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(m_BaggedObjectUID, ErrorCheck: false);
		if ((bool)objectFromUniqueID && m_FavouriteFoodTypes.Contains(objectFromUniqueID.m_TypeIdentifier))
		{
			SetState(State.Eat);
			FaceTarget();
		}
		else if (TileManager.Instance.GetTile(m_TileCoord).m_TileType == Tile.TileType.Soil)
		{
			SetState(State.Eat);
		}
	}

	private void RunFromThreat(TileCoordObject NearbyThreat)
	{
		AudioManager.Instance.StartEvent("AnimalChickenScared", this);
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

	protected override void GoToSecondFood()
	{
		List<TileCoord> randomTilesOfTypeInRange = TileHelpers.GetRandomTilesOfTypeInRange(Tile.TileType.Soil, m_TileCoord, m_EatRange, this, ObjectTypeList.m_Total, AFO.AT.Primary, "", 5);
		m_FindingFavouriteFood = false;
		RequestFind(randomTilesOfTypeInRange);
		SetState(State.FindingTarget);
	}

	private bool GoToCoop()
	{
		TileHelpers.GetClippedTileCoordArea(m_TileCoord + new TileCoord(-m_CoopRange, -m_CoopRange), m_TileCoord + new TileCoord(m_CoopRange, m_CoopRange), out var TopLeft, out var BottomRight);
		List<TileCoordObject> objectsOfType = PlotManager.Instance.GetObjectsOfType(ObjectType.ChickenCoop, TopLeft, BottomRight, this, ObjectTypeList.m_Total, AFO.AT.Secondary, "");
		List<TileCoordObject> list = new List<TileCoordObject>();
		foreach (TileCoordObject item in objectsOfType)
		{
			if (!item.GetComponent<ChickenCoop>().AnyChickenIngredients())
			{
				list.Add(item);
			}
		}
		if (list.Count > 0)
		{
			m_FindCoop = true;
			RequestFind(list);
			SetState(State.FindingTarget);
			return true;
		}
		return false;
	}

	private void UpdateStateNoneNext()
	{
		TileCoordObject nearbyThreat = GetNearbyThreat(m_TileCoord);
		if ((bool)nearbyThreat)
		{
			RunFromThreat(nearbyThreat);
		}
		else if (m_RequestWaitCarry)
		{
			SetState(State.WaitCarry);
		}
		else if (DayNightManager.Instance.GetIsNightTime())
		{
			if (!GoToCoop())
			{
				SetState(State.Sleep);
			}
		}
		else
		{
			m_MoveNormalDelay = 0.4f;
			GoToFavouriteFood();
		}
	}

	protected override void UpdateStateNone()
	{
		if (!(m_StateTimer > 0.5f) || m_Wobbler.m_Wobbling)
		{
			return;
		}
		if (m_FatCount >= m_MaxFatCount)
		{
			if (GoToCoop())
			{
				return;
			}
			if (Random.Range(0, 100) == 0)
			{
				SetState(State.DoBusiness);
				return;
			}
		}
		UpdateStateNoneNext();
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

	protected override void UpdateStateEat()
	{
		float num = 0f;
		if ((int)(m_StateTimer * 60f) % 12 < 6)
		{
			num = 1f;
		}
		if (num == 1f && m_OldEatPercent == 0f)
		{
			AudioManager.Instance.StartEvent("AnimalChickenEating", this, Remember: true);
		}
		m_OldEatPercent = num;
		m_Body.transform.localRotation = Quaternion.Euler(-90f - 45f * num, 0f, 180f);
		m_Head.transform.localRotation = Quaternion.Euler(65f * num, 0f, 0f);
		if (!(m_StateTimer > m_EatDelay) || !m_Plot)
		{
			return;
		}
		m_Body.transform.localRotation = Quaternion.Euler(-90f, 0f, 180f);
		m_Head.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		SetBaggedObject(null);
		SetBaggedTile(default(TileCoord));
		bool flag = false;
		float num2 = 0f;
		TileCoordObject objectTypesAtTile = m_Plot.GetObjectTypesAtTile(m_FavouriteFoodTypes, m_TileCoord);
		if ((bool)objectTypesAtTile && !SpawnAnimationManager.Instance.GetIsObjectSpawning(objectTypesAtTile))
		{
			num2 = 0.5f;
			objectTypesAtTile.StopUsing();
			flag = true;
			if (objectTypesAtTile.m_TypeIdentifier == ObjectType.WheatSeed)
			{
				QuestManager.Instance.AddEvent(QuestEvent.Type.FeedChicken, Bot: false, 0, this);
			}
		}
		else if (TileManager.Instance.GetTile(m_TileCoord).m_TileType == Tile.TileType.Soil)
		{
			flag = true;
			num2 = 0.25f;
		}
		if (flag)
		{
			if (TileManager.Instance.GetTile(m_TileCoord).m_WalledArea != null)
			{
				num2 *= 4f;
			}
			if (CheatManager.Instance.m_FastEat)
			{
				num2 = 5f;
			}
			m_EatCount += num2;
			m_FatCount += num2;
			if (m_FatCount >= m_MaxFatCount)
			{
				m_FatCount = m_MaxFatCount;
			}
			UpdateFatCount();
		}
		SetState(State.None);
	}

	protected override void UpdateStateCarry()
	{
		if ((bool)m_Carrier && (bool)m_Carrier.GetComponent<Farmer>())
		{
			m_CarryLegsTimer += TimeManager.Instance.m_NormalDelta;
			if ((int)(m_CarryLegsTimer * 60f) % 10 < 5)
			{
				m_LeftWing.transform.localRotation = Quaternion.Euler(0f, 120f, 0f);
				m_RightWing.transform.localRotation = Quaternion.Euler(0f, -120f, 0f);
			}
			else
			{
				m_LeftWing.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
				m_RightWing.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			}
		}
		m_FeatherParticles.transform.position = base.transform.position + new Vector3(0f, 1f, 0f);
		if (m_PlaySound == null)
		{
			m_PlaySound = AudioManager.Instance.StartEvent("AnimalChickenCarried", this, Remember: true, Follow: true);
		}
	}

	protected override void UpdateStateSleep()
	{
		if (!DayNightManager.Instance.GetIsNightTime())
		{
			if ((bool)m_ChickenCoop)
			{
				SetState(State.WaitToExitBuilding);
			}
			else
			{
				SetState(State.None);
			}
		}
	}

	protected override void UpdateStateEnterBuilding()
	{
		if (SpawnAnimationManager.Instance.GetIsObjectSpawning(this))
		{
			return;
		}
		if ((bool)m_ChickenCoop && m_ChickenCoop.GetIsSavable())
		{
			UpdatePositionToTilePosition(m_ChickenCoop.m_TileCoord);
			if (DayNightManager.Instance.GetIsNightTime())
			{
				SetState(State.Sleep);
			}
			else
			{
				SetState(State.DoBusiness);
			}
		}
		else
		{
			UpdatePositionToTilePosition(m_TileCoord);
			SetState(State.None);
		}
	}

	protected override void UpdateStateDoBusiness()
	{
		if (m_OldStateTimer < 2.5f && m_StateTimer >= 2.5f)
		{
			TileCoord startPosition;
			TileCoord endPosition;
			if (m_ChickenCoop != null)
			{
				startPosition = m_ChickenCoop.GetSpawnPointEject();
				endPosition = m_ChickenCoop.GetSpawnPoint();
				QuestManager.Instance.AddEvent(QuestEvent.Type.ChickenCoopMakeEgg, Bot: false, 0, this);
				BadgeManager.Instance.AddEvent(BadgeEvent.Type.Eggs);
			}
			else
			{
				startPosition = m_TileCoord;
				endPosition = TileHelpers.GetRandomEmptyTile(m_TileCoord);
			}
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Egg, endPosition.ToWorldPositionTileCentered(), Quaternion.identity);
			SpawnAnimationManager.Instance.AddJump(baseClass, startPosition, endPosition, 0f, baseClass.transform.position.y, 4f);
		}
		m_OldStateTimer = m_StateTimer;
		if (m_StateTimer > 3f)
		{
			m_FatCount = 0f;
			UpdateFatCount();
			if (m_ChickenCoop != null)
			{
				SetState(State.ExitBuilding);
			}
			else
			{
				SetState(State.None);
			}
		}
	}

	protected override void UpdateStateExitBuilding()
	{
		m_CarryLegsTimer += TimeManager.Instance.m_NormalDelta;
		if ((int)(m_CarryLegsTimer * 60f) % 10 < 5)
		{
			m_LeftWing.transform.localRotation = Quaternion.Euler(0f, 120f, 0f);
			m_RightWing.transform.localRotation = Quaternion.Euler(0f, -120f, 0f);
		}
		else
		{
			m_LeftWing.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			m_RightWing.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		}
		m_FeatherParticles.transform.position = base.transform.position + new Vector3(0f, 1f, 0f);
		if (!SpawnAnimationManager.Instance.GetIsObjectSpawning(this))
		{
			m_LeftWing.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			m_RightWing.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			SetState(State.None);
		}
	}

	protected override void UpdateStateFindingTarget()
	{
		if (m_FindCoop)
		{
			if (m_FindFinished)
			{
				m_FindCoop = false;
				if (m_FoundObjects != null && m_FoundObjects.Count > 0)
				{
					TileCoordObject tileCoordObject = m_FoundObjects[0];
					m_TargetChickenCoopID = tileCoordObject.m_UniqueID;
					RequestGoTo(tileCoordObject.GetComponent<Building>().GetAccessPosition());
				}
				else if (DayNightManager.Instance.GetIsNightTime())
				{
					SetState(State.Sleep);
				}
				else
				{
					UpdateStateNoneNext();
				}
			}
		}
		else
		{
			base.UpdateStateFindingTarget();
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
