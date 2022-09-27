using SimpleJSON;
using UnityEngine;

public class StorageFertiliser : Storage
{
	private enum State
	{
		Idle,
		Converting,
		Total
	}

	private State m_State;

	private float m_StateTimer;

	public float m_Fraction;

	private Transform m_Hinge;

	private Transform m_Hinge2;

	private float m_OpenTimer2;

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(0, 0), new TileCoord(1, 1), new TileCoord(0, 2));
		SetState(State.Idle);
		SetObjectType(ObjectType.Fertiliser);
		m_Fraction = 0f;
	}

	protected new void Awake()
	{
		base.Awake();
		m_Capacity = 100;
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_Hinge = m_ModelRoot.transform.Find("Hinge");
		m_Hinge2 = m_ModelRoot.transform.Find("Hinge2");
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		JSONUtils.Set(Node, "Fraction", m_Fraction);
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		m_Fraction = JSONUtils.GetAsFloat(Node, "Fraction", 0f);
	}

	private float GetObjectFertiliserValue(ObjectType NewType)
	{
		if (Food.GetIsTypeFood(NewType))
		{
			return VariableManager.Instance.GetVariableAsFloat("FertiliserValueFood");
		}
		return VariableManager.Instance.GetVariableAsFloat(NewType, "FertiliserValue");
	}

	private bool CanAcceptObjectType(ObjectType NewType)
	{
		if (!Food.GetIsTypeFood(NewType) && NewType != ObjectType.Fertiliser && NewType != ObjectType.Manure && NewType != ObjectType.Straw && NewType != ObjectType.TreeSeed && NewType != ObjectType.TreeMulberry && NewType != ObjectType.WheatSeed && NewType != ObjectType.Coconut && NewType != ObjectType.CarrotSeed && NewType != ObjectType.CottonSeeds && NewType != ObjectType.BullrushesSeeds && NewType != ObjectType.WeedDug && NewType != ObjectType.Pumpkin && NewType != ObjectType.GrassCut && NewType != ObjectType.Turf && NewType != ObjectType.Seedling && NewType != ObjectType.SeedlingMulberry && NewType != ObjectType.HayBale && NewType != ObjectType.Dough && NewType != ObjectType.DoughGood)
		{
			return false;
		}
		return true;
	}

	protected override bool CanAcceptObject(BaseClass NewObject, ObjectType NewType)
	{
		if (!CanAcceptObjectType(NewType))
		{
			return false;
		}
		return base.CanAcceptObject(NewObject, NewType);
	}

	public override int GetStoredForDisplay()
	{
		int num = base.GetStoredForDisplay();
		if (m_State == State.Converting)
		{
			num--;
		}
		return num;
	}

	private void StartAddOrganic(AFO Info)
	{
		Info.m_Object.transform.position = m_ModelRoot.transform.position;
		StartOpenLid();
	}

	private void EndAddOrganic(AFO Info)
	{
		Actionable @object = Info.m_Object;
		AudioManager.Instance.StartEvent("BuildingStorageAdd", this);
		float objectFertiliserValue = GetObjectFertiliserValue(@object.m_TypeIdentifier);
		m_Fraction += objectFertiliserValue;
		int num = (int)m_Fraction;
		m_Fraction -= num;
		if (num > 0)
		{
			AddToStored(m_ObjectType, num, Info.m_Actioner);
			if (@object.m_TypeIdentifier != ObjectType.Fertiliser)
			{
				SetState(State.Converting);
			}
			QuestManager.Instance.AddEvent(QuestEvent.Type.Make, Info.m_Actioner.m_TypeIdentifier == ObjectType.Worker, ObjectType.Fertiliser, this, num);
		}
		@object.StopUsing();
	}

	private void AbortAddOrganic(AFO Info)
	{
	}

	private ActionType GetActionFromOrganic(AFO Info)
	{
		Info.m_StartAction = StartAddOrganic;
		Info.m_EndAction = EndAddOrganic;
		Info.m_AbortAction = AbortAddOrganic;
		Info.m_FarmerState = Farmer.State.Adding;
		if (m_State == State.Converting)
		{
			return ActionType.Total;
		}
		if (m_State != 0)
		{
			return ActionType.Fail;
		}
		int stored = GetStored(CheckStack: true, CheckReserved: false);
		int capacity = GetCapacity();
		if (stored >= capacity)
		{
			return ActionType.Fail;
		}
		return ActionType.AddResource;
	}

	private void StartRelease(AFO Info)
	{
		ReleaseStored(m_ObjectType, Info.m_Actioner);
		AudioManager.Instance.StartEvent("BuildingStorageTake", this);
		Info.m_Actioner.GetComponent<Farmer>().m_FarmerCarry.AddTempCarry(m_ObjectType);
		StartOpenLid2();
	}

	private void AbortRelease(AFO Info)
	{
		Info.m_Actioner.GetComponent<Farmer>().m_FarmerCarry.RemoveTempCarry();
		AddToStored(m_ObjectType, 1, Info.m_Actioner);
	}

	private new ActionType GetActionFromNothing(AFO Info)
	{
		Info.m_StartAction = StartRelease;
		Info.m_AbortAction = AbortRelease;
		Info.m_FarmerState = Farmer.State.Taking;
		if (m_State != 0)
		{
			return ActionType.Fail;
		}
		if (GetStored() == 0)
		{
			return ActionType.Fail;
		}
		return ActionType.TakeResource;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (Info.m_ActionType == AFO.AT.Primary)
		{
			return GetActionFromNothing(Info);
		}
		if (Info.m_ActionType == AFO.AT.Secondary && CanAcceptObjectType(Info.m_ObjectType))
		{
			return GetActionFromOrganic(Info);
		}
		return ActionType.Total;
	}

	private void SetState(State NewState)
	{
		m_State = NewState;
		m_StateTimer = 0f;
	}

	private void UpdateConverting()
	{
		if ((int)(m_StateTimer * 60f) % 15 < 7)
		{
			m_ModelRoot.transform.localScale = new Vector3(0.9f, 1.3f, 0.9f);
		}
		else
		{
			m_ModelRoot.transform.localScale = new Vector3(1f, 1f, 1f);
		}
		if (m_StateTimer >= VariableManager.Instance.GetVariableAsFloat(ObjectType.StorageFertiliser, "ConversionDelay"))
		{
			m_ModelRoot.transform.localScale = new Vector3(1f, 1f, 1f);
			SetState(State.Idle);
		}
	}

	private void UpdateLid2()
	{
		if (m_OpenTimer2 > 0f)
		{
			m_OpenTimer2 -= TimeManager.Instance.m_NormalDelta;
			if (m_OpenTimer2 <= 0f)
			{
				m_OpenTimer2 = 0f;
				CloseLid2();
			}
		}
	}

	protected override void Update()
	{
		base.Update();
		UpdateLid2();
		State state = m_State;
		if (state == State.Converting)
		{
			UpdateConverting();
		}
		m_StateTimer += TimeManager.Instance.m_NormalDelta;
	}

	public override void StartOpenLid()
	{
		base.StartOpenLid();
		m_Hinge.localRotation = Quaternion.Euler(-20f, 0f, 180f);
	}

	public override void CloseLid()
	{
		base.CloseLid();
		m_Hinge.localRotation = Quaternion.Euler(-90f, 0f, 180f);
	}

	public void StartOpenLid2()
	{
		m_OpenTimer2 = 0.2f;
		m_Hinge2.localRotation = Quaternion.Euler(30f, 0f, 180f);
	}

	public void CloseLid2()
	{
		m_Hinge2.localRotation = Quaternion.Euler(-90f, 0f, 180f);
	}
}
