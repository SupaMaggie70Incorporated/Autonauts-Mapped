using System;
using SimpleJSON;
using UnityEngine;

public class Ziggurat : LinkedSystemConverter
{
	private enum ZigguratState
	{
		Idle,
		Incinerating,
		Total
	}

	private ZigguratState m_ZigguratState;

	private float m_ZigguratStateTimer;

	private PlaySound m_PlaySound;

	private MyParticles m_Particles;

	private int m_ObjectUID;

	private Action<AFO> m_EndAddAnimal;

	private Action<AFO> m_StartAddAnimal;

	public override void Restart()
	{
		base.Restart();
		if (!ObjectTypeList.m_Loading)
		{
			CollectionManager.Instance.AddCollectable("Ziggurat", this);
		}
		SetDimensions(new TileCoord(-3, -6), new TileCoord(3, 0), new TileCoord(0, 1));
		SetSpawnPoint(new TileCoord(0, -1));
		SetState(ZigguratState.Idle);
		HideSpawnModel();
		m_PulleySide = 1;
	}

	protected new void Awake()
	{
		base.Awake();
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_EndAddAnimal = EndAddAnimal;
		m_StartAddAnimal = StartAddAnimal;
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		DestroyTarget();
		base.StopUsing(AndDestroy);
	}

	private void DestroyTarget()
	{
		if (m_ObjectUID != 0)
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(m_ObjectUID, ErrorCheck: false);
			if ((bool)objectFromUniqueID)
			{
				objectFromUniqueID.StopUsing();
			}
			m_ObjectUID = 0;
		}
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		CollectionManager.Instance.AddCollectable("Ziggurat", this);
	}

	private void StartAddAnimal(AFO Info)
	{
		Info.m_Object.SendAction(new ActionInfo(ActionType.BeingHeld, m_TileCoord, this));
		Info.m_Object.GetComponent<Savable>().SetIsSavable(IsSavable: false);
		m_ObjectUID = Info.m_Object.m_UniqueID;
		Info.m_Object.transform.position = m_IngredientsRoot.position;
		int resultEnergyRequired = GetResultEnergyRequired();
		((LinkedSystemMechanical)m_LinkedSystem).UseEnergy(resultEnergyRequired);
	}

	private void EndAddAnimal(AFO Info)
	{
		Actionable @object = Info.m_Object;
		if ((bool)@object)
		{
			@object.transform.rotation = base.transform.rotation;
		}
		SetState(ZigguratState.Incinerating);
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		Info.m_StartAction = m_StartAddAnimal;
		Info.m_EndAction = m_EndAddAnimal;
		Info.m_FarmerState = Farmer.State.Adding;
		if (m_State != 0)
		{
			return ActionType.Fail;
		}
		if (!CanAcceptIngredient(Info.m_ObjectType))
		{
			return ActionType.Fail;
		}
		if (m_LinkedSystem == null)
		{
			return ActionType.Fail;
		}
		int resultEnergyRequired = GetResultEnergyRequired();
		if (!((LinkedSystemMechanical)m_LinkedSystem).GetIsEnergyAvailable(resultEnergyRequired))
		{
			return ActionType.Fail;
		}
		return ActionType.AddResource;
	}

	public override bool CanAcceptIngredient(ObjectType NewType)
	{
		if (NewType == ObjectType.Folk)
		{
			if (!QuestManager.Instance.GetIsLastLevelActive())
			{
				return true;
			}
			return false;
		}
		if (Animal.GetIsTypeAnimal(NewType) || NewType == ObjectType.BeesNest)
		{
			return true;
		}
		return base.CanAcceptIngredient(NewType);
	}

	private void SetState(ZigguratState NewState)
	{
		ZigguratState zigguratState = m_ZigguratState;
		if (zigguratState == ZigguratState.Incinerating)
		{
			StopIncinerating();
		}
		m_ZigguratState = NewState;
		m_ZigguratStateTimer = 0f;
		zigguratState = m_ZigguratState;
		if (zigguratState == ZigguratState.Incinerating)
		{
			StartIncinerating();
		}
		ModManager.Instance.CheckBuildingStateChangedCallback(m_UniqueID, m_ZigguratState.ToString());
	}

	private void StartIncinerating()
	{
		m_PlaySound = AudioManager.Instance.StartEvent("BuildingZigguratTransform", this, Remember: true);
		m_Particles = ParticlesManager.Instance.CreateParticles("ZigguratTransform", m_IngredientsRoot.position + new Vector3(0f, 1f, 0f), Quaternion.Euler(-90f, 0f, 0f));
	}

	private void UpdateIncineratingAnimation()
	{
	}

	private void StopIncinerating()
	{
		m_Particles.Stop();
		ParticlesManager.Instance.DestroyParticles(m_Particles, WaitUntilNoParticles: true);
		AudioManager.Instance.StopEvent(m_PlaySound);
		AudioManager.Instance.StartEvent("BuildingZigguratDone", this);
	}

	private void UpdateIncinerating()
	{
		UpdateIncineratingAnimation();
		if (m_ZigguratStateTimer > m_ConversionDelay)
		{
			m_ZigguratStateTimer = 0f;
			m_ModelRoot.transform.localScale = new Vector3(1f, 1f, 1f);
			DestroyTarget();
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.AnimalBird, m_IngredientsRoot.position, Quaternion.Euler(0f, UnityEngine.Random.Range(0, 360), 0f));
			baseClass.WorldCreated();
			baseClass.GetComponent<AnimalBird>().FlyOutOfWorld();
			SetState(ZigguratState.Idle);
		}
	}

	protected new void Update()
	{
		UpdatePulley();
		ZigguratState zigguratState = m_ZigguratState;
		if (zigguratState == ZigguratState.Incinerating)
		{
			UpdateIncinerating();
		}
		m_ZigguratStateTimer += TimeManager.Instance.m_NormalDelta;
	}
}
