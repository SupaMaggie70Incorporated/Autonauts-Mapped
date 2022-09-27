using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class WorkerGroup
{
	public static Color m_DefaultColour = new Color(0.75f, 0.75f, 0.75f);

	public static Color[] m_Colours;

	public string m_Name;

	public string m_Description;

	public int m_ID;

	public int m_ColourIndex;

	public List<int> m_WorkerUIDs;

	public WorkerGroupPanel m_WorkerGroupPanel;

	public bool m_Collapsed;

	public List<int> m_GroupUIDs;

	public int m_GroupParentGroup = -1;

	public static void Init()
	{
		int[] obj = new int[16]
		{
			16724530, 16756786, 16777010, 3342130, 3342335, 5000447, 16711935, 13553325, 16752800, 10049551,
			11710983, 1613872, 2130048, 41215, 9726161, 8355691
		};
		int num = 0;
		m_Colours = new Color[obj.Length];
		int[] array = obj;
		foreach (int colour in array)
		{
			m_Colours[num] = GeneralUtils.ColorFromHex(colour);
			num++;
		}
	}

	public WorkerGroup(string Name = "", int ID = 0)
	{
		m_Name = Name;
		m_Description = "";
		m_ID = ID;
		m_ColourIndex = (ID - 1) % m_Colours.Length;
		m_WorkerUIDs = new List<int>();
		m_GroupUIDs = new List<int>();
		m_Collapsed = false;
	}

	public void Save(JSONNode Node)
	{
		JSONUtils.Set(Node, "Name", m_Name);
		JSONUtils.Set(Node, "Description", m_Description);
		JSONUtils.Set(Node, "ID", m_ID);
		JSONUtils.Set(Node, "Colour", m_ColourIndex);
		JSONUtils.Set(Node, "Collapsed", m_Collapsed);
		JSONUtils.Set(Node, "Parent", m_GroupParentGroup);
		JSONArray jSONArray = (JSONArray)(Node["UIDs"] = new JSONArray());
		int num = 0;
		foreach (int workerUID in m_WorkerUIDs)
		{
			jSONArray[num] = workerUID;
			num++;
		}
		JSONArray jSONArray2 = (JSONArray)(Node["GroupUIDs"] = new JSONArray());
		num = 0;
		foreach (int groupUID in m_GroupUIDs)
		{
			jSONArray2[num] = groupUID;
			num++;
		}
	}

	public void Load(JSONNode Node)
	{
		m_Name = JSONUtils.GetAsString(Node, "Name", "Group");
		m_Description = JSONUtils.GetAsString(Node, "Description", "");
		m_ID = JSONUtils.GetAsInt(Node, "ID", 0);
		m_ColourIndex = JSONUtils.GetAsInt(Node, "Colour", 0);
		m_Collapsed = JSONUtils.GetAsBool(Node, "Collapsed", DefaultValue: false);
		m_GroupParentGroup = JSONUtils.GetAsInt(Node, "Parent", -1);
		TabWorkers.Instance.AddGroup(this);
		JSONArray asArray = Node["UIDs"].AsArray;
		for (int i = 0; i < asArray.Count; i++)
		{
			int asInt = asArray[i].AsInt;
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(asInt, ErrorCheck: false);
			if (objectFromUniqueID == null)
			{
				ErrorMessage.LogError("Group " + m_Name + " contains an invalid object : " + asInt);
			}
			else
			{
				TabWorkers.Instance.SetWorkerGroup(objectFromUniqueID.GetComponent<Worker>(), this);
			}
		}
	}

	public void LoadGroupsOfGroups(JSONNode Node)
	{
		JSONArray asArray = Node["GroupUIDs"].AsArray;
		for (int i = 0; i < asArray.Count; i++)
		{
			int asInt = asArray[i].AsInt;
			WorkerGroup groupFromID = WorkerGroupManager.Instance.GetGroupFromID(asInt);
			if (groupFromID == null)
			{
				ErrorMessage.LogError("[WorkerGroup - Load] Group " + m_Name + " cannot be found : " + asInt);
			}
			else
			{
				TabWorkers.Instance.SetGroupGroup(groupFromID, this);
			}
		}
	}

	public void ClearTemp()
	{
		List<int> list = new List<int>();
		foreach (int workerUID in m_WorkerUIDs)
		{
			list.Add(workerUID);
		}
		foreach (int item in list)
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(item, ErrorCheck: false);
			if ((bool)objectFromUniqueID)
			{
				RemoveWorker(objectFromUniqueID.GetComponent<Worker>(), Temp: true);
			}
		}
		m_WorkerUIDs.Clear();
	}

	public void AddWorker(Worker NewObject, bool Temp = false)
	{
		if (!m_WorkerUIDs.Contains(NewObject.m_UniqueID))
		{
			m_WorkerUIDs.Add(NewObject.m_UniqueID);
			if (!Temp)
			{
				NewObject.GetComponent<Worker>().m_Group = this;
				m_WorkerGroupPanel.UpdateWorkersAndGroups();
			}
		}
		else
		{
			Debug.Log("Object " + NewObject.m_UniqueID + " already added to group " + m_Name);
		}
	}

	public void AddGroup(WorkerGroup NewObject)
	{
		if (!m_GroupUIDs.Contains(NewObject.m_ID))
		{
			m_GroupUIDs.Add(NewObject.m_ID);
			NewObject.m_GroupParentGroup = m_ID;
			m_WorkerGroupPanel.UpdateWorkersAndGroups();
		}
		else
		{
			Debug.Log("[WorkerGroup - AddGroup] Object " + NewObject.m_ID + " already added to group " + m_Name);
		}
	}

	public void RemoveWorker(Worker NewObject, bool Temp = false)
	{
		if (m_WorkerUIDs.Contains(NewObject.m_UniqueID))
		{
			m_WorkerUIDs.Remove(NewObject.m_UniqueID);
			if (!Temp)
			{
				NewObject.GetComponent<Worker>().m_Group = null;
				m_WorkerGroupPanel.UpdateWorkersAndGroups();
			}
		}
		else
		{
			Debug.Log("Can't find object " + NewObject.m_UniqueID + " in group " + m_Name);
		}
	}

	public void RemoveGroup(WorkerGroup NewObject)
	{
		if (m_GroupUIDs.Contains(NewObject.m_ID))
		{
			m_GroupUIDs.Remove(NewObject.m_ID);
			NewObject.m_GroupParentGroup = -1;
			m_WorkerGroupPanel.UpdateWorkersAndGroups();
		}
		else
		{
			Debug.Log("[WorkerGroup - RemoveGroup] Can't find object " + NewObject.m_ID + " in group " + m_Name);
		}
	}

	public Color GetColour()
	{
		return m_Colours[m_ColourIndex];
	}

	public void SetColourIndex(int Index)
	{
		m_ColourIndex = Index;
		if ((bool)m_WorkerGroupPanel)
		{
			m_WorkerGroupPanel.UpdateSelectedColour();
		}
		foreach (int workerUID in m_WorkerUIDs)
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(workerUID, ErrorCheck: false);
			if ((bool)objectFromUniqueID)
			{
				Worker component = objectFromUniqueID.GetComponent<Worker>();
				component.m_WorkerInfoPanel.UpdateSelectedColour();
				component.m_WorkerArrow.UpdateColour();
			}
		}
	}

	public void Delete()
	{
		foreach (int workerUID in m_WorkerUIDs)
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(workerUID, ErrorCheck: false);
			if ((bool)objectFromUniqueID)
			{
				objectFromUniqueID.GetComponent<Worker>().m_Group = null;
			}
		}
	}
}
