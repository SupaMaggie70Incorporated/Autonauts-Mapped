using SimpleJSON;
using UnityEngine;

public class StoneHeads : Wonder
{
	private enum State
	{
		Idle,
		Incinerating,
		Total
	}

	private State m_State;

	private float m_StateTimer;

	protected TileCoord m_SpawnPoint;

	private PlaySound m_PlaySound;

	private Transform m_IngredientsRoot;

	public override void Restart()
	{
		base.Restart();
		if (!ObjectTypeList.m_Loading)
		{
			CollectionManager.Instance.AddCollectable("StoneHeads", this);
		}
		SetDimensions(new TileCoord(0, -1), new TileCoord(1, 0), new TileCoord(0, 1));
		SetState(State.Idle);
		ChangeAccessPointToIn();
	}

	protected new void Awake()
	{
		base.Awake();
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_IngredientsRoot = m_ModelRoot.transform.Find("IngredientsPoint");
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		CollectionManager.Instance.AddCollectable("StoneHeads", this);
	}

	private void StartAddAnything(AFO Info)
	{
		Info.m_Object.transform.position = m_IngredientsRoot.position;
	}

	private void EndAddAnything(AFO Info)
	{
		Actionable @object = Info.m_Object;
		if ((bool)GameStateManager.Instance.GetCurrentState().GetComponent<GameStateNormal>() && (bool)@object.GetComponent<Worker>())
		{
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateNormal>().RemoveSelectedWorker(@object.GetComponent<Worker>());
		}
		@object.GetComponent<TileCoordObject>().SetTilePosition(GetAccessPosition());
		@object.StopUsing();
		m_PlaySound = AudioManager.Instance.StartEvent("BuildingIncineratorIncinerating", this, Remember: true);
		SetState(State.Incinerating);
	}

	protected override ActionType GetActionFromAnything(AFO Info)
	{
		Info.m_StartAction = StartAddAnything;
		Info.m_EndAction = EndAddAnything;
		Info.m_FarmerState = Farmer.State.Adding;
		if (m_State != 0)
		{
			return ActionType.Fail;
		}
		if (Animal.GetIsTypeAnimal(Info.m_ObjectType) || Info.m_ObjectType == ObjectType.Folk || Info.m_ObjectType == ObjectType.FolkSeed || Info.m_ObjectType == ObjectType.CertificateReward || Info.m_ObjectType == ObjectType.BeesNest)
		{
			return ActionType.Fail;
		}
		if (Info.m_ObjectType == ObjectType.Worker && Info.m_Object != null && Info.m_Object.GetComponent<Worker>().m_FarmerCarry.GetTopObject() != null)
		{
			return ActionType.Fail;
		}
		return ActionType.AddResource;
	}

	private void SetState(State NewState)
	{
		m_State = NewState;
		m_StateTimer = 0f;
		ModManager.Instance.CheckBuildingStateChangedCallback(m_UniqueID, m_State.ToString());
	}

	private void UpdateIncineratingAnimation()
	{
		if ((int)(m_StateTimer * 60f) % 20 < 10)
		{
			m_ModelRoot.transform.localScale = new Vector3(0.9f, 1.3f, 0.9f);
		}
		else
		{
			m_ModelRoot.transform.localScale = new Vector3(1f, 1f, 1f);
		}
	}

	private void UpdateIncinerating()
	{
		UpdateIncineratingAnimation();
		if (m_StateTimer > 0.5f)
		{
			m_StateTimer = 0f;
			AudioManager.Instance.StopEvent(m_PlaySound);
			SetState(State.Idle);
		}
	}

	private void Update()
	{
		State state = m_State;
		if (state == State.Incinerating)
		{
			UpdateIncinerating();
		}
		m_StateTimer += TimeManager.Instance.m_NormalDelta;
	}

	public bool ModAddObject(Actionable ActionObject)
	{
		if (m_State != 0)
		{
			return false;
		}
		if (Animal.GetIsTypeAnimal(ActionObject.m_TypeIdentifier) || ActionObject.m_TypeIdentifier == ObjectType.Folk || ActionObject.m_TypeIdentifier == ObjectType.FolkSeed || ActionObject.m_TypeIdentifier == ObjectType.CertificateReward || ActionObject.m_TypeIdentifier == ObjectType.BeesNest)
		{
			return false;
		}
		if (ActionObject.m_TypeIdentifier == ObjectType.Worker && ActionObject != null && ActionObject.GetComponent<Worker>().m_FarmerCarry.GetTopObject() != null)
		{
			return false;
		}
		if ((bool)GameStateManager.Instance.GetCurrentState().GetComponent<GameStateNormal>() && (bool)ActionObject.GetComponent<Worker>())
		{
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateNormal>().RemoveSelectedWorker(ActionObject.GetComponent<Worker>());
		}
		ActionObject.GetComponent<TileCoordObject>().SetTilePosition(GetAccessPosition());
		ActionObject.StopUsing();
		m_PlaySound = AudioManager.Instance.StartEvent("BuildingIncineratorIncinerating", this, Remember: true);
		SetState(State.Incinerating);
		return true;
	}
}
