using UnityEngine;

public class ToolFishingRod : ToolFillable
{
	[HideInInspector]
	public StringLine m_Line;

	[HideInInspector]
	public GameObject m_LinePoint;

	private GameObject m_Float;

	public static bool GetIsTypeFishingRod(ObjectType NewType)
	{
		if (NewType == ObjectType.ToolFishingRod || NewType == ObjectType.ToolFishingRodGood)
		{
			return true;
		}
		return false;
	}

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("ToolFishingRod", m_TypeIdentifier);
	}

	public override void PostCreate()
	{
		base.PostCreate();
		m_Line = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.StringLine, base.transform.localPosition, Quaternion.identity).GetComponent<StringLine>();
		m_Line.gameObject.SetActive(value: false);
		m_LinePoint = m_ModelRoot.transform.Find("LinePoint").gameObject;
		GameObject original = (GameObject)Resources.Load("Models/Special/FishingFloat", typeof(GameObject));
		m_Float = Object.Instantiate(original, base.transform.localPosition, Quaternion.identity, base.transform.parent);
		m_Float.SetActive(value: false);
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		base.transform.localPosition = new Vector3(0f, 0f, 0f);
		base.transform.localRotation = Quaternion.identity;
	}

	public void SetLineVisible(bool Visible)
	{
		m_Line.gameObject.SetActive(Visible);
	}

	public void SetFloatPosition(Vector3 Position, float Slack)
	{
		if (Position.y < PlotMeshBuilderWater.m_WaterLevel)
		{
			Position.y = PlotMeshBuilderWater.m_WaterLevel;
		}
		m_Line.SetPoints(m_LinePoint.transform.position, Position, Slack);
		m_Float.transform.position = Position;
	}

	public Vector3 GetFloatPosition()
	{
		return m_Float.transform.position;
	}

	public void SetFloatVisible(bool Visible)
	{
		m_Float.SetActive(Visible);
	}

	public override bool CanAcceptObjectType(ObjectType NewType)
	{
		if (NewType != ObjectType.FishBait)
		{
			return false;
		}
		return base.CanAcceptObjectType(NewType);
	}
}
