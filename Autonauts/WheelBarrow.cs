using UnityEngine;

public class WheelBarrow : Cart
{
	public override void Restart()
	{
		base.Restart();
		m_ModelRoot.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
	}

	public override void SendAction(ActionInfo Info)
	{
		base.SendAction(Info);
		switch (Info.m_Action)
		{
		case ActionType.Engaged:
			m_ModelRoot.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			break;
		case ActionType.Disengaged:
			m_ModelRoot.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			break;
		}
	}
}
