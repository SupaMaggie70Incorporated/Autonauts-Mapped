using System;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class IndustryTree : MonoBehaviour
{
	public static IndustryTree Instance;

	private Transform m_Position;

	private Transform m_Scaler;

	private float m_MinX;

	private float m_MaxX;

	private float m_MinY;

	private float m_MaxY;

	private static float m_ActiveTimer;

	public static bool m_ActiveOn;

	private float m_LowestY;

	private List<IndustryTreeNode> m_Nodes;

	private IndustryTreeNode m_GrabbedNode;

	private bool m_GrabbedGroup;

	private string m_SaveFileName;

	private IndustryTreeNode m_ActiveNode;

	public static bool m_Hide;

	private float m_FlashingTimer;

	public bool m_FlashOn;

	private List<BaseImage> m_GuideLines;

	public bool m_BackgroundHover;

	private GameObject m_SelectedEffect;

	private float m_SelectedEffectTimer;

	private static float m_SelectedEffectDelay = 0.2f;

	private Background m_Background;

	private IndustryTreeNode m_ActiveMajorNode;

	private List<MainMissionPanel> m_Panels;

	private void Awake()
	{
		Instance = this;
		m_Background = base.transform.Find("Background").GetComponent<Background>();
		m_Scaler = base.transform.Find("Scaler");
		m_Position = m_Scaler.transform.Find("Position");
		m_GrabbedNode = null;
		SetupQuestTree();
		m_SaveFileName = "Data/IndustryTreeData";
		Load();
		ArrangeMajorNodes();
		SetupGuidelines();
		UpdateHiddenNodes();
		m_ActiveMajorNode = null;
		UpdateActiveMajorNode();
		m_SelectedEffect = (GameObject)Resources.Load("Prefabs/Hud/Autopedia/Tree/NodeSelectedEffect", typeof(GameObject));
	}

	public void Save()
	{
		JSONNode jSONNode = new JSONObject();
		JSONArray jSONArray = (JSONArray)(jSONNode["Nodes"] = new JSONArray());
		int num = 0;
		foreach (IndustryTreeNode node in m_Nodes)
		{
			JSONNode jSONNode3 = new JSONObject();
			jSONNode3["Name"] = QuestManager.Instance.m_Data.GetQuestNameFromID(node.m_Quest.m_ID);
			jSONNode3["X"] = node.m_XIndex;
			jSONNode3["Y"] = node.m_YIndex;
			jSONArray[num] = jSONNode3;
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
		JSONArray asArray = JSON.Parse(((TextAsset)Resources.Load(m_SaveFileName, typeof(TextAsset))).text)["Nodes"].AsArray;
		m_MinX = 1E+11f;
		m_MaxX = -1E+11f;
		m_MinY = 1E+11f;
		m_MaxY = -1E+11f;
		for (int i = 0; i < asArray.Count; i++)
		{
			JSONNode asObject = asArray[i].AsObject;
			string text = asObject["Name"];
			Quest.ID questIDFromName = QuestManager.Instance.m_Data.GetQuestIDFromName(text);
			foreach (IndustryTreeNode node in m_Nodes)
			{
				if (node.m_Quest.m_ID == questIDFromName)
				{
					int asInt = asObject["X"].AsInt;
					int asInt2 = asObject["Y"].AsInt;
					SetNodePosition(node, asInt, asInt2);
					if (node.transform.localPosition.x < m_MinX)
					{
						m_MinX = node.transform.localPosition.x;
					}
					if (node.transform.localPosition.x > m_MaxX)
					{
						m_MaxX = node.transform.localPosition.x;
					}
					if (node.transform.localPosition.y < m_MinY)
					{
						m_MinY = node.transform.localPosition.y;
					}
					if (node.transform.localPosition.y > m_MaxY)
					{
						m_MaxY = node.transform.localPosition.y;
					}
					break;
				}
			}
		}
	}

	public void MouseEnter()
	{
		m_BackgroundHover = true;
	}

	public void MouseExit()
	{
		m_BackgroundHover = false;
	}

	private void SetupGuidelines()
	{
		m_GuideLines = new List<BaseImage>();
		Transform transform = m_Position.transform.Find("GuideLines");
		BaseImage component = transform.Find("GuideLine").GetComponent<BaseImage>();
		component.SetActive(Active: false);
		float num = IndustryTreeNode.m_Spacing * 5f;
		float y = 100000f;
		for (int i = 0; i < 40; i++)
		{
			BaseImage baseImage = UnityEngine.Object.Instantiate(component, transform);
			baseImage.SetActive(Active: true);
			baseImage.GetComponent<RectTransform>().sizeDelta = new Vector2(num, y);
			baseImage.transform.localPosition = new Vector3((float)i * num, 0f, 0f);
			Color colour = new Color(1f, 1f, 1f, 0.065f);
			if (i == 0)
			{
				colour = new Color(1f, 1f, 1f, 0.3f);
			}
			else if (i % 2 != 0)
			{
				colour = new Color(0.5f, 0.5f, 0.5f, 0.065f);
			}
			baseImage.SetColour(colour);
			m_GuideLines.Add(baseImage);
		}
	}

	private void AddLine(IndustryTreeNode NewNode, Quest.ID NewID, Transform LineParent)
	{
		Quest quest = QuestManager.Instance.m_Data.GetQuest(NewID);
		foreach (IndustryTreeNode node in m_Nodes)
		{
			if (node.m_Quest == quest)
			{
				UILineRenderer component = UnityEngine.Object.Instantiate((GameObject)Resources.Load("Prefabs/Hud/Autopedia/Tree/IndustryNodeLine", typeof(GameObject)), new Vector3(0f, 0f, 0f), Quaternion.identity, LineParent).GetComponent<UILineRenderer>();
				component.transform.localPosition = default(Vector3);
				NewNode.AddLine(component, node);
				node.m_NodeParent = NewNode;
				break;
			}
		}
	}

	private void SetupQuestTree()
	{
		m_Nodes = new List<IndustryTreeNode>();
		Transform lineParent = m_Position.transform.Find("Lines");
		m_Panels = new List<MainMissionPanel>();
		Quest[] questData = QuestManager.Instance.m_Data.m_QuestData;
		foreach (Quest quest in questData)
		{
			if (quest != null && quest.m_Type != 0 && quest.m_MajorNode)
			{
				Transform parent = m_Position.transform.Find("MainMissions");
				MainMissionPanel component = UnityEngine.Object.Instantiate((GameObject)Resources.Load("Prefabs/Hud/Autopedia/Tree/MainMissionPanel", typeof(GameObject)), new Vector3(0f, 0f, 0f), Quaternion.identity, parent).GetComponent<MainMissionPanel>();
				m_Panels.Add(component);
			}
		}
		new List<Quest>();
		int num = 0;
		questData = QuestManager.Instance.m_Data.m_QuestData;
		foreach (Quest quest2 in questData)
		{
			if (quest2 != null && quest2.m_Type != 0)
			{
				IndustryLevel industryLevelFromQuest = QuestData.Instance.m_IndustryData.GetIndustryLevelFromQuest(quest2);
				Transform transform = m_Position.transform;
				string text = "IndustryTreeNode";
				if (quest2.m_MajorNode)
				{
					text = ((!quest2.m_Complete) ? "MajorClosedTreeNode" : "MajorOpenTreeNode");
				}
				else if (quest2.m_Type == Quest.Type.Research)
				{
					text = "ResearchTreeNode";
				}
				else if (quest2.m_Type == Quest.Type.Important)
				{
					text = "ImportantTreeNode";
				}
				IndustryTreeNode component2 = UnityEngine.Object.Instantiate((GameObject)Resources.Load("Prefabs/Hud/Autopedia/Tree/" + text, typeof(GameObject)), new Vector3(0f, 0f, 0f), Quaternion.identity, transform).GetComponent<IndustryTreeNode>();
				component2.SetObjectType(quest2);
				component2.SetParents(industryLevelFromQuest, this, transform, lineParent);
				m_Nodes.Add(component2);
				if (quest2.m_MajorNode)
				{
					m_Panels[num].SetMissionNode(component2);
					num++;
				}
			}
		}
		CreateLines();
	}

	private bool GetQuestMajor(Quest.ID NewID)
	{
		Quest quest = QuestManager.Instance.GetQuest(NewID);
		return QuestData.Instance.m_IndustryData.GetIndustryLevelFromQuest(quest)?.m_MajorNode ?? false;
	}

	private void CreateLines()
	{
		Transform lineParent = m_Position.transform.Find("Lines");
		foreach (IndustryTreeNode node in m_Nodes)
		{
			if (node.m_Quest.m_MajorNode || !node.m_Quest.m_ShowUnlockedQuests)
			{
				continue;
			}
			foreach (Quest.ID item in node.m_Quest.m_QuestsUnlocked)
			{
				if (!GetQuestMajor(item))
				{
					AddLine(node, item, lineParent);
				}
			}
			UpdateLines(node);
		}
	}

	private void AddNodeToPanel(IndustryTreeNode NewNode, MainMissionPanel NewPanel)
	{
		foreach (IndustryTreeNode node in m_Nodes)
		{
			if (!node.m_Level.m_MajorNode && NewNode.m_Quest.m_QuestsUnlocked.Contains(node.m_Quest.m_ID))
			{
				NewPanel.AddNode(node);
				AddNodeToPanel(node, NewPanel);
			}
		}
	}

	private void ArrangeMajorNodes()
	{
		int num = 0;
		foreach (MainMissionPanel panel in m_Panels)
		{
			panel.SetPosition(0, num);
			num += 2;
			AddNodeToPanel(panel.m_MainNode, panel);
		}
	}

	private void SetNodeVisible(IndustryTreeNode NewNode, bool Visible)
	{
		NewNode.SetVisible(Visible);
		foreach (IndustryTreeNode node in m_Nodes)
		{
			if (!node.m_Level.m_MajorNode && NewNode.m_Quest.m_QuestsUnlocked.Contains(node.m_Quest.m_ID))
			{
				SetNodeVisible(node, Visible);
			}
		}
	}

	private void UpdateActiveMajorNode()
	{
		int num = 0;
		foreach (MainMissionPanel panel in m_Panels)
		{
			panel.SetSelected(Selected: false);
			panel.UpdatePosition(0, num);
			num += panel.GetIntHeight();
		}
		num = 0;
		foreach (MainMissionPanel panel2 in m_Panels)
		{
			IndustryTreeNode mainNode = panel2.m_MainNode;
			bool flag = mainNode == m_ActiveMajorNode;
			foreach (IndustryTreeNode node in m_Nodes)
			{
				if (!node.m_Level.m_MajorNode && mainNode.m_Quest.m_QuestsUnlocked.Contains(node.m_Quest.m_ID))
				{
					SetNodeVisible(node, flag);
				}
			}
			panel2.SetSelected(flag);
			panel2.UpdatePosition(0, num);
			num += panel2.GetIntHeight();
			mainNode.SetLinesVisible(flag);
		}
	}

	private void UnlockQuest(Quest NewQuest)
	{
		QuestManager.Instance.CheatCompleteQuest(NewQuest);
		ModeButton.Get(ModeButton.Type.BuildingPalette).Show();
	}

	private void UnlockMission(Quest ClickedQuest)
	{
		if (ClickedQuest.GetIsComplete())
		{
			return;
		}
		UnlockQuest(ClickedQuest);
		foreach (IndustryTreeNode node in m_Nodes)
		{
			foreach (Quest.ID item in node.m_Quest.m_QuestsUnlocked)
			{
				if (item == ClickedQuest.m_ID)
				{
					UnlockMission(node.m_Quest);
					break;
				}
			}
		}
	}

	public void QuestClicked(IndustryTreeNode NewNode, bool Unlock)
	{
		Quest quest = NewNode.m_Quest;
		if (Unlock)
		{
			UnlockMission(quest);
			foreach (IndustryTreeNode node in m_Nodes)
			{
				node.UpdateStatus();
			}
			TabQuests.Instance.UpdateLists();
		}
		IndustryLevel industryLevelFromQuest = QuestData.Instance.m_IndustryData.GetIndustryLevelFromQuest(quest);
		GameStateManager.Instance.GetCurrentState().GetComponent<GameStateIndustry>().IndustryTreeNodeClicked(industryLevelFromQuest);
		SetActiveLevel(NewNode);
	}

	public void PanTo(IndustryTreeNode NewLevel)
	{
		m_Position.localPosition = -m_ActiveNode.transform.localPosition;
	}

	private IndustryTreeNode GetMajorNodeFromNode(IndustryTreeNode SelectNode)
	{
		foreach (IndustryTreeNode node in m_Nodes)
		{
			if (node.m_Quest.m_QuestsUnlocked.Contains(SelectNode.m_Quest.m_ID))
			{
				if (node.m_Level.m_MajorNode)
				{
					return node;
				}
				return GetMajorNodeFromNode(node);
			}
		}
		return null;
	}

	public void SetActiveLevel(IndustryTreeNode NewLevel)
	{
		if ((bool)m_ActiveNode)
		{
			m_ActiveNode.SetSelected(Selected: false);
		}
		m_ActiveNode = NewLevel;
		if ((bool)m_ActiveNode)
		{
			m_ActiveNode.SetSelected(Selected: true);
		}
		MarkLockedObjectMissions();
		if (!m_ActiveNode)
		{
			return;
		}
		if (m_ActiveNode.m_Level.m_MajorNode)
		{
			if (m_ActiveMajorNode == m_ActiveNode)
			{
				m_ActiveMajorNode = null;
			}
			else
			{
				m_ActiveMajorNode = m_ActiveNode;
			}
			UpdateActiveMajorNode();
			return;
		}
		IndustryTreeNode majorNodeFromNode = GetMajorNodeFromNode(m_ActiveNode);
		if (majorNodeFromNode != m_ActiveMajorNode)
		{
			m_ActiveMajorNode = majorNodeFromNode;
			UpdateActiveMajorNode();
		}
	}

	public void SetActiveLevel(IndustryLevel NewLevel)
	{
		foreach (IndustryTreeNode node in m_Nodes)
		{
			if (node.m_Level == NewLevel)
			{
				SetActiveLevel(node);
				PanTo(node);
				return;
			}
		}
		SetActiveLevel((IndustryTreeNode)null);
	}

	public void SetActiveLevel(Quest NewQuest)
	{
		if (NewQuest != null)
		{
			foreach (IndustryTreeNode node in m_Nodes)
			{
				if (node.m_Quest.m_ID == NewQuest.m_ID)
				{
					SetActiveLevel(node);
					PanTo(node);
					return;
				}
			}
		}
		SetActiveLevel((IndustryTreeNode)null);
	}

	private void UpdateHiddenNodes()
	{
		foreach (IndustryTreeNode node in m_Nodes)
		{
			node.SetHide(m_Hide);
		}
	}

	public void ToggleHide()
	{
		m_Hide = !m_Hide;
		UpdateHiddenNodes();
	}

	public void SetPosition(Vector3 Position)
	{
		if (Position.x < 0f - m_MaxX)
		{
			Position.x = 0f - m_MaxX;
		}
		if (Position.x > 0f - m_MinX)
		{
			Position.x = 0f - m_MinX;
		}
		if (Position.y < 0f - m_MaxY)
		{
			Position.y = 0f - m_MaxY;
		}
		if (Position.y > 0f - m_MinY)
		{
			Position.y = 0f - m_MinY;
		}
		Position.x = -2000f;
		m_Position.localPosition = Position;
	}

	public Vector3 GetPosition()
	{
		return m_Position.localPosition;
	}

	public void SetScale(float Scale)
	{
		m_Scaler.transform.localScale = new Vector3(Scale, Scale, Scale);
	}

	public void Grabbed(IndustryTreeNode NewNode)
	{
		m_GrabbedGroup = false;
		m_GrabbedNode = NewNode;
		if (!NewNode)
		{
			return;
		}
		GameStateManager.Instance.GetCurrentState().GetComponent<GameStateIndustry>().ShowSave();
		foreach (IndustryTreeNode node in m_Nodes)
		{
			if (node.GetIsSelected() && node == NewNode)
			{
				m_GrabbedGroup = true;
				break;
			}
		}
		if (m_GrabbedGroup)
		{
			foreach (IndustryTreeNode node2 in m_Nodes)
			{
				if (node2.GetIsSelected())
				{
					node2.m_LastXIndex = node2.m_XIndex;
					node2.m_LastYIndex = node2.m_YIndex;
				}
			}
		}
		NewNode.m_LastXIndex = NewNode.m_XIndex;
		NewNode.m_LastYIndex = NewNode.m_YIndex;
	}

	private int UpdateQuestLine(IndustryTreeNode NewNode, Quest.ID NewID, int i)
	{
		foreach (IndustryTreeNode node in m_Nodes)
		{
			if (node.m_Quest.m_ID == NewID)
			{
				Vector3 localPosition = NewNode.transform.localPosition;
				Vector3 localPosition2 = node.transform.localPosition;
				float num = localPosition2.x - localPosition.x;
				NewNode.m_Lines[i].Points[0] = localPosition + new Vector3(IndustryTreeNode.m_Width * 0.5f, 0f, 0f);
				NewNode.m_Lines[i].Points[1] = localPosition + new Vector3(num * 0.5f, 0f, 0f);
				NewNode.m_Lines[i].Points[2] = localPosition2 + new Vector3((0f - num) * 0.5f, 0f, 0f);
				NewNode.m_Lines[i].Points[3] = localPosition2 + new Vector3((0f - IndustryTreeNode.m_Width) * 0.5f, 0f, 0f);
				NewNode.m_Lines[i].color = new Color(1f, 0f, 0f);
				NewNode.m_Lines[i].color = new Color(1f, 1f, 1f);
				i++;
			}
		}
		return i;
	}

	public void UpdateLines(IndustryTreeNode NewNode)
	{
		if (NewNode.m_Quest.m_MajorNode || !NewNode.m_Quest.m_ShowUnlockedQuests)
		{
			return;
		}
		int i = 0;
		foreach (Quest.ID item in NewNode.m_Quest.m_QuestsUnlocked)
		{
			if (!GetQuestMajor(item))
			{
				i = UpdateQuestLine(NewNode, item, i);
			}
		}
	}

	private void SetNodePosition(IndustryTreeNode NewNode, int x, int y)
	{
		NewNode.SetPosition(x, y);
		UpdateLines(NewNode);
		foreach (IndustryTreeNode node in m_Nodes)
		{
			if (node.m_Quest.m_QuestsUnlocked.Contains(NewNode.m_Quest.m_ID))
			{
				UpdateLines(node);
			}
		}
	}

	public void SelectArea(Vector3 TopLeft, Vector3 BottomRight)
	{
		Vector3 vector = m_Position.transform.InverseTransformPoint(TopLeft);
		Vector3 vector2 = m_Position.transform.InverseTransformPoint(BottomRight);
		foreach (IndustryTreeNode node in m_Nodes)
		{
			if (node.m_Visible)
			{
				float x = node.transform.localPosition.x;
				float y = node.transform.localPosition.y;
				if (x > vector.x && x < vector2.x && y > vector.y && y < vector2.y)
				{
					node.SetSelected(Selected: true);
				}
				else
				{
					node.SetSelected(Selected: false);
				}
			}
		}
	}

	public void ClearSelected()
	{
		foreach (IndustryTreeNode node in m_Nodes)
		{
			node.SetSelected(Selected: false);
		}
	}

	public void SetHighLight(IndustryTreeNode NewNode, bool Highlight)
	{
	}

	private void UpdateActiveNode()
	{
		if ((bool)m_ActiveNode)
		{
			m_SelectedEffectTimer += TimeManager.Instance.m_PauseDelta;
			if (m_SelectedEffectTimer > m_SelectedEffectDelay)
			{
				m_SelectedEffectTimer = 0f;
				UnityEngine.Object.Instantiate(m_SelectedEffect, m_ActiveNode.transform.parent).transform.localPosition = m_ActiveNode.transform.localPosition;
			}
		}
	}

	private void Update()
	{
		if ((bool)m_GrabbedNode)
		{
			Vector3 vector = m_Position.transform.InverseTransformPoint(Input.mousePosition);
			int num = (int)(vector.x / IndustryTreeNode.m_Spacing);
			int num2 = (int)((0f - vector.y) / IndustryTreeNode.m_Spacing);
			int num3 = num - m_GrabbedNode.m_LastXIndex;
			int num4 = num2 - m_GrabbedNode.m_LastYIndex;
			SetNodePosition(m_GrabbedNode, num, num2);
			if (m_GrabbedGroup)
			{
				foreach (IndustryTreeNode node in m_Nodes)
				{
					if (node.GetIsSelected())
					{
						SetNodePosition(node, node.m_LastXIndex + num3, node.m_LastYIndex + num4);
					}
				}
			}
		}
		m_ActiveTimer += TimeManager.Instance.m_PauseDelta;
		m_ActiveOn = (int)(m_ActiveTimer * 60f) % 20 < 5;
		m_FlashingTimer += TimeManager.Instance.m_PauseDelta;
		m_FlashOn = (int)(m_ActiveTimer * 60f) % 10 < 5;
		UpdateActiveNode();
	}

	public void Refresh(bool ChangesMade)
	{
		foreach (IndustryTreeNode node in m_Nodes)
		{
			node.ClearLines();
			node.UpdateStatus();
		}
		CreateLines();
	}

	private IndustryTreeNode GetNodeFromQuest(Quest NewQuest)
	{
		foreach (IndustryTreeNode node in m_Nodes)
		{
			if (node.m_Quest == NewQuest)
			{
				return node;
			}
		}
		return null;
	}

	private Dictionary<IndustryTreeNode, int> GetMissionsFromLockedObjects(List<ObjectType> LockedObjects)
	{
		Dictionary<IndustryTreeNode, int> dictionary = new Dictionary<IndustryTreeNode, int>();
		IndustryTreeNode nodeFromQuest = GetNodeFromQuest(GameStateIndustry.m_SelectedQuest);
		if ((bool)nodeFromQuest)
		{
			dictionary.Add(nodeFromQuest, 0);
		}
		foreach (IndustryTreeNode node in m_Nodes)
		{
			Quest quest = node.m_Quest;
			foreach (ObjectType LockedObject in LockedObjects)
			{
				if (quest.m_ObjectsUnlocked.Contains(LockedObject) || quest.m_BuildingsUnlocked.Contains(LockedObject))
				{
					if (!dictionary.ContainsKey(node))
					{
						dictionary.Add(node, 0);
					}
					break;
				}
			}
		}
		return dictionary;
	}

	private void AddLockedMissions(Dictionary<IndustryTreeNode, int> LockedNodes)
	{
		if (GameStateIndustry.m_SelectedQuest == null)
		{
			return;
		}
		foreach (QuestEvent item in GameStateIndustry.m_SelectedQuest.m_EventsRequired)
		{
			if (item.m_Type == QuestEvent.Type.CompleteMission)
			{
				Quest.ID newID = (Quest.ID)item.m_ExtraData;
				Quest quest = QuestData.Instance.GetQuest(newID);
				IndustryTreeNode nodeFromQuest = GetNodeFromQuest(quest);
				if ((bool)nodeFromQuest)
				{
					LockedNodes.Add(nodeFromQuest, 0);
				}
				else
				{
					Debug.Log("Node missing " + newID);
				}
			}
		}
	}

	private Dictionary<IndustryTreeNode, int> GetAllMissionsFromLockedMissions(Dictionary<IndustryTreeNode, int> LockedMissions)
	{
		Dictionary<IndustryTreeNode, int> dictionary = new Dictionary<IndustryTreeNode, int>();
		foreach (KeyValuePair<IndustryTreeNode, int> LockedMission in LockedMissions)
		{
			IndustryTreeNode key = LockedMission.Key;
			if (!dictionary.ContainsKey(key))
			{
				dictionary.Add(key, 0);
			}
			IndustryTreeNode industryTreeNode = key;
			while (industryTreeNode.m_NodeParent != null)
			{
				industryTreeNode = industryTreeNode.m_NodeParent;
				if (!dictionary.ContainsKey(industryTreeNode))
				{
					dictionary.Add(industryTreeNode, 0);
				}
			}
		}
		return dictionary;
	}

	private void MarkMissingMissions(Dictionary<IndustryTreeNode, int> NewNodes, Dictionary<IndustryTreeNode, int> AllNodes)
	{
		foreach (IndustryTreeNode node in m_Nodes)
		{
			if (NewNodes.ContainsKey(node) && node.m_Quest != GameStateIndustry.m_SelectedQuest)
			{
				node.SetFlashing(Flashing: true);
			}
			else
			{
				node.SetFlashing(Flashing: false);
			}
			for (int i = 0; i < node.m_LineNode.Count; i++)
			{
				IndustryTreeNode key = node.m_LineNode[i];
				if (AllNodes.ContainsKey(key))
				{
					node.SetLineFlashing(i, Flashing: true);
				}
				else
				{
					node.SetLineFlashing(i, Flashing: false);
				}
			}
		}
	}

	public void MarkLockedObjectMissions()
	{
		List<ObjectType> lockedObjects = new List<ObjectType>();
		if (GameStateIndustry.m_SelectedQuest != null)
		{
			lockedObjects = GameStateIndustry.m_SelectedQuest.GetObjectsLocked();
		}
		Dictionary<IndustryTreeNode, int> missionsFromLockedObjects = GetMissionsFromLockedObjects(lockedObjects);
		AddLockedMissions(missionsFromLockedObjects);
		Dictionary<IndustryTreeNode, int> allMissionsFromLockedMissions = GetAllMissionsFromLockedMissions(missionsFromLockedObjects);
		MarkMissingMissions(missionsFromLockedObjects, allMissionsFromLockedMissions);
	}

	public Vector3 GetNodePosition(Quest.ID NewID)
	{
		foreach (IndustryTreeNode node in m_Nodes)
		{
			if (node.m_Quest.m_ID == NewID)
			{
				return node.transform.localPosition;
			}
		}
		return default(Vector3);
	}

	public void UpdateMode(GameStateIndustry.Mode NewMode)
	{
		m_Background.SetColour(new Color(0.5f, 0.5f, 0.5f, 1f));
	}
}
