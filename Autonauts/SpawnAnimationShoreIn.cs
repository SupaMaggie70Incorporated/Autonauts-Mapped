using SimpleJSON;
using UnityEngine;

public class SpawnAnimationShoreIn : SpawnAnimation
{
	private TileCoord m_TilePosition;

	private Vector3 m_MovementDirection;

	private bool m_Started;

	public SpawnAnimationShoreIn()
		: base(Type.ShoreIn, null)
	{
	}

	public SpawnAnimationShoreIn(BaseClass NewObject, TileCoord TilePosition, Vector3 MovementDirection)
		: base(Type.ShoreIn, NewObject)
	{
		m_TilePosition = TilePosition;
		m_MovementDirection = MovementDirection;
	}

	public override void Save(JSONNode NewNode)
	{
		base.Save(NewNode);
		m_TilePosition.Save(NewNode, "T");
		JSONUtils.Set(NewNode, "MoveX", m_MovementDirection.x);
		JSONUtils.Set(NewNode, "MoveY", m_MovementDirection.y);
	}

	public override void Load(JSONNode NewNode)
	{
		base.Load(NewNode);
		m_TilePosition.Load(NewNode, "T");
		m_MovementDirection.x = JSONUtils.GetAsFloat(NewNode, "MoveX", 0f);
		m_MovementDirection.y = JSONUtils.GetAsFloat(NewNode, "MoveX", 0f);
	}

	public override bool Update()
	{
		base.Update();
		Vector3 vector = m_MovementDirection * 2f + new Vector3(0f, -1.2f, 0f);
		if (!m_Started)
		{
			m_NewObject.GetComponent<TileCoordObject>().SetPosition(m_TilePosition.ToWorldPositionTileCentered() + vector * 1f, UpdateCoord: false);
			if (TileMapAnimationManager.Instance.m_OldWaveDirection1 == -1f && TileMapAnimationManager.Instance.m_WaveDirection1 == 1f)
			{
				m_Started = true;
			}
		}
		else
		{
			float waveOffset = TileMapAnimationManager.Instance.m_WaveOffset1;
			m_NewObject.GetComponent<TileCoordObject>().SetPosition(m_TilePosition.ToWorldPositionTileCentered() + vector * (1f - waveOffset), UpdateCoord: false);
			if (TileMapAnimationManager.Instance.m_OldWaveDirection1 == 1f && TileMapAnimationManager.Instance.m_WaveDirection1 == -1f)
			{
				return false;
			}
		}
		return true;
	}

	public override void Start()
	{
		base.Start();
		m_Started = false;
	}

	public override void End(bool Success)
	{
		base.End(Success);
		EndInWorld();
		m_NewObject.GetComponent<TileCoordObject>().SetPosition(m_TilePosition.ToWorldPositionTileCentered());
	}
}
