using SimpleJSON;
using UnityEngine;

public class Train : Minecart
{
	public static float m_MaxWater = 10f;

	public static float m_MaxFuel = 1500f;

	public static float m_WaterUsePerSecond = 0.1f;

	public static float m_FuelUsePerSecond = 6f;

	public static float m_RefuellingTime = 2f;

	public float m_Water;

	public float m_Fuel;

	private MyParticles m_Particles;

	private MyParticles m_ParticlesMoving;

	private Transform m_ParticlePoints;

	private Transform m_Pistons;

	private Transform m_PistonRodPoint;

	private Transform m_PistonRodPoint2;

	private Transform m_ConnectingRod;

	private Transform m_ConnectingRod2;

	private Transform m_ConnectingRodPoint;

	private Transform m_ConnectingRodPoint2;

	private Vector3 m_PistonsStart;

	private Vector3 m_ConnectingRodPointStart;

	private PlaySound m_IdleSound;

	public WallFloorIcon m_WallFloorIcon;

	public static bool IsObjectTypeAcceptableFuel(ObjectType NewType)
	{
		if (NewType == ObjectType.Log || NewType == ObjectType.Stick || NewType == ObjectType.Plank || NewType == ObjectType.Pole || NewType == ObjectType.Charcoal || NewType == ObjectType.Coal)
		{
			return true;
		}
		return false;
	}

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("Train", m_TypeIdentifier);
	}

	public override void Restart()
	{
		base.Restart();
		SetBaseDelay(VariableManager.Instance.GetVariableAsFloat(ObjectType.Train, "BaseDelay"));
		m_Particles.Clear();
		m_ParticlesMoving.Clear();
		UpdateParticles();
		SetFrontWheelRotation(Quaternion.Euler(-90f, 0f, -180f));
		m_MoveSoundName = "TrainMotion";
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_MaxCarriages = VariableManager.Instance.GetVariableAsInt(ObjectType.Train, "MaxCarriages");
		m_MaxWater = VariableManager.Instance.GetVariableAsFloat(ObjectType.Train, "MaxWater");
		m_MaxFuel = VariableManager.Instance.GetVariableAsFloat(ObjectType.Train, "MaxFuel");
		m_WaterUsePerSecond = VariableManager.Instance.GetVariableAsFloat(ObjectType.Train, "WaterUsePerSecond");
		m_FuelUsePerSecond = VariableManager.Instance.GetVariableAsFloat(ObjectType.Train, "FuelUsePerSecond");
		m_RefuellingTime = VariableManager.Instance.GetVariableAsFloat(ObjectType.Train, "RefuellingTime");
		if ((bool)ParticlesManager.Instance)
		{
			m_Particles = ParticlesManager.Instance.CreateParticles("TrainSmoke", default(Vector3), Quaternion.Euler(-90f, 0f, 0f));
			m_ParticlesMoving = ParticlesManager.Instance.CreateParticles("TrainSmokeMoving", default(Vector3), Quaternion.Euler(-90f, 0f, 0f));
			m_ParticlePoints = m_ModelRoot.transform.Find("SmokePoint");
		}
		m_Pistons = m_ModelRoot.transform.Find("PistonNode/Pistons");
		m_PistonsStart = m_Pistons.transform.localPosition;
		m_PistonRodPoint = m_Pistons.transform.Find("PistonRodPoint.001");
		m_PistonRodPoint2 = m_Pistons.transform.Find("PistonRodPoint.002");
		m_ConnectingRod = m_Pistons.transform.Find("ConnectingRod.001");
		m_ConnectingRod2 = m_Pistons.transform.Find("ConnectingRod.002");
		m_ConnectingRodPoint = m_Wheels2.transform.Find("ConnectingRodPoint.001");
		m_ConnectingRodPoint2 = m_Wheels2.transform.Find("ConnectingRodPoint.002");
		m_ConnectingRodPointStart = m_ModelRoot.transform.InverseTransformPoint(m_ConnectingRodPoint.position);
		if (m_WallFloorIcon == null)
		{
			m_WallFloorIcon = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.WallFloorIcon, base.transform.position, Quaternion.identity).GetComponent<WallFloorIcon>();
			m_WallFloorIcon.SetScale(4f);
			m_WallFloorIcon.SetTarget(base.gameObject, new Vector3(0f, 3f, 0f));
			m_WallFloorIcon.gameObject.SetActive(value: false);
		}
	}

	protected new void OnDestroy()
	{
		if ((bool)m_Particles)
		{
			ParticlesManager.Instance.DestroyParticles(m_Particles);
		}
		if ((bool)m_ParticlesMoving)
		{
			ParticlesManager.Instance.DestroyParticles(m_ParticlesMoving);
		}
		if (m_WallFloorIcon != null)
		{
			m_WallFloorIcon.StopUsing();
			m_WallFloorIcon = null;
		}
		base.OnDestroy();
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		if (m_IdleSound != null)
		{
			AudioManager.Instance.StopEvent(m_IdleSound);
			m_IdleSound = null;
		}
		if (m_WallFloorIcon != null)
		{
			m_WallFloorIcon.StopUsing();
			m_WallFloorIcon = null;
		}
		SetFrontWheelRotation(Quaternion.Euler(-90f, 0f, -180f));
		if ((bool)m_Particles)
		{
			m_Particles.Stop();
		}
		if ((bool)m_ParticlesMoving)
		{
			m_Particles.Stop();
		}
		base.StopUsing(AndDestroy);
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		JSONUtils.Set(Node, "Water", m_Water);
		JSONUtils.Set(Node, "Fuel", m_Fuel);
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		m_Fuel = JSONUtils.GetAsFloat(Node, "Fuel", 0f);
		m_Water = JSONUtils.GetAsFloat(Node, "Water", 0f);
		UpdateNeedsPower();
	}

	private void AddWater(float Amount)
	{
		m_Water += Amount;
		if (m_Water > m_MaxWater)
		{
			m_Water = m_MaxWater;
		}
		UpdateNeedsPower();
	}

	private void AddWater(ToolFillable Bucket)
	{
		float amount = Bucket.m_Stored;
		Bucket.Empty(Bucket.m_Stored);
		AddWater(amount);
	}

	private void AddFuel(float Amount)
	{
		m_Fuel += Amount;
		if (m_Fuel > m_MaxFuel)
		{
			m_Fuel = m_MaxFuel;
		}
		UpdateNeedsPower();
	}

	private void AddFuel(Holdable NewObject)
	{
		float variableAsFloat = VariableManager.Instance.GetVariableAsFloat(NewObject.m_TypeIdentifier, "Fuel", CheckValid: false);
		AddFuel(variableAsFloat);
	}

	private bool GetHasFuelAndWater()
	{
		if (m_Fuel > 0f && m_Water > 0f)
		{
			return true;
		}
		return false;
	}

	public float GetFuelPercent()
	{
		return m_Fuel / m_MaxFuel;
	}

	public float GetWaterPercent()
	{
		return m_Water / m_MaxWater;
	}

	public override bool GetIsPowered()
	{
		return true;
	}

	public override void SetState(State NewState)
	{
		switch (m_State)
		{
		case State.None:
			if (m_IdleSound != null)
			{
				AudioManager.Instance.StopEvent(m_IdleSound);
				m_IdleSound = null;
			}
			break;
		case State.Refuelling:
			StopRefuelling();
			break;
		}
		base.SetState(NewState);
		if (m_State == State.None && GetHasFuelAndWater() && m_IdleSound == null)
		{
			m_IdleSound = AudioManager.Instance.StartEvent("TrainIdle", this, Remember: true);
		}
	}

	public override bool CanDoAction(ActionInfo Info, bool RightNow)
	{
		ActionType action = Info.m_Action;
		if ((uint)(action - 5) <= 1u && !GetHasFuelAndWater())
		{
			return false;
		}
		return base.CanDoAction(Info, RightNow);
	}

	public void StartActionAddFuel(AFO Info)
	{
		Actionable @object = Info.m_Object;
		@object.GetComponent<Savable>().SetIsSavable(IsSavable: false);
		AddFuel(@object.GetComponent<Holdable>());
	}

	private void EndAddFuel(AFO Info)
	{
		Info.m_Object.StopUsing();
	}

	private void AbortAddFuel(AFO Info)
	{
		Actionable @object = Info.m_Object;
		m_Fuel -= BurnableFuel.GetFuelEnergy(@object.m_TypeIdentifier);
		@object.GetComponent<Savable>().SetIsSavable(IsSavable: true);
	}

	private ActionType GetActionFromFuel(AFO Info)
	{
		Info.m_StartAction = StartActionAddFuel;
		Info.m_EndAction = EndAddFuel;
		Info.m_AbortAction = AbortAddFuel;
		Info.m_FarmerState = Farmer.State.Adding;
		if (m_State != 0)
		{
			return ActionType.Fail;
		}
		float fuelEnergy = BurnableFuel.GetFuelEnergy(Info.m_ObjectType);
		if (m_Fuel != 0f && m_Fuel + fuelEnergy > m_MaxFuel)
		{
			return ActionType.Fail;
		}
		return ActionType.AddResource;
	}

	public void StartActionAddFillable(AFO Info)
	{
		ToolFillable component = Info.m_Object.GetComponent<ToolFillable>();
		int num = component.m_Stored;
		if (m_Water + (float)num > m_MaxWater)
		{
			num = (int)(m_MaxWater - m_Water);
		}
		component.Empty(num);
		AddWater(num);
	}

	private void EndAddFillable(AFO Info)
	{
	}

	private void AbortAddFillable(AFO Info)
	{
	}

	private ActionType GetActionFromFillable(AFO Info)
	{
		Info.m_StartAction = StartActionAddFillable;
		Info.m_EndAction = EndAddFillable;
		Info.m_AbortAction = AbortAddFillable;
		Info.m_FarmerState = Farmer.State.Adding;
		if (m_State != 0)
		{
			return ActionType.Fail;
		}
		if (Info.m_Object == null)
		{
			return ActionType.Fail;
		}
		ToolFillable component = Info.m_Object.GetComponent<ToolFillable>();
		if (component == null)
		{
			return ActionType.Fail;
		}
		if (component.m_HeldType != ObjectType.Water)
		{
			return ActionType.Fail;
		}
		return ActionType.AddResource;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (Info.m_ActionType == AFO.AT.Secondary)
		{
			if (IsObjectTypeAcceptableFuel(Info.m_ObjectType))
			{
				ActionType actionFromFuel = GetActionFromFuel(Info);
				if (actionFromFuel != ActionType.Total)
				{
					return actionFromFuel;
				}
			}
			if (ToolFillable.GetIsTypeFillable(Info.m_ObjectType))
			{
				ActionType actionFromFillable = GetActionFromFillable(Info);
				if (actionFromFillable != ActionType.Total)
				{
					return actionFromFillable;
				}
			}
		}
		return base.GetActionFromObject(Info);
	}

	protected override bool CanMove()
	{
		if (!GetHasFuelAndWater())
		{
			return false;
		}
		return base.CanMove();
	}

	private void StartRefuelling(TrainRefuellingStation NewStation)
	{
		if (NewStation.m_Fuel != 0f || NewStation.m_Water != 0f)
		{
			float num = m_MaxFuel - m_Fuel;
			if (num > NewStation.m_Fuel)
			{
				num = NewStation.m_Fuel;
			}
			float num2 = m_MaxWater - m_Water;
			if (num2 > NewStation.m_Water)
			{
				num2 = NewStation.m_Water;
			}
			AddFuel(num);
			NewStation.RemoveFuel(num);
			AddWater(num2);
			NewStation.RemoveWater(num2);
			SetState(State.Refuelling);
			NewStation.StartRefuelling();
		}
	}

	private TrainRefuellingStation GetAdjacentRefuellingStation()
	{
		Tile tile = TileManager.Instance.GetTile(m_TileCoord);
		if (tile != null && tile.m_Floor != null)
		{
			TrainTrack component = tile.m_Floor.GetComponent<TrainTrack>();
			if ((bool)component && (component.m_TypeIdentifier == ObjectType.TrainTrack || component.m_TypeIdentifier == ObjectType.TrainTrackBridge))
			{
				Building stop = component.GetComponent<TrainTrackStraight>().GetStop();
				if ((bool)stop && stop.m_TypeIdentifier == ObjectType.TrainRefuellingStation)
				{
					return stop.GetComponent<TrainRefuellingStation>();
				}
			}
		}
		return null;
	}

	private void CheckForRefuellingStation()
	{
		TrainRefuellingStation adjacentRefuellingStation = GetAdjacentRefuellingStation();
		if ((bool)adjacentRefuellingStation && ((m_Fuel != m_MaxFuel && adjacentRefuellingStation.m_Fuel > 0f) || (m_Water != m_MaxWater && adjacentRefuellingStation.m_Water > 0f)))
		{
			StartRefuelling(adjacentRefuellingStation);
		}
	}

	public override void EndGoTo()
	{
		base.EndGoTo();
		AudioManager.Instance.StartEvent("TrainStopped", this);
		UpdateNeedsPower();
		CheckForRefuellingStation();
	}

	private void UpdateMoving()
	{
		m_Fuel -= m_FuelUsePerSecond * TimeManager.Instance.m_NormalDelta;
		if (m_Fuel < 0f)
		{
			m_Fuel = 0f;
		}
		m_Water -= m_WaterUsePerSecond * TimeManager.Instance.m_NormalDelta;
		if (m_Water < 0f)
		{
			m_Water = 0f;
		}
	}

	private void UpdateRefuelling()
	{
		float num = 0f;
		if (m_StateTimer >= m_RefuellingTime)
		{
			SetState(State.None);
		}
		else if ((int)(m_StateTimer * 60f) % 20 < 10)
		{
			num = 0.1f;
		}
		base.transform.localScale = new Vector3(1f - num, 1f + num, 1f - num);
	}

	private void StopRefuelling()
	{
		base.transform.localScale = new Vector3(1f, 1f, 1f);
	}

	private void UpdateParticles()
	{
		if (m_Particles == null)
		{
			return;
		}
		if (m_State == State.Moving)
		{
			if (!m_ParticlesMoving.GetIsPlaying())
			{
				m_ParticlesMoving.Play();
			}
			m_Particles.Stop();
		}
		else
		{
			m_ParticlesMoving.Stop();
			if (GetHasFuelAndWater())
			{
				if (!m_Particles.GetIsPlaying())
				{
					m_Particles.Play();
				}
			}
			else
			{
				m_Particles.Stop();
			}
		}
		m_Particles.transform.position = m_ParticlePoints.position;
		m_ParticlesMoving.transform.position = m_ParticlePoints.position;
	}

	private void SetFrontWheelRotation(Quaternion NewRotation)
	{
		m_Wheels2.transform.localRotation = NewRotation;
		Vector3 vector = m_ModelRoot.transform.InverseTransformPoint(m_ConnectingRodPoint.position) - m_ConnectingRodPointStart;
		m_Pistons.transform.localPosition = m_PistonsStart + new Vector3(0f, vector.magnitude, 0f);
		m_ConnectingRod.transform.LookAt(m_ConnectingRodPoint);
		m_ConnectingRod.transform.localRotation = m_ConnectingRod.transform.localRotation * Quaternion.Euler(-90f, 0f, 0f);
		m_ConnectingRod2.transform.LookAt(m_ConnectingRodPoint2);
		m_ConnectingRod2.transform.localRotation = m_ConnectingRod2.transform.localRotation * Quaternion.Euler(-90f, 0f, 0f);
	}

	private void UpdateNeedsPower()
	{
		if ((bool)m_WallFloorIcon)
		{
			if (GetHasFuelAndWater() || !GeneralUtils.m_InGame)
			{
				m_WallFloorIcon.Set(WallsNeeded: false, FloorsNeeded: false, PowerNeeded: false);
			}
			else
			{
				m_WallFloorIcon.Set(WallsNeeded: false, FloorsNeeded: false, PowerNeeded: true);
			}
		}
	}

	private void UpdateWheels()
	{
		float num = 1800f;
		if (!m_MovingForwards)
		{
			num = 0f - num;
		}
		float num2 = num * TimeManager.Instance.m_NormalDelta;
		Quaternion quaternion = Quaternion.Euler(0f - num2, 0f, 0f);
		m_Wheels1.transform.localRotation = m_Wheels1.transform.localRotation * quaternion;
		quaternion = Quaternion.Euler(num2 * 0.5f, 0f, 0f);
		SetFrontWheelRotation(m_Wheels2.transform.localRotation * quaternion);
		foreach (Carriage carriage in m_Carriages)
		{
			carriage.UpdateWheels(0f - num2);
		}
	}

	protected override void UpdateMoveAnimation()
	{
		if (m_State == State.Moving)
		{
			m_MovementTimer += TimeManager.Instance.m_NormalDelta;
			UpdateWheels();
		}
	}

	protected new void Update()
	{
		UpdateParticles();
		if (m_State == State.Moving)
		{
			UpdateMoving();
		}
		else if (m_State == State.Refuelling)
		{
			UpdateRefuelling();
		}
		base.Update();
		if (m_State == State.None)
		{
			CheckForRefuellingStation();
		}
	}
}
