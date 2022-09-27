using System;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public class ModQuest
{
	public void SetQuestComplete(string QuestID, bool DoCeremony = false)
	{
		if (!Enum.TryParse<Quest.ID>(QuestID, out var result))
		{
			string descriptionOverride = "Error: ModQuest.SetQuestComplete '" + QuestID + "' - Item not found";
			ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
		}
		else
		{
			Quest quest = QuestManager.Instance.GetQuest(result);
			QuestManager.Instance.CheatCompleteQuest(quest, DoCeremony);
		}
	}

	public void SetAllQuestsComplete()
	{
		QuestManager.Instance.CompleteAll();
	}

	public bool IsObjectTypeUnlocked(string ObjectTypeString)
	{
		try
		{
			ObjectType result = ObjectType.Nothing;
			if (!Enum.TryParse<ObjectType>(ObjectTypeString, out result))
			{
				result = ModManager.Instance.GetModObjectTypeFromName(ObjectTypeString);
			}
			if (result == ObjectType.Nothing)
			{
				string descriptionOverride = "Error: ModQuest.IsObjectTypeUnlocked '" + ObjectTypeString + "' - Object Type Not Recognised";
				ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
				return false;
			}
			return !QuestManager.Instance.GetIsObjectLockedAny(result);
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModQuest.IsObjectTypeUnlocked Error: " + ex.ToString());
		}
		return false;
	}
}
