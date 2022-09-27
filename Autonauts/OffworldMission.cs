using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class OffworldMission
{
	public int m_ID;

	public bool m_Daily;

	public int m_Tickets;

	public List<IngredientRequirement> m_Requirements;

	public List<int> m_Progress;

	public bool m_Complete;

	public bool m_Selected;

	public int m_Planet;

	public static string[] m_PlanetNames = new string[6] { "Earth", "Mars", "AltairIV", "Mongo", "Magrathea", "Delos5" };

	public OffworldMission(bool Daily, int ID = -1)
	{
		m_Daily = Daily;
		m_ID = ID;
		m_Requirements = new List<IngredientRequirement>();
		m_Progress = new List<int>();
		m_Complete = false;
		m_Selected = false;
		m_Planet = Random.Range(0, m_PlanetNames.Length);
	}

	public void Save(JSONNode RootNode)
	{
		JSONArray jSONArray = (JSONArray)(RootNode["R"] = new JSONArray());
		int num = 0;
		foreach (IngredientRequirement requirement in m_Requirements)
		{
			jSONArray[num] = ObjectTypeList.Instance.GetSaveNameFromIdentifier(requirement.m_Type);
			num++;
			jSONArray[num] = requirement.m_Count;
			num++;
		}
		if (m_Complete)
		{
			JSONUtils.Set(RootNode, "C", Value: true);
		}
		else
		{
			jSONArray = (JSONArray)(RootNode["P"] = new JSONArray());
			num = 0;
			foreach (int item in m_Progress)
			{
				jSONArray[num] = item;
				num++;
			}
		}
		RootNode["S"] = m_Selected;
		RootNode["PL"] = m_Planet;
		RootNode["ID"] = m_ID;
	}

	public void Load(JSONNode RootNode)
	{
		m_Requirements = new List<IngredientRequirement>();
		JSONArray asArray = RootNode["R"].AsArray;
		int num;
		for (num = 0; num < asArray.Count; num++)
		{
			string saveName = asArray[num];
			ObjectType identifierFromSaveName = ObjectTypeList.Instance.GetIdentifierFromSaveName(saveName);
			num++;
			int count = asArray[num];
			AddRequirement(identifierFromSaveName, count);
		}
		m_Complete = JSONUtils.GetAsBool(RootNode, "C", DefaultValue: false);
		m_Progress = new List<int>();
		if (!m_Complete)
		{
			asArray = RootNode["P"].AsArray;
			for (int i = 0; i < asArray.Count; i++)
			{
				int item = asArray[i];
				m_Progress.Add(item);
			}
		}
		else
		{
			for (int j = 0; j < m_Requirements.Count; j++)
			{
				m_Progress.Add(m_Requirements[j].m_Count);
			}
		}
		m_Selected = JSONUtils.GetAsBool(RootNode, "S", DefaultValue: false);
		m_Planet = JSONUtils.GetAsInt(RootNode, "PL", 0);
		m_ID = JSONUtils.GetAsInt(RootNode, "ID", 0);
		CalcTicketAmount();
	}

	public void AddRequirement(ObjectType NewType, int Count)
	{
		IngredientRequirement item = new IngredientRequirement(NewType, Count);
		m_Requirements.Add(item);
		m_Progress.Add(0);
	}

	public void CalcTicketAmount()
	{
		if (m_Daily)
		{
			m_Tickets = VariableManager.Instance.GetVariableAsInt("DailyMissionTickets");
		}
		else
		{
			m_Tickets = VariableManager.Instance.GetVariableAsInt("RegularMissionTickets");
		}
	}

	public void CheckProgress()
	{
		for (int i = 0; i < m_Requirements.Count; i++)
		{
			IngredientRequirement ingredientRequirement = m_Requirements[i];
			if (m_Progress[i] != ingredientRequirement.m_Count)
			{
				return;
			}
		}
		m_Complete = true;
	}

	public bool AddObject(ObjectType NewType)
	{
		for (int i = 0; i < m_Requirements.Count; i++)
		{
			IngredientRequirement ingredientRequirement = m_Requirements[i];
			if ((ingredientRequirement.m_Type == NewType || (FolkHeart.GetIsFolkHeart(NewType) && ingredientRequirement.m_Type == ObjectType.HeartAny)) && m_Progress[i] < ingredientRequirement.m_Count)
			{
				m_Progress[i]++;
				CheckProgress();
				return true;
			}
		}
		return false;
	}

	public bool GetIsObjectTypeRequired(ObjectType NewType)
	{
		for (int i = 0; i < m_Requirements.Count; i++)
		{
			IngredientRequirement ingredientRequirement = m_Requirements[i];
			if ((ingredientRequirement.m_Type == NewType || (FolkHeart.GetIsFolkHeart(NewType) && ingredientRequirement.m_Type == ObjectType.HeartAny)) && m_Progress[i] < ingredientRequirement.m_Count)
			{
				return true;
			}
		}
		return false;
	}

	public void ForceComplete()
	{
		for (int i = 0; i < m_Requirements.Count; i++)
		{
			IngredientRequirement ingredientRequirement = m_Requirements[i];
			m_Progress[i] = ingredientRequirement.m_Count;
		}
		m_Complete = true;
	}

	private string GetPlanetName(int Planet)
	{
		string text = m_PlanetNames[Planet];
		if (TextManager.Instance.DoesExist("Planet" + text))
		{
			text = TextManager.Instance.Get("Planet" + text);
		}
		return text;
	}

	public string GetName(bool Short)
	{
		string planetName = GetPlanetName(m_Planet);
		string text = "";
		if (Short)
		{
			text = "Short";
		}
		if (m_Daily)
		{
			return TextManager.Instance.Get("SpacePortSelectDailyJob" + text, planetName);
		}
		ObjectType type = m_Requirements[0].m_Type;
		string text2 = "";
		if (type == ObjectType.HeartAny)
		{
			text2 = ObjectTypeList.Instance.GetHumanReadableNameFromIdentifier(ObjectType.FolkHeart);
		}
		else
		{
			ObjectCategory categoryFromType = ObjectTypeList.Instance.GetCategoryFromType(type);
			text2 = ObjectTypeList.Instance.GetCategoryName(categoryFromType);
			text2 = TextManager.Instance.Get(text2);
		}
		return TextManager.Instance.Get("SpacePortSelectRegularJob" + text, planetName, text2);
	}

	public string GetImage()
	{
		return "SpacePort/Planet" + m_PlanetNames[m_Planet];
	}

	public int GetReputationPoints()
	{
		if (m_Daily)
		{
			return VariableManager.Instance.GetVariableAsInt("ReputationPointsDaily");
		}
		return VariableManager.Instance.GetVariableAsInt("ReputationPointsRegular");
	}

	public int GetTickets()
	{
		return m_Tickets;
	}
}
