using System.Collections.Generic;
using UnityEngine;

public class VariableManager : MonoBehaviour
{
	public struct Value
	{
		public int m_Int;

		public float m_Float;

		public string m_String;

		public List<object> m_List;
	}

	public static VariableManager Instance;

	private VariableData m_Data;

	private VariableDataIngredients m_DataIngredients;

	private VariableDataBuildings m_DataBuildings;

	private VariableDataIngredientsBuildings m_DataIngredientsBuildings;

	public VariableDataConverters m_DataConverters;

	private VariableDataIngredientsSpecial m_DataIngredientsSpecial;

	private VariableDataPrizes m_DataPrizes;

	private VariableDataPrizesIngredients m_DataPrizesIngredients;

	public Dictionary<string, Value> m_Variables { get; private set; }

	private void Awake()
	{
		Instance = this;
		m_Variables = new Dictionary<string, Value>();
		m_Data = new VariableData();
		m_Data.Init();
		m_DataIngredients = new VariableDataIngredients();
		m_DataIngredients.Init();
		m_DataBuildings = new VariableDataBuildings();
		m_DataBuildings.Init();
		m_DataIngredientsBuildings = new VariableDataIngredientsBuildings();
		m_DataIngredientsBuildings.Init();
		m_DataPrizes = new VariableDataPrizes();
		m_DataPrizes.Init();
		m_DataPrizesIngredients = new VariableDataPrizesIngredients();
		m_DataPrizesIngredients.Init();
		m_DataConverters = new VariableDataConverters();
		m_DataConverters.Init();
		m_DataIngredientsSpecial = new VariableDataIngredientsSpecial();
		m_DataIngredientsSpecial.Init();
	}

	public void ReInit()
	{
		m_Data.Init();
	}

	public void SetVariable(string Name, int Int)
	{
		if (!m_Variables.ContainsKey(Name))
		{
			m_Variables[Name] = default(Value);
		}
		Value value = m_Variables[Name];
		value.m_Int = Int;
		value.m_Float = Int;
		m_Variables[Name] = value;
	}

	public void SetVariable(string Name, float Float)
	{
		if (!m_Variables.ContainsKey(Name))
		{
			m_Variables[Name] = default(Value);
		}
		Value value = m_Variables[Name];
		value.m_Float = Float;
		value.m_Int = (int)Float;
		m_Variables[Name] = value;
	}

	public void SetVariable(string Name, string String)
	{
		if (!m_Variables.ContainsKey(Name))
		{
			m_Variables[Name] = default(Value);
		}
		Value value = m_Variables[Name];
		value.m_String = String;
		m_Variables[Name] = value;
	}

	public void SetVariable(string Name, List<object> NewList)
	{
		if (!m_Variables.ContainsKey(Name))
		{
			m_Variables[Name] = default(Value);
		}
		Value value = m_Variables[Name];
		value.m_List = NewList;
		m_Variables[Name] = value;
	}

	public void AddVariable(string Name, object AddValue)
	{
		if (!m_Variables.ContainsKey(Name))
		{
			m_Variables[Name] = default(Value);
		}
		Value value = m_Variables[Name];
		if (value.m_List == null)
		{
			value.m_List = new List<object>();
		}
		value.m_List.Add(AddValue);
		m_Variables[Name] = value;
	}

	public int GetVariableAsInt(string Name, bool CheckValid = true)
	{
		if (m_Variables.ContainsKey(Name))
		{
			return m_Variables[Name].m_Int;
		}
		if (CheckValid)
		{
			Debug.Log("Couldn't find variable " + Name);
		}
		return 0;
	}

	public float GetVariableAsFloat(string Name, bool CheckValid = true)
	{
		if (m_Variables.ContainsKey(Name))
		{
			return m_Variables[Name].m_Float;
		}
		if (CheckValid)
		{
			Debug.Log("Couldn't find variable " + Name);
		}
		return 0f;
	}

	public string GetVariableAsString(string Name, bool CheckValid = true)
	{
		if (m_Variables.ContainsKey(Name))
		{
			return m_Variables[Name].m_String;
		}
		if (CheckValid)
		{
			Debug.Log("Couldn't find variable " + Name);
		}
		return "";
	}

	public List<object> GetVariableAsList(string Name, bool CheckValid = true)
	{
		if (m_Variables.ContainsKey(Name))
		{
			return m_Variables[Name].m_List;
		}
		if (CheckValid)
		{
			Debug.Log("Couldn't find variable " + Name);
		}
		return null;
	}

	public string GetVariableName(Farmer.State Action, ObjectType TargetType, ObjectType ToolType)
	{
		return string.Concat(string.Concat(Action.ToString() + ".", ObjectTypeList.Instance.GetSaveNameFromIdentifier(TargetType), "."), ObjectTypeList.Instance.GetSaveNameFromIdentifier(ToolType));
	}

	public void SetVariable(Farmer.State Action, ObjectType TargetType, ObjectType ToolType, int Int)
	{
		string variableName = GetVariableName(Action, TargetType, ToolType);
		SetVariable(variableName, Int);
	}

	public int GetVariableAsInt(Farmer.State Action, ObjectType TargetType, ObjectType ToolType, bool CheckValid = true)
	{
		string variableName = GetVariableName(Action, TargetType, ToolType);
		return GetVariableAsInt(variableName, CheckValid);
	}

	public string GetVariableName(Farmer.State Action, Tile.TileType NewType, ObjectType ToolType)
	{
		return string.Concat(string.Concat(Action.ToString() + ".", NewType.ToString(), "."), ObjectTypeList.Instance.GetSaveNameFromIdentifier(ToolType));
	}

	public void SetVariable(Farmer.State Action, Tile.TileType NewType, ObjectType ToolType, int Int)
	{
		string variableName = GetVariableName(Action, NewType, ToolType);
		SetVariable(variableName, Int);
	}

	public int GetVariableAsInt(Farmer.State Action, Tile.TileType NewType, ObjectType ToolType, bool CheckValid = true)
	{
		string variableName = GetVariableName(Action, NewType, ToolType);
		return GetVariableAsInt(variableName, CheckValid);
	}

	public string GetVariableName(ObjectType NewType, string VariableName)
	{
		return string.Concat(ObjectTypeList.Instance.GetSaveNameFromIdentifier(NewType) + ".", VariableName);
	}

	public void SetVariable(ObjectType NewType, string VariableName, int Int)
	{
		string variableName = GetVariableName(NewType, VariableName);
		SetVariable(variableName, Int);
	}

	public void SetVariable(ObjectType NewType, string VariableName, float Float)
	{
		string variableName = GetVariableName(NewType, VariableName);
		SetVariable(variableName, Float);
	}

	public void SetVariable(ObjectType NewType, string VariableName, string String)
	{
		string variableName = GetVariableName(NewType, VariableName);
		SetVariable(variableName, String);
	}

	public int GetVariableAsInt(ObjectType NewType, string VariableName, bool CheckValid = true)
	{
		string variableName = GetVariableName(NewType, VariableName);
		return GetVariableAsInt(variableName, CheckValid);
	}

	public float GetVariableAsFloat(ObjectType NewType, string VariableName, bool CheckValid = true)
	{
		string variableName = GetVariableName(NewType, VariableName);
		return GetVariableAsFloat(variableName, CheckValid);
	}

	public string GetVariableAsString(ObjectType NewType, string VariableName, bool CheckValid = true)
	{
		string variableName = GetVariableName(NewType, VariableName);
		return GetVariableAsString(variableName, CheckValid);
	}

	public string GetVariableName(Quest.ID NewType, string VariableName)
	{
		return string.Concat(NewType.ToString() + ".", VariableName);
	}

	public void SetVariable(Quest.ID NewType, string VariableName, int Int)
	{
		string variableName = GetVariableName(NewType, VariableName);
		SetVariable(variableName, Int);
	}

	public void SetVariable(Quest.ID NewType, string VariableName, float Float)
	{
		string variableName = GetVariableName(NewType, VariableName);
		SetVariable(variableName, Float);
	}

	public void SetVariable(Quest.ID NewType, string VariableName, string String)
	{
		string variableName = GetVariableName(NewType, VariableName);
		SetVariable(variableName, String);
	}

	public int GetVariableAsInt(Quest.ID NewType, string VariableName, bool CheckValid = true)
	{
		string variableName = GetVariableName(NewType, VariableName);
		return GetVariableAsInt(variableName, CheckValid);
	}

	public float GetVariableAsFloat(Quest.ID NewType, string VariableName, bool CheckValid = true)
	{
		string variableName = GetVariableName(NewType, VariableName);
		return GetVariableAsFloat(variableName, CheckValid);
	}

	public string GetVariableAsString(Quest.ID NewType, string VariableName, bool CheckValid = true)
	{
		string variableName = GetVariableName(NewType, VariableName);
		return GetVariableAsString(variableName, CheckValid);
	}

	public ConverterResults GetResultsForBuilding(ObjectType BuildingType)
	{
		return m_DataConverters.GetResults(BuildingType);
	}

	public ObjectType GetConverterForObject(ObjectType NewType)
	{
		if (m_DataConverters.m_Results == null)
		{
			return ObjectTypeList.m_Total;
		}
		return m_DataConverters.GetConverterForObject(NewType);
	}

	public IngredientRequirement[] GetSpecialIngredients(ObjectType NewType)
	{
		return m_DataIngredientsSpecial.GetIngredients(NewType);
	}

	public ObjectType GetSpecialConverterType(ObjectType NewType)
	{
		return m_DataIngredientsSpecial.GetConverterType(NewType);
	}
}
