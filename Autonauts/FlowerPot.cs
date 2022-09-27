using System;
using SimpleJSON;
using UnityEngine;

public class FlowerPot : Holdable
{
	public enum State
	{
		Empty,
		Fertiliser,
		Seeds,
		Germinate,
		Growing,
		Grown,
		Dying,
		Dead,
		Total
	}

	[HideInInspector]
	public State m_State;

	private float m_StateTimer;

	[HideInInspector]
	public FlowerWild.Type m_Type;

	private GameObject m_Seeds;

	private GameObject m_FlowerModel;

	private GameObject m_FlowerPoint;

	private static float m_GerminateDelay = 1f;

	private static float m_GrowingDelay = 10f;

	private static float m_DyingDelay = 20f;

	private Wobbler m_Wobbler;

	private State m_RefreshState;

	private float m_RefreshStateTimer;

	public override void Restart()
	{
		base.Restart();
		m_FlowerPoint = m_ModelRoot.transform.Find("FlowerGrownPoint").gameObject;
		m_Seeds = m_ModelRoot.transform.Find("Seeds").gameObject;
		m_Seeds.SetActive(value: false);
		m_Seeds.GetComponent<MeshRenderer>().enabled = true;
		m_Type = FlowerWild.Type.Total;
		SetState(State.Empty);
		m_Wobbler.Restart();
	}

	protected new void Awake()
	{
		base.Awake();
		m_FlowerModel = null;
		m_Wobbler = new Wobbler();
	}

	protected new void OnDestroy()
	{
		if ((bool)m_FlowerModel)
		{
			UnityEngine.Object.Destroy(m_FlowerModel.gameObject);
		}
		base.OnDestroy();
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		base.StopUsing(AndDestroy);
		if ((bool)m_FlowerModel)
		{
			UnityEngine.Object.Destroy(m_FlowerModel.gameObject);
		}
	}

	public override string GetHumanReadableName()
	{
		string text = base.GetHumanReadableName();
		if (m_Type != FlowerWild.Type.Total)
		{
			string text2 = TextManager.Instance.Get(FlowerWild.m_TypeNames[(int)m_Type]);
			text = text + " (" + text2 + ")";
		}
		return text;
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		JSONUtils.Set(Node, "ST", (int)m_State);
		JSONUtils.Set(Node, "STT", m_StateTimer);
		JSONUtils.Set(Node, "Type", (int)m_Type);
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		m_RefreshState = (State)JSONUtils.GetAsInt(Node, "ST", 0);
		m_RefreshStateTimer = JSONUtils.GetAsFloat(Node, "STT", 0f);
		m_Type = (FlowerWild.Type)JSONUtils.GetAsInt(Node, "Type", 0);
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		base.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
	}

	public override void SendAction(ActionInfo Info)
	{
		switch (Info.m_Action)
		{
		case ActionType.Poke:
			m_Wobbler.Go(0.5f, 5f, 0.5f);
			base.enabled = true;
			break;
		case ActionType.Refresh:
		case ActionType.RefreshFirst:
			UpdateModel();
			SetState(m_RefreshState);
			m_StateTimer = m_RefreshStateTimer;
			break;
		}
		base.SendAction(Info);
	}

	private void EndScythe(AFO Info)
	{
		int num = UnityEngine.Random.Range(1, 3);
		for (int i = 0; i < num; i++)
		{
			TileCoord randomEmptyTile = TileHelpers.GetRandomEmptyTile(m_TileCoord);
			ObjectType seedType = FlowerWild.GetSeedType(m_Type);
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(seedType, randomEmptyTile.ToWorldPositionTileCentered(), base.transform.localRotation);
			SpawnAnimationManager.Instance.AddJump(baseClass, m_TileCoord, randomEmptyTile, 0f, baseClass.transform.position.y, 3f);
		}
		AudioManager.Instance.StartEvent("ObjectCreated", this);
		SetState(State.Empty);
	}

	private ActionType GetActionFromScythe(AFO Info)
	{
		Info.m_EndAction = EndScythe;
		Info.m_FarmerState = Farmer.State.Scythe;
		if (Info.m_ActionType == AFO.AT.Primary)
		{
			if (m_State == State.Grown)
			{
				return ActionType.UseInHands;
			}
			return ActionType.Fail;
		}
		return ActionType.Total;
	}

	private void StartSeeds(AFO Info)
	{
		Actionable @object = Info.m_Object;
		SetType(@object.GetComponent<FlowerSeeds>().m_Type);
		if (m_State == State.Empty)
		{
			SetState(State.Seeds);
		}
		else
		{
			SetState(State.Germinate);
		}
	}

	private void EndSeeds(AFO Info)
	{
		Info.m_Object.StopUsing();
		AudioManager.Instance.StartEvent("FlowerPotSeeded", this);
	}

	private ActionType GetActionFromSeeds(AFO Info)
	{
		Info.m_StartAction = StartSeeds;
		Info.m_EndAction = EndSeeds;
		Info.m_FarmerState = Farmer.State.Adding;
		if (Info.m_ActionType == AFO.AT.Secondary)
		{
			if (m_State == State.Fertiliser || m_State == State.Empty || m_State == State.Dead)
			{
				return ActionType.AddResource;
			}
			return ActionType.Fail;
		}
		return ActionType.Total;
	}

	private void StartFertiliser(AFO Info)
	{
		if (m_State == State.Empty)
		{
			SetState(State.Fertiliser);
		}
		else
		{
			SetState(State.Germinate);
		}
	}

	private void EndFertiliser(AFO Info)
	{
		Info.m_Object.StopUsing();
		AudioManager.Instance.StartEvent("FlowerPotSeeded", this);
	}

	private ActionType GetActionFromFertiliser(AFO Info)
	{
		Info.m_StartAction = StartFertiliser;
		Info.m_EndAction = EndFertiliser;
		Info.m_FarmerState = Farmer.State.Adding;
		if (Info.m_ActionType == AFO.AT.Secondary)
		{
			if (m_State == State.Seeds || m_State == State.Empty || m_State == State.Dead)
			{
				return ActionType.AddResource;
			}
			return ActionType.Fail;
		}
		return ActionType.Total;
	}

	private void StartWateringCan(AFO Info)
	{
		Actionable @object = Info.m_Object;
		if (@object.GetComponent<ToolWateringCan>().m_HeldType == ObjectType.Water)
		{
			@object.GetComponent<ToolWateringCan>().Empty(1);
			AudioManager.Instance.StartEvent("FlowerPotGrowing", this);
			if (m_State == State.Dead)
			{
				SetState(State.Germinate);
			}
			else
			{
				SetState(State.Grown);
			}
		}
	}

	private ActionType GetActionFromWateringCan(AFO Info)
	{
		Info.m_StartAction = StartWateringCan;
		Info.m_FarmerState = Farmer.State.Adding;
		Info.m_RequirementsOut = m_State.ToString();
		if (Info.m_ActionType == AFO.AT.Secondary)
		{
			if ((m_State == State.Dying || m_State == State.Grown) && (Info.m_Object == null || Info.m_Object.GetComponent<ToolWateringCan>().m_HeldType == ObjectType.Water))
			{
				return ActionType.AddResource;
			}
			return ActionType.Fail;
		}
		Info.m_RequirementsOut = "";
		return ActionType.Total;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		ObjectType objectType = Info.m_ObjectType;
		if (FarmerStateScythe.GetIsToolAcceptable(objectType))
		{
			return GetActionFromScythe(Info);
		}
		if (FlowerSeeds.GetIsTypeFlowerSeeds(objectType))
		{
			return GetActionFromSeeds(Info);
		}
		if (objectType == ObjectType.Fertiliser)
		{
			return GetActionFromFertiliser(Info);
		}
		return base.GetActionFromObject(Info);
	}

	private void SetState(State NewState)
	{
		m_State = NewState;
		m_StateTimer = 0f;
		switch (m_State)
		{
		case State.Seeds:
			m_Seeds.SetActive(value: true);
			m_FlowerModel.SetActive(value: false);
			break;
		case State.Growing:
			m_Seeds.SetActive(value: false);
			m_FlowerModel.SetActive(value: true);
			m_FlowerModel.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
			m_FlowerModel.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			break;
		case State.Grown:
			m_Seeds.SetActive(value: false);
			m_FlowerModel.SetActive(value: true);
			m_FlowerModel.transform.localScale = new Vector3(1f, 1f, 1f);
			m_FlowerModel.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			break;
		case State.Dying:
			m_Seeds.SetActive(value: false);
			m_FlowerModel.SetActive(value: true);
			m_FlowerModel.transform.localScale = new Vector3(1f, 1f, 1f);
			m_FlowerModel.transform.localRotation = Quaternion.Euler(45f, 0f, 45f);
			break;
		case State.Dead:
			m_FlowerModel.SetActive(value: false);
			m_Seeds.SetActive(value: true);
			break;
		case State.Empty:
			m_Type = FlowerWild.Type.Total;
			UpdateModel();
			m_Seeds.SetActive(value: false);
			break;
		case State.Fertiliser:
		case State.Germinate:
			break;
		}
	}

	public void SetType(FlowerWild.Type NewType)
	{
		m_Type = NewType;
		UpdateModel();
	}

	private void UpdateModel()
	{
		if ((bool)m_FlowerModel)
		{
			UnityEngine.Object.Destroy(m_FlowerModel.gameObject);
		}
		if (m_Type != FlowerWild.Type.Total)
		{
			string text = FlowerWild.m_ModelNames[(int)m_Type];
			GameObject gameObject = UnityEngine.Object.Instantiate((GameObject)Resources.Load("Models/Crops/" + text, typeof(GameObject)), default(Vector3), Quaternion.identity, m_FlowerPoint.transform);
			gameObject.name = "Flower";
			InstantiationManager.Instance.SetLayer(gameObject, (Layers)base.gameObject.layer);
			gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			gameObject.transform.localRotation = Quaternion.identity;
			m_FlowerModel = gameObject;
			m_FlowerModel.SetActive(value: false);
		}
	}

	private void Update()
	{
		switch (m_State)
		{
		case State.Germinate:
			if (m_StateTimer > m_GerminateDelay)
			{
				SetState(State.Growing);
				AudioManager.Instance.StartEvent("FlowerPotGrown", this);
			}
			break;
		case State.Growing:
			if (m_StateTimer > m_GrowingDelay)
			{
				SetState(State.Grown);
				AudioManager.Instance.StartEvent("FlowerPotGrown", this);
			}
			break;
		case State.Dying:
			if (m_StateTimer > m_DyingDelay)
			{
				SetState(State.Dead);
				AudioManager.Instance.StartEvent("FlowerPotDead", this);
			}
			break;
		}
		m_StateTimer += TimeManager.Instance.m_NormalDelta;
		m_Wobbler.Update();
		float x = Mathf.Sin(m_Wobbler.m_Height * (float)Math.PI * 2f) * 10f;
		m_FlowerPoint.transform.rotation = Quaternion.Euler(x, 0f, 0f);
	}
}
