using System;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;
using UnityEngine;

public class IndustryData
{
	public List<Industry> m_Industries;

	public List<IndustryLevel> m_ResearchMissions;

	private IndustrySub NewSub;

	private IndustryLevel NewLevel;

	private Industry NewIndustry;

	private string m_SaveFileName = "Data/IndustryData";

	public void Save()
	{
		JSONNode jSONNode = new JSONObject();
		JSONArray jSONArray = (JSONArray)(jSONNode["Levels"] = new JSONArray());
		int num = 0;
		foreach (Industry industry in m_Industries)
		{
			foreach (IndustrySub subIndustry in industry.m_SubIndustries)
			{
				foreach (IndustryLevel industryLevel in subIndustry.m_IndustryLevels)
				{
					JSONNode jSONNode3 = new JSONObject();
					industryLevel.Save(jSONNode3);
					jSONArray[num] = jSONNode3;
					num++;
				}
			}
		}
		jSONArray = (JSONArray)(jSONNode["Research"] = new JSONArray());
		num = 0;
		foreach (IndustryLevel researchMission in m_ResearchMissions)
		{
			JSONNode jSONNode5 = new JSONObject();
			researchMission.Save(jSONNode5);
			jSONArray[num] = jSONNode5;
			num++;
		}
		string contents = jSONNode.ToString();
		try
		{
			File.WriteAllText(Application.dataPath + "/Resources/" + m_SaveFileName + ".txt", contents);
		}
		catch (UnauthorizedAccessException ex)
		{
			Debug.Log("UnauthorizedAccessException : " + m_SaveFileName + " " + ex.ToString());
		}
	}

	public void Load()
	{
		JSONNode jSONNode = JSON.Parse(((TextAsset)Resources.Load(m_SaveFileName, typeof(TextAsset))).text);
		JSONArray asArray = jSONNode["Levels"].AsArray;
		foreach (Industry industry in m_Industries)
		{
			foreach (IndustrySub subIndustry in industry.m_SubIndustries)
			{
				foreach (IndustryLevel industryLevel in subIndustry.m_IndustryLevels)
				{
					for (int i = 0; i < asArray.Count; i++)
					{
						JSONNode asObject = asArray[i].AsObject;
						if ((string)asObject["Name"] == industryLevel.m_ID.ToString())
						{
							industryLevel.Load(asObject);
							break;
						}
					}
				}
			}
		}
		asArray = jSONNode["Research"].AsArray;
		if (!(asArray != null) || asArray.IsNull)
		{
			return;
		}
		foreach (IndustryLevel researchMission in m_ResearchMissions)
		{
			for (int j = 0; j < asArray.Count; j++)
			{
				JSONNode asObject2 = asArray[j].AsObject;
				if ((string)asObject2["Name"] == researchMission.m_ID.ToString())
				{
					researchMission.Load(asObject2);
					break;
				}
			}
		}
	}

	private Industry CreateIndustry(string Name, Color NewColour)
	{
		Industry industry = new Industry("Industry" + Name, NewColour);
		m_Industries.Add(industry);
		return industry;
	}

	private IndustryLevel CreateResearch(Quest.ID NewID, ObjectType NewType, Quest.ResearchType NewResearchType)
	{
		IndustryLevel industryLevel = new IndustryLevel(NewID, null, Quest.Type.Research, MajorNode: false, ShowUnlockedQuests: true);
		industryLevel.m_ResearchObjectType = NewType;
		industryLevel.m_ResearchType = NewResearchType;
		int variableAsInt = VariableManager.Instance.GetVariableAsInt(NewID, "Hearts", CheckValid: false);
		industryLevel.AddRequirement(QuestEvent.Type.Research, BotOnly: false, NewType, variableAsInt);
		m_ResearchMissions.Add(industryLevel);
		return industryLevel;
	}

	public IndustryData()
	{
		m_Industries = new List<Industry>();
		m_ResearchMissions = new List<IndustryLevel>();
		Load();
		foreach (Industry industry in m_Industries)
		{
			industry.ConvertToQuests();
		}
		foreach (IndustryLevel researchMission in m_ResearchMissions)
		{
			researchMission.ConvertToQuest(0);
		}
	}

	public IndustryLevel GetIndustryLevelFromQuest(Quest NewQuest)
	{
		return NewQuest?.m_IndustryLevel;
	}

	public void SetIndustryLevelForQuest(Quest NewQuest)
	{
		foreach (Industry industry in m_Industries)
		{
			foreach (IndustrySub subIndustry in industry.m_SubIndustries)
			{
				foreach (IndustryLevel industryLevel in subIndustry.m_IndustryLevels)
				{
					if (industryLevel.m_Quest == NewQuest)
					{
						NewQuest.m_IndustryLevel = industryLevel;
						return;
					}
				}
			}
		}
		foreach (IndustryLevel researchMission in m_ResearchMissions)
		{
			if (researchMission.m_Quest == NewQuest)
			{
				NewQuest.m_IndustryLevel = researchMission;
				return;
			}
		}
		NewQuest.m_IndustryLevel = null;
	}
}
