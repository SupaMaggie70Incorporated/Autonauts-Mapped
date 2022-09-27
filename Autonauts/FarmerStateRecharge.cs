using UnityEngine;

public class FarmerStateRecharge : FarmerStateBase
{
	private PlaySound m_Sound;

	public override bool GetIsAdjacentTile(TileCoord TargetTile, BaseClass Object)
	{
		return true;
	}

	public override void StartState()
	{
		base.StartState();
		m_Farmer.m_FinalPosition = m_Farmer.transform.position;
		Vector3 position = m_Farmer.m_FarmerAction.m_CurrentInfo.m_Object.transform.position;
		Vector3 vector = position - m_Farmer.transform.position;
		float num = 0f - Mathf.Atan2(vector.z, vector.x);
		m_ActionDelta.x = 0f - Mathf.Cos(num);
		m_ActionDelta.y = 0f;
		m_ActionDelta.z = Mathf.Sin(num);
		position += m_ActionDelta * 2f;
		m_Farmer.transform.position = position;
		m_Farmer.m_FinalRotation = Quaternion.Euler(0f, num * 57.29578f - 90f, 0f);
		m_Sound = AudioManager.Instance.StartEvent("WorkerWinding", m_Farmer, Remember: true);
	}

	public override void EndState()
	{
		base.EndState();
		m_Farmer.transform.position = m_Farmer.m_FinalPosition;
		m_Farmer.transform.rotation = m_Farmer.m_FinalRotation;
		QuestManager.Instance.AddEvent(QuestEvent.Type.RechargeBot, m_Farmer.m_TypeIdentifier == ObjectType.Worker, 0, m_Farmer);
		if (m_Sound != null)
		{
			AudioManager.Instance.StopEvent(m_Sound);
		}
	}

	public override void StartWorking()
	{
		if (m_Sound == null)
		{
			m_Sound = AudioManager.Instance.StartEvent("WorkerWinding", m_Farmer, Remember: true);
		}
	}

	public override void StopWorking()
	{
		if (m_Sound != null)
		{
			AudioManager.Instance.StopEvent(m_Sound);
			m_Sound = null;
		}
	}

	public override void UpdateState()
	{
		base.UpdateState();
		float num = 0f;
		if ((int)(m_Farmer.m_StateTimer * 60f) % 8 < 4)
		{
			num = 1f;
		}
		m_Farmer.transform.rotation = m_Farmer.m_FinalRotation * Quaternion.Euler(0f, 35f * (1f - num), 0f);
		float num2 = 1f;
		if (m_Farmer.m_TypeIdentifier == ObjectType.Worker)
		{
			num2 = m_Farmer.GetComponent<Worker>().m_DriveInfo.m_RechargeDelay;
		}
		if (m_Farmer.m_StateTimer > num2)
		{
			m_Farmer.m_FarmerAction.DoRecharge();
			DoEndAction();
		}
	}
}
