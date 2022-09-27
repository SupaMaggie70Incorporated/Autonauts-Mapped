using System;
using UnityEngine;

public class Wheat : Holdable
{
	private Vector3 m_StartJumpPosition;

	private float m_JumpTimer;

	public override void Restart()
	{
		base.Restart();
		m_JumpTimer = 0f;
		base.enabled = false;
	}

	private void UseFlail(AFO Info)
	{
		m_StartJumpPosition = base.transform.localPosition;
		m_JumpTimer = 0.05f;
		base.enabled = true;
	}

	private void EndFlail(AFO Info)
	{
		StopUsing();
		BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.WheatSeed, base.transform.position, base.transform.localRotation);
		SpawnAnimationManager.Instance.AddJump(baseClass, m_TileCoord, m_TileCoord, 0f, baseClass.transform.position.y, 3f);
		BaseClass newObject = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Straw, base.transform.position, base.transform.localRotation);
		SpawnAnimationManager.Instance.AddJump(newObject, m_TileCoord, m_TileCoord, 0f, baseClass.transform.position.y, 3f);
		AudioManager.Instance.StartEvent("ObjectCreated", this);
		QuestManager.Instance.AddEvent(QuestEvent.Type.Make, Info.m_Actioner.m_TypeIdentifier == ObjectType.Worker, ObjectType.WheatSeed, this);
	}

	private ActionType GetActionFromFlail(AFO Info)
	{
		Info.m_UseAction = UseFlail;
		Info.m_EndAction = EndFlail;
		Info.m_FarmerState = Farmer.State.Flail;
		if (Info.m_ActionType == AFO.AT.Primary)
		{
			return ActionType.UseInHands;
		}
		return ActionType.Total;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (FarmerStateFlail.GetIsToolAcceptable(Info.m_ObjectType))
		{
			return GetActionFromFlail(Info);
		}
		return base.GetActionFromObject(Info);
	}

	private void Update()
	{
		if (m_JumpTimer > 0f)
		{
			m_JumpTimer -= TimeManager.Instance.m_NormalDelta;
			float num = 1f - m_JumpTimer / 0.05f;
			if (num >= 1f)
			{
				base.enabled = false;
				base.transform.localPosition = m_StartJumpPosition;
			}
			else
			{
				base.transform.localPosition = m_StartJumpPosition + new Vector3(0f, Mathf.Sin(num * (float)Math.PI) * 0.5f, 0f);
			}
		}
		else
		{
			base.enabled = false;
		}
	}
}
