public class Top : Clothing
{
	public static bool GetIsTypeTop(ObjectType NewType)
	{
		if (NewType == ObjectType.TopPoncho || NewType == ObjectType.TopJumper || NewType == ObjectType.TopJacket || NewType == ObjectType.TopBlazer || NewType == ObjectType.TopTuxedo || NewType == ObjectType.TopTunic || NewType == ObjectType.TopDress || NewType == ObjectType.TopShirt || NewType == ObjectType.TopShirtTie || NewType == ObjectType.TopGown || NewType == ObjectType.TopToga || NewType == ObjectType.TopRobe || NewType == ObjectType.TopCoat || NewType == ObjectType.TopCoatScarf || NewType == ObjectType.TopSuit || NewType == ObjectType.TopDungarees || NewType == ObjectType.TopLumberjack || NewType == ObjectType.TopTShirtShow || NewType == ObjectType.TopAdventurer || NewType == ObjectType.TopMac || NewType == ObjectType.TopPlumber || NewType == ObjectType.TopTree || NewType == ObjectType.TopDungareesClown || NewType == ObjectType.TopJumper02 || NewType == ObjectType.TopApron || NewType == ObjectType.TopSanta || NewType == ObjectType.TopWally || NewType == ObjectType.TopTShirt02 || NewType == ObjectType.TopFox || NewType == ObjectType.TopDinosaur || NewType == ObjectType.TopBunny || NewType == ObjectType.TopDuck || NewType == ObjectType.TopFrog || NewType == ObjectType.TopPanda || NewType == ObjectType.TopPenguin || NewType == ObjectType.TopRoyalGuard || NewType == ObjectType.TopSuit02 || ModManager.Instance.ModTopClass.IsItCustomType(NewType))
		{
			return true;
		}
		return false;
	}

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("Top", m_TypeIdentifier);
	}
}
