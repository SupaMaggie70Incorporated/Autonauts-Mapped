using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadPanel : BaseMenu
{
	private static float m_SpacingX = 200f;

	private static float m_SpacingY = 170f;

	private static int m_PreviewsWide = 1;

	public bool m_Load;

	private bool m_Recordings;

	private List<SaveFilePreview> m_Previews;

	private BaseScrollView m_ScrollView;

	private BaseText m_Title;

	private BaseToggle m_AutosavesToggle;

	private Action<string, bool> m_OnPreviewSelected;

	private Action<string> m_OnPreviewDeleted;

	private SaveFilePreview m_SaveFilePreview;

	private static bool m_ShowAutoSaves = true;

	protected new void Awake()
	{
		base.Awake();
		Transform transform = base.transform.Find("BasePanelOptions").Find("Panel");
		m_Title = transform.Find("TitleStrip/Title").GetComponent<BaseText>();
		m_ScrollView = transform.Find("BaseScrollView").GetComponent<BaseScrollView>();
	}

	protected new void Start()
	{
		base.Start();
		m_AutosavesToggle = base.transform.Find("BasePanelOptions").Find("Panel").Find("AutosavesToggle")
			.GetComponent<BaseToggle>();
		AddAction(m_AutosavesToggle, OnAutosavesClicked);
		BaseButton component = base.transform.Find("BasePanelOptions").Find("Panel").Find("OpenExplorer")
			.GetComponent<BaseButton>();
		AddAction(component, OnOpenExplorerClicked);
		m_AutosavesToggle.gameObject.SetActive(m_Load);
		m_AutosavesToggle.SetStartOn(m_ShowAutoSaves);
	}

	public void Init(bool Load, bool Recordings, Action<string, bool> OnPreviewSelected, Action<string> OnPreviewDeleted)
	{
		m_Load = Load;
		m_Recordings = Recordings;
		m_OnPreviewSelected = OnPreviewSelected;
		m_OnPreviewDeleted = OnPreviewDeleted;
		SetupTitleAndButton();
		CreatePreviews();
	}

	private void SetupTitleAndButton()
	{
		if (m_Load)
		{
			if (m_Recordings)
			{
				m_Title.SetTextFromID("SaveLoadPanelTitleRecordings");
			}
			else
			{
				m_Title.SetTextFromID("SaveLoadPanelTitleLoad");
			}
		}
		else
		{
			m_Title.SetTextFromID("SaveLoadPanelTitleSave");
		}
	}

	private void AddPreview(GameObject Prefab, string NewName)
	{
		SaveFile saveFile = new SaveFile();
		saveFile.LoadPreview(NewName);
		Transform parent = m_ScrollView.GetContent().transform;
		SaveFilePreview component = UnityEngine.Object.Instantiate(Prefab, new Vector3(0f, 0f, 0f), Quaternion.identity, parent).GetComponent<SaveFilePreview>();
		component.Init(saveFile, this);
		float x = (float)(m_Previews.Count % m_PreviewsWide) * m_SpacingX - m_SpacingX * (float)(m_PreviewsWide - 1);
		float y = 0f - (float)(m_Previews.Count / m_PreviewsWide) * m_SpacingY - m_ScrollView.m_Padding * 0.5f;
		component.GetComponent<RectTransform>().anchoredPosition = new Vector3(x, y, 0f);
		m_Previews.Add(component);
	}

	private void CreatePreviews()
	{
		GameObject gameObject = (GameObject)Resources.Load("Prefabs/Hud/SaveLoad/SaveFilePreview", typeof(GameObject));
		m_SpacingY = gameObject.GetComponent<RectTransform>().sizeDelta.y + 10f;
		List<string> allSaveNames = SaveFile.GetAllSaveNames(Autosaves: false, m_Recordings);
		if (m_ShowAutoSaves && m_Load)
		{
			foreach (string allSaveName in SaveFile.GetAllSaveNames(Autosaves: true, m_Recordings))
			{
				allSaveNames.Add(allSaveName);
			}
		}
		m_Previews = new List<SaveFilePreview>();
		if (!m_Load)
		{
			AddPreview(gameObject, "");
		}
		foreach (string item in allSaveNames)
		{
			AddPreview(gameObject, item);
		}
		UpdateScrollRectSize(m_Previews.Count, gameObject);
	}

	private void UpdateScrollRectSize(int NumPreviews, GameObject Prefab)
	{
		float num = (float)(NumPreviews - 1) / (float)m_PreviewsWide * m_SpacingY + Prefab.GetComponent<RectTransform>().rect.height;
		if (NumPreviews % m_PreviewsWide != 0)
		{
			num += m_SpacingY;
		}
		m_ScrollView.SetScrollSize(num);
		m_ScrollView.SetScrollValue(0f);
	}

	private void DestroyPreviews()
	{
		foreach (SaveFilePreview preview in m_Previews)
		{
			UnityEngine.Object.Destroy(preview.gameObject);
		}
		m_Previews.Clear();
	}

	private void RefreshPreviews()
	{
		DestroyPreviews();
		CreatePreviews();
	}

	public bool DeleteSaveFile()
	{
		bool result = m_SaveFilePreview.m_SaveFile.Delete(m_SaveFilePreview.m_SaveFile.m_Name);
		RefreshPreviews();
		return result;
	}

	public void OnPreviewSelected(SaveFilePreview NewPreview, bool QuickLoad)
	{
		m_SaveFilePreview = NewPreview;
		m_OnPreviewSelected(m_SaveFilePreview.m_SaveFile.m_Name, QuickLoad);
	}

	public void OnPreviewDeleteClicked(SaveFilePreview NewPreview)
	{
		m_SaveFilePreview = NewPreview;
		m_OnPreviewDeleted(m_SaveFilePreview.m_SaveFile.m_Name);
	}

	public void OnAutosavesClicked(BaseGadget NewGadget)
	{
		m_ShowAutoSaves = m_AutosavesToggle.GetOn();
		RefreshPreviews();
	}

	public void OnOpenExplorerClicked(BaseGadget NewGadget)
	{
		Application.OpenURL("file://" + SaveFile.GetBasePath(Autosave: false));
	}
}
