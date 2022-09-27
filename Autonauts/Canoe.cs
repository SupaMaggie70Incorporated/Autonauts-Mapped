using UnityEngine;

public class Canoe : Vehicle
{
	private GameObject m_Ripple1;

	private GameObject m_Ripple2;

	private float m_RippleTimer;

	[HideInInspector]
	public bool m_InWater;

	public override void Restart()
	{
		base.Restart();
		SetBaseDelay(0.3f);
		m_Ripple1 = m_ModelRoot.transform.Find("Ripple.001").gameObject;
		m_Ripple2 = m_ModelRoot.transform.Find("Ripple.002").gameObject;
		m_Ripple1.SetActive(value: false);
		m_Ripple2.SetActive(value: false);
		m_Ripple1.GetComponent<MeshRenderer>().enabled = true;
		m_Ripple2.GetComponent<MeshRenderer>().enabled = true;
		m_RippleTimer = 0f;
		m_InWater = false;
		m_FloatsInWater = true;
	}

	protected override void StopStateMoving()
	{
		base.StopStateMoving();
		m_Ripple1.SetActive(value: false);
		m_Ripple2.SetActive(value: false);
	}

	protected override void UpdateStateMoving()
	{
		m_RippleTimer += TimeManager.Instance.m_NormalDelta;
		switch ((int)(m_RippleTimer * 60f) % 24 / 6)
		{
		case 0:
		case 1:
			m_Ripple1.SetActive(value: false);
			m_Ripple2.SetActive(value: false);
			break;
		case 2:
			m_Ripple1.SetActive(value: true);
			m_Ripple2.SetActive(value: false);
			break;
		case 3:
			m_Ripple1.SetActive(value: false);
			m_Ripple2.SetActive(value: true);
			break;
		}
		base.UpdateStateMoving();
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		base.transform.localPosition = new Vector3(0f, -0.35f, 0f);
		base.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		m_InWater = false;
	}

	protected override void ActionDropped(Actionable PreviousHolder, TileCoord DropLocation)
	{
		base.ActionDropped(PreviousHolder, DropLocation);
		if (TileHelpers.GetTileWater(TileManager.Instance.GetTile(DropLocation).m_TileType))
		{
			m_InWater = true;
		}
		else
		{
			m_InWater = false;
		}
	}

	public override bool RequestGoTo(TileCoord Destination, Actionable TargetObject = null, bool LessOne = false, int Range = 0)
	{
		return base.RequestGoTo(Destination, TargetObject, LessOne, Range);
	}

	public override void SendAction(ActionInfo Info)
	{
		ActionType action = Info.m_Action;
		if (action == ActionType.MoveTo)
		{
			Tile tile = TileManager.Instance.GetTile(m_TileCoord);
			if (TileHelpers.GetTileWater(tile.m_TileType) && tile.m_Floor != null)
			{
				return;
			}
		}
		base.SendAction(Info);
	}

	public override object GetActionInfo(GetActionInfo Info)
	{
		GetAction action = Info.m_Action;
		if (action == GetAction.IsDisengagable)
		{
			if (TileHelpers.GetTileWaterDeep(TileManager.Instance.GetTile(m_TileCoord).m_TileType))
			{
				return false;
			}
			return true;
		}
		return base.GetActionInfo(Info);
	}

	public override ActionType GetActionFromObject(AFO Info)
	{
		if (Info.m_ActionType == AFO.AT.Primary && Info.m_ObjectType == ObjectTypeList.m_Total)
		{
			string text = "FNOCanoeEmpty";
			if ((bool)m_Engager)
			{
				text = "FNOCanoeFull";
			}
			if (Info.m_RequirementsIn == "" || Info.m_RequirementsIn == text)
			{
				Info.m_FarmerState = Farmer.State.PickingUp;
				Info.m_RequirementsOut = text;
				if (m_State != 0)
				{
					return ActionType.Fail;
				}
				return ActionType.Pickup;
			}
			if (Info.m_RequirementsIn != "")
			{
				return ActionType.Fail;
			}
		}
		return base.GetActionFromObject(Info);
	}
}
