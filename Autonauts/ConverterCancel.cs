using UnityEngine;

public class ConverterCancel : BuildingCancel
{
	private ConverterRollover m_Rollover;

	public override void SetBuilding(Building NewBuilding)
	{
		base.SetBuilding(NewBuilding);
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Rollovers/ConverterRollover", typeof(GameObject));
		m_Rollover = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, base.transform).GetComponent<ConverterRollover>();
		m_Rollover.SetConverterTarget(NewBuilding.GetComponent<Converter>());
		m_Rollover.ForceOpen();
		AddObjectToPanel(m_Rollover.gameObject);
	}

	public void ConfirmCancel()
	{
		Disengage();
		m_Building.GetComponent<Converter>().CancelBuild();
		GameStateManager.Instance.PopState();
	}

	public override void OnCancelSelected(BaseGadget NewGadget)
	{
		if (m_Building.m_TypeIdentifier == ObjectType.ConverterFoundation)
		{
			if (!m_Building.GetComponent<ConverterFoundation>().CanInstantDelete())
			{
				GameStateManager.Instance.PushState(GameStateManager.State.Confirm);
				GameStateManager.Instance.GetCurrentState().GetComponent<GameStateConfirm>().SetConfirm(ConfirmCancel, "ConfirmCancelBuild", "ConfirmCancelBuildDescription");
			}
			else
			{
				ConfirmCancel();
			}
		}
		else
		{
			ConfirmCancel();
		}
	}

	public override void OnCancelEnter(BaseGadget NewGadget)
	{
		base.OnCancelEnter(NewGadget);
		if (ResearchStation.GetIsTypeResearchStation(m_Building.m_TypeIdentifier))
		{
			HudManager.Instance.ActivateWarningRollover(Activate: true, "ResearchCancelButton");
		}
		else if (m_Building.m_TypeIdentifier == ObjectType.ConverterFoundation && m_Building.GetComponent<ConverterFoundation>().m_NewBuilding.m_BuildingToUpgradeFrom != ObjectTypeList.m_Total)
		{
			HudManager.Instance.ActivateWarningRollover(Activate: true, "UpgradeCancelButton");
		}
		else
		{
			HudManager.Instance.ActivateWarningRollover(Activate: true, "ConverterCancelButton");
		}
	}

	public override void OnCancelExit(BaseGadget NewGadget)
	{
		base.OnCancelExit(NewGadget);
		HudManager.Instance.ActivateWarningRollover(Activate: false);
	}
}
