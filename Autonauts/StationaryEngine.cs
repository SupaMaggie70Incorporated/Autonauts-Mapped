using SimpleJSON;
using UnityEngine;

public class StationaryEngine : LinkedSystemEngine
{
	private static float m_WaterToEnergy = 100f;

	protected TileCoord m_SpawnPoint;

	private PlaySound m_PlaySound;

	[HideInInspector]
	public float m_Water;

	[HideInInspector]
	public float m_WaterCapacity;

	private ObjectType m_ConvertingFuelType;

	private MyParticles m_Smoke;

	private Transform m_SmokeTransform;

	private GameObject m_Flywheel;

	private GameObject m_Beam;

	private GameObject m_ConnectingRod;

	private GameObject m_PistonRod;

	private GameObject m_ConnectingRodPoint;

	private GameObject m_PistonRodPoint;

	private Vector3 m_ConnectingRodPointStart;

	private float m_CrankRadius;

	private MyLight m_FireLight;

	private PlaySound m_RunningSound;

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(-1, -1), new TileCoord(2, 0), new TileCoord(-2, 0));
		m_EnergyCapacity = VariableManager.Instance.GetVariableAsInt(ObjectType.StationaryEngine, "EnergyCapacity");
		m_WaterCapacity = VariableManager.Instance.GetVariableAsInt(ObjectType.StationaryEngine, "WaterCapacity");
		m_RunningSound = null;
		if (m_Smoke == null)
		{
			m_SmokeTransform = m_ModelRoot.transform.Find("SmokePoint");
			if ((bool)ParticlesManager.Instance)
			{
				m_Smoke = ParticlesManager.Instance.CreateParticles("StationarySteamEngineSmoke", m_SmokeTransform.position, Quaternion.Euler(-90f, 0f, 0f));
				m_Smoke.Stop();
			}
		}
		m_PulleySide = 3;
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_Flywheel = m_ModelRoot.transform.Find("Wheels").gameObject;
		m_Beam = m_ModelRoot.transform.Find("Beam").gameObject;
		m_ConnectingRod = m_Beam.transform.Find("ConnectingRod").gameObject;
		m_PistonRod = m_Beam.transform.Find("PistonRod").gameObject;
		m_ConnectingRodPoint = m_Flywheel.transform.Find("ConnectingRodPoint").gameObject;
		m_ConnectingRodPointStart = m_ModelRoot.transform.InverseTransformPoint(m_ConnectingRodPoint.transform.position);
		m_PistonRodPoint = m_ModelRoot.transform.Find("PistonRodPoint").gameObject;
		m_CrankRadius = m_ConnectingRodPoint.transform.position.y - m_Flywheel.transform.position.y;
		m_FireLight = LightManager.Instance.LoadLight("GenericFire", base.transform, new Vector3(0f, 0f, 0f));
		Vector3 position = m_ModelRoot.transform.Find("FirePoint").position;
		m_FireLight.transform.position = position;
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		ParticlesManager.Instance.DestroyParticles(m_Smoke);
		m_Smoke = null;
		if (m_RunningSound != null)
		{
			AudioManager.Instance.StopEvent(m_RunningSound);
			m_RunningSound = null;
		}
		base.StopUsing(AndDestroy);
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		JSONUtils.Set(Node, "Water", m_Water);
		JSONUtils.Set(Node, "ConvertingFuelType", ObjectTypeList.Instance.GetSaveNameFromIdentifier(m_ConvertingFuelType));
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		m_Water = JSONUtils.GetAsFloat(Node, "Water", 0f);
		string asString = JSONUtils.GetAsString(Node, "ConvertingFuelType", "Unknown");
		if (asString == "Unknown")
		{
			m_ConvertingFuelType = ObjectTypeList.m_Total;
		}
		else
		{
			m_ConvertingFuelType = ObjectTypeList.Instance.GetIdentifierFromSaveName(asString, Check: false);
		}
	}

	public override bool IsEnergyReady()
	{
		if (m_Water == 0f)
		{
			return false;
		}
		if (m_WallMissing || m_FloorMissing)
		{
			return false;
		}
		return base.IsEnergyReady();
	}

	public void AddFuel(ObjectType NewType)
	{
		AddEnergy((int)BurnableFuel.GetFuelEnergy(NewType));
	}

	public override void UseEnergy(int Amount)
	{
		m_Water -= (float)Amount / m_WaterToEnergy;
		if (m_Water < 0f)
		{
			m_Water = 0f;
		}
		base.UseEnergy(Amount);
	}

	public void AddWater(int Amount)
	{
		m_Water += Amount;
		if (m_Water > m_WaterCapacity)
		{
			m_Water = m_WaterCapacity;
		}
		if (m_LinkedSystem != null)
		{
			((LinkedSystemMechanical)m_LinkedSystem).UpdateConverters();
		}
	}

	public override void CheckWallsFloors()
	{
		base.CheckWallsFloors();
		if (m_LinkedSystem != null)
		{
			((LinkedSystemMechanical)m_LinkedSystem).UpdateConverters();
		}
	}

	public override float GetEnergy()
	{
		float num = m_Energy;
		float num2 = m_Water * m_WaterToEnergy;
		if (num2 < num)
		{
			num = num2;
		}
		return num;
	}

	public override void SendAction(ActionInfo Info)
	{
		base.SendAction(Info);
		ActionType action = Info.m_Action;
		if ((uint)(action - 41) <= 1u && (bool)m_Smoke)
		{
			m_Smoke.transform.position = m_SmokeTransform.position;
		}
	}

	private void StartAddBucket(AFO Info)
	{
		AddWater(1);
	}

	private void EndAddBucket(AFO Info)
	{
		ToolFillable component = Info.m_Object.GetComponent<ToolBucket>();
		if ((bool)component)
		{
			component.Empty(1);
		}
		AudioManager.Instance.StartEvent("BuildingIngredientAdd", this);
	}

	private void AbortAddBucket(AFO Info)
	{
		AddWater(-1);
		ToolFillable component = Info.m_Object.GetComponent<ToolBucket>();
		if ((bool)component)
		{
			component.Fill(component.m_HeldType, 1);
		}
	}

	private ActionType GetActionFromBucket(AFO Info)
	{
		Info.m_StartAction = StartAddBucket;
		Info.m_EndAction = EndAddBucket;
		Info.m_AbortAction = AbortAddBucket;
		Info.m_FarmerState = Farmer.State.Adding;
		if (m_State != 0)
		{
			return ActionType.Fail;
		}
		if ((bool)Info.m_Object)
		{
			ToolBucket component = Info.m_Object.GetComponent<ToolBucket>();
			if (!component)
			{
				return ActionType.Fail;
			}
			if (component.m_HeldType != ObjectType.Water)
			{
				return ActionType.Fail;
			}
		}
		if (m_Water == m_WaterCapacity)
		{
			return ActionType.Fail;
		}
		return ActionType.AddResource;
	}

	private void StartAddFuel(AFO Info)
	{
		Actionable @object = Info.m_Object;
		m_ConvertingFuelType = @object.m_TypeIdentifier;
		SetState(State.ConvertingFuel);
	}

	private void EndAddFuel(AFO Info)
	{
		Info.m_Object.StopUsing();
		AudioManager.Instance.StartEvent("BuildingIngredientAdd", this);
		m_PlaySound = AudioManager.Instance.StartEvent("BuildingStationarySteamEngineConverting", this, Remember: true);
	}

	protected void AbortAddFuel(AFO Info)
	{
		SetState(State.Idle);
	}

	private ActionType GetActionFromFuel(AFO Info)
	{
		Info.m_StartAction = StartAddFuel;
		Info.m_EndAction = EndAddFuel;
		Info.m_AbortAction = AbortAddFuel;
		Info.m_FarmerState = Farmer.State.Adding;
		if (m_State != 0)
		{
			return ActionType.Fail;
		}
		if ((int)BurnableFuel.GetFuelEnergy(Info.m_ObjectType) > m_EnergyCapacity - m_Energy)
		{
			return ActionType.Fail;
		}
		return ActionType.AddResource;
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		ObjectType objectType = Info.m_ObjectType;
		if (Info.m_ActionType == AFO.AT.Secondary)
		{
			if (ToolBucket.GetIsTypeBucket(objectType))
			{
				return GetActionFromBucket(Info);
			}
			if (GetIsObjectAcceptableAsFuel(objectType))
			{
				return GetActionFromFuel(Info);
			}
		}
		return ActionType.Total;
	}

	public static bool GetIsObjectAcceptableAsFuel(ObjectType NewType)
	{
		return BurnableFuel.GetIsBurnableFuel(NewType);
	}

	public override void SetBlueprint(bool Blueprint, bool Register = false)
	{
		base.SetBlueprint(Blueprint, Register);
		m_FireLight.SetActive(!Blueprint);
		m_Smoke.gameObject.SetActive(!Blueprint);
	}

	private void UpdateConvertingFuelAnimation()
	{
		if ((int)(m_StateTimer * 60f) % 20 < 10)
		{
			m_ModelRoot.transform.localScale = new Vector3(0.9f, 1.3f, 0.9f);
		}
		else
		{
			m_ModelRoot.transform.localScale = new Vector3(1f, 1f, 1f);
		}
	}

	private void UpdateConvertingFuel()
	{
		UpdateConvertingFuelAnimation();
		if (m_StateTimer > 0.5f)
		{
			m_StateTimer = 0f;
			AddFuel(m_ConvertingFuelType);
			AudioManager.Instance.StopEvent(m_PlaySound);
			SetState(State.Idle);
		}
	}

	private float GetSpeed()
	{
		if (m_EnergyUsedTimer > 0f)
		{
			return 1f;
		}
		return 0.2f;
	}

	private bool GetRunning()
	{
		if ((m_Energy > 0 && m_Water > 0f) || m_EnergyUsedTimer > 0f)
		{
			return true;
		}
		return false;
	}

	private void UpdateSmoke()
	{
		if (GetRunning())
		{
			if (!m_Smoke.m_Particles.isPlaying)
			{
				m_Smoke.Play();
			}
			float speed = GetSpeed();
			ParticleSystem.EmissionModule emission = m_Smoke.m_Particles.emission;
			emission.rateOverTime = speed * 4f;
		}
		else if (m_Smoke.m_Particles.isPlaying)
		{
			m_Smoke.Stop();
		}
	}

	private void UpdateFire()
	{
		if (GetRunning())
		{
			m_FireLight.SetActive(Active: true);
		}
		else
		{
			m_FireLight.SetActive(Active: false);
		}
	}

	private void UpdateFlyWheel()
	{
		if (GetRunning())
		{
			float speed = GetSpeed();
			m_Flywheel.transform.localRotation = m_Flywheel.transform.localRotation * Quaternion.Euler(0f, -720f * TimeManager.Instance.m_NormalDelta * speed, 0f);
			float num = (m_Flywheel.transform.position.y - m_ConnectingRodPoint.transform.position.y) / m_CrankRadius;
			m_Beam.transform.localRotation = Quaternion.Euler(0f, 0f, -10f * num) * Quaternion.Euler(-90f, 0f, 180f);
			m_ConnectingRod.transform.LookAt(m_ConnectingRodPoint.transform.position);
			m_ConnectingRod.transform.localRotation = m_ConnectingRod.transform.localRotation * Quaternion.Euler(0f, 90f, 0f);
			m_PistonRod.transform.LookAt(m_PistonRodPoint.transform.position);
			m_PistonRod.transform.localRotation = m_PistonRod.transform.localRotation * Quaternion.Euler(-180f, 0f, 0f);
		}
	}

	private void UpdateSound()
	{
		if (GetRunning())
		{
			if (m_RunningSound == null)
			{
				m_RunningSound = AudioManager.Instance.StartEvent("BuildingStationarySteamEngineRunning", this, Remember: true);
			}
			if (m_RunningSound != null && m_RunningSound.m_Result != null)
			{
				float num = (GetSpeed() - 0.2f) / 0.8f;
				m_RunningSound.m_Result.ActingVariation.VarAudio.pitch = 0.5f + num * 0.5f;
			}
		}
		else if (m_RunningSound != null)
		{
			AudioManager.Instance.StopEvent(m_RunningSound);
			m_RunningSound = null;
		}
	}

	protected new void Update()
	{
		base.Update();
		State state = m_State;
		if (state == State.ConvertingFuel)
		{
			UpdateConvertingFuel();
		}
		if (TimeManager.Instance.m_NormalTimeEnabled && !m_WallMissing && !m_FloorMissing)
		{
			UpdateSmoke();
			UpdateFire();
			UpdateFlyWheel();
			UpdateSound();
		}
	}
}
