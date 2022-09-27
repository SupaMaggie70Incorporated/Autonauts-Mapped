using UnityEngine;

public class FarmerStateSweeping : FarmerStateTool
{
	public FarmerStateSweeping()
	{
		m_ActionSoundName = "";
		m_NoToolIconName = "GenIcons/GenIconToolBroom";
		m_AdjacentTile = true;
	}

	public static bool GetIsToolAcceptable(ObjectType NewType)
	{
		return NewType == ObjectType.ToolBroom;
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
		GetFinalRotationTile();
		StandInTargetTile();
		m_NumActionCounts = 5;
	}

	public override void EndState()
	{
		base.EndState();
		StandInTargetTile();
		m_Farmer.m_FarmerCarry.m_ToolModel.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		m_Farmer.m_FarmerCarry.m_ToolModel.transform.localPosition = new Vector3(0f, 0f, 0f);
	}

	protected override void ActionSuccess(Actionable TargetObject)
	{
		base.ActionSuccess(TargetObject);
		if (TargetObject != null)
		{
			TargetObject.StopUsing();
		}
	}

	protected override void SendEvents(Actionable TargetObject)
	{
	}

	public override void UpdateState()
	{
		base.UpdateState();
		Actionable targetObject = GetTargetObject();
		if (targetObject == null)
		{
			DoEndAction();
			return;
		}
		float num = 0f;
		if ((int)(m_Farmer.m_StateTimer * 60f) % 15 < 8)
		{
			num = 1f;
		}
		m_Farmer.transform.position = m_Farmer.m_FinalPosition + new Vector3(0f, num * 0.5f, 0f);
		m_Farmer.m_FarmerCarry.m_ToolModel.transform.localPosition = new Vector3(1.5f - num * 1f, 0f, -1f);
		m_Farmer.m_FarmerCarry.m_ToolModel.transform.localRotation = Quaternion.Euler(0f, 0f, 25f + num * -50f);
		if (num != m_OldActionPercent)
		{
			m_OldActionPercent = num;
			if (num == 1f)
			{
				AudioManager.Instance.StartEvent("ToolSweepUse", m_Farmer);
				ParticlesManager.Instance.CreateParticles("SweepDust", m_Farmer.transform.TransformPoint(new Vector3(0f, 1f, -1f)), m_Farmer.transform.rotation * Quaternion.Euler(90f, 0f, 0f), AutoDestroy: true);
				DoAction(targetObject);
			}
			else
			{
				AudioManager.Instance.StartEvent("ToolSweepUse", m_Farmer);
				ParticlesManager.Instance.CreateParticles("SweepDust", m_Farmer.transform.TransformPoint(new Vector3(0f, 1f, -1f)), m_Farmer.transform.rotation * Quaternion.Euler(90f, 180f, 0f), AutoDestroy: true);
				CheckActionDone(targetObject);
			}
		}
	}
}
