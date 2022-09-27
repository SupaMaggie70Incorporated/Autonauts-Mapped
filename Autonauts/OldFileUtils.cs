using System.Collections.Generic;
using UnityEngine;

public class OldFileUtils : MonoBehaviour
{
	public static void CheckQuests()
	{
		Quest quest = QuestManager.Instance.GetQuest(Quest.ID.GlueFinal);
		if (quest == null || !quest.GetIsComplete())
		{
			return;
		}
		foreach (Quest.ID item in quest.m_QuestsUnlocked)
		{
			QuestManager.Instance.AddQuest(item, DoCeremony: false);
		}
	}

	public static string GetObjectSaveName(string OldName)
	{
		string result = OldName;
		if (LoadJSON.m_LoadingVersion < 48 && OldName == "BackpackCrude")
		{
			result = "UpgradePlayerInventoryCrude";
		}
		return result;
	}

	public static bool CheckCullObjects(string OldName)
	{
		if (LoadJSON.m_LoadingVersion < 97 && OldName == "CertificateReward")
		{
			return true;
		}
		return false;
	}

	public static void CheckScriptInstruction(ref WorkerInstruction NewInstruction)
	{
		if (LoadJSON.m_LoadingVersion < 92 && NewInstruction.m_Variable2 == "FNOClothingClean")
		{
			NewInstruction.m_Variable2 = "";
		}
		if ((LoadJSON.m_LoadingVersion < 136 || (LoadJSON.m_LoadingVersion == 136 && LoadJSON.m_LoadingVersionFraction < 18)) && NewInstruction.m_Instruction == WorkerInstruction.Instruction.SetValue)
		{
			int.TryParse(NewInstruction.m_Variable1, out var result);
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(result, ErrorCheck: false);
			if (objectFromUniqueID != null)
			{
				int.TryParse(NewInstruction.m_Variable3, out var result2);
				ObjectType type = objectFromUniqueID.GetComponent<Converter>().m_Results[result2][0].m_Type;
				NewInstruction.m_Variable3 = ObjectTypeList.Instance.GetSaveNameFromIdentifier(type);
			}
		}
	}

	private static string UpdateFindAreaArgument(string OldArgument, int ObjectUID, out int NewObjectUID)
	{
		char[] separator = new char[1] { ' ' };
		string[] array = OldArgument.Split(separator);
		int num = GeneralUtils.IntParseFast(array[0]);
		int num2 = GeneralUtils.IntParseFast(array[1]);
		int num3 = GeneralUtils.IntParseFast(array[2]);
		int num4 = GeneralUtils.IntParseFast(array[3]);
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(ObjectUID, ErrorCheck: false);
		NewObjectUID = ObjectUID;
		if ((bool)objectFromUniqueID && Sign.GetIsTypeSign(objectFromUniqueID.m_TypeIdentifier))
		{
			Sign component = objectFromUniqueID.GetComponent<Sign>();
			num = component.m_TopLeft.x;
			num2 = component.m_TopLeft.y;
			num3 = component.m_BottomRight.x;
			num4 = component.m_BottomRight.y;
			NewObjectUID = 0;
		}
		if (num != num3 && ((uint)num & (true ? 1u : 0u)) != 0)
		{
			num++;
		}
		if (num2 != num4 && ((uint)num2 & (true ? 1u : 0u)) != 0)
		{
			num2++;
		}
		return num + " " + num2 + " " + num3 + " " + num4;
	}

	public static void CheckHighInstructionActioninfo(HighInstruction NewInstruction)
	{
		if (LoadJSON.m_LoadingVersion < 92)
		{
			ActionInfo actionInfo = NewInstruction.m_ActionInfo;
			if (actionInfo.m_ActionRequirement == "FNOClothingClean")
			{
				actionInfo.m_ActionRequirement = "";
			}
		}
		if (LoadJSON.m_LoadingVersion < 132 && NewInstruction.m_Type == HighInstruction.Type.FindNearestTile)
		{
			bool flag = false;
			if (NewInstruction.m_ActionInfo.m_ActionType == AFO.AT.Primary && NewInstruction.m_ActionInfo.m_ActionRequirement == Tile.GetNameFromType(Tile.TileType.Soil) && FarmerStateShovel.GetIsToolAcceptable(ObjectTypeList.Instance.GetIdentifierFromSaveName(NewInstruction.m_ActionInfo.m_Value)))
			{
				flag = true;
				NewInstruction.m_Argument = UpdateFindAreaArgument(NewInstruction.m_Argument, NewInstruction.m_ActionInfo.m_ObjectUID, out var NewObjectUID);
				NewInstruction.m_ActionInfo.m_ObjectUID = NewObjectUID;
			}
			if (!flag)
			{
				NewInstruction.m_Argument = NewInstruction.m_Argument + " " + HighInstruction.FindType.Full;
			}
			else
			{
				NewInstruction.m_Argument = NewInstruction.m_Argument + " " + HighInstruction.FindType.Even;
			}
		}
		if ((LoadJSON.m_LoadingVersion < 136 || (LoadJSON.m_LoadingVersion == 136 && LoadJSON.m_LoadingVersionFraction < 18)) && NewInstruction.m_Type == HighInstruction.Type.SetValue)
		{
			ActionInfo actionInfo2 = NewInstruction.m_ActionInfo;
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(actionInfo2.m_ObjectUID, ErrorCheck: false);
			if (objectFromUniqueID != null)
			{
				int.TryParse(actionInfo2.m_Value2, out var result);
				ObjectType type = objectFromUniqueID.GetComponent<Converter>().m_Results[result][0].m_Type;
				actionInfo2.m_Value2 = ObjectTypeList.Instance.GetSaveNameFromIdentifier(type);
			}
		}
	}

	public static void CheckAddTakeScript(List<WorkerInstruction> NewInstructions)
	{
		if (LoadJSON.m_LoadingVersion >= 118)
		{
			return;
		}
		int num = NewInstructions.Count;
		for (int i = 0; i < num; i++)
		{
			WorkerInstruction workerInstruction = NewInstructions[i];
			if (workerInstruction.m_Instruction == WorkerInstruction.Instruction.AddResource || workerInstruction.m_Instruction == WorkerInstruction.Instruction.TakeResource)
			{
				WorkerInstruction item = default(WorkerInstruction);
				item.Set(WorkerInstruction.Instruction.SetVariable);
				item.m_Variable1 = "Global5";
				if (workerInstruction.m_Instruction == WorkerInstruction.Instruction.TakeResource)
				{
					item.m_Variable2 = "0";
				}
				else
				{
					item.m_Variable2 = "1";
				}
				NewInstructions.Insert(i, item);
				num++;
				i++;
			}
		}
	}

	public static void CheckFindArea(WorkerScriptLocal NewScript, string[] GlobalVariables, Worker NewWorker)
	{
		if (LoadJSON.m_LoadingVersion >= 132)
		{
			return;
		}
		List<WorkerInstruction> instructions = NewScript.m_Script.m_Instructions;
		for (int i = 0; i < instructions.Count; i++)
		{
			WorkerInstruction workerInstruction = instructions[i];
			if (workerInstruction.m_Instruction != WorkerInstruction.Instruction.FindNearestTile)
			{
				continue;
			}
			bool flag = false;
			if (NewWorker.m_WorkerName == "Tree2 DigHoles1")
			{
				flag = flag;
			}
			WorkerInstruction value = new WorkerInstruction(instructions[i - 4]);
			WorkerInstruction value2 = new WorkerInstruction(instructions[i - 3]);
			if (workerInstruction.m_Variable4 == ActionType.UseInHands.ToString() && workerInstruction.m_Variable1 == Tile.GetNameFromType(Tile.TileType.Soil))
			{
				WorkerInstruction workerInstruction2 = instructions[i - 2];
				if (FarmerStateShovel.GetIsToolAcceptable(ObjectTypeList.Instance.GetIdentifierFromSaveName(workerInstruction2.m_Variable2)))
				{
					flag = true;
					int result = 0;
					int.TryParse(value2.m_Variable2, out result);
					value.m_Variable2 = UpdateFindAreaArgument(value.m_Variable2, result, out var NewObjectUID);
					value2.m_Variable2 = NewObjectUID.ToString();
				}
			}
			if (!flag)
			{
				ref string variable = ref value.m_Variable2;
				variable = variable + " " + HighInstruction.FindType.Full;
			}
			else
			{
				ref string variable2 = ref value.m_Variable2;
				variable2 = variable2 + " " + HighInstruction.FindType.Even;
			}
			instructions[i - 3] = value2;
			instructions[i - 4] = value;
			if (NewScript.m_CurrentInstruction >= i - 4 && NewScript.m_CurrentInstruction <= i)
			{
				GlobalVariables[3] = value.m_Variable2;
				GlobalVariables[6] = value2.m_Variable2;
			}
		}
	}

	public static void CheckScript(WorkerScriptLocal NewScript, string[] GlobalVariables, Worker NewWorker)
	{
		CheckAddTakeScript(NewScript.m_Script.m_Instructions);
		CheckFindArea(NewScript, GlobalVariables, NewWorker);
	}

	public static void CheckUsedTiles(Tile[] Data, int TilesWide, int TilesHigh)
	{
		if (LoadJSON.m_LoadingVersion >= 129)
		{
			return;
		}
		for (int i = 0; i < TilesHigh; i++)
		{
			for (int j = 0; j < TilesWide; j++)
			{
				int num = i * TilesWide + j;
				Tile.TileType tileType = Data[num].m_TileType;
				if (tileType == Tile.TileType.StoneUsed)
				{
					Data[num].m_TileType = Tile.TileType.Stone;
				}
				if (tileType == Tile.TileType.ClayUsed)
				{
					Data[num].m_TileType = Tile.TileType.Clay;
				}
			}
		}
	}

	private static void CheckPostQuests()
	{
		if (LoadJSON.m_LoadingVersion >= 134)
		{
			return;
		}
		Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("Worker");
		int value = 0;
		if (collection != null)
		{
			value = collection.Count;
		}
		Quest quest = QuestManager.Instance.GetQuest(Quest.ID.AcademyRobotics2);
		if (quest != null)
		{
			quest.CheckEvent(QuestEvent.Type.Make, Bot: false, ObjectType.Worker, null, value);
			if (quest.GetIsComplete())
			{
				QuestManager.Instance.CheatCompleteQuest(quest);
			}
		}
	}

	public static void CheckPostLoad()
	{
		CheckPostQuests();
	}
}
