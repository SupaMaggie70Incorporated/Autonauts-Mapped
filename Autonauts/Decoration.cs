using UnityEngine;

public class Decoration : TileMover
{
	public static bool GetIsTypeDecoration(ObjectType NewType)
	{
		if (NewType == ObjectType.GnomeRaw || NewType == ObjectType.Gnome || ModManager.Instance.ModDecorativeClass.IsItCustomType(NewType) || NewType == ObjectType.Gnome2 || NewType == ObjectType.Gnome3 || NewType == ObjectType.Gnome4 || NewType == ObjectType.Gnome5 || NewType == ObjectType.Gnome6)
		{
			return true;
		}
		return false;
	}

	public override void Restart()
	{
		base.Restart();
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		base.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
	}
}
