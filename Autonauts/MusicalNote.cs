using UnityEngine;

public class MusicalNote : Effect2D
{
	private Vector3 m_Velocity;

	private float m_Life;

	public override void Restart()
	{
		base.Restart();
		m_Velocity = new Vector3(0f, 1f, 0f);
		m_Life = 0.5f;
		UpdateTransform();
	}

	protected new void Awake()
	{
		base.Awake();
		SetSprite("Effects/MusicalNote");
	}

	private void Update()
	{
		if (!(TimeManager.Instance == null))
		{
			m_Velocity *= 0.75f;
			m_WorldPosition += m_Velocity;
			UpdateTransform();
			m_Life -= TimeManager.Instance.m_NormalDelta;
			if (m_Life <= 0f)
			{
				StopUsing();
			}
		}
	}
}
