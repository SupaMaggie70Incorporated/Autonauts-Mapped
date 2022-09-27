public class WorkerHeadMk3 : WorkerHead
{
	public static bool GetIsTypeHeadMk3(ObjectType NewType)
	{
		if (NewType != ObjectType.WorkerHeadMk3 && NewType != ObjectType.WorkerHeadMk3Variant1 && NewType != ObjectType.WorkerHeadMk3Variant2 && NewType != ObjectType.WorkerHeadMk3Variant3)
		{
			return NewType == ObjectType.WorkerHeadMk3Variant4;
		}
		return true;
	}
}
