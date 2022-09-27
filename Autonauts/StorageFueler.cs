using SimpleJSON;

public class StorageFueler : Storage
{
	public BurnableInfo.Tier m_Tier;

	public float m_FuelCapacity;

	public float m_Fuel;

	public ObjectType m_RequiredType;

	public int m_RequiredCount;

	public override void Restart()
	{
		base.Restart();
		m_Fuel = 0f;
		m_RequiredCount = 0;
	}

	protected new void Awake()
	{
		base.Awake();
		m_Tier = BurnableInfo.Tier.Crude;
		m_FuelCapacity = 100f;
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		JSONUtils.Set(Node, "Fuel", m_Fuel);
		JSONUtils.Set(Node, "RequiredCount", m_RequiredCount);
	}

	public override void Load(JSONNode Node)
	{
		m_RequiredCount = JSONUtils.GetAsInt(Node, "RequiredCount", 0);
		if (m_RequiredCount < 0)
		{
			m_RequiredCount = 0;
		}
		m_Fuel = JSONUtils.GetAsFloat(Node, "Fuel", 0f);
		if (m_Fuel < 0f)
		{
			m_Fuel = 0f;
		}
		base.Load(Node);
	}

	public float GetFuelPercent()
	{
		return m_Fuel / m_FuelCapacity;
	}

	private float GetFuelRequired()
	{
		float result = 0f;
		if (m_ObjectType != ObjectTypeList.m_Total)
		{
			result = VariableManager.Instance.GetVariableAsFloat(m_ObjectType, "FuelRequired");
		}
		return result;
	}

	protected virtual void DoConversion(Actionable Actioner)
	{
		AddToStored(m_ObjectType, 1, Actioner);
		UpdateStored();
		m_Fuel -= GetFuelRequired();
		if (m_Fuel < 0f)
		{
			m_Fuel = 0f;
		}
		m_RequiredCount--;
		if (m_RequiredCount < 0)
		{
			m_RequiredCount = 0;
		}
	}

	private bool AreRequrementsMet()
	{
		if (m_Fuel < GetFuelRequired())
		{
			return false;
		}
		if (m_RequiredCount == 0)
		{
			return false;
		}
		if (GetStored() == GetCapacity())
		{
			return false;
		}
		return true;
	}

	protected void StartAddFuel(AFO Info)
	{
		Actionable @object = Info.m_Object;
		@object.GetComponent<Savable>().SetIsSavable(IsSavable: false);
		m_Fuel += BurnableFuel.GetFuelEnergy(@object.m_TypeIdentifier);
		if (m_Fuel > m_FuelCapacity)
		{
			m_Fuel = m_FuelCapacity;
		}
	}

	protected void EndAddFuel(AFO Info)
	{
		Info.m_Object.StopUsing();
	}

	protected void AbortAddFuel(AFO Info)
	{
		Actionable @object = Info.m_Object;
		m_Fuel -= BurnableFuel.GetFuelEnergy(@object.m_TypeIdentifier);
		if (m_Fuel < 0f)
		{
			m_Fuel = 0f;
		}
		@object.GetComponent<Savable>().SetIsSavable(IsSavable: true);
	}

	protected virtual ActionType GetActionFromFuel(AFO Info)
	{
		Info.m_StartAction = StartAddFuel;
		Info.m_EndAction = EndAddFuel;
		Info.m_AbortAction = AbortAddFuel;
		Info.m_FarmerState = Farmer.State.Adding;
		if (m_Fuel == m_FuelCapacity)
		{
			return ActionType.Fail;
		}
		if (m_Fuel + BurnableFuel.GetFuelEnergy(Info.m_ObjectType) > m_FuelCapacity)
		{
			return ActionType.Fail;
		}
		return ActionType.AddResource;
	}

	protected virtual void StartAddRequiredType(AFO Info)
	{
		Actionable @object = Info.m_Object;
		if (m_RequiredType != @object.m_TypeIdentifier)
		{
			m_RequiredType = @object.m_TypeIdentifier;
		}
		m_RequiredCount++;
		@object.GetComponent<Savable>().SetIsSavable(IsSavable: false);
	}

	protected virtual void EndAddRequiredType(AFO Info)
	{
		Info.m_Object.StopUsing();
	}

	protected void AbortAddRequiredType(AFO Info)
	{
		Actionable @object = Info.m_Object;
		m_RequiredCount--;
		if (m_RequiredCount < 0)
		{
			m_RequiredCount = 0;
		}
		@object.GetComponent<Savable>().SetIsSavable(IsSavable: true);
	}

	protected virtual ActionType GetActionFromRequiredType(AFO Info)
	{
		Info.m_StartAction = StartAddRequiredType;
		Info.m_EndAction = EndAddRequiredType;
		Info.m_AbortAction = AbortAddRequiredType;
		Info.m_FarmerState = Farmer.State.Adding;
		if (m_RequiredCount != 0)
		{
			return ActionType.Fail;
		}
		return ActionType.AddResource;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (Info.m_ActionType == AFO.AT.Secondary)
		{
			if (BurnableFuel.GetIsBurnableFuel(Info.m_ObjectType))
			{
				if (BurnableFuel.GetFuelTier(Info.m_ObjectType) == m_Tier)
				{
					ActionType actionFromFuel = GetActionFromFuel(Info);
					if (actionFromFuel != ActionType.Total)
					{
						return actionFromFuel;
					}
				}
			}
			else if (CanAcceptRequiredType(Info.m_ObjectType))
			{
				ActionType actionFromRequiredType = GetActionFromRequiredType(Info);
				if (actionFromRequiredType != ActionType.Total)
				{
					return actionFromRequiredType;
				}
			}
		}
		return base.GetActionFromObject(Info);
	}

	protected virtual bool CanAcceptRequiredType(ObjectType NewType)
	{
		if (m_RequiredType == ObjectType.Nothing)
		{
			return true;
		}
		if (m_RequiredCount == 0 && m_Stored == 0)
		{
			return true;
		}
		if (NewType == m_RequiredType)
		{
			return true;
		}
		return false;
	}

	private new void Update()
	{
		base.Update();
		if (!SaveLoadManager.Instance.m_Loading && AreRequrementsMet())
		{
			DoConversion(null);
		}
	}
}
