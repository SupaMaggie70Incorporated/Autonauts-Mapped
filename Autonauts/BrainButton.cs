using System;
using UnityEngine;

public class BrainButton
{
	public StandardButtonImage m_Button;

	private bool m_Interactable;

	private bool m_Flashing;

	private float m_FlashTimer;

	public BrainButton(GameObject Root, string Name, Action<BaseGadget> NewAction)
	{
		m_Button = Root.transform.Find(Name).GetComponent<StandardButtonImage>();
		if (NewAction != null)
		{
			m_Button.SetAction(NewAction, m_Button);
		}
	}

	public void SetInteractable(bool Interactable)
	{
		m_Button.SetInteractable(Interactable);
		m_Interactable = Interactable;
	}

	public void SetLocked(bool Locked)
	{
		if (m_Interactable)
		{
			m_Button.SetInteractable(!Locked);
		}
	}

	public void SetSprite(string Sprite)
	{
		m_Button.SetSprite(Sprite);
	}

	public void SetFlashing(bool Flash)
	{
		m_Flashing = Flash;
		m_FlashTimer = 0f;
		m_Button.SetBackingColour(new Color(1f, 1f, 1f, 1f));
	}

	public void Update()
	{
		if (m_Flashing)
		{
			m_FlashTimer += TimeManager.Instance.m_NormalDelta;
			if ((int)(m_FlashTimer * 60f) % 12 < 6)
			{
				m_Button.SetBackingColour(new Color(0.5f, 0.5f, 0.5f, 1f));
			}
			else
			{
				m_Button.SetBackingColour(new Color(1f, 1f, 1f, 1f));
			}
		}
	}
}
