public class RecordingAction
{
	public enum Action
	{
		Create,
		Move,
		Special,
		Destroy,
		ShowPlot,
		ChangeTile,
		Total
	}

	public Action m_Action;

	public RecordingStamp m_Stamp;

	public int m_UID;

	public ObjectType m_Type;

	public int m_Index;

	public int m_X;

	public int m_Y;

	public int m_Rotation;

	public int m_OldX;

	public int m_OldY;

	public int m_OldRotation;

	public object m_SpecialData;

	public object m_OldSpecialData;
}
