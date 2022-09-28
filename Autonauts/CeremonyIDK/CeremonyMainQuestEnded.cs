using UnityEngine;

public class CeremonyMainQuestEnded : CeremonyQuestEnded
{
	private enum State
	{
		ShowPlan,
		PanToTransmitter,
		UpgradeTransmitter,
		Total
	}

	private State m_State2;

	private void Awake()
	{
		AudioManager.Instance.StartEvent("CeremonyMainQuestEnded");
		SetState(State.ShowPlan);
	}

	private void SetState(State NewState)
	{
		m_State2 = NewState;
	}

	protected override void EndPlans()
	{
		End();
	}

	private void End()
	{
		Object.Destroy(base.gameObject);
		CeremonyManager.Instance.CeremonyEnded();
	}
}
