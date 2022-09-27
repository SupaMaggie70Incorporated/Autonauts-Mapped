using UnityEngine;

public class TutorialPointerInfo
{
	public bool m_Active;

	public TutorialPointer.TargetType m_TargetType;

	public Vector2 m_Anchor;

	public Vector2 m_Pivot;

	public Vector2 m_Offset;

	public TutorialPointerInfo(bool Active, Vector2 Anchor, Vector2 Pivot, Vector2 Offset, TutorialPointer.TargetType NewTargetType = TutorialPointer.TargetType.Total)
	{
		m_Active = Active;
		m_Anchor = Anchor;
		m_Pivot = Pivot;
		m_Offset = Offset;
		m_TargetType = NewTargetType;
	}
}
