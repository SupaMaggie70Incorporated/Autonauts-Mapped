using System.Collections.Generic;
using UnityEngine;

public class WallFloorIcon : Effect2D
{
	private bool m_WallsNeeded;

	private bool m_FloorsNeeded;

	private bool m_PowerNeeded;

	private List<string> m_SpriteNames;

	private int m_NameIndex;

	private float m_NameTimer;

	private static float m_NameDelay = 1f;

	public override void Restart()
	{
		base.Restart();
		UpdateTransform();
		m_SpriteNames = new List<string>();
		m_WallsNeeded = false;
		m_FloorsNeeded = false;
		m_PowerNeeded = false;
	}

	protected new void Awake()
	{
		base.Awake();
		SetSprite("WallNeeded");
	}

	private void UpdateSprite()
	{
		SetSprite(m_SpriteNames[m_NameIndex]);
	}

	public void Set(bool WallsNeeded, bool FloorsNeeded, bool PowerNeeded)
	{
		if (m_WallsNeeded != WallsNeeded || m_FloorsNeeded != FloorsNeeded || m_PowerNeeded != PowerNeeded)
		{
			m_WallsNeeded = WallsNeeded;
			m_FloorsNeeded = FloorsNeeded;
			m_PowerNeeded = PowerNeeded;
			m_SpriteNames.Clear();
			if (FloorsNeeded)
			{
				m_SpriteNames.Add("FloorNeeded");
			}
			if (WallsNeeded)
			{
				m_SpriteNames.Add("WallNeeded");
			}
			if (PowerNeeded)
			{
				m_SpriteNames.Add("PowerNeeded");
			}
			m_NameIndex = 0;
			m_NameTimer = 0f;
			if (m_SpriteNames.Count > 0)
			{
				UpdateSprite();
			}
		}
		base.gameObject.SetActive(WallsNeeded || FloorsNeeded || PowerNeeded);
		UpdateTransform();
	}

	public Color GetColour()
	{
		return m_Image.color;
	}

	public Sprite GetSprite()
	{
		return m_Image.sprite;
	}

	private void UpdateFlashing()
	{
		m_Timer += TimeManager.Instance.m_NormalDelta;
		Color color = new Color(1f, 0f, 0f);
		if ((int)(m_Timer * 60f) % 10 < 5)
		{
			color = new Color(1f, 1f, 1f);
		}
		m_Image.color = color;
	}

	private void UpdateSpriteCycle()
	{
		m_NameTimer += TimeManager.Instance.m_NormalDelta;
		if (m_NameTimer >= m_NameDelay)
		{
			m_NameTimer = 0f;
			m_NameIndex++;
			if (m_NameIndex >= m_SpriteNames.Count)
			{
				m_NameIndex = 0;
			}
			UpdateSprite();
		}
	}

	private void Update()
	{
		if (!(TimeManager.Instance == null))
		{
			UpdateFlashing();
			UpdateSpriteCycle();
			UpdateTransform();
		}
	}
}
