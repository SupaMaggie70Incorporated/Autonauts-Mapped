using UnityEngine;

public class CameraState
{
	protected Camera m_Camera;

	public CameraState(Camera NewCamera)
	{
		m_Camera = NewCamera;
	}

	protected void FinaliseCamera(Vector3 Position, Quaternion Rotation)
	{
		CameraManager.Instance.FinaliseCamera(Position, Rotation);
	}

	public virtual void StartUse()
	{
	}

	public virtual void EndUse()
	{
	}

	public virtual void UpdateInput()
	{
	}

	public virtual void UpdateCamera()
	{
	}

	public virtual void LateUpdate()
	{
	}
}
