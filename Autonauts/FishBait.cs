using UnityEngine;

public class FishBait : Holdable
{
	private enum State
	{
		OutOfWaterWait,
		OutOfWaterWriggle,
		Total
	}

	private State m_State;

	private float m_StateTimer;

	private float m_WriggleTimer;

	private Transform m_TailHinge;

	public override void Restart()
	{
		base.Restart();
		float num = Random.Range(0.5f, 1.5f);
		base.transform.localScale = new Vector3(num, num, num);
		SetState(State.OutOfWaterWait);
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_TailHinge = m_ModelRoot.transform.Find("TailHinge");
	}

	private void EndRod(AFO Info)
	{
		StopUsing();
	}

	private ActionType GetActionFromRod(AFO Info)
	{
		Info.m_EndAction = EndRod;
		Info.m_FarmerState = Farmer.State.PickingUp;
		if ((bool)Info.m_Object && ToolFillable.GetIsTypeFillable(Info.m_Object.m_TypeIdentifier) && Info.m_Object.GetComponent<ToolFillable>().GetIsFull())
		{
			return ActionType.Fail;
		}
		return ActionType.UseInHands;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		ObjectType objectType = Info.m_ObjectType;
		if (Info.m_ActionType == AFO.AT.Primary && FarmerStateFishing.GetIsToolAcceptable(objectType))
		{
			return GetActionFromRod(Info);
		}
		return base.GetActionFromObject(Info);
	}

	private void SetState(State NewState)
	{
		m_State = NewState;
		m_StateTimer = 0f;
		if (m_State == State.OutOfWaterWait)
		{
			m_StateTimer = 0f - Random.Range(1f, 2f);
		}
	}

	private void Update()
	{
		switch (m_State)
		{
		case State.OutOfWaterWait:
			if (m_StateTimer >= 0f)
			{
				SetState(State.OutOfWaterWriggle);
			}
			break;
		case State.OutOfWaterWriggle:
			if (m_StateTimer >= 0.25f)
			{
				m_ModelRoot.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
				if ((bool)m_TailHinge)
				{
					m_TailHinge.localRotation = Quaternion.Euler(0f, 0f, 0f) * ObjectUtils.m_ModelRotator;
				}
				SetState(State.OutOfWaterWait);
			}
			else if ((int)(m_StateTimer * 60f) % 10 < 5)
			{
				m_ModelRoot.transform.localRotation = Quaternion.Euler(0f, 10f, 0f);
				if ((bool)m_TailHinge)
				{
					m_TailHinge.localRotation = Quaternion.Euler(0f, -30f, 0f) * ObjectUtils.m_ModelRotator;
				}
			}
			else
			{
				m_ModelRoot.transform.localRotation = Quaternion.Euler(0f, -10f, 0f);
				if ((bool)m_TailHinge)
				{
					m_TailHinge.localRotation = Quaternion.Euler(0f, 30f, 0f) * ObjectUtils.m_ModelRotator;
				}
			}
			break;
		}
		m_StateTimer += TimeManager.Instance.m_NormalDelta;
	}
}
