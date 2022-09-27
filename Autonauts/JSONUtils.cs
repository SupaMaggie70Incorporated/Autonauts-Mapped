using SimpleJSON;
using UnityEngine;

public class JSONUtils : MonoBehaviour
{
	public static void Set(JSONNode Node, string Name, JSONNode NewNode)
	{
		Node[Name] = NewNode;
	}

	public static void Set(JSONNode Node, string Name, JSONArray NewNode)
	{
		Node[Name] = NewNode;
	}

	public static void Set(JSONNode Node, string Name, string Value)
	{
		Node[Name] = Value;
	}

	public static void Set(JSONNode Node, string Name, int Value)
	{
		Node[Name] = Value;
	}

	public static void Set(JSONNode Node, string Name, float Value)
	{
		Node[Name] = (int)(Value * 100f);
	}

	public static void Set(JSONNode Node, string Name, bool Value)
	{
		Node[Name] = Value;
	}

	public static JSONNode GetAsNode(JSONNode Node, string Name)
	{
		JSONNode jSONNode = Node[Name];
		if (jSONNode == null || jSONNode.IsNull)
		{
			return null;
		}
		return jSONNode;
	}

	public static JSONArray GetAsArray(JSONNode Node, string Name)
	{
		JSONNode jSONNode = Node[Name];
		if (jSONNode == null || jSONNode.IsNull)
		{
			return new JSONArray();
		}
		return jSONNode.AsArray;
	}

	public static string GetAsString(JSONNode Node, string Name, string DefaultValue)
	{
		JSONNode jSONNode = Node[Name];
		if (jSONNode == null || jSONNode.IsNull)
		{
			return DefaultValue;
		}
		return jSONNode;
	}

	public static int GetAsInt(JSONNode Node, string Name, int DefaultValue)
	{
		JSONNode jSONNode = Node[Name];
		if (jSONNode == null || jSONNode.IsNull)
		{
			return DefaultValue;
		}
		return jSONNode.AsInt;
	}

	public static float GetAsFloat(JSONNode Node, string Name, float DefaultValue)
	{
		JSONNode jSONNode = Node[Name];
		if (jSONNode == null || jSONNode.IsNull)
		{
			return DefaultValue;
		}
		return (float)jSONNode.AsInt / 100f;
	}

	public static bool GetAsBool(JSONNode Node, string Name, bool DefaultValue)
	{
		JSONNode jSONNode = Node[Name];
		if (jSONNode == null || jSONNode.IsNull)
		{
			return DefaultValue;
		}
		return jSONNode.AsBool;
	}
}
