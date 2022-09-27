using UnityEngine;

public class WaterSparkles : MonoBehaviour
{
	private MyParticles m_Particles;

	private void Awake()
	{
		m_Particles = GetComponent<MyParticles>();
	}

	private void Update()
	{
		if (TileManager.Instance == null || PlotManager.Instance == null)
		{
			return;
		}
		Vector3 position = CameraManager.Instance.m_Camera.transform.position;
		position.y = 0f;
		base.transform.position = position;
		for (int i = 0; i < 3; i++)
		{
			float num = 50f;
			float x = Random.Range(0f - num, num);
			float z = Random.Range(0f - num, num);
			position = new Vector3(x, 0f, z);
			position += base.transform.position;
			TileCoord position2 = new TileCoord(position);
			Tile.TileType tileType = TileManager.Instance.GetTileType(position2);
			if ((tileType == Tile.TileType.SeaWaterDeep || tileType == Tile.TileType.SeaWaterShallow) && PlotManager.Instance.GetPlotAtTile(position2).m_Visible)
			{
				ParticleSystem.EmitParams emitParams = default(ParticleSystem.EmitParams);
				emitParams.position = position;
				m_Particles.m_Particles.Emit(emitParams, 1);
			}
		}
	}
}
