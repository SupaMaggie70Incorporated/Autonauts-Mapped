using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class AnalyticsManager : MonoBehaviour
{
	public static void WorldCreated(GameOptions NewOptions)
	{
		AnalyticsEvent.Custom("World_Created", new Dictionary<string, object>
		{
			{
				"Mode",
				NewOptions.m_GameMode.ToString()
			},
			{
				"Size",
				NewOptions.m_GameSize.ToString()
			},
			{ "Tutorial", NewOptions.m_TutorialEnabled }
		});
	}

	public static void CertificateComplete(Quest NewQuest)
	{
		if (NewQuest.m_ID == Quest.ID.AcademyBasics)
		{
			AnalyticsEvent.Custom("Tutorial_Complete", new Dictionary<string, object> { 
			{
				"Time",
				TimeManager.Instance.m_TotalRealTime
			} });
		}
		AnalyticsEvent.Custom("Certificate_Complete", new Dictionary<string, object>
		{
			{
				"Name",
				NewQuest.m_ID.ToString()
			},
			{
				"Time",
				TimeManager.Instance.m_TotalRealTime
			}
		});
	}

	public static void ResearchComplete(Quest NewQuest)
	{
		AnalyticsEvent.Custom("Research_Complete", new Dictionary<string, object>
		{
			{
				"Name",
				NewQuest.m_ID.ToString()
			},
			{
				"Time",
				TimeManager.Instance.m_TotalRealTime
			}
		});
	}

	public static void BadgeComplete(Badge NewBadge)
	{
		AnalyticsEvent.Custom("Badge_Complete", new Dictionary<string, object>
		{
			{
				"Name",
				NewBadge.m_ID.ToString()
			},
			{
				"Time",
				TimeManager.Instance.m_TotalRealTime
			}
		});
	}

	public static void OffworldMissionStarted(OffworldMission Complete)
	{
		if (!Complete.m_Daily)
		{
			AnalyticsEvent.Custom("OffworldMission_Regular_Started", new Dictionary<string, object>
			{
				{
					"ID",
					Complete.m_ID.ToString()
				},
				{
					"Tickets",
					Complete.GetTickets().ToString()
				},
				{
					"Time",
					TimeManager.Instance.m_TotalRealTime
				}
			});
		}
		else
		{
			AnalyticsEvent.Custom("OffworldMission_Daily_Started", new Dictionary<string, object>
			{
				{
					"ID",
					Complete.m_ID.ToString()
				},
				{
					"Tickets",
					Complete.GetTickets().ToString()
				},
				{
					"Time",
					TimeManager.Instance.m_TotalRealTime
				}
			});
		}
	}

	public static void OffworldMissionComplete(OffworldMission Complete)
	{
		if (!Complete.m_Daily)
		{
			AnalyticsEvent.Custom("OffworldMission_Regular_Complete", new Dictionary<string, object>
			{
				{
					"ID",
					Complete.m_ID.ToString()
				},
				{
					"Tickets",
					Complete.GetTickets().ToString()
				},
				{
					"Time",
					TimeManager.Instance.m_TotalRealTime
				}
			});
		}
		else
		{
			AnalyticsEvent.Custom("OffworldMission_Daily_Complete", new Dictionary<string, object>
			{
				{
					"ID",
					Complete.m_ID.ToString()
				},
				{
					"Tickets",
					Complete.GetTickets().ToString()
				},
				{
					"Time",
					TimeManager.Instance.m_TotalRealTime
				}
			});
		}
	}
}
