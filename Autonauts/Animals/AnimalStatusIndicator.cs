using UnityEngine;
using UnityEngine.UI;

public class AnimalStatusIndicator : Indicator
{
	[HideInInspector]
	public enum State
	{
		Off,
		Sleep,
		Full,
		NeedFood,
		NeedPen,
		Enclosed,
		Total
	}

	public static bool m_NeedPenVisible;

	private bool m_Sleep;

	private bool m_Full;

	private bool m_NeedFood;

	private bool m_NeedPen;

	[HideInInspector]
	public State m_State;

	private float m_StateTimer;

	private Image m_Image;

	private TileCoordObject m_Parent;

	public void Restart()
	{
	}

	protected new void Awake()
	{
		base.Awake();
		m_Offset = 100f;
	}

	public void SetAllOff()
	{
		m_Sleep = false;
		m_Full = false;
		m_NeedFood = false;
		m_NeedPen = false;
		UpdateIcon();
	}

	public void SetSleeping(bool Sleeping)
	{
		m_Sleep = Sleeping;
		UpdateIcon();
	}

	public void SetFull(bool Full)
	{
		m_Full = Full;
		m_NeedFood = false;
		UpdateIcon();
	}

	public void SetEnclosed(bool Enclosed)
	{
		if (Enclosed)
		{
			if (m_NeedPen)
			{
				m_NeedPen = false;
				SetState(State.Enclosed);
			}
		}
		else
		{
			m_NeedPen = true;
			UpdateIcon();
		}
	}

	public void SetNeedFood(bool NeedsFood)
	{
		m_NeedFood = NeedsFood;
		m_Full = false;
		UpdateIcon();
	}

	private void UpdateIcon()
	{
		State state = State.Off;
		if (m_Sleep)
		{
			state = State.Sleep;
		}
		else if (m_Full)
		{
			state = State.Full;
		}
		else if (m_NeedFood)
		{
			state = State.NeedFood;
		}
		else if (m_NeedPen && m_NeedPenVisible)
		{
			state = State.NeedPen;
		}
		if (m_State != state)
		{
			SetState(state);
		}
	}

	public void SetParent(TileCoordObject NewParent)
	{
		m_Parent = NewParent;
		SetState(State.Off);
	}

	private void SetState(State NewState)
	{
		if (m_Image == null)
		{
			m_Image = GetComponent<Image>();
		}
		State state = m_State;
		m_State = NewState;
		if (m_State == State.Off || SaveLoadManager.m_Video)
		{
			base.gameObject.SetActive(value: false);
		}
		else
		{
			base.gameObject.SetActive(value: true);
		}
		string text = "";
		switch (m_State)
		{
		case State.Sleep:
			text = "AnimalSleep";
			break;
		case State.Full:
			if (AnimalCow.GetIsTypeCow(m_Parent.m_TypeIdentifier))
			{
				text = "AnimalCowFull";
			}
			else if (AnimalSheep.GetIsTypeSheep(m_Parent.m_TypeIdentifier))
			{
				text = "AnimalSheepFull";
			}
			else if (m_Parent.m_TypeIdentifier == ObjectType.AnimalChicken)
			{
				text = "AnimalChickenFull";
			}
			break;
		case State.NeedFood:
			if (AnimalCow.GetIsTypeCow(m_Parent.m_TypeIdentifier) || AnimalSheep.GetIsTypeSheep(m_Parent.m_TypeIdentifier))
			{
				text = "AnimalCowNeedsFood";
			}
			else if (m_Parent.m_TypeIdentifier == ObjectType.AnimalChicken)
			{
				text = "AnimalChickenNeedsFood";
			}
			else if (m_Parent.m_TypeIdentifier == ObjectType.AnimalBee)
			{
				text = "AnimalBeeNeedsFlowers";
			}
			break;
		case State.NeedPen:
			text = "AnimalNeedsEnclosure";
			break;
		case State.Enclosed:
			text = "AnimalEnclosed";
			break;
		}
		if (text != "")
		{
			Sprite sprite = (Sprite)Resources.Load("Textures/Hud/Indicators/" + text, typeof(Sprite));
			m_Image.sprite = sprite;
		}
		if (state != m_State)
		{
			m_StateTimer = 0f;
		}
		m_Image.enabled = true;
		UpdatePosition();
	}

	private void UpdatePosition()
	{
		if (m_Parent == null)
		{
			return;
		}
		if (AnimalGrazer.GetIsTypeAnimalGrazer(m_Parent.m_TypeIdentifier))
		{
			m_Offset = 50f;
			AnimalGrazer component = m_Parent.GetComponent<AnimalGrazer>();
			if ((bool)component.m_Head)
			{
				UpdateTransform(component.m_Head.transform.position);
			}
		}
		if (AnimalPetDog.GetIsTypeAnimalPetDog(m_Parent.m_TypeIdentifier))
		{
			m_Offset = 50f;
			AnimalPetDog component2 = m_Parent.GetComponent<AnimalPetDog>();
			if ((bool)component2.m_Head)
			{
				UpdateTransform(component2.m_Head.transform.position);
			}
		}
		else if (m_Parent.m_TypeIdentifier == ObjectType.AnimalBee)
		{
			m_Offset = 50f;
			UpdateTransform(m_Parent.transform.position);
		}
		else if (m_Parent.m_TypeIdentifier == ObjectType.ChickenCoop)
		{
			m_Offset = 0f;
			UpdateTransform(m_Parent.transform.position + new Vector3(0f, 4f, 0f));
		}
	}

	private void Update()
	{
		UpdatePosition();
		switch (m_State)
		{
		case State.Full:
			if ((int)(m_StateTimer * 60f) % 32 > 16)
			{
				m_Image.enabled = false;
			}
			else if (!m_Image.enabled)
			{
				m_Image.enabled = true;
			}
			break;
		case State.Enclosed:
			if (m_StateTimer > 3f)
			{
				SetState(State.Off);
			}
			break;
		}
		if ((bool)TimeManager.Instance)
		{
			m_StateTimer += TimeManager.Instance.m_NormalDelta;
		}
	}
}
