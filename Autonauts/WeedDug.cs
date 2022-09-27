public class WeedDug : Holdable
{
	public override void SendAction(ActionInfo Info)
	{
		ActionType action = Info.m_Action;
		if (action == ActionType.SpawnEnd)
		{
			AudioManager.Instance.StartEvent("ObjectLand", this);
		}
		base.SendAction(Info);
	}
}
