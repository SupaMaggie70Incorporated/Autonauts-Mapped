using System.Collections.Generic;
using UnityEngine;

public class FarmerStateLearning : FarmerStateMove
{
	private bool m_RequestStopLearning;

	private WorkerLookAt m_LookAtEffect;

	public override void StartState()
	{
		base.StartState();
		m_SafetyDelay = 0f;
		m_RequestStopLearning = false;
		m_LookAtEffect = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.WorkerLookAt, m_Farmer.transform.position, Quaternion.identity).GetComponent<WorkerLookAt>();
		m_LookAtEffect.SetOwnerAndTarget(m_Farmer, m_Farmer.m_LearningTarget);
		m_LookAtEffect.SetActive(Active: false);
	}

	public override void EndState()
	{
		base.EndState();
		m_LookAtEffect.StopUsing();
		m_LookAtEffect = null;
	}

	public void RequestStopLearning()
	{
		if (!m_Farmer.m_Move)
		{
			m_Farmer.StopLearning();
		}
		else
		{
			m_RequestStopLearning = true;
		}
	}

	private void CheckMove()
	{
		if ((bool)m_Farmer.m_EngagedObject && Minecart.GetIsTypeMinecart(m_Farmer.m_EngagedObject.m_TypeIdentifier))
		{
			return;
		}
		GameStateBase currentState = GameStateManager.Instance.GetCurrentState();
		if (currentState.m_BaseState == GameStateManager.State.TeachWorker && currentState.GetComponent<GameStateTeachWorker2>().m_ScriptEdit.GetFreeMemory() <= 0)
		{
			return;
		}
		List<BaseClass> players = CollectionManager.Instance.GetPlayers();
		TileCoord tileCoord = players[0].GetComponent<FarmerPlayer>().m_TileCoord;
		if ((tileCoord - m_Farmer.m_TileCoord).Magnitude() > 5f)
		{
			TileCoord tileCoord2 = players[0].GetComponent<FarmerPlayer>().m_GoToTilePosition;
			if (players[0].GetComponent<Farmer>().m_State == Farmer.State.Engaged && (bool)players[0].GetComponent<Farmer>().m_EngagedObject)
			{
				Vehicle component = players[0].GetComponent<Farmer>().m_EngagedObject.GetComponent<Vehicle>();
				if ((bool)component)
				{
					tileCoord2 = component.m_TileCoord;
				}
			}
			if (m_Farmer.m_GoToTilePosition != tileCoord2)
			{
				m_Farmer.SendAction(new ActionInfo(ActionType.Stop, default(TileCoord)));
				m_Farmer.SendAction(new ActionInfo(ActionType.MoveToLessOne, tileCoord2));
				m_StartAnimation = true;
			}
		}
		if (!m_Farmer.GetIsDoingSomething())
		{
			m_Farmer.SendAction(new ActionInfo(ActionType.LookAt, tileCoord));
		}
	}

	private bool CheckStop()
	{
		if ((CollectionManager.Instance.GetPlayers()[0].GetComponent<FarmerPlayer>().m_TileCoord - m_Farmer.m_TileCoord).Magnitude() < 3f)
		{
			m_Farmer.SendAction(new ActionInfo(ActionType.Stop, default(TileCoord)));
			m_LookAtEffect.UpdateShape();
			return true;
		}
		return false;
	}

	public bool NextGoTo()
	{
		if (m_RequestStopLearning)
		{
			m_Farmer.SendAction(new ActionInfo(ActionType.Stop, default(TileCoord)));
			m_Farmer.StopLearning();
			return false;
		}
		if (CheckStop())
		{
			return false;
		}
		CheckMove();
		return true;
	}

	public override void UpdateState()
	{
		bool move = m_Farmer.m_Move;
		if ((bool)m_Farmer.m_EngagedObject && Vehicle.GetIsTypeVehicle(m_Farmer.m_EngagedObject.m_TypeIdentifier))
		{
			move = m_Farmer.m_EngagedObject.GetComponent<Vehicle>().m_Move;
		}
		if (!move)
		{
			m_LookAtEffect.SetActive(Active: true);
			if (m_RequestStopLearning)
			{
				m_Farmer.StopLearning();
			}
			else
			{
				CheckMove();
			}
		}
		else
		{
			m_LookAtEffect.SetActive(Active: false);
			if (m_Farmer.m_EngagedObject == null)
			{
				base.UpdateState();
			}
		}
	}
}
