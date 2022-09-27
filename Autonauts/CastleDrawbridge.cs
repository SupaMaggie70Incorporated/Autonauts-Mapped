using UnityEngine;

public class CastleDrawbridge : Floor
{
	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(0, -2), new TileCoord(2, 0), new TileCoord(0, 0));
		HideAccessModel();
	}

	public override object GetActionInfo(GetActionInfo Info)
	{
		GetAction action = Info.m_Action;
		if ((uint)(action - 3) <= 1u)
		{
			foreach (TileCoord tile2 in m_Tiles)
			{
				Tile tile = TileManager.Instance.GetTile(tile2);
				if ((bool)tile.m_BuildingFootprint)
				{
					return false;
				}
				if ((bool)tile.m_Farmer)
				{
					return false;
				}
				if ((bool)PlotManager.Instance.GetSelectableObjectAtTile(m_TileCoord))
				{
					return false;
				}
			}
		}
		return base.GetActionInfo(Info);
	}

	protected override void TileCoordChanged(TileCoord Position)
	{
		base.TileCoordChanged(Position);
		Vector3 position = base.transform.position;
		if (position.y < 0f)
		{
			position.y = 0f;
			base.transform.position = position;
		}
	}

	public override bool CanBuildOn()
	{
		return false;
	}
}
