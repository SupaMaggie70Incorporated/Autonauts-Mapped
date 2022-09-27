using System.Collections.Generic;

public class ConverterResults
{
	public List<List<IngredientRequirement>> m_Requirements;

	public List<List<IngredientRequirement>> m_Results;

	public ConverterResults()
	{
		m_Requirements = new List<List<IngredientRequirement>>();
		m_Results = new List<List<IngredientRequirement>>();
		List<IngredientRequirement> results = new List<IngredientRequirement>
		{
			new IngredientRequirement(ObjectTypeList.m_Total, 1)
		};
		Add(new List<IngredientRequirement>(), results);
	}

	public void Add(List<IngredientRequirement> Requirements, List<IngredientRequirement> Results)
	{
		m_Requirements.Add(Requirements);
		m_Results.Add(Results);
	}

	public void Add(ConverterResults NewResults)
	{
		for (int num = NewResults.m_Requirements.Count - 1; num > 0; num--)
		{
			List<IngredientRequirement> item = NewResults.m_Requirements[num];
			if (!m_Requirements.Contains(item))
			{
				m_Requirements.Insert(1, item);
			}
		}
		for (int num2 = NewResults.m_Results.Count - 1; num2 > 0; num2--)
		{
			List<IngredientRequirement> item2 = NewResults.m_Results[num2];
			if (!m_Results.Contains(item2))
			{
				m_Results.Insert(1, item2);
			}
		}
	}

	public void Remove(List<IngredientRequirement> Requirements, List<IngredientRequirement> Results)
	{
		m_Requirements.Remove(Requirements);
		m_Results.Remove(Results);
	}

	public bool Contains(ObjectType NewType)
	{
		foreach (List<IngredientRequirement> result in m_Results)
		{
			if (result[0].m_Type == NewType)
			{
				return true;
			}
		}
		return false;
	}
}
