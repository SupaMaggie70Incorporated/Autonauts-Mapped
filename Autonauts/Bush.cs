using System;
using SimpleJSON;
using UnityEngine;

public class Bush : Flora
{
	[HideInInspector]
	public enum State
	{
		GrowingNothing,
		GrowingSmall,
		GrowingBerries,
		Idle,
		Total
	}

	[HideInInspector]
	public State m_State;

	[HideInInspector]
	public State m_PreviousState;

	private float m_StateTimer;

	private State m_NewState;

	private float m_NewStateTimer;

	private float m_Delay;

	private Wobbler m_Wobbler;

	private GameObject m_Berries;

	public override void RegisterClass()
	{
		base.RegisterClass();
		ModelManager.Instance.AddModel("Models/Crops/Bush2", ObjectType.Bush, RandomVariants: true);
		ModelManager.Instance.AddModel("Models/Crops/Bush3", ObjectType.Bush, RandomVariants: true);
		ModelManager.Instance.AddModel("Models/Crops/BushCultivated", ObjectType.Bush);
		ObjectTypeList.Instance.SetTypeSleep(ObjectType.Bush);
	}

	public override void Restart()
	{
		base.Restart();
		m_Wobbler.Restart();
		m_State = State.Idle;
		SetState(m_State);
	}

	protected new void Awake()
	{
		base.Awake();
		m_Wobbler = new Wobbler();
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		base.StopUsing(AndDestroy);
		LoadNewModel("Models/Crops/Bush", RandomVariants: true);
	}

	public override string GetHumanReadableName()
	{
		string text = base.GetHumanReadableName();
		if (m_State == State.GrowingSmall)
		{
			text = text + " (" + TextManager.Instance.Get("Growing") + ")";
		}
		return text;
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		JSONUtils.Set(Node, "ST", (int)m_State);
		JSONUtils.Set(Node, "STT", m_StateTimer);
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		m_NewState = (State)JSONUtils.GetAsInt(Node, "ST", 0);
		m_NewStateTimer = JSONUtils.GetAsFloat(Node, "STT", 0f);
	}

	public override void SendAction(ActionInfo Info)
	{
		switch (Info.m_Action)
		{
		case ActionType.Bump:
			m_Wobbler.Go(0.5f, 5f, 0.5f);
			Wake();
			break;
		case ActionType.Refresh:
		case ActionType.RefreshFirst:
			SetState(m_NewState);
			m_StateTimer = m_NewStateTimer;
			break;
		}
		base.SendAction(Info);
	}

	private void UseShears(AFO Info)
	{
		m_Wobbler.Go(0.5f, 5f, 0.5f);
		Wake();
		Vector3 vector = base.transform.position + new Vector3(0f, 1.5f, 0f);
		Vector3 vector2 = Info.m_Object.transform.position - vector;
		float y = (0f - Mathf.Atan2(vector2.z, vector2.x)) * 57.29578f - 90f;
		MyParticles newParticles = ParticlesManager.Instance.CreateParticles("BashLeaves", vector, Quaternion.Euler(-12f, y, 0f));
		ParticlesManager.Instance.DestroyParticles(newParticles, WaitUntilNoParticles: true);
	}

	private void EndShears(AFO Info)
	{
		StopUsing();
		ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Hedge, m_TileCoord.ToWorldPositionTileCentered(), Quaternion.identity).GetComponent<Hedge>();
		RouteFinding.UpdateTileWalk(m_TileCoord.x, m_TileCoord.y);
	}

	private ActionType GetActionFromShears(AFO Info)
	{
		Info.m_UseAction = UseShears;
		Info.m_EndAction = EndShears;
		Info.m_FarmerState = Farmer.State.Shearing;
		if (Info.m_ActionType == AFO.AT.Primary)
		{
			if (m_State == State.Idle || m_State == State.GrowingBerries)
			{
				return ActionType.UseInHands;
			}
			return ActionType.Fail;
		}
		return ActionType.Total;
	}

	private void UseShovel(AFO Info)
	{
		m_Wobbler.Go(0.5f, 5f, 0.5f);
		Wake();
	}

	private void EndShovel(AFO Info)
	{
		if (m_State == State.Idle)
		{
			int num = UnityEngine.Random.Range(1, 3);
			for (int i = 0; i < num; i++)
			{
				TileCoord randomEmptyTile = TileHelpers.GetRandomEmptyTile(m_TileCoord);
				BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Berries, randomEmptyTile.ToWorldPositionTileCentered(), Quaternion.identity);
				SpawnAnimationManager.Instance.AddJump(baseClass, m_TileCoord, randomEmptyTile, 0f, baseClass.transform.position.y, 4f);
				QuestManager.Instance.AddEvent(QuestEvent.Type.ForageFood, Info.m_Actioner.m_TypeIdentifier == ObjectType.Worker, 0, this);
				BadgeManager.Instance.AddEvent(BadgeEvent.Type.Berries);
			}
			if (num > 0)
			{
				AudioManager.Instance.StartEvent("ObjectCreated", this);
			}
			QuestManager.Instance.AddEvent(QuestEvent.Type.Make, Info.m_Actioner.m_TypeIdentifier == ObjectType.Worker, ObjectType.Berries, null);
		}
		StopUsing();
	}

	private ActionType GetActionFromShovel(AFO Info)
	{
		Info.m_UseAction = UseShovel;
		Info.m_EndAction = EndShovel;
		Info.m_FarmerState = Farmer.State.Shovel;
		if (Info.m_ActionType == AFO.AT.Primary)
		{
			if (m_State != State.GrowingSmall)
			{
				return ActionType.UseInHands;
			}
			return ActionType.Fail;
		}
		return ActionType.Total;
	}

	private void UseFlail(AFO Info)
	{
		m_Wobbler.Go(0.5f, 5f, 0.5f);
		Wake();
		Vector3 vector = base.transform.position + new Vector3(0f, 1.5f, 0f);
		Vector3 vector2 = Info.m_Object.transform.position - vector;
		float y = (0f - Mathf.Atan2(vector2.z, vector2.x)) * 57.29578f - 90f;
		MyParticles newParticles = ParticlesManager.Instance.CreateParticles("BashLeaves", vector, Quaternion.Euler(-12f, y, 0f));
		ParticlesManager.Instance.DestroyParticles(newParticles, WaitUntilNoParticles: true);
	}

	private void EndFlail(AFO Info)
	{
		int num = UnityEngine.Random.Range(1, 1);
		for (int i = 0; i < num; i++)
		{
			TileCoord randomEmptyTile = TileHelpers.GetRandomEmptyTile(m_TileCoord);
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Berries, randomEmptyTile.ToWorldPositionTileCentered(), Quaternion.identity);
			SpawnAnimationManager.Instance.AddJump(baseClass, m_TileCoord, randomEmptyTile, 0f, baseClass.transform.position.y, 4f);
			QuestManager.Instance.AddEvent(QuestEvent.Type.ForageFood, Info.m_Actioner.m_TypeIdentifier == ObjectType.Worker, 0, this);
			BadgeManager.Instance.AddEvent(BadgeEvent.Type.Berries);
		}
		if (num > 0)
		{
			AudioManager.Instance.StartEvent("ObjectCreated", this);
		}
		QuestManager.Instance.AddEvent(QuestEvent.Type.Make, Info.m_Actioner.m_TypeIdentifier == ObjectType.Worker, ObjectType.Berries, null);
		SetState(State.GrowingBerries);
	}

	private ActionType GetActionFromFlail(AFO Info)
	{
		Info.m_UseAction = UseFlail;
		Info.m_EndAction = EndFlail;
		Info.m_FarmerState = Farmer.State.Flail;
		if (Info.m_ActionType == AFO.AT.Primary)
		{
			if (m_State == State.Idle)
			{
				return ActionType.UseInHands;
			}
			return ActionType.Fail;
		}
		return ActionType.Total;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		ObjectType objectType = Info.m_ObjectType;
		if (FarmerStateShovel.GetIsToolAcceptable(objectType))
		{
			return GetActionFromShovel(Info);
		}
		if (FarmerStateFlail.GetIsToolAcceptable(objectType))
		{
			return GetActionFromFlail(Info);
		}
		if (FarmerStateShearing.GetIsToolAcceptable(objectType))
		{
			return GetActionFromShears(Info);
		}
		return base.GetActionFromObject(Info);
	}

	public void SetState(State NewState)
	{
		State state = m_State;
		m_State = NewState;
		m_StateTimer = 0f;
		switch (m_State)
		{
		case State.GrowingNothing:
			m_Berries.SetActive(value: false);
			Wake();
			SetScale(0f);
			m_Delay = VariableManager.Instance.GetVariableAsFloat(ObjectType.Bush, "GrowSeedDelay");
			break;
		case State.GrowingSmall:
			m_Berries.SetActive(value: false);
			Wake();
			SetScale(0.3f);
			if (state != State.GrowingSmall)
			{
				m_Wobbler.Go(0.5f, 5f, 0.5f);
			}
			m_Delay = VariableManager.Instance.GetVariableAsFloat(ObjectType.Bush, "GrowBushDelay");
			break;
		case State.GrowingBerries:
			m_Berries.SetActive(value: false);
			Wake();
			SetScale(1f);
			if (state != State.Idle)
			{
				m_Wobbler.Go(0.5f, 5f, 0.5f);
			}
			m_Delay = VariableManager.Instance.GetVariableAsFloat(ObjectType.Bush, "GrowBerriesDelay");
			break;
		case State.Idle:
			m_Berries.SetActive(value: true);
			Wake();
			SetScale(1f);
			if (state != State.Idle)
			{
				m_Wobbler.Go(0.5f, 5f, 0.5f);
			}
			break;
		}
	}

	private void UpdateWobbler()
	{
		m_Wobbler.Update();
		float x = Mathf.Sin(m_Wobbler.m_Height * (float)Math.PI * 2f) * 10f;
		m_ModelRoot.transform.localRotation = Quaternion.Euler(x, 0f, 0f);
	}

	private void Update()
	{
		UpdateWobbler();
		switch (m_State)
		{
		case State.GrowingNothing:
			if (m_StateTimer > m_Delay)
			{
				SetState(State.GrowingSmall);
				AudioManager.Instance.StartEvent("CropGrowing", this);
			}
			break;
		case State.GrowingSmall:
			if (m_StateTimer > m_Delay)
			{
				AudioManager.Instance.StartEvent("CropGrown", this);
				SetState(State.GrowingBerries);
			}
			break;
		case State.GrowingBerries:
			if (m_StateTimer > m_Delay)
			{
				AudioManager.Instance.StartEvent("BushBerriesGrown", this);
				SetState(State.Idle);
			}
			break;
		case State.Idle:
			if (m_Wobbler.m_Height == 0f)
			{
				Sleep();
			}
			break;
		}
		m_StateTimer += TimeManager.Instance.m_NormalDelta;
	}

	protected override void UpdateWorldCreated()
	{
		if (!m_WorldCreated)
		{
			LoadNewModel("Models/Crops/BushCultivated");
		}
		else
		{
			LoadNewModel("Models/Crops/Bush", RandomVariants: true);
		}
		UpdatePlotVisibility();
		m_Berries = m_ModelRoot.transform.Find("Berries").gameObject;
		m_Berries.SetActive(m_State == State.Idle);
		base.UpdateWorldCreated();
	}
}
