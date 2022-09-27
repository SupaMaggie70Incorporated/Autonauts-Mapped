using System.Collections.Generic;
using UnityEngine;

public class FolkHeart : Holdable
{
	public static List<ObjectType> m_Types;

	public static void InitFolkHeart()
	{
		m_Types = new List<ObjectType>();
		ObjectType[] array = new ObjectType[7]
		{
			ObjectType.FolkHeart,
			ObjectType.FolkHeart2,
			ObjectType.FolkHeart3,
			ObjectType.FolkHeart4,
			ObjectType.FolkHeart5,
			ObjectType.FolkHeart6,
			ObjectType.FolkHeart7
		};
		foreach (ObjectType item in array)
		{
			m_Types.Add(item);
		}
	}

	public static bool GetIsFolkHeart(ObjectType NewType)
	{
		if (m_Types.Contains(NewType))
		{
			return true;
		}
		return false;
	}

	public static ObjectType GetObjectTypeFromTier(int Tier)
	{
		return m_Types[Tier];
	}

	public static int GetTierFromObjectType(ObjectType NewType)
	{
		return m_Types.IndexOf(NewType);
	}

	public override void Restart()
	{
		base.Restart();
		float tierScale = BaseClass.GetTierScale(GetTierFromObjectType(m_TypeIdentifier));
		base.transform.localScale = new Vector3(tierScale, tierScale, tierScale);
	}

	public override string GetHumanReadableName()
	{
		int variableAsInt = VariableManager.Instance.GetVariableAsInt(m_TypeIdentifier, "Value");
		if (variableAsInt == 1)
		{
			return TextManager.Instance.Get("FolkHeart");
		}
		string text = TextManager.Instance.Get("FolkHeart1");
		return text + $"{variableAsInt:N0}";
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		base.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
	}
}
