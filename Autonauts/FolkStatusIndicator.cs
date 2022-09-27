using UnityEngine;
using UnityEngine.UI;

public class FolkStatusIndicator : Indicator
{
	[HideInInspector]
	public enum State
	{
		None,
		Unhoused,
		Happy,
		Unhappy,
		BadObject,
		Total
	}

	[HideInInspector]
	public State m_State;

	private float m_StateTimer;

	private Image m_Image;

	private Folk m_FolkOwner;

	private Housing m_HousingOwner;

	private string[] m_UnhappyImages = new string[5] { "", "FolkHungry", "FolkHousing", "FolkClothing", "FolkDirty" };

	public void Restart()
	{
	}

	protected new void Awake()
	{
		base.Awake();
		m_Offset = 100f;
	}

	public void SetFolk(Actionable NewOwner)
	{
		m_Image = GetComponent<Image>();
		if (NewOwner.m_TypeIdentifier == ObjectType.Folk)
		{
			m_FolkOwner = NewOwner.GetComponent<Folk>();
		}
		else
		{
			m_HousingOwner = NewOwner.GetComponent<Housing>();
		}
		SetState(State.Unhoused);
	}

	public void SetState(State NewState)
	{
		State state = m_State;
		m_State = NewState;
		if (state != m_State)
		{
			if (NewState == State.None)
			{
				m_Image.enabled = false;
			}
			else
			{
				string text = "";
				switch (NewState)
				{
				case State.Unhoused:
					text = "FolkUnOwned";
					break;
				case State.BadObject:
					text = "FolkBadObject";
					break;
				case State.Happy:
					text = "FolkHappy";
					break;
				case State.Unhappy:
					text = "FolkUnhappy";
					if ((bool)m_FolkOwner)
					{
						UpdateUnhappy(Force: true);
					}
					break;
				}
				if (NewState != State.Unhappy)
				{
					Sprite sprite = (Sprite)Resources.Load("Textures/Hud/Indicators/" + text, typeof(Sprite));
					m_Image.sprite = sprite;
					m_Image.enabled = true;
				}
				else
				{
					m_Image.enabled = false;
				}
			}
			m_StateTimer = 0f;
		}
		if ((bool)m_FolkOwner)
		{
			UpdateTransform(m_FolkOwner.transform.position);
		}
		if ((bool)m_HousingOwner)
		{
			UpdateTransform(m_HousingOwner.transform.position);
		}
	}

	private void UpdateUnhappy(bool Force = false)
	{
	}

	public void UpdateIndicator()
	{
		if ((bool)m_FolkOwner)
		{
			UpdateTransform(m_FolkOwner.transform.position);
		}
		if ((bool)m_HousingOwner)
		{
			UpdateTransform(m_HousingOwner.transform.position);
		}
		if (m_State == State.Unhappy)
		{
			UpdateUnhappy();
		}
		if ((bool)TimeManager.Instance)
		{
			m_StateTimer += TimeManager.Instance.m_NormalDelta;
		}
	}
}
