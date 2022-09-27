using System;
using UnityEngine;
using UnityEngine.Events;

public class GameStateSave : GameStateBase
{
	[Serializable]
	public class FileSelectEvent : UnityEvent<string>
	{
	}

	public static GameStateSave Instance;

	private string m_FileName;

	private bool m_SaveConfirmed;

	private SaveLoadPanel m_SaveLoadPanel;

	[HideInInspector]
	public GameObject m_Menu;

	protected new void Awake()
	{
		Instance = this;
		base.Awake();
		Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/SaveLoad/SaveLoadPanel", typeof(GameObject));
		m_SaveLoadPanel = UnityEngine.Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, menusRootTransform).GetComponent<SaveLoadPanel>();
		m_SaveLoadPanel.transform.localPosition = new Vector3(HudManager.Instance.m_HalfScaledWidth, HudManager.Instance.m_HalfScaledHeight, 0f);
		m_SaveLoadPanel.Init(Load: false, Recordings: false, SaveFilePreviewSelected, SaveFileDeleteSelected);
		m_SaveConfirmed = false;
	}

	public override void ShutDown()
	{
		UnityEngine.Object.Destroy(m_SaveLoadPanel.gameObject);
		base.ShutDown();
	}

	public override void Pushed(GameStateManager.State NewState)
	{
		base.Pushed(NewState);
	}

	public override void Popped(GameStateManager.State NewState)
	{
		base.Popped(NewState);
		if (m_SaveConfirmed)
		{
			m_SaveConfirmed = false;
			GameStateManager.Instance.PushState(GameStateManager.State.Confirm);
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateConfirm>().SetConfirm(ConfirmSave, "ConfirmSaveGame");
		}
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

	public void ConfirmSave()
	{
		if (SaveLoadManager.Instance.Save(m_FileName))
		{
			GameStateManager.Instance.PopState();
			GameStateManager.Instance.PushState(GameStateManager.State.OK);
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateOK>().SetMessage("OKFileSaved");
			return;
		}
		GameStateManager.Instance.PushState(GameStateManager.State.Error);
		if (SaveFile.CheckDiskFull())
		{
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateError>().SetError("ErrorSaveFailed", "ErrorSaveDiskFull");
		}
		else
		{
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateError>().SetError("ErrorSaveFailed", "ErrorSaveDenied");
		}
	}

	public void ConfirmSave2()
	{
		m_SaveConfirmed = true;
	}

	public void SaveFilePreviewSelected(string Name, bool QuickLoad)
	{
		m_FileName = Name;
		if (SaveLoadManager.Instance.DoesFileExist(Name))
		{
			if (!QuickLoad)
			{
				GameStateManager.Instance.PushState(GameStateManager.State.NewGame);
				GameStateManager.Instance.GetCurrentState().GetComponent<GameStateNewGame>().SetDataFromFile(m_FileName, Load: false);
			}
			else
			{
				GameStateManager.Instance.PushState(GameStateManager.State.Confirm);
				GameStateManager.Instance.GetCurrentState().GetComponent<GameStateConfirm>().SetConfirm(ConfirmSave, "ConfirmSaveGame");
			}
		}
		else
		{
			ConfirmSave();
		}
	}

	public override void UpdateState()
	{
		if (!m_SaveConfirmed && MyInputManager.m_Rewired.GetButtonDown("Quit"))
		{
			AudioManager.Instance.StartEvent("UIOptionCancelled");
			GameStateManager.Instance.PopState();
		}
	}
}
