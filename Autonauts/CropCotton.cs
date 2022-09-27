using SimpleJSON;

public class CropCotton : Crop
{
	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("CropCotton", m_TypeIdentifier);
		ObjectTypeList.Instance.SetTypeSleep(ObjectType.CropCotton);
		ModelManager.Instance.AddModel("Models/Crops/CropCottonCultivated", ObjectType.CropCotton);
	}

	public override void Restart()
	{
		base.Restart();
		if (!ObjectTypeList.m_Loading)
		{
			CollectionManager.Instance.AddCollectable("CropCotton", this);
		}
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		base.StopUsing(AndDestroy);
		if ((bool)FailSafeManager.Instance)
		{
			FailSafeManager.Instance.CropCottonRemove(m_TileCoord);
		}
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		CollectionManager.Instance.AddCollectable("CropCotton", this);
	}

	protected override void UpdateWorldCreated()
	{
		string text = "CropCottonCultivated";
		if (m_WorldCreated)
		{
			text = "CropCotton";
		}
		LoadNewModel("Models/Crops/" + text);
		base.UpdateWorldCreated();
	}

	public override void Cut(bool Create)
	{
		if (Create)
		{
			TileCoordObject.SpawnObjects(m_TileCoord, ObjectType.CottonBall, m_Yield, m_Yield, AroundTile: false);
			AudioManager.Instance.StartEvent("ObjectCreated", this);
		}
		base.Cut(Create);
	}

	protected override void EndScythe(AFO Info)
	{
		base.EndScythe(Info);
		FailSafeManager.Instance.CropCottonCut();
	}
}
