using SimpleJSON;
using UnityEngine;

public class Flora : Selectable
{
	protected bool m_WorldCreated;

	protected float m_Scale;

	protected float m_StartingScale;

	protected float m_Heading;

	public static bool GetIsTypeFlora(ObjectType NewType)
	{
		if (MyTree.GetIsTree(NewType) || NewType == ObjectType.Bush || NewType == ObjectType.Hedge || NewType == ObjectType.Grass || NewType == ObjectType.Weed || Crop.GetIsTypeCrop(NewType) || NewType == ObjectType.FlowerWild || NewType == ObjectType.Mushroom || NewType == ObjectType.CropPumpkin || NewType == ObjectType.Bullrushes)
		{
			return true;
		}
		return false;
	}

	public override void Restart()
	{
		base.Restart();
		TileManager.Instance.SetAssociatedObject(m_TileCoord, this);
		RecordingManager.Instance.UpdateObject(this);
		m_Scale = 1f;
		m_StartingScale = 1f;
		UpdateWorldCreated();
	}

	protected new void Awake()
	{
		base.Awake();
		m_WorldCreated = false;
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		RecordingManager.Instance.RemoveObject(this);
		m_WorldCreated = false;
		TileManager.Instance.ClearAssociatedObject(m_TileCoord, this);
		base.StopUsing(AndDestroy);
	}

	public override void WorldCreated()
	{
		base.WorldCreated();
		m_WorldCreated = true;
		UpdateWorldCreated();
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		JSONUtils.Set(Node, "W", m_WorldCreated);
	}

	public override void Load(JSONNode Node)
	{
		TileManager.Instance.ClearAssociatedObject(m_TileCoord, this);
		base.Load(Node);
		TileManager.Instance.SetAssociatedObject(m_TileCoord, this);
		m_WorldCreated = JSONUtils.GetAsBool(Node, "W", DefaultValue: false);
		UpdateWorldCreated();
	}

	protected void RandomRotation()
	{
		m_Heading = Random.Range(0, 360);
		base.transform.rotation = Quaternion.Euler(0f, m_Heading, 0f);
	}

	protected void SetScale(float Scale)
	{
		m_Scale = Scale;
		UpdateScale();
	}

	protected virtual void UpdateScale()
	{
		float num = m_StartingScale * m_Scale;
		m_ModelRoot.transform.localScale = new Vector3(num, num, num);
	}

	protected virtual void UpdateWorldCreated()
	{
		base.transform.localPosition = m_TileCoord.ToWorldPositionTileCentered();
		if (m_WorldCreated)
		{
			float num = 0.5f;
			base.transform.localPosition += new Vector3(Random.Range(0f - num, num), 0f, Random.Range(0f - num, num));
		}
		if (m_WorldCreated)
		{
			m_StartingScale = 0.5f + Random.Range(0f, 0.4f);
		}
		else
		{
			m_StartingScale = 0.9f + Random.Range(0f, 0.2f);
		}
		UpdateScale();
		RandomRotation();
	}
}
