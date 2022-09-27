using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.UI;

public class HudManager : MonoBehaviour
{
	public static HudManager Instance;

	public static bool m_ValidInstance;

	[HideInInspector]
	public bool m_RolloversEnabled;

	public ConverterRollover m_ConverterRollover;

	private ResearchRollover m_ResearchRollover;

	private StorageRollover m_StorageRollover;

	private StorageFertiliserRollover m_StorageFertiliserRollover;

	private StorageSeedlingsRollover m_StorageSeedlingsRollover;

	private StorageBeehiveRollover m_StorageBeehiveRollover;

	private TroughRollover m_TroughRollover;

	private HousingRollover m_HousingRollover;

	private FolkSeedRehydratorRollover m_FolkSeedRehydratorRollover;

	private StationarySteamEngineRollover m_StationarySteamEngineRollover;

	private SilkwormStationRollover m_SilkwormStationRollover;

	private SpacePortRollover m_SpacePortRollover;

	private LinkedSystemMechanicalRollover m_LinkedSystemMechanicalRollover;

	private TrainRollover m_TrainRollover;

	private TrainRefuellingStationRollover m_TrainRefuellingStationRollover;

	private FlowerPotRollover m_FlowerPotRollover;

	private HoldableRollover m_HoldableRollover;

	private HoldableRollover m_SignRollover;

	private BotRollover m_BotRollover;

	private TrackRollover m_TrackRollover;

	private WarningRollover m_WarningRollover;

	private FolkRollover m_FolkRollover;

	private CropRollover m_CropRollover;

	[HideInInspector]
	public UIRollover m_UIRollover;

	public QuestRollover m_QuestRollover;

	public QuestCompleteRollover m_QuestCompleteRollover;

	private BadgeRollover m_BadgeRollover;

	public CameraSequence m_Sequence;

	private MouseActions m_MouseActions;

	[HideInInspector]
	public Transform m_CanvasRootTransform;

	[HideInInspector]
	public Transform m_EffectsRootTransform;

	[HideInInspector]
	public Transform m_IndicatorsRootTransform;

	[HideInInspector]
	public Transform m_WorkerPanelsRootTransform;

	[HideInInspector]
	public Transform m_WorkerNamesRootTransform;

	[HideInInspector]
	public Transform m_WorldRolloversRootTransform;

	[HideInInspector]
	public Transform m_RolloversRootTransform;

	[HideInInspector]
	public Transform m_TabsRootTransform;

	[HideInInspector]
	public Transform m_TeachingEffectRootTransform;

	[HideInInspector]
	public Transform m_HUDRootTransform;

	[HideInInspector]
	public Transform m_MenusRootTransform;

	[HideInInspector]
	public Transform m_CeremoniesRootTransform;

	[HideInInspector]
	public Transform m_RenderCanvasRootTransform;

	[HideInInspector]
	public Transform m_SaveImageRootTransform;

	[HideInInspector]
	public Transform m_UIRenderCanvasRootTransform;

	[HideInInspector]
	public Transform m_ScaledHUDRootTransform;

	public Transform m_RootTransform;

	private GameObject m_MinusOne;

	private Text m_ExternalWarning;

	private float m_ExternalWarningTimer;

	private PointToObject m_PointToObject;

	private BaseClass m_ObjectPointedAt;

	[HideInInspector]
	public float m_ScreenHeightScaler;

	[HideInInspector]
	public float m_CanvasWidth;

	[HideInInspector]
	public float m_CanvasHeight;

	[HideInInspector]
	public float m_HalfCanvasWidth;

	[HideInInspector]
	public float m_HalfCanvasHeight;

	[HideInInspector]
	public Camera m_RenderCamera;

	[HideInInspector]
	public Camera m_UIRenderCamera;

	private SaveImage m_SaveImage;

	private TabManager m_TabManager;

	private CeremonyManager m_CeremonyManager;

	public InventoryBar m_InventoryBar;

	private TeachWorkerScriptEdit m_ScriptEdit;

	public EditGroup m_CurrentEditGroup;

	private EditGroup m_EditGroup;

	private EditGroup m_EditGroupTemp;

	private PauseButton m_PauseButton;

	public float m_ScaledWidth;

	public float m_ScaledHeight;

	public float m_HalfScaledWidth;

	public float m_HalfScaledHeight;

	private bool m_HideHUD;

	public ModeButton[] m_ModeButtons;

	private GameObject m_ModeButtonObject;

	public TutorialPanelController m_TutorialPanelController;

	public TutorialPointerManager m_TutorialPointerManager;

	private bool m_CaptureScreen;

	private bool m_HudButtonsActive;

	public FastButton m_FastTimeButton;

	private Dictionary<string, int> m_OldCounts = new Dictionary<string, int>();

	private void Awake()
	{
		Instance = this;
		m_ValidInstance = true;
		m_CanvasRootTransform = GameObject.Find("Canvas").transform;
		m_RootTransform = m_CanvasRootTransform.Find("Root");
		m_CanvasWidth = m_CanvasRootTransform.GetComponent<CanvasScaler>().referenceResolution.x;
		m_CanvasHeight = m_CanvasRootTransform.GetComponent<CanvasScaler>().referenceResolution.y;
		m_HalfCanvasWidth = m_CanvasWidth / 2f;
		m_HalfCanvasHeight = m_CanvasHeight / 2f;
		m_ScreenHeightScaler = 1f;
		m_HUDRootTransform = m_RootTransform.Find("HUD");
		m_MenusRootTransform = m_RootTransform.Find("Menus");
		m_HudButtonsActive = true;
	}

	public void CreateUIRollover()
	{
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Rollovers/UIRollover", typeof(GameObject));
		m_UIRollover = UnityEngine.Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, m_MenusRootTransform).GetComponent<UIRollover>();
		m_UIRollover.transform.localPosition = new Vector3(0f, 0f, 0f);
		original = (GameObject)Resources.Load("Prefabs/Hud/Rollovers/BadgeRollover", typeof(GameObject));
		m_BadgeRollover = UnityEngine.Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, m_MenusRootTransform).GetComponent<BadgeRollover>();
		m_BadgeRollover.transform.localPosition = new Vector3(0f, 0f, 0f);
	}

	public void InitMainMenu()
	{
		CreateUIRollover();
		UpdateScaledDimensions();
	}

	private void GetRoots()
	{
		m_EffectsRootTransform = m_RootTransform.Find("Effects");
		m_IndicatorsRootTransform = m_RootTransform.Find("Indicators");
		m_WorkerPanelsRootTransform = m_RootTransform.Find("WorkerPanels");
		m_WorkerNamesRootTransform = m_RootTransform.Find("WorkerNames");
		m_WorldRolloversRootTransform = m_RootTransform.Find("WorldRollovers");
		m_RolloversRootTransform = m_RootTransform.Find("Rollovers");
		m_TabsRootTransform = m_RootTransform.Find("Tabs");
		m_TeachingEffectRootTransform = m_RootTransform.Find("TeachingEffect");
		m_CeremoniesRootTransform = m_RootTransform.Find("Ceremonies");
		m_SaveImageRootTransform = m_RootTransform.Find("SaveIcon");
		m_ScaledHUDRootTransform = m_RootTransform.Find("ScaledHUD");
		if ((bool)GameObject.Find("CanvasCamera"))
		{
			m_RenderCamera = GameObject.Find("CanvasCamera").GetComponent<Camera>();
			m_RenderCamera.gameObject.SetActive(value: false);
			m_RenderCanvasRootTransform = GameObject.Find("RenderCanvas").transform;
			m_UIRenderCamera = GameObject.Find("UICanvasCamera").GetComponent<Camera>();
			m_UIRenderCamera.gameObject.SetActive(value: false);
			m_UIRenderCanvasRootTransform = GameObject.Find("UIRenderCanvas").transform;
		}
	}

	public void InitPlayback()
	{
		GetRoots();
		CreateUIRollover();
		UpdateScaledDimensions();
	}

	public void InitGame()
	{
		GetRoots();
		UpdateScaledDimensions();
		GameObject gameObject = null;
		gameObject = (GameObject)Resources.Load("Prefabs/Hud/Ceremonies/CeremonyManager", typeof(GameObject));
		m_CeremonyManager = UnityEngine.Object.Instantiate(gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity, m_CeremoniesRootTransform).GetComponent<CeremonyManager>();
		m_CeremonyManager.transform.localPosition = new Vector3(0f, 0f, 0f);
		gameObject = (GameObject)Resources.Load("Prefabs/Hud/Rollovers/HoldableRollover", typeof(GameObject));
		m_HoldableRollover = UnityEngine.Object.Instantiate(gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity, m_RolloversRootTransform).GetComponent<HoldableRollover>();
		m_HoldableRollover.transform.localPosition = new Vector3(0f, 0f, 0f);
		gameObject = (GameObject)Resources.Load("Prefabs/Hud/Rollovers/SignRollover", typeof(GameObject));
		m_SignRollover = UnityEngine.Object.Instantiate(gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity, m_RolloversRootTransform).GetComponent<HoldableRollover>();
		m_SignRollover.transform.localPosition = new Vector3(0f, 0f, 0f);
		gameObject = (GameObject)Resources.Load("Prefabs/Hud/Rollovers/BotRollover", typeof(GameObject));
		m_BotRollover = UnityEngine.Object.Instantiate(gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity, m_RolloversRootTransform).GetComponent<BotRollover>();
		m_BotRollover.transform.localPosition = new Vector3(0f, 0f, 0f);
		gameObject = (GameObject)Resources.Load("Prefabs/Hud/Rollovers/TrackRollover", typeof(GameObject));
		m_TrackRollover = UnityEngine.Object.Instantiate(gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity, m_RolloversRootTransform).GetComponent<TrackRollover>();
		m_TrackRollover.transform.localPosition = new Vector3(0f, 0f, 0f);
		gameObject = (GameObject)Resources.Load("Prefabs/Hud/Rollovers/WarningRollover", typeof(GameObject));
		m_WarningRollover = UnityEngine.Object.Instantiate(gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity, m_RolloversRootTransform).GetComponent<WarningRollover>();
		m_WarningRollover.transform.localPosition = new Vector3(0f, 0f, 0f);
		gameObject = (GameObject)Resources.Load("Prefabs/Hud/Rollovers/FolkRollover", typeof(GameObject));
		m_FolkRollover = UnityEngine.Object.Instantiate(gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity, m_RolloversRootTransform).GetComponent<FolkRollover>();
		m_FolkRollover.transform.localPosition = new Vector3(0f, 0f, 0f);
		gameObject = (GameObject)Resources.Load("Prefabs/Hud/Rollovers/CropRollover", typeof(GameObject));
		m_CropRollover = UnityEngine.Object.Instantiate(gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity, m_RolloversRootTransform).GetComponent<CropRollover>();
		m_CropRollover.transform.localPosition = new Vector3(0f, 0f, 0f);
		gameObject = (GameObject)Resources.Load("Prefabs/Hud/Rollovers/ConverterRollover", typeof(GameObject));
		m_ConverterRollover = UnityEngine.Object.Instantiate(gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity, m_RolloversRootTransform).GetComponent<ConverterRollover>();
		m_ConverterRollover.transform.localPosition = new Vector3(0f, 0f, 0f);
		gameObject = (GameObject)Resources.Load("Prefabs/Hud/Rollovers/ResearchRollover", typeof(GameObject));
		m_ResearchRollover = UnityEngine.Object.Instantiate(gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity, m_RolloversRootTransform).GetComponent<ResearchRollover>();
		m_ResearchRollover.transform.localPosition = new Vector3(0f, 0f, 0f);
		gameObject = (GameObject)Resources.Load("Prefabs/Hud/Rollovers/StorageRollover", typeof(GameObject));
		m_StorageRollover = UnityEngine.Object.Instantiate(gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity, m_RolloversRootTransform).GetComponent<StorageRollover>();
		m_StorageRollover.transform.localPosition = new Vector3(0f, 0f, 0f);
		gameObject = (GameObject)Resources.Load("Prefabs/Hud/Rollovers/StorageFertiliserRollover", typeof(GameObject));
		m_StorageFertiliserRollover = UnityEngine.Object.Instantiate(gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity, m_RolloversRootTransform).GetComponent<StorageFertiliserRollover>();
		m_StorageFertiliserRollover.transform.localPosition = new Vector3(0f, 0f, 0f);
		gameObject = (GameObject)Resources.Load("Prefabs/Hud/Rollovers/StorageSeedlingsRollover", typeof(GameObject));
		m_StorageSeedlingsRollover = UnityEngine.Object.Instantiate(gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity, m_RolloversRootTransform).GetComponent<StorageSeedlingsRollover>();
		m_StorageSeedlingsRollover.transform.localPosition = new Vector3(0f, 0f, 0f);
		gameObject = (GameObject)Resources.Load("Prefabs/Hud/Rollovers/StorageBeehiveRollover", typeof(GameObject));
		m_StorageBeehiveRollover = UnityEngine.Object.Instantiate(gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity, m_RolloversRootTransform).GetComponent<StorageBeehiveRollover>();
		m_StorageBeehiveRollover.transform.localPosition = new Vector3(0f, 0f, 0f);
		gameObject = (GameObject)Resources.Load("Prefabs/Hud/Rollovers/SilkwormStationRollover", typeof(GameObject));
		m_SilkwormStationRollover = UnityEngine.Object.Instantiate(gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity, m_RolloversRootTransform).GetComponent<SilkwormStationRollover>();
		m_SilkwormStationRollover.transform.localPosition = new Vector3(0f, 0f, 0f);
		gameObject = (GameObject)Resources.Load("Prefabs/Hud/Rollovers/SpacePortRollover", typeof(GameObject));
		m_SpacePortRollover = UnityEngine.Object.Instantiate(gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity, m_RolloversRootTransform).GetComponent<SpacePortRollover>();
		m_SpacePortRollover.transform.localPosition = new Vector3(0f, 0f, 0f);
		gameObject = (GameObject)Resources.Load("Prefabs/Hud/Rollovers/TroughRollover", typeof(GameObject));
		m_TroughRollover = UnityEngine.Object.Instantiate(gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity, m_RolloversRootTransform).GetComponent<TroughRollover>();
		m_TroughRollover.transform.localPosition = new Vector3(0f, 0f, 0f);
		gameObject = (GameObject)Resources.Load("Prefabs/Hud/Rollovers/HousingRollover", typeof(GameObject));
		m_HousingRollover = UnityEngine.Object.Instantiate(gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity, m_RolloversRootTransform).GetComponent<HousingRollover>();
		m_HousingRollover.transform.localPosition = new Vector3(0f, 0f, 0f);
		gameObject = (GameObject)Resources.Load("Prefabs/Hud/Rollovers/FolkSeedRehydratorRollover", typeof(GameObject));
		m_FolkSeedRehydratorRollover = UnityEngine.Object.Instantiate(gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity, m_RolloversRootTransform).GetComponent<FolkSeedRehydratorRollover>();
		m_FolkSeedRehydratorRollover.transform.localPosition = new Vector3(0f, 0f, 0f);
		gameObject = (GameObject)Resources.Load("Prefabs/Hud/Rollovers/StationarySteamEngineRollover", typeof(GameObject));
		m_StationarySteamEngineRollover = UnityEngine.Object.Instantiate(gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity, m_RolloversRootTransform).GetComponent<StationarySteamEngineRollover>();
		m_StationarySteamEngineRollover.transform.localPosition = new Vector3(0f, 0f, 0f);
		gameObject = (GameObject)Resources.Load("Prefabs/Hud/Rollovers/LinkedSystemMechanicalRollover", typeof(GameObject));
		m_LinkedSystemMechanicalRollover = UnityEngine.Object.Instantiate(gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity, m_RolloversRootTransform).GetComponent<LinkedSystemMechanicalRollover>();
		m_LinkedSystemMechanicalRollover.transform.localPosition = new Vector3(0f, 0f, 0f);
		gameObject = (GameObject)Resources.Load("Prefabs/Hud/Rollovers/TrainRollover", typeof(GameObject));
		m_TrainRollover = UnityEngine.Object.Instantiate(gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity, m_RolloversRootTransform).GetComponent<TrainRollover>();
		m_TrainRollover.transform.localPosition = new Vector3(0f, 0f, 0f);
		gameObject = (GameObject)Resources.Load("Prefabs/Hud/Rollovers/TrainRefuellingStationRollover", typeof(GameObject));
		m_TrainRefuellingStationRollover = UnityEngine.Object.Instantiate(gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity, m_RolloversRootTransform).GetComponent<TrainRefuellingStationRollover>();
		m_TrainRefuellingStationRollover.transform.localPosition = new Vector3(0f, 0f, 0f);
		gameObject = (GameObject)Resources.Load("Prefabs/Hud/Rollovers/FlowerPotRollover", typeof(GameObject));
		m_FlowerPotRollover = UnityEngine.Object.Instantiate(gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity, m_RolloversRootTransform).GetComponent<FlowerPotRollover>();
		m_FlowerPotRollover.transform.localPosition = new Vector3(0f, 0f, 0f);
		CreateUIRollover();
		gameObject = (GameObject)Resources.Load("Prefabs/Hud/Rollovers/QuestRollover", typeof(GameObject));
		m_QuestRollover = UnityEngine.Object.Instantiate(gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity, m_MenusRootTransform).GetComponent<QuestRollover>();
		m_QuestRollover.transform.localPosition = new Vector3(0f, 0f, 0f);
		gameObject = (GameObject)Resources.Load("Prefabs/Hud/Rollovers/QuestCompleteRollover", typeof(GameObject));
		m_QuestCompleteRollover = UnityEngine.Object.Instantiate(gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity, m_MenusRootTransform).GetComponent<QuestCompleteRollover>();
		m_QuestCompleteRollover.transform.localPosition = new Vector3(0f, 0f, 0f);
		gameObject = (GameObject)Resources.Load("Prefabs/Hud/Tabs/TabManager", typeof(GameObject));
		m_TabManager = UnityEngine.Object.Instantiate(gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity, m_TabsRootTransform).GetComponent<TabManager>();
		m_TabManager.transform.localPosition = new Vector3(0f, 0f, 0f);
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Scripting/TeachWorkerScriptController", typeof(GameObject));
		TeachWorkerScriptController component = UnityEngine.Object.Instantiate(original, m_MenusRootTransform).GetComponent<TeachWorkerScriptController>();
		original = (GameObject)Resources.Load("Prefabs/Hud/Scripting/TeachWorkerScriptEdit", typeof(GameObject));
		m_ScriptEdit = UnityEngine.Object.Instantiate(original, component.transform).GetComponent<TeachWorkerScriptEdit>();
		m_ScriptEdit.SetActive(Active: false);
		original = (GameObject)Resources.Load("Prefabs/Hud/Scripting/EditGroup", typeof(GameObject));
		m_EditGroup = UnityEngine.Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, m_MenusRootTransform).GetComponent<EditGroup>();
		m_EditGroup.GetComponent<RectTransform>().anchoredPosition = default(Vector2);
		original = (GameObject)Resources.Load("Prefabs/Hud/Scripting/EditGroupTemp", typeof(GameObject));
		m_EditGroupTemp = UnityEngine.Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, m_MenusRootTransform).GetComponent<EditGroup>();
		m_EditGroupTemp.GetComponent<RectTransform>().anchoredPosition = default(Vector2);
		Transform hUDRootTransform = m_HUDRootTransform;
		gameObject = (GameObject)Resources.Load("Prefabs/Hud/ExternalWarning", typeof(GameObject));
		m_ExternalWarning = UnityEngine.Object.Instantiate(gameObject, default(Vector3), Quaternion.identity, hUDRootTransform).GetComponent<Text>();
		m_ExternalWarning.transform.localPosition = new Vector3(m_CanvasWidth - 20f, 20f, 0f);
		m_ExternalWarning.gameObject.SetActive(value: false);
		m_ExternalWarningTimer = 0f;
		gameObject = (GameObject)Resources.Load("Prefabs/Hud/Inventory/InventoryBar", typeof(GameObject));
		m_InventoryBar = UnityEngine.Object.Instantiate(gameObject, default(Vector3), Quaternion.identity, hUDRootTransform).GetComponent<InventoryBar>();
		m_InventoryBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(40f, 5f);
		m_InventoryBar.SetInventoryButtonActive(Active: true);
		gameObject = (GameObject)Resources.Load("Prefabs/Hud/Scripting/PointToObject", typeof(GameObject));
		m_PointToObject = UnityEngine.Object.Instantiate(gameObject, default(Vector3), Quaternion.identity, m_IndicatorsRootTransform).GetComponent<PointToObject>();
		m_PointToObject.SetTarget(null);
		gameObject = (GameObject)Resources.Load("Prefabs/Hud/SaveImage", typeof(GameObject));
		m_SaveImage = UnityEngine.Object.Instantiate(gameObject, default(Vector3), Quaternion.identity, m_SaveImageRootTransform.transform).GetComponent<SaveImage>();
		m_SaveImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(-62f, 69f);
		gameObject = (GameObject)Resources.Load("Prefabs/Hud/PauseButton", typeof(GameObject));
		m_PauseButton = UnityEngine.Object.Instantiate(gameObject, m_ScaledHUDRootTransform).GetComponent<PauseButton>();
		m_PauseButton.gameObject.SetActive(value: false);
		gameObject = (GameObject)Resources.Load("Prefabs/Hud/Rollovers/MouseActions", typeof(GameObject));
		m_MouseActions = UnityEngine.Object.Instantiate(gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity, m_MenusRootTransform).GetComponent<MouseActions>();
		m_MouseActions.SetActive(Active: false);
		gameObject = (GameObject)Resources.Load("Prefabs/Hud/Tutorial/TutorialPointerManager", typeof(GameObject));
		m_TutorialPointerManager = UnityEngine.Object.Instantiate(gameObject, m_RolloversRootTransform).GetComponent<TutorialPointerManager>();
		gameObject = (GameObject)Resources.Load("Prefabs/Hud/Tutorial/TutorialPanelController", typeof(GameObject));
		m_TutorialPanelController = UnityEngine.Object.Instantiate(gameObject, m_RolloversRootTransform).GetComponent<TutorialPanelController>();
		gameObject = (GameObject)Resources.Load("Prefabs/Hud/Settings/CameraSequence", typeof(GameObject));
		m_Sequence = UnityEngine.Object.Instantiate(gameObject, Instance.m_ScaledHUDRootTransform).GetComponent<CameraSequence>();
		m_Sequence.SetActive(Active: false);
		CreateModeButtons();
		m_FastTimeButton = GameObject.Find("Fast").GetComponent<FastButton>();
		PostProcessingBehaviour component2 = CameraManager.Instance.m_Camera.GetComponent<PostProcessingBehaviour>();
		component2.profile.vignette.enabled = true;
		VignetteModel.Settings settings = component2.profile.vignette.settings;
		settings.intensity = 0.45f;
		component2.profile.vignette.settings = settings;
		m_RolloversEnabled = true;
		m_MinusOne = null;
	}

	private void OnDestroy()
	{
		if ((bool)m_PauseButton)
		{
			UnityEngine.Object.DestroyImmediate(m_PauseButton.gameObject);
			UnityEngine.Object.DestroyImmediate(m_SaveImage.gameObject);
			UnityEngine.Object.DestroyImmediate(m_InventoryBar.gameObject);
			UnityEngine.Object.DestroyImmediate(m_ExternalWarning.gameObject);
			UnityEngine.Object.DestroyImmediate(m_EditGroup.gameObject);
			UnityEngine.Object.DestroyImmediate(m_EditGroupTemp.gameObject);
			UnityEngine.Object.DestroyImmediate(m_ScriptEdit.gameObject);
			UnityEngine.Object.DestroyImmediate(m_PointToObject.gameObject);
			UnityEngine.Object.DestroyImmediate(m_TabManager.gameObject);
			UnityEngine.Object.DestroyImmediate(m_QuestCompleteRollover.gameObject);
			UnityEngine.Object.DestroyImmediate(m_QuestRollover.gameObject);
			UnityEngine.Object.DestroyImmediate(m_BadgeRollover.gameObject);
			UnityEngine.Object.DestroyImmediate(m_FlowerPotRollover.gameObject);
			UnityEngine.Object.DestroyImmediate(m_StationarySteamEngineRollover.gameObject);
			UnityEngine.Object.DestroyImmediate(m_LinkedSystemMechanicalRollover.gameObject);
			UnityEngine.Object.DestroyImmediate(m_TrainRollover.gameObject);
			UnityEngine.Object.DestroyImmediate(m_TrainRefuellingStationRollover.gameObject);
			UnityEngine.Object.DestroyImmediate(m_FolkSeedRehydratorRollover.gameObject);
			UnityEngine.Object.DestroyImmediate(m_HousingRollover.gameObject);
			UnityEngine.Object.DestroyImmediate(m_TroughRollover.gameObject);
			UnityEngine.Object.DestroyImmediate(m_StorageBeehiveRollover.gameObject);
			UnityEngine.Object.DestroyImmediate(m_StorageSeedlingsRollover.gameObject);
			UnityEngine.Object.DestroyImmediate(m_StorageFertiliserRollover.gameObject);
			UnityEngine.Object.DestroyImmediate(m_SilkwormStationRollover.gameObject);
			UnityEngine.Object.DestroyImmediate(m_SpacePortRollover.gameObject);
			UnityEngine.Object.DestroyImmediate(m_StorageRollover.gameObject);
			UnityEngine.Object.DestroyImmediate(m_ResearchRollover.gameObject);
			UnityEngine.Object.DestroyImmediate(m_ConverterRollover.gameObject);
			UnityEngine.Object.DestroyImmediate(m_CropRollover.gameObject);
			UnityEngine.Object.DestroyImmediate(m_FolkRollover.gameObject);
			UnityEngine.Object.DestroyImmediate(m_WarningRollover.gameObject);
			UnityEngine.Object.DestroyImmediate(m_HoldableRollover.gameObject);
			UnityEngine.Object.DestroyImmediate(m_SignRollover.gameObject);
			UnityEngine.Object.DestroyImmediate(m_BotRollover.gameObject);
			UnityEngine.Object.DestroyImmediate(m_TrackRollover.gameObject);
			UnityEngine.Object.DestroyImmediate(m_CeremonyManager.gameObject);
			UnityEngine.Object.DestroyImmediate(m_Sequence.gameObject);
		}
		if ((bool)m_UIRollover)
		{
			UnityEngine.Object.DestroyImmediate(m_UIRollover.gameObject);
		}
		if ((bool)m_ModeButtonObject)
		{
			UnityEngine.Object.DestroyImmediate(m_ModeButtonObject.gameObject);
		}
		if (Instance == this)
		{
			m_ValidInstance = false;
		}
	}

	private void CreateModeButtons()
	{
		m_ModeButtons = new ModeButton[9];
		Transform hUDRootTransform = m_HUDRootTransform;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/ModeButtons", typeof(GameObject));
		m_ModeButtonObject = UnityEngine.Object.Instantiate(original, default(Vector3), Quaternion.identity, hUDRootTransform).gameObject;
		m_ModeButtonObject.GetComponent<RectTransform>().anchoredPosition = default(Vector2);
	}

	public void SetVisible(bool Visible)
	{
		if ((bool)m_IndicatorsRootTransform)
		{
			m_IndicatorsRootTransform.gameObject.SetActive(Visible);
		}
		if ((bool)m_WorkerPanelsRootTransform)
		{
			m_WorkerPanelsRootTransform.gameObject.SetActive(Visible);
		}
		if ((bool)m_WorkerNamesRootTransform)
		{
			m_WorkerNamesRootTransform.gameObject.SetActive(Visible);
		}
		if ((bool)m_RolloversRootTransform)
		{
			m_RolloversRootTransform.gameObject.SetActive(Visible);
		}
		if ((bool)m_TabsRootTransform)
		{
			m_TabsRootTransform.gameObject.SetActive(Visible);
		}
		if ((bool)m_TeachingEffectRootTransform)
		{
			m_TeachingEffectRootTransform.gameObject.SetActive(Visible);
		}
		if ((bool)m_HUDRootTransform)
		{
			m_HUDRootTransform.gameObject.SetActive(Visible);
		}
		if ((bool)m_MenusRootTransform)
		{
			m_MenusRootTransform.gameObject.SetActive(Visible);
		}
		if ((bool)m_CeremoniesRootTransform)
		{
			m_CeremoniesRootTransform.gameObject.SetActive(Visible);
		}
		if ((bool)m_EffectsRootTransform)
		{
			m_EffectsRootTransform.gameObject.SetActive(Visible);
		}
	}

	public void ActivateConverterRollover(bool Activate, Converter Target = null)
	{
		if (!Activate || m_RolloversEnabled)
		{
			if (!Activate)
			{
				m_ConverterRollover.SetConverterTarget(null);
				m_FolkSeedRehydratorRollover.SetStorageTarget(null);
			}
			else if (Target.m_TypeIdentifier == ObjectType.FolkSeedRehydrator)
			{
				m_FolkSeedRehydratorRollover.SetStorageTarget(Target.GetComponent<FolkSeedRehydrator>());
			}
			else
			{
				m_ConverterRollover.SetConverterTarget(Target);
			}
		}
	}

	public void ActivateConverterRollover(bool Activate, ObjectType NewType)
	{
		if (!Activate)
		{
			m_ConverterRollover.SetConverterTarget(ObjectTypeList.m_Total);
			m_FolkSeedRehydratorRollover.SetStorageTarget(null);
		}
		else
		{
			m_ConverterRollover.SetConverterTarget(NewType);
		}
	}

	public void ActivateConverterRollover(bool Activate, ObjectType NewType, Converter Target = null)
	{
		if (!Activate)
		{
			m_ConverterRollover.SetConverterTarget(ObjectTypeList.m_Total);
			m_FolkSeedRehydratorRollover.SetStorageTarget(null);
		}
		else
		{
			m_ConverterRollover.SetConverterTarget(Target, NewType);
		}
	}

	public void ActivateResearchRollover(bool Activate, ResearchStation Target = null)
	{
		if (!(m_ResearchRollover == null) && (!Activate || m_RolloversEnabled))
		{
			if (!Activate)
			{
				m_ResearchRollover.SetResearchTarget((ResearchStation)null);
			}
			else
			{
				m_ResearchRollover.SetResearchTarget(Target);
			}
		}
	}

	public void ActivateResearchRollover(bool Activate, Quest NewQuest = null)
	{
		if (!(m_ResearchRollover == null) && (!Activate || m_RolloversEnabled))
		{
			if (!Activate)
			{
				m_ResearchRollover.SetResearchTarget((Quest)null);
			}
			else
			{
				m_ResearchRollover.SetResearchTarget(NewQuest);
			}
			if ((bool)TimeManager.Instance && TimeManager.Instance.m_NormalTimeEnabled)
			{
				m_ResearchRollover.UpdateWhilePaused(Update: false);
			}
			else
			{
				m_ResearchRollover.UpdateWhilePaused(Update: true);
			}
		}
	}

	public void ActivateStorageRollover(bool Activate, BaseClass Target = null)
	{
		if (!Activate || m_RolloversEnabled)
		{
			if (!Activate)
			{
				m_StorageRollover.SetStorageTarget(null);
				m_StorageFertiliserRollover.SetStorageTarget(null);
				m_StorageSeedlingsRollover.SetStorageTarget(null);
				m_StorageBeehiveRollover.SetStorageTarget(null);
			}
			else if (Target.m_TypeIdentifier == ObjectType.StorageFertiliser)
			{
				m_StorageFertiliserRollover.SetStorageTarget(Target.GetComponent<StorageFertiliser>());
			}
			else if (Target.m_TypeIdentifier == ObjectType.StorageSeedlings)
			{
				m_StorageSeedlingsRollover.SetStorageTarget(Target.GetComponent<StorageSeedlings>());
			}
			else if (Target.m_TypeIdentifier == ObjectType.StorageBeehive || Target.m_TypeIdentifier == ObjectType.StorageBeehiveCrude)
			{
				m_StorageBeehiveRollover.SetStorageTarget(Target.GetComponent<StorageBeehive>());
			}
			else if (Target.m_TypeIdentifier == ObjectType.FolkSeedPod)
			{
				m_StorageRollover.SetStorageTarget(Target);
			}
			else
			{
				m_StorageRollover.SetStorageTarget(Target);
			}
		}
	}

	public void ActivateStationarySteamEngineRollover(bool Activate, BaseClass Target = null)
	{
		if (!Activate || m_RolloversEnabled)
		{
			if (!Activate)
			{
				m_StationarySteamEngineRollover.SetTarget(null);
			}
			else
			{
				m_StationarySteamEngineRollover.SetTarget(Target.GetComponent<LinkedSystemEngine>());
			}
		}
	}

	public void ActivateSilkwormStationRollover(bool Activate, BaseClass Target = null)
	{
		if (!Activate || m_RolloversEnabled)
		{
			if (!Activate)
			{
				m_SilkwormStationRollover.SetTarget(null);
			}
			else
			{
				m_SilkwormStationRollover.SetTarget(Target.GetComponent<SilkwormStation>());
			}
		}
	}

	public void ActivateSpacePortRollover(bool Activate, BaseClass Target = null)
	{
		if (!Activate || m_RolloversEnabled)
		{
			if (!Activate)
			{
				m_SpacePortRollover.SetTarget((SpacePort)null);
			}
			else
			{
				m_SpacePortRollover.SetTarget(Target.GetComponent<SpacePort>());
			}
		}
	}

	public void ActivateSpacePortRollover(bool Activate, OffworldMission NewMission)
	{
		if (!Activate)
		{
			m_SpacePortRollover.SetTarget((OffworldMission)null);
		}
		else
		{
			m_SpacePortRollover.SetTarget(NewMission);
		}
	}

	public void ActivateLinkedSystemMechanicalRollover(bool Activate, BeltLinkage Target = null)
	{
		if (!Activate || m_RolloversEnabled)
		{
			if (!Activate)
			{
				m_LinkedSystemMechanicalRollover.SetTarget(null);
			}
			else
			{
				m_LinkedSystemMechanicalRollover.SetTarget(Target);
			}
		}
	}

	public void ActivateTrainRollover(bool Activate, BaseClass Target = null)
	{
		if (!Activate || m_RolloversEnabled)
		{
			if (!Activate)
			{
				m_TrainRollover.SetTarget(null);
			}
			else
			{
				m_TrainRollover.SetTarget(Target.GetComponent<Train>());
			}
		}
	}

	public void ActivateTrainRefuellingStationRollover(bool Activate, BaseClass Target = null)
	{
		if (!Activate || m_RolloversEnabled)
		{
			if (!Activate)
			{
				m_TrainRefuellingStationRollover.SetTarget(null);
			}
			else
			{
				m_TrainRefuellingStationRollover.SetTarget(Target.GetComponent<TrainRefuellingStation>());
			}
		}
	}

	public void ActivateHoldableRollover(bool Activate, Selectable Target = null, bool ToTop = false)
	{
		if (!Activate || m_RolloversEnabled)
		{
			m_HoldableRollover.SetTarget(Tile.TileType.Total);
			m_HoldableRollover.SetTarget(ObjectTypeList.m_Total);
			if ((bool)Target)
			{
				if (Target.m_TypeIdentifier == ObjectType.FlowerPot)
				{
					Instance.ActivateFlowerPotRollover(Activate, Target.GetComponent<Selectable>());
				}
				else if ((bool)Target.GetComponent<Converter>())
				{
					Instance.ActivateConverterRollover(Activate, Target.GetComponent<Converter>());
				}
				else if ((bool)Target.GetComponent<ResearchStation>())
				{
					Instance.ActivateResearchRollover(Activate, Target.GetComponent<ResearchStation>());
				}
				else if (!Target.GetComponent<Storage>())
				{
					if ((bool)Target.GetComponent<MobileStorage>())
					{
						Instance.ActivateStorageRollover(Activate, Target.GetComponent<BaseClass>());
					}
					else if ((bool)Target.GetComponent<LinkedSystemEngine>())
					{
						Instance.ActivateStationarySteamEngineRollover(Activate, Target.GetComponent<LinkedSystemEngine>());
					}
					else if ((bool)Target.GetComponent<SilkwormStation>())
					{
						Instance.ActivateSilkwormStationRollover(Activate, Target.GetComponent<SilkwormStation>());
					}
					else if ((bool)Target.GetComponent<SpacePort>())
					{
						Instance.ActivateSpacePortRollover(Activate, Target.GetComponent<SpacePort>());
					}
					else if ((bool)Target.GetComponent<BeltLinkage>())
					{
						Instance.ActivateLinkedSystemMechanicalRollover(Activate, Target.GetComponent<BeltLinkage>());
					}
					else if ((bool)Target.GetComponent<Train>())
					{
						Instance.ActivateTrainRollover(Activate, Target.GetComponent<Train>());
					}
					else if ((bool)Target.GetComponent<TrainRefuellingStation>())
					{
						Instance.ActivateTrainRefuellingStationRollover(Activate, Target.GetComponent<TrainRefuellingStation>());
					}
					else if ((bool)Target.GetComponent<Folk>())
					{
						Instance.ActivateFolkRollover(Activate, Target.GetComponent<Folk>());
					}
					else if (Target.m_TypeIdentifier == ObjectType.CropWheat || Target.m_TypeIdentifier == ObjectType.CropCotton || Target.m_TypeIdentifier == ObjectType.CropCarrot)
					{
						Instance.ActivateCropRollover(Activate, Target);
					}
					else if ((bool)Target.GetComponent<Trough>())
					{
						Instance.ActivateTroughRollover(Activate, Target);
					}
					else if ((bool)Target.GetComponent<Housing>())
					{
						Instance.ActivateHousingRollover(Activate, Target);
					}
					else if ((bool)Target.GetComponent<Worker>())
					{
						Instance.ActivateBotRollover(Activate, Target.GetComponent<Worker>());
					}
					else if ((bool)Target.GetComponent<Sign>())
					{
						Instance.ActivateSignRollover(Activate, Target.GetComponent<Sign>());
					}
					else if ((bool)Target.GetComponent<CertificateReward>())
					{
						CertificateReward component = Target.GetComponent<CertificateReward>();
						Quest quest = QuestManager.Instance.GetQuest(component.m_QuestID);
						Instance.ActivateQuestRollover(Activate, quest);
					}
					else if (!Activate)
					{
						m_HoldableRollover.SetTarget(null);
					}
					else
					{
						m_HoldableRollover.SetTarget(Target);
					}
				}
			}
			else
			{
				m_HoldableRollover.SetTarget(Target);
			}
			if (ToTop)
			{
				m_HoldableRollover.transform.SetParent(m_MenusRootTransform);
			}
			else
			{
				m_HoldableRollover.transform.SetParent(m_RolloversRootTransform);
			}
		}
		if (TimeManager.Instance.m_NormalTimeEnabled)
		{
			m_HoldableRollover.UpdateWhilePaused(Update: false);
		}
		else
		{
			m_HoldableRollover.UpdateWhilePaused(Update: true);
		}
	}

	public void ActivateHoldableRollover(bool Activate, ObjectType Target)
	{
		if (!Activate || m_RolloversEnabled)
		{
			m_HoldableRollover.SetTarget(Tile.TileType.Total);
			m_HoldableRollover.SetTarget(null);
			if (!Activate)
			{
				m_HoldableRollover.SetTarget(ObjectTypeList.m_Total);
			}
			else
			{
				m_HoldableRollover.SetTarget(Target);
			}
		}
	}

	public void ActivateBotRollover(bool Activate, Worker NewBot)
	{
		if (!Activate || m_RolloversEnabled)
		{
			if (!Activate)
			{
				m_BotRollover.SetTarget(null);
			}
			else
			{
				m_BotRollover.SetTarget(NewBot);
			}
		}
	}

	public void ActivateSignRollover(bool Activate, Sign NewSign)
	{
		if (!Activate || m_RolloversEnabled)
		{
			if (!Activate)
			{
				m_SignRollover.SetTarget(null);
			}
			else
			{
				m_SignRollover.SetTarget(NewSign);
			}
		}
	}

	public void ActivateTrackRollover(bool Activate, TrainTrack NewTrack)
	{
		if (!Activate || m_RolloversEnabled)
		{
			if (!Activate)
			{
				m_TrackRollover.SetTarget(null);
			}
			else
			{
				m_TrackRollover.SetTarget(NewTrack);
			}
		}
	}

	public void ActivateWarningRollover(bool Activate, string Target = "")
	{
		if (!Activate || m_RolloversEnabled)
		{
			if (!Activate)
			{
				m_WarningRollover.SetTarget("");
			}
			else
			{
				m_WarningRollover.SetTarget(Target);
			}
		}
	}

	public void ActivateFlowerPotRollover(bool Activate, Selectable Target = null)
	{
		if (!Activate || m_RolloversEnabled)
		{
			if (!Activate)
			{
				m_FlowerPotRollover.SetTarget(null);
			}
			else
			{
				m_FlowerPotRollover.SetTarget(Target.GetComponent<FlowerPot>());
			}
		}
	}

	public void ActivateTroughRollover(bool Activate, Selectable Target)
	{
		if (!Activate || m_RolloversEnabled)
		{
			if (!Activate)
			{
				m_TroughRollover.SetTarget(null);
			}
			else
			{
				m_TroughRollover.SetTarget(Target.GetComponent<Trough>());
			}
		}
	}

	public void ActivateHousingRollover(bool Activate, Selectable Target)
	{
		if (!Activate || m_RolloversEnabled)
		{
			if (!Activate)
			{
				m_HousingRollover.SetTarget(null);
			}
			else
			{
				m_HousingRollover.SetTarget(Target.GetComponent<Housing>());
			}
		}
	}

	public void ActivateFolkRollover(bool Activate, Selectable Target = null)
	{
		if (!Activate || m_RolloversEnabled)
		{
			if (!Activate)
			{
				m_FolkRollover.SetTarget(null);
			}
			else if ((bool)Target)
			{
				m_FolkRollover.SetTarget(Target.GetComponent<Folk>());
			}
		}
	}

	public void ActivateCropRollover(bool Activate, Selectable Target = null)
	{
		if (!Activate || m_RolloversEnabled)
		{
			if (!Activate)
			{
				m_CropRollover.SetTarget(null);
			}
			else
			{
				m_CropRollover.SetTarget(Target);
			}
		}
	}

	private Vector3 ConvertUIToScreenSpaceIt(Vector3 Point, Transform NewTransform)
	{
		Vector3 vector = Point;
		vector.x *= NewTransform.localScale.x;
		vector.y *= NewTransform.localScale.y;
		vector += NewTransform.localPosition;
		if ((bool)NewTransform.parent && NewTransform.parent.GetComponent<Canvas>() == null)
		{
			vector = ConvertUIToScreenSpaceIt(vector, NewTransform.parent);
		}
		return vector;
	}

	public Vector3 ConvertUIToScreenSpace(Vector3 Point, Transform NewTransform)
	{
		return ConvertUIToScreenSpaceIt(Point, NewTransform) + new Vector3(640f, 360f);
	}

	public void ActivateUIRollover(bool Activate, string Target, Vector3 Position)
	{
		if (Activate)
		{
			m_UIRollover.SetTarget(Target);
			m_UIRollover.transform.SetParent(m_RootTransform);
			m_UIRollover.transform.SetParent(m_MenusRootTransform);
		}
		else
		{
			m_UIRollover.SetTarget("");
		}
		if ((bool)TimeManager.Instance && TimeManager.Instance.m_NormalTimeEnabled)
		{
			m_UIRollover.UpdateWhilePaused(Update: false);
		}
		else
		{
			m_UIRollover.UpdateWhilePaused(Update: true);
		}
	}

	public void ActivateQuestRollover(bool Activate, Quest Target)
	{
		if (Target != null && (QuestManager.Instance.GetQuestComplete(Target.m_ID) || CheatManager.Instance.m_CheatMissions))
		{
			if (Target.m_Type == Quest.Type.Research)
			{
				ActivateResearchRollover(Activate, Target);
			}
			else
			{
				ActivateQuestCompleteRollover(Activate, Target);
			}
			return;
		}
		if (Target != null && Target.m_Type == Quest.Type.Research)
		{
			ActivateResearchRollover(Activate, Target);
			return;
		}
		if (Target == null)
		{
			ActivateResearchRollover(Activate: false, (Quest)null);
			ActivateQuestCompleteRollover(Activate: false, null);
		}
		if (!Activate)
		{
			m_QuestRollover.SetTarget(null);
		}
		else
		{
			m_QuestRollover.SetTarget(Target);
		}
		m_QuestRollover.transform.SetParent(m_RootTransform);
		m_QuestRollover.transform.SetParent(m_MenusRootTransform);
		if (TimeManager.Instance.m_NormalTimeEnabled)
		{
			m_QuestRollover.UpdateWhilePaused(Update: false);
		}
		else
		{
			m_QuestRollover.UpdateWhilePaused(Update: true);
		}
	}

	public void ActivateQuestCompleteRollover(bool Activate, Quest Target)
	{
		if (!Activate)
		{
			m_QuestCompleteRollover.SetTarget(null);
		}
		else
		{
			m_QuestCompleteRollover.SetTarget(Target);
		}
		m_QuestCompleteRollover.transform.SetParent(m_RootTransform);
		m_QuestCompleteRollover.transform.SetParent(m_MenusRootTransform);
		if (TimeManager.Instance.m_NormalTimeEnabled)
		{
			m_QuestCompleteRollover.UpdateWhilePaused(Update: false);
		}
		else
		{
			m_QuestCompleteRollover.UpdateWhilePaused(Update: true);
		}
	}

	public void ActivateBadgeRollover(bool Activate, Badge Target)
	{
		m_BadgeRollover.SetTarget(Target);
		m_BadgeRollover.transform.SetParent(m_RootTransform);
		m_BadgeRollover.transform.SetParent(m_MenusRootTransform);
	}

	public void CheckQuestRolloverProgress(QuestEvent.Type TestEvent, bool BotOnly, object ExtraData)
	{
		if ((bool)m_QuestRollover)
		{
			m_QuestRollover.CheckProgress(TestEvent, BotOnly, ExtraData);
		}
	}

	public void ActivateTileRollover(bool Activate, Tile.TileType Type, TileCoord Position)
	{
		if ((!SaveLoadManager.m_Video || Input.GetKey(KeyCode.Backslash)) && (!Activate || m_RolloversEnabled))
		{
			m_HoldableRollover.SetTarget(Type);
		}
	}

	public void HideRollovers()
	{
		if (!(m_ConverterRollover == null))
		{
			ActivateConverterRollover(Activate: false);
			ActivateResearchRollover(Activate: false, (ResearchStation)null);
			ActivateStorageRollover(Activate: false);
			ActivateHoldableRollover(Activate: false);
			ActivateBotRollover(Activate: false, null);
			ActivateSignRollover(Activate: false, null);
			ActivateFlowerPotRollover(Activate: false);
			ActivateFolkRollover(Activate: false);
			ActivateCropRollover(Activate: false);
			ActivateTileRollover(Activate: false, Tile.TileType.Total, default(TileCoord));
			ActivateTroughRollover(Activate: false, null);
			ActivateHousingRollover(Activate: false, null);
		}
	}

	public void MinusOne()
	{
	}

	public void RolloversEnabled(bool Enabled)
	{
		if (GeneralUtils.m_InGame)
		{
			m_RolloversEnabled = Enabled;
			m_ConverterRollover.SetConverterTarget(null);
			m_ResearchRollover.SetResearchTarget((ResearchStation)null);
			m_StorageRollover.SetStorageTarget(null);
			m_StorageFertiliserRollover.SetStorageTarget(null);
			m_StorageSeedlingsRollover.SetStorageTarget(null);
			m_StorageBeehiveRollover.SetStorageTarget(null);
			m_TroughRollover.SetTarget(null);
			m_HousingRollover.SetTarget(null);
			m_FolkSeedRehydratorRollover.SetStorageTarget(null);
			m_StationarySteamEngineRollover.SetTarget(null);
			m_SilkwormStationRollover.SetTarget(null);
			m_SpacePortRollover.SetTarget((SpacePort)null);
			m_LinkedSystemMechanicalRollover.SetTarget(null);
			m_TrainRollover.SetTarget(null);
			m_TrainRefuellingStationRollover.SetTarget(null);
			m_FlowerPotRollover.SetTarget(null);
			m_HoldableRollover.SetTarget(null);
			m_HoldableRollover.SetTarget(Tile.TileType.Total);
			m_BotRollover.SetTarget(null);
			m_TrackRollover.SetTarget(null);
			m_WarningRollover.SetTarget("");
			m_FolkRollover.SetTarget(null);
			m_CropRollover.SetTarget(null);
			m_UIRollover.SetTarget("");
			m_QuestRollover.SetTarget(null);
			m_QuestRollover.gameObject.SetActive(Enabled);
			m_QuestCompleteRollover.SetTarget(null);
			m_QuestCompleteRollover.gameObject.SetActive(Enabled);
			m_BadgeRollover.SetTarget(null);
		}
	}

	public void SetExternalLevel()
	{
		m_ExternalWarning.gameObject.SetActive(value: true);
	}

	public void PointToObject(BaseClass NewObject)
	{
		m_ObjectPointedAt = NewObject;
		m_PointToObject.GetComponent<PointToObject>().SetTarget(m_ObjectPointedAt);
	}

	public void StopPointingToObject(BaseClass NewObject)
	{
		if (m_ObjectPointedAt == NewObject)
		{
			m_ObjectPointedAt = null;
			m_PointToObject.GetComponent<PointToObject>().SetTarget(null);
		}
	}

	public void PointToTile(TileCoord NewCoord)
	{
		m_ObjectPointedAt = null;
		m_PointToObject.GetComponent<PointToObject>().SetTargetTile(NewCoord);
	}

	public void StopPointingToTile()
	{
		m_ObjectPointedAt = null;
		m_PointToObject.GetComponent<PointToObject>().SetTarget(null);
	}

	private void UpdateScaledDimensions()
	{
		m_ScaledWidth = m_MenusRootTransform.GetComponent<RectTransform>().rect.width;
		m_ScaledHeight = m_MenusRootTransform.GetComponent<RectTransform>().rect.height;
		m_HalfScaledWidth = m_ScaledWidth / 2f;
		m_HalfScaledHeight = m_ScaledHeight / 2f;
	}

	private void Update()
	{
		UpdateScaledDimensions();
		if (m_MinusOne != null)
		{
			m_MinusOne.transform.SetParent(m_RootTransform);
			Transform parent = GameObject.Find("Canvas").transform;
			m_MinusOne.transform.SetParent(parent);
			m_MinusOne.transform.localPosition = new Vector3(0f, 0f, 0f);
		}
		Rect rect = m_RootTransform.GetComponent<RectTransform>().rect;
		Camera main = Camera.main;
		m_ScreenHeightScaler = (float)main.pixelHeight / rect.height;
		m_CanvasWidth = m_CanvasRootTransform.GetComponent<Canvas>().GetComponent<RectTransform>().rect.width;
		m_CanvasHeight = m_CanvasRootTransform.GetComponent<Canvas>().GetComponent<RectTransform>().rect.height;
		m_HalfCanvasWidth = m_CanvasWidth / 2f;
		m_HalfCanvasHeight = m_CanvasHeight / 2f;
		if ((bool)m_ExternalWarning)
		{
			m_ExternalWarningTimer += TimeManager.Instance.m_NormalDelta;
			if (m_ExternalWarningTimer >= 3f)
			{
				m_ExternalWarning.color = new Color(1f, 1f, 1f, 0.25f);
			}
		}
		if ((bool)CheatManager.Instance && CheatManager.Instance.m_CheatsEnabled && MyInputManager.m_Rewired.GetButtonDown("HideHUD"))
		{
			m_HideHUD = !m_HideHUD;
			float x = 1f;
			if (m_HideHUD)
			{
				x = 0f;
			}
			m_RootTransform.transform.localScale = new Vector3(x, 1f, 1f);
		}
		if (m_CaptureScreen)
		{
			m_CaptureScreen = false;
			ClipboardHelper.CopyToClipboard(ScreenCapture.CaptureScreenshotAsTexture());
			ErrorMessage.Instance.HidePanel(Hide: false);
		}
		else if (MyInputManager.m_Rewired.GetButtonDown("Screenshot"))
		{
			m_CaptureScreen = true;
			ErrorMessage.Instance.HidePanel(Hide: true);
			string text = Application.persistentDataPath + "/Screenshots";
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			ScreenCapture.CaptureScreenshot(text + "/Autonauts_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png");
		}
	}

	private void DebugObjects()
	{
		GameObject[] array = UnityEngine.Object.FindObjectsOfType<GameObject>();
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if (dictionary.ContainsKey(gameObject.name))
			{
				dictionary[gameObject.name]++;
			}
			else
			{
				dictionary.Add(gameObject.name, 1);
			}
		}
		Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
		foreach (KeyValuePair<string, int> item in dictionary)
		{
			if (m_OldCounts.ContainsKey(item.Key))
			{
				if (m_OldCounts[item.Key] < item.Value)
				{
					dictionary2.Add(item.Key, item.Value - m_OldCounts[item.Key]);
				}
			}
			else
			{
				m_OldCounts.Add(item.Key, 0);
			}
			m_OldCounts[item.Key] = item.Value;
		}
		while (dictionary2.Count > 0)
		{
			int num = 1000000000;
			string text = "";
			foreach (KeyValuePair<string, int> item2 in dictionary2)
			{
				if (item2.Value < num)
				{
					num = item2.Value;
					text = item2.Key;
				}
			}
			Debug.Log(text + " + " + num);
			dictionary2.Remove(text);
		}
	}

	public Vector3 ScreenToCanvas(Vector3 Position)
	{
		Vector3 result = Position;
		result.x = result.x / (float)CameraManager.Instance.m_Camera.pixelWidth * m_CanvasWidth;
		result.y = result.y / (float)CameraManager.Instance.m_Camera.pixelHeight * m_CanvasHeight;
		return result;
	}

	public Vector3 CanvasToScreen(Vector3 Position)
	{
		Vector3 result = Position;
		result.x = result.x * (float)CameraManager.Instance.m_Camera.pixelWidth / m_CanvasWidth;
		result.y = result.y * (float)CameraManager.Instance.m_Camera.pixelHeight / m_CanvasHeight;
		return result;
	}

	public Vector3 GetMouseInWorldSpace()
	{
		return m_CanvasRootTransform.transform.TransformPoint(ScreenToCanvas(Input.mousePosition) - new Vector3(m_HalfCanvasWidth, m_HalfCanvasHeight, 0f));
	}

	public void SetSaveImageActive()
	{
		m_SaveImage.Activate();
	}

	public bool GetHudButtonsActive()
	{
		return m_HudButtonsActive;
	}

	public void SetHudButtonsActive(bool Active)
	{
		m_HudButtonsActive = Active;
		if (TabManager.Instance != null)
		{
			TabManager.Instance.SetActive(Active);
		}
		if (m_InventoryBar != null)
		{
			m_InventoryBar.SetActive(Active);
		}
		if (m_ModeButtonObject != null)
		{
			m_ModeButtonObject.SetActive(Active);
		}
	}

	public void EnablePauseButton(bool Enable)
	{
		m_PauseButton.gameObject.SetActive(Enable);
	}

	public void StartGame()
	{
		List<BaseClass> players = CollectionManager.Instance.GetPlayers();
		if (players.Count > 0)
		{
			m_InventoryBar.SetObject(players[0].GetComponent<Farmer>());
		}
		if (GameOptionsManager.Instance.m_Options.m_GameMode == GameOptions.GameMode.ModeCreative)
		{
			ModeButton.Get(ModeButton.Type.Creative).Show();
		}
		if (GameOptionsManager.Instance.m_Options.m_GameMode != GameOptions.GameMode.ModeCreative)
		{
			Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("Folk");
			if (collection != null && collection.Count > 0)
			{
				ModeButton.Get(ModeButton.Type.Evolution).Show();
			}
		}
	}

	public void ToggleHotBar()
	{
	}

	public void SetHotBarEnabled(bool Enabled)
	{
	}

	public void SetIndicatorsVisible(bool Visible)
	{
		if ((bool)m_IndicatorsRootTransform)
		{
			m_IndicatorsRootTransform.gameObject.SetActive(Visible);
		}
		if ((bool)m_WorkerPanelsRootTransform)
		{
			m_WorkerPanelsRootTransform.gameObject.SetActive(Visible);
		}
		if ((bool)m_WorkerNamesRootTransform)
		{
			m_WorkerNamesRootTransform.gameObject.SetActive(Visible);
		}
		if ((bool)m_EffectsRootTransform)
		{
			m_EffectsRootTransform.gameObject.SetActive(Visible);
		}
	}

	public void FarmerToolUsed(Farmer NewFarmer)
	{
		if (m_InventoryBar != null && m_InventoryBar.m_Farmer == NewFarmer)
		{
			m_InventoryBar.UpdateHealth();
		}
		GameStateBase currentState = GameStateManager.Instance.GetCurrentState();
		if (currentState.m_BaseState == GameStateManager.State.Inventory)
		{
			currentState.GetComponent<GameStateInventory>().FarmerToolUsed(NewFarmer);
		}
	}

	public void SetMouseActions(ActionInfo NewInfo, ActionType NewAction, ActionInfo NewInfo2, ActionType NewAction2, ActionInfo NewAltInfo, ActionType NewAltAction, ActionInfo NewAltInfo2, ActionType NewAltAction2)
	{
		m_MouseActions.SetActions(NewInfo, NewAction, NewInfo2, NewAction2, NewAltInfo, NewAltAction, NewAltInfo2, NewAltAction2);
	}

	public void SetGroup(WorkerGroup NewGroup)
	{
		if ((bool)m_CurrentEditGroup)
		{
			m_CurrentEditGroup.SetGroup(null);
		}
		if (NewGroup != null)
		{
			if (NewGroup == WorkerGroupManager.Instance.m_TempGroup)
			{
				m_CurrentEditGroup = m_EditGroupTemp;
			}
			else
			{
				m_CurrentEditGroup = m_EditGroup;
			}
			m_CurrentEditGroup.SetGroup(NewGroup);
		}
		else
		{
			m_CurrentEditGroup = null;
		}
	}
}
