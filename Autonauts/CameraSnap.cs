using SimpleJSON;
using UnityEngine;

public class CameraSnap
{
	public int f;

	public bool m_Free;

	public Vector3 m_Position;

	public Quaternion m_Rotation;

	public bool m_DOFEnabled;

	public bool m_AutoDOFEnabled;

	public float m_DOFFocalDistance;

	public float m_DOFFocalLength;

	public float m_DOFAperture;

	public void Save(JSONNode NewNode)
	{
		JSONUtils.Set(NewNode, "F", f);
		JSONUtils.Set(NewNode, "Free", m_Free);
		JSONUtils.Set(NewNode, "x", m_Position.x);
		JSONUtils.Set(NewNode, "y", m_Position.y);
		JSONUtils.Set(NewNode, "z", m_Position.z);
		JSONUtils.Set(NewNode, "qx", m_Rotation.x);
		JSONUtils.Set(NewNode, "qy", m_Rotation.y);
		JSONUtils.Set(NewNode, "qz", m_Rotation.z);
		JSONUtils.Set(NewNode, "qw", m_Rotation.w);
		JSONUtils.Set(NewNode, "DOF", m_DOFEnabled);
		JSONUtils.Set(NewNode, "Auto", m_AutoDOFEnabled);
		JSONUtils.Set(NewNode, "FDist", m_DOFFocalDistance);
		JSONUtils.Set(NewNode, "FLen", m_DOFFocalLength);
		JSONUtils.Set(NewNode, "Ap", m_DOFAperture);
	}

	public void Load(JSONNode NewNode)
	{
		f = JSONUtils.GetAsInt(NewNode, "F", 0);
		m_Free = JSONUtils.GetAsBool(NewNode, "Free", DefaultValue: false);
		m_Position.x = JSONUtils.GetAsFloat(NewNode, "x", 0f);
		m_Position.y = JSONUtils.GetAsFloat(NewNode, "y", 0f);
		m_Position.z = JSONUtils.GetAsFloat(NewNode, "z", 0f);
		m_Rotation.x = JSONUtils.GetAsFloat(NewNode, "qx", 0f);
		m_Rotation.y = JSONUtils.GetAsFloat(NewNode, "qy", 0f);
		m_Rotation.z = JSONUtils.GetAsFloat(NewNode, "qz", 0f);
		m_Rotation.w = JSONUtils.GetAsFloat(NewNode, "qw", 0f);
		m_DOFEnabled = JSONUtils.GetAsBool(NewNode, "DOF", DefaultValue: false);
		m_AutoDOFEnabled = JSONUtils.GetAsBool(NewNode, "Auto", DefaultValue: false);
		m_DOFFocalDistance = JSONUtils.GetAsFloat(NewNode, "FDist", 0f);
		m_DOFFocalLength = JSONUtils.GetAsFloat(NewNode, "FLen", 0f);
		m_DOFAperture = JSONUtils.GetAsFloat(NewNode, "Ap", 0f);
	}
}
