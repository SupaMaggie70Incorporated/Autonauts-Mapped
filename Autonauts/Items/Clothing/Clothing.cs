using SimpleJSON;
using UnityEngine;

public class Clothing : Holdable
{
	public enum Type
	{
		Hat,
		Top,
		Total
	}

	[HideInInspector]
	public enum State
	{
		Clean,
		Dirty,
		Wet,
		Total
	}

	public static string[] m_StateNames = new string[3] { "ClothingStateClean", "ClothingStateDirty", "ClothingStateWet" };

	[HideInInspector]
	public float m_Dirt;

	[HideInInspector]
	public bool m_Wet;

	public static Type GetTypeFromString(string Name)
	{
		for (int i = 0; i < 2; i++)
		{
			Type type = (Type)i;
			if (type.ToString() == Name)
			{
				return (Type)i;
			}
		}
		return Type.Total;
	}

	public static Type GetTypeFromObjectType(ObjectType NewType)
	{
		if (Hat.GetIsTypeHat(NewType))
		{
			return Type.Hat;
		}
		if (Top.GetIsTypeTop(NewType))
		{
			return Type.Top;
		}
		return Type.Total;
	}

	public static bool GetIsTypeClothing(ObjectType NewType)
	{
		if (Hat.GetIsTypeHat(NewType) || Top.GetIsTypeTop(NewType))
		{
			return true;
		}
		return false;
	}

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("Clothing", m_TypeIdentifier);
	}

	public override void Restart()
	{
		base.Restart();
		m_Dirt = 0f;
		m_Wet = false;
		UpdateMaterial();
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		JSONUtils.Set(Node, "DT", m_Dirt);
		JSONUtils.Set(Node, "Wet", m_Wet);
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		m_Dirt = JSONUtils.GetAsFloat(Node, "DT", 0f);
		m_Wet = JSONUtils.GetAsBool(Node, "Wet", DefaultValue: false);
		UpdateMaterial();
	}

	public void ReadyWear(GameObject ParentObject, Transform ParentTransform)
	{
		base.transform.SetParent(ParentTransform);
		base.transform.localPosition = new Vector3(0f, 0f, 0f);
		base.transform.SetParent(MapManager.Instance.m_ObjectsRootTransform);
	}

	public virtual void Wear(GameObject ParentObject, Transform ParentTransform)
	{
		base.transform.SetParent(ParentTransform);
		base.transform.localPosition = new Vector3(0f, 0f, 0f);
		base.transform.localRotation = Quaternion.identity;
		base.transform.localScale = new Vector3(1f, 1f, 1f);
	}

	public virtual void Remove()
	{
		base.transform.parent = MapManager.Instance.m_ObjectsRootTransform;
		UpdateTierScale();
	}

	protected override void ActionDropped(Actionable PreviousHolder, TileCoord DropLocation)
	{
		base.ActionDropped(PreviousHolder, DropLocation);
		UpdateTierScale();
	}

	public override bool CanDoAction(ActionInfo Info, bool RightNow)
	{
		ActionType action = Info.m_Action;
		if (action == ActionType.UseInHands)
		{
			if ((bool)Info.m_Object && (bool)Info.m_Object.GetComponent<ToolBasket>())
			{
				return true;
			}
			return false;
		}
		return base.CanDoAction(Info, RightNow);
	}

	public override object GetActionInfo(GetActionInfo Info)
	{
		GetAction action = Info.m_Action;
		if (action == GetAction.IsStorable)
		{
			if ((bool)Info.m_Object && (bool)Info.m_Object.GetComponent<StorageGeneric>() && (GetIsDirty() || GetIsWet()))
			{
				return false;
			}
			return true;
		}
		return base.GetActionInfo(Info);
	}

	private void UpdateMaterial()
	{
		if (!GetIsDirty() && !GetIsWet())
		{
			string modelNameFromIdentifier = ObjectTypeList.Instance.GetModelNameFromIdentifier(m_TypeIdentifier);
			LoadNewModel(modelNameFromIdentifier);
			return;
		}
		string text = "";
		if (GetIsDirty())
		{
			text = "SharedBrown";
		}
		if (GetIsWet())
		{
			text = "SharedGlass";
		}
		Material material = (Material)Resources.Load("Models/Materials/" + text, typeof(Material));
		MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].material = material;
		}
	}

	public void SetIsClean()
	{
		m_Dirt = 0f;
		UpdateMaterial();
	}

	public void AddDirt(float Dirt)
	{
		m_Dirt += Dirt;
		if (m_Dirt > 1f)
		{
			m_Dirt = 1f;
		}
		UpdateMaterial();
	}

	public void SetDirt(float Dirt)
	{
		m_Dirt = Dirt;
		UpdateMaterial();
	}

	public bool GetIsDirty()
	{
		if (m_Dirt == 1f)
		{
			return true;
		}
		return false;
	}

	public void SetIsWet(bool Wet)
	{
		m_Wet = Wet;
		UpdateMaterial();
	}

	public bool GetIsWet()
	{
		return m_Wet;
	}

	public State GetState()
	{
		if (GetIsDirty())
		{
			return State.Dirty;
		}
		if (GetIsWet())
		{
			return State.Wet;
		}
		return State.Clean;
	}
}
