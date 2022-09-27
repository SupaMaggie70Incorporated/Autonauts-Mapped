public struct IngredientRequirement
{
	public ObjectType m_Type;

	public int m_Count;

	public IngredientRequirement(ObjectType Type, int Count)
	{
		m_Type = Type;
		m_Count = Count;
	}
}
