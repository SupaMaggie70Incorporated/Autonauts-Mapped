public class WorkerHeadMk2 : WorkerHead
{
	public static bool GetIsTypeHeadMk2(ObjectType NewType)
	{
		if (NewType != ObjectType.WorkerHeadMk2 && NewType != ObjectType.WorkerHeadMk2Variant1 && NewType != ObjectType.WorkerHeadMk2Variant2 && NewType != ObjectType.WorkerHeadMk2Variant3)
		{
			return NewType == ObjectType.WorkerHeadMk2Variant4;
		}
		return true;
	}
}
