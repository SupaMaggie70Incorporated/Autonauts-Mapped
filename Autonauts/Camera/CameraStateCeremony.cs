using UnityEngine;

public class CameraStateCeremony : CameraState
{
	private GameObject m_TrackObject;

	public CameraStateCeremony(Camera NewCamera)
		: base(NewCamera)
	{
	}

	public void SetTrackObject(GameObject NewObject)
	{
		m_TrackObject = NewObject;
	}

	public override void StartUse()
	{
	}

	public override void EndUse()
	{
	}

	public override void UpdateInput()
	{
	}

	public override void LateUpdate()
	{
		if ((bool)m_TrackObject)
		{
			CameraManager.Instance.m_CameraPosition = m_TrackObject.transform.position;
			m_Camera.transform.position = CameraManager.Instance.m_CameraPosition + CameraManager.Instance.m_CameraZoomedPosition;
			m_Camera.transform.rotation = CameraManager.Instance.m_CameraRotation;
		}
	}
}
