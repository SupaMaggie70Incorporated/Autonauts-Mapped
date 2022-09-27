using SimpleJSON;

public class CropCarrot : Crop
{
	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("CropCarrot", m_TypeIdentifier);
		ObjectTypeList.Instance.SetTypeSleep(ObjectType.CropCarrot);
	}

	public override void Restart()
	{
		base.Restart();
		if (!ObjectTypeList.m_Loading)
		{
			CollectionManager.Instance.AddCollectable("CropCarrot", this);
		}
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		CollectionManager.Instance.AddCollectable("CropCarrot", this);
	}

	public override void Cut(bool Create)
	{
		if (Create)
		{
			TileCoordObject.SpawnObjects(m_TileCoord, ObjectType.Carrot, m_Yield, m_Yield, AroundTile: false);
			TileCoordObject.SpawnObjects(m_TileCoord, ObjectType.CarrotSeed, 1, 2, AroundTile: false);
			AudioManager.Instance.StartEvent("ObjectCreated", this);
		}
		base.Cut(Create);
	}
}
