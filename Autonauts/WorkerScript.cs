using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class WorkerScript
{
	public string m_Name;

	public List<WorkerInstruction> m_Instructions;

	public void Start()
	{
		m_Name = "Script";
		m_Instructions = new List<WorkerInstruction>();
	}

	public void AddInstruction(WorkerInstruction.Instruction NewInstruction, string m_Variable1 = "", string m_Variable2 = "", string m_Variable3 = "", string m_Variable4 = "")
	{
		WorkerInstruction item = default(WorkerInstruction);
		item.m_Instruction = NewInstruction;
		item.m_Variable1 = m_Variable1;
		item.m_Variable2 = m_Variable2;
		item.m_Variable3 = m_Variable3;
		item.m_Variable4 = m_Variable4;
		m_Instructions.Add(item);
	}

	public void Save(JSONNode Node)
	{
		JSONUtils.Set(Node, "Name", m_Name);
		JSONArray jSONArray = (JSONArray)(Node["Instructions"] = new JSONArray());
		for (int i = 0; i < m_Instructions.Count; i++)
		{
			jSONArray[i] = new JSONObject();
			m_Instructions[i].Save(jSONArray[i]);
		}
	}

	public void Load(JSONNode Node)
	{
		m_Name = JSONUtils.GetAsString(Node, "Name", "Name");
		JSONArray asArray = Node["Instructions"].AsArray;
		for (int i = 0; i < asArray.Count; i++)
		{
			WorkerInstruction item = default(WorkerInstruction);
			if (!item.Load(asArray[i]))
			{
				Debug.Log("Trying to load unknown instruction " + (string)asArray[i]["Ins"] + " at line " + i + " in script " + m_Name);
			}
			else
			{
				m_Instructions.Add(item);
			}
		}
	}

	public void PostLoad()
	{
		for (int i = 0; i < m_Instructions.Count; i++)
		{
			WorkerInstruction value = m_Instructions[i];
			value.PostLoad();
			m_Instructions[i] = value;
		}
	}

	public string GetScriptAsString(int CurrentInstruction = -1)
	{
		string text = "";
		for (int i = 0; i < m_Instructions.Count; i++)
		{
			text += m_Instructions[i].GetToString();
			text = ((i != CurrentInstruction) ? (text + "\n") : (text + " <<<\n"));
		}
		return text;
	}

	public void SetScriptFromStringArray(List<string> Strings)
	{
	}
}
