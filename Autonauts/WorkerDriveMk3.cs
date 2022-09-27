public class WorkerDriveMk3 : WorkerDrive
{
	public static bool GetIsTypeDriveMk3(ObjectType NewType)
	{
		if (NewType != ObjectType.WorkerDriveMk3 && NewType != ObjectType.WorkerDriveMk3Variant1 && NewType != ObjectType.WorkerDriveMk3Variant2 && NewType != ObjectType.WorkerDriveMk3Variant3)
		{
			return NewType == ObjectType.WorkerDriveMk3Variant4;
		}
		return true;
	}
}
