using System.Collections.Generic;
using UnityEngine;

public class BotServerSelect : BuildingSelect
{
	public static BotServerSelect Instance;

	private BaseAutoComplete m_AutoComplete;

	private List<BotServerStorageScript> m_Scripts;

	private BaseButton m_UploadButton;

	public Worker m_Target;

	private BotServerStorageScript m_DefaultPrefab;

	private BaseScrollView m_ScrollView;

	private Vector2 m_StartPosition;

	private float m_Height;

	private Transform m_Parent;

	private new void Awake()
	{
		Instance = this;
		base.Awake();
		SetupBotSearch();
		SetupStoredList();
	}

	protected new void Start()
	{
		base.Start();
		m_AutoComplete.ForceFocus();
		RemoveAction(m_UploadButton);
		m_UploadButton.SetAction(OnUploadClicked, m_UploadButton);
	}

	private void OnDestroy()
	{
		if ((bool)m_Building)
		{
			if ((bool)m_Building.m_Engager)
			{
				m_Building.m_Engager.SendAction(new ActionInfo(ActionType.DisengageObject, m_Building.m_Engager.GetComponent<TileCoordObject>().m_TileCoord, m_Building));
			}
			m_Building.GetComponent<BotServer>().SetWorking(Working: false);
		}
	}

	private void SetupStoredList()
	{
		m_Scripts = new List<BotServerStorageScript>();
		m_UploadButton = base.transform.Find("StorageList/UploadButton").GetComponent<BaseButton>();
		m_UploadButton.SetInteractable(Interactable: false);
		m_ScrollView = base.transform.Find("StorageList/BaseScrollView").GetComponent<BaseScrollView>();
		m_DefaultPrefab = m_ScrollView.GetContent().transform.Find("Script").GetComponent<BotServerStorageScript>();
		m_DefaultPrefab.gameObject.SetActive(value: false);
		m_StartPosition = m_DefaultPrefab.GetComponent<RectTransform>().anchoredPosition;
		m_Height = m_DefaultPrefab.GetComponent<RectTransform>().sizeDelta.y + 5f;
		m_Parent = m_DefaultPrefab.transform.parent;
		foreach (StoredScript script in WorkerScriptManager.Instance.m_Scripts)
		{
			AddScript(script);
		}
		UpdatePositions();
	}

	public void SetTarget(Worker NewWorker)
	{
		m_Target = NewWorker;
		if ((bool)NewWorker && NewWorker.m_WorkerInterpreter.m_HighInstructions != null && !NewWorker.m_Learning)
		{
			m_UploadButton.SetInteractable(Interactable: true);
		}
		foreach (BotServerStorageScript script in m_Scripts)
		{
			script.UpdateDownloadAvailable();
		}
	}

	private void AddScript(StoredScript NewStoredScript)
	{
		BotServerStorageScript botServerStorageScript = Object.Instantiate(m_DefaultPrefab, m_Parent);
		botServerStorageScript.gameObject.SetActive(value: true);
		botServerStorageScript.SetScript(NewStoredScript, this);
		m_Scripts.Insert(0, botServerStorageScript);
	}

	private void UpdatePositions()
	{
		Vector2 startPosition = m_StartPosition;
		foreach (BotServerStorageScript script in m_Scripts)
		{
			script.GetComponent<RectTransform>().anchoredPosition = startPosition;
			startPosition.y -= m_Height;
		}
		m_ScrollView.SetScrollSize(0f - startPosition.y);
	}

	public void Delete(BotServerStorageScript NewScript)
	{
		m_Scripts.Remove(NewScript);
		WorkerScriptManager.Instance.RemoveScript(NewScript.m_Script);
		Object.Destroy(NewScript.gameObject);
		UpdatePositions();
	}

	public void Download(BotServerStorageScript NewScript)
	{
		HighInstructionList instructions = NewScript.m_Script.m_Instructions;
		m_Target.m_WorkerInterpreter.SetHighInstructions(instructions.m_List, Start: false);
		GameStateManager.Instance.PopState();
	}

	public void OnUploadClicked(BaseGadget NewGadget)
	{
		HighInstructionList highInstructions = m_Target.m_WorkerInterpreter.m_HighInstructions;
		if (highInstructions != null)
		{
			StoredScript newStoredScript = WorkerScriptManager.Instance.AddScript(m_Target.m_WorkerName, highInstructions, m_Target.m_Head);
			AddScript(newStoredScript);
			UpdatePositions();
		}
	}

	private void SetupBotSearch()
	{
		m_AutoComplete = base.transform.Find("Search/BaseAutoComplete").GetComponent<BaseAutoComplete>();
		m_AutoComplete.SetObjectsCallback(GetPossibleSearchObjects);
		m_AutoComplete.SetAction(OnSearchComplete, m_AutoComplete);
	}

	public override void SetBuilding(Building NewBuilding)
	{
		base.SetBuilding(NewBuilding);
		m_Building.GetComponent<BotServer>().SetWorking(Working: true);
	}

	private void AddGroup(WorkerGroupPanel NewGroup, List<BaseAutoCompletePossible> PossibleObjects, string NewText)
	{
		if (!NewGroup.m_IsGroup)
		{
			return;
		}
		string text = NewGroup.m_Group.m_Name.ToUpper();
		if (text != "" && text.Contains(NewText))
		{
			BaseAutoCompletePossible item = new BaseAutoCompletePossible(NewGroup.m_Group.m_Name, NewGroup);
			PossibleObjects.Add(item);
		}
		foreach (int groupUID in NewGroup.m_Group.m_GroupUIDs)
		{
			WorkerGroup groupFromID = WorkerGroupManager.Instance.GetGroupFromID(groupUID);
			AddGroup(groupFromID.m_WorkerGroupPanel, PossibleObjects, NewText);
		}
	}

	private List<BaseAutoCompletePossible> GetPossibleSearchObjects(string SearchString)
	{
		List<BaseAutoCompletePossible> list = new List<BaseAutoCompletePossible>();
		string text = SearchString.ToUpper().TrimStart(' ').TrimEnd(' ');
		if (text != "")
		{
			Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("Worker");
			if (collection != null)
			{
				foreach (KeyValuePair<BaseClass, int> item2 in collection)
				{
					Worker component = item2.Key.GetComponent<Worker>();
					string text2 = component.m_WorkerName.ToUpper();
					if (text2 != "" && text2.Contains(text))
					{
						BaseAutoCompletePossible item = new BaseAutoCompletePossible(component.m_WorkerName, component);
						list.Add(item);
					}
				}
			}
			{
				foreach (WorkerGroupPanel item3 in TabWorkers.Instance.m_Group)
				{
					AddGroup(item3, list, text);
				}
				return list;
			}
		}
		return list;
	}

	public void OnSearchComplete(BaseAutoComplete NewGadget)
	{
		object currentObject = m_AutoComplete.GetCurrentObject();
		if (currentObject == null)
		{
			return;
		}
		if (currentObject is Worker)
		{
			Worker worker = (Worker)currentObject;
			GameStateManager.Instance.PopState();
			CameraManager.Instance.Focus(worker.transform.position);
			if (GameStateManager.Instance.GetActualState() == GameStateManager.State.Normal)
			{
				GameStateManager.Instance.GetCurrentState().GetComponent<GameStateNormal>().SetSelectedWorker(worker);
			}
			if (TabManager.Instance.m_ActiveTabType != TabManager.TabType.Workers)
			{
				TabManager.Instance.TabClicked(TabManager.TabType.Workers);
			}
			TabWorkers.Instance.ScrollToPanel(worker.m_WorkerInfoPanel);
		}
		if (currentObject is WorkerGroupPanel)
		{
			WorkerGroupPanel newPanel = (WorkerGroupPanel)currentObject;
			GameStateManager.Instance.PopState();
			if (TabManager.Instance.m_ActiveTabType != TabManager.TabType.Workers)
			{
				TabManager.Instance.TabClicked(TabManager.TabType.Workers);
			}
			TabWorkers.Instance.ScrollToPanel(newPanel);
		}
	}
}
