using SimpleJSON;
using UnityEngine;

public class TrainRefuellingStation : TrainTrackStop
{
	private enum State
	{
		Idle,
		Refuelling,
		Total
	}

	public static float m_MaxWater = 40f;

	public static float m_MaxFuel = 2000f;

	public static float m_FullPercent = 0.95f;

	private State m_State;

	private float m_StateTimer;

	public float m_Water;

	public float m_Fuel;

	private PlaySound m_PlaySound;

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(0, 0), new TileCoord(0, 0), new TileCoord(0, -1));
		HideAccessModel(Hide: false);
		ChangeAccessPointToIn();
		m_State = State.Idle;
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		SetState(State.Idle);
		base.StopUsing(AndDestroy);
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		JSONUtils.Set(Node, "Water", m_Water);
		JSONUtils.Set(Node, "Fuel", m_Fuel);
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		m_Fuel = JSONUtils.GetAsFloat(Node, "Fuel", 0f);
		m_Water = JSONUtils.GetAsFloat(Node, "Water", 0f);
	}

	public void AddWater(float Amount)
	{
		m_Water += Amount;
		if (m_Water > m_MaxWater)
		{
			m_Water = m_MaxWater;
		}
	}

	public void RemoveWater(float Amount)
	{
		m_Water -= Amount;
		if (m_Water < 0f)
		{
			m_Water = 0f;
		}
	}

	private void AddWater(ToolFillable Bucket)
	{
		float amount = Bucket.m_Stored;
		Bucket.Empty(Bucket.m_Stored);
		AddWater(amount);
	}

	public void AddFuel(float Amount)
	{
		m_Fuel += Amount;
		if (m_Fuel > m_MaxFuel)
		{
			m_Fuel = m_MaxFuel;
		}
	}

	public void RemoveFuel(float Amount)
	{
		m_Fuel -= Amount;
		if (m_Fuel < 0f)
		{
			m_Fuel = 0f;
		}
	}

	private void AddFuel(Holdable NewObject)
	{
		float variableAsFloat = VariableManager.Instance.GetVariableAsFloat(NewObject.m_TypeIdentifier, "Fuel", CheckValid: false);
		AddFuel(variableAsFloat);
	}

	private bool GetHasFuelAndWater()
	{
		if (m_Fuel > 0f && m_Water > 0f)
		{
			return true;
		}
		return false;
	}

	public float GetFuelPercent()
	{
		return m_Fuel / m_MaxFuel;
	}

	public float GetWaterPercent()
	{
		return m_Water / m_MaxWater;
	}

	public void StartRefuelling()
	{
		SetState(State.Refuelling);
	}

	private void SetState(State NewState)
	{
		State state = m_State;
		if (state == State.Refuelling)
		{
			StopRefuelling();
			if (m_PlaySound != null)
			{
				AudioManager.Instance.StopEvent(m_PlaySound);
				m_PlaySound = null;
			}
		}
		m_State = NewState;
		m_StateTimer = 0f;
		state = m_State;
		if (state == State.Refuelling && m_PlaySound == null)
		{
			m_PlaySound = AudioManager.Instance.StartEvent("TrainRefuelling", this, Remember: true);
		}
		ModManager.Instance.CheckBuildingStateChangedCallback(m_UniqueID, m_State.ToString());
	}

	public void StartActionAddFuel(AFO Info)
	{
		Actionable @object = Info.m_Object;
		@object.GetComponent<Savable>().SetIsSavable(IsSavable: false);
		AddFuel(@object.GetComponent<Holdable>());
	}

	private void EndAddFuel(AFO Info)
	{
		Info.m_Object.StopUsing();
	}

	private void AbortAddFuel(AFO Info)
	{
		Actionable @object = Info.m_Object;
		m_Fuel -= BurnableFuel.GetFuelEnergy(@object.m_TypeIdentifier);
		@object.GetComponent<Savable>().SetIsSavable(IsSavable: true);
	}

	private ActionType GetActionFromFuel(AFO Info)
	{
		Info.m_StartAction = StartActionAddFuel;
		Info.m_EndAction = EndAddFuel;
		Info.m_AbortAction = AbortAddFuel;
		Info.m_FarmerState = Farmer.State.Adding;
		float fuelEnergy = BurnableFuel.GetFuelEnergy(Info.m_ObjectType);
		if (m_Fuel != 0f && m_Fuel + fuelEnergy > m_MaxFuel)
		{
			return ActionType.Fail;
		}
		return ActionType.AddResource;
	}

	public void StartActionAddFillable(AFO Info)
	{
		ToolFillable component = Info.m_Object.GetComponent<ToolFillable>();
		int num = component.m_Stored;
		if (m_Water + (float)num > m_MaxWater)
		{
			num = Mathf.CeilToInt(m_MaxWater - m_Water);
		}
		component.Empty(num);
		AddWater(num);
	}

	private void EndAddFillable(AFO Info)
	{
	}

	private void AbortAddFillable(AFO Info)
	{
	}

	private ActionType GetActionFromFillable(AFO Info)
	{
		Info.m_StartAction = StartActionAddFillable;
		Info.m_EndAction = EndAddFillable;
		Info.m_AbortAction = AbortAddFillable;
		Info.m_FarmerState = Farmer.State.Adding;
		if (Info.m_Object == null)
		{
			return ActionType.Fail;
		}
		ToolFillable component = Info.m_Object.GetComponent<ToolFillable>();
		if (component == null)
		{
			return ActionType.Fail;
		}
		if (component.m_HeldType != ObjectType.Water)
		{
			return ActionType.Fail;
		}
		if (m_Water == m_MaxWater)
		{
			return ActionType.Fail;
		}
		return ActionType.AddResource;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (Info.m_ActionType == AFO.AT.Secondary)
		{
			if (Train.IsObjectTypeAcceptableFuel(Info.m_ObjectType))
			{
				ActionType actionFromFuel = GetActionFromFuel(Info);
				if (actionFromFuel != ActionType.Total)
				{
					return actionFromFuel;
				}
			}
			if (ToolFillable.GetIsTypeFillable(Info.m_ObjectType))
			{
				ActionType actionFromFillable = GetActionFromFillable(Info);
				if (actionFromFillable != ActionType.Total)
				{
					return actionFromFillable;
				}
			}
		}
		return base.GetActionFromObject(Info);
	}

	private void UpdateRefuelling()
	{
		float num = 0f;
		if (m_StateTimer >= Train.m_RefuellingTime)
		{
			SetState(State.Idle);
		}
		else if ((int)(m_StateTimer * 60f) % 20 < 10)
		{
			num = 0.1f;
		}
		base.transform.localScale = new Vector3(1f - num, 1f + num, 1f - num);
	}

	private void StopRefuelling()
	{
		base.transform.localScale = new Vector3(1f, 1f, 1f);
	}

	protected new void Update()
	{
		if (m_State == State.Refuelling)
		{
			UpdateRefuelling();
		}
		m_StateTimer += TimeManager.Instance.m_NormalDelta;
		base.Update();
	}
}
