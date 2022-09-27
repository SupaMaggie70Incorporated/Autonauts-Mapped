using UnityEngine;

public class MechanicalRod : BaseClass
{
	public int m_Length;

	public Building m_Parent;

	public Building m_ConnectedTo;

	public override void StopUsing(bool AndDestroy = true)
	{
		CleanHighlight();
		base.StopUsing(AndDestroy);
		m_ConnectedTo.GetComponent<BeltLinkage>().m_RodConnectTo = null;
	}

	public void SetLength(int TileLength)
	{
		m_Length = TileLength;
		base.transform.localScale = new Vector3(1f, 1f, TileLength);
	}

	public void ConnectTo(Building StartBuilding, Building EndBuilding)
	{
		m_Parent = StartBuilding;
		m_ConnectedTo = EndBuilding;
		base.transform.SetParent(StartBuilding.GetComponent<BeltLinkage>().m_ConnectPoint);
		base.transform.localPosition = default(Vector3);
		m_ConnectedTo.GetComponent<BeltLinkage>().SetRodConnectedTo(StartBuilding.GetComponent<BeltLinkage>());
		int length = EndBuilding.m_TileCoord.y - StartBuilding.m_TileCoord.y;
		SetLength(length);
	}

	private void Update()
	{
		if (m_Parent.m_LinkedSystem != null)
		{
			float speed = ((LinkedSystemMechanical)m_Parent.m_LinkedSystem).GetSpeed();
			if (speed > 0f)
			{
				base.transform.localRotation = base.transform.localRotation * Quaternion.Euler(0f, 0f, -720f * TimeManager.Instance.m_NormalDelta * speed);
			}
		}
	}
}
