using System.Collections.Generic;
using UnityEngine;

public class ObjectTypeInfo
{
	public ObjectType m_Type;

	public string m_SaveName;

	public string m_PrefabName;

	public string m_ModelName;

	public bool m_Reusable;

	public List<IngredientRequirement[]> m_Ingredients;

	public bool m_InfinteRecusion;

	public string m_IconName;

	public bool m_Stackable;

	public Layers m_Layer;

	public Transform m_Parent;

	public bool m_Building;

	public float m_Height;

	public float m_Offset;

	public int m_Weight;

	public bool m_Sleepy;

	public ObjectSubCategory m_SubCategory;

	public ObjectType m_StorageType;

	public int m_Tier;

	public bool m_Holdable;

	public ObjectUseType m_UseType;

	public bool m_MissionUsable;

	public ObjectTypeInfo(ObjectType Type, string SaveName, string PrefabName, string ModelName, bool Reusable, IngredientRequirement[] Ingredients, string IconName, bool Stackable, bool IsBuilding, float Height, float Offset, int Weight, ObjectSubCategory SubCategory)
	{
		m_Type = Type;
		m_SaveName = SaveName;
		m_PrefabName = PrefabName;
		m_ModelName = ModelName;
		m_Reusable = Reusable;
		m_Ingredients = new List<IngredientRequirement[]>();
		m_IconName = IconName;
		m_Stackable = Stackable;
		m_Layer = Layers.Total;
		m_Parent = null;
		m_Building = IsBuilding;
		m_Height = Height;
		m_Offset = Offset;
		m_Weight = Weight;
		m_SubCategory = SubCategory;
		m_Tier = 0;
		m_InfinteRecusion = false;
		m_MissionUsable = true;
	}
}
