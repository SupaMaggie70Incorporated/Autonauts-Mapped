using UnityEngine;

public class XPPlus1 : Effect2D
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
		SetSprite("Effects/XPPlus1");
	}

	public void SetComplete(bool Complete, bool Idea)
	{
		if (!Idea)
		{
			if (Complete)
			{
				SetSprite("Effects/XPComplete");
			}
			else
			{
				SetSprite("Effects/XPPlus1");
			}
		}
		else if (Complete)
		{
			SetSprite("Effects/IdeaComplete");
		}
		else
		{
			SetSprite("Effects/IdeaPlus1");
		}
	}

	public void SetSpacePort(bool Complete)
	{
		if (Complete)
		{
			SetSprite("Effects/SpacePortComplete");
		}
		else
		{
			SetSprite("Effects/SpacePortPlus1");
		}
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
