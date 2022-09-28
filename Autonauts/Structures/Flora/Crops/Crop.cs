using SimpleJSON;
using UnityEngine;

public class Crop : Flora
{
	public enum State
	{
		Grow,
		Idle,
		Wild,
		Regrow
	}

	protected State m_State;

	private float m_StateTimer;

	private MeshRenderer m_Mesh;

	private Wobbler m_Wobbler;

	private Wobbler m_CutWobbler;

	private Tile m_Tile;

	private float m_NormalHeight;

	[HideInInspector]
	public int m_Yield;

	private int m_MaxYield;

	private float m_GrowDelay;

	[HideInInspector]
	public Tile.TileType m_StartTile;

	[HideInInspector]
	public ObjectType m_Fertiliser;

	[HideInInspector]
	public bool m_Watered;

	public static bool GetIsTypeCrop(ObjectType NewType)
	{
		if (NewType == ObjectType.CropWheat || NewType == ObjectType.CropCotton || NewType == ObjectType.CropCarrot)
		{
			return true;
		}
		return false;
	}

	public override void Restart()
	{
		m_State = State.Idle;
		m_NormalHeight = VariableManager.Instance.GetVariableAsFloat(m_TypeIdentifier, "MaxHeight");
		m_Wobbler.Restart();
		m_CutWobbler.Restart();
		m_StateTimer = 0f;
		m_StartTile = Tile.TileType.Soil;
		m_Fertiliser = ObjectTypeList.m_Total;
		m_Watered = false;
		UpdateYield();
		base.Restart();
		SetHeight(0f);
		m_Mesh = GetComponentInChildren<MeshRenderer>();
		int num = Random.Range(0, 4);
		base.transform.rotation = Quaternion.Euler(0f, num * 90, 0f);
		NewTileCoord();
		SetState(State.Idle);
		SetHeight(m_NormalHeight);
	}

	protected new void Awake()
	{
		base.Awake();
		m_Wobbler = new Wobbler();
		m_CutWobbler = new Wobbler();
	}

	private void NewTileCoord()
	{
		m_Tile = TileManager.Instance.GetTile(m_TileCoord);
		m_StartTile = m_Tile.m_TileType;
		UpdateYield();
	}

	public override string GetHumanReadableName()
	{
		string text = base.GetHumanReadableName();
		if (m_State == State.Wild)
		{
			text = text + " (" + TextManager.Instance.Get("CropWild") + ")";
		}
		if (m_State == State.Grow)
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
		JSONUtils.Set(Node, "TI", Tile.GetNameFromType(m_StartTile));
		if (m_Fertiliser != ObjectTypeList.m_Total)
		{
			JSONUtils.Set(Node, "FE", ObjectTypeList.Instance.GetSaveNameFromIdentifier(m_Fertiliser));
		}
		if (m_Watered)
		{
			JSONUtils.Set(Node, "WA", 1);
		}
	}

	public override void Load(JSONNode Node)
	{
		string asString = JSONUtils.GetAsString(Node, "TI", "");
		m_StartTile = Tile.GetTypeFromName(asString);
		asString = JSONUtils.GetAsString(Node, "FE", "");
		m_Fertiliser = ObjectTypeList.Instance.GetIdentifierFromSaveName(asString, Check: false);
		if (JSONUtils.GetAsInt(Node, "WA", 0) == 1)
		{
			m_Watered = true;
		}
		base.Load(Node);
		NewTileCoord();
		UpdateYield();
		SetState((State)JSONUtils.GetAsInt(Node, "ST", 0));
		m_StateTimer = JSONUtils.GetAsFloat(Node, "STT", 0f);
	}

	protected override void UpdateWorldCreated()
	{
		m_Mesh = GetComponentInChildren<MeshRenderer>();
		SetState(m_State);
		UpdatePlotVisibility();
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
		m_CutWobbler.Go(0.25f, 2f, 1f);
		Wake();
	}

	protected virtual void EndScythe(AFO Info)
	{
		Cut(Create: true);
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

	private void UseShovel(AFO Info)
	{
		m_Wobbler.Go(0.5f, 5f, 0.5f);
		Wake();
	}

	protected virtual void EndShovel(AFO Info)
	{
		Cut(Create: true);
	}

	private ActionType GetActionFromShovel(AFO Info)
	{
		Info.m_UseAction = UseShovel;
		Info.m_EndAction = EndShovel;
		Info.m_FarmerState = Farmer.State.Shovel;
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

	private void EndFertiliser(AFO Info)
	{
		m_CutWobbler.Go(0.25f, 2f, 0.25f);
		Wake();
		m_Fertiliser = Info.m_ObjectType;
		UpdateYield();
		Info.m_Object.StopUsing();
	}

	private ActionType GetActionFromFertiliser(AFO Info)
	{
		Info.m_EndAction = EndFertiliser;
		Info.m_FarmerState = Farmer.State.Seed;
		if (Info.m_ActionType == AFO.AT.Secondary)
		{
			if (m_Fertiliser == ObjectTypeList.m_Total)
			{
				return ActionType.UseInHands;
			}
			return ActionType.Fail;
		}
		return ActionType.Total;
	}

	private void StartFillable(AFO Info)
	{
		Actionable @object = Info.m_Object;
		if (@object.GetComponent<ToolFillable>().m_HeldType == ObjectType.Water)
		{
			m_CutWobbler.Go(0.25f, 2f, 0.25f);
			Wake();
			m_Watered = true;
			UpdateYield();
			@object.GetComponent<ToolFillable>().Empty(1);
		}
	}

	private ActionType GetActionFromFillable(AFO Info)
	{
		Info.m_StartAction = StartFillable;
		Info.m_FarmerState = Farmer.State.Adding;
		if (Info.m_Object == null)
		{
			return ActionType.Fail;
		}
		if (Info.m_ActionType == AFO.AT.Secondary)
		{
			if (!m_Watered && (Info.m_Object == null || Info.m_Object.GetComponent<ToolFillable>().m_HeldType == ObjectType.Water))
			{
				return ActionType.AddResource;
			}
			Info.m_RequirementsOut = "";
			return ActionType.Fail;
		}
		Info.m_RequirementsOut = "";
		return ActionType.Total;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		ObjectType objectType = Info.m_ObjectType;
		if (GetIsScythable() && (FarmerStateScythe.GetIsToolAcceptable(objectType) || objectType == ObjectType.RockSharp))
		{
			ActionType actionFromScythe = GetActionFromScythe(Info);
			if (actionFromScythe != ActionType.Total)
			{
				return actionFromScythe;
			}
		}
		if (GetIsDiggable() && (FarmerStateShovel.GetIsToolAcceptable(objectType) || objectType == ObjectType.Stick))
		{
			ActionType actionFromShovel = GetActionFromShovel(Info);
			if (actionFromShovel != ActionType.Total)
			{
				return actionFromShovel;
			}
		}
		if (objectType == ObjectType.Fertiliser)
		{
			ActionType actionFromFertiliser = GetActionFromFertiliser(Info);
			if (actionFromFertiliser != ActionType.Total)
			{
				return actionFromFertiliser;
			}
		}
		if (ToolFillable.GetIsTypeFillable(objectType))
		{
			ActionType actionFromFillable = GetActionFromFillable(Info);
			if (actionFromFillable != ActionType.Total)
			{
				return actionFromFillable;
			}
		}
		return base.GetActionFromObject(Info);
	}

	private bool GetIsScythable()
	{
		if (m_TypeIdentifier == ObjectType.CropWheat || m_TypeIdentifier == ObjectType.CropCotton)
		{
			return true;
		}
		return false;
	}

	private bool GetIsDiggable()
	{
		if (m_TypeIdentifier == ObjectType.CropCarrot)
		{
			return true;
		}
		return false;
	}

	public void SetState(State NewState)
	{
		State state = m_State;
		if (state == State.Regrow)
		{
			m_Mesh.enabled = true;
			Sleep();
		}
		m_State = NewState;
		switch (m_State)
		{
		case State.Idle:
			SetHeight(m_NormalHeight, m_Wobbler.m_Height + m_CutWobbler.m_Height);
			break;
		case State.Grow:
			Wake();
			break;
		case State.Regrow:
			m_Mesh.enabled = false;
			Wake();
			break;
		}
		m_StateTimer = 0f;
	}

	public void SetGrowing()
	{
		SetState(State.Grow);
		SetHeight(0f);
		Wake();
	}

	public bool GetGrown()
	{
		if (m_State == State.Idle)
		{
			return true;
		}
		return false;
	}

	public virtual void Cut(bool Create)
	{
		if (Create)
		{
			StopUsing();
		}
		else
		{
			SetState(State.Regrow);
		}
	}

	private void UpdateYield()
	{
		int num = 2;
		if (m_StartTile == Tile.TileType.SoilTilled)
		{
			num += VariableManager.Instance.GetVariableAsInt(m_TypeIdentifier, "TilledBoost");
		}
		if (m_Fertiliser == ObjectType.Fertiliser)
		{
			num += VariableManager.Instance.GetVariableAsInt(m_TypeIdentifier, "FertiliserBoost");
		}
		if (m_Watered)
		{
			num += VariableManager.Instance.GetVariableAsInt(m_TypeIdentifier, "WaterBoost");
		}
		m_Yield = num;
		m_MaxYield = VariableManager.Instance.GetVariableAsInt(m_TypeIdentifier, "MaxYield");
		UpdateGrowDelay();
	}

	private void UpdateGrowDelay()
	{
		float num = (m_GrowDelay = VariableManager.Instance.GetVariableAsFloat(m_TypeIdentifier, "GrowDelay"));
	}

	private float GetYieldPercent()
	{
		float num = (float)m_Yield / (float)m_MaxYield;
		if (num > 1f)
		{
			num = 1f;
		}
		return num;
	}

	private void SetHeight(float Height, float Addition = 0f)
	{
		Height *= GetYieldPercent();
		Height += Addition;
		Vector3 localScale = base.transform.localScale;
		localScale.y = Height + 0.01f;
		if (m_TypeIdentifier == ObjectType.CropCarrot)
		{
			if (localScale.y < 0.1f)
			{
				localScale.y = 0.1f;
			}
			localScale.x = localScale.y;
			localScale.z = localScale.y;
		}
		base.transform.localScale = localScale;
	}

	private void UpdateStateWheatGrow()
	{
		if ((bool)m_Tile.m_MiscObject)
		{
			return;
		}
		float num = m_StateTimer / m_GrowDelay;
		m_StateTimer += TimeManager.Instance.m_NormalDelta;
		if (m_StateTimer > m_GrowDelay)
		{
			SetState(State.Idle);
			m_Wobbler.Go(0.5f, 10f, 0.25f);
			AudioManager.Instance.StartEvent("CropGrown", this);
			QuestManager.Instance.AddEvent(QuestEvent.Type.GrowWheat, Bot: false, 0, this);
			return;
		}
		float num2 = m_StateTimer / m_GrowDelay;
		float num3 = 0f;
		bool flag = false;
		if (num2 >= 0.75f)
		{
			if (num < 0.75f)
			{
				flag = true;
			}
			num3 = 0.75f;
		}
		else if (num2 >= 0.5f)
		{
			if (num < 0.5f)
			{
				flag = true;
			}
			num3 = 0.5f;
		}
		else if (num2 >= 0.25f)
		{
			if (num < 0.25f)
			{
				flag = true;
			}
			num3 = 0.25f;
		}
		if (flag)
		{
			m_Wobbler.Go(0.5f, 5f, 0.125f);
			AudioManager.Instance.StartEvent("CropGrowing", this);
		}
		m_Wobbler.Update();
		SetHeight(m_NormalHeight * num3, m_Wobbler.m_Height);
	}

	private void UpdateStateWheat()
	{
		m_Wobbler.Update();
		m_CutWobbler.Update();
		SetHeight(m_NormalHeight, m_Wobbler.m_Height + m_CutWobbler.m_Height);
		if (m_Wobbler.m_Height == 0f && m_CutWobbler.m_Height == 0f)
		{
			Sleep();
		}
	}

	private void UpdateStateRegrow()
	{
		m_StateTimer += TimeManager.Instance.m_NormalDelta;
		if (m_StateTimer > 20f)
		{
			SetState(State.Grow);
		}
	}

	private void Update()
	{
		switch (m_State)
		{
		case State.Grow:
			UpdateStateWheatGrow();
			break;
		case State.Idle:
		case State.Wild:
			UpdateStateWheat();
			break;
		case State.Regrow:
			UpdateStateRegrow();
			break;
		}
	}
}
