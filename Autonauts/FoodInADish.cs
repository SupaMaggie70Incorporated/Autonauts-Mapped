using SimpleJSON;

public class FoodInADish : Food
{
	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		JSONUtils.Set(Node, "Used", m_UsageCount);
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		m_UsageCount = JSONUtils.GetAsInt(Node, "Used", 0);
	}
}
