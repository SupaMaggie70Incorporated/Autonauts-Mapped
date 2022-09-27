using SimpleJSON;
using UnityEngine;

public class Grass : Flora
{
	private enum State
	{
		GrassGrow,
		Grass
	}

	private State m_State;

	private float m_StateTimer;

	private MeshRenderer m_Mesh;

	private Color m_GrassGrowColour;

	private Color m_GrassColour;

	private Wobbler m_Wobbler;

	private Tile m_Tile;

	private float m_NormalHeight;

	private float m_NibblePercent;

	private float m_GrowDelay;

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("Grass", m_TypeIdentifier);
		ObjectTypeList.Instance.SetTypeSleep(ObjectType.Grass);
	}

	public override void Restart()
	{
		m_State = State.Grass;
		base.Restart();
		if (!ObjectTypeList.m_Loading)
		{
			CollectionManager.Instance.AddCollectable("Grass", this);
		}
		SetHeight(0f);
		m_NormalHeight = 1f;
		m_NibblePercent = 1f;
		m_GrowDelay = VariableManager.Instance.GetVariableAsFloat(m_TypeIdentifier, "GrowDelay");
		m_Mesh = GetComponentInChildren<MeshRenderer>();
		m_Wobbler.Restart();
		m_GrassGrowColour = new Color(0.25f, 0.25f, 0.25f);
		m_GrassColour = new Color(1f, 1f, 1f);
		int num = Random.Range(0, 4);
		base.transform.rotation = Quaternion.Euler(0f, num * 90, 0f);
		NewTileCoord();
		SetState(State.Grass);
		SetHeight(m_NormalHeight);
	}

	protected new void Awake()
	{
		base.Awake();
		m_Wobbler = new Wobbler();
	}

	public override string GetHumanReadableName()
	{
		string text = base.GetHumanReadableName();
		if (m_State == State.GrassGrow)
		{
			text = text + " (" + TextManager.Instance.Get("Growing") + ")";
		}
		return text;
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
		CollectionManager.Instance.AddCollectable("Grass", this);
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
		m_Wobbler.Go(0.25f, 2f, 1f);
		Wake();
	}

	private void EndShovel(AFO Info)
	{
		Dig();
	}

	private ActionType GetActionFromShovel(AFO Info)
	{
		Info.m_UseAction = UseShovel;
		Info.m_EndAction = EndShovel;
		Info.m_FarmerState = Farmer.State.Shovel;
		if (Info.m_ActionType == AFO.AT.Primary)
		{
			if (m_State == State.Grass)
			{
				return ActionType.UseInHands;
			}
			if (m_State == State.GrassGrow && Info.m_Actioner.m_TypeIdentifier == ObjectType.FarmerPlayer)
			{
				return ActionType.UseInHands;
			}
			return ActionType.Fail;
		}
		return ActionType.Total;
	}

	private void UseScythe(AFO Info)
	{
		m_Wobbler.Go(0.25f, 2f, 1f);
		Wake();
	}

	private void EndScythe(AFO Info)
	{
		Cut(Info.m_Actioner);
	}

	private ActionType GetActionFromScythe(AFO Info)
	{
		Info.m_UseAction = UseScythe;
		Info.m_EndAction = EndScythe;
		Info.m_FarmerState = Farmer.State.Scythe;
		if (Info.m_ActionType == AFO.AT.Primary)
		{
			if (m_State == State.Grass)
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
		if ((bool)Info.m_Actioner && Animal.GetIsTypeAnimal(Info.m_Actioner.m_TypeIdentifier))
		{
			if (m_State == State.Grass)
			{
				return ActionType.Pickup;
			}
			return ActionType.Total;
		}
		if (FarmerStateScythe.GetIsToolAcceptable(objectType) || objectType == ObjectType.RockSharp)
		{
			ActionType actionFromScythe = GetActionFromScythe(Info);
			if (actionFromScythe != ActionType.Total)
			{
				return actionFromScythe;
			}
		}
		if (FarmerStateShovel.GetIsToolAcceptable(objectType))
		{
			ActionType actionFromShovel = GetActionFromShovel(Info);
			if (actionFromShovel != ActionType.Total)
			{
				return actionFromShovel;
			}
		}
		return base.GetActionFromObject(Info);
	}

	private void SetState(State NewState)
	{
		m_State = NewState;
		switch (m_State)
		{
		case State.Grass:
			m_NibblePercent = 1f;
			break;
		case State.GrassGrow:
			SetHeight(0f);
			Wake();
			m_NibblePercent = 1f;
			break;
		}
		m_StateTimer = 0f;
	}

	public void SetGrowing()
	{
		SetState(State.GrassGrow);
		Wake();
	}

	public bool GetGrown()
	{
		if (m_State == State.Grass)
		{
			return true;
		}
		return false;
	}

	private void MakeGoodies()
	{
		int num = Random.Range(1, 3);
		for (int i = 0; i < num; i++)
		{
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.GrassCut, base.transform.position, base.transform.localRotation);
			SpawnAnimationManager.Instance.AddJump(baseClass, m_TileCoord, m_TileCoord, 0f, baseClass.transform.position.y, 3f);
		}
		AudioManager.Instance.StartEvent("ObjectCreated", this);
	}

	public void Dig()
	{
		if (m_State == State.Grass)
		{
			MakeGoodies();
		}
		StopUsing();
	}

	public void Cut(BaseClass Cutter)
	{
		if ((bool)Cutter && (bool)Cutter.GetComponent<Farmer>())
		{
			MakeGoodies();
		}
		SetHeight(0f);
		SetState(State.GrassGrow);
	}

	private void SetHeight(float Height)
	{
		Vector3 localScale = base.transform.localScale;
		localScale.y = Height + 0.01f;
		base.transform.localScale = localScale;
	}

	private void UpdateStateGrassGrow()
	{
		if (!m_Tile.m_MiscObject)
		{
			m_StateTimer += TimeManager.Instance.m_NormalDelta;
			if (m_StateTimer > m_GrowDelay)
			{
				SetState(State.Grass);
				m_Wobbler.Go(0.5f, 10f, 0.25f);
				AudioManager.Instance.StartEvent("CropGrown", this);
			}
		}
	}

	public void Nibble(float Percent)
	{
		m_NibblePercent = Percent;
		m_Wobbler.Go(0.25f, 2f, 0.25f);
	}

	private void UpdateStateGrass()
	{
		m_Wobbler.Update();
		SetHeight(m_Wobbler.m_Height + m_NormalHeight * m_NibblePercent);
		if (m_Wobbler.m_Height == 0f)
		{
			Sleep();
		}
	}

	private void Update()
	{
		switch (m_State)
		{
		case State.GrassGrow:
			UpdateStateGrassGrow();
			break;
		case State.Grass:
			UpdateStateGrass();
			break;
		}
	}

	protected override void UpdateWorldCreated()
	{
	}
}
