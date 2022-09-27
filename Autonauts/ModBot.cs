using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using Newtonsoft.Json;
using SimpleJSON;
using UnityEngine;

[MoonSharpUserData]
public class ModBot
{
	public int SpawnBot(string Name, int HeadLevel = 0, int FrameLevel = 0, int DriveLevel = 0, int HeadVariant = 0, int FrameVariant = 0, int DriveVariant = 0, int PositionX = 0, int PositionY = 0)
	{
		if (!GeneralUtils.m_InGame)
		{
			return -1;
		}
		try
		{
			TileCoord tileCoord = new TileCoord(PositionX, PositionY);
			if (!tileCoord.GetIsValid())
			{
				string descriptionOverride = "Error: ModBot.SpawnBot - Canot spawn bot outside of map limits! (" + PositionX + "," + PositionY + ")";
				ModManager.Instance.SetErrorLua(ModManager.ErrorState.Error_Misc, descriptionOverride);
				return -1;
			}
			Worker component = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.Worker, tileCoord.ToWorldPositionTileCentered(), Quaternion.identity).GetComponent<Worker>();
			if (HeadLevel == 0)
			{
				component.SetHead(ObjectType.WorkerHeadMk0);
			}
			if (HeadLevel == 1)
			{
				switch (HeadVariant)
				{
				case 0:
					component.SetHead(ObjectType.WorkerHeadMk1);
					break;
				case 1:
					component.SetHead(ObjectType.WorkerHeadMk1Variant1);
					break;
				case 2:
					component.SetHead(ObjectType.WorkerHeadMk1Variant2);
					break;
				case 3:
					component.SetHead(ObjectType.WorkerHeadMk1Variant3);
					break;
				case 4:
					component.SetHead(ObjectType.WorkerHeadMk1Variant4);
					break;
				}
			}
			if (HeadLevel == 2)
			{
				switch (HeadVariant)
				{
				case 0:
					component.SetHead(ObjectType.WorkerHeadMk2);
					break;
				case 1:
					component.SetHead(ObjectType.WorkerHeadMk2Variant1);
					break;
				case 2:
					component.SetHead(ObjectType.WorkerHeadMk2Variant2);
					break;
				case 3:
					component.SetHead(ObjectType.WorkerHeadMk2Variant3);
					break;
				case 4:
					component.SetHead(ObjectType.WorkerHeadMk2Variant4);
					break;
				}
			}
			if (HeadLevel == 3)
			{
				switch (HeadVariant)
				{
				case 0:
					component.SetHead(ObjectType.WorkerHeadMk3);
					break;
				case 1:
					component.SetHead(ObjectType.WorkerHeadMk3Variant1);
					break;
				case 2:
					component.SetHead(ObjectType.WorkerHeadMk3Variant2);
					break;
				case 3:
					component.SetHead(ObjectType.WorkerHeadMk3Variant3);
					break;
				case 4:
					component.SetHead(ObjectType.WorkerHeadMk3Variant4);
					break;
				}
			}
			if (FrameLevel == 0)
			{
				component.SetFrame(ObjectType.WorkerFrameMk0);
			}
			if (FrameLevel == 1)
			{
				switch (FrameVariant)
				{
				case 0:
					component.SetFrame(ObjectType.WorkerFrameMk1);
					break;
				case 1:
					component.SetFrame(ObjectType.WorkerFrameMk1Variant1);
					break;
				case 2:
					component.SetFrame(ObjectType.WorkerFrameMk1Variant2);
					break;
				case 3:
					component.SetFrame(ObjectType.WorkerFrameMk1Variant3);
					break;
				case 4:
					component.SetFrame(ObjectType.WorkerFrameMk1Variant4);
					break;
				}
			}
			if (FrameLevel == 2)
			{
				switch (FrameVariant)
				{
				case 0:
					component.SetFrame(ObjectType.WorkerFrameMk2);
					break;
				case 1:
					component.SetFrame(ObjectType.WorkerFrameMk2Variant1);
					break;
				case 2:
					component.SetFrame(ObjectType.WorkerFrameMk2Variant2);
					break;
				case 3:
					component.SetFrame(ObjectType.WorkerFrameMk2Variant3);
					break;
				case 4:
					component.SetFrame(ObjectType.WorkerFrameMk2Variant4);
					break;
				}
			}
			if (FrameLevel == 3)
			{
				switch (FrameVariant)
				{
				case 0:
					component.SetFrame(ObjectType.WorkerFrameMk3);
					break;
				case 1:
					component.SetFrame(ObjectType.WorkerFrameMk3Variant1);
					break;
				case 2:
					component.SetFrame(ObjectType.WorkerFrameMk3Variant2);
					break;
				case 3:
					component.SetFrame(ObjectType.WorkerFrameMk3Variant3);
					break;
				case 4:
					component.SetFrame(ObjectType.WorkerFrameMk3Variant4);
					break;
				}
			}
			if (DriveLevel == 0)
			{
				component.SetDrive(ObjectType.WorkerDriveMk0);
			}
			if (DriveLevel == 1)
			{
				switch (DriveVariant)
				{
				case 0:
					component.SetDrive(ObjectType.WorkerDriveMk1);
					break;
				case 1:
					component.SetDrive(ObjectType.WorkerDriveMk1Variant1);
					break;
				case 2:
					component.SetDrive(ObjectType.WorkerDriveMk1Variant2);
					break;
				case 3:
					component.SetDrive(ObjectType.WorkerDriveMk1Variant3);
					break;
				case 4:
					component.SetDrive(ObjectType.WorkerDriveMk1Variant4);
					break;
				}
			}
			if (DriveLevel == 2)
			{
				switch (DriveVariant)
				{
				case 0:
					component.SetDrive(ObjectType.WorkerDriveMk2);
					break;
				case 1:
					component.SetDrive(ObjectType.WorkerDriveMk2Variant1);
					break;
				case 2:
					component.SetDrive(ObjectType.WorkerDriveMk2Variant2);
					break;
				case 3:
					component.SetDrive(ObjectType.WorkerDriveMk2Variant3);
					break;
				case 4:
					component.SetDrive(ObjectType.WorkerDriveMk2Variant4);
					break;
				}
			}
			if (DriveLevel == 3)
			{
				switch (DriveVariant)
				{
				case 0:
					component.SetDrive(ObjectType.WorkerDriveMk3);
					break;
				case 1:
					component.SetDrive(ObjectType.WorkerDriveMk3Variant1);
					break;
				case 2:
					component.SetDrive(ObjectType.WorkerDriveMk3Variant2);
					break;
				case 3:
					component.SetDrive(ObjectType.WorkerDriveMk3Variant3);
					break;
				case 4:
					component.SetDrive(ObjectType.WorkerDriveMk3Variant4);
					break;
				}
			}
			component.SetWorkerName(Name);
			component.UpdateModel();
			return component.m_UniqueID;
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModBot.SpawnBot Error: " + ex.ToString());
		}
		return -1;
	}

	public List<string> GetBotGroupNames()
	{
		List<string> list = new List<string>();
		try
		{
			foreach (WorkerGroup group in WorkerGroupManager.Instance.m_Groups)
			{
				list.Add(group.m_Name);
			}
			return list;
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModBot.GetBotGroupNames Error: " + ex.ToString());
			return list;
		}
	}

	public List<int> GetAllBotUIDs()
	{
		List<int> list = new List<int>();
		try
		{
			Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("Worker");
			if (collection != null)
			{
				foreach (KeyValuePair<BaseClass, int> item in collection)
				{
					Worker component = item.Key.GetComponent<Worker>();
					list.Add(component.m_UniqueID);
				}
				return list;
			}
			return list;
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModBot.GetAllBotUIDs Error: " + ex.ToString());
			return list;
		}
	}

	public List<int> GetAllBotUIDsInGroup(string GroupName)
	{
		List<int> list = new List<int>();
		try
		{
			foreach (WorkerGroup group in WorkerGroupManager.Instance.m_Groups)
			{
				if (!(group.m_Name == GroupName))
				{
					continue;
				}
				foreach (int workerUID in group.m_WorkerUIDs)
				{
					list.Add(workerUID);
				}
			}
			return list;
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModBot.GetAllBotUIDsInGroup Error: " + ex.ToString());
			return list;
		}
	}

	public void MoveTo(int UID, int x, int y)
	{
		Worker worker = null;
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
		if (objectFromUniqueID != null)
		{
			worker = objectFromUniqueID.GetComponent<Worker>();
		}
		if (worker != null && new TileCoord(x, y).GetIsValid() && worker.m_State == Farmer.State.None)
		{
			worker.SendAction(new ActionInfo(ActionType.MoveTo, new TileCoord(x, y)));
		}
	}

	public void MoveTo(string UID, int x, int y)
	{
	}

	public void SetName(int UID, string NewName)
	{
		Worker worker = null;
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
		if (objectFromUniqueID != null)
		{
			worker = objectFromUniqueID.GetComponent<Worker>();
		}
		if (worker != null)
		{
			worker.SetWorkerName(NewName);
		}
	}

	public void SetName(string UID, string NewName)
	{
	}

	public string GetName(int UID)
	{
		try
		{
			Worker worker = null;
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
			if (objectFromUniqueID != null)
			{
				worker = objectFromUniqueID.GetComponent<Worker>();
			}
			if (worker != null)
			{
				return worker.GetWorkerName();
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModBot.GetName Error: " + ex.ToString());
		}
		return "";
	}

	public string GetName(string UID)
	{
		return "";
	}

	public Table GetHeldObjectUIDs(int UID)
	{
		Table table = new Table(ModManager.Instance.GetLastCalledScript());
		try
		{
			Worker worker = null;
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
			if (objectFromUniqueID != null)
			{
				worker = objectFromUniqueID.GetComponent<Worker>();
				if (worker != null)
				{
					foreach (Holdable item in worker.m_FarmerCarry.m_CarryObject)
					{
						if ((bool)item)
						{
							table.Append(DynValue.NewNumber(item.m_UniqueID));
						}
					}
					return table;
				}
				return table;
			}
			return table;
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModBot.GetHeldObjectUIDs Error: " + ex.ToString());
			return table;
		}
	}

	public Table GetHeldObjectUIDs(string UID)
	{
		return new Table(ModManager.Instance.GetLastCalledScript(), DynValue.NewNumber(-1.0));
	}

	public Table GetAllHeldObjectsUIDs(int UID)
	{
		if (ObjectTypeList.Instance.GetObjectFromUniqueID(UID) == null)
		{
			return new Table(ModManager.Instance.GetLastCalledScript(), DynValue.NewNumber(-1.0));
		}
		return GetHeldObjectUIDs(UID);
	}

	public string GetState(int UID)
	{
		try
		{
			Worker worker = null;
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
			if (objectFromUniqueID != null)
			{
				worker = objectFromUniqueID.GetComponent<Worker>();
				if (worker != null)
				{
					string text = worker.m_State.ToString();
					if (worker.m_WorkerInterpreter.m_Paused)
					{
						return "Paused";
					}
					if (text == "None" && worker.m_Energy <= 0f && worker.m_WorkerIndicator.m_State == WorkerStatusIndicator.State.NoEnergy)
					{
						return "Discharged";
					}
					return text;
				}
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModBot.GetState Error: " + ex.ToString());
		}
		return "";
	}

	public string GetState(string UID)
	{
		return "";
	}

	public bool GetIsRunningScript(int UID)
	{
		try
		{
			Worker worker = null;
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
			if (objectFromUniqueID != null)
			{
				worker = objectFromUniqueID.GetComponent<Worker>();
				if (worker != null)
				{
					return worker.m_WorkerInterpreter.GetCurrentScript() != null;
				}
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModBot.GetIsRunningScript Error: " + ex.ToString());
		}
		return false;
	}

	public bool GetIsRunningScript(string UID)
	{
		return false;
	}

	public bool GetIsLearning(int UID)
	{
		try
		{
			Worker worker = null;
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
			if (objectFromUniqueID != null)
			{
				worker = objectFromUniqueID.GetComponent<Worker>();
				if (worker != null)
				{
					return worker.m_Learning;
				}
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModBot.GetIsLearning Error: " + ex.ToString());
		}
		return false;
	}

	public bool GetIsLearning(string UID)
	{
		return false;
	}

	public Table GetParts(int UID)
	{
		try
		{
			Table table = new Table(ModManager.Instance.GetLastCalledScript());
			Worker worker = null;
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
			if (objectFromUniqueID != null)
			{
				worker = objectFromUniqueID.GetComponent<Worker>();
				if (worker != null)
				{
					table.Append(DynValue.NewString(worker.m_Head.ToString()));
					table.Append(DynValue.NewString(worker.m_Frame.ToString()));
					table.Append(DynValue.NewString(worker.m_Drive.ToString()));
					return table;
				}
			}
		}
		catch (Exception ex)
		{
			ModManager.Instance.WriteModError("ModBot.GetParts Error: " + ex.ToString());
		}
		return new Table(ModManager.Instance.GetLastCalledScript(), DynValue.NewString(""), DynValue.NewString(""), DynValue.NewString(""));
	}

	public Table GetParts(string UID)
	{
		return new Table(ModManager.Instance.GetLastCalledScript(), DynValue.NewString(""), DynValue.NewString(""), DynValue.NewString(""));
	}

	public void DropAllHeldObjects(int UID)
	{
		Worker worker = null;
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
		if (objectFromUniqueID != null)
		{
			worker = objectFromUniqueID.GetComponent<Worker>();
		}
		if (worker != null && (bool)worker.m_FarmerCarry && worker.m_FarmerCarry.m_CarryObject.Count > 0)
		{
			worker.m_FarmerCarry.DropAllObjects();
		}
	}

	public void DropAllHeldObjects(string UID)
	{
	}

	public void DropAllUpgrades(int UID)
	{
		Worker worker = null;
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
		if (objectFromUniqueID != null)
		{
			worker = objectFromUniqueID.GetComponent<Worker>();
		}
		if (worker != null && (bool)worker.m_FarmerUpgrades)
		{
			worker.m_FarmerUpgrades.DropAllObjects();
		}
	}

	public void DropAllUpgrades(string UID)
	{
	}

	public string GetScriptSavegameFormat(int UID, bool LogErrors = false)
	{
		string text = "ModBot.GetScriptSavegameFormat Error: ";
		string text2 = "[]";
		try
		{
			Worker worker = null;
			HighInstructionList highInstructionList = null;
			JSONObject jSONObject = new JSONObject();
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
			if (objectFromUniqueID != null)
			{
				worker = objectFromUniqueID.GetComponent<Worker>();
				if (worker != null && worker.m_WorkerInterpreter != null && worker.m_WorkerInterpreter.m_HighInstructions != null && worker.m_WorkerInterpreter.m_HighInstructions.m_List != null && worker.m_WorkerInterpreter.m_HighInstructions.m_List.Count > 0)
				{
					highInstructionList = worker.m_WorkerInterpreter.m_HighInstructions;
				}
			}
			if (highInstructionList != null)
			{
				highInstructionList.Save(jSONObject);
				text2 = jSONObject["HighInstructionsArray"].ToString();
				try
				{
					text2 = new ModBotScript(text2).ToString();
				}
				catch (ModBotScript.InvalidInstructionException ex)
				{
					ModManager.Instance.WriteModError(text + "Encountered a legitimate HighInstruction that violates the rules for a QuarantinedInstruction (validation is flawed).\n" + ex.ToString());
					if (LogErrors)
					{
						ModManager.Instance.WriteModDebug(text + "Unsupported script instruction. This script can not be validated, attempting to save it to a bot will fail. " + ex.Message);
					}
				}
				catch (JsonSerializationException ex2)
				{
					ModManager.Instance.WriteModError(text + "Failed to ensure HighInstructionList.Save returned safe JSON (a QuarantinedInstruction attribute may have the wrong type).\n" + ex2.ToString());
					if (LogErrors)
					{
						ModManager.Instance.WriteModDebug(text + "Unsupported script structure. This script can not be validated, attempting to save it to a bot will fail. " + ex2.Message);
					}
				}
			}
		}
		catch (Exception ex3)
		{
			ModManager.Instance.WriteModError(text + ex3.ToString());
		}
		if (text2 == "[]" && LogErrors)
		{
			ModManager.Instance.WriteModDebug(text + "Could not find bot (" + UID + ") or bot has no script.");
		}
		return text2;
	}

	public string GetScriptSavegameFormat(string UID, bool LogErrors = false)
	{
		if (LogErrors)
		{
			ModManager.Instance.WriteModDebug("ModBot.GetScriptSavegameFormat Error: UID must be a numerical.");
		}
		return "[]";
	}

	public bool SetScriptSavegameFormat(int UID, string JSON, bool LogErrors = false)
	{
		string text = "ModBot.SetScriptSavegameFormat Error: ";
		try
		{
			Worker worker = null;
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
			if (objectFromUniqueID != null)
			{
				worker = objectFromUniqueID.GetComponent<Worker>();
				if (worker != null && worker.m_WorkerInterpreter != null)
				{
					if (!worker.GetIsDoingSomething() && !worker.m_Learning)
					{
						try
						{
							ModBotScript modBotScript = new ModBotScript(JSON);
							worker.StopAllScripts();
							worker.m_WorkerInterpreter.m_HighInstructions.Load(modBotScript.ToJSONNode());
							if (TeachWorkerScriptEdit.Instance.m_CurrentTarget == worker)
							{
								TeachWorkerScriptEdit.Instance.UpdateTargetInstructions();
							}
							return true;
						}
						catch (JsonSerializationException ex)
						{
							if (LogErrors)
							{
								ModManager.Instance.WriteModDebug(text + "Unsupported script structure. " + ex.Message);
							}
						}
						catch (ModBotScript.InvalidInstructionException ex2)
						{
							if (LogErrors)
							{
								ModManager.Instance.WriteModDebug(text + "Unsupported script instruction. " + ex2.Message);
							}
						}
					}
					else if (LogErrors)
					{
						ModManager.Instance.WriteModDebug(text + "Bot (" + UID + ") is busy.");
					}
				}
				else if (LogErrors)
				{
					ModManager.Instance.WriteModDebug(text + "Could not find bot (" + UID + ").");
				}
			}
		}
		catch (Exception ex3)
		{
			ModManager.Instance.WriteModError(text + ex3.ToString());
		}
		return false;
	}

	public bool SetScriptSavegameFormat(string UID, string JSON, bool LogErrors = false)
	{
		if (LogErrors)
		{
			ModManager.Instance.WriteModDebug("ModBot.SetScriptSavegameFormat Error: UID must be a numerical.");
		}
		return false;
	}

	public void StopScript(int UID, bool Graceful = false)
	{
		Worker worker = null;
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
		if (!(objectFromUniqueID != null))
		{
			return;
		}
		worker = objectFromUniqueID.GetComponent<Worker>();
		if (!(worker != null) || !(worker.m_WorkerInterpreter != null))
		{
			return;
		}
		if (Graceful)
		{
			WorkerScriptLocal currentScript = worker.m_WorkerInterpreter.GetCurrentScript();
			if (currentScript == null || currentScript.m_Script == null)
			{
				return;
			}
			for (int i = 0; i < currentScript.m_Script.m_Instructions.Count; i++)
			{
				WorkerInstruction workerInstruction = currentScript.m_Script.m_Instructions[i];
				if (workerInstruction.m_Instruction == WorkerInstruction.Instruction.Label && workerInstruction.m_Variable1 == "Label1")
				{
					WorkerInstruction value = default(WorkerInstruction);
					value.Set(WorkerInstruction.Instruction.JumpTo);
					value.m_Variable1 = "Label0";
					currentScript.m_Script.m_Instructions[i + 1] = value;
				}
			}
		}
		else
		{
			worker.StopAllScripts();
		}
	}

	public void StopScript(string UID, bool Graceful = false)
	{
	}

	public void RechargeBot(int UID)
	{
		Worker worker = null;
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
		if (objectFromUniqueID != null)
		{
			worker = objectFromUniqueID.GetComponent<Worker>();
			if (worker != null)
			{
				worker.Recharge();
			}
		}
	}

	public void RechargeBot(string UID)
	{
	}

	public void StartScript(int UID)
	{
		Worker worker = null;
		BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(UID, ErrorCheck: false);
		if (objectFromUniqueID != null)
		{
			worker = objectFromUniqueID.GetComponent<Worker>();
			if (worker != null && worker.m_WorkerInterpreter != null && !worker.GetIsDoingSomething())
			{
				worker.m_WorkerInterpreter.StartScript();
			}
		}
	}

	public void StartScript(string UID)
	{
	}
}
