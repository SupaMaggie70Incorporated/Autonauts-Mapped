using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStateEdit : GameStateBase
{
	[HideInInspector]
	public enum State
	{
		Add,
		Delete,
		Duplicate,
		AddDuplicate,
		Drag,
		Move,
		Moving,
		Area,
		AreaDrag,
		AreaSelected,
		Select,
		Selected,
		Total
	}

	private enum UndoAction
	{
		Add,
		Delete,
		Move,
		Total
	}

	public static GameStateEdit Instance;

	[HideInInspector]
	public State m_State;

	private GameObject m_BinIcon;

	private Image m_BinImage;

	public static ObjectType m_CurrentObjectType;

	public BuildingPalette m_BuildingPalette;

	private TileCoord m_DragStartPosition;

	private TileCoord m_DragOldPosition;

	private bool m_DragVertical;

	private bool m_Drag2D;

	private List<Building> m_DragModels;

	private bool m_DragBad;

	private Building m_MoveBuilding;

	private Vector3 m_MoveMousePosition;

	private int m_MoveCursorRotation;

	private TileCoord m_ModeStartPosition;

	private bool m_Deleting;

	private ObjectType m_DeleteType;

	private Vector3 m_DeleteMousePosition;

	private TileCoord m_AreaStartCoord;

	private TileCoord m_AreaLastCoord;

	private AreaIndicator m_AreaIndicator;

	private Dictionary<Building, int> m_AreaSelectBuildings;

	private bool m_AreaSelected;

	private UndoAction m_UndoAction;

	private List<Building> m_UndoBuildings;

	private List<CursorModel> m_UndoModels;

	private TileCoord m_UndoMove;

	private int m_UndoRotation;

	private bool m_UndoGroup;

	private TileCoord m_DuplicatePosition;

	private bool m_ActionSuccess;

	private int m_CursorRotation;

	private List<ObjectType> m_SearchTypes;

	private bool m_ButtonHover;

	private BaseClass m_TargetObject;

	private bool m_CTRLHeld;

	private int m_AreaSelectLeft;

	private int m_AreaSelectRight;

	private int m_AreaSelectTop;

	private int m_AreaSelectBottom;

	public static void Init()
	{
		m_CurrentObjectType = ObjectTypeList.m_Total;
	}

	protected new void Awake()
	{
		Instance = this;
		base.Awake();
		QuestManager.Instance.AddEvent(QuestEvent.Type.EditMode, Bot: false, null, null);
		Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Buildings/BuildingPalette", typeof(GameObject));
		m_BuildingPalette = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, menusRootTransform).GetComponent<BuildingPalette>();
		m_BuildingPalette.GetComponent<RectTransform>().anchoredPosition = default(Vector2);
		GameObject original2 = (GameObject)Resources.Load("Prefabs/Hud/BinIcon", typeof(GameObject));
		m_BinIcon = Object.Instantiate(original2, new Vector3(0f, 0f, 0f), Quaternion.identity, menusRootTransform);
		m_BinImage = m_BinIcon.transform.Find("Image").GetComponent<Image>();
		GameObject original3 = (GameObject)Resources.Load("Prefabs/AreaIndicator", typeof(GameObject));
		m_AreaIndicator = Object.Instantiate(original3, new Vector3(0f, 0f, 0f), Quaternion.identity, MapManager.Instance.m_MiscRootTransform).GetComponent<AreaIndicator>();
		m_AreaIndicator.gameObject.SetActive(value: false);
		m_AreaSelectBuildings = new Dictionary<Building, int>();
		SetCurrentObjectType(m_CurrentObjectType);
		HudManager.Instance.RolloversEnabled(Enabled: false);
		HudManager.Instance.SetHudButtonsActive(Active: false);
		ModeButton.Get(ModeButton.Type.BuildingPalette).SetNew(New: false);
		m_DragModels = new List<Building>();
		m_UndoBuildings = new List<Building>();
		m_UndoModels = new List<CursorModel>();
		m_UndoAction = UndoAction.Total;
		UpdateButtons();
		MaterialManager.Instance.SetDesaturation(Desaturated: true, BuildingsDesaturated: false);
		PlotManager.Instance.SetDesaturation(Desaturated: true);
		AudioManager.Instance.DuckWorldSFX(0.2f);
		SetupSelectedTypes();
		SetState(State.Add);
	}

	protected new void OnDestroy()
	{
		SetState(State.Total);
		LinkedSystemManager.Instance.EndEditMode();
		CustomStandaloneInputModule.Instance.SetIgnoreUI(Ignore: false);
		QuestManager.Instance.AddEvent(QuestEvent.Type.EndEditMode, Bot: false, null, null);
		AudioManager.Instance.RestoreWorldSFX();
		MaterialManager.Instance.SetDesaturation(Desaturated: false, BuildingsDesaturated: false);
		PlotManager.Instance.SetDesaturation(Desaturated: false);
		ClearUndo();
		UnHighlightAreaBuildings();
		DestroyDragModels();
		Cursor.Instance.SetModel(ObjectTypeList.m_Total);
		HudManager.Instance.RolloversEnabled(Enabled: true);
		HudManager.Instance.SetHudButtonsActive(Active: true);
		base.OnDestroy();
		Object.Destroy(m_BuildingPalette.gameObject);
		Object.Destroy(m_BinIcon.gameObject);
	}

	private void SetupSelectedTypes()
	{
		m_SearchTypes = new List<ObjectType>();
		foreach (ObjectType type in Converter.m_Types)
		{
			if (type != ObjectType.ConverterFoundation)
			{
				m_SearchTypes.Add(type);
			}
		}
		m_SearchTypes.Add(ObjectType.StoneHeads);
		m_SearchTypes.Add(ObjectType.ResearchStationCrude);
		m_SearchTypes.Add(ObjectType.ResearchStationCrude2);
		m_SearchTypes.Add(ObjectType.ResearchStationCrude3);
		m_SearchTypes.Add(ObjectType.ResearchStationCrude4);
		m_SearchTypes.Add(ObjectType.ResearchStationCrude5);
		m_SearchTypes.Add(ObjectType.ResearchStationCrude6);
		m_SearchTypes.Add(ObjectType.Aquarium);
		m_SearchTypes.Add(ObjectType.AquariumGood);
		m_SearchTypes.Add(ObjectType.SilkwormStation);
		m_SearchTypes.Add(ObjectType.SpacePort);
		foreach (ObjectType type2 in Storage.m_Types)
		{
			m_SearchTypes.Add(type2);
		}
		foreach (KeyValuePair<ObjectType, string> modIDOriginal in ModManager.Instance.ModConverterClass.ModIDOriginals)
		{
			m_SearchTypes.Add(modIDOriginal.Key);
		}
		foreach (KeyValuePair<ObjectType, string> modIDOriginal2 in ModManager.Instance.ModBuildingClass.ModIDOriginals)
		{
			m_SearchTypes.Add(modIDOriginal2.Key);
		}
	}

	public override void Pushed(GameStateManager.State NewState)
	{
		SetState(State.Add);
		base.Pushed(NewState);
		m_BuildingPalette.gameObject.SetActive(value: false);
		m_BinIcon.SetActive(value: false);
		Cursor.Instance.SetModel(ObjectTypeList.m_Total);
	}

	public override void Popped(GameStateManager.State NewState)
	{
		base.Popped(NewState);
		m_BuildingPalette.gameObject.SetActive(value: true);
		UpdateAddCursor();
	}

	private void UpdateButtons()
	{
		if (m_UndoAction == UndoAction.Total)
		{
			m_BuildingPalette.SetUndoAvailable(Available: false);
		}
		else
		{
			m_BuildingPalette.SetUndoAvailable(Available: true);
		}
	}

	private void UpdateAddCursor()
	{
		ObjectType objectType = m_CurrentObjectType;
		ObjectType objectType2 = ResourceManager.Instance.FindBuildingResource(objectType);
		if (objectType2 != ObjectTypeList.m_Total)
		{
			objectType = objectType2;
		}
		Cursor.Instance.m_ModelIdentifier = ObjectTypeList.m_Total;
		bool cTRLHeld = MyInputManager.Instance.GetCTRLHeld();
		Cursor.Instance.SetModel(objectType, cTRLHeld);
	}

	private void SetAddMode()
	{
		UpdateAddCursor();
		m_BinIcon.SetActive(value: false);
		UnHighlightAreaBuildings();
	}

	private void SetDeleteMode()
	{
		Cursor.Instance.SetModel(ObjectTypeList.m_Total);
		UpdateBinIconPosition(Show: false);
		m_Deleting = false;
		m_DeleteType = ObjectTypeList.m_Total;
	}

	private void SetDuplicateMode()
	{
		Cursor.Instance.SetModel(ObjectTypeList.m_Total);
		UpdateBinIconPosition(Show: false);
	}

	private void SetDragMode()
	{
		if (Floor2D.GetIsTypeFloor2D(m_CurrentObjectType))
		{
			m_Drag2D = true;
		}
		else
		{
			m_Drag2D = false;
		}
		Cursor.Instance.SetModel(ObjectTypeList.m_Total);
		m_BinIcon.SetActive(value: false);
		m_DragOldPosition = m_DragStartPosition;
		CreateDragModels(m_DragOldPosition);
		CustomStandaloneInputModule.Instance.SetIgnoreUI(Ignore: true);
	}

	private void SetMoveMode()
	{
		Cursor.Instance.SetModel(ObjectTypeList.m_Total);
		UpdateBinIconPosition(Show: false);
		m_MoveBuilding = null;
		Cursor.Instance.SetModelBadException(null);
	}

	private void SetAreaMode()
	{
		m_BinIcon.SetActive(value: false);
		Cursor.Instance.SetModel(ObjectTypeList.m_Total);
	}

	private void UnHighlightAreaBuildings()
	{
		foreach (KeyValuePair<Building, int> areaSelectBuilding in m_AreaSelectBuildings)
		{
			Building key = areaSelectBuilding.Key;
			if ((bool)key)
			{
				key.SetHighlight(Highlighted: false);
			}
		}
		m_AreaSelectBuildings.Clear();
	}

	private void EndDrag()
	{
		DestroyDragModels();
		m_BuildingPalette.UpdateBuiltCount();
		CustomStandaloneInputModule.Instance.SetIgnoreUI(Ignore: false);
	}

	private void EndAddDuplicate()
	{
		m_AreaSelectBuildings.Clear();
		Cursor.Instance.SetModelBadException(null);
		Cursor.Instance.SetModel(ObjectTypeList.m_Total);
	}

	private void EndAreaDragMode()
	{
		m_AreaIndicator.gameObject.SetActive(value: false);
		CustomStandaloneInputModule.Instance.SetIgnoreUI(Ignore: false);
	}

	private void SetAreaDragMode()
	{
		m_BinIcon.SetActive(value: false);
		m_AreaIndicator.gameObject.SetActive(value: true);
		CustomStandaloneInputModule.Instance.SetIgnoreUI(Ignore: true);
	}

	private void SetAreaSelectedMode()
	{
	}

	private void EndMoving()
	{
		Cursor.Instance.SetModelBadException(null);
		if (m_AreaSelectBuildings.Count > 0)
		{
			foreach (KeyValuePair<Building, int> areaSelectBuilding in m_AreaSelectBuildings)
			{
				areaSelectBuilding.Key.SetBeingMoved(Moved: false);
			}
			return;
		}
		m_MoveBuilding.SetBeingMoved(Moved: false);
	}

	private void SetSelectMode()
	{
		m_BinIcon.SetActive(value: false);
		Cursor.Instance.SetModel(ObjectTypeList.m_Total);
		MaterialManager.Instance.SetDesaturation(Desaturated: true, BuildingsDesaturated: true);
		ModelManager.Instance.SetSearchTypesHighlight(m_SearchTypes, Desaturated: true, AndOccluded: false);
	}

	private void EndSelect()
	{
		ModelManager.Instance.SetSearchTypesHighlight(m_SearchTypes, Desaturated: false, AndOccluded: false);
		MaterialManager.Instance.SetDesaturation(Desaturated: true, BuildingsDesaturated: false);
	}

	private void SetSelectedMode()
	{
		m_BinIcon.SetActive(value: false);
		Cursor.Instance.SetModel(ObjectTypeList.m_Total);
	}

	private void EndSelected()
	{
		m_BuildingPalette.SetSelectedBuilding(null);
	}

	public void SetState(State NewState)
	{
		switch (m_State)
		{
		case State.Drag:
			EndDrag();
			break;
		case State.AreaDrag:
			EndAreaDragMode();
			break;
		case State.AddDuplicate:
			EndAddDuplicate();
			break;
		case State.Moving:
			EndMoving();
			break;
		case State.Select:
			EndSelect();
			break;
		case State.Selected:
			EndSelected();
			break;
		}
		m_State = NewState;
		switch (m_State)
		{
		case State.Add:
			SetAddMode();
			break;
		case State.Delete:
			SetDeleteMode();
			break;
		case State.Duplicate:
			SetDuplicateMode();
			break;
		case State.Move:
			SetMoveMode();
			break;
		case State.Drag:
			SetDragMode();
			break;
		case State.Area:
			SetAreaMode();
			break;
		case State.AreaDrag:
			SetAreaDragMode();
			break;
		case State.AreaSelected:
			SetAreaSelectedMode();
			break;
		case State.Select:
			SetSelectMode();
			break;
		case State.Selected:
			SetSelectedMode();
			break;
		}
		m_BuildingPalette.GetComponent<BuildingPalette>().UpdateMode(m_State);
	}

	public void ToggleDelete()
	{
		if (m_State == State.Delete)
		{
			SetState(State.Add);
		}
		else
		{
			SetState(State.Delete);
		}
	}

	public void ToggleDuplicate()
	{
		if (m_State == State.Duplicate || m_State == State.AddDuplicate)
		{
			SetState(State.Add);
		}
		else
		{
			SetState(State.Duplicate);
		}
	}

	public void ToggleMove()
	{
		if (m_State == State.Move)
		{
			SetState(State.Add);
		}
		else
		{
			SetState(State.Move);
		}
	}

	public void ToggleArea()
	{
		if (m_State == State.Area || m_State == State.AreaSelected)
		{
			SetState(State.Add);
		}
		else
		{
			SetState(State.Area);
		}
	}

	public void ToggleSelect()
	{
		if (m_State == State.Select)
		{
			SetState(State.Add);
		}
		else
		{
			SetState(State.Select);
		}
	}

	public bool IsInDeleteMode()
	{
		return m_State == State.Delete;
	}

	public void SetCurrentObjectType(ObjectType ModelType)
	{
		m_CurrentObjectType = ModelType;
		if (m_State != 0)
		{
			SetState(State.Add);
		}
		UpdateAddCursor();
	}

	private bool UndoAdd()
	{
		bool flag = true;
		foreach (Building undoBuilding in m_UndoBuildings)
		{
			if (!(bool)undoBuilding.GetActionInfo(new GetActionInfo(GetAction.IsDeletable)))
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			foreach (Building undoBuilding2 in m_UndoBuildings)
			{
				DestroyBuilding(undoBuilding2);
			}
			m_BuildingPalette.UpdateBuiltCounts();
			return true;
		}
		return false;
	}

	private bool UndoDelete()
	{
		foreach (Building undoBuilding in m_UndoBuildings)
		{
			if (MapManager.Instance.CheckBuildingIntersection(undoBuilding, new List<BaseClass>(), out var _))
			{
				return false;
			}
		}
		foreach (Building undoBuilding2 in m_UndoBuildings)
		{
			undoBuilding2.gameObject.SetActive(value: true);
			int rotation = undoBuilding2.m_Rotation;
			undoBuilding2.Restart();
			undoBuilding2.SetRotation(rotation);
			MapManager.Instance.AddBuilding(undoBuilding2);
			if (undoBuilding2.m_TypeIdentifier == ObjectType.ConverterFoundation)
			{
				undoBuilding2.GetComponent<ConverterFoundation>().SetupNewBuilding();
			}
			if (!BuildingManager.Instance.GetIsInstantBuild() && undoBuilding2.m_TypeIdentifier != ObjectType.ConverterFoundation)
			{
				ResourceManager.Instance.AddResource(undoBuilding2.m_TypeIdentifier, -1);
			}
		}
		foreach (Building undoBuilding3 in m_UndoBuildings)
		{
			BuildingManager.Instance.RefreshBuilding(undoBuilding3);
		}
		m_BuildingPalette.UpdateBuiltCounts();
		return true;
	}

	private bool UndoMove()
	{
		List<MovingBuilding> list = new List<MovingBuilding>();
		int num = 0;
		foreach (CursorModel undoModel in m_UndoModels)
		{
			list.Add(new MovingBuilding(0, undoModel.m_Model.GetComponent<Building>(), undoModel.m_Position, undoModel.m_Rotation));
			num++;
		}
		BuildingManager.Instance.MoveBuildings(list);
		return true;
	}

	public void Undo()
	{
		if (m_UndoAction == UndoAction.Total)
		{
			return;
		}
		bool flag = false;
		switch (m_UndoAction)
		{
		case UndoAction.Add:
			flag = UndoAdd();
			break;
		case UndoAction.Delete:
			flag = UndoDelete();
			if (!flag)
			{
				m_BuildingPalette.ShowObstruction();
			}
			break;
		case UndoAction.Move:
			flag = UndoMove();
			break;
		}
		if (flag)
		{
			m_UndoAction = UndoAction.Total;
			m_UndoBuildings.Clear();
			m_UndoModels.Clear();
			m_BuildingPalette.SetUndoAvailable(Available: false);
		}
	}

	public void UpdateUndoStillValid()
	{
		if (m_UndoAction == UndoAction.Total)
		{
			return;
		}
		for (int num = m_UndoBuildings.Count - 1; num >= 0; num--)
		{
			if (m_UndoBuildings[num] == null)
			{
				foreach (CursorModel undoModel in m_UndoModels)
				{
					if (undoModel.m_Model == m_UndoBuildings[num])
					{
						m_UndoModels.Remove(undoModel);
						break;
					}
				}
				m_UndoBuildings.RemoveAt(num);
			}
		}
		if (m_UndoBuildings.Count == 0)
		{
			ClearUndo();
		}
	}

	private void ClearUndo()
	{
		if (m_UndoAction == UndoAction.Delete)
		{
			foreach (Building undoBuilding in m_UndoBuildings)
			{
				undoBuilding.Delete();
			}
		}
		m_UndoBuildings.Clear();
		m_UndoModels.Clear();
		m_UndoAction = UndoAction.Total;
		m_BuildingPalette.SetUndoAvailable(Available: false);
	}

	private void AddUndoAction(UndoAction NewAction, Building NewBuilding)
	{
		ClearUndo();
		m_UndoAction = NewAction;
		m_UndoBuildings.Add(NewBuilding);
		m_BuildingPalette.SetUndoAvailable(Available: true);
		m_UndoGroup = false;
	}

	private void AddUndoAction(UndoAction NewAction, List<Building> NewBuildings)
	{
		ClearUndo();
		m_UndoAction = NewAction;
		foreach (Building NewBuilding in NewBuildings)
		{
			m_UndoBuildings.Add(NewBuilding);
		}
		m_BuildingPalette.SetUndoAvailable(Available: true);
		m_UndoGroup = true;
	}

	private void AddUndoAction(UndoAction NewAction, Dictionary<Building, int> NewBuildings)
	{
		ClearUndo();
		m_UndoAction = NewAction;
		foreach (KeyValuePair<Building, int> NewBuilding in NewBuildings)
		{
			m_UndoBuildings.Add(NewBuilding.Key);
		}
		m_BuildingPalette.SetUndoAvailable(Available: true);
		m_UndoGroup = true;
	}

	private void AddUndoAction(UndoAction NewAction, Building NewBuilding, TileCoord Move, int Rotation)
	{
		ClearUndo();
		m_UndoAction = NewAction;
		m_UndoBuildings.Add(NewBuilding);
		m_UndoModels.Add(new CursorModel(NewBuilding, NewBuilding.m_TileCoord, NewBuilding.m_Rotation));
		m_UndoMove = Move;
		m_UndoRotation = Rotation;
		m_BuildingPalette.SetUndoAvailable(Available: true);
		m_UndoGroup = false;
	}

	private void AddUndoAction(UndoAction NewAction, Dictionary<Building, int> NewBuildings, TileCoord Move, int Rotation)
	{
		ClearUndo();
		m_UndoAction = NewAction;
		foreach (KeyValuePair<Building, int> NewBuilding in NewBuildings)
		{
			m_UndoBuildings.Add(NewBuilding.Key);
			m_UndoModels.Add(new CursorModel(NewBuilding.Key, NewBuilding.Key.m_TileCoord, NewBuilding.Key.m_Rotation));
		}
		m_UndoMove = Move;
		m_UndoRotation = Rotation;
		m_BuildingPalette.SetUndoAvailable(Available: true);
		m_UndoGroup = true;
	}

	private bool DestroyBuilding(Actionable TargetObject)
	{
		TargetObject.GetComponent<Building>().PlayerDeleted();
		return BuildingManager.Instance.DestroyBuilding(TargetObject);
	}

	private Building AddBuilding(TileCoord TilePosition, ObjectType NewType, int Rotation, bool ForceBlueprint = false, Building OriginalBuilding = null, bool IncludeUpgrades = true)
	{
		Building result = BuildingManager.Instance.AddBuilding(TilePosition, NewType, Rotation, OriginalBuilding, Instant: false, ForceBlueprint, IncludeUpgrades);
		m_BuildingPalette.UpdateBuiltCounts();
		return result;
	}

	public static bool GetSingleBuildingTypeExists(ObjectType NewType)
	{
		int num = 1;
		if (m_CurrentObjectType == NewType)
		{
			num = 2;
		}
		if (ObjectTypeList.m_ObjectTypeCounts[(int)NewType] >= num)
		{
			return true;
		}
		Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("ConverterFoundation");
		if (collection != null)
		{
			foreach (KeyValuePair<BaseClass, int> item in collection)
			{
				if (item.Key.GetComponent<ConverterFoundation>().m_NewBuilding.m_TypeIdentifier == NewType)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool GetIsSingleBuildingType(ObjectType NewType)
	{
		if (CheatManager.Instance.m_CheatsEnabled)
		{
			return false;
		}
		if (NewType == ObjectType.FolkSeedPod || NewType == ObjectType.TranscendBuilding || NewType == ObjectType.SpacePort)
		{
			return true;
		}
		return false;
	}

	private void UpdateAddCursor(bool Show)
	{
		if (m_ButtonHover)
		{
			return;
		}
		if (m_CurrentObjectType != ObjectTypeList.m_Total)
		{
			Sprite sprite = (Sprite)Resources.Load("Textures/Hud/Buttons/BlueprintButton", typeof(Sprite));
			m_BinIcon.GetComponent<Image>().sprite = sprite;
			ObjectType objectType = m_CurrentObjectType;
			ObjectType objectType2 = ResourceManager.Instance.FindBuildingResource(objectType);
			if (objectType2 != ObjectTypeList.m_Total)
			{
				objectType = objectType2;
			}
			sprite = IconManager.Instance.GetIcon(objectType);
			m_BinImage.GetComponent<Image>().sprite = sprite;
			m_BinImage.gameObject.SetActive(value: true);
			m_BinIcon.transform.localPosition = HudManager.Instance.ScreenToCanvas(Input.mousePosition) + new Vector3(40f, 0f, 0f);
			m_BinIcon.SetActive(Show);
			m_BinIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(75f, 75f);
		}
		else
		{
			m_BinIcon.SetActive(value: false);
		}
	}

	public void CursorHoverButton(bool Hover)
	{
		m_ButtonHover = Hover;
		if (Hover)
		{
			m_BinIcon.SetActive(value: false);
		}
		else if (m_CurrentObjectType != ObjectTypeList.m_Total && m_State != State.Select && m_State != State.Selected)
		{
			m_BinIcon.SetActive(value: true);
		}
		else
		{
			m_BinIcon.SetActive(value: false);
		}
	}

	private void UpdateKeyPresses()
	{
		if (!m_EventSystem.IsUIInUse())
		{
			CameraManager.Instance.UpdateInput();
			if (MyInputManager.m_Rewired.GetButtonDown("EditDelete"))
			{
				SetState(State.Delete);
			}
			if (MyInputManager.m_Rewired.GetButtonDown("EditMove"))
			{
				SetState(State.Move);
			}
			if (MyInputManager.m_Rewired.GetButtonDown("SelectBuilding"))
			{
				SetState(State.Select);
			}
			if (MyInputManager.m_Rewired.GetButtonDown("Quit") || MyInputManager.m_Rewired.GetButtonDown("Edit"))
			{
				AudioManager.Instance.StartEvent("UICloseWindow");
				GameStateManager.Instance.SetState(GameStateManager.State.Normal);
			}
		}
	}

	private bool UpdateCursorType()
	{
		if (GameOptionsManager.Instance.m_Options.m_GameMode == GameOptions.GameMode.ModeCreative)
		{
			return false;
		}
		if (GetIsSingleBuildingType(m_CurrentObjectType))
		{
			return false;
		}
		bool cTRLHeld = MyInputManager.Instance.GetCTRLHeld();
		if (cTRLHeld != m_CTRLHeld)
		{
			m_CTRLHeld = cTRLHeld;
			ObjectType modelIdentifier = Cursor.Instance.m_ModelIdentifier;
			Cursor.Instance.SetModel(ObjectTypeList.m_Total);
			Cursor.Instance.SetModel(modelIdentifier, m_CTRLHeld);
			return true;
		}
		return false;
	}

	private void UpdateAdd()
	{
		if (!m_EventSystem.IsUIInFocus())
		{
			bool flag = false;
			UpdateCursorType();
			if (!GetIsSingleBuildingType(m_CurrentObjectType) || !GetSingleBuildingTypeExists(m_CurrentObjectType))
			{
				TileCoord tileCoord = default(TileCoord);
				bool flag2 = true;
				GameObject gameObject = TestMouseCollision(out var CollisionPoint, TestTiles: false, TestBuildings: true, TestWorkers: false, TestMisc: false, TestWorkersDead: false);
				if ((bool)gameObject && m_CurrentObjectType != ObjectTypeList.m_Total)
				{
					gameObject = GetRootObject(gameObject);
					if ((bool)gameObject && (bool)gameObject.GetComponent<Actionable>() && (bool)gameObject.GetComponent<Actionable>().GetActionInfo(new GetActionInfo(GetAction.IsPickable)))
					{
						Building component = gameObject.GetComponent<Building>();
						if (component.CanBuildTypeUpon(m_CurrentObjectType))
						{
							tileCoord = component.m_TileCoord;
							Cursor.Instance.TargetTile(component.m_TileCoord);
							Cursor.Instance.SetRotation(component.m_Rotation);
							flag2 = false;
							flag = true;
						}
					}
				}
				if (flag2)
				{
					gameObject = TestMouseCollision(out CollisionPoint, TestTiles: true, TestBuildings: false, TestWorkers: false, TestMisc: false, TestWorkersDead: false);
					if ((bool)gameObject && m_CurrentObjectType != ObjectTypeList.m_Total && (bool)gameObject.GetComponent<PlotRoot>())
					{
						tileCoord = new TileCoord(CollisionPoint);
						Building buildingFootprint = TileManager.Instance.GetTile(tileCoord).m_BuildingFootprint;
						if ((bool)buildingFootprint && buildingFootprint.CanBuildTypeUpon(m_CurrentObjectType))
						{
							tileCoord = buildingFootprint.m_TileCoord;
							Cursor.Instance.TargetTile(buildingFootprint.m_TileCoord);
							Cursor.Instance.SetRotation(buildingFootprint.m_Rotation);
							flag = true;
							flag2 = false;
						}
						if (flag2)
						{
							Cursor.Instance.TargetTile(tileCoord);
							Cursor.Instance.SetRotation(m_CursorRotation);
							flag = true;
						}
					}
				}
				if (m_CurrentObjectType != ObjectTypeList.m_Total)
				{
					if (!flag)
					{
						Cursor.Instance.NoTarget();
					}
					else if (TestMouseButtonDown(0))
					{
						if (Cursor.Instance.m_ModelBad)
						{
							AudioManager.Instance.StartEvent("UIBlueprintAddedFail");
						}
						else if (Building.GetIsTypeDragable(m_CurrentObjectType))
						{
							m_DragStartPosition = tileCoord;
							SetState(State.Drag);
						}
						else
						{
							Building newBuilding = AddBuilding(tileCoord, m_CurrentObjectType, Cursor.Instance.GetRotation(), m_CTRLHeld);
							AddUndoAction(UndoAction.Add, newBuilding);
							AudioManager.Instance.StartEvent("UIBlueprintAdded");
							if (GetIsSingleBuildingType(m_CurrentObjectType) && GetSingleBuildingTypeExists(m_CurrentObjectType))
							{
								Cursor.Instance.SetModel(ObjectTypeList.m_Total);
								SetCurrentObjectType(ObjectTypeList.m_Total);
								m_BuildingPalette.SetCurrentType(ObjectTypeList.m_Total);
							}
							QuestManager.Instance.AddEvent(QuestEvent.Type.AddBlueprint, Bot: false, m_CurrentObjectType, null);
							UpdateAddCursor();
						}
					}
					else if (TestMouseButtonDown(1))
					{
						Tile tile = TileManager.Instance.GetTile(tileCoord);
						Building building = tile.m_Building;
						if (building == null && (bool)tile.m_Floor)
						{
							building = tile.m_Floor;
						}
						if ((bool)building)
						{
							building = building.GetTopBuilding();
							if (building.m_TypeIdentifier == ObjectType.ConverterFoundation && building.GetComponent<Converter>().m_Ingredients.Count == 0)
							{
								AudioManager.Instance.StartEvent("UIBlueprintDeleted");
								DestroyBuilding(building);
								AddUndoAction(UndoAction.Delete, building);
								m_BuildingPalette.UpdateBuiltCounts();
							}
						}
					}
					else if (MyInputManager.m_Rewired.GetButtonDown("Rotate") && Building.GetIsTypeRotatable(m_CurrentObjectType))
					{
						Cursor.Instance.SetRotation((Cursor.Instance.GetRotation() + 1) % 4);
						m_CursorRotation = Cursor.Instance.GetRotation();
					}
				}
			}
			UpdateAddCursor(Show: false);
		}
		else
		{
			Cursor.Instance.NoTarget();
			UpdateAddCursor(Show: true);
		}
		UpdateKeyPresses();
	}

	public void ConfirmDelete()
	{
		m_DeleteType = m_TargetObject.GetComponent<Building>().m_TypeIdentifier;
		m_DeleteMousePosition = Input.mousePosition;
		Building component = m_TargetObject.GetComponent<Building>();
		if ((bool)component && m_AreaSelectBuildings.ContainsKey(component))
		{
			Dictionary<Building, int> dictionary = new Dictionary<Building, int>();
			foreach (KeyValuePair<Building, int> areaSelectBuilding in m_AreaSelectBuildings)
			{
				if ((bool)areaSelectBuilding.Key.GetActionInfo(new GetActionInfo(GetAction.IsDeletable)))
				{
					dictionary.Add(areaSelectBuilding.Key, areaSelectBuilding.Value);
				}
			}
			foreach (KeyValuePair<Building, int> item in dictionary)
			{
				Building key = item.Key;
				DestroyBuilding(key);
			}
			AddUndoAction(UndoAction.Delete, dictionary);
			m_AreaSelectBuildings.Clear();
			AudioManager.Instance.StartEvent("UIBlueprintDeleted");
		}
		else
		{
			UnHighlightAreaBuildings();
			new List<Building>().Add(component);
			if (DestroyBuilding(component))
			{
				AudioManager.Instance.StartEvent("UIBlueprintDeleted");
			}
			AddUndoAction(UndoAction.Delete, component);
		}
		m_BuildingPalette.UpdateBuiltCounts();
	}

	private bool GetTargetObjectPartBuilt()
	{
		Building component = m_TargetObject.GetComponent<Building>();
		if ((bool)component && m_AreaSelectBuildings.ContainsKey(component))
		{
			new Dictionary<Building, int>();
			foreach (KeyValuePair<Building, int> areaSelectBuilding in m_AreaSelectBuildings)
			{
				if ((bool)areaSelectBuilding.Key.GetActionInfo(new GetActionInfo(GetAction.IsDeletable)) && areaSelectBuilding.Key.m_TypeIdentifier == ObjectType.ConverterFoundation && !areaSelectBuilding.Key.GetComponent<ConverterFoundation>().CanInstantDelete())
				{
					return true;
				}
			}
		}
		else if ((bool)component && component.m_TypeIdentifier == ObjectType.ConverterFoundation && !component.GetComponent<ConverterFoundation>().CanInstantDelete())
		{
			return true;
		}
		return false;
	}

	private void UpdateDelete()
	{
		if (m_Deleting != Input.GetMouseButton(0))
		{
			if (m_Deleting)
			{
				m_DeleteType = ObjectTypeList.m_Total;
			}
			m_Deleting = Input.GetMouseButton(0);
			if (m_Deleting && !m_ActionSuccess)
			{
				AudioManager.Instance.StartEvent("UIBuildingDeleteFail");
				m_DeleteType = ObjectTypeList.m_Total;
			}
		}
		if (!m_EventSystem.IsUIInFocus())
		{
			bool flag = false;
			UpdateBinIconPosition(Show: true);
			Actionable actionable = null;
			TileCoord tileCoord = default(TileCoord);
			Vector3 CollisionPoint;
			GameObject gameObject = TestMouseCollision(out CollisionPoint, TestTiles: true, TestBuildings: true, TestWorkers: false, TestMisc: false, TestWorkersDead: false);
			m_ActionSuccess = true;
			if ((bool)gameObject)
			{
				if ((bool)gameObject && (bool)gameObject.GetComponent<PlotRoot>())
				{
					tileCoord = new TileCoord(CollisionPoint);
					Tile tile = TileManager.Instance.GetTile(tileCoord);
					if ((bool)tile.m_BuildingFootprint)
					{
						gameObject = tile.m_BuildingFootprint.gameObject;
					}
				}
				else
				{
					gameObject = GetRootObject(gameObject);
				}
				if ((bool)gameObject && (bool)gameObject.GetComponent<Building>())
				{
					tileCoord = gameObject.GetComponent<Building>().m_TileCoord;
					if ((bool)gameObject.GetComponent<Floor>() || ((bool)gameObject.GetComponent<ConverterFoundation>() && (bool)gameObject.GetComponent<ConverterFoundation>().m_NewBuilding.GetComponent<Floor>()))
					{
						gameObject.GetComponent<Building>();
					}
					else
					{
						gameObject = TileManager.Instance.GetTile(tileCoord).m_Building.GetTopBuilding().gameObject;
					}
					if (m_DeleteType == ObjectTypeList.m_Total || m_DeleteType == gameObject.GetComponent<Building>().m_TypeIdentifier)
					{
						actionable = gameObject.GetComponent<Actionable>();
						Cursor.Instance.TargetTile(tileCoord);
						Cursor.Instance.Target(gameObject);
						flag = true;
						m_ActionSuccess = (bool)gameObject.GetComponent<Building>().GetActionInfo(new GetActionInfo(GetAction.IsDeletable));
					}
				}
			}
			if (!GetIsAreaSelectUsable(Delete: true))
			{
				m_ActionSuccess = false;
			}
			Cursor.Instance.SetUsable(m_ActionSuccess);
			if (actionable == null || actionable.GetComponent<Building>() == null || !m_AreaSelectBuildings.ContainsKey(actionable.GetComponent<Building>()))
			{
				HighlightObject(actionable);
			}
			if (!flag)
			{
				Cursor.Instance.NoTarget();
				if (Input.GetMouseButton(0))
				{
					UnHighlightAreaBuildings();
				}
			}
			else
			{
				float magnitude = (m_DeleteMousePosition - Input.mousePosition).magnitude;
				if (m_Deleting && (m_DeleteType == ObjectTypeList.m_Total || magnitude > 5f) && m_ActionSuccess)
				{
					m_TargetObject = actionable;
					if (GetTargetObjectPartBuilt())
					{
						GameStateManager.Instance.PushState(GameStateManager.State.Confirm);
						GameStateManager.Instance.GetCurrentState().GetComponent<GameStateConfirm>().SetConfirm(ConfirmDelete, "ConfirmDeleteBuilding", "ConfirmDeleteBuildingDescription");
					}
					else
					{
						ConfirmDelete();
					}
				}
			}
		}
		else
		{
			m_BinIcon.SetActive(value: false);
			HighlightObject(null);
			Cursor.Instance.NoTarget();
		}
		if (!m_EventSystem.IsUIInUse())
		{
			CameraManager.Instance.UpdateInput();
		}
		if (MyInputManager.m_Rewired.GetButtonDown("Quit") || MyInputManager.m_Rewired.GetButtonDown("EditDelete"))
		{
			HighlightObject(null);
			AudioManager.Instance.StartEvent("UICloseWindow");
			GameStateManager.Instance.SetState(GameStateManager.State.Normal);
		}
		if (MyInputManager.m_Rewired.GetButtonDown("Edit"))
		{
			AudioManager.Instance.StartEvent("UICloseWindow");
			GameStateManager.Instance.SetState(GameStateManager.State.Normal);
		}
		if (MyInputManager.m_Rewired.GetButtonDown("EditMove"))
		{
			HighlightObject(null);
			SetState(State.Move);
		}
	}

	private void UpdateDuplicate()
	{
		if (!m_EventSystem.IsUIInFocus())
		{
			bool flag = false;
			UpdateBinIconPosition(Show: true);
			Actionable actionable = null;
			TileCoord tileCoord = default(TileCoord);
			Vector3 CollisionPoint;
			GameObject gameObject = TestMouseCollision(out CollisionPoint, TestTiles: true, TestBuildings: true, TestWorkers: false, TestMisc: false, TestWorkersDead: false);
			m_ActionSuccess = true;
			if ((bool)gameObject)
			{
				if ((bool)gameObject && (bool)gameObject.GetComponent<PlotRoot>())
				{
					tileCoord = new TileCoord(CollisionPoint);
					Tile tile = TileManager.Instance.GetTile(tileCoord);
					if ((bool)tile.m_BuildingFootprint)
					{
						gameObject = tile.m_BuildingFootprint.gameObject;
					}
				}
				else
				{
					gameObject = GetRootObject(gameObject);
				}
				if ((bool)gameObject && (bool)gameObject.GetComponent<Building>())
				{
					tileCoord = gameObject.GetComponent<Building>().m_TileCoord;
					Building building;
					if ((bool)gameObject.GetComponent<Floor>() || ((bool)gameObject.GetComponent<ConverterFoundation>() && (bool)gameObject.GetComponent<ConverterFoundation>().m_NewBuilding.GetComponent<Floor>()))
					{
						building = gameObject.GetComponent<Building>();
					}
					else
					{
						building = TileManager.Instance.GetTile(tileCoord).m_Building;
						gameObject = building.GetTopBuilding().gameObject;
					}
					actionable = gameObject.GetComponent<Actionable>();
					Cursor.Instance.TargetTile(tileCoord);
					Cursor.Instance.Target(gameObject);
					flag = true;
					if (!(bool)building.GetActionInfo(new GetActionInfo(GetAction.IsDuplicatable)))
					{
						m_ActionSuccess = false;
					}
				}
			}
			Cursor.Instance.SetUsable(m_ActionSuccess);
			if (actionable == null || actionable.GetComponent<Building>() == null || !m_AreaSelectBuildings.ContainsKey(actionable.GetComponent<Building>()))
			{
				HighlightObject(actionable);
			}
			if (!flag)
			{
				Cursor.Instance.NoTarget();
				if (Input.GetMouseButton(0))
				{
					UnHighlightAreaBuildings();
				}
			}
			else if (Input.GetMouseButtonDown(0))
			{
				if (!m_ActionSuccess)
				{
					AudioManager.Instance.StartEvent("UIBuildingDeleteFail");
				}
				else
				{
					HighlightObject(null);
					List<BaseClass> list = new List<BaseClass>();
					if (actionable.GetComponent<Building>() != null && m_AreaSelectBuildings.ContainsKey(actionable.GetComponent<Building>()))
					{
						List<Building> list2 = new List<Building>();
						foreach (KeyValuePair<Building, int> areaSelectBuilding in m_AreaSelectBuildings)
						{
							Building key = areaSelectBuilding.Key;
							if (!(bool)key.GetActionInfo(new GetActionInfo(GetAction.IsDuplicatable)))
							{
								list2.Add(key);
							}
						}
						foreach (Building item in list2)
						{
							m_AreaSelectBuildings.Remove(item);
						}
						if (m_AreaSelectBuildings.Count == 0)
						{
							return;
						}
						foreach (KeyValuePair<Building, int> areaSelectBuilding2 in m_AreaSelectBuildings)
						{
							Building key2 = areaSelectBuilding2.Key;
							key2.SetHighlight(Highlighted: false);
							list.Add(key2.GetComponent<BaseClass>());
						}
					}
					else
					{
						m_AreaSelectBuildings.Clear();
						if ((bool)actionable.GetActionInfo(new GetActionInfo(GetAction.IsDuplicatable)))
						{
							m_AreaSelectBuildings.Add(actionable.GetComponent<Building>(), 0);
							list.Add(actionable.GetComponent<BaseClass>());
						}
					}
					if (list.Count > 0)
					{
						m_ModeStartPosition = tileCoord;
						GetAreaSelectRectangle();
						m_DuplicatePosition = tileCoord;
						Cursor.Instance.TargetTile(m_DuplicatePosition);
						Cursor.Instance.SetModels(list, m_DuplicatePosition, New: true);
						SetState(State.AddDuplicate);
						m_CursorRotation = Cursor.Instance.GetRotation();
						AudioManager.Instance.StartEvent("UIBlueprintAdded");
					}
				}
			}
		}
		else
		{
			m_BinIcon.SetActive(value: false);
			HighlightObject(null);
			Cursor.Instance.NoTarget();
		}
		if (!m_EventSystem.IsUIInUse())
		{
			CameraManager.Instance.UpdateInput();
		}
		if (MyInputManager.m_Rewired.GetButtonDown("Quit") || MyInputManager.m_Rewired.GetButtonDown("EditDelete"))
		{
			HighlightObject(null);
			AudioManager.Instance.StartEvent("UICloseWindow");
			SetState(State.Add);
		}
		if (MyInputManager.m_Rewired.GetButtonDown("Edit"))
		{
			AudioManager.Instance.StartEvent("UICloseWindow");
			GameStateManager.Instance.SetState(GameStateManager.State.Normal);
		}
	}

	private void UpdateAddDuplicate()
	{
		if (!m_EventSystem.IsUIInFocus())
		{
			bool flag = false;
			UpdateBinIconPosition(Show: true);
			TileCoord tileCoord = default(TileCoord);
			Vector3 CollisionPoint;
			GameObject gameObject = TestMouseCollision(out CollisionPoint, TestTiles: true, TestBuildings: false, TestWorkers: false, TestMisc: false, TestWorkersDead: false);
			if ((bool)gameObject && (bool)gameObject.GetComponent<PlotRoot>())
			{
				tileCoord = new TileCoord(CollisionPoint);
				if (tileCoord.x + m_AreaSelectLeft < 0)
				{
					tileCoord.x = -m_AreaSelectLeft;
				}
				if (tileCoord.x + m_AreaSelectRight >= TileManager.Instance.m_TilesWide)
				{
					tileCoord.x = TileManager.Instance.m_TilesWide - 1 - m_AreaSelectRight;
				}
				if (tileCoord.y + m_AreaSelectTop < 0)
				{
					tileCoord.y = -m_AreaSelectTop;
				}
				if (tileCoord.y + m_AreaSelectBottom >= TileManager.Instance.m_TilesHigh)
				{
					tileCoord.y = TileManager.Instance.m_TilesHigh - 1 - m_AreaSelectBottom;
				}
				Cursor.Instance.TargetTile(tileCoord);
				flag = true;
			}
			if (!flag)
			{
				Cursor.Instance.NoTarget();
			}
			else if (Input.GetMouseButtonDown(0))
			{
				if (Cursor.Instance.m_ModelBad)
				{
					AudioManager.Instance.StartEvent("UIBlueprintAddedFail");
				}
				else
				{
					List<MovingBuilding> movingBuildingList = GetMovingBuildingList();
					List<Building> list = new List<Building>();
					int rotation = Cursor.Instance.GetRotation();
					foreach (MovingBuilding item2 in movingBuildingList)
					{
						Building building = item2.m_Building;
						TileCoord tileCoord2 = building.m_TileCoord - m_DuplicatePosition;
						tileCoord2.Rotate(rotation);
						TileCoord tilePosition = tileCoord + tileCoord2;
						int rotation2 = building.m_Rotation;
						rotation2 = ((m_AreaSelectBuildings.Count <= 1) ? ((rotation2 + rotation - m_CursorRotation) % 4) : ((rotation2 + rotation) % 4));
						ObjectType typeIdentifier = building.m_TypeIdentifier;
						if (typeIdentifier == ObjectType.ConverterFoundation)
						{
							typeIdentifier = building.GetComponent<ConverterFoundation>().m_NewBuilding.m_TypeIdentifier;
						}
						Building item = AddBuilding(tilePosition, typeIdentifier, rotation2, ForceBlueprint: false, building, IncludeUpgrades: false);
						list.Add(item);
					}
					AddUndoAction(UndoAction.Add, list);
					AudioManager.Instance.StartEvent("UIBlueprintAdded");
				}
			}
			else if (MyInputManager.m_Rewired.GetButtonDown("Rotate") && !Cursor.Instance.m_Snapping)
			{
				Cursor.Instance.SetRotation((Cursor.Instance.GetRotation() + 1) % 4);
				Cursor.Instance.GetAreaRectangle(out m_AreaSelectLeft, out m_AreaSelectRight, out m_AreaSelectTop, out m_AreaSelectBottom);
			}
		}
		else
		{
			m_BinIcon.SetActive(value: false);
			HighlightObject(null);
			Cursor.Instance.NoTarget();
		}
		if (!m_EventSystem.IsUIInUse())
		{
			CameraManager.Instance.UpdateInput();
		}
		if (MyInputManager.m_Rewired.GetButtonDown("Quit") || MyInputManager.m_Rewired.GetButtonDown("EditDelete"))
		{
			HighlightObject(null);
			AudioManager.Instance.StartEvent("UICloseWindow");
			SetState(State.Add);
		}
		if (MyInputManager.m_Rewired.GetButtonDown("Edit"))
		{
			AudioManager.Instance.StartEvent("UICloseWindow");
			GameStateManager.Instance.SetState(GameStateManager.State.Normal);
		}
	}

	private void DestroyDragModels()
	{
		foreach (Building dragModel in m_DragModels)
		{
			if (dragModel.m_TypeIdentifier == ObjectType.ConverterFoundation)
			{
				ModelManager.Instance.RestoreStandardMaterials(dragModel.GetComponent<ConverterFoundation>().m_NewBuilding);
				MeshCollider[] componentsInChildren = dragModel.GetComponentsInChildren<MeshCollider>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].enabled = true;
				}
			}
			else
			{
				ModelManager.Instance.RestoreStandardMaterials(dragModel);
			}
			dragModel.StopUsing();
		}
		m_DragModels.Clear();
	}

	private void Cap2DArea(TileCoord TopLeftIn, TileCoord BottomRightIn, out TileCoord TopLeft, out TileCoord BottomRight)
	{
		int num = 100;
		TopLeft = TopLeftIn;
		BottomRight = BottomRightIn;
		int num4;
		do
		{
			int num2 = BottomRight.x - TopLeft.x + 1;
			int num3 = BottomRight.y - TopLeft.y + 1;
			num4 = num3 * num2;
			if (num4 <= num)
			{
				continue;
			}
			if (num3 == 1)
			{
				BottomRight.x--;
			}
			else if (num2 == 1)
			{
				BottomRight.y--;
			}
			else if (num2 > num3)
			{
				if (num3 > 1)
				{
					if (m_DragStartPosition.y >= BottomRight.y)
					{
						BottomRight.y--;
					}
					else
					{
						TopLeft.y++;
					}
				}
				else if (m_DragStartPosition.x >= BottomRight.x)
				{
					BottomRight.x--;
				}
				else
				{
					TopLeft.x--;
				}
			}
			else if (num2 > 1)
			{
				if (m_DragStartPosition.x >= BottomRight.x)
				{
					BottomRight.x--;
				}
				else
				{
					TopLeft.x++;
				}
			}
			else if (m_DragStartPosition.y >= BottomRight.y)
			{
				BottomRight.y--;
			}
			else
			{
				TopLeft.y--;
			}
		}
		while (num4 > num);
	}

	private void CreateDragModels(TileCoord End)
	{
		DestroyDragModels();
		List<TileCoord> list = new List<TileCoord>();
		int num = Cursor.Instance.GetRotation();
		if (m_Drag2D)
		{
			TileCoord TopLeft = default(TileCoord);
			TileCoord BottomRight = default(TileCoord);
			if (m_DragStartPosition.x < End.x)
			{
				TopLeft.x = m_DragStartPosition.x;
				BottomRight.x = End.x;
			}
			else
			{
				TopLeft.x = End.x;
				BottomRight.x = m_DragStartPosition.x;
			}
			if (m_DragStartPosition.y < End.y)
			{
				TopLeft.y = m_DragStartPosition.y;
				BottomRight.y = End.y;
			}
			else
			{
				TopLeft.y = End.y;
				BottomRight.y = m_DragStartPosition.y;
			}
			Cap2DArea(TopLeft, BottomRight, out TopLeft, out BottomRight);
			for (int i = TopLeft.y; i <= BottomRight.y; i++)
			{
				for (int j = TopLeft.x; j <= BottomRight.x; j++)
				{
					list.Add(new TileCoord(j, i));
				}
			}
			m_DragVertical = false;
		}
		else
		{
			TileCoord tileCoord = default(TileCoord);
			int num2 = 0;
			TileCoord tileCoord2 = End - m_DragStartPosition;
			if (tileCoord2.x != 0 || tileCoord2.y != 0)
			{
				if (Mathf.Abs(tileCoord2.x) >= Mathf.Abs(tileCoord2.y))
				{
					if (tileCoord2.x < 0)
					{
						tileCoord.x = -1;
						if (num != 0 && num != 2)
						{
							num = 0;
						}
					}
					else
					{
						tileCoord.x = 1;
						if (num != 0 && num != 2)
						{
							num = 2;
						}
					}
					num2 = Mathf.Abs(tileCoord2.x) + 1;
				}
				else
				{
					if (tileCoord2.y < 0)
					{
						tileCoord.y = -1;
						if (num != 1 && num != 3)
						{
							num = 1;
						}
					}
					else
					{
						tileCoord.y = 1;
						if (num != 1 && num != 3)
						{
							num = 3;
						}
					}
					num2 = Mathf.Abs(tileCoord2.y) + 1;
				}
			}
			else
			{
				num2 = 1;
			}
			if (tileCoord.x == 0 && tileCoord.y == 0)
			{
				if (Cursor.Instance.GetRotation() == 0 || Cursor.Instance.GetRotation() == 2)
				{
					m_DragVertical = false;
				}
				else
				{
					m_DragVertical = true;
				}
			}
			else if (tileCoord.x != 0)
			{
				m_DragVertical = false;
			}
			else
			{
				m_DragVertical = true;
			}
			int num3 = 100;
			if (num2 > num3)
			{
				num2 = num3;
			}
			for (int k = 0; k < num2; k++)
			{
				list.Add(m_DragStartPosition + tileCoord * k);
			}
		}
		bool isTypeFloor = Floor.GetIsTypeFloor(m_CurrentObjectType);
		m_DragBad = false;
		int num4 = ResourceManager.Instance.GetResource(m_CurrentObjectType);
		for (int l = 0; l < list.Count; l++)
		{
			TileCoord tileCoord3 = list[l];
			Vector3 vector = tileCoord3.ToWorldPositionTileCentered();
			if (Bridge.GetIsTypeBridge(m_CurrentObjectType) || m_CurrentObjectType == ObjectType.TrainTrackBridge)
			{
				vector.y = 0f;
			}
			Tile tile = TileManager.Instance.GetTile(tileCoord3);
			if ((bool)tile.m_Building && !isTypeFloor)
			{
				vector.y += tile.m_Building.GetBuildingHeightOffset();
			}
			Building component;
			if (!m_CTRLHeld && (num4 > 0 || CheatManager.Instance.m_InstantBuild || GameOptionsManager.Instance.m_Options.m_GameMode == GameOptions.GameMode.ModeCreative))
			{
				num4--;
				component = ObjectTypeList.Instance.CreateObjectFromIdentifier(m_CurrentObjectType, vector, Quaternion.identity).GetComponent<Building>();
			}
			else
			{
				component = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.ConverterFoundation, new Vector3(-1f, -1f), Quaternion.identity).GetComponent<Building>();
				component.GetComponent<ConverterFoundation>().SetNewBuilding(m_CurrentObjectType);
				component.GetComponent<ConverterFoundation>().ShowBlueprint(Show: false);
			}
			component.transform.localPosition = vector;
			component.GetComponent<Savable>().SetIsSavable(IsSavable: false);
			component.GetComponent<TileCoordObject>().SetTilePosition(tileCoord3);
			component.GetComponent<Building>().SetRotation(num);
			Building building = component;
			if (building.m_TypeIdentifier == ObjectType.ConverterFoundation)
			{
				building = building.GetComponent<ConverterFoundation>().m_NewBuilding;
			}
			if (!m_DragBad)
			{
				bool flag = false;
				if ((bool)tile.m_Building && !isTypeFloor)
				{
					if ((tile.m_Building.m_TypeIdentifier == ObjectType.ConverterFoundation && tile.m_Building.GetComponent<ConverterFoundation>().m_NewBuilding.m_TypeIdentifier == m_CurrentObjectType) || tile.m_Building.m_TypeIdentifier == m_CurrentObjectType)
					{
						flag = true;
					}
					Building topBuilding = tile.m_Building.GetTopBuilding();
					if (tile.m_Building.m_MaxLevels != 1 && (!topBuilding.GetNewLevelAllowed(component) || !tile.m_Building.GetNewLevelAllowed(component)))
					{
						m_DragBad = true;
					}
				}
				if ((bool)tile.m_Floor && isTypeFloor && ((tile.m_Floor.m_TypeIdentifier == ObjectType.ConverterFoundation && tile.m_Floor.GetComponent<ConverterFoundation>().m_NewBuilding.m_TypeIdentifier == m_CurrentObjectType) || tile.m_Floor.m_TypeIdentifier == m_CurrentObjectType))
				{
					flag = true;
				}
				if (!flag)
				{
					List<BaseClass> modelBadException = new List<BaseClass>();
					if (MapManager.Instance.CheckBuildingIntersection(component, modelBadException, out var _))
					{
						m_DragBad = true;
					}
				}
			}
			m_DragModels.Add(component);
		}
		m_BuildingPalette.SetBuiltCount(num4);
		int resource = ResourceManager.Instance.GetResource(m_CurrentObjectType);
		for (int m = 0; m < m_DragModels.Count; m++)
		{
			MeshRenderer[] componentsInChildren = m_DragModels[m].GetComponentsInChildren<MeshRenderer>();
			for (int n = 0; n < componentsInChildren.Length; n++)
			{
				Material[] materials = componentsInChildren[n].materials;
				foreach (Material material in materials)
				{
					if (m_DragBad)
					{
						material.color = new Color(1f, 0f, 0f, 0.5f);
					}
					else if (m >= resource)
					{
						material.color = new Color(0.75f, 0.75f, 1f, 0.65f);
					}
				}
			}
		}
	}

	public bool CheckBuildingInDragModels(Building NewBuilding)
	{
		if (m_DragModels == null)
		{
			return false;
		}
		return m_DragModels.Contains(NewBuilding);
	}

	private void ConvertDragModels()
	{
		bool isTypeFloor = Floor.GetIsTypeFloor(m_CurrentObjectType);
		List<Building> list = new List<Building>();
		foreach (Building dragModel in m_DragModels)
		{
			TileCoord position = new TileCoord(dragModel.transform.localPosition);
			Tile tile = TileManager.Instance.GetTile(position);
			bool flag = true;
			if ((bool)tile.m_Building && !isTypeFloor && tile.m_Building.m_NumLevels == tile.m_Building.m_MaxLevels)
			{
				flag = false;
			}
			if ((bool)tile.m_Floor && isTypeFloor)
			{
				flag = false;
				if (m_CurrentObjectType == ObjectType.TrainTrack || m_CurrentObjectType == ObjectType.TrainTrackBridge)
				{
					TrainTrackStraight trainTrackStraight = ((tile.m_Floor.m_TypeIdentifier != ObjectType.ConverterFoundation) ? tile.m_Floor.GetComponent<TrainTrackStraight>() : tile.m_Floor.GetComponent<ConverterFoundation>().m_NewBuilding.GetComponent<TrainTrackStraight>());
					bool flag2 = true;
					if (m_DragModels.Count == 1)
					{
						if (dragModel.m_Rotation % 2 == 1)
						{
							flag2 = false;
						}
					}
					else
					{
						flag2 = !m_DragVertical;
					}
					if (!trainTrackStraight.m_Cross && trainTrackStraight.GetHorizontal() != flag2)
					{
						trainTrackStraight.SetCross();
					}
				}
			}
			if (!flag)
			{
				continue;
			}
			Vector3 localPosition = dragModel.transform.localPosition;
			Building building;
			if (m_CTRLHeld || !BuildingManager.Instance.GetIsInstantBuild())
			{
				int resource = ResourceManager.Instance.GetResource(m_CurrentObjectType);
				if (m_CTRLHeld || resource == 0)
				{
					ConverterFoundation component = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.ConverterFoundation, localPosition, Quaternion.identity).GetComponent<ConverterFoundation>();
					component.SetNewBuilding(m_CurrentObjectType);
					building = component;
				}
				else
				{
					ResourceManager.Instance.ReleaseResource(m_CurrentObjectType);
					building = ObjectTypeList.Instance.CreateObjectFromIdentifier(m_CurrentObjectType, localPosition, Quaternion.identity).GetComponent<Building>();
				}
			}
			else
			{
				building = ObjectTypeList.Instance.CreateObjectFromIdentifier(m_CurrentObjectType, localPosition, Quaternion.identity).GetComponent<Building>();
			}
			building.SetRotation(dragModel.m_Rotation);
			MapManager.Instance.AddBuilding(building);
			list.Add(building);
			if (!SaveLoadManager.Instance.m_Loading && building != null)
			{
				ModManager.Instance.CheckBuildingTypeSpawnedCallback(building);
			}
		}
		AddUndoAction(UndoAction.Add, list);
		foreach (Building dragModel2 in m_DragModels)
		{
			TileCoord position2 = new TileCoord(dragModel2.transform.localPosition);
			Tile tile2 = TileManager.Instance.GetTile(position2);
			Building building2 = tile2.m_Building;
			if (building2 == null)
			{
				building2 = tile2.m_Floor;
			}
			RefreshManager.Instance.AddObject(building2);
			TileCoord tileCoord = building2.m_TileCoord;
			if (tileCoord.y > 0)
			{
				tile2 = TileManager.Instance.GetTile(tileCoord + new TileCoord(0, -1));
				if ((bool)tile2.m_Building)
				{
					RefreshManager.Instance.AddObject(tile2.m_Building);
				}
				if ((bool)tile2.m_Floor)
				{
					RefreshManager.Instance.AddObject(tile2.m_Floor);
				}
			}
			if (tileCoord.y < TileManager.Instance.m_TilesHigh - 1)
			{
				tile2 = TileManager.Instance.GetTile(tileCoord + new TileCoord(0, 1));
				if ((bool)tile2.m_Building)
				{
					RefreshManager.Instance.AddObject(tile2.m_Building);
				}
				if ((bool)tile2.m_Floor)
				{
					RefreshManager.Instance.AddObject(tile2.m_Floor);
				}
			}
			if (tileCoord.x > 0)
			{
				tile2 = TileManager.Instance.GetTile(tileCoord + new TileCoord(-1, 0));
				if ((bool)tile2.m_Building)
				{
					RefreshManager.Instance.AddObject(tile2.m_Building);
				}
				if ((bool)tile2.m_Floor)
				{
					RefreshManager.Instance.AddObject(tile2.m_Floor);
				}
			}
			if (tileCoord.x < TileManager.Instance.m_TilesWide - 1)
			{
				tile2 = TileManager.Instance.GetTile(tileCoord + new TileCoord(1, 0));
				if ((bool)tile2.m_Building)
				{
					RefreshManager.Instance.AddObject(tile2.m_Building);
				}
				if ((bool)tile2.m_Floor)
				{
					RefreshManager.Instance.AddObject(tile2.m_Floor);
				}
			}
			if (!building2.GetComponent<Floor>())
			{
				continue;
			}
			if (tileCoord.x > 0)
			{
				if (tileCoord.y > 0)
				{
					tile2 = TileManager.Instance.GetTile(tileCoord + new TileCoord(-1, -1));
					if ((bool)tile2.m_Floor)
					{
						RefreshManager.Instance.AddObject(tile2.m_Floor);
					}
				}
				if (tileCoord.y < TileManager.Instance.m_TilesHigh - 1)
				{
					tile2 = TileManager.Instance.GetTile(tileCoord + new TileCoord(-1, 1));
					if ((bool)tile2.m_Floor)
					{
						RefreshManager.Instance.AddObject(tile2.m_Floor);
					}
				}
			}
			if (tileCoord.x >= TileManager.Instance.m_TilesWide - 1)
			{
				continue;
			}
			if (tileCoord.y > 0)
			{
				tile2 = TileManager.Instance.GetTile(tileCoord + new TileCoord(1, -1));
				if ((bool)tile2.m_Floor)
				{
					RefreshManager.Instance.AddObject(tile2.m_Floor);
				}
			}
			if (tileCoord.y < TileManager.Instance.m_TilesHigh - 1)
			{
				tile2 = TileManager.Instance.GetTile(tileCoord + new TileCoord(1, 1));
				if ((bool)tile2.m_Floor)
				{
					RefreshManager.Instance.AddObject(tile2.m_Floor);
				}
			}
		}
	}

	private void UpdateDrag()
	{
		if (!m_EventSystem.IsUIInFocus())
		{
			bool flag = UpdateCursorType();
			bool flag2 = false;
			TileCoord tileCoord = default(TileCoord);
			Vector3 CollisionPoint;
			GameObject gameObject = TestMouseCollision(out CollisionPoint, TestTiles: true, TestBuildings: false, TestWorkers: false, TestMisc: false, TestWorkersDead: false);
			if ((bool)gameObject && (bool)gameObject.GetComponent<PlotRoot>())
			{
				tileCoord = new TileCoord(CollisionPoint);
				Cursor.Instance.TargetTile(tileCoord);
				flag2 = true;
			}
			if (!flag2)
			{
				Cursor.Instance.NoTarget();
				DestroyDragModels();
			}
			else if (tileCoord != m_DragOldPosition || flag)
			{
				m_DragOldPosition = tileCoord;
				CreateDragModels(tileCoord);
			}
		}
		if (!m_EventSystem.IsUIInUse())
		{
			CameraManager.Instance.UpdateInput();
		}
		if (!Input.GetMouseButton(0))
		{
			if (!m_DragBad)
			{
				AudioManager.Instance.StartEvent("UIBlueprintAdded");
				ConvertDragModels();
				SetState(State.Add);
				m_BuildingPalette.UpdateBuiltCounts();
			}
			else
			{
				AudioManager.Instance.StartEvent("UIBlueprintAddedFail");
				SetState(State.Add);
			}
		}
		if (MyInputManager.m_Rewired.GetButtonDown("Quit"))
		{
			AudioManager.Instance.StartEvent("UICloseWindow");
			SetState(State.Add);
		}
	}

	private void TestBuildingDragged()
	{
		bool flag = false;
		if (MyInputManager.m_Rewired.GetButtonDown("Rotate") && !Cursor.Instance.m_Snapping)
		{
			flag = true;
		}
		if (!((m_MoveMousePosition - Input.mousePosition).magnitude > 5f || flag))
		{
			return;
		}
		HighlightObject(null);
		List<BaseClass> list = new List<BaseClass>();
		if (m_MoveBuilding.GetComponent<Building>() != null && m_AreaSelectBuildings.ContainsKey(m_MoveBuilding.GetComponent<Building>()))
		{
			foreach (KeyValuePair<Building, int> areaSelectBuilding in m_AreaSelectBuildings)
			{
				Building key = areaSelectBuilding.Key;
				list.Add(key.GetComponent<BaseClass>());
				key.SetBeingMoved(Moved: true);
			}
		}
		else
		{
			list.Add(m_MoveBuilding.GetComponent<BaseClass>());
			m_MoveBuilding.SetBeingMoved(Moved: true);
		}
		Cursor.Instance.TargetTile(m_ModeStartPosition);
		Cursor.Instance.SetModels(list, m_ModeStartPosition, New: false);
		Cursor.Instance.SetModelBadException(list);
		SetState(State.Moving);
		if (flag)
		{
			Cursor.Instance.SetRotation((Cursor.Instance.GetRotation() + 1) % 4);
			Cursor.Instance.GetAreaRectangle(out m_AreaSelectLeft, out m_AreaSelectRight, out m_AreaSelectTop, out m_AreaSelectBottom);
		}
	}

	private bool GetBottomBuildingRequired(Building NewBuilding)
	{
		if (NewBuilding == null)
		{
			return false;
		}
		if (Storage.GetIsTypeStorage(NewBuilding.m_TypeIdentifier) && NewBuilding.GetComponent<Storage>().GetStored() > 0)
		{
			return true;
		}
		if (NewBuilding.m_Levels != null)
		{
			foreach (Building level in NewBuilding.m_Levels)
			{
				if (level.m_TypeIdentifier == ObjectType.ConverterFoundation && level.GetComponent<ConverterFoundation>().m_NewBuilding.m_BuildingToUpgradeFrom != ObjectTypeList.m_Total)
				{
					return true;
				}
			}
		}
		return false;
	}

	private void GetAreaSelectRectangle()
	{
		m_AreaSelectLeft = 10000;
		m_AreaSelectRight = -10000;
		m_AreaSelectTop = 10000;
		m_AreaSelectBottom = -10000;
		foreach (KeyValuePair<Building, int> areaSelectBuilding in m_AreaSelectBuildings)
		{
			foreach (TileCoord tile in areaSelectBuilding.Key.m_Tiles)
			{
				TileCoord tileCoord = tile - m_ModeStartPosition;
				if (m_AreaSelectLeft > tileCoord.x)
				{
					m_AreaSelectLeft = tileCoord.x;
				}
				if (m_AreaSelectRight < tileCoord.x)
				{
					m_AreaSelectRight = tileCoord.x;
				}
				if (m_AreaSelectTop > tileCoord.y)
				{
					m_AreaSelectTop = tileCoord.y;
				}
				if (m_AreaSelectBottom < tileCoord.y)
				{
					m_AreaSelectBottom = tileCoord.y;
				}
			}
		}
	}

	private bool GetIsAreaSelectUsable(bool Delete)
	{
		if (m_AreaSelectBuildings != null)
		{
			GetAction action = GetAction.IsMovable;
			if (Delete)
			{
				action = GetAction.IsDeletable;
			}
			foreach (KeyValuePair<Building, int> areaSelectBuilding in m_AreaSelectBuildings)
			{
				Building key = areaSelectBuilding.Key;
				if (!(bool)key.GetActionInfo(new GetActionInfo(action)) || key.m_Plot == null)
				{
					return false;
				}
			}
		}
		return true;
	}

	private void UpdateMove()
	{
		if (!m_EventSystem.IsUIInFocus())
		{
			bool flag = false;
			UpdateBinIconPosition(Show: true);
			Actionable actionable = null;
			TileCoord tileCoord = default(TileCoord);
			GameObject gameObject = TestMouseCollision(out var CollisionPoint, TestTiles: true, TestBuildings: true, TestWorkers: false, TestMisc: false, TestWorkersDead: false);
			m_ActionSuccess = true;
			if ((bool)gameObject)
			{
				if ((bool)gameObject.GetComponent<PlotRoot>())
				{
					tileCoord = new TileCoord(CollisionPoint);
					Cursor.Instance.TargetTile(tileCoord);
					Tile tile = TileManager.Instance.GetTile(tileCoord);
					if ((bool)tile.m_BuildingFootprint)
					{
						gameObject = tile.m_BuildingFootprint.GetTopBuilding().gameObject;
					}
					else if ((bool)tile.m_Floor)
					{
						gameObject = tile.m_Floor.gameObject;
					}
				}
				if (!gameObject.GetComponent<PlotRoot>())
				{
					gameObject = GetRootObject(gameObject);
					if ((bool)gameObject.GetComponent<Building>())
					{
						tileCoord = gameObject.GetComponent<Building>().m_TileCoord;
						if ((bool)gameObject.GetComponent<Floor>() || ((bool)gameObject.GetComponent<ConverterFoundation>() && (bool)gameObject.GetComponent<ConverterFoundation>().m_NewBuilding.GetComponent<Floor>()))
						{
							Building component = gameObject.GetComponent<Building>();
						}
						else
						{
							Building component = TileManager.Instance.GetTile(tileCoord).m_Building;
							gameObject = ((!GetBottomBuildingRequired(component)) ? component.GetTopBuilding().gameObject : component.gameObject);
						}
						actionable = gameObject.GetComponent<Actionable>();
						Cursor.Instance.Target(gameObject);
						flag = true;
						m_ActionSuccess = (bool)gameObject.GetComponent<Building>().GetActionInfo(new GetActionInfo(GetAction.IsMovable));
					}
				}
			}
			if (!GetIsAreaSelectUsable(Delete: false))
			{
				m_ActionSuccess = false;
			}
			Cursor.Instance.SetUsable(m_ActionSuccess);
			if (actionable == null || actionable.GetComponent<Building>() == null || !m_AreaSelectBuildings.ContainsKey(actionable.GetComponent<Building>()))
			{
				HighlightObject(actionable);
			}
			if (!flag)
			{
				Cursor.Instance.NoTarget();
				if (Input.GetMouseButton(0))
				{
					if ((bool)m_MoveBuilding)
					{
						TestBuildingDragged();
					}
					else
					{
						UnHighlightAreaBuildings();
						m_AreaStartCoord = tileCoord;
						m_AreaLastCoord = tileCoord;
						m_AreaIndicator.SetCoords(m_AreaStartCoord, m_AreaStartCoord);
						SetState(State.AreaDrag);
					}
				}
				else
				{
					m_MoveBuilding = null;
				}
			}
			else if (Input.GetMouseButton(0))
			{
				if (!m_ActionSuccess)
				{
					if (Input.GetMouseButtonDown(0))
					{
						AudioManager.Instance.StartEvent("UIBuildingDeleteFail");
					}
				}
				else if (m_MoveBuilding == null)
				{
					if (!m_AreaSelectBuildings.ContainsKey(actionable.GetComponent<Building>()))
					{
						m_AreaSelected = false;
						UnHighlightAreaBuildings();
						Building component2 = actionable.GetComponent<Building>();
						if (GetBottomBuildingRequired(component2))
						{
							m_AreaSelectBuildings.Add(component2, 0);
							if (component2.m_Levels != null)
							{
								foreach (Building level in component2.m_Levels)
								{
									m_AreaSelectBuildings.Add(level, 0);
								}
							}
						}
					}
					m_MoveBuilding = actionable.GetComponent<Building>();
					m_MoveMousePosition = Input.mousePosition;
					m_ModeStartPosition = tileCoord;
					m_MoveCursorRotation = Cursor.Instance.GetRotation();
					GetAreaSelectRectangle();
				}
				else
				{
					TestBuildingDragged();
				}
			}
			else
			{
				m_MoveBuilding = null;
			}
		}
		else
		{
			m_BinIcon.SetActive(value: false);
			HighlightObject(null);
			Cursor.Instance.NoTarget();
		}
		if (!m_EventSystem.IsUIInUse())
		{
			CameraManager.Instance.UpdateInput();
		}
		if (MyInputManager.m_Rewired.GetButtonDown("Quit") || MyInputManager.m_Rewired.GetButtonDown("EditMove"))
		{
			HighlightObject(null);
			AudioManager.Instance.StartEvent("UICloseWindow");
			GameStateManager.Instance.SetState(GameStateManager.State.Normal);
		}
		if (MyInputManager.m_Rewired.GetButtonDown("Edit"))
		{
			AudioManager.Instance.StartEvent("UICloseWindow");
			GameStateManager.Instance.SetState(GameStateManager.State.Normal);
		}
		if (MyInputManager.m_Rewired.GetButtonDown("EditDelete"))
		{
			HighlightObject(null);
			SetState(State.Delete);
		}
	}

	private void UpdateBinIconPosition(bool Show)
	{
		string text = "Bin";
		if (m_State == State.Move || m_State == State.Moving || m_State == State.AddDuplicate)
		{
			text = "Move";
		}
		if (m_State == State.Duplicate)
		{
			text = "Duplicate";
		}
		if (!m_ActionSuccess)
		{
			text = "Invalid";
		}
		Sprite sprite = (Sprite)Resources.Load("Textures/Hud/Edit/" + text, typeof(Sprite));
		m_BinIcon.GetComponent<Image>().sprite = sprite;
		m_BinIcon.transform.localPosition = HudManager.Instance.ScreenToCanvas(Input.mousePosition) + new Vector3(40f, 0f, 0f);
		m_BinIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(30f, 30f);
		m_BinImage.gameObject.SetActive(value: false);
		if (Show)
		{
			m_BinIcon.SetActive(value: true);
		}
	}

	private static int SortBuildingByIndex(MovingBuilding p1, MovingBuilding p2)
	{
		return p1.m_SortValue - p2.m_SortValue;
	}

	private List<MovingBuilding> GetMovingBuildingList()
	{
		List<MovingBuilding> list = new List<MovingBuilding>();
		int num = 0;
		foreach (KeyValuePair<Building, int> areaSelectBuilding in m_AreaSelectBuildings)
		{
			Building key = areaSelectBuilding.Key;
			Building component = Cursor.Instance.m_Model[num].m_Model.GetComponent<Building>();
			MovingBuilding movingBuilding = new MovingBuilding(0, key, new TileCoord(component.transform.position), component.m_Rotation);
			movingBuilding.m_SortValue = areaSelectBuilding.Value;
			list.Add(movingBuilding);
			num++;
		}
		list.Sort(SortBuildingByIndex);
		return list;
	}

	private void MoveBuildings(TileCoord TilePosition)
	{
		int rotation = Cursor.Instance.GetRotation();
		if (!(TilePosition != m_MoveBuilding.m_TileCoord) && rotation == m_MoveBuilding.m_Rotation)
		{
			return;
		}
		TileCoord tileCoord = TilePosition - m_MoveBuilding.m_TileCoord;
		int num = rotation - m_MoveBuilding.m_Rotation;
		if (m_AreaSelectBuildings.Count > 0)
		{
			List<MovingBuilding> movingBuildingList = GetMovingBuildingList();
			AddUndoAction(UndoAction.Move, m_AreaSelectBuildings, -tileCoord, -rotation);
			BuildingManager.Instance.MoveBuildings(movingBuildingList);
			if (m_AreaSelected)
			{
				foreach (KeyValuePair<Building, int> areaSelectBuilding in m_AreaSelectBuildings)
				{
					areaSelectBuilding.Key.SetHighlight(Highlighted: true);
				}
			}
			if (!m_AreaSelected)
			{
				m_AreaSelectBuildings.Clear();
			}
		}
		else
		{
			AddUndoAction(UndoAction.Move, m_MoveBuilding, -tileCoord, -num);
			int rotation2 = Cursor.Instance.GetRotation();
			BuildingManager.Instance.MoveBuilding(m_MoveBuilding, TilePosition, rotation2);
		}
		Cursor.Instance.SetModel(ObjectTypeList.m_Total);
	}

	private void UpdateMovingObjects()
	{
		List<BaseClass> list = new List<BaseClass>();
		List<TileCoordObject> list2 = new List<TileCoordObject>();
		if (m_AreaSelectBuildings.Count > 0)
		{
			foreach (KeyValuePair<Building, int> areaSelectBuilding in m_AreaSelectBuildings)
			{
				Building key = areaSelectBuilding.Key;
				list2.Clear();
				MovingBuilding.GetOutputObjects(list2, key);
				foreach (TileCoordObject item in list2)
				{
					list.Add(item);
				}
			}
		}
		else
		{
			list2.Clear();
			MovingBuilding.GetOutputObjects(list2, m_MoveBuilding);
			foreach (TileCoordObject item2 in list2)
			{
				list.Add(item2);
			}
		}
		Cursor.Instance.SetExtraBadException(list);
	}

	private void UpdateMoving()
	{
		if (!m_EventSystem.IsUIInFocus())
		{
			bool flag = false;
			UpdateBinIconPosition(Show: true);
			UpdateMovingObjects();
			TileCoord tileCoord = default(TileCoord);
			Vector3 CollisionPoint;
			GameObject gameObject = TestMouseCollision(out CollisionPoint, TestTiles: true, TestBuildings: false, TestWorkers: false, TestMisc: false, TestWorkersDead: false);
			if ((bool)gameObject && (bool)gameObject.GetComponent<PlotRoot>())
			{
				tileCoord = new TileCoord(CollisionPoint);
				if (tileCoord.x + m_AreaSelectLeft < 0)
				{
					tileCoord.x = -m_AreaSelectLeft;
				}
				if (tileCoord.x + m_AreaSelectRight >= TileManager.Instance.m_TilesWide)
				{
					tileCoord.x = TileManager.Instance.m_TilesWide - 1 - m_AreaSelectRight;
				}
				if (tileCoord.y + m_AreaSelectTop < 0)
				{
					tileCoord.y = -m_AreaSelectTop;
				}
				if (tileCoord.y + m_AreaSelectBottom >= TileManager.Instance.m_TilesHigh)
				{
					tileCoord.y = TileManager.Instance.m_TilesHigh - 1 - m_AreaSelectBottom;
				}
				Cursor.Instance.TargetTile(tileCoord);
				flag = true;
				m_ActionSuccess = true;
				Building building = null;
				if (m_AreaSelectBuildings.Count == 1)
				{
					using Dictionary<Building, int>.Enumerator enumerator = m_AreaSelectBuildings.GetEnumerator();
					if (enumerator.MoveNext())
					{
						building = enumerator.Current.Key;
					}
				}
				else if (m_AreaSelectBuildings.Count == 0)
				{
					building = m_MoveBuilding;
				}
				if ((bool)building)
				{
					ObjectType newType = ((building.m_TypeIdentifier != ObjectType.ConverterFoundation) ? building.m_TypeIdentifier : building.GetComponent<ConverterFoundation>().m_NewBuilding.m_TypeIdentifier);
					Building building2 = TileManager.Instance.GetTile(tileCoord).m_Building;
					if ((bool)building2 && building2 != building && building2.CanBuildTypeUpon(newType))
					{
						tileCoord = building2.m_TileCoord;
						Cursor.Instance.SnapTo(building2.m_TileCoord, building2.m_Rotation);
					}
					else
					{
						Cursor.Instance.StopSnap();
					}
				}
			}
			if (m_AreaSelectBuildings.Count > 0 && !GetIsAreaSelectUsable(Delete: false))
			{
				m_ActionSuccess = false;
			}
			if (m_AreaSelectBuildings.Count == 0 && !(bool)m_MoveBuilding.GetActionInfo(new GetActionInfo(GetAction.IsMovable)))
			{
				m_ActionSuccess = false;
			}
			Cursor.Instance.SetUsable(m_ActionSuccess);
			if (!flag)
			{
				Cursor.Instance.NoTarget();
			}
			else if (Input.GetMouseButtonUp(0))
			{
				if (Cursor.Instance.m_ModelBad || !m_ActionSuccess)
				{
					AudioManager.Instance.StartEvent("UIBlueprintAddedFail");
				}
				else
				{
					MoveBuildings(tileCoord);
				}
				SetState(State.Move);
			}
			else if (MyInputManager.m_Rewired.GetButtonDown("Rotate") && !Cursor.Instance.m_Snapping)
			{
				Cursor.Instance.SetRotation((Cursor.Instance.GetRotation() + 1) % 4);
				Cursor.Instance.GetAreaRectangle(out m_AreaSelectLeft, out m_AreaSelectRight, out m_AreaSelectTop, out m_AreaSelectBottom);
			}
		}
		else
		{
			m_BinIcon.SetActive(value: false);
		}
		if (!m_EventSystem.IsUIInUse())
		{
			CameraManager.Instance.UpdateInput();
		}
		if (MyInputManager.m_Rewired.GetButtonDown("Quit"))
		{
			Cursor.Instance.SetModel(ObjectTypeList.m_Total);
			AudioManager.Instance.StartEvent("UICloseWindow");
			SetState(State.Add);
		}
	}

	private void UpdateArea()
	{
		m_BinIcon.SetActive(value: false);
		if (!m_EventSystem.IsUIInFocus())
		{
			bool flag = false;
			TileCoord tileCoord = default(TileCoord);
			Vector3 CollisionPoint;
			GameObject gameObject = TestMouseCollision(out CollisionPoint, TestTiles: true, TestBuildings: false, TestWorkers: false, TestMisc: false, TestWorkersDead: false);
			if ((bool)gameObject && (bool)gameObject.GetComponent<PlotRoot>())
			{
				tileCoord = new TileCoord(CollisionPoint);
				Cursor.Instance.TargetTile(tileCoord);
				flag = true;
			}
			if (!flag)
			{
				Cursor.Instance.NoTarget();
			}
			else if (Input.GetMouseButton(0))
			{
				m_AreaStartCoord = tileCoord;
				m_AreaLastCoord = tileCoord;
				m_AreaIndicator.SetCoords(m_AreaStartCoord, m_AreaStartCoord);
				SetState(State.AreaDrag);
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
		if (MyInputManager.m_Rewired.GetButtonDown("Quit"))
		{
			AudioManager.Instance.StartEvent("UICloseWindow");
			SetState(State.Add);
		}
		if (MyInputManager.m_Rewired.GetButtonDown("Edit"))
		{
			AudioManager.Instance.StartEvent("UICloseWindow");
			GameStateManager.Instance.SetState(GameStateManager.State.Normal);
		}
	}

	public void BuildingDestroyed(Building NewBuilding)
	{
		bool flag = false;
		if (m_AreaSelectBuildings.ContainsKey(NewBuilding))
		{
			m_AreaSelectBuildings.Remove(NewBuilding);
			flag = true;
		}
		if (m_State == State.Moving && (flag || m_MoveBuilding == NewBuilding))
		{
			Cursor.Instance.SetModel(ObjectTypeList.m_Total);
			Cursor.Instance.SetModelBadException(null);
			Cursor.Instance.SetExtraBadException(null);
			SetState(State.Add);
		}
	}

	public void BuildingChanged(Building NewBuilding)
	{
		if (m_State == State.Selected && m_BuildingPalette.m_SelectedBuilding == NewBuilding)
		{
			SetState(State.Add);
		}
	}

	private void UpdateAreaDrag()
	{
		if (!m_EventSystem.IsUIInFocus())
		{
			bool flag = false;
			TileCoord tileCoord = default(TileCoord);
			Vector3 CollisionPoint;
			GameObject gameObject = TestMouseCollision(out CollisionPoint, TestTiles: true, TestBuildings: false, TestWorkers: false, TestMisc: false, TestWorkersDead: false);
			if ((bool)gameObject && (bool)gameObject.GetComponent<PlotRoot>())
			{
				tileCoord = new TileCoord(CollisionPoint);
				bool flag2 = false;
				flag2 = MyInputManager.Instance.GetCTRLHeld();
				if (m_AreaLastCoord != tileCoord || flag2 != m_CTRLHeld)
				{
					if (GameOptionsManager.Instance.m_Options.m_GameMode != GameOptions.GameMode.ModeCreative)
					{
						m_CTRLHeld = flag2;
					}
					Dictionary<Building, int> dictionary = new Dictionary<Building, int>(m_AreaSelectBuildings);
					m_AreaLastCoord = tileCoord;
					Cursor.Instance.TargetTile(tileCoord);
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
					for (int i = topLeft.y; i <= bottomRight.y; i++)
					{
						for (int j = topLeft.x; j <= bottomRight.x; j++)
						{
							Tile tile = TileManager.Instance.GetTile(new TileCoord(j, i));
							if (!m_CTRLHeld)
							{
								Building floor = tile.m_Floor;
								if ((bool)floor && !m_AreaSelectBuildings.ContainsKey(floor))
								{
									floor.SetHighlight(Highlighted: true);
									m_AreaSelectBuildings.Add(floor, m_AreaSelectBuildings.Count);
								}
								if ((bool)floor && dictionary.ContainsKey(floor))
								{
									dictionary.Remove(floor);
								}
							}
							Building buildingFootprint = tile.m_BuildingFootprint;
							if ((bool)buildingFootprint && !m_AreaSelectBuildings.ContainsKey(buildingFootprint))
							{
								buildingFootprint.SetHighlight(Highlighted: true);
								m_AreaSelectBuildings.Add(buildingFootprint, m_AreaSelectBuildings.Count);
								if (buildingFootprint.m_Levels != null)
								{
									foreach (Building level in buildingFootprint.m_Levels)
									{
										if ((bool)level)
										{
											level.SetHighlight(Highlighted: true);
											if (!m_AreaSelectBuildings.ContainsKey(level))
											{
												m_AreaSelectBuildings.Add(level, m_AreaSelectBuildings.Count);
											}
										}
									}
								}
							}
							if (!buildingFootprint || !dictionary.ContainsKey(buildingFootprint))
							{
								continue;
							}
							dictionary.Remove(buildingFootprint);
							if (buildingFootprint.m_Levels == null)
							{
								continue;
							}
							foreach (Building level2 in buildingFootprint.m_Levels)
							{
								dictionary.Remove(level2);
							}
						}
					}
					foreach (KeyValuePair<Building, int> item in dictionary)
					{
						if ((bool)item.Key)
						{
							item.Key.SetHighlight(Highlighted: false);
						}
						m_AreaSelectBuildings.Remove(item.Key);
					}
					if (m_AreaSelectBuildings.Count > 0)
					{
						m_AreaSelected = true;
					}
				}
				flag = true;
			}
			if (!flag)
			{
				Cursor.Instance.NoTarget();
			}
			else if (Input.GetMouseButtonUp(0))
			{
				SetState(State.Move);
			}
		}
		if (!m_EventSystem.IsUIInUse())
		{
			CameraManager.Instance.UpdateInput();
		}
		if (MyInputManager.m_Rewired.GetButtonDown("Quit"))
		{
			UnHighlightAreaBuildings();
			AudioManager.Instance.StartEvent("UICloseWindow");
			SetState(State.Add);
		}
		if (MyInputManager.m_Rewired.GetButtonDown("Edit"))
		{
			SetState(State.Move);
			AudioManager.Instance.StartEvent("UICloseWindow");
			GameStateManager.Instance.SetState(GameStateManager.State.Normal);
		}
	}

	private void UpdateAreaSelected()
	{
		if (!m_EventSystem.IsUIInUse())
		{
			CameraManager.Instance.UpdateInput();
		}
		if (MyInputManager.m_Rewired.GetButtonDown("Quit"))
		{
			UnHighlightAreaBuildings();
			AudioManager.Instance.StartEvent("UICloseWindow");
			SetState(State.Add);
		}
		if (MyInputManager.m_Rewired.GetButtonDown("Edit"))
		{
			AudioManager.Instance.StartEvent("UICloseWindow");
			GameStateManager.Instance.SetState(GameStateManager.State.Normal);
		}
	}

	private void CheckKeyPresses()
	{
		if (m_EventSystem.IsUIInUse() || m_State == State.Moving || m_State == State.Drag || m_State == State.AreaDrag || m_State == State.AreaSelected)
		{
			return;
		}
		if (MyInputManager.m_Rewired.GetButtonDown("Planning"))
		{
			AudioManager.Instance.StartEvent("UIEditModeSelected");
			GameStateManager.Instance.PushState(GameStateManager.State.Planning);
			return;
		}
		if (MyInputManager.m_Rewired.GetButtonDown("Undo"))
		{
			Undo();
		}
		if (MyInputManager.m_Rewired.GetButtonDown("SelectBuilding"))
		{
			SetState(State.Select);
		}
		CheckPlanningToggle();
	}

	private void UpdateSelect()
	{
		if (!m_EventSystem.IsUIInFocus())
		{
			bool flag = false;
			Actionable actionable = null;
			TileCoord tileCoord = default(TileCoord);
			Vector3 CollisionPoint;
			GameObject gameObject = TestMouseCollision(out CollisionPoint, TestTiles: true, TestBuildings: true, TestWorkers: false, TestMisc: false, TestWorkersDead: false);
			m_ActionSuccess = true;
			if ((bool)gameObject)
			{
				if ((bool)gameObject && (bool)gameObject.GetComponent<PlotRoot>())
				{
					tileCoord = new TileCoord(CollisionPoint);
					Tile tile = TileManager.Instance.GetTile(tileCoord);
					if ((bool)tile.m_BuildingFootprint)
					{
						gameObject = tile.m_BuildingFootprint.gameObject;
					}
				}
				else
				{
					gameObject = GetRootObject(gameObject);
				}
				if ((bool)gameObject && (bool)gameObject.GetComponent<Building>())
				{
					tileCoord = gameObject.GetComponent<Building>().m_TileCoord;
					if ((bool)gameObject.GetComponent<Building>().GetBottomBuilding())
					{
						gameObject = gameObject.GetComponent<Building>().GetBottomBuilding().gameObject;
					}
					if (m_SearchTypes.Contains(gameObject.GetComponent<Actionable>().m_TypeIdentifier))
					{
						actionable = gameObject.GetComponent<Actionable>();
						Cursor.Instance.TargetTile(tileCoord);
						Cursor.Instance.Target(gameObject);
						flag = true;
					}
				}
			}
			HighlightObject(actionable);
			if (!flag)
			{
				Cursor.Instance.NoTarget();
				if (Input.GetMouseButton(0))
				{
					UnHighlightAreaBuildings();
				}
			}
			else if (Input.GetMouseButtonDown(0))
			{
				SetState(State.Selected);
				m_BuildingPalette.SetSelectedBuilding(actionable.GetComponent<Building>());
			}
		}
		else
		{
			m_BinIcon.SetActive(value: false);
			HighlightObject(null);
			Cursor.Instance.NoTarget();
		}
		UpdateKeyPresses();
	}

	public void SelectedBuidingTeamSelected()
	{
		List<Worker> referencingWorkers = m_BuildingPalette.m_SelectedBuilding.GetReferencingWorkers();
		GameStateManager.Instance.SetState(GameStateManager.State.Normal);
		GameStateNormal component = GameStateManager.Instance.GetCurrentState().GetComponent<GameStateNormal>();
		if (referencingWorkers.Count == 1)
		{
			component.SetSelectedWorker(referencingWorkers[0]);
		}
		else
		{
			if (referencingWorkers.Count <= 1)
			{
				return;
			}
			WorkerGroup tempGroup = WorkerGroupManager.Instance.m_TempGroup;
			tempGroup.ClearTemp();
			component.ClearSelectedWorkers();
			foreach (Worker item in referencingWorkers)
			{
				tempGroup.AddWorker(item, Temp: true);
			}
			component.SetSelectedGroup(tempGroup);
		}
	}

	private void UpdateSelected()
	{
		UpdateKeyPresses();
	}

	public override void UpdateState()
	{
		CheatManager.Instance.UpdateNormal();
		if ((bool)Cursor.Instance)
		{
			if (Cursor.Instance.m_BadObjects.Count == 0)
			{
				Cursor.Instance.SetUsable(Usable: true);
			}
			else
			{
				Cursor.Instance.SetUsable(Usable: false);
			}
		}
		CheckKeyPresses();
		switch (m_State)
		{
		case State.Add:
			UpdateAdd();
			break;
		case State.Delete:
			UpdateDelete();
			break;
		case State.Duplicate:
			UpdateDuplicate();
			break;
		case State.AddDuplicate:
			UpdateAddDuplicate();
			break;
		case State.Drag:
			UpdateDrag();
			break;
		case State.Move:
			UpdateMove();
			break;
		case State.Moving:
			UpdateMoving();
			break;
		case State.Area:
			UpdateArea();
			break;
		case State.AreaDrag:
			UpdateAreaDrag();
			break;
		case State.AreaSelected:
			UpdateAreaSelected();
			break;
		case State.Select:
			UpdateSelect();
			break;
		case State.Selected:
			UpdateSelected();
			break;
		}
		UpdateUndoStillValid();
	}
}
