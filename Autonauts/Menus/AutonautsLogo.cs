public class AutonautsLogo : BaseImage
{
	public bool m_Small;

	protected new void Awake()
	{
		base.Awake();
		string text = "AutonautsLogo";
		if (m_Small)
		{
			text += "2";
		}
		if (SettingsManager.Instance.m_Language == SettingsManager.Language.ChineseSimplified)
		{
			text += "CN";
		}
		SetSprite(text);
	}
}
