public class WorkerPart : Holdable
{
	public enum Type
	{
		Drive,
		Frame,
		Head,
		Total
	}

	public static bool GetIsTypePart(ObjectType NewType)
	{
		if (WorkerDrive.GetIsTypeDrive(NewType) || WorkerFrame.GetIsTypeFrame(NewType) || WorkerHead.GetIsTypeHead(NewType))
		{
			return true;
		}
		return false;
	}

	public virtual Type GetPartType()
	{
		return Type.Total;
	}
}
