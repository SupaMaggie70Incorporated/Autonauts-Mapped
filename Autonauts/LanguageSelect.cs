using UnityEngine;

public class LanguageSelect : BaseMenu
{
	private BasePanelOptions m_Panel;

	private ButtonList m_ButtonList;

	protected new void Awake()
	{
		base.Awake();
		m_Panel = base.transform.Find("Panel").GetComponent<BasePanelOptions>();
		m_ButtonList = m_Panel.transform.Find("LanguageButtons").GetComponent<ButtonList>();
		m_ButtonList.m_CreateObjectCallback = OnCreateObject;
		m_ButtonList.m_ObjectCount = 11;
		if (SettingsManager.Instance.m_Language == SettingsManager.Language.Total)
		{
			m_ButtonList.transform.localPosition = default(Vector3);
			m_Panel.GetBackButton().SetActive(Active: false);
			m_Panel.GetComponent<RectTransform>().sizeDelta = m_Panel.GetComponent<RectTransform>().sizeDelta + new Vector2(0f, -40f);
		}
	}

	public void OnCreateObject(GameObject NewObject, int Index)
	{
		SettingsManager.Language language = (SettingsManager.Language)Index;
		string text = language.ToString();
		string sprite = "CountryFlags/Flag" + text;
		BaseButtonImage component = NewObject.GetComponent<BaseButtonImage>();
		component.SetSprite(sprite);
		NewObject.GetComponent<BaseButtonImage>().SetRolloverFromID("Language" + text);
		AddAction(component, OnLanguage);
	}

	public void OnLanguage(BaseGadget NewGadget)
	{
		SettingsManager.Language language = SettingsManager.Language.English;
		foreach (BaseGadget button in m_ButtonList.m_Buttons)
		{
			if (button.GetComponent<BaseButton>() == m_NewButtonClicked)
			{
				SettingsManager.Instance.SetLanguage(language);
				SettingsManager.Instance.Save();
				TextManager.Instance.Load();
				WorkerStatusIndicator.Clear();
				SessionManager.Instance.LoadLevel(LoadLevel: false, "MainMenu");
				GameStateManager.Instance.SetState(GameStateManager.State.SceneChange);
				break;
			}
			language++;
		}
	}
}
