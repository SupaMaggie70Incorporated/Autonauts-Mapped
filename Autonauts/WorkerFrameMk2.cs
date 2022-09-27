public class WorkerFrameMk2 : WorkerFrame
{
	public static bool GetIsTypeFrameMk2(ObjectType NewType)
	{
		if (NewType != ObjectType.WorkerFrameMk2 && NewType != ObjectType.WorkerFrameMk2Variant1 && NewType != ObjectType.WorkerFrameMk2Variant2 && NewType != ObjectType.WorkerFrameMk2Variant3)
		{
			return NewType == ObjectType.WorkerFrameMk2Variant4;
		}
		return true;
	}
}
