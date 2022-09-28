using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class ConverterFoundation : Converter
{
	[HideInInspector]
	public Building m_NewBuilding;

	private GameObject m_BuildingRoot;

	private int m_IngredientsAdded;

	private float m_IngredientsAddedTimer;

	private static float m_IngredientsAddedDelay = 0.25f;

	private PlaySound m_PlaySound;

	[HideInInspector]
	public bool m_Locked;

	private Wobbler m_CreateWobble;

	private float m_JumpTimer;

	public int m_Stage;

	public int m_NumStages;

	private GameObject[] m_StageModels;

	private int m_LastStage;

	public bool m_TempIngredientsFlag;

	private bool m_UpgradingBlocked;

	private List<GameObject> m_BlueprintTiles;

	public override void Restart()
	{
		base.Restart();
		m_DisplayIngredients = true;
		SetDimensions(new TileCoord(0, 0), new TileCoord(0, 0), new TileCoord(0, 0));
		SetSpawnPoint(new TileCoord(0, 0));
		m_IngredientsAdded = 0;
		m_IngredientsAddedTimer = 0f;
		m_Stage = 0;
		m_Locked = false;
		m_CreateWobble.Restart();
		HideAccessModel();
		m_SpawnModel.SetActive(value: false);
		m_TempIngredientsFlag = false;
		m_UpgradingBlocked = false;
	}

	protected new void Awake()
	{
		base.Awake();
		m_NewBuilding = null;
		m_CreateWobble = new Wobbler();
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_IngredientsRoot = base.transform.Find("IngredientsPoint");
	}

	public override void SetHighlight(bool Highlighted)
	{
		if (!m_Locked)
		{
			base.SetHighlight(Highlighted);
		}
	}

	protected new void OnDestroy()
	{
		DestroyBlueprint();
		base.OnDestroy();
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		if (GameStateManager.Instance.GetActualState() == GameStateManager.State.Edit)
		{
			if ((bool)m_ParentBuilding)
			{
				GameStateEdit.Instance.BuildingDestroyed(m_ParentBuilding);
			}
			GameStateEdit.Instance.BuildingDestroyed(this);
		}
		ShowBlueprint(Show: false);
		if ((bool)m_NewBuilding)
		{
			m_NewBuilding.StopUsing(AndDestroy);
		}
		if (AndDestroy)
		{
			m_NewBuilding = null;
		}
		base.StopUsing(AndDestroy);
	}

	public override string GetHumanReadableName()
	{
		if (m_NewBuilding == null)
		{
			return "";
		}
		return ObjectTypeList.Instance.GetHumanReadableNameFromIdentifier(m_NewBuilding.m_TypeIdentifier) + " " + base.GetHumanReadableName();
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		JSONUtils.Set(Node, "New", ObjectTypeList.Instance.GetSaveNameFromIdentifier(m_NewBuilding.m_TypeIdentifier));
		JSONUtils.Set(Node, "NewID", m_NewBuilding.m_UniqueID);
		JSONUtils.Set(Node, "Stage", m_Stage);
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		m_Stage = JSONUtils.GetAsInt(Node, "Stage", 0);
		ObjectType identifierFromSaveName = ObjectTypeList.Instance.GetIdentifierFromSaveName(JSONUtils.GetAsString(Node, "New", ""), Check: false);
		if (identifierFromSaveName != ObjectTypeList.m_Total)
		{
			int ingredientsStagesFromIdentifier = ObjectTypeList.Instance.GetIngredientsStagesFromIdentifier(identifierFromSaveName);
			if (m_Stage >= ingredientsStagesFromIdentifier)
			{
				m_Stage = ingredientsStagesFromIdentifier - 1;
			}
			int asInt = JSONUtils.GetAsInt(Node, "NewID", 0);
			SetNewBuilding(identifierFromSaveName, asInt);
		}
		ObjectTypeList.Instance.ChangeActionable(m_NewBuilding, JSONUtils.GetAsInt(Node, "NewID", 0));
		SetRotation(m_Rotation);
	}

	protected override void TileCoordChanged(TileCoord Position)
	{
		if ((bool)m_NewBuilding)
		{
			m_NewBuilding.GetComponent<Building>().SetTilePosition(Position);
		}
		base.TileCoordChanged(Position);
	}

	public override TileCoord GetAccessPosition()
	{
		if ((bool)m_NewBuilding)
		{
			return m_NewBuilding.GetAccessPosition();
		}
		return base.GetAccessPosition();
	}

	public override void SetRotation(int Rotation)
	{
		if ((bool)m_NewBuilding && m_NewBuilding.m_TypeIdentifier == ObjectType.BeltLinkage)
		{
			Rotation = 0;
		}
		base.SetRotation(Rotation);
		if ((bool)m_NewBuilding)
		{
			m_NewBuilding.GetComponent<Building>().SetRotation(Rotation);
			m_NewBuilding.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		}
	}

	public override void UpdateTiles()
	{
		if ((bool)m_NewBuilding)
		{
			if ((bool)m_ParentBuilding && m_NewBuilding.m_BuildingToUpgradeFrom != ObjectTypeList.m_Total)
			{
				m_NewBuilding.HideAccessModel(m_AccessModelHidden);
			}
			m_NewBuilding.SetTilePosition(m_TileCoord);
			m_NewBuilding.m_Rotation = m_Rotation;
			m_NewBuilding.UpdateTiles();
			m_Tiles = m_NewBuilding.m_Tiles;
		}
	}

	public override bool CanStack()
	{
		if ((bool)m_NewBuilding && m_NewBuilding.m_BuildingToUpgradeFrom == ObjectTypeList.m_Total)
		{
			return true;
		}
		return false;
	}

	private void CreateBlueprint()
	{
		m_BlueprintTiles = new List<GameObject>();
		foreach (TileCoord tile in m_NewBuilding.m_Tiles)
		{
			Vector3 position = tile.ToWorldPositionTileCentered();
			position.y = base.transform.position.y;
			position += new Vector3(0f, 0.05f, 0f);
			GameObject gameObject = ModelManager.Instance.Instantiate(ObjectTypeList.m_Total, "Models/Buildings/BuildingBlueprintTile", base.transform, RandomVariants: false);
			gameObject.transform.position = position;
			m_BlueprintTiles.Add(gameObject);
		}
	}

	public void DestroyBlueprint()
	{
		if (m_BlueprintTiles == null)
		{
			return;
		}
		foreach (GameObject blueprintTile in m_BlueprintTiles)
		{
			Object.Destroy(blueprintTile);
		}
		m_BlueprintTiles = null;
	}

	public void ShowBlueprint(bool Show)
	{
		if (m_BlueprintTiles == null)
		{
			return;
		}
		foreach (GameObject blueprintTile in m_BlueprintTiles)
		{
			blueprintTile.SetActive(Show);
		}
	}

	private void GetStageModels()
	{
		m_LastStage = -1;
		int childCount = m_NewBuilding.m_ModelRoot.transform.childCount;
		m_StageModels = new GameObject[m_NumStages];
		for (int i = 0; i < childCount; i++)
		{
			GameObject gameObject = m_NewBuilding.m_ModelRoot.transform.GetChild(i).gameObject;
			if (gameObject.name.Contains("Stage") && int.TryParse(gameObject.name.Substring(5, gameObject.name.Length - 5), out var result))
			{
				m_StageModels[result] = gameObject;
			}
		}
	}

	private void UpdateStageModels(bool Loading = false)
	{
		if (m_StageModels == null)
		{
			ModelManager.Instance.RestoreStandardMaterials(m_NewBuilding);
			return;
		}
		int numStages = m_NumStages;
		if (m_Stage == numStages)
		{
			ModelManager.Instance.RestoreStandardMaterials(m_NewBuilding);
		}
		else
		{
			for (int i = 0; i < m_NumStages; i++)
			{
				bool active = i <= m_Stage;
				m_StageModels[i].SetActive(active);
				if (i >= m_LastStage && i < m_Stage)
				{
					ModelManager.Instance.RestoreStandardMaterialsChild(m_NewBuilding, "Stage" + i);
				}
			}
		}
		m_LastStage = m_Stage;
	}

	public void SetupNewBuilding()
	{
		if (m_NewBuilding.m_ExtraTiles != null)
		{
			foreach (KeyValuePair<TileCoord, int> extraTile in m_NewBuilding.m_ExtraTiles)
			{
				if (extraTile.Value == 1)
				{
					AddTile(extraTile.Key);
				}
				else
				{
					RemoveTile(extraTile.Key);
				}
			}
		}
		SetDimensions(m_NewBuilding.m_TopLeft, m_NewBuilding.m_BottomRight, m_NewBuilding.m_AccessPoint);
		m_Requirements = new List<List<IngredientRequirement>>();
		m_Results = new List<List<IngredientRequirement>>();
		List<IngredientRequirement> list = new List<IngredientRequirement>();
		list.Add(new IngredientRequirement(ObjectTypeList.m_Total, 1));
		AddRequirements(new List<IngredientRequirement>(), list);
		list = new List<IngredientRequirement>();
		list.Add(new IngredientRequirement(m_NewBuilding.m_TypeIdentifier, 1));
		AddRequirements(null, list);
		SetResultToCreate(1);
		m_NumStages = ObjectTypeList.Instance.GetIngredientsStagesFromIdentifier(m_NewBuilding.m_TypeIdentifier);
		if (m_NumStages > 1)
		{
			GetStageModels();
			ChangeRequirementsForStage(1, m_Stage);
			UpdateStageModels();
		}
	}

	public void SetNewBuilding(ObjectType NewBuildingType, int UID = -1)
	{
		m_NewBuilding = ObjectTypeList.Instance.CreateObjectFromIdentifier(NewBuildingType, base.transform.localPosition, Quaternion.identity, UID).GetComponent<Building>();
		m_NewBuilding.transform.parent = base.transform;
		m_NewBuilding.transform.localPosition = default(Vector3);
		m_NewBuilding.SetTilePosition(m_TileCoord);
		m_NewBuilding.m_ParentBuilding = this;
		if (m_NewBuilding.m_MaxLevels != 1)
		{
			CalcHeight();
			m_Levels = new List<Building>();
			m_NumLevels = m_NewBuilding.m_NumLevels;
			m_MaxLevels = m_NewBuilding.m_MaxLevels;
		}
		m_BuildingRoot = m_NewBuilding.m_ModelRoot;
		SetupNewBuilding();
		MakeBuildingTransparent();
		if (m_NumStages > 1)
		{
			m_LastStage = -1;
			UpdateStageModels(Loading: true);
		}
		m_NewBuilding.SetBlueprint(Blueprint: true);
		m_BaseConversionDelay = VariableManager.Instance.GetVariableAsFloat(NewBuildingType, "BuildDelay");
		UpdateConversionDelay();
		CreateBlueprint();
	}

	public override bool GetNewLevelAllowed(Building NewBuilding)
	{
		if (m_NewBuilding.GetComponent<Wall>() == null && (NewBuilding.m_TileCoord != m_TileCoord || NewBuilding.m_Rotation != m_Rotation))
		{
			return false;
		}
		if (m_TotalLevels + NewBuilding.m_TotalLevels > m_MaxLevels)
		{
			return false;
		}
		if (NewBuilding.m_TypeIdentifier != ObjectType.ConverterFoundation)
		{
			return false;
		}
		if (NewBuilding.GetComponent<ConverterFoundation>().m_TempIngredientsFlag)
		{
			return false;
		}
		return m_NewBuilding.GetNewLevelAllowed(NewBuilding.GetComponent<ConverterFoundation>().m_NewBuilding);
	}

	public void MakeBuildingTransparent()
	{
		if (m_NewBuilding.m_ModelRoot.activeSelf)
		{
			ModelManager.Instance.SetMaterialsTransparent(m_NewBuilding.gameObject, Transparent: true);
		}
		else
		{
			ModelManager.Instance.SetMaterialsTransparent(m_BuildingRoot, Transparent: true);
		}
	}

	private bool GetIsRefreshable()
	{
		if (m_NewBuilding == null)
		{
			return false;
		}
		if ((bool)m_NewBuilding.GetComponent<Wall>() || Bridge.GetIsTypeBridge(m_NewBuilding.m_TypeIdentifier) || m_NewBuilding.m_TypeIdentifier == ObjectType.TrainTrack || m_NewBuilding.m_TypeIdentifier == ObjectType.TrainTrackBridge)
		{
			return true;
		}
		return false;
	}

	public override void SendAction(ActionInfo Info)
	{
		ActionType action = Info.m_Action;
		if ((uint)(action - 41) <= 1u)
		{
			if (GetIsRefreshable())
			{
				m_NewBuilding.m_AccessModel.transform.parent = null;
				m_NewBuilding.m_Level = m_Level;
				m_NewBuilding.SendAction(Info);
				SetRotation(m_NewBuilding.m_Rotation);
				m_NewBuilding.SetRotation(0);
				m_BuildingRoot = m_NewBuilding.m_ModelRoot;
				m_NewBuilding.m_AccessModel.transform.parent = m_NewBuilding.transform;
			}
			bool show = false;
			if (m_ParentBuilding == null || m_NewBuilding.m_BuildingToUpgradeFrom != ObjectTypeList.m_Total)
			{
				show = true;
			}
			ShowBlueprint(show);
		}
		base.SendAction(Info);
	}

	public override bool CanDoAction(ActionInfo Info, bool RightNow)
	{
		switch (Info.m_Action)
		{
		case ActionType.Refresh:
		case ActionType.RefreshFirst:
			if (GetIsRefreshable())
			{
				return true;
			}
			return false;
		case ActionType.Engaged:
			return false;
		default:
			return base.CanDoAction(Info, RightNow);
		}
	}

	public override object GetActionInfo(GetActionInfo Info)
	{
		switch (Info.m_Action)
		{
		case GetAction.IsDeletable:
			if (m_Locked)
			{
				return false;
			}
			if (m_NewBuilding.m_BuildingToUpgradeFrom != ObjectTypeList.m_Total && m_ParentBuilding != null)
			{
				return false;
			}
			if (m_State == State.Converting || m_State == State.Creating || m_CreateWobble.m_Height != 0f)
			{
				return false;
			}
			break;
		case GetAction.IsMovable:
			if (m_Locked)
			{
				return false;
			}
			break;
		case GetAction.IsDuplicatable:
			if (m_NewBuilding.m_TypeIdentifier == ObjectType.FolkSeedPod)
			{
				return false;
			}
			if (m_NewBuilding.m_BuildingToUpgradeFrom != ObjectTypeList.m_Total && m_ParentBuilding != null)
			{
				return false;
			}
			break;
		case GetAction.IsPickable:
			if ((bool)m_ParentBuilding && m_ParentBuilding.GetIsSavable())
			{
				return true;
			}
			break;
		}
		return base.GetActionInfo(Info);
	}

	protected override void EndAdd(AFO Info)
	{
		Actionable @object = Info.m_Object;
		if (!ToolFillable.GetIsTypeFillable(@object.m_TypeIdentifier))
		{
			@object.GetComponent<Actionable>().SendAction(new ActionInfo(ActionType.BeingHeld, default(TileCoord), this));
		}
		AudioManager.Instance.StartEvent("BuildingBlueprintAdd", this);
		UpdateIngredients();
		if (m_Ingredients.Count != m_IngredientsAdded)
		{
			m_IngredientsAdded = m_Ingredients.Count;
			m_IngredientsAddedTimer = m_IngredientsAddedDelay;
		}
		if (!SaveLoadManager.Instance.m_Loading)
		{
			if (AreRequrementsMet() && m_AutoConvert)
			{
				StartConversion(Info.m_Actioner);
			}
			else
			{
				AddAnimationManager.Instance.Add(this, Adding: true);
			}
		}
	}

	private bool CanAcceptObject(BaseClass NewObject, ObjectType NewObjectType)
	{
		if (m_Locked)
		{
			return false;
		}
		if (m_NewBuilding == null || m_Plot == null)
		{
			return false;
		}
		if ((bool)NewObject)
		{
			NewObjectType = NewObject.m_TypeIdentifier;
		}
		ObjectType objectType = NewObjectType;
		if (NewObject != null && ToolFillable.GetIsTypeFillable(NewObject.m_TypeIdentifier))
		{
			objectType = NewObject.GetComponent<ToolFillable>().m_HeldType;
		}
		bool flag = CanAcceptIngredient(objectType);
		if (flag && !Floor.GetIsTypeFloor(m_NewBuilding.m_TypeIdentifier) && WillRequrementsBeMet(1, objectType) && (bool)m_Plot.GetSelectableObjectAtTile(m_TileCoord, this, ExcludeBuildings: true))
		{
			return false;
		}
		return flag;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (Info.m_ActionType == AFO.AT.Primary)
		{
			Info.m_FarmerState = Farmer.State.Engaged;
			if (m_TypeIdentifier == ObjectType.ConverterFoundation && TutorialPanelController.Instance.GetActive())
			{
				return ActionType.Fail;
			}
			if ((bool)Info.m_Actioner && Info.m_Actioner.m_TypeIdentifier == ObjectType.Worker)
			{
				return ActionType.Fail;
			}
			if (!IsBusy() && GameStateManager.Instance.GetActualState() != GameStateManager.State.TeachWorker && m_Engager == null)
			{
				return ActionType.EngageObject;
			}
			return ActionType.Total;
		}
		if (Info.m_ActionType == AFO.AT.Secondary)
		{
			Info.m_StartAction = StartAddAnything;
			Info.m_EndAction = EndAdd;
			Info.m_AbortAction = base.AbortAddAnything;
			Info.m_FarmerState = Farmer.State.Adding;
			if (m_NewBuilding.m_BuildingToUpgradeFrom != ObjectTypeList.m_Total && m_UpgradingBlocked)
			{
				return ActionType.Fail;
			}
			if (!CanAcceptObject(Info.m_Object, Info.m_ObjectType))
			{
				return ActionType.Fail;
			}
			return ActionType.AddResource;
		}
		return ActionType.Total;
	}

	public override bool CanAcceptIngredient(ObjectType Type)
	{
		if (m_Locked)
		{
			return false;
		}
		if (GameStateManager.Instance.GetActualState() == GameStateManager.State.Edit && GameStateManager.Instance.GetCurrentState().GetComponent<GameStateEdit>().CheckBuildingInDragModels(this))
		{
			return false;
		}
		return base.CanAcceptIngredient(Type);
	}

	public override void StartConverting()
	{
		base.StartConverting();
		if ((bool)m_NewBuilding && m_NewBuilding.m_BuildingToUpgradeFrom != ObjectTypeList.m_Total && (GetIsSavable() || (bool)m_ParentBuilding) && m_State == State.Idle)
		{
			ModelManager.Instance.RestoreStandardMaterials(m_NewBuilding);
		}
		if ((bool)m_NewBuilding && (bool)m_NewBuilding.GetComponent<TrainTrack>())
		{
			m_PlaySound = AudioManager.Instance.StartEvent("TrackMaking", this, Remember: true);
		}
		else
		{
			m_PlaySound = AudioManager.Instance.StartEvent("BuildingBlueprintMaking", this, Remember: true);
		}
		if (m_Level == 0 && !SaveLoadManager.Instance.m_Loading && m_Stage == m_LastStage - 1)
		{
			MapManager.Instance.RemoveBuilding(this);
			MapManager.Instance.AddBuilding(this);
		}
		if (GameStateManager.Instance.GetCurrentState().m_BaseState == GameStateManager.State.BuildingSelect && GameStateManager.Instance.GetCurrentState().GetComponent<GameStateBuildingSelect>().m_Building == this)
		{
			GameStateManager.Instance.PopState();
		}
		if (m_Stage == m_LastStage - 1)
		{
			MapManager.Instance.UpdateBuildingOnTiles(this);
		}
	}

	public void FarmerJump()
	{
		if (m_NewBuilding.m_TypeIdentifier >= ObjectType.Total)
		{
			ModManager.Instance.GetCustomModelTransform(m_NewBuilding.m_TypeIdentifier, out var _, out var _, out var ModelScale);
			m_BuildingRoot.transform.localScale = new Vector3(ModelScale.x * 0.9f, ModelScale.y * 1.3f, ModelScale.z * 0.9f);
		}
		else
		{
			m_BuildingRoot.transform.localScale = new Vector3(0.9f, 1.3f, 0.9f);
		}
		m_JumpTimer = 0.2f;
		UpdateIngredients();
	}

	protected override void UpdateConverting()
	{
		if (!(m_JumpTimer > 0f))
		{
			return;
		}
		m_JumpTimer -= TimeManager.Instance.m_NormalDelta;
		if (m_JumpTimer <= 0f)
		{
			m_JumpTimer = 0f;
			if (m_NewBuilding.m_TypeIdentifier >= ObjectType.Total)
			{
				ModManager.Instance.GetCustomModelTransform(m_NewBuilding.m_TypeIdentifier, out var _, out var _, out var ModelScale);
				m_BuildingRoot.transform.localScale = ModelScale;
			}
			else
			{
				m_BuildingRoot.transform.localScale = new Vector3(1f, 1f, 1f);
			}
		}
	}

	protected override void EndConverting()
	{
		AudioManager.Instance.StopEvent(m_PlaySound);
		if (HudManager.Instance.m_ConverterRollover.m_Target == this)
		{
			HudManager.Instance.m_ConverterRollover.SetConverterTarget(null);
		}
		if (m_NewBuilding.m_TypeIdentifier >= ObjectType.Total)
		{
			ModManager.Instance.GetCustomModelTransform(m_NewBuilding.m_TypeIdentifier, out var _, out var _, out var ModelScale);
			m_BuildingRoot.transform.localScale = ModelScale;
		}
		else
		{
			m_BuildingRoot.transform.localScale = new Vector3(1f, 1f, 1f);
		}
		m_Stage++;
		UpdateStageModels();
		if (m_Stage < m_NumStages)
		{
			ChangeRequirementsForStage(1, m_Stage);
			AudioManager.Instance.StartEvent("BuildingStageComplete", this);
		}
		else
		{
			AudioManager.Instance.StartEvent("BuildingBlueprintComplete", this);
		}
		m_CreateWobble.Go(0.5f, 5f, 0.25f);
		DestroyIngredients();
	}

	public static void TransferToNewBuilding(Building OldBuilding, Building NewBuilding)
	{
		if (Converter.GetIsTypeConverter(OldBuilding.m_TypeIdentifier))
		{
			ObjectType resultType = OldBuilding.GetComponent<Converter>().GetResultType();
			Converter component = NewBuilding.GetComponent<Converter>();
			int resultIndex = component.GetResultIndex(resultType);
			component.SetResultToCreate(resultIndex);
		}
		if (ResearchStation.GetIsTypeResearchStation(OldBuilding.m_TypeIdentifier))
		{
			ResearchStation component2 = OldBuilding.GetComponent<ResearchStation>();
			NewBuilding.GetComponent<ResearchStation>().SetCurrentResearchQuest(component2.m_CurrentResearchQuest);
		}
		if ((bool)OldBuilding.GetComponent<Fueler>() && (bool)NewBuilding.GetComponent<Fueler>())
		{
			Fueler component3 = OldBuilding.GetComponent<Fueler>();
			NewBuilding.GetComponent<Fueler>().SetFuel(component3.m_Fuel);
		}
		if ((bool)NewBuilding.GetComponent<Storage>())
		{
			NewBuilding.GetComponent<Storage>().CopyFrom(OldBuilding.GetComponent<Storage>());
			NewBuilding.GetComponent<Storage>().Transfer(OldBuilding.GetComponent<Storage>());
		}
		if (OldBuilding.GetName() != "")
		{
			NewBuilding.SetName(OldBuilding.GetName());
		}
	}

	public static void TransferBotsToNewBuilding(Building OldBuilding, Building NewBuilding)
	{
		Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("Worker");
		if (collection != null)
		{
			foreach (KeyValuePair<BaseClass, int> item in collection)
			{
				bool flag = false;
				foreach (HighInstruction item2 in item.Key.GetComponent<Worker>().m_WorkerInterpreter.m_HighInstructions.m_List)
				{
					flag = item2.ChangeUID(OldBuilding.m_UniqueID, NewBuilding.m_UniqueID, flag);
				}
				if (flag)
				{
					item.Key.GetComponent<Worker>().m_WorkerInterpreter.UpdateCurrentScript();
				}
			}
		}
		WorkerScriptManager.Instance.ChangeBuildingUID(OldBuilding.m_UniqueID, NewBuilding.m_UniqueID);
	}

	private void EndCreating()
	{
		m_NewBuilding.m_ModelRoot.SetActive(value: true);
		m_NewBuilding.transform.SetParent(base.transform);
		m_NewBuilding.transform.localPosition = base.transform.localPosition;
		m_NewBuilding.m_ParentBuilding = null;
		if (HudManager.Instance.m_ConverterRollover.m_Target == this)
		{
			HudManager.Instance.m_ConverterRollover.SetConverterTarget(null);
		}
		bool flag = false;
		bool flag2 = false;
		Building bottomBuilding = GetBottomBuilding();
		if (m_Level != 0)
		{
			if (m_NewBuilding.m_BuildingToUpgradeFrom != ObjectTypeList.m_Total)
			{
				MapManager.Instance.RemoveBuilding(bottomBuilding);
				TransferToNewBuilding(bottomBuilding, m_NewBuilding);
				flag2 = true;
				m_NewBuilding.HideAccessModel(Hide: false);
				m_NewBuilding.UpdateTiles();
			}
			else
			{
				flag = true;
				int index = bottomBuilding.m_Levels.IndexOf(this);
				bottomBuilding.m_Levels[index] = m_NewBuilding;
				m_NewBuilding.m_Level = m_Level;
				m_NewBuilding.m_AccessModel.SetActive(value: false);
				m_NewBuilding.m_ParentBuilding = bottomBuilding;
				if ((bool)m_NewBuilding.GetComponent<Storage>())
				{
					ObjectType objectType = bottomBuilding.GetComponent<Storage>().m_ObjectType;
					m_NewBuilding.GetComponent<Storage>().SetObjectType(objectType);
				}
			}
		}
		else
		{
			MapManager.Instance.RemoveBuilding(this);
		}
		m_NewBuilding.transform.parent = MapManager.Instance.m_ObjectsRootTransform;
		m_NewBuilding.transform.localPosition = base.transform.localPosition;
		m_NewBuilding.transform.localScale = new Vector3(1f, 1f, 1f);
		m_NewBuilding.SetRotation(m_Rotation);
		m_NewBuilding.SetTilePosition(m_TileCoord);
		m_NewBuilding.CheckWallsFloors();
		m_NewBuilding.SetBlueprint(Blueprint: false, Register: true);
		if (!flag)
		{
			MapManager.Instance.AddBuilding(m_NewBuilding);
			if (m_Levels != null)
			{
				List<Building> list = new List<Building>();
				foreach (Building level in m_Levels)
				{
					list.Add(level);
				}
				foreach (Building item in list)
				{
					RemoveBuilding(item);
					m_NewBuilding.AddBuilding(item);
				}
				m_Levels = null;
			}
		}
		if (flag2)
		{
			TransferBotsToNewBuilding(bottomBuilding, m_NewBuilding);
			bottomBuilding.RemoveBuilding(this);
			foreach (Building level2 in bottomBuilding.m_Levels)
			{
				level2.StopUsing();
			}
			bottomBuilding.StopUsing();
		}
		QuestManager.Instance.AddEvent(QuestEvent.Type.Build, m_LastEngagerType == ObjectType.Worker, m_NewBuilding.m_TypeIdentifier, m_NewBuilding);
		QuestManager.Instance.AddEvent(QuestEvent.Type.BuildAnything, m_LastEngagerType == ObjectType.Worker, 0, m_NewBuilding);
		if (Floor.GetIsTypeFloor(m_NewBuilding.m_TypeIdentifier))
		{
			StatsManager.Instance.AddEvent(StatsManager.StatEvent.Flooring);
		}
		else if (Wall.GetIsTypeWall(m_NewBuilding.m_TypeIdentifier))
		{
			StatsManager.Instance.AddEvent(StatsManager.StatEvent.Walls);
		}
		else
		{
			StatsManager.Instance.AddEvent(StatsManager.StatEvent.Buildings);
		}
		m_NewBuilding.SendAction(new ActionInfo(ActionType.RefreshFirst, default(TileCoord)));
		if (m_NewBuilding.m_Level != 0)
		{
			m_NewBuilding.SetIsSavable(IsSavable: false);
		}
		m_NewBuilding.ForceHighlight(Highlighted: false);
		if ((bool)m_NewBuilding.GetComponent<Converter>())
		{
			NewIconManager.Instance.ConverterBuilt(m_NewBuilding.GetComponent<Converter>());
		}
		m_NewBuilding.CheckAddCollectable(FromLoad: false);
		RecordingManager.Instance.UpdateObject(m_NewBuilding);
		if (m_NewBuilding.m_TypeIdentifier == ObjectType.FolkSeedPod)
		{
			CeremonyManager.Instance.AddCeremony(CeremonyManager.CeremonyType.FolkSeedUnlocked);
		}
		if (!flag2 && Converter.GetIsTypeConverter(m_NewBuilding.m_TypeIdentifier) && m_NewBuilding.GetComponent<Converter>().m_Results.Count > 2)
		{
			if (m_NewBuilding.m_TypeIdentifier == ObjectType.WorkerAssembler)
			{
				m_NewBuilding.GetComponent<Converter>().SetResultToCreate(1);
			}
			else
			{
				m_NewBuilding.GetComponent<Converter>().SetResultToCreate(0);
			}
		}
		if (GameStateManager.Instance.GetActualState() == GameStateManager.State.Edit)
		{
			GameStateEdit.Instance.BuildingChanged(bottomBuilding);
		}
		ModManager.Instance.CheckBuildingTypeSpawnedCallback(m_NewBuilding);
		m_NewBuilding = null;
		StopUsing();
	}

	private void NextStage()
	{
		if (m_Stage == m_NumStages)
		{
			EndCreating();
		}
		else
		{
			SetState(State.Idle);
		}
	}

	public override void CheckWallsFloors()
	{
		if (m_NewBuilding != null)
		{
			m_NewBuilding.CheckWallsFloors();
		}
	}

	protected override void CalcHeight()
	{
		if (m_NewBuilding != null)
		{
			m_LevelHeight = m_NewBuilding.m_LevelHeight;
		}
		else
		{
			base.CalcHeight();
		}
	}

	public override void HideWallFloorIcon()
	{
		m_NewBuilding.HideWallFloorIcon();
	}

	public override bool CanBuildTypeUpon(ObjectType NewType)
	{
		if (m_MaxLevels != 1 && m_NewBuilding.m_TypeIdentifier == NewType)
		{
			return true;
		}
		return false;
	}

	private void UpdateCreating()
	{
		m_CreateWobble.Update();
		m_NewBuilding.transform.localScale = new Vector3(1f, m_CreateWobble.m_Height + 1f, 1f);
		if (m_CreateWobble.m_Height == 0f)
		{
			NextStage();
		}
	}

	public override void CancelBuild()
	{
		if (m_NumStages > 1)
		{
			for (int i = 0; i < m_Stage; i++)
			{
				IngredientRequirement[] ingredientsFromIdentifier = ObjectTypeList.Instance.GetIngredientsFromIdentifier(m_Results[1][0].m_Type, i);
				for (int j = 0; j < ingredientsFromIdentifier.Length; j++)
				{
					ObjectType type = ingredientsFromIdentifier[j].m_Type;
					for (int k = 0; k < ingredientsFromIdentifier[j].m_Count; k++)
					{
						if (!ToolFillable.GetIsTypeLiquid(type))
						{
							BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(type, default(Vector3), Quaternion.identity);
							AddIngredient(baseClass.GetComponent<Holdable>());
							baseClass.GetComponent<Actionable>().SendAction(new ActionInfo(ActionType.BeingHeld, default(TileCoord), this));
						}
					}
				}
			}
		}
		base.CancelBuild();
	}

	public override void PlayerDeleted()
	{
		CancelBuild();
		base.PlayerDeleted();
	}

	public bool CanInstantDelete()
	{
		if (m_Ingredients.Count > 0 || m_Stage != 0)
		{
			return false;
		}
		return true;
	}

	private void UpdateUpgrading()
	{
		if (m_NewBuilding.m_BuildingToUpgradeFrom == ObjectTypeList.m_Total || (!GetIsSavable() && !m_ParentBuilding) || m_State != 0)
		{
			return;
		}
		List<BaseClass> list = new List<BaseClass>();
		Building building = TileManager.Instance.GetTile(m_TileCoord).m_Building;
		if ((bool)building)
		{
			list.Add(building);
		}
		Selectable Object;
		bool flag = (MapManager.Instance.CheckBuildingIntersection(m_NewBuilding, list, out Object, null, IgnoreAccessTile: true) ? true : false);
		if (flag == m_UpgradingBlocked)
		{
			return;
		}
		m_UpgradingBlocked = flag;
		MeshCollider[] componentsInChildren = m_NewBuilding.GetComponentsInChildren<MeshCollider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = !m_UpgradingBlocked;
		}
		Color color = new Color(1f, 1f, 1f, 0.5f);
		if (m_UpgradingBlocked)
		{
			color = new Color(1f, 0f, 0f, 0.5f);
		}
		MeshRenderer[] componentsInChildren2 = m_NewBuilding.GetComponentsInChildren<MeshRenderer>();
		for (int i = 0; i < componentsInChildren2.Length; i++)
		{
			Material[] materials = componentsInChildren2[i].materials;
			for (int j = 0; j < materials.Length; j++)
			{
				materials[j].color = color;
			}
		}
		if (m_BlueprintTiles == null)
		{
			return;
		}
		foreach (GameObject blueprintTile in m_BlueprintTiles)
		{
			blueprintTile.GetComponent<MeshRenderer>().material.color = color;
		}
	}

	protected new void Update()
	{
		UpdateUpgrading();
		if (m_IngredientsAddedTimer > 0f)
		{
			m_IngredientsAddedTimer -= TimeManager.Instance.m_NormalDelta;
			if (m_IngredientsAddedTimer < 0f)
			{
				m_IngredientsAddedTimer = 0f;
			}
			_ = m_IngredientsAddedTimer / m_IngredientsAddedDelay;
		}
		if (m_State == State.Creating)
		{
			UpdateCreating();
		}
		else
		{
			base.Update();
		}
	}
}
