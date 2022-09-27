using SimpleJSON;
using UnityEngine;

public class WorldSettings : MonoBehaviour
{
	public static WorldSettings Instance;

	private int[] m_WorkerSerials;

	private int m_WorkerGroup;

	private bool m_AutopediaSeen;

	private void Awake()
	{
		Instance = this;
		int num = 4;
		m_WorkerSerials = new int[num];
		for (int i = 0; i < num; i++)
		{
			m_WorkerSerials[i] = 0;
		}
		m_WorkerGroup = 0;
		m_AutopediaSeen = false;
	}

	public void Save(JSONNode Node)
	{
		JSONArray jSONArray = (JSONArray)(Node["WorkerSerials"] = new JSONArray());
		for (int i = 0; i < m_WorkerSerials.Length; i++)
		{
			jSONArray[i] = m_WorkerSerials[i];
		}
		JSONUtils.Set(Node, "WorkerGroup", m_WorkerGroup);
		JSONUtils.Set(Node, "AutopediaSeen", m_AutopediaSeen);
	}

	public void Load(JSONNode Node)
	{
		JSONNode jSONNode = Node["WorkerSerials"];
		for (int i = 0; i < jSONNode.Count; i++)
		{
			if (i < m_WorkerSerials.Length)
			{
				m_WorkerSerials[i] = jSONNode[i].AsInt;
			}
		}
		m_WorkerGroup = JSONUtils.GetAsInt(Node, "WorkerGroup", 0);
		m_AutopediaSeen = JSONUtils.GetAsBool(Node, "AutopediaSeen", DefaultValue: false);
	}

	public int CreateWorker(int Level)
	{
		int result = m_WorkerSerials[Level];
		m_WorkerSerials[Level]++;
		return result;
	}

	public int CreateGroup()
	{
		m_WorkerGroup++;
		return m_WorkerGroup;
	}

	public bool GetAutopediaSeen()
	{
		return m_AutopediaSeen;
	}

	public void SetAutopediaSeen()
	{
		m_AutopediaSeen = true;
	}
}
