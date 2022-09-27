using UnityEngine;

public class GameStatePlayCameraSequence : GameStateBase
{
	private Quaternion m_OldRotation;

	private Vector3 m_OldPosition;

	protected new void Awake()
	{
		base.Awake();
		m_OldRotation = CameraManager.Instance.m_CameraRotation;
		m_OldPosition = CameraManager.Instance.m_CameraPosition;
		CameraManager.Instance.SetState(CameraManager.State.PlaySequence);
	}

	protected new void OnDestroy()
	{
		base.OnDestroy();
		CameraManager.Instance.SetState(CameraManager.State.Free);
	}

	public override void UpdateState()
	{
		if (!CameraManager.Instance.m_PlayingSpline)
		{
			GameStateManager.Instance.PopState();
		}
		if (MyInputManager.m_Rewired.GetButtonDown("ToggleFreeCam") || MyInputManager.m_Rewired.GetButtonDown("Quit"))
		{
			if (MyInputManager.m_Rewired.GetButtonDown("ToggleFreeCam"))
			{
				AudioManager.Instance.StartEvent("UIOptionSelected");
			}
			GameStateManager.Instance.PopState();
		}
	}
}
