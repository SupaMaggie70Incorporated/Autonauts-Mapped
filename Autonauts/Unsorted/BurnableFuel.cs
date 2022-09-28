using System.Collections.Generic;
using UnityEngine;

public class BurnableFuel : MonoBehaviour
{
	private static Dictionary<ObjectType, BurnableInfo.Tier> m_BurnableTiers;

	public static Dictionary<ObjectType, float> m_Fuels;

	public static void PrecacheVariables()
	{
		m_Fuels = new Dictionary<ObjectType, float>();
		for (int i = 0; i < (int)ObjectTypeList.m_Total; i++)
		{
			ObjectType objectType = (ObjectType)i;
			if (GetIsBurnableFuel(objectType))
			{
				float variableAsFloat = VariableManager.Instance.GetVariableAsFloat(objectType, "Fuel");
				m_Fuels.Add(objectType, variableAsFloat);
			}
		}
	}

	public static void Init()
	{
		m_BurnableTiers = new Dictionary<ObjectType, BurnableInfo.Tier>();
		m_BurnableTiers.Add(ObjectType.Stick, BurnableInfo.Tier.Crude);
		m_BurnableTiers.Add(ObjectType.Pole, BurnableInfo.Tier.Crude);
		m_BurnableTiers.Add(ObjectType.Plank, BurnableInfo.Tier.Crude);
		m_BurnableTiers.Add(ObjectType.Log, BurnableInfo.Tier.Crude);
		m_BurnableTiers.Add(ObjectType.Charcoal, BurnableInfo.Tier.Normal);
		m_BurnableTiers.Add(ObjectType.Coal, BurnableInfo.Tier.Super);
		m_BurnableTiers.Add(ObjectType.HayBale, BurnableInfo.Tier.Hay);
		m_BurnableTiers.Add(ObjectType.Fertiliser, BurnableInfo.Tier.Fertiliser);
	}

	public static bool GetIsBurnableFuel(ObjectType NewType)
	{
		return m_BurnableTiers.ContainsKey(NewType);
	}

	public static BurnableInfo.Tier GetFuelTier(ObjectType NewType)
	{
		if (GetIsBurnableFuel(NewType))
		{
			return m_BurnableTiers[NewType];
		}
		return BurnableInfo.Tier.Total;
	}

	public static float GetFuelEnergy(ObjectType NewType)
	{
		if (GetIsBurnableFuel(NewType))
		{
			return VariableManager.Instance.GetVariableAsFloat(NewType, "Fuel");
		}
		return 0f;
	}
}
