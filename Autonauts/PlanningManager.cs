using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class PlanningManager : MonoBehaviour
{
	public static PlanningManager Instance;

	private List<PlanningArea> m_PlanningArea;

	public bool m_ShowAreas;

	public bool m_ShowNames;

	public bool m_ShowDimensions;

	private void Awake()
	{
		Instance = this;
		m_PlanningArea = new List<PlanningArea>();
		m_ShowAreas = false;
		m_ShowNames = true;
		m_ShowDimensions = true;
		PlanningArea.InitColours();
	}

	public void Save(JSONNode ParentNode)
	{
		JSONUtils.Set(ParentNode, "ShowAreas", m_ShowAreas);
		JSONUtils.Set(ParentNode, "ShowNames", m_ShowNames);
		JSONUtils.Set(ParentNode, "ShowDimensions", m_ShowDimensions);
		JSONArray jSONArray = (JSONArray)(ParentNode["Areas"] = new JSONArray());
		int num = 0;
		foreach (PlanningArea item in m_PlanningArea)
		{
			JSONNode jSONNode2 = new JSONObject();
			item.Save(jSONNode2);
			jSONArray[num] = jSONNode2;
			num++;
		}
	}

	public void Load(JSONNode ParentNode)
	{
		JSONArray asArray = ParentNode["Areas"].AsArray;
		for (int i = 0; i < asArray.Count; i++)
		{
			JSONNode asObject = asArray[i].AsObject;
			PlanningArea planningArea = new PlanningArea();
			planningArea.SetVisible(Visible: false);
			planningArea.Load(asObject);
			m_PlanningArea.Add(planningArea);
		}
		bool asBool = JSONUtils.GetAsBool(ParentNode, "ShowNames", DefaultValue: true);
		SetShowNames(asBool);
		asBool = JSONUtils.GetAsBool(ParentNode, "ShowDimensions", DefaultValue: true);
		SetShowDimensions(asBool);
		m_ShowAreas = JSONUtils.GetAsBool(ParentNode, "ShowAreas", DefaultValue: false);
		UpdateShowAreas();
	}

	public PlanningArea NewArea(TileCoord TopLeft, TileCoord BottomRight)
	{
		PlanningArea planningArea = new PlanningArea();
		planningArea.SetDefaults();
		planningArea.SetCoords(TopLeft, BottomRight);
		planningArea.m_AreaIndicator.SetNameEnabled(m_ShowNames);
		planningArea.m_AreaIndicator.SetDimensionsEnabled(m_ShowDimensions);
		m_PlanningArea.Add(planningArea);
		return planningArea;
	}

	public PlanningArea GetAreaAtTile(TileCoord NewCoord)
	{
		PlanningArea result = null;
		foreach (PlanningArea item in m_PlanningArea)
		{
			if (item.ContainsCoord(NewCoord))
			{
				result = item;
			}
		}
		return result;
	}

	public void DeleteArea(PlanningArea NewArea)
	{
		m_PlanningArea.Remove(NewArea);
		NewArea.Delete();
	}

	public void DeselectAll(bool Deselect, PlanningArea Except)
	{
		foreach (PlanningArea item in m_PlanningArea)
		{
			if (item != Except)
			{
				item.SetDeselected(Deselect);
			}
		}
	}

	public void SetVisible(bool Visible)
	{
		foreach (PlanningArea item in m_PlanningArea)
		{
			item.SetVisible(Visible);
		}
	}

	public void UpdateNamesMode()
	{
		SetVisible(m_ShowNames);
	}

	public void UpdateDimensions()
	{
		SetVisible(m_ShowDimensions);
	}

	public void SetShowNames(bool ShowNames)
	{
		m_ShowNames = ShowNames;
		foreach (PlanningArea item in m_PlanningArea)
		{
			item.m_AreaIndicator.SetNameEnabled(m_ShowNames);
		}
	}

	public void SetShowDimensions(bool ShowDimensions)
	{
		m_ShowDimensions = ShowDimensions;
		foreach (PlanningArea item in m_PlanningArea)
		{
			item.m_AreaIndicator.SetDimensionsEnabled(m_ShowDimensions);
		}
	}

	public void UpdateShowAreas()
	{
		if (GameStateManager.Instance.GetActualState() == GameStateManager.State.Planning && (bool)GameStateManager.Instance.GetCurrentState().GetComponent<GameStatePlanning>().m_Planning)
		{
			SetVisible(Visible: true);
		}
		else
		{
			SetVisible(m_ShowAreas);
		}
	}

	public void ToggleShowAreas()
	{
		m_ShowAreas = !m_ShowAreas;
		UpdateShowAreas();
	}

	public void SetShowAreas(bool Show)
	{
		m_ShowAreas = Show;
		UpdateShowAreas();
	}
}
