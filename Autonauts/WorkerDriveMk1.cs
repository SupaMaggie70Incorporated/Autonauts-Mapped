public class WorkerDriveMk1 : WorkerDrive
{
	public static bool GetIsTypeDriveMk1(ObjectType NewType)
	{
		if (NewType != ObjectType.WorkerDriveMk1 && NewType != ObjectType.WorkerDriveMk1Variant1 && NewType != ObjectType.WorkerDriveMk1Variant2 && NewType != ObjectType.WorkerDriveMk1Variant3 && NewType != ObjectType.WorkerDriveMk1Variant4)
		{
			return NewType == ObjectType.WorkerDriveROB;
		}
		return true;
	}
}
