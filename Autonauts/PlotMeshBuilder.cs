using UnityEngine;

public class PlotMeshBuilder : MonoBehaviour
{
	public static PlotMeshBuilder Instance;

	private static int m_NumWallSubMeshes = 2;

	protected Plot m_Plot;

	private TileMesh m_Full;

	private TileMesh[] m_Quarter;

	private TileMesh[] m_TopEdge;

	private TileMesh[] m_LeftEdge;

	private TileMesh[] m_TopLeftEdge;

	private TileMesh[] m_TopLeftInner;

	private TileMesh[] m_TopEdgeWall;

	private TileMesh[] m_LeftEdgeWall;

	private TileMesh[] m_TopLeftEdgeWall;

	private TileMesh[] m_TopLeftInnerWall;

	protected bool m_Initted;

	public float m_BevelPercent = 0.05f;

	public float m_BevelSize;

	public float m_HalfTile;

	public float m_XPercent;

	public float m_YPercent;

	public float m_PlotTX;

	public float m_PlotTY;

	public float m_USize;

	public float m_VSize;

	public float m_HalfUSize;

	public float m_HalfVSize;

	protected float m_MaxDepth;

	protected float m_DepthRatio;

	private void Awake()
	{
		Instance = this;
		m_Initted = false;
	}

	private void InitVars()
	{
		m_BevelPercent = 0.025f;
		m_BevelSize = Tile.m_Size * m_BevelPercent;
		m_HalfTile = Tile.m_Size * 0.5f;
		m_XPercent = (float)TileManager.Instance.m_TilesWide / (float)TileManager.Instance.m_TileMapTexture.width;
		m_YPercent = (float)TileManager.Instance.m_TilesHigh / (float)TileManager.Instance.m_TileMapTexture.height;
		m_PlotTX = 1f / (float)PlotManager.Instance.m_PlotsWide * m_XPercent;
		m_PlotTY = 1f / (float)PlotManager.Instance.m_PlotsHigh * m_YPercent;
		m_USize = 1f / (float)Plot.m_PlotTilesWide * m_PlotTX;
		m_VSize = 1f / (float)Plot.m_PlotTilesHigh * m_PlotTY;
		m_HalfUSize = m_USize * 0.5f;
		m_HalfVSize = m_VSize * 0.5f;
		m_MaxDepth = 1.66666663f;
		m_DepthRatio = (m_MaxDepth - m_BevelSize) * 0.4f;
	}

	protected virtual void CreateGeometry()
	{
		Vector3[] vertices = new Vector3[4]
		{
			new Vector3(0f, 0f, 0f),
			new Vector3(Tile.m_Size, 0f, 0f),
			new Vector3(0f, 0f, 0f - Tile.m_Size),
			new Vector3(Tile.m_Size, 0f, 0f - Tile.m_Size)
		};
		Color[] colours = new Color[4]
		{
			new Color(0f, 0f, 0f),
			new Color(0f, 0f, 0f),
			new Color(0f, 0f, 0f),
			new Color(0f, 0f, 0f)
		};
		m_Full = new TileMesh(1);
		m_Full.AddQuad(0, vertices, colours);
		TileMesh tileMesh = new TileMesh(1);
		Vector3[] vertices2 = new Vector3[4]
		{
			new Vector3(0f - m_HalfTile, 0f, m_HalfTile),
			new Vector3(0f, 0f, m_HalfTile),
			new Vector3(0f - m_HalfTile, 0f, 0f),
			new Vector3(0f, 0f, 0f)
		};
		tileMesh.AddQuad(0, vertices2);
		m_Quarter = CreateMeshesWithRotations(tileMesh, m_HalfTile, m_HalfUSize, m_HalfVSize);
		tileMesh = new TileMesh(1);
		Vector3[] vertices3 = new Vector3[4]
		{
			new Vector3(0f - m_HalfTile, 0f - m_BevelSize, m_HalfTile),
			new Vector3(0f, 0f - m_BevelSize, m_HalfTile),
			new Vector3(0f - m_HalfTile, 0f, m_HalfTile - m_BevelSize),
			new Vector3(0f, 0f, m_HalfTile - m_BevelSize)
		};
		tileMesh.AddQuad(0, vertices3);
		Vector3[] vertices4 = new Vector3[4]
		{
			new Vector3(0f - m_HalfTile, 0f, m_HalfTile - m_BevelSize),
			new Vector3(0f, 0f, m_HalfTile - m_BevelSize),
			new Vector3(0f - m_HalfTile, 0f, 0f),
			new Vector3(0f, 0f, 0f)
		};
		tileMesh.AddQuad(0, vertices4);
		m_TopEdge = CreateMeshesWithRotations(tileMesh, m_HalfTile, m_HalfUSize, m_HalfVSize);
		tileMesh = new TileMesh(1);
		Vector3[] vertices5 = new Vector3[4]
		{
			new Vector3(0f - m_HalfTile, 0f - m_BevelSize, 0f),
			new Vector3(0f - m_HalfTile, 0f - m_BevelSize, m_HalfTile),
			new Vector3(0f - m_HalfTile + m_BevelSize, 0f, 0f),
			new Vector3(0f - m_HalfTile + m_BevelSize, 0f, m_HalfTile)
		};
		tileMesh.AddQuad(0, vertices5);
		Vector3[] vertices6 = new Vector3[4]
		{
			new Vector3(0f - m_HalfTile + m_BevelSize, 0f, 0f),
			new Vector3(0f - m_HalfTile + m_BevelSize, 0f, m_HalfTile),
			new Vector3(0f, 0f, 0f),
			new Vector3(0f, 0f, m_HalfTile)
		};
		tileMesh.AddQuad(0, vertices6);
		m_LeftEdge = CreateMeshesWithRotations(tileMesh, m_HalfTile, m_HalfUSize, m_HalfVSize);
		tileMesh = new TileMesh(1);
		Vector3[] vertices7 = new Vector3[4]
		{
			new Vector3(0f - m_HalfTile + m_BevelSize, 0f - m_BevelSize, m_HalfTile),
			new Vector3(0f, 0f - m_BevelSize, m_HalfTile),
			new Vector3(0f - m_HalfTile + m_BevelSize, 0f, m_HalfTile - m_BevelSize),
			new Vector3(0f, 0f, m_HalfTile - m_BevelSize)
		};
		tileMesh.AddQuad(0, vertices7);
		Vector3[] vertices8 = new Vector3[4]
		{
			new Vector3(0f - m_HalfTile, 0f - m_BevelSize, 0f),
			new Vector3(0f - m_HalfTile, 0f - m_BevelSize, m_HalfTile - m_BevelSize),
			new Vector3(0f - m_HalfTile + m_BevelSize, 0f, 0f),
			new Vector3(0f - m_HalfTile + m_BevelSize, 0f, m_HalfTile - m_BevelSize)
		};
		tileMesh.AddQuad(0, vertices8);
		Vector3[] vertices9 = new Vector3[4]
		{
			new Vector3(0f - m_HalfTile + m_BevelSize, 0f, m_HalfTile - m_BevelSize),
			new Vector3(0f, 0f, m_HalfTile - m_BevelSize),
			new Vector3(0f - m_HalfTile + m_BevelSize, 0f, 0f),
			new Vector3(0f, 0f, 0f)
		};
		tileMesh.AddQuad(0, vertices9);
		Vector3[] vertices10 = new Vector3[3]
		{
			new Vector3(0f - m_HalfTile, 0f - m_BevelSize, m_HalfTile - m_BevelSize),
			new Vector3(0f - m_HalfTile + m_BevelSize, 0f - m_BevelSize, m_HalfTile),
			new Vector3(0f - m_HalfTile + m_BevelSize, 0f, m_HalfTile - m_BevelSize)
		};
		tileMesh.AddTri(0, vertices10);
		m_TopLeftEdge = CreateMeshesWithRotations(tileMesh, m_HalfTile, m_HalfUSize, m_HalfVSize);
		tileMesh = new TileMesh(1);
		Vector3[] vertices11 = new Vector3[5]
		{
			new Vector3(0f, 0f, 0f),
			new Vector3(0f - m_HalfTile, 0f, 0f),
			new Vector3(0f - m_HalfTile, 0f, m_HalfTile - m_BevelSize),
			new Vector3(0f - m_HalfTile + m_BevelSize, 0f, m_HalfTile),
			new Vector3(0f, 0f, m_HalfTile)
		};
		tileMesh.AddFan(0, vertices11);
		Vector3[] vertices12 = new Vector3[4]
		{
			new Vector3(0f - m_HalfTile + m_BevelSize, 0f, m_HalfTile),
			new Vector3(0f - m_HalfTile, 0f, m_HalfTile - m_BevelSize),
			new Vector3(0f - m_HalfTile, 0f - m_BevelSize, m_HalfTile + m_BevelSize),
			new Vector3(0f - m_HalfTile - m_BevelSize, 0f - m_BevelSize, m_HalfTile)
		};
		tileMesh.AddQuad(0, vertices12);
		m_TopLeftInner = CreateMeshesWithRotations(tileMesh, m_HalfTile, m_HalfUSize, m_HalfVSize);
		tileMesh = new TileMesh(2);
		Vector3[] vertices13 = new Vector3[4]
		{
			new Vector3(0f, 0f - m_BevelSize, m_HalfTile),
			new Vector3(0f - m_HalfTile, 0f - m_BevelSize, m_HalfTile),
			new Vector3(0f, 0f - m_MaxDepth, m_HalfTile + m_DepthRatio),
			new Vector3(0f - m_HalfTile, 0f - m_MaxDepth, m_HalfTile + m_DepthRatio)
		};
		Vector2[] uVs = new Vector2[4]
		{
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f)
		};
		tileMesh.AddQuad(0, vertices13, uVs);
		m_TopEdgeWall = CreateMeshesWithRotations(tileMesh, m_HalfTile, m_HalfUSize, m_HalfVSize);
		tileMesh = new TileMesh(m_NumWallSubMeshes);
		Vector3[] vertices14 = new Vector3[4]
		{
			new Vector3(0f - m_HalfTile, 0f - m_BevelSize, m_HalfTile),
			new Vector3(0f - m_HalfTile, 0f - m_BevelSize, 0f),
			new Vector3(0f - m_HalfTile - m_DepthRatio, 0f - m_MaxDepth, m_HalfTile),
			new Vector3(0f - m_HalfTile - m_DepthRatio, 0f - m_MaxDepth, 0f)
		};
		Vector2[] uVs2 = new Vector2[4]
		{
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f)
		};
		tileMesh.AddQuad(0, vertices14, uVs2);
		m_LeftEdgeWall = CreateMeshesWithRotations(tileMesh, m_HalfTile, m_HalfUSize, m_HalfVSize);
		tileMesh = new TileMesh(m_NumWallSubMeshes);
		Vector3[] vertices15 = new Vector3[4]
		{
			new Vector3(0f, 0f - m_BevelSize, m_HalfTile),
			new Vector3(0f - m_HalfTile + m_BevelSize, 0f - m_BevelSize, m_HalfTile),
			new Vector3(0f, 0f - m_MaxDepth, m_HalfTile + m_DepthRatio),
			new Vector3(0f - m_HalfTile - m_DepthRatio + m_BevelSize, 0f - m_MaxDepth, m_HalfTile + m_DepthRatio)
		};
		Vector2[] uVs3 = new Vector2[4]
		{
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f)
		};
		tileMesh.AddQuad(0, vertices15, uVs3);
		Vector3[] vertices16 = new Vector3[4]
		{
			new Vector3(0f - m_HalfTile + m_BevelSize, 0f - m_BevelSize, m_HalfTile),
			new Vector3(0f - m_HalfTile, 0f - m_BevelSize, m_HalfTile - m_BevelSize),
			new Vector3(0f - m_HalfTile - m_DepthRatio + m_BevelSize, 0f - m_MaxDepth, m_HalfTile + m_DepthRatio),
			new Vector3(0f - m_HalfTile - m_DepthRatio, 0f - m_MaxDepth, m_HalfTile + m_DepthRatio - m_BevelSize)
		};
		Vector2[] uVs4 = new Vector2[4]
		{
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f)
		};
		tileMesh.AddQuad(0, vertices16, uVs4);
		Vector3[] vertices17 = new Vector3[4]
		{
			new Vector3(0f - m_HalfTile, 0f - m_BevelSize, m_HalfTile - m_BevelSize),
			new Vector3(0f - m_HalfTile, 0f - m_BevelSize, 0f),
			new Vector3(0f - m_HalfTile - m_DepthRatio, 0f - m_MaxDepth, m_HalfTile + m_DepthRatio - m_BevelSize),
			new Vector3(0f - m_HalfTile - m_DepthRatio, 0f - m_MaxDepth, 0f)
		};
		Vector2[] uVs5 = new Vector2[4]
		{
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f)
		};
		tileMesh.AddQuad(0, vertices17, uVs5);
		m_TopLeftEdgeWall = CreateMeshesWithRotations(tileMesh, m_HalfTile, m_HalfUSize, m_HalfVSize);
		tileMesh = new TileMesh(m_NumWallSubMeshes);
		Vector3[] vertices18 = new Vector3[4]
		{
			new Vector3(0f - m_HalfTile, 0f - m_BevelSize, m_HalfTile + m_HalfTile),
			new Vector3(0f - m_HalfTile, 0f - m_BevelSize, m_HalfTile + m_BevelSize),
			new Vector3(0f - m_HalfTile - m_DepthRatio, 0f - m_MaxDepth, m_HalfTile + m_HalfTile),
			new Vector3(0f - m_HalfTile - m_DepthRatio, 0f - m_MaxDepth, m_HalfTile + m_DepthRatio + m_BevelSize)
		};
		Vector2[] uVs6 = new Vector2[4]
		{
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f)
		};
		tileMesh.AddQuad(0, vertices18, uVs6);
		Vector3[] vertices19 = new Vector3[4]
		{
			new Vector3(0f - m_HalfTile, 0f - m_BevelSize, m_HalfTile + m_BevelSize),
			new Vector3(0f - m_HalfTile - m_BevelSize, 0f - m_BevelSize, m_HalfTile),
			new Vector3(0f - m_HalfTile - m_DepthRatio, 0f - m_MaxDepth, m_HalfTile + m_DepthRatio + m_BevelSize),
			new Vector3(0f - m_HalfTile - m_DepthRatio - m_BevelSize, 0f - m_MaxDepth, m_HalfTile + m_DepthRatio)
		};
		Vector2[] uVs7 = new Vector2[4]
		{
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f)
		};
		tileMesh.AddQuad(0, vertices19, uVs7);
		Vector3[] vertices20 = new Vector3[4]
		{
			new Vector3(0f - m_HalfTile - m_BevelSize, 0f - m_BevelSize, m_HalfTile),
			new Vector3(0f - m_HalfTile - m_HalfTile, 0f - m_BevelSize, m_HalfTile),
			new Vector3(0f - m_HalfTile - m_DepthRatio - m_BevelSize, 0f - m_MaxDepth, m_HalfTile + m_DepthRatio),
			new Vector3(0f - m_HalfTile - m_HalfTile, 0f - m_MaxDepth, m_HalfTile + m_DepthRatio)
		};
		Vector2[] uVs8 = new Vector2[4]
		{
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f)
		};
		tileMesh.AddQuad(0, vertices20, uVs8);
		m_TopLeftInnerWall = CreateMeshesWithRotations(tileMesh, m_HalfTile, m_USize, m_VSize);
	}

	public void Init()
	{
		InitVars();
		CreateGeometry();
	}

	protected TileMesh[] CreateMeshesWithRotations(TileMesh OldMesh, float HalfTile, float HalfUSize, float HalfVSize)
	{
		Vector3 vertexOffset = new Vector3(HalfTile, 0f, 0f - HalfTile);
		Vector2 uVOffset = new Vector2(HalfUSize, HalfVSize);
		return new TileMesh[4]
		{
			OldMesh.CreateNewRotatedMesh(0f, vertexOffset, uVOffset),
			OldMesh.CreateNewRotatedMesh(-90f, vertexOffset, uVOffset),
			OldMesh.CreateNewRotatedMesh(-180f, vertexOffset, uVOffset),
			OldMesh.CreateNewRotatedMesh(-270f, vertexOffset, uVOffset)
		};
	}

	private void BuildMeshSimple()
	{
		int num = 4;
		Vector3[] array = new Vector3[num];
		Vector3[] array2 = new Vector3[num];
		Vector2[] array3 = new Vector2[num];
		int[] array4 = new int[6];
		array[0] = new Vector3(0f, 0f, 0f);
		array[1] = new Vector3(Tile.m_Size * (float)Plot.m_PlotTilesWide, 0f, 0f);
		array[2] = new Vector3(0f, 0f, (0f - Tile.m_Size) * (float)Plot.m_PlotTilesHigh);
		array[3] = new Vector3(Tile.m_Size * (float)Plot.m_PlotTilesWide, 0f, (0f - Tile.m_Size) * (float)Plot.m_PlotTilesHigh);
		float num2 = (float)TileManager.Instance.m_TilesWide / (float)TileManager.Instance.m_TileMapTexture.width;
		float num3 = (float)TileManager.Instance.m_TilesHigh / (float)TileManager.Instance.m_TileMapTexture.height;
		float num4 = 1f / (float)PlotManager.Instance.m_PlotsWide * num2;
		float num5 = 1f / (float)PlotManager.Instance.m_PlotsHigh * num3;
		float num6 = (float)m_Plot.m_PlotX * num4;
		float num7 = (float)m_Plot.m_PlotY * num5;
		array3[0] = new Vector2(num6, num7);
		array3[1] = new Vector2(num6 + num4, num7);
		array3[2] = new Vector2(num6, num7 + num5);
		array3[3] = new Vector2(num6 + num4, num7 + num5);
		for (int i = 0; i < num; i++)
		{
			array2[i] = Vector3.up;
		}
		array4[0] = 0;
		array4[1] = 1;
		array4[2] = 2;
		array4[3] = 1;
		array4[4] = 3;
		array4[5] = 2;
		Mesh mesh = new Mesh();
		mesh.vertices = array;
		mesh.triangles = array4;
		mesh.normals = array2;
		mesh.uv = array3;
		m_Plot.m_Root.GetComponent<MeshFilter>().mesh = mesh;
		m_Plot.m_Root.GetComponent<MeshCollider>().sharedMesh = mesh;
		m_Plot.m_Mesh = m_Plot.m_Root.GetComponent<MeshRenderer>();
		if (m_Plot.m_PlotY == 0 || m_Plot.m_PlotY == PlotManager.Instance.m_PlotsHigh - 1 || m_Plot.m_PlotX == 0 || m_Plot.m_PlotX == PlotManager.Instance.m_PlotsWide - 1)
		{
			TileMesh wallMesh = new TileMesh(m_NumWallSubMeshes);
			SetWallMesh(wallMesh);
		}
	}

	private void GetBoundaryMesh(TileMesh NewMeshWall)
	{
		if (m_Plot.m_PlotY == 0)
		{
			float z = 0f;
			Vector3[] vertices = new Vector3[4]
			{
				new Vector3(Tile.m_Size * (float)Plot.m_PlotTilesWide, 0f, z),
				new Vector3(0f, 0f, z),
				new Vector3(Tile.m_Size * (float)Plot.m_PlotTilesWide, (0f - Tile.m_Size) * (float)Plot.m_PlotTilesWide, z),
				new Vector3(0f, (0f - Tile.m_Size) * (float)Plot.m_PlotTilesWide, z)
			};
			Vector2[] uVs = new Vector2[4]
			{
				new Vector2(1f, 1f),
				new Vector2(0f, 1f),
				new Vector2(1f, 0f),
				new Vector2(0f, 0f)
			};
			NewMeshWall.AddQuad(1, vertices, uVs);
		}
		if (m_Plot.m_PlotY == PlotManager.Instance.m_PlotsHigh - 1)
		{
			float z2 = (0f - Tile.m_Size) * (float)Plot.m_PlotTilesHigh;
			Vector3[] vertices2 = new Vector3[4]
			{
				new Vector3(0f, 0f, z2),
				new Vector3(Tile.m_Size * (float)Plot.m_PlotTilesWide, 0f, z2),
				new Vector3(0f, (0f - Tile.m_Size) * (float)Plot.m_PlotTilesWide, z2),
				new Vector3(Tile.m_Size * (float)Plot.m_PlotTilesWide, (0f - Tile.m_Size) * (float)Plot.m_PlotTilesWide, z2)
			};
			Vector2[] uVs2 = new Vector2[4]
			{
				new Vector2(1f, 1f),
				new Vector2(0f, 1f),
				new Vector2(1f, 0f),
				new Vector2(0f, 0f)
			};
			NewMeshWall.AddQuad(1, vertices2, uVs2);
		}
		if (m_Plot.m_PlotX == 0)
		{
			float x = 0f;
			Vector3[] vertices3 = new Vector3[4]
			{
				new Vector3(x, 0f, 0f),
				new Vector3(x, 0f, (0f - Tile.m_Size) * (float)Plot.m_PlotTilesHigh),
				new Vector3(x, (0f - Tile.m_Size) * (float)Plot.m_PlotTilesWide, 0f),
				new Vector3(x, (0f - Tile.m_Size) * (float)Plot.m_PlotTilesWide, (0f - Tile.m_Size) * (float)Plot.m_PlotTilesHigh)
			};
			float num = 0f;
			if (m_Plot.m_PlotY % 2 == 1)
			{
				num = 0.5f;
			}
			Vector2[] uVs3 = new Vector2[4]
			{
				new Vector2(num + 0.5f, 1f),
				new Vector2(num, 1f),
				new Vector2(num + 0.5f, 0f),
				new Vector2(num, 0f)
			};
			NewMeshWall.AddQuad(1, vertices3, uVs3);
		}
		if (m_Plot.m_PlotX == PlotManager.Instance.m_PlotsWide - 1)
		{
			float x2 = Tile.m_Size * (float)Plot.m_PlotTilesWide;
			Vector3[] vertices4 = new Vector3[4]
			{
				new Vector3(x2, 0f, (0f - Tile.m_Size) * (float)Plot.m_PlotTilesHigh),
				new Vector3(x2, 0f, 0f),
				new Vector3(x2, (0f - Tile.m_Size) * (float)Plot.m_PlotTilesWide, (0f - Tile.m_Size) * (float)Plot.m_PlotTilesHigh),
				new Vector3(x2, (0f - Tile.m_Size) * (float)Plot.m_PlotTilesWide, 0f)
			};
			float num2 = 0f;
			if (m_Plot.m_PlotY % 2 == 1)
			{
				num2 = 0.5f;
			}
			Vector2[] uVs4 = new Vector2[4]
			{
				new Vector2(num2 + 0.5f, 1f),
				new Vector2(num2, 1f),
				new Vector2(num2 + 0.5f, 0f),
				new Vector2(num2, 0f)
			};
			NewMeshWall.AddQuad(1, vertices4, uVs4);
		}
	}

	private void SetWallMesh(TileMesh NewMeshWall)
	{
		GetBoundaryMesh(NewMeshWall);
		int count = NewMeshWall.m_Vertices.Count;
		Vector3[] array = new Vector3[count];
		Vector2[] array2 = new Vector2[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = NewMeshWall.m_Vertices[i];
			array2[i] = NewMeshWall.m_UVs[i];
		}
		Vector3[] normals = new Vector3[count];
		Mesh mesh = new Mesh();
		mesh.vertices = array;
		mesh.normals = normals;
		mesh.uv = array2;
		int num2 = (mesh.subMeshCount = NewMeshWall.m_Triangles.Length);
		for (int j = 0; j < num2; j++)
		{
			int count2 = NewMeshWall.m_Triangles[j].Count;
			int[] array3 = new int[count2];
			for (int k = 0; k < count2; k++)
			{
				array3[k] = NewMeshWall.m_Triangles[j][k];
			}
			mesh.SetTriangles(array3, j);
		}
		mesh.RecalculateNormals();
		m_Plot.m_Walls.GetComponent<MeshFilter>().mesh = mesh;
		m_Plot.m_Walls.GetComponent<MeshCollider>().sharedMesh = mesh;
	}

	private void BuildMeshComplex()
	{
		TileMesh tileMesh = new TileMesh(1);
		TileMesh tileMesh2 = new TileMesh(m_NumWallSubMeshes);
		float num = (float)TileManager.Instance.m_TilesWide / (float)TileManager.Instance.m_TileMapTexture.width;
		float num2 = (float)TileManager.Instance.m_TilesHigh / (float)TileManager.Instance.m_TileMapTexture.height;
		float num3 = 1f / (float)PlotManager.Instance.m_PlotsWide * num;
		float num4 = 1f / (float)PlotManager.Instance.m_PlotsHigh * num2;
		float x = (float)m_Plot.m_PlotX * num3;
		float y = (float)m_Plot.m_PlotY * num4;
		Vector2 vector = new Vector2(x, y);
		for (int i = 0; i < Plot.m_PlotTilesHigh; i++)
		{
			for (int j = 0; j < Plot.m_PlotTilesWide; j++)
			{
				float tileHeight = TileManager.Instance.GetTileHeight(new TileCoord(j + m_Plot.m_TileX, i + m_Plot.m_TileY));
				float num5 = TileManager.Instance.GetTileHeight(new TileCoord(j + m_Plot.m_TileX - 1, i + m_Plot.m_TileY)) - tileHeight;
				float num6 = TileManager.Instance.GetTileHeight(new TileCoord(j + m_Plot.m_TileX + 1, i + m_Plot.m_TileY)) - tileHeight;
				float num7 = TileManager.Instance.GetTileHeight(new TileCoord(j + m_Plot.m_TileX, i + m_Plot.m_TileY - 1)) - tileHeight;
				float num8 = TileManager.Instance.GetTileHeight(new TileCoord(j + m_Plot.m_TileX, i + m_Plot.m_TileY + 1)) - tileHeight;
				float num9 = TileManager.Instance.GetTileHeight(new TileCoord(j + m_Plot.m_TileX - 1, i + m_Plot.m_TileY - 1)) - tileHeight;
				float num10 = TileManager.Instance.GetTileHeight(new TileCoord(j + m_Plot.m_TileX + 1, i + m_Plot.m_TileY - 1)) - tileHeight;
				float num11 = TileManager.Instance.GetTileHeight(new TileCoord(j + m_Plot.m_TileX - 1, i + m_Plot.m_TileY + 1)) - tileHeight;
				float num12 = TileManager.Instance.GetTileHeight(new TileCoord(j + m_Plot.m_TileX + 1, i + m_Plot.m_TileY + 1)) - tileHeight;
				Vector3 vertexOffset = new Vector3((float)j * Tile.m_Size, tileHeight, (float)(-i) * Tile.m_Size);
				float x2 = (float)j / (float)Plot.m_PlotTilesWide * num3;
				float y2 = (float)i / (float)Plot.m_PlotTilesHigh * num4;
				Vector2 uVOffset = new Vector2(x2, y2) + vector;
				if (num5 < 0f || num6 < 0f || num7 < 0f || num8 < 0f || num9 < 0f || num10 < 0f || num11 < 0f || num12 < 0f)
				{
					if (num7 < 0f && num5 < 0f)
					{
						tileMesh.AddMesh(m_TopLeftEdge[0], vertexOffset, uVOffset);
						tileMesh2.AddMesh(m_TopLeftEdgeWall[0], vertexOffset, uVOffset);
					}
					else if (num5 < 0f)
					{
						tileMesh.AddMesh(m_LeftEdge[0], vertexOffset, uVOffset);
						if (num9 < 0f || num7 > 0f)
						{
							tileMesh2.AddMesh(m_LeftEdgeWall[0], vertexOffset, uVOffset);
						}
					}
					else if (num7 < 0f)
					{
						tileMesh.AddMesh(m_TopEdge[0], vertexOffset, uVOffset);
						if (num9 < 0f || num5 > 0f)
						{
							tileMesh2.AddMesh(m_TopEdgeWall[0], vertexOffset, uVOffset);
						}
					}
					else if (num9 < 0f)
					{
						tileMesh.AddMesh(m_TopLeftInner[0], vertexOffset, uVOffset);
						tileMesh2.AddMesh(m_TopLeftInnerWall[0], vertexOffset, uVOffset);
					}
					else
					{
						tileMesh.AddMesh(m_Quarter[0], vertexOffset, uVOffset);
					}
					if (num7 < 0f && num6 < 0f)
					{
						tileMesh.AddMesh(m_TopLeftEdge[1], vertexOffset, uVOffset);
						tileMesh2.AddMesh(m_TopLeftEdgeWall[1], vertexOffset, uVOffset);
					}
					else if (num7 < 0f)
					{
						tileMesh.AddMesh(m_LeftEdge[1], vertexOffset, uVOffset);
						if (num10 < 0f || num6 > 0f)
						{
							tileMesh2.AddMesh(m_LeftEdgeWall[1], vertexOffset, uVOffset);
						}
					}
					else if (num6 < 0f)
					{
						tileMesh.AddMesh(m_TopEdge[1], vertexOffset, uVOffset);
						if (num10 < 0f || num7 > 0f)
						{
							tileMesh2.AddMesh(m_TopEdgeWall[1], vertexOffset, uVOffset);
						}
					}
					else if (num10 < 0f)
					{
						tileMesh.AddMesh(m_TopLeftInner[1], vertexOffset, uVOffset);
						tileMesh2.AddMesh(m_TopLeftInnerWall[1], vertexOffset, uVOffset);
					}
					else
					{
						tileMesh.AddMesh(m_Quarter[1], vertexOffset, uVOffset);
					}
					if (num8 < 0f && num6 < 0f)
					{
						tileMesh.AddMesh(m_TopLeftEdge[2], vertexOffset, uVOffset);
						tileMesh2.AddMesh(m_TopLeftEdgeWall[2], vertexOffset, uVOffset);
					}
					else if (num6 < 0f)
					{
						tileMesh.AddMesh(m_LeftEdge[2], vertexOffset, uVOffset);
						if (num12 < 0f || num8 > 0f)
						{
							tileMesh2.AddMesh(m_LeftEdgeWall[2], vertexOffset, uVOffset);
						}
					}
					else if (num8 < 0f)
					{
						tileMesh.AddMesh(m_TopEdge[2], vertexOffset, uVOffset);
						if (num12 < 0f || num6 > 0f)
						{
							tileMesh2.AddMesh(m_TopEdgeWall[2], vertexOffset, uVOffset);
						}
					}
					else if (num12 < 0f)
					{
						tileMesh.AddMesh(m_TopLeftInner[2], vertexOffset, uVOffset);
						tileMesh2.AddMesh(m_TopLeftInnerWall[2], vertexOffset, uVOffset);
					}
					else
					{
						tileMesh.AddMesh(m_Quarter[2], vertexOffset, uVOffset);
					}
					if (num8 < 0f && num5 < 0f)
					{
						tileMesh.AddMesh(m_TopLeftEdge[3], vertexOffset, uVOffset);
						tileMesh2.AddMesh(m_TopLeftEdgeWall[3], vertexOffset, uVOffset);
					}
					else if (num8 < 0f)
					{
						tileMesh.AddMesh(m_LeftEdge[3], vertexOffset, uVOffset);
						if (num11 < 0f || num5 > 0f)
						{
							tileMesh2.AddMesh(m_LeftEdgeWall[3], vertexOffset, uVOffset);
						}
					}
					else if (num5 < 0f)
					{
						tileMesh.AddMesh(m_TopEdge[3], vertexOffset, uVOffset);
						if (num11 < 0f || num8 > 0f)
						{
							tileMesh2.AddMesh(m_TopEdgeWall[3], vertexOffset, uVOffset);
						}
					}
					else if (num11 < 0f)
					{
						tileMesh.AddMesh(m_TopLeftInner[3], vertexOffset, uVOffset);
						tileMesh2.AddMesh(m_TopLeftInnerWall[3], vertexOffset, uVOffset);
					}
					else
					{
						tileMesh.AddMesh(m_Quarter[3], vertexOffset, uVOffset);
					}
				}
				else
				{
					tileMesh.AddMesh(m_Full, vertexOffset, uVOffset);
				}
			}
		}
		int count = tileMesh.m_Vertices.Count;
		Vector3[] array = new Vector3[count];
		Vector2[] array2 = new Vector2[count];
		for (int k = 0; k < count; k++)
		{
			array[k] = tileMesh.m_Vertices[k];
			array2[k] = tileMesh.m_UVs[k];
		}
		Vector3[] normals = new Vector3[count];
		Mesh mesh = new Mesh();
		mesh.vertices = array;
		mesh.normals = normals;
		mesh.uv = array2;
		int num14 = (mesh.subMeshCount = tileMesh.m_Triangles.Length);
		for (int l = 0; l < num14; l++)
		{
			int count2 = tileMesh.m_Triangles[l].Count;
			int[] array3 = new int[count2];
			for (int m = 0; m < count2; m++)
			{
				array3[m] = tileMesh.m_Triangles[l][m];
			}
			mesh.SetTriangles(array3, l);
		}
		mesh.RecalculateNormals();
		m_Plot.m_Root.GetComponent<MeshFilter>().mesh = mesh;
		m_Plot.m_Root.GetComponent<MeshCollider>().sharedMesh = mesh;
		m_Plot.m_Mesh = m_Plot.m_Root.GetComponent<MeshRenderer>();
		SetWallMesh(tileMesh2);
	}

	public virtual void BuildMesh(Plot NewPlot)
	{
		if (!m_Initted)
		{
			m_Initted = true;
			Init();
		}
		m_Plot = NewPlot;
		int num = 0;
		for (int i = -1; i < Plot.m_PlotTilesHigh + 1; i++)
		{
			for (int j = -1; j < Plot.m_PlotTilesWide + 1; j++)
			{
				TileCoord position = new TileCoord(j + m_Plot.m_TileX, i + m_Plot.m_TileY);
				if (TileManager.Instance.GetTileHeight(position) == 0f)
				{
					num++;
				}
			}
		}
		if (num == (Plot.m_PlotTilesWide + 2) * (Plot.m_PlotTilesHigh + 2))
		{
			BuildMeshSimple();
		}
		else
		{
			BuildMeshComplex();
		}
	}
}
