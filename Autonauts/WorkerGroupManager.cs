using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class WorkerGroupManager : MonoBehaviour
{
	private static int m_MaxGroups = 300;

	public static WorkerGroupManager Instance;

	public List<WorkerGroup> m_Groups;

	public WorkerGroup m_TempGroup;

	private int m_IDCounter;

	private void Awake()
	{
		Instance = this;
		m_Groups = new List<WorkerGroup>();
		m_TempGroup = new WorkerGroup("Temp");
		m_IDCounter = 0;
	}

	public void Save(JSONNode Node)
	{
		JSONUtils.Set(Node, "IDCounter", m_IDCounter);
		JSONArray jSONArray = (JSONArray)(Node["Groups"] = new JSONArray());
		int num = 0;
		foreach (WorkerGroup group in m_Groups)
		{
			JSONNode jSONNode2 = new JSONObject();
			group.Save(jSONNode2);
			jSONArray[num] = jSONNode2;
			num++;
		}
	}

	public void Load(JSONNode Node)
	{
		m_IDCounter = JSONUtils.GetAsInt(Node, "IDCounter", 0);
		JSONArray asArray = Node["Groups"].AsArray;
		for (int i = 0; i < asArray.Count; i++)
		{
			JSONNode node = asArray[i];
			if (m_Groups.Count < m_MaxGroups)
			{
				WorkerGroup workerGroup = new WorkerGroup();
				workerGroup.Load(node);
				m_Groups.Add(workerGroup);
			}
		}
		for (int j = 0; j < asArray.Count; j++)
		{
			JSONNode node = asArray[j];
			m_Groups[j].LoadGroupsOfGroups(node);
		}
	}

	public WorkerGroup CreateNewGroup()
	{
		if (m_Groups.Count == m_MaxGroups)
		{
			return null;
		}
		m_IDCounter++;
		WorkerGroup workerGroup = new WorkerGroup(TextManager.Instance.Get("EdtiGroupDefaultName") + m_IDCounter, m_IDCounter);
		m_Groups.Add(workerGroup);
		return workerGroup;
	}

	public void DeleteGroup(WorkerGroup NewGroup)
	{
		m_Groups.Remove(NewGroup);
		NewGroup.Delete();
	}

	public WorkerGroup GetGroupFromID(int DesiredID)
	{
		foreach (WorkerGroup group in m_Groups)
		{
			if (group.m_ID == DesiredID)
			{
				return group;
			}
		}
		return null;
	}
}
