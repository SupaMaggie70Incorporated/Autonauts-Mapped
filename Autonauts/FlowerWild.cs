using System;
using SimpleJSON;
using UnityEngine;

public class FlowerWild : Flora
{
	public enum Type
	{
		Flower1,
		Flower2,
		Flower3,
		Flower4,
		Flower5,
		Flower6,
		Flower7,
		Total
	}

	public enum State
	{
		Growing,
		Idle,
		RenewingPollen,
		Total
	}

	private static float m_GrowTime = 10f;

	private static float m_PollenRenewTime = 10f;

	private static float m_GrowingScale = 0.2f;

	[HideInInspector]
	public Type m_Type;

	[HideInInspector]
	public static string[] m_TypeNames = new string[7] { "FlowerWild1", "FlowerWild2", "FlowerWild3", "FlowerWild4", "FlowerWild5", "FlowerWild6", "FlowerWild7" };

	public static string[] m_ModelNames = new string[7] { "Flower01", "Flower02", "Flower03", "Flower04", "Flower05", "Flower06", "Flower07" };

	public static ObjectType[] m_SeedTypes = new ObjectType[7]
	{
		ObjectType.FlowerSeeds01,
		ObjectType.FlowerSeeds02,
		ObjectType.FlowerSeeds03,
		ObjectType.FlowerSeeds04,
		ObjectType.FlowerSeeds05,
		ObjectType.FlowerSeeds06,
		ObjectType.FlowerSeeds07
	};

	public static ObjectType[] m_BunchTypes = new ObjectType[7]
	{
		ObjectType.FlowerBunch01,
		ObjectType.FlowerBunch02,
		ObjectType.FlowerBunch03,
		ObjectType.FlowerBunch04,
		ObjectType.FlowerBunch05,
		ObjectType.FlowerBunch06,
		ObjectType.FlowerBunch07
	};

	public State m_State;

	private float m_StateTimer;

	private Wobbler m_Wobbler;

	private State m_RefreshState;

	private float m_RefreshStateTimer;

	public override void RegisterClass()
	{
		base.RegisterClass();
		for (int i = 0; i < m_ModelNames.Length; i++)
		{
			ModelManager.Instance.AddModel("Models/Crops/" + m_ModelNames[i], ObjectType.FlowerWild);
		}
		ObjectTypeList.Instance.SetTypeSleep(ObjectType.FlowerWild);
	}

	public override void Restart()
	{
		base.Restart();
		if (!ObjectTypeList.m_Loading)
		{
			CollectionManager.Instance.AddCollectable("FlowerWild", this);
		}
		m_Wobbler.Restart();
		m_Type = Type.Total;
		SetState(State.Idle);
	}

	protected new void Awake()
	{
		base.Awake();
		m_Wobbler = new Wobbler();
	}

	public void SetGrowing()
	{
		SetState(State.Growing);
		Wake();
	}

	public override string GetHumanReadableName()
	{
		string text = base.GetHumanReadableName();
		if (m_Type != Type.Total)
		{
			string text2 = TextManager.Instance.Get(m_TypeNames[(int)m_Type]);
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
		CollectionManager.Instance.AddCollectable("FlowerWild", this);
		m_RefreshState = (State)JSONUtils.GetAsInt(Node, "ST", 0);
		m_RefreshStateTimer = JSONUtils.GetAsFloat(Node, "STT", 0f);
		m_Type = (Type)JSONUtils.GetAsInt(Node, "Type", 0);
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
			UpdateModel();
			SetState(m_RefreshState);
			m_StateTimer = m_RefreshStateTimer;
			break;
		}
		base.SendAction(Info);
	}

	public void BeeUsed()
	{
		SetState(State.RenewingPollen);
	}

	private void UseScythe(AFO Info)
	{
		AudioManager.Instance.StartEvent("CropRuffle", this);
		m_Wobbler.Go(0.5f, 5f, 0.5f);
		Wake();
	}

	private void EndScythe(AFO Info)
	{
		if (FarmerStateScythe.GetIsToolAcceptable(Info.m_Actioner.GetComponent<Farmer>().m_FarmerCarry.GetLastObjectType()))
		{
			TileCoord randomEmptyTile = TileHelpers.GetRandomEmptyTile(m_TileCoord);
			ObjectType bunchType = GetBunchType(m_Type);
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(bunchType, randomEmptyTile.ToWorldPositionTileCentered(), base.transform.localRotation);
			SpawnAnimationManager.Instance.AddJump(baseClass, m_TileCoord, randomEmptyTile, 0f, baseClass.transform.position.y, 3f);
			int num = UnityEngine.Random.Range(1, 3);
			for (int i = 0; i < num; i++)
			{
				randomEmptyTile = TileHelpers.GetRandomEmptyTile(m_TileCoord);
				bunchType = GetSeedType(m_Type);
				BaseClass baseClass2 = ObjectTypeList.Instance.CreateObjectFromIdentifier(bunchType, randomEmptyTile.ToWorldPositionTileCentered(), base.transform.localRotation);
				SpawnAnimationManager.Instance.AddJump(baseClass2, m_TileCoord, randomEmptyTile, 0f, baseClass2.transform.position.y, 3f);
			}
			AudioManager.Instance.StartEvent("ObjectCreated", this);
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
			if (m_State == State.Idle || m_State == State.RenewingPollen)
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

	private ActionType GetActionFromShovel(AFO Info)
	{
		Info.m_UseAction = UseShovel;
		Info.m_EndAction = EndScythe;
		Info.m_FarmerState = Farmer.State.Shovel;
		if (Info.m_ActionType == AFO.AT.Primary)
		{
			if (m_State == State.Idle || m_State == State.RenewingPollen)
			{
				return ActionType.UseInHands;
			}
			return ActionType.Fail;
		}
		return ActionType.Total;
	}

	private void EndBee(AFO Info)
	{
		SetState(State.RenewingPollen);
	}

	private ActionType GetActionFromBee(AFO Info)
	{
		Info.m_EndAction = EndBee;
		if (m_State == State.Idle)
		{
			return ActionType.UseInHands;
		}
		return ActionType.Total;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		ObjectType objectType = Info.m_ObjectType;
		if (FarmerStateScythe.GetIsToolAcceptable(objectType))
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
		if ((bool)Info.m_Actioner && Info.m_Actioner.m_TypeIdentifier == ObjectType.AnimalBee)
		{
			return GetActionFromBee(Info);
		}
		return base.GetActionFromObject(Info);
	}

	public static ObjectType GetSeedType(Type Type)
	{
		return m_SeedTypes[(int)Type];
	}

	public static ObjectType GetBunchType(Type Type)
	{
		return m_BunchTypes[(int)Type];
	}

	private void SetState(State NewState)
	{
		if (m_State == State.Growing)
		{
			SetScale(1f);
		}
		m_State = NewState;
		m_StateTimer = 0f;
		switch (NewState)
		{
		case State.Growing:
			SetScale(m_GrowingScale);
			break;
		case State.RenewingPollen:
			Wake();
			break;
		}
	}

	public void SetType(Type NewType)
	{
		m_Type = NewType;
		UpdateModel();
	}

	public void SetTypeFromSeed(ObjectType SeedType)
	{
		Type type = (Type)(SeedType - 386);
		SetType(type);
	}

	private void UpdateModel()
	{
		if (m_Type != Type.Total)
		{
			string text = m_ModelNames[(int)m_Type];
			LoadNewModel("Models/Crops/" + text);
			UpdatePlotVisibility();
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
		case State.Growing:
			if (m_StateTimer > m_GrowTime)
			{
				AudioManager.Instance.StartEvent("CropGrown", this);
				if (!m_WorldCreated)
				{
					QuestManager.Instance.AddEvent(QuestEvent.Type.GrowFlower, Bot: false, 0, this);
				}
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
		case State.RenewingPollen:
			if (m_StateTimer > m_PollenRenewTime)
			{
				AudioManager.Instance.StartEvent("CropGrown", this);
				SetState(State.Idle);
				m_Wobbler.Go(0.5f, 5f, 0.5f);
			}
			break;
		}
		m_StateTimer += TimeManager.Instance.m_NormalDelta;
	}
}
