using UnityEngine;

public class BadgeEvent
{
	public enum Type
	{
		Tutorial,
		Berries,
		Mushrooms,
		Milk,
		Wool,
		Eggs,
		Fish,
		Tools,
		Honey,
		Colonists,
		Mining,
		Pottery,
		Clothes,
		PlotsUncovered,
		TreesCut,
		Food,
		CropsCut,
		MobileStorage,
		AnythingStored,
		BotsMade,
		GameComplete,
		Total
	}

	public Type m_Type;

	public int m_Count;

	public static string GetNameFromType(Type NewType)
	{
		return "BadgeEvent" + NewType;
	}

	public BadgeEvent(Type NewType)
	{
		m_Type = NewType;
		m_Count = 0;
	}

	public void Save()
	{
		PlayerPrefs.SetInt(GetNameFromType(m_Type), m_Count);
	}

	public void Load()
	{
		string nameFromType = GetNameFromType(m_Type);
		if (PlayerPrefs.HasKey(nameFromType))
		{
			m_Count = PlayerPrefs.GetInt(nameFromType);
		}
	}

	public void Clear()
	{
		m_Count = 0;
		Save();
	}

	public void AddEvent(int Amount)
	{
		m_Count += Amount;
	}
}
