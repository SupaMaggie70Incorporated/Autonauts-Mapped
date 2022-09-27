using UnityEngine;

public class TrailData
{
	public Vector3 m_Start;

	public Vector3 m_End;

	public float m_Time;

	public TrailData(Vector3 Start, Vector3 End, float Time)
	{
		m_Start = Start;
		m_End = End;
		m_Time = Time;
	}
}
