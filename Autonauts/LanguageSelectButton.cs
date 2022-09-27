using UnityEngine;
using UnityEngine.UI;

public class LanguageSelectButton : MonoBehaviour
{
	private void Awake()
	{
		SettingsManager.Language language = SettingsManager.Instance.m_Language;
		string text = "Flag" + language;
		Sprite sprite = (Sprite)Resources.Load("Textures/Hud/CountryFlags/" + text, typeof(Sprite));
		base.transform.Find("Button").GetComponent<Image>().sprite = sprite;
	}

	public void OnClick()
	{
		GameStateManager.Instance.PushState(GameStateManager.State.LanguageSelect);
	}
}
