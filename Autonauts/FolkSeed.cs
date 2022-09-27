using SimpleJSON;

public class FolkSeed : Holdable
{
	public override void Restart()
	{
		base.Restart();
		if (!ObjectTypeList.m_Loading)
		{
			CollectionManager.Instance.AddCollectable("FolkSeed", this);
		}
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		CollectionManager.Instance.AddCollectable("FolkSeed", this);
	}
}
