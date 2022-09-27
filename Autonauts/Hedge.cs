using System;
using UnityEngine;

public class Hedge : Flora
{
	private Wobbler m_Wobbler;

	public override void RegisterClass()
	{
		base.RegisterClass();
		ObjectTypeList.Instance.SetTypeSleep(ObjectType.Hedge);
	}

	public override void Restart()
	{
		base.Restart();
		m_Wobbler.Restart();
		m_ModelRoot.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
	}

	protected new void Awake()
	{
		base.Awake();
		m_Wobbler = new Wobbler();
	}

	public override void SendAction(ActionInfo Info)
	{
		ActionType action = Info.m_Action;
		if (action == ActionType.Bump)
		{
			m_Wobbler.Go(0.5f, 5f, 0.5f);
			Wake();
		}
		base.SendAction(Info);
	}

	private void UseShovel(AFO Info)
	{
		m_Wobbler.Go(0.5f, 5f, 0.5f);
		Wake();
	}

	private void EndShovel(AFO Info)
	{
		StopUsing();
	}

	private ActionType GetActionFromShovel(AFO Info)
	{
		Info.m_UseAction = UseShovel;
		Info.m_EndAction = EndShovel;
		Info.m_FarmerState = Farmer.State.Shovel;
		return ActionType.UseInHands;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		ObjectType objectType = Info.m_ObjectType;
		if (Info.m_ActionType == AFO.AT.Primary && FarmerStateShovel.GetIsToolAcceptable(objectType))
		{
			return GetActionFromShovel(Info);
		}
		return base.GetActionFromObject(Info);
	}

	private void UpdateWobbler()
	{
		m_Wobbler.Update();
		float x = Mathf.Sin(m_Wobbler.m_Height * (float)Math.PI * 2f) * 10f;
		m_ModelRoot.transform.localRotation = Quaternion.Euler(x, 0f, 0f);
	}

	private void Update()
	{
		UpdateWobbler();
		if (m_Wobbler.m_Height == 0f)
		{
			Sleep();
		}
	}

	protected override void UpdateWorldCreated()
	{
		float scale = 0.95f + UnityEngine.Random.Range(0f, 0.1f);
		SetScale(scale);
	}
}
