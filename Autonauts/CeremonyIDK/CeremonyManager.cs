using System.Collections.Generic;
using UnityEngine;

public class CeremonyManager : MonoBehaviour
{
	public enum CeremonyType
	{
		ResearchEnded,
		QuestEnded,
		GlueComplete,
		TutorialComplete,
		TutorialCompleteTop,
		TutorialFreeBot,
		CertificateEnded,
		CertificateAcademyEnded,
		RevealCertificate,
		MainQuestEnded,
		ResearchStarted,
		ResearchLevelUnlocked,
		EraComplete,
		FirstFolk,
		FirstBot,
		FirstTool,
		FirstResearch,
		FolkLevelUp,
		TutorialFinished,
		PhaseOneComplete,
		PhaseTwoComplete,
		PhaseThreeComplete,
		GameComplete,
		SpacePortComplete,
		OffworldMissionComplete,
		OffworldObjectUnlocked,
		BadgeRevealed,
		RocketIntro,
		CommsIntro,
		Go,
		FolkSeedUnlocked,
		Video,
		Total
	}

	public class NewCeremonyData
	{
		public CeremonyType m_Type;

		public Quest m_Quest;

		public ObjectType m_ObjectType;

		public Badge m_Badge;

		public Era m_Era;

		public OffworldMission m_Mission;

		public Vector3 m_Position;

		public int m_TargetUID;

		public string m_StringID;

		public List<ObjectType> m_UnlockedObjects;
	}

	public static CeremonyManager Instance;

	private string[] m_PrefabNames = new string[32]
	{
		"CeremonyResearchEnded", "CeremonyQuestEnded", "", "CeremonyTutorialComplete", "CeremonyTutorialCompleteTop", "CeremonyTutorialFreeBot", "CeremonyCertificateEnded", "CeremonyCertificateAcademyEnded", "CeremonyRevealCertificate", "CeremonyMainQuestEnded",
		"CeremonyResearchStarted", "CeremonyResearchLevelUnlocked", "CeremonyEraComplete", "CeremonyFirstFolk", "CeremonyFirstBot", "CeremonyFirstTool", "CeremonyFirstResearch", "CeremonyFolkLevelUp", "CeremonyTutorialFinished", "CeremonyPhaseOneComplete",
		"CeremonyPhaseTwoComplete", "CeremonyPhaseThreeComplete", "CeremonyGameComplete", "CeremonySpacePortComplete", "CeremonyOffworldMissionComplete", "CeremonyOffworldObjectUnlocked", "CeremonyBadgeRevealed", "CeremonyRocketIntro", "CeremonyCommsIntro", "CeremonyGo",
		"CeremonyFolkSeedUnlocked", "CeremonyVideo"
	};

	private List<NewCeremonyData> m_NewCeremonies;

	private CeremonyBase m_CurrentCeremony;

	public NewCeremonyData m_CurrentCeremonyData;

	public CeremonyType m_CurrentCeremonyType;

	private int m_Delay;

	private float m_CeremonyDelay;

	public bool m_GluePlaying;

	public Quest m_GlueQuest;

	private List<BotMessage> m_BotMessages;

	private void Awake()
	{
		Instance = this;
		m_CurrentCeremony = null;
		m_NewCeremonies = new List<NewCeremonyData>();
		m_BotMessages = new List<BotMessage>();
	}

	public void StartCeremony(NewCeremonyData NewData)
	{
		if (NewData.m_Type == CeremonyType.Total)
		{
			return;
		}
		CeremonyBase ceremonyBase = null;
		string text = m_PrefabNames[(int)NewData.m_Type];
		if (text != "")
		{
			Transform parent = HudManager.Instance.m_CeremoniesRootTransform;
			if (GameStateManager.Instance.GetActualState() == GameStateManager.State.Autopedia)
			{
				parent = HudManager.Instance.m_MenusRootTransform;
			}
			ceremonyBase = Object.Instantiate((GameObject)Resources.Load("Prefabs/Hud/Ceremonies/" + text, typeof(GameObject)), new Vector3(0f, 0f, 0f), Quaternion.identity, parent).GetComponent<CeremonyBase>();
			if ((bool)ceremonyBase.GetComponent<RectTransform>())
			{
				ceremonyBase.GetComponent<RectTransform>().anchoredPosition = default(Vector2);
			}
		}
		CollectionManager.Instance.GetPlayers();
		m_CurrentCeremonyType = NewData.m_Type;
		switch (NewData.m_Type)
		{
		case CeremonyType.RocketIntro:
			GameStateManager.Instance.PushState(GameStateManager.State.Ceremony);
			break;
		case CeremonyType.CommsIntro:
			GameStateManager.Instance.PushState(GameStateManager.State.Ceremony);
			break;
		case CeremonyType.Go:
			GameStateManager.Instance.PushState(GameStateManager.State.Ceremony);
			break;
		case CeremonyType.GlueComplete:
			m_GluePlaying = true;
			m_CurrentCeremony = null;
			m_GlueQuest = NewData.m_Quest;
			CheckForNewCeremonies();
			return;
		case CeremonyType.TutorialFreeBot:
			ceremonyBase.GetComponent<CeremonyTutorialFreeBot>().SetQuest(NewData.m_Quest);
			GameStateManager.Instance.PushState(GameStateManager.State.Ceremony);
			break;
		case CeremonyType.TutorialComplete:
			ceremonyBase.GetComponent<CeremonyTutorialComplete>().SetQuest(NewData.m_Quest);
			GameStateManager.Instance.PushState(GameStateManager.State.Ceremony);
			break;
		case CeremonyType.TutorialCompleteTop:
			ceremonyBase.GetComponent<CeremonyTutorialComplete>().SetQuest(NewData.m_Quest);
			GameStateManager.Instance.PushState(GameStateManager.State.Ceremony);
			if ((bool)Academy.Instance)
			{
				Academy.Instance.GetCertificateFromLesson(NewData.m_Quest).UpdateLesson();
			}
			break;
		case CeremonyType.ResearchStarted:
			GameStateManager.Instance.PushState(GameStateManager.State.Ceremony);
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateCeremony>().SetBlur();
			ceremonyBase.GetComponent<CeremonyResearchStarted>().SetQuest(NewData.m_Quest, NewData.m_UnlockedObjects);
			break;
		case CeremonyType.ResearchEnded:
			ceremonyBase.GetComponent<CeremonyResearchEnded>().SetQuest(NewData.m_Quest, NewData.m_UnlockedObjects);
			GameStateManager.Instance.PushState(GameStateManager.State.Ceremony);
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateCeremony>().SetBlur();
			break;
		case CeremonyType.ResearchLevelUnlocked:
			GameStateManager.Instance.PushState(GameStateManager.State.Autopedia);
			ceremonyBase.transform.SetParent(HudManager.Instance.m_MenusRootTransform);
			Autopedia.Instance.SetPage(Autopedia.Page.Research);
			GameStateAutopedia.Instance.CeremonyPlaying(Playing: true, NewData.m_Quest);
			GameStateManager.Instance.PushState(GameStateManager.State.Ceremony);
			ceremonyBase.GetComponent<CeremonyResearchLevelUnlocked>().SetQuest(NewData.m_Quest, NewData.m_UnlockedObjects);
			break;
		case CeremonyType.QuestEnded:
			ceremonyBase.GetComponent<CeremonyQuestEnded>().SetQuest(NewData.m_Quest, NewData.m_UnlockedObjects);
			GameStateManager.Instance.PushState(GameStateManager.State.Ceremony);
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateCeremony>().SetBlur();
			break;
		case CeremonyType.CertificateEnded:
			ceremonyBase.GetComponent<CeremonyCertificateEnded>().SetQuest(NewData.m_Quest, NewData.m_UnlockedObjects);
			GameStateManager.Instance.PushState(GameStateManager.State.Ceremony);
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateCeremony>().SetBlur();
			break;
		case CeremonyType.CertificateAcademyEnded:
			ceremonyBase.GetComponent<CeremonyCertificateAcademyEnded>().SetQuest(NewData.m_Quest, NewData.m_UnlockedObjects);
			GameStateManager.Instance.PushState(GameStateManager.State.Ceremony);
			break;
		case CeremonyType.RevealCertificate:
			ceremonyBase.GetComponent<CeremonyRevealCertificate>().SetQuest(NewData.m_Quest, NewData.m_UnlockedObjects);
			GameStateManager.Instance.PushState(GameStateManager.State.Ceremony);
			break;
		case CeremonyType.MainQuestEnded:
			ceremonyBase.GetComponent<CeremonyMainQuestEnded>().SetQuest(NewData.m_Quest, NewData.m_UnlockedObjects);
			GameStateManager.Instance.PushState(GameStateManager.State.Ceremony);
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateCeremony>().SetBlur();
			break;
		case CeremonyType.BadgeRevealed:
			ceremonyBase.GetComponent<CeremonyBadgeRevealed>().SetBadge(NewData.m_Badge);
			GameStateManager.Instance.PushState(GameStateManager.State.Ceremony);
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateCeremony>().SetBlur();
			break;
		case CeremonyType.EraComplete:
			ceremonyBase.GetComponent<CeremonyEraComplete>().SetQuest(NewData.m_Quest, NewData.m_UnlockedObjects);
			GameStateManager.Instance.PushState(GameStateManager.State.Ceremony);
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateCeremony>().SetBlur();
			break;
		case CeremonyType.FirstFolk:
			GameStateManager.Instance.PushState(GameStateManager.State.Ceremony);
			break;
		case CeremonyType.FirstBot:
			GameStateManager.Instance.PushState(GameStateManager.State.Ceremony);
			break;
		case CeremonyType.FirstTool:
			GameStateManager.Instance.PushState(GameStateManager.State.Ceremony);
			break;
		case CeremonyType.FirstResearch:
			GameStateManager.Instance.PushState(GameStateManager.State.Ceremony);
			break;
		case CeremonyType.TutorialFinished:
			GameStateManager.Instance.PushState(GameStateManager.State.Ceremony);
			break;
		case CeremonyType.PhaseOneComplete:
			GameStateManager.Instance.PushState(GameStateManager.State.Ceremony);
			break;
		case CeremonyType.PhaseTwoComplete:
			GameStateManager.Instance.PushState(GameStateManager.State.Ceremony);
			break;
		case CeremonyType.PhaseThreeComplete:
			GameStateManager.Instance.PushState(GameStateManager.State.Ceremony);
			break;
		case CeremonyType.GameComplete:
			GameStateManager.Instance.PushState(GameStateManager.State.Ceremony);
			break;
		case CeremonyType.SpacePortComplete:
			GameStateManager.Instance.PushState(GameStateManager.State.Ceremony);
			break;
		case CeremonyType.OffworldMissionComplete:
			ceremonyBase.GetComponent<CeremonyOffworldMissionComplete>().SetMission(NewData.m_Mission);
			GameStateManager.Instance.PushState(GameStateManager.State.Ceremony);
			break;
		case CeremonyType.OffworldObjectUnlocked:
			ceremonyBase.GetComponent<CeremonyOffworldObjectUnlocked>().SetObjectType(NewData.m_UnlockedObjects[0]);
			GameStateManager.Instance.PushState(GameStateManager.State.Ceremony);
			break;
		case CeremonyType.FolkLevelUp:
			ceremonyBase.GetComponent<CeremonyFolkLevelUp>().SetTarget(NewData.m_TargetUID);
			GameStateManager.Instance.PushState(GameStateManager.State.Ceremony);
			break;
		case CeremonyType.FolkSeedUnlocked:
			GameStateManager.Instance.PushState(GameStateManager.State.Ceremony);
			break;
		case CeremonyType.Video:
			GameStateManager.Instance.PushState(GameStateManager.State.Ceremony);
			break;
		}
		m_CurrentCeremony = ceremonyBase;
		m_CurrentCeremonyData = NewData;
	}

	public void AddCeremony(CeremonyType NewType)
	{
		NewCeremonyData newCeremonyData = new NewCeremonyData();
		newCeremonyData.m_Type = NewType;
		m_NewCeremonies.Add(newCeremonyData);
		if (NewType == CeremonyType.RocketIntro)
		{
			CheckForNewCeremonies();
		}
	}

	public void AddCeremony(CeremonyType NewType, OffworldMission NewMission)
	{
		NewCeremonyData newCeremonyData = new NewCeremonyData();
		newCeremonyData.m_Type = NewType;
		newCeremonyData.m_Mission = NewMission;
		m_NewCeremonies.Add(newCeremonyData);
		m_CeremonyDelay = 1f;
	}

	public void AddCeremony(CeremonyType NewType, Quest NewQuest, List<ObjectType> UnlockedObjects)
	{
		if (GameStateManager.Instance.GetActualState() == GameStateManager.State.Autopedia)
		{
			StartImmediateCeremony(NewType, NewQuest, UnlockedObjects);
			return;
		}
		NewCeremonyData newCeremonyData = new NewCeremonyData();
		newCeremonyData.m_Type = NewType;
		newCeremonyData.m_Quest = NewQuest;
		newCeremonyData.m_UnlockedObjects = UnlockedObjects;
		m_NewCeremonies.Add(newCeremonyData);
		m_CeremonyDelay = 1f;
	}

	public void AddCeremony(CeremonyType NewType, ObjectType NewObjectType, Quest NewQuest)
	{
		NewCeremonyData newCeremonyData = new NewCeremonyData();
		newCeremonyData.m_Type = NewType;
		newCeremonyData.m_Quest = NewQuest;
		newCeremonyData.m_ObjectType = NewObjectType;
		m_NewCeremonies.Add(newCeremonyData);
		m_CeremonyDelay = 1f;
	}

	public void AddCeremony(CeremonyType NewType, ObjectType NewObjectType)
	{
		NewCeremonyData newCeremonyData = new NewCeremonyData();
		newCeremonyData.m_Type = NewType;
		newCeremonyData.m_ObjectType = NewObjectType;
		m_NewCeremonies.Add(newCeremonyData);
		CheckForNewCeremonies();
	}

	public void AddCeremony(CeremonyType NewType, Badge NewBadge)
	{
		NewCeremonyData newCeremonyData = new NewCeremonyData();
		newCeremonyData.m_Type = NewType;
		newCeremonyData.m_Badge = NewBadge;
		m_NewCeremonies.Add(newCeremonyData);
		m_CeremonyDelay = 1f;
	}

	public void AddCeremony(CeremonyType NewType, int UID)
	{
		NewCeremonyData newCeremonyData = new NewCeremonyData();
		newCeremonyData.m_Type = NewType;
		newCeremonyData.m_TargetUID = UID;
		m_NewCeremonies.Add(newCeremonyData);
		m_CeremonyDelay = 1f;
	}

	public void StartImmediateCeremony(CeremonyType NewType, Quest NewQuest, List<ObjectType> UnlockedObjects)
	{
		NewCeremonyData newCeremonyData = new NewCeremonyData();
		newCeremonyData.m_Type = NewType;
		newCeremonyData.m_Quest = NewQuest;
		newCeremonyData.m_UnlockedObjects = UnlockedObjects;
		StartCeremony(newCeremonyData);
	}

	private static int SortCompletedQuests(NewCeremonyData p1, NewCeremonyData p2)
	{
		return p1.m_Type - p2.m_Type;
	}

	private void CheckForNewCeremonies()
	{
		if (m_CeremonyDelay > 0f)
		{
			m_CeremonyDelay -= TimeManager.Instance.m_NormalDelta;
			if (m_CeremonyDelay < 0f)
			{
				m_CeremonyDelay = 0f;
			}
		}
		else if (!m_CurrentCeremony)
		{
			if (m_NewCeremonies.Count > 0)
			{
				m_NewCeremonies.Sort(SortCompletedQuests);
				NewCeremonyData newData = m_NewCeremonies[0];
				m_NewCeremonies.RemoveAt(0);
				StartCeremony(newData);
			}
			else if (m_BotMessages.Count > 0)
			{
				BotMessage botMessage = m_BotMessages[0];
				m_BotMessages.Remove(botMessage);
				GameStateManager.Instance.PushState(GameStateManager.State.Error);
				GameStateManager.Instance.GetCurrentState().GetComponent<GameStateError>().SetText(botMessage.m_NewMessageTitle, botMessage.m_NewMessage, botMessage.m_Bot);
			}
		}
	}

	public void CeremonyEnded(bool CheckForMoreCeremonies = true)
	{
		if ((m_CurrentCeremonyData.m_Type == CeremonyType.QuestEnded || m_CurrentCeremonyData.m_Type == CeremonyType.MainQuestEnded) && m_CurrentCeremonyData.m_Quest.UnlocksResearch())
		{
			GameStateManager.Instance.PopState(Immediate: true);
			NewCeremonyData newCeremonyData = new NewCeremonyData();
			newCeremonyData.m_Type = CeremonyType.ResearchStarted;
			newCeremonyData.m_Quest = m_CurrentCeremonyData.m_Quest;
			StartCeremony(newCeremonyData);
			return;
		}
		bool flag = false;
		if ((bool)GameStateManager.Instance.GetCurrentState().GetComponent<GameStateCeremony>())
		{
			GameStateManager.Instance.PopState(Immediate: true);
			flag = true;
		}
		m_CurrentCeremony = null;
		m_CurrentCeremonyType = CeremonyType.Total;
		if (CheckForMoreCeremonies && ((bool)GameStateManager.Instance.GetCurrentState().GetComponent<GameStateNormal>() || flag))
		{
			CheckForNewCeremonies();
		}
	}

	public void SkipCeremony()
	{
		if ((bool)m_CurrentCeremony.GetComponent<CeremonyQuestEnded>() && m_CurrentCeremony.GetComponent<CeremonyQuestEnded>().IsPlaying())
		{
			m_CurrentCeremony.Skip();
			return;
		}
		Transmitter.SetTransmittingGlobal(Transmit: false);
		m_NewCeremonies.Clear();
		m_CurrentCeremony.Skip();
		Object.Destroy(m_CurrentCeremony.gameObject);
		if (CameraManager.Instance.m_State != 0)
		{
			CameraManager.Instance.SetState(CameraManager.State.Normal);
		}
		CeremonyEnded();
	}

	public bool IsObjectTypeInCeremonyQueue(ObjectType NewType)
	{
		foreach (NewCeremonyData newCeremony in m_NewCeremonies)
		{
			if (newCeremony.m_Quest != null && (newCeremony.m_Quest.m_ObjectsUnlocked.Contains(NewType) || newCeremony.m_Quest.m_BuildingsUnlocked.Contains(NewType)))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsQuestTypeInCeremonyQueue(Quest.ID QuestType)
	{
		foreach (NewCeremonyData newCeremony in m_NewCeremonies)
		{
			if (newCeremony.m_Quest != null && newCeremony.m_Quest.m_ID == QuestType)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsCeremonyInCeremonyQueue(CeremonyType NewType)
	{
		foreach (NewCeremonyData newCeremony in m_NewCeremonies)
		{
			if (newCeremony.m_Type == NewType)
			{
				return true;
			}
		}
		return false;
	}

	public void DoCertificateAcademyEnded(Quest NewQuest, List<ObjectType> UnlockedObjects)
	{
		GameStateManager.Instance.PushState(GameStateManager.State.Autopedia);
		Autopedia.Instance.SetPage(Autopedia.Page.Academy);
		GameStateAutopedia.Instance.CeremonyPlaying(Playing: true, NewQuest);
		Instance.StartImmediateCeremony(CeremonyType.CertificateAcademyEnded, NewQuest, UnlockedObjects);
	}

	public void DoRevealCertificate(Quest NewQuest, List<ObjectType> UnlockedObjects)
	{
		Instance.StartImmediateCeremony(CeremonyType.RevealCertificate, NewQuest, UnlockedObjects);
	}

	public void DoResearchLevelUnlocked(Quest NewQuest, List<ObjectType> UnlockedObjects)
	{
		GameStateManager.Instance.PushState(GameStateManager.State.Autopedia);
		Autopedia.Instance.SetPage(Autopedia.Page.Academy);
		GameStateAutopedia.Instance.CeremonyPlaying(Playing: true, NewQuest);
		Instance.StartImmediateCeremony(CeremonyType.ResearchLevelUnlocked, NewQuest, UnlockedObjects);
	}

	public void GlueEnded()
	{
		m_GluePlaying = false;
	}

	public void DoMessage(string NewTitle, string NewMessage, Worker NewBot)
	{
		foreach (BotMessage botMessage in m_BotMessages)
		{
			if (botMessage.m_Bot == NewBot)
			{
				return;
			}
		}
		if (!GameStateError.Instance || !(GameStateError.Instance.m_Bot == NewBot))
		{
			BotMessage item = new BotMessage(NewBot, NewTitle, NewMessage);
			m_BotMessages.Add(item);
		}
	}

	public void Update()
	{
		GameStateManager.State baseState = GameStateManager.Instance.GetCurrentState().m_BaseState;
		if (baseState == GameStateManager.State.Normal || baseState == GameStateManager.State.CreateWorld)
		{
			if (m_Delay > 0)
			{
				m_Delay--;
			}
			else
			{
				CheckForNewCeremonies();
			}
		}
		else
		{
			m_Delay = 2;
		}
	}
}
