public class WorkerDrive : WorkerPart
{
	public static bool GetIsTypeDrive(ObjectType NewType)
	{
		if (!WorkerDriveMk0.GetIsTypeDriveMk0(NewType) && !WorkerDriveMk1.GetIsTypeDriveMk1(NewType) && !WorkerDriveMk2.GetIsTypeDriveMk2(NewType))
		{
			return WorkerDriveMk3.GetIsTypeDriveMk3(NewType);
		}
		return true;
	}

	public static bool GetIsTypeVibraty(ObjectType NewType)
	{
		if (NewType == ObjectType.WorkerDriveMk2 || NewType == ObjectType.WorkerDriveMk2Variant1 || WorkerDriveMk3.GetIsTypeDriveMk3(NewType))
		{
			return false;
		}
		return true;
	}

	public override Type GetPartType()
	{
		return Type.Drive;
	}
}
