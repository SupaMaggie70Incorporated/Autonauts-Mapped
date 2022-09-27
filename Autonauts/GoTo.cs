using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class GoTo : TileMover
{
	[HideInInspector]
	public TileCoord m_GoToTilePosition;

	[HideInInspector]
	public bool m_GoToLessOne;

	[HideInInspector]
	public int m_GoToRange;

	[HideInInspector]
	public Actionable m_GoToTargetObject;

	[HideInInspector]
	public Vector3 m_FinalPosition;

	[HideInInspector]
	public Quaternion m_FinalRotation;

	[HideInInspector]
	public List<TileCoord> m_Path;

	[HideInInspector]
	public bool m_FindFinished;

	[HideInInspector]
	public List<TileCoord> m_FindDestinations;

	[HideInInspector]
	public List<TileCoord> m_FoundDestinations;

	[HideInInspector]
	public List<TileCoordObject> m_FindObjects;

	[HideInInspector]
	public List<TileCoordObject> m_FoundObjects;

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("GoTo", m_TypeIdentifier);
	}

	protected new void Awake()
	{
		base.Awake();
		m_FinalPosition = base.transform.position;
		m_FinalRotation = base.transform.rotation;
		m_Path = new List<TileCoord>();
		base.gameObject.AddComponent(typeof(Seeker));
	}

	public void WarpTo(float x, float z, float Rotation)
	{
		base.transform.position = new Vector3(x, 0f, z);
		base.transform.rotation = Quaternion.Euler(0f, Rotation, 0f);
		m_FinalPosition = base.transform.position;
		TileCoord tilePosition = default(TileCoord);
		tilePosition.FromWorldPosition(m_FinalPosition);
		SetTilePosition(tilePosition);
		m_GoToTilePosition = m_TileCoord;
		m_FinalRotation = base.transform.rotation;
	}

	private bool GetIsObstructionAtTile(TileCoord NewCoord)
	{
		bool result = false;
		TileManager.Instance.GetTileWalkable(NewCoord, out var WorkerWalk, out var AnimalWalk, out var BoatWalk, out var PlayerWalk, TestFarmer: true, out var _);
		if (m_TypeIdentifier == ObjectType.FarmerPlayer)
		{
			result = !PlayerWalk;
		}
		else if (m_TypeIdentifier == ObjectType.Worker)
		{
			result = !WorkerWalk;
		}
		else if (Animal.GetIsTypeAnimal(m_TypeIdentifier))
		{
			result = !AnimalWalk;
		}
		else if (m_TypeIdentifier == ObjectType.Canoe)
		{
			result = !BoatWalk;
		}
		else if (Vehicle.GetIsTypeVehicle(m_TypeIdentifier))
		{
			result = ((!GetComponent<Vehicle>().m_Engager || GetComponent<Vehicle>().m_Engager.m_TypeIdentifier != ObjectType.FarmerPlayer) ? (!WorkerWalk) : (!PlayerWalk));
		}
		return result;
	}

	public virtual void NextGoTo()
	{
		if (m_TileCoord == m_GoToTilePosition)
		{
			EndGoTo();
			return;
		}
		if (m_Path == null || m_Path.Count == 0)
		{
			EndGoTo();
			return;
		}
		TileCoord tileCoord = m_Path[0] - m_TileCoord;
		TileCoord newCoord = m_TileCoord + tileCoord;
		if (GetIsObstructionAtTile(newCoord) && !GetIsObstructionAtTile(m_TileCoord))
		{
			ObstructionEncountered();
			return;
		}
		MoveDirection(tileCoord);
		if (m_Move)
		{
			m_Path.RemoveAt(0);
		}
	}

	public virtual void ObstructionEncountered()
	{
	}

	public void OnPathComplete(Path p)
	{
		m_Path = new List<TileCoord>();
		if (p != null && p.path.Count > 1)
		{
			GraphNode graphNode = p.path[p.path.Count - 1];
			float num = (new TileCoord(((GridNode)graphNode).XCoordinateInGrid, TileManager.Instance.m_TilesHigh - 1 - ((GridNode)graphNode).ZCoordinateInGrid) - m_GoToTilePosition).Magnitude();
			float num2 = 5f;
			if (m_GoToRange != 0)
			{
				num2 = (float)m_GoToRange * Tile.m_Size;
			}
			if (num < num2)
			{
				for (int i = 1; i < p.path.Count; i++)
				{
					graphNode = p.path[i];
					m_Path.Add(new TileCoord(((GridNode)graphNode).XCoordinateInGrid, TileManager.Instance.m_TilesHigh - 1 - ((GridNode)graphNode).ZCoordinateInGrid));
				}
			}
		}
		StartGoTo(m_GoToTilePosition, m_GoToTargetObject, m_GoToLessOne, m_GoToRange);
	}

	public virtual bool RequestGoTo(TileCoord Destination, Actionable TargetObject = null, bool LessOne = false, int Range = 0)
	{
		m_GoToTilePosition = Destination;
		m_GoToTargetObject = TargetObject;
		m_GoToLessOne = LessOne;
		m_GoToRange = Range;
		m_Path = RouteFinding.FindSolidRoute(m_TileCoord, m_GoToTilePosition, this, Synchronous: false, m_GoToRange);
		return true;
	}

	public virtual bool StartGoTo(TileCoord Destination, Actionable TargetObject = null, bool LessOne = false, int Range = 0)
	{
		m_GoToTilePosition = Destination;
		m_GoToTargetObject = TargetObject;
		if (Destination == m_TileCoord)
		{
			EndGoTo();
			return false;
		}
		if (m_Path.Count == 0)
		{
			if (Range != 0 && (m_GoToTilePosition.ToWorldPosition() - m_TileCoord.ToWorldPosition()).magnitude <= (float)Range * Tile.m_Size)
			{
				EndGoTo();
			}
			m_Path = null;
			return false;
		}
		if (LessOne && m_Path.Count > 0)
		{
			m_Path.RemoveAt(m_Path.Count - 1);
		}
		NextGoTo();
		if (!m_Move)
		{
			return false;
		}
		return true;
	}

	public virtual void StopGoTo()
	{
	}

	public virtual void EndGoTo()
	{
	}

	protected override void MoveEnded()
	{
		m_FinalPosition = m_GoToTilePosition.ToWorldPositionTileCentered(m_FloatsInWater);
		NextGoTo();
	}

	public void OnPathCompleteFind(Path p)
	{
		m_Path = new List<TileCoord>();
		if (!(p is MultiTargetPath multiTargetPath))
		{
			ErrorMessage.LogError("The Path was no MultiTargetPath");
		}
		else
		{
			if (multiTargetPath.nodePaths == null)
			{
				return;
			}
			int num = 1000000;
			int num2 = -1;
			for (int i = 0; i < multiTargetPath.nodePaths.Length; i++)
			{
				List<GraphNode> list = multiTargetPath.nodePaths[i];
				if (list == null || list.Count <= 0)
				{
					continue;
				}
				GraphNode graphNode = list[list.Count - 1];
				TileCoord tileCoord = new TileCoord(((GridNode)graphNode).XCoordinateInGrid, TileManager.Instance.m_TilesHigh - 1 - ((GridNode)graphNode).ZCoordinateInGrid);
				if (m_FindObjects != null)
				{
					foreach (TileCoordObject findObject in m_FindObjects)
					{
						if (!findObject)
						{
							continue;
						}
						TileCoord tileCoord2 = findObject.m_TileCoord;
						if (ObjectTypeList.Instance.GetIsBuilding(findObject.m_TypeIdentifier))
						{
							tileCoord2 = findObject.GetComponent<Building>().GetAccessPosition();
						}
						TileCoord tileCoord3 = tileCoord2 - tileCoord;
						if (findObject.m_TypeIdentifier == ObjectType.ConverterFoundation && (tileCoord3.x < -1 || tileCoord3.x > 1 || tileCoord3.y < -1 || tileCoord3.y > 1) && findObject.GetComponent<ConverterFoundation>().GetAdjacentTiles().Contains(tileCoord))
						{
							tileCoord3 = default(TileCoord);
						}
						if (tileCoord3.x >= -1 && tileCoord3.x <= 1 && tileCoord3.y >= -1 && tileCoord3.y <= 1)
						{
							if (num > list.Count)
							{
								num = list.Count;
								num2 = m_FoundObjects.Count;
							}
							m_FoundObjects.Add(findObject);
							m_FindObjects.Remove(findObject);
							break;
						}
					}
				}
				else
				{
					if (num > list.Count)
					{
						num = list.Count;
						num2 = m_FoundDestinations.Count;
					}
					m_FoundDestinations.Add(tileCoord);
				}
			}
			if (num2 != -1)
			{
				if (m_FindObjects != null)
				{
					TileCoordObject item = m_FoundObjects[num2];
					m_FoundObjects.RemoveAt(num2);
					m_FoundObjects.Insert(0, item);
				}
				else
				{
					TileCoord item2 = m_FoundDestinations[num2];
					m_FoundDestinations.RemoveAt(num2);
					m_FoundDestinations.Insert(0, item2);
				}
			}
			m_FindFinished = true;
		}
	}

	public virtual bool RequestFind(List<TileCoord> Destinations)
	{
		m_FindDestinations = Destinations;
		m_FoundDestinations = new List<TileCoord>();
		m_FindObjects = null;
		m_FoundObjects = null;
		m_FindFinished = false;
		m_Path = RouteFinding.FindNearestRoute(m_TileCoord, Destinations, this);
		return true;
	}

	public virtual bool RequestFind(List<TileCoordObject> NewObjects)
	{
		m_FindDestinations = null;
		m_FoundDestinations = null;
		m_FindObjects = NewObjects;
		m_FoundObjects = new List<TileCoordObject>();
		m_FindFinished = false;
		m_Path = RouteFinding.FindNearestRoute(m_TileCoord, NewObjects, this);
		return true;
	}

	public void RemoveBaggedObjects()
	{
		List<TileCoordObject> list = new List<TileCoordObject>();
		foreach (TileCoordObject foundObject in m_FoundObjects)
		{
			if (BaggedManager.Instance.IsObjectBagged(foundObject))
			{
				list.Add(foundObject);
			}
		}
		foreach (TileCoordObject item in list)
		{
			m_FoundObjects.Remove(item);
		}
	}

	public void RemoveBaggedTiles()
	{
		List<TileCoord> list = new List<TileCoord>();
		foreach (TileCoord foundDestination in m_FoundDestinations)
		{
			if (BaggedManager.Instance.IsTileBagged(foundDestination))
			{
				list.Add(foundDestination);
			}
		}
		foreach (TileCoord item in list)
		{
			m_FoundDestinations.Remove(item);
		}
	}

	public void RemoveBagged()
	{
		if (m_FoundObjects != null)
		{
			RemoveBaggedObjects();
		}
		if (m_FoundDestinations != null)
		{
			RemoveBaggedTiles();
		}
	}
}
