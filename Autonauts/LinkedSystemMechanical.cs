using System.Collections.Generic;
using UnityEngine;

public class LinkedSystemMechanical : LinkedSystem
{
	private List<Building> m_Engines;

	private List<Building> m_Converters;

	public int m_TotalPotentialEnergy;

	public int m_MaxUsedEnergy;

	public float m_TotalEnergyAvailable;

	public bool m_UsingEnergy;

	private int m_LastEngineUsed;

	public Material m_BeltMaterial;

	private float m_BeltTextureOffset;

	private float m_BeltBend;

	private Quaternion m_PulleyRotation;

	private float m_Speed;

	public LinkedSystemMechanical()
		: base(SystemType.Mechanical)
	{
		m_Engines = new List<Building>();
		m_Converters = new List<Building>();
		m_TotalEnergyAvailable = 0f;
		m_TotalPotentialEnergy = 0;
		m_MaxUsedEnergy = 0;
		m_UsingEnergy = false;
		m_LastEngineUsed = 0;
		m_BeltMaterial = MaterialManager.Instance.AddMaterial("Materials/Belt");
		m_BeltTextureOffset = 0f;
		m_PulleyRotation = Quaternion.Euler(0f, 0f, 0f);
	}

	~LinkedSystemMechanical()
	{
		MaterialManager.Instance.DestroyMaterial(m_BeltMaterial);
	}

	public override bool CanAddBuilding(Building NewBuilding)
	{
		if (BeltLinkage.CanTypeConnectTo(NewBuilding.m_TypeIdentifier))
		{
			if ((bool)NewBuilding.m_ConnectedTo && m_Buildings.ContainsKey(NewBuilding.m_ConnectedTo))
			{
				return true;
			}
		}
		else if ((bool)NewBuilding.GetComponent<BeltLinkage>())
		{
			BeltLinkage component = NewBuilding.GetComponent<BeltLinkage>();
			MechanicalRod rod = component.m_Rod;
			if ((bool)rod && m_Buildings.ContainsKey(rod.m_ConnectedTo))
			{
				return true;
			}
			BeltLinkage rodConnectTo = component.m_RodConnectTo;
			if ((bool)rodConnectTo && m_Buildings.ContainsKey(rodConnectTo))
			{
				return true;
			}
			MechanicalBelt belt = component.m_Belt;
			if ((bool)belt && m_Buildings.ContainsKey(belt.m_ConnectedTo))
			{
				return true;
			}
			rodConnectTo = component.m_BeltConnectTo;
			if ((bool)rodConnectTo && m_Buildings.ContainsKey(rodConnectTo))
			{
				return true;
			}
			foreach (KeyValuePair<Building, int> building in m_Buildings)
			{
				if (building.Key.m_ConnectedTo == NewBuilding)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void UpdateConverters()
	{
		UpdateEnergyAvailable();
		foreach (Building converter in m_Converters)
		{
			converter.CheckWallsFloors();
		}
	}

	public override void AddBuilding(Building NewBuilding, int Value = 0)
	{
		base.AddBuilding(NewBuilding, Value);
		if ((bool)NewBuilding.GetComponent<LinkedSystemEngine>())
		{
			m_Engines.Add(NewBuilding);
			m_LastEngineUsed = 0;
			LinkedSystemEngine component = NewBuilding.GetComponent<LinkedSystemEngine>();
			m_TotalPotentialEnergy += component.m_EnergyCapacity;
		}
		else if ((bool)NewBuilding.GetComponent<LinkedSystemConverter>())
		{
			m_Converters.Add(NewBuilding);
			m_MaxUsedEnergy += NewBuilding.GetComponent<LinkedSystemConverter>().GetResultEnergyRequired();
		}
		UpdateConverters();
	}

	public override void RemoveBuilding(Building NewBuilding)
	{
		base.RemoveBuilding(NewBuilding);
		if ((bool)NewBuilding.GetComponent<LinkedSystemEngine>())
		{
			m_Engines.Remove(NewBuilding);
			m_LastEngineUsed = 0;
			LinkedSystemEngine component = NewBuilding.GetComponent<LinkedSystemEngine>();
			m_TotalPotentialEnergy -= component.m_EnergyCapacity;
		}
		else if ((bool)NewBuilding.GetComponent<LinkedSystemConverter>())
		{
			m_Converters.Remove(NewBuilding);
			m_MaxUsedEnergy -= NewBuilding.GetComponent<LinkedSystemConverter>().GetResultEnergyRequired();
		}
		UpdateConverters();
	}

	public bool GetIsEnergyAvailable(int Amount)
	{
		return m_TotalEnergyAvailable >= (float)Amount;
	}

	public bool GetIsWallMissing()
	{
		foreach (Building engine in m_Engines)
		{
			if (engine.m_WallMissing)
			{
				return true;
			}
		}
		return false;
	}

	public bool GetIsFloorMissing()
	{
		foreach (Building engine in m_Engines)
		{
			if (engine.m_FloorMissing)
			{
				return true;
			}
		}
		return false;
	}

	public bool GetIsPowerMissing()
	{
		if (m_Engines.Count == 0)
		{
			return true;
		}
		return false;
	}

	public bool GetIsUsingEnergy()
	{
		return m_UsingEnergy;
	}

	public int GetPotentialEnergy()
	{
		return m_TotalPotentialEnergy;
	}

	public int GetMaxUsedEnergy()
	{
		return m_MaxUsedEnergy;
	}

	private void UpdateSpeed()
	{
		float targetSpeed = GetTargetSpeed();
		float num = 1f * TimeManager.Instance.m_NormalDelta;
		if (m_Speed < targetSpeed)
		{
			m_Speed += num;
			if (m_Speed >= targetSpeed)
			{
				m_Speed = targetSpeed;
			}
		}
		else if (m_Speed > targetSpeed)
		{
			m_Speed -= num;
			if (m_Speed <= targetSpeed)
			{
				m_Speed = targetSpeed;
			}
		}
	}

	public float GetTargetSpeed()
	{
		if (GetIsUsingEnergy())
		{
			return 1f;
		}
		if (GetIsEnergyAvailable(1))
		{
			return 0.2f;
		}
		return 0f;
	}

	public float GetSpeed()
	{
		return m_Speed;
	}

	private void UpdatePulleyRotation()
	{
		m_PulleyRotation *= Quaternion.Euler(0f, 0f, -720f * TimeManager.Instance.m_NormalDelta * GetSpeed());
	}

	public Quaternion GetPulleyRotation()
	{
		return m_PulleyRotation;
	}

	public void UseEnergy(int Amount)
	{
		if ((float)Amount > m_TotalEnergyAvailable)
		{
			return;
		}
		int num = m_LastEngineUsed;
		do
		{
			num++;
			if (num == m_Engines.Count)
			{
				num = 0;
			}
			LinkedSystemEngine component = m_Engines[num].GetComponent<LinkedSystemEngine>();
			if (component.IsEnergyReady())
			{
				if (Amount <= component.m_Energy)
				{
					component.UseEnergy(Amount);
					Amount = 0;
					break;
				}
				Amount -= component.m_Energy;
				component.UseEnergy(component.m_Energy);
			}
		}
		while (num != m_LastEngineUsed);
		m_LastEngineUsed = num;
		m_TotalEnergyAvailable -= Amount;
	}

	private void UpdateEnergyAvailable()
	{
		m_TotalEnergyAvailable = 0f;
		m_UsingEnergy = false;
		foreach (LinkedSystemEngine engine in m_Engines)
		{
			if (engine.m_EnergyUsedTimer > 0f)
			{
				m_UsingEnergy = true;
			}
			if (engine.IsEnergyReady())
			{
				m_TotalEnergyAvailable += engine.GetEnergy();
			}
		}
	}

	private void UpdateBeltMaterial()
	{
		m_BeltTextureOffset += TimeManager.Instance.m_NormalDelta * GetSpeed() * 8f;
		m_BeltMaterial.SetTextureOffset("_MainTex", new Vector2(m_BeltTextureOffset, 0f));
		float num = 0.05f;
		if (GetIsUsingEnergy())
		{
			num = 0.2f;
		}
		m_BeltBend += TimeManager.Instance.m_NormalDelta * GetSpeed() * 0.25f;
		float value = Mathf.Sin(m_BeltBend * 180f) * num;
		m_BeltMaterial.SetFloat("_Bend", value);
	}

	public override void Update()
	{
		UpdateSpeed();
		UpdateEnergyAvailable();
		UpdateBeltMaterial();
		UpdatePulleyRotation();
	}
}
