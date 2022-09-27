using UnityEngine;

public class FarmerStateCelebrateMakeInHand : FarmerStateBase
{
	private Vector3 m_OldObjectPosition;

	private Quaternion m_OldObjectRotation;

	private Transform m_OldObjectParent;

	public override void StartState()
	{
		base.StartState();
		m_Farmer.m_FinalRotation = m_Farmer.transform.rotation;
		m_Farmer.transform.rotation = Quaternion.identity;
		Holdable topObject = m_Farmer.m_FarmerCarry.GetTopObject();
		m_OldObjectPosition = topObject.transform.localPosition;
		m_OldObjectRotation = topObject.transform.localRotation;
		m_OldObjectParent = topObject.transform.parent;
		topObject.transform.SetParent(m_Farmer.transform);
		topObject.transform.localPosition = new Vector3(0f, 3f, 0f);
		topObject.transform.localRotation = Quaternion.identity;
		AudioManager.Instance.StartEvent("UICeremonyMakeInHand");
	}

	public override void EndState()
	{
		base.EndState();
		m_Farmer.transform.rotation = m_Farmer.m_FinalRotation;
		Holdable topObject = m_Farmer.m_FarmerCarry.GetTopObject();
		topObject.transform.SetParent(m_OldObjectParent);
		topObject.transform.localPosition = m_OldObjectPosition;
		topObject.transform.localRotation = m_OldObjectRotation;
	}

	public override void UpdateState()
	{
		base.UpdateState();
		if (m_Farmer.m_StateTimer > 1f)
		{
			DoEndAction();
		}
	}
}
