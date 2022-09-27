public class WorkerFrameMk1 : WorkerFrame
{
	public static bool GetIsTypeFrameMk1(ObjectType NewType)
	{
		if (NewType != ObjectType.WorkerFrameMk1 && NewType != ObjectType.WorkerFrameMk1Variant1 && NewType != ObjectType.WorkerFrameMk1Variant2 && NewType != ObjectType.WorkerFrameMk1Variant3 && NewType != ObjectType.WorkerFrameMk1Variant4)
		{
			return NewType == ObjectType.WorkerFrameROB;
		}
		return true;
	}
}
