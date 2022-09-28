using System;
using UnityEngine;

public class Arc : BaseClass
{
	private static int m_NumSegments = 30;

	private static float m_TextureScale = 0.2f;

	private MeshFilter m_MeshFilter;

	private MeshRenderer m_Mesh;

	private float m_TextureOffset;

	public bool m_Animate;

	protected new void Awake()
	{
		base.Awake();
		m_MeshFilter = base.gameObject.AddComponent<MeshFilter>();
		m_Mesh = base.gameObject.AddComponent<MeshRenderer>();
		m_Mesh.sharedMaterial = MaterialManager.Instance.m_ArcMaterial;
		m_Animate = true;
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		base.StopUsing(AndDestroy);
		DestroyMesh();
	}

	private void DestroyMesh()
	{
		if ((bool)m_MeshFilter.mesh)
		{
			UnityEngine.Object.Destroy(m_MeshFilter.mesh);
		}
	}

	private void BuildMeshSimple(float Length, float Height, float Width)
	{
		DestroyMesh();
		int num = (m_NumSegments + 1) * 2;
		Vector3[] array = new Vector3[num];
		Vector3[] array2 = new Vector3[num];
		Vector2[] array3 = new Vector2[num];
		for (int i = 0; i < m_NumSegments + 1; i++)
		{
			float num2 = (float)i / (float)m_NumSegments;
			Vector3 vector = new Vector3((0f - Length) * num2, 0f, 0f);
			float num3 = Mathf.Sin(num2 * (float)Math.PI) * Height;
			vector.y += num3;
			int num4 = i * 2;
			array[num4] = vector + new Vector3(0f, 0f, Width / 2f);
			array[num4 + 1] = vector + new Vector3(0f, 0f, (0f - Width) / 2f);
			float x = (0f - vector.x) * m_TextureScale;
			array3[num4] = new Vector2(x, 0f);
			array3[num4 + 1] = new Vector2(x, 1f);
			array2[num4] = Vector3.up;
			array2[num4 + 1] = Vector3.up;
		}
		int numSegments = m_NumSegments;
		int[] array4 = new int[numSegments * 6 * 2];
		for (int j = 0; j < numSegments; j++)
		{
			array4[j * 6] = j * 2;
			array4[j * 6 + 1] = j * 2 + 1;
			array4[j * 6 + 2] = j * 2 + 2;
			array4[j * 6 + 3] = j * 2 + 1;
			array4[j * 6 + 4] = j * 2 + 3;
			array4[j * 6 + 5] = j * 2 + 2;
		}
		Mesh mesh = new Mesh();
		mesh.vertices = array;
		mesh.triangles = array4;
		mesh.normals = array2;
		mesh.uv = array3;
		m_MeshFilter.mesh = mesh;
	}

	public void SetTarget(Vector3 Target, float Height, float Width = 2f)
	{
		Vector3 vector = Target - base.transform.position;
		BuildMeshSimple(vector.magnitude, Height, Width);
		float num = 0f - Mathf.Atan2(vector.z, vector.x);
		base.transform.rotation = Quaternion.Euler(0f, num * 57.29578f + 180f, 0f);
	}

	private void Update()
	{
		if (m_Animate)
		{
			m_TextureOffset += TimeManager.Instance.m_NormalDelta * 4f;
			m_Mesh.sharedMaterial.SetTextureOffset("_MainTex", new Vector2(0f - m_TextureOffset, 0f));
		}
	}
}
