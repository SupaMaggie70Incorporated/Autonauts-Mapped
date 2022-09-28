using SimpleJSON;

public class CropWheat : Crop
{
	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("CropWheat", m_TypeIdentifier);
		ObjectTypeList.Instance.SetTypeSleep(ObjectType.CropWheat);
		ModelManager.Instance.AddModel("Models/Crops/CropWheatCultivated", ObjectType.CropWheat);
	}

	public override void Restart()
	{
		base.Restart();
		if (!ObjectTypeList.m_Loading)
		{
			CollectionManager.Instance.AddCollectable("CropWheat", this);
		}
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		base.StopUsing(AndDestroy);
		if ((bool)FailSafeManager.Instance)
		{
			FailSafeManager.Instance.CropWheatRemove(m_TileCoord);
		}
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		CollectionManager.Instance.AddCollectable("CropWheat", this);
	}

	protected override void UpdateWorldCreated()
	{
		string text = "CropWheatCultivated";
		if (m_WorldCreated)
		{
			text = "CropWheat";
		}
		LoadNewModel("Models/Crops/" + text);
		base.UpdateWorldCreated();
	}

	public override void Cut(bool Create)
	{
		if (Create)
		{
			TileCoordObject.SpawnObjects(m_TileCoord, ObjectType.Wheat, m_Yield, m_Yield, AroundTile: false);
			AudioManager.Instance.StartEvent("ObjectCreated", this);
		}
		base.Cut(Create);
	}

	protected override void EndScythe(AFO Info)
	{
		base.EndScythe(Info);
		FailSafeManager.Instance.CropWheatCut();
	}
}
