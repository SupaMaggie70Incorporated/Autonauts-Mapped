using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Rendering;

public class Worker : Farmer
{
	public enum WorkerType
	{
		Mk0,
		Mk1,
		Mk2,
		Mk3,
		Total
	}

	private static bool m_LoseEnergy = true;

	private static bool m_AllowEnergy = true;

	public static WorkerFrameInfo[] m_AllFrameInfo;

	public static WorkerHeadInfo[] m_AllHeadInfo;

	public static WorkerDriveInfo[] m_AllDriveInfo;

	public int m_FrameLevel;

	public WorkerFrameInfo m_FrameInfo;

	public int m_HeadLevel;

	public WorkerHeadInfo m_HeadInfo;

	public int m_DriveLevel;

	public WorkerDriveInfo m_DriveInfo;

	public int m_ExtraMemorySize;

	public int m_ExtraSearchRange;

	public int m_ExtraSearchDelay;

	public int m_ExtraMovementDelay;

	public float m_ExtraMovementScale;

	public float m_ExtraEnergy;

	public static bool m_BadgeUsable = false;

	[HideInInspector]
	public WorkerInterpreter m_WorkerInterpreter;

	private bool m_ListenRequested;

	[HideInInspector]
	public WorkerStatusIndicator m_WorkerIndicator;

	[HideInInspector]
	public WorkerArrow m_WorkerArrow;

	[HideInInspector]
	public string m_WorkerName;

	private MyParticles m_DustParticles;

	[HideInInspector]
	public WorkerInfoPanel m_WorkerInfoPanel;

	private MeshRenderer m_BulbModel;

	private Light m_Light;

	private Light m_FrontLight;

	private float m_LightTimer;

	private bool m_LightEnabled;

	[HideInInspector]
	public ObjectType m_Drive;

	[HideInInspector]
	public ObjectType m_Head;

	[HideInInspector]
	public ObjectType m_Frame;

	private GameObject m_DriveModel;

	private GameObject m_HeadModel;

	private GameObject m_FrameModel;

	private GameObject m_Key;

	private PlaySound m_WorkingSound;

	public float m_NoToolTimer;

	[HideInInspector]
	public WorkerGroup m_Group;

	private bool m_ModelDirty;

	public static float m_NightIntensity = 1.5f;

	public static float m_DayIntensity = 2f;

	public static float m_LearnIntensityBoost = 1f;

	public static float m_ErrorIntensityBoost = 1f;

	public static float m_NightRange = 1.5f;

	public static float m_DayRange = 2f;

	public static Color m_IdleColour = new Color(0.75f, 0.35f, 0f);

	public static Color m_WorkingColour = new Color(0f, 0.5f, 0f);

	public static Color m_ErrorColour = new Color(1f, 0f, 0f);

	private MeshRenderer m_Renderer;

	private Mesh[] m_TempMeshes;

	private MeshFilter m_Filter;

	private static Quaternion m_ParticleRotation = Quaternion.Euler(-90f, 0f, 0f);

	private Color m_LightColour;

	private float m_LightIntensity;

	private float m_LightRange;

	private GameObject m_Collider;

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("Worker", m_TypeIdentifier);
	}

	public override void Restart()
	{
		base.Restart();
		InitStats();
		m_EnergyLossPerSecond = 0.5f;
		m_DriveModel = null;
		m_HeadModel = null;
		m_FrameModel = null;
		SetMoveSpeedScale(1f);
		SetActionSpeedScale(1f);
		m_WorkerInterpreter.Restart();
		m_WorkerIndicator.Restart();
		m_WorkerArrow.Restart();
		m_ListenRequested = false;
		AddToWorld();
		SetState(State.None);
		m_LightTimer = Random.Range(0f, 1f);
		m_FrontLight.transform.localPosition = new Vector3(0f, 2.315f, -0.579f);
		m_FrontLight.range = 0.5f;
		m_WorkerIndicator.UpdateIndicator();
		m_ExtraMemorySize = 0;
		m_ExtraSearchRange = 0;
		m_ExtraSearchDelay = 0;
		m_ExtraMovementDelay = 0;
		m_ExtraMovementScale = 0f;
		m_ExtraEnergy = 0f;
		m_Group = null;
		UpdateLights();
		m_WorkingSound = null;
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		RemoveFromWorld();
		m_WorkerName = "";
		base.StopUsing(AndDestroy);
	}

	public void RemoveFromWorld()
	{
		StopWorkingSound();
		if ((bool)m_WorkerIndicator)
		{
			m_WorkerIndicator.gameObject.SetActive(value: false);
		}
		ShowName(Show: false);
		if ((bool)m_DustParticles)
		{
			if ((bool)ParticlesManager.Instance)
			{
				ParticlesManager.Instance.DestroyParticles(m_DustParticles);
			}
			m_DustParticles = null;
		}
		if (m_WorkerInfoPanel != null)
		{
			if ((bool)TabWorkers.Instance)
			{
				TabWorkers.Instance.RemoveWorker(this);
			}
			m_WorkerInfoPanel = null;
		}
		if ((bool)BuildingReferenceManager.Instance)
		{
			BuildingReferenceManager.Instance.RemoveWorker(this);
		}
		CollectionManager.Instance.RemoveCollectable("Worker", this);
	}

	public void AddToWorld()
	{
		if (!ObjectTypeList.m_Loading)
		{
			CollectionManager.Instance.AddCollectable("Worker", this);
		}
		if ((bool)TabManager.Instance && TabManager.Instance.m_ActiveTabType == TabManager.TabType.Workers)
		{
			ShowName(Show: true);
		}
		if ((bool)ParticlesManager.Instance)
		{
			m_DustParticles = ParticlesManager.Instance.CreateParticles("BotDust", default(Vector3), m_ParticleRotation);
			m_DustParticles.Stop();
		}
		if (m_WorkerInfoPanel == null && (bool)TabWorkers.Instance)
		{
			m_WorkerInfoPanel = TabWorkers.Instance.AddWorker(this);
		}
	}

	private void InitStats()
	{
		if (m_AllFrameInfo == null)
		{
			int num = 4;
			m_AllFrameInfo = new WorkerFrameInfo[num];
			m_AllHeadInfo = new WorkerHeadInfo[num];
			m_AllDriveInfo = new WorkerDriveInfo[num];
			for (int i = 0; i < num; i++)
			{
				m_AllFrameInfo[i] = new WorkerFrameInfo();
				m_AllHeadInfo[i] = new WorkerHeadInfo();
				m_AllDriveInfo[i] = new WorkerDriveInfo();
			}
		}
		ObjectType[] array = new ObjectType[4]
		{
			ObjectType.WorkerFrameMk0,
			ObjectType.WorkerFrameMk1,
			ObjectType.WorkerFrameMk2,
			ObjectType.WorkerFrameMk3
		};
		for (int j = 0; j < array.Length; j++)
		{
			m_AllFrameInfo[j].m_WorkingSoundName = VariableManager.Instance.GetVariableAsString(array[j], "WorkingSoundName");
			m_AllFrameInfo[j].m_CarrySize = VariableManager.Instance.GetVariableAsInt(array[j], "CarrySize");
			m_AllFrameInfo[j].m_InventorySize = VariableManager.Instance.GetVariableAsInt(array[j], "InventorySize");
			m_AllFrameInfo[j].m_UpgradeSize = VariableManager.Instance.GetVariableAsInt(array[j], "UpgradeSize");
			m_AllFrameInfo[j].m_Scale = VariableManager.Instance.GetVariableAsFloat(array[j], "FrameScale");
		}
		ObjectType[] array2 = new ObjectType[4]
		{
			ObjectType.WorkerHeadMk0,
			ObjectType.WorkerHeadMk1,
			ObjectType.WorkerHeadMk2,
			ObjectType.WorkerHeadMk3
		};
		for (int k = 0; k < array2.Length; k++)
		{
			m_AllHeadInfo[k].m_MaxInstructions = VariableManager.Instance.GetVariableAsInt(array2[k], "MaxInstructions");
			m_AllHeadInfo[k].m_SerialPrefix = VariableManager.Instance.GetVariableAsString(array2[k], "SerialPrefix");
			m_AllHeadInfo[k].m_FindNearestRange = VariableManager.Instance.GetVariableAsInt(array2[k], "FindNearestRange");
			m_AllHeadInfo[k].m_FindNearestDelay = VariableManager.Instance.GetVariableAsInt(array2[k], "FindNearestDelay");
			m_AllHeadInfo[k].m_Scale = VariableManager.Instance.GetVariableAsFloat(array2[k], "HeadScale");
		}
		ObjectType[] array3 = new ObjectType[4]
		{
			ObjectType.WorkerDriveMk0,
			ObjectType.WorkerDriveMk1,
			ObjectType.WorkerDriveMk2,
			ObjectType.WorkerDriveMk3
		};
		for (int l = 0; l < array3.Length; l++)
		{
			m_AllDriveInfo[l].m_MoveSoundName = VariableManager.Instance.GetVariableAsString(array3[l], "MoveSoundName");
			m_AllDriveInfo[l].m_MoveStoneSoundName = VariableManager.Instance.GetVariableAsString(array3[l], "MoveStoneSoundName");
			m_AllDriveInfo[l].m_MoveClaySoundName = VariableManager.Instance.GetVariableAsString(array3[l], "MoveClaySoundName");
			m_AllDriveInfo[l].m_SpeedScale = VariableManager.Instance.GetVariableAsFloat(array3[l], "SpeedScale");
			m_AllDriveInfo[l].m_MoveInitialDelay = VariableManager.Instance.GetVariableAsInt(array3[l], "MoveInitialDelay");
			m_AllDriveInfo[l].m_RechargeDelay = VariableManager.Instance.GetVariableAsFloat(array3[l], "RechargeDelay");
			m_AllDriveInfo[l].m_Energy = VariableManager.Instance.GetVariableAsFloat(array3[l], "DriveEnergy");
			m_AllDriveInfo[l].m_Scale = VariableManager.Instance.GetVariableAsFloat(array3[l], "DriveScale");
		}
	}

	protected new void Awake()
	{
		InitStats();
		base.Awake();
		m_DustParticles = null;
		m_WorkerInterpreter = base.gameObject.AddComponent<WorkerInterpreter>();
		GameObject original = (GameObject)Resources.Load("Prefabs/WorldObjects/Other/WorkerStatusIndicator", typeof(GameObject));
		Transform parent = null;
		if ((bool)HudManager.Instance)
		{
			parent = HudManager.Instance.m_IndicatorsRootTransform;
		}
		m_WorkerIndicator = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, parent).GetComponent<WorkerStatusIndicator>();
		m_WorkerIndicator.SetFarmer(this);
		m_WorkerIndicator.Restart();
		original = (GameObject)Resources.Load("Prefabs/WorldObjects/Other/WorkerArrow", typeof(GameObject));
		parent = null;
		if ((bool)HudManager.Instance)
		{
			parent = HudManager.Instance.m_WorkerNamesRootTransform;
		}
		m_WorkerArrow = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, parent).GetComponent<WorkerArrow>();
		m_WorkerArrow.SetWorker(this);
		m_WorkerArrow.Restart();
		m_WorkerInfoPanel = null;
	}

	protected new void OnDestroy()
	{
		if ((bool)m_WorkerIndicator)
		{
			Object.Destroy(m_WorkerIndicator.gameObject);
		}
		if ((bool)m_WorkerArrow)
		{
			Object.Destroy(m_WorkerArrow.gameObject);
		}
		if ((bool)m_DustParticles)
		{
			Object.Destroy(m_DustParticles.gameObject);
		}
		base.OnDestroy();
	}

	public override void PostCreate()
	{
		base.PostCreate();
		if ((bool)m_ModelRoot)
		{
			GameObject original = (GameObject)Resources.Load("Prefabs/Lights/WorkerLight", typeof(GameObject));
			m_Light = Object.Instantiate(original, base.transform.position, Quaternion.identity, m_ModelRoot.transform).GetComponent<Light>();
			m_Light.transform.localPosition = new Vector3(0f, 0f, 0f);
			m_LightTimer = 0f;
			original = (GameObject)Resources.Load("Prefabs/Lights/WorkerFrontLight", typeof(GameObject));
			m_FrontLight = Object.Instantiate(original, base.transform.position, Quaternion.identity, m_ModelRoot.transform).GetComponent<Light>();
			m_FrontLight.transform.localPosition = new Vector3(0f, 0f, 0f);
		}
	}

	public override void PostLoad()
	{
		base.PostLoad();
		if ((bool)m_WorkerInterpreter)
		{
			m_WorkerInterpreter.PostLoad();
		}
	}

	public void SetListeningHighlight(bool Highlight)
	{
		if (m_State == State.Listening)
		{
			((FarmerStateListening)m_States[24]).SetHighlighted(Highlight);
		}
	}

	public override string GetCheatRolloverText()
	{
		return string.Concat(base.GetCheatRolloverText() + "\n\r", "Energy = ", (int)m_Energy);
	}

	public void ShowName(bool Show)
	{
		m_WorkerArrow.ShowName(Show);
	}

	public void ScriptChanged()
	{
		AudioManager.Instance.StartEvent("WorkerAcknowledgeTeach", this);
	}

	public void NewScriptTaught(WorkerScript NewScript)
	{
		AudioManager.Instance.StartEvent("WorkerAcknowledgeTeach", this);
		m_WorkerInterpreter.StartScript(NewScript);
	}

	public void NewHighScriptTaught(List<HighInstruction> NewInstructions, bool Start = true)
	{
		if (NewInstructions.Count > 0)
		{
			AudioManager.Instance.StartEvent("WorkerAcknowledgeTeach", this);
		}
		m_WorkerInterpreter.SetHighInstructions(NewInstructions, Start);
	}

	public void StopAllScripts()
	{
		AudioManager.Instance.StartEvent("WorkerStop", this);
		m_WorkerInterpreter.StopAll();
		m_WorkerIndicator.SetBusy(Busy: false);
		m_WorkerIndicator.SetQuestion(Question: false);
		m_WorkerIndicator.SetNoTool(NoTool: false, State.Total, ObjectTypeList.m_Total);
	}

	private void UpdateLights()
	{
		m_Light.gameObject.SetActive(SettingsManager.Instance.m_LightsEnabled);
		m_FrontLight.gameObject.SetActive(SettingsManager.Instance.m_LightsEnabled);
	}

	public State GetStateFromActionInfo(ActionInfo Info, out ActionType NewAction)
	{
		Holdable topObject = m_FarmerCarry.GetTopObject();
		Actionable.m_ReusableActionFromObject.Init(topObject, Info.m_ActionObjectType, this, Info.m_ActionType, Info.m_ActionRequirement, Info.m_Position);
		NewAction = ActionType.Total;
		if ((bool)Info.m_Object)
		{
			NewAction = Info.m_Object.GetActionFromObjectSafe(Actionable.m_ReusableActionFromObject);
		}
		return Actionable.m_ReusableActionFromObject.m_FarmerState;
	}

	public override void SendAction(ActionInfo Info)
	{
		if (m_ListenRequested)
		{
			m_ListenRequested = false;
			if (m_State != State.Held && m_State != State.BeingEngaged)
			{
				if (Info.m_Action == ActionType.DisengageObject)
				{
					m_FarmerAction.SendAction(Info);
				}
				StartListening();
				return;
			}
		}
		if (Info.m_Action != ActionType.UseInHands)
		{
			m_NoToolTimer = 0f;
		}
		switch (Info.m_Action)
		{
		case ActionType.UseInHands:
		{
			if (!(m_EngagedObject == null))
			{
				break;
			}
			ActionType NewAction;
			State stateFromActionInfo = GetStateFromActionInfo(Info, out NewAction);
			if (stateFromActionInfo != State.Total)
			{
				Holdable topObject = m_FarmerCarry.GetTopObject();
				bool flag = m_States[(int)stateFromActionInfo].IsToolAcceptable(topObject);
				if (ToolFillable.GetIsTypeFillable(Info.m_ActionObjectType) && (topObject == null || !ToolFillable.GetIsTypeFillable(topObject.m_TypeIdentifier)))
				{
					flag = false;
				}
				if (!flag && (bool)topObject)
				{
					flag = GetStateSpecialTool(stateFromActionInfo, topObject.m_TypeIdentifier, Info.m_Object);
					if (flag && m_FarmerCarry.GetCarryCount() != 1)
					{
						flag = false;
					}
				}
				if (!flag)
				{
					m_WorkerIndicator.SetNoTool(NoTool: true, stateFromActionInfo, Info.m_ActionObjectType);
					m_NoToolTimer = 1f;
				}
				else
				{
					m_WorkerIndicator.SetNoTool(NoTool: false, State.Total, ObjectTypeList.m_Total);
				}
			}
			else
			{
				m_WorkerIndicator.SetNoTool(NoTool: false, State.Total, ObjectTypeList.m_Total);
			}
			m_WorkerInterpreter.CheckRunning();
			if (m_NoToolTimer > 0f)
			{
				m_NoToolTimer -= TimeManager.Instance.m_NormalDelta;
				if (!(m_NoToolTimer < 0f))
				{
					return;
				}
				m_NoToolTimer = 0f;
			}
			if (m_WorkerIndicator.m_NoTool || NewAction == ActionType.Fail || NewAction == ActionType.Total)
			{
				return;
			}
			break;
		}
		case ActionType.UpdateLights:
			UpdateLights();
			break;
		case ActionType.Engaged:
		{
			m_Engager = Info.m_Object;
			SetState(State.BeingEngaged);
			ObjectType topObjectType = Info.m_Object.GetComponent<Farmer>().m_FarmerCarry.GetTopObjectType();
			if (topObjectType == ObjectType.DataStorageCrude && Info.m_ActionType == AFO.AT.AltPrimary)
			{
				m_WorkerIndicator.SetCopying(Copying: true);
			}
			else if (topObjectType == ObjectType.DataStorageCrude && Info.m_ActionType == AFO.AT.AltSecondary)
			{
				m_WorkerIndicator.SetRestoring(Restoring: true);
			}
			break;
		}
		case ActionType.Disengaged:
			m_Engager = null;
			SetState(State.None);
			break;
		}
		base.SendAction(Info);
	}

	private bool IsTargetPositionValid(ActionInfo Info)
	{
		if ((bool)Info.m_Object && Vehicle.GetIsTypeVehicle(Info.m_Object.m_TypeIdentifier) && Info.m_Position != Info.m_Object.GetComponent<TileCoordObject>().m_TileCoord)
		{
			return false;
		}
		return true;
	}

	public override bool CanDoAction(ActionInfo Info, bool RightNow)
	{
		switch (Info.m_Action)
		{
		case ActionType.MoveTo:
		case ActionType.MoveToLessOne:
		case ActionType.MoveToRange:
			if (!IsTargetPositionValid(Info))
			{
				return false;
			}
			if (m_ListenRequested)
			{
				m_ListenRequested = false;
				m_WorkerIndicator.SetTryingToListen(Listen: false);
				if (m_State != State.Held)
				{
					StartListening();
					return false;
				}
			}
			m_NoToolTimer = 0f;
			break;
		case ActionType.Engaged:
			if (m_Engager != null)
			{
				return false;
			}
			return true;
		case ActionType.Disengaged:
			return true;
		}
		return base.CanDoAction(Info, RightNow);
	}

	public override object GetActionInfo(GetActionInfo Info)
	{
		if (Info.m_Action == GetAction.GetObjectType)
		{
			if (Info.m_Value == AFO.AT.AltPrimary.ToString())
			{
				if ((bool)m_FarmerClothes.Get(FarmerClothes.Type.Hat))
				{
					return m_FarmerClothes.Get(FarmerClothes.Type.Hat).m_TypeIdentifier;
				}
				if ((bool)m_FarmerClothes.Get(FarmerClothes.Type.Top))
				{
					return m_FarmerClothes.Get(FarmerClothes.Type.Top).m_TypeIdentifier;
				}
				return ObjectTypeList.m_Total;
			}
			return m_FarmerCarry.GetTopObjectType();
		}
		return base.GetActionInfo(Info);
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		JSONUtils.Set(Node, "WN", m_WorkerName);
		string saveNameFromIdentifier = ObjectTypeList.Instance.GetSaveNameFromIdentifier(m_Frame);
		JSONUtils.Set(Node, "FM", saveNameFromIdentifier);
		saveNameFromIdentifier = ObjectTypeList.Instance.GetSaveNameFromIdentifier(m_Head);
		JSONUtils.Set(Node, "HD", saveNameFromIdentifier);
		saveNameFromIdentifier = ObjectTypeList.Instance.GetSaveNameFromIdentifier(m_Drive);
		JSONUtils.Set(Node, "DV", saveNameFromIdentifier);
		Node["Interpreter"] = new JSONObject();
		JSONNode node = Node["Interpreter"];
		m_WorkerInterpreter.Save(node);
	}

	public override void Load(JSONNode Node)
	{
		PlotManager.Instance.m_CapObjects = false;
		string asString = JSONUtils.GetAsString(Node, "FM", "");
		ObjectType identifierFromSaveName = ObjectTypeList.Instance.GetIdentifierFromSaveName(asString, Check: false);
		if (identifierFromSaveName != ObjectTypeList.m_Total)
		{
			SetFrame(identifierFromSaveName);
		}
		asString = JSONUtils.GetAsString(Node, "HD", "");
		identifierFromSaveName = ObjectTypeList.Instance.GetIdentifierFromSaveName(asString, Check: false);
		if (identifierFromSaveName != ObjectTypeList.m_Total)
		{
			SetHead(identifierFromSaveName);
		}
		asString = JSONUtils.GetAsString(Node, "DV", "");
		identifierFromSaveName = ObjectTypeList.Instance.GetIdentifierFromSaveName(asString, Check: false);
		if (identifierFromSaveName != ObjectTypeList.m_Total)
		{
			SetDrive(identifierFromSaveName);
		}
		UpdateModel();
		base.Load(Node);
		CollectionManager.Instance.AddCollectable("Worker", this);
		m_WorkerName = JSONUtils.GetAsString(Node, "WN", "Bot");
		m_WorkerInfoPanel.UpdateName();
		m_WorkerInfoPanel.UpdateHeldItem();
		m_WorkerArrow.UpdateName();
		JSONNode node = Node["Interpreter"];
		m_WorkerInterpreter.Load(node);
		if (m_Energy == 0f)
		{
			m_WorkerIndicator.SetNoEnergy(NoEnergy: true);
		}
		else
		{
			m_WorkerIndicator.SetNoEnergy(NoEnergy: false);
		}
		PlotManager.Instance.m_CapObjects = true;
	}

	public override string GetHumanReadableName()
	{
		string humanReadableNameFromIdentifier = ObjectTypeList.Instance.GetHumanReadableNameFromIdentifier(m_TypeIdentifier);
		string workerName = GetWorkerName();
		return humanReadableNameFromIdentifier + " (" + workerName + ")";
	}

	public void AttemptAddPart(Holdable NewPart)
	{
		WorkerPart.Type partType = NewPart.GetComponent<WorkerPart>().GetPartType();
		ObjectType objectType = ObjectTypeList.m_Total;
		switch (partType)
		{
		case WorkerPart.Type.Drive:
			objectType = m_Drive;
			break;
		case WorkerPart.Type.Frame:
			objectType = m_Frame;
			break;
		case WorkerPart.Type.Head:
			objectType = m_Head;
			break;
		}
		if (objectType != ObjectTypeList.m_Total)
		{
			NewPart.StopUsing();
			TileCoord randomEmptyTile = TileHelpers.GetRandomEmptyTile(m_TileCoord);
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(objectType, randomEmptyTile.ToWorldPositionTileCentered(), Quaternion.identity);
			SpawnAnimationManager.Instance.AddJump(baseClass, m_TileCoord, randomEmptyTile, 0f, baseClass.transform.position.y, 4f);
			switch (partType)
			{
			case WorkerPart.Type.Drive:
				SetDrive(NewPart.m_TypeIdentifier);
				break;
			case WorkerPart.Type.Frame:
				SetFrame(NewPart.m_TypeIdentifier);
				break;
			case WorkerPart.Type.Head:
				SetHead(NewPart.m_TypeIdentifier);
				break;
			}
		}
	}

	public void StartActionPart(AFO Info)
	{
		m_FarmerAction.m_CurrentInfo = null;
		SetState(State.UpgradedWait);
	}

	private void EndAddPart(AFO Info)
	{
		AttemptAddPart(Info.m_Object.GetComponent<Holdable>());
		if (m_State != State.Engaged || m_EngagedObject == null || !Vehicle.GetIsTypeVehicle(m_EngagedObject.m_TypeIdentifier))
		{
			SetState(State.Upgraded);
			Info.m_Actioner.GetComponent<Farmer>().SetState(State.Upgrading);
		}
	}

	private void AbortAddPart(AFO Info)
	{
		SetState(State.None);
	}

	private ActionType GetActionFromPart(AFO Info)
	{
		Info.m_StartAction = StartActionPart;
		Info.m_EndAction = EndAddPart;
		Info.m_AbortAction = AbortAddPart;
		Info.m_FarmerState = State.Adding;
		if (m_State != 0)
		{
			return ActionType.Fail;
		}
		if (Info.m_Object == null)
		{
			return ActionType.Fail;
		}
		if (!WorkerPart.GetIsTypePart(Info.m_Object.m_TypeIdentifier))
		{
			return ActionType.Fail;
		}
		WorkerPart.Type partType = Info.m_Object.GetComponent<WorkerPart>().GetPartType();
		ObjectType objectType = ObjectTypeList.m_Total;
		switch (partType)
		{
		case WorkerPart.Type.Drive:
			objectType = m_Drive;
			break;
		case WorkerPart.Type.Frame:
			objectType = m_Frame;
			break;
		case WorkerPart.Type.Head:
			objectType = m_Head;
			break;
		}
		if (objectType == Info.m_ObjectType)
		{
			return ActionType.Fail;
		}
		return ActionType.AddResource;
	}

	public void StartActionClothing(AFO Info)
	{
		Clothing component = Info.m_Object.GetComponent<Clothing>();
		m_FarmerClothes.ReadyAdd(component);
	}

	private void EndAddClothing(AFO Info)
	{
		Clothing component = Info.m_Object.GetComponent<Clothing>();
		m_FarmerClothes.Add(component);
		switch (Clothing.GetTypeFromObjectType(component.m_TypeIdentifier))
		{
		case Clothing.Type.Top:
			ModManager.Instance.CheckCustomCallback(ModManager.CallbackTypes.ClothingTopAdded, component.m_TypeIdentifier, m_TileCoord, component.m_UniqueID, m_UniqueID);
			break;
		case Clothing.Type.Hat:
			ModManager.Instance.CheckCustomCallback(ModManager.CallbackTypes.ClothingHatAdded, component.m_TypeIdentifier, m_TileCoord, component.m_UniqueID, m_UniqueID);
			break;
		}
	}

	private ActionType GetActionFromClothing(AFO Info)
	{
		Info.m_StartAction = StartActionClothing;
		Info.m_EndAction = EndAddClothing;
		Info.m_FarmerState = State.Adding;
		if (m_State != 0)
		{
			return ActionType.Fail;
		}
		Holdable holdable = m_FarmerClothes.Get(FarmerClothes.Type.Hat);
		if (holdable != null && holdable.m_TypeIdentifier == Info.m_ObjectType)
		{
			return ActionType.Fail;
		}
		holdable = m_FarmerClothes.Get(FarmerClothes.Type.Top);
		if (holdable != null && holdable.m_TypeIdentifier == Info.m_ObjectType)
		{
			return ActionType.Fail;
		}
		return ActionType.AddResource;
	}

	private void StartDataStoragePrimary(AFO Info)
	{
		m_WorkerIndicator.SetCopying(Copying: true);
	}

	private void EndDataStoragePrimary(AFO Info)
	{
		m_WorkerIndicator.SetCopying(Copying: false);
		if ((bool)m_Engager)
		{
			Farmer component = m_Engager.GetComponent<Farmer>();
			component.SendAction(new ActionInfo(ActionType.DisengageObject, m_TileCoord, null, "", "", AFO.AT.AltSecondary));
			Holdable topObject = component.m_FarmerCarry.GetTopObject();
			if ((bool)topObject && topObject.m_TypeIdentifier == ObjectType.DataStorageCrude)
			{
				topObject.GetComponent<DataStorageCrude>().Copy(m_WorkerInterpreter.m_HighInstructions.m_List, GetWorkerName());
			}
		}
	}

	private void AbortDataStoragePrimary(AFO Info)
	{
		m_WorkerIndicator.SetCopying(Copying: false);
	}

	private ActionType GetActionFromDataStoragePrimary(AFO Info)
	{
		Info.m_StartAction = StartDataStoragePrimary;
		Info.m_EndAction = EndDataStoragePrimary;
		Info.m_AbortAction = AbortDataStoragePrimary;
		Info.m_FarmerState = State.Engaged;
		if (m_WorkerInterpreter.m_HighInstructions == null || m_WorkerInterpreter.m_HighInstructions.m_List == null || m_WorkerInterpreter.m_HighInstructions.m_List.Count == 0)
		{
			return ActionType.Total;
		}
		if (m_State != 0)
		{
			return ActionType.Fail;
		}
		return ActionType.EngageObject;
	}

	private void StartDataStorageSecondary(AFO Info)
	{
		m_WorkerIndicator.SetRestoring(Restoring: true);
	}

	private void EndDataStorageSecondary(AFO Info)
	{
		m_WorkerIndicator.SetRestoring(Restoring: false);
		if (!m_Engager)
		{
			return;
		}
		Farmer component = m_Engager.GetComponent<Farmer>();
		component.SendAction(new ActionInfo(ActionType.DisengageObject, m_TileCoord, null, "", "", AFO.AT.AltSecondary));
		Holdable topObject = component.m_FarmerCarry.GetTopObject();
		if ((bool)topObject && topObject.m_TypeIdentifier == ObjectType.DataStorageCrude)
		{
			m_WorkerInterpreter.SetHighInstructions(topObject.GetComponent<DataStorageCrude>().Paste(), Start: false);
			if (TeachWorkerScriptEdit.Instance.m_CurrentTarget == this)
			{
				TeachWorkerScriptEdit.Instance.UpdateTargetInstructions();
			}
		}
	}

	private void AbortDataStorageSecondary(AFO Info)
	{
		m_WorkerIndicator.SetRestoring(Restoring: false);
	}

	private ActionType GetActionFromDataStorageSecondary(AFO Info)
	{
		Info.m_StartAction = StartDataStorageSecondary;
		Info.m_EndAction = EndDataStorageSecondary;
		Info.m_AbortAction = AbortDataStorageSecondary;
		Info.m_FarmerState = State.Engaged;
		if (m_State != 0)
		{
			return ActionType.Fail;
		}
		if (m_WorkerInterpreter.GetCurrentScript() != null)
		{
			return ActionType.Fail;
		}
		return ActionType.EngageObject;
	}

	public void StartActionNothing(AFO Info)
	{
		Holdable holdable = m_FarmerClothes.Get(FarmerClothes.Type.Hat);
		if (holdable == null)
		{
			holdable = m_FarmerClothes.Get(FarmerClothes.Type.Top);
		}
		m_FarmerClothes.Remove(holdable);
		Info.m_Actioner.GetComponent<Farmer>().m_FarmerCarry.TryAddCarry(holdable);
		switch (Clothing.GetTypeFromObjectType(holdable.m_TypeIdentifier))
		{
		case Clothing.Type.Top:
			ModManager.Instance.CheckCustomCallback(ModManager.CallbackTypes.ClothingTopAdded, holdable.m_TypeIdentifier, m_TileCoord, holdable.m_UniqueID, m_UniqueID);
			break;
		case Clothing.Type.Hat:
			ModManager.Instance.CheckCustomCallback(ModManager.CallbackTypes.ClothingHatAdded, holdable.m_TypeIdentifier, m_TileCoord, holdable.m_UniqueID, m_UniqueID);
			break;
		}
	}

	private void EndAddNothing(AFO Info)
	{
	}

	private void AbortAddNothing(AFO Info)
	{
		Holdable lastObject = Info.m_Actioner.GetComponent<Farmer>().m_FarmerCarry.GetLastObject();
		m_FarmerClothes.Add(lastObject);
	}

	private ActionType GetActionFromNothing(AFO Info)
	{
		Info.m_StartAction = StartActionNothing;
		Info.m_EndAction = EndAddNothing;
		Info.m_AbortAction = AbortAddNothing;
		Info.m_FarmerState = State.Taking;
		if (m_State != 0)
		{
			return ActionType.Fail;
		}
		if (!m_FarmerClothes.Get(FarmerClothes.Type.Hat) && !m_FarmerClothes.Get(FarmerClothes.Type.Top))
		{
			return ActionType.Fail;
		}
		return ActionType.TakeResource;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (Info.m_ActionType == AFO.AT.Primary)
		{
			if (Info.m_RequirementsIn == "" || Info.m_RequirementsIn == "FNOBotRecharge")
			{
				Info.m_FarmerState = State.Recharge;
				Info.m_RequirementsOut = "FNOBotRecharge";
				if (NeedRecharged())
				{
					return ActionType.Recharge;
				}
				if (Info.m_RequirementsIn != "")
				{
					return ActionType.Fail;
				}
				Info.m_RequirementsOut = "";
			}
			if (Info.m_RequirementsIn == "" || Info.m_RequirementsIn == "FNOBotIdle")
			{
				Info.m_FarmerState = State.PickingUp;
				if (m_State == State.Engaged)
				{
					return ActionType.Fail;
				}
				if (m_Energy == 0f)
				{
					return ActionType.Fail;
				}
				Info.m_RequirementsOut = "FNOBotIdle";
				if (m_WorkerInterpreter.GetCurrentScript() == null && !m_Learning)
				{
					return ActionType.Pickup;
				}
				if (Info.m_RequirementsIn != "")
				{
					return ActionType.Fail;
				}
				Info.m_RequirementsOut = "";
			}
		}
		if (Info.m_ActionType == AFO.AT.AltSecondary)
		{
			if (WorkerPart.GetIsTypePart(Info.m_ObjectType))
			{
				return GetActionFromPart(Info);
			}
			if (Clothing.GetIsTypeClothing(Info.m_ObjectType))
			{
				return GetActionFromClothing(Info);
			}
			if (Info.m_ObjectType == ObjectType.DataStorageCrude)
			{
				return GetActionFromDataStorageSecondary(Info);
			}
		}
		if (Info.m_ActionType == AFO.AT.AltPrimary)
		{
			if (Info.m_ObjectType == ObjectType.DataStorageCrude)
			{
				return GetActionFromDataStoragePrimary(Info);
			}
			if (Info.m_ObjectType == ObjectTypeList.m_Total && ((bool)m_FarmerClothes.Get(FarmerClothes.Type.Hat) || (bool)m_FarmerClothes.Get(FarmerClothes.Type.Top)))
			{
				return GetActionFromNothing(Info);
			}
		}
		return base.GetActionFromObject(Info);
	}

	private void UpdateFrameModelPosition()
	{
		if (m_DriveModel != null && m_FrameModel != null)
		{
			Vector3 localPosition = m_DriveModel.transform.Find("FramePoint").localPosition;
			localPosition.Scale(m_DriveModel.transform.localScale);
			m_FrameModel.transform.localPosition = localPosition;
		}
	}

	private void UpdateHeadModelPosition()
	{
		if (m_HeadModel != null && m_DriveModel != null && m_FrameModel != null)
		{
			Vector3 localPosition = m_DriveModel.transform.Find("FramePoint").localPosition;
			localPosition.Scale(m_DriveModel.transform.localScale);
			Vector3 localPosition2 = m_FrameModel.transform.Find("HeadPoint").localPosition;
			localPosition2.Scale(m_FrameModel.transform.localScale);
			m_HeadModel.transform.localPosition = localPosition + localPosition2;
		}
	}

	public void UpdateFrame(ObjectType NewType)
	{
		GameObject frameModel = m_FrameModel;
		if (m_FrameModel != null)
		{
			frameModel.transform.SetParent(null);
		}
		string modelNameFromIdentifier = ObjectTypeList.Instance.GetModelNameFromIdentifier(NewType);
		GameObject modelRoot = m_ModelRoot;
		m_FrameModel = InstantiationManager.Instance.LoadModel(NewType, modelRoot, modelNameFromIdentifier, "Frame");
		m_FarmerCarry.GetNodes();
		m_FrameLevel = 0;
		if (WorkerFrameMk1.GetIsTypeFrameMk1(NewType))
		{
			m_FrameLevel = 1;
		}
		else if (WorkerFrameMk2.GetIsTypeFrameMk2(NewType))
		{
			m_FrameLevel = 2;
		}
		else if (WorkerFrameMk3.GetIsTypeFrameMk3(NewType))
		{
			m_FrameLevel = 3;
		}
		m_FrameInfo = m_AllFrameInfo[m_FrameLevel];
		m_FarmerCarry.SetCapacity(m_FrameInfo.m_CarrySize);
		m_FarmerInventory.SetCapacity(m_FrameInfo.m_InventorySize);
		m_FarmerUpgrades.SetCapacity(m_FrameInfo.m_UpgradeSize);
		if (frameModel != null)
		{
			Object.DestroyImmediate(frameModel.gameObject);
		}
	}

	public void UpdateHead(ObjectType NewType)
	{
		GameObject headModel = m_HeadModel;
		if (m_HeadModel != null)
		{
			m_Light.transform.SetParent(null);
			m_FrontLight.transform.SetParent(null);
			headModel.transform.SetParent(null);
		}
		string modelNameFromIdentifier = ObjectTypeList.Instance.GetModelNameFromIdentifier(NewType);
		GameObject modelRoot = m_ModelRoot;
		m_HeadModel = InstantiationManager.Instance.LoadModel(NewType, modelRoot, modelNameFromIdentifier, "Head");
		m_HeadLevel = 0;
		if (WorkerHeadMk1.GetIsTypeHeadMk1(NewType))
		{
			m_HeadLevel = 1;
		}
		else if (WorkerHeadMk2.GetIsTypeHeadMk2(NewType))
		{
			m_HeadLevel = 2;
		}
		else if (WorkerHeadMk3.GetIsTypeHeadMk3(NewType))
		{
			m_HeadLevel = 3;
		}
		m_HeadInfo = m_AllHeadInfo[m_HeadLevel];
		if (m_WorkerName == "")
		{
			SetSerialNumber();
		}
		SetupLight();
		if (headModel != null)
		{
			Object.DestroyImmediate(headModel.gameObject);
		}
	}

	public void UpdateDrive(ObjectType NewType)
	{
		GameObject driveModel = m_DriveModel;
		if (m_DriveModel != null)
		{
			driveModel.transform.SetParent(null);
		}
		string modelNameFromIdentifier = ObjectTypeList.Instance.GetModelNameFromIdentifier(NewType);
		m_DriveModel = InstantiationManager.Instance.LoadModel(NewType, m_ModelRoot, modelNameFromIdentifier, "Drive");
		Transform transform = ObjectUtils.FindDeepChild(m_DriveModel.transform, "Key");
		if (transform != null)
		{
			m_Key = transform.gameObject;
		}
		m_DriveLevel = 0;
		if (WorkerDriveMk1.GetIsTypeDriveMk1(NewType))
		{
			m_DriveLevel = 1;
		}
		else if (WorkerDriveMk2.GetIsTypeDriveMk2(NewType))
		{
			m_DriveLevel = 2;
		}
		else if (WorkerDriveMk3.GetIsTypeDriveMk3(NewType))
		{
			m_DriveLevel = 3;
		}
		m_DriveInfo = m_AllDriveInfo[m_DriveLevel];
		SetOverallSpeedScale(GetTotalMovementScale());
		if (m_DriveInfo.m_Energy <= 0f)
		{
			m_AllowEnergy = false;
		}
		if (m_Energy != 0f)
		{
			m_Energy = m_DriveInfo.m_Energy;
		}
		if (driveModel != null)
		{
			Object.DestroyImmediate(driveModel.gameObject);
		}
	}

	private void SetupLight()
	{
		if ((bool)m_HeadModel.transform.Find("BotBulb"))
		{
			m_BulbModel = m_HeadModel.transform.Find("BotBulb").GetComponentInChildren<MeshRenderer>();
		}
		Transform parent = ObjectUtils.FindDeepChild(m_HeadModel.transform, "BulbPoint");
		m_Light.transform.SetParent(parent);
		m_Light.transform.localPosition = new Vector3(0f, 0f, 0f);
		parent = ObjectUtils.FindDeepChild(m_HeadModel.transform, "ScreenPoint");
		if ((bool)parent)
		{
			m_FrontLight.gameObject.SetActive(value: true);
			m_FrontLight.transform.SetParent(parent);
			m_FrontLight.transform.localPosition = new Vector3(0f, 0f, 0f);
		}
		else
		{
			m_FrontLight.gameObject.SetActive(value: false);
			m_FrontLight.transform.SetParent(base.transform);
		}
	}

	public void SetFrame(ObjectType NewType)
	{
		ObjectType frame = m_Frame;
		m_Frame = NewType;
		m_ModelDirty = true;
		RecordingManager.Instance.SpecialMessage(this, RecordingStamp.SpecialMessage.SetBotFrame, m_Frame, frame);
	}

	public void SetHead(ObjectType NewType)
	{
		ObjectType head = m_Head;
		m_Head = NewType;
		m_ModelDirty = true;
		RecordingManager.Instance.SpecialMessage(this, RecordingStamp.SpecialMessage.SetBotHead, m_Head, head);
	}

	public void SetDrive(ObjectType NewType)
	{
		ObjectType drive = m_Drive;
		m_Drive = NewType;
		m_ModelDirty = true;
		RecordingManager.Instance.SpecialMessage(this, RecordingStamp.SpecialMessage.SetBotDrive, m_Drive, drive);
	}

	private void SetupCombinedMesh()
	{
		if (!(m_Renderer != null))
		{
			m_Renderer = m_ModelRoot.AddComponent<MeshRenderer>();
			Material[] sharedMaterials = new Material[2]
			{
				MaterialManager.Instance.m_Material,
				MaterialManager.Instance.m_MaterialTrans
			};
			m_Renderer.sharedMaterials = sharedMaterials;
			m_Filter = m_ModelRoot.AddComponent<MeshFilter>();
			m_Filter.mesh = new Mesh();
			int num = m_Renderer.sharedMaterials.Length;
			m_TempMeshes = new Mesh[num];
			for (int i = 0; i < num; i++)
			{
				m_TempMeshes[i] = new Mesh();
			}
		}
	}

	private void CombineModels()
	{
		SetupCombinedMesh();
		int num = m_Renderer.sharedMaterials.Length;
		List<CombineInstance>[] array = new List<CombineInstance>[num];
		int[] array2 = new int[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = new List<CombineInstance>();
			array2[i] = 0;
		}
		List<MeshRenderer> list = new List<MeshRenderer>();
		MeshRenderer[] componentsInChildren = m_ModelRoot.GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer meshRenderer in componentsInChildren)
		{
			if (meshRenderer.gameObject != m_Key && meshRenderer != m_BulbModel && meshRenderer != m_Renderer)
			{
				list.Add(meshRenderer);
			}
		}
		Matrix4x4 inverse = base.transform.localToWorldMatrix.inverse;
		foreach (MeshRenderer item2 in list)
		{
			MeshFilter component = item2.GetComponent<MeshFilter>();
			for (int k = 0; k < item2.sharedMaterials.Length; k++)
			{
				int num2 = 0;
				if (item2.sharedMaterials[k] == MaterialManager.Instance.m_MaterialTrans)
				{
					num2 = 1;
				}
				CombineInstance item = default(CombineInstance);
				item.mesh = component.sharedMesh;
				item.subMeshIndex = k;
				item.transform = inverse * component.transform.localToWorldMatrix;
				array2[num2] += component.sharedMesh.vertexCount;
				array[num2].Add(item);
			}
			item2.enabled = false;
			Object.Destroy(item2);
		}
		CombineInstance[] array3 = new CombineInstance[num];
		for (int l = 0; l < num; l++)
		{
			m_TempMeshes[l].indexFormat = IndexFormat.UInt16;
			m_TempMeshes[l].CombineMeshes(array[l].ToArray());
			array3[l].mesh = m_TempMeshes[l];
			array3[l].subMeshIndex = 0;
			array3[l].transform = Matrix4x4.identity;
		}
		m_Filter.mesh.indexFormat = IndexFormat.UInt16;
		m_Filter.mesh.CombineMeshes(array3, mergeSubMeshes: false);
	}

	public void UpdateModel()
	{
		if (m_ModelDirty)
		{
			m_FarmerCarry.PreRefreshPlayerModel();
			m_FarmerInventory.PreRefreshPlayerModel();
			m_FarmerClothes.PreRefreshPlayerModel();
			m_FarmerUpgrades.PreRefreshPlayerModel();
			UpdateFrame(m_Frame);
			UpdateHead(m_Head);
			UpdateDrive(m_Drive);
			m_FarmerCarry.PreRefreshPlayerModel();
			m_FarmerClothes.PreRefreshPlayerModel();
			m_FarmerUpgrades.PreRefreshPlayerModel();
			UpdateScales();
			CombineModels();
			if ((bool)m_Collider)
			{
				Object.Destroy(m_Collider.gameObject);
			}
			Bounds bounds = ObjectUtils.ObjectBounds(m_ModelRoot);
			GameObject original = (GameObject)Resources.Load("Prefabs/WorldObjects/WorkerParts/WorkerCollision", typeof(GameObject));
			m_Collider = Object.Instantiate(original, base.transform.position, Quaternion.identity, base.transform);
			m_Collider.transform.localScale = bounds.size;
			m_Collider.transform.localPosition = bounds.center - base.transform.position;
			m_Collider.transform.localRotation = Quaternion.identity;
			m_Collider.SetActive(value: false);
			m_FarmerClothes.RefreshParents();
			m_FarmerClothes.SwapPointAxis(FarmerClothes.Type.Top);
			m_FarmerCarry.RefreshPlayerModel();
			m_FarmerClothes.RefreshPlayerModel();
			m_FarmerUpgrades.ModelUpdated();
			m_Animator.Rebind();
			m_WorkerIndicator.UpdateHeight();
			m_ModelDirty = false;
		}
	}

	public void UpdateCulling()
	{
		if ((bool)m_Key)
		{
			m_Key.layer = 13;
		}
	}

	public string GetWorkerName()
	{
		return m_WorkerName;
	}

	public void SetWorkerName(string NewName)
	{
		if (!m_BadgeUsable)
		{
			m_WorkerName = NewName;
		}
		m_WorkerInfoPanel.UpdateName();
		m_WorkerArrow.UpdateName();
	}

	public void SetSerialNumber()
	{
		string text = (m_WorkerName = string.Concat(str1: ((WorldSettings.Instance.CreateWorker(m_HeadLevel) + 1) % 1000).ToString("D3"), str0: m_HeadInfo.m_SerialPrefix));
		if ((bool)m_WorkerInfoPanel)
		{
			m_WorkerInfoPanel.UpdateName();
		}
		m_WorkerArrow.UpdateName();
	}

	private void UpdateSpeed()
	{
		float moveBaseDelay = VariableManager.Instance.GetVariableAsFloat(ObjectType.Worker, "BaseDelay");
		Holdable topObject = m_FarmerCarry.GetTopObject();
		if (topObject != null && topObject.m_Weight > m_FarmerCarry.m_TotalCapacity)
		{
			moveBaseDelay = 1f;
		}
		SetMoveBaseDelay(moveBaseDelay);
	}

	public override bool StartGoTo(TileCoord Destination, Actionable TargetObject = null, bool LessOne = false, int Range = 0)
	{
		if ((bool)m_DustParticles)
		{
			m_DustParticles.Play();
		}
		UpdateSpeed();
		return base.StartGoTo(Destination, TargetObject, LessOne, Range);
	}

	public override void NextGoTo()
	{
		QuestManager.Instance.AddWorkerMove(this);
		CheckNoEnergy();
		if (m_Energy == 0f && !m_Nudge)
		{
			return;
		}
		if (m_Learning)
		{
			bool requestGoToFarmer = m_RequestGoToFarmer;
			if (!m_LearningState.NextGoTo())
			{
				if (requestGoToFarmer)
				{
					m_RequestGoToFarmer = requestGoToFarmer;
					CheckNewRequest();
				}
				return;
			}
		}
		UpdateSpeed();
		base.NextGoTo();
	}

	public override void StopGoTo()
	{
		base.StopGoTo();
		if ((bool)m_DustParticles)
		{
			m_DustParticles.Stop();
		}
	}

	public override void EndGoTo()
	{
		StopGoTo();
		base.EndGoTo();
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		GameStateBase currentState = GameStateManager.Instance.GetCurrentState();
		if ((bool)currentState.GetComponent<GameStateNormal>())
		{
			currentState.GetComponent<GameStateNormal>().CheckTargetPickedUp(this);
		}
		if ((bool)currentState.GetComponent<GameStateInventory>())
		{
			currentState.GetComponent<GameStateInventory>().CheckTargetPickedUp(this);
		}
	}

	public override void SetState(State NewState, bool Abort = false)
	{
		if (NewState == State.None || NewState == State.Listening)
		{
			if (Abort)
			{
				m_WorkerInterpreter.ActionActive(Active: false, Success: false);
			}
			else
			{
				m_WorkerInterpreter.ActionActive(Active: false, Success: true);
			}
		}
		else if (NewState == State.Engaged && SaveLoadManager.Instance.m_Loading && (bool)m_EngagedObject && Vehicle.GetIsTypeVehicle(m_EngagedObject.m_TypeIdentifier))
		{
			m_WorkerInterpreter.ActionActive(Active: false, Success: true);
		}
		else
		{
			m_WorkerInterpreter.ActionActive(Active: true, Success: false);
		}
		base.SetState(NewState, Abort);
	}

	public override void EngagedObjectActionActive(bool Active, bool Success)
	{
		base.EngagedObjectActionActive(Active, Success);
		m_WorkerInterpreter.ActionActive(Active, Success);
	}

	public void AddCarry()
	{
		m_WorkerIndicator.SetNoTool(NoTool: false, State.Total, ObjectTypeList.m_Total);
		m_WorkerInterpreter.CheckRunning();
	}

	public void CarryChanged()
	{
		if ((bool)m_WorkerInfoPanel)
		{
			m_WorkerInfoPanel.UpdateHeldItem();
		}
	}

	public void StartListening()
	{
		m_WorkerIndicator.SetTryingToListen(Listen: false);
		if (m_InterruptState == State.Paused)
		{
			((FarmerStatePaused)m_States[(int)m_InterruptState]).SetListening(Listening: true);
			return;
		}
		m_WorkerInterpreter.Pause(Pause: true);
		m_WorkerArrow.Pause(Paused: true);
		if (m_EngagedObject == null || !Vehicle.GetIsTypeVehicle(m_EngagedObject.m_TypeIdentifier))
		{
			SetState(State.Listening);
		}
		else
		{
			m_States[24].StartState();
		}
	}

	public void StopListening()
	{
		if (m_InterruptState == State.Paused)
		{
			((FarmerStatePaused)m_States[(int)m_InterruptState]).SetListening(Listening: false);
			return;
		}
		m_WorkerInterpreter.Pause(Pause: false);
		m_WorkerArrow.Pause(Paused: false);
		if (m_EngagedObject == null || !Vehicle.GetIsTypeVehicle(m_EngagedObject.m_TypeIdentifier))
		{
			SetState(State.None);
		}
		else
		{
			m_States[24].EndState();
		}
	}

	public bool GetIsListening()
	{
		if (m_State == State.Listening)
		{
			return true;
		}
		if (m_InterruptState == State.Paused)
		{
			return true;
		}
		if (m_State == State.Engaged && (bool)m_EngagedObject && Vehicle.GetIsTypeVehicle(m_EngagedObject.m_TypeIdentifier) && m_EngagedObject.GetComponent<Vehicle>().m_State == Vehicle.State.Listening)
		{
			return true;
		}
		return false;
	}

	public void RequestPause()
	{
		if (m_Energy > 0f)
		{
			if (m_InterruptState == State.Paused)
			{
				StartListening();
				return;
			}
			if (m_State == State.None && (m_WorkerInterpreter.GetCurrentScript() == null || m_WorkerInterpreter.m_InstructionFailed))
			{
				StartListening();
				return;
			}
			if (m_State == State.Engaged && Vehicle.GetIsTypeVehicle(m_EngagedObject.m_TypeIdentifier))
			{
				m_EngagedObject.GetComponent<Vehicle>().RequestPause();
				return;
			}
			m_ListenRequested = true;
			m_WorkerIndicator.SetTryingToListen(Listen: true);
		}
	}

	public void Unpause()
	{
		if (m_ListenRequested)
		{
			m_ListenRequested = false;
			m_WorkerIndicator.SetTryingToListen(Listen: false);
		}
		else if (m_State == State.Listening || m_InterruptState == State.Paused)
		{
			StopListening();
		}
		else if (m_State == State.Engaged && Vehicle.GetIsTypeVehicle(m_EngagedObject.m_TypeIdentifier))
		{
			m_EngagedObject.GetComponent<Vehicle>().Unpause();
		}
	}

	public void CheckRequestPause()
	{
		if (m_ListenRequested && m_State == State.None)
		{
			m_ListenRequested = false;
			m_WorkerIndicator.SetTryingToListen(Listen: false);
			StartListening();
		}
		else if (m_State == State.Engaged && (bool)m_EngagedObject && Vehicle.GetIsTypeVehicle(m_EngagedObject.m_TypeIdentifier))
		{
			m_EngagedObject.GetComponent<Vehicle>().CheckRequestPause();
		}
	}

	public void Recharge()
	{
		m_WorkerIndicator.SetNoEnergy(NoEnergy: false);
		m_WorkerIndicator.SetLowEnergy(LowEnergy: false);
		m_WorkerIndicator.SetState(WorkerStatusIndicator.State.Off);
		AddEnergy(m_DriveInfo.m_Energy);
		m_WorkerInterpreter.CheckRunning();
		StartWorkingSound();
	}

	public void SetLayer(Layers NewLayer)
	{
		InstantiationManager.Instance.SetLayer(base.gameObject, NewLayer);
		UpdateCulling();
	}

	public void AddFuel(ObjectType FuelType)
	{
		m_WorkerIndicator.SetNoEnergy(NoEnergy: false);
		m_WorkerIndicator.SetLowEnergy(LowEnergy: false);
		m_WorkerIndicator.SetState(WorkerStatusIndicator.State.Off);
		float fuelEnergy = BurnableFuel.GetFuelEnergy(FuelType);
		AddEnergy(fuelEnergy);
		SetLayer(Layers.Workers);
	}

	public void Selected(bool Select)
	{
		m_WorkerArrow.SetSelected(Select);
	}

	public void RequestStopLearning()
	{
		m_LearningState.RequestStopLearning();
	}

	public int GetFreeMemory()
	{
		int totalMaxInstuctions = GetTotalMaxInstuctions();
		int instructionCount = HighInstruction.GetInstructionCount(m_WorkerInterpreter.m_HighInstructions.m_List);
		return totalMaxInstuctions - instructionCount;
	}

	public int GetTotalMaxInstuctions()
	{
		return m_HeadInfo.m_MaxInstructions + m_ExtraMemorySize;
	}

	public int GetTotalSearchRange()
	{
		return m_HeadInfo.m_FindNearestRange + m_ExtraSearchRange;
	}

	public int GetTotalSearchDelay()
	{
		return m_HeadInfo.m_FindNearestDelay - m_ExtraSearchDelay;
	}

	public int GetTotalMovementDelay()
	{
		return m_DriveInfo.m_MoveInitialDelay - m_ExtraMovementDelay;
	}

	public float GetTotalMovementScale()
	{
		return m_DriveInfo.m_SpeedScale + m_ExtraMovementScale;
	}

	private void UpdateHeadScale()
	{
		if (m_HeadModel != null)
		{
			float num = m_HeadInfo.m_Scale;
			if ((m_ExtraMemorySize > 0 || m_ExtraSearchRange > 0) && num == 1f)
			{
				num = 1.35f;
			}
			m_HeadModel.transform.localScale = new Vector3(num, num, num);
		}
	}

	private void UpdateFrameScale()
	{
		if (m_FrameModel != null)
		{
			float num = m_FrameInfo.m_Scale;
			if (m_FarmerCarry.m_ExtraCapacity > 0 && num == 1f)
			{
				num = 1.25f;
			}
			m_FrameModel.transform.localScale = new Vector3(num, num, num);
		}
	}

	private void UpdateDriveScale()
	{
		if (m_DriveModel != null)
		{
			float num = m_DriveInfo.m_Scale;
			if ((m_ExtraMovementDelay > 0 || m_ExtraEnergy > 0f) && num == 1f)
			{
				num = 1.25f;
			}
			m_DriveModel.transform.localScale = new Vector3(num, num, num);
		}
	}

	private void UpdateScales()
	{
		UpdateHeadScale();
		UpdateFrameScale();
		UpdateDriveScale();
		UpdateFrameModelPosition();
		UpdateHeadModelPosition();
		m_FarmerCarry.UpdateScales();
		m_FarmerUpgrades.UpdateScales();
	}

	public void SetExtraMemory(int Size)
	{
		m_ExtraMemorySize = Size;
		m_WorkerInterpreter.UpdateMemorySize();
		m_ModelDirty = true;
	}

	public void SetExtraSearch(int Range, int Delay)
	{
		m_ExtraSearchRange = Range;
		m_ExtraSearchDelay = Delay;
		m_ModelDirty = true;
	}

	public void SetExtraMovement(int Delay, float Scale)
	{
		m_ExtraMovementDelay = Delay;
		m_ExtraMovementScale = Scale;
		SetOverallSpeedScale(GetTotalMovementScale());
		m_ModelDirty = true;
	}

	public void SetExtraEnergy(float Energy)
	{
		m_ExtraEnergy = Energy;
		m_ModelDirty = true;
	}

	public void SetCarryCapacity()
	{
		m_ModelDirty = true;
	}

	private void UpdateParticlePosition()
	{
		if ((bool)m_DustParticles)
		{
			m_DustParticles.transform.position = base.transform.position;
		}
	}

	public void IndicatorStateChanged()
	{
		m_WorkerInfoPanel.UpdateStatusImage();
	}

	public void InterpreterScriptChanged()
	{
		if (m_WorkerInterpreter.GetCurrentScript() == null && m_State == State.Engaged && ObjectTypeList.Instance.GetIsBuilding(m_EngagedObject.m_TypeIdentifier))
		{
			SendAction(new ActionInfo(ActionType.DisengageObject, new TileCoord(0, 0)));
		}
		m_WorkerInfoPanel.UpdateStatusImage();
	}

	private void UpdateLightColour(bool Enabled)
	{
		int num = 0;
		if (m_WorkerInterpreter.GetCurrentScript() != null)
		{
			num = 1;
		}
		if (m_WorkerIndicator.m_State == WorkerStatusIndicator.State.NoEnergy || m_WorkerIndicator.m_State == WorkerStatusIndicator.State.NoTool)
		{
			num = 2;
		}
		m_Light.color = MaterialManager.m_BotBulbColours[num];
		if ((bool)m_BulbModel)
		{
			num *= 2;
			if (!Enabled)
			{
				num++;
			}
			m_BulbModel.sharedMaterial = MaterialManager.Instance.m_BotBulbMaterials[num];
		}
	}

	private void UpdateLight()
	{
		if (m_State == State.Held)
		{
			m_Light.enabled = false;
			return;
		}
		int num = 60;
		if (m_Learning || m_WorkerIndicator.m_State == WorkerStatusIndicator.State.NoEnergy || m_State == State.BeingEngaged)
		{
			num = 10;
		}
		m_LightTimer += TimeManager.Instance.m_NormalDelta;
		bool flag = true;
		if (SettingsManager.Instance.m_FlashiesEnabled)
		{
			flag = (((int)(m_LightTimer * 60f) % num < num / 2) ? true : false);
		}
		if (flag == m_LightEnabled && SettingsManager.Instance.m_FlashiesEnabled)
		{
			return;
		}
		m_LightEnabled = flag;
		if ((CameraManager.Instance.m_Camera.transform.position - m_Light.transform.position).magnitude > 75f)
		{
			m_Light.enabled = false;
			m_FrontLight.enabled = false;
		}
		else
		{
			m_Light.enabled = flag;
			m_FrontLight.enabled = true;
		}
		if (flag)
		{
			float num2 = m_NightIntensity + DayNightManager.Instance.m_LightAdjuster * (m_DayIntensity - m_NightIntensity);
			if (m_Learning)
			{
				num2 += m_LearnIntensityBoost;
			}
			if (m_WorkerIndicator.m_State == WorkerStatusIndicator.State.NoEnergy || m_WorkerIndicator.m_State == WorkerStatusIndicator.State.NoTool)
			{
				num2 += m_ErrorIntensityBoost;
			}
			if (m_LightIntensity != num2)
			{
				m_LightIntensity = num2;
				m_Light.intensity = num2;
			}
			float num3 = m_NightRange + (1f - DayNightManager.Instance.m_LightAdjuster) * (m_DayRange - m_NightRange);
			if (m_LightRange != num3)
			{
				m_LightRange = num3;
				m_Light.range = num3;
			}
		}
		UpdateLightColour(flag);
	}

	public void Nudge(TileCoord NewPosition, TileCoord Direction)
	{
		TileManager.Instance.GetTileWalkable(NewPosition, out var WorkerWalk, out var AnimalWalk, out var BoatWalk, out var PlayerWalk, TestFarmer: false, out var WalkCost);
		if (!WorkerWalk)
		{
			return;
		}
		Plot plotAtTile = PlotManager.Instance.GetPlotAtTile(NewPosition);
		if (!plotAtTile.m_ObjectDictionary.ContainsKey(ObjectType.Worker))
		{
			return;
		}
		foreach (TileCoordObject item in plotAtTile.m_ObjectDictionary[ObjectType.Worker])
		{
			Worker component = item.GetComponent<Worker>();
			if (component != null && component != this && component.m_TileCoord == NewPosition && !component.m_Nudge && component.m_State == State.None && component.m_WorkerInterpreter.GetCurrentScript() == null)
			{
				TileCoord tileCoord = NewPosition + Direction;
				TileManager.Instance.GetTileWalkable(tileCoord, out WorkerWalk, out AnimalWalk, out BoatWalk, out PlayerWalk, TestFarmer: false, out WalkCost);
				if (WorkerWalk)
				{
					component.Nudge(tileCoord, Direction);
					component.m_Nudge = true;
					component.SendAction(new ActionInfo(ActionType.MoveTo, tileCoord));
				}
			}
		}
	}

	private void UpdateKey()
	{
		if ((bool)m_Key)
		{
			Quaternion localRotation = m_Key.transform.localRotation;
			localRotation *= Quaternion.Euler(0f, 720f * TimeManager.Instance.m_NormalDelta, 0f);
			m_Key.transform.localRotation = localRotation;
		}
	}

	private void UpdateWorkingSound()
	{
		if (m_WorkingSound == null && !EngagedWithPoweredVehicle())
		{
			string workingSoundName = GetComponent<Worker>().m_FrameInfo.m_WorkingSoundName;
			m_WorkingSound = AudioManager.Instance.StartEvent(workingSoundName, this, Remember: true, Follow: true);
		}
	}

	private void StartWorkingSound()
	{
		if (m_States[(int)m_State] != null)
		{
			m_States[(int)m_State].StartWorking();
		}
	}

	private void StopWorkingSound()
	{
		if (m_States[(int)m_State] != null)
		{
			m_States[(int)m_State].StopWorking();
		}
		if (m_WorkingSound != null)
		{
			AudioManager.Instance.StopEvent(m_WorkingSound);
			m_WorkingSound = null;
		}
	}

	public void CreateColliders()
	{
		m_Collider.SetActive(value: true);
		MeshCollider[] componentsInChildren = base.transform.GetComponentsInChildren<MeshCollider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = false;
		}
	}

	public void DestroyColliders()
	{
		m_Collider.SetActive(value: false);
		MeshCollider[] componentsInChildren = base.transform.GetComponentsInChildren<MeshCollider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = true;
		}
	}

	public void SetOccluded(bool Occluded)
	{
		Material[] sharedMaterials = new Material[2]
		{
			MaterialManager.Instance.m_MaterialOccluded,
			MaterialManager.Instance.m_MaterialTransOccluded
		};
		m_Renderer.sharedMaterials = sharedMaterials;
		if ((bool)m_BulbModel)
		{
			m_BulbModel.material = MaterialManager.Instance.m_MaterialOccluded;
		}
		if ((bool)m_Key)
		{
			m_Key.GetComponent<MeshRenderer>().sharedMaterial = MaterialManager.Instance.m_MaterialOccluded;
		}
	}

	public void RestoreStandardMaterials()
	{
		Material[] sharedMaterials = new Material[2]
		{
			MaterialManager.Instance.m_Material,
			MaterialManager.Instance.m_MaterialTrans
		};
		m_Renderer.sharedMaterials = sharedMaterials;
		if ((bool)m_Key)
		{
			m_Key.GetComponent<MeshRenderer>().sharedMaterial = MaterialManager.Instance.m_Material;
		}
		SetupLight();
	}

	public void GoToPlayer(FarmerPlayer NewPlayer)
	{
		TileCoord tileCoord = NewPlayer.m_TileCoord;
		if (m_State == State.Engaged && (bool)m_EngagedObject && Vehicle.GetIsTypeVehicle(m_EngagedObject.m_TypeIdentifier))
		{
			m_EngagedObject.GetComponent<Vehicle>().RequestGoTo(tileCoord);
		}
		else
		{
			RequestGoTo(tileCoord);
		}
	}

	public void TogglePauseScript()
	{
		if (m_WorkerInterpreter.GetCurrentScript() != null && (m_State != State.Engaged || !Vehicle.GetIsTypeVehicle(m_EngagedObject.m_TypeIdentifier)))
		{
			if (m_InterruptState == State.Total)
			{
				SetInterruptState(State.Paused);
			}
			else if (m_InterruptState == State.Paused)
			{
				SetInterruptState(State.Total);
			}
		}
	}

	public void ToggleCelebrate()
	{
		if (m_State != State.Engaged || !Vehicle.GetIsTypeVehicle(m_EngagedObject.m_TypeIdentifier))
		{
			if (m_InterruptState == State.Total)
			{
				SetInterruptState(State.CelebrateEraComplete);
			}
			else if (m_InterruptState == State.CelebrateEraComplete)
			{
				SetInterruptState(State.Total);
			}
		}
	}

	public bool CheckTrackBuilding(Building NewBuilding)
	{
		return m_WorkerInterpreter.CheckTrackBuilding(NewBuilding);
	}

	private bool EngagedWithPoweredVehicle()
	{
		if (m_EngagedObject == null)
		{
			return false;
		}
		if (!Vehicle.GetIsTypeVehicle(m_EngagedObject.m_TypeIdentifier))
		{
			return false;
		}
		return m_EngagedObject.GetComponent<Vehicle>().GetIsPowered();
	}

	private bool GetUseEnergy()
	{
		if (m_LoseEnergy && !WorkerDriveMk3.GetIsTypeDriveMk3(m_Drive) && !EngagedWithPoweredVehicle())
		{
			return true;
		}
		return false;
	}

	protected override void Update()
	{
		if ((bool)WorkerScriptManager.Instance && m_WorkerName == WorkerScriptManager.Instance.m_BotBreakName)
		{
			m_WorkerName = m_WorkerName;
		}
		UpdateModel();
		bool flag = false;
		if (m_InterruptState == State.Total && TimeManager.Instance.m_NormalTimeEnabled)
		{
			if ((m_Energy > 0f || m_AllowEnergy) && GetIsDoingSomething() && m_State != State.Held && m_State != State.Upgraded && m_State != State.BeingEngaged && !m_Learning && !GetIsListening())
			{
				if (m_LoseEnergy && m_AllowEnergy && GetUseEnergy())
				{
					m_Energy -= (m_EnergyLossPerSecond - m_ExtraEnergy) * TimeManager.Instance.m_NormalDelta;
				}
				CheckNoEnergy();
				ScaleSpeedToEnergy();
				if ((m_State == State.Adding || m_State == State.Taking) && m_Energy == 0f)
				{
					m_Energy = 0.0001f;
					flag = true;
				}
				if (m_Energy > 0f)
				{
					UpdateKey();
				}
				if (m_Energy == 0f)
				{
					StopWorkingSound();
				}
				else
				{
					UpdateWorkingSound();
				}
				if (m_Energy == 0f)
				{
					m_WorkerIndicator.SetNoEnergy(NoEnergy: true);
					m_WorkerIndicator.SetLowEnergy(LowEnergy: false);
				}
				else if (m_ActionSpeedScale != 1f)
				{
					m_WorkerIndicator.SetNoEnergy(NoEnergy: false);
					m_WorkerIndicator.SetLowEnergy(LowEnergy: true);
				}
				else
				{
					m_WorkerIndicator.SetNoEnergy(NoEnergy: false);
					m_WorkerIndicator.SetLowEnergy(LowEnergy: false);
				}
			}
			else
			{
				if (m_Nudge)
				{
					SetActionSpeedScale(1f);
				}
				else
				{
					ScaleSpeedToEnergy();
				}
				StopWorkingSound();
			}
			if (m_Energy > 0f && m_WorkerInterpreter.GetCurrentScript() != null)
			{
				m_WorkerInterpreter.UpdateScript();
				if (m_WorkerInterpreter.GetCurrentScript() != null && m_WorkerInterpreter.m_InstructionProcessing)
				{
					WorkerInstruction.Instruction instruction = m_WorkerInterpreter.GetWorkerInstruction().m_Instruction;
					if (instruction == WorkerInstruction.Instruction.FindNearestObject || instruction == WorkerInstruction.Instruction.FindNearestTile)
					{
						if (m_WorkerIndicator.m_State != WorkerStatusIndicator.State.Question)
						{
							m_WorkerIndicator.SetQuestion(Question: true);
							m_WorkerIndicator.SetBusy(Busy: false);
							m_WorkerInterpreter.CheckRunning();
						}
					}
					else if (m_WorkerIndicator.m_State != WorkerStatusIndicator.State.Busy)
					{
						m_WorkerIndicator.SetQuestion(Question: false);
						m_WorkerIndicator.SetBusy(Busy: true);
						m_WorkerInterpreter.CheckRunning();
					}
				}
				else if (m_WorkerIndicator.m_State == WorkerStatusIndicator.State.Question)
				{
					m_WorkerIndicator.SetQuestion(Question: false);
					m_WorkerInterpreter.CheckRunning();
				}
				else if (m_WorkerIndicator.m_State == WorkerStatusIndicator.State.Busy)
				{
					m_WorkerIndicator.SetBusy(Busy: false);
					m_WorkerInterpreter.CheckRunning();
				}
			}
		}
		else
		{
			StopWorkingSound();
		}
		if (m_WorkerIndicator.m_State != 0)
		{
			m_WorkerIndicator.UpdateIndicator();
		}
		if (m_WorkerArrow.m_Visible)
		{
			m_WorkerArrow.UpdateIndicator();
		}
		UpdateParticlePosition();
		UpdateLight();
		base.Update();
		if (flag && m_State == State.None)
		{
			m_Energy = 0f;
			m_WorkerIndicator.SetNoEnergy(NoEnergy: true);
			m_WorkerIndicator.SetLowEnergy(LowEnergy: false);
		}
		if (TimeManager.Instance.m_NormalTimeEnabled && m_Energy > 0f && m_InterruptState != State.Paused)
		{
			m_Animator.speed = TimeManager.Instance.m_TimeScale * m_ActionSpeedScale * m_OverallSpeedScale;
		}
		else
		{
			m_Animator.speed = 0f;
		}
	}
}
