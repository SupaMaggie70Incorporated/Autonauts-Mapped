using SimpleJSON;
using UnityEngine;

public class Crane : Vehicle
{
	[HideInInspector]
	public GameObject m_Wheels1;

	[HideInInspector]
	public GameObject m_Wheels2;

	[HideInInspector]
	public Actionable m_HeldObject;

	[HideInInspector]
	public GameObject m_AttachPoint;

	[HideInInspector]
	public GameObject m_DrivePoint;

	public static bool GetIsTypeCrane(ObjectType NewType)
	{
		if (NewType == ObjectType.CraneCrude)
		{
			return true;
		}
		return false;
	}

	public override void Restart()
	{
		base.Restart();
		m_Wheels1 = m_ModelRoot.transform.Find("Wheels.001").gameObject;
		m_Wheels2 = m_ModelRoot.transform.Find("Wheels.002").gameObject;
		m_AttachPoint = m_ModelRoot.transform.Find("AttachPoint").gameObject;
		m_DrivePoint = m_ModelRoot.transform.Find("DrivingPoint").gameObject;
		SetBaseDelay(0.3f);
		m_HeldObject = null;
		m_MoveSoundName = "CraneMotion";
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		if ((bool)m_HeldObject)
		{
			m_HeldObject.SendAction(new ActionInfo(ActionType.Dropped, m_TileCoord, this));
			m_HeldObject = null;
		}
		base.StopUsing(AndDestroy);
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		if ((bool)m_HeldObject)
		{
			JSONObject node = (JSONObject)(Node["CarryObject"] = new JSONObject());
			string saveNameFromIdentifier = ObjectTypeList.Instance.GetSaveNameFromIdentifier(m_HeldObject.GetComponent<BaseClass>().m_TypeIdentifier);
			JSONUtils.Set(node, "ID", saveNameFromIdentifier);
			m_HeldObject.GetComponent<Savable>().Save(node);
		}
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		JSONNode jSONNode = Node["CarryObject"];
		if (jSONNode != null && !jSONNode.IsNull)
		{
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromSaveName(jSONNode, new Vector3(0f, 0f, 0f), Quaternion.identity);
			if ((bool)baseClass)
			{
				baseClass.GetComponent<Savable>().Load(jSONNode);
				AddCarry(baseClass.GetComponent<Holdable>());
			}
		}
	}

	private void AddCarry(Actionable CarryObject)
	{
		if (!(m_HeldObject != null))
		{
			m_HeldObject = CarryObject;
			CarryObject.transform.parent = m_AttachPoint.transform;
			CarryObject.SendAction(new ActionInfo(ActionType.BeingHeld, default(TileCoord), this));
			CarryObject.transform.localPosition = default(Vector3);
			CarryObject.transform.localRotation = ObjectUtils.m_ModelRotator * Quaternion.Euler(0f, 90f, 0f);
		}
	}

	private void DropAll()
	{
		if (!(m_HeldObject == null))
		{
			m_HeldObject.SendAction(new ActionInfo(ActionType.Dropped, m_GoToTilePosition, this));
			m_HeldObject.GetComponent<Holdable>().SetHighlight(Highlighted: true);
			m_HeldObject.GetComponent<Holdable>().SetHighlight(Highlighted: false);
			m_HeldObject = null;
		}
	}

	private bool CanPickupObject(Actionable TargetObject)
	{
		if (TargetObject == null)
		{
			return false;
		}
		if ((bool)TargetObject.GetComponent<Savable>() && !TargetObject.GetComponent<Savable>().GetIsSavable())
		{
			return false;
		}
		if (!TargetObject.CanDoAction(new ActionInfo(ActionType.BeingHeld, default(TileCoord), this)))
		{
			return false;
		}
		return true;
	}

	public override void SendAction(ActionInfo Info)
	{
		switch (Info.m_Action)
		{
		case ActionType.Pickup:
			if (CanPickupObject(Info.m_Object))
			{
				BaggedManager.Instance.Remove(Info.m_Object);
				AddCarry(Info.m_Object);
				AudioManager.Instance.StartEvent("CranePickup", this);
			}
			break;
		case ActionType.DropAll:
			DropAll();
			AudioManager.Instance.StartEvent("CraneDrop", this);
			break;
		case ActionType.TakeResource:
		case ActionType.AddResource:
			if ((bool)Info.m_Object)
			{
				Info.m_Object.StartAction(m_HeldObject, this, Info.m_ActionType, Info.m_Position);
				Info.m_Object.EndAction(m_HeldObject, this, Info.m_ActionType, Info.m_Position);
			}
			break;
		}
		base.SendAction(Info);
	}

	public override bool CanDoAction(ActionInfo Info, bool RightNow)
	{
		switch (Info.m_Action)
		{
		case ActionType.Disengaged:
			return true;
		case ActionType.BeingHeld:
			if (!GetIsTypeCrane(Info.m_ObjectType))
			{
				return false;
			}
			break;
		}
		return base.CanDoAction(Info, RightNow);
	}

	public override ActionType GetAutoAction(ActionInfo Info)
	{
		if ((bool)Info.m_Object)
		{
			ObjectType typeIdentifier = Info.m_Object.m_TypeIdentifier;
			if (!Vehicle.GetIsTypeVehicle(typeIdentifier) && !TrainTrack.GetIsTypeTrainTrack(typeIdentifier) && typeIdentifier != ObjectType.Plot && !Floor.GetIsTypeFloor(typeIdentifier) && typeIdentifier != ObjectType.StoneHeads && typeIdentifier != ObjectType.SpacePort)
			{
				return ActionType.Total;
			}
		}
		ActionType autoAction = base.GetAutoAction(Info);
		if (autoAction == ActionType.AddResource && m_HeldObject == null)
		{
			return ActionType.Total;
		}
		if (autoAction == ActionType.Pickup || autoAction == ActionType.DropAll || autoAction == ActionType.AddResource || autoAction == ActionType.TakeResource)
		{
			Actionable.m_ReusableActionFromObject.m_AdjacentTile = true;
			Actionable.m_ReusableActionFromObject.m_MoveRange = 0;
		}
		return autoAction;
	}

	private void AutoAction()
	{
		ObjectType actionObjectType = ObjectTypeList.m_Total;
		if ((bool)m_HeldObject)
		{
			actionObjectType = m_HeldObject.m_TypeIdentifier;
		}
		m_Engager.GetComponent<FarmerPlayer>().AutoAction(new ActionInfo(ActionType.Total, m_GoToTilePosition, m_GoToTargetObject, "", "", m_ActionType, "", actionObjectType), AndAction: true);
	}

	public void AddWheelRotation(Quaternion NewRotation)
	{
		m_Wheels1.transform.localRotation = m_Wheels1.transform.localRotation * NewRotation;
		m_Wheels2.transform.localRotation = m_Wheels2.transform.localRotation * NewRotation;
	}

	public override void StopGoTo()
	{
		base.StopGoTo();
	}

	public override void EndGoTo()
	{
		base.EndGoTo();
		StopGoTo();
		if ((bool)m_Engager && m_Engager.m_TypeIdentifier == ObjectType.FarmerPlayer)
		{
			AutoAction();
		}
	}
}
