using UnityEngine;

public class MyCanvas : Holdable
{
	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		if ((Holder.m_TypeIdentifier == ObjectType.FarmerPlayer || Holder.m_TypeIdentifier == ObjectType.Worker) && Holder.GetComponent<Farmer>().m_FarmerCarry.m_CarryObject.Contains(this))
		{
			int num = Holder.GetComponent<Farmer>().m_FarmerCarry.m_CarryObject.IndexOf(this);
			float height = ObjectTypeList.Instance.GetHeight(m_TypeIdentifier);
			base.transform.localPosition = new Vector3(0f, 1f, -0.5f - (float)num * height);
			base.transform.localRotation = Quaternion.Euler(0f, 90f, 90f);
		}
	}

	protected override void ActionDropped(Actionable PreviousHolder, TileCoord DropLocation)
	{
		base.ActionDropped(PreviousHolder, DropLocation);
		base.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
	}
}
