using System;
using UnityEngine;

public class Fish : Holdable
{
	private enum State
	{
		OutOfWaterWait,
		OutOfWaterWriggle,
		Aquarium,
		ChopJump,
		Total
	}

	private State m_State;

	private float m_StateTimer;

	private float m_WriggleTimer;

	private Transform m_TailHinge;

	private Vector3 m_StartChopPosition;

	private Actionable m_Holder;

	private float m_Scale;

	private float m_Length;

	private static float m_AquariumScale = 0.5f;

	private static float m_AquariumRadius = 2.5f;

	private Vector3 m_AquariumPosition;

	private Vector3 m_AquariumTarget;

	private float m_AquariumTargetDelay;

	private float m_AquariumSpeed;

	private float m_AquariumHeading;

	private float m_AquariumHeight;

	private bool m_AquariumClose;

	public static bool GetIsTypeFish(ObjectType NewType)
	{
		if (NewType == ObjectType.FishSalmon || NewType == ObjectType.FishCatFish || NewType == ObjectType.FishMahiMahi || NewType == ObjectType.FishOrangeRoughy || NewType == ObjectType.FishPerch || NewType == ObjectType.FishCarp || NewType == ObjectType.FishGar || NewType == ObjectType.FishMonkfish || NewType == ObjectType.FishMarlin)
		{
			return true;
		}
		return false;
	}

	public override void Restart()
	{
		base.Restart();
		m_Scale = UnityEngine.Random.Range(0.5f, 1.5f);
		base.transform.localScale = new Vector3(m_Scale, m_Scale, m_Scale);
		m_Length = ObjectUtils.ObjectBounds(base.gameObject).size.x * m_AquariumScale * 0.5f;
		SetState(State.OutOfWaterWait);
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_TailHinge = m_ModelRoot.transform.Find("TailHinge");
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

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		m_Holder = Holder;
		if (Aquarium.GetIsTypeAquiarium(m_Holder.m_TypeIdentifier))
		{
			SetState(State.Aquarium);
		}
	}

	protected override void ActionDropped(Actionable PreviousHolder, TileCoord DropLocation)
	{
		base.ActionDropped(PreviousHolder, DropLocation);
		m_Holder = null;
		SetState(State.OutOfWaterWait);
	}

	private void UseSharpRock(AFO Info)
	{
		m_StartChopPosition = base.transform.localPosition;
		SetState(State.ChopJump);
		base.enabled = true;
	}

	private void EndSharpRock(AFO Info)
	{
		BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.FishRaw, base.transform.position, base.transform.localRotation);
		SpawnAnimationManager.Instance.AddJump(baseClass, m_TileCoord, m_TileCoord, 0f, baseClass.transform.position.y, 3f);
		AudioManager.Instance.StartEvent("ObjectCreated", this);
		QuestManager.Instance.AddEvent(QuestEvent.Type.Make, Info.m_Actioner.m_TypeIdentifier == ObjectType.Worker, ObjectType.FishRaw, Info.m_Actioner);
		StopUsing();
	}

	private ActionType GetActionFromSharpRock(AFO Info)
	{
		Info.m_UseAction = UseSharpRock;
		Info.m_EndAction = EndSharpRock;
		Info.m_FarmerState = Farmer.State.Chopping;
		if (m_State != 0 && m_State != State.OutOfWaterWriggle)
		{
			return ActionType.Fail;
		}
		return ActionType.UseInHands;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		ObjectType objectType = Info.m_ObjectType;
		if (Info.m_ActionType == AFO.AT.Primary && (objectType == ObjectType.RockSharp || objectType == ObjectType.ToolBlade))
		{
			return GetActionFromSharpRock(Info);
		}
		return base.GetActionFromObject(Info);
	}

	private void SetState(State NewState)
	{
		State state = m_State;
		if (state == State.Aquarium)
		{
			base.transform.localScale = new Vector3(m_Scale, m_Scale, m_Scale);
		}
		m_State = NewState;
		m_StateTimer = 0f;
		switch (m_State)
		{
		case State.OutOfWaterWait:
			m_StateTimer = 0f - UnityEngine.Random.Range(1f, 2f);
			break;
		case State.Aquarium:
			m_AquariumPosition = GetRandomAquariumPosition();
			m_AquariumHeight = GetRandomAquariumHeight();
			NextAquariumTarget();
			break;
		}
	}

	private void UpdateChopJump()
	{
		float num = m_StateTimer / 0.05f;
		if (num >= 1f)
		{
			base.enabled = false;
			m_State = State.OutOfWaterWait;
			base.transform.localPosition = m_StartChopPosition;
		}
		else
		{
			base.transform.localPosition = m_StartChopPosition + new Vector3(0f, Mathf.Sin(num * (float)Math.PI) * 0.5f, 0f);
		}
	}

	private float GetRandomAquariumHeight()
	{
		float result = 0f;
		if (m_Holder.m_TypeIdentifier == ObjectType.Aquarium)
		{
			result = UnityEngine.Random.Range(0.3f, 1.2f);
		}
		else if (m_Holder.m_TypeIdentifier == ObjectType.AquariumGood)
		{
			float num = 3f * Tile.m_Size;
			result = UnityEngine.Random.Range(0.3f, num + 0.3f);
		}
		return result;
	}

	private Vector3 GetAquariumTopLeft()
	{
		return new Vector3((0f - Tile.m_Size) * 2.25f, 0f, (0f - Tile.m_Size) * 1.25f);
	}

	private Vector3 GetAquariumBottomRight()
	{
		float x = 5.5f * Tile.m_Size;
		float z = 2.5f * Tile.m_Size;
		return GetAquariumTopLeft() + new Vector3(x, 0f, z);
	}

	private Vector3 GetRandomAquariumPosition()
	{
		Vector3 result = default(Vector3);
		if (m_Holder.m_TypeIdentifier == ObjectType.Aquarium)
		{
			float num = UnityEngine.Random.Range(0, 360);
			float num2 = UnityEngine.Random.Range(0f, m_AquariumRadius - m_Length);
			result = new Vector3(Mathf.Cos(num * ((float)Math.PI / 180f)) * num2, 0f, Mathf.Sin(num * ((float)Math.PI / 180f)) * num2);
		}
		else if (m_Holder.m_TypeIdentifier == ObjectType.AquariumGood)
		{
			float x = UnityEngine.Random.Range(GetAquariumTopLeft().x, GetAquariumBottomRight().x);
			float z = UnityEngine.Random.Range(GetAquariumTopLeft().z, GetAquariumBottomRight().z);
			result = new Vector3(x, 0f, z);
		}
		return result;
	}

	private void NextAquariumTarget()
	{
		int num = 0;
		do
		{
			m_AquariumTarget = GetRandomAquariumPosition();
			num++;
		}
		while ((m_AquariumTarget - m_AquariumPosition).magnitude < 1f && num < 50);
		m_AquariumTargetDelay = UnityEngine.Random.Range(4, 7);
		m_AquariumSpeed = 1f;
		m_StateTimer = 0f;
		m_AquariumClose = false;
	}

	private void UpdateAquarium()
	{
		if (m_StateTimer > m_AquariumTargetDelay)
		{
			NextAquariumTarget();
		}
		Vector3 vector = m_AquariumTarget - m_AquariumPosition;
		float num = Mathf.Atan2(vector.z, vector.x) * 57.29578f;
		Vector3 vector2 = new Vector3(Mathf.Cos(m_AquariumHeading * ((float)Math.PI / 180f)), 0f, Mathf.Sin(m_AquariumHeading * ((float)Math.PI / 180f)));
		if (vector.magnitude < vector2.magnitude * m_AquariumSpeed || m_AquariumClose)
		{
			m_AquariumClose = true;
			if (m_AquariumSpeed > 0.1f)
			{
				m_AquariumSpeed -= 0.5f * TimeManager.Instance.m_NormalDelta;
			}
			else
			{
				m_AquariumSpeed = 0.1f;
			}
			m_ModelRoot.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			m_TailHinge.localRotation = Quaternion.Euler(0f, 0f, 0f) * ObjectUtils.m_ModelRotator;
		}
		else
		{
			float num2 = (num - m_AquariumHeading) % 360f;
			if (num2 > 180f)
			{
				num2 = 360f - num2;
			}
			if (num2 < -180f)
			{
				num2 = 360f + num2;
			}
			m_AquariumHeading += num2 * TimeManager.Instance.m_NormalDelta * 5f;
			float num3 = num2 / 90f;
			if (num3 > 1f)
			{
				num3 = 1f;
			}
			if (num3 < -1f)
			{
				num3 = -1f;
			}
			m_ModelRoot.transform.localRotation = Quaternion.Euler(0f, -20f * num3, 0f);
			m_TailHinge.localRotation = Quaternion.Euler(0f, 60f * num3, 0f) * ObjectUtils.m_ModelRotator;
		}
		m_AquariumPosition += vector2 * m_AquariumSpeed * TimeManager.Instance.m_NormalDelta;
		if (m_Holder.m_TypeIdentifier == ObjectType.Aquarium)
		{
			float num4 = m_AquariumRadius - m_Length;
			if (m_AquariumPosition.magnitude > num4)
			{
				m_AquariumPosition.Normalize();
				m_AquariumPosition *= num4;
			}
		}
		else if (m_Holder.m_TypeIdentifier == ObjectType.AquariumGood)
		{
			if (m_AquariumPosition.x < GetAquariumTopLeft().x + m_Length)
			{
				m_AquariumPosition.x = GetAquariumTopLeft().x + m_Length;
			}
			if (m_AquariumPosition.z < GetAquariumTopLeft().z + m_Length)
			{
				m_AquariumPosition.z = GetAquariumTopLeft().z + m_Length;
			}
			if (m_AquariumPosition.x > GetAquariumBottomRight().x - m_Length)
			{
				m_AquariumPosition.x = GetAquariumBottomRight().x - m_Length;
			}
			if (m_AquariumPosition.z > GetAquariumBottomRight().z - m_Length)
			{
				m_AquariumPosition.z = GetAquariumBottomRight().z - m_Length;
			}
		}
		base.transform.localPosition = m_Holder.transform.TransformPoint(new Vector3(0f, m_AquariumHeight, Tile.m_Size) + m_AquariumPosition);
		base.transform.localRotation = Quaternion.Euler(0f, 0f - m_AquariumHeading + 180f, 0f) * m_Holder.transform.rotation;
		base.transform.localScale = new Vector3(m_AquariumScale * m_Scale, m_AquariumScale * m_Scale, m_AquariumScale * m_Scale);
	}

	private void Update()
	{
		if (TimeManager.Instance == null)
		{
			return;
		}
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
				m_TailHinge.localRotation = Quaternion.Euler(0f, 0f, 0f) * ObjectUtils.m_ModelRotator;
				SetState(State.OutOfWaterWait);
			}
			else if ((int)(m_StateTimer * 60f) % 10 < 5)
			{
				m_ModelRoot.transform.localRotation = Quaternion.Euler(0f, 10f, 0f);
				m_TailHinge.localRotation = Quaternion.Euler(0f, -30f, 0f) * ObjectUtils.m_ModelRotator;
			}
			else
			{
				m_ModelRoot.transform.localRotation = Quaternion.Euler(0f, -10f, 0f);
				m_TailHinge.localRotation = Quaternion.Euler(0f, 30f, 0f) * ObjectUtils.m_ModelRotator;
			}
			break;
		case State.Aquarium:
			UpdateAquarium();
			break;
		case State.ChopJump:
			UpdateChopJump();
			break;
		}
		m_StateTimer += TimeManager.Instance.m_NormalDelta;
	}
}
