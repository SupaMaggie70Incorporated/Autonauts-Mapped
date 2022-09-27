using System.Collections.Generic;
using MoonSharp.Interpreter;
using UnityEngine;

public class Actionable : BaseClass
{
	public static Dictionary<string, ActionType> m_ActionNames;

	public static AFO m_ReusableActionFromObject;

	[HideInInspector]
	public bool m_DoingAction;

	[HideInInspector]
	public Actionable m_Engager;

	public static void Init()
	{
		m_ReusableActionFromObject = new AFO();
		m_ActionNames = new Dictionary<string, ActionType>();
		for (int i = 0; i <= 49; i++)
		{
			ActionType value = (ActionType)i;
			m_ActionNames.Add(value.ToString(), value);
		}
	}

	public static ActionType GetActionFromName(string Name)
	{
		if (!m_ActionNames.ContainsKey(Name))
		{
			return ActionType.Total;
		}
		return m_ActionNames[Name];
	}

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("Actionable", m_TypeIdentifier);
	}

	public override void Restart()
	{
		base.Restart();
		m_DoingAction = false;
		m_Engager = null;
	}

	public virtual void SendAction(ActionInfo Info)
	{
	}

	public virtual bool CanDoAction(ActionInfo Info, bool RightNow = true)
	{
		return false;
	}

	public virtual object GetActionInfo(GetActionInfo Info)
	{
		switch (Info.m_Action)
		{
		case GetAction.IsDeletable:
		case GetAction.IsMovable:
		case GetAction.IsDuplicatable:
			if (m_DoingAction)
			{
				return false;
			}
			return true;
		case GetAction.IsDisengagable:
			return true;
		case GetAction.IsBusy:
			return false;
		case GetAction.IsPickable:
			return true;
		default:
			return null;
		}
	}

	public virtual ActionType GetActionFromObject(AFO Info)
	{
		Info.m_FarmerState = Farmer.State.Total;
		if (Info.m_ActionType == AFO.AT.Primary && ModCheckUseObjectOnObject(Info))
		{
			Info.m_FarmerState = Farmer.State.ModAction;
			Info.m_EndAction = ModEndAction;
			return ActionType.UseInHands;
		}
		if (Info.m_ActionType == AFO.AT.Secondary && Info.m_ObjectType != ObjectTypeList.m_Total)
		{
			Info.m_FarmerState = Farmer.State.Dropping;
			if (ObjectTypeList.Instance.GetCanDropInto(m_TypeIdentifier))
			{
				Info.m_StartAction = null;
				Info.m_UseAction = null;
				Info.m_EndAction = null;
				Info.m_RequirementsOut = "";
				return ActionType.DropAll;
			}
			return ActionType.Fail;
		}
		return ActionType.Total;
	}

	public ActionType GetActionFromObjectSafe(AFO Info)
	{
		ActionType actionFromObject = GetActionFromObject(Info);
		if (AddAnimationManager.Instance.IsAnimating(this) || m_DoingAction)
		{
			if ((actionFromObject == ActionType.Total || actionFromObject == ActionType.AddResource) && ResearchStation.GetIsTypeResearchStation(m_TypeIdentifier))
			{
				return ActionType.Total;
			}
			if ((actionFromObject == ActionType.AddResource || actionFromObject == ActionType.TakeResource) && (Storage.GetIsTypeStorage(m_TypeIdentifier) || m_TypeIdentifier == ObjectType.SpacePort))
			{
				return ActionType.Total;
			}
			return ActionType.Fail;
		}
		return actionFromObject;
	}

	public void StartAction(Actionable NewObject, Actionable Actioner, AFO.AT NewActionType, TileCoord Location)
	{
		if (m_DoingAction)
		{
			ErrorMessage.LogError("Object already doing an action : " + ObjectTypeList.Instance.GetHumanReadableNameFromIdentifier(m_TypeIdentifier));
		}
		ObjectType newObjectType = ObjectTypeList.m_Total;
		if (NewObject != null)
		{
			newObjectType = NewObject.m_TypeIdentifier;
		}
		m_ReusableActionFromObject.Init(NewObject, newObjectType, Actioner, NewActionType, "", Location);
		GetActionFromObject(m_ReusableActionFromObject);
		if (m_ReusableActionFromObject.m_StartAction != null)
		{
			if (m_TypeIdentifier != ObjectType.Plot && !Storage.GetIsTypeMediumStorage(m_TypeIdentifier))
			{
				m_DoingAction = true;
			}
			m_ReusableActionFromObject.m_StartAction(m_ReusableActionFromObject);
		}
	}

	public void UseAction(Actionable NewObject, Actionable Actioner, AFO.AT NewActionType, TileCoord Location)
	{
		ObjectType newObjectType = ObjectTypeList.m_Total;
		if (NewObject != null)
		{
			newObjectType = NewObject.m_TypeIdentifier;
		}
		m_ReusableActionFromObject.Init(NewObject, newObjectType, Actioner, NewActionType, "", Location, AFO.Stage.Use);
		GetActionFromObject(m_ReusableActionFromObject);
		if (m_ReusableActionFromObject.m_UseAction != null)
		{
			m_ReusableActionFromObject.m_UseAction(m_ReusableActionFromObject);
		}
	}

	public void EndAction(Actionable NewObject, Actionable Actioner, AFO.AT NewActionType, TileCoord Location)
	{
		m_DoingAction = false;
		ObjectType newObjectType = ObjectTypeList.m_Total;
		if (NewObject != null)
		{
			newObjectType = NewObject.m_TypeIdentifier;
		}
		m_ReusableActionFromObject.Init(NewObject, newObjectType, Actioner, NewActionType, "", Location, AFO.Stage.End);
		GetActionFromObject(m_ReusableActionFromObject);
		if (m_ReusableActionFromObject.m_EndAction != null)
		{
			m_ReusableActionFromObject.m_EndAction(m_ReusableActionFromObject);
		}
	}

	public void AbortAction(Actionable NewObject, Actionable Actioner, AFO.AT NewActionType, TileCoord Location)
	{
		m_DoingAction = false;
		ObjectType newObjectType = ObjectTypeList.m_Total;
		if (NewObject != null)
		{
			newObjectType = NewObject.m_TypeIdentifier;
		}
		m_ReusableActionFromObject.Init(NewObject, newObjectType, Actioner, NewActionType, "", Location, AFO.Stage.Abort);
		GetActionFromObject(m_ReusableActionFromObject);
		if (m_ReusableActionFromObject.m_AbortAction != null)
		{
			m_ReusableActionFromObject.m_AbortAction(m_ReusableActionFromObject);
		}
	}

	public void EndAction(AFO Info)
	{
		m_DoingAction = false;
		if (Info.m_EndAction != null)
		{
			Info.m_EndAction(Info);
		}
	}

	public void AbortAction(AFO Info)
	{
		m_DoingAction = false;
		if (Info.m_AbortAction != null)
		{
			Info.m_AbortAction(Info);
		}
	}

	public ActionType GetActionType(Actionable NewObject, Actionable Actioner, AFO.AT NewActionType, string RequirementsIn, TileCoord Location)
	{
		ObjectType newObjectType = ObjectTypeList.m_Total;
		if (NewObject != null)
		{
			newObjectType = NewObject.m_TypeIdentifier;
		}
		m_ReusableActionFromObject.Init(NewObject, newObjectType, Actioner, NewActionType, RequirementsIn, Location);
		return GetActionFromObject(m_ReusableActionFromObject);
	}

	public virtual ActionType GetAutoAction(ActionInfo Info)
	{
		return ActionType.Total;
	}

	protected bool ModCheckUseObjectOnObject(AFO Info)
	{
		if (ModManager.Instance.ModToolClass.CustomToolInfo.ContainsKey(Info.m_ObjectType) && ModManager.Instance.ModToolClass.CustomToolInfo[Info.m_ObjectType].ObjectsToUseOn.Contains(m_TypeIdentifier))
		{
			return true;
		}
		return false;
	}

	public virtual void ModEndAction(AFO Info)
	{
		if (!ModManager.Instance.ModToolClass.CustomToolInfo.ContainsKey(Info.m_ObjectType))
		{
			return;
		}
		ModTool.ModToolInfo modToolInfo = ModManager.Instance.ModToolClass.CustomToolInfo[Info.m_ObjectType];
		if (!modToolInfo.ObjectsToUseOn.Contains(m_TypeIdentifier))
		{
			return;
		}
		int num = m_UniqueID;
		string str = m_TypeIdentifier.ToString();
		if (m_TypeIdentifier >= ObjectType.Total)
		{
			str = ModManager.Instance.m_ModStrings[m_TypeIdentifier];
		}
		if (modToolInfo.DestroyObject)
		{
			StopUsing();
			num = -1;
		}
		TileCoord tileCoord = new TileCoord(base.transform.position);
		int num2 = 0;
		foreach (ObjectType item in modToolInfo.ObjectsToProduce)
		{
			for (int i = 0; i < modToolInfo.ObjectsToProduceAmount[num2]; i++)
			{
				BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(item, tileCoord.ToWorldPositionTileCentered(), Quaternion.identity);
				SpawnAnimationManager.Instance.AddJump(baseClass, tileCoord, tileCoord, 0f, baseClass.transform.position.y, 4f);
			}
			num2++;
		}
		if (modToolInfo.Callback.Function != null)
		{
			DynValue[] args = new DynValue[5]
			{
				DynValue.NewNumber(Info.m_Actioner.m_UniqueID),
				DynValue.NewNumber(Info.m_Position.x),
				DynValue.NewNumber(Info.m_Position.y),
				DynValue.NewNumber(num),
				DynValue.NewString(str)
			};
			ModManager.Instance.Callback(modToolInfo.OwnerScript, modToolInfo.Callback, args);
		}
	}
}
