using UnityEngine;

public class SpacePort : Wonder
{
	public enum State
	{
		Idle,
		Incinerating,
		Total
	}

	public State m_State;

	private float m_StateTimer;

	private PlaySound m_PlaySound;

	private Transform m_IngredientsRoot;

	public SpacePortRocket m_Rocket;

	protected TileCoord m_SpawnPoint;

	[HideInInspector]
	public GameObject m_SpawnModel;

	private GameObject m_BlueprintModel;

	private GameObject m_Lights;

	private Material m_LightsMaterial;

	private float m_LightsTimer;

	private ObjectType m_BlueprintType;

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(-1, -3), new TileCoord(2, 0), new TileCoord(2, 1));
		m_SpawnPoint = new TileCoord(-1, 1);
		SetState(State.Idle);
		ChangeAccessPointToIn();
	}

	protected new void Awake()
	{
		base.Awake();
		m_SpawnModel = null;
	}

	public override void PostCreate()
	{
		base.PostCreate();
		ChangeAccessPointToIn();
		m_SpawnModel = ModelManager.Instance.Instantiate(ObjectTypeList.m_Total, "Models/Buildings/BuildingSpawnPoint", base.transform, RandomVariants: false);
		UpdateAccessModelPosition();
		m_IngredientsRoot = m_ModelRoot.transform.Find("IngredientsPoint");
		Transform transform = ObjectUtils.FindDeepChild(m_ModelRoot.transform, "Blueprint");
		if ((bool)transform)
		{
			m_BlueprintModel = transform.gameObject;
		}
		transform = ObjectUtils.FindDeepChild(m_ModelRoot.transform, "Lights");
		if ((bool)transform)
		{
			m_Lights = transform.gameObject;
			m_LightsMaterial = m_Lights.GetComponent<MeshRenderer>().material;
		}
		if ((bool)CollectionManager.Instance)
		{
			CollectionManager.Instance.AddCollectable("SpacePort", this);
		}
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		if ((bool)m_Rocket)
		{
			m_Rocket.StopUsing();
			m_Rocket = null;
		}
		base.StopUsing(AndDestroy);
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

	private void UpdateRocket()
	{
		if (!m_Blueprint)
		{
			if (m_Rocket == null)
			{
				m_Rocket = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.SpacePortRocket, base.transform.position, Quaternion.identity).GetComponent<SpacePortRocket>();
				m_Rocket.SetState(SpacePortRocket.State.Return);
			}
			m_Rocket.transform.position = base.transform.TransformPoint(new Vector3(Tile.m_Size * 0.5f, 0.75f, Tile.m_Size * 1.75f));
			m_Rocket.transform.rotation = base.transform.rotation;
		}
	}

	public override void SetBlueprint(bool Blueprint, bool Register = false)
	{
		base.SetBlueprint(Blueprint, Register);
		if ((bool)m_BlueprintModel)
		{
			m_BlueprintModel.SetActive(!Blueprint);
		}
		if ((bool)m_Lights)
		{
			m_Lights.SetActive(!Blueprint);
		}
		if (!Blueprint)
		{
			if (!QuestManager.Instance.GetQuestComplete(Quest.ID.GlueSpacePort))
			{
				UpdateRocket();
				m_Rocket.SetState(SpacePortRocket.State.InSpace);
			}
			QuestManager.Instance.AddEvent(QuestEvent.Type.SpacePortComplete, Bot: false, null, this);
			OffworldMissionsManager.Instance.SetupRegularMissions();
		}
		if (Blueprint)
		{
			m_SpawnModel.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 0.25f);
		}
		else
		{
			ModelManager.Instance.RestoreStandardMaterials(ObjectTypeList.m_Total, m_SpawnModel, "Models/Buildings/BuildingSpawnPoint");
		}
	}

	public override void CheckWallsFloors()
	{
		base.CheckWallsFloors();
	}

	public override void SendAction(ActionInfo Info)
	{
		base.SendAction(Info);
		if ((bool)m_Engager && Info.m_Action != ActionType.Disengaged && Info.m_Action != ActionType.SetValue)
		{
			return;
		}
		switch (Info.m_Action)
		{
		case ActionType.Engaged:
			m_Engager = Info.m_Object;
			if ((bool)m_Engager.GetComponent<FarmerPlayer>())
			{
				GameStateManager.Instance.StartSpacePort(this);
			}
			break;
		case ActionType.Disengaged:
			m_Engager = null;
			break;
		}
	}

	public override bool CanDoAction(ActionInfo Info, bool RightNow)
	{
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
		GetAction action = Info.m_Action;
		if (action == GetAction.IsDuplicatable)
		{
			return false;
		}
		return base.GetActionInfo(Info);
	}

	private void StartAddAnything(AFO Info)
	{
		bool flag = false;
		if (!ToolFillable.GetIsTypeFillable(Info.m_Object.m_TypeIdentifier))
		{
			flag = true;
		}
		else if (Info.m_Object.GetComponent<ToolFillable>().m_HeldType == ObjectTypeList.m_Total)
		{
			flag = true;
		}
		if (flag)
		{
			Info.m_Object.transform.position = m_IngredientsRoot.position;
		}
	}

	private void EndAddAnything(AFO Info)
	{
		Actionable @object = Info.m_Object;
		ObjectType newType = Info.m_ObjectType;
		if (ToolFillable.GetIsTypeFillable(@object.m_TypeIdentifier) && Info.m_Object.GetComponent<ToolFillable>().m_HeldType != ObjectTypeList.m_Total)
		{
			newType = Info.m_Object.GetComponent<ToolFillable>().m_HeldType;
			@object.GetComponent<ToolFillable>().Empty(1);
		}
		else if (Info.m_Actioner.GetComponent<Crane>() != null)
		{
			Crane component = Info.m_Actioner.GetComponent<Crane>();
			if ((bool)component.m_HeldObject)
			{
				@object = component.m_HeldObject;
				newType = component.m_HeldObject.m_TypeIdentifier;
				component.SendAction(new ActionInfo(ActionType.DropAll, component.m_TileCoord));
				@object.StopUsing();
			}
		}
		else
		{
			@object.StopUsing();
		}
		OffworldMission offworldMission = OffworldMissionsManager.Instance.AddObjectType(newType);
		XPPlus1 component2 = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.XPPlus1, base.transform.position, Quaternion.identity).GetComponent<XPPlus1>();
		component2.SetSpacePort(offworldMission.m_Complete);
		component2.SetScale(4f);
		component2.SetWorldPosition(base.transform.position);
		if (offworldMission.m_Complete)
		{
			m_Rocket.SetState(SpacePortRocket.State.Leave);
		}
	}

	protected override ActionType GetActionFromAnything(AFO Info)
	{
		Info.m_StartAction = StartAddAnything;
		Info.m_EndAction = EndAddAnything;
		Info.m_FarmerState = Farmer.State.Adding;
		ObjectType newType = Info.m_ObjectType;
		if (Info.m_Actioner.GetComponent<Crane>() != null)
		{
			Crane component = Info.m_Actioner.GetComponent<Crane>();
			if ((bool)component.m_HeldObject)
			{
				newType = component.m_HeldObject.m_TypeIdentifier;
			}
		}
		if ((bool)Info.m_Object && ToolFillable.GetIsTypeFillable(Info.m_ObjectType) && Info.m_Object.GetComponent<ToolFillable>().m_HeldType != ObjectTypeList.m_Total)
		{
			newType = Info.m_Object.GetComponent<ToolFillable>().m_HeldType;
		}
		if (!OffworldMissionsManager.Instance.GetIsObjectTypeRequired(newType))
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
			if (GameStateManager.Instance.GetActualState() != GameStateManager.State.TeachWorker && m_Engager == null)
			{
				return ActionType.EngageObject;
			}
			return ActionType.Fail;
		}
		if (Info.m_ActionType == AFO.AT.Secondary)
		{
			return GetActionFromAnything(Info);
		}
		return ActionType.Total;
	}

	private void SetState(State NewState)
	{
		m_State = NewState;
		m_StateTimer = 0f;
		ModManager.Instance.CheckBuildingStateChangedCallback(m_UniqueID, m_State.ToString());
	}

	private void UpdateIncineratingAnimation()
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

	private void UpdateIncinerating()
	{
		UpdateIncineratingAnimation();
		if (m_StateTimer > 0.5f)
		{
			m_StateTimer = 0f;
			AudioManager.Instance.StopEvent(m_PlaySound);
			SetState(State.Idle);
		}
	}

	private void UpdateLights()
	{
		if (!m_Blueprint)
		{
			m_LightsTimer += TimeManager.Instance.m_NormalDelta;
			float num = 2f;
			if (SettingsManager.Instance.m_FlashiesEnabled && (int)(m_LightsTimer * 60f) % 60 < 30)
			{
				num = 0f;
			}
			m_LightsMaterial.SetColor("_EmissionColor", new Color(1f, 0f, 0f) * num);
		}
	}

	private void UpdateBlueprint()
	{
		if (OffworldMissionsManager.Instance == null || !m_BlueprintModel)
		{
			return;
		}
		ObjectType objectType = ObjectType.Nothing;
		OffworldMission selectedMission = OffworldMissionsManager.Instance.m_SelectedMission;
		if (selectedMission != null)
		{
			for (int i = 0; i < selectedMission.m_Requirements.Count; i++)
			{
				if (selectedMission.m_Progress[i] != selectedMission.m_Requirements[i].m_Count)
				{
					objectType = selectedMission.m_Requirements[i].m_Type;
					break;
				}
			}
		}
		if (objectType == ObjectTypeList.m_Total)
		{
			objectType = ObjectType.Nothing;
		}
		if (objectType != m_BlueprintType)
		{
			m_BlueprintType = objectType;
			Sprite icon = IconManager.Instance.GetIcon(objectType);
			if ((bool)icon)
			{
				MeshRenderer component = m_BlueprintModel.GetComponent<MeshRenderer>();
				component.material.SetTexture("_MainTex", icon.texture);
				StandardShaderUtils.ChangeRenderMode(component.material, StandardShaderUtils.BlendMode.Transparent);
			}
		}
	}

	private void Update()
	{
		State state = m_State;
		if (state == State.Incinerating)
		{
			UpdateIncinerating();
		}
		UpdateRocket();
		UpdateLights();
		UpdateBlueprint();
		m_StateTimer += TimeManager.Instance.m_NormalDelta;
	}
}
