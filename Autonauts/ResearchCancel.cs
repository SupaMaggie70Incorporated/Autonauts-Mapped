using UnityEngine;

public class ResearchCancel : BuildingCancel
{
	private ResearchRollover m_Rollover;

	protected new void Start()
	{
		m_Rollover.SetResearchTarget(m_Building.GetComponent<ResearchStation>());
		m_Rollover.ForceOpen();
		base.Start();
	}

	public override void SetBuilding(Building NewBuilding)
	{
		base.SetBuilding(NewBuilding);
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Rollovers/ResearchRollover", typeof(GameObject));
		m_Rollover = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, base.transform).GetComponent<ResearchRollover>();
		AddObjectToPanel(m_Rollover.gameObject);
	}

	public override void OnCancelSelected(BaseGadget NewGadget)
	{
		Disengage();
		if ((bool)m_Building.GetComponent<ResearchStation>())
		{
			m_Building.GetComponent<ResearchStation>().CancelResearch();
		}
		GameStateManager.Instance.PopState();
	}

	public override void OnCancelEnter(BaseGadget NewGadget)
	{
		base.OnCancelEnter(NewGadget);
		HudManager.Instance.ActivateWarningRollover(Activate: true, "ResearchCancelButton");
	}

	public override void OnCancelExit(BaseGadget NewGadget)
	{
		base.OnCancelExit(NewGadget);
		HudManager.Instance.ActivateWarningRollover(Activate: false);
	}
}
