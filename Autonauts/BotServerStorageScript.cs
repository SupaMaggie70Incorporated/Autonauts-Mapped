using UnityEngine;

public class BotServerStorageScript : MonoBehaviour
{
	private BaseImage m_Head;

	private BaseInputField m_Name;

	private BaseButtonImage m_Download;

	private BaseButtonImage m_Delete;

	public StoredScript m_Script;

	private BotServerSelect m_Parent;

	private void CheckGadgets()
	{
		if (!(m_Head != null))
		{
			m_Head = base.transform.Find("Head").GetComponent<BaseImage>();
			m_Name = base.transform.Find("Name").GetComponent<BaseInputField>();
			m_Download = base.transform.Find("DownloadButton").GetComponent<BaseButtonImage>();
			m_Download.SetInteractable(Interactable: false);
			m_Delete = base.transform.Find("DeleteButton").GetComponent<BaseButtonImage>();
		}
	}

	private void Start()
	{
		CheckGadgets();
		m_Parent.RemoveAction(m_Download);
		m_Parent.RemoveAction(m_Delete);
		m_Parent.RemoveAction(m_Name);
		m_Name.SetAction(OnNameChanged, m_Name);
		m_Download.SetAction(OnDownloadClicked, m_Download);
		m_Delete.SetAction(OnDeleteClicked, m_Delete);
	}

	public void SetScript(StoredScript NewScript, BotServerSelect Parent)
	{
		CheckGadgets();
		m_Parent = Parent;
		m_Script = NewScript;
		m_Name.SetStartText(m_Script.m_Name);
		Sprite icon = IconManager.Instance.GetIcon(NewScript.m_Head);
		m_Head.SetSprite(icon);
		UpdateDownloadAvailable();
	}

	public void UpdateDownloadAvailable()
	{
		if (m_Parent.m_Target != null && !m_Parent.m_Target.m_Learning && m_Parent.m_Target.m_WorkerInterpreter.GetCurrentScript() == null)
		{
			m_Download.SetInteractable(Interactable: true);
		}
		else
		{
			m_Download.SetInteractable(Interactable: false);
		}
	}

	public void OnNameChanged(BaseGadget NewGadget)
	{
		m_Script.m_Name = m_Name.GetText();
	}

	public void ConfirmDelete()
	{
		m_Parent.Delete(this);
	}

	public void OnDownloadClicked(BaseGadget NewGadget)
	{
		m_Parent.Download(this);
	}

	public void OnDeleteClicked(BaseGadget NewGadget)
	{
		GameStateManager.Instance.PushState(GameStateManager.State.Confirm);
		GameStateManager.Instance.GetCurrentState().GetComponent<GameStateConfirm>().SetConfirm(ConfirmDelete, "ConfirmDeleteScript");
	}
}
