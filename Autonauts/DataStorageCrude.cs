using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class DataStorageCrude : Holdable
{
	public enum State
	{
		Empty,
		Full,
		Total
	}

	[HideInInspector]
	public State m_State;

	private float m_StateTimer;

	[HideInInspector]
	public List<HighInstruction> m_HighInstructions;

	[HideInInspector]
	public string m_WorkerName;

	public override void Restart()
	{
		base.Restart();
		m_HighInstructions = new List<HighInstruction>();
		m_WorkerName = TextManager.Instance.Get("DataStorageCrudeBlank");
		m_State = State.Empty;
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		if (m_HighInstructions.Count <= 0)
		{
			return;
		}
		JSONUtils.Set(Node, "Name", m_WorkerName);
		JSONArray jSONArray = (JSONArray)(Node["HighInstructionsArray"] = new JSONArray());
		int num = 0;
		foreach (HighInstruction highInstruction in m_HighInstructions)
		{
			jSONArray[num] = new JSONObject();
			highInstruction.Save(jSONArray[num]);
			num++;
		}
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		JSONNode jSONNode = Node["HighInstructionsArray"];
		if (jSONNode != null && !jSONNode.IsNull)
		{
			string asString = JSONUtils.GetAsString(Node, "Name", "");
			List<HighInstruction> list = new List<HighInstruction>();
			for (int i = 0; i < jSONNode.Count; i++)
			{
				JSONNode asObject = jSONNode[i].AsObject;
				HighInstruction highInstruction = new HighInstruction(HighInstruction.Type.Total, null);
				highInstruction.Load(asObject);
				list.Add(highInstruction);
			}
			SetData(list, asString);
		}
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		base.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (Info.m_ActionType == AFO.AT.Primary && Info.m_ObjectType == ObjectType.ToolBroom)
		{
			return GetActionFromBroom(Info);
		}
		return GetActionFromCurrentState(Info, "DataStorage", m_State.ToString());
	}

	private void SetData(List<HighInstruction> Instructions, string Name)
	{
		m_HighInstructions = HighInstruction.Copy(Instructions);
		m_WorkerName = Name;
		m_State = State.Full;
	}

	public void Copy(List<HighInstruction> Instructions, string Name)
	{
		SetData(Instructions, Name);
	}

	public List<HighInstruction> Paste()
	{
		return HighInstruction.Copy(m_HighInstructions);
	}

	public override string GetHumanReadableName()
	{
		return base.GetHumanReadableName() + " (" + m_WorkerName + ")";
	}
}
