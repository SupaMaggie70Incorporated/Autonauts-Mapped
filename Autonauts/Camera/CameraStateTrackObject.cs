using UnityEngine;

public class CameraStateTrackObject : CameraState
{
	private GameObject m_TrackObject;

	public CameraStateTrackObject(Camera NewCamera)
		: base(NewCamera)
	{
	}

	private void UpdateFollowWorkerPosition()
	{
	}

	public void SetTrackObject(GameObject NewObject)
	{
		m_TrackObject = NewObject;
		if ((bool)m_TrackObject)
		{
			UpdateFollowWorkerPosition();
		}
	}

	public override void StartUse()
	{
		if ((bool)m_TrackObject)
		{
			UpdateFollowWorkerPosition();
		}
	}

	public override void EndUse()
	{
		m_TrackObject = null;
	}

	public override void UpdateInput()
	{
		CameraManager.Instance.UpdateMouseWheel();
		CameraManager.Instance.UpdateMousePan(AllowPan: false);
		CameraManager.Instance.UpdateRotateReset();
		FinaliseCamera(CameraManager.Instance.m_CameraPosition + CameraManager.Instance.m_CameraZoomedPosition, CameraManager.Instance.m_CameraRotation);
	}

	public override void UpdateCamera()
	{
		UpdateFollowWorkerPosition();
		CameraManager.Instance.m_CameraPosition = m_TrackObject.transform.position;
		CameraManager.Instance.m_CameraPosition.y = 0f;
		m_Camera.transform.position = CameraManager.Instance.m_CameraPosition + CameraManager.Instance.m_CameraZoomedPosition;
		m_Camera.transform.rotation = CameraManager.Instance.m_CameraRotation;
		CameraManager.Instance.m_CameraFinalPosition = m_Camera.transform.position;
	}

	public override void LateUpdate()
	{
		CameraManager.Instance.m_CameraPosition = m_TrackObject.transform.position;
		CameraManager.Instance.m_CameraPosition.y = 0f;
		m_Camera.transform.position = CameraManager.Instance.m_CameraPosition + CameraManager.Instance.m_CameraZoomedPosition;
		m_Camera.transform.rotation = CameraManager.Instance.m_CameraRotation;
		CameraManager.Instance.m_CameraFinalPosition = m_Camera.transform.position;
	}
}
