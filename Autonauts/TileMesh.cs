using System;
using System.Collections.Generic;
using UnityEngine;

public class TileMesh
{
	public List<Vector3> m_Vertices;

	public List<Vector2> m_UVs;

	public List<Color> m_Colours;

	public List<int>[] m_Triangles;

	public int[] m_TriangleCount;

	public TileMesh(int SubMeshes)
	{
		m_Vertices = new List<Vector3>();
		m_UVs = new List<Vector2>();
		m_Colours = new List<Color>();
		m_Triangles = new List<int>[SubMeshes];
		for (int i = 0; i < SubMeshes; i++)
		{
			m_Triangles[i] = new List<int>();
		}
		m_TriangleCount = new int[SubMeshes];
	}

	public void AddTri(int SubMesh, Vector3[] Vertices, Vector2[] UVs, Color[] Colours = null)
	{
		int count = m_Vertices.Count;
		m_Triangles[SubMesh].Add(count);
		m_Triangles[SubMesh].Add(count + 1);
		m_Triangles[SubMesh].Add(count + 2);
		m_TriangleCount[SubMesh] += Vertices.Length;
		foreach (Vector3 item in Vertices)
		{
			m_Vertices.Add(item);
		}
		foreach (Vector2 item2 in UVs)
		{
			m_UVs.Add(item2);
		}
		if (Colours != null)
		{
			foreach (Color item3 in Colours)
			{
				m_Colours.Add(item3);
			}
		}
	}

	public void AddTri(int SubMesh, Vector3[] Vertices, Color[] Colours = null)
	{
		Vector2[] array = new Vector2[Vertices.Length];
		for (int i = 0; i < Vertices.Length; i++)
		{
			array[i].x = Vertices[i].x / Tile.m_Size * PlotMeshBuilder.Instance.m_USize;
			array[i].y = (0f - Vertices[i].z) / Tile.m_Size * PlotMeshBuilder.Instance.m_VSize;
		}
		AddTri(SubMesh, Vertices, array, Colours);
	}

	public void AddQuad(int SubMesh, Vector3[] Vertices, Vector2[] UVs, Color[] Colours = null)
	{
		int count = m_Vertices.Count;
		m_Triangles[SubMesh].Add(count);
		m_Triangles[SubMesh].Add(count + 1);
		m_Triangles[SubMesh].Add(count + 2);
		m_Triangles[SubMesh].Add(count + 1);
		m_Triangles[SubMesh].Add(count + 3);
		m_Triangles[SubMesh].Add(count + 2);
		m_TriangleCount[SubMesh] += Vertices.Length;
		foreach (Vector3 item in Vertices)
		{
			m_Vertices.Add(item);
		}
		foreach (Vector2 item2 in UVs)
		{
			m_UVs.Add(item2);
		}
		if (Colours != null)
		{
			foreach (Color item3 in Colours)
			{
				m_Colours.Add(item3);
			}
		}
	}

	public void AddQuad(int SubMesh, Vector3[] Vertices, Color[] Colours = null)
	{
		Vector2[] array = new Vector2[Vertices.Length];
		for (int i = 0; i < Vertices.Length; i++)
		{
			array[i].x = Vertices[i].x / Tile.m_Size * PlotMeshBuilder.Instance.m_USize;
			array[i].y = (0f - Vertices[i].z) / Tile.m_Size * PlotMeshBuilder.Instance.m_VSize;
		}
		AddQuad(SubMesh, Vertices, array, Colours);
	}

	public void AddFan(int SubMesh, Vector3[] Vertices, Vector2[] UVs, Color[] Colours = null)
	{
		int count = m_Vertices.Count;
		for (int i = 0; i < Vertices.Length - 2; i++)
		{
			m_Triangles[SubMesh].Add(count);
			m_Triangles[SubMesh].Add(count + 1 + i);
			m_Triangles[SubMesh].Add(count + 2 + i);
		}
		m_TriangleCount[SubMesh] += Vertices.Length;
		foreach (Vector3 item in Vertices)
		{
			m_Vertices.Add(item);
		}
		foreach (Vector2 item2 in UVs)
		{
			m_UVs.Add(item2);
		}
		if (Colours != null)
		{
			foreach (Color item3 in Colours)
			{
				m_Colours.Add(item3);
			}
		}
	}

	public void AddFan(int SubMesh, Vector3[] Vertices, Color[] Colours = null)
	{
		Vector2[] array = new Vector2[Vertices.Length];
		for (int i = 0; i < Vertices.Length; i++)
		{
			array[i].x = Vertices[i].x / Tile.m_Size * PlotMeshBuilder.Instance.m_USize;
			array[i].y = (0f - Vertices[i].z) / Tile.m_Size * PlotMeshBuilder.Instance.m_VSize;
		}
		AddFan(SubMesh, Vertices, array, Colours);
	}

	public void AddStrip(int SubMesh, Vector3[] Vertices, Vector2[] UVs, Color[] Colours = null)
	{
		int count = m_Vertices.Count;
		for (int i = 0; i < Vertices.Length - 2; i++)
		{
			if (i % 2 == 0)
			{
				m_Triangles[SubMesh].Add(count + i);
				m_Triangles[SubMesh].Add(count + 1 + i);
				m_Triangles[SubMesh].Add(count + 2 + i);
			}
			else
			{
				m_Triangles[SubMesh].Add(count + i);
				m_Triangles[SubMesh].Add(count + 2 + i);
				m_Triangles[SubMesh].Add(count + 1 + i);
			}
		}
		m_TriangleCount[SubMesh] += Vertices.Length;
		foreach (Vector3 item in Vertices)
		{
			m_Vertices.Add(item);
		}
		foreach (Vector2 item2 in UVs)
		{
			m_UVs.Add(item2);
		}
		if (Colours != null)
		{
			foreach (Color item3 in Colours)
			{
				m_Colours.Add(item3);
			}
		}
	}

	public void AddStrip(int SubMesh, Vector3[] Vertices, Color[] Colours = null)
	{
		Vector2[] array = new Vector2[Vertices.Length];
		for (int i = 0; i < Vertices.Length; i++)
		{
			array[i].x = Vertices[i].x / Tile.m_Size * PlotMeshBuilder.Instance.m_USize;
			array[i].y = (0f - Vertices[i].z) / Tile.m_Size * PlotMeshBuilder.Instance.m_VSize;
		}
		AddStrip(SubMesh, Vertices, array, Colours);
	}

	public TileMesh CreateNewRotatedMesh(float Rotation, Vector3 VertexOffset, Vector2 UVOffset)
	{
		TileMesh tileMesh = new TileMesh(m_Triangles.Length);
		float num = Mathf.Sin(Rotation * ((float)Math.PI / 180f));
		float num2 = Mathf.Cos(Rotation * ((float)Math.PI / 180f));
		float num3 = Mathf.Sin((0f - Rotation) * ((float)Math.PI / 180f));
		float num4 = Mathf.Cos((0f - Rotation) * ((float)Math.PI / 180f));
		for (int i = 0; i < m_Vertices.Count; i++)
		{
			Vector3 vector = default(Vector3);
			vector.x = num2 * m_Vertices[i].x - num * m_Vertices[i].z;
			vector.z = num * m_Vertices[i].x + num2 * m_Vertices[i].z;
			vector.y = m_Vertices[i].y;
			tileMesh.m_Vertices.Add(vector + VertexOffset);
			Vector2 vector2 = default(Vector2);
			vector2.x = num4 * m_UVs[i].x - num3 * m_UVs[i].y;
			vector2.y = num3 * m_UVs[i].x + num4 * m_UVs[i].y;
			tileMesh.m_UVs.Add(vector2 + UVOffset);
			if (m_Colours != null && m_Colours.Count > 0)
			{
				Color item = default(Color);
				item.r = num2 * m_Colours[i].r - num * m_Colours[i].g;
				item.g = num * m_Colours[i].r + num2 * m_Colours[i].g;
				item.b = m_Colours[i].b;
				tileMesh.m_Colours.Add(item);
			}
		}
		for (int j = 0; j < m_Triangles.Length; j++)
		{
			for (int k = 0; k < m_Triangles[j].Count; k++)
			{
				tileMesh.m_Triangles[j].Add(m_Triangles[j][k]);
			}
			tileMesh.m_TriangleCount[j] = m_TriangleCount[j];
		}
		return tileMesh;
	}

	public void AddMesh(TileMesh NewMesh, Vector3 VertexOffset, Vector2 UVOffset)
	{
		for (int i = 0; i < m_Triangles.Length; i++)
		{
			int count = m_Vertices.Count;
			for (int j = 0; j < NewMesh.m_Triangles[i].Count; j++)
			{
				m_Triangles[i].Add(count + NewMesh.m_Triangles[i][j]);
			}
			m_TriangleCount[i] += NewMesh.m_TriangleCount[i];
		}
		for (int k = 0; k < NewMesh.m_Vertices.Count; k++)
		{
			Vector3 item = NewMesh.m_Vertices[k] + VertexOffset;
			m_Vertices.Add(item);
			Vector2 item2 = NewMesh.m_UVs[k] + UVOffset;
			m_UVs.Add(item2);
			if (NewMesh.m_Colours != null && NewMesh.m_Colours.Count > 0)
			{
				Color item3 = NewMesh.m_Colours[k];
				m_Colours.Add(item3);
			}
		}
	}
}
