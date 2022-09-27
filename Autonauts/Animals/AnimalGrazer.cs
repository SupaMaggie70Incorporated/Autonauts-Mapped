using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class AnimalGrazer : Animal
{
	[HideInInspector]
	public enum State
	{
		None,
		Moving,
		Eat,
		Poop,
		Create,
		WaitCarry,
		Carry,
		Full,
		RequestMove,
		Sleep,
		EnterBuilding,
		DoBusiness,
		WaitToExitBuilding,
		ExitBuilding,
		FindingTarget,
		Total
	}

	public State m_State;

	public float m_StateTimer;

	public float m_EatCount;

	protected float m_MaxEatCount;

	public float m_FatCount;

	protected float m_MaxFatCount;

	public float m_OldFatPercent;

	protected bool m_RequestWaitCarry;

	protected Actionable m_Carrier;

	protected int m_CarryPosition;

	protected float m_CarryRotation;

	protected float m_CarryLegsTimer;

	protected float m_EatDelay;

	protected int m_ThreatRange = 5;

	[HideInInspector]
	public GameObject m_HatParent;

	[HideInInspector]
	public Hat m_CurrentHat;

	[HideInInspector]
	public GameObject m_Head;

	[HideInInspector]
	public GameObject m_Eye;

	[HideInInspector]
	public GameObject m_Eye2;

	protected bool m_Busy;

	protected bool m_FindingFavouriteFood;

	protected List<ObjectType> m_FavouriteFoodTypes;

	protected int m_EatRange;

	private bool m_FullSearch;

	private State m_TempState;

	private float m_TempStateTimer;

	protected Vector3 m_HeadPosition;

	private float m_OldHeadY;

	protected float m_EatHeadMoveSize;

	private bool m_FindBuilding;

	private int m_TargetBuildingID;

	public static bool GetIsTypeAnimalGrazer(ObjectType NewType)
	{
		if (NewType == ObjectType.AnimalChicken || AnimalSheep.GetIsTypeSheep(NewType) || AnimalCow.GetIsTypeCow(NewType))
		{
			return true;
		}
		return false;
	}

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("AnimalGrazer", m_TypeIdentifier);
	}

	public override void Restart()
	{
		base.Restart();
		m_MoveNormalDelay = 0.2f;
		if (!ObjectTypeList.m_Loading)
		{
			CollectionManager.Instance.AddCollectable("AnimalGrazer", this);
		}
		m_CurrentHat = null;
		m_EatCount = 0f;
		m_MaxEatCount = 10f;
		m_FatCount = 0f;
		m_MaxFatCount = 25f;
		m_OldFatPercent = 0f;
		m_EatDelay = 3f;
		m_RequestWaitCarry = false;
		m_State = State.None;
		SetState(State.None);
		m_StateTimer = Random.Range(-4f, 0f);
		m_Busy = false;
		m_OldHeadY = 0f;
		m_FullSearch = false;
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_EatRange = 15;
		m_FavouriteFoodTypes = new List<ObjectType>();
		m_FavouriteFoodTypes.Add(ObjectType.Trough);
		m_FavouriteFoodTypes.Add(ObjectType.HayBale);
		m_FavouriteFoodTypes.Add(ObjectType.Wheat);
		m_FavouriteFoodTypes.Add(ObjectType.WheatSeed);
		m_FavouriteFoodTypes.Add(ObjectType.CarrotSeed);
		m_FavouriteFoodTypes.Add(ObjectType.Straw);
		m_FavouriteFoodTypes.Add(ObjectType.GrassCut);
		if ((bool)m_ModelRoot)
		{
			m_HatParent = ObjectUtils.FindDeepChild(m_ModelRoot.transform, "HatPoint").gameObject;
			string aName = "HeadRoot";
			if (m_TypeIdentifier == ObjectType.AnimalChicken)
			{
				aName = "Body";
			}
			Transform transform = ObjectUtils.FindDeepChild(m_ModelRoot.transform, aName);
			if ((bool)m_HatParent && (bool)transform)
			{
				Quaternion localRotation = Quaternion.Inverse(transform.localRotation) * m_HatParent.transform.localRotation;
				m_HatParent.transform.localRotation = localRotation;
			}
		}
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		SetBaggedObject(null);
		if (m_CurrentHat != null)
		{
			m_CurrentHat.StopUsing();
			m_CurrentHat = null;
		}
		base.StopUsing(AndDestroy);
	}

	public override string GetCheatRolloverText()
	{
		string text = base.GetCheatRolloverText() + "\n\r";
		float num = m_FatCount / m_MaxFatCount;
		return text + "Food = " + (int)(num * 100f) + "%";
	}

	public override string GetHumanReadableName()
	{
		string text = base.GetHumanReadableName();
		if (TileManager.Instance.GetTile(m_TileCoord).m_WalledArea != null)
		{
			text = text + " " + TextManager.Instance.Get("AnimalEnclosed");
		}
		if (GetIsFull())
		{
			text = text + " (" + TextManager.Instance.Get("AnimalFull") + ")";
		}
		if (m_State == State.Sleep)
		{
			text = text + " (" + TextManager.Instance.Get("AnimalSleeping") + ")";
		}
		return text;
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
		JSONUtils.Set(Node, "EC", m_EatCount);
		JSONUtils.Set(Node, "FC", m_FatCount);
		JSONUtils.Set(Node, "TB", m_TargetBuildingID);
		if (m_CurrentHat != null)
		{
			JSONNode jSONNode = new JSONObject();
			string saveNameFromIdentifier = ObjectTypeList.Instance.GetSaveNameFromIdentifier(m_CurrentHat.m_TypeIdentifier);
			JSONUtils.Set(jSONNode, "ID", saveNameFromIdentifier);
			m_CurrentHat.Save(jSONNode);
			Node["Hat"] = jSONNode;
		}
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		CollectionManager.Instance.AddCollectable("AnimalGrazer", this);
		m_TempState = (State)JSONUtils.GetAsInt(Node, "ST", 0);
		if (m_TempState == State.RequestMove)
		{
			m_TempState = State.None;
		}
		m_TempStateTimer = JSONUtils.GetAsFloat(Node, "STT", 0f);
		m_EatCount = JSONUtils.GetAsFloat(Node, "EC", 0f);
		m_FatCount = JSONUtils.GetAsFloat(Node, "FC", 0f);
		if (m_FatCount < 0f)
		{
			m_FatCount = 0f;
		}
		m_TargetBuildingID = JSONUtils.GetAsInt(Node, "TB", 0);
		UpdateFatScale(m_OldFatPercent = GetFatPercent());
		JSONNode jSONNode = Node["Hat"];
		if (jSONNode != null && !jSONNode.IsNull)
		{
			JSONNode asObject = jSONNode.AsObject;
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromSaveName(asObject, new Vector3(0f, 0f, 0f), Quaternion.identity);
			if ((bool)baseClass)
			{
				baseClass.GetComponent<Savable>().Load(asObject);
				ApplyHat(baseClass.GetComponent<Hat>());
			}
		}
	}

	public override object GetActionInfo(GetActionInfo Info)
	{
		if (Info.m_Action == GetAction.GetObjectType)
		{
			if (Info.m_Value == AFO.AT.AltPrimary.ToString() && m_CurrentHat != null)
			{
				return m_CurrentHat.m_TypeIdentifier;
			}
			return ObjectTypeList.m_Total;
		}
		return base.GetActionInfo(Info);
	}

	private void StartAddHat(AFO Info)
	{
		Actionable @object = Info.m_Object;
		AudioManager.Instance.StartEvent("ClothingApplied", this);
		ApplyHat(@object.GetComponent<Hat>());
		ModManager.Instance.CheckCustomCallback(ModManager.CallbackTypes.ClothingHatAdded, @object.m_TypeIdentifier, m_TileCoord, @object.m_UniqueID, m_UniqueID);
	}

	private void AbortAddHat(AFO Info)
	{
		RemoveHat(Jump: false);
	}

	private ActionType GetActionFromHat(AFO Info)
	{
		Info.m_FarmerState = Farmer.State.Adding;
		Info.m_StartAction = StartAddHat;
		Info.m_AbortAction = AbortAddHat;
		if (m_CurrentHat != null && m_CurrentHat.m_TypeIdentifier == Info.m_ObjectType)
		{
			return ActionType.Fail;
		}
		return ActionType.AddResource;
	}

	public void StartActionNothingAlt(AFO Info)
	{
		Holdable newHoldable = RemoveHat(Jump: false);
		Info.m_Actioner.GetComponent<Farmer>().m_FarmerCarry.TryAddCarry(newHoldable);
	}

	private void AbortAddNothingAlt(AFO Info)
	{
		Holdable lastObject = Info.m_Actioner.GetComponent<Farmer>().m_FarmerCarry.GetLastObject();
		ApplyHat(lastObject.GetComponent<Hat>());
	}

	private ActionType GetActionFromNothingAlt(AFO Info)
	{
		Info.m_StartAction = StartActionNothingAlt;
		Info.m_AbortAction = AbortAddNothingAlt;
		Info.m_FarmerState = Farmer.State.Taking;
		if (m_CurrentHat == null)
		{
			return ActionType.Fail;
		}
		return ActionType.TakeResource;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (Info.m_ActionType == AFO.AT.AltSecondary && Hat.GetIsTypeHat(Info.m_ObjectType))
		{
			return GetActionFromHat(Info);
		}
		if (Info.m_ActionType == AFO.AT.AltPrimary && Info.m_ObjectType == ObjectTypeList.m_Total && (bool)m_CurrentHat)
		{
			return GetActionFromNothingAlt(Info);
		}
		return base.GetActionFromObject(Info);
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		base.transform.localRotation = Quaternion.Euler(180f, 90f, 0f);
		m_Carrier = Holder;
		SetBaggedObject(null);
		SetBaggedTile(default(TileCoord));
		SetState(State.Carry);
		WalledAreaManager.Instance.CheckAllAreasForCowsOrSheep(m_TileCoord);
	}

	protected override void ActionDropped(Actionable PreviousHolder, TileCoord DropLocation)
	{
		base.ActionDropped(PreviousHolder, DropLocation);
		base.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		m_Carrier = null;
		if (m_State != State.ExitBuilding)
		{
			SetState(State.None);
		}
		CheckEnclosed();
		WalledAreaManager.Instance.CheckAllAreasForCowsOrSheep(m_TileCoord);
	}

	private void CheckEnclosed()
	{
		if (TileManager.Instance.GetTile(m_TileCoord).m_WalledArea != null)
		{
			m_Indicator.SetEnclosed(Enclosed: true);
		}
		else
		{
			m_Indicator.SetEnclosed(Enclosed: false);
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

	protected virtual void UpdateFatScale(float Percent)
	{
	}

	private Hat RemoveHat(bool Jump)
	{
		if (m_CurrentHat != null)
		{
			ModManager.Instance.CheckCustomCallback(ModManager.CallbackTypes.ClothingHatRemoved, m_CurrentHat.m_TypeIdentifier, m_TileCoord, m_CurrentHat.m_UniqueID, m_UniqueID);
			m_CurrentHat.Remove();
			TileCoord randomEmptyTile = TileHelpers.GetRandomEmptyTile(m_TileCoord);
			m_CurrentHat.ForceHighlight(Highlighted: false);
			m_CurrentHat.SendAction(new ActionInfo(ActionType.Dropped, randomEmptyTile, this));
			if (Jump)
			{
				float yOffset = ObjectTypeList.Instance.GetYOffset(m_CurrentHat.m_TypeIdentifier);
				SpawnAnimationManager.Instance.AddJump(m_CurrentHat, m_TileCoord, randomEmptyTile, 0f, yOffset, 4f);
			}
			Hat currentHat = m_CurrentHat;
			m_CurrentHat = null;
			return currentHat;
		}
		return null;
	}

	private void ApplyHat(Hat NewHat)
	{
		RemoveHat(Jump: true);
		if ((bool)NewHat)
		{
			m_CurrentHat = NewHat;
			m_CurrentHat.SendAction(new ActionInfo(ActionType.BeingHeld, default(TileCoord), this));
			m_CurrentHat.Wear(base.gameObject, m_HatParent.transform);
		}
	}

	protected virtual void BodyIncreased()
	{
	}

	protected virtual void UpdateFatCount()
	{
		float fatPercent = GetFatPercent();
		if (m_OldFatPercent != fatPercent)
		{
			m_OldFatPercent = fatPercent;
			BodyIncreased();
		}
	}

	protected void FaceTarget()
	{
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(m_BaggedObjectUID, ErrorCheck: false);
		if ((bool)objectFromUniqueID)
		{
			TileCoord tileCoord = objectFromUniqueID.GetComponent<TileCoordObject>().m_TileCoord;
			if (m_TileCoord != tileCoord)
			{
				Vector3 vector = tileCoord.ToWorldPositionTileCentered() - base.transform.position;
				float num = 0f - Mathf.Atan2(vector.z, vector.x);
				base.transform.localRotation = Quaternion.Euler(0f, num * 57.29578f - 90f, 0f);
			}
		}
	}

	public void RequestWaitCarry()
	{
		m_RequestWaitCarry = true;
	}

	public void StartCarry(GameObject Carrier)
	{
	}

	protected virtual void EndCarry()
	{
	}

	public override void SendAction(ActionInfo Info)
	{
		switch (Info.m_Action)
		{
		case ActionType.BeingHeld:
			m_TempState = State.Total;
			break;
		case ActionType.Refresh:
		case ActionType.RefreshFirst:
			if (m_TempState != State.Total)
			{
				SetState(m_TempState);
				m_StateTimer = m_TempStateTimer;
				m_TempState = State.Total;
			}
			break;
		}
		base.SendAction(Info);
	}

	public virtual void SetState(State NewState)
	{
		switch (m_State)
		{
		case State.Full:
			m_Indicator.SetFull(Full: false);
			break;
		case State.Sleep:
			m_Indicator.SetSleeping(Sleeping: false);
			break;
		}
		m_State = NewState;
		m_StateTimer = 0f;
		switch (m_State)
		{
		case State.Full:
			m_Indicator.SetFull(Full: true);
			break;
		case State.Sleep:
			m_Indicator.SetSleeping(Sleeping: true);
			break;
		}
	}

	public override bool RequestGoTo(TileCoord Destination, Actionable TargetObject = null, bool LessOne = false, int Range = 0)
	{
		if (m_State != 0 && m_State != State.Moving && m_State != State.RequestMove && m_State != State.FindingTarget)
		{
			return false;
		}
		m_Indicator.SetNeedFood(NeedsFood: false);
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
		CheckEnclosed();
	}

	public override void ObstructionEncountered()
	{
		base.ObstructionEncountered();
		SetState(State.RequestMove);
		base.RequestGoTo(m_GoToTilePosition, m_GoToTargetObject, m_GoToLessOne);
	}

	private bool CheckTargetBuilding()
	{
		if (m_FatCount >= m_MaxFatCount)
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(m_TargetBuildingID, ErrorCheck: false);
			if ((bool)objectFromUniqueID)
			{
				TileCoord accessPosition = objectFromUniqueID.GetComponent<Building>().GetAccessPosition();
				if (m_TileCoord == accessPosition)
				{
					AnimalStation component = objectFromUniqueID.GetComponent<AnimalStation>();
					if (!component.GetIsFull())
					{
						Vector3 position = base.transform.position;
						component.StartAction(this, null, AFO.AT.Secondary, m_TileCoord);
						SpawnAnimationManager.Instance.AddJump(this, position, base.transform.position, 5f);
						SetState(State.EnterBuilding);
					}
					else
					{
						SetState(State.None);
					}
				}
				else
				{
					SetState(State.None);
				}
				return true;
			}
		}
		return false;
	}

	public override void EndGoTo()
	{
		base.EndGoTo();
		if (!CheckTargetBuilding())
		{
			CheckEnclosed();
			SetState(State.None);
			TestOccupiedTile();
		}
	}

	protected virtual void TestOccupiedTile()
	{
	}

	public bool GetIsFull()
	{
		return m_FatCount == m_MaxFatCount;
	}

	public TileCoordObject GetNearbyThreat(TileCoord NewTileCoord)
	{
		return null;
	}

	protected virtual void GoToSecondFood()
	{
		List<TileCoordObject> list = (m_FullSearch ? PlotManager.Instance.GetObjectsOfType(ObjectType.Grass, m_TileCoord, m_EatRange, this, ObjectTypeList.m_Total, AFO.AT.Primary, "") : PlotManager.Instance.GetNearestObjectOfType(ObjectType.Grass, m_TileCoord, m_EatRange, this, ObjectTypeList.m_Total, AFO.AT.Primary, ""));
		if (list.Count > 0)
		{
			m_FindingFavouriteFood = false;
			RequestFind(list);
			SetState(State.FindingTarget);
		}
		else
		{
			m_Indicator.SetNeedFood(NeedsFood: true);
		}
	}

	protected void GoToFavouriteFood()
	{
		SetBaggedObject(null);
		List<TileCoordObject> objectsOfTypes = PlotManager.Instance.GetObjectsOfTypes(m_FavouriteFoodTypes, m_TileCoord, m_EatRange, this, ObjectTypeList.m_Total, AFO.AT.Primary, "");
		if (objectsOfTypes.Count > 0)
		{
			m_FindingFavouriteFood = true;
			RequestFind(objectsOfTypes);
			SetState(State.FindingTarget);
		}
		else
		{
			m_FullSearch = false;
			GoToSecondFood();
		}
	}

	protected bool GoToBuilding(ObjectType NewBuildingType, int Range)
	{
		TileHelpers.GetClippedTileCoordArea(m_TileCoord + new TileCoord(-Range, -Range), m_TileCoord + new TileCoord(Range, Range), out var TopLeft, out var BottomRight);
		List<TileCoordObject> objectsOfType = PlotManager.Instance.GetObjectsOfType(NewBuildingType, TopLeft, BottomRight, this, m_TypeIdentifier, AFO.AT.Secondary, "");
		if (objectsOfType.Count > 0)
		{
			m_FindBuilding = true;
			RequestFind(objectsOfType);
			SetState(State.FindingTarget);
			return true;
		}
		return false;
	}

	private void IsBaggedStillValid()
	{
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(m_BaggedObjectUID, ErrorCheck: false);
		if ((bool)objectFromUniqueID && (bool)objectFromUniqueID.GetComponent<Holdable>() && objectFromUniqueID.GetComponent<Holdable>().m_BeingHeld)
		{
			objectFromUniqueID = null;
			m_BaggedObjectUID = -1;
			m_BaggedObject = null;
		}
	}

	protected virtual void UpdateStateNone()
	{
	}

	protected virtual void UpdateStateMove()
	{
		IsBaggedStillValid();
	}

	protected virtual void UpdateStateEat()
	{
		float num = 0f;
		if ((int)(m_StateTimer * 60f) % 12 < 6)
		{
			num = 0f - m_EatHeadMoveSize;
		}
		m_Head.transform.localPosition = m_HeadPosition + new Vector3(0f, num, 0f);
		IsBaggedStillValid();
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(m_BaggedObjectUID, ErrorCheck: false);
		if (m_OldHeadY != num)
		{
			m_OldHeadY = num;
			if (m_OldHeadY != 0f)
			{
				if ((bool)objectFromUniqueID && (objectFromUniqueID.m_TypeIdentifier == ObjectType.HayBale || objectFromUniqueID.m_TypeIdentifier == ObjectType.Trough))
				{
					objectFromUniqueID.GetComponent<Actionable>().SendAction(new ActionInfo(ActionType.Bump, m_TileCoord, this));
				}
				else
				{
					BaseClass associatedObject = TileManager.Instance.GetTile(m_TileCoord).m_AssociatedObject;
					if ((bool)associatedObject && associatedObject.m_TypeIdentifier == ObjectType.Grass)
					{
						float percent = 1f - m_StateTimer / m_EatDelay;
						associatedObject.GetComponent<Grass>().Nibble(percent);
					}
				}
			}
		}
		if (!(m_StateTimer > m_EatDelay))
		{
			return;
		}
		m_Head.transform.localPosition = m_HeadPosition;
		SetBaggedObject(null);
		bool flag = false;
		float num2 = 0f;
		if ((bool)objectFromUniqueID)
		{
			if (objectFromUniqueID.m_TypeIdentifier == ObjectType.Trough)
			{
				flag = objectFromUniqueID.GetComponent<Trough>().EatHay();
				num2 = 0.75f;
			}
			else if (objectFromUniqueID.m_TypeIdentifier == ObjectType.HayBale)
			{
				objectFromUniqueID.GetComponent<HayBale>().Eat();
				flag = true;
				num2 = 0.5f;
			}
			else if (objectFromUniqueID.m_TypeIdentifier == ObjectType.Grass)
			{
				flag = m_Plot.ReapTile(m_TileCoord);
				num2 = 0.25f;
			}
			else if (objectFromUniqueID.m_TypeIdentifier == ObjectType.GrassCut)
			{
				EatFood(objectFromUniqueID);
				flag = true;
				num2 = 0.25f;
			}
			else
			{
				EatFood(objectFromUniqueID);
				flag = true;
				num2 = 0.5f;
			}
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
			bool flag2 = true;
			if (AnimalCow.GetIsTypeCow(m_TypeIdentifier) && (bool)QuestManager.Instance && QuestManager.Instance.GetIsObjectLocked(ObjectType.ToolBucketCrude))
			{
				flag2 = false;
			}
			else if (AnimalSheep.GetIsTypeSheep(m_TypeIdentifier) && (bool)QuestManager.Instance && QuestManager.Instance.GetIsObjectLocked(ObjectType.ToolShears))
			{
				flag2 = false;
			}
			if (flag2 || m_FatCount + num2 < m_MaxFatCount)
			{
				m_FatCount += num2;
				if (m_FatCount >= m_MaxFatCount)
				{
					m_FatCount = m_MaxFatCount;
				}
			}
			UpdateFatCount();
		}
		SetState(State.None);
	}

	protected virtual void UpdateStatePoop()
	{
	}

	protected virtual void UpdateStateCreate()
	{
	}

	protected virtual void UpdateStateCarry()
	{
	}

	protected virtual void UpdateStateSleep()
	{
	}

	protected virtual void UpdateStateEnterBuilding()
	{
	}

	protected virtual void UpdateStateDoBusiness()
	{
	}

	protected virtual void UpdateStateExitBuilding()
	{
	}

	protected void UpdateEnterBuilding()
	{
		if (SpawnAnimationManager.Instance.GetIsObjectSpawning(this))
		{
			return;
		}
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(m_TargetBuildingID, ErrorCheck: false);
		if ((bool)objectFromUniqueID && objectFromUniqueID.GetComponent<Savable>().GetIsSavable())
		{
			objectFromUniqueID.GetComponent<AnimalStation>().EndAction(this, null, AFO.AT.Secondary, m_TileCoord);
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

	protected virtual void UpdateStateFull()
	{
		if (GetIsSavable() && m_StateTimer >= VariableManager.Instance.GetVariableAsFloat(m_TypeIdentifier, "MaxFullTime") && !BaggedManager.Instance.IsObjectBagged(this))
		{
			m_FatCount = 0f;
			UpdateFatCount();
			SetState(State.None);
		}
	}

	protected virtual void UpdateStateWaitToExitBuilding()
	{
		if (m_StateTimer >= 0f)
		{
			SetState(State.ExitBuilding);
		}
	}

	private void UpdateFindingBuilding()
	{
		if (m_FindFinished)
		{
			m_FindBuilding = false;
			if (!m_Busy && m_FoundObjects != null && m_FoundObjects.Count > 0 && (bool)m_FoundObjects[0] && (bool)m_FoundObjects[0].GetComponent<Building>())
			{
				TileCoordObject tileCoordObject = m_FoundObjects[0];
				m_TargetBuildingID = tileCoordObject.m_UniqueID;
				RequestGoTo(tileCoordObject.GetComponent<Building>().GetAccessPosition());
			}
			else if (DayNightManager.Instance.GetIsNightTime())
			{
				SetState(State.Sleep);
			}
			else
			{
				SetState(State.None);
				UpdateStateNone();
			}
		}
	}

	protected virtual void UpdateStateFindingTarget()
	{
		if (m_FindBuilding)
		{
			UpdateFindingBuilding();
		}
		else
		{
			if (!m_FindFinished)
			{
				return;
			}
			RemoveBagged();
			if (m_FoundObjects != null && m_FoundObjects.Count > 0)
			{
				TileCoordObject tileCoordObject = (TileCoordObject)(m_BaggedObject = m_FoundObjects[0]);
				m_BaggedObjectUID = m_BaggedObject.m_UniqueID;
				if (tileCoordObject.m_TypeIdentifier == ObjectType.Trough)
				{
					TileCoord newGrazerPosition = tileCoordObject.GetComponent<Trough>().GetNewGrazerPosition(m_TileCoord);
					RequestGoTo(newGrazerPosition, tileCoordObject);
					return;
				}
				SetBaggedObject(m_BaggedObject.GetComponent<TileCoordObject>());
				bool lessOne = false;
				if (tileCoordObject.m_TypeIdentifier == ObjectType.HayBale)
				{
					lessOne = true;
				}
				RequestGoTo(tileCoordObject.m_TileCoord, tileCoordObject, lessOne);
			}
			else if (m_FoundDestinations != null && m_FoundDestinations.Count > 0)
			{
				int index = Random.Range(0, m_FoundDestinations.Count);
				TileCoord tileCoord = m_FoundDestinations[index];
				SetBaggedTile(tileCoord);
				RequestGoTo(tileCoord);
			}
			else if (m_FindingFavouriteFood)
			{
				m_FullSearch = false;
				GoToSecondFood();
				if (m_FindingFavouriteFood)
				{
					SetState(State.None);
				}
			}
			else if (!m_FullSearch)
			{
				m_FullSearch = true;
				GoToSecondFood();
			}
			else
			{
				m_Indicator.SetNeedFood(NeedsFood: true);
				SetState(State.None);
			}
		}
	}

	protected void EatFood(BaseClass NewObject)
	{
		if (NewObject.m_TypeIdentifier == ObjectType.HayBale)
		{
			NewObject.GetComponent<HayBale>().Eat();
		}
		else
		{
			NewObject.StopUsing();
		}
	}

	protected virtual void Update()
	{
		if ((bool)TimeManager.Instance && !TimeManager.Instance.m_NormalTimeEnabled)
		{
			return;
		}
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
		case State.Poop:
			UpdateStatePoop();
			break;
		case State.Create:
			UpdateStateCreate();
			break;
		case State.Carry:
			if ((bool)m_Carrier && (bool)m_Carrier.GetComponent<Farmer>())
			{
				UpdateStateCarry();
			}
			break;
		case State.Full:
			UpdateStateFull();
			break;
		case State.Sleep:
			UpdateStateSleep();
			break;
		case State.EnterBuilding:
			UpdateStateEnterBuilding();
			break;
		case State.DoBusiness:
			UpdateStateDoBusiness();
			break;
		case State.ExitBuilding:
			UpdateStateExitBuilding();
			break;
		case State.WaitToExitBuilding:
			UpdateStateWaitToExitBuilding();
			break;
		case State.FindingTarget:
			UpdateStateFindingTarget();
			break;
		}
		if ((bool)TimeManager.Instance)
		{
			m_StateTimer += TimeManager.Instance.m_NormalDelta;
		}
	}
}
