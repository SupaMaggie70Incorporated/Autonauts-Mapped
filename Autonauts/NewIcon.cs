using UnityEngine;

public class NewIcon : Effect2D
{
	public override void Restart()
	{
		base.Restart();
		UpdateTransform();
	}

	protected new void Awake()
	{
		base.Awake();
		SetSprite("Ceremonies/NewThing");
	}

	private void Update()
	{
		m_Timer += TimeManager.Instance.m_NormalDelta;
		Color color = new Color(1f, 0f, 0f);
		if ((int)(m_Timer * 60f) % 10 < 5)
		{
			color = new Color(1f, 1f, 1f);
		}
		m_Image.color = color;
		UpdateTransform();
	}
}
