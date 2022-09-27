using DarkTonic.MasterAudio;

public class PlaySound
{
	public PlaySoundResult m_Result;

	public string m_EventName;

	public TileCoordObject m_Object;

	public float m_MaxRange;

	public PlaySound(string EventName, TileCoordObject NewObject, float MaxRange)
	{
		m_EventName = EventName;
		m_Object = NewObject;
		m_MaxRange = MaxRange;
	}

	public PlaySound(PlaySoundResult Result)
	{
		m_Result = Result;
	}
}
