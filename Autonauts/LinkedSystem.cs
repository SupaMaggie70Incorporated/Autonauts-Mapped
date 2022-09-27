using System.Collections.Generic;

public class LinkedSystem
{
	public enum SystemType
	{
		Mechanical,
		Track,
		Total
	}

	public SystemType m_Type;

	public Dictionary<Building, int> m_Buildings;

	public LinkedSystem(SystemType Type)
	{
		m_Type = Type;
		m_Buildings = new Dictionary<Building, int>();
	}

	public virtual bool CanAddBuilding(Building NewBuilding)
	{
		return false;
	}

	public virtual void AddBuilding(Building NewBuilding, int Value = 0)
	{
		if (!m_Buildings.ContainsKey(NewBuilding))
		{
			m_Buildings.Add(NewBuilding, Value);
			NewBuilding.SetLinkedSystem(this);
		}
	}

	public bool GetContainsBuilding(Building NewBuilding)
	{
		return m_Buildings.ContainsKey(NewBuilding);
	}

	public virtual void RemoveBuilding(Building NewBuilding)
	{
		if (m_Buildings.ContainsKey(NewBuilding))
		{
			m_Buildings.Remove(NewBuilding);
			NewBuilding.SetLinkedSystem(null);
		}
	}

	public virtual void Update()
	{
	}

	public virtual void EndEditMode()
	{
	}
}
