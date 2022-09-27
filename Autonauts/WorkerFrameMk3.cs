public class WorkerFrameMk3 : WorkerFrame
{
	public static bool GetIsTypeFrameMk3(ObjectType NewType)
	{
		if (NewType != ObjectType.WorkerFrameMk3 && NewType != ObjectType.WorkerFrameMk3Variant1 && NewType != ObjectType.WorkerFrameMk3Variant2 && NewType != ObjectType.WorkerFrameMk3Variant3)
		{
			return NewType == ObjectType.WorkerFrameMk3Variant4;
		}
		return true;
	}
}
