using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class ToolBasket : MyTool
{
	private GameObject m_Item;

	[HideInInspector]
	public List<BaseClass> m_HeldObjects;

	[HideInInspector]
	public int m_Capacity;

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("ToolBasket", m_TypeIdentifier);
	}

	public override void Restart()
	{
		base.Restart();
		m_HeldObjects = new List<BaseClass>();
		UpdateContentsModel();
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_Capacity = 10;
		m_Item = m_ModelRoot.transform.Find("Contents").gameObject;
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		JSONArray jSONArray = (JSONArray)(Node["HeldObjects"] = new JSONArray());
		for (int i = 0; i < m_HeldObjects.Count; i++)
		{
			JSONNode jSONNode2 = new JSONObject();
			string saveNameFromIdentifier = ObjectTypeList.Instance.GetSaveNameFromIdentifier(m_HeldObjects[i].GetComponent<BaseClass>().m_TypeIdentifier);
			JSONUtils.Set(jSONNode2, "ID", saveNameFromIdentifier);
			m_HeldObjects[i].GetComponent<Savable>().Save(jSONNode2);
			jSONArray[i] = jSONNode2;
		}
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		JSONArray asArray = Node["HeldObjects"].AsArray;
		for (int i = 0; i < asArray.Count; i++)
		{
			JSONNode asObject = asArray[i].AsObject;
			BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromSaveName(asObject, new Vector3(0f, 0f, 0f), Quaternion.identity);
			if ((bool)baseClass)
			{
				baseClass.GetComponent<Savable>().Load(asObject);
				Add(baseClass.GetComponent<BaseClass>());
			}
		}
		UpdateContentsModel();
	}

	protected virtual void UpdateContentsModel()
	{
		m_Item.SetActive(!GetIsEmpty());
		m_Item.GetComponent<MeshRenderer>().enabled = !GetIsEmpty();
	}

	public virtual void Add(BaseClass NewObject)
	{
		m_HeldObjects.Add(NewObject);
		UpdateContentsModel();
		AudioManager.Instance.StartEvent("ToolBucketFill", this);
		NewObject.GetComponent<Actionable>().SendAction(new ActionInfo(ActionType.BeingHeld, default(TileCoord), this));
		NewObject.GetComponent<Actionable>().SendAction(new ActionInfo(ActionType.Stowed, default(TileCoord), this));
		NewObject.transform.SetParent(base.transform);
		NewObject.gameObject.SetActive(value: false);
	}

	public virtual void Empty()
	{
		m_HeldObjects.Clear();
		UpdateContentsModel();
		AudioManager.Instance.StartEvent("ToolBucketEmpty", this);
	}

	public bool CanAcceptObject(BaseClass NewObject)
	{
		if (NewObject == null)
		{
			return false;
		}
		if (GetIsFull())
		{
			return false;
		}
		if ((bool)NewObject.GetComponent<Clothing>())
		{
			if (GetIsEmpty())
			{
				return true;
			}
			if (GetContainsClothing() && NewObject.GetComponent<Clothing>().GetIsDirty() == m_HeldObjects[0].GetComponent<Clothing>().GetIsDirty())
			{
				return true;
			}
		}
		else if ((bool)NewObject.GetComponent<Dish>())
		{
			if (GetIsEmpty())
			{
				return true;
			}
			if (GetContainsDishes())
			{
				return true;
			}
		}
		return false;
	}

	public bool GetIsEmpty()
	{
		return m_HeldObjects.Count == 0;
	}

	public bool GetIsFull()
	{
		return m_HeldObjects.Count == m_Capacity;
	}

	public bool GetContainsClothing()
	{
		if (GetIsEmpty() || m_HeldObjects[0].GetComponent<Clothing>() == null)
		{
			return false;
		}
		return true;
	}

	public bool GetContainsDishes()
	{
		if (GetIsEmpty() || m_HeldObjects[0].GetComponent<Dish>() == null)
		{
			return false;
		}
		return true;
	}

	public BaseClass GetFirstObject()
	{
		if (m_HeldObjects.Count == 0)
		{
			return null;
		}
		return m_HeldObjects[0];
	}

	public BaseClass RemoveFirstObject()
	{
		Actionable component = m_HeldObjects[0].GetComponent<Actionable>();
		if ((bool)component)
		{
			m_HeldObjects.RemoveAt(0);
			component.SendAction(new ActionInfo(ActionType.Dropped, default(TileCoord)));
			component.gameObject.SetActive(value: true);
		}
		UpdateContentsModel();
		return component;
	}

	public void Remove(BaseClass NewObject)
	{
		m_HeldObjects.Remove(NewObject);
		NewObject.GetComponent<Actionable>().SendAction(new ActionInfo(ActionType.Dropped, default(TileCoord)));
		NewObject.gameObject.SetActive(value: true);
		UpdateContentsModel();
	}

	public override string GetHumanReadableName()
	{
		string text = base.GetHumanReadableName();
		if (!GetIsEmpty())
		{
			text = ((!GetIsFull()) ? (text + " (" + m_HeldObjects.Count + "/" + m_Capacity + " ") : (text + " (" + TextManager.Instance.Get("BasketFull") + " "));
			string text2 = "";
			string text3 = "";
			if ((bool)m_HeldObjects[0].GetComponent<Clothing>())
			{
				text2 = TextManager.Instance.Get("BasketClothes");
				Clothing.State state = m_HeldObjects[0].GetComponent<Clothing>().GetState();
				text3 = Clothing.m_StateNames[(int)state];
			}
			else
			{
				text2 = TextManager.Instance.Get("BasketDishes");
				text3 = "";
			}
			text3 = TextManager.Instance.Get(text3);
			text = text + text3 + " " + text2 + ")";
		}
		return text;
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		base.transform.localPosition = new Vector3(0f, 0f, 0f);
		base.transform.localRotation = Quaternion.identity;
	}

	public override object GetActionInfo(GetActionInfo Info)
	{
		GetAction action = Info.m_Action;
		if (action == GetAction.IsStorable)
		{
			if ((bool)Info.m_Object && Info.m_Object.m_TypeIdentifier == ObjectType.StorageGeneric)
			{
				if (m_HeldObjects.Count == 0)
				{
					return true;
				}
				return false;
			}
			return true;
		}
		return base.GetActionInfo(Info);
	}

	public BaseClass GetCleanHat()
	{
		foreach (BaseClass heldObject in m_HeldObjects)
		{
			if ((bool)heldObject.GetComponent<Hat>() && !heldObject.GetComponent<Hat>().GetIsDirty() && !heldObject.GetComponent<Hat>().GetIsWet())
			{
				Remove(heldObject);
				return heldObject;
			}
		}
		return null;
	}

	public BaseClass GetCleanTop()
	{
		foreach (BaseClass heldObject in m_HeldObjects)
		{
			if ((bool)heldObject.GetComponent<Top>() && !heldObject.GetComponent<Top>().GetIsDirty() && !heldObject.GetComponent<Top>().GetIsWet())
			{
				Remove(heldObject);
				return heldObject;
			}
		}
		return null;
	}

	public void DropContents(TileCoord EmptyTile, Farmer NewFarmer)
	{
		while ((bool)GetFirstObject())
		{
			Holdable component = RemoveFirstObject().GetComponent<Holdable>();
			component.SendAction(new ActionInfo(ActionType.Dropped, EmptyTile, NewFarmer));
			component.UpdatePositionToTilePosition(EmptyTile);
			PlotManager.Instance.RemoveObject(component);
			PlotManager.Instance.AddObject(component);
			component.gameObject.SetActive(value: true);
		}
	}
}
