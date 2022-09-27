using SimpleJSON;
using UnityEngine;

public class ToolFillable : MyTool
{
	[HideInInspector]
	public ObjectType m_HeldType;

	[HideInInspector]
	public ObjectType m_LastHeldType;

	[HideInInspector]
	public int m_Capacity;

	[HideInInspector]
	public int m_Stored;

	private int m_LastStored;

	public static bool GetIsTypeFillable(ObjectType NewType)
	{
		if (ToolBucket.GetIsTypeBucket(NewType))
		{
			return true;
		}
		if (ToolWateringCan.GetIsTypeWateringCan(NewType))
		{
			return true;
		}
		switch (NewType)
		{
		case ObjectType.ToolPitchfork:
			return true;
		default:
			if (!ToolFishingRod.GetIsTypeFishingRod(NewType))
			{
				return false;
			}
			goto case ObjectType.ToolFishingStick;
		case ObjectType.ToolFishingStick:
			return true;
		}
	}

	public static bool GetIsTypeLiquid(ObjectType NewType)
	{
		if (NewType == ObjectType.Water || NewType == ObjectType.SeaWater || NewType == ObjectType.Milk || NewType == ObjectType.Honey || NewType == ObjectType.Sand || NewType == ObjectType.Soil || NewType == ObjectType.Mortar)
		{
			return true;
		}
		return false;
	}

	public static bool GetIsTypeEmptyable(ObjectType NewType)
	{
		if (NewType == ObjectType.ToolPitchfork || NewType == ObjectType.ToolFishingStick || ToolFishingRod.GetIsTypeFishingRod(NewType))
		{
			return false;
		}
		return true;
	}

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("ToolFillable", m_TypeIdentifier);
	}

	public override void Restart()
	{
		base.Restart();
		m_HeldType = ObjectTypeList.m_Total;
		m_Capacity = VariableManager.Instance.GetVariableAsInt(m_TypeIdentifier, "Capacity");
		m_Stored = 0;
		m_LastStored = m_Stored;
		UpdateContentsModel();
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		JSONUtils.Set(Node, "Held", ObjectTypeList.Instance.GetSaveNameFromIdentifier(m_HeldType));
		JSONUtils.Set(Node, "Stored", m_Stored);
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		string asString = JSONUtils.GetAsString(Node, "Held", "");
		m_HeldType = ObjectTypeList.Instance.GetIdentifierFromSaveName(asString, Check: false);
		m_LastHeldType = m_HeldType;
		m_Stored = JSONUtils.GetAsInt(Node, "Stored", 0);
		m_LastStored = m_Stored;
		UpdateContentsModel();
	}

	protected virtual void UpdateContentsModel()
	{
	}

	public virtual void Fill(ObjectType NewType, int Amount)
	{
		m_HeldType = NewType;
		m_LastHeldType = m_HeldType;
		m_LastStored = m_Stored;
		m_Stored += Amount;
		if (m_Stored > m_Capacity)
		{
			m_Stored = m_Capacity;
		}
		UpdateContentsModel();
	}

	public virtual void Empty(int Amount)
	{
		m_LastStored = m_Stored;
		m_Stored -= Amount;
		if (m_Stored <= 0)
		{
			m_Stored = 0;
			m_HeldType = ObjectTypeList.m_Total;
		}
		UpdateContentsModel();
	}

	public int GetSpace()
	{
		return m_Capacity - m_Stored;
	}

	public bool GetIsEmpty()
	{
		return m_HeldType == ObjectTypeList.m_Total;
	}

	public bool GetIsFull()
	{
		return m_Stored == m_Capacity;
	}

	public override string GetHumanReadableName()
	{
		string text = base.GetHumanReadableName();
		if (m_HeldType != ObjectTypeList.m_Total)
		{
			string saveNameFromIdentifier = ObjectTypeList.Instance.GetSaveNameFromIdentifier(m_HeldType);
			text = text + " (" + m_Stored + "/" + m_Capacity + " " + TextManager.Instance.Get(saveNameFromIdentifier) + ")";
		}
		return text;
	}

	public void RestoreLastStored()
	{
		m_Stored = m_LastStored;
		m_HeldType = m_LastHeldType;
		UpdateContentsModel();
	}

	public virtual bool CanAcceptObjectType(ObjectType NewType)
	{
		if (!GetIsEmpty() && m_HeldType != NewType)
		{
			return false;
		}
		if (GetIsFull())
		{
			return false;
		}
		return true;
	}
}
