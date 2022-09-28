using System.Collections.Generic;
using UnityEngine;

public class BuildingPalette : BaseMenu
{
	private static ObjectSubCategory m_CurrentCategory;

	public static ObjectType m_CurrentType;

	public static List<ObjectType> m_NewUnlocked;

	public static List<ObjectSubCategory> m_NewCategoryUnlocked;

	private List<ObjectSubCategory> m_ValidCategories;

	private float m_Spacing;

	private List<BuildingButton> m_Buttons;

	private List<BuildingCategoryButton> m_CategoryButtons;

	private ObjectSelectScrollView m_ScrollView;

	private StandardButtonImage m_BinButton;

	private StandardButtonImage m_DuplicateButton;

	private StandardButtonImage m_MoveButton;

	private StandardButtonImage m_AreaButton;

	private StandardButtonImage m_UndoButton;

	private StandardButtonImage m_SelectButton;

	private StandardButtonImage m_PlanningButton;

	private Transform m_InactiveTabs;

	private Transform m_ActiveTabs;

	private BuildingCategoryButton m_CategoryButtonPrefab;

	private bool m_UndoAvailable;

	public List<BuildingCategoryInfo> m_CategoryInfo;

	private List<ObjectType> m_UnlockedBuildings;

	private GameObject m_Obstruction;

	private float m_ObstructionTimer;

	public Building m_SelectedBuilding;

	private BaseInputField m_SelectedBuildingName;

	private BaseButton m_SelectedBuildingTeam;

	public static void AddNewUnlocked(ObjectType NewType)
	{
		if (!TutorialPanelController.Instance.GetActive())
		{
			m_NewUnlocked.Add(NewType);
			ModeButton.Get(ModeButton.Type.BuildingPalette).SetNew(New: true);
		}
	}

	public static void InitFirst()
	{
		m_CurrentCategory = ObjectSubCategory.BuildingsWorkshop;
		m_CurrentType = ObjectType.StorageGeneric;
		m_NewUnlocked = new List<ObjectType>();
	}

	private void SetupCategories()
	{
		m_CategoryInfo = new List<BuildingCategoryInfo>();
		m_ValidCategories = new List<ObjectSubCategory>();
		m_ValidCategories.Add(ObjectSubCategory.BuildingsWorkshop);
		m_ValidCategories.Add(ObjectSubCategory.BuildingsStorage);
		m_ValidCategories.Add(ObjectSubCategory.BuildingsSpecial);
		m_ValidCategories.Add(ObjectSubCategory.BuildingsFood);
		m_ValidCategories.Add(ObjectSubCategory.BuildingsWalls);
		m_ValidCategories.Add(ObjectSubCategory.BuildingsFloors);
		m_ValidCategories.Add(ObjectSubCategory.BuildingsTrains);
		m_ValidCategories.Add(ObjectSubCategory.BuildingsAnimals);
		m_ValidCategories.Add(ObjectSubCategory.BuildingsClothing);
		m_ValidCategories.Add(ObjectSubCategory.BuildingsMisc);
		m_ValidCategories.Add(ObjectSubCategory.BuildingsDecoration);
		m_ValidCategories.Add(ObjectSubCategory.BuildingsPrizes);
		foreach (ObjectSubCategory validCategory in m_ValidCategories)
		{
			List<ObjectType> list = new List<ObjectType>();
			for (int i = 0; i < (int)ObjectTypeList.m_Total; i++)
			{
				ObjectType objectType = (ObjectType)i;
				if (ObjectTypeList.Instance.GetSubCategoryFromType(objectType) == validCategory)
				{
					list.Add(objectType);
				}
			}
			BuildingCategoryInfo item = new BuildingCategoryInfo(validCategory, list, ObjectTypeList.Instance.GetSubCategoryName(validCategory));
			m_CategoryInfo.Add(item);
		}
	}

	protected new void Awake()
	{
		base.Awake();
		m_CurrentType = GameStateEdit.m_CurrentObjectType;
		SetupCategories();
		m_ActiveTabs = base.transform.Find("ActiveTabs");
		m_InactiveTabs = base.transform.Find("InactiveTabs");
		m_ScrollView = base.transform.Find("ObjectSelectScrollView").GetComponent<ObjectSelectScrollView>();
		m_CategoryButtonPrefab = m_ActiveTabs.Find("BuildingCategoryButton").GetComponent<BuildingCategoryButton>();
		m_CategoryButtonPrefab.gameObject.SetActive(value: false);
		m_BinButton = base.transform.Find("BinButton").Find("StandardButtonImage").GetComponent<StandardButtonImage>();
		m_BinButton.m_OnEnterEvent = OnButtonEnter;
		m_BinButton.m_OnExitEvent = OnButtonExit;
		m_DuplicateButton = base.transform.Find("DuplicateButton").Find("StandardButtonImage").GetComponent<StandardButtonImage>();
		m_DuplicateButton.m_OnEnterEvent = OnButtonEnter;
		m_DuplicateButton.m_OnExitEvent = OnButtonExit;
		m_MoveButton = base.transform.Find("MoveButton").Find("StandardButtonImage").GetComponent<StandardButtonImage>();
		m_MoveButton.m_OnEnterEvent = OnButtonEnter;
		m_MoveButton.m_OnExitEvent = OnButtonExit;
		m_AreaButton = base.transform.Find("AreaButton").Find("StandardButtonImage").GetComponent<StandardButtonImage>();
		m_AreaButton.m_OnEnterEvent = OnButtonEnter;
		m_AreaButton.m_OnExitEvent = OnButtonExit;
		m_UndoButton = base.transform.Find("UndoButton").Find("StandardButtonImage").GetComponent<StandardButtonImage>();
		m_UndoButton.m_OnEnterEvent = OnButtonEnter;
		m_UndoButton.m_OnExitEvent = OnButtonExit;
		m_PlanningButton = base.transform.Find("PlanningButton").Find("StandardButtonImage").GetComponent<StandardButtonImage>();
		m_PlanningButton.m_OnEnterEvent = OnButtonEnter;
		m_PlanningButton.m_OnExitEvent = OnButtonExit;
		m_SelectButton = base.transform.Find("SelectButton").Find("StandardButtonImage").GetComponent<StandardButtonImage>();
		m_SelectButton.m_OnEnterEvent = OnButtonEnter;
		m_SelectButton.m_OnExitEvent = OnButtonExit;
		m_SelectedBuildingName = base.transform.Find("Name").GetComponent<BaseInputField>();
		m_SelectedBuildingTeam = base.transform.Find("TeamButton").GetComponent<BaseButton>();
		SetSelectedBuilding(null);
		m_Obstruction = base.transform.Find("ObstructionPanel").gameObject;
		m_Obstruction.SetActive(value: false);
		m_ObstructionTimer = 0f;
		BaseButtonBack component = base.transform.Find("StandardCancelButton").GetComponent<BaseButtonBack>();
		component.m_OnEnterEvent = OnButtonEnter;
		component.m_OnExitEvent = OnButtonExit;
		m_UndoAvailable = false;
		m_Buttons = new List<BuildingButton>();
		foreach (KeyValuePair<ObjectType, int> item in QuestManager.Instance.m_BuildingsLocked)
		{
			LockBuilding(item.Key, Locked: true);
		}
	}

	protected new void Start()
	{
		base.Start();
		AddAction(m_BinButton, OnBinClicked);
		AddAction(m_DuplicateButton, OnDuplicateClicked);
		AddAction(m_MoveButton, OnMoveClicked);
		AddAction(m_AreaButton, OnAreaClicked);
		AddAction(m_UndoButton, OnUndoClicked);
		AddAction(m_SelectButton, OnSelectClicked);
		AddAction(m_PlanningButton, OnPlanningClicked);
		AddAction(m_SelectedBuildingName, OnSelectedBuildingNameChanged);
		AddAction(m_SelectedBuildingTeam, OnSelectedBuildingTeamClicked);
		CreateCategoryButtons();
		CreateBuildingButtons();
		ResetScrollView();
	}

	private void OnDestroy()
	{
		if ((bool)CameraManager.Instance)
		{
			CameraManager.Instance.SetGlobalShader(CameraManager.ShaderType.Normal);
		}
	}

	public void OnButtonEnter(BaseGadget NewGadget)
	{
		GameStateEdit.Instance.CursorHoverButton(Hover: true);
	}

	public void OnButtonExit(BaseGadget NewGadget)
	{
		GameStateEdit.Instance.CursorHoverButton(Hover: false);
	}

	private void ResetScrollView()
	{
		m_ScrollView.SetScrollValue(0f);
	}

	public void CreateCategoryButtons()
	{
		Transform inactiveTabs = m_InactiveTabs;
		m_CategoryButtons = new List<BuildingCategoryButton>();
		List<ObjectSubCategory> list = new List<ObjectSubCategory>();
		foreach (ObjectSubCategory validCategory in m_ValidCategories)
		{
			int index = validCategory - m_ValidCategories[0];
			foreach (ObjectType building in m_CategoryInfo[index].m_Buildings)
			{
				bool flag = false;
				bool flag2 = false;
				if (GameOptionsManager.Instance.m_Options.m_GameMode == GameOptions.GameMode.ModeCampaign)
				{
					flag2 = true;
				}
				else if (validCategory == ObjectSubCategory.Prizes || validCategory == ObjectSubCategory.BuildingsPrizes)
				{
					flag2 = true;
				}
				else
				{
					flag = true;
				}
				if (flag2 && m_CategoryInfo[index].m_Buildings.Contains(building) && !QuestManager.Instance.m_BuildingsLocked.ContainsKey(building))
				{
					flag = true;
				}
				if (flag && !CeremonyManager.Instance.IsObjectTypeInCeremonyQueue(building))
				{
					list.Add(validCategory);
					break;
				}
			}
		}
		float num = m_CategoryButtonPrefab.GetComponent<RectTransform>().sizeDelta.x + 5f;
		int num2 = 0;
		foreach (ObjectSubCategory item in list)
		{
			int index2 = item - m_ValidCategories[0];
			bool @new = false;
			foreach (ObjectType item2 in m_NewUnlocked)
			{
				if (m_CategoryInfo[index2].m_Buildings.Contains(item2))
				{
					@new = true;
					break;
				}
			}
			BuildingCategoryButton component = Object.Instantiate(m_CategoryButtonPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, inactiveTabs).GetComponent<BuildingCategoryButton>();
			component.gameObject.SetActive(value: true);
			component.SetInfo(this, m_CategoryInfo[index2], @new);
			float x = m_CategoryButtonPrefab.transform.localPosition.x + (float)num2 * num;
			float y = m_CategoryButtonPrefab.transform.localPosition.y;
			component.transform.localPosition = new Vector3(x, y, 0f);
			m_CategoryButtons.Add(component);
			num2++;
		}
		UpdateCategoryButtons();
	}

	private void UpdateCategoryButtons()
	{
		foreach (BuildingCategoryButton categoryButton in m_CategoryButtons)
		{
			if (categoryButton.m_Info.m_Category == m_CurrentCategory)
			{
				categoryButton.transform.SetParent(m_InactiveTabs);
				Vector3 localPosition = categoryButton.transform.localPosition;
				localPosition.y = 20f;
				categoryButton.transform.localPosition = localPosition;
				categoryButton.SetSelected(Selected: true);
			}
			else
			{
				categoryButton.transform.SetParent(m_InactiveTabs);
				Vector3 localPosition2 = categoryButton.transform.localPosition;
				localPosition2.y = 0f;
				categoryButton.transform.localPosition = localPosition2;
				categoryButton.SetSelected(Selected: false);
			}
		}
	}

	private void DeleteBuildingButtons()
	{
		foreach (BuildingButton button in m_Buttons)
		{
			Object.Destroy(button.gameObject);
		}
		m_Buttons.Clear();
	}

	private void CreateBuildingButtons()
	{
		BuildingCategoryInfo buildingCategoryInfo = m_CategoryInfo[m_CurrentCategory - m_ValidCategories[0]];
		m_UnlockedBuildings = new List<ObjectType>();
		for (int i = 0; i < buildingCategoryInfo.m_Buildings.Count; i++)
		{
			if (!buildingCategoryInfo.m_Locked[i] && !CeremonyManager.Instance.IsObjectTypeInCeremonyQueue(buildingCategoryInfo.m_Buildings[i]))
			{
				m_UnlockedBuildings.Add(buildingCategoryInfo.m_Buildings[i]);
			}
		}
		m_Spacing = m_ScrollView.m_ButtonList.m_Object.GetComponent<RectTransform>().sizeDelta.x + 10f;
		float scrollSize = (float)m_UnlockedBuildings.Count * m_Spacing + 10f;
		if (m_UnlockedBuildings.Count == 0)
		{
			scrollSize = 0f;
		}
		m_ScrollView.SetScrollSize(scrollSize);
		m_ScrollView.m_ButtonList.m_ObjectCount = m_UnlockedBuildings.Count;
		m_ScrollView.m_ButtonList.m_CreateObjectCallback = OnCreateBlueprintButton;
		m_ScrollView.m_ButtonList.m_ButtonsPerRow = m_UnlockedBuildings.Count;
		m_ScrollView.m_ButtonList.CreateButtons();
		UpdateBuildingButtons();
		UpdateCurrentType();
	}

	public void OnCreateBlueprintButton(GameObject NewGadget, int Index)
	{
		ObjectType objectType = m_UnlockedBuildings[Index];
		bool @new = false;
		if (m_NewUnlocked.Contains(objectType))
		{
			@new = true;
		}
		BuildingButton component = NewGadget.GetComponent<BuildingButton>();
		component.SetInfo(this, objectType, @new);
		m_Buttons.Add(component);
	}

	public void UpdateCurrentType()
	{
		foreach (BuildingButton button in m_Buttons)
		{
			if (button.m_Building == m_CurrentType)
			{
				m_ScrollView.PopGadgetOnTop(button);
				button.SetSelected(Selected: true);
			}
			else
			{
				button.SetSelected(Selected: false);
			}
		}
	}

	public void SetCurrentType(ObjectType NewType)
	{
		m_CurrentType = NewType;
		UpdateCurrentType();
	}

	private BuildingButton GetBuildingButton()
	{
		for (int i = 0; i < m_Buttons.Count; i++)
		{
			if (m_Buttons[i].m_Building == m_CurrentType)
			{
				return m_Buttons[i];
			}
		}
		return null;
	}

	public void SetBuiltCount(int Count)
	{
		BuildingButton buildingButton = GetBuildingButton();
		if (buildingButton != null)
		{
			buildingButton.SetBuiltCount(Count);
		}
	}

	public void UpdateBuiltCount()
	{
		BuildingButton buildingButton = GetBuildingButton();
		if (buildingButton != null)
		{
			buildingButton.UpdateBuiltCount();
		}
	}

	public void SetBuilding(BuildingButton NewBuilding)
	{
		ObjectType building = NewBuilding.m_Building;
		if (m_NewUnlocked.Contains(building))
		{
			m_NewUnlocked.Remove(building);
		}
		GameStateManager.Instance.GetCurrentState().GetComponent<GameStateEdit>().SetCurrentObjectType(building);
		SetCurrentType(building);
	}

	public void SetCategory(ObjectSubCategory NewCategory)
	{
		if (m_CurrentCategory != NewCategory)
		{
			m_CurrentCategory = NewCategory;
			ResetScrollView();
			DeleteBuildingButtons();
			CreateBuildingButtons();
			UpdateCategoryButtons();
		}
	}

	public void LockBuilding(ObjectType BuildingType, bool Locked)
	{
		for (int i = 0; i < m_CategoryInfo.Count; i++)
		{
			for (int j = 0; j < m_CategoryInfo[i].m_Buildings.Count; j++)
			{
				if (m_CategoryInfo[i].m_Buildings[j] == BuildingType)
				{
					m_CategoryInfo[i].m_Locked[j] = Locked;
					break;
				}
			}
		}
		if (!Locked)
		{
			DeleteBuildingButtons();
			CreateBuildingButtons();
		}
	}

	public void UpdateMode(GameStateEdit.State NewState)
	{
		if (NewState == GameStateEdit.State.Delete)
		{
			m_BinButton.SetSelected(Selected: true);
		}
		else
		{
			m_BinButton.SetSelected(Selected: false);
		}
		if (NewState == GameStateEdit.State.Duplicate || NewState == GameStateEdit.State.AddDuplicate)
		{
			m_DuplicateButton.SetSelected(Selected: true);
		}
		else
		{
			m_DuplicateButton.SetSelected(Selected: false);
		}
		if (NewState == GameStateEdit.State.Move || NewState == GameStateEdit.State.Moving)
		{
			m_MoveButton.SetSelected(Selected: true);
		}
		else
		{
			m_MoveButton.SetSelected(Selected: false);
		}
		if (NewState == GameStateEdit.State.Area || NewState == GameStateEdit.State.AreaDrag)
		{
			m_AreaButton.SetSelected(Selected: true);
		}
		else
		{
			m_AreaButton.SetSelected(Selected: false);
		}
		if (NewState == GameStateEdit.State.Select || NewState == GameStateEdit.State.Selected)
		{
			m_SelectButton.SetSelected(Selected: true);
		}
		else
		{
			m_SelectButton.SetSelected(Selected: false);
		}
	}

	public void UpdateBuiltCounts()
	{
		for (int i = 0; i < m_Buttons.Count; i++)
		{
			m_Buttons[i].UpdateBuiltCount();
			m_Buttons[i].UpdateButtonUsable();
		}
	}

	public void SetUndoAvailable(bool Available)
	{
		m_UndoAvailable = Available;
		if (!Available)
		{
			m_UndoButton.SetInteractable(Interactable: false);
		}
		else
		{
			m_UndoButton.SetInteractable(Interactable: true);
		}
	}

	public void OnBinClicked(BaseGadget NewGadget)
	{
		if (GameStateManager.Instance.GetActualState() == GameStateManager.State.Edit)
		{
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateEdit>().ToggleDelete();
		}
	}

	public void OnDuplicateClicked(BaseGadget NewGadget)
	{
		if (GameStateManager.Instance.GetActualState() == GameStateManager.State.Edit)
		{
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateEdit>().ToggleDuplicate();
		}
	}

	public void OnMoveClicked(BaseGadget NewGadget)
	{
		if (GameStateManager.Instance.GetActualState() == GameStateManager.State.Edit)
		{
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateEdit>().ToggleMove();
		}
	}

	public void OnAreaClicked(BaseGadget NewGadget)
	{
		if (GameStateManager.Instance.GetActualState() == GameStateManager.State.Edit)
		{
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateEdit>().ToggleArea();
		}
	}

	public void OnUndoClicked(BaseGadget NewGadget)
	{
		if (GameStateManager.Instance.GetActualState() == GameStateManager.State.Edit)
		{
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateEdit>().Undo();
		}
	}

	public void OnSelectClicked(BaseGadget NewGadget)
	{
		if (GameStateManager.Instance.GetActualState() == GameStateManager.State.Edit)
		{
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateEdit>().ToggleSelect();
		}
	}

	public void OnPlanningClicked(BaseGadget NewGadget)
	{
		GameStateManager.Instance.PushState(GameStateManager.State.Planning);
	}

	public void OnConnectedClicked(BaseGadget NewGadget)
	{
		CameraManager.Instance.ToggleConnected();
	}

	public void OnWalledAreasClicked(BaseGadget NewGadget)
	{
		CameraManager.Instance.ToggleWalledAreas();
	}

	public GameObject GetButtonFromObjectType(ObjectType NewType)
	{
		ObjectSubCategory subCategoryFromType = ObjectTypeList.Instance.GetSubCategoryFromType(NewType);
		if (m_CurrentCategory == subCategoryFromType)
		{
			foreach (BuildingButton button in m_Buttons)
			{
				if (button.m_Building == NewType)
				{
					return button.gameObject;
				}
			}
		}
		else
		{
			foreach (BuildingCategoryButton categoryButton in m_CategoryButtons)
			{
				if (categoryButton.m_Info.m_Category == subCategoryFromType)
				{
					return categoryButton.gameObject;
				}
			}
		}
		return null;
	}

	public void ShowObstruction()
	{
		m_Obstruction.SetActive(value: true);
		m_ObstructionTimer = 2f;
	}

	public void SetSelectedBuilding(Building SelectedBuilding)
	{
		m_SelectedBuilding = SelectedBuilding;
		if (m_SelectedBuilding != null)
		{
			m_SelectedBuildingName.SetStartText(m_SelectedBuilding.GetHumanReadableName());
			m_SelectedBuildingName.SetInteractable(Interactable: true);
			bool interactable = BuildingReferenceManager.Instance.ShowBuildingReferences(m_SelectedBuilding);
			m_SelectedBuildingTeam.SetInteractable(interactable);
		}
		else
		{
			m_SelectedBuildingName.SetStartText(TextManager.Instance.Get("BuildingSelectedNameDefault"));
			m_SelectedBuildingName.SetInteractable(Interactable: false);
			m_SelectedBuildingTeam.SetInteractable(Interactable: false);
			BuildingReferenceManager.Instance.Clear();
		}
	}

	public void OnSelectedBuildingNameChanged(BaseGadget NewGadget)
	{
		m_SelectedBuilding.SetName(m_SelectedBuildingName.GetText());
	}

	public void OnSelectedBuildingTeamClicked(BaseGadget NewGadget)
	{
		if (GameStateManager.Instance.GetActualState() == GameStateManager.State.Edit)
		{
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateEdit>().SelectedBuidingTeamSelected();
		}
	}

	private void UpdateBuildingButtons()
	{
		float num = m_Buttons[0].GetComponent<RectTransform>().anchoredPosition.x;
		foreach (BuildingButton button in m_Buttons)
		{
			ObjectType building = button.m_Building;
			if (VariableManager.Instance.GetVariableAsInt(building, "UpgradeFrom", CheckValid: false) == 0 || ResourceManager.Instance.GetResource(building) != 0)
			{
				button.SetActive(Active: true);
				float y = button.GetComponent<RectTransform>().anchoredPosition.y;
				button.GetComponent<RectTransform>().anchoredPosition = new Vector2(num, y);
				num += m_Spacing;
			}
			else
			{
				button.SetActive(Active: false);
			}
		}
		num += 10f;
		if (m_Buttons.Count == 0)
		{
			num = 0f;
		}
		m_ScrollView.SetScrollSize(num);
	}

	protected new void Update()
	{
		base.Update();
		UpdateBuildingButtons();
		if (m_ObstructionTimer > 0f)
		{
			m_ObstructionTimer -= TimeManager.Instance.m_NormalDelta;
			if (m_ObstructionTimer <= 0f)
			{
				m_Obstruction.SetActive(value: false);
			}
		}
	}
}
