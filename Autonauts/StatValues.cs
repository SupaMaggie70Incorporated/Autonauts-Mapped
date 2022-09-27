using SimpleJSON;

public class StatValues
{
	public enum Type
	{
		Count,
		Rate,
		Time,
		Total
	}

	public StatsManager.Stat m_ID;

	public Type m_Type;

	public StatValues(StatsManager.Stat ID, Type NewType)
	{
		m_ID = ID;
		m_Type = NewType;
	}

	public virtual string GetAsString()
	{
		return "";
	}

	public virtual void Add()
	{
	}

	public virtual void Save(JSONNode Node)
	{
	}

	public virtual void Load(JSONNode Node)
	{
	}

	public virtual void Update()
	{
	}
}
