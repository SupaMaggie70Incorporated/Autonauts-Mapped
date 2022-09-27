using System;
using UnityEngine;

public class FarmerStateCreated : FarmerStateBase
{
	public override void UpdateState()
	{
		base.UpdateState();
		float num = 0.25f;
		float num2 = m_Farmer.m_StateTimer / num;
		float x = 0f;
		float y = Mathf.Sin(num2 * (float)Math.PI) * 6f;
		float z = (0f - num2) * Tile.m_Size;
		m_Farmer.transform.position = m_Farmer.m_FinalPosition + new Vector3(x, y, z);
		if (m_Farmer.m_StateTimer >= num)
		{
			DoEndAction();
			m_Farmer.WarpTo(m_Farmer.m_FinalPosition.x, m_Farmer.m_FinalPosition.z - Tile.m_Size, 90f);
		}
	}
}
