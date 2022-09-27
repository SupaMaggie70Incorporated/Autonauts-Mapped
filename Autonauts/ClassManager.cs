using System.Collections.Generic;

public class ClassManager
{
	public static ClassManager Instance;

	private Dictionary<string, Dictionary<ObjectType, int>> m_Classes;

	public ClassManager()
	{
		Instance = this;
		m_Classes = new Dictionary<string, Dictionary<ObjectType, int>>();
	}

	public void RegisterClass(string BaseClass, ObjectType ObjectType)
	{
		if (!m_Classes.ContainsKey(BaseClass))
		{
			m_Classes.Add(BaseClass, new Dictionary<ObjectType, int>());
		}
		Dictionary<ObjectType, int> dictionary = m_Classes[BaseClass];
		if (!dictionary.ContainsKey(ObjectType))
		{
			dictionary.Add(ObjectType, 0);
		}
	}

	public bool GetObjectTypeUsesClass(string BaseClass, ObjectType ObjectType)
	{
		if (m_Classes.ContainsKey(BaseClass))
		{
			return m_Classes[BaseClass].ContainsKey(ObjectType);
		}
		ErrorMessage.LogError("BaseClass " + BaseClass + " doesn't exit");
		return false;
	}
}
