using System.Collections.Generic;
using UnityEngine;

public class LinkedSystemManager : MonoBehaviour
{
	public static LinkedSystemManager Instance;

	private static bool m_Log;

	private static bool m_LogVerbose;

	private List<LinkedSystem> m_Systems;

	private List<Building> m_AddBuildings;

	private List<Building> m_RemoveBuildings;

	private List<Building> m_UpdateBuildings;

	private void Awake()
	{
		Instance = this;
		m_Systems = new List<LinkedSystem>();
		m_AddBuildings = new List<Building>();
		m_RemoveBuildings = new List<Building>();
		m_UpdateBuildings = new List<Building>();
	}

	public void AddBuilding(Building NewBuilding)
	{
		if (!m_AddBuildings.Contains(NewBuilding) && !m_RemoveBuildings.Contains(NewBuilding))
		{
			m_AddBuildings.Add(NewBuilding);
		}
	}

	public void RemoveBuilding(Building NewBuilding, bool Destroy)
	{
		if (Destroy)
		{
			if (m_UpdateBuildings.Contains(NewBuilding))
			{
				m_UpdateBuildings.Remove(NewBuilding);
			}
			if (m_AddBuildings.Contains(NewBuilding))
			{
				m_AddBuildings.Remove(NewBuilding);
			}
			if (m_RemoveBuildings.Contains(NewBuilding))
			{
				m_RemoveBuildings.Remove(NewBuilding);
			}
			DoRemoveBuilding(NewBuilding);
		}
		else
		{
			if (m_UpdateBuildings.Contains(NewBuilding))
			{
				m_UpdateBuildings.Remove(NewBuilding);
			}
			if (m_AddBuildings.Contains(NewBuilding))
			{
				m_AddBuildings.Remove(NewBuilding);
			}
			else if (!m_RemoveBuildings.Contains(NewBuilding))
			{
				m_RemoveBuildings.Add(NewBuilding);
			}
		}
	}

	public void UpdateBuilding(Building NewBuilding)
	{
		if (m_RemoveBuildings.Contains(NewBuilding))
		{
			m_RemoveBuildings.Remove(NewBuilding);
		}
		m_RemoveBuildings.Add(NewBuilding);
		if (m_AddBuildings.Contains(NewBuilding))
		{
			m_AddBuildings.Remove(NewBuilding);
		}
		m_AddBuildings.Add(NewBuilding);
	}

	private LinkedSystem.SystemType GetSystemType(ObjectType NewType)
	{
		if (LinkedSystemEngine.GetIsTypeLinkedSystemEngine(NewType) || LinkedSystemConverter.GetIsTypeLinkedSystemConverter(NewType) || NewType == ObjectType.BeltLinkage)
		{
			return LinkedSystem.SystemType.Mechanical;
		}
		if (TrainTrack.GetIsTypeTrainTrack(NewType))
		{
			return LinkedSystem.SystemType.Track;
		}
		return LinkedSystem.SystemType.Total;
	}

	private void MergeSystems(LinkedSystem First, LinkedSystem Other)
	{
		foreach (KeyValuePair<Building, int> building in Other.m_Buildings)
		{
			if (!First.GetContainsBuilding(building.Key))
			{
				First.AddBuilding(building.Key, building.Value);
			}
			else if ((building.Key.m_TypeIdentifier == ObjectType.TrainTrack || building.Key.m_TypeIdentifier == ObjectType.TrainTrackBridge) && building.Key.GetComponent<TrainTrackStraight>().m_Cross)
			{
				if (m_LogVerbose)
				{
					Debug.Log("Two become one");
				}
				First.m_Buildings[building.Key] = 2;
			}
		}
		m_Systems.Remove(Other);
	}

	private void CreateNewSystem(LinkedSystem.SystemType NewType, Building NewBuilding, int Value = 0)
	{
		LinkedSystem linkedSystem = null;
		switch (NewType)
		{
		case LinkedSystem.SystemType.Mechanical:
			linkedSystem = new LinkedSystemMechanical();
			break;
		case LinkedSystem.SystemType.Track:
			linkedSystem = new LinkedSystemTrack();
			break;
		}
		linkedSystem.AddBuilding(NewBuilding, Value);
		m_Systems.Add(linkedSystem);
	}

	private void DoAddBuilding(Building NewBuilding)
	{
		if (m_LogVerbose)
		{
			Debug.Log("DoAdd " + NewBuilding.m_UniqueID);
		}
		LinkedSystem.SystemType systemType = GetSystemType(NewBuilding.m_TypeIdentifier);
		if (systemType == LinkedSystem.SystemType.Total)
		{
			return;
		}
		List<LinkedSystem> list = new List<LinkedSystem>();
		foreach (LinkedSystem system in m_Systems)
		{
			if (system.m_Type == systemType && system.CanAddBuilding(NewBuilding))
			{
				list.Add(system);
			}
		}
		bool flag = true;
		int value = 0;
		if (NewBuilding.m_TypeIdentifier == ObjectType.TrainTrack || NewBuilding.m_TypeIdentifier == ObjectType.TrainTrackBridge)
		{
			TrainTrackStraight component = NewBuilding.GetComponent<TrainTrackStraight>();
			if (!component.m_Cross)
			{
				value = (component.GetVertical() ? 1 : 0);
			}
			else
			{
				flag = false;
				LinkedSystem linkedSystem = null;
				LinkedSystem linkedSystem2 = null;
				LinkedSystem linkedSystem3 = null;
				LinkedSystem linkedSystem4 = null;
				foreach (LinkedSystem item in list)
				{
					if ((bool)component.m_ConnectedLeft && item.m_Buildings.ContainsKey(component.m_ConnectedLeft))
					{
						linkedSystem = item;
					}
					if ((bool)component.m_ConnectedRight && item.m_Buildings.ContainsKey(component.m_ConnectedRight))
					{
						linkedSystem2 = item;
					}
				}
				if (linkedSystem != null)
				{
					if (!linkedSystem.m_Buildings.ContainsKey(NewBuilding))
					{
						linkedSystem.AddBuilding(NewBuilding);
					}
					if (linkedSystem2 != null && linkedSystem != linkedSystem2)
					{
						MergeSystems(linkedSystem, linkedSystem2);
						list.Remove(linkedSystem2);
					}
				}
				else if (linkedSystem2 != null)
				{
					if (!linkedSystem2.m_Buildings.ContainsKey(NewBuilding))
					{
						linkedSystem2.AddBuilding(NewBuilding);
					}
				}
				else
				{
					CreateNewSystem(systemType, NewBuilding);
				}
				foreach (LinkedSystem item2 in list)
				{
					if ((bool)component.m_ConnectedUp && item2.m_Buildings.ContainsKey(component.m_ConnectedUp))
					{
						linkedSystem3 = item2;
					}
					if ((bool)component.m_ConnectedDown && item2.m_Buildings.ContainsKey(component.m_ConnectedDown))
					{
						linkedSystem4 = item2;
					}
				}
				if (linkedSystem3 != null)
				{
					if (!linkedSystem3.m_Buildings.ContainsKey(NewBuilding))
					{
						linkedSystem3.AddBuilding(NewBuilding, 1);
					}
					if (linkedSystem4 != null && linkedSystem3 != linkedSystem4)
					{
						MergeSystems(linkedSystem3, linkedSystem4);
					}
				}
				else if (linkedSystem4 != null)
				{
					if (!linkedSystem4.m_Buildings.ContainsKey(NewBuilding))
					{
						linkedSystem4.AddBuilding(NewBuilding, 1);
					}
				}
				else
				{
					CreateNewSystem(systemType, NewBuilding, 1);
				}
			}
		}
		if (!flag)
		{
			return;
		}
		if (list.Count > 0)
		{
			LinkedSystem linkedSystem5 = list[0];
			linkedSystem5.AddBuilding(NewBuilding, value);
			list.Remove(linkedSystem5);
			{
				foreach (LinkedSystem item3 in list)
				{
					MergeSystems(linkedSystem5, item3);
				}
				return;
			}
		}
		CreateNewSystem(systemType, NewBuilding, value);
	}

	private void RemoveFromAllSystems(Building NewBuilding)
	{
	}

	private bool DoRemoveBuilding(Building NewBuilding)
	{
		if (m_LogVerbose)
		{
			Debug.Log("DoRemove " + NewBuilding.m_UniqueID);
		}
		List<Building> list = new List<Building>();
		List<LinkedSystem> list2 = new List<LinkedSystem>();
		foreach (LinkedSystem system in m_Systems)
		{
			if (!system.GetContainsBuilding(NewBuilding))
			{
				continue;
			}
			if (m_LogVerbose)
			{
				Debug.Log("System found");
			}
			system.RemoveBuilding(NewBuilding);
			foreach (KeyValuePair<Building, int> building in system.m_Buildings)
			{
				if (!list.Contains(building.Key))
				{
					list.Add(building.Key);
				}
				building.Key.SetLinkedSystem(null);
			}
			list2.Add(system);
		}
		foreach (Building item in list)
		{
			if (!TrainTrack.GetIsTypeTrainTrack(item.m_TypeIdentifier))
			{
				continue;
			}
			foreach (LinkedSystem system2 in m_Systems)
			{
				if (system2.m_Buildings.ContainsKey(item))
				{
					system2.m_Buildings.Remove(item);
					if (system2.m_Buildings.Count == 0)
					{
						list2.Add(system2);
					}
				}
			}
		}
		foreach (LinkedSystem item2 in list2)
		{
			m_Systems.Remove(item2);
		}
		foreach (Building item3 in list)
		{
			DoAddBuilding(item3);
		}
		if (m_LogVerbose)
		{
			Debug.Log("Removed. Systems left " + m_Systems.Count);
			foreach (LinkedSystem system3 in m_Systems)
			{
				Debug.Log("buildings = " + system3.m_Buildings.Count);
			}
		}
		return false;
	}

	private void DoUpdateBuilding(Building NewBuilding)
	{
		if (m_LogVerbose)
		{
			Debug.Log("DoUpdate " + NewBuilding.m_UniqueID);
		}
		DoRemoveBuilding(NewBuilding);
		DoAddBuilding(NewBuilding);
	}

	private void UpdateBufferedChanges()
	{
		if (m_Log)
		{
			Debug.Log("*** UpdateBufferedChanges ***");
		}
		bool flag = false;
		if (m_UpdateBuildings.Count > 0)
		{
			bool flag2 = false;
			foreach (Building updateBuilding in m_UpdateBuildings)
			{
				flag2 = true;
				DoUpdateBuilding(updateBuilding);
			}
			if (flag2)
			{
				if (m_Log)
				{
					Debug.Log("Updating " + m_UpdateBuildings.Count);
				}
				flag = true;
			}
			m_UpdateBuildings.Clear();
		}
		if (m_RemoveBuildings.Count > 0)
		{
			bool flag3 = false;
			foreach (Building removeBuilding in m_RemoveBuildings)
			{
				if (DoRemoveBuilding(removeBuilding))
				{
					flag3 = true;
				}
			}
			if (flag3)
			{
				if (m_Log)
				{
					Debug.Log("Removing " + m_RemoveBuildings.Count);
				}
				flag = true;
			}
			m_RemoveBuildings.Clear();
		}
		if (m_AddBuildings.Count > 0)
		{
			bool flag4 = false;
			foreach (Building addBuilding in m_AddBuildings)
			{
				if (addBuilding.GetIsSavable())
				{
					flag4 = true;
					DoAddBuilding(addBuilding);
				}
			}
			if (flag4)
			{
				if (m_Log)
				{
					Debug.Log("Adding " + m_AddBuildings.Count);
				}
				flag = true;
			}
			m_AddBuildings.Clear();
		}
		if (!flag || !m_Log)
		{
			return;
		}
		Debug.Log("Total Systems = " + m_Systems.Count);
		foreach (LinkedSystem system in m_Systems)
		{
			Debug.Log("     " + system.m_Buildings.Count);
		}
	}

	public void UpdateAll()
	{
		UpdateBufferedChanges();
		foreach (LinkedSystem system in m_Systems)
		{
			system.Update();
		}
	}

	public void EndEditMode()
	{
		foreach (LinkedSystem system in m_Systems)
		{
			system.EndEditMode();
		}
	}
}
