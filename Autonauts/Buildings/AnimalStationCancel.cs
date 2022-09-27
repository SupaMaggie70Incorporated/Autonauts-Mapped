using UnityEngine;

public class AnimalStationCancel : BuildingCancel
{
	private HoldableRollover m_Rollover;

	public override void SetBuilding(Building NewBuilding)
	{
		base.SetBuilding(NewBuilding);
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Rollovers/HoldableRollover", typeof(GameObject));
		m_Rollover = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, base.transform).GetComponent<HoldableRollover>();
		m_Rollover.SetTarget(NewBuilding);
		m_Rollover.ForceOpen();
		AddObjectToPanel(m_Rollover.gameObject);
	}

	public override void OnCancelSelected(BaseGadget NewGadget)
	{
		Disengage();
		m_Building.GetComponent<AnimalStation>().StopAll();
		GameStateManager.Instance.PopState();
	}

	public override void OnCancelEnter(BaseGadget NewGadget)
	{
		base.OnCancelEnter(NewGadget);
		HudManager.Instance.ActivateWarningRollover(Activate: true, "AnimalStationCancelButton");
	}

	public override void OnCancelExit(BaseGadget NewGadget)
	{
		base.OnCancelExit(NewGadget);
		HudManager.Instance.ActivateWarningRollover(Activate: false);
	}
}
