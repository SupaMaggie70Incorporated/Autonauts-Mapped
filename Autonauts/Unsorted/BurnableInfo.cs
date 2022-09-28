public class BurnableInfo
{
	public enum Tier
	{
		Crude,
		Normal,
		Super,
		Hay,
		Fertiliser,
		Total
	}

	public static ObjectType GetObjectTypeFromTier(Tier NewTier)
	{
		ObjectType result = ObjectType.Log;
		switch (NewTier)
		{
		case Tier.Normal:
			result = ObjectType.Charcoal;
			break;
		case Tier.Super:
			result = ObjectType.Coal;
			break;
		case Tier.Hay:
			result = ObjectType.HayBale;
			break;
		case Tier.Fertiliser:
			result = ObjectType.Fertiliser;
			break;
		}
		return result;
	}
}
