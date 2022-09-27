using System.Collections.Generic;
using SimpleJSON;

public class Fueler : Converter
{
	public BurnableInfo.Tier m_Tier;

	public float m_Capacity;

	public float m_Fuel;

	public static bool GetIsTypeFueler(ObjectType NewType)
	{
		if (NewType == ObjectType.Cauldron || NewType == ObjectType.ClayFurnace || NewType == ObjectType.CookingPotCrude || NewType == ObjectType.KilnCrude || NewType == ObjectType.OvenCrude || NewType == ObjectType.Oven || NewType == ObjectType.Furnace)
		{
			return true;
		}
		return false;
	}

	public override void Restart()
	{
		base.Restart();
		m_Tier = BurnableInfo.Tier.Crude;
		m_Capacity = 100f;
		m_Fuel = 0f;
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		JSONUtils.Set(Node, "Fuel", m_Fuel);
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		m_Fuel = JSONUtils.GetAsFloat(Node, "Fuel", 0f);
		FuelChanged();
	}

	public void SetFuel(float Fuel)
	{
		m_Fuel = Fuel;
	}

	public float GetFuelPercent()
	{
		return m_Fuel / m_Capacity;
	}

	private float GetFuelRequired()
	{
		if (m_ResultsToCreate == 0)
		{
			return 0f;
		}
		List<IngredientRequirement> list = m_Results[m_ResultsToCreate];
		return VariableManager.Instance.GetVariableAsFloat(list[0].m_Type, "FuelRequired", CheckValid: false);
	}

	public override bool AreRequrementsMet()
	{
		if (m_ResultsToCreate == 0)
		{
			return false;
		}
		if (!base.AreRequrementsMet())
		{
			return false;
		}
		if (m_Fuel < GetFuelRequired())
		{
			return false;
		}
		return true;
	}

	public override void StartConverting()
	{
		base.StartConverting();
		if (!SaveLoadManager.Instance.m_Loading)
		{
			m_Fuel -= GetFuelRequired();
			FuelChanged();
		}
	}

	protected virtual void FuelChanged()
	{
	}

	protected void StartAddFuel(AFO Info)
	{
		Actionable @object = Info.m_Object;
		@object.GetComponent<Savable>().SetIsSavable(IsSavable: false);
		m_Fuel += BurnableFuel.GetFuelEnergy(@object.m_TypeIdentifier);
		if (m_Fuel > m_Capacity)
		{
			m_Fuel = m_Capacity;
		}
	}

	protected void EndAddFuel(AFO Info)
	{
		Info.m_Object.StopUsing();
		if (m_ResultsToCreate != 0 && !SaveLoadManager.Instance.m_Loading && AreRequrementsMet() && m_AutoConvert)
		{
			StartConversion(Info.m_Actioner);
		}
		FuelChanged();
	}

	protected void AbortAddFuel(AFO Info)
	{
		Actionable @object = Info.m_Object;
		m_Fuel -= BurnableFuel.GetFuelEnergy(@object.m_TypeIdentifier);
		@object.GetComponent<Savable>().SetIsSavable(IsSavable: true);
	}

	protected virtual ActionType GetActionFromFuel(AFO Info)
	{
		Info.m_StartAction = StartAddFuel;
		Info.m_EndAction = EndAddFuel;
		Info.m_AbortAction = AbortAddFuel;
		Info.m_FarmerState = Farmer.State.Adding;
		if (IsBusy())
		{
			return ActionType.Fail;
		}
		ObjectType type = m_Results[m_ResultsToCreate][0].m_Type;
		if (type != ObjectTypeList.m_Total)
		{
			float variableAsFloat = VariableManager.Instance.GetVariableAsFloat(type, "FuelRequired", CheckValid: false);
			if (m_Fuel < variableAsFloat)
			{
				return ActionType.AddResource;
			}
		}
		float fuelEnergy = BurnableFuel.GetFuelEnergy(Info.m_ObjectType);
		if (m_Fuel != 0f && m_Fuel + fuelEnergy > m_Capacity)
		{
			return ActionType.Fail;
		}
		return ActionType.AddResource;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (Info.m_ActionType == AFO.AT.Secondary && IsObjectTypeAcceptableFuel(Info.m_ObjectType))
		{
			ActionType actionFromFuel = GetActionFromFuel(Info);
			if (actionFromFuel != ActionType.Total)
			{
				return actionFromFuel;
			}
		}
		return base.GetActionFromObject(Info);
	}

	public bool IsObjectTypeAcceptableFuel(ObjectType NewType)
	{
		if (!BurnableFuel.GetIsBurnableFuel(NewType))
		{
			return false;
		}
		if (BurnableFuel.GetFuelTier(NewType) != m_Tier)
		{
			return false;
		}
		return true;
	}

	public void ModFuelChanged()
	{
		FuelChanged();
	}
}
