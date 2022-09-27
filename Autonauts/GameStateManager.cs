using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
	public enum State
	{
		Normal,
		SelectWorker,
		TeachWorker,
		Edit,
		Planning,
		BuildingSelect,
		Inventory,
		DragInventorySlot,
		CheatTools,
		CreativeTools,
		Paused,
		PlatformPaused,
		Save,
		Load,
		Confirm,
		Settings,
		About,
		SelectObject,
		FreeCam,
		PlayCameraSequence,
		BackupRestore,
		RenameSign,
		Loading,
		CreateWorld,
		Ceremony,
		NewGame,
		Terraform,
		EditArea,
		Error,
		OK,
		Badges,
		Industry,
		Evolution,
		Academy,
		Research,
		Autopedia,
		Drag,
		Stats,
		AnyKey,
		Arcade,
		MissionEditor,
		MissionList,
		ObjectSelect,
		SetTargetTile,
		SetSpacePort,
		Start,
		MainMenuCreate,
		MainMenu,
		LanguageSelect,
		ModsPanel,
		ModsUploadConfirm,
		ModsError,
		ModsOptions,
		ModsAnyKey,
		ModsPopup,
		ModsPopupConfirm,
		ModsPanelLocalOnly,
		PlaybackLoading,
		Playback,
		SceneChange,
		Total
	}

	public static GameStateManager Instance;

	private string[] m_PrefabNames = new string[60]
	{
		"GameStateNormal", "GameStateSelectWorker", "GameStateTeachWorker2", "GameStateEdit", "GameStatePlanning", "GameStateBuildingSelect", "GameStateInventory", "GameStateDragInventorySlot", "GameStateCheatTools", "GameStateCreativeTools",
		"GameStatePaused", "GameStatePlatformPaused", "GameStateSave", "GameStateLoad", "GameStateConfirm", "GameStateSettings", "GameStateAbout", "GameStateSelectObject", "GameStateFreeCam", "GameStatePlayCameraSequence",
		"GameStateBackupRestore", "GameStateRenameSign", "GameStateLoading", "GameStateCreateWorld", "GameStateCeremony", "GameStateNewGame", "GameStateTerraform", "GameStateEditArea", "GameStateError", "GameStateOK",
		"GameStateBadges", "GameStateIndustry", "GameStateEvolution", "GameStateAcademy", "GameStateResearch", "GameStateAutopedia", "GameStateDrag", "GameStateStats", "GameStateAnyKey", "GameStateArcade",
		"GameStateMissionEditor", "GameStateMissionList", "GameStateObjectSelect", "GameStateSetTargetTile", "GameStateSetSpacePort", "GameStateStart", "GameStateMainMenuCreate", "GameStateMainMenu", "GameStateLanguageSelect", "GameStateModsPanel",
		"GameStateModsUploadConfirm", "GameStateModsError", "GameStateModsOptions", "GameStateModsAnyKey", "GameStateModsPopup", "GameStateModsPopupConfirm", "GameStateModsPanelLocalOnly", "GameStatePlaybackLoading", "GameStatePlayback", "GameStateSceneChange"
	};

	private List<GameStateBase> m_StateStack;

	private bool m_FirstTimeInit;

	private State m_CurrentState;

	public State m_OldState;

	private void Awake()
	{
		Instance = this;
		m_StateStack = new List<GameStateBase>();
		m_FirstTimeInit = true;
	}

	protected void OnDestroy()
	{
		DestroyStack(Immediate: true);
	}

	private void DestroyStopState(bool Immediate)
	{
		Immediate = true;
		m_StateStack[m_StateStack.Count - 1].ShutDown();
		if (Immediate)
		{
			Object.DestroyImmediate(m_StateStack[m_StateStack.Count - 1].gameObject);
		}
		else
		{
			Object.Destroy(m_StateStack[m_StateStack.Count - 1].gameObject);
		}
		m_StateStack.RemoveAt(m_StateStack.Count - 1);
	}

	private void DestroyStack(bool Immediate = false)
	{
		while (m_StateStack.Count != 0)
		{
			DestroyStopState(Immediate);
		}
	}

	public void PushState(State NewState)
	{
		if (m_StateStack.Count > 0)
		{
			if (m_StateStack[m_StateStack.Count - 1].m_BaseState == NewState)
			{
				return;
			}
			m_StateStack[m_StateStack.Count - 1].Pushed(NewState);
		}
		if (NewState != State.Total)
		{
			GameStateBase component = Object.Instantiate((GameObject)Resources.Load("Prefabs/GameStates/" + m_PrefabNames[(int)NewState], typeof(GameObject)), new Vector3(0f, 0f, 0f), Quaternion.identity, null).GetComponent<GameStateBase>();
			component.m_BaseState = NewState;
			m_StateStack.Add(component);
		}
	}

	public void PopState(bool Immediate = false)
	{
		m_OldState = m_StateStack[m_StateStack.Count - 1].m_BaseState;
		DestroyStopState(Immediate);
		if (m_StateStack.Count > 0)
		{
			m_StateStack[m_StateStack.Count - 1].Popped(m_OldState);
		}
		else
		{
			SetState(State.Normal);
		}
	}

	public void SetState(State NewState)
	{
		DestroyStack();
		PushState(NewState);
	}

	public void StartWardrobe(Wardrobe NewWardrobe)
	{
		PushState(State.Inventory);
		List<BaseClass> players = CollectionManager.Instance.GetPlayers();
		Instance.GetCurrentState().GetComponent<GameStateInventory>().SetInfo(players[0].GetComponent<FarmerPlayer>(), NewWardrobe);
	}

	public void StartAquarium(Aquarium NewAquarium)
	{
		PushState(State.Inventory);
		List<BaseClass> players = CollectionManager.Instance.GetPlayers();
		Instance.GetCurrentState().GetComponent<GameStateInventory>().SetInfo(players[0].GetComponent<FarmerPlayer>(), NewAquarium);
	}

	public void StartCatapult(Catapult NewCatapult)
	{
		PushState(State.SetTargetTile);
		List<BaseClass> players = CollectionManager.Instance.GetPlayers();
		Instance.GetCurrentState().GetComponent<GameStateSetTargetTile>().SetInfo(players[0].GetComponent<FarmerPlayer>(), NewCatapult);
	}

	public void StartSelectBuilding(Building NewBuilding)
	{
		if (NewBuilding.m_TypeIdentifier != ObjectType.BotServer)
		{
			GameStateNormal component = GetCurrentState().GetComponent<GameStateNormal>();
			if ((bool)component)
			{
				component.ClearSelectedWorkers();
			}
		}
		PushState(State.BuildingSelect);
		m_StateStack[m_StateStack.Count - 1].GetComponent<GameStateBuildingSelect>().SetBuilding(NewBuilding);
	}

	public void StartRenameSign(Sign NewSign)
	{
		PushState(State.RenameSign);
		m_StateStack[m_StateStack.Count - 1].GetComponent<GameStateRenameSign>().SetSign(NewSign);
	}

	public void StartSpacePort(SpacePort NewSpacePort)
	{
		PushState(State.SetSpacePort);
		List<BaseClass> players = CollectionManager.Instance.GetPlayers();
		Instance.GetCurrentState().GetComponent<GameStateSetSpacePort>().SetInfo(players[0].GetComponent<FarmerPlayer>(), NewSpacePort);
	}

	public GameStateBase GetCurrentState()
	{
		if (m_StateStack.Count == 0)
		{
			return null;
		}
		return m_StateStack[m_StateStack.Count - 1];
	}

	public GameStateBase GetState(State NewState)
	{
		foreach (GameStateBase item in m_StateStack)
		{
			if (item.m_BaseState == NewState)
			{
				return item;
			}
		}
		return null;
	}

	public State GetActualState()
	{
		if (m_StateStack.Count == 0)
		{
			return State.Total;
		}
		return m_StateStack[m_StateStack.Count - 1].m_BaseState;
	}

	private void Update()
	{
		if (m_FirstTimeInit)
		{
			m_FirstTimeInit = false;
			if ((bool)CameraManager.Instance)
			{
				CameraManager.Instance.UpdateInput();
			}
		}
		if (Application.isEditor && Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift))
		{
			SteamTest.Instance.TestToggleOverlay();
		}
		else if (m_StateStack.Count != 0)
		{
			GameStateBase gameStateBase = m_StateStack[m_StateStack.Count - 1];
			if ((bool)gameStateBase)
			{
				gameStateBase.UpdateState();
			}
		}
	}
}
