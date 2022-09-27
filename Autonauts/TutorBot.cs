using System.Collections.Generic;
using UnityEngine;

public class TutorBot : GoTo
{
	public enum State
	{
		RocketLand,
		Idle,
		MovingWaiting,
		Moving,
		Talking,
		Flap,
		TutorialFinished,
		Away,
		Total
	}

	public static TutorBot Instance;

	private State m_State;

	private float m_StateTimer;

	private Light m_FrontLight;

	private GameObject m_LeftWing;

	private GameObject m_RightWing;

	private float m_Speed;

	public override void Restart()
	{
		Instance = this;
		base.Restart();
		SetState(State.Idle);
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		base.StopUsing(AndDestroy);
		Instance = null;
	}

	public override bool CanDoAction(ActionInfo Info, bool RightNow)
	{
		ActionType action = Info.m_Action;
		if (action == ActionType.BeingHeld)
		{
			return false;
		}
		return base.CanDoAction(Info, RightNow);
	}

	public override void PostCreate()
	{
		base.PostCreate();
		Transform transform = ObjectUtils.FindDeepChild(m_ModelRoot.transform, "ScreenPoint");
		if ((bool)transform)
		{
			GameObject original = (GameObject)Resources.Load("Prefabs/Lights/WorkerFrontLight", typeof(GameObject));
			m_FrontLight = Object.Instantiate(original, base.transform.position, Quaternion.identity, m_ModelRoot.transform).GetComponent<Light>();
			m_FrontLight.transform.SetParent(transform);
			m_FrontLight.transform.localPosition = new Vector3(0f, 0f, 0f);
		}
		m_LeftWing = m_ModelRoot.transform.Find("Wing1").gameObject;
		m_RightWing = m_ModelRoot.transform.Find("Wing2").gameObject;
	}

	public void RocketKicked(Vector3 StartPosition)
	{
		base.gameObject.SetActive(value: true);
		SpawnAnimationManager.Instance.AddJump(this, StartPosition, base.transform.position, 4f);
		SetState(State.RocketLand);
	}

	public void RocketEnd()
	{
		base.gameObject.SetActive(value: true);
	}

	public void StartTalking()
	{
		if (m_State != State.Talking)
		{
			SetState(State.Talking);
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Emoticon, base.transform.position, Quaternion.identity);
			baseClass.GetComponent<Emoticon>().SetWorldPosition(base.transform.position + new Vector3(0f, 2f, 0f));
			baseClass.GetComponent<Emoticon>().SetScale(2f);
			baseClass.GetComponent<Emoticon>().SetEmoticon("", 1f, "EmoticonBubble");
		}
	}

	public void StopTalking(bool Ended)
	{
		if (Ended)
		{
			SetState(State.Flap);
		}
		else
		{
			SetState(State.Idle);
		}
	}

	private void SetWings(float Rotation)
	{
		m_LeftWing.transform.localRotation = Quaternion.Euler(-90f - Rotation, 90f, 90f);
		m_RightWing.transform.localRotation = Quaternion.Euler(-90f + Rotation, 90f, 90f);
	}

	public void SetState(State NewState)
	{
		if ((m_State != State.Away && m_State != State.TutorialFinished) || NewState == State.Away || NewState == State.TutorialFinished)
		{
			State state = m_State;
			if (state == State.Talking)
			{
				EndStateTalking();
				SetWings(0f);
			}
			m_State = NewState;
			m_StateTimer = 0f;
			switch (m_State)
			{
			case State.TutorialFinished:
				SetRotation(2);
				break;
			case State.Away:
				AudioManager.Instance.StartEvent("TutorBotReturn", this);
				break;
			case State.Flap:
				break;
			}
		}
	}

	private void CheckMove()
	{
		List<BaseClass> players = CollectionManager.Instance.GetPlayers();
		TileCoord tileCoord = players[0].GetComponent<FarmerPlayer>().m_TileCoord;
		if ((tileCoord - m_TileCoord).Magnitude() > 5f)
		{
			TileCoord goToTilePosition = players[0].GetComponent<FarmerPlayer>().m_GoToTilePosition;
			if (m_GoToTilePosition != goToTilePosition)
			{
				SendAction(new ActionInfo(ActionType.Stop, default(TileCoord)));
				RequestGoTo(goToTilePosition, players[0].GetComponent<Actionable>(), LessOne: true);
				SetState(State.MovingWaiting);
			}
		}
		if (m_State == State.Idle)
		{
			LookAt(tileCoord);
		}
	}

	private bool CheckStop()
	{
		if ((CollectionManager.Instance.GetPlayers()[0].GetComponent<FarmerPlayer>().m_TileCoord - m_TileCoord).Magnitude() < 3f)
		{
			EndGoTo();
			SetState(State.Idle);
			return true;
		}
		return false;
	}

	public override bool StartGoTo(TileCoord Destination, Actionable TargetObject = null, bool LessOne = false, int Range = 0)
	{
		bool num = base.StartGoTo(Destination, TargetObject, LessOne, Range);
		if (num)
		{
			SetState(State.Moving);
			return num;
		}
		SetState(State.Idle);
		return num;
	}

	public override void NextGoTo()
	{
		if (!CheckStop())
		{
			base.NextGoTo();
			CheckMove();
		}
	}

	private void EndStateTalking()
	{
		base.transform.localScale = new Vector3(1f, 1f, 1f);
	}

	private void UpdateTalking()
	{
		if ((double)m_StateTimer < 0.4)
		{
			if (m_StateTimer < 0.1f)
			{
				SetWings(80f);
			}
			else if (m_StateTimer < 0.2f)
			{
				SetWings(100f);
			}
			else if (m_StateTimer < 0.3f)
			{
				SetWings(80f);
			}
			else
			{
				SetWings(100f);
			}
		}
		else
		{
			SetWings(0f);
		}
		float num = -0.05f;
		if ((int)(m_StateTimer * 60f) % 8 < 4)
		{
			num = 0.05f;
		}
		base.transform.localScale = new Vector3(1f - num, 1f + num, 1f - num);
	}

	private void Update()
	{
		switch (m_State)
		{
		case State.Idle:
		{
			float num = 0f;
			if ((int)(m_StateTimer * 60f) % 50 < 25)
			{
				num = 0.05f;
			}
			base.transform.localScale = new Vector3(1f - num, 1f + num, 1f - num);
			CheckMove();
			break;
		}
		case State.Moving:
			UpdateMovement();
			if (!m_Move)
			{
				SetState(State.Idle);
			}
			break;
		case State.Talking:
			UpdateTalking();
			break;
		case State.Flap:
			if (m_StateTimer > 0.25f)
			{
				SetState(State.Idle);
			}
			break;
		case State.Away:
			if (m_StateTimer > 1f)
			{
				m_Speed += 1f * TimeManager.Instance.m_NormalDelta;
				Vector3 position = base.transform.position;
				position.y += m_Speed;
				base.transform.position = position;
				if ((int)(m_StateTimer * 60f) % 8 < 4)
				{
					SetWings(70f);
				}
				else
				{
					SetWings(120f);
				}
			}
			break;
		}
		if (TimeManager.Instance.m_PauseTimeEnabled)
		{
			m_StateTimer += TimeManager.Instance.m_PauseDelta;
		}
		else
		{
			m_StateTimer += TimeManager.Instance.m_NormalDelta;
		}
	}
}
