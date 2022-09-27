using SimpleJSON;
using UnityEngine;

public class StonePath : Floor
{
	public override void RegisterClass()
	{
		base.RegisterClass();
		ModelManager.Instance.AddModel("Models/Buildings/Floors/StonePath2", ObjectType.StonePath, RandomVariants: true);
		ModelManager.Instance.AddModel("Models/Buildings/Floors/StonePath3", ObjectType.StonePath, RandomVariants: true);
	}

	public override void Restart()
	{
		base.Restart();
		SetDimensions(new TileCoord(0, 0), new TileCoord(0, 0), new TileCoord(0, 0));
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		SetRotation(Random.Range(0, 4));
	}
}
