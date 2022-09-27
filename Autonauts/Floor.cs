public class Floor : Building
{
	public static bool GetIsTypeFloor(ObjectType NewType)
	{
		if (Floor2D.GetIsTypeFloor2D(NewType))
		{
			return true;
		}
		if (NewType == ObjectType.SandPath || NewType == ObjectType.StonePath || Bridge.GetIsTypeBridge(NewType) || NewType == ObjectType.RoadCrude || NewType == ObjectType.RoadGood || TrainTrack.GetIsTypeTrainTrack(NewType))
		{
			return true;
		}
		return false;
	}

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("Floor", m_TypeIdentifier);
		ObjectTypeList.Instance.SetTypeSleep(ObjectType.SandPath);
		ObjectTypeList.Instance.SetTypeSleep(ObjectType.StonePath);
		ObjectTypeList.Instance.SetTypeSleep(ObjectType.Bridge);
		ObjectTypeList.Instance.SetTypeSleep(ObjectType.BridgeStone);
		ObjectTypeList.Instance.SetTypeSleep(ObjectType.RoadGood);
		ObjectTypeList.Instance.SetTypeSleep(ObjectType.FlooringBrick);
		ObjectTypeList.Instance.SetTypeSleep(ObjectType.FlooringFlagstone);
		ObjectTypeList.Instance.SetTypeSleep(ObjectType.FlooringParquet);
		ObjectTypeList.Instance.SetTypeSleep(ObjectType.TrainTrack);
		ObjectTypeList.Instance.SetTypeSleep(ObjectType.TrainTrackCurve);
		ObjectTypeList.Instance.SetTypeSleep(ObjectType.TrainTrackPointsLeft);
		ObjectTypeList.Instance.SetTypeSleep(ObjectType.TrainTrackPointsRight);
		ObjectTypeList.Instance.SetTypeSleep(ObjectType.TrainTrackBridge);
		ObjectTypeList.Instance.SetTypeSleep(ObjectType.RoadCrude);
		ObjectTypeList.Instance.SetTypeSleep(ObjectType.FlooringCrude);
		ObjectTypeList.Instance.SetTypeSleep(ObjectType.Workshop);
		ObjectTypeList.Instance.SetTypeSleep(ObjectType.FlooringChequer);
	}

	public override void Restart()
	{
		base.Restart();
		HideAccessModel();
		Sleep();
	}

	public override bool IsSelectable()
	{
		return false;
	}

	public virtual float GetHeight()
	{
		return 0.2f;
	}

	public override object GetActionInfo(GetActionInfo Info)
	{
		GetAction action = Info.m_Action;
		_ = action - 3;
		_ = 1;
		return base.GetActionInfo(Info);
	}

	protected override void RefreshConnected()
	{
		BuildingManager.Instance.RefreshSurroundingBuildings(this);
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (Info.m_ActionType == AFO.AT.Secondary && Info.m_ObjectType != ObjectTypeList.m_Total)
		{
			Info.m_FarmerState = Farmer.State.Dropping;
			if (ObjectTypeList.Instance.GetCanDropInto(m_TypeIdentifier))
			{
				return ActionType.DropAll;
			}
			return ActionType.Fail;
		}
		return ActionType.Total;
	}

	public virtual bool CanBuildOn()
	{
		return true;
	}
}
