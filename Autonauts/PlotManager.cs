using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class PlotManager : MonoBehaviour
{
	public static PlotManager Instance;

	private static bool m_DebugLog = false;

	private static bool m_DebugError = true;

	[HideInInspector]
	public int m_PlotsWide;

	[HideInInspector]
	public int m_PlotsHigh;

	[HideInInspector]
	public float m_PlotWorldWidth;

	[HideInInspector]
	public float m_PlotWorldHeight;

	[HideInInspector]
	public Plot[] m_Plots;

	[HideInInspector]
	public List<Plot> m_VisiblePlots;

	private List<Clouds> m_Clouds;

	private Vector3 m_CloudsUVOffset;

	private Vector3 m_WindyUVOffset;

	public List<TileCoordObject>[] m_TileObjects;

	public bool m_CapObjects;

	private void Awake()
	{
		Instance = this;
		m_PlotWorldWidth = Tile.m_Size * (float)Plot.m_PlotTilesWide;
		m_PlotWorldHeight = Tile.m_Size * (float)Plot.m_PlotTilesHigh;
		SetDesaturation(Desaturated: false);
		m_CapObjects = true;
	}

	private void OnDestroy()
	{
		if (m_Clouds != null && m_Clouds[0] != null)
		{
			m_Clouds[0].GetComponent<MeshRenderer>().sharedMaterial.mainTextureOffset = default(Vector2);
		}
		Plot[] plots = m_Plots;
		for (int i = 0; i < plots.Length; i++)
		{
			Object.DestroyImmediate(plots[i].gameObject);
		}
	}

	public void Save(JSONNode Node)
	{
		JSONArray jSONArray = (JSONArray)(Node["PlotsVisible"] = new JSONArray());
		for (int i = 0; i < m_PlotsHigh; i++)
		{
			for (int j = 0; j < m_PlotsWide; j++)
			{
				int num = i * m_PlotsWide + j;
				if (m_Plots[num].m_Visible)
				{
					jSONArray[num] = 1;
				}
				else
				{
					jSONArray[num] = 0;
				}
			}
		}
	}

	public void Load(JSONNode Node)
	{
		JSONArray asArray = Node["PlotsVisible"].AsArray;
		for (int i = 0; i < m_PlotsHigh; i++)
		{
			for (int j = 0; j < m_PlotsWide; j++)
			{
				int num = i * m_PlotsWide + j;
				m_Plots[num].SetVisible(asArray[num].AsInt == 1);
			}
		}
	}

	public void CreatePlots(int TilesWide, int TilesHigh)
	{
		m_PlotsWide = TilesWide / Plot.m_PlotTilesWide;
		m_PlotsHigh = TilesHigh / Plot.m_PlotTilesHigh;
		m_Plots = new Plot[m_PlotsWide * m_PlotsHigh];
		m_VisiblePlots = new List<Plot>();
		for (int i = 0; i < m_PlotsHigh; i++)
		{
			for (int j = 0; j < m_PlotsWide; j++)
			{
				int num = i * m_PlotsWide + j;
				Vector3 position = new Vector3((float)j * m_PlotWorldWidth, 0f, 0f - (float)i * m_PlotWorldHeight);
				Plot component = InstantiationManager.Instance.CreateObject(ObjectType.Plot, "Plot", "", position, Quaternion.identity).GetComponent<Plot>();
				component.SetOffset(j, i);
				m_Plots[num] = component;
			}
		}
		m_TileObjects = new List<TileCoordObject>[TilesWide * TilesHigh];
		for (int k = 0; k < TilesWide * TilesHigh; k++)
		{
			m_TileObjects[k] = new List<TileCoordObject>();
		}
	}

	public void CreateClouds()
	{
		m_Clouds = new List<Clouds>();
		for (int i = -1; i < m_PlotsHigh + 1; i++)
		{
			for (int j = -1; j < m_PlotsWide + 1; j++)
			{
				Vector3 position = new Vector3((float)j * m_PlotWorldWidth, 80f, 0f - (float)i * m_PlotWorldHeight);
				Clouds component = Object.Instantiate((GameObject)Resources.Load("Prefabs/Clouds", typeof(GameObject)), default(Vector3), Quaternion.identity, MapManager.Instance.m_PlotRootTransform).GetComponent<Clouds>();
				component.GetComponent<MeshRenderer>().sharedMaterial = MaterialManager.Instance.m_CloudMaterial;
				component.SetPosition(position);
				m_Clouds.Add(component);
			}
		}
		m_CloudsUVOffset = default(Vector3);
		m_WindyUVOffset = default(Vector3);
	}

	private void UpdateClouds()
	{
		if (m_Clouds != null)
		{
			m_CloudsUVOffset.x += TimeManager.Instance.m_NormalDelta * 0.01f;
			m_CloudsUVOffset.y += TimeManager.Instance.m_NormalDelta * 0.005f;
			MaterialManager.Instance.m_CloudMaterial.mainTextureOffset = m_CloudsUVOffset;
		}
		m_WindyUVOffset.x += TimeManager.Instance.m_NormalDelta * 0.1f;
		m_WindyUVOffset.y += TimeManager.Instance.m_NormalDelta * 0.05f;
		if ((bool)MaterialManager.Instance.m_MaterialWindy.GetTexture("_OffsetTex"))
		{
			MaterialManager.Instance.m_MaterialWindy.SetTextureOffset("_OffsetTex", m_WindyUVOffset);
		}
	}

	public void UpdatePlotVisibility(Plot NewPlot, bool Visible)
	{
		if (Visible)
		{
			if (!m_VisiblePlots.Contains(NewPlot))
			{
				m_VisiblePlots.Add(NewPlot);
			}
		}
		else if (m_VisiblePlots.Contains(NewPlot))
		{
			m_VisiblePlots.Remove(NewPlot);
		}
	}

	public Plot GetPlotAtTile(int x, int y)
	{
		if (x < 0 || x >= TileManager.Instance.m_TilesWide || y < 0 || y >= TileManager.Instance.m_TilesHigh)
		{
			return null;
		}
		int num = y / Plot.m_PlotTilesHigh * m_PlotsWide + x / Plot.m_PlotTilesWide;
		return m_Plots[num];
	}

	public Plot GetPlotAtTile(TileCoord Position)
	{
		return GetPlotAtTile(Position.x, Position.y);
	}

	public Plot GetPlotAtPlot(int x, int y)
	{
		if (x < 0 || x >= m_PlotsWide || y < 0 || y >= m_PlotsHigh)
		{
			return null;
		}
		return m_Plots[y * m_PlotsWide + x];
	}

	public Plot GetPlotAtPosition(float x, float z)
	{
		int x2 = (int)(x / m_PlotWorldWidth);
		int y = (int)((0f - z) / m_PlotWorldHeight);
		return GetPlotAtPlot(x2, y);
	}

	public Plot GetPlotAtPosition(Vector3 Position)
	{
		return GetPlotAtPosition(Position.x, Position.z);
	}

	public void RemoveObject(TileCoordObject Object)
	{
		Plot plot = Object.m_Plot;
		if ((bool)plot)
		{
			if (m_DebugLog)
			{
				Debug.Log(string.Concat("Remove ", Object.m_UniqueID, " type ", Object.m_TypeIdentifier, " from ", Object.m_TileCoord.x, ",", Object.m_TileCoord.y));
			}
			if (m_DebugError && !m_TileObjects[Object.m_TileCoord.GetIndex()].Contains(Object))
			{
				Debug.Log(string.Concat("*** Doesn't exist *** Remove ", Object.m_UniqueID, " type ", Object.m_TypeIdentifier, " from ", Object.m_TileCoord.x, ",", Object.m_TileCoord.y));
			}
			m_TileObjects[Object.m_TileCoord.GetIndex()].Remove(Object);
			plot.RemoveObject(Object);
		}
		else
		{
			if (m_DebugLog)
			{
				Debug.Log("No plot");
			}
			if (m_DebugError && m_TileObjects[Object.m_TileCoord.GetIndex()].Contains(Object))
			{
				Debug.Log(string.Concat("*** Remove ", Object.m_UniqueID, " type ", Object.m_TypeIdentifier, " from ", Object.m_TileCoord.x, ",", Object.m_TileCoord.y, " with no plot ***"));
			}
		}
	}

	public void AddObject(TileCoordObject Object)
	{
		Plot plotAtTile = GetPlotAtTile(Object.m_TileCoord);
		if ((bool)plotAtTile)
		{
			if (m_DebugLog)
			{
				Debug.Log(string.Concat("Add ", Object.m_UniqueID, " type ", Object.m_TypeIdentifier, " to ", Object.m_TileCoord.x, ",", Object.m_TileCoord.y));
			}
			if (m_DebugError && m_TileObjects[Object.m_TileCoord.GetIndex()].Contains(Object))
			{
				if (!m_DebugLog)
				{
					Debug.Log(string.Concat("Add ", Object.m_UniqueID, " type ", Object.m_TypeIdentifier, " to ", Object.m_TileCoord.x, ",", Object.m_TileCoord.y));
				}
				Debug.Log("*** Already exist ***");
			}
			m_TileObjects[Object.m_TileCoord.GetIndex()].Add(Object);
			plotAtTile.AddObject(Object);
		}
		else
		{
			ErrorMessage.LogError("AddObject : Trying to add object outside of world " + Object.transform.position.x + "," + Object.transform.position.z);
		}
	}

	public void UpdateObject(TileCoordObject Object, TileCoord NewCoord)
	{
		Plot plotAtTile = GetPlotAtTile(NewCoord);
		if (Object.m_Plot != plotAtTile)
		{
			RemoveObject(Object);
			Object.m_TileCoord = NewCoord;
			if ((bool)plotAtTile)
			{
				if (m_DebugLog)
				{
					Debug.Log(string.Concat("Update Add ", Object.m_UniqueID, " type ", Object.m_TypeIdentifier, " to ", NewCoord.x, ",", NewCoord.y));
				}
				if (m_DebugError && m_TileObjects[NewCoord.GetIndex()].Contains(Object))
				{
					if (!m_DebugLog)
					{
						Debug.Log(string.Concat("Update Add ", Object.m_UniqueID, " type ", Object.m_TypeIdentifier, " to ", NewCoord.x, ",", NewCoord.y));
					}
					Debug.Log("*** Already exist ***");
				}
				m_TileObjects[NewCoord.GetIndex()].Add(Object);
				plotAtTile.AddObject(Object);
			}
			else
			{
				ErrorMessage.LogError(string.Concat("UpdateObject : Trying to add object ", Object.m_UniqueID, " type ", Object.m_TypeIdentifier, " outside of world ", NewCoord.x, ",", NewCoord.y));
			}
			return;
		}
		if ((bool)Object.m_Plot)
		{
			if (m_DebugLog)
			{
				Debug.Log(string.Concat("Update Remove ", Object.m_UniqueID, " type ", Object.m_TypeIdentifier, " from ", Object.m_TileCoord.x, ",", Object.m_TileCoord.y));
			}
			if (m_DebugError && !m_TileObjects[Object.m_TileCoord.GetIndex()].Contains(Object))
			{
				Debug.Log(string.Concat("*** Doesn't exist *** Update Remove ", Object.m_UniqueID, " type ", Object.m_TypeIdentifier, " from ", Object.m_TileCoord.x, ",", Object.m_TileCoord.y));
			}
			m_TileObjects[Object.m_TileCoord.GetIndex()].Remove(Object);
		}
		else if (m_DebugError && m_TileObjects[Object.m_TileCoord.GetIndex()].Contains(Object))
		{
			ErrorMessage.LogError(string.Concat("*** Update Remove ", Object.m_UniqueID, " type ", Object.m_TypeIdentifier, " from ", Object.m_TileCoord.x, ",", Object.m_TileCoord.y, " with no plot ***"));
		}
		Object.m_TileCoord = NewCoord;
		if (m_DebugLog)
		{
			Debug.Log(string.Concat("Update Add ", Object.m_UniqueID, " type ", Object.m_TypeIdentifier, " to ", NewCoord.x, ",", NewCoord.y));
		}
		if (m_DebugError && m_TileObjects[NewCoord.GetIndex()].Contains(Object))
		{
			if (!m_DebugLog)
			{
				Debug.Log(string.Concat("Update Add ", Object.m_UniqueID, " type ", Object.m_TypeIdentifier, " to ", NewCoord.x, ",", NewCoord.y));
			}
			Debug.Log("*** Already exist ***");
		}
		m_TileObjects[NewCoord.GetIndex()].Add(Object);
	}

	public Selectable GetSelectableObjectAtTile(TileCoord NewPosition, Actionable ExcludedObject = null, bool ExcludeBuildings = false)
	{
		int num = NewPosition.y * TileManager.Instance.m_TilesWide + NewPosition.x;
		List<TileCoordObject> obj = m_TileObjects[num];
		Selectable selectable = null;
		foreach (TileCoordObject item in obj)
		{
			if (Selectable.GetIsSelectable(item) && item != ExcludedObject && (!ExcludeBuildings || item.GetComponent<Building>() == null))
			{
				if ((bool)item.GetComponent<Farmer>())
				{
					return item.GetComponent<Selectable>();
				}
				if (selectable == null)
				{
					selectable = item.GetComponent<Selectable>();
				}
			}
		}
		return selectable;
	}

	public List<TileCoordObject> GetObjectsAtTile(TileCoord NewPosition)
	{
		if (TileManager.Instance.GetTile(NewPosition) == null)
		{
			return null;
		}
		int index = NewPosition.GetIndex();
		return m_TileObjects[index];
	}

	public List<TileCoordObject> GetObjectsAtTile(TileCoord NewPosition, ObjectType NewType)
	{
		return GetPlotAtTile(NewPosition).GetObjectsTypeAtTile(NewType, NewPosition);
	}

	public void GetArea(TileCoord Position, int Radius, out TileCoord TopLeft, out TileCoord BottomRight)
	{
		TopLeft = Position - new TileCoord(Radius, Radius);
		BottomRight = Position + new TileCoord(Radius, Radius);
		if (TopLeft.x < 0)
		{
			TopLeft.x = 0;
		}
		if (TopLeft.y < 0)
		{
			TopLeft.y = 0;
		}
		if (BottomRight.x >= TileManager.Instance.m_TilesWide)
		{
			BottomRight.x = TileManager.Instance.m_TilesWide - 1;
		}
		if (BottomRight.y >= TileManager.Instance.m_TilesHigh)
		{
			BottomRight.y = TileManager.Instance.m_TilesHigh - 1;
		}
		TopLeft.x /= Plot.m_PlotTilesWide;
		TopLeft.y /= Plot.m_PlotTilesHigh;
		BottomRight.x /= Plot.m_PlotTilesWide;
		BottomRight.y /= Plot.m_PlotTilesHigh;
	}

	public void GetArea(TileCoord TopLeftIn, TileCoord BottomRightIn, out TileCoord TopLeft, out TileCoord BottomRight)
	{
		TopLeft = new TileCoord(TopLeftIn.x / Plot.m_PlotTilesWide, TopLeftIn.y / Plot.m_PlotTilesHigh);
		BottomRight = new TileCoord(BottomRightIn.x / Plot.m_PlotTilesWide, BottomRightIn.y / Plot.m_PlotTilesHigh);
	}

	public List<TileCoordObject> GetNearestObjectOfType(ObjectType NewType, TileCoord TopLeftTile, TileCoord BottomRightTile, Actionable Requester, ObjectType UseObjectType, AFO.AT NewActionType, string ActionRequirement, bool IgnorePlotVisibility = false, bool IgnoreAction = false)
	{
		List<TileCoordObject> list = new List<TileCoordObject>();
		Holdable holdable = null;
		if ((bool)Requester && (Requester.m_TypeIdentifier == ObjectType.FarmerPlayer || Requester.m_TypeIdentifier == ObjectType.Worker))
		{
			Holdable topObject = Requester.GetComponent<Farmer>().m_FarmerCarry.GetTopObject();
			if ((bool)topObject)
			{
				if (topObject.m_TypeIdentifier == UseObjectType)
				{
					holdable = topObject;
				}
				else if (MyTool.GetType(topObject.m_TypeIdentifier) != MyTool.Type.Total && MyTool.GetType(topObject.m_TypeIdentifier) == MyTool.GetType(UseObjectType))
				{
					holdable = topObject;
				}
				else if (ObjectTypeList.Instance.GetUseTypeFromType(topObject.m_TypeIdentifier) == ObjectTypeList.Instance.GetUseTypeFromType(UseObjectType))
				{
					holdable = topObject;
					if (holdable != null)
					{
						UseObjectType = topObject.m_TypeIdentifier;
					}
				}
			}
		}
		Actionable.m_ReusableActionFromObject.Init(holdable, UseObjectType, Requester, NewActionType, ActionRequirement, default(TileCoord));
		GetArea(TopLeftTile, BottomRightTile, out var TopLeft, out var BottomRight);
		TileCoord tileCoord = Requester.GetComponent<TileCoordObject>().m_TileCoord;
		TileCoordObject tileCoordObject = null;
		float num = 1E+12f;
		for (int i = TopLeft.y; i <= BottomRight.y; i++)
		{
			for (int j = TopLeft.x; j <= BottomRight.x; j++)
			{
				Plot plotAtPlot = GetPlotAtPlot(j, i);
				if (!(plotAtPlot.m_Visible || IgnorePlotVisibility) || !plotAtPlot.m_ObjectDictionary.ContainsKey(NewType))
				{
					continue;
				}
				List<TileCoordObject> list2 = plotAtPlot.m_ObjectDictionary[NewType];
				bool flag = false;
				if (list2.Count > 0 && ObjectTypeList.Instance.GetIsBuilding(list2[0].m_TypeIdentifier))
				{
					flag = true;
				}
				foreach (TileCoordObject item in list2)
				{
					Building building = null;
					if (flag)
					{
						building = item.GetComponent<Building>();
					}
					if (!item.GetIsSavable() && (!flag || !building.CanBeSearchedFor()))
					{
						continue;
					}
					TileCoord tileCoord2 = item.m_TileCoord;
					if (tileCoord2.x < TopLeftTile.x || tileCoord2.x > BottomRightTile.x || tileCoord2.y < TopLeftTile.y || tileCoord2.y > BottomRightTile.y)
					{
						continue;
					}
					float num2 = (tileCoord - tileCoord2).MagnitudeSqr();
					if (!(num2 < num))
					{
						continue;
					}
					ActionType actionFromObjectSafe = item.GetActionFromObjectSafe(Actionable.m_ReusableActionFromObject);
					if (!((actionFromObjectSafe != ActionType.Total && actionFromObjectSafe != ActionType.Fail) || IgnoreAction))
					{
						continue;
					}
					Building building2 = TileManager.Instance.GetTile(item.m_TileCoord).m_Building;
					if ((!flag && !(building2 == null) && !Building.GetIsTypeWalkable(building2.m_TypeIdentifier) && !Door.GetIsTypeDoor(building2.m_TypeIdentifier) && building2.m_TypeIdentifier != ObjectType.ConverterFoundation) || !(!BaggedManager.Instance.IsObjectBagged(item) || IgnoreAction) || SpawnAnimationManager.Instance.GetIsObjectSpawning(item) || !(item != Requester))
					{
						continue;
					}
					TileCoordObject tileCoordObject2 = item;
					if (flag && building.m_ParentBuilding != null)
					{
						tileCoordObject2 = building.m_ParentBuilding;
						if (NewType == ObjectType.ConverterFoundation)
						{
							tileCoordObject2 = building.GetTopFoundation();
						}
					}
					tileCoordObject = tileCoordObject2;
					num = num2;
				}
			}
		}
		if ((bool)tileCoordObject)
		{
			list.Add(tileCoordObject);
		}
		return list;
	}

	public List<TileCoordObject> GetObjectsOfTypes(ObjectType NewType, TileCoord TopLeftTile, TileCoord BottomRightTile, Actionable Requester, ObjectType UseObjectType, AFO.AT NewActionType, string ActionRequirement, bool IgnorePlotVisibility = false, List<TileCoordObject> OldObjects = null, bool CheckBagged = true)
	{
		List<TileCoordObject> list = new List<TileCoordObject>();
		Holdable holdable = null;
		if ((bool)Requester && (Requester.m_TypeIdentifier == ObjectType.FarmerPlayer || Requester.m_TypeIdentifier == ObjectType.Worker))
		{
			Holdable topObject = Requester.GetComponent<Farmer>().m_FarmerCarry.GetTopObject();
			if ((bool)topObject)
			{
				if (topObject.m_TypeIdentifier == UseObjectType)
				{
					holdable = topObject;
				}
				else if (MyTool.GetType(topObject.m_TypeIdentifier) != MyTool.Type.Total && MyTool.GetType(topObject.m_TypeIdentifier) == MyTool.GetType(UseObjectType))
				{
					holdable = topObject;
				}
				else if (ObjectTypeList.Instance.GetUseTypeFromType(topObject.m_TypeIdentifier) == ObjectTypeList.Instance.GetUseTypeFromType(UseObjectType))
				{
					holdable = topObject;
					if (holdable != null)
					{
						UseObjectType = topObject.m_TypeIdentifier;
					}
				}
			}
		}
		Actionable.m_ReusableActionFromObject.Init(holdable, UseObjectType, Requester, NewActionType, ActionRequirement, default(TileCoord));
		GetArea(TopLeftTile, BottomRightTile, out var TopLeft, out var BottomRight);
		for (int i = TopLeft.y; i <= BottomRight.y; i++)
		{
			for (int j = TopLeft.x; j <= BottomRight.x; j++)
			{
				Plot plotAtPlot = GetPlotAtPlot(j, i);
				if (!(plotAtPlot.m_Visible || IgnorePlotVisibility) || !plotAtPlot.m_ObjectDictionary.ContainsKey(NewType))
				{
					continue;
				}
				List<TileCoordObject> list2 = plotAtPlot.m_ObjectDictionary[NewType];
				bool flag = false;
				if (list2.Count > 0 && ObjectTypeList.Instance.GetIsBuilding(list2[0].m_TypeIdentifier))
				{
					flag = true;
				}
				foreach (TileCoordObject item2 in list2)
				{
					Building building = null;
					if (flag)
					{
						building = item2.GetComponent<Building>();
					}
					if (!item2.GetIsSavable() && (!flag || !building.CanBeSearchedFor()))
					{
						continue;
					}
					TileCoord tileCoord = item2.m_TileCoord;
					if (tileCoord.x < TopLeftTile.x || tileCoord.x > BottomRightTile.x || tileCoord.y < TopLeftTile.y || tileCoord.y > BottomRightTile.y)
					{
						continue;
					}
					ActionType actionFromObjectSafe = item2.GetActionFromObjectSafe(Actionable.m_ReusableActionFromObject);
					if (actionFromObjectSafe == ActionType.Total || actionFromObjectSafe == ActionType.Fail)
					{
						continue;
					}
					Building building2 = TileManager.Instance.GetTile(item2.m_TileCoord).m_Building;
					if ((!flag && !(building2 == null) && !Building.GetIsTypeWalkable(building2.m_TypeIdentifier) && !Door.GetIsTypeDoor(building2.m_TypeIdentifier) && building2.m_TypeIdentifier != ObjectType.ConverterFoundation) || (CheckBagged && BaggedManager.Instance.IsObjectBagged(item2)) || SpawnAnimationManager.Instance.GetIsObjectSpawning(item2) || !(item2 != Requester))
					{
						continue;
					}
					TileCoordObject item = item2;
					if (flag && building.m_ParentBuilding != null)
					{
						item = building.m_ParentBuilding;
						if (NewType == ObjectType.ConverterFoundation)
						{
							item = building.GetTopFoundation();
						}
						if ((OldObjects != null && OldObjects.Contains(item)) || (OldObjects == null && list.Contains(item)))
						{
							continue;
						}
					}
					if (OldObjects != null)
					{
						OldObjects.Add(item);
					}
					else
					{
						list.Add(item);
					}
				}
			}
		}
		return list;
	}

	public List<TileCoordObject> GetObjectsOfType(ObjectType Type, TileCoord TopLeft, TileCoord BottomRight, Actionable Requester, ObjectType UseObjectType, AFO.AT NewActionType, string ActionRequirement, bool IgnorePlotVisibility = false, bool CheckBagged = true)
	{
		return GetObjectsOfTypes(Type, TopLeft, BottomRight, Requester, UseObjectType, NewActionType, ActionRequirement, IgnorePlotVisibility, null, CheckBagged);
	}

	public List<TileCoordObject> GetObjectsOfTypes(List<ObjectType> Types, TileCoord TopLeftTile, TileCoord BottomRightTile, Actionable Requester, ObjectType UseObjectType, AFO.AT NewActionType, string ActionRequirement, bool IgnorePlotVisibility = false, List<TileCoordObject> OldObjects = null, bool CheckBagged = true)
	{
		List<TileCoordObject> list = new List<TileCoordObject>();
		foreach (ObjectType Type in Types)
		{
			GetObjectsOfTypes(Type, TopLeftTile, BottomRightTile, Requester, UseObjectType, NewActionType, ActionRequirement, IgnorePlotVisibility, list, CheckBagged);
		}
		return list;
	}

	public List<TileCoordObject> GetObjectsOfTypes(List<ObjectType> Types, TileCoord Position, int Radius, Actionable Requester, ObjectType UseObjectType, AFO.AT NewActionType, string ActionRequirement, bool IgnorePlotVisibility = false, bool CheckBagged = true)
	{
		TileHelpers.GetClippedTileCoordArea(Position + new TileCoord(-Radius, -Radius), Position + new TileCoord(Radius, Radius), out var TopLeft, out var BottomRight);
		List<TileCoordObject> list = new List<TileCoordObject>();
		foreach (ObjectType Type in Types)
		{
			GetObjectsOfTypes(Type, TopLeft, BottomRight, Requester, UseObjectType, NewActionType, ActionRequirement, IgnorePlotVisibility, list, CheckBagged);
		}
		return list;
	}

	public List<TileCoordObject> GetNearestObjectOfType(ObjectType Type, TileCoord Position, int Radius, Actionable Requester, ObjectType UseObjectType, AFO.AT NewActionType, string ActionRequirement, bool IgnorePlotVisibility = false, bool CheckBagged = true, bool IgnoreAction = false)
	{
		TileHelpers.GetClippedTileCoordArea(Position + new TileCoord(-Radius, -Radius), Position + new TileCoord(Radius, Radius), out var TopLeft, out var BottomRight);
		return GetNearestObjectOfType(Type, TopLeft, BottomRight, Requester, UseObjectType, NewActionType, ActionRequirement, IgnorePlotVisibility, IgnoreAction);
	}

	public List<TileCoordObject> GetObjectsOfType(ObjectType Type, TileCoord Position, int Radius, Actionable Requester, ObjectType UseObjectType, AFO.AT NewActionType, string ActionRequirement, bool IgnorePlotVisibility = false, bool CheckBagged = true)
	{
		TileHelpers.GetClippedTileCoordArea(Position + new TileCoord(-Radius, -Radius), Position + new TileCoord(Radius, Radius), out var TopLeft, out var BottomRight);
		return GetObjectsOfTypes(Type, TopLeft, BottomRight, Requester, UseObjectType, NewActionType, ActionRequirement, IgnorePlotVisibility, null, CheckBagged);
	}

	public List<TileCoordObject> GetNearestObjectForBroom(TileCoord TopLeftTile, TileCoord BottomRightTile, Actionable Requester, ObjectType UseObjectType, AFO.AT NewActionType, string ActionRequirement, bool IgnorePlotVisibility = false, bool IgnoreAction = false)
	{
		List<TileCoordObject> list = new List<TileCoordObject>();
		Holdable holdable = null;
		if ((bool)Requester && (Requester.m_TypeIdentifier == ObjectType.FarmerPlayer || Requester.m_TypeIdentifier == ObjectType.Worker))
		{
			Holdable topObject = Requester.GetComponent<Farmer>().m_FarmerCarry.GetTopObject();
			if ((bool)topObject)
			{
				if (topObject.m_TypeIdentifier == UseObjectType)
				{
					holdable = topObject;
				}
				else if (MyTool.GetType(topObject.m_TypeIdentifier) != MyTool.Type.Total && MyTool.GetType(topObject.m_TypeIdentifier) == MyTool.GetType(UseObjectType))
				{
					holdable = topObject;
				}
				else if (ObjectTypeList.Instance.GetUseTypeFromType(topObject.m_TypeIdentifier) == ObjectTypeList.Instance.GetUseTypeFromType(UseObjectType))
				{
					holdable = topObject;
					if (holdable != null)
					{
						UseObjectType = topObject.m_TypeIdentifier;
					}
				}
			}
		}
		Actionable.m_ReusableActionFromObject.Init(holdable, UseObjectType, Requester, NewActionType, ActionRequirement, default(TileCoord));
		GetArea(TopLeftTile, BottomRightTile, out var TopLeft, out var BottomRight);
		TileCoord tileCoord = Requester.GetComponent<TileCoordObject>().m_TileCoord;
		TileCoordObject tileCoordObject = null;
		float num = 1E+12f;
		for (int i = TopLeft.y; i <= BottomRight.y; i++)
		{
			for (int j = TopLeft.x; j <= BottomRight.x; j++)
			{
				Plot plotAtPlot = GetPlotAtPlot(j, i);
				if (!(plotAtPlot.m_Visible || IgnorePlotVisibility))
				{
					continue;
				}
				foreach (KeyValuePair<ObjectType, List<TileCoordObject>> item in plotAtPlot.m_ObjectDictionary)
				{
					if (!ToolBroom.GetIsObjectTypeAcceptable(item.Key))
					{
						continue;
					}
					foreach (TileCoordObject item2 in item.Value)
					{
						if (!item2.GetIsSavable())
						{
							continue;
						}
						TileCoord tileCoord2 = item2.m_TileCoord;
						if (tileCoord2.x < TopLeftTile.x || tileCoord2.x > BottomRightTile.x || tileCoord2.y < TopLeftTile.y || tileCoord2.y > BottomRightTile.y)
						{
							continue;
						}
						float num2 = (tileCoord - tileCoord2).MagnitudeSqr();
						if (!(num2 < num))
						{
							continue;
						}
						ActionType actionFromObjectSafe = item2.GetActionFromObjectSafe(Actionable.m_ReusableActionFromObject);
						if ((actionFromObjectSafe != ActionType.Total && actionFromObjectSafe != ActionType.Fail) || IgnoreAction)
						{
							Building building = TileManager.Instance.GetTile(item2.m_TileCoord).m_Building;
							if ((building == null || Building.GetIsTypeWalkable(building.m_TypeIdentifier) || Door.GetIsTypeDoor(building.m_TypeIdentifier) || building.m_TypeIdentifier == ObjectType.ConverterFoundation) && (!BaggedManager.Instance.IsObjectBagged(item2) || IgnoreAction) && !SpawnAnimationManager.Instance.GetIsObjectSpawning(item2) && item2 != Requester)
							{
								tileCoordObject = item2;
								num = num2;
							}
						}
					}
				}
			}
		}
		if ((bool)tileCoordObject)
		{
			list.Add(tileCoordObject);
		}
		return list;
	}

	public List<TileCoordObject> GetObjectsForBroom(TileCoord TopLeftTile, TileCoord BottomRightTile, Actionable Requester, ObjectType UseObjectType, AFO.AT NewActionType, string ActionRequirement, bool IgnorePlotVisibility = false, List<TileCoordObject> OldObjects = null, bool CheckBagged = true)
	{
		List<TileCoordObject> list = new List<TileCoordObject>();
		Holdable holdable = null;
		if ((bool)Requester && (Requester.m_TypeIdentifier == ObjectType.FarmerPlayer || Requester.m_TypeIdentifier == ObjectType.Worker))
		{
			Holdable topObject = Requester.GetComponent<Farmer>().m_FarmerCarry.GetTopObject();
			if ((bool)topObject)
			{
				if (topObject.m_TypeIdentifier == UseObjectType)
				{
					holdable = topObject;
				}
				else if (MyTool.GetType(topObject.m_TypeIdentifier) != MyTool.Type.Total && MyTool.GetType(topObject.m_TypeIdentifier) == MyTool.GetType(UseObjectType))
				{
					holdable = topObject;
				}
				else if (ObjectTypeList.Instance.GetUseTypeFromType(topObject.m_TypeIdentifier) == ObjectTypeList.Instance.GetUseTypeFromType(UseObjectType))
				{
					holdable = topObject;
					if (holdable != null)
					{
						UseObjectType = topObject.m_TypeIdentifier;
					}
				}
			}
		}
		Actionable.m_ReusableActionFromObject.Init(holdable, UseObjectType, Requester, NewActionType, ActionRequirement, default(TileCoord));
		GetArea(TopLeftTile, BottomRightTile, out var TopLeft, out var BottomRight);
		for (int i = TopLeft.y; i <= BottomRight.y; i++)
		{
			for (int j = TopLeft.x; j <= BottomRight.x; j++)
			{
				Plot plotAtPlot = GetPlotAtPlot(j, i);
				if (!(plotAtPlot.m_Visible || IgnorePlotVisibility))
				{
					continue;
				}
				foreach (KeyValuePair<ObjectType, List<TileCoordObject>> item2 in plotAtPlot.m_ObjectDictionary)
				{
					if (!ToolBroom.GetIsObjectTypeAcceptable(item2.Key))
					{
						continue;
					}
					foreach (TileCoordObject item3 in item2.Value)
					{
						if (!item3.GetIsSavable())
						{
							continue;
						}
						TileCoord tileCoord = item3.m_TileCoord;
						if (tileCoord.x < TopLeftTile.x || tileCoord.x > BottomRightTile.x || tileCoord.y < TopLeftTile.y || tileCoord.y > BottomRightTile.y)
						{
							continue;
						}
						ActionType actionFromObjectSafe = item3.GetActionFromObjectSafe(Actionable.m_ReusableActionFromObject);
						if (actionFromObjectSafe == ActionType.Total || actionFromObjectSafe == ActionType.Fail)
						{
							continue;
						}
						Building building = TileManager.Instance.GetTile(item3.m_TileCoord).m_Building;
						if ((building == null || Building.GetIsTypeWalkable(building.m_TypeIdentifier) || Door.GetIsTypeDoor(building.m_TypeIdentifier) || building.m_TypeIdentifier == ObjectType.ConverterFoundation) && (!CheckBagged || !BaggedManager.Instance.IsObjectBagged(item3)) && !SpawnAnimationManager.Instance.GetIsObjectSpawning(item3) && item3 != Requester)
						{
							TileCoordObject item = item3;
							if (OldObjects != null)
							{
								OldObjects.Add(item);
							}
							else
							{
								list.Add(item);
							}
						}
					}
				}
			}
		}
		return list;
	}

	public List<TileCoordObject> GetObjectsInArea(ObjectType Type, TileCoord TopLeftTile, TileCoord BottomRightTile, bool IgnorePlotVisibility = false, bool CheckBagged = true, bool BypassBuildings = true)
	{
		List<TileCoordObject> list = new List<TileCoordObject>();
		GetArea(TopLeftTile, BottomRightTile, out var TopLeft, out var BottomRight);
		for (int i = TopLeft.y; i <= BottomRight.y; i++)
		{
			for (int j = TopLeft.x; j <= BottomRight.x; j++)
			{
				Plot plotAtPlot = GetPlotAtPlot(j, i);
				if (!(plotAtPlot.m_Visible || IgnorePlotVisibility))
				{
					continue;
				}
				foreach (KeyValuePair<ObjectType, List<TileCoordObject>> item in plotAtPlot.m_ObjectDictionary)
				{
					foreach (TileCoordObject item2 in item.Value)
					{
						if (!item2.GetIsSavable())
						{
							continue;
						}
						TileCoord tileCoord = item2.m_TileCoord;
						if (tileCoord.x >= TopLeftTile.x && tileCoord.x <= BottomRightTile.x && tileCoord.y >= TopLeftTile.y && tileCoord.y <= BottomRightTile.y)
						{
							Building building = TileManager.Instance.GetTile(item2.m_TileCoord).m_Building;
							if ((building == null || Building.GetIsTypeWalkable(building.m_TypeIdentifier) || Door.GetIsTypeDoor(building.m_TypeIdentifier) || BypassBuildings || building.m_TypeIdentifier == ObjectType.ConverterFoundation) && (!CheckBagged || !BaggedManager.Instance.IsObjectBagged(item2)) && !SpawnAnimationManager.Instance.GetIsObjectSpawning(item2) && Type == item2.m_TypeIdentifier)
							{
								list.Add(item2);
							}
						}
					}
				}
			}
		}
		return list;
	}

	public List<TileCoordObject> GetBuildingsInArea(ObjectType Type, TileCoord TopLeftTile, TileCoord BottomRightTile, bool IgnorePlotVisibility = false, bool CheckBagged = true)
	{
		List<TileCoordObject> list = new List<TileCoordObject>();
		if (!ObjectTypeList.Instance.GetIsBuilding(Type))
		{
			return list;
		}
		GetArea(TopLeftTile, BottomRightTile, out var TopLeft, out var BottomRight);
		for (int i = TopLeft.y; i <= BottomRight.y; i++)
		{
			for (int j = TopLeft.x; j <= BottomRight.x; j++)
			{
				Plot plotAtPlot = GetPlotAtPlot(j, i);
				if (!(plotAtPlot.m_Visible || IgnorePlotVisibility) || !plotAtPlot.m_ObjectDictionary.ContainsKey(Type))
				{
					continue;
				}
				List<TileCoordObject> list2 = plotAtPlot.m_ObjectDictionary[Type];
				bool flag = false;
				if (list2.Count > 0 && ObjectTypeList.Instance.GetIsBuilding(list2[0].m_TypeIdentifier))
				{
					flag = true;
				}
				foreach (TileCoordObject item in list2)
				{
					Building building = null;
					if (flag)
					{
						building = item.GetComponent<Building>();
					}
					if (item.GetIsSavable() || (flag && building.CanBeSearchedFor()))
					{
						TileCoord tileCoord = item.m_TileCoord;
						if (tileCoord.x >= TopLeftTile.x && tileCoord.x <= BottomRightTile.x && tileCoord.y >= TopLeftTile.y && tileCoord.y <= BottomRightTile.y && Type == item.m_TypeIdentifier)
						{
							list.Add(item);
						}
					}
				}
			}
		}
		return list;
	}

	public void TileHeightChanged(TileCoord Position)
	{
		Plot plotAtTile = GetPlotAtTile(Position);
		plotAtTile.StackObjectsAtTile(Position);
		if (plotAtTile.m_ObjectDictionary.ContainsKey(ObjectType.Worker))
		{
			foreach (TileCoordObject item in plotAtTile.m_ObjectDictionary[ObjectType.Worker])
			{
				if (item.m_TileCoord == Position)
				{
					item.UpdatePositionToTilePosition(Position);
				}
			}
		}
		if (!plotAtTile.m_ObjectDictionary.ContainsKey(ObjectType.FarmerPlayer))
		{
			return;
		}
		foreach (TileCoordObject item2 in plotAtTile.m_ObjectDictionary[ObjectType.FarmerPlayer])
		{
			if (item2.m_TileCoord == Position)
			{
				item2.UpdatePositionToTilePosition(Position);
			}
		}
	}

	public void FinishMerge()
	{
		Plot[] plots = m_Plots;
		for (int i = 0; i < plots.Length; i++)
		{
			plots[i].FinishMerge();
		}
	}

	private void Update()
	{
		UpdateClouds();
	}

	public void SetDesaturation(bool Desaturated)
	{
		float value = 1f;
		if (Desaturated)
		{
			value = 0f;
		}
		Shader.SetGlobalFloat("_TileMapSaturation", value);
		Plot[] plots = m_Plots;
		for (int i = 0; i < plots.Length; i++)
		{
			plots[i].SetDesaturation(Desaturated);
		}
	}
}
