using UnityEngine;

public class TrainTrackBridge : TrainTrackStraight
{
	private bool m_End;

	public override void RegisterClass()
	{
		base.RegisterClass();
		ModelManager.Instance.AddModel("Models/Buildings/Floors/TrainTrackBridgeEnd", ObjectType.TrainTrackBridge);
		ModelManager.Instance.AddModel("Models/Buildings/Floors/TrainTrackBridgeCross", ObjectType.TrainTrackBridge);
		ModelManager.Instance.AddModel("Models/Buildings/Floors/TrainTrackBridge", ObjectType.TrainTrackBridge);
	}

	public override void Restart()
	{
		base.Restart();
		TrainTrackStraight.m_CrossModelName = "TrainTrackBridgeCross";
		TrainTrackStraight.m_StraightModelName = "TrainTrackBridge";
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

	public override float GetHeight()
	{
		float num = 0f;
		Tile.TileType tileType = TileManager.Instance.GetTileType(m_TileCoord);
		if (m_End && !TileHelpers.GetTileWater(tileType))
		{
			return 0.42f;
		}
		return 1.24f;
	}

	private bool CheckForEnd(TrainTrack Connection)
	{
		if (Connection == null || Connection.m_TypeIdentifier != ObjectType.TrainTrackBridge)
		{
			return true;
		}
		return false;
	}

	protected override void Refresh()
	{
		base.Refresh();
		m_End = false;
		if (m_Cross)
		{
			return;
		}
		if (GetVertical())
		{
			if (CheckForEnd(m_ConnectedUp))
			{
				m_End = true;
				SetRotation(3);
			}
			else if (CheckForEnd(m_ConnectedDown))
			{
				m_End = true;
				SetRotation(1);
			}
		}
		else if (CheckForEnd(m_ConnectedLeft))
		{
			m_End = true;
			SetRotation(2);
		}
		else if (CheckForEnd(m_ConnectedRight))
		{
			m_End = true;
			SetRotation(0);
		}
		Tile.TileType tileType = TileManager.Instance.GetTileType(m_TileCoord);
		if (m_End && !TileHelpers.GetTileWater(tileType))
		{
			LoadNewModel("Models/Buildings/Floors/TrainTrackBridgeEnd");
		}
		else
		{
			LoadNewModel("Models/Buildings/Floors/TrainTrackBridge");
		}
	}
}
