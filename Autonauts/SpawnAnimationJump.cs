using System;
using SimpleJSON;
using UnityEngine;

public class SpawnAnimationJump : SpawnAnimation
{
	public Vector3 m_StartPosition;

	public Vector3 m_EndPosition;

	public float m_JumpHeight;

	public float m_Time;

	public float m_Delay;

	public Actionable m_FinishTarget;

	public Actionable m_Spawner;

	public Actionable m_Target;

	public int m_Frame;

	private int m_FinishID;

	private bool m_DustLand;

	private TrailMaker2 m_TrailMaker;

	public SpawnAnimationJump()
		: base(Type.Jump, null)
	{
	}

	private Vector3 TestEndPosition(BaseClass NewObject, Vector3 OldPosition)
	{
		Vector3 result = OldPosition;
		if (result.y < PlotMeshBuilderWater.m_WaterLevel)
		{
			float num = ObjectTypeList.Instance.GetHeight(NewObject.m_TypeIdentifier) * NewObject.transform.localScale.y;
			if (result.y < PlotMeshBuilderWater.m_WaterLevel - num / 2f)
			{
				result.y = PlotMeshBuilderWater.m_WaterLevel - num / 2f;
			}
		}
		return result;
	}

	public SpawnAnimationJump(BaseClass NewObject, TileCoord StartPosition, TileCoord EndPosition, float StartHeight, float EndHeight, float JumpHeight, float Time, float Delay, Actionable FinishTarget, bool DustLand, Actionable Spawner, Actionable Target)
		: base(Type.Jump, NewObject)
	{
		m_StartPosition = StartPosition.ToWorldPositionTileCentered();
		m_StartPosition.y = StartHeight;
		m_EndPosition = EndPosition.ToWorldPositionTileCentered();
		m_EndPosition.y = EndHeight;
		m_EndPosition = TestEndPosition(NewObject, m_EndPosition);
		m_JumpHeight = JumpHeight;
		m_Time = Time;
		m_Delay = Delay;
		m_FinishTarget = FinishTarget;
		m_DustLand = DustLand;
		m_Spawner = Spawner;
		m_Target = Target;
		m_Frame = TimeManager.Instance.m_Frame;
		UpdatePosition();
		m_TrailMaker = TrailManager.Instance.StartTrail(NewObject.transform);
	}

	public SpawnAnimationJump(BaseClass NewObject, Vector3 StartPosition, Vector3 EndPosition, float JumpHeight, float Time, float Delay, Actionable FinishTarget, bool DustLand, Actionable Spawner, Actionable Target)
		: base(Type.Jump, NewObject)
	{
		m_StartPosition = StartPosition;
		m_EndPosition = TestEndPosition(NewObject, EndPosition);
		m_JumpHeight = JumpHeight;
		m_Time = Time;
		m_Delay = Delay;
		m_FinishTarget = FinishTarget;
		m_DustLand = DustLand;
		m_Spawner = Spawner;
		m_Target = Target;
		m_Frame = TimeManager.Instance.m_Frame;
		UpdatePosition();
		m_TrailMaker = TrailManager.Instance.StartTrail(NewObject.transform);
	}

	public override bool IsSavable()
	{
		if ((bool)m_FinishTarget && (bool)m_FinishTarget.GetComponent<Farmer>())
		{
			return false;
		}
		return true;
	}

	public override void Save(JSONNode NewNode)
	{
		base.Save(NewNode);
		JSONUtils.Set(NewNode, "SX", m_StartPosition.x);
		JSONUtils.Set(NewNode, "SY", m_StartPosition.y);
		JSONUtils.Set(NewNode, "SZ", m_StartPosition.z);
		JSONUtils.Set(NewNode, "EX", m_EndPosition.x);
		JSONUtils.Set(NewNode, "EY", m_EndPosition.y);
		JSONUtils.Set(NewNode, "EZ", m_EndPosition.z);
		JSONUtils.Set(NewNode, "D", m_Delay);
		JSONUtils.Set(NewNode, "JumpHeight", m_JumpHeight);
		JSONUtils.Set(NewNode, "DL", m_DustLand);
		if ((bool)m_FinishTarget)
		{
			JSONUtils.Set(NewNode, "FinishTarget", m_FinishTarget.m_UniqueID);
		}
	}

	public override void Load(JSONNode NewNode)
	{
		base.Load(NewNode);
		m_StartPosition.x = JSONUtils.GetAsFloat(NewNode, "SX", 0f);
		m_StartPosition.y = JSONUtils.GetAsFloat(NewNode, "SY", 0f);
		m_StartPosition.z = JSONUtils.GetAsFloat(NewNode, "SZ", 0f);
		m_EndPosition.x = JSONUtils.GetAsFloat(NewNode, "EX", 0f);
		m_EndPosition.y = JSONUtils.GetAsFloat(NewNode, "EY", 0f);
		m_EndPosition.z = JSONUtils.GetAsFloat(NewNode, "EZ", 0f);
		m_Delay = JSONUtils.GetAsFloat(NewNode, "D", 0f);
		m_JumpHeight = JSONUtils.GetAsFloat(NewNode, "JumpHeight", 0f);
		m_DustLand = JSONUtils.GetAsBool(NewNode, "DL", DefaultValue: false);
		m_FinishID = JSONUtils.GetAsInt(NewNode, "FinishTarget", -1);
	}

	public override void PostLoad()
	{
		base.PostLoad();
		if (m_FinishID != -1)
		{
			BaseClass objectFromUniqueID = ObjectTypeList.Instance.GetObjectFromUniqueID(m_FinishID, ErrorCheck: false);
			if ((bool)objectFromUniqueID)
			{
				m_FinishTarget = objectFromUniqueID.GetComponent<Actionable>();
			}
		}
	}

	private void UpdatePosition()
	{
		float num = m_Time / m_Delay;
		Vector3 position = (m_EndPosition - m_StartPosition) * num + m_StartPosition;
		position.y += Mathf.Sin(num * (float)Math.PI) * m_JumpHeight;
		if ((bool)m_NewObject)
		{
			m_NewObject.transform.position = position;
		}
	}

	public override bool Update()
	{
		base.Update();
		UpdatePosition();
		m_Time += TimeManager.Instance.m_NormalDelta;
		if (m_Time >= m_Delay)
		{
			return false;
		}
		return true;
	}

	public override void End(bool Success)
	{
		base.End(Success);
		TrailManager.Instance.StopTrail(m_TrailMaker);
		if ((bool)m_FinishTarget && (bool)m_FinishTarget.GetComponent<Farmer>())
		{
			EndInWorld();
			m_FinishTarget.GetComponent<Farmer>().SpawnEnd(m_NewObject, Success);
			m_NewObject.transform.position = m_EndPosition;
		}
		else
		{
			EndInWorld();
			m_NewObject.GetComponent<TileCoordObject>().SetPosition(m_EndPosition);
			if (!SaveLoadManager.Instance.m_Loading)
			{
				ModManager.Instance.CheckItemTypeSpawnedCallback(m_NewObject.m_UniqueID, m_NewObject.m_TypeIdentifier, m_NewObject.GetComponent<TileCoordObject>().m_TileCoord);
			}
		}
		if (m_DustLand)
		{
			float num = (float)m_NewObject.GetComponent<Holdable>().m_Weight * 0.25f + 1f;
			MyParticles myParticles = ParticlesManager.Instance.CreateParticles("ObjectLand", m_EndPosition + new Vector3(0f, 2f, 0f), Quaternion.Euler(90f, 0f, 0f));
			ParticlesManager.Instance.DestroyParticles(myParticles, WaitUntilNoParticles: true);
			myParticles.transform.localScale = new Vector3(num, num, num);
			AudioManager.Instance.StartEvent("ObjectLand", m_NewObject.GetComponent<TileCoordObject>());
		}
	}

	public override void Abort()
	{
		base.Abort();
		TrailManager.Instance.StopTrail(m_TrailMaker);
		if ((bool)m_FinishTarget && (bool)m_FinishTarget.GetComponent<Farmer>())
		{
			EndInWorld();
			m_NewObject.transform.position = m_StartPosition;
			m_FinishTarget.GetComponent<Farmer>().SpawnAbort(m_NewObject);
		}
		else
		{
			EndInWorld();
			m_NewObject.GetComponent<TileCoordObject>().SetPosition(m_StartPosition);
		}
	}
}
