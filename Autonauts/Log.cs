using System;
using UnityEngine;

public class Log : Holdable
{
	public enum State
	{
		Waiting,
		ChopJump,
		Total
	}

	[HideInInspector]
	public State m_State;

	private float m_StateTimer;

	private Vector3 m_StartChopPosition;

	public override void Restart()
	{
		base.Restart();
		SetState(State.Waiting);
		base.enabled = false;
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		base.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
	}

	public override bool CanDoAction(ActionInfo Info, bool RightNow)
	{
		ActionType action = Info.m_Action;
		if (action == ActionType.BeingHeld && m_State == State.ChopJump)
		{
			return false;
		}
		return base.CanDoAction(Info, RightNow);
	}

	private void UseAxe(AFO Info)
	{
		m_StartChopPosition = base.transform.localPosition;
		SetState(State.ChopJump);
		base.enabled = true;
	}

	private void EndAxe(AFO Info)
	{
		for (int i = 0; i < 2; i++)
		{
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Plank, base.transform.position, base.transform.localRotation);
			SpawnAnimationManager.Instance.AddJump(baseClass, m_TileCoord, m_TileCoord, 0f, baseClass.transform.position.y, 3f);
			QuestManager.Instance.AddEvent(QuestEvent.Type.Make, Info.m_Actioner.m_TypeIdentifier == ObjectType.Worker, ObjectType.Plank, Info.m_Actioner);
		}
		AudioManager.Instance.StartEvent("ObjectCreated", this);
		StopUsing();
	}

	private ActionType GetActionFromAxe(AFO Info)
	{
		Info.m_UseAction = UseAxe;
		Info.m_EndAction = EndAxe;
		Info.m_FarmerState = Farmer.State.Chopping;
		if (m_State != 0)
		{
			return ActionType.Fail;
		}
		return ActionType.UseInHands;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		ObjectType objectType = Info.m_ObjectType;
		if (Info.m_ActionType == AFO.AT.Primary && FarmerStateChopping.GetIsToolAcceptable(objectType))
		{
			return GetActionFromAxe(Info);
		}
		return base.GetActionFromObject(Info);
	}

	private void SetState(State NewState)
	{
		m_State = NewState;
		m_StateTimer = 0f;
	}

	private void UpdateChopJump()
	{
		float num = m_StateTimer / 0.05f;
		if (num >= 1f)
		{
			base.enabled = false;
			m_State = State.Waiting;
			base.transform.localPosition = m_StartChopPosition;
		}
		else
		{
			base.transform.localPosition = m_StartChopPosition + new Vector3(0f, Mathf.Sin(num * (float)Math.PI) * 0.5f, 0f);
		}
	}

	private void Update()
	{
		State state = m_State;
		if (state == State.ChopJump)
		{
			UpdateChopJump();
		}
		m_StateTimer += TimeManager.Instance.m_NormalDelta;
	}
}
