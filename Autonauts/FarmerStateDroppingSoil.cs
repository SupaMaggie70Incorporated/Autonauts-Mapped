using UnityEngine;

public class FarmerStateDroppingSoil : FarmerStateTool
{
	private bool m_UsingStick;

	public FarmerStateDroppingSoil()
	{
		m_ActionSoundName = "ToolShovelDig";
		m_NoToolIconName = "GenIcons/GenIconToolBucketMetal";
		m_AdjacentTile = true;
	}

	public override bool IsToolAcceptable(Holdable NewObject)
	{
		if (NewObject == null)
		{
			return false;
		}
		return NewObject.GetComponent<ToolBucket>();
	}

	public override void StartState()
	{
		base.StartState();
		GetFinalRotationTile();
		StandInTargetTile();
	}

	public override void EndState()
	{
		base.EndState();
		StandInTargetTile();
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
		if ((int)(m_Farmer.m_StateTimer * 60f) % 12 < 6)
		{
			num = 1f;
		}
		if (num != m_OldActionPercent)
		{
			m_OldActionPercent = num;
			if (num == 1f)
			{
				m_Farmer.transform.position = m_Farmer.m_FinalPosition + new Vector3(0f, 2f, 0f);
				DoAction(targetObject);
			}
			else
			{
				m_Farmer.transform.position = m_Farmer.m_FinalPosition;
				CheckActionDone(targetObject);
			}
		}
	}
}
