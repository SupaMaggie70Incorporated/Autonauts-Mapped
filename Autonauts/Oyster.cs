using SimpleJSON;
using UnityEngine;

public class Oyster : Holdable
{
	private bool m_Wild;

	private static float m_LifeDelay = 20f;

	private new void Awake()
	{
		base.Awake();
		base.enabled = true;
	}

	public override void Save(JSONNode Node)
	{
		base.Save(Node);
		JSONUtils.Set(Node, "PU", m_Wild);
	}

	public override void Load(JSONNode Node)
	{
		base.Load(Node);
		m_Wild = JSONUtils.GetAsBool(Node, "PU", DefaultValue: false);
	}

	protected override void ActionBeingHeld(Actionable Holder)
	{
		base.ActionBeingHeld(Holder);
		base.enabled = false;
		m_Wild = false;
		base.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
	}

	public void SetWild()
	{
		m_Wild = true;
	}

	private void Update()
	{
		if (!m_Wild)
		{
			base.enabled = false;
		}
		else if (!BaggedManager.Instance.IsObjectBagged(this))
		{
			m_LifeTimer += TimeManager.Instance.m_NormalDelta;
			if (m_LifeTimer > m_LifeDelay && TileMapAnimationManager.Instance.m_OldWaveDirection1 == 1f && TileMapAnimationManager.Instance.m_WaveDirection1 == -1f)
			{
				Vector3 tileDirection = ShorelineManager.Instance.GetTileDirection(m_TileCoord);
				SpawnAnimationManager.Instance.AddShoreOut(this, m_TileCoord, tileDirection);
			}
		}
	}
}
