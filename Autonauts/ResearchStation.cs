using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class ResearchStation : Building
{
	public enum State
	{
		Idle,
		Researching,
		Cancelling,
		Total
	}

	public static float m_ResearchModuleDelay = 1f;

	protected float m_CancelDelay = 0.25f;

	[HideInInspector]
	public State m_State;

	private State m_TempState;

	public float m_StateTimer;

	[HideInInspector]
	public Holdable m_CurrentResearchObject;

	[HideInInspector]
	public Quest.ID m_CurrentResearchQuest;

	private int m_CurrentResearchLevel;

	[HideInInspector]
	public float m_ResearchProgress;

	protected bool m_DisplayIngredients;

	protected Transform m_IngredientsRoot;

	protected static float m_IngredientSpacing = 0.5f;

	private Actionable m_ObjectInvolved;

	[HideInInspector]
	public List<Quest.ID> m_Quests;

	protected GameObject m_BlueprintModel;

	private int m_HeartValue;

	protected Animator m_Animator;

	public List<GameObject> m_UpgradeModels;

	public int m_CurrentModule;

	public int m_NumModules;

	private ResearchStationAnimControl m_AnimControl;

	private Transform m_HeartPoint;

	public Actionable m_Heart;

	private GameObject m_Vessel;

	public static bool GetIsTypeResearchStation(ObjectType NewType)
	{
		if (NewType == ObjectType.ResearchStationCrude || NewType == ObjectType.ResearchStationCrude2 || NewType == ObjectType.ResearchStationCrude3 || NewType == ObjectType.ResearchStationCrude4 || NewType == ObjectType.ResearchStationCrude5 || NewType == ObjectType.ResearchStationCrude6)
		{
			return true;
		}
		return false;
	}

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("ResearchStation", m_TypeIdentifier);
	}

	public override void CheckAddCollectable(bool FromLoad)
	{
		base.CheckAddCollectable(FromLoad);
		if (!ObjectTypeList.m_Loading || FromLoad)
		{
			CollectionManager.Instance.AddCollectable("ResearchStation", this);
		}
	}

	public override void Restart()
	{
		base.Restart();
		m_Quests = new List<Quest.ID>();
		if ((bool)QuestManager.Instance)
		{
			for (int i = 0; i < m_NumModules; i++)
			{
				foreach (ResearchInfo researchInfo in QuestManager.Instance.m_Data.m_ResearchData.GetLevelInfo(i).m_ResearchInfos)
				{
					m_Quests.Add(researchInfo.m_ID);
				}
			}
		}
		m_CurrentResearchObject = null;
		SetCurrentResearchQuest(Quest.ID.Total);
		m_TempState = State.Idle;
		m_DisplayIngredients = false;
		m_AnimControl.Restart();
		m_State = State.Total;
		SetState(State.Idle);
	}

	protected virtual void GetModelParts()
	{
		m_Animator = GetComponent<Animator>();
		m_Animator.Rebind();
		m_AnimControl = GetComponent<ResearchStationAnimControl>();
		m_AnimControl.GetModelParts();
		m_HeartPoint = m_UpgradeModels[0].transform.Find("HeartPoint");
		m_IngredientsRoot = m_UpgradeModels[0].transform.Find("SamplePlate").Find("IngredientsPoint");
		m_IngredientsRoot.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
		m_BlueprintModel = m_UpgradeModels[0].transform.Find("SamplePlate").Find("Blueprint").gameObject;
	}

	public override void PostCreate()
	{
		base.PostCreate();
		ChangeAccessPointToIn();
		m_NumModules = 1;
		m_UpgradeModels = new List<GameObject>();
		m_UpgradeModels.Add(m_ModelRoot.transform.Find("BaseApparatus").gameObject);
		GetModelParts();
		m_Vessel = ModelManager.Instance.Instantiate(ObjectTypeList.m_Total, "Models/Special/ResearchVessel", m_IngredientsRoot.transform, RandomVariants: false);
		m_Vessel.transform.localPosition = default(Vector3);
		m_Vessel.gameObject.SetActive(value: false);
		for (int i = 1; i < m_UpgradeModels.Count; i++)
		{
			GameObject gameObject = m_UpgradeModels[i];
			if ((bool)gameObject)
			{
				gameObject.gameObject.SetActive(value: false);
			}
		}
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		if ((bool)m_CurrentResearchObject)
		{
			CreateCancelledItem();
		}
		base.StopUsing(AndDestroy);
	}

	public new void Awake()
	{
		base.Awake();
	}

	protected override void TileCoordChanged(TileCoord Position)
	{
		base.TileCoordChanged(Position);
	}

	public override void UpdateTiles()
	{
		base.UpdateTiles();
	}

	protected bool IsBusy()
	{
		if (m_State != 0)
		{
			return true;
		}
		return false;
	}

	public override bool IsSelectable()
	{
		return !IsBusy();
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		JSONUtils.Set(Node, "State", (int)m_State);
		Quest quest = QuestManager.Instance.GetQuest(m_CurrentResearchQuest);
		if (quest != null)
		{
			JSONUtils.Set(Node, "ResearchQuest", quest.m_Title);
		}
		if (m_CurrentResearchObject != null)
		{
			JSONNode jSONNode = new JSONObject();
			string saveNameFromIdentifier = ObjectTypeList.Instance.GetSaveNameFromIdentifier(m_CurrentResearchObject.GetComponent<BaseClass>().m_TypeIdentifier);
			JSONUtils.Set(jSONNode, "ID", saveNameFromIdentifier);
			m_CurrentResearchObject.GetComponent<Savable>().Save(jSONNode);
			Node["CurrentObject"] = jSONNode;
		}
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		m_TempState = (State)JSONUtils.GetAsInt(Node, "State", 0);
		JSONNode jSONNode = Node["ResearchQuest"];
		if (jSONNode != null && !jSONNode.IsNull)
		{
			Quest quest = QuestManager.Instance.GetQuest(jSONNode);
			if (quest != null)
			{
				SetCurrentResearchQuest(quest.m_ID);
			}
		}
		JSONNode jSONNode2 = Node["CurrentObject"];
		if (jSONNode2 != null && !jSONNode2.IsNull)
		{
			JSONNode jSONNode3 = jSONNode2;
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromSaveName(jSONNode3, new Vector3(0f, 0f, 0f), Quaternion.identity);
			if ((bool)baseClass)
			{
				baseClass.GetComponent<Savable>().Load(jSONNode3);
				SetResearchObject(baseClass.GetComponent<Holdable>());
				baseClass.GetComponent<Savable>().SetIsSavable(IsSavable: false);
				baseClass.GetComponent<Actionable>().SendAction(new ActionInfo(ActionType.BeingHeld, default(TileCoord), this));
			}
		}
	}

	private void SetResearchObject(Holdable NewObject, bool UpdateAnim = true)
	{
		m_CurrentResearchObject = NewObject;
		if ((bool)m_CurrentResearchObject)
		{
			if ((bool)m_IngredientsRoot)
			{
				m_CurrentResearchObject.transform.SetParent(m_IngredientsRoot);
				m_CurrentResearchObject.transform.localPosition = new Vector3(0f, ResearchStationAnimControl.m_SamplePlateDepth, 0f);
			}
			if (ToolFillable.GetIsTypeLiquid(m_CurrentResearchObject.m_TypeIdentifier))
			{
				m_Vessel.gameObject.SetActive(value: true);
			}
			else
			{
				m_Vessel.gameObject.SetActive(value: false);
			}
		}
		else
		{
			m_Vessel.gameObject.SetActive(value: false);
		}
		if (UpdateAnim)
		{
			m_AnimControl.UpdateSample();
		}
	}

	public override void SendAction(ActionInfo Info)
	{
		if ((bool)m_Engager && Info.m_Action != ActionType.Disengaged && Info.m_Action != ActionType.SetValue && Info.m_Action != ActionType.Refresh)
		{
			return;
		}
		switch (Info.m_Action)
		{
		case ActionType.Engaged:
			if (!IsBusy())
			{
				m_Engager = Info.m_Object;
				if ((bool)m_Engager.GetComponent<FarmerPlayer>())
				{
					GameStateManager.Instance.StartSelectBuilding(this);
				}
			}
			break;
		case ActionType.Disengaged:
			m_Engager = null;
			break;
		case ActionType.Refresh:
		case ActionType.RefreshFirst:
			SetState(m_TempState);
			break;
		}
	}

	public override object GetActionInfo(GetActionInfo Info)
	{
		GetAction action = Info.m_Action;
		if ((uint)(action - 3) <= 2u)
		{
			return m_State == State.Idle;
		}
		return base.GetActionInfo(Info);
	}

	public override bool CanDoAction(ActionInfo Info, bool RightNow)
	{
		if (IsBusy() && RightNow)
		{
			return false;
		}
		if ((bool)m_Engager && Info.m_Action != ActionType.Disengaged && Info.m_Action != ActionType.SetValue)
		{
			return false;
		}
		switch (Info.m_Action)
		{
		case ActionType.Engaged:
			if (m_CurrentResearchObject == null)
			{
				return false;
			}
			return true;
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

	private void StartAddFolkHeart(AFO Info)
	{
		Info.m_Object.transform.position = m_HeartPoint.position;
	}

	private void EndAddFolkHeart(AFO Info)
	{
		Actionable actionable = (m_Heart = Info.m_Object);
		m_Heart.SendAction(new ActionInfo(ActionType.BeingHeld, default(TileCoord), this));
		AudioManager.Instance.StartEvent("BuildingIngredientAdd", this);
		m_ObjectInvolved = Info.m_Actioner;
		m_HeartValue = VariableManager.Instance.GetVariableAsInt(actionable.m_TypeIdentifier, "Value");
		SetState(State.Researching);
	}

	private ActionType GetActionFromFolkHeart(AFO Info)
	{
		Info.m_StartAction = StartAddFolkHeart;
		Info.m_EndAction = EndAddFolkHeart;
		Info.m_FarmerState = Farmer.State.Adding;
		if (m_CurrentResearchQuest == Quest.ID.Total)
		{
			return ActionType.Fail;
		}
		if (m_CurrentResearchObject == null)
		{
			return ActionType.Fail;
		}
		if ((bool)m_Engager)
		{
			return ActionType.Total;
		}
		if (m_State != 0)
		{
			return ActionType.Total;
		}
		return ActionType.AddResource;
	}

	private void StartAddAnything(AFO Info)
	{
		Actionable @object = Info.m_Object;
		if ((bool)@object.GetComponent<ToolFillable>())
		{
			ObjectType heldType = @object.GetComponent<ToolFillable>().m_HeldType;
			SetResearchObject(ObjectTypeList.Instance.CreateObjectFromIdentifier(heldType, default(Vector3), Quaternion.identity).GetComponent<Holdable>(), UpdateAnim: false);
			@object.GetComponent<ToolFillable>().Empty(1);
		}
		else
		{
			SetResearchObject(@object.GetComponent<Holdable>(), UpdateAnim: false);
		}
	}

	private void AbortAddAnything(AFO Info)
	{
		SetResearchObject(null);
	}

	private void EndAddAnything(AFO Info)
	{
		m_CurrentResearchObject.SendAction(new ActionInfo(ActionType.BeingHeld, default(TileCoord)));
		m_CurrentResearchObject.transform.localPosition = default(Vector3);
		AudioManager.Instance.StartEvent("BuildingResearchSampleAdded", this);
		m_AnimControl.UpdateSample();
	}

	protected override ActionType GetActionFromAnything(AFO Info)
	{
		Info.m_StartAction = StartAddAnything;
		Info.m_AbortAction = AbortAddAnything;
		Info.m_EndAction = EndAddAnything;
		Info.m_FarmerState = Farmer.State.Adding;
		if (m_CurrentResearchQuest == Quest.ID.Total)
		{
			return ActionType.Fail;
		}
		if ((bool)m_Engager)
		{
			return ActionType.Fail;
		}
		if (m_State != 0)
		{
			return ActionType.Fail;
		}
		if (m_CurrentResearchObject != null)
		{
			return ActionType.Fail;
		}
		ObjectType objectType = ObjectTypeList.m_Total;
		if ((bool)Info.m_Object)
		{
			objectType = Info.m_Object.m_TypeIdentifier;
		}
		if (ToolFillable.GetIsTypeFillable(objectType))
		{
			objectType = Info.m_Object.GetComponent<ToolFillable>().m_HeldType;
		}
		if (QuestManager.Instance.GetQuest(m_CurrentResearchQuest).m_ObjectTypeRequired != objectType)
		{
			return ActionType.Fail;
		}
		return ActionType.AddResource;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		ObjectType objectType = Info.m_ObjectType;
		if (Info.m_ActionType == AFO.AT.Primary)
		{
			Info.m_FarmerState = Farmer.State.Engaged;
			if (m_State != 0)
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
			if (FolkHeart.GetIsFolkHeart(objectType))
			{
				return GetActionFromFolkHeart(Info);
			}
			if (objectType != ObjectTypeList.m_Total)
			{
				return GetActionFromAnything(Info);
			}
		}
		return ActionType.Total;
	}

	protected virtual void SetState(State NewState)
	{
		if (m_State == NewState)
		{
			return;
		}
		State state = m_State;
		if (state == State.Researching)
		{
			StopResearching();
			if (QuestManager.Instance.DoResearch(m_CurrentResearchObject, m_CurrentResearchQuest, m_ObjectInvolved, m_HeartValue))
			{
				EndResearching();
			}
			if ((bool)m_ObjectInvolved)
			{
				m_ObjectInvolved.SendAction(new ActionInfo(ActionType.DisengageObject, default(TileCoord), this));
			}
			if ((bool)m_Heart)
			{
				m_Heart.StopUsing();
			}
		}
		m_State = NewState;
		m_StateTimer = 0f;
		state = m_State;
		if (state == State.Researching)
		{
			StartResearching();
			if ((bool)m_ObjectInvolved)
			{
				m_ObjectInvolved.SendAction(new ActionInfo(ActionType.EngageObject, default(TileCoord), this));
			}
		}
		m_AnimControl.UpdateState();
		ModManager.Instance.CheckBuildingStateChangedCallback(m_UniqueID, m_State.ToString());
	}

	public void ResumeResearch(Actionable ObjectInvolved)
	{
		m_ObjectInvolved = ObjectInvolved;
	}

	protected void EnableModule(int Module)
	{
		m_UpgradeModels[Module].SetActive(value: true);
		m_NumModules++;
	}

	private void StartResearching()
	{
		m_CurrentModule = 0;
		StartModuleAnimation(m_CurrentModule);
	}

	private void StopResearching()
	{
		m_Animator.Play("Idle", 0, 0f);
	}

	public virtual void DoModuleAction()
	{
	}

	protected virtual void StartModuleAnimation(int Module)
	{
	}

	protected virtual void StopModuleAnimation(int Module)
	{
		m_Animator.Play("Idle", Module, 0f);
	}

	private void UpdateResearching()
	{
		if (m_StateTimer > m_ResearchModuleDelay)
		{
			m_StateTimer = 0f;
			StopModuleAnimation(m_CurrentModule);
			m_CurrentModule++;
			if (m_CurrentModule == m_CurrentResearchLevel)
			{
				SetState(State.Idle);
			}
			else
			{
				StartModuleAnimation(m_CurrentModule);
			}
		}
	}

	public float GetTotalResearchDelay()
	{
		return (float)m_CurrentResearchLevel * m_ResearchModuleDelay;
	}

	public float GetCurrentResearchTimer()
	{
		return (float)m_CurrentModule * m_ResearchModuleDelay + m_StateTimer;
	}

	public float GetCurrentResearchTimerPercent()
	{
		return GetTotalResearchDelay() / GetCurrentResearchTimer();
	}

	private void EndResearching()
	{
		StopResearching();
		if ((bool)m_CurrentResearchObject)
		{
			m_CurrentResearchObject.StopUsing();
			SetResearchObject(null);
		}
		SetCurrentResearchQuest(Quest.ID.Total);
		AudioManager.Instance.StartEvent("BuildingResearchMakingComplete", this);
	}

	public bool HasResearchStarted()
	{
		if (m_CurrentResearchObject != null)
		{
			return true;
		}
		return false;
	}

	public void CancelResearch()
	{
		SetState(State.Cancelling);
	}

	protected void CreateCancelledItem()
	{
		Holdable currentResearchObject = m_CurrentResearchObject;
		m_CurrentResearchObject.transform.SetParent(ObjectTypeList.Instance.GetParentFromIdentifier(currentResearchObject.m_TypeIdentifier));
		currentResearchObject.gameObject.SetActive(value: true);
		TileCoord tileCoord = m_TileCoord;
		currentResearchObject.SendAction(new ActionInfo(ActionType.Dropped, tileCoord));
		if (ObjectTypeList.Instance.GetModelNameFromIdentifier(currentResearchObject.m_TypeIdentifier) == "" || currentResearchObject.m_TypeIdentifier == ObjectType.Sand)
		{
			currentResearchObject.StopUsing();
			return;
		}
		currentResearchObject.UpdatePositionToTilePosition(GetAccessPosition());
		PlotManager.Instance.RemoveObject(currentResearchObject);
		PlotManager.Instance.AddObject(currentResearchObject);
		SpawnAnimationManager.Instance.AddJump(currentResearchObject, tileCoord, GetAccessPosition(), 0f, currentResearchObject.transform.position.y, 4f);
		currentResearchObject.ForceHighlight(Highlighted: false);
		AudioManager.Instance.StartEvent("ObjectCreated", this);
	}

	private void UpdateIdle()
	{
		if (m_CurrentResearchQuest != Quest.ID.Total && QuestManager.Instance.GetQuestComplete(m_CurrentResearchQuest))
		{
			EndResearching();
			if ((bool)m_Engager && m_Engager.m_TypeIdentifier == ObjectType.FarmerPlayer && GameStateManager.Instance.GetCurrentState().m_BaseState == GameStateManager.State.BuildingSelect)
			{
				GameStateManager.Instance.PopState();
			}
		}
	}

	private void UpdateCancelling()
	{
		if (m_StateTimer >= m_CancelDelay)
		{
			m_StateTimer = 0f;
			CreateCancelledItem();
			AddAnimationManager.Instance.Add(this, Adding: false);
			SetResearchObject(null);
			SetState(State.Idle);
		}
	}

	public bool CanDoResearch(Quest.ID NewQuest)
	{
		return m_Quests.Contains(NewQuest);
	}

	public void SetCurrentResearchQuest(Quest.ID NewQuestID)
	{
		m_CurrentResearchQuest = NewQuestID;
		ObjectType newType = ObjectType.Nothing;
		if (m_CurrentResearchQuest != Quest.ID.Total)
		{
			newType = QuestManager.Instance.GetQuest(NewQuestID).m_ObjectTypeRequired;
			ResearchInfo infoFromQuestID = QuestManager.Instance.m_Data.m_ResearchData.GetInfoFromQuestID(NewQuestID);
			m_CurrentResearchLevel = 1;
			if (infoFromQuestID != null)
			{
				m_CurrentResearchLevel = infoFromQuestID.m_Level + 1;
			}
		}
		Sprite icon = IconManager.Instance.GetIcon(newType);
		if ((bool)m_BlueprintModel)
		{
			MeshRenderer component = m_BlueprintModel.GetComponent<MeshRenderer>();
			component.material.SetTexture("_MainTex", icon.texture);
			StandardShaderUtils.ChangeRenderMode(component.material, StandardShaderUtils.BlendMode.Transparent);
		}
	}

	protected void ShowCurrentModule(bool Show)
	{
		for (int i = 0; i < m_NumModules; i++)
		{
			GameObject gameObject = m_UpgradeModels[i];
			if (!Show)
			{
				gameObject.SetActive(value: true);
			}
			else if (i == m_NumModules - 1)
			{
				gameObject.SetActive(value: true);
			}
			else
			{
				gameObject.SetActive(value: false);
			}
		}
	}

	public override void SetBlueprint(bool Blueprint, bool Register = false)
	{
		base.SetBlueprint(Blueprint, Register);
		m_AnimControl.SetBlueprint(Blueprint);
		ShowCurrentModule(Blueprint);
	}

	protected void Update()
	{
		switch (m_State)
		{
		case State.Idle:
			UpdateIdle();
			break;
		case State.Researching:
			UpdateResearching();
			break;
		case State.Cancelling:
			UpdateCancelling();
			break;
		}
		m_StateTimer += TimeManager.Instance.m_NormalDelta;
	}

	public bool ModAddObject(Actionable ActionObject)
	{
		if (m_CurrentResearchQuest == Quest.ID.Total)
		{
			return false;
		}
		if (FolkHeart.GetIsFolkHeart(ActionObject.m_TypeIdentifier) && ActionObject.GetComponent<FolkHeart>() != null)
		{
			ActionObject.transform.position = m_HeartPoint.position;
			m_Heart = ActionObject;
			m_Heart.SendAction(new ActionInfo(ActionType.BeingHeld, default(TileCoord), this));
			AudioManager.Instance.StartEvent("BuildingIngredientAdd", this);
			m_HeartValue = VariableManager.Instance.GetVariableAsInt(ActionObject.m_TypeIdentifier, "Value");
			SetState(State.Researching);
			return true;
		}
		if (m_CurrentResearchObject == null && QuestManager.Instance.GetQuest(m_CurrentResearchQuest).m_ObjectTypeRequired == ActionObject.m_TypeIdentifier)
		{
			if ((bool)ActionObject.GetComponent<ToolFillable>())
			{
				ObjectType heldType = ActionObject.GetComponent<ToolFillable>().m_HeldType;
				SetResearchObject(ObjectTypeList.Instance.CreateObjectFromIdentifier(heldType, default(Vector3), Quaternion.identity).GetComponent<Holdable>(), UpdateAnim: false);
				ActionObject.GetComponent<ToolFillable>().Empty(1);
			}
			else
			{
				SetResearchObject(ActionObject.GetComponent<Holdable>(), UpdateAnim: false);
			}
			m_CurrentResearchObject.SendAction(new ActionInfo(ActionType.BeingHeld, default(TileCoord)));
			m_CurrentResearchObject.transform.localPosition = default(Vector3);
			AudioManager.Instance.StartEvent("BuildingResearchSampleAdded", this);
			if ((bool)m_AnimControl)
			{
				m_AnimControl.UpdateSample();
			}
			return true;
		}
		return false;
	}
}
