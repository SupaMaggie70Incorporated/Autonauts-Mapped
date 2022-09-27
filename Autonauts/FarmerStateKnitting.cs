using UnityEngine;

public class FarmerStateKnitting : FarmerStateBase
{
	private RockingChair m_RockingChair;

	public static bool GetIsToolAcceptable(ObjectType NewType)
	{
		return true;
	}

	public override bool IsToolAcceptable(Holdable NewObject)
	{
		if (NewObject == null)
		{
			return false;
		}
		return GetIsToolAcceptable(NewObject.m_TypeIdentifier);
	}

	public override void StartState()
	{
		base.StartState();
		m_RockingChair = null;
		if ((bool)m_Farmer.m_FarmerAction.m_CurrentObject)
		{
			m_RockingChair = m_Farmer.m_FarmerAction.m_CurrentObject.GetComponent<RockingChair>();
			m_RockingChair.StartConversion(m_Farmer);
		}
		m_FinalPosition = m_Farmer.transform.position;
		m_FinalRotation = m_Farmer.transform.rotation;
	}

	public override void EndState()
	{
		base.EndState();
		m_Farmer.transform.position = m_FinalPosition;
		m_Farmer.transform.rotation = m_FinalRotation;
	}

	public override void UpdateState()
	{
		base.UpdateState();
		if ((bool)m_RockingChair)
		{
			m_Farmer.transform.position = m_RockingChair.m_ModelRoot.transform.TransformPoint(new Vector3(0f, 1f, 0f));
			m_Farmer.transform.rotation = m_RockingChair.m_ModelRoot.transform.rotation;
		}
		if (!m_RockingChair || m_RockingChair.m_State != Converter.State.Converting)
		{
			DoEndAction();
		}
	}
}
