using System.Collections.Generic;
using UnityEngine;

public class ShorelineManager : MonoBehaviour
{
	public static ShorelineManager Instance;

	private Dictionary<int, ShorelineTile> m_ShorelineTiles;

	private PlaySound m_AmbienceWaves;

	private void Awake()
	{
		Instance = this;
		m_ShorelineTiles = new Dictionary<int, ShorelineTile>();
	}

	private ShorelineTile GetTileInShorelineList(TileCoord NewCoord)
	{
		int index = NewCoord.GetIndex();
		if (m_ShorelineTiles.ContainsKey(index))
		{
			return m_ShorelineTiles[index];
		}
		return null;
	}

	public void CheckAddShorelineTile(int x, int y, bool Up, bool Down, bool Left, bool Right)
	{
		if (x == 0 || y == 0 || x == TileManager.Instance.m_TilesWide - 1 || y == TileManager.Instance.m_TilesHigh - 1)
		{
			return;
		}
		TileCoord tileCoord = new TileCoord(x, y);
		if (GetTileInShorelineList(tileCoord) == null)
		{
			int direction = 0;
			if (!Up)
			{
				direction = ((!Left) ? 1 : ((!Right) ? 7 : 0));
			}
			else if (!Down)
			{
				direction = ((!Left) ? 3 : (Right ? 4 : 5));
			}
			else if (!Left)
			{
				direction = 2;
			}
			else if (!Right)
			{
				direction = 6;
			}
			m_ShorelineTiles.Add(tileCoord.GetIndex(), new ShorelineTile(tileCoord, direction));
			PlotManager.Instance.GetPlotAtTile(tileCoord).AddWaves(tileCoord);
		}
	}

	public void CheckRemoveShorelineTile(int x, int y)
	{
		TileCoord tileCoord = new TileCoord(x, y);
		if (GetTileInShorelineList(tileCoord) != null)
		{
			m_ShorelineTiles.Remove(tileCoord.GetIndex());
			PlotManager.Instance.GetPlotAtTile(tileCoord).RemoveWaves(tileCoord);
		}
	}

	private Vector3 GetMovementFromDirection(int Direction)
	{
		Vector3 result = default(Vector3);
		if (Direction == 0 || Direction == 1 || Direction == 7)
		{
			result.z = -1f;
		}
		if (Direction == 3 || Direction == 4 || Direction == 5)
		{
			result.z = 1f;
		}
		if (Direction == 1 || Direction == 2 || Direction == 3)
		{
			result.x = 1f;
		}
		if (Direction == 5 || Direction == 6 || Direction == 7)
		{
			result.x = -1f;
		}
		return result;
	}

	public Vector3 GetTileDirection(TileCoord Position)
	{
		ShorelineTile tileInShorelineList = GetTileInShorelineList(Position);
		return GetMovementFromDirection(tileInShorelineList.m_Direction);
	}

	private ShorelineTile GetRandomTile()
	{
		int num = Random.Range(0, m_ShorelineTiles.Count);
		int num2 = 0;
		foreach (KeyValuePair<int, ShorelineTile> shorelineTile in m_ShorelineTiles)
		{
			if (num2 == num)
			{
				return shorelineTile.Value;
			}
			num2++;
		}
		return null;
	}

	private void UpdateOysters()
	{
	}

	public float GetBestDistanceToWaves(Vector3 TestPosition)
	{
		TileCoord position = new TileCoord(TestPosition);
		PlotManager.Instance.GetArea(position, Plot.m_PlotTilesHigh, out var TopLeft, out var BottomRight);
		float num = 1E+08f;
		for (int i = TopLeft.y; i <= BottomRight.y; i++)
		{
			for (int j = TopLeft.x; j <= BottomRight.x; j++)
			{
				Plot plotAtPlot = PlotManager.Instance.GetPlotAtPlot(j, i);
				if ((bool)plotAtPlot)
				{
					float distanceSqrToNearestWave = plotAtPlot.GetDistanceSqrToNearestWave(TestPosition);
					if (num > distanceSqrToNearestWave)
					{
						num = distanceSqrToNearestWave;
					}
				}
			}
		}
		return Mathf.Sqrt(num);
	}

	public void StartWaves()
	{
		m_AmbienceWaves = AudioManager.Instance.StartEventAmbient("AmbienceWaves", base.gameObject, Remember: true);
		if (m_AmbienceWaves != null && m_AmbienceWaves.m_Result != null)
		{
			m_AmbienceWaves.m_Result.ActingVariation.AdjustVolume(0f);
		}
		UpdateWavesSound();
	}

	private void UpdateWavesSound()
	{
		GameStateManager.State actualState = GameStateManager.Instance.GetActualState();
		if (actualState != GameStateManager.State.Loading && actualState != GameStateManager.State.CreateWorld && m_AmbienceWaves != null && m_AmbienceWaves.m_Result != null)
		{
			Vector3 position = CameraManager.Instance.m_Camera.transform.position;
			float bestDistanceToWaves = GetBestDistanceToWaves(position);
			float num = 1f - bestDistanceToWaves / ((float)Plot.m_PlotTilesHigh * Tile.m_Size);
			if (num < 0f)
			{
				num = 0f;
			}
			if (num > 1f)
			{
				num = 1f;
			}
			m_AmbienceWaves.m_Result.ActingVariation.AdjustVolume(num);
		}
	}

	private void Update()
	{
		if (GeneralUtils.m_InGame)
		{
			UpdateOysters();
		}
		UpdateWavesSound();
	}
}
