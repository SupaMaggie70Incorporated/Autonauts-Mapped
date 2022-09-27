using UnityEngine;

public class LogCabin : Housing
{
	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(0, 0), new TileCoord(1, 0), new TileCoord(0, 1));
	}

	public override void PostCreate()
	{
		if (SaveLoadManager.m_Video)
		{
			Vector3 localPosition = m_ModelRoot.transform.localPosition;
			localPosition.z += Tile.m_Size - 0.5f;
			m_ModelRoot.transform.localPosition = localPosition;
		}
		base.PostCreate();
	}
}
