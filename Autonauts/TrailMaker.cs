using System.Collections.Generic;
using UnityEngine;

public class TrailMaker : MonoBehaviour
{
	private bool m_PreTrailActive;

	private List<TrailData> m_Data;

	private float m_Time;

	private Transform m_FollowObject;

	private Vector3 m_StartPoint;

	private Vector3 m_EndPoint;

	private Vector3 m_StartPositon;

	private Quaternion m_StartRotation;

	private MeshRenderer m_Mesh;

	private bool m_First;

	private void Awake()
	{
		m_Mesh = GetComponent<MeshRenderer>();
		m_Data = new List<TrailData>();
	}

	public void StartPreTrail(Transform FollowObject, Vector3 StartPoint, Vector3 EndPoint)
	{
		m_FollowObject = FollowObject;
		m_StartPoint = StartPoint;
		m_EndPoint = EndPoint;
		m_PreTrailActive = true;
		m_Time = 0f;
		m_Data.Clear();
		m_First = true;
		m_Mesh.enabled = false;
		MeshFilter component = GetComponent<MeshFilter>();
		if (component.mesh == null)
		{
			component.mesh = new Mesh();
		}
	}

	public void EndPreTrail()
	{
		m_PreTrailActive = false;
		if (m_First && m_FollowObject != null)
		{
			m_StartPositon = m_FollowObject.position;
			m_StartRotation = m_FollowObject.rotation;
		}
		m_First = true;
	}

	private void DoEnd()
	{
		if (!(m_FollowObject == null))
		{
			Vector3 position = m_FollowObject.position;
			Quaternion rotation = m_FollowObject.rotation;
			int num = 10;
			for (int i = 0; i <= num; i++)
			{
				float num2 = (float)i / (float)num;
				base.transform.position = (position - m_StartPositon) * num2 + m_StartPositon;
				base.transform.rotation = Quaternion.Slerp(m_StartRotation, rotation, num2);
				Vector3 start = base.transform.TransformPoint(m_StartPoint);
				Vector3 end = base.transform.TransformPoint(m_EndPoint);
				m_Data.Add(new TrailData(start, end, num2 * m_Time));
			}
			m_Mesh.enabled = true;
			BuildMesh();
			m_Time = 0f;
		}
	}

	private void BuildMesh()
	{
		int num = m_Data.Count * 2;
		Vector3[] array = new Vector3[num];
		Vector3[] array2 = new Vector3[num];
		Vector2[] array3 = new Vector2[num];
		for (int i = 0; i < m_Data.Count; i++)
		{
			array[i * 2] = m_Data[i].m_Start - m_Data[0].m_Start;
			array[i * 2 + 1] = m_Data[i].m_End - m_Data[0].m_Start;
			float x = m_Data[i].m_Time / m_Time;
			array3[i * 2] = new Vector2(x, 1f);
			array3[i * 2 + 1] = new Vector2(x, 0f);
			array2[i * 2] = Vector3.up;
			array2[i * 2 + 1] = Vector3.up;
		}
		int num2 = (num - 2) * 3;
		int[] array4 = new int[num2];
		for (int j = 0; j < num2 / 6; j++)
		{
			array4[j * 6] = j * 2;
			array4[j * 6 + 1] = j * 2 + 1;
			array4[j * 6 + 2] = j * 2 + 2;
			array4[j * 6 + 3] = j * 2 + 1;
			array4[j * 6 + 4] = j * 2 + 3;
			array4[j * 6 + 5] = j * 2 + 2;
		}
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		mesh.triangles = null;
		mesh.vertices = array;
		mesh.normals = array2;
		mesh.uv = array3;
		mesh.triangles = array4;
		base.transform.position = m_Data[0].m_Start;
		base.transform.rotation = Quaternion.identity;
	}

	private void Update()
	{
		if (m_FollowObject == null)
		{
			TrailManager.Instance.TrailFinished(this);
			return;
		}
		if (m_PreTrailActive)
		{
			if (m_First)
			{
				m_First = false;
				m_StartPositon = m_FollowObject.position;
				m_StartRotation = m_FollowObject.rotation;
			}
			m_Time += TimeManager.Instance.m_NormalDelta;
			return;
		}
		if (m_First)
		{
			m_First = false;
			DoEnd();
		}
		float num = 0.075f;
		float num2 = m_Time / num;
		m_Mesh.material.SetFloat("_Offset", 0f - num2);
		m_Time += TimeManager.Instance.m_NormalDelta;
		if (m_Time >= num)
		{
			TrailManager.Instance.TrailFinished(this);
		}
	}
}
