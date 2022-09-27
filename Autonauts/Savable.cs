using SimpleJSON;
using UnityEngine;

public class Savable : Actionable
{
	private bool m_IsSavable;

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("Savable", m_TypeIdentifier);
	}

	public override void Restart()
	{
		base.Restart();
		if ((bool)CollectionManager.Instance)
		{
			CollectionManager.Instance.AddCollectable("Savable", this);
		}
		m_IsSavable = true;
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		base.StopUsing(AndDestroy);
		m_IsSavable = false;
	}

	protected new void Awake()
	{
		base.Awake();
		m_IsSavable = true;
	}

	public virtual void Save(JSONNode Node)
	{
		if (m_UniqueID == -1)
		{
			if ((bool)GetComponent<TileCoordObject>())
			{
				Debug.Log(string.Concat("An object with -1 is saved ", m_TypeIdentifier, " ", GetComponent<TileCoordObject>().m_TileCoord.x, ",", GetComponent<TileCoordObject>().m_TileCoord.y));
			}
			else
			{
				Debug.Log("An object with -1 is saved " + m_TypeIdentifier);
			}
		}
		JSONUtils.Set(Node, "UID", m_UniqueID);
	}

	public virtual void Load(JSONNode Node)
	{
		int asInt = JSONUtils.GetAsInt(Node, "UID", -1);
		if (asInt != -1)
		{
			ObjectTypeList.Instance.ChangeActionable(this, asInt);
		}
		else
		{
			asInt = -1;
		}
	}

	public bool GetIsSavable()
	{
		return m_IsSavable;
	}

	public void SetIsSavable(bool IsSavable)
	{
		if (m_IsSavable != IsSavable)
		{
			m_IsSavable = IsSavable;
			ObjectTypeList.Instance.ActionableOnGround(this, m_IsSavable);
		}
	}

	public virtual void PostLoad()
	{
		UpdateUniqueIDs();
	}

	public virtual void UpdateUniqueIDs()
	{
		if (m_UniqueID == -1)
		{
			if ((bool)GetComponent<TileCoordObject>())
			{
				Debug.Log(string.Concat("An object with -1 is created ", m_TypeIdentifier, " ", GetComponent<TileCoordObject>().m_TileCoord.x, ",", GetComponent<TileCoordObject>().m_TileCoord.y));
			}
			else
			{
				Debug.Log("An object with -1 is created " + m_TypeIdentifier);
			}
			m_UniqueID = ObjectTypeList.Instance.AddActionable(this);
		}
	}

	public override object GetActionInfo(GetActionInfo Info)
	{
		GetAction action = Info.m_Action;
		if (action == GetAction.IsPickable)
		{
			return GetIsSavable();
		}
		return base.GetActionInfo(Info);
	}
}
