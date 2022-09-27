using System;
using UnityEngine;

public class MechanicalBelt : BaseClass
{
	private static int m_NumSegments = 10;

	private static float m_Width = 0.4f;

	private static float m_TextureScale = 1f;

	private static float m_Spacing = 0.4f;

	public int m_Length;

	public GameObject m_ParentPulley;

	public GameObject m_ConnectedToPulley;

	private Vector3 m_StartPoint;

	private Vector3 m_EndPoint;

	public Building m_Parent;

	public Building m_ConnectedTo;

	private MeshFilter m_MeshFilter;

	private MeshRenderer m_Mesh;

	protected new void Awake()
	{
		base.Awake();
		m_ParentPulley = ModelManager.Instance.Instantiate(ObjectTypeList.m_Total, "Models/Special/MechanicalPulley", base.transform, RandomVariants: false);
		m_ConnectedToPulley = ModelManager.Instance.Instantiate(ObjectTypeList.m_Total, "Models/Special/MechanicalPulley", base.transform, RandomVariants: false);
		m_MeshFilter = base.gameObject.AddComponent<MeshFilter>();
		m_Mesh = base.gameObject.AddComponent<MeshRenderer>();
	}

	public override void StopUsing(bool AndDestroy = true)
	{
		CleanHighlight();
		base.StopUsing(AndDestroy);
		if ((bool)m_ConnectedTo)
		{
			if ((bool)m_ConnectedTo.GetComponent<BeltLinkage>())
			{
				m_ConnectedTo.GetComponent<BeltLinkage>().m_BeltConnectTo = null;
			}
			else
			{
				m_ConnectedTo.StopConnection();
			}
		}
		DestroyMesh();
	}

	public void SetLength(int TileLength)
	{
		m_Length = TileLength;
		base.transform.localScale = new Vector3(TileLength, 1f, 1f);
	}

	public void ConnectTo(Building StartBuilding, Building EndBuilding)
	{
		m_Parent = StartBuilding;
		m_ConnectedTo = EndBuilding;
		base.transform.SetParent(StartBuilding.GetComponent<BeltLinkage>().m_ConnectPoint);
		base.transform.localPosition = default(Vector3);
		if ((bool)m_ConnectedTo.GetComponent<BeltLinkage>())
		{
			m_ConnectedTo.GetComponent<BeltLinkage>().SetBeltConnectedTo(StartBuilding.GetComponent<BeltLinkage>());
		}
		else
		{
			m_ConnectedTo.StartConnectionTo(StartBuilding);
		}
		m_EndPoint = EndBuilding.m_TileCoord.ToWorldPositionTileCentered();
		if ((bool)EndBuilding.FindNode("BeltPoint"))
		{
			m_EndPoint = EndBuilding.FindNode("BeltPoint").transform.position;
		}
		m_StartPoint = StartBuilding.m_TileCoord.ToWorldPositionTileCentered();
		if ((bool)StartBuilding.FindNode("BeltPoint"))
		{
			m_StartPoint = StartBuilding.FindNode("BeltPoint").transform.position;
		}
		if ((bool)StartBuilding.GetComponent<BeltLinkage>() && (bool)StartBuilding.GetComponent<BeltLinkage>().m_Rod)
		{
			MechanicalRod rod = StartBuilding.GetComponent<BeltLinkage>().m_Rod;
			float num = m_EndPoint.z;
			float z = rod.transform.position.z;
			float num2 = z - (float)rod.m_Length * Tile.m_Size;
			if (num < num2)
			{
				num = num2;
			}
			if (num > z)
			{
				num = z;
			}
			m_StartPoint.z = num;
		}
		UpdateLinkedSystem();
		UpdateSegments();
	}

	public void UpdateLinkedSystem()
	{
		if (m_Parent.m_LinkedSystem != null)
		{
			m_Mesh.sharedMaterial = ((LinkedSystemMechanical)m_Parent.m_LinkedSystem).m_BeltMaterial;
		}
		else
		{
			m_Mesh.sharedMaterial = MaterialManager.Instance.m_MaterialBelt;
		}
	}

	private void DestroyMesh()
	{
		if ((bool)m_MeshFilter.mesh)
		{
			UnityEngine.Object.Destroy(m_MeshFilter.mesh);
		}
	}

	private void BuildMeshSimple()
	{
		DestroyMesh();
		int num = (m_NumSegments + 1) * 4;
		Vector3[] array = new Vector3[num];
		Vector3[] array2 = new Vector3[num];
		Vector2[] array3 = new Vector2[num];
		Vector2[] array4 = new Vector2[num];
		Vector3 vector = m_StartPoint;
		Vector3 vector2 = m_EndPoint;
		if (m_StartPoint.x < m_EndPoint.x)
		{
			vector = m_EndPoint;
			vector2 = m_StartPoint;
		}
		int num2 = num / 2;
		for (int i = 0; i < m_NumSegments + 1; i++)
		{
			float num3 = (float)i / (float)m_NumSegments;
			Vector3 vector3 = (vector2 - vector) * num3 + vector;
			vector3 -= base.transform.position;
			float num4 = 0f - Mathf.Sin(num3 * (float)Math.PI);
			vector3.y += num4;
			int num5 = i * 2;
			array[num5] = vector3 + new Vector3(0f, m_Spacing / 2f, m_Width / 2f);
			array[num5 + 1] = vector3 + new Vector3(0f, m_Spacing / 2f, (0f - m_Width) / 2f);
			array[num5 + num2] = vector3 + new Vector3(0f, (0f - m_Spacing) / 2f, m_Width / 2f);
			array[num5 + 1 + num2] = vector3 + new Vector3(0f, (0f - m_Spacing) / 2f, (0f - m_Width) / 2f);
			float num6 = (0f - vector3.x) * m_TextureScale;
			array3[num5] = new Vector2(num6, 0f);
			array3[num5 + 1] = new Vector2(num6, 1f);
			num6 = 1f - num6;
			array3[num5 + num2] = new Vector2(num6, 0f);
			array3[num5 + 1 + num2] = new Vector2(num6, 1f);
			array4[num5] = new Vector2(num4, 0f);
			array4[num5 + 1] = new Vector2(num4, 0f);
			array4[num5 + num2] = new Vector2(num4, 0f);
			array4[num5 + 1 + num2] = new Vector2(num4, 0f);
			array2[num5] = Vector3.up;
			array2[num5 + 1] = Vector3.up;
			array2[num5 + num2] = Vector3.up;
			array2[num5 + 1 + num2] = Vector3.up;
		}
		int numSegments = m_NumSegments;
		int[] array5 = new int[numSegments * 6 * 2];
		int num7 = numSegments * 6;
		for (int j = 0; j < numSegments; j++)
		{
			array5[j * 6] = j * 2;
			array5[j * 6 + 1] = j * 2 + 1;
			array5[j * 6 + 2] = j * 2 + 2;
			array5[j * 6 + 3] = j * 2 + 1;
			array5[j * 6 + 4] = j * 2 + 3;
			array5[j * 6 + 5] = j * 2 + 2;
			array5[j * 6 + num7] = j * 2 + num2;
			array5[j * 6 + 1 + num7] = j * 2 + 1 + num2;
			array5[j * 6 + 2 + num7] = j * 2 + 2 + num2;
			array5[j * 6 + 3 + num7] = j * 2 + 1 + num2;
			array5[j * 6 + 4 + num7] = j * 2 + 3 + num2;
			array5[j * 6 + 5 + num7] = j * 2 + 2 + num2;
		}
		Mesh mesh = new Mesh();
		mesh.vertices = array;
		mesh.triangles = array5;
		mesh.normals = array2;
		mesh.uv = array3;
		mesh.uv2 = array4;
		m_MeshFilter.mesh = mesh;
	}

	private void UpdateSegments()
	{
		m_ParentPulley.transform.position = m_StartPoint;
		m_ConnectedToPulley.transform.position = m_EndPoint;
		BuildMeshSimple();
	}

	private void UpdateMovingAnimation()
	{
		if (m_Parent.m_LinkedSystem != null && ((LinkedSystemMechanical)m_Parent.m_LinkedSystem).GetSpeed() > 0f)
		{
			m_ParentPulley.transform.localRotation = ((LinkedSystemMechanical)m_Parent.m_LinkedSystem).GetPulleyRotation();
			m_ConnectedToPulley.transform.localRotation = m_ParentPulley.transform.localRotation;
		}
	}

	private void Update()
	{
		UpdateMovingAnimation();
	}
}
