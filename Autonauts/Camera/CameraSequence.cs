using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class CameraSequence : BasePanelOptions
{
	public static CameraSequence Instance;

	public List<CameraSequenceWaypoint> m_Waypoints;

	private BaseScrollView m_ScrollView;

	private CameraSequenceWaypoint m_WaypointPrefab;

	private float m_WaypointPrefabHeight;

	private BaseButton m_PlayButton;

	private void Awake()
	{
		Instance = this;
		m_Waypoints = new List<CameraSequenceWaypoint>();
		m_ScrollView = base.transform.Find("Panel/BaseScrollView").GetComponent<BaseScrollView>();
		m_WaypointPrefab = m_ScrollView.GetContent().transform.Find("CameraSequenceWaypoint").GetComponent<CameraSequenceWaypoint>();
		m_WaypointPrefab.SetActive(Active: false);
		m_WaypointPrefabHeight = m_WaypointPrefab.GetHeight();
		m_PlayButton = base.transform.Find("Panel/PlayButton").GetComponent<BaseButton>();
		m_PlayButton.SetAction(OnPlayClicked, m_PlayButton);
	}

	public void Save(JSONNode Node)
	{
		JSONArray jSONArray = (JSONArray)(Node["Waypoints"] = new JSONArray());
		int num = 0;
		foreach (CameraSequenceWaypoint waypoint in m_Waypoints)
		{
			JSONNode jSONNode2 = new JSONObject();
			JSONUtils.Set(jSONNode2, "x", waypoint.m_Transform.transform.position.x);
			JSONUtils.Set(jSONNode2, "y", waypoint.m_Transform.transform.position.y);
			JSONUtils.Set(jSONNode2, "z", waypoint.m_Transform.transform.position.z);
			JSONUtils.Set(jSONNode2, "rx", waypoint.m_Transform.transform.rotation.eulerAngles.x);
			JSONUtils.Set(jSONNode2, "ry", waypoint.m_Transform.transform.rotation.eulerAngles.y);
			JSONUtils.Set(jSONNode2, "rz", waypoint.m_Transform.transform.rotation.eulerAngles.z);
			JSONUtils.Set(jSONNode2, "t", waypoint.m_Time);
			jSONArray[num] = jSONNode2;
			num++;
		}
	}

	public void Load(JSONNode Node)
	{
		JSONArray asArray = Node["Waypoints"].AsArray;
		for (int i = 0; i < asArray.Count; i++)
		{
			JSONObject asObject = asArray[i].AsObject;
			float asFloat = JSONUtils.GetAsFloat(asObject, "x", 0f);
			float asFloat2 = JSONUtils.GetAsFloat(asObject, "y", 0f);
			float asFloat3 = JSONUtils.GetAsFloat(asObject, "z", 0f);
			float asFloat4 = JSONUtils.GetAsFloat(asObject, "rx", 0f);
			float asFloat5 = JSONUtils.GetAsFloat(asObject, "ry", 0f);
			float asFloat6 = JSONUtils.GetAsFloat(asObject, "rz", 0f);
			float asFloat7 = JSONUtils.GetAsFloat(asObject, "t", 0f);
			Vector3 position = new Vector3(asFloat, asFloat2, asFloat3);
			Quaternion rotation = Quaternion.Euler(asFloat4, asFloat5, asFloat6);
			AddWaypoint(position, rotation, DOF: false, 0f, 0f, 0f, asFloat7);
		}
		Sort();
	}

	private static int SortWaypoints(CameraSequenceWaypoint p1, CameraSequenceWaypoint p2)
	{
		if (p1.m_Time == p2.m_Time)
		{
			return 0;
		}
		if (p1.m_Time < p2.m_Time)
		{
			return -1;
		}
		return 1;
	}

	public void Sort()
	{
		m_Waypoints.Sort(SortWaypoints);
		float num = m_WaypointPrefabHeight + 5f;
		m_ScrollView.SetScrollSize(num * (float)m_Waypoints.Count);
		float num2 = -10f;
		foreach (CameraSequenceWaypoint waypoint in m_Waypoints)
		{
			Vector2 position = waypoint.GetPosition();
			position.y = num2;
			waypoint.SetPosition(position);
			num2 -= num;
		}
		if (m_Waypoints.Count >= 2)
		{
			m_PlayButton.SetInteractable(Interactable: true);
		}
		else
		{
			m_PlayButton.SetInteractable(Interactable: false);
		}
	}

	public void AddWaypoint()
	{
		Transform transform = CameraManager.Instance.m_Camera.transform;
		float time = 0f;
		if (m_Waypoints.Count > 0)
		{
			time = m_Waypoints[m_Waypoints.Count - 1].m_Time + 1f;
		}
		bool dOFEnabled = SettingsManager.Instance.m_DOFEnabled;
		float dOFFocalDistance = SettingsManager.Instance.m_DOFFocalDistance;
		float dOFFocalLength = SettingsManager.Instance.m_DOFFocalLength;
		float dOFAperture = SettingsManager.Instance.m_DOFAperture;
		AddWaypoint(transform.position, transform.rotation, dOFEnabled, dOFFocalDistance, dOFFocalLength, dOFAperture, time);
		Sort();
	}

	public void AddWaypoint(Vector3 Position, Quaternion Rotation, bool DOF, float DOFFocalDistance, float DOFFocalLength, float DOFApeture, float Time)
	{
		CameraSequenceWaypoint component = Object.Instantiate(m_WaypointPrefab, m_WaypointPrefab.transform.parent).GetComponent<CameraSequenceWaypoint>();
		component.SetActive(Active: true);
		component.SetData(Position, Rotation, DOF, DOFFocalDistance, DOFFocalLength, DOFApeture, Time);
		m_Waypoints.Add(component);
	}

	public void DeleteWaypoint(CameraSequenceWaypoint NewWaypoint)
	{
		if (m_Waypoints.Contains(NewWaypoint))
		{
			m_Waypoints.Remove(NewWaypoint);
			Object.Destroy(NewWaypoint.gameObject);
			Sort();
		}
	}

	public void OnPlayClicked(BaseGadget NewGadget)
	{
		GameStateManager.Instance.PushState(GameStateManager.State.PlayCameraSequence);
	}
}
