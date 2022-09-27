using System;
using System.Collections.Generic;
using MoonSharp.Interpreter.Interop;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SimpleJSON;

public class ModBotScript
{
	public class InvalidInstructionException : ArgumentException
	{
		public InvalidInstructionException()
		{
		}

		public InvalidInstructionException(string message)
			: base(message)
		{
		}

		public InvalidInstructionException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}

	public struct QuarantinedInstruction
	{
		[JsonProperty("Type")]
		public string m_Type;

		[JsonProperty("ArgName")]
		public string m_Argument;

		[JsonProperty("Line")]
		public int m_ScriptLineNumber;

		[JsonProperty("OT")]
		public string m_ObjectType;

		[JsonProperty("UID")]
		public int m_ObjectUID;

		public int X;

		public int Y;

		[JsonProperty("V1")]
		public string m_Value;

		[JsonProperty("V2")]
		public string m_Value2;

		[JsonProperty("A")]
		[JsonConverter(typeof(StringEnumConverter))]
		public ActionType m_Action;

		[JsonProperty("R")]
		public string m_ActionRequirement;

		[JsonProperty("AT")]
		public AFO.AT m_ActionType;

		[JsonProperty("AOT")]
		public string m_ActionObjectType;

		[JsonProperty("Children")]
		public List<QuarantinedInstruction> m_Children;

		[JsonProperty("Children2")]
		public List<QuarantinedInstruction> m_Children2;
	}

	private string m_JSONString = "[]";

	private int m_MapWidth;

	private int m_MapHeight;

	[JsonProperty("HighInstructionsArray")]
	public List<QuarantinedInstruction> m_ModInstructions;

	[MoonSharpVisible(false)]
	public ModBotScript(string JSONString)
	{
		m_ModInstructions = new List<QuarantinedInstruction>();
		Init(JSONString);
	}

	[MoonSharpVisible(false)]
	public ModBotScript(JSONNode Node)
	{
		m_ModInstructions = new List<QuarantinedInstruction>();
		Init(Node["HighInstructionsArray"].ToString());
	}

	private void Init(string JSONString)
	{
		m_MapWidth = TileManager.Instance.m_TilesWide;
		m_MapHeight = TileManager.Instance.m_TilesHigh;
		JsonSerializerSettings settings = new JsonSerializerSettings
		{
			NullValueHandling = NullValueHandling.Ignore
		};
		m_ModInstructions = JsonConvert.DeserializeObject<List<QuarantinedInstruction>>(JSONString, settings);
		if (m_ModInstructions != null)
		{
			ValidateScript(m_ModInstructions);
			m_JSONString = JsonConvert.SerializeObject(m_ModInstructions, settings);
		}
	}

	private void ValidateScript(List<QuarantinedInstruction> Instructions)
	{
		foreach (QuarantinedInstruction Instruction in Instructions)
		{
			string text = ValidateInstruction(Instruction);
			if (text != "")
			{
				m_JSONString = "[]";
				throw new InvalidInstructionException(text + ": " + JsonConvert.SerializeObject(Instruction));
			}
			if (Instruction.m_Children != null && Instruction.m_Children.Count > 0)
			{
				ValidateScript(Instruction.m_Children);
			}
			if (Instruction.m_Children2 != null && Instruction.m_Children2.Count > 0)
			{
				ValidateScript(Instruction.m_Children2);
			}
		}
	}

	[MoonSharpVisible(false)]
	public override string ToString()
	{
		return m_JSONString;
	}

	[MoonSharpVisible(false)]
	public JSONNode ToJSONNode()
	{
		return JSON.Parse("{\"HighInstructionsArray\":" + m_JSONString + "}");
	}

	private string ValidateInstruction(QuarantinedInstruction Instruction)
	{
		bool flag = false;
		HighInstructionInfo[] info = HighInstruction.m_Info;
		foreach (HighInstructionInfo highInstructionInfo in info)
		{
			if (Instruction.m_Type == highInstructionInfo.m_Name)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			return "Invalid instruction \"Type\" " + Instruction.m_Type;
		}
		if (Instruction.m_Type == "InstructionRepeat")
		{
			if (!Instruction.m_Argument.StartsWith("Repeat"))
			{
				return "Invalid ArgName (expected \"repeat\" ConditionType) for Type " + Instruction.m_Type;
			}
			if (!Enum.TryParse<HighInstruction.ConditionType>(Instruction.m_Argument.Substring("Repeat".Length), out var result))
			{
				return "Invalid ArgName (expected ConditionType) for Type " + Instruction.m_Type;
			}
			if (Array.IndexOf(HighInstruction.m_RepeatTypes, result) == -1)
			{
				return "Invalid ArgName (expected \"repeat\" ConditionType) for Type " + Instruction.m_Type;
			}
		}
		else if (Instruction.m_Type.StartsWith("InstructionIf"))
		{
			if (!Instruction.m_Argument.StartsWith("If"))
			{
				return "Invalid ArgName (expected \"if\" ConditionType) for Type " + Instruction.m_Type;
			}
			if (!Enum.TryParse<HighInstruction.ConditionType>(Instruction.m_Argument.Substring("If".Length), out var result2))
			{
				return "Invalid ArgName (expected ConditionType) for Type " + Instruction.m_Type;
			}
			if (Array.IndexOf(HighInstruction.m_IfTypes, result2) == -1)
			{
				return "Invalid ArgName (expected \"if\" ConditionType) for Type " + Instruction.m_Type;
			}
		}
		if (Instruction.m_Type.StartsWith("InstructionFind"))
		{
			string text = "";
			char[] separator = new char[1] { ' ' };
			string[] array = Instruction.m_Argument.Split(separator);
			if (array.Length < 4 || array.Length > 5)
			{
				return "Invalid ArgName for " + Instruction.m_Type;
			}
			int[] array2 = new int[4];
			for (int j = 0; j < 4; j++)
			{
				int.TryParse(array[j], out array2[j]);
				if (array2[j] < 0)
				{
					array2[j] = 0;
				}
			}
			if (array2[0] >= m_MapWidth)
			{
				array2[0] = m_MapWidth - 1;
			}
			if (array2[1] >= m_MapHeight)
			{
				array2[1] = m_MapHeight - 1;
			}
			if (array2[2] >= m_MapWidth)
			{
				array2[0] = m_MapWidth - 1;
			}
			if (array2[3] >= m_MapHeight)
			{
				array2[3] = m_MapHeight - 1;
			}
			text = array2[0] + " " + array2[1] + " " + array2[2] + " " + array2[3];
			if (array.Length == 5)
			{
				text = text + " " + array[4];
			}
			if (text != Instruction.m_Argument)
			{
				return "Invalid ArgName for " + Instruction.m_Type + " - search area invalid or out of bounds";
			}
		}
		if (Instruction.X < 0 || Instruction.X >= m_MapWidth)
		{
			return "X out of bounds";
		}
		if (Instruction.Y < 0 || Instruction.Y >= m_MapHeight)
		{
			return "Y out of bounds";
		}
		if (Instruction.m_Type == "InstructionFindNearestTile" && (!Instruction.m_ActionRequirement.StartsWith("Tile") || !Enum.TryParse<Tile.TileType>(Instruction.m_ActionRequirement.Substring("Tile".Length), out var _)))
		{
			return "Invalid requirement attribute \"R\" for " + Instruction.m_Type + ", expecting a Tile Type";
		}
		if (Instruction.m_ObjectType != "Nothing")
		{
			ObjectType result4 = ObjectType.Nothing;
			if (!Enum.TryParse<ObjectType>(Instruction.m_ObjectType, out result4))
			{
				result4 = ModManager.Instance.GetModObjectTypeFromName(Instruction.m_ObjectType);
			}
			if (result4 == ObjectType.Nothing)
			{
				return "Invalid object type attribute \"OT\": \"" + Instruction.m_ObjectType + "\"";
			}
		}
		if (Instruction.m_ActionObjectType != "Nothing")
		{
			ObjectType result5 = ObjectType.Nothing;
			if (!Enum.TryParse<ObjectType>(Instruction.m_ActionObjectType, out result5))
			{
				result5 = ModManager.Instance.GetModObjectTypeFromName(Instruction.m_ActionObjectType);
			}
			if (result5 == ObjectType.Nothing)
			{
				return "Invalid action object type attribute \"AOT\": \"" + Instruction.m_ActionObjectType + "\"";
			}
		}
		return "";
	}
}
