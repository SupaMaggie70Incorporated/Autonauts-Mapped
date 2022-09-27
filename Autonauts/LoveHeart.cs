using UnityEngine;

public class LoveHeart : Effect2D
{
	private Vector3 m_Velocity;

	private float m_Life;

	public override void Restart()
	{
		base.Restart();
		m_Velocity = new Vector3(0f, 2f, 0f);
		m_Life = 1f;
		UpdateTransform();
	}

	protected new void Awake()
	{
		base.Awake();
		SetSprite("Effects/Loveheart");
	}

	private void Update()
	{
		m_Velocity *= 0.75f;
		m_WorldPosition += m_Velocity;
		UpdateTransform();
		if ((bool)TimeManager.Instance)
		{
			m_Life -= TimeManager.Instance.m_NormalDelta;
		}
		if (m_Life <= 0f)
		{
			StopUsing();
		}
	}
}
