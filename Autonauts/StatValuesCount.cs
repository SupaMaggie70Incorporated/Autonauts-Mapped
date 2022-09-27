using SimpleJSON;

public class StatValuesCount : StatValues
{
	private int m_Count;

	public StatValuesCount(StatsManager.Stat ID)
		: base(ID, Type.Count)
	{
		m_Count = 0;
	}

	public override void Add()
	{
		m_Count++;
	}

	public override string GetAsString()
	{
		return m_Count.ToString();
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		JSONUtils.Set(Node, "Count", m_Count);
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		m_Count = JSONUtils.GetAsInt(Node, "Count", 0);
	}
}
