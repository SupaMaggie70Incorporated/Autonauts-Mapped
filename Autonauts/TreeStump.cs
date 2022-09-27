using System;
using SimpleJSON;
using UnityEngine;

public class TreeStump : Flora
{
	public enum State
	{
		Idle
	}

	private State m_State;

	private float m_StateTimer;

	private Wobbler m_Wobbler;

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("TreeStump", m_TypeIdentifier);
	}

	public override void Restart()
	{
		base.Restart();
		if (!ObjectTypeList.m_Loading)
		{
			CollectionManager.Instance.AddCollectable("TreeStump", this);
		}
		m_StateTimer = 0f;
		m_Wobbler.Restart();
		base.enabled = false;
	}

	protected new void Awake()
	{
		base.Awake();
		m_Wobbler = new Wobbler();
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
		CollectionManager.Instance.AddCollectable("TreeStump", this);
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
		m_Wobbler.Go(0.25f, 2f, 1f);
		base.enabled = true;
	}

	private void EndShovel(AFO Info)
	{
		StopUsing();
		TileManager.Instance.SetTileType(m_TileCoord, Tile.TileType.SoilHole);
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
		m_StateTimer = 0f;
	}

	private void UpdateIdle()
	{
		m_Wobbler.Update();
		float x = Mathf.Sin(m_Wobbler.m_Height * (float)Math.PI * 2f) * 10f;
		m_ModelRoot.transform.localRotation = Quaternion.Euler(x, 0f, 0f);
		if (m_Wobbler.m_Height == 0f)
		{
			base.enabled = false;
		}
	}

	private void Update()
	{
		if (m_State == State.Idle)
		{
			UpdateIdle();
		}
	}
}
