using System.Collections.Generic;
using UnityEngine;

public class CeremonyBase : MonoBehaviour
{
	public Quest m_Quest;

	public List<ObjectType> m_UnlockedObjects;

	protected Vector3 m_CameraPosition;

	protected float m_CameraDistance;

	private bool m_PannedTo;

	public virtual void SetQuest(Quest NewQuest, List<ObjectType> UnlockedObjects)
	{
		m_Quest = NewQuest;
		m_UnlockedObjects = new List<ObjectType>();
		if (UnlockedObjects == null)
		{
			return;
		}
		foreach (ObjectType UnlockedObject in UnlockedObjects)
		{
			m_UnlockedObjects.Add(UnlockedObject);
		}
	}

	public virtual void Skip()
	{
		if (m_PannedTo)
		{
			CameraManager.Instance.Focus(m_CameraPosition);
			CameraManager.Instance.SetDistance(m_CameraDistance);
		}
	}

	protected void PanTo(BaseClass Object, Vector3 ExtraOffset, float Distance, float Time)
	{
		m_PannedTo = true;
		m_CameraPosition = CameraManager.Instance.m_CameraPosition;
		m_CameraDistance = CameraManager.Instance.m_Distance;
		CameraManager.Instance.PanTo(Object, ExtraOffset, Distance, Time);
	}

	protected void ReturnPanTo(float Time)
	{
		if (m_PannedTo)
		{
			m_PannedTo = false;
			CameraManager.Instance.PanTo(m_CameraPosition, m_CameraDistance, Time);
		}
	}
}
