using System;

public class AFO
{
	public enum AT
	{
		Primary,
		Secondary,
		AltPrimary,
		AltSecondary,
		Total
	}

	public enum Stage
	{
		Start,
		Use,
		End,
		Abort,
		Total
	}

	public Actionable m_Object;

	public ObjectType m_ObjectType;

	public Actionable m_Actioner;

	public AT m_ActionType;

	public Stage m_Stage;

	public string m_RequirementsIn;

	public TileCoord m_Position;

	public Farmer.State m_FarmerState;

	public Action<AFO> m_StartAction;

	public Action<AFO> m_UseAction;

	public Action<AFO> m_EndAction;

	public Action<AFO> m_AbortAction;

	public string m_RequirementsOut;

	public bool m_AdjacentTile;

	public int m_MoveRange;

	public AFO()
	{
	}

	public AFO(AFO Original)
	{
		Clone(Original);
	}

	public void Clone(AFO Original)
	{
		m_Object = Original.m_Object;
		m_ObjectType = Original.m_ObjectType;
		m_Actioner = Original.m_Actioner;
		m_ActionType = Original.m_ActionType;
		m_RequirementsIn = Original.m_RequirementsIn;
		m_Position = Original.m_Position;
		m_FarmerState = Original.m_FarmerState;
		m_StartAction = Original.m_StartAction;
		m_UseAction = Original.m_UseAction;
		m_EndAction = Original.m_EndAction;
		m_AbortAction = Original.m_AbortAction;
		m_RequirementsOut = Original.m_RequirementsOut;
		m_AdjacentTile = Original.m_AdjacentTile;
		m_MoveRange = Original.m_MoveRange;
		m_Stage = Original.m_Stage;
	}

	public AFO(Actionable NewObject, ObjectType NewObjectType, Actionable Actioner, AT NewActionType, string RequirementsIn, TileCoord Position, Stage NewStage = Stage.Start)
	{
		m_Object = NewObject;
		m_ObjectType = NewObjectType;
		m_Actioner = Actioner;
		m_ActionType = NewActionType;
		m_RequirementsIn = RequirementsIn;
		m_Position = Position;
		m_Stage = NewStage;
		ClearOut();
	}

	public void Init(Actionable NewObject, ObjectType NewObjectType, Actionable Actioner, AT NewActionType, string RequirementsIn, TileCoord Position, Stage NewStage = Stage.Start)
	{
		m_Object = NewObject;
		m_ObjectType = NewObjectType;
		m_Actioner = Actioner;
		m_ActionType = NewActionType;
		m_RequirementsIn = RequirementsIn;
		m_Position = Position;
		m_Stage = NewStage;
		ClearOut();
	}

	public void ClearOut()
	{
		m_FarmerState = Farmer.State.Total;
		m_StartAction = null;
		m_UseAction = null;
		m_EndAction = null;
		m_AbortAction = null;
		m_RequirementsOut = "";
		m_AdjacentTile = false;
		m_MoveRange = 0;
	}
}
