using System;
using SimpleJSON;
using UnityEngine;

public class Mushroom : Flora
{
	[HideInInspector]
	public enum State
	{
		GrowingNothing,
		GrowingSmall,
		GrowingBig,
		Idle,
		Total
	}

	[HideInInspector]
	public State m_State;

	[HideInInspector]
	public State m_PreviousState;

	private float m_StateTimer;

	private Wobbler m_Wobbler;

	private static float m_GrowingNothingDelay = 1f;

	private float m_GrowingSmallDelay;

	public override void RegisterClass()
	{
		base.RegisterClass();
		ModelManager.Instance.AddModel("Models/Crops/Mushroom2", ObjectType.Mushroom, RandomVariants: true);
		ModelManager.Instance.AddModel("Models/Crops/Mushroom3", ObjectType.Mushroom, RandomVariants: true);
		ModelManager.Instance.AddModel("Models/Crops/MushroomCultivated", ObjectType.Mushroom);
	}

	public override void Restart()
	{
		base.Restart();
		m_Wobbler.Restart();
		m_State = State.Idle;
		m_GrowingSmallDelay = VariableManager.Instance.GetVariableAsFloat(m_TypeIdentifier, "GrowDelay");
		base.enabled = false;
	}

	protected new void Awake()
	{
		base.Awake();
		m_Wobbler = new Wobbler();
	}

	public override string GetHumanReadableName()
	{
		string text = base.GetHumanReadableName();
		if (m_State == State.GrowingSmall || m_State == State.GrowingBig)
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
		m_State = (State)JSONUtils.GetAsInt(Node, "ST", 0);
		SetState(m_State);
		m_StateTimer = JSONUtils.GetAsFloat(Node, "STT", 0f);
	}

	public override void SendAction(ActionInfo Info)
	{
		ActionType action = Info.m_Action;
		if (action == ActionType.Bump)
		{
			m_Wobbler.Go(0.5f, 5f, 0.5f);
			base.enabled = true;
		}
		base.SendAction(Info);
	}

	private void UseShovel(AFO Info)
	{
		m_Wobbler.Go(0.5f, 5f, 0.5f);
		base.enabled = true;
	}

	private void EndShovel(AFO Info)
	{
		int num = 3;
		if (Info.m_Actioner.GetComponent<Farmer>().m_FarmerCarry.GetLastObjectType() == ObjectType.Stick)
		{
			num = 1;
		}
		for (int i = 0; i < num; i++)
		{
			TileCoord randomEmptyTile = TileHelpers.GetRandomEmptyTile(m_TileCoord);
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.MushroomDug, randomEmptyTile.ToWorldPositionTileCentered(), Quaternion.identity);
			SpawnAnimationManager.Instance.AddJump(baseClass, m_TileCoord, randomEmptyTile, 0f, baseClass.transform.position.y, 4f);
			QuestManager.Instance.AddEvent(QuestEvent.Type.ForageFood, Info.m_Actioner.m_TypeIdentifier == ObjectType.Worker, 0, this);
			BadgeManager.Instance.AddEvent(BadgeEvent.Type.Mushrooms);
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
		if (FarmerStateShovel.GetIsToolAcceptable(objectType) || objectType == ObjectType.Stick)
		{
			ActionType actionFromShovel = GetActionFromShovel(Info);
			if (actionFromShovel != ActionType.Total)
			{
				return actionFromShovel;
			}
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
			base.enabled = true;
			SetScale(0f);
			break;
		case State.GrowingSmall:
			base.enabled = true;
			SetScale(0.3f);
			if (state != State.GrowingSmall)
			{
				m_Wobbler.Go(0.5f, 5f, 0.5f);
			}
			break;
		case State.GrowingBig:
			base.enabled = true;
			SetScale(0.6f);
			if (state != State.GrowingBig)
			{
				m_Wobbler.Go(0.5f, 5f, 0.5f);
			}
			break;
		case State.Idle:
			base.enabled = true;
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
			if (m_StateTimer > m_GrowingNothingDelay)
			{
				SetState(State.GrowingSmall);
				AudioManager.Instance.StartEvent("CropGrowing", this);
			}
			break;
		case State.GrowingSmall:
			if (m_StateTimer > m_GrowingSmallDelay / 2f)
			{
				SetState(State.GrowingBig);
				AudioManager.Instance.StartEvent("CropGrowing", this);
			}
			break;
		case State.GrowingBig:
			if (m_StateTimer > m_GrowingSmallDelay / 2f)
			{
				if (!m_WorldCreated)
				{
					QuestManager.Instance.AddEvent(QuestEvent.Type.GrowMushroom, Bot: false, 0, this);
				}
				SetState(State.Idle);
				AudioManager.Instance.StartEvent("CropGrown", this);
			}
			break;
		case State.Idle:
			if (m_Wobbler.m_Height == 0f)
			{
				base.enabled = false;
			}
			break;
		}
		m_StateTimer += TimeManager.Instance.m_NormalDelta;
	}

	protected override void UpdateWorldCreated()
	{
		if (!m_WorldCreated)
		{
			LoadNewModel("Models/Crops/MushroomCultivated");
		}
		else
		{
			LoadNewModel("Models/Crops/Mushroom", RandomVariants: true);
		}
		UpdatePlotVisibility();
		base.UpdateWorldCreated();
	}
}
