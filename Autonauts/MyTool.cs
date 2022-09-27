using UnityEngine;

public class MyTool : Holdable
{
	public enum Type
	{
		Axe,
		Basket,
		Broom,
		Bucket,
		Chisel,
		Dredger,
		FishingRod,
		Flail,
		Hoe,
		Mallet,
		Paddle,
		Pick,
		Pitchfork,
		Scythe,
		Shears,
		Shovel,
		Torch,
		WateringCan,
		Net,
		Blade,
		Total
	}

	public static Type GetType(ObjectType NewType)
	{
		if (ModManager.Instance.ModToolClass.IsItCustomType(NewType))
		{
			Type value = Type.Total;
			ModManager.Instance.ModToolClass.ToolBaseType.TryGetValue(NewType, out value);
			return value;
		}
		switch (NewType)
		{
		case ObjectType.ToolShovelStone:
		case ObjectType.ToolShovel:
			return Type.Shovel;
		case ObjectType.ToolHoeStone:
		case ObjectType.ToolHoe:
			return Type.Hoe;
		case ObjectType.ToolAxeStone:
		case ObjectType.ToolAxe:
			return Type.Axe;
		case ObjectType.ToolScytheStone:
		case ObjectType.ToolScythe:
			return Type.Scythe;
		case ObjectType.ToolPickStone:
		case ObjectType.ToolPick:
			return Type.Pick;
		case ObjectType.ToolBucketCrude:
		case ObjectType.ToolBucket:
		case ObjectType.ToolBucketMetal:
			return Type.Bucket;
		case ObjectType.ToolMallet:
			return Type.Mallet;
		case ObjectType.ToolChiselCrude:
		case ObjectType.ToolChisel:
			return Type.Chisel;
		case ObjectType.ToolFlailCrude:
		case ObjectType.ToolFlail:
			return Type.Flail;
		case ObjectType.ToolShears:
			return Type.Shears;
		case ObjectType.ToolWateringCan:
			return Type.WateringCan;
		case ObjectType.ToolFishingStick:
		case ObjectType.ToolFishingRod:
		case ObjectType.ToolFishingRodGood:
			return Type.FishingRod;
		case ObjectType.ToolBroom:
			return Type.Broom;
		case ObjectType.ToolPitchfork:
			return Type.Pitchfork;
		case ObjectType.ToolTorchCrude:
			return Type.Torch;
		case ObjectType.ToolDredgerCrude:
			return Type.Dredger;
		case ObjectType.ToolNetCrude:
		case ObjectType.ToolNet:
			return Type.Net;
		case ObjectType.ToolBlade:
			return Type.Blade;
		default:
			return Type.Total;
		}
	}

	public static string GetTypeName(ObjectType NewType)
	{
		Type type = GetType(NewType);
		if (type == Type.Total)
		{
			return "";
		}
		string text = TextManager.Instance.Get("ToolType" + type);
		return "(" + text + ")";
	}

	public static bool GetIsTypeTool(ObjectType NewType)
	{
		if (GetType(NewType) != Type.Total)
		{
			return true;
		}
		return false;
	}

	public static bool GetIsTypeSpecialTool(ObjectType NewType)
	{
		if (NewType == ObjectType.Stick || NewType == ObjectType.Rock || NewType == ObjectType.RockSharp)
		{
			return true;
		}
		return false;
	}

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("Tool", m_TypeIdentifier);
	}

	public override void Restart()
	{
		base.Restart();
		base.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
	}

	protected override void Used()
	{
		AudioManager.Instance.StartEvent("ToolBroken", this);
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
	}

	protected override void ActionDropped(Actionable PreviousHolder, TileCoord DropLocation)
	{
		base.ActionDropped(PreviousHolder, DropLocation);
		base.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
	}
}
