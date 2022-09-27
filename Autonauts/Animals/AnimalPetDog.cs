using UnityEngine;

public class AnimalPetDog : AnimalPet
{
	private enum State
	{
		Idle,
		Sit,
		Sleep,
		ChaseTail,
		LickBum,
		Sniff,
		Dig,
		Alert,
		RequestMove,
		Moving,
		Carry,
		Total
	}

	private static float m_AttentionSpan = 30f;

	private static float m_BoredomSpan = 30f;

	private static float m_NearPlayerRange = Tile.m_Size * 2f;

	private static float m_FarPlayerRange = Tile.m_Size * 5f;

	private State m_State;

	private float m_StateTimer;

	private string[] m_Animations = new string[11]
	{
		"Idle", "Sit", "Sleep", "ChaseTail", "LickBum", "Sniff", "Dig", "Idle", "Idle", "Idle",
		"Carry"
	};

	private float m_AttentionTimer;

	private float m_BoredomTimer;

	private string m_Animation;

	private Animator m_Animator;

	public GameObject m_Head;

	public static bool GetIsTypeAnimalPetDog(ObjectType NewType)
	{
		if (NewType == ObjectType.AnimalPetDog || NewType == ObjectType.AnimalPetDog2)
		{
			return true;
		}
		return false;
	}

	public override void Restart()
	{
		base.Restart();
		m_State = State.Total;
		SetState(State.Idle);
	}

	public override string GetHumanReadableName()
	{
		string text = base.GetHumanReadableName();
		if (m_State == State.Sleep)
		{
			text = text + " (" + TextManager.Instance.Get("AnimalSleeping") + ")";
		}
		return text;
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_Head = m_ModelRoot.transform.Find("HeadRoot").gameObject;
		m_Animator = GetComponent<Animator>();
		m_Animator.Rebind();
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		base.transform.localRotation = Quaternion.Euler(180f, 90f, 0f);
		base.transform.localPosition = new Vector3(0f, 1.5f, 0f);
		SetBaggedObject(null);
		SetBaggedTile(default(TileCoord));
		SetState(State.Carry);
	}

	protected override void ActionDropped(Actionable PreviousHolder, TileCoord DropLocation)
	{
		base.ActionDropped(PreviousHolder, DropLocation);
		base.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		SetState(State.Idle);
	}

	public override bool RequestGoTo(TileCoord Destination, Actionable TargetObject = null, bool LessOne = false, int Range = 0)
	{
		SetState(State.RequestMove);
		return base.RequestGoTo(Destination, TargetObject, LessOne, Range);
	}

	public override bool StartGoTo(TileCoord Destination, Actionable TargetObject = null, bool LessOne = false, int Range = 0)
	{
		if (m_State != 0 && m_State != State.Moving && m_State != State.RequestMove)
		{
			return false;
		}
		SetState(State.Moving);
		if (!base.StartGoTo(Destination, TargetObject, LessOne, Range))
		{
			if (m_State == State.Moving)
			{
				SetState(State.Idle);
			}
			return false;
		}
		return true;
	}

	public override void NextGoTo()
	{
		base.NextGoTo();
	}

	public override void ObstructionEncountered()
	{
		base.ObstructionEncountered();
		SetState(State.RequestMove);
		base.RequestGoTo(m_GoToTilePosition, m_GoToTargetObject, m_GoToLessOne);
	}

	public override void EndGoTo()
	{
		base.EndGoTo();
		SetState(State.Idle);
	}

	private void SetState(State NewState)
	{
		switch (m_State)
		{
		case State.Moving:
			m_ModelRoot.transform.localPosition = new Vector3(0f, 0f, 0f);
			break;
		case State.Sleep:
			m_Indicator.SetSleeping(Sleeping: false);
			break;
		}
		m_State = NewState;
		m_StateTimer = 0f;
		switch (m_State)
		{
		case State.Sleep:
			m_Indicator.SetSleeping(Sleeping: true);
			break;
		case State.Alert:
		{
			FaceTarget(GetPlayer().m_TileCoord);
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Emoticon, base.transform.position, Quaternion.identity);
			baseClass.GetComponent<Emoticon>().SetWorldPosition(base.transform.position + new Vector3(0f, 2f, 0f));
			baseClass.GetComponent<Emoticon>().SetScale(2f);
			baseClass.GetComponent<Emoticon>().SetEmoticon("", 1f, "EmoticonExclamation");
			baseClass.GetComponent<Emoticon>().Follow(this);
			break;
		}
		}
		string text = m_Animations[(int)m_State];
		if (m_Animation != text)
		{
			m_Animation = text;
			m_Animator.Play(text, -1, 0f);
		}
	}

	public override void PlayerCall()
	{
		base.PlayerCall();
		m_AttentionTimer = m_AttentionSpan;
		if (m_State != State.RequestMove && m_State != State.Moving && m_State != State.Carry)
		{
			SetState(State.Alert);
			AudioManager.Instance.StartEvent("DogAnswer", this);
		}
	}

	private void GoToPlayer()
	{
		TileCoord tileNearPlayer = GetTileNearPlayer(m_NearPlayerRange);
		RequestGoTo(tileNearPlayer);
	}

	private void GoToRandomTileNearby(int Range)
	{
		TileCoord tileCoord = m_TileCoord;
		int num = 0;
		do
		{
			tileCoord.x += Random.Range(-Range, Range);
			tileCoord.y += Random.Range(-Range, Range);
			num++;
		}
		while ((!tileCoord.GetIsValid() || !PlotManager.Instance.GetPlotAtTile(tileCoord).m_Visible) && num < 1000);
		if (num < 1000)
		{
			RequestGoTo(tileCoord);
		}
	}

	private void DoRandomIdle()
	{
		if (m_BoredomTimer > 0f)
		{
			int num = 0;
			bool flag = false;
			do
			{
				num = Random.Range(0, 6);
				flag = false;
				if (num == 3 && TileManager.Instance.GetTile(m_TileCoord).m_Floor != null)
				{
					flag = true;
				}
			}
			while (flag);
			switch (num)
			{
			case 0:
				SetState(State.LickBum);
				break;
			case 1:
				SetState(State.ChaseTail);
				break;
			case 2:
				SetState(State.Sniff);
				break;
			case 3:
				SetState(State.Dig);
				break;
			default:
				GoToRandomTileNearby(10);
				break;
			}
		}
		else
		{
			m_AttentionTimer = m_AttentionSpan;
			GoToPlayer();
		}
	}

	private void UpdateStateIdle()
	{
		if (!(m_StateTimer > 2f))
		{
			return;
		}
		if (m_AttentionTimer > 0f)
		{
			if (GetPlayerRange() > m_FarPlayerRange)
			{
				GoToPlayer();
			}
			else
			{
				SetState(State.Sit);
			}
		}
		else
		{
			DoRandomIdle();
		}
	}

	private void UpdateStateSit()
	{
		if (!(m_StateTimer > 5f))
		{
			return;
		}
		if (m_AttentionTimer > 0f)
		{
			if (GetPlayerRange() > m_FarPlayerRange)
			{
				GoToPlayer();
			}
			else
			{
				SetState(State.Sleep);
			}
		}
		else
		{
			DoRandomIdle();
		}
	}

	private bool IsAnimationPlaying()
	{
		if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
		{
			return true;
		}
		return false;
	}

	private void UpdateStateChaseTail()
	{
		if (!IsAnimationPlaying())
		{
			SetState(State.Idle);
		}
	}

	private void UpdateStateLickBum()
	{
		if (m_StateTimer > 2f)
		{
			SetState(State.Idle);
		}
	}

	public void ActionDig()
	{
		Vector3 localPosition = m_TileCoord.ToWorldPositionTileCentered();
		MyParticles myParticles = ParticlesManager.Instance.CreateParticles("Dig", localPosition, Quaternion.Euler(-70f, 0f, 0f));
		myParticles.transform.SetParent(base.transform);
		myParticles.transform.localPosition = default(Vector3);
		myParticles.transform.localRotation = Quaternion.Euler(-70f, 0f, 0f);
		ParticlesManager.Instance.DestroyParticles(myParticles, WaitUntilNoParticles: true);
	}

	private void UpdateStateDig()
	{
		if (m_StateTimer > 2f)
		{
			SetState(State.Idle);
		}
	}

	private void UpdateStateSleep()
	{
		if (m_StateTimer > 2f)
		{
			SetState(State.Sit);
		}
	}

	private void UpdateStateAlert()
	{
		if (m_StateTimer > 0.5f)
		{
			GoToPlayer();
		}
	}

	private void UpdateStateMove()
	{
		UpdateMovement();
		if (m_Move)
		{
			float y = 0f;
			if ((int)(m_StateTimer * 60f) % 8 < 5)
			{
				y = 0.5f;
			}
			base.transform.localPosition += new Vector3(0f, y, 0f);
		}
	}

	private void Update()
	{
		if (TimeManager.Instance == null)
		{
			return;
		}
		switch (m_State)
		{
		case State.Idle:
			UpdateStateIdle();
			break;
		case State.Sit:
		case State.Sleep:
			UpdateStateSit();
			break;
		case State.ChaseTail:
			UpdateStateChaseTail();
			break;
		case State.LickBum:
		case State.Sniff:
			UpdateStateLickBum();
			break;
		case State.Dig:
			UpdateStateDig();
			break;
		case State.Alert:
			UpdateStateAlert();
			break;
		case State.Moving:
			UpdateStateMove();
			break;
		}
		m_StateTimer += TimeManager.Instance.m_NormalDelta;
		if (m_AttentionTimer > 0f)
		{
			m_AttentionTimer -= TimeManager.Instance.m_NormalDelta;
			if (m_AttentionTimer <= 0f)
			{
				m_BoredomTimer = m_BoredomSpan;
			}
		}
		else
		{
			m_BoredomTimer -= TimeManager.Instance.m_NormalDelta;
		}
		if (TimeManager.Instance.m_NormalTimeEnabled)
		{
			m_Animator.speed = TimeManager.Instance.m_TimeScale;
		}
		else
		{
			m_Animator.speed = 0f;
		}
	}
}
