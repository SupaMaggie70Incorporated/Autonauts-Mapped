using System.Collections.Generic;
using UnityEngine;

public class GameStateEditArea : GameStateBase
{
	public static GameStateEditArea Instance;

	[HideInInspector]
	public TileCoord m_TilePosition;

	private AreaIndicator m_AreaIndicator;

	public HighInstruction m_Instruction;

	private TileCoord m_OldPosition1;

	private TileCoord m_OldPosition2;

	private HighInstruction.FindType m_OldFindType;

	private int m_OldUID;

	private TileCoord m_LastTilePosition;

	private bool m_MouseButtonHeld;

	private TileCoord m_OldDragPosition;

	private TileCoord m_DragAreaSize;

	private TileCoord m_DragOffset;

	public int m_MaxSize;

	private int m_TempUID;

	public EditArea m_EditArea;

	private bool m_OldButtonsActive;

	protected new void Awake()
	{
		base.Awake();
		Instance = this;
		m_MouseButtonHeld = TestMouseButtonDown(0);
		Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/EditArea", typeof(GameObject));
		m_EditArea = Object.Instantiate(original, menusRootTransform).GetComponent<EditArea>();
		m_EditArea.SetParent(this);
		m_OldButtonsActive = HudManager.Instance.GetHudButtonsActive();
		HudManager.Instance.RolloversEnabled(Enabled: false);
		HudManager.Instance.SetHudButtonsActive(Active: false);
	}

	protected new void OnDestroy()
	{
		SetState(EditArea.State.Total);
		QuestManager.Instance.AddEvent(QuestEvent.Type.EndEditSearchArea, Bot: false, null, null);
		HudManager.Instance.RolloversEnabled(Enabled: true);
		HudManager.Instance.SetHudButtonsActive(m_OldButtonsActive);
		TabManager.Instance.SetActive(Active: false);
		Object.Destroy(m_EditArea.gameObject);
		if ((bool)m_AreaIndicator)
		{
			m_AreaIndicator.SetActive(Active: false);
			m_AreaIndicator.SetBeingEdited(Edited: false);
			m_AreaIndicator.SetDimensionsEnabled(Enabled: false);
		}
		base.OnDestroy();
	}

	private void StartSelectSign(bool Start)
	{
		MaterialManager.Instance.SetDesaturation(Start, Start);
		PlotManager.Instance.SetDesaturation(Start);
		List<ObjectType> list = new List<ObjectType>();
		foreach (ObjectType type in Converter.m_Types)
		{
			if (type != ObjectType.ConverterFoundation)
			{
				list.Add(type);
			}
		}
		list.Add(ObjectType.Sign);
		list.Add(ObjectType.Sign2);
		list.Add(ObjectType.Sign3);
		list.Add(ObjectType.Billboard);
		ModelManager.Instance.SetSearchTypesHighlight(list, Start, AndOccluded: false);
	}

	private void SetState(EditArea.State NewState)
	{
		if (m_EditArea == null || m_AreaIndicator == null)
		{
			return;
		}
		EditArea.State state = m_EditArea.m_State;
		if (state == EditArea.State.SelectSign)
		{
			StartSelectSign(Start: false);
			m_EditArea.SetTitle("EditAreaTitleEdit");
		}
		m_EditArea.SetState(NewState);
		switch (m_EditArea.m_State)
		{
		case EditArea.State.EditArea:
			m_AreaIndicator.SetUsed(Used: true);
			m_AreaIndicator.SetBeingEdited(Edited: false);
			break;
		case EditArea.State.DraggingAnchor:
			m_AreaIndicator.SetBeingEdited(Edited: true);
			break;
		case EditArea.State.DraggingArea:
			m_AreaIndicator.SetBeingEdited(Edited: true);
			break;
		case EditArea.State.SelectSign:
			m_EditArea.SetTitle("EditAreaTitleSelect");
			StartSelectSign(Start: true);
			if (m_Instruction.m_ActionInfo.m_ObjectUID == 0)
			{
				m_AreaIndicator.SetUsed(Used: false);
			}
			break;
		}
		m_AreaIndicator.UpdateArea();
	}

	private void Remember()
	{
		if (!(m_AreaIndicator == null))
		{
			m_OldPosition1 = m_AreaIndicator.m_TopLeft;
			m_OldPosition2 = m_AreaIndicator.m_BottomRight;
			m_OldFindType = m_AreaIndicator.m_FindType;
			if (m_Instruction != null)
			{
				m_OldUID = m_Instruction.m_ActionInfo.m_ObjectUID;
			}
		}
	}

	public void Restore()
	{
		if (m_Instruction != null && m_Instruction.m_ActionInfo.m_ObjectUID != m_OldUID && m_OldUID != 0)
		{
			DisableOldSignArea();
			m_Instruction.m_ActionInfo.m_ObjectUID = m_OldUID;
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(m_Instruction.m_ActionInfo.m_ObjectUID);
			ShowObjectIndicator(objectFromUniqueID, Show: true);
		}
		if ((bool)m_AreaIndicator)
		{
			m_AreaIndicator.SetCoords(m_OldPosition1, m_OldPosition2);
			m_AreaIndicator.SetFindType(m_OldFindType);
			m_AreaIndicator.UpdateArea();
		}
	}

	private void SetAreaIndicator(AreaIndicator NewIndicator)
	{
		m_AreaIndicator = NewIndicator;
		m_EditArea.UpdateIndicator();
	}

	public void SetIndicator(AreaIndicator NewIndicator, int MaxSize, HighInstruction NewInstruction)
	{
		m_Instruction = NewInstruction;
		SetAreaIndicator(NewIndicator);
		m_MaxSize = MaxSize;
		Remember();
		if (NewInstruction == null)
		{
			m_EditArea.DisableToggleButton();
			m_AreaIndicator.SetFindType(HighInstruction.FindType.Full);
		}
		if (m_Instruction != null && m_Instruction.m_ActionInfo.m_ObjectUID != 0)
		{
			SetState(EditArea.State.SelectSign);
		}
		else
		{
			SetState(EditArea.State.EditArea);
		}
	}

	private void UpdateEditArea()
	{
		if (m_AreaIndicator == null)
		{
			return;
		}
		if (!m_EventSystem.IsUIInFocus())
		{
			Cursor.Instance.SetModel(ObjectTypeList.m_Total);
			Actionable targetObject = null;
			TileCoord tileCoord = default(TileCoord);
			Vector3 CollisionPoint;
			GameObject gameObject = TestMouseCollision(out CollisionPoint, TestTiles: true, TestBuildings: false, TestWorkers: false, TestMisc: false, TestWorkersDead: false);
			if ((bool)gameObject && (bool)gameObject.GetComponent<PlotRoot>())
			{
				tileCoord = new TileCoord(CollisionPoint);
				m_AreaIndicator.SetAnchorFromPosition(CollisionPoint);
			}
			HighlightObject(targetObject);
			if (m_MouseButtonHeld)
			{
				m_MouseButtonHeld = TestMouseButtonDown(0);
			}
			else if (m_AreaIndicator.m_AnchorActive && TestMouseButtonDown(0))
			{
				if (m_AreaIndicator.m_AnchorX == 0.5f && m_AreaIndicator.m_AnchorY == 0.5f)
				{
					m_OldDragPosition = m_AreaIndicator.m_TopLeft;
					m_DragAreaSize = m_AreaIndicator.m_BottomRight - m_OldDragPosition;
					m_DragOffset = tileCoord - m_OldDragPosition;
					AudioManager.Instance.StartEvent("AreaGrab");
					SetState(EditArea.State.DraggingArea);
				}
				else
				{
					m_OldDragPosition.x = (int)((float)(m_AreaIndicator.m_BottomRight.x - m_AreaIndicator.m_TopLeft.x) * m_AreaIndicator.m_Scale.x) + m_AreaIndicator.m_TopLeft.x;
					m_OldDragPosition.y = (int)((float)(m_AreaIndicator.m_BottomRight.y - m_AreaIndicator.m_TopLeft.y) * m_AreaIndicator.m_Scale.z) + m_AreaIndicator.m_TopLeft.y;
					m_OldDragPosition = m_AreaIndicator.m_TopLeft;
					m_DragAreaSize = m_AreaIndicator.m_BottomRight - m_OldDragPosition;
					m_DragOffset = tileCoord - m_OldDragPosition;
					m_LastTilePosition = m_OldDragPosition;
					SetState(EditArea.State.DraggingAnchor);
				}
			}
		}
		else
		{
			m_AreaIndicator.SetAnchor(Active: false, 0f, 0f);
		}
		if (!m_EventSystem.IsUIInUse())
		{
			CameraManager.Instance.UpdateInput();
		}
		if (MyInputManager.m_Rewired.GetButtonDown("Quit"))
		{
			AudioManager.Instance.StartEvent("UIOptionCancelled");
			Restore();
			GameStateManager.Instance.PopState(Immediate: true);
		}
		CheckPlanningToggle();
	}

	public static int GetTileNotVisibleX(int x1, int x2, int y)
	{
		for (int i = x1; i <= x2; i++)
		{
			Plot plotAtTile = PlotManager.Instance.GetPlotAtTile(new TileCoord(i, y));
			if ((bool)plotAtTile && !plotAtTile.m_Visible)
			{
				return i;
			}
		}
		return -1;
	}

	public static int GetTileNotVisibleY(int y1, int y2, int x)
	{
		for (int i = y1; i <= y2; i++)
		{
			Plot plotAtTile = PlotManager.Instance.GetPlotAtTile(new TileCoord(x, i));
			if ((bool)plotAtTile && !plotAtTile.m_Visible)
			{
				return i;
			}
		}
		return -1;
	}

	private int GetDistanceToVisibleX(int y, int x1, int x2)
	{
		int i;
		for (i = x1; i < x2 && !PlotManager.Instance.GetPlotAtTile(new TileCoord(i, y)).m_Visible; i++)
		{
		}
		return i - x1;
	}

	private int GetDistanceToVisibleY(int x, int y1, int y2)
	{
		int i;
		for (i = y1; i < y2 && !PlotManager.Instance.GetPlotAtTile(new TileCoord(x, i)).m_Visible; i++)
		{
		}
		return i - y1;
	}

	public static TileCoord GetAreaInVisiblePlots(TileCoord Coord1, TileCoord Coord2, bool TopLeft, int MaxSize)
	{
		TileCoord result = Coord1;
		if (TopLeft)
		{
			int num = Coord2.x;
			int num2 = Coord2.y;
			bool flag = true;
			bool flag2 = true;
			do
			{
				int x = num;
				if (flag)
				{
					if (num != result.x && GetTileNotVisibleY(num2, Coord2.y, num - 1) == -1 && Coord2.x - num < MaxSize)
					{
						num--;
					}
					else
					{
						flag = false;
					}
				}
				if (flag2)
				{
					if (num2 != result.y && GetTileNotVisibleX(x, Coord2.x, num2 - 1) == -1 && Coord2.y - num2 < MaxSize)
					{
						num2--;
					}
					else
					{
						flag2 = false;
					}
				}
			}
			while (flag || flag2);
			result.x = num;
			result.y = num2;
		}
		else
		{
			int num3 = Coord2.x;
			int num4 = Coord2.y;
			bool flag3 = true;
			bool flag4 = true;
			do
			{
				int x2 = num3;
				if (flag3)
				{
					if (num3 != result.x && GetTileNotVisibleY(Coord2.y, num4, num3 + 1) == -1 && num3 - Coord2.x < MaxSize)
					{
						num3++;
					}
					else
					{
						flag3 = false;
					}
				}
				if (flag4)
				{
					if (num4 != result.y && GetTileNotVisibleX(Coord2.x, x2, num4 + 1) == -1 && num4 - Coord2.y < MaxSize)
					{
						num4++;
					}
					else
					{
						flag4 = false;
					}
				}
			}
			while (flag3 || flag4);
			result.x = num3;
			result.y = num4;
		}
		return result;
	}

	public static void MakeAreaInVisiblePlots(TileCoord CenterCoord, out TileCoord TopLeft, out TileCoord BottomRight, int MaxSize)
	{
		int num = CenterCoord.x;
		int num2 = CenterCoord.y;
		int num3 = CenterCoord.x;
		int num4 = CenterCoord.y;
		bool flag = true;
		bool flag2 = true;
		bool flag3 = true;
		bool flag4 = true;
		do
		{
			if (flag)
			{
				if (GetTileNotVisibleY(num2, num4, num - 1) == -1 && num3 - num < MaxSize)
				{
					num--;
				}
				else
				{
					flag = false;
				}
			}
			if (flag2)
			{
				if (GetTileNotVisibleX(num, num3, num2 - 1) == -1 && num4 - num2 < MaxSize)
				{
					num2--;
				}
				else
				{
					flag2 = false;
				}
			}
			if (flag3)
			{
				if (GetTileNotVisibleY(num2, num4, num3 + 1) == -1 && num3 - num < MaxSize)
				{
					num3++;
				}
				else
				{
					flag3 = false;
				}
			}
			if (flag4)
			{
				if (GetTileNotVisibleX(num, num3, num4 + 1) == -1 && num4 - num2 < MaxSize)
				{
					num4++;
				}
				else
				{
					flag4 = false;
				}
			}
		}
		while (flag || flag2 || flag3 || flag4);
		TopLeft.x = num;
		TopLeft.y = num2;
		BottomRight.x = num3;
		BottomRight.y = num4;
	}

	private int GetX(int OldX, int NewX, int StartY, int EndY, bool Left)
	{
		int num = 1;
		if (Left)
		{
			num = -1;
		}
		while (OldX != NewX)
		{
			if (GetTileNotVisibleY(StartY, EndY, OldX + num) != -1)
			{
				return OldX;
			}
			OldX += num;
		}
		return NewX;
	}

	private int GetY(int OldY, int NewY, int StartX, int EndX, bool Top)
	{
		int num = 1;
		if (Top)
		{
			num = -1;
		}
		while (OldY != NewY)
		{
			if (GetTileNotVisibleX(StartX, EndX, OldY + num) != -1)
			{
				return OldY;
			}
			OldY += num;
		}
		return NewY;
	}

	public void SetAnchorPosition(TileCoord TilePosition)
	{
		if (m_AreaIndicator.m_AnchorX == 0f && TilePosition.x > m_AreaIndicator.m_BottomRight.x)
		{
			TilePosition.x = m_AreaIndicator.m_BottomRight.x;
		}
		if (m_AreaIndicator.m_AnchorX == 1f && TilePosition.x < m_AreaIndicator.m_TopLeft.x)
		{
			TilePosition.x = m_AreaIndicator.m_TopLeft.x;
		}
		if (m_AreaIndicator.m_AnchorY == 0f && TilePosition.y > m_AreaIndicator.m_BottomRight.y)
		{
			TilePosition.y = m_AreaIndicator.m_BottomRight.y;
		}
		if (m_AreaIndicator.m_AnchorY == 1f && TilePosition.y < m_AreaIndicator.m_TopLeft.y)
		{
			TilePosition.y = m_AreaIndicator.m_TopLeft.y;
		}
		if (m_AreaIndicator.m_AnchorX == 0f && m_AreaIndicator.m_BottomRight.x - TilePosition.x > m_MaxSize)
		{
			TilePosition.x = m_AreaIndicator.m_BottomRight.x - m_MaxSize;
		}
		if (m_AreaIndicator.m_AnchorX == 1f && TilePosition.x - m_AreaIndicator.m_TopLeft.x > m_MaxSize)
		{
			TilePosition.x = m_AreaIndicator.m_TopLeft.x + m_MaxSize;
		}
		if (m_AreaIndicator.m_AnchorY == 0f && m_AreaIndicator.m_BottomRight.y - TilePosition.y > m_MaxSize)
		{
			TilePosition.y = m_AreaIndicator.m_BottomRight.y - m_MaxSize;
		}
		if (m_AreaIndicator.m_AnchorY == 1f && TilePosition.y - m_AreaIndicator.m_TopLeft.y > m_MaxSize)
		{
			TilePosition.y = m_AreaIndicator.m_TopLeft.y + m_MaxSize;
		}
		if (m_AreaIndicator.m_AnchorX == 0f && TilePosition.x < m_AreaIndicator.m_TopLeft.x)
		{
			TilePosition.x = GetX(m_AreaIndicator.m_TopLeft.x, TilePosition.x, m_AreaIndicator.m_TopLeft.y, m_AreaIndicator.m_BottomRight.y, Left: true);
		}
		else if (m_AreaIndicator.m_AnchorX == 1f && TilePosition.x > m_AreaIndicator.m_BottomRight.x)
		{
			TilePosition.x = GetX(m_AreaIndicator.m_BottomRight.x, TilePosition.x, m_AreaIndicator.m_TopLeft.y, m_AreaIndicator.m_BottomRight.y, Left: false);
		}
		if (m_AreaIndicator.m_AnchorY == 0f && TilePosition.y < m_AreaIndicator.m_TopLeft.y)
		{
			TilePosition.y = GetY(m_AreaIndicator.m_TopLeft.y, TilePosition.y, m_AreaIndicator.m_TopLeft.x, m_AreaIndicator.m_BottomRight.x, Top: true);
		}
		else if (m_AreaIndicator.m_AnchorY == 1f && TilePosition.y > m_AreaIndicator.m_BottomRight.y)
		{
			TilePosition.y = GetY(m_AreaIndicator.m_BottomRight.y, TilePosition.y, m_AreaIndicator.m_TopLeft.x, m_AreaIndicator.m_BottomRight.x, Top: false);
		}
		TileCoord topLeft = m_AreaIndicator.m_TopLeft;
		TileCoord bottomRight = m_AreaIndicator.m_BottomRight;
		if (m_AreaIndicator.m_AnchorX == 0f)
		{
			topLeft.x = TilePosition.x;
		}
		else if (m_AreaIndicator.m_AnchorX == 1f)
		{
			bottomRight.x = TilePosition.x;
		}
		if (m_AreaIndicator.m_AnchorY == 0f)
		{
			topLeft.y = TilePosition.y;
		}
		else if (m_AreaIndicator.m_AnchorY == 1f)
		{
			bottomRight.y = TilePosition.y;
		}
		m_AreaIndicator.SetCoords(topLeft, bottomRight);
		m_EditArea.UpdateDimensions();
	}

	private void UpdateDraggingAnchor()
	{
		bool flag = false;
		TileCoord tileCoord = default(TileCoord);
		Vector3 CollisionPoint;
		GameObject gameObject = TestMouseCollision(out CollisionPoint, TestTiles: true, TestBuildings: false, TestWorkers: false, TestMisc: false, TestWorkersDead: false);
		if ((bool)gameObject && (bool)gameObject.GetComponent<PlotRoot>())
		{
			tileCoord = new TileCoord(CollisionPoint);
			flag = true;
		}
		if (!flag)
		{
			Cursor.Instance.NoTarget();
		}
		else if (Input.GetMouseButton(0))
		{
			_ = tileCoord - m_DragOffset;
			if (tileCoord != m_LastTilePosition)
			{
				m_LastTilePosition = tileCoord;
				AudioManager.Instance.StartEvent("AreaScale");
				SetAnchorPosition(tileCoord);
			}
		}
		else
		{
			SetState(EditArea.State.EditArea);
			AudioManager.Instance.StartEvent("AreaRelease");
		}
		if (!m_EventSystem.IsUIInUse())
		{
			CameraManager.Instance.UpdateInput();
		}
		if (MyInputManager.m_Rewired.GetButtonDown("Quit"))
		{
			AudioManager.Instance.StartEvent("UIOptionCancelled");
			m_AreaIndicator.SetCoords(m_OldDragPosition, m_OldDragPosition + m_DragAreaSize);
			SetState(EditArea.State.EditArea);
		}
	}

	private void SetAnchorPositions(TileCoord TilePosition)
	{
		if (TilePosition.x < 0)
		{
			TilePosition.x = 0;
		}
		int num = TilePosition.x + m_DragAreaSize.x - (TileManager.Instance.m_TilesWide - 1);
		if (num > 0)
		{
			TilePosition.x -= num;
		}
		if (TilePosition.y < 0)
		{
			TilePosition.y = 0;
		}
		num = TilePosition.y + m_DragAreaSize.y - (TileManager.Instance.m_TilesHigh - 1);
		if (num > 0)
		{
			TilePosition.y -= num;
		}
		for (int i = 0; i <= m_DragAreaSize.y; i++)
		{
			for (int j = 0; j <= m_DragAreaSize.x; j++)
			{
				if (!PlotManager.Instance.GetPlotAtTile(TilePosition + new TileCoord(j, i)).m_Visible)
				{
					return;
				}
			}
		}
		m_AreaIndicator.SetCoords(TilePosition, TilePosition + m_DragAreaSize);
	}

	private void UpdateDraggingArea()
	{
		bool flag = false;
		TileCoord tileCoord = default(TileCoord);
		Vector3 CollisionPoint;
		GameObject gameObject = TestMouseCollision(out CollisionPoint, TestTiles: true, TestBuildings: false, TestWorkers: false, TestMisc: false, TestWorkersDead: false);
		if ((bool)gameObject && (bool)gameObject.GetComponent<PlotRoot>())
		{
			tileCoord = new TileCoord(CollisionPoint);
			flag = true;
		}
		if (!flag)
		{
			Cursor.Instance.NoTarget();
		}
		else if (Input.GetMouseButton(0))
		{
			TileCoord tileCoord2 = tileCoord - m_DragOffset;
			if (tileCoord2 != m_LastTilePosition)
			{
				m_LastTilePosition = tileCoord2;
				AudioManager.Instance.StartEvent("AreaMove");
				SetAnchorPositions(tileCoord2);
			}
		}
		else
		{
			SetState(EditArea.State.EditArea);
			AudioManager.Instance.StartEvent("AreaRelease");
		}
		if (!m_EventSystem.IsUIInUse())
		{
			CameraManager.Instance.UpdateInput();
		}
		if (MyInputManager.m_Rewired.GetButtonDown("Quit"))
		{
			AudioManager.Instance.StartEvent("UIOptionCancelled");
			m_AreaIndicator.SetCoords(m_OldDragPosition, m_OldDragPosition + m_DragAreaSize);
			SetState(EditArea.State.EditArea);
		}
	}

	private void ShowObjectIndicator(BaseClass NewObject, bool Show)
	{
		if (Sign.GetIsTypeSign(NewObject.m_TypeIdentifier))
		{
			NewObject.GetComponent<Sign>().KeepIndicator(Show);
			NewObject.GetComponent<Sign>().ShowIndicator(Show);
		}
		if ((bool)NewObject.GetComponent<Converter>())
		{
			NewObject.GetComponent<Converter>().ShowIndicator(Show);
		}
	}

	private void DisableOldSignArea()
	{
		if (m_Instruction.m_ActionInfo.m_ObjectUID != 0)
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(m_Instruction.m_ActionInfo.m_ObjectUID);
			if ((bool)objectFromUniqueID)
			{
				ShowObjectIndicator(objectFromUniqueID, Show: false);
			}
		}
	}

	private void UpdateSelectSign()
	{
		bool flag = false;
		if (!m_EventSystem.IsUIInFocus())
		{
			Cursor.Instance.SetModel(ObjectTypeList.m_Total);
			Actionable actionable = null;
			GameObject gameObject = TestMouseCollision(out var _, TestTiles: false, TestBuildings: true, TestWorkers: false, TestMisc: true, TestWorkersDead: false);
			if ((bool)gameObject)
			{
				if ((bool)gameObject.GetComponent<PlotRoot>())
				{
					if ((bool)gameObject.transform.parent)
					{
						actionable = gameObject.transform.parent.GetComponent<Actionable>();
					}
				}
				else
				{
					gameObject = GetRootObject(gameObject);
					if ((bool)gameObject && (Sign.GetIsTypeSign(gameObject.GetComponent<BaseClass>().m_TypeIdentifier) || ((bool)gameObject.GetComponent<Converter>() && gameObject.GetComponent<Converter>().m_TypeIdentifier != ObjectType.ConverterFoundation)))
					{
						actionable = gameObject.GetComponent<TileCoordObject>();
					}
				}
				if (actionable != null)
				{
					flag = true;
					Cursor.Instance.Target(actionable.gameObject);
				}
			}
			HighlightObject(actionable);
			if ((bool)actionable && Sign.GetIsTypeSign(actionable.m_TypeIdentifier))
			{
				HighInstruction.FindType findType = m_Instruction.GetFindType();
				actionable.GetComponent<Sign>().m_Indicator.SetFindType(findType);
			}
			if (!flag)
			{
				Cursor.Instance.NoTarget();
			}
			else if (m_MouseButtonHeld)
			{
				m_MouseButtonHeld = TestMouseButtonDown(0);
			}
			else if (TestMouseButtonDown(0) && (Sign.GetIsTypeSign(actionable.m_TypeIdentifier) || (bool)actionable.GetComponent<Converter>()))
			{
				DisableOldSignArea();
				m_Instruction.m_ActionInfo.m_ObjectUID = actionable.m_UniqueID;
				ShowObjectIndicator(actionable, Show: true);
				if ((bool)actionable.GetComponent<Sign>())
				{
					SetAreaIndicator(actionable.GetComponent<Sign>().m_Indicator);
				}
				else if ((bool)actionable.GetComponent<Converter>())
				{
					SetAreaIndicator(actionable.GetComponent<Converter>().m_Indicator);
				}
			}
		}
		if (!m_EventSystem.IsUIInUse())
		{
			CameraManager.Instance.UpdateInput();
		}
		if (MyInputManager.m_Rewired.GetButtonDown("Quit"))
		{
			AudioManager.Instance.StartEvent("UIOptionCancelled");
			Restore();
			GameStateManager.Instance.PopState(Immediate: true);
		}
		CheckPlanningToggle();
	}

	public void ToggleSign()
	{
		if (m_EditArea == null || m_Instruction == null)
		{
			return;
		}
		if (m_EditArea.m_State == EditArea.State.SelectSign)
		{
			m_TempUID = m_Instruction.m_ActionInfo.m_ObjectUID;
			m_Instruction.m_ActionInfo.m_ObjectUID = 0;
		}
		else if (m_TempUID != 0)
		{
			m_Instruction.m_ActionInfo.m_ObjectUID = m_TempUID;
		}
		TeachWorkerScriptEdit.Instance.RemakeAreaIndicators();
		SetAreaIndicator(AreaIndicatorManager.Instance.GetAreaIndicatorFromInstruction(m_Instruction));
		if ((bool)m_AreaIndicator)
		{
			m_AreaIndicator.SetActive(Active: true);
			m_AreaIndicator.UpdateArea();
			if (m_EditArea.m_State == EditArea.State.EditArea && m_Instruction.m_ActionInfo.m_ObjectUID == 0)
			{
				m_AreaIndicator.SetBeingEdited(Edited: false);
			}
		}
		if (m_EditArea.m_State == EditArea.State.EditArea)
		{
			SetState(EditArea.State.SelectSign);
		}
		else
		{
			SetState(EditArea.State.EditArea);
		}
	}

	public void MaxArea()
	{
		if (!(m_AreaIndicator == null))
		{
			int maxSize = ((m_Instruction != null) ? TeachWorkerScriptEdit.Instance.m_CurrentTarget.GetTotalSearchRange() : m_MaxSize);
			TileCoord centerCoord = m_AreaIndicator.m_BottomRight + m_AreaIndicator.m_TopLeft;
			centerCoord.x /= 2;
			centerCoord.y /= 2;
			MakeAreaInVisiblePlots(centerCoord, out var TopLeft, out var BottomRight, maxSize);
			m_AreaIndicator.SetCoords(TopLeft, BottomRight);
			m_EditArea.UpdateDimensions();
		}
	}

	public void FindClicked()
	{
		if (!(m_AreaIndicator == null))
		{
			TileCoord tileCoord = m_AreaIndicator.m_BottomRight + m_AreaIndicator.m_TopLeft;
			tileCoord.x /= 2;
			tileCoord.y /= 2;
			CameraManager.Instance.Focus(tileCoord.ToWorldPositionTileCenteredWithoutHeight());
		}
	}

	public void SetFindType(HighInstruction.FindType NewType)
	{
		m_AreaIndicator.SetFindType(NewType);
		m_Instruction.SetFindType(NewType);
	}

	public AreaIndicator GetAreaIndicator()
	{
		return m_AreaIndicator;
	}

	public override void UpdateState()
	{
		switch (m_EditArea.m_State)
		{
		case EditArea.State.EditArea:
			UpdateEditArea();
			break;
		case EditArea.State.DraggingAnchor:
			UpdateDraggingAnchor();
			break;
		case EditArea.State.DraggingArea:
			UpdateDraggingArea();
			break;
		case EditArea.State.SelectSign:
			UpdateSelectSign();
			break;
		}
	}
}
