using System.Collections.Generic;
using UnityEngine;

public class GameStateSelectWorker : GameStateBase
{
	private enum State
	{
		Idle,
		Dragging,
		SelectList
	}

	public static GameStateSelectWorker Instance;

	private State m_State;

	private ObjectSelectBar m_Title;

	[HideInInspector]
	public TileCoord m_TilePosition;

	[HideInInspector]
	public Worker m_TargetObject;

	private bool m_WorkersFound;

	private TeachWorkerScriptEdit m_ScriptEdit;

	private Worker m_CurrentTarget;

	private Worker m_NextTarget;

	private int m_NextTargetTimer;

	private bool m_MouseHeld;

	private bool m_DragTest;

	private Vector3 m_DragMouse;

	private TileCoord m_DragTestPosition;

	private TileCoord m_DraggingStartTile;

	private TileCoord m_DraggingEndTile;

	private AreaIndicator m_DraggingArea;

	private TileCoord m_TopLeft;

	private TileCoord m_BottomRight;

	private List<Worker> m_SelectedBots;

	private TileCoord m_PreviousTilePosition;

	private BotSelectList m_BotSelectList;

	private Worker m_SelectedBot;

	protected new void Awake()
	{
		base.Awake();
		Instance = this;
		Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Standard/ObjectSelectBar", typeof(GameObject));
		m_Title = Object.Instantiate(original, default(Vector3), Quaternion.identity, menusRootTransform).GetComponent<ObjectSelectBar>();
		m_Title.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -30f);
		m_Title.SetTitle("SelectWorkerTitle");
		original = (GameObject)Resources.Load("Prefabs/Hud/BotSelectList", typeof(GameObject));
		m_BotSelectList = Object.Instantiate(original, menusRootTransform).GetComponent<BotSelectList>();
		m_BotSelectList.gameObject.SetActive(value: false);
		MaterialManager.Instance.SetDesaturation(Desaturated: true, BuildingsDesaturated: true);
		PlotManager.Instance.SetDesaturation(Desaturated: true);
		Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("Worker");
		m_WorkersFound = false;
		if (collection != null)
		{
			foreach (KeyValuePair<BaseClass, int> item in collection)
			{
				Worker component = item.Key.GetComponent<Worker>();
				if (!component.m_BeingHeld)
				{
					component.RequestPause();
					m_WorkersFound = true;
				}
			}
		}
		if ((bool)WhistleIndicator.Instance)
		{
			Object.Destroy(WhistleIndicator.Instance.gameObject);
		}
		original = (GameObject)Resources.Load("Prefabs/AreaIndicator", typeof(GameObject));
		m_DraggingArea = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, MapManager.Instance.m_MiscRootTransform).GetComponent<AreaIndicator>();
		m_DraggingArea.gameObject.SetActive(value: false);
		m_DraggingArea.SetActive(Active: true);
		m_ScriptEdit = TeachWorkerScriptEdit.Instance;
		m_ScriptEdit.SetActive(Active: false);
		m_CurrentTarget = null;
		m_NextTarget = null;
		m_NextTargetTimer = 0;
		HudManager.Instance.RolloversEnabled(Enabled: false);
		HudManager.Instance.SetHudButtonsActive(Active: false);
		TeachWorkerScriptController.Instance.SetShort(Short: true);
		m_SelectedBots = new List<Worker>();
	}

	protected new void OnDestroy()
	{
		List<Worker> list = new List<Worker>();
		foreach (Worker selectedBot in m_SelectedBots)
		{
			list.Add(selectedBot);
		}
		foreach (Worker item in list)
		{
			RemoveBot(item);
		}
		Object.Destroy(m_DraggingArea.gameObject);
		TeachWorkerScriptController.Instance.SetShort(Short: false);
		HudManager.Instance.SetHudButtonsActive(Active: true);
		HudManager.Instance.RolloversEnabled(Enabled: true);
		Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("Worker");
		if (collection != null)
		{
			foreach (KeyValuePair<BaseClass, int> item2 in collection)
			{
				Worker component = item2.Key.GetComponent<Worker>();
				component.Unpause();
				component.RestoreStandardMaterials();
			}
		}
		MaterialManager.Instance.SetDesaturation(Desaturated: false, BuildingsDesaturated: false);
		PlotManager.Instance.SetDesaturation(Desaturated: false);
		CollectionManager.Instance.GetPlayers()[0].GetComponent<FarmerPlayer>().Whistle(Start: false);
		m_ScriptEdit.SetTarget(null);
		base.OnDestroy();
		Object.Destroy(m_BotSelectList.gameObject);
		Object.Destroy(m_Title.gameObject);
	}

	public override void Popped(GameStateManager.State NewState)
	{
		base.Popped(NewState);
		HudManager.Instance.RolloversEnabled(Enabled: false);
		HudManager.Instance.SetHudButtonsActive(Active: false);
	}

	public void SetWorker(Worker NewWorker, bool Immediate = false)
	{
		if (m_ScriptEdit == null)
		{
			NewWorker = null;
		}
		if (NewWorker != m_CurrentTarget)
		{
			if (m_NextTarget != NewWorker && !Immediate)
			{
				m_NextTarget = NewWorker;
				m_NextTargetTimer = 0;
			}
			else
			{
				m_NextTargetTimer++;
				if (m_NextTargetTimer == 10 || Immediate)
				{
					if ((bool)m_CurrentTarget)
					{
						m_CurrentTarget.SetListeningHighlight(Highlight: false);
					}
					m_CurrentTarget = NewWorker;
					if ((bool)m_CurrentTarget)
					{
						m_CurrentTarget.SetListeningHighlight(Highlight: true);
					}
					if ((bool)NewWorker)
					{
						m_ScriptEdit.SetTarget(NewWorker, BotSelect: true);
						m_ScriptEdit.SetTeaching(Teaching: false);
						AudioManager.Instance.StartEvent("WorkerIndicated", NewWorker);
					}
					else
					{
						m_ScriptEdit.SetTarget(null);
					}
				}
			}
		}
		m_ScriptEdit.GetFinishedScaling();
	}

	private void SetState(State NewState)
	{
		m_State = NewState;
		switch (m_State)
		{
		case State.Idle:
			m_DragTest = false;
			m_PreviousTilePosition = default(TileCoord);
			break;
		case State.SelectList:
			SetWorker(null, Immediate: true);
			break;
		}
	}

	public void SelectSingleBot(Worker TargetObject, TileCoord TilePosition)
	{
		List<BaseClass> players = CollectionManager.Instance.GetPlayers();
		if (TargetObject.GetComponent<Worker>().m_Energy > 0f && TargetObject.GetComponent<Worker>().GetIsListening())
		{
			m_TilePosition = TilePosition;
			m_TargetObject = TargetObject;
			GameStateManager.Instance.PopState(Immediate: true);
			QuestManager.Instance.AddEvent(QuestEvent.Type.SelectBot, Bot: false, 0, null);
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateNormal>().SetSelectedWorker(m_TargetObject);
			TabWorkers.Instance.ScrollToPanel(m_TargetObject.GetComponent<Worker>().m_WorkerInfoPanel);
		}
		else
		{
			players[0].GetComponent<FarmerPlayer>().GoToAndAction(TilePosition, TargetObject);
			GameStateManager.Instance.SetState(GameStateManager.State.Normal);
		}
	}

	private void StartDraggingArea(TileCoord TilePosition)
	{
		SetState(State.Dragging);
		m_DraggingStartTile = TilePosition;
		m_DraggingEndTile = TilePosition;
		m_DraggingArea.gameObject.SetActive(value: true);
		UpdateDraggingArea();
	}

	private void UpdateDraggingArea()
	{
		m_TopLeft = default(TileCoord);
		m_BottomRight = default(TileCoord);
		if (m_DraggingStartTile.x < m_DraggingEndTile.x)
		{
			m_TopLeft.x = m_DraggingStartTile.x;
			m_BottomRight.x = m_DraggingEndTile.x;
		}
		else
		{
			m_BottomRight.x = m_DraggingStartTile.x;
			m_TopLeft.x = m_DraggingEndTile.x;
		}
		if (m_DraggingStartTile.y < m_DraggingEndTile.y)
		{
			m_TopLeft.y = m_DraggingStartTile.y;
			m_BottomRight.y = m_DraggingEndTile.y;
		}
		else
		{
			m_BottomRight.y = m_DraggingStartTile.y;
			m_TopLeft.y = m_DraggingEndTile.y;
		}
		m_DraggingArea.SetCoords(m_TopLeft, m_BottomRight);
	}

	private void AddBot(Worker NewWorker)
	{
		m_SelectedBots.Add(NewWorker);
		NewWorker.ForceHighlight(Highlighted: true);
	}

	private void RemoveBot(Worker NewWorker)
	{
		m_SelectedBots.Remove(NewWorker);
		NewWorker.ForceHighlight(Highlighted: false);
	}

	private void UpdateDraggingBots()
	{
		List<Worker> list = new List<Worker>();
		foreach (Worker selectedBot in m_SelectedBots)
		{
			list.Add(selectedBot);
		}
		List<Worker> list2 = new List<Worker>();
		PlotManager.Instance.GetArea(m_TopLeft, m_BottomRight, out var TopLeft, out var BottomRight);
		for (int i = TopLeft.y; i <= BottomRight.y; i++)
		{
			for (int j = TopLeft.x; j <= BottomRight.x; j++)
			{
				Plot plotAtPlot = PlotManager.Instance.GetPlotAtPlot(j, i);
				if (!plotAtPlot.m_Visible || !plotAtPlot.m_ObjectDictionary.ContainsKey(ObjectType.Worker))
				{
					continue;
				}
				foreach (TileCoordObject item in plotAtPlot.m_ObjectDictionary[ObjectType.Worker])
				{
					TileCoord tileCoord = item.m_TileCoord;
					if (tileCoord.x < m_TopLeft.x || tileCoord.x > m_BottomRight.x || tileCoord.y < m_TopLeft.y || tileCoord.y > m_BottomRight.y)
					{
						continue;
					}
					Worker component = item.GetComponent<Worker>();
					if (component.m_Energy > 0f && component.GetIsListening())
					{
						if (list.Contains(component))
						{
							list.Remove(component);
						}
						else
						{
							list2.Add(component);
						}
					}
				}
			}
		}
		foreach (Worker item2 in list)
		{
			RemoveBot(item2);
		}
		foreach (Worker item3 in list2)
		{
			AddBot(item3);
		}
	}

	private void EndDraggingArea()
	{
		List<Worker> list = new List<Worker>();
		foreach (Worker selectedBot in m_SelectedBots)
		{
			list.Add(selectedBot);
		}
		GameStateManager.Instance.PopState(Immediate: true);
		if (GameStateManager.Instance.GetActualState() != 0)
		{
			return;
		}
		GameStateNormal component = GameStateManager.Instance.GetCurrentState().GetComponent<GameStateNormal>();
		if (list.Count == 1)
		{
			component.SetSelectedWorker(list[0]);
		}
		else
		{
			if (list.Count <= 1)
			{
				return;
			}
			WorkerGroup tempGroup = WorkerGroupManager.Instance.m_TempGroup;
			tempGroup.ClearTemp();
			component.ClearSelectedWorkers();
			foreach (Worker item in list)
			{
				tempGroup.AddWorker(item, Temp: true);
			}
			component.SetSelectedGroup(tempGroup);
		}
	}

	private void StartDragTest(TileCoord TilePosition)
	{
		m_DragTest = true;
		m_DragMouse = Input.mousePosition;
		m_DragTestPosition = TilePosition;
	}

	private void UpdateDragTest()
	{
		_ = (Vector2)Input.mousePosition;
		if ((Input.mousePosition - m_DragMouse).magnitude > 5f)
		{
			m_DragTest = false;
			StartDraggingArea(m_DragTestPosition);
		}
	}

	private void UpdateKeys()
	{
		if (MyInputManager.m_Rewired.GetButtonDown("Whistle") || MyInputManager.m_Rewired.GetButtonDown("Quit") || !m_WorkersFound)
		{
			CollectionManager.Instance.GetPlayers()[0].GetComponent<FarmerPlayer>().UseWhistle(FarmerPlayer.WhistleCall.Cancel);
			GameStateManager.Instance.SetState(GameStateManager.State.Normal);
		}
		CameraManager.Instance.UpdateInput();
	}

	private void UpdateIdle()
	{
		bool flag = false;
		Cursor.Instance.SetModel(ObjectTypeList.m_Total);
		Worker worker = null;
		TileCoord tileCoord = default(TileCoord);
		GameObject gameObject = TestMouseCollision(out var CollisionPoint, TestTiles: true, TestBuildings: false, TestWorkers: false, TestMisc: false, TestWorkersDead: true);
		if ((bool)gameObject)
		{
			if ((bool)gameObject.GetComponent<PlotRoot>())
			{
				tileCoord = new TileCoord(CollisionPoint);
				Tile tile = TileManager.Instance.GetTile(tileCoord);
				if ((bool)tile.m_Farmer)
				{
					gameObject = tile.m_Farmer.gameObject;
				}
			}
			gameObject = GetRootObject(gameObject);
			if ((bool)gameObject && (bool)gameObject.GetComponent<Worker>() && (gameObject.GetComponent<Worker>().m_Energy == 0f || gameObject.GetComponent<Worker>().GetIsListening()))
			{
				worker = gameObject.GetComponent<Worker>();
				tileCoord = worker.m_TileCoord;
				flag = true;
				Cursor.Instance.Target(gameObject);
			}
		}
		List<TileCoordObject> list = new List<TileCoordObject>();
		if (tileCoord != m_PreviousTilePosition)
		{
			m_PreviousTilePosition = tileCoord;
			Plot plotAtTile = PlotManager.Instance.GetPlotAtTile(tileCoord);
			if ((bool)plotAtTile)
			{
				list = plotAtTile.GetObjectsTypeAtTile(ObjectType.Worker, tileCoord);
				if (list.Count > 1)
				{
					m_BotSelectList.SetBots(list);
					m_BotSelectList.gameObject.SetActive(value: true);
					m_BotSelectList.SetInteractable(Interactable: false);
				}
				else
				{
					m_BotSelectList.gameObject.SetActive(value: false);
				}
			}
		}
		if (m_BotSelectList.gameObject.activeSelf)
		{
			worker = null;
			flag = true;
		}
		HighlightObject(worker);
		SetWorker(worker);
		if (!flag)
		{
			Cursor.Instance.NoTarget();
		}
		else if (m_DragTest && Input.GetMouseButtonUp(0))
		{
			if ((bool)worker)
			{
				SelectSingleBot(worker, tileCoord);
			}
			else
			{
				m_BotSelectList.SetInteractable(Interactable: true);
				SetState(State.SelectList);
			}
		}
		if (!m_DragTest)
		{
			if (Input.GetMouseButtonDown(0))
			{
				m_DragTest = true;
				if (flag)
				{
					StartDragTest(tileCoord);
				}
				else
				{
					StartDraggingArea(tileCoord);
				}
			}
		}
		else
		{
			UpdateDragTest();
		}
		UpdateKeys();
	}

	private void UpdateDragging()
	{
		Cursor.Instance.SetModel(ObjectTypeList.m_Total);
		TileCoord tileCoord = default(TileCoord);
		GameObject gameObject = TestMouseCollision(out var CollisionPoint, TestTiles: true, TestBuildings: false, TestWorkers: false, TestMisc: false, TestWorkersDead: false);
		if ((bool)gameObject && (bool)gameObject.GetComponent<PlotRoot>())
		{
			tileCoord = new TileCoord(CollisionPoint);
			Tile tile = TileManager.Instance.GetTile(tileCoord);
			if ((bool)tile.m_Farmer)
			{
				gameObject = tile.m_Farmer.gameObject;
			}
		}
		SetWorker(null);
		if (0 == 0)
		{
			Cursor.Instance.NoTarget();
		}
		if (Input.GetMouseButton(0))
		{
			m_DraggingEndTile = tileCoord;
			UpdateDraggingArea();
			UpdateDraggingBots();
		}
		else
		{
			EndDraggingArea();
		}
		UpdateKeys();
	}

	public void BotSelect(Worker m_Worker)
	{
		m_SelectedBot = m_Worker;
	}

	public void SelectList()
	{
		m_BotSelectList.SetInteractable(Interactable: true);
		SetState(State.SelectList);
	}

	private void UpdateSelectList()
	{
		if ((bool)m_SelectedBot)
		{
			SelectSingleBot(m_SelectedBot, m_SelectedBot.m_TileCoord);
		}
		else if (!CustomStandaloneInputModule.Instance.IsUIInFocus() && Input.GetMouseButtonDown(0))
		{
			SetState(State.Idle);
			return;
		}
		if (MyInputManager.m_Rewired.GetButtonDown("Quit"))
		{
			SetState(State.Idle);
		}
		if (MyInputManager.m_Rewired.GetButtonDown("Whistle"))
		{
			CollectionManager.Instance.GetPlayers()[0].GetComponent<FarmerPlayer>().UseWhistle(FarmerPlayer.WhistleCall.Cancel);
			GameStateManager.Instance.SetState(GameStateManager.State.Normal);
		}
	}

	public override void UpdateState()
	{
		switch (m_State)
		{
		case State.Idle:
			UpdateIdle();
			break;
		case State.Dragging:
			UpdateDragging();
			break;
		case State.SelectList:
			UpdateSelectList();
			break;
		}
	}
}
