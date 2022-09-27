using UnityEngine;

public class HighInstructionInfo
{
	public enum Category
	{
		Control,
		Movement,
		Item,
		Inventory,
		Action,
		Find,
		Total
	}

	public Category m_Category;

	public string m_Name;

	public static HighInstructionCategoryInfo[] m_CategoryInfo = new HighInstructionCategoryInfo[6]
	{
		new HighInstructionCategoryInfo(new Color32(225, 169, 26, byte.MaxValue), "CategoryControl", "CategoryControl"),
		new HighInstructionCategoryInfo(new Color32(74, 108, 212, byte.MaxValue), "CategoryMovement", "CategoryMovement"),
		new HighInstructionCategoryInfo(new Color32(238, 125, 22, byte.MaxValue), "CategoryItem", "CategoryItem"),
		new HighInstructionCategoryInfo(new Color32(44, 165, 226, byte.MaxValue), "CategoryInventory", "CategoryInventory"),
		new HighInstructionCategoryInfo(new Color32(14, 154, 108, byte.MaxValue), "CategoryAction", "CategoryAction"),
		new HighInstructionCategoryInfo(new Color32(99, 45, 153, byte.MaxValue), "CategoryFind", "CategoryFind")
	};

	public HighInstructionInfo(Category NewCategory, string Name)
	{
		m_Category = NewCategory;
		m_Name = Name;
	}
}
