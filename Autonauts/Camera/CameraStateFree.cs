using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraStateFree : CameraState
{
	private Vector3 m_FreeCameraPosition;

	private Quaternion m_FreeCameraRotation;

	private float m_LastMouseX;

	private float m_LastMouseY;

	private float m_FreeCamHeading;

	private float m_FreeCamPitch;

	private float m_FreePanSpeed;

	private float m_FreeFOV;

	private bool m_FirstUse;

	public CameraStateFree(Camera NewCamera)
		: base(NewCamera)
	{
		m_FirstUse = true;
	}

	public void ResetFirstUse()
	{
		m_FirstUse = true;
	}

	public override void StartUse()
	{
		if (m_FirstUse)
		{
			m_FirstUse = false;
			m_FreeCameraPosition = m_Camera.transform.position;
			m_FreeCameraRotation = m_Camera.transform.rotation;
			m_FreeCamHeading = m_Camera.transform.rotation.eulerAngles.y;
			m_FreeCamPitch = m_Camera.transform.rotation.eulerAngles.x;
			m_FreePanSpeed = 20f;
			m_FreeFOV = 60f;
		}
	}

	public override void EndUse()
	{
		m_Camera.fieldOfView = 60f;
	}

	public override void UpdateInput()
	{
		float num = m_FreePanSpeed * TimeManager.Instance.m_NormalDelta;
		if (Input.GetKey(KeyCode.LeftShift))
		{
			num *= 3f;
		}
		if (MyInputManager.m_Rewired.GetButton("PanUp"))
		{
			m_FreeCameraPosition.x += Mathf.Sin(m_FreeCamHeading * ((float)Math.PI / 180f)) * num;
			m_FreeCameraPosition.z += Mathf.Cos(m_FreeCamHeading * ((float)Math.PI / 180f)) * num;
		}
		if (MyInputManager.m_Rewired.GetButton("PanDown"))
		{
			m_FreeCameraPosition.x -= Mathf.Sin(m_FreeCamHeading * ((float)Math.PI / 180f)) * num;
			m_FreeCameraPosition.z -= Mathf.Cos(m_FreeCamHeading * ((float)Math.PI / 180f)) * num;
		}
		if (MyInputManager.m_Rewired.GetButton("PanLeft"))
		{
			m_FreeCameraPosition.x -= Mathf.Cos(m_FreeCamHeading * ((float)Math.PI / 180f)) * num;
			m_FreeCameraPosition.z += Mathf.Sin(m_FreeCamHeading * ((float)Math.PI / 180f)) * num;
		}
		if (MyInputManager.m_Rewired.GetButton("PanRight"))
		{
			m_FreeCameraPosition.x += Mathf.Cos(m_FreeCamHeading * ((float)Math.PI / 180f)) * num;
			m_FreeCameraPosition.z -= Mathf.Sin(m_FreeCamHeading * ((float)Math.PI / 180f)) * num;
		}
		if (MyInputManager.m_Rewired.GetButton("Edit"))
		{
			m_FreeCameraPosition.y -= num;
			if (m_FreeCameraPosition.y < 0.5f)
			{
				m_FreeCameraPosition.y = 0.5f;
			}
		}
		if (MyInputManager.m_Rewired.GetButton("InventoryStow"))
		{
			m_FreeCameraPosition.y += num;
		}
		if (MyInputManager.m_Rewired.GetButton("Recenter"))
		{
			List<BaseClass> players = CollectionManager.Instance.GetPlayers();
			CameraManager.Instance.m_CameraPosition.x = players[0].transform.position.x;
			CameraManager.Instance.m_CameraPosition.z = players[0].transform.position.z;
			m_Camera.transform.position = CameraManager.Instance.m_CameraPosition + CameraManager.Instance.m_CameraZoomedPosition;
			CameraManager.Instance.m_CameraFinalPosition = m_Camera.transform.position;
			m_Camera.transform.rotation = CameraManager.Instance.m_CameraRotation;
			m_FreeFOV = 60f;
			m_Camera.fieldOfView = m_FreeFOV;
			m_FreeCameraPosition = m_Camera.transform.position;
			m_FreeCameraRotation = m_Camera.transform.rotation;
			m_FreeCamHeading = m_Camera.transform.rotation.eulerAngles.y;
			m_FreeCamPitch = m_Camera.transform.rotation.eulerAngles.x;
			m_FreePanSpeed = 20f;
		}
		else if (!Input.GetMouseButton(2) && UnityEngine.Cursor.lockState == CursorLockMode.Locked)
		{
			float num2 = m_FreeFOV / 100f * 0.9f + 0.1f;
			float axis = MyInputManager.m_Rewired.GetAxis("MouseX");
			m_FreeCamHeading += axis * 3f * num2;
			float axis2 = MyInputManager.m_Rewired.GetAxis("MouseY");
			m_FreeCamPitch -= axis2 * 3f * num2;
			if (m_FreeCamPitch < -89f)
			{
				m_FreeCamPitch = -89f;
			}
			if (m_FreeCamPitch > 89f)
			{
				m_FreeCamPitch = 89f;
			}
			m_FreeCameraRotation = Quaternion.Euler(m_FreeCamPitch, m_FreeCamHeading, 0f);
		}
		if (MyInputManager.m_Rewired.GetButton("CameraSpeedDown"))
		{
			m_FreePanSpeed -= m_FreePanSpeed * 0.05f;
			if (m_FreePanSpeed < 10f)
			{
				m_FreePanSpeed = 10f;
			}
		}
		if (MyInputManager.m_Rewired.GetButton("CameraSpeedUp"))
		{
			m_FreePanSpeed += m_FreePanSpeed * 0.05f;
			if (m_FreePanSpeed > 200f)
			{
				m_FreePanSpeed = 200f;
			}
		}
		float axis3 = MyInputManager.m_Rewired.GetAxis("MouseScrollWheel");
		if (axis3 > 0f)
		{
			m_FreeFOV -= m_FreeFOV * 0.15f;
			if (m_FreeFOV < 1f)
			{
				m_FreeFOV = 1f;
			}
		}
		if (axis3 < 0f)
		{
			m_FreeFOV += m_FreeFOV * 0.15f;
			if (m_FreeFOV > 100f)
			{
				m_FreeFOV = 100f;
			}
		}
		m_Camera.fieldOfView = m_FreeFOV;
		m_FreeCameraPosition = CameraManager.Instance.CheckMapBounds(m_FreeCameraPosition);
		FinaliseCamera(m_FreeCameraPosition, m_FreeCameraRotation);
	}

	public override void UpdateCamera()
	{
	}
}
