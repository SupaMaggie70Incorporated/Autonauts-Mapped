public class GameOptionsPreview : GameOptionsManager
{
	protected void Awake()
	{
		m_Options.m_MapTileData = null;
		m_Options.SetDefaults();
	}
}
