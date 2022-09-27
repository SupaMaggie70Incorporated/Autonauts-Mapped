public class WorkerHead : WorkerPart
{
	public static bool GetIsTypeHead(ObjectType NewType)
	{
		if (!WorkerHeadMk0.GetIsTypeHeadMk0(NewType) && !WorkerHeadMk1.GetIsTypeHeadMk1(NewType) && !WorkerHeadMk2.GetIsTypeHeadMk2(NewType))
		{
			return WorkerHeadMk3.GetIsTypeHeadMk3(NewType);
		}
		return true;
	}

	public override Type GetPartType()
	{
		return Type.Head;
	}
}
