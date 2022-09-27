using System;
using System.Collections.Generic;
using UnityEngine;

public class StringLine : BaseClass
{
	private Vector3 m_StartPoint;

	private Vector3 m_EndPoint;

	private static int m_NumSegments = 10;

	private static float m_Thickness = 0.05f;

	private List<GameObject> m_Segments;

	private float m_Slack;

	protected new void Awake()
	{
		base.Awake();
		m_StartPoint = base.transform.position;
		m_EndPoint = m_StartPoint;
		GameObject original = (GameObject)Resources.Load("Models/Special/FishingLine", typeof(GameObject));
		m_Segments = new List<GameObject>();
		for (int i = 0; i < m_NumSegments; i++)
		{
			GameObject item = UnityEngine.Object.Instantiate(original, m_StartPoint, Quaternion.identity, base.transform);
			m_Segments.Add(item);
		}
	}

	public void SetPoints(Vector3 StartPoint, Vector3 EndPoint, float Slack)
	{
		m_StartPoint = StartPoint;
		m_EndPoint = EndPoint;
		m_Slack = Slack;
		UpdateSegments();
	}

	private void UpdateSegments()
	{
		Vector3[] array = new Vector3[m_NumSegments + 1];
		for (int i = 0; i < m_NumSegments + 1; i++)
		{
			float num = (float)i / (float)m_NumSegments;
			array[i] = (m_EndPoint - m_StartPoint) * num + m_StartPoint;
			float num2 = Mathf.Sin(num * (float)Math.PI) * m_Slack;
			array[i].y -= num2;
		}
		for (int j = 0; j < m_NumSegments; j++)
		{
			GameObject obj = m_Segments[j];
			Vector3 position = (array[j + 1] + array[j]) * 0.5f;
			obj.transform.position = position;
			obj.transform.LookAt(array[j + 1]);
			float z = (array[j + 1] - array[j]).magnitude * 0.5f;
			obj.transform.localScale = new Vector3(m_Thickness, m_Thickness, z);
		}
	}

	private void Update()
	{
		UpdateSegments();
	}
}
