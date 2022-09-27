using UnityEngine;

public class Clouds : MonoBehaviour
{
	private void BuildMesh(Vector3 Position)
	{
		int num = 4;
		Vector3[] array = new Vector3[num];
		Vector3[] array2 = new Vector3[num];
		Vector2[] array3 = new Vector2[num];
		Vector2[] array4 = new Vector2[num];
		int[] array5 = new int[6];
		float num2 = Tile.m_Size * (float)Plot.m_PlotTilesWide;
		float num3 = Tile.m_Size * (float)Plot.m_PlotTilesHigh;
		array[0] = new Vector3(0f, 0f, 0f);
		array[1] = new Vector3(num2, 0f, 0f);
		array[2] = new Vector3(0f, 0f, 0f - num3);
		array[3] = new Vector3(num2, 0f, 0f - num3);
		Vector2 vector = new Vector2(0.005f, 0.005f);
		for (int i = 0; i < num; i++)
		{
			array3[i] = new Vector3((array[i].x + Position.x) * vector.x, (array[i].z + Position.z) * vector.y);
			array4[i] = new Vector3((array[i].x + Position.x) / (num2 * (float)PlotManager.Instance.m_PlotsWide), (0f - (array[i].z + Position.z)) / (num3 * (float)PlotManager.Instance.m_PlotsHigh));
		}
		for (int j = 0; j < num; j++)
		{
			array2[j] = Vector3.up;
		}
		array5[0] = 0;
		array5[1] = 1;
		array5[2] = 2;
		array5[3] = 1;
		array5[4] = 3;
		array5[5] = 2;
		Mesh mesh = new Mesh();
		mesh.vertices = array;
		mesh.triangles = array5;
		mesh.normals = array2;
		mesh.uv = array3;
		mesh.uv2 = array4;
		GetComponent<MeshFilter>().mesh = mesh;
		GetComponent<MeshCollider>().sharedMesh = mesh;
	}

	public void SetPosition(Vector3 Position)
	{
		base.transform.position = Position;
		BuildMesh(Position);
	}
}
