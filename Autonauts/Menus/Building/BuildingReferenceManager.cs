using System.Collections.Generic;
using UnityEngine;

public class BuildingReferenceManager : MonoBehaviour
{
	public static BuildingReferenceManager Instance;

	private Building m_TargetBuilding;

	private List<BuildingReferenceArc> m_Arcs;

	private void Awake()
	{
		Instance = this;
		m_TargetBuilding = null;
		m_Arcs = new List<BuildingReferenceArc>();
	}

	private void OnDestroy()
	{
		Clear();
	}

	public void RemoveWorker(Worker NewBot)
	{
		foreach (BuildingReferenceArc arc in m_Arcs)
		{
			if (arc.m_Bot == NewBot)
			{
				m_Arcs.Remove(arc);
				DestroyArc(arc);
				break;
			}
		}
	}

	public bool ShowBuildingReferences(Building NewBuilding)
	{
		m_TargetBuilding = NewBuilding;
		List<Worker> referencingWorkers = NewBuilding.GetReferencingWorkers();
		List<BuildingReferenceArc> list = new List<BuildingReferenceArc>();
		foreach (BuildingReferenceArc arc in m_Arcs)
		{
			list.Add(arc);
		}
		foreach (Worker item2 in referencingWorkers)
		{
			bool flag = false;
			foreach (BuildingReferenceArc arc2 in m_Arcs)
			{
				if (arc2.m_Bot == item2)
				{
					flag = true;
					list.Remove(arc2);
					break;
				}
			}
			if (!flag)
			{
				BuildingReferenceArc item = CreateArc(item2);
				m_Arcs.Add(item);
			}
		}
		foreach (BuildingReferenceArc item3 in list)
		{
			m_Arcs.Remove(item3);
			DestroyArc(item3);
		}
		return referencingWorkers.Count > 0;
	}

	public void Clear()
	{
		m_TargetBuilding = null;
		foreach (BuildingReferenceArc arc in m_Arcs)
		{
			DestroyArc(arc);
		}
		m_Arcs.Clear();
	}

	private BuildingReferenceArc CreateArc(Worker NewBot)
	{
		Arc component = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Arc, default(Vector3), base.transform.localRotation).GetComponent<Arc>();
		component.transform.position = m_TargetBuilding.m_TileCoord.ToWorldPositionTileCentered();
		return new BuildingReferenceArc(m_TargetBuilding, NewBot, component);
	}

	private void DestroyArc(BuildingReferenceArc NewArc)
	{
		NewArc.m_Arc.StopUsing();
	}

	private void Update()
	{
		foreach (BuildingReferenceArc arc in m_Arcs)
		{
			arc.Update();
		}
	}
}
