using System.Collections.Generic;
using UnityEngine;

public class NewIconManager : MonoBehaviour
{
	public static NewIconManager Instance;

	private List<ObjectType> m_NewObjects;

	private List<ObjectType> m_QueuedObjects;

	private List<ObjectType> m_NewConverters;

	private List<Quest.ID> m_QueuedResearchQuests;

	private List<ObjectType> m_NewResearches;

	private void Awake()
	{
		Instance = this;
		m_NewObjects = new List<ObjectType>();
		m_QueuedObjects = new List<ObjectType>();
		m_NewConverters = new List<ObjectType>();
		m_QueuedResearchQuests = new List<Quest.ID>();
		m_NewResearches = new List<ObjectType>();
	}

	private bool CanConverterUseNewObject(Converter NewConverter, ObjectType NewObjectType)
	{
		if (NewConverter.m_Blueprint)
		{
			return false;
		}
		if (NewConverter.m_TypeIdentifier == ObjectType.FolkSeedPod)
		{
			return false;
		}
		if (NewConverter.CanMakeObject(NewObjectType))
		{
			return true;
		}
		if (NewConverter.m_BuildingToUpgradeTo == NewObjectType)
		{
			return true;
		}
		return false;
	}

	private void MarkConverterNew(ObjectType NewObjectType, bool New)
	{
		Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("Converter");
		if (collection == null)
		{
			return;
		}
		foreach (KeyValuePair<BaseClass, int> item in collection)
		{
			if (!item.Key)
			{
				continue;
			}
			Converter component = item.Key.GetComponent<Converter>();
			if ((bool)component && CanConverterUseNewObject(component, NewObjectType))
			{
				component.SetNew(New);
				if (!m_NewConverters.Contains(component.m_TypeIdentifier))
				{
					m_NewConverters.Add(component.m_TypeIdentifier);
				}
			}
		}
	}

	public bool IsObjectNew(ObjectType NewObjectType)
	{
		return m_NewObjects.Contains(NewObjectType);
	}

	public void NewObjectUnlocked(ObjectType NewObjectType)
	{
		if (!TutorialPanelController.Instance.GetActive())
		{
			m_QueuedObjects.Add(NewObjectType);
		}
	}

	public void ConverterSeen(Converter NewConverter)
	{
		ObjectType typeIdentifier = NewConverter.m_TypeIdentifier;
		foreach (List<IngredientRequirement> result in NewConverter.m_Results)
		{
			ObjectType type = result[0].m_Type;
			if (m_NewObjects.Contains(type))
			{
				m_NewObjects.Remove(type);
			}
		}
		if (!m_NewConverters.Contains(typeIdentifier))
		{
			return;
		}
		m_NewConverters.Remove(typeIdentifier);
		foreach (KeyValuePair<BaseClass, int> item in CollectionManager.Instance.GetCollection("Converter"))
		{
			if (item.Key.m_TypeIdentifier == typeIdentifier)
			{
				item.Key.GetComponent<Converter>().SetNew(New: false);
			}
		}
	}

	public void ConverterBuilt(Converter NewConverter)
	{
		ObjectType typeIdentifier = NewConverter.m_TypeIdentifier;
		Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("Converter");
		int num = 0;
		foreach (KeyValuePair<BaseClass, int> item in collection)
		{
			if (item.Key.GetComponent<Converter>().GetIsSavable() && item.Key.m_TypeIdentifier == typeIdentifier)
			{
				num++;
			}
		}
		if (num == 1)
		{
			ConverterSeen(NewConverter);
		}
	}

	private void UpdateNewConverters()
	{
		List<ObjectType> list = new List<ObjectType>();
		foreach (ObjectType queuedObject in m_QueuedObjects)
		{
			if (!CeremonyManager.Instance.IsObjectTypeInCeremonyQueue(queuedObject))
			{
				m_NewObjects.Add(queuedObject);
				MarkConverterNew(queuedObject, New: true);
				list.Add(queuedObject);
			}
		}
		foreach (ObjectType item in list)
		{
			m_QueuedObjects.Remove(item);
		}
	}

	private void MarkResearchNew(Quest.ID NewResearchID, bool New)
	{
		Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("ResearchStation");
		if (collection == null)
		{
			return;
		}
		foreach (KeyValuePair<BaseClass, int> item in collection)
		{
			if (!item.Key)
			{
				continue;
			}
			ResearchStation component = item.Key.GetComponent<ResearchStation>();
			if ((bool)component && component.CanDoResearch(NewResearchID) && !component.m_Blueprint)
			{
				component.SetNew(New);
				if (!m_NewResearches.Contains(component.m_TypeIdentifier))
				{
					m_NewResearches.Add(component.m_TypeIdentifier);
				}
			}
		}
	}

	public bool IsResearchNew(Quest.ID NewQuestID)
	{
		return m_QueuedResearchQuests.Contains(NewQuestID);
	}

	public void NewResearchQuestUnlocked(Quest.ID NewQuestID)
	{
		m_QueuedResearchQuests.Add(NewQuestID);
	}

	public void ResearchSeen(ResearchStation NewResearch)
	{
		ObjectType typeIdentifier = NewResearch.m_TypeIdentifier;
		if (!m_NewResearches.Contains(typeIdentifier))
		{
			return;
		}
		m_NewResearches.Remove(typeIdentifier);
		foreach (KeyValuePair<BaseClass, int> item in CollectionManager.Instance.GetCollection("ResearchStation"))
		{
			if (item.Key.m_TypeIdentifier == typeIdentifier)
			{
				item.Key.GetComponent<ResearchStation>().SetNew(New: false);
			}
		}
	}

	public void ResearchBuilt(ResearchStation NewResearch)
	{
		ObjectType typeIdentifier = NewResearch.m_TypeIdentifier;
		Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("ResearchStation");
		int num = 0;
		foreach (KeyValuePair<BaseClass, int> item in collection)
		{
			if (item.Key.GetComponent<ResearchStation>().GetIsSavable() && item.Key.m_TypeIdentifier == typeIdentifier)
			{
				num++;
			}
		}
		if (num == 1)
		{
			ResearchSeen(NewResearch);
		}
	}

	private void UpdateNewResearch()
	{
		List<Quest.ID> list = new List<Quest.ID>();
		foreach (Quest.ID queuedResearchQuest in m_QueuedResearchQuests)
		{
			if (!CeremonyManager.Instance.IsQuestTypeInCeremonyQueue(queuedResearchQuest))
			{
				MarkResearchNew(queuedResearchQuest, New: true);
				list.Add(queuedResearchQuest);
			}
		}
		foreach (Quest.ID item in list)
		{
			m_QueuedResearchQuests.Remove(item);
		}
	}

	private void Update()
	{
		UpdateNewConverters();
		UpdateNewResearch();
	}
}
