using SimpleJSON;
using UnityEngine;

public class SpawnAnimation
{
	public enum Type
	{
		Jump,
		ShoreIn,
		ShoreOut,
		Total
	}

	public Type m_Type;

	public BaseClass m_NewObject;

	public SpawnAnimation(Type NewType, BaseClass NewObject)
	{
		m_Type = NewType;
		m_NewObject = NewObject;
	}

	public virtual void Save(JSONNode NewNode)
	{
		JSONUtils.Set(NewNode, "Type", (int)m_Type);
		BaseClass newObject = m_NewObject;
		JSONUtils.Set(NewNode, "ID", newObject.m_UniqueID);
	}

	public virtual void Load(JSONNode NewNode)
	{
		int asInt = JSONUtils.GetAsInt(NewNode, "ID", 0);
		m_NewObject = ObjectTypeList.Instance.GetObjectFromUniqueID(asInt, ErrorCheck: false);
		if (m_NewObject == null)
		{
			Debug.Log("SpawnAnimation.Load : Couldn't find object with UID " + asInt);
		}
	}

	public virtual void PostLoad()
	{
		if ((bool)m_NewObject)
		{
			m_NewObject.GetComponent<Actionable>().SendAction(new ActionInfo(ActionType.Refresh, default(TileCoord)));
		}
	}

	public virtual void Start()
	{
		m_NewObject.enabled = false;
	}

	protected void EndInWorld()
	{
		m_NewObject.enabled = true;
	}

	public virtual void End(bool Success)
	{
		m_NewObject.GetComponent<Actionable>().SendAction(new ActionInfo(ActionType.SpawnEnd, default(TileCoord), null, Success.ToString()));
	}

	public virtual bool IsSavable()
	{
		return true;
	}

	public virtual void Abort()
	{
	}

	public virtual bool Update()
	{
		return true;
	}
}
