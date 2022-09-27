using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewGameOptions : BaseMenu
{
	private bool m_InGame;

	private bool m_InPlayback;

	private bool m_Load;

	private bool m_Save;

	private GameObject m_Panel;

	private GameObject m_BackButton;

	private GameOptions.GameMode m_GameMode;

	private BaseButtonText m_CampaignButton;

	private BaseButtonText m_FreeButton;

	private BaseButtonText m_CreativeButton;

	private BaseRawImage m_MapImage;

	private Image m_CursorImage;

	private Image m_FlagImage;

	private Image m_NewFlagImage;

	private ButtonList m_ResourceList;

	private BaseText m_DefaultResource;

	private BaseInputField m_SeedInputField;

	private BaseText m_SeedFinalText;

	private BaseInputField m_NameInputField;

	private BaseButtonImage m_NewButton;

	private BaseToggle m_SmallMapToggle;

	private BaseToggle m_RandomObjectsToggle;

	private BaseToggle m_RecordingToggle;

	private BaseToggle m_TutorialToggle;

	private BaseButtonImage m_StartButton;

	private BaseImage m_GeneratingPanel;

	private BaseText m_Generating;

	private PlaySound m_GeneratingSound;

	private Texture2D m_TileMapTexture;

	private float m_Timer;

	private GameOptions m_GameOptionsManager;

	private TileCoord m_NewFlagCoord;

	private bool m_ValidStartPosition;

	private bool m_HoverMap;

	private List<BaseText> m_Resources;

	private SaveFile m_SaveFile;

	private bool m_Adjusting;

	private Dictionary<int, int> m_UsedTiles;

	private static int m_FloodSizeRequired = 5000;

	private bool m_BlockNewMap;

	private Tile.TileType m_IndicatedTileType = Tile.TileType.Total;

	private float m_ResourceFlashTimer;

	private void CheckGadgets()
	{
		if (!m_Panel)
		{
			m_Panel = base.transform.Find("BasePanelOptions").gameObject;
			m_CampaignButton = m_Panel.transform.Find("CampaignButton").GetComponent<BaseButtonText>();
			m_FreeButton = m_Panel.transform.Find("FreeButton").GetComponent<BaseButtonText>();
			m_CreativeButton = m_Panel.transform.Find("CreativeButton").GetComponent<BaseButtonText>();
			m_MapImage = m_Panel.transform.Find("MapMask").Find("Map").GetComponent<BaseRawImage>();
			m_MapImage.SetAction(OnMapClicked, m_MapImage);
			m_CursorImage = m_MapImage.transform.Find("Cursor").GetComponent<Image>();
			m_FlagImage = m_MapImage.transform.Find("FlagImage").GetComponent<Image>();
			m_NewFlagImage = m_MapImage.transform.Find("NewFlagImage").GetComponent<Image>();
			m_GeneratingPanel = m_MapImage.transform.Find("GeneratingPanel").GetComponent<BaseImage>();
			m_GeneratingPanel.gameObject.SetActive(value: false);
			m_Generating = m_MapImage.transform.Find("Generating").GetComponent<BaseText>();
			m_Generating.gameObject.SetActive(value: false);
			m_ResourceList = m_Panel.transform.Find("Results").Find("ResourceList").GetComponent<ButtonList>();
			m_ResourceList.m_CreateObjectCallback = OnCreateResource;
			m_NameInputField = m_Panel.transform.Find("NameInputField").GetComponent<BaseInputField>();
			m_SeedInputField = m_Panel.transform.Find("SeedInputField").GetComponent<BaseInputField>();
			m_SeedInputField.m_OnValueChangedEvent = OnSeedValueChanged;
			m_SeedFinalText = m_SeedInputField.transform.Find("Text Area").Find("FinalText").GetComponent<BaseText>();
			m_NewButton = m_Panel.transform.Find("NewButton").GetComponent<BaseButtonImage>();
			m_RandomObjectsToggle = m_Panel.transform.Find("RandomObjectsToggle").GetComponent<BaseToggle>();
			m_RecordingToggle = m_Panel.transform.Find("RecordingToggle").GetComponent<BaseToggle>();
			m_SmallMapToggle = m_Panel.transform.Find("SmallMapToggle").GetComponent<BaseToggle>();
			m_TutorialToggle = m_Panel.transform.Find("TutorialToggle").GetComponent<BaseToggle>();
			m_StartButton = m_Panel.transform.Find("StartButton").GetComponent<BaseButtonImage>();
		}
	}

	protected new void Awake()
	{
		base.Awake();
		CheckGadgets();
		AddAction(m_CampaignButton, OnCampaignClicked);
		AddAction(m_FreeButton, OnFreeClicked);
		AddAction(m_CreativeButton, OnCreativeClicked);
		AddAction(m_SmallMapToggle, OnSmallMapChanged);
		AddAction(m_RecordingToggle, OnRecordingChanged);
		AddAction(m_NewButton, OnNewClicked);
		AddAction(m_SeedInputField, OnSeedChanged);
		AddAction(m_NameInputField, OnNameChanged);
		AddAction(m_StartButton, OnStartClicked);
		m_SaveFile = null;
	}

	private void OnDestroy()
	{
		StopGenerating();
		HudManager.Instance.ActivateUIRollover(Activate: false, "", default(Vector3));
	}

	public void OnCreateResource(GameObject NewObject, int Index)
	{
		int Count = 0;
		Sprite NewImage = null;
		int Min = 0;
		string Rollover = "";
		Tile.TileType NewTileType = Tile.TileType.Total;
		if (m_SaveFile != null)
		{
			m_SaveFile.m_Summary.GetResourceInfo((SaveFileSummary.Resource)Index, out Count, out Min, out NewImage, out Rollover, out NewTileType);
		}
		else
		{
			SaveFileSummary.GetResourceInfoStatic((SaveFileSummary.Resource)Index, out Count, out Min, out NewImage, out Rollover, out NewTileType, m_GameOptionsManager.m_MapTileData);
		}
		Color newColour = GeneralUtils.ColorFromHex(13298687);
		if (m_ResourceList.m_Buttons.Count % 2 == 0)
		{
			newColour = GeneralUtils.ColorFromHex(9361919);
		}
		NewObject.GetComponent<NewGameResource>().Set(Count, Min, NewImage, Rollover, newColour, this, NewTileType);
	}

	private void SetupResources()
	{
		m_ResourceList.m_ObjectCount = 11;
		m_ResourceList.CreateButtons();
	}

	private void SetData(GameOptions NewGameOptionsManager, TileManager NewTileManager, bool Load, bool Save, TileCoord PlayerPosition, TileCoord FlagPosition)
	{
		m_GameOptionsManager = NewGameOptionsManager;
		m_Load = Load;
		m_Save = Save;
		SetMode(NewGameOptionsManager.m_GameMode);
		if (NewGameOptionsManager.m_GameSize == GameOptions.GameSize.Small)
		{
			m_SmallMapToggle.SetOn(On: true);
		}
		else
		{
			m_SmallMapToggle.SetOn(On: false);
		}
		m_RandomObjectsToggle.SetOn(NewGameOptionsManager.m_RandomObjectsEnabled);
		m_RecordingToggle.SetOn(NewGameOptionsManager.m_RecordingEnabled);
		m_TutorialToggle.SetOn(NewGameOptionsManager.m_TutorialEnabled);
		if (m_InGame || m_InPlayback)
		{
			if (m_GameMode != 0)
			{
				m_CampaignButton.SetActive(Active: false);
			}
			if (m_GameMode != GameOptions.GameMode.ModeFree)
			{
				m_FreeButton.SetActive(Active: false);
			}
			if (m_GameMode != GameOptions.GameMode.ModeCreative)
			{
				m_CreativeButton.SetActive(Active: false);
			}
			m_CampaignButton.SetInteractable(Interactable: false);
			m_FreeButton.SetInteractable(Interactable: false);
			m_CreativeButton.SetInteractable(Interactable: false);
			m_SmallMapToggle.SetInteractable(Interactable: false);
			m_RandomObjectsToggle.SetInteractable(Interactable: false);
			m_TutorialToggle.gameObject.SetActive(value: false);
			m_NewButton.gameObject.SetActive(value: false);
			m_FlagImage.gameObject.SetActive(value: false);
			m_NewFlagImage.gameObject.SetActive(value: false);
			m_SeedInputField.SetInteractable(Interactable: false);
			m_SeedInputField.ReadOnly(Read: true);
			if (m_Load)
			{
				m_NameInputField.SetInteractable(Interactable: false);
				m_NameInputField.ReadOnly(Read: true);
			}
			if (!NewGameOptionsManager.m_RecordingEnabled || m_Load)
			{
				m_RecordingToggle.SetInteractable(Interactable: false);
			}
			if (Load)
			{
				m_StartButton.SetRolloverFromID("NewGameLoad");
			}
			else if (Save)
			{
				m_StartButton.SetRolloverFromID("NewGameSave");
			}
			else
			{
				m_StartButton.SetRolloverFromID("NewGameApply");
			}
			m_StartButton.SetSprite("Buttons/StandardAcceptButton");
			if ((bool)NewTileManager)
			{
				NewGameOptionsManager.SetMapData(NewTileManager.m_Tiles, NewTileManager.m_TilesWide, NewTileManager.m_TilesHigh);
			}
			NewGameOptionsManager.SetPlayerPosition(PlayerPosition);
			NewGameOptionsManager.SetFlagPosition(FlagPosition);
			if (m_Load || m_Save)
			{
				m_CursorImage.gameObject.SetActive(value: false);
			}
			else
			{
				m_CursorImage.gameObject.SetActive(value: true);
			}
			if (m_InPlayback)
			{
				m_NameInputField.SetInteractable(Interactable: false);
				m_NameInputField.ReadOnly(Read: true);
				m_RecordingToggle.SetInteractable(Interactable: false);
			}
			UpdateAll();
		}
		else
		{
			m_CursorImage.gameObject.SetActive(value: false);
			m_NewFlagImage.gameObject.SetActive(value: false);
			m_StartButton.SetRolloverFromID("NewGameStart");
			if (m_Load || m_Save)
			{
				m_NameInputField.SetInteractable(Interactable: false);
				m_NameInputField.ReadOnly(Read: true);
			}
			StartNewMap();
		}
		m_Timer = 0f;
	}

	private void UpdateAll()
	{
		UpdateSeedText();
		UpdateNameText();
		UpdatePlayerPosition();
		UpdateFlagPosition();
		UpdateMapImage();
		SetupResources();
	}

	public void SetDataFromFile(string FileName, bool Load)
	{
		m_InGame = true;
		m_SaveFile = new SaveFile();
		m_SaveFile.LoadPreview(FileName);
		SetData(m_SaveFile.m_Summary.m_GameOptions, null, Load, !Load, default(TileCoord), default(TileCoord));
		UpdateAll();
	}

	public void SetDataFromNew()
	{
		CheckGadgets();
		m_InGame = false;
		if (SceneManager.GetActiveScene().name == "Main")
		{
			m_InGame = true;
		}
		m_InPlayback = false;
		if (SceneManager.GetActiveScene().name == "Playback")
		{
			m_InPlayback = true;
		}
		TileCoord tileCoord = new TileCoord(GameOptionsManager.Instance.m_Options.m_MapWidth / 2, GameOptionsManager.Instance.m_Options.m_MapHeight / 2);
		if (m_InGame || m_InPlayback)
		{
			List<BaseClass> players = CollectionManager.Instance.GetPlayers();
			if (players != null && players.Count > 0)
			{
				tileCoord = players[0].GetComponent<Farmer>().m_TileCoord;
			}
		}
		GameOptionsManager.Instance.m_Options.m_PlayerPosition = tileCoord;
		SetData(GameOptionsManager.Instance.m_Options, TileManager.Instance, Load: false, Save: false, tileCoord, tileCoord);
	}

	private void SetMode(GameOptions.GameMode NewMode)
	{
		m_GameMode = NewMode;
		new Color(0.75f, 0.75f, 0.75f);
		new Color(1f, 1f, 1f);
		new Color(0.5f, 0.5f, 0.5f);
		m_CampaignButton.SetSelected(m_GameMode == GameOptions.GameMode.ModeCampaign);
		m_FreeButton.SetSelected(m_GameMode == GameOptions.GameMode.ModeFree);
		m_CreativeButton.SetSelected(m_GameMode == GameOptions.GameMode.ModeCreative);
	}

	private void UpdateMapImage()
	{
		if (m_SaveFile != null)
		{
			m_SaveFile.m_Thumbnail.m_Texture.filterMode = FilterMode.Point;
			m_MapImage.SetTexture(m_SaveFile.m_Thumbnail.m_Texture);
			return;
		}
		if (!GeneralUtils.m_InGame)
		{
			ModManager.Instance.SetInitialMapData(m_GameOptionsManager);
		}
		int mapWidth = m_GameOptionsManager.m_MapWidth;
		int mapHeight = m_GameOptionsManager.m_MapHeight;
		if (m_TileMapTexture != null)
		{
			Object.Destroy(m_TileMapTexture);
		}
		m_TileMapTexture = new Texture2D(mapWidth, mapHeight, TextureFormat.RGBA32, mipChain: false, linear: true);
		if (m_TileMapTexture == null)
		{
			m_TileMapTexture = new Texture2D(mapWidth, mapHeight, TextureFormat.ARGB32, mipChain: false, linear: true);
		}
		m_TileMapTexture.filterMode = FilterMode.Point;
		Color32[] array = new Color32[mapWidth * mapHeight];
		Color32 color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
		Color32 color2 = GeneralUtils.ColorFromHex(16732068);
		Color32 color3 = GeneralUtils.ColorFromHex(10039402);
		for (int i = 0; i < mapHeight; i++)
		{
			for (int j = 0; j < mapWidth; j++)
			{
				int num = (mapHeight - i - 1) * mapWidth + j;
				if (m_GameOptionsManager.m_MapTileData != null)
				{
					int num2 = i * mapWidth + j;
					Tile.TileType tileType = m_GameOptionsManager.m_MapTileData[num2];
					if (GeneralUtils.m_InGame)
					{
						Tile tile = TileManager.Instance.GetTile(new TileCoord(j, i));
						if ((bool)tile.m_AssociatedObject)
						{
							if (tile.m_AssociatedObject.m_TypeIdentifier == ObjectType.TreePine)
							{
								tileType = Tile.TileType.Trees;
							}
							else if (tile.m_AssociatedObject.m_TypeIdentifier == ObjectType.CropCotton)
							{
								tileType = Tile.TileType.CropCotton;
							}
							else if (tile.m_AssociatedObject.m_TypeIdentifier == ObjectType.CropWheat)
							{
								tileType = Tile.TileType.CropWheat;
							}
							else if (tile.m_AssociatedObject.m_TypeIdentifier == ObjectType.TallBoulder)
							{
								tileType = Tile.TileType.Raised;
							}
						}
						if ((bool)tile.m_Building || (bool)tile.m_BuildingFootprint)
						{
							array[num] = color2;
						}
						else if ((bool)tile.m_Floor)
						{
							array[num] = color3;
						}
						else
						{
							array[num] = Tile.m_TileInfo[(int)tileType].m_MapColour;
						}
					}
					else
					{
						array[num] = Tile.m_TileInfo[(int)tileType].m_MapColour;
					}
				}
				else
				{
					array[num] = color;
				}
			}
		}
		m_TileMapTexture.SetPixels32(0, 0, mapWidth, mapHeight, array);
		m_TileMapTexture.Apply();
		m_TileMapTexture.filterMode = FilterMode.Point;
		m_MapImage.SetTexture(m_TileMapTexture);
	}

	private void UpdatePlayerPosition()
	{
		float num = (float)m_GameOptionsManager.m_PlayerPosition.x / (float)m_GameOptionsManager.m_MapWidth - 0.5f;
		float num2 = (float)m_GameOptionsManager.m_PlayerPosition.y / (float)m_GameOptionsManager.m_MapHeight - 0.5f;
		num *= m_MapImage.GetComponent<RectTransform>().rect.width;
		num2 *= m_MapImage.GetComponent<RectTransform>().rect.height;
		m_CursorImage.transform.localPosition = new Vector3(num, 0f - num2, 0f);
	}

	private void UpdateFlagPosition()
	{
		float num = (float)m_GameOptionsManager.m_FlagPosition.x / (float)m_GameOptionsManager.m_MapWidth - 0.5f;
		float num2 = (float)m_GameOptionsManager.m_FlagPosition.y / (float)m_GameOptionsManager.m_MapHeight - 0.5f;
		num *= m_MapImage.GetComponent<RectTransform>().rect.width;
		num2 *= m_MapImage.GetComponent<RectTransform>().rect.height;
		m_FlagImage.transform.localPosition = new Vector3(num, 0f - num2, 0f);
	}

	private void UpdateSeedText()
	{
		m_BlockNewMap = true;
		m_SeedInputField.SetPlaceholderText(m_GameOptionsManager.m_MapSeed.ToString());
		m_SeedInputField.SetText(m_GameOptionsManager.m_MapSeed.ToString());
		m_BlockNewMap = false;
	}

	private void UpdateNameText()
	{
		m_NameInputField.SetPlaceholderText(m_GameOptionsManager.m_MapName);
		m_NameInputField.SetText(m_GameOptionsManager.m_MapName);
	}

	private bool GetAreAllResourcesAvailable()
	{
		for (int i = 0; i < 11; i++)
		{
			int Count = 0;
			Sprite NewImage = null;
			int Min = 0;
			string Rollover = "";
			Tile.TileType NewTileType;
			if (m_SaveFile != null)
			{
				m_SaveFile.m_Summary.GetResourceInfo((SaveFileSummary.Resource)i, out Count, out Min, out NewImage, out Rollover, out NewTileType);
			}
			else
			{
				SaveFileSummary.GetResourceInfoStatic((SaveFileSummary.Resource)i, out Count, out Min, out NewImage, out Rollover, out NewTileType, m_GameOptionsManager.m_MapTileData);
			}
			if (Count < Min)
			{
				return false;
			}
		}
		return true;
	}

	private void StartGenerating()
	{
		m_GeneratingPanel.gameObject.SetActive(value: true);
		m_Generating.gameObject.SetActive(value: true);
		if (base.gameObject.activeSelf)
		{
			m_GeneratingSound = AudioManager.Instance.StartEvent("Scanning", null, Remember: true);
		}
	}

	private void StopGenerating()
	{
		if (m_GeneratingSound != null)
		{
			AudioManager.Instance.StopEvent(m_GeneratingSound);
			m_GeneratingSound = null;
		}
		m_GeneratingPanel.gameObject.SetActive(value: false);
		m_Generating.gameObject.SetActive(value: false);
	}

	private void FloodFillTiles(int x, int y)
	{
		if (m_UsedTiles.Count >= m_FloodSizeRequired)
		{
			return;
		}
		int num = y * m_GameOptionsManager.m_MapWidth + x;
		if (m_UsedTiles.ContainsKey(num))
		{
			return;
		}
		m_UsedTiles.Add(num, 0);
		Tile.TileType tileType = m_GameOptionsManager.m_MapTileData[num];
		if (tileType != Tile.TileType.WaterDeep && tileType != Tile.TileType.SeaWaterDeep)
		{
			if (x > 0)
			{
				FloodFillTiles(x - 1, y);
			}
			if (x < m_GameOptionsManager.m_MapWidth - 1)
			{
				FloodFillTiles(x + 1, y);
			}
			if (y > 0)
			{
				FloodFillTiles(x, y - 1);
			}
			if (y < m_GameOptionsManager.m_MapHeight - 1)
			{
				FloodFillTiles(x, y + 1);
			}
		}
	}

	private bool GetIsBadTile(TileCoord NewCoord)
	{
		_ = m_GameOptionsManager.m_MapWidth;
		_ = m_GameOptionsManager.m_MapHeight;
		m_UsedTiles = new Dictionary<int, int>();
		FloodFillTiles(NewCoord.x, NewCoord.y);
		if (m_UsedTiles.Count >= m_FloodSizeRequired)
		{
			return false;
		}
		return true;
	}

	public void MapGenerated()
	{
		if (m_Adjusting || GetAreAllResourcesAvailable())
		{
			StopGenerating();
			m_StartButton.SetInteractable(Interactable: true);
			UpdateAll();
		}
		else
		{
			m_GameOptionsManager.m_MapSeed++;
			UpdateSeedFinalText();
			RefreshMap();
		}
	}

	private void StartNewMap(bool Adjusting = false)
	{
		if (m_SaveFile == null)
		{
			m_Adjusting = Adjusting;
			WorldGenerator.Instance.StartNewMap(MapGenerated, CreateObjects: false);
			StopGenerating();
			StartGenerating();
			if (Adjusting)
			{
				m_Generating.SetTextFromID("NewGameAdjusting");
			}
			else
			{
				m_Generating.SetTextFromID("NewGameGenerating");
				m_GameOptionsManager.m_MapTileData = null;
				UpdateMapImage();
			}
			m_FlagImage.gameObject.SetActive(value: false);
			m_StartButton.SetInteractable(Interactable: false);
		}
	}

	private void RefreshMap()
	{
		if (!m_Load && !m_Save && !m_InGame && !m_InPlayback)
		{
			StartNewMap();
		}
	}

	public void OnCampaignClicked(BaseGadget NewGadget)
	{
		SetMode(GameOptions.GameMode.ModeCampaign);
	}

	public void OnFreeClicked(BaseGadget NewGadget)
	{
		SetMode(GameOptions.GameMode.ModeFree);
	}

	public void OnCreativeClicked(BaseGadget NewGadget)
	{
		SetMode(GameOptions.GameMode.ModeCreative);
	}

	public void OnSmallMapChanged(BaseGadget NewGadget)
	{
		if (m_SmallMapToggle.GetOn())
		{
			GameOptionsManager.Instance.m_Options.SetMapSize(GameOptions.GameSize.Small);
		}
		else
		{
			GameOptionsManager.Instance.m_Options.SetMapSize(GameOptions.GameSize.Medium);
		}
		TileCoord playerPosition = new TileCoord(GameOptionsManager.Instance.m_Options.m_MapWidth / 2 + Plot.m_PlotTilesWide / 2, GameOptionsManager.Instance.m_Options.m_MapHeight / 2 + Plot.m_PlotTilesHigh / 2);
		GameOptionsManager.Instance.m_Options.m_PlayerPosition = playerPosition;
		RefreshMap();
	}

	public void ConfirmStopRecording()
	{
		GameOptionsManager.Instance.m_Options.m_RecordingEnabled = false;
		if (m_InGame)
		{
			m_RecordingToggle.SetInteractable(Interactable: false);
		}
	}

	public void CancelStopRecording()
	{
		m_RecordingToggle.SetOn(On: true);
	}

	public void OnRecordingChanged(BaseGadget NewGadget)
	{
		if (!m_RecordingToggle.GetOn())
		{
			GameStateManager.Instance.PushState(GameStateManager.State.Confirm);
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateConfirm>().SetConfirm(ConfirmStopRecording, "ConfirmStopRecording", "ConfirmStopRecordingDescription", CancelStopRecording);
		}
	}

	public void OnStartClicked(BaseGadget NewGadget)
	{
		GameOptionsManager.Instance.m_Options.m_GameMode = m_GameMode;
		if (m_SmallMapToggle.GetOn())
		{
			GameOptionsManager.Instance.m_Options.SetMapSize(GameOptions.GameSize.Small);
		}
		else
		{
			GameOptionsManager.Instance.m_Options.SetMapSize(GameOptions.GameSize.Medium);
		}
		GameOptionsManager.Instance.m_Options.m_RandomObjectsEnabled = m_RandomObjectsToggle.GetOn();
		GameOptionsManager.Instance.m_Options.m_RecordingEnabled = m_RecordingToggle.GetOn();
		GameOptionsManager.Instance.m_Options.m_TutorialEnabled = m_TutorialToggle.GetOn();
		SaveLoadManager.InitEverything();
		if (m_Load)
		{
			AudioManager.Instance.StartEvent("UIOptionSelected");
			GameStateLoad.Instance.ConfirmLoad();
			ModManager.Instance.ResetBeforeLoad();
			GameStateManager.Instance.PopState();
		}
		else if (m_Save)
		{
			AudioManager.Instance.StartEvent("UIOptionSelected");
			GameStateSave.Instance.ConfirmSave2();
			GameStateManager.Instance.PopState();
		}
		else if (m_InGame || m_InPlayback)
		{
			AudioManager.Instance.StartEvent("UIOptionSelected");
			GameStateManager.Instance.PopState();
		}
		else
		{
			ModManager.Instance.ResetBeforeLoad();
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateNewGame>().StartNewGame();
		}
	}

	public void OnNewClicked(BaseGadget NewGadget)
	{
		TileCoord playerPosition = new TileCoord(GameOptionsManager.Instance.m_Options.m_MapWidth / 2 + Plot.m_PlotTilesWide / 2, GameOptionsManager.Instance.m_Options.m_MapHeight / 2 + Plot.m_PlotTilesHigh / 2);
		GameOptionsManager.Instance.m_Options.m_PlayerPosition = playerPosition;
		m_GameOptionsManager.NewMapSeed();
		RefreshMap();
	}

	private void UpdateSeedFinalText()
	{
		if (int.TryParse(m_SeedInputField.GetText(), out var result))
		{
			string mapSeedString = GeneralUtils.GetMapSeedString(result);
			m_SeedFinalText.SetText(mapSeedString);
		}
	}

	public void OnSeedValueChanged(BaseGadget NewGadget)
	{
		UpdateSeedFinalText();
	}

	public void OnSeedChanged(BaseGadget NewGadget)
	{
		if (!m_BlockNewMap && int.TryParse(m_SeedInputField.GetText(), out var result))
		{
			if (result < 0)
			{
				result = -result;
				m_SeedInputField.SetText(result.ToString());
			}
			m_GameOptionsManager.m_MapSeed = result;
			UpdateSeedFinalText();
			RefreshMap();
		}
	}

	public void OnNameChanged(BaseGadget NewGadget)
	{
		m_GameOptionsManager.m_MapName = m_NameInputField.GetText();
	}

	public void OnMapClicked(BaseGadget NewGadget)
	{
		if (!m_InGame && !m_InPlayback && m_ValidStartPosition && !WorldGenerator.Instance.m_Generating)
		{
			GameOptionsManager.Instance.m_Options.SetPlayerPosition(m_NewFlagCoord);
			m_GameOptionsManager.m_FlagPosition = m_NewFlagCoord;
			m_FlagImage.transform.localPosition = m_NewFlagImage.transform.localPosition;
			StartNewMap(Adjusting: true);
		}
	}

	private bool GetIsLandingAreaInvalid(TileCoord Position)
	{
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < 2; j++)
			{
				Tile.TileType tileType = m_GameOptionsManager.m_MapTileData[(Position.y - j) * m_GameOptionsManager.m_MapWidth + Position.x + i];
				if (TileHelpers.GetTileWater(tileType) || tileType == Tile.TileType.Raised)
				{
					return true;
				}
			}
		}
		return false;
	}

	private void UpdateNewFlagCursor()
	{
		if (m_SaveFile != null)
		{
			return;
		}
		Vector3 localPosition = m_MapImage.transform.InverseTransformPoint(Input.mousePosition);
		m_NewFlagImage.transform.localPosition = localPosition;
		Rect rect = m_MapImage.GetComponent<RectTransform>().rect;
		float num = (localPosition.x / rect.width + 0.5f) * (float)m_GameOptionsManager.m_MapWidth;
		float num2 = (1f - (localPosition.y / rect.height + 0.5f)) * (float)m_GameOptionsManager.m_MapHeight;
		m_NewFlagCoord = new TileCoord((int)num, (int)num2);
		if (num >= 1f && num < (float)(m_GameOptionsManager.m_MapWidth - 2) && num2 >= 3f && num2 < (float)(m_GameOptionsManager.m_MapHeight - 1))
		{
			Tile.TileType newType = m_GameOptionsManager.m_MapTileData[(int)num2 * m_GameOptionsManager.m_MapWidth + (int)num];
			string target = TextManager.Instance.Get(Tile.GetNameFromType(newType));
			HudManager.Instance.ActivateUIRollover(Activate: true, target, default(Vector3));
			m_HoverMap = true;
			if (!m_InGame && !m_InPlayback)
			{
				if (!GetIsLandingAreaInvalid(m_NewFlagCoord) && !GetIsBadTile(m_NewFlagCoord))
				{
					m_NewFlagImage.gameObject.SetActive(value: true);
				}
				else
				{
					m_NewFlagImage.gameObject.SetActive(value: false);
				}
			}
		}
		else
		{
			if (m_HoverMap)
			{
				HudManager.Instance.ActivateUIRollover(Activate: false, "", default(Vector3));
				m_HoverMap = false;
			}
			m_NewFlagImage.gameObject.SetActive(value: false);
		}
		m_ValidStartPosition = m_NewFlagImage.gameObject.activeSelf;
	}

	public void ResourceIndicated(Tile.TileType NewTileType)
	{
		m_IndicatedTileType = NewTileType;
		m_ResourceFlashTimer = 0f;
		Color color = new Color(0f, 0f, 0f, 0f);
		if (NewTileType != Tile.TileType.Total)
		{
			color = Tile.m_TileInfo[(int)NewTileType].m_MapColour;
		}
		Vector4 value = new Vector4(color.r, color.g, color.b);
		m_MapImage.GetComponent<RawImage>().materialForRendering.SetVector("_OldColour", value);
	}

	public void UpdateIndicatedResource()
	{
		if (m_IndicatedTileType == Tile.TileType.Total)
		{
			return;
		}
		if (SettingsManager.Instance.m_FlashiesEnabled)
		{
			if (TimeManager.Instance.m_PauseTimeEnabled)
			{
				m_ResourceFlashTimer += TimeManager.Instance.m_PauseDelta;
			}
			else
			{
				m_ResourceFlashTimer += TimeManager.Instance.m_NormalDelta;
			}
		}
		bool num = (int)(m_ResourceFlashTimer * 60f) % 20 < 10;
		Color color = Tile.m_TileInfo[(int)m_IndicatedTileType].m_MapColour;
		if (num)
		{
			if (m_IndicatedTileType == Tile.TileType.Coal)
			{
				color = new Color(1f, 1f, 1f, 1f);
			}
			else
			{
				color.r *= 0.25f;
				color.g *= 0.25f;
				color.b *= 0.25f;
			}
		}
		m_MapImage.GetComponent<RawImage>().materialForRendering.SetVector("_NewColour", color);
	}

	protected new void Update()
	{
		base.Update();
		if (this == null || WorldGenerator.Instance.m_Generating)
		{
			return;
		}
		if (!m_Load && !m_Save)
		{
			m_Timer += TimeManager.Instance.m_NormalDelta;
			if (m_InGame || m_InPlayback)
			{
				if ((int)(m_Timer * 60f) % 20 < 10)
				{
					m_CursorImage.gameObject.SetActive(value: true);
				}
				else
				{
					m_CursorImage.gameObject.SetActive(value: false);
				}
			}
			else if ((int)(m_Timer * 60f) % 20 < 10)
			{
				m_FlagImage.gameObject.SetActive(value: true);
			}
			else
			{
				m_FlagImage.gameObject.SetActive(value: false);
			}
		}
		UpdateNewFlagCursor();
		UpdateIndicatedResource();
	}
}
