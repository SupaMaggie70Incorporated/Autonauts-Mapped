using UnityEngine;

public class CameraStateNormal : CameraState
{
	public CameraStateNormal(Camera NewCamera)
		: base(NewCamera)
	{
	}

	public override void StartUse()
	{
	}

	public override void EndUse()
	{
	}

	public override void UpdateInput()
	{
		CameraManager.Instance.UpdateKeyboardPan();
		CameraManager.Instance.UpdateMousePan(AllowPan: true);
		CameraManager.Instance.UpdateMouseWheel();
		FinaliseCamera(CameraManager.Instance.m_CameraPosition + CameraManager.Instance.m_CameraZoomedPosition, CameraManager.Instance.m_CameraRotation);
	}

	public override void UpdateCamera()
	{
		FinaliseCamera(CameraManager.Instance.m_CameraPosition + CameraManager.Instance.m_CameraZoomedPosition, CameraManager.Instance.m_CameraRotation);
	}
}
