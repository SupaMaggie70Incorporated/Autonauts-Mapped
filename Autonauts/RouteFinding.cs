using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class RouteFinding : MonoBehaviour
{
	private static AstarPath m_AStar;

	private static GridGraph[] m_Graph;

	private static List<TileCoord> m_Path;

	private static TileCoord m_End;

	private static bool m_Scan;

	public static void InitTiles()
	{
		m_AStar = Object.Instantiate((GameObject)Resources.Load("Prefabs/RouteFinding", typeof(GameObject)), new Vector3(0f, 0f, 0f), Quaternion.identity, null).GetComponent<AstarPath>();
		NavGraph[] graphs = m_AStar.data.graphs;
		m_Graph = new GridGraph[4];
		for (int i = 0; i < 4; i++)
		{
			m_Graph[i] = (GridGraph)graphs[i];
			m_Graph[i].SetDimensions(TileManager.Instance.m_TilesWide, TileManager.Instance.m_TilesHigh, Tile.m_Size);
			float num = (float)TileManager.Instance.m_TilesWide * Tile.m_Size;
			float num2 = (float)TileManager.Instance.m_TilesHigh * Tile.m_Size;
			m_Graph[i].center = new Vector3(num / 2f, 0f, (0f - num2) / 2f);
		}
		m_AStar.Scan();
	}

	public static void ShutDown()
	{
		if ((bool)m_AStar)
		{
			Object.DestroyImmediate(m_AStar.gameObject);
		}
	}

	public static void UpdateTileWalk(int x, int y)
	{
		bool WorkerWalk = true;
		bool AnimalWalk = true;
		bool BoatWalk = true;
		bool PlayerWalk = true;
		float WalkCost = 0f;
		TileManager.Instance.GetTileWalkable(new TileCoord(x, y), out WorkerWalk, out AnimalWalk, out BoatWalk, out PlayerWalk, TestFarmer: true, out WalkCost);
		WalkCost /= 0.2f;
		WalkCost *= 3000f;
		int num = (TileManager.Instance.m_TilesHigh - 1 - y) * TileManager.Instance.m_TilesWide + x;
		m_Graph[0].nodes[num].Penalty = (uint)WalkCost;
		m_Graph[0].nodes[num].Walkable = WorkerWalk;
		m_Graph[1].nodes[num].Walkable = AnimalWalk;
		m_Graph[2].nodes[num].Walkable = BoatWalk;
		m_Graph[3].nodes[num].Walkable = PlayerWalk;
		m_Scan = true;
	}

	public static void UpdateAllTiles()
	{
	}

	private static void OnPathComplete(Path p)
	{
		m_Path = new List<TileCoord>();
		if (p == null || p.path.Count <= 1)
		{
			return;
		}
		GraphNode graphNode = p.path[p.path.Count - 1];
		if (new TileCoord(((GridNode)graphNode).XCoordinateInGrid, TileManager.Instance.m_TilesHigh - 1 - ((GridNode)graphNode).ZCoordinateInGrid) == m_End)
		{
			for (int i = 1; i < p.path.Count; i++)
			{
				graphNode = p.path[i];
				m_Path.Add(new TileCoord(((GridNode)graphNode).XCoordinateInGrid, TileManager.Instance.m_TilesHigh - 1 - ((GridNode)graphNode).ZCoordinateInGrid));
			}
		}
	}

	private static int GetShift(Actionable Mover)
	{
		int result = 0;
		if (Mover.m_TypeIdentifier == ObjectType.Worker || Mover.m_TypeIdentifier == ObjectType.TutorBot)
		{
			result = 0;
		}
		else if ((bool)Mover.GetComponent<Animal>())
		{
			result = 1;
		}
		else if ((bool)Mover.GetComponent<Canoe>())
		{
			result = 2;
		}
		else if (Mover.m_TypeIdentifier == ObjectType.FarmerPlayer)
		{
			result = 3;
		}
		else if (Vehicle.GetIsTypeVehicle(Mover.m_TypeIdentifier))
		{
			Actionable engager = Mover.GetComponent<Vehicle>().m_Engager;
			if ((bool)engager && engager.m_TypeIdentifier == ObjectType.FarmerPlayer)
			{
				result = 3;
			}
		}
		return result;
	}

	public static List<TileCoord> FindSolidRoute(TileCoord Start, TileCoord End, Actionable Mover, bool Synchronous = false, int Range = 0)
	{
		List<TileCoord> result = new List<TileCoord>();
		m_End = End;
		if (Mover.m_TypeIdentifier == ObjectType.FarmerPlayer || Mover.m_TypeIdentifier == ObjectType.TutorBot || Mover.m_TypeIdentifier == ObjectType.Worker || (bool)Mover.GetComponent<Animal>() || (bool)Mover.GetComponent<Vehicle>() || (bool)Mover.GetComponent<GoToMod>())
		{
			int shift = GetShift(Mover);
			OnPathDelegate onPathDelegate = null;
			onPathDelegate = ((!Synchronous) ? new OnPathDelegate(Mover.GetComponent<GoTo>().OnPathComplete) : new OnPathDelegate(OnPathComplete));
			ABPath aBPath = ABPath.Construct(Start.ToWorldPositionTileCentered(), End.ToWorldPositionTileCentered(), onPathDelegate);
			if (Range != 0)
			{
				aBPath = XPath.Construct(Start.ToWorldPositionTileCentered(), End.ToWorldPositionTileCentered(), onPathDelegate);
				((XPath)aBPath).endingCondition = new EndingConditionProximity(aBPath, (float)Range * Tile.m_Size);
			}
			NNInfoInternal nearest = m_Graph[shift].GetNearest(Start.ToWorldPositionTileCentered());
			NNInfoInternal nearest2 = m_Graph[shift].GetNearest(End.ToWorldPositionTileCentered());
			if (nearest.node.Area != nearest2.node.Area)
			{
				onPathDelegate(null);
				return result;
			}
			aBPath.nnConstraint.graphMask = 1 << shift;
			Mover.GetComponent<Seeker>().StartPath(aBPath);
			if (Synchronous)
			{
				aBPath.BlockUntilCalculated();
			}
		}
		else
		{
			Debug.Log("Who the what now?");
		}
		if (Synchronous)
		{
			return m_Path;
		}
		return result;
	}

	public static List<TileCoord> FindNearestRoute(TileCoord Start, List<TileCoord> Ends, Actionable Mover, bool Synchronous = false)
	{
		List<TileCoord> result = new List<TileCoord>();
		if (Mover.m_TypeIdentifier == ObjectType.FarmerPlayer || Mover.m_TypeIdentifier == ObjectType.TutorBot || Mover.m_TypeIdentifier == ObjectType.Worker || (bool)Mover.GetComponent<Animal>() || (bool)Mover.GetComponent<Vehicle>() || (bool)Mover.GetComponent<GoToMod>())
		{
			OnPathDelegate onPathDelegate = null;
			onPathDelegate = ((!Synchronous) ? new OnPathDelegate(Mover.GetComponent<GoTo>().OnPathCompleteFind) : new OnPathDelegate(OnPathComplete));
			Vector3[] array = new Vector3[Ends.Count];
			int num = 0;
			foreach (TileCoord End in Ends)
			{
				array[num] = End.ToWorldPositionTileCentered();
				num++;
			}
			MultiTargetPath multiTargetPath = MultiTargetPath.Construct(Start.ToWorldPositionTileCentered(), array, null);
			int shift = GetShift(Mover);
			multiTargetPath.nnConstraint.graphMask = 1 << shift;
			Mover.GetComponent<Seeker>().StartPath(multiTargetPath, onPathDelegate);
			if (Synchronous)
			{
				multiTargetPath.BlockUntilCalculated();
			}
		}
		else
		{
			Debug.Log("Who the what now?");
		}
		if (Synchronous)
		{
			return m_Path;
		}
		return result;
	}

	public static List<TileCoord> FindNearestRoute(TileCoord Start, List<TileCoordObject> Ends, Actionable Mover, bool Synchronous = false)
	{
		List<TileCoord> result = new List<TileCoord>();
		if (Mover.m_TypeIdentifier == ObjectType.FarmerPlayer || Mover.m_TypeIdentifier == ObjectType.TutorBot || Mover.m_TypeIdentifier == ObjectType.Worker || (bool)Mover.GetComponent<Animal>() || (bool)Mover.GetComponent<Vehicle>() || (bool)Mover.GetComponent<GoToMod>())
		{
			OnPathDelegate onPathDelegate = null;
			onPathDelegate = ((!Synchronous) ? new OnPathDelegate(Mover.GetComponent<GoTo>().OnPathCompleteFind) : new OnPathDelegate(OnPathComplete));
			Vector3[] array = new Vector3[Ends.Count];
			int num = 0;
			foreach (TileCoordObject End in Ends)
			{
				if (ObjectTypeList.Instance.GetIsBuilding(End.m_TypeIdentifier))
				{
					array[num] = End.GetComponent<Building>().GetAccessPosition().ToWorldPositionTileCenteredWithoutHeight();
				}
				else
				{
					array[num] = End.m_TileCoord.ToWorldPositionTileCenteredWithoutHeight();
				}
				num++;
			}
			MultiTargetPath multiTargetPath = MultiTargetPath.Construct(Start.ToWorldPositionTileCentered(), array, null);
			int shift = GetShift(Mover);
			multiTargetPath.nnConstraint.graphMask = 1 << shift;
			Mover.GetComponent<Seeker>().StartPath(multiTargetPath, onPathDelegate);
			if (Synchronous)
			{
				multiTargetPath.BlockUntilCalculated();
			}
		}
		else
		{
			Debug.Log("Who the what now?");
		}
		if (Synchronous)
		{
			return m_Path;
		}
		return result;
	}

	private void Update()
	{
		if (m_Scan)
		{
			m_AStar.Scan();
			m_Scan = false;
		}
	}
}
