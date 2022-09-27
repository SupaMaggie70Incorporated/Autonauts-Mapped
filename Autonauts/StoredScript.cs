using System.Collections.Generic;
using UnityEngine;

public class StoredScript
{
	public HighInstructionList m_Instructions;

	public string m_Name;

	public ObjectType m_Head;

	public int m_UID;

	public List<Worker> m_LinkedBots;

	public List<DataStorageCrude> m_LinkedDataStorages;

	public StoredScript(HighInstructionList Instructions, string Name, ObjectType Head, int UID)
	{
		m_Instructions = Instructions;
		m_Name = Name;
		m_Head = Head;
		m_UID = UID;
		m_LinkedBots = new List<Worker>();
		m_LinkedDataStorages = new List<DataStorageCrude>();
	}

	public void LinkBot(Worker NewBot)
	{
		if (m_LinkedBots.Contains(NewBot))
		{
			Debug.Log("Bot " + NewBot.GetWorkerName() + " already linked to script " + m_Name);
		}
		else
		{
			m_LinkedBots.Add(NewBot);
		}
	}

	public void UnlinkBot(Worker NewBot)
	{
		if (!m_LinkedBots.Contains(NewBot))
		{
			Debug.Log("Bot " + NewBot.GetWorkerName() + " not linked to script " + m_Name);
		}
		else
		{
			m_LinkedBots.Remove(NewBot);
		}
	}

	public int GetNumLinkedBots()
	{
		return m_LinkedBots.Count;
	}

	public void LinkDataStorage(DataStorageCrude NewDataStorage)
	{
		if (m_LinkedDataStorages.Contains(NewDataStorage))
		{
			Debug.Log("DataStorage " + NewDataStorage.m_UniqueID + " already linked to script " + m_Name);
		}
		else
		{
			m_LinkedDataStorages.Add(NewDataStorage);
		}
	}

	public void UnlinkDataStorage(DataStorageCrude NewDataStorage)
	{
		if (!m_LinkedDataStorages.Contains(NewDataStorage))
		{
			Debug.Log("DataStorage " + NewDataStorage.m_UniqueID + " not linked to script " + m_Name);
		}
		else
		{
			m_LinkedDataStorages.Remove(NewDataStorage);
		}
	}

	public int GetNumLinkedDataStorages()
	{
		return m_LinkedDataStorages.Count;
	}

	public void UnregisterDataStorages()
	{
		List<DataStorageCrude> list = new List<DataStorageCrude>();
		foreach (DataStorageCrude linkedDataStorage in m_LinkedDataStorages)
		{
			list.Add(linkedDataStorage);
		}
		m_LinkedDataStorages.Clear();
	}
}
