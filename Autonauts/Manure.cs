using UnityEngine;

public class Manure : Holdable
{
	private Tile m_Tile;

	protected new void Awake()
	{
		m_Tile = null;
		base.Awake();
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		if (m_Tile != null && m_Tile.m_MiscObject == this)
		{
			m_Tile.m_MiscObject = null;
		}
		base.StopUsing(AndDestroy);
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		base.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		if (m_Tile != null && m_Tile.m_MiscObject == this)
		{
			m_Tile.m_MiscObject = null;
		}
	}

	protected override void ActionDropped(Actionable PreviousHolder, TileCoord DropLocation)
	{
		base.ActionDropped(PreviousHolder, DropLocation);
		m_Tile = TileManager.Instance.GetTile(DropLocation);
		m_Tile.m_MiscObject = this;
	}

	protected override void TileCoordChanged(TileCoord Position)
	{
		if (m_Tile != null && m_Tile.m_MiscObject == this)
		{
			m_Tile.m_MiscObject = null;
		}
		base.TileCoordChanged(Position);
		m_Tile = TileManager.Instance.GetTile(m_TileCoord);
		m_Tile.m_MiscObject = this;
	}
}
