using UnityEngine;
using UnityEngine.UI;

public class GameStatePlanning : GameStateBase
{
	public enum State
	{
		New,
		NewDrag,
		Select,
		Edit,
		DraggingArea,
		DraggingAnchor,
		Total
	}

	public static GameStatePlanning Instance;

	public Planning m_Planning;

	[HideInInspector]
	public State m_State;

	private GameObject m_Cursor;

	private bool m_CursorShow;

	private bool m_WorldCursorVisible;

	private TileCoord m_AreaStartCoord;

	private TileCoord m_AreaLastCoord;

	private AreaIndicator m_AreaIndicator;

	private Vector3 m_CursorHitPosition;

	private PlanningArea m_SelectedArea;

	private PlanningArea m_HighlightedArea;

	private TileCoord m_OldDragPosition;

	private TileCoord m_DragAreaSize;

	private TileCoord m_DragOffset;

	private TileCoord m_LastTilePosition;

	protected new void Awake()
	{
		Instance = this;
		base.Awake();
		Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Buildings/Planning", typeof(GameObject));
		m_Planning = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, menusRootTransform).GetComponent<Planning>();
		m_Planning.GetComponent<RectTransform>().anchoredPosition = default(Vector2);
		GameObject original2 = (GameObject)Resources.Load("Prefabs/Hud/BinIcon", typeof(GameObject));
		m_Cursor = Object.Instantiate(original2, new Vector3(0f, 0f, 0f), Quaternion.identity, menusRootTransform);
		m_Cursor.transform.Find("Image").GetComponent<Image>().gameObject.SetActive(value: false);
		m_Cursor.GetComponent<RectTransform>().sizeDelta = new Vector2(30f, 30f);
		GameObject original3 = (GameObject)Resources.Load("Prefabs/AreaIndicator", typeof(GameObject));
		m_AreaIndicator = Object.Instantiate(original3, new Vector3(0f, 0f, 0f), Quaternion.identity, MapManager.Instance.m_MiscRootTransform).GetComponent<AreaIndicator>();
		m_AreaIndicator.gameObject.SetActive(value: false);
		m_State = State.Total;
		SetState(State.Select);
		PlanningManager.Instance.SetVisible(Visible: true);
	}

	protected new void OnDestroy()
	{
		SetSelectedArea(null);
		SetHighlightedArea(null);
		CustomStandaloneInputModule.Instance.SetIgnoreUI(Ignore: false);
		Object.Destroy(m_Planning.gameObject);
		m_Planning = null;
		Object.Destroy(m_Cursor.gameObject);
		Object.Destroy(m_AreaIndicator.gameObject);
		PlanningManager.Instance.UpdateShowAreas();
	}

	public void SetCursorImage(string NewImage)
	{
		Sprite sprite = (Sprite)Resources.Load("Textures/Hud/Edit/" + NewImage, typeof(Sprite));
		m_Cursor.GetComponent<Image>().sprite = sprite;
	}

	public void EnableCursorImage(bool Enable)
	{
		m_CursorShow = Enable;
	}

	public void UpdateCursorImage(bool Show)
	{
		if (Show && m_CursorShow)
		{
			m_Cursor.transform.localPosition = HudManager.Instance.ScreenToCanvas(Input.mousePosition) + new Vector3(40f, 0f, 0f);
			m_Cursor.SetActive(value: true);
		}
		else
		{
			m_Cursor.SetActive(value: false);
		}
	}

	public void UpdateShowToggle()
	{
		m_Planning.UpdateShowToggle();
	}

	private void SetSelectedArea(PlanningArea NewArea)
	{
		if (m_SelectedArea != null)
		{
			m_SelectedArea.m_AreaIndicator.SetAnchor(Active: false, 0f, 0f);
			m_SelectedArea.m_AreaIndicator.SetActive(Active: false);
			m_SelectedArea.UpdateColour();
		}
		PlanningManager.Instance.DeselectAll(Deselect: false, null);
		m_Planning.SetSelectedArea(NewArea);
		m_SelectedArea = NewArea;
		if (m_SelectedArea != null)
		{
			PlanningManager.Instance.DeselectAll(Deselect: true, m_SelectedArea);
			m_SelectedArea.m_AreaIndicator.SetActive(Active: true);
			m_SelectedArea.UpdateColour();
		}
	}

	private void SetHighlightedArea(PlanningArea NewArea)
	{
		if (m_HighlightedArea != null)
		{
			m_HighlightedArea.m_AreaIndicator.SetBeingEdited(Edited: false);
			m_HighlightedArea.m_AreaIndicator.UpdateArea();
		}
		m_HighlightedArea = NewArea;
		if (m_HighlightedArea != null)
		{
			m_HighlightedArea.m_AreaIndicator.SetBeingEdited(Edited: true);
			m_HighlightedArea.m_AreaIndicator.UpdateArea();
		}
	}

	public void DeleteSelectedArea()
	{
		if (m_SelectedArea != null)
		{
			PlanningArea selectedArea = m_SelectedArea;
			SetSelectedArea(null);
			PlanningManager.Instance.DeleteArea(selectedArea);
			SetState(State.Select);
		}
	}

	private bool UpdateNormal()
	{
		bool flag = false;
		if (!m_EventSystem.IsUIInFocus())
		{
			TileCoord tileCoord = default(TileCoord);
			m_CursorHitPosition = default(Vector3);
			GameObject gameObject = TestMouseCollision(out m_CursorHitPosition, TestTiles: true, TestBuildings: false, TestWorkers: false, TestMisc: false, TestWorkersDead: false);
			if ((bool)gameObject && (bool)gameObject.GetComponent<PlotRoot>())
			{
				tileCoord = new TileCoord(m_CursorHitPosition);
				Cursor.Instance.m_TileCoord = tileCoord;
				if (m_WorldCursorVisible)
				{
					Cursor.Instance.TargetTile(tileCoord);
				}
				else
				{
					Cursor.Instance.SetVisible(Visible: false);
				}
				flag = true;
			}
			if (!flag)
			{
				Cursor.Instance.NoTarget();
			}
		}
		else
		{
			Cursor.Instance.NoTarget();
		}
		if (!m_EventSystem.IsUIInUse())
		{
			CameraManager.Instance.UpdateInput();
		}
		return flag;
	}

	private void CheckNormalKeys()
	{
		if (!m_EventSystem.IsUIInUse())
		{
			CheckPlanningToggle();
			if (MyInputManager.m_Rewired.GetButtonDown("Edit"))
			{
				AudioManager.Instance.StartEvent("UIUnpause");
				GameStateManager.Instance.PopState();
			}
			else if (MyInputManager.m_Rewired.GetButtonDown("Quit") || MyInputManager.m_Rewired.GetButtonDown("Planning"))
			{
				AudioManager.Instance.StartEvent("UIUnpause");
				GameStateManager.Instance.PopState();
				GameStateManager.Instance.PopState();
			}
		}
	}

	private void StartStateNew()
	{
		m_WorldCursorVisible = true;
		SetCursorImage("Area");
		EnableCursorImage(Enable: true);
		SetSelectedArea(null);
		m_Planning.SetNewSelected(Selected: true);
	}

	private void EndStateNew()
	{
		m_WorldCursorVisible = false;
		m_Planning.SetNewSelected(Selected: false);
	}

	private void UpdateStateNew()
	{
		if (UpdateNormal() && Input.GetMouseButton(0))
		{
			SetState(State.NewDrag);
		}
		CheckNormalKeys();
	}

	private void StartStateNewDrag()
	{
		m_AreaStartCoord = Cursor.Instance.m_TileCoord;
		m_AreaLastCoord = Cursor.Instance.m_TileCoord;
		m_AreaIndicator.SetCoords(m_AreaStartCoord, m_AreaStartCoord);
		m_AreaIndicator.gameObject.SetActive(value: true);
		m_AreaIndicator.SetDimensionsEnabled(PlanningManager.Instance.m_ShowDimensions);
		CustomStandaloneInputModule.Instance.SetIgnoreUI(Ignore: true);
		EnableCursorImage(Enable: false);
	}

	private void EndStateNewDrag()
	{
		m_AreaIndicator.gameObject.SetActive(value: false);
		CustomStandaloneInputModule.Instance.SetIgnoreUI(Ignore: false);
	}

	private void UpdateStateNewDrag()
	{
		bool num = UpdateNormal();
		m_AreaIndicator.SetDimensionsEnabled(PlanningManager.Instance.m_ShowDimensions);
		if (num)
		{
			if (Input.GetMouseButton(0))
			{
				TileCoord tileCoord = Cursor.Instance.m_TileCoord;
				if (m_AreaLastCoord != tileCoord)
				{
					m_AreaLastCoord = tileCoord;
					TileCoord topLeft = default(TileCoord);
					TileCoord bottomRight = default(TileCoord);
					if (m_AreaStartCoord.x < tileCoord.x)
					{
						topLeft.x = m_AreaStartCoord.x;
						bottomRight.x = tileCoord.x;
					}
					else
					{
						bottomRight.x = m_AreaStartCoord.x;
						topLeft.x = tileCoord.x;
					}
					if (m_AreaStartCoord.y < tileCoord.y)
					{
						topLeft.y = m_AreaStartCoord.y;
						bottomRight.y = tileCoord.y;
					}
					else
					{
						bottomRight.y = m_AreaStartCoord.y;
						topLeft.y = tileCoord.y;
					}
					m_AreaIndicator.SetCoords(topLeft, bottomRight);
					m_Planning.UpdateArea();
				}
			}
			else
			{
				PlanningArea selectedArea = PlanningManager.Instance.NewArea(m_AreaIndicator.m_TopLeft, m_AreaIndicator.m_BottomRight);
				SetSelectedArea(selectedArea);
				SetState(State.Edit);
			}
		}
		if (MyInputManager.m_Rewired.GetButtonDown("Quit"))
		{
			SetState(State.New);
		}
	}

	private void StartStateSelect()
	{
		m_WorldCursorVisible = true;
		SetCursorImage("Move");
		EnableCursorImage(Enable: true);
		SetSelectedArea(null);
		SetHighlightedArea(null);
		m_Planning.SetSelectSelected(Selected: true);
	}

	private void EndStateSelect()
	{
		m_WorldCursorVisible = false;
		SetHighlightedArea(null);
		m_Planning.SetSelectSelected(Selected: false);
	}

	private void UpdateStateSelect()
	{
		if (UpdateNormal())
		{
			PlanningArea areaAtTile = PlanningManager.Instance.GetAreaAtTile(Cursor.Instance.m_TileCoord);
			if (areaAtTile != null)
			{
				SetHighlightedArea(areaAtTile);
				if (Input.GetMouseButtonDown(0))
				{
					SetSelectedArea(areaAtTile);
					SetState(State.Edit);
				}
			}
			else
			{
				SetHighlightedArea(null);
			}
		}
		else
		{
			SetHighlightedArea(null);
		}
		CheckNormalKeys();
	}

	private void StartStateEdit()
	{
		EnableCursorImage(Enable: false);
	}

	private void EndStateEdit()
	{
	}

	private void UpdateStateEdit()
	{
		if (UpdateNormal())
		{
			bool num = m_SelectedArea.SetAnchorFromPosition(m_CursorHitPosition);
			m_SelectedArea.UpdateColour();
			if (num)
			{
				if (Input.GetMouseButtonDown(0))
				{
					if (m_SelectedArea.m_AreaIndicator.m_AnchorX == 0.5f && m_SelectedArea.m_AreaIndicator.m_AnchorY == 0.5f)
					{
						AudioManager.Instance.StartEvent("AreaGrab");
						SetState(State.DraggingArea);
					}
					else
					{
						SetState(State.DraggingAnchor);
					}
					return;
				}
			}
			else if (Input.GetMouseButtonDown(0))
			{
				SetState(State.Select);
				return;
			}
		}
		CheckNormalKeys();
		if (!m_EventSystem.IsUIInUse() && MyInputManager.m_Rewired.GetButtonDown("EditDelete"))
		{
			DeleteSelectedArea();
		}
	}

	private void StartStateDraggingArea()
	{
		m_DragAreaSize = m_SelectedArea.m_AreaIndicator.m_BottomRight - m_SelectedArea.m_AreaIndicator.m_TopLeft;
		m_DragOffset = Cursor.Instance.m_TileCoord - m_SelectedArea.m_AreaIndicator.m_TopLeft;
		m_LastTilePosition = Cursor.Instance.m_TileCoord;
		m_SelectedArea.m_AreaIndicator.SetUsed(Used: true);
		m_SelectedArea.m_AreaIndicator.SetBeingEdited(Edited: true);
		EnableCursorImage(Enable: false);
	}

	private void EndStateDraggingArea()
	{
		m_SelectedArea.m_AreaIndicator.SetBeingEdited(Edited: false);
		m_SelectedArea.m_AreaIndicator.UpdateArea();
	}

	private void UpdateStateDraggingArea()
	{
		bool flag = UpdateNormal();
		if (Input.GetMouseButton(0))
		{
			if (flag)
			{
				TileCoord tileCoord = Cursor.Instance.m_TileCoord - m_DragOffset;
				TileCoord bottomRight = tileCoord + m_DragAreaSize;
				if (tileCoord.x < 0)
				{
					tileCoord.x = 0;
				}
				if (tileCoord.y < 0)
				{
					tileCoord.y = 0;
				}
				if (bottomRight.x >= TileManager.Instance.m_TilesWide)
				{
					bottomRight.x = TileManager.Instance.m_TilesWide - 1;
				}
				if (bottomRight.y >= TileManager.Instance.m_TilesHigh)
				{
					bottomRight.y = TileManager.Instance.m_TilesHigh - 1;
				}
				if (Cursor.Instance.m_TileCoord != m_LastTilePosition)
				{
					m_LastTilePosition = Cursor.Instance.m_TileCoord;
					AudioManager.Instance.StartEvent("AreaMove");
					m_SelectedArea.SetCoords(tileCoord, bottomRight);
				}
			}
		}
		else if (!Input.GetMouseButton(0))
		{
			SetState(State.Edit);
			AudioManager.Instance.StartEvent("AreaRelease");
		}
		if (MyInputManager.m_Rewired.GetButtonDown("Quit"))
		{
			SetState(State.Edit);
		}
	}

	private void StartStateDraggingAnchor()
	{
		m_OldDragPosition.x = (int)((float)(m_SelectedArea.m_AreaIndicator.m_BottomRight.x - m_SelectedArea.m_AreaIndicator.m_TopLeft.x) * m_SelectedArea.m_AreaIndicator.m_Scale.x) + m_SelectedArea.m_AreaIndicator.m_TopLeft.x;
		m_OldDragPosition.y = (int)((float)(m_SelectedArea.m_AreaIndicator.m_BottomRight.y - m_SelectedArea.m_AreaIndicator.m_TopLeft.y) * m_SelectedArea.m_AreaIndicator.m_Scale.z) + m_SelectedArea.m_AreaIndicator.m_TopLeft.y;
		m_OldDragPosition = m_SelectedArea.m_AreaIndicator.m_TopLeft;
		m_DragAreaSize = m_SelectedArea.m_AreaIndicator.m_BottomRight - m_OldDragPosition;
		m_DragOffset = Cursor.Instance.m_TileCoord - m_OldDragPosition;
		m_LastTilePosition = m_OldDragPosition;
		m_SelectedArea.m_AreaIndicator.SetUsed(Used: true);
		m_SelectedArea.m_AreaIndicator.SetBeingEdited(Edited: true);
		EnableCursorImage(Enable: false);
	}

	private void EndStateDraggingAnchor()
	{
		m_SelectedArea.m_AreaIndicator.SetBeingEdited(Edited: false);
		m_SelectedArea.m_AreaIndicator.UpdateArea();
	}

	private void SetAnchorPosition(TileCoord NewPosition)
	{
		if (m_SelectedArea.m_AreaIndicator.m_AnchorX == 0f && NewPosition.x > m_SelectedArea.m_AreaIndicator.m_BottomRight.x)
		{
			NewPosition.x = m_SelectedArea.m_AreaIndicator.m_BottomRight.x;
		}
		if (m_SelectedArea.m_AreaIndicator.m_AnchorX == 1f && NewPosition.x < m_SelectedArea.m_AreaIndicator.m_TopLeft.x)
		{
			NewPosition.x = m_SelectedArea.m_AreaIndicator.m_TopLeft.x;
		}
		if (m_SelectedArea.m_AreaIndicator.m_AnchorY == 0f && NewPosition.y > m_SelectedArea.m_AreaIndicator.m_BottomRight.y)
		{
			NewPosition.y = m_SelectedArea.m_AreaIndicator.m_BottomRight.y;
		}
		if (m_SelectedArea.m_AreaIndicator.m_AnchorY == 1f && NewPosition.y < m_SelectedArea.m_AreaIndicator.m_TopLeft.y)
		{
			NewPosition.y = m_SelectedArea.m_AreaIndicator.m_TopLeft.y;
		}
		TileCoord topLeft = m_SelectedArea.m_AreaIndicator.m_TopLeft;
		TileCoord bottomRight = m_SelectedArea.m_AreaIndicator.m_BottomRight;
		if (m_SelectedArea.m_AreaIndicator.m_AnchorX == 0f)
		{
			topLeft.x = NewPosition.x;
		}
		else if (m_SelectedArea.m_AreaIndicator.m_AnchorX == 1f)
		{
			bottomRight.x = NewPosition.x;
		}
		if (m_SelectedArea.m_AreaIndicator.m_AnchorY == 0f)
		{
			topLeft.y = NewPosition.y;
		}
		else if (m_SelectedArea.m_AreaIndicator.m_AnchorY == 1f)
		{
			bottomRight.y = NewPosition.y;
		}
		m_SelectedArea.SetCoords(topLeft, bottomRight);
		m_Planning.UpdateArea();
	}

	private void UpdateStateDraggingAnchor()
	{
		bool flag = UpdateNormal();
		if (Input.GetMouseButton(0))
		{
			if (flag && m_LastTilePosition != Cursor.Instance.m_TileCoord)
			{
				m_LastTilePosition = Cursor.Instance.m_TileCoord;
				AudioManager.Instance.StartEvent("AreaScale");
				SetAnchorPosition(m_LastTilePosition);
			}
		}
		else
		{
			SetState(State.Edit);
			AudioManager.Instance.StartEvent("AreaRelease");
		}
		if (MyInputManager.m_Rewired.GetButtonDown("Quit"))
		{
			SetState(State.Edit);
		}
	}

	public void SetState(State NewState)
	{
		switch (m_State)
		{
		case State.New:
			EndStateNew();
			break;
		case State.NewDrag:
			EndStateNewDrag();
			break;
		case State.Select:
			EndStateSelect();
			break;
		case State.Edit:
			EndStateEdit();
			break;
		case State.DraggingArea:
			EndStateDraggingArea();
			break;
		case State.DraggingAnchor:
			EndStateDraggingAnchor();
			break;
		}
		m_State = NewState;
		switch (m_State)
		{
		case State.New:
			StartStateNew();
			break;
		case State.NewDrag:
			StartStateNewDrag();
			break;
		case State.Select:
			StartStateSelect();
			break;
		case State.Edit:
			StartStateEdit();
			break;
		case State.DraggingArea:
			StartStateDraggingArea();
			break;
		case State.DraggingAnchor:
			StartStateDraggingAnchor();
			break;
		}
	}

	public override void UpdateState()
	{
		if (!m_EventSystem.IsUIInFocus())
		{
			UpdateCursorImage(Show: true);
		}
		else
		{
			UpdateCursorImage(Show: false);
		}
		switch (m_State)
		{
		case State.New:
			UpdateStateNew();
			break;
		case State.NewDrag:
			UpdateStateNewDrag();
			break;
		case State.Select:
			UpdateStateSelect();
			break;
		case State.Edit:
			UpdateStateEdit();
			break;
		case State.DraggingArea:
			UpdateStateDraggingArea();
			break;
		case State.DraggingAnchor:
			UpdateStateDraggingAnchor();
			break;
		}
	}
}
