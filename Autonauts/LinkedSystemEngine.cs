using SimpleJSON;
using UnityEngine;

public class LinkedSystemEngine : Building
{
	protected enum State
	{
		Idle,
		ConvertingFuel,
		Total
	}

	protected State m_State;

	protected float m_StateTimer;

	[HideInInspector]
	public int m_Energy;

	[HideInInspector]
	public int m_EnergyCapacity;

	[HideInInspector]
	public float m_EnergyUsedTimer;

	private GameObject m_Pulley;

	protected int m_PulleySide;

	private float m_PulleyFlashTimer;

	private MeshRenderer[] m_PulleyMaterials;

	public static bool GetIsTypeLinkedSystemEngine(ObjectType NewType)
	{
		if (NewType == ObjectType.Windmill || NewType == ObjectType.GiantWaterWheel || NewType == ObjectType.StationaryEngine)
		{
			return true;
		}
		return false;
	}

	public override void Restart()
	{
		base.Restart();
		m_EnergyUsedTimer = 0f;
		SetState(State.Idle);
	}

	public override void PostCreate()
	{
		base.PostCreate();
		CreatePulley();
	}

	private void CreatePulley()
	{
		Transform newParent = FindNode("BeltPoint").transform;
		m_Pulley = ModelManager.Instance.Instantiate(ObjectTypeList.m_Total, "Models/Special/MechanicalPulley", newParent, RandomVariants: false);
		m_Pulley.transform.localPosition = default(Vector3);
		m_Pulley.transform.localRotation = Quaternion.Euler(-90f, 0f, 180f);
		m_PulleyMaterials = m_Pulley.GetComponentsInChildren<MeshRenderer>();
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		JSONUtils.Set(Node, "State", (int)m_State);
		JSONUtils.Set(Node, "Energy", m_Energy);
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		m_Energy = JSONUtils.GetAsInt(Node, "Energy", 0);
		SetState((State)JSONUtils.GetAsInt(Node, "State", 0));
	}

	public int GetPulleySide()
	{
		return (m_PulleySide + m_Rotation) % 4;
	}

	public virtual bool IsEnergyReady()
	{
		if (m_Energy == 0)
		{
			return false;
		}
		return true;
	}

	public virtual float GetEnergy()
	{
		return m_Energy;
	}

	public void AddEnergy(int Amount)
	{
		m_Energy += Amount;
		if (m_Energy > m_EnergyCapacity)
		{
			m_Energy = m_EnergyCapacity;
		}
		if (m_LinkedSystem != null)
		{
			((LinkedSystemMechanical)m_LinkedSystem).UpdateConverters();
		}
	}

	public virtual void UseEnergy(int Amount)
	{
		m_Energy -= Amount;
		if (m_Energy < 0)
		{
			m_Energy = 0;
		}
		m_EnergyUsedTimer = 5f;
		if (m_LinkedSystem != null)
		{
			((LinkedSystemMechanical)m_LinkedSystem).UpdateConverters();
		}
	}

	public override void SendAction(ActionInfo Info)
	{
		base.SendAction(Info);
		ActionType action = Info.m_Action;
		if ((uint)(action - 41) <= 1u)
		{
			RefreshConnection();
			UpdatePulleyState();
		}
	}

	protected void SetState(State NewState)
	{
		m_State = NewState;
		m_StateTimer = 0f;
	}

	private void UpdatePulleyState()
	{
		if (!(m_Pulley == null))
		{
			if (m_LinkedSystem == null || m_LinkedSystem.m_Buildings.Count == 1)
			{
				m_Pulley.SetActive(value: true);
			}
			else
			{
				m_Pulley.SetActive(value: false);
			}
		}
	}

	public override void SetLinkedSystem(LinkedSystem NewSystem)
	{
		base.SetLinkedSystem(NewSystem);
		UpdatePulleyState();
	}

	private void UpdatePulley()
	{
		UpdatePulleyState();
		if (m_Pulley.gameObject.activeSelf)
		{
			m_PulleyFlashTimer += TimeManager.Instance.m_NormalDelta;
			Material sharedMaterial = MaterialManager.Instance.m_MaterialPulleyOff;
			if ((int)(m_PulleyFlashTimer * 60f) % 12 < 6)
			{
				sharedMaterial = MaterialManager.Instance.m_MaterialPulleyOn;
			}
			MeshRenderer[] pulleyMaterials = m_PulleyMaterials;
			for (int i = 0; i < pulleyMaterials.Length; i++)
			{
				pulleyMaterials[i].sharedMaterial = sharedMaterial;
			}
		}
	}

	protected void Update()
	{
		m_StateTimer += TimeManager.Instance.m_NormalDelta;
		if (m_EnergyUsedTimer > 0f)
		{
			m_EnergyUsedTimer -= TimeManager.Instance.m_NormalDelta;
			if (m_EnergyUsedTimer < 0f)
			{
				m_EnergyUsedTimer = 0f;
			}
		}
		UpdatePulley();
	}
}
