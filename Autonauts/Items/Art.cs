using UnityEngine;

public class Art : Holdable
{
	public static bool GetIsTypeArt(ObjectType NewType)
	{
		if (NewType == ObjectType.ArtPortrait || NewType == ObjectType.ArtStillLife || NewType == ObjectType.ArtAbstract)
		{
			return true;
		}
		return false;
	}

	protected override void ActionDropped(Actionable PreviousHolder, TileCoord DropLocation)
	{
		base.ActionDropped(PreviousHolder, DropLocation);
		base.gameObject.SetActive(value: true);
		UpdateTierScale();
	}

	public void ReadyHold(Transform ParentTransform)
	{
		base.transform.SetParent(ParentTransform);
		base.transform.localPosition = new Vector3(0f, 0f, 0f);
		base.transform.localScale = new Vector3(1f, 1f, 1f);
		base.transform.SetParent(MapManager.Instance.m_ObjectsRootTransform);
	}

	public void Hold(Transform NewParent)
	{
		base.transform.SetParent(NewParent);
		base.transform.localPosition = default(Vector3);
		base.transform.localRotation = Quaternion.Euler(-90f, 0f, 180f);
		base.transform.localScale = new Vector3(1f, 1f, 1f);
		base.gameObject.SetActive(value: false);
	}
}
