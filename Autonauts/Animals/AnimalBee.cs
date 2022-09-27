using System;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class AnimalBee : Animal
{
	public enum State
	{
		InHive,
		Idle,
		MovingToFlower,
		GettingPollen,
		ReturnToHive,
		Carry,
		Total
	}

	private static float m_InHiveDelay = 3f;

	private static float m_IdleDelay = 2f;

	private static float m_GettingPollenDelay = 2f;

	private static int m_MaxEatCount = 5;

	private static int m_BaggedObjectRange = 10;

	private static float m_Speed = 10f;

	private static float m_MoveToFinishDistance = 0.25f;

	private static float m_HiveHeight = 3f;

	private static float m_BaggedObjectHeight = 1.5f;

	private static float m_WobbleHeight = 0.5f;

	public State m_State;

	public float m_StateTimer;

	private int m_EatCount;

	private StorageBeehive m_Hive;

	private Vector3 m_Position;

	private Vector3 m_MoveToPosition;

	private Vector3 m_MoveDelta;

	private float m_MoveDistance;

	private float m_MoveTimer;

	private float m_OldDiff;

	private PlaySound m_PlaySound;

	private int m_TempHiveID;

	public override void Restart()
	{
		base.Restart();
		m_EatCount = 0;
	}

	public override void PostCreate()
	{
		base.PostCreate();
		InstantiationManager.Instance.SetLayer(base.gameObject, Layers.Used0);
		_ = (bool)m_ModelRoot;
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		base.StopUsing(AndDestroy);
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		JSONUtils.Set(Node, "ST", (int)m_State);
		JSONUtils.Set(Node, "STT", m_StateTimer);
		JSONUtils.Set(Node, "EC", m_EatCount);
		JSONUtils.Set(Node, "Hive", m_Hive.m_UniqueID);
		SetBaggedObject(null);
	}

	public override void Load(JSONNode Node)
	{
		TileCoord tileCoord = default(TileCoord);
		tileCoord.Load(Node, "T");
		if (!tileCoord.GetIsValid())
		{
			JSONUtils.Set(Node, "TX", 0);
			JSONUtils.Set(Node, "TY", 0);
		}
		base.Load(Node);
		SetState(State.InHive);
		m_StateTimer = JSONUtils.GetAsFloat(Node, "STT", 0f);
		m_EatCount = JSONUtils.GetAsInt(Node, "EC", 0);
		m_TempHiveID = JSONUtils.GetAsInt(Node, "Hive", 0);
		m_Position = base.transform.position;
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		base.transform.localRotation = Quaternion.Euler(180f, 90f, 0f);
		SetBaggedObject(null);
		if ((bool)Holder && (bool)Holder.GetComponent<Farmer>())
		{
			SetState(State.Carry);
		}
	}

	protected override void ActionDropped(Actionable PreviousHolder, TileCoord DropLocation)
	{
		base.ActionDropped(PreviousHolder, DropLocation);
		base.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		SetPosition(DropLocation.ToWorldPositionTileCentered());
		m_Position = base.transform.position;
		SetState(State.Idle);
	}

	public override void SendAction(ActionInfo Info)
	{
		ActionType action = Info.m_Action;
		if ((uint)(action - 41) <= 1u)
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(m_TempHiveID, ErrorCheck: false);
			if ((bool)objectFromUniqueID)
			{
				objectFromUniqueID.GetComponent<StorageBeehive>().AddBee(this);
				SetBeeHive(objectFromUniqueID.GetComponent<StorageBeehive>());
				m_Position = base.transform.position;
				m_MoveToPosition = m_Position;
			}
			else
			{
				Debug.Log("Bee.Refresh : Couldn't find beehive with UID " + m_TempHiveID);
			}
		}
		base.SendAction(Info);
	}

	public override bool CanDoAction(ActionInfo Info, bool RightNow)
	{
		ActionType action = Info.m_Action;
		if (action == ActionType.BeingHeld)
		{
			if (m_State == State.GettingPollen || m_State == State.Idle)
			{
				return true;
			}
			return false;
		}
		return base.CanDoAction(Info, RightNow);
	}

	public override bool RequestGoTo(TileCoord Destination, Actionable TargetObject = null, bool LessOne = false, int Range = 0)
	{
		return base.RequestGoTo(Destination, TargetObject, LessOne, Range);
	}

	public override bool StartGoTo(TileCoord Destination, Actionable TargetObject = null, bool LessOne = false, int Range = 0)
	{
		if (!base.StartGoTo(Destination, TargetObject, LessOne, Range))
		{
			return false;
		}
		return true;
	}

	public override void NextGoTo()
	{
		base.NextGoTo();
	}

	public override void EndGoTo()
	{
		base.EndGoTo();
	}

	public void SetBeeHive(StorageBeehive NewHive)
	{
		m_Hive = NewHive;
		m_StateTimer = 0f - UnityEngine.Random.Range(0f, 4f);
		Vector3 position = base.transform.position;
		position.y = m_HiveHeight;
		base.transform.position = position;
		if ((bool)m_Hive)
		{
			UpdatePositionToTilePosition(m_Hive.m_TileCoord);
		}
	}

	public void SetState(State NewState)
	{
		switch (m_State)
		{
		case State.InHive:
			m_ModelRoot.SetActive(value: true);
			break;
		case State.MovingToFlower:
			AudioManager.Instance.StopEvent(m_PlaySound);
			break;
		case State.ReturnToHive:
			AudioManager.Instance.StopEvent(m_PlaySound);
			break;
		}
		m_State = NewState;
		m_StateTimer = 0f;
		switch (m_State)
		{
		case State.InHive:
			if ((bool)m_Hive)
			{
				UpdatePositionToTilePosition(m_Hive.m_TileCoord);
			}
			m_ModelRoot.SetActive(value: false);
			break;
		case State.GettingPollen:
			m_StateTimer = 0f - UnityEngine.Random.Range(0f, 1f);
			break;
		case State.MovingToFlower:
			m_PlaySound = AudioManager.Instance.StartEvent("AnimalBeeIdle", this, Remember: true, Follow: true);
			break;
		case State.ReturnToHive:
			m_PlaySound = AudioManager.Instance.StartEvent("AnimalBeeIdle", this, Remember: true, Follow: true);
			break;
		case State.Idle:
			break;
		}
	}

	public BaseClass GetRandomFlower()
	{
		List<BaseClass> list = new List<BaseClass>();
		if (m_Hive == null)
		{
			return null;
		}
		TileCoord tileCoord = m_Hive.m_TileCoord + new TileCoord(-m_BaggedObjectRange, -m_BaggedObjectRange);
		TileCoord tileCoord2 = m_Hive.m_TileCoord + new TileCoord(m_BaggedObjectRange, m_BaggedObjectRange);
		TileCoord tileCoord3 = new TileCoord(tileCoord.x / Plot.m_PlotTilesWide, tileCoord.y / Plot.m_PlotTilesHigh);
		TileCoord tileCoord4 = new TileCoord(tileCoord2.x / Plot.m_PlotTilesWide, tileCoord2.y / Plot.m_PlotTilesHigh);
		for (int i = tileCoord3.y; i <= tileCoord4.y; i++)
		{
			for (int j = tileCoord3.x; j <= tileCoord4.x; j++)
			{
				Plot plotAtPlot = PlotManager.Instance.GetPlotAtPlot(j, i);
				if (!plotAtPlot || !plotAtPlot.m_Visible)
				{
					continue;
				}
				if (plotAtPlot.m_ObjectDictionary.ContainsKey(ObjectType.FlowerWild))
				{
					foreach (TileCoordObject item in plotAtPlot.m_ObjectDictionary[ObjectType.FlowerWild])
					{
						if ((item.m_TileCoord - m_Hive.m_TileCoord).Magnitude() < (float)m_BaggedObjectRange && item.GetComponent<FlowerWild>().m_State == FlowerWild.State.Idle && !BaggedManager.Instance.IsObjectBagged(item))
						{
							list.Add(item);
						}
					}
				}
				if (!plotAtPlot.m_ObjectDictionary.ContainsKey(ObjectType.FlowerPot))
				{
					continue;
				}
				foreach (TileCoordObject item2 in plotAtPlot.m_ObjectDictionary[ObjectType.FlowerPot])
				{
					if (item2.GetComponent<FlowerPot>().m_State == FlowerPot.State.Grown && (item2.m_TileCoord - m_Hive.m_TileCoord).Magnitude() < (float)m_BaggedObjectRange)
					{
						list.Add(item2);
					}
				}
			}
		}
		if (list.Count == 0)
		{
			return null;
		}
		return list[UnityEngine.Random.Range(0, list.Count)];
	}

	private void MoveTo(TileCoord NewPosition, float Height)
	{
		m_Position = base.transform.position;
		m_MoveToPosition = NewPosition.ToWorldPositionTileCentered();
		m_MoveToPosition.y = Height;
		m_MoveDistance = (m_MoveToPosition - m_Position).magnitude;
		m_MoveTimer = 0f;
		Vector3 moveDelta = m_MoveToPosition - m_Position;
		moveDelta.Normalize();
		m_MoveDelta = moveDelta;
		float y = -90f - Mathf.Atan2(moveDelta.z, moveDelta.x) * 57.29578f;
		base.transform.rotation = Quaternion.Euler(0f, y, 0f);
		m_OldDiff = 1E+08f;
	}

	private bool UpdateMoveTo()
	{
		float magnitude = (m_MoveToPosition - m_Position).magnitude;
		if (magnitude < m_MoveToFinishDistance || magnitude > m_OldDiff)
		{
			SetPosition(m_MoveToPosition);
			return false;
		}
		m_OldDiff = magnitude;
		m_Position += m_MoveDelta * m_Speed * TimeManager.Instance.m_NormalDelta;
		if (m_Position.x < 0f)
		{
			m_Position.x = 0f;
		}
		if (m_Position.z > 0f)
		{
			m_Position.z = 0f;
		}
		if (m_Position.x >= (float)TileManager.Instance.m_TilesWide * Tile.m_Size - 0.1f)
		{
			m_Position.x = (float)TileManager.Instance.m_TilesWide * Tile.m_Size - 0.1f;
		}
		if (m_Position.z <= (float)(-TileManager.Instance.m_TilesHigh) * Tile.m_Size + 0.1f)
		{
			m_Position.z = (float)(-TileManager.Instance.m_TilesHigh) * Tile.m_Size + 0.1f;
		}
		Vector3 position = m_Position;
		float num = magnitude / m_MoveDistance;
		float num2 = Mathf.Sin(num * (float)Math.PI);
		position.y += num2 * 2.5f;
		m_MoveTimer += TimeManager.Instance.m_NormalDelta;
		float num3 = Mathf.Cos(m_MoveTimer * (float)Math.PI * 2f) * m_MoveDelta.z * m_WobbleHeight * num;
		float num4 = Mathf.Sin(m_MoveTimer * (float)Math.PI * 3f) * m_WobbleHeight * num;
		float num5 = Mathf.Cos(m_MoveTimer * (float)Math.PI * 2f) * m_MoveDelta.x * m_WobbleHeight * num;
		position.x += num3;
		position.y += num4;
		position.z += num5;
		SetPosition(position);
		return true;
	}

	private void MoveToFlower()
	{
		BaseClass randomFlower = GetRandomFlower();
		if ((bool)randomFlower)
		{
			if (m_BaggedObject == randomFlower)
			{
				SetState(State.GettingPollen);
				return;
			}
			SetBaggedObject(randomFlower.GetComponent<TileCoordObject>());
			MoveTo(m_BaggedObject.GetComponent<TileCoordObject>().m_TileCoord, m_BaggedObjectHeight);
			SetState(State.MovingToFlower);
		}
		else if (m_State != 0 && m_State != State.Idle)
		{
			ReturnToHive();
		}
	}

	private void ReturnToHive()
	{
		m_BaggedObject = null;
		MoveTo(m_Hive.m_TileCoord, m_HiveHeight);
		SetState(State.ReturnToHive);
	}

	private void UpdateInHive()
	{
		if (m_StateTimer > m_InHiveDelay)
		{
			MoveToFlower();
			if (m_State == State.InHive)
			{
				m_Indicator.SetNeedFood(NeedsFood: true);
			}
			else
			{
				m_Indicator.SetNeedFood(NeedsFood: false);
			}
		}
	}

	private void UpdateIdle()
	{
		float x = Mathf.Cos(m_StateTimer * (float)Math.PI * 2f) * m_WobbleHeight;
		float num = Mathf.Sin(m_StateTimer * (float)Math.PI * 3f) * m_WobbleHeight;
		float z = Mathf.Sin(m_StateTimer * (float)Math.PI * 2f) * m_WobbleHeight;
		base.transform.position = m_Position + new Vector3(x, num + m_WobbleHeight, z);
		if (m_StateTimer > m_IdleDelay)
		{
			MoveToFlower();
		}
	}

	private void UpdateMovingToFlower()
	{
		if (!UpdateMoveTo())
		{
			SetState(State.GettingPollen);
			if ((bool)m_BaggedObject && (bool)m_BaggedObject.GetComponent<FlowerWild>())
			{
				m_BaggedObject.GetComponent<FlowerWild>().BeeUsed();
				SetBaggedObject(null);
			}
		}
	}

	private void UpdateGettingPollen()
	{
		if (m_StateTimer > m_GettingPollenDelay)
		{
			m_EatCount++;
			if (m_EatCount >= m_MaxEatCount)
			{
				ReturnToHive();
			}
			else
			{
				MoveToFlower();
			}
		}
	}

	private void UpdateReturnToHive()
	{
		if (!UpdateMoveTo())
		{
			if (m_EatCount >= m_MaxEatCount)
			{
				m_EatCount = 0;
				m_Hive.AddHoney();
			}
			SetState(State.InHive);
		}
	}

	private void Update()
	{
		if (TimeManager.Instance == null)
		{
			return;
		}
		if (m_Hive == null)
		{
			StopUsing();
			return;
		}
		switch (m_State)
		{
		case State.InHive:
			UpdateInHive();
			break;
		case State.Idle:
			UpdateIdle();
			break;
		case State.MovingToFlower:
			UpdateMovingToFlower();
			break;
		case State.GettingPollen:
			UpdateGettingPollen();
			break;
		case State.ReturnToHive:
			UpdateReturnToHive();
			break;
		}
		m_StateTimer += TimeManager.Instance.m_NormalDelta;
	}
}
