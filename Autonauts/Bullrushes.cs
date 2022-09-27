using System;
using SimpleJSON;
using UnityEngine;

public class Bullrushes : Flora
{
	public enum State
	{
		Wild,
		Growing,
		Idle,
		Total
	}

	private static float m_GrowingScale = 0.2f;

	public State m_State;

	private float m_StateTimer;

	private State m_RefreshState;

	private float m_RefreshStateTimer;

	private Wobbler m_Wobbler;

	public override void RegisterClass()
	{
		base.RegisterClass();
		ObjectTypeList.Instance.SetTypeSleep(ObjectType.Bullrushes);
	}

	public override void Restart()
	{
		base.Restart();
		if (!ObjectTypeList.m_Loading)
		{
			CollectionManager.Instance.AddCollectable("Bullrushes", this);
		}
		m_Wobbler.Restart();
		SetState(State.Idle);
	}

	protected new void Awake()
	{
		base.Awake();
		m_Wobbler = new Wobbler();
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		base.StopUsing(AndDestroy);
		if ((bool)FailSafeManager.Instance)
		{
			FailSafeManager.Instance.BullrushesRemove(m_TileCoord);
		}
	}

	public override string GetHumanReadableName()
	{
		string text = base.GetHumanReadableName();
		if (m_State == State.Wild)
		{
			text = text + " (" + TextManager.Instance.Get("CropWild") + ")";
		}
		if (m_State == State.Growing)
		{
			text = text + " (" + TextManager.Instance.Get("Growing") + ")";
		}
		return text;
	}

	public void SetGrowing()
	{
		SetState(State.Growing);
		Wake();
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
		CollectionManager.Instance.AddCollectable("Bullrushes", this);
		SetState((State)JSONUtils.GetAsInt(Node, "ST", 0));
		m_StateTimer = JSONUtils.GetAsFloat(Node, "STT", 0f);
	}

	public override void SendAction(ActionInfo Info)
	{
		ActionType action = Info.m_Action;
		if (action == ActionType.Bump)
		{
			m_Wobbler.Go(0.5f, 5f, 0.125f);
			Wake();
		}
		base.SendAction(Info);
	}

	private void UseScythe(AFO Info)
	{
		AudioManager.Instance.StartEvent("CropRuffle", this);
		m_Wobbler.Go(0.25f, 2f, 1f);
		Wake();
	}

	protected virtual void EndScythe(AFO Info)
	{
		Cut();
	}

	private ActionType GetActionFromScythe(AFO Info)
	{
		Info.m_UseAction = UseScythe;
		Info.m_EndAction = EndScythe;
		Info.m_FarmerState = Farmer.State.Scythe;
		if (Info.m_ActionType == AFO.AT.Primary)
		{
			if (m_State == State.Idle || m_State == State.Wild)
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
		if (FarmerStateScythe.GetIsToolAcceptable(objectType) || objectType == ObjectType.RockSharp)
		{
			ActionType actionFromScythe = GetActionFromScythe(Info);
			if (actionFromScythe != ActionType.Total)
			{
				return actionFromScythe;
			}
		}
		return base.GetActionFromObject(Info);
	}

	private void SetState(State NewState)
	{
		m_State = NewState;
		m_StateTimer = 0f;
		switch (NewState)
		{
		case State.Wild:
			SetScale(1f);
			break;
		case State.Growing:
			SetScale(m_GrowingScale);
			Wake();
			break;
		case State.Idle:
			SetScale(1f);
			break;
		}
	}

	private void Cut()
	{
		int num = UnityEngine.Random.Range(1, 3);
		for (int i = 0; i < num; i++)
		{
			TileCoord randomEmptyTile = TileHelpers.GetRandomEmptyTile(m_TileCoord);
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.BullrushesSeeds, randomEmptyTile.ToWorldPositionTileCentered(), base.transform.localRotation);
			SpawnAnimationManager.Instance.AddJump(baseClass, m_TileCoord, randomEmptyTile, 0f, baseClass.transform.position.y, 3f);
		}
		num = 1;
		for (int j = 0; j < num; j++)
		{
			TileCoord randomEmptyTile2 = TileHelpers.GetRandomEmptyTile(m_TileCoord);
			BaseClass baseClass2 = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.BullrushesStems, randomEmptyTile2.ToWorldPositionTileCentered(), base.transform.localRotation);
			SpawnAnimationManager.Instance.AddJump(baseClass2, m_TileCoord, randomEmptyTile2, 0f, baseClass2.transform.position.y, 3f);
		}
		AudioManager.Instance.StartEvent("ObjectCreated", this);
		StopUsing();
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
		case State.Growing:
			if (m_StateTimer > VariableManager.Instance.GetVariableAsFloat(m_TypeIdentifier, "GrowDelay"))
			{
				AudioManager.Instance.StartEvent("CropGrown", this);
				SetState(State.Idle);
				m_Wobbler.Go(0.5f, 5f, 0.5f);
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
}
