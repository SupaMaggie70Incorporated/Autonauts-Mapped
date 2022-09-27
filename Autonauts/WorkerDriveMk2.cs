public class WorkerDriveMk2 : WorkerDrive
{
	public static bool GetIsTypeDriveMk2(ObjectType NewType)
	{
		if (NewType != ObjectType.WorkerDriveMk2 && NewType != ObjectType.WorkerDriveMk2Variant1 && NewType != ObjectType.WorkerDriveMk2Variant2 && NewType != ObjectType.WorkerDriveMk2Variant3)
		{
			return NewType == ObjectType.WorkerDriveMk2Variant4;
		}
		return true;
	}
}
