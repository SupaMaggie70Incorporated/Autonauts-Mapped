public class GameStateDrag : GameStateBase
{
	public override void UpdateState()
	{
		if (MyInputManager.m_Rewired.GetButtonDown("Quit"))
		{
			GameStateManager.Instance.PopState();
			TabWorkers.Instance.StopDrag();
			AudioManager.Instance.StartEvent("UIOptionCancelled");
		}
	}
}
