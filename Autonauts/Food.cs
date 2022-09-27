public class Food : Holdable
{
	public static float GetFoodEnergy(ObjectType NewType)
	{
		return VariableManager.Instance.GetVariableAsFloat(NewType, "Energy", CheckValid: false);
	}

	public static bool GetIsTypeFood(ObjectType NewType)
	{
		return GetFoodEnergy(NewType) > 0f;
	}

	public static bool GetIsTypeCookedFood(ObjectType NewType)
	{
		if (NewType == ObjectType.MushroomSoup || NewType == ObjectType.MushroomStew || NewType == ObjectType.MushroomPie || NewType == ObjectType.BerriesStew || NewType == ObjectType.BerriesJam || NewType == ObjectType.BerriesPie || NewType == ObjectType.Porridge || NewType == ObjectType.BreadCrude || NewType == ObjectType.Bread || NewType == ObjectType.BreadButtered || NewType == ObjectType.PumpkinSoup || NewType == ObjectType.PumpkinStew || NewType == ObjectType.PumpkinPie || NewType == ObjectType.AppleStew || NewType == ObjectType.AppleJam || NewType == ObjectType.ApplePie || NewType == ObjectType.FishSoup || NewType == ObjectType.FishStew || NewType == ObjectType.FishPie || NewType == ObjectType.BerriesCake || NewType == ObjectType.AppleCake || NewType == ObjectType.BreadPudding || NewType == ObjectType.CarrotCurry || NewType == ObjectType.FishCake || NewType == ObjectType.PumpkinCake || NewType == ObjectType.CarrotCake || NewType == ObjectType.MushroomPudding || NewType == ObjectType.MushroomBurger || NewType == ObjectType.FishBurger || NewType == ObjectType.CarrotBurger || NewType == ObjectType.BerryDanish || NewType == ObjectType.AppleDanish || NewType == ObjectType.PumpkinBurger || NewType == ObjectType.CreamBrioche)
		{
			return true;
		}
		return false;
	}

	public static bool GetIsTypeFood(AFO Info)
	{
		BaseClass @object = Info.m_Object;
		if (@object == null)
		{
			return false;
		}
		if (ToolFillable.GetIsTypeFillable(@object.m_TypeIdentifier))
		{
			if (Info.m_Stage == AFO.Stage.Start)
			{
				return GetIsTypeFood(@object.GetComponent<ToolFillable>().m_HeldType);
			}
			return GetIsTypeFood(@object.GetComponent<ToolFillable>().m_LastHeldType);
		}
		return GetIsTypeFood(@object.m_TypeIdentifier);
	}
}
