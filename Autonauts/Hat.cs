public class Hat : Clothing
{
	public static bool GetIsTypeHat(ObjectType NewType)
	{
		if (NewType == ObjectType.HatChullo || NewType == ObjectType.HatFarmerCap || NewType == ObjectType.HatCap || NewType == ObjectType.HatFarmer || NewType == ObjectType.HatBeret || NewType == ObjectType.HatSugegasa || NewType == ObjectType.HatFez || NewType == ObjectType.HatChef || NewType == ObjectType.HatKnittedBeanie || NewType == ObjectType.HatAdventurer || NewType == ObjectType.HatWig01 || NewType == ObjectType.HatMushroom || NewType == ObjectType.HatLumberjack || NewType == ObjectType.HatBaseballShow || NewType == ObjectType.HatCrown || NewType == ObjectType.HatMortarboard || NewType == ObjectType.HatSouwester || NewType == ObjectType.HatTree || NewType == ObjectType.HatMadHatter || NewType == ObjectType.HatCloche || NewType == ObjectType.HatAcorn || NewType == ObjectType.HatSanta || NewType == ObjectType.HatWally || NewType == ObjectType.HatParty || NewType == ObjectType.HatViking || NewType == ObjectType.HatBox || NewType == ObjectType.HatTrain || NewType == ObjectType.HatSailor || NewType == ObjectType.HatMiner || NewType == ObjectType.HatFox || NewType == ObjectType.HatDinosaur || NewType == ObjectType.HatAntlers || NewType == ObjectType.HatBunny || NewType == ObjectType.HatDuck || NewType == ObjectType.HatFrog || NewType == ObjectType.HatPanda || NewType == ObjectType.HatPenguin || NewType == ObjectType.HatBearskin || NewType == ObjectType.HatCaptain || NewType == ObjectType.HatCowboy || NewType == ObjectType.HatBoater || NewType == ObjectType.HatCatintheHat || NewType == ObjectType.HatDrumMajor || NewType == ObjectType.HatGat || NewType == ObjectType.HatFedora || NewType == ObjectType.HatSombrero || NewType == ObjectType.HatSmurf || NewType == ObjectType.HatTrafficCone || NewType == ObjectType.HatWig02 || ModManager.Instance.ModHatClass.IsItCustomType(NewType))
		{
			return true;
		}
		return false;
	}

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("Hat", m_TypeIdentifier);
	}
}
