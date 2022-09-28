using System;
using SimpleJSON;
using UnityEngine;

public class CropPumpkin : Flora
{
	private enum State
	{
		GrowingNothing,
		GrowingSmall,
		Grown
	}

	private State m_State;

	private float m_StateTimer;

	private Wobbler m_Wobbler;

	private Tile m_Tile;

	private static float m_GrowingNothingDelay = 1f;

	private static float m_GrowingSmallDelay = 20f;

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("CropPumpkin", m_TypeIdentifier);
		ModelManager.Instance.AddModel("Models/Crops/CropPumpkinCultivated", ObjectType.CropPumpkin);
	}

	public override void Restart()
	{
		base.Restart();
		m_StateTimer = 0f;
		m_Wobbler.Restart();
		SetState(State.Grown);
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
		SetState((State)JSONUtils.GetAsInt(Node, "ST", 0));
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

	private void UseScythe(AFO Info)
	{
		m_Wobbler.Go(0.25f, 2f, 1f);
		base.enabled = true;
	}

	private void EndScythe(AFO Info)
	{
		BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Pumpkin, m_TileCoord.ToWorldPositionTileCentered(), Quaternion.identity);
		SpawnAnimationManager.Instance.AddJump(baseClass, m_TileCoord, m_TileCoord, 0f, baseClass.transform.position.y, 4f);
		if (TileManager.Instance.GetTileType(m_TileCoord) == Tile.TileType.SoilTilled)
		{
			TileManager.Instance.SetTileType(m_TileCoord, Tile.TileType.Soil);
		}
		StopUsing();
	}

	private ActionType GetActionFromScythe(AFO Info)
	{
		Info.m_UseAction = UseScythe;
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

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (FarmerStateScythe.GetIsToolAcceptable(Info.m_ObjectType))
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
		State state = m_State;
		m_State = NewState;
		switch (m_State)
		{
		case State.GrowingNothing:
			SetScale(0f);
			base.enabled = true;
			break;
		case State.GrowingSmall:
			SetScale(0.3f);
			base.enabled = true;
			if (state != State.GrowingSmall)
			{
				m_Wobbler.Go(0.5f, 5f, 0.5f);
			}
			break;
		case State.Grown:
			SetScale(1f);
			base.enabled = true;
			if (state != State.Grown)
			{
				m_Wobbler.Go(0.5f, 5f, 0.5f);
			}
			break;
		}
		m_StateTimer = 0f;
	}

	public void SetGrowing()
	{
		SetState(State.GrowingNothing);
		base.enabled = true;
	}

	public bool GetGrown()
	{
		if (m_State == State.Grown)
		{
			return true;
		}
		return false;
	}

	public void Cut(bool Create)
	{
		BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Pumpkin, base.transform.position, Quaternion.identity);
		AudioManager.Instance.StartEvent("ObjectCreated", baseClass.GetComponent<TileCoordObject>());
		StopUsing();
		if (!SaveLoadManager.Instance.m_Loading)
		{
			ModManager.Instance.CheckItemTypeSpawnedCallback(baseClass.m_UniqueID, baseClass.m_TypeIdentifier, baseClass.GetComponent<TileCoordObject>().m_TileCoord);
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
				AudioManager.Instance.StartEvent("CropGrowing", this);
				SetState(State.GrowingSmall);
			}
			break;
		case State.GrowingSmall:
			if (m_StateTimer > m_GrowingSmallDelay)
			{
				SetState(State.Grown);
				AudioManager.Instance.StartEvent("CropGrown", this);
			}
			break;
		case State.Grown:
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
			LoadNewModel("Models/Crops/CropPumpkinCultivated");
		}
		else
		{
			LoadNewModel("Models/Crops/CropPumpkin", RandomVariants: true);
		}
		UpdatePlotVisibility();
		base.UpdateWorldCreated();
	}
}
