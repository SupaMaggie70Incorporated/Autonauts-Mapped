using UnityEngine;

public class ProcessModelInfo
{
	public ObjectType m_Type;

	public GameObject m_Model;

	public bool m_RandomVariants;

	public bool m_ForceBuilding;

	public ProcessModelInfo(ObjectType NewType, bool RandomVariants, bool ForceBuilding)
	{
		m_Type = NewType;
		m_RandomVariants = RandomVariants;
		m_ForceBuilding = ForceBuilding;
	}
}
