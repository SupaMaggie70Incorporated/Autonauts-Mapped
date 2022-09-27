using System.Collections.Generic;

public class LinkedSystemTrack : LinkedSystem
{
	public List<Building> m_Stops;

	public List<TrainTrackPoints> m_Points;

	private List<Minecart> m_Minecarts;

	public LinkedSystemTrack()
		: base(SystemType.Track)
	{
		m_Minecarts = new List<Minecart>();
		m_Stops = new List<Building>();
		m_Points = new List<TrainTrackPoints>();
	}

	public bool GetIsEnoughTrack()
	{
		if (m_Buildings.Count < 3)
		{
			return false;
		}
		return true;
	}

	public bool GetHasAnyStops()
	{
		if (m_Stops.Count > 0)
		{
			return true;
		}
		return false;
	}

	public int GetNumMinecarts()
	{
		return m_Minecarts.Count;
	}

	public bool GetHasOneMinecart()
	{
		if (m_Minecarts.Count == 1)
		{
			return true;
		}
		return false;
	}

	public bool GetAllGood()
	{
		if (!GetIsEnoughTrack() || !GetHasAnyStops() || GetNumMinecarts() != 1)
		{
			return false;
		}
		return true;
	}

	public override void AddBuilding(Building NewBuilding, int Value = 0)
	{
		base.AddBuilding(NewBuilding, Value);
		UpdateCounts();
	}

	public override void RemoveBuilding(Building NewBuilding)
	{
		base.RemoveBuilding(NewBuilding);
		UpdateCounts();
	}

	public bool GetCanDeleteTrack(TrainTrack NewTrack)
	{
		foreach (Minecart minecart in m_Minecarts)
		{
			if (minecart.OnTrack(NewTrack))
			{
				return false;
			}
		}
		return true;
	}

	public void ShowPlayerControls(bool Show)
	{
		foreach (TrainTrackStop stop in m_Stops)
		{
			stop.ShowPlayerStop(Show);
		}
		foreach (KeyValuePair<Building, int> building in m_Buildings)
		{
			TrainTrackPoints component = building.Key.GetComponent<TrainTrackPoints>();
			if ((bool)component)
			{
				component.ShowPlayerSwitch(Show);
			}
		}
	}

	public void UpdateCounts()
	{
		m_Stops.Clear();
		m_Minecarts.Clear();
		m_Points.Clear();
		foreach (KeyValuePair<Building, int> building in m_Buildings)
		{
			TrainTrack component = building.Key.GetComponent<TrainTrack>();
			if (component.m_TypeIdentifier == ObjectType.TrainTrack || component.m_TypeIdentifier == ObjectType.TrainTrackBridge)
			{
				Building stop = component.GetComponent<TrainTrackStraight>().GetStop();
				if ((bool)stop)
				{
					m_Stops.Add(stop);
				}
			}
			if (TrainTrackPoints.GetIsTypeTrainTrackPoints(component.m_TypeIdentifier))
			{
				m_Points.Add(component.GetComponent<TrainTrackPoints>());
			}
			foreach (TileCoord tile in component.m_Tiles)
			{
				foreach (TileCoordObject item in PlotManager.Instance.GetObjectsAtTile(tile))
				{
					if (Minecart.GetIsTypeMinecart(item.m_TypeIdentifier))
					{
						m_Minecarts.Add(item.GetComponent<Minecart>());
					}
				}
			}
		}
	}

	public override void Update()
	{
		base.Update();
	}

	public override bool CanAddBuilding(Building NewBuilding)
	{
		foreach (KeyValuePair<Building, int> building in m_Buildings)
		{
			TrainTrack component = building.Key.GetComponent<TrainTrack>();
			if (!(component.m_ConnectedUp == NewBuilding) && !(component.m_ConnectedDown == NewBuilding) && !(component.m_ConnectedLeft == NewBuilding) && !(component.m_ConnectedRight == NewBuilding))
			{
				continue;
			}
			if (component.m_TypeIdentifier == ObjectType.TrainTrack || component.m_TypeIdentifier == ObjectType.TrainTrackBridge)
			{
				if (component.m_ConnectedUp == NewBuilding || component.m_ConnectedDown == NewBuilding)
				{
					if (building.Value == 1 || building.Value == 2)
					{
						return true;
					}
				}
				else if (building.Value == 0 || building.Value == 2)
				{
					return true;
				}
				continue;
			}
			return true;
		}
		return false;
	}

	public override void EndEditMode()
	{
		foreach (KeyValuePair<Building, int> building in m_Buildings)
		{
			TrainTrack component = building.Key.GetComponent<TrainTrack>();
			if ((bool)component)
			{
				component.RefreshBuffers(Force: true);
			}
		}
	}
}
