using UnityEngine;

public class PlotMeshBuilderWater : PlotMeshBuilder
{
	public static PlotMeshBuilderWater m_Instance;

	public static float m_WaterLevel = -0.2f;

	public static float Bevel1 = 0.125f * Tile.m_Size;

	public static float Bevel2 = 0.065f * Tile.m_Size;

	private static int m_NumSubMeshes = 3;

	private TileMesh[] m_Full;

	private TileMesh[][] m_WaterTopLeft;

	private TileMesh[][] m_WaterLeft;

	private TileMesh[][] m_WaterInner;

	private float Small = 0.01f;

	private float m_SurfWidth = 0.5f;

	private void Awake()
	{
		m_Instance = this;
		m_Initted = false;
	}

	private void CreateFullGeometry(int Index)
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
		m_Full[Index] = new TileMesh(m_NumSubMeshes);
		m_Full[Index].AddQuad(0, vertices, colours);
	}

	private void CreateTopLeftGeometry(int Index)
	{
		TileMesh tileMesh = new TileMesh(m_NumSubMeshes);
		Vector3[] vertices = new Vector3[5]
		{
			new Vector3(m_HalfTile, 0f, 0f - m_HalfTile),
			new Vector3(0f - m_HalfTile, 0f, 0f - m_HalfTile),
			new Vector3(0f - m_HalfTile, 0f, m_HalfTile - Bevel1),
			new Vector3(0f - m_HalfTile + Bevel1, 0f, m_HalfTile),
			new Vector3(m_HalfTile, 0f, m_HalfTile)
		};
		Color[] colours = new Color[5]
		{
			new Color(0f, 0f, 0f),
			new Color(1f, 0f, 0f),
			new Color(1f, -1f, 0f),
			new Color(1f, -1f, 0f),
			new Color(0f, -1f, 0f)
		};
		tileMesh.AddFan(0, vertices, colours);
		Vector3[] vertices2 = new Vector3[7]
		{
			new Vector3(0f - m_HalfTile, 0f, m_HalfTile),
			new Vector3(m_HalfTile, 0f, m_HalfTile),
			new Vector3(m_HalfTile, 0f, m_HalfTile - Small),
			new Vector3(0f - m_HalfTile + Bevel1, 0f, m_HalfTile - Small),
			new Vector3(0f - m_HalfTile + Small, 0f, m_HalfTile - Bevel1),
			new Vector3(0f - m_HalfTile + Small, 0f, 0f - m_HalfTile),
			new Vector3(0f - m_HalfTile, 0f, 0f - m_HalfTile)
		};
		Color[] colours2 = new Color[7]
		{
			new Color(0f, 0f, 1f),
			new Color(0f, 0f, 1f),
			new Color(0f, -1f, 1f),
			new Color(1f, -1f, 1f),
			new Color(1f, -1f, 1f),
			new Color(1f, 0f, 1f),
			new Color(0f, 0f, 1f)
		};
		tileMesh.AddFan(0, vertices2, colours2);
		if (Index == 0)
		{
			Vector3[] vertices3 = new Vector3[8]
			{
				new Vector3(0f - m_HalfTile + m_SurfWidth, 0f, 0f - m_HalfTile),
				new Vector3(0f - m_HalfTile, 0f, 0f - m_HalfTile),
				new Vector3(0f - m_HalfTile + m_SurfWidth, 0f, m_HalfTile - m_SurfWidth - Bevel2),
				new Vector3(0f - m_HalfTile, 0f, m_HalfTile - Bevel1),
				new Vector3(0f - m_HalfTile + m_SurfWidth + Bevel2, 0f, m_HalfTile - m_SurfWidth),
				new Vector3(0f - m_HalfTile + Bevel1, 0f, m_HalfTile),
				new Vector3(m_HalfTile, 0f, m_HalfTile - m_SurfWidth),
				new Vector3(m_HalfTile, 0f, m_HalfTile)
			};
			Color[] colours3 = new Color[8]
			{
				new Color(1f, 0f, 0f),
				new Color(1f, 0f, 0f),
				new Color(1f, -1f, 0f),
				new Color(1f, -1f, 0f),
				new Color(1f, -1f, 0f),
				new Color(1f, -1f, 0f),
				new Color(0f, -1f, 0f),
				new Color(0f, -1f, 0f)
			};
			tileMesh.AddStrip(1, vertices3, colours3);
			Vector3[] vertices4 = new Vector3[8]
			{
				new Vector3(0f - m_HalfTile + Small, 0f, 0f - m_HalfTile),
				new Vector3(0f - m_HalfTile + Bevel2, 0f, 0f - m_HalfTile),
				new Vector3(0f - m_HalfTile + Small, 0f, m_HalfTile - Small - Bevel1),
				new Vector3(0f - m_HalfTile + Bevel2, 0f, m_HalfTile - Bevel2 - Bevel1),
				new Vector3(0f - m_HalfTile + Bevel1, 0f, m_HalfTile - Small),
				new Vector3(0f - m_HalfTile + Bevel2 + Bevel1, 0f, m_HalfTile - Bevel2),
				new Vector3(m_HalfTile, 0f, m_HalfTile - Small),
				new Vector3(m_HalfTile, 0f, m_HalfTile - Bevel2)
			};
			Color[] colours4 = new Color[8]
			{
				new Color(1f, 0f, 0f),
				new Color(0f, 0f, 0f),
				new Color(1f, -1f, 0f),
				new Color(0f, 0f, 0f),
				new Color(1f, -1f, 0f),
				new Color(0f, 0f, 0f),
				new Color(0f, -1f, 0f),
				new Color(0f, 0f, 0f)
			};
			tileMesh.AddStrip(2, vertices4, colours4);
		}
		m_WaterTopLeft[Index] = CreateMeshesWithRotations(tileMesh, m_HalfTile, m_USize, m_VSize);
	}

	private void CreateLeftGeometry(int Index)
	{
		TileMesh tileMesh = new TileMesh(m_NumSubMeshes);
		Vector3[] vertices = new Vector3[4]
		{
			new Vector3(m_HalfTile, 0f, 0f - m_HalfTile),
			new Vector3(0f - m_HalfTile, 0f, 0f - m_HalfTile),
			new Vector3(0f - m_HalfTile, 0f, m_HalfTile),
			new Vector3(m_HalfTile, 0f, m_HalfTile)
		};
		Color[] colours = new Color[4]
		{
			new Color(0f, 0f, 0f),
			new Color(1f, 0f, 0f),
			new Color(1f, 0f, 0f),
			new Color(0f, 0f, 0f)
		};
		tileMesh.AddFan(0, vertices, colours);
		Vector3[] vertices2 = new Vector3[4]
		{
			new Vector3(0f - m_HalfTile, 0f, m_HalfTile),
			new Vector3(0f - m_HalfTile + Small, 0f, m_HalfTile),
			new Vector3(0f - m_HalfTile + Small, 0f, 0f - m_HalfTile),
			new Vector3(0f - m_HalfTile, 0f, 0f - m_HalfTile)
		};
		Color[] colours2 = new Color[4]
		{
			new Color(0f, 0f, 1f),
			new Color(1f, 0f, 1f),
			new Color(1f, 0f, 1f),
			new Color(0f, 0f, 1f)
		};
		tileMesh.AddFan(0, vertices2, colours2);
		if (Index == 0)
		{
			Vector3[] vertices3 = new Vector3[4]
			{
				new Vector3(0f - m_HalfTile, 0f, 0f - m_HalfTile),
				new Vector3(0f - m_HalfTile, 0f, m_HalfTile),
				new Vector3(0f - m_HalfTile + m_SurfWidth, 0f, m_HalfTile),
				new Vector3(0f - m_HalfTile + m_SurfWidth, 0f, 0f - m_HalfTile)
			};
			Color[] colours3 = new Color[4]
			{
				new Color(1f, 0f, 0f),
				new Color(1f, 0f, 0f),
				new Color(1f, 0f, 0f),
				new Color(1f, 0f, 0f)
			};
			tileMesh.AddFan(1, vertices3, colours3);
			Vector3[] vertices4 = new Vector3[4]
			{
				new Vector3(0f - m_HalfTile + Bevel2, 0f, m_HalfTile),
				new Vector3(0f - m_HalfTile, 0f, m_HalfTile),
				new Vector3(0f - m_HalfTile, 0f, 0f - m_HalfTile),
				new Vector3(0f - m_HalfTile + Bevel2, 0f, 0f - m_HalfTile)
			};
			Color[] colours4 = new Color[4]
			{
				new Color(0f, 0f, 0f),
				new Color(1f, 0f, 0f),
				new Color(1f, 0f, 0f),
				new Color(0f, 0f, 0f)
			};
			tileMesh.AddFan(2, vertices4, colours4);
		}
		m_WaterLeft[Index] = CreateMeshesWithRotations(tileMesh, m_HalfTile, m_USize, m_VSize);
	}

	private void CreateInnerGeometry(int Index)
	{
		TileMesh tileMesh = new TileMesh(m_NumSubMeshes);
		Vector3[] vertices = new Vector3[3]
		{
			new Vector3(m_HalfTile, 0f, 0f - m_HalfTile),
			new Vector3(0f - m_HalfTile, 0f, 0f - m_HalfTile),
			new Vector3(0f - m_HalfTile, 0f, m_HalfTile)
		};
		Color[] colours = new Color[3]
		{
			new Color(0f, 0f, 0f),
			new Color(0f, 0f, 0f),
			new Color(0f, -1f, 0f)
		};
		tileMesh.AddTri(0, vertices, colours);
		Vector3[] vertices2 = new Vector3[3]
		{
			new Vector3(m_HalfTile, 0f, 0f - m_HalfTile),
			new Vector3(0f - m_HalfTile - Bevel2, 0f, m_HalfTile),
			new Vector3(0f - m_HalfTile, 0f, m_HalfTile)
		};
		Color[] colours2 = new Color[3]
		{
			new Color(0f, 0f, 0f),
			new Color(1f, -1f, 0f),
			new Color(0f, -1f, 0f)
		};
		tileMesh.AddTri(0, vertices2, colours2);
		Vector3[] vertices3 = new Vector3[3]
		{
			new Vector3(m_HalfTile, 0f, 0f - m_HalfTile),
			new Vector3(0f - m_HalfTile - Bevel2, 0f, m_HalfTile),
			new Vector3(0f - m_HalfTile, 0f, m_HalfTile + Bevel2)
		};
		Color[] colours3 = new Color[3]
		{
			new Color(0f, 0f, 0f),
			new Color(1f, -1f, 0f),
			new Color(1f, -1f, 0f)
		};
		tileMesh.AddTri(0, vertices3, colours3);
		Vector3[] vertices4 = new Vector3[3]
		{
			new Vector3(m_HalfTile, 0f, 0f - m_HalfTile),
			new Vector3(0f - m_HalfTile, 0f, m_HalfTile),
			new Vector3(0f - m_HalfTile, 0f, m_HalfTile + Bevel2)
		};
		Color[] colours4 = new Color[3]
		{
			new Color(0f, 0f, 0f),
			new Color(1f, 0f, 0f),
			new Color(1f, -1f, 0f)
		};
		tileMesh.AddTri(0, vertices4, colours4);
		Vector3[] vertices5 = new Vector3[3]
		{
			new Vector3(m_HalfTile, 0f, 0f - m_HalfTile),
			new Vector3(0f - m_HalfTile, 0f, m_HalfTile),
			new Vector3(m_HalfTile, 0f, m_HalfTile)
		};
		Color[] colours5 = new Color[3]
		{
			new Color(0f, 0f, 0f),
			new Color(1f, 0f, 0f),
			new Color(0f, 0f, 0f)
		};
		tileMesh.AddTri(0, vertices5, colours5);
		float num = 0f;
		Vector3[] vertices6 = new Vector3[3]
		{
			new Vector3(0f - m_HalfTile, 0f, m_HalfTile),
			new Vector3(0f - m_HalfTile + num - Small, 0f, m_HalfTile),
			new Vector3(0f - m_HalfTile + num, 0f, m_HalfTile - num + Bevel2)
		};
		Color[] colours6 = new Color[3]
		{
			new Color(0f, 0f, 1f),
			new Color(1f, 0f, 1f),
			new Color(1f, -1f, 1f)
		};
		tileMesh.AddTri(0, vertices6, colours6);
		Vector3[] vertices7 = new Vector3[3]
		{
			new Vector3(0f - m_HalfTile, 0f, m_HalfTile),
			new Vector3(0f - m_HalfTile + num - Bevel2, 0f, m_HalfTile - num),
			new Vector3(0f - m_HalfTile + num, 0f, m_HalfTile - num + Bevel2)
		};
		Color[] colours7 = new Color[3]
		{
			new Color(0f, 0f, 1f),
			new Color(1f, -1f, 1f),
			new Color(1f, -1f, 1f)
		};
		tileMesh.AddTri(0, vertices7, colours7);
		Vector3[] vertices8 = new Vector3[3]
		{
			new Vector3(0f - m_HalfTile, 0f, m_HalfTile),
			new Vector3(0f - m_HalfTile + num - Bevel2, 0f, m_HalfTile - num),
			new Vector3(0f - m_HalfTile, 0f, m_HalfTile - num + Small)
		};
		Color[] colours8 = new Color[3]
		{
			new Color(0f, 0f, 1f),
			new Color(1f, -1f, 1f),
			new Color(0f, -1f, 1f)
		};
		tileMesh.AddTri(0, vertices8, colours8);
		if (Index == 0)
		{
			Color[] array = new Color[8]
			{
				new Color(0f, -1f, 0f),
				new Color(0f, -1f, 0f),
				new Color(1f, -1f, 0f),
				new Color(1f, -1f, 0f),
				new Color(1f, -1f, 0f),
				new Color(1f, -1f, 0f),
				new Color(1f, 0f, 0f),
				new Color(1f, 0f, 0f)
			};
			Vector3[] array2 = new Vector3[8]
			{
				new Vector3(0f - m_HalfTile, 0f, m_HalfTile - Bevel2 - m_SurfWidth),
				new Vector3(0f - m_HalfTile, 0f, m_HalfTile - Bevel2),
				new Vector3(0f - m_HalfTile + m_SurfWidth, 0f, m_HalfTile - Bevel2 - m_SurfWidth),
				new Vector3(0f - m_HalfTile + Small, 0f, m_HalfTile - Bevel2),
				new Vector3(0f - m_HalfTile + Bevel2 + m_SurfWidth, 0f, m_HalfTile - m_SurfWidth),
				new Vector3(0f - m_HalfTile + Bevel2, 0f, m_HalfTile - Small),
				new Vector3(0f - m_HalfTile + Bevel2 + m_SurfWidth, 0f, m_HalfTile),
				new Vector3(0f - m_HalfTile + Bevel2, 0f, m_HalfTile)
			};
			Vector3 vector = default(Vector3);
			for (int i = 0; i < array2.Length; i++)
			{
				vector.x = array[i].r;
				vector.z = array[i].g;
				vector.y = 0f;
				array2[i] -= vector * Bevel2;
			}
			Vector3[] vertices9 = new Vector3[3]
			{
				array2[0],
				array2[1],
				array2[2]
			};
			Color[] colours9 = new Color[3]
			{
				array[0],
				array[1],
				array[2]
			};
			tileMesh.AddTri(1, vertices9, colours9);
			Vector3[] vertices10 = new Vector3[3]
			{
				array2[1],
				array2[2],
				array2[3]
			};
			Color[] colours10 = new Color[3]
			{
				array[1],
				array[2],
				array[3]
			};
			tileMesh.AddTri(1, vertices10, colours10);
			Vector3[] vertices11 = new Vector3[3]
			{
				array2[2],
				array2[3],
				array2[4]
			};
			Color[] colours11 = new Color[3]
			{
				array[2],
				array[3],
				array[4]
			};
			tileMesh.AddTri(1, vertices11, colours11);
			Vector3[] vertices12 = new Vector3[3]
			{
				array2[3],
				array2[5],
				array2[4]
			};
			Color[] colours12 = new Color[3]
			{
				array[3],
				array[5],
				array[4]
			};
			tileMesh.AddTri(1, vertices12, colours12);
			Vector3[] vertices13 = new Vector3[3]
			{
				array2[4],
				array2[5],
				array2[6]
			};
			Color[] colours13 = new Color[3]
			{
				array[4],
				array[5],
				array[6]
			};
			tileMesh.AddTri(1, vertices13, colours13);
			Vector3[] vertices14 = new Vector3[3]
			{
				array2[5],
				array2[6],
				array2[7]
			};
			Color[] colours14 = new Color[3]
			{
				array[5],
				array[6],
				array[7]
			};
			tileMesh.AddTri(1, vertices14, colours14);
			Vector3[] array3 = new Vector3[6]
			{
				new Vector3(0f - m_HalfTile, 0f, m_HalfTile - Small),
				new Vector3(0f - m_HalfTile, 0f, m_HalfTile - Bevel2),
				new Vector3(0f - m_HalfTile - Bevel2 + Small, 0f, m_HalfTile - Small),
				new Vector3(0f - m_HalfTile + Bevel2, 0f, m_HalfTile),
				new Vector3(0f - m_HalfTile + Small, 0f, m_HalfTile + Bevel2 - Small),
				new Vector3(0f - m_HalfTile + Small, 0f, m_HalfTile)
			};
			Color[] array4 = new Color[6]
			{
				new Color(0f, -1f, 0f),
				new Color(0f, 0f, 0f),
				new Color(1f, -1f, 0f),
				new Color(0f, 0f, 0f),
				new Color(1f, -1f, 0f),
				new Color(1f, 0f, 0f)
			};
			Vector3[] vertices15 = new Vector3[3]
			{
				array3[0],
				array3[2],
				array3[1]
			};
			Color[] colours15 = new Color[3]
			{
				array4[0],
				array4[2],
				array4[1]
			};
			tileMesh.AddTri(2, vertices15, colours15);
			Vector3[] vertices16 = new Vector3[3]
			{
				array3[1],
				array3[3],
				array3[2]
			};
			Color[] colours16 = new Color[3]
			{
				array4[1],
				array4[3],
				array4[2]
			};
			tileMesh.AddTri(2, vertices16, colours16);
			Vector3[] vertices17 = new Vector3[3]
			{
				array3[2],
				array3[3],
				array3[4]
			};
			Color[] colours17 = new Color[3]
			{
				array4[2],
				array4[3],
				array4[4]
			};
			tileMesh.AddTri(2, vertices17, colours17);
			Vector3[] vertices18 = new Vector3[3]
			{
				array3[3],
				array3[4],
				array3[5]
			};
			Color[] colours18 = new Color[3]
			{
				array4[3],
				array4[4],
				array4[5]
			};
			tileMesh.AddTri(2, vertices18, colours18);
		}
		m_WaterInner[Index] = CreateMeshesWithRotations(tileMesh, m_HalfTile, m_USize, m_VSize);
	}

	protected override void CreateGeometry()
	{
		int num = 2;
		m_Full = new TileMesh[num];
		m_WaterTopLeft = new TileMesh[num][];
		m_WaterLeft = new TileMesh[num][];
		m_WaterInner = new TileMesh[num][];
		for (int i = 0; i < num; i++)
		{
			CreateFullGeometry(i);
			CreateTopLeftGeometry(i);
			CreateLeftGeometry(i);
			CreateInnerGeometry(i);
		}
	}

	private bool DoesPlotContainSeaWater()
	{
		bool result = false;
		for (int i = 0; i < Plot.m_PlotTilesHigh; i++)
		{
			for (int j = 0; j < Plot.m_PlotTilesWide; j++)
			{
				TileCoord position = new TileCoord(j + m_Plot.m_TileX, i + m_Plot.m_TileY);
				if (TileHelpers.GetTileWater(TileManager.Instance.GetTile(position).m_TileType))
				{
					result = true;
				}
			}
		}
		return result;
	}

	private bool GetIsWaterWaveEdge(Tile.TileType OldType, TileCoord NewPosition)
	{
		if (OldType != Tile.TileType.SeaWaterDeep && OldType != Tile.TileType.SeaWaterShallow)
		{
			return false;
		}
		Tile.TileType tileTypeCapped = TileManager.Instance.GetTileTypeCapped(NewPosition);
		if (OldType == Tile.TileType.SeaWaterShallow && tileTypeCapped == Tile.TileType.SeaWaterDeep)
		{
			return false;
		}
		if (tileTypeCapped == OldType)
		{
			return false;
		}
		return true;
	}

	private void BuildWaterMesh()
	{
		TileMesh tileMesh = new TileMesh(m_NumSubMeshes);
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
				TileCoord position = new TileCoord(j + m_Plot.m_TileX, i + m_Plot.m_TileY);
				Tile tile = TileManager.Instance.GetTile(position);
				if (!TileHelpers.GetTileWater(tile.m_TileType))
				{
					continue;
				}
				Vector3 vertexOffset = new Vector3((float)j * Tile.m_Size, m_WaterLevel, (float)(-i) * Tile.m_Size);
				float x2 = (float)j / (float)Plot.m_PlotTilesWide * num3;
				float y2 = (float)i / (float)Plot.m_PlotTilesHigh * num4;
				Vector2 uVOffset = new Vector2(x2, y2) + vector;
				bool isWaterWaveEdge = GetIsWaterWaveEdge(tile.m_TileType, new TileCoord(position.x, position.y - 1));
				bool isWaterWaveEdge2 = GetIsWaterWaveEdge(tile.m_TileType, new TileCoord(position.x, position.y + 1));
				bool isWaterWaveEdge3 = GetIsWaterWaveEdge(tile.m_TileType, new TileCoord(position.x - 1, position.y));
				bool isWaterWaveEdge4 = GetIsWaterWaveEdge(tile.m_TileType, new TileCoord(position.x + 1, position.y));
				bool isWaterWaveEdge5 = GetIsWaterWaveEdge(tile.m_TileType, new TileCoord(position.x - 1, position.y - 1));
				bool isWaterWaveEdge6 = GetIsWaterWaveEdge(tile.m_TileType, new TileCoord(position.x + 1, position.y - 1));
				bool isWaterWaveEdge7 = GetIsWaterWaveEdge(tile.m_TileType, new TileCoord(position.x - 1, position.y + 1));
				bool isWaterWaveEdge8 = GetIsWaterWaveEdge(tile.m_TileType, new TileCoord(position.x + 1, position.y + 1));
				int num5 = 0;
				if (tile.m_TileType == Tile.TileType.SeaWaterDeep)
				{
					num5 = 1;
				}
				if (isWaterWaveEdge)
				{
					if (isWaterWaveEdge && isWaterWaveEdge3)
					{
						tileMesh.AddMesh(m_WaterTopLeft[num5][0], vertexOffset, uVOffset);
					}
					else if (isWaterWaveEdge && isWaterWaveEdge4)
					{
						tileMesh.AddMesh(m_WaterTopLeft[num5][1], vertexOffset, uVOffset);
					}
					else
					{
						tileMesh.AddMesh(m_WaterLeft[num5][1], vertexOffset, uVOffset);
					}
				}
				else if (isWaterWaveEdge2)
				{
					if (isWaterWaveEdge2 && isWaterWaveEdge4)
					{
						tileMesh.AddMesh(m_WaterTopLeft[num5][2], vertexOffset, uVOffset);
					}
					else if (isWaterWaveEdge2 && isWaterWaveEdge3)
					{
						tileMesh.AddMesh(m_WaterTopLeft[num5][3], vertexOffset, uVOffset);
					}
					else
					{
						tileMesh.AddMesh(m_WaterLeft[num5][3], vertexOffset, uVOffset);
					}
				}
				else if (isWaterWaveEdge3)
				{
					tileMesh.AddMesh(m_WaterLeft[num5][0], vertexOffset, uVOffset);
				}
				else if (isWaterWaveEdge4)
				{
					tileMesh.AddMesh(m_WaterLeft[num5][2], vertexOffset, uVOffset);
				}
				else if (isWaterWaveEdge5)
				{
					tileMesh.AddMesh(m_WaterInner[num5][0], vertexOffset, uVOffset);
				}
				else if (isWaterWaveEdge6)
				{
					tileMesh.AddMesh(m_WaterInner[num5][1], vertexOffset, uVOffset);
				}
				else if (isWaterWaveEdge8)
				{
					tileMesh.AddMesh(m_WaterInner[num5][2], vertexOffset, uVOffset);
				}
				else if (isWaterWaveEdge7)
				{
					tileMesh.AddMesh(m_WaterInner[num5][3], vertexOffset, uVOffset);
				}
				else
				{
					tileMesh.AddMesh(m_Full[num5], vertexOffset, uVOffset);
				}
			}
		}
		int count = tileMesh.m_Vertices.Count;
		Vector3[] array = new Vector3[count];
		Vector2[] array2 = new Vector2[count];
		Color[] array3 = new Color[count];
		for (int k = 0; k < count; k++)
		{
			array[k] = tileMesh.m_Vertices[k];
			array2[k] = tileMesh.m_UVs[k];
			array3[k] = tileMesh.m_Colours[k];
		}
		Vector3[] normals = new Vector3[count];
		Mesh mesh = new Mesh();
		mesh.vertices = array;
		mesh.normals = normals;
		mesh.uv = array2;
		mesh.colors = array3;
		int num7 = (mesh.subMeshCount = tileMesh.m_Triangles.Length);
		for (int l = 0; l < num7; l++)
		{
			int count2 = tileMesh.m_Triangles[l].Count;
			int[] array4 = new int[count2];
			for (int m = 0; m < count2; m++)
			{
				array4[m] = tileMesh.m_Triangles[l][m];
			}
			mesh.SetTriangles(array4, l);
		}
		mesh.RecalculateNormals();
		m_Plot.m_Water.GetComponent<MeshFilter>().mesh = mesh;
		m_Plot.m_Water.GetComponent<MeshCollider>().sharedMesh = mesh;
		m_Plot.m_WaterMesh = m_Plot.m_Water.GetComponent<MeshRenderer>();
	}

	public override void BuildMesh(Plot NewPlot)
	{
		if (!m_Initted)
		{
			m_Initted = true;
			Init();
		}
		m_Plot = NewPlot;
		if (DoesPlotContainSeaWater())
		{
			BuildWaterMesh();
		}
		else
		{
			m_Plot.m_WaterMesh = null;
		}
	}
}
