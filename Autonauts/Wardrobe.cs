using SimpleJSON;
using UnityEngine;

public class Wardrobe : Wonder
{
	private enum State
	{
		Idle,
		Changing,
		Total
	}

	private State m_State;

	private float m_StateTimer;

	private Transform m_IngredientsRoot;

	private GameObject m_Hinge1;

	private GameObject m_Hinge2;

	public override void Restart()
	{
		base.Restart();
		if (!ObjectTypeList.m_Loading)
		{
			CollectionManager.Instance.AddCollectable("Wardrobe", this);
		}
		SetDimensions(new TileCoord(0, 0), new TileCoord(1, 0), new TileCoord(0, 1));
		SetState(State.Idle);
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_IngredientsRoot = m_ModelRoot.transform.Find("IngredientsPoint");
		if ((bool)m_ModelRoot.transform.Find("Hinge1"))
		{
			m_Hinge1 = m_ModelRoot.transform.Find("Hinge1").gameObject;
			m_Hinge2 = m_ModelRoot.transform.Find("Hinge2").gameObject;
		}
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		CollectionManager.Instance.AddCollectable("Wardrobe", this);
	}

	private bool IsBusy()
	{
		if (m_State != 0)
		{
			return true;
		}
		return false;
	}

	private void OpenDoors()
	{
		if ((bool)m_Hinge1)
		{
			m_Hinge1.transform.localRotation = Quaternion.Euler(-90f, 0f, -70f);
			m_Hinge2.transform.localRotation = Quaternion.Euler(-90f, 0f, 70f);
		}
	}

	private void CloseDoors()
	{
		if ((bool)m_Hinge1)
		{
			m_Hinge1.transform.localRotation = Quaternion.Euler(-90f, 0f, 180f);
			m_Hinge2.transform.localRotation = Quaternion.Euler(-90f, 0f, 180f);
		}
	}

	public override void SendAction(ActionInfo Info)
	{
		base.SendAction(Info);
		if (IsBusy() || ((bool)m_Engager && Info.m_Action != ActionType.Disengaged))
		{
			return;
		}
		switch (Info.m_Action)
		{
		case ActionType.Engaged:
			if (GameStateManager.Instance.GetActualState() == GameStateManager.State.Normal && (bool)Info.m_Object.GetComponent<FarmerPlayer>())
			{
				m_Engager = Info.m_Object;
				OpenDoors();
				GameStateManager.Instance.StartWardrobe(this);
			}
			break;
		case ActionType.Disengaged:
			m_Engager = null;
			CloseDoors();
			break;
		}
	}

	public override bool CanDoAction(ActionInfo Info, bool RightNow)
	{
		if (IsBusy() && RightNow)
		{
			return false;
		}
		if ((bool)m_Engager && Info.m_Action != ActionType.Disengaged)
		{
			return false;
		}
		switch (Info.m_Action)
		{
		case ActionType.Engaged:
			if (GameStateManager.Instance.GetActualState() == GameStateManager.State.Normal && m_Engager == null)
			{
				return true;
			}
			return false;
		case ActionType.Disengaged:
			if ((bool)m_Engager)
			{
				return true;
			}
			return false;
		default:
			return false;
		}
	}

	public override object GetActionInfo(GetActionInfo Info)
	{
		switch (Info.m_Action)
		{
		case GetAction.IsMovable:
			if (IsBusy())
			{
				return false;
			}
			return !m_DoingAction;
		case GetAction.IsBusy:
			if (IsBusy())
			{
				return true;
			}
			return m_DoingAction;
		default:
			return base.GetActionInfo(Info);
		}
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (Info.m_ActionType == AFO.AT.Primary)
		{
			Info.m_FarmerState = Farmer.State.Engaged;
			if (m_State != 0)
			{
				return ActionType.Fail;
			}
			if (GameStateManager.Instance.GetActualState() == GameStateManager.State.Normal && m_Engager == null)
			{
				return ActionType.EngageObject;
			}
			return ActionType.Fail;
		}
		return ActionType.Total;
	}

	private void SetState(State NewState)
	{
		m_State = NewState;
		m_StateTimer = 0f;
		ModManager.Instance.CheckBuildingStateChangedCallback(m_UniqueID, m_State.ToString());
	}

	private void UpdateChangingAnimation()
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

	private void UpdateChanging()
	{
		UpdateChangingAnimation();
	}

	public bool CanAdd(Holdable NewObject)
	{
		return WardrobeManager.Instance.CanAdd(NewObject);
	}

	public Holdable ReleaseObject(int Index)
	{
		return WardrobeManager.Instance.ReleaseObject(Index);
	}

	public void AttemptAdd(Holdable NewObject)
	{
		WardrobeManager.Instance.AttemptAdd(NewObject);
	}

	public int GetCapacity()
	{
		return WardrobeManager.Instance.m_Capacity;
	}

	public Holdable GetObject(int Index)
	{
		return WardrobeManager.Instance.GetObject(Index);
	}

	private void Update()
	{
		State state = m_State;
		if (state == State.Changing)
		{
			UpdateChanging();
		}
		m_StateTimer += TimeManager.Instance.m_NormalDelta;
	}
}
