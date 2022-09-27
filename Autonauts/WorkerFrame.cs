public class WorkerFrame : WorkerPart
{
	public static bool GetIsTypeFrame(ObjectType NewType)
	{
		if (!WorkerFrameMk0.GetIsTypeFrameMk0(NewType) && !WorkerFrameMk1.GetIsTypeFrameMk1(NewType) && !WorkerFrameMk2.GetIsTypeFrameMk2(NewType))
		{
			return WorkerFrameMk3.GetIsTypeFrameMk3(NewType);
		}
		return true;
	}

	public override Type GetPartType()
	{
		return Type.Frame;
	}
}
