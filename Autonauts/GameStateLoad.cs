using System;
using UnityEngine;
using UnityEngine.Events;

public class GameStateLoad : GameStateBase
{
	[Serializable]
	public class FileSelectEvent : UnityEvent<string>
	{
	}

	public static GameStateLoad Instance;

	private string m_FileName;

	private bool m_LoadConfirmed;

	private bool m_Recordings;

	private SaveLoadPanel m_SaveLoadPanel;

	private GameObject m_Blocker;

	[HideInInspector]
	public GameObject m_Menu;

	protected new void Awake()
	{
		base.Awake();
		Instance = this;
		m_LoadConfirmed = false;
	}

	public void Init(bool Recordings)
	{
		m_Recordings = Recordings;
		Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/SaveLoad/SaveLoadPanel", typeof(GameObject));
		m_SaveLoadPanel = UnityEngine.Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, menusRootTransform).GetComponent<SaveLoadPanel>();
		m_SaveLoadPanel.transform.localPosition = new Vector3(HudManager.Instance.m_HalfScaledWidth, HudManager.Instance.m_HalfScaledHeight, 0f);
		m_SaveLoadPanel.Init(Load: true, m_Recordings, SaveFilePreviewSelected, SaveFileDeleteSelected);
		original = (GameObject)Resources.Load("Prefabs/Hud/SaveLoad/Blocker", typeof(GameObject));
		m_Blocker = UnityEngine.Object.Instantiate(original, menusRootTransform).gameObject;
		m_Blocker.SetActive(value: false);
	}

	public override void ShutDown()
	{
		UnityEngine.Object.Destroy(m_SaveLoadPanel.gameObject);
		base.ShutDown();
	}

	public override void Pushed(GameStateManager.State NewState)
	{
		m_Blocker.SetActive(value: true);
		base.Pushed(NewState);
	}

	public override void Popped(GameStateManager.State NewState)
	{
		m_Blocker.SetActive(value: false);
		base.Popped(NewState);
		if (m_LoadConfirmed)
		{
			DoLoad();
		}
	}

	public void ConfirmLoad()
	{
		m_LoadConfirmed = true;
	}

	private void DoLoad()
	{
		base.gameObject.SetActive(value: false);
		if (!m_Recordings)
		{
			SessionManager.Instance.LoadLevel(LoadLevel: true, "Main");
		}
		else
		{
			SessionManager.Instance.LoadLevel(LoadLevel: false, "Playback");
		}
		GameStateManager.Instance.SetState(GameStateManager.State.SceneChange);
	}

	public void ConfirmDelete()
	{
		if (!m_SaveLoadPanel.DeleteSaveFile())
		{
			GameStateManager.Instance.PushState(GameStateManager.State.Error);
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateError>().SetError("ErrorDeleteFailed", "ErrorDeleteDenied");
		}
	}

	public void SaveFileDeleteSelected(string Name)
	{
		m_FileName = Name;
		GameStateManager.Instance.PushState(GameStateManager.State.Confirm);
		GameStateManager.Instance.GetCurrentState().GetComponent<GameStateConfirm>().SetConfirm(ConfirmDelete, "ConfirmDeleteGame");
	}

	public void SaveFilePreviewSelected(string Name, bool QuickLoad)
	{
		m_FileName = Name;
		SessionManager.Instance.m_LoadFileName = m_FileName;
		int num = SaveLoadManager.Instance.CheckValidFile(m_FileName);
		if (num == 0)
		{
			GameStateManager.Instance.PushState(GameStateManager.State.NewGame);
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateNewGame>().SetDataFromFile(m_FileName, Load: true);
			if (QuickLoad)
			{
				ModManager.Instance.ResetBeforeLoad();
				GameStateManager.Instance.GetCurrentState().GetComponent<GameStateNewGame>().m_Menu.OnStartClicked(null);
			}
		}
		else
		{
			GameStateManager.Instance.PushState(GameStateManager.State.Error);
			if (num == 1)
			{
				GameStateManager.Instance.GetCurrentState().GetComponent<GameStateError>().SetError("ErrorLoadFailed", "ErrorLoadInvalid");
			}
			else
			{
				GameStateManager.Instance.GetCurrentState().GetComponent<GameStateError>().SetError("ErrorLoadFailed", "ErrorLoadOldFile");
			}
		}
	}

	public override void UpdateState()
	{
		if (!m_LoadConfirmed && MyInputManager.m_Rewired.GetButtonDown("Quit"))
		{
			AudioManager.Instance.StartEvent("UIOptionCancelled");
			GameStateManager.Instance.PopState();
		}
	}
}
