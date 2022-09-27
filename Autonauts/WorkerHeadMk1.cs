public class WorkerHeadMk1 : WorkerHead
{
	public static bool GetIsTypeHeadMk1(ObjectType NewType)
	{
		if (NewType != ObjectType.WorkerHeadMk1 && NewType != ObjectType.WorkerHeadMk1Variant1 && NewType != ObjectType.WorkerHeadMk1Variant2 && NewType != ObjectType.WorkerHeadMk1Variant3 && NewType != ObjectType.WorkerHeadMk1Variant4)
		{
			return NewType == ObjectType.WorkerHeadROB;
		}
		return true;
	}
}
