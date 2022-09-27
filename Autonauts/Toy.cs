using UnityEngine;

public class Toy : Holdable
{
	public static bool GetIsTypeToy(ObjectType NewType)
	{
		if (NewType == ObjectType.Doll || NewType == ObjectType.JackInTheBox || NewType == ObjectType.DollHouse || ModManager.Instance.ModToyClass.IsItCustomType(NewType) || NewType == ObjectType.Spaceship || NewType == ObjectType.ToyTrain || NewType == ObjectType.ToyHorse || NewType == ObjectType.ToyHorseCart || NewType == ObjectType.ToyHorseCarriage)
		{
			return true;
		}
		return false;
	}

	public static bool NeedsHorsePlacement(ObjectType NewType)
	{
		if (NewType == ObjectType.ToyHorseCart || NewType == ObjectType.ToyHorseCarriage || NewType == ObjectType.DollHouse || NewType == ObjectType.Spaceship || NewType == ObjectType.ToyTrain)
		{
			return true;
		}
		return false;
	}

	public void ReadyHold(Transform ParentTransform)
	{
		base.transform.SetParent(ParentTransform);
		base.transform.localPosition = new Vector3(0.434f, 0f, -0.813f);
		base.transform.localScale = new Vector3(1f, 1f, 1f);
		base.transform.SetParent(MapManager.Instance.m_ObjectsRootTransform);
	}

	public void Hold(Transform NewParent)
	{
		base.transform.SetParent(NewParent);
		base.transform.localPosition = new Vector3(0.434f, 0f, -0.813f);
		if (m_TypeIdentifier == ObjectType.DollHouse)
		{
			base.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		}
		else
		{
			base.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
		}
		base.transform.localScale = new Vector3(1f, 1f, 1f);
	}

	protected override void ActionDropped(Actionable PreviousHolder, TileCoord DropLocation)
	{
		base.ActionDropped(PreviousHolder, DropLocation);
		UpdateTierScale();
	}
}
