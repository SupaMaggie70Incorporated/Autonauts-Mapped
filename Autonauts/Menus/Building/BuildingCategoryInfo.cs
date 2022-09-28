using System.Collections.Generic;

public class BuildingCategoryInfo
{
	public ObjectSubCategory m_Category;

	public List<ObjectType> m_Buildings;

	public List<bool> m_Locked;

	public string m_Name;

	public BuildingCategoryInfo(ObjectSubCategory Category, List<ObjectType> Buildings, string Name)
	{
		m_Category = Category;
		m_Buildings = Buildings;
		m_Locked = new List<bool>();
		for (int i = 0; i < m_Buildings.Count; i++)
		{
			m_Locked.Add(item: false);
		}
		m_Name = Name;
	}
}
