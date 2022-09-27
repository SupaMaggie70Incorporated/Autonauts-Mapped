public class Wonder : Building
{
	public static bool GetIsTypeWonder(ObjectType NewType)
	{
		if (NewType == ObjectType.StoneHeads || NewType == ObjectType.GiantWaterWheel || NewType == ObjectType.Wardrobe || NewType == ObjectType.Catapult || NewType == ObjectType.BotServer || NewType == ObjectType.StoneHenge || NewType == ObjectType.SpacePort)
		{
			return true;
		}
		return false;
	}
}
