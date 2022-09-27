using UnityEngine;
using UnityEngine.UI;

public class SaveFilePreview : BaseMenu
{
	public SaveFile m_SaveFile;

	public bool m_Autosave;

	private BasePanel m_Panel;

	private BasePanel m_Thumbnail;

	private BaseText m_Title;

	private BaseText m_Date;

	private BaseText m_MapName;

	private BaseButton m_Delete;

	private BaseButton m_Info;

	private BaseButton m_New;

	private BaseInputField m_NewName;

	private SaveLoadPanel m_Parent;

	protected new void Awake()
	{
		base.Awake();
		Transform transform = base.transform.Find("BasePanel");
		m_Panel = transform.GetComponent<BasePanel>();
		m_Panel.m_OnEnterEvent = OnEnter;
		m_Panel.m_OnExitEvent = OnExit;
		m_Thumbnail = transform.Find("Thumbnail").GetComponent<BasePanel>();
		m_Title = transform.Find("Title").GetComponent<BaseText>();
		m_Date = transform.Find("Date").GetComponent<BaseText>();
		m_MapName = transform.Find("MapName").GetComponent<BaseText>();
		m_Delete = transform.Find("DeleteButton").GetComponent<BaseButton>();
		m_Info = transform.Find("InfoButton").GetComponent<BaseButton>();
		m_New = transform.Find("NewButton").GetComponent<BaseButton>();
		m_NewName = transform.Find("NewName").GetComponent<BaseInputField>();
	}

	protected new void Start()
	{
		base.Start();
		m_Panel.SetAction(OnQuickLoadClicked, m_Panel);
		AddAction(m_Delete, OnDeleteClicked);
		AddAction(m_Info, OnClicked);
		AddAction(m_New, OnNewName);
	}

	private void SetNewButton()
	{
		m_New.gameObject.SetActive(!m_SaveFile.m_PreviewLoaded);
		m_NewName.gameObject.SetActive(!m_SaveFile.m_PreviewLoaded);
	}

	private void SetThumbnail()
	{
		if (m_SaveFile.m_PreviewLoaded && m_SaveFile.m_Thumbnail != null && m_SaveFile.m_Thumbnail.m_Texture != null)
		{
			Sprite sprite = Sprite.Create(m_SaveFile.m_Thumbnail.m_Texture, new Rect(0f, 0f, m_SaveFile.m_Thumbnail.m_Texture.width, m_SaveFile.m_Thumbnail.m_Texture.height), new Vector2(0.5f, 0.5f));
			sprite.texture.filterMode = FilterMode.Point;
			m_Thumbnail.transform.Find("Mask").Find("Image").GetComponent<Image>()
				.sprite = sprite;
		}
		m_Thumbnail.gameObject.SetActive(m_SaveFile.m_PreviewLoaded);
	}

	private void SetSummary()
	{
		string text = m_SaveFile.m_Name;
		if (m_Autosave)
		{
			text = TextManager.Instance.Get("AutosaveFilename") + " ";
			text += m_SaveFile.m_Name.Substring(SaveLoadManager.m_AutosaveName.Length);
		}
		m_Title.SetText(text);
		m_Title.gameObject.SetActive(m_SaveFile.m_PreviewLoaded);
		string dateString = m_SaveFile.m_Summary.GetDateString();
		m_Date.SetText(dateString);
		m_Date.gameObject.SetActive(m_SaveFile.m_PreviewLoaded);
		string mapName = m_SaveFile.m_Summary.m_GameOptions.m_MapName;
		m_MapName.SetText(mapName);
		m_MapName.gameObject.SetActive(m_SaveFile.m_PreviewLoaded);
		m_Delete.gameObject.SetActive(m_SaveFile.m_PreviewLoaded);
		m_Info.gameObject.SetActive(m_SaveFile.m_PreviewLoaded);
		if (m_Autosave)
		{
			m_Panel.SetBackingColour(new Color(0.5f, 1f, 1f));
		}
	}

	public void Init(SaveFile NewSaveFile, SaveLoadPanel NewParent)
	{
		m_SaveFile = NewSaveFile;
		m_Parent = NewParent;
		m_Autosave = false;
		if (m_SaveFile.m_Name.Contains(SaveLoadManager.m_AutosaveName))
		{
			m_Autosave = true;
		}
		SetSummary();
		SetThumbnail();
		SetNewButton();
	}

	public void OnClicked(BaseGadget NewGadget)
	{
		if (m_SaveFile.m_PreviewLoaded)
		{
			m_Parent.OnPreviewSelected(this, QuickLoad: false);
		}
	}

	public void OnDeleteClicked(BaseGadget NewGadget)
	{
		m_Parent.OnPreviewDeleteClicked(this);
	}

	public void OnQuickLoadClicked(BaseGadget NewGadget)
	{
		if (m_SaveFile.m_PreviewLoaded)
		{
			m_Parent.OnPreviewSelected(this, QuickLoad: true);
		}
	}

	public void OnNewName(BaseGadget NewGadget)
	{
		m_SaveFile.m_Name = m_NewName.GetText();
		if (m_SaveFile.m_Name != "")
		{
			m_Parent.OnPreviewSelected(this, QuickLoad: false);
		}
	}

	public void OnEnter(BaseGadget NewGadget)
	{
		m_Panel.SetBorderVisible(Visible: true);
	}

	public void OnExit(BaseGadget NewGadget)
	{
		m_Panel.SetBorderVisible(Visible: false);
	}
}
