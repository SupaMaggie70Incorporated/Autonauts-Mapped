using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class HighInstructionList
{
	public List<HighInstruction> m_List;

	private List<AreaIndicator> m_InstructionIndicators;

	private List<HighInstruction> m_InstructionHigh;

	private bool m_ShowAreaIndicators;

	private void Init()
	{
		m_List = new List<HighInstruction>();
		m_InstructionIndicators = new List<AreaIndicator>();
		m_InstructionHigh = new List<HighInstruction>();
	}

	public HighInstructionList()
	{
		Init();
	}

	public HighInstructionList(HighInstructionList OldInstructions)
	{
		Init();
		Copy(OldInstructions);
	}

	public void Save(JSONNode Node)
	{
		JSONArray jSONArray = (JSONArray)(Node["HighInstructionsArray"] = new JSONArray());
		int num = 0;
		foreach (HighInstruction item in m_List)
		{
			jSONArray[num] = new JSONObject();
			item.Save(jSONArray[num]);
			num++;
		}
	}

	public void Load(JSONNode Node)
	{
		JSONNode jSONNode = Node["HighInstructionsArray"];
		if (jSONNode != null && !jSONNode.IsNull)
		{
			List<HighInstruction> list = new List<HighInstruction>();
			for (int i = 0; i < jSONNode.Count; i++)
			{
				JSONNode asObject = jSONNode[i].AsObject;
				HighInstruction highInstruction = new HighInstruction(HighInstruction.Type.Total, null);
				highInstruction.Load(asObject);
				list.Add(highInstruction);
			}
			Set(list);
		}
	}

	public void PostLoad()
	{
		foreach (HighInstruction item in m_List)
		{
			item.PostLoad();
		}
		foreach (HighInstruction item2 in m_InstructionHigh)
		{
			item2.PostLoad();
		}
	}

	public void Clear()
	{
		m_List.Clear();
		MakeAreaIndicators();
	}

	public void Set(List<HighInstruction> OldList)
	{
		m_List = OldList;
		MakeAreaIndicators();
	}

	public void Copy(List<HighInstruction> CopyList)
	{
		m_List = HighInstruction.Copy(CopyList);
		MakeAreaIndicators();
	}

	public void Add(HighInstruction NewInstruction)
	{
		m_List.Add(NewInstruction);
		MakeAreaIndicators();
	}

	public void AddToChild(HighInstruction ChildInstruction, HighInstruction NewInstruction, bool Children2)
	{
		if (Children2)
		{
			ChildInstruction.m_Children2.Add(NewInstruction);
		}
		else
		{
			ChildInstruction.m_Children.Add(NewInstruction);
		}
		NewInstruction.m_Parent = ChildInstruction;
		MakeAreaIndicators();
	}

	public void Insert(int Index, HighInstruction NewInstruction)
	{
		m_List.Insert(Index, NewInstruction);
		MakeAreaIndicators();
	}

	public void InsertToChild(HighInstruction ChildInstruction, int Index, HighInstruction NewInstruction, bool Children2)
	{
		if (Children2)
		{
			ChildInstruction.m_Children2.Insert(Index, NewInstruction);
		}
		else
		{
			ChildInstruction.m_Children.Insert(Index, NewInstruction);
		}
		NewInstruction.m_Parent = ChildInstruction;
		MakeAreaIndicators();
	}

	public void Remove(HighInstruction NewInstruction)
	{
		int num = 0;
		if (NewInstruction.m_Parent == null)
		{
			m_List.Remove(NewInstruction);
		}
		else if (NewInstruction.m_Parent.m_Children.Contains(NewInstruction))
		{
			num = NewInstruction.m_Parent.m_Children.IndexOf(NewInstruction);
			NewInstruction.m_Parent.m_Children.RemoveAt(num);
		}
		else
		{
			num = NewInstruction.m_Parent.m_Children2.IndexOf(NewInstruction);
			NewInstruction.m_Parent.m_Children2.RemoveAt(num);
		}
		MakeAreaIndicators();
	}

	public void Copy(HighInstructionList CopyList)
	{
		Copy(CopyList.m_List);
		MakeAreaIndicators();
	}

	private void CopyLineNumbers(List<HighInstruction> NewList, List<HighInstruction> Copylist)
	{
		if (NewList.Count != Copylist.Count)
		{
			Debug.Log("Instruction counts don't match");
			return;
		}
		for (int i = 0; i < NewList.Count; i++)
		{
			NewList[i].m_ScriptLineNumber = Copylist[i].m_ScriptLineNumber;
			if (NewList[i].m_Children.Count > 0 && Copylist[i].m_Children.Count > 0)
			{
				CopyLineNumbers(NewList[i].m_Children, Copylist[i].m_Children);
			}
			if (NewList[i].m_Children2.Count > 0 && Copylist[i].m_Children2.Count > 0)
			{
				CopyLineNumbers(NewList[i].m_Children2, Copylist[i].m_Children2);
			}
		}
	}

	public void CopyLineNumbers(HighInstructionList CopyList)
	{
		CopyLineNumbers(m_List, CopyList.m_List);
	}

	private void CreateAreaIndicators(List<HighInstruction> NewInstructions, List<AreaIndicator> OldList)
	{
		foreach (HighInstruction NewInstruction in NewInstructions)
		{
			if (NewInstruction.m_Children.Count != 0)
			{
				CreateAreaIndicators(NewInstruction.m_Children, OldList);
			}
			if (NewInstruction.m_Children2.Count != 0)
			{
				CreateAreaIndicators(NewInstruction.m_Children2, OldList);
			}
			if (!NewInstruction.GetIsGetNearest())
			{
				continue;
			}
			AreaIndicator areaIndicator = AreaIndicatorManager.Instance.GetAreaIndicatorFromInstruction(NewInstruction);
			if (!areaIndicator)
			{
				if (NewInstruction.m_ActionInfo.m_ObjectUID != 0)
				{
					BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(NewInstruction.m_ActionInfo.m_ObjectUID, ErrorCheck: false);
					if ((bool)objectFromUniqueID)
					{
						ShowObjectIndicator(objectFromUniqueID, Show: true);
						if ((bool)objectFromUniqueID.GetComponent<Sign>())
						{
							areaIndicator = objectFromUniqueID.GetComponent<Sign>().m_Indicator;
						}
						if ((bool)objectFromUniqueID.GetComponent<Converter>())
						{
							areaIndicator = objectFromUniqueID.GetComponent<Converter>().m_Indicator;
						}
						KeepObjectIndicator(areaIndicator.m_Sign, Show: true);
						areaIndicator.SetActive(Active: false);
					}
					else
					{
						NewInstruction.m_ActionInfo.m_ObjectUID = 0;
						areaIndicator = AreaIndicatorManager.Instance.Add();
						areaIndicator.SetInstruction(NewInstruction);
					}
				}
				else
				{
					areaIndicator = AreaIndicatorManager.Instance.Add();
					areaIndicator.SetInstruction(NewInstruction);
				}
			}
			else
			{
				OldList.Remove(areaIndicator);
			}
			m_InstructionIndicators.Add(areaIndicator);
			m_InstructionHigh.Add(NewInstruction);
		}
	}

	private void KeepObjectIndicator(BaseClass NewObject, bool Show)
	{
		if (Sign.GetIsTypeSign(NewObject.m_TypeIdentifier))
		{
			NewObject.GetComponent<Sign>().KeepIndicator(Show);
		}
	}

	private void ShowObjectIndicator(BaseClass NewObject, bool Show)
	{
		if (Sign.GetIsTypeSign(NewObject.m_TypeIdentifier))
		{
			NewObject.GetComponent<Sign>().ShowIndicator(Show);
		}
		if ((bool)NewObject.GetComponent<Converter>())
		{
			NewObject.GetComponent<Converter>().ShowIndicator(Show);
		}
	}

	public void MakeAreaIndicators()
	{
		List<AreaIndicator> list = new List<AreaIndicator>();
		foreach (AreaIndicator instructionIndicator in m_InstructionIndicators)
		{
			list.Add(instructionIndicator);
		}
		m_InstructionIndicators.Clear();
		m_InstructionHigh.Clear();
		if (m_ShowAreaIndicators)
		{
			CreateAreaIndicators(m_List, list);
		}
		foreach (AreaIndicator item in list)
		{
			if ((bool)item.m_Sign)
			{
				KeepObjectIndicator(item.m_Sign, Show: false);
				ShowObjectIndicator(item.m_Sign, Show: false);
			}
			else
			{
				AreaIndicatorManager.Instance.Remove(item);
			}
		}
	}

	public void ClearAreaIndicators()
	{
		foreach (AreaIndicator instructionIndicator in m_InstructionIndicators)
		{
			if ((bool)instructionIndicator.m_Sign)
			{
				KeepObjectIndicator(instructionIndicator.m_Sign, Show: false);
				ShowObjectIndicator(instructionIndicator.m_Sign, Show: false);
			}
			else
			{
				AreaIndicatorManager.Instance.Remove(instructionIndicator);
			}
		}
		m_InstructionIndicators.Clear();
		m_InstructionHigh.Clear();
	}

	public void UpdateAreaIndicators()
	{
		foreach (AreaIndicator instructionIndicator in m_InstructionIndicators)
		{
			instructionIndicator.UpdateArea();
		}
	}

	public void ShowAreaIndicators(bool Show)
	{
		m_ShowAreaIndicators = Show;
		MakeAreaIndicators();
		for (int i = 0; i < m_InstructionIndicators.Count; i++)
		{
			AreaIndicator areaIndicator = m_InstructionIndicators[i];
			areaIndicator.SetVisible(Show);
			areaIndicator.SetFindType(m_InstructionHigh[i].GetFindType());
		}
	}

	public void ScaleAreaIndicators(bool Up)
	{
		if (Up)
		{
			m_ShowAreaIndicators = true;
			MakeAreaIndicators();
		}
		for (int i = 0; i < m_InstructionIndicators.Count; i++)
		{
			AreaIndicator areaIndicator = m_InstructionIndicators[i];
			if ((bool)areaIndicator.m_Sign)
			{
				if (!Up)
				{
					KeepObjectIndicator(areaIndicator.m_Sign, Show: false);
				}
				ShowObjectIndicator(areaIndicator.m_Sign, Up);
			}
			else
			{
				areaIndicator.Scale(Up);
			}
			areaIndicator.SetFindType(m_InstructionHigh[i].GetFindType());
		}
		if (!Up)
		{
			m_InstructionIndicators.Clear();
			m_InstructionHigh.Clear();
			m_ShowAreaIndicators = false;
		}
	}

	public void CancelScaleAreaIndicators()
	{
		foreach (AreaIndicator instructionIndicator in m_InstructionIndicators)
		{
			instructionIndicator.CancelScale();
		}
	}
}
