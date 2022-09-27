using System;
using SimpleJSON;
using UnityEngine;

public class Weed : Flora
{
	public enum State
	{
		WeedGrow,
		Weed
	}

	private State m_State;

	private float m_StateTimer;

	private Wobbler m_Wobbler;

	private Wobbler m_DigWobbler;

	private Tile m_Tile;

	private float m_NormalHeight;

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("Weed", m_TypeIdentifier);
		ObjectTypeList.Instance.SetTypeSleep(ObjectType.Weed);
	}

	public override void Restart()
	{
		m_State = State.Weed;
		base.Restart();
		if (!ObjectTypeList.m_Loading)
		{
			CollectionManager.Instance.AddCollectable("Weed", this);
		}
		SetHeight(0f);
		m_NormalHeight = 1f;
		m_Wobbler.Restart();
		m_DigWobbler.Restart();
		NewTileCoord();
		SetGrown();
	}

	protected new void Awake()
	{
		base.Awake();
		m_Wobbler = new Wobbler();
		m_DigWobbler = new Wobbler();
	}

	private void NewTileCoord()
	{
		m_Tile = TileManager.Instance.GetTile(m_TileCoord);
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		JSONUtils.Set(Node, "ST", (int)m_State);
		JSONUtils.Set(Node, "STT", m_StateTimer);
	}

	public override void Load(JSONNode Node)
	{
		SetState((State)JSONUtils.GetAsInt(Node, "ST", 0));
		m_StateTimer = JSONUtils.GetAsFloat(Node, "STT", 0f);
		base.Load(Node);
		CollectionManager.Instance.AddCollectable("Weed", this);
		NewTileCoord();
	}

	public override void SendAction(ActionInfo Info)
	{
		ActionType action = Info.m_Action;
		if (action == ActionType.Bump)
		{
			m_Wobbler.Go(0.5f, 5f, 0.5f);
			Wake();
		}
		base.SendAction(Info);
	}

	private void UseShovel(AFO Info)
	{
		m_DigWobbler.Go(0.5f, 5f, 0.5f);
		Wake();
	}

	private void EndShovel(AFO Info)
	{
		Cut();
	}

	private ActionType GetActionFromShovel(AFO Info)
	{
		Info.m_UseAction = UseShovel;
		Info.m_EndAction = EndShovel;
		Info.m_FarmerState = Farmer.State.Shovel;
		if (Info.m_ActionType == AFO.AT.Primary)
		{
			if (m_State == State.Weed)
			{
				return ActionType.UseInHands;
			}
			return ActionType.Fail;
		}
		return ActionType.Total;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (FarmerStateShovel.GetIsToolAcceptable(Info.m_ObjectType))
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
		m_State = NewState;
		switch (m_State)
		{
		case State.Weed:
			SetScale(1f);
			break;
		case State.WeedGrow:
			Wake();
			SetScale(0.2f);
			break;
		}
		m_StateTimer = 0f;
	}

	public void SetGrown()
	{
		SetState(State.Weed);
		SetHeight(m_NormalHeight);
		Sleep();
	}

	public bool GetGrown()
	{
		if (m_State == State.Weed)
		{
			return true;
		}
		return false;
	}

	public void Cut()
	{
		for (int i = 0; i < 2; i++)
		{
			TileCoord randomEmptyTile = TileHelpers.GetRandomEmptyTile(m_TileCoord);
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.WeedDug, randomEmptyTile.ToWorldPositionTileCentered(), Quaternion.identity);
			SpawnAnimationManager.Instance.AddJump(baseClass, m_TileCoord, randomEmptyTile, 0f, baseClass.transform.position.y, 4f);
		}
		SetHeight(0f);
		SetState(State.WeedGrow);
		StopUsing();
	}

	private void SetHeight(float Height)
	{
	}

	private void UpdateStateWeedGrow()
	{
		if (!m_Tile.m_MiscObject && (!m_Tile.m_AssociatedObject || !(m_Tile.m_AssociatedObject != this)) && !m_Tile.m_Floor && !m_Tile.m_Building && !m_Tile.m_BuildingFootprint)
		{
			m_StateTimer += TimeManager.Instance.m_NormalDelta;
			if (m_StateTimer > VariableManager.Instance.GetVariableAsFloat(ObjectType.Weed, "GrowDelay"))
			{
				SetState(State.Weed);
				m_Wobbler.Go(0.5f, 10f, 0.25f);
				AudioManager.Instance.StartEvent("CropWeedAppear", this);
			}
		}
	}

	private void UpdateStateWeed()
	{
		m_Wobbler.Update();
		float height = m_Wobbler.m_Height;
		m_ModelRoot.transform.localScale = new Vector3(1f + height, 1f - height, 1f + height);
		m_DigWobbler.Update();
		float x = Mathf.Sin(m_DigWobbler.m_Height * (float)Math.PI * 2f) * 10f;
		m_ModelRoot.transform.localRotation = Quaternion.Euler(x, 0f, 0f);
		if (m_Wobbler.m_Height == 0f && m_DigWobbler.m_Height == 0f)
		{
			Sleep();
		}
	}

	private void Update()
	{
		switch (m_State)
		{
		case State.WeedGrow:
			UpdateStateWeedGrow();
			break;
		case State.Weed:
			UpdateStateWeed();
			break;
		}
	}
}
