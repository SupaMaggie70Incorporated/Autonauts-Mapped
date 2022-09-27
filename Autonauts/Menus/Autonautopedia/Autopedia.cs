using System.Collections.Generic;
using UnityEngine;

public class Autopedia : MonoBehaviour
{
	public enum Page
	{
		Academy,
		Research,
		Objects,
		Tips,
		Total
	}

	public static Autopedia Instance;

	public static Page m_Page;

	private static bool m_ObjectsPageNew;

	private static bool m_TipsPageNew;

	public bool m_CeremonyPlaying;

	private BaseButtonImage m_BackButton;

	private BaseText m_Title;

	private float m_PinnedTimer;

	private GameObject m_Pinned;

	private BaseText m_PinnedText;

	public List<AutopediaPage> m_Pages;

	public ButtonList m_ButtonList;

	private BaseAutoComplete m_Search;

	public static void Init()
	{
		m_Page = Page.Academy;
		Objects.Init();
	}

	private void Awake()
	{
		Instance = this;
		m_BackButton = base.transform.Find("TitleBar/BackButton").GetComponent<BaseButtonImage>();
		m_BackButton.SetAction(OnBackClicked, m_BackButton);
		InitPinned();
		m_Title = base.transform.Find("TitleBar/Title").GetComponent<BaseText>();
		m_Pages = new List<AutopediaPage>();
		for (int i = 0; i < 4; i++)
		{
			Page page = (Page)i;
			string n = page.ToString();
			AutopediaPage component = base.transform.Find(n).GetComponent<AutopediaPage>();
			component.SetActive(Active: false);
			m_Pages.Add(component);
		}
		SetupSearch();
		SetupPageButtons();
		if (GameOptionsManager.Instance.m_Options.m_GameMode != 0)
		{
			m_Page = Page.Objects;
		}
		SetPage(m_Page);
	}

	private void Start()
	{
		for (int i = 0; i < 4; i++)
		{
			SetNew((Page)i, New: false);
		}
		SetNew(Page.Objects, m_ObjectsPageNew);
		SetNew(Page.Tips, m_TipsPageNew);
		m_ButtonList.m_Buttons[(int)m_Page].SetSelected(Selected: true);
		if (GameOptionsManager.Instance.m_Options.m_GameMode != 0)
		{
			foreach (BaseGadget button in m_ButtonList.m_Buttons)
			{
				button.SetActive(Active: false);
			}
			return;
		}
		Quest quest = QuestManager.Instance.GetQuest(Quest.ID.ResearchLevel1);
		if (quest != null && !quest.GetIsComplete())
		{
			m_ButtonList.m_Buttons[1].SetActive(Active: false);
		}
	}

	protected void SetupPageButtons()
	{
		m_ButtonList = base.transform.Find("TitleBar/ButtonList").GetComponent<ButtonList>();
		m_ButtonList.m_ObjectCount = 4;
		m_ButtonList.m_CreateObjectCallback = OnCreatePageButton;
		m_ButtonList.CreateButtons();
	}

	public void OnCreatePageButton(GameObject NewGadget, int Index)
	{
		StandardButtonImage component = NewGadget.GetComponent<StandardButtonImage>();
		Page page = (Page)Index;
		string sprite = "Autopedia/Autopedia" + page;
		component.SetSprite(sprite);
		page = (Page)Index;
		string rolloverFromID = "Autopedia" + page;
		component.SetRolloverFromID(rolloverFromID);
		component.SetAction(OnPageButtonClicked, component);
	}

	public void SetNew(Page NewPage, bool New)
	{
		NewThing component = m_ButtonList.m_Buttons[(int)NewPage].transform.Find("NewThing").GetComponent<NewThing>();
		component.UpdateWhilePaused();
		component.gameObject.SetActive(New);
	}

	public void ObjectsFirstTime()
	{
		m_ObjectsPageNew = true;
		SetNew(Page.Objects, New: true);
	}

	public void TipsFirstTime()
	{
		m_TipsPageNew = true;
		SetNew(Page.Tips, New: true);
	}

	public void OnBackClicked(BaseGadget NewGadget)
	{
		GameStateManager.Instance.PopState();
	}

	public void OnPageButtonClicked(BaseGadget NewGadget)
	{
		int num = m_ButtonList.m_Buttons.IndexOf(NewGadget);
		if (num == 2)
		{
			m_ObjectsPageNew = false;
		}
		if (num == 3)
		{
			m_TipsPageNew = false;
		}
		SetNew((Page)num, New: false);
		SetPage((Page)num);
		QuestManager.Instance.AddEvent(QuestEvent.Type.SelectAutopediaObjects, Bot: false, null, null);
	}

	public void CeremonyPlaying(bool Playing, Quest NewQuest)
	{
		m_CeremonyPlaying = Playing;
		m_BackButton.SetInteractable(!Playing);
		m_Pages[(int)m_Page].CeremonyPlaying(Playing, NewQuest);
	}

	public void SetPage(Page NewPage)
	{
		m_Pages[(int)m_Page].SetActive(Active: false);
		m_ButtonList.m_Buttons[(int)m_Page].SetSelected(Selected: false);
		if (m_Page == Page.Objects)
		{
			m_Search.SetActive(Active: false);
		}
		if (m_Page == Page.Research && Research.Instance.SelectedTechnologyActive())
		{
			Research.Instance.HideSelectedTechnology();
		}
		m_Page = NewPage;
		m_Pages[(int)m_Page].SetActive(Active: true);
		m_ButtonList.m_Buttons[(int)m_Page].SetSelected(Selected: true);
		if (m_Page == Page.Objects)
		{
			m_Search.SetActive(Active: true);
		}
		string newText = "Autopedia" + m_Page;
		m_Title.SetTextFromID(newText);
	}

	public void InitPinned()
	{
		m_Pinned = base.transform.Find("PinnedCeremony").gameObject;
		m_PinnedText = m_Pinned.transform.Find("Title").GetComponent<BaseText>();
		m_Pinned.SetActive(value: false);
		m_PinnedTimer = 0f;
	}

	public void StartPinned(Quest NewQuest)
	{
		m_PinnedTimer = 2f;
		m_Pinned.SetActive(value: true);
		m_PinnedText.SetTextFromID(NewQuest.m_Title);
	}

	public void UpdatePinned()
	{
		if (m_PinnedTimer > 0f)
		{
			m_PinnedTimer -= TimeManager.Instance.m_PauseDelta;
			if (m_PinnedTimer <= 0f)
			{
				m_PinnedTimer = 0f;
				m_Pinned.SetActive(value: false);
			}
		}
	}

	private List<BaseAutoCompletePossible> GetPossibleSearchObjects(string SearchString)
	{
		List<BaseAutoCompletePossible> list = new List<BaseAutoCompletePossible>();
		string text = SearchString.ToUpper().TrimStart(' ').TrimEnd(' ');
		if (text != "")
		{
			for (int i = 0; i < (int)ObjectTypeList.m_Total; i++)
			{
				ObjectType objectType = (ObjectType)i;
				string text2 = ObjectTypeList.Instance.GetHumanReadableNameFromIdentifier(objectType).ToUpper();
				if (text2 != "" && text2.Contains(text))
				{
					ObjectSubCategory subCategoryFromType = ObjectTypeList.Instance.GetSubCategoryFromType(objectType);
					if (subCategoryFromType != ObjectSubCategory.Hidden && subCategoryFromType != ObjectSubCategory.Any && ((subCategoryFromType != ObjectSubCategory.Prizes && subCategoryFromType != ObjectSubCategory.BuildingsPrizes) || QuestManager.Instance.GetQuestComplete(Quest.ID.GlueSpacePort)))
					{
						text2 = ObjectTypeList.Instance.GetHumanReadableNameFromIdentifier(objectType);
						BaseAutoCompletePossible item = new BaseAutoCompletePossible(text2, objectType);
						list.Add(item);
					}
				}
			}
		}
		return list;
	}

	private void SetupSearch()
	{
		m_Search = base.transform.Find("TitleBar/Search").GetComponent<BaseAutoComplete>();
		m_Search.SetObjectsCallback(GetPossibleSearchObjects);
		m_Search.SetActive(Active: false);
		m_Search.SetAction(OnSearchComplete, m_Search);
	}

	public void OnSearchComplete(BaseAutoComplete NewGadget)
	{
		ObjectType newType = (ObjectType)m_Search.GetCurrentObject();
		m_Pages[2].GetComponent<Objects>().SetObjectType(newType, Flash: true);
	}

	private void Update()
	{
		UpdatePinned();
	}

	public void EscPressed()
	{
		if (m_Page == Page.Research && Research.Instance.SelectedTechnologyActive())
		{
			Research.Instance.HideSelectedTechnology();
			return;
		}
		AudioManager.Instance.StartEvent("UIUnpause");
		GameStateManager.Instance.PopState();
	}
}
