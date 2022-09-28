using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class Converter : Building
{
	public enum State
	{
		Idle,
		Converting,
		Creating,
		Cancelling,
		Total
	}

	public static string m_ResultsToCreateName = "ResultIndex";

	public static List<ObjectType> m_Types;

	[HideInInspector]
	public State m_State;

	protected float m_StateTimer;

	[HideInInspector]
	public List<List<IngredientRequirement>> m_Requirements;

	[HideInInspector]
	public List<List<IngredientRequirement>> m_Results;

	[HideInInspector]
	public List<Holdable> m_Ingredients;

	protected float m_BaseConversionDelay;

	protected float m_ConversionDelay;

	protected float m_CreateDelay;

	protected float m_CancelDelay;

	private int m_NumCreated;

	protected int m_TypeCreated;

	public TileCoord m_SpawnPoint;

	[HideInInspector]
	public GameObject m_SpawnModel;

	[HideInInspector]
	public int m_ResultsToCreate;

	[HideInInspector]
	public bool m_AutoConvert;

	private GameObject m_BlueprintModel;

	private GameObject m_BlueprintBaseModel;

	protected bool m_DisplayIngredients;

	public Transform m_IngredientsRoot;

	public static float m_IngredientSpacing = 0.5f;

	protected Actionable m_ObjectInvolved;

	protected ObjectType m_LastEngagerType;

	public AreaIndicator m_Indicator;

	[HideInInspector]
	public float m_IngredientsHeight;

	protected Animator m_Animator;

	protected float m_AnimConvertTimer;

	protected float m_SquashIngredientsTimer;

	private float m_SquashScale;

	protected MyParticles m_BubblesParticles;

	private float m_BubblesHeight;

	protected MyParticles m_SmokeParticles;

	private Vector3[] m_IngredientScales;

	public Holdable m_LastAddedIngredient;

	public static bool GetIsTypeConverter(ObjectType NewType)
	{
		if (NewType == ObjectType.ConverterFoundation || NewType == ObjectType.BenchSaw || NewType == ObjectType.Workbench || NewType == ObjectType.WorkerAssembler || NewType == ObjectType.WheatHammer || NewType == ObjectType.CogBench || NewType == ObjectType.CookingPotCrude || NewType == ObjectType.ChoppingBlock || NewType == ObjectType.ClayFurnace || NewType == ObjectType.HatMaker || NewType == ObjectType.ButterChurn || NewType == ObjectType.KitchenTable || NewType == ObjectType.SpinningWheel || NewType == ObjectType.RockingChair || NewType == ObjectType.OvenCrude || NewType == ObjectType.Oven || NewType == ObjectType.KilnCrude || NewType == ObjectType.Cauldron || NewType == ObjectType.MortarMixerCrude || NewType == ObjectType.Quern || NewType == ObjectType.WorkerWorkbenchMk1 || NewType == ObjectType.WorkerWorkbenchMk2 || NewType == ObjectType.ClayStationCrude || NewType == ObjectType.FolkSeedRehydrator || NewType == ObjectType.StringWinderCrude || NewType == ObjectType.Barn || NewType == ObjectType.ChickenCoop || NewType == ObjectType.VehicleAssembler || NewType == ObjectType.WorkbenchMk2 || NewType == ObjectType.WorkbenchStructural || NewType == ObjectType.FolkSeedPod || NewType == ObjectType.PotCrude || NewType == ObjectType.MasonryBench || NewType == ObjectType.LoomCrude || NewType == ObjectType.ToyStationCrude || NewType == ObjectType.VehicleAssemblerGood || ((bool)ModManager.Instance && ModManager.Instance.ModConverterClass.IsItCustomType(NewType)) || NewType == ObjectType.CrudePlantBreedingStation || NewType == ObjectType.CrudeAnimalBreedingStation || NewType == ObjectType.HayBalerCrude || NewType == ObjectType.WorkerWorkbenchMk3 || NewType == ObjectType.Furnace || LinkedSystemConverter.GetIsTypeLinkedSystemConverter(NewType) || NewType == ObjectType.MortarMixerGood || NewType == ObjectType.SpinningJenny || NewType == ObjectType.Easel)
		{
			return true;
		}
		return false;
	}

	public new static void Init()
	{
		m_Types = new List<ObjectType>();
	}

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("Converter", m_TypeIdentifier);
		m_Types.Add(m_TypeIdentifier);
	}

	public override void CheckAddCollectable(bool FromLoad)
	{
		base.CheckAddCollectable(FromLoad);
		if (!ObjectTypeList.m_Loading || FromLoad)
		{
			CollectionManager.Instance.AddCollectable("Converter", this);
		}
	}

	public override void Restart()
	{
		base.Restart();
		m_Ingredients = new List<Holdable>();
		ConverterResults resultsForBuilding = VariableManager.Instance.GetResultsForBuilding(m_TypeIdentifier);
		m_Requirements = resultsForBuilding.m_Requirements;
		m_Results = resultsForBuilding.m_Results;
		SetResultToCreate(0);
		FindFirstObject();
		m_BaseConversionDelay = VariableManager.Instance.GetVariableAsFloat(m_TypeIdentifier, "ConversionDelay");
		if (SaveLoadManager.m_Video)
		{
			m_BaseConversionDelay = 1f;
		}
		UpdateConversionDelay();
		m_CreateDelay = 0.5f;
		m_CancelDelay = 0.25f;
		m_SpawnPoint = new TileCoord(0, 0);
		m_AutoConvert = true;
		m_DisplayIngredients = false;
		m_Animator = GetComponent<Animator>();
		if ((bool)m_Animator)
		{
			m_Animator.Rebind();
		}
		m_State = State.Total;
		SetState(State.Idle);
	}

	public override void PostCreate()
	{
		base.PostCreate();
		ChangeAccessPointToIn();
		m_SpawnModel = ModelManager.Instance.Instantiate(ObjectTypeList.m_Total, "Models/Buildings/BuildingSpawnPoint", base.transform, RandomVariants: false);
		UpdateAccessModelPosition();
		if ((bool)m_ModelRoot)
		{
			m_IngredientsRoot = m_ModelRoot.transform.Find("IngredientsPoint");
			if ((bool)m_IngredientsRoot && m_TypeIdentifier != ObjectType.Easel)
			{
				m_IngredientsRoot.transform.localRotation = Quaternion.Euler(0f, 90f, 0f) * Quaternion.Euler(0f, -90f, 0f);
			}
			if ((bool)m_ModelRoot.transform.Find("Blueprint"))
			{
				m_BlueprintModel = m_ModelRoot.transform.Find("Blueprint").gameObject;
			}
			if ((bool)m_ModelRoot.transform.Find("BlueprintBase"))
			{
				m_BlueprintBaseModel = m_ModelRoot.transform.Find("BlueprintBase").gameObject;
			}
		}
		if (m_IngredientsRoot == null)
		{
			m_IngredientsRoot = base.transform;
		}
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		DestroyBubbles();
		DestroySmoke();
		if (m_State != 0)
		{
			SetState(State.Idle);
		}
		base.StopUsing(AndDestroy);
	}

	public override void PlayerDeleted()
	{
		foreach (Holdable ingredient in m_Ingredients)
		{
			ingredient.gameObject.SetActive(value: true);
			TileCoord spawnPointEject = GetSpawnPointEject();
			ingredient.SendAction(new ActionInfo(ActionType.Dropped, spawnPointEject));
			if (ObjectTypeList.Instance.GetModelNameFromIdentifier(ingredient.m_TypeIdentifier) == "" || !ingredient.GetIsSavable() || ToolFillable.GetIsTypeLiquid(ingredient.m_TypeIdentifier))
			{
				ingredient.StopUsing();
				continue;
			}
			SpawnAnimationManager.Instance.StopObjectSpawning(ingredient);
			ingredient.UpdatePositionToTilePosition(m_TileCoord);
			PlotManager.Instance.RemoveObject(ingredient);
			PlotManager.Instance.AddObject(ingredient);
			TileCoord endPosition = m_TileCoord;
			if (!GetIsSavable())
			{
				endPosition = GetNearestAdjacentTile(m_TileCoord);
			}
			SpawnAnimationManager.Instance.AddJump(ingredient, spawnPointEject, endPosition, 0f, ingredient.transform.position.y, 4f);
			PlayResourceSound(m_Ingredients[0].m_TypeIdentifier);
		}
		base.PlayerDeleted();
	}

	public new void Awake()
	{
		base.Awake();
		m_SpawnModel = null;
	}

	protected new void OnDestroy()
	{
		DestroyBubbles();
		DestroySmoke();
		base.OnDestroy();
	}

	public override string GetHumanReadableName()
	{
		if (m_Name != "")
		{
			return m_Name;
		}
		string text = base.GetHumanReadableName();
		if (m_CountIndex != 0)
		{
			text = text + " " + m_CountIndex;
		}
		return text;
	}

	private void UpdateSpawnPosition()
	{
		if ((bool)m_SpawnModel && (m_TileCoord.x != 0 || m_TileCoord.y != 0))
		{
			float heightOffGround = GetSpawnPoint().GetHeightOffGround();
			Vector3 position = m_SpawnModel.transform.position;
			position.y = heightOffGround + Building.m_AccessHeight;
			m_SpawnModel.transform.position = position;
		}
	}

	protected override void TileCoordChanged(TileCoord Position)
	{
		TileCoord tileCoord = m_TileCoord;
		base.TileCoordChanged(Position);
		UpdateSpawnPosition();
		UpdateCoordChanged(Position, tileCoord);
	}

	public void HideSpawnModel(bool Hide = true)
	{
		m_SpawnModel.SetActive(!Hide);
		UpdateTiles();
	}

	public override void UpdateTiles()
	{
		base.UpdateTiles();
		if ((bool)m_SpawnModel && m_SpawnModel.activeSelf)
		{
			m_Tiles.Add(GetSpawnPoint());
		}
	}

	public override void UpdateAccessModelPosition()
	{
		base.UpdateAccessModelPosition();
		if ((bool)m_SpawnModel)
		{
			m_SpawnModel.transform.localPosition = m_SpawnPoint.ToWorldPosition() + new Vector3(0f, Building.m_AccessHeight, 0f);
			int num = 0;
			num = ((m_SpawnPoint.x < m_TopLeft.x) ? 2 : ((m_SpawnPoint.y > m_BottomRight.y) ? 1 : ((m_SpawnPoint.x <= m_BottomRight.x) ? 3 : 0)));
			m_SpawnModel.transform.localRotation = Quaternion.Euler(0f, 90 * (num + 2), 0f);
		}
	}

	protected void SetSpawnPoint(TileCoord SpawnPoint)
	{
		m_SpawnPoint = SpawnPoint;
		UpdateTiles();
		UpdateAccessModelPosition();
		TileCoordChanged(m_TileCoord);
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
		string value = "";
		if (m_Results.Count > 0)
		{
			value = ObjectTypeList.Instance.GetSaveNameFromIdentifier(m_Results[m_ResultsToCreate][0].m_Type);
		}
		JSONUtils.Set(Node, "ToCreateItem", value);
		JSONUtils.Set(Node, "NumCreated", m_NumCreated);
		JSONArray jSONArray = (JSONArray)(Node["IngredientsItems"] = new JSONArray());
		int num = 0;
		for (int i = 0; i < m_Ingredients.Count; i++)
		{
			JSONNode jSONNode2 = new JSONObject();
			if (m_Ingredients[i] == null)
			{
				ErrorMessage.LogError("null ingredient detected in " + GetHumanReadableName() + " at " + m_TileCoord.x + "," + m_TileCoord.y);
			}
			else
			{
				string saveNameFromIdentifier = ObjectTypeList.Instance.GetSaveNameFromIdentifier(m_Ingredients[i].GetComponent<BaseClass>().m_TypeIdentifier);
				JSONUtils.Set(jSONNode2, "ID", saveNameFromIdentifier);
				m_Ingredients[i].GetComponent<Savable>().Save(jSONNode2);
				jSONArray[num] = jSONNode2;
				num++;
			}
		}
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		JSONArray asArray = Node["IngredientsItems"].AsArray;
		if (asArray != null && !asArray.IsNull)
		{
			for (int i = 0; i < asArray.Count; i++)
			{
				JSONNode asObject = asArray[i].AsObject;
				BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromSaveName(asObject, new Vector3(0f, 0f, 0f), Quaternion.identity);
				if ((bool)baseClass)
				{
					BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(baseClass.m_UniqueID, ErrorCheck: false);
					if (objectFromUniqueID != null && objectFromUniqueID.GetComponent<TileCoordObject>().m_Plot != null)
					{
						objectFromUniqueID.StopUsing();
					}
					baseClass.GetComponent<Savable>().Load(asObject);
					AddIngredient(baseClass.GetComponent<Holdable>());
					baseClass.GetComponent<Savable>().SetIsSavable(IsSavable: false);
					baseClass.GetComponent<Actionable>().SendAction(new ActionInfo(ActionType.BeingHeld, default(TileCoord), this));
				}
			}
		}
		UpdateIngredients();
		JSONNode jSONNode = Node["ToCreateItem"];
		if (jSONNode != null && !jSONNode.IsNull)
		{
			SetResultToCreate(0);
			ObjectType identifierFromSaveName = ObjectTypeList.Instance.GetIdentifierFromSaveName(jSONNode, Check: false);
			int resultIndex = GetResultIndex(identifierFromSaveName);
			if (resultIndex != -1)
			{
				SetResultToCreate(resultIndex);
			}
		}
		SetState((State)JSONUtils.GetAsInt(Node, "State", 0));
		m_NumCreated = JSONUtils.GetAsInt(Node, "NumCreated", 0);
	}

	public virtual void SetResultToCreate(int Result)
	{
		m_ResultsToCreate = Result;
		if (m_Results.Count > m_ResultsToCreate && (bool)m_BlueprintModel)
		{
			ObjectType objectType = ObjectType.Nothing;
			if (m_ResultsToCreate != 0)
			{
				objectType = m_Results[Result][0].m_Type;
			}
			if (objectType == ObjectTypeList.m_Total)
			{
				objectType = ObjectType.Nothing;
			}
			Sprite icon = IconManager.Instance.GetIcon(objectType);
			if ((bool)icon)
			{
				MeshRenderer component = m_BlueprintModel.GetComponent<MeshRenderer>();
				component.material.SetTexture("_MainTex", icon.texture);
				StandardShaderUtils.ChangeRenderMode(component.material, StandardShaderUtils.BlendMode.Transparent);
			}
		}
	}

	private int CountAvailableResults(out int Last)
	{
		int num = 0;
		Last = 0;
		for (int i = 1; i < m_Results.Count; i++)
		{
			ObjectType type = m_Results[i][0].m_Type;
			if (!QuestManager.Instance.m_ObjectsLocked.ContainsKey(type) && !CeremonyManager.Instance.IsObjectTypeInCeremonyQueue(type))
			{
				num++;
				Last = i;
			}
		}
		return num;
	}

	public override void SendAction(ActionInfo Info)
	{
		base.SendAction(Info);
		ActionType action = Info.m_Action;
		if ((uint)(action - 41) <= 1u)
		{
			UpdateBubbles();
			UpdateSmoke();
		}
		if (IsBusy() || ((bool)m_Engager && Info.m_Action != ActionType.Disengaged && Info.m_Action != ActionType.SetValue))
		{
			return;
		}
		switch (Info.m_Action)
		{
		case ActionType.Engaged:
			m_Engager = Info.m_Object;
			m_LastEngagerType = m_Engager.m_TypeIdentifier;
			if ((bool)m_Engager.GetComponent<FarmerPlayer>())
			{
				GameStateManager.Instance.StartSelectBuilding(this);
			}
			break;
		case ActionType.Disengaged:
			m_Engager = null;
			break;
		case ActionType.SetValue:
			if (m_Ingredients.Count == 0 && Info.m_Value == m_ResultsToCreateName)
			{
				ObjectType identifierFromSaveName = ObjectTypeList.Instance.GetIdentifierFromSaveName(Info.m_Value2);
				int resultIndex = GetResultIndex(identifierFromSaveName);
				SetResultToCreate(resultIndex);
			}
			break;
		case ActionType.GetValue:
			if (Info.m_Value == m_ResultsToCreateName)
			{
				Info.m_Value2 = m_ResultsToCreate.ToString();
			}
			break;
		case ActionType.Refresh:
		case ActionType.RefreshFirst:
			if (m_Results.Count == 2)
			{
				SetResultToCreate(1);
			}
			UpdateBubbles();
			UpdateSmoke();
			if (SaveLoadManager.Instance.m_Loading && m_State == State.Idle && m_Ingredients.Count > 0 && AreRequrementsMet() && m_AutoConvert && m_ResultsToCreate != 0)
			{
				StartConversion(null);
			}
			UpdateSpawnPosition();
			break;
		case ActionType.SetBagged:
		case ActionType.SetUnbagged:
		case ActionType.Recharge:
			break;
		}
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
			if ((m_Ingredients.Count == 0 || GameStateManager.Instance.GetCurrentState().GetComponent<GameStateTeachWorker2>() == null) && m_Engager == null)
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
		case ActionType.SetValue:
			if (Info.m_Value == m_ResultsToCreateName)
			{
				return true;
			}
			break;
		case ActionType.GetValue:
			if (Info.m_Value == m_ResultsToCreateName)
			{
				return true;
			}
			break;
		}
		return false;
	}

	public override object GetActionInfo(GetActionInfo Info)
	{
		switch (Info.m_Action)
		{
		case GetAction.IsDeletable:
		case GetAction.IsMovable:
			if (IsBusy())
			{
				return false;
			}
			break;
		case GetAction.IsBusy:
			if (IsBusy())
			{
				return true;
			}
			return m_DoingAction;
		}
		return base.GetActionInfo(Info);
	}

	protected virtual void StartAddAnything(AFO Info)
	{
		Actionable actionable = Info.m_Object;
		ObjectType typeIdentifier = actionable.m_TypeIdentifier;
		if ((bool)actionable.GetComponent<ToolFillable>() && actionable.GetComponent<ToolFillable>().m_HeldType != ObjectTypeList.m_Total)
		{
			typeIdentifier = actionable.GetComponent<ToolFillable>().m_HeldType;
			actionable.GetComponent<ToolFillable>().Empty(1);
			actionable = ObjectTypeList.Instance.CreateObjectFromIdentifier(typeIdentifier, base.transform.position, Quaternion.identity).GetComponent<Actionable>();
			DespawnManager.Instance.Remove(actionable.GetComponent<Holdable>());
		}
		if (Fish.GetIsTypeFish(Info.m_ObjectType))
		{
			typeIdentifier = ObjectType.FishAny;
		}
		if (FolkHeart.GetIsFolkHeart(Info.m_ObjectType))
		{
			typeIdentifier = ObjectType.HeartAny;
		}
		actionable.GetComponent<Savable>().SetIsSavable(IsSavable: false);
		AddIngredient(actionable.GetComponent<Holdable>());
		m_LastAddedIngredient = actionable.GetComponent<Holdable>();
	}

	protected virtual void EndAddAnything(AFO Info)
	{
		Actionable @object = Info.m_Object;
		if ((bool)@object)
		{
			@object.SendAction(new ActionInfo(ActionType.BeingHeld, default(TileCoord), this));
		}
		AudioManager.Instance.StartEvent("BuildingIngredientAdd", this);
		UpdateIngredients();
		if (!SaveLoadManager.Instance.m_Loading && AreRequrementsMet() && m_AutoConvert)
		{
			StartConversion(Info.m_Actioner);
		}
	}

	protected void AbortAddAnything(AFO Info)
	{
		Actionable @object = Info.m_Object;
		_ = @object.m_TypeIdentifier;
		if ((bool)m_LastAddedIngredient)
		{
			RemoveIngredient(m_LastAddedIngredient);
			if ((bool)@object.GetComponent<ToolFillable>() && m_LastAddedIngredient != @object)
			{
				m_LastAddedIngredient.StopUsing();
			}
			if (ToolFillable.GetIsTypeFillable(m_LastAddedIngredient.m_TypeIdentifier) && (bool)Info.m_Actioner && Info.m_Actioner.m_TypeIdentifier == ObjectType.Worker)
			{
				Info.m_Actioner.GetComponent<Farmer>().m_FarmerAction.UndoAdd(@object.GetComponent<Actionable>());
			}
		}
		if (!@object.GetComponent<ToolFillable>() || !(m_LastAddedIngredient != @object))
		{
			@object.GetComponent<Savable>().SetIsSavable(IsSavable: true);
		}
	}

	protected override ActionType GetActionFromAnything(AFO Info)
	{
		ObjectType type = Info.m_ObjectType;
		Info.m_StartAction = StartAddAnything;
		Info.m_EndAction = EndAddAnything;
		Info.m_AbortAction = AbortAddAnything;
		Info.m_FarmerState = Farmer.State.Adding;
		if (m_WallMissing || m_FloorMissing)
		{
			return ActionType.Fail;
		}
		if (Info.m_Object != null)
		{
			if (ToolFillable.GetIsTypeFillable(Info.m_Object.m_TypeIdentifier) && Info.m_Object.GetComponent<ToolFillable>().m_HeldType != ObjectTypeList.m_Total)
			{
				type = Info.m_Object.GetComponent<ToolFillable>().m_HeldType;
			}
			if (IsIngredientNeeded(ObjectType.FishAny) && Fish.GetIsTypeFish(Info.m_ObjectType))
			{
				type = ObjectType.FishAny;
			}
			if (IsIngredientNeeded(ObjectType.HeartAny) && FolkHeart.GetIsFolkHeart(Info.m_ObjectType))
			{
				type = ObjectType.HeartAny;
			}
		}
		if ((bool)m_Engager)
		{
			return ActionType.Fail;
		}
		if (IsBusy())
		{
			return ActionType.Fail;
		}
		if (IsBeingUpgraded())
		{
			return ActionType.Fail;
		}
		if (!CanAcceptIngredient(type))
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
			if (m_State != 0)
			{
				return ActionType.Fail;
			}
			if ((m_Ingredients.Count == 0 || GameStateManager.Instance.GetActualState() != GameStateManager.State.TeachWorker) && m_Engager == null)
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
		return base.GetActionFromObject(Info);
	}

	protected virtual void SetState(State NewState)
	{
		State state = m_State;
		if (state == State.Converting)
		{
			if ((bool)m_ObjectInvolved)
			{
				m_ObjectInvolved.SendAction(new ActionInfo(ActionType.DisengageObject, default(TileCoord), this));
			}
			EndConverting();
		}
		m_State = NewState;
		m_StateTimer = 0f;
		state = m_State;
		if (state == State.Converting)
		{
			if ((bool)m_ObjectInvolved)
			{
				m_ObjectInvolved.SendAction(new ActionInfo(ActionType.EngageObject, default(TileCoord), this));
			}
			StartConverting();
		}
		ModManager.Instance.CheckBuildingStateChangedCallback(m_UniqueID, m_State.ToString());
	}

	protected void AddRequirements(List<IngredientRequirement> Requirements, List<IngredientRequirement> Results)
	{
		if (Requirements == null)
		{
			Requirements = new List<IngredientRequirement>();
			if (Results[0].m_Type != ObjectTypeList.m_Total)
			{
				IngredientRequirement[] ingredientsFromIdentifier = ObjectTypeList.Instance.GetIngredientsFromIdentifier(Results[0].m_Type);
				for (int i = 0; i < ingredientsFromIdentifier.Length; i++)
				{
					Requirements.Add(new IngredientRequirement(ingredientsFromIdentifier[i].m_Type, ingredientsFromIdentifier[i].m_Count));
				}
			}
		}
		m_Requirements.Add(Requirements);
		m_Results.Add(Results);
		FindFirstObject();
	}

	protected void ChangeRequirementsForStage(int Index, int Stage)
	{
		m_Requirements[Index] = new List<IngredientRequirement>();
		List<IngredientRequirement> list = m_Requirements[Index];
		IngredientRequirement[] ingredientsFromIdentifier = ObjectTypeList.Instance.GetIngredientsFromIdentifier(m_Results[Index][0].m_Type, Stage);
		for (int i = 0; i < ingredientsFromIdentifier.Length; i++)
		{
			list.Add(new IngredientRequirement(ingredientsFromIdentifier[i].m_Type, ingredientsFromIdentifier[i].m_Count));
		}
	}

	public bool IsIngredientNeeded(ObjectType Type)
	{
		if (m_ResultsToCreate >= m_Requirements.Count)
		{
			return false;
		}
		foreach (IngredientRequirement item in m_Requirements[m_ResultsToCreate])
		{
			if (item.m_Type == Type)
			{
				if (GetIngredientCount(Type) < item.m_Count)
				{
					return true;
				}
				return false;
			}
		}
		return false;
	}

	public virtual bool CanAcceptIngredient(ObjectType Type)
	{
		if (IsBusy())
		{
			return false;
		}
		return IsIngredientNeeded(Type);
	}

	public virtual void AddIngredient(Holdable NewIngredient)
	{
		if (!IsBusy())
		{
			BaseClass.TestObject(NewIngredient);
			NewIngredient.transform.position = GetIngredientPosition(NewIngredient);
			m_Ingredients.Add(NewIngredient);
		}
	}

	public void RemoveIngredient(Holdable NewIngredient)
	{
		m_Ingredients.Remove(NewIngredient);
	}

	public void StartConversion(Actionable ObjectInvolved)
	{
		m_ObjectInvolved = ObjectInvolved;
		if ((bool)m_ObjectInvolved)
		{
			m_LastEngagerType = m_ObjectInvolved.m_TypeIdentifier;
		}
		SetState(State.Converting);
	}

	public void ResumeConversion(Actionable ObjectInvolved)
	{
		m_ObjectInvolved = ObjectInvolved;
	}

	protected virtual Vector3 GetIngredientPosition(BaseClass NewObject)
	{
		Vector3 vector = default(Vector3);
		if (m_DisplayIngredients)
		{
			float num = 0f;
			foreach (Holdable ingredient in m_Ingredients)
			{
				if (ingredient == null)
				{
					ErrorMessage.LogError("null ingredient detected in " + GetHumanReadableName() + " at " + m_TileCoord.x + "," + m_TileCoord.y);
				}
				else
				{
					float height = ObjectTypeList.Instance.GetHeight(ingredient.m_TypeIdentifier);
					num += height * ingredient.transform.localScale.y;
				}
			}
			vector = m_IngredientsRoot.transform.position;
			vector.y += num;
		}
		else
		{
			vector = base.transform.position;
		}
		return vector;
	}

	protected virtual void UpdateIngredients()
	{
		if (m_DisplayIngredients)
		{
			float num = 0f;
			foreach (Holdable ingredient in m_Ingredients)
			{
				ingredient.SendAction(new ActionInfo(ActionType.Show, default(TileCoord), this));
				ingredient.transform.parent = m_IngredientsRoot;
				ingredient.transform.localPosition = new Vector3(0f, num, 0f);
				ingredient.transform.localRotation = Quaternion.Euler(0f, Random.Range(0, 360), 0f);
				float height = ObjectTypeList.Instance.GetHeight(ingredient.m_TypeIdentifier);
				num += height * ingredient.transform.localScale.y;
			}
			m_IngredientsHeight = num;
			return;
		}
		foreach (Holdable ingredient2 in m_Ingredients)
		{
			ingredient2.SendAction(new ActionInfo(ActionType.Hide, default(TileCoord), this));
		}
	}

	public virtual void StartConverting()
	{
		if ((bool)m_Animator)
		{
			m_Animator.Play("Converting", -1, 0f);
		}
	}

	protected virtual void UpdateConverting()
	{
	}

	protected virtual void EndConverting()
	{
		ModManager.Instance.CheckCustomCallback(ModManager.CallbackTypes.ConverterComplete, m_TypeIdentifier, m_TileCoord, -1, m_UniqueID);
		if ((bool)m_Animator)
		{
			m_Animator.Play("Idle", -1, 0f);
		}
	}

	public int GetIngredientCount(ObjectType NewType)
	{
		int num = 0;
		foreach (Holdable ingredient in m_Ingredients)
		{
			if (ingredient.m_TypeIdentifier == NewType)
			{
				num++;
			}
			else if (NewType == ObjectType.FishAny && Fish.GetIsTypeFish(ingredient.m_TypeIdentifier))
			{
				num++;
			}
			else if (NewType == ObjectType.HeartAny && FolkHeart.GetIsFolkHeart(ingredient.m_TypeIdentifier))
			{
				num++;
			}
			else if (NewType == ObjectType.WorkerFrameAny && WorkerFrame.GetIsTypeFrame(ingredient.m_TypeIdentifier))
			{
				num++;
			}
			else if (NewType == ObjectType.WorkerHeadAny && WorkerHead.GetIsTypeHead(ingredient.m_TypeIdentifier))
			{
				num++;
			}
			else if (NewType == ObjectType.WorkerDriveAny && WorkerDrive.GetIsTypeDrive(ingredient.m_TypeIdentifier))
			{
				num++;
			}
			else if (NewType == ObjectType.HatAny && Hat.GetIsTypeHat(ingredient.m_TypeIdentifier))
			{
				num++;
			}
			else if (NewType == ObjectType.TopAny && Top.GetIsTypeTop(ingredient.m_TypeIdentifier))
			{
				num++;
			}
		}
		return num;
	}

	public virtual bool AreRequrementsMet()
	{
		if (m_Requirements.Count <= m_ResultsToCreate)
		{
			return false;
		}
		foreach (IngredientRequirement item in m_Requirements[m_ResultsToCreate])
		{
			if (GetIngredientCount(item.m_Type) != item.m_Count)
			{
				return false;
			}
		}
		return true;
	}

	private void FindFirstObject()
	{
	}

	public override void SetBlueprint(bool Blueprint, bool Register = false)
	{
		base.SetBlueprint(Blueprint, Register);
		if ((bool)m_BlueprintModel)
		{
			m_BlueprintModel.SetActive(!Blueprint);
		}
		if ((bool)m_BlueprintBaseModel)
		{
			m_BlueprintBaseModel.SetActive(!Blueprint);
		}
		if (Blueprint)
		{
			m_SpawnModel.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 0.25f);
		}
		else
		{
			ModelManager.Instance.RestoreStandardMaterials(ObjectTypeList.m_Total, m_SpawnModel, "Models/Buildings/BuildingSpawnPoint");
		}
		FindFirstObject();
		if ((bool)m_BubblesParticles)
		{
			m_BubblesParticles.gameObject.SetActive(!Blueprint);
		}
		if ((bool)m_SmokeParticles)
		{
			m_SmokeParticles.gameObject.SetActive(!Blueprint);
		}
	}

	protected bool WillRequrementsBeMet(int ResultsIndex, ObjectType NewObject)
	{
		foreach (IngredientRequirement item in m_Requirements[ResultsIndex])
		{
			int num = GetIngredientCount(item.m_Type);
			if (item.m_Type == NewObject)
			{
				num++;
			}
			if (num < item.m_Count)
			{
				return false;
			}
		}
		return true;
	}

	public override TileCoord GetSpawnPoint()
	{
		TileCoord tileCoord = default(TileCoord);
		tileCoord.Copy(m_SpawnPoint);
		tileCoord.Rotate(m_Rotation);
		return tileCoord + m_TileCoord;
	}

	public TileCoord GetSpawnPointEject()
	{
		TileCoord tileCoord = default(TileCoord);
		tileCoord.Copy(m_SpawnPoint);
		if (m_SpawnPoint.x > m_BottomRight.x)
		{
			tileCoord.x--;
		}
		else if (m_SpawnPoint.x < m_TopLeft.x)
		{
			tileCoord.x++;
		}
		else if (m_SpawnPoint.y > m_BottomRight.y)
		{
			tileCoord.y--;
		}
		else if (m_SpawnPoint.y < m_TopLeft.y)
		{
			tileCoord.y++;
		}
		tileCoord.Rotate(m_Rotation);
		return tileCoord + m_TileCoord;
	}

	protected void PlayResourceSound(ObjectType NewType)
	{
		AudioManager.Instance.StartEvent("ObjectCreated", this);
	}

	protected virtual void FinishNewObject(BaseClass NewObject)
	{
	}

	private BaseClass IngredientsGetContainer()
	{
		for (int i = 0; i < m_Ingredients.Count; i++)
		{
			ObjectType typeIdentifier = m_Ingredients[i].m_TypeIdentifier;
			if (typeIdentifier == ObjectType.PotClay || typeIdentifier == ObjectType.LargeBowlClay || ToolFillable.GetIsTypeFillable(typeIdentifier))
			{
				return m_Ingredients[i];
			}
		}
		return null;
	}

	private void CheckForContainer(BaseClass NewObject)
	{
		BaseClass baseClass = IngredientsGetContainer();
		if ((bool)baseClass)
		{
			NewObject.GetComponent<Holdable>().m_UsageCount = baseClass.GetComponent<Holdable>().m_UsageCount;
		}
	}

	protected virtual BaseClass CreateNewItem()
	{
		if (m_Results[m_ResultsToCreate][m_TypeCreated].m_Type == ObjectTypeList.m_Total)
		{
			m_TypeCreated++;
			return null;
		}
		TileCoord spawnPoint = GetSpawnPoint();
		Vector3 position = spawnPoint.ToWorldPositionTileCentered();
		BaseClass baseClass = null;
		ObjectType type = m_Results[m_ResultsToCreate][m_TypeCreated].m_Type;
		if (ToolFillable.GetIsTypeLiquid(type) && type != ObjectType.Mortar)
		{
			baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.ToolBucket, position, Quaternion.identity);
			int capacity = baseClass.GetComponent<ToolFillable>().m_Capacity;
			baseClass.GetComponent<ToolFillable>().Fill(m_Results[m_ResultsToCreate][m_TypeCreated].m_Type, capacity);
		}
		else
		{
			baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(m_Results[m_ResultsToCreate][m_TypeCreated].m_Type, position, Quaternion.identity);
			FinishNewObject(baseClass);
		}
		CheckForContainer(baseClass);
		TileCoord spawnPointEject = GetSpawnPointEject();
		SpawnAnimationManager.Instance.AddJump(baseClass, spawnPointEject, spawnPoint, 0f, baseClass.transform.position.y, 4f, 0.2f, null, DustLand: false, this);
		PlayResourceSound(m_Results[m_ResultsToCreate][m_TypeCreated].m_Type);
		m_NumCreated++;
		if (m_NumCreated >= m_Results[m_ResultsToCreate][m_TypeCreated].m_Count)
		{
			m_NumCreated = 0;
			m_TypeCreated++;
		}
		bool bot = false;
		if (m_LastEngagerType == ObjectType.Worker)
		{
			bot = true;
		}
		switch (baseClass.m_TypeIdentifier)
		{
		case ObjectType.ToolAxeStone:
		case ObjectType.ToolShovelStone:
		case ObjectType.ToolPickStone:
		case ObjectType.ToolHoeStone:
		case ObjectType.ToolScytheStone:
		case ObjectType.ToolFlail:
			QuestManager.Instance.AddEvent(QuestEvent.Type.MakeCrudeTool, bot, 0, baseClass);
			break;
		case ObjectType.ToolAxe:
		case ObjectType.ToolShovel:
		case ObjectType.ToolPick:
		case ObjectType.ToolHoe:
		case ObjectType.ToolScythe:
			QuestManager.Instance.AddEvent(QuestEvent.Type.MakeCrudeMetalTool, bot, 0, baseClass);
			break;
		}
		if (MyTool.GetIsTypeTool(baseClass.m_TypeIdentifier))
		{
			BadgeManager.Instance.AddEvent(BadgeEvent.Type.Tools);
		}
		QuestManager.Instance.AddEvent(QuestEvent.Type.Make, bot, baseClass.m_TypeIdentifier, baseClass);
		QuestManager.Instance.AddEvent(QuestEvent.Type.MakeConverter, bot, baseClass.m_TypeIdentifier, baseClass);
		if (baseClass.m_TypeIdentifier == ObjectType.ToolFishingRodGood)
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.Make, bot, ObjectType.ToolFishingRod, baseClass);
		}
		if (Hat.GetIsTypeHat(baseClass.m_TypeIdentifier))
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.MakeHat, bot, 0, baseClass);
		}
		if (Top.GetIsTypeTop(baseClass.m_TypeIdentifier))
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.MakeTop, bot, 0, baseClass);
		}
		if (Clothing.GetIsTypeClothing(baseClass.m_TypeIdentifier))
		{
			BadgeManager.Instance.AddEvent(BadgeEvent.Type.Clothes);
		}
		if (Food.GetIsTypeCookedFood(baseClass.m_TypeIdentifier))
		{
			BadgeManager.Instance.AddEvent(BadgeEvent.Type.Food);
		}
		if (baseClass.m_TypeIdentifier == ObjectType.Folk)
		{
			BadgeManager.Instance.AddEvent(BadgeEvent.Type.Colonists);
		}
		return baseClass;
	}

	public ObjectType GetFirstRequiredIngredient(int ResultsIndex)
	{
		foreach (IngredientRequirement item in m_Requirements[ResultsIndex])
		{
			if (GetIngredientCount(item.m_Type) != item.m_Count)
			{
				return item.m_Type;
			}
		}
		return ObjectTypeList.m_Total;
	}

	public virtual void CancelBuild()
	{
		SetState(State.Cancelling);
	}

	protected void CreateCancelledItem()
	{
		if (m_Ingredients.Count > 0)
		{
			Holdable holdable = m_Ingredients[0];
			holdable.gameObject.SetActive(value: true);
			TileCoord spawnPointEject = GetSpawnPointEject();
			holdable.SendAction(new ActionInfo(ActionType.Dropped, spawnPointEject));
			if (ObjectTypeList.Instance.GetModelNameFromIdentifier(holdable.m_TypeIdentifier) == "" || !holdable.GetIsSavable() || ToolFillable.GetIsTypeLiquid(holdable.m_TypeIdentifier) || holdable.m_TypeIdentifier == ObjectType.PaintBlue || holdable.m_TypeIdentifier == ObjectType.PaintRed || holdable.m_TypeIdentifier == ObjectType.PaintYellow)
			{
				holdable.StopUsing();
			}
			else
			{
				TileCoord tileCoord = ((m_TypeIdentifier != ObjectType.ConverterFoundation) ? GetSpawnPoint() : TileHelpers.GetRandomEmptyTile(m_TileCoord));
				holdable.UpdatePositionToTilePosition(tileCoord);
				PlotManager.Instance.RemoveObject(holdable);
				PlotManager.Instance.AddObject(holdable);
				SpawnAnimationManager.Instance.AddJump(holdable, spawnPointEject, tileCoord, 0f, holdable.transform.position.y, 4f);
				holdable.ForceHighlight(Highlighted: false);
				PlayResourceSound(m_Ingredients[0].m_TypeIdentifier);
			}
			m_Ingredients.RemoveAt(0);
		}
		UpdateIngredients();
	}

	protected void CheckBurnedIngredients()
	{
		for (int i = 0; i < m_Ingredients.Count; i++)
		{
			if (BurnableFuel.GetFuelTier(m_Ingredients[i].m_TypeIdentifier) == BurnableInfo.Tier.Crude)
			{
				QuestManager.Instance.AddEvent(QuestEvent.Type.BurnWood, Bot: false, 0, this);
			}
		}
	}

	protected void DestroyIngredients()
	{
		List<Holdable> list = new List<Holdable>();
		for (int i = 0; i < m_Ingredients.Count; i++)
		{
			list.Add(m_Ingredients[i]);
		}
		m_Ingredients.Clear();
		foreach (Holdable item in list)
		{
			item.StopUsing();
		}
		UpdateIngredients();
	}

	protected void Update()
	{
		switch (m_State)
		{
		case State.Converting:
			UpdateConverting();
			if (m_StateTimer >= m_ConversionDelay)
			{
				m_NumCreated = 0;
				m_TypeCreated = 0;
				for (int i = 0; i < m_Ingredients.Count; i++)
				{
					m_Ingredients[i].gameObject.SetActive(value: false);
				}
				SetState(State.Creating);
				m_StateTimer = m_CreateDelay;
			}
			break;
		case State.Creating:
			if (m_StateTimer >= m_CreateDelay)
			{
				m_StateTimer = 0f;
				BaseClass baseClass = CreateNewItem();
				AddAnimationManager.Instance.Add(this, Adding: false);
				DestroyIngredients();
				int objectUniqueID = -1;
				if (baseClass != null)
				{
					objectUniqueID = baseClass.m_UniqueID;
				}
				ModManager.Instance.CheckCustomCallback(ModManager.CallbackTypes.ConverterCreateItem, m_TypeIdentifier, m_TileCoord, objectUniqueID, m_UniqueID);
				if (m_TypeCreated == m_Results[m_ResultsToCreate].Count)
				{
					SetState(State.Idle);
				}
			}
			break;
		case State.Cancelling:
			if (!(m_StateTimer >= m_CancelDelay))
			{
				break;
			}
			m_StateTimer = 0f;
			CreateCancelledItem();
			AddAnimationManager.Instance.Add(this, Adding: false);
			if (m_Ingredients.Count != 0)
			{
				break;
			}
			if (m_TypeIdentifier == ObjectType.ConverterFoundation)
			{
				if ((bool)m_ParentBuilding)
				{
					m_ParentBuilding.DeleteFoundations();
				}
				else
				{
					DeleteFoundations();
				}
			}
			else
			{
				SetState(State.Idle);
			}
			break;
		}
		float normalDelta = TimeManager.Instance.m_NormalDelta;
		m_StateTimer += normalDelta;
	}

	public bool CanMakeObject(ObjectType NewType)
	{
		foreach (List<IngredientRequirement> result in m_Results)
		{
			foreach (IngredientRequirement item in result)
			{
				if (item.m_Type == NewType)
				{
					return true;
				}
			}
		}
		return false;
	}

	public List<IngredientRequirement> GetUnlockedResults()
	{
		Dictionary<ObjectType, int> objectsLocked = QuestManager.Instance.m_ObjectsLocked;
		List<IngredientRequirement> list = new List<IngredientRequirement>();
		for (int i = 0; i < m_Results.Count; i++)
		{
			ObjectType type = m_Results[i][0].m_Type;
			if (!objectsLocked.ContainsKey(type) && !CeremonyManager.Instance.IsObjectTypeInCeremonyQueue(type) && type != ObjectTypeList.m_Total)
			{
				list.Add(m_Results[i][0]);
			}
		}
		return list;
	}

	public int GetResultIndex(ObjectType NewType)
	{
		for (int i = 0; i < m_Results.Count; i++)
		{
			if (m_Results[i][0].m_Type == NewType)
			{
				return i;
			}
		}
		return -1;
	}

	public ObjectType GetResultType()
	{
		if (m_ResultsToCreate == -1 || m_ResultsToCreate >= m_Results.Count)
		{
			return ObjectTypeList.m_Total;
		}
		return m_Results[m_ResultsToCreate][0].m_Type;
	}

	public bool GetIsResultCreated()
	{
		if (m_State == State.Converting || m_State == State.Creating)
		{
			return true;
		}
		ObjectType objectType = m_Results[m_ResultsToCreate][0].m_Type;
		if (objectType == ObjectType.BasicWorker)
		{
			objectType = ObjectType.Worker;
		}
		TileCoord spawnPoint = GetSpawnPoint();
		foreach (TileCoordObject item in PlotManager.Instance.GetPlotAtTile(spawnPoint).GetObjectsTypeAtTile(objectType, spawnPoint))
		{
			if (!BaggedManager.Instance.IsObjectBagged(item) || objectType == ObjectType.Worker)
			{
				return true;
			}
		}
		return false;
	}

	protected void UpdateConversionDelay()
	{
		float num = 1f / m_BaseConversionDelay;
		float num2 = num;
		if (m_ConnectedToTransmitter)
		{
			num2 += num * 0.25f;
		}
		m_ConversionDelay = 1f / num2;
	}

	protected override void ConnectionToTransmitterChanged()
	{
		base.ConnectionToTransmitterChanged();
		UpdateConversionDelay();
	}

	public void ShowIndicator(bool Show)
	{
		if (Show)
		{
			if (!m_Indicator)
			{
				m_Indicator = AreaIndicatorManager.Instance.Add();
				m_Indicator.SetSign(this);
				m_Indicator.SetActive(Active: true);
				m_Indicator.Scale(Up: true);
			}
			else
			{
				m_Indicator.SetActive(Active: true);
				m_Indicator.Scale(Up: true);
			}
		}
		else if ((bool)m_Indicator)
		{
			m_Indicator.Scale(Up: false);
			m_Indicator = null;
		}
	}

	public void UpdateCoordChanged(TileCoord NewPosition, TileCoord OldPosition)
	{
		if ((bool)m_Indicator)
		{
			m_Indicator.UpdateArea();
		}
	}

	protected void CreateBubbles(float Height)
	{
		if (m_BubblesParticles == null)
		{
			if ((bool)ParticlesManager.Instance)
			{
				m_BubblesParticles = ParticlesManager.Instance.CreateParticles("CauldronSimmer", default(Vector3), Quaternion.identity);
			}
			m_BubblesHeight = Height;
		}
	}

	protected void DestroyBubbles()
	{
		if ((bool)m_BubblesParticles)
		{
			ParticlesManager.Instance.DestroyParticles(m_BubblesParticles);
			m_BubblesParticles = null;
		}
	}

	protected void UpdateBubbles()
	{
		if ((bool)m_BubblesParticles)
		{
			m_BubblesParticles.transform.position = base.transform.position + new Vector3(0f, m_BubblesHeight, 0f);
			m_BubblesParticles.transform.rotation = Quaternion.Euler(-90f, 0f, 90f);
		}
	}

	protected void CreateSmoke()
	{
		if (m_SmokeParticles == null && (bool)ParticlesManager.Instance)
		{
			m_SmokeParticles = ParticlesManager.Instance.CreateParticles("CauldronSteam", default(Vector3), Quaternion.identity);
		}
	}

	protected void DestroySmoke()
	{
		if ((bool)m_SmokeParticles)
		{
			ParticlesManager.Instance.DestroyParticles(m_SmokeParticles);
			m_SmokeParticles = null;
		}
	}

	protected void UpdateSmoke()
	{
		if ((bool)m_SmokeParticles)
		{
			m_SmokeParticles.transform.position = base.transform.position + new Vector3(0f, 1.5f, 0f);
			m_SmokeParticles.transform.rotation = Quaternion.Euler(0f, 45f, 0f);
		}
	}

	public virtual void DoConvertAnimAction()
	{
	}

	protected MyParticles DoConvertAnimActionParticles(string ParticleName)
	{
		float y = Random.Range(0f, m_IngredientsHeight);
		Vector3 localPosition = m_IngredientsRoot.position + new Vector3(0f, y, 0f);
		float num = Random.Range(60, 120);
		if (Random.Range(0, 2) == 0)
		{
			num += 180f;
		}
		MyParticles myParticles = ParticlesManager.Instance.CreateParticles(ParticleName, localPosition, Quaternion.Euler(-30f, num, 0f));
		ParticlesManager.Instance.DestroyParticles(myParticles, WaitUntilNoParticles: true);
		return myParticles;
	}

	protected void UpdateConvertAnimTimer(float Delay)
	{
		m_AnimConvertTimer += TimeManager.Instance.m_NormalDelta;
		if (m_AnimConvertTimer > Delay)
		{
			m_AnimConvertTimer -= Delay;
			DoConvertAnimAction();
		}
	}

	protected void ConvertScaleWobble()
	{
		if ((int)(m_StateTimer * 60f) % 20 < 10)
		{
			m_ModelRoot.transform.localScale = new Vector3(0.9f, 1.3f, 0.9f);
		}
		else
		{
			m_ModelRoot.transform.localScale = new Vector3(1f, 1f, 1f);
		}
	}

	protected void EndScaleWobble()
	{
		m_ModelRoot.transform.localScale = new Vector3(1f, 1f, 1f);
	}

	protected void ConvertVibrate()
	{
		if ((int)(m_StateTimer * 60f) % 8 < 4)
		{
			m_ModelRoot.transform.localPosition = new Vector3(0f, 0f, 0f);
		}
		else
		{
			m_ModelRoot.transform.localPosition = new Vector3(0f, 0.2f, 0f);
		}
	}

	protected void EndVibrate()
	{
		m_ModelRoot.transform.localPosition = new Vector3(0f, 0f, 0f);
	}

	protected float GetConversionPercent()
	{
		return m_StateTimer / m_ConversionDelay;
	}

	protected void StartIngredientsDown()
	{
		m_IngredientScales = new Vector3[m_Ingredients.Count];
		int num = 0;
		foreach (Holdable ingredient in m_Ingredients)
		{
			if (ingredient.m_TypeIdentifier != ObjectType.FolkSeed)
			{
				m_IngredientScales[num] = ingredient.transform.localScale;
			}
			num++;
		}
	}

	protected void MoveIngredientsDown()
	{
		float num = 0f - GetConversionPercent() * m_IngredientsHeight;
		int num2 = 0;
		foreach (Holdable ingredient in m_Ingredients)
		{
			if (ingredient.m_TypeIdentifier != ObjectType.FolkSeed)
			{
				ingredient.SendAction(new ActionInfo(ActionType.Show, default(TileCoord), this));
				ingredient.transform.parent = m_IngredientsRoot;
				ingredient.transform.localPosition = new Vector3(0f, num, 0f);
				float height = ObjectTypeList.Instance.GetHeight(ingredient.m_TypeIdentifier);
				if (num < 0f - height)
				{
					ingredient.gameObject.SetActive(value: false);
				}
				float num3 = 1f - (0f - num) / height;
				if (num3 > 1f)
				{
					num3 = 1f;
				}
				if (num3 < 0f)
				{
					num3 = 0f;
				}
				ingredient.transform.localScale = m_IngredientScales[num2] * num3;
				num += height * m_IngredientScales[num2].y;
			}
			num2++;
		}
	}

	protected void EndIngredientsDown()
	{
	}

	protected void SpinIngredients(float Speed)
	{
		foreach (Holdable ingredient in m_Ingredients)
		{
			ingredient.transform.rotation = ingredient.transform.rotation * Quaternion.Euler(0f, Speed * TimeManager.Instance.m_NormalDelta, 0f);
		}
	}

	protected void SquashIngredients(float Scale = 1f)
	{
		m_SquashScale = Scale;
		float num = 0.8f * m_SquashScale;
		float num2 = 0f;
		foreach (Holdable ingredient in m_Ingredients)
		{
			ingredient.transform.localScale = new Vector3(1.2f, num, 1.2f);
			ingredient.transform.localPosition = new Vector3(0f, num2, 0f);
			num2 += ObjectTypeList.Instance.GetHeight(ingredient.m_TypeIdentifier) * num;
		}
		m_SquashIngredientsTimer = 0.1f;
	}

	protected void UpdateSquashIngredients()
	{
		m_SquashIngredientsTimer -= TimeManager.Instance.m_NormalDelta;
		if (m_SquashIngredientsTimer < 0f)
		{
			m_SquashIngredientsTimer = 0f;
			EndSquashIngredients();
		}
	}

	protected void EndSquashIngredients()
	{
		float num = 0f;
		foreach (Holdable ingredient in m_Ingredients)
		{
			ingredient.transform.localScale = new Vector3(1f, m_SquashScale, 1f);
			ingredient.transform.localPosition = new Vector3(0f, num, 0f);
			num += ObjectTypeList.Instance.GetHeight(ingredient.m_TypeIdentifier) * m_SquashScale;
		}
	}

	protected void UpdateFunnel(GameObject Funnel)
	{
		if ((bool)Funnel)
		{
			if ((int)(m_StateTimer * 60f) % 12 < 6)
			{
				Funnel.transform.localScale = new Vector3(1.2f, 0.8f, 1.2f);
			}
			else
			{
				Funnel.transform.localScale = new Vector3(0.8f, 1.2f, 0.8f);
			}
		}
	}

	protected void EndFunnel(GameObject Funnel)
	{
		if ((bool)Funnel)
		{
			Funnel.transform.localScale = new Vector3(1f, 1f, 1f);
		}
	}

	public void UpdateModIngredients()
	{
		UpdateIngredients();
	}
}
