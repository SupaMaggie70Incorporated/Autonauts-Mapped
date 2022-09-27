using SimpleJSON;

public class WorkerScriptLocal
{
	public static int m_MaxLocalVariables = 16;

	public WorkerScript m_Script;

	public int m_CurrentInstruction;

	public string[] m_LocalVariables;

	public float m_WaitTimer;

	public bool m_GetNearestStarted;

	public void Start()
	{
		m_CurrentInstruction = 0;
		m_LocalVariables = new string[m_MaxLocalVariables];
		for (int i = 0; i < m_MaxLocalVariables; i++)
		{
			m_LocalVariables[i] = "";
		}
		m_WaitTimer = 0f;
		m_GetNearestStarted = false;
	}

	public void Save(JSONNode Node)
	{
		Node["Script"] = new JSONObject();
		m_Script.Save(Node["Script"]);
		JSONUtils.Set(Node, "Instruction", m_CurrentInstruction);
		JSONArray jSONArray = (JSONArray)(Node["LocalArray"] = new JSONArray());
		for (int i = 0; i < m_MaxLocalVariables; i++)
		{
			jSONArray[i] = m_LocalVariables[i];
		}
	}

	public void Load(JSONNode Node)
	{
		m_Script = new WorkerScript();
		m_Script.Start();
		m_Script.Load(Node["Script"]);
		m_CurrentInstruction = JSONUtils.GetAsInt(Node, "Instruction", 0);
		JSONArray asArray = Node["LocalArray"].AsArray;
		for (int i = 0; i < m_MaxLocalVariables; i++)
		{
			m_LocalVariables[i] = asArray[i];
		}
	}
}
