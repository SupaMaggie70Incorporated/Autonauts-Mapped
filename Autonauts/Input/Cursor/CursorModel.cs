public class CursorModel
{
	public BaseClass m_Model;

	public TileCoord m_Position;

	public int m_Rotation;

	public CursorModel(BaseClass Model, TileCoord Position, int Rotation)
	{
		m_Model = Model;
		m_Position = Position;
		m_Rotation = Rotation;
	}
}
