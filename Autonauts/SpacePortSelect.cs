using System.Collections.Generic;
using UnityEngine;

public class SpacePortSelect : BuildingSelect
{
	private SpacePortMission m_DefaultPrefab;

	private BaseScrollView m_ScrollView;

	private List<SpacePortMission> m_Missions;

	private Vector2 m_StartPosition;

	private float m_Height;

	private Transform m_Parent;

	private SpacePortMission m_TempMission;

	private void OnDestroy()
	{
		if ((bool)m_Building && (bool)m_Building.m_Engager)
		{
			m_Building.m_Engager.SendAction(new ActionInfo(ActionType.DisengageObject, m_Building.m_Engager.GetComponent<TileCoordObject>().m_TileCoord, m_Building));
		}
	}

	protected new void Start()
	{
		base.Start();
		CreateMissions();
	}

	private void DestroyMissions()
	{
		foreach (SpacePortMission mission in m_Missions)
		{
			Object.Destroy(mission.gameObject);
		}
		m_Missions.Clear();
	}

	private void CreateMissions()
	{
		m_Missions = new List<SpacePortMission>();
		m_ScrollView = base.transform.Find("MissionList/BaseScrollView").GetComponent<BaseScrollView>();
		m_DefaultPrefab = m_ScrollView.GetContent().transform.Find("Mission").GetComponent<SpacePortMission>();
		m_DefaultPrefab.gameObject.SetActive(value: false);
		m_StartPosition = m_DefaultPrefab.GetComponent<RectTransform>().anchoredPosition;
		m_Height = m_DefaultPrefab.GetComponent<RectTransform>().sizeDelta.y + 5f;
		m_Parent = m_DefaultPrefab.transform.parent;
		Vector2 anchoredPosition = m_DefaultPrefab.GetComponent<RectTransform>().anchoredPosition;
		foreach (OffworldMission item in OffworldMissionsManager.Instance.m_RegularMissionsAvailable)
		{
			SpacePortMission spacePortMission = Object.Instantiate(m_DefaultPrefab, m_DefaultPrefab.transform.parent);
			spacePortMission.gameObject.SetActive(value: true);
			spacePortMission.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
			spacePortMission.SetMission(item, this);
			m_Missions.Add(spacePortMission);
			anchoredPosition.y -= m_Height;
		}
		OffworldMission dailyMissionAvailable = OffworldMissionsManager.Instance.m_DailyMissionAvailable;
		if (dailyMissionAvailable != null)
		{
			SpacePortMission spacePortMission2 = Object.Instantiate(m_DefaultPrefab, m_DefaultPrefab.transform.parent);
			spacePortMission2.gameObject.SetActive(value: true);
			spacePortMission2.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
			spacePortMission2.SetMission(dailyMissionAvailable, this);
			m_Missions.Add(spacePortMission2);
		}
	}

	private void UpdateMissions()
	{
		foreach (SpacePortMission mission in m_Missions)
		{
			mission.UpdateMission();
		}
	}

	public void ConfirmAccept()
	{
		OffworldMissionsManager.Instance.SelectMission(m_TempMission.m_Mission);
		UpdateMissions();
		GameStateManager.Instance.PopState();
	}

	public void MissionAccepted(SpacePortMission NewMission)
	{
		OffworldMissionsManager.Instance.SelectMission(NewMission.m_Mission);
		UpdateMissions();
		GameStateManager.Instance.PopState();
	}

	public void ConfirmStop()
	{
		DestroyMissions();
		OffworldMissionsManager.Instance.SelectMission(null);
		CreateMissions();
		AudioManager.Instance.StartEvent("SpacePortJobCancelled");
	}

	public void MissionStopped(SpacePortMission NewMission)
	{
		m_TempMission = NewMission;
		GameStateManager.Instance.PushState(GameStateManager.State.Confirm);
		string description = "ConfirmStopMissionDescription";
		if (NewMission.m_Mission.m_Daily)
		{
			description = ((!OffworldMissionsManager.Instance.GetIsDailyMissionSameDate()) ? "ConfirmDeclineMissionDescription" : "ConfirmStopMissionDailyDescription");
		}
		GameStateManager.Instance.GetCurrentState().GetComponent<GameStateConfirm>().SetConfirm(ConfirmStop, "ConfirmStopMission", description);
	}

	public void ConfirmDecline()
	{
		DestroyMissions();
		OffworldMissionsManager.Instance.DiscardRegularMission(m_TempMission.m_Mission);
		CreateMissions();
		AudioManager.Instance.StartEvent("SpacePortJobDeleted");
	}

	public void MissionDeclined(SpacePortMission NewMission)
	{
		m_TempMission = NewMission;
		GameStateManager.Instance.PushState(GameStateManager.State.Confirm);
		GameStateManager.Instance.GetCurrentState().GetComponent<GameStateConfirm>().SetConfirm(ConfirmDecline, "ConfirmDeclineMission", "ConfirmDeclineMissionDescription");
	}
}
