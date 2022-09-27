using UnityEngine;

public class ModeButton : StandardButtonImage
{
	public enum Type
	{
		Industry,
		BuildingPalette,
		Evolution,
		Badges,
		Stats,
		Creative,
		Academy,
		Research,
		Autopedia,
		Total
	}

	private NewThing m_NewIcon;

	[SerializeField]
	public Type m_Type;

	[HideInInspector]
	public bool m_Show;

	private bool m_Active;

	protected new void Awake()
	{
		base.Start();
		SetAction(OnClick, this);
		SetType(m_Type);
	}

	protected new void Start()
	{
	}

	public static ModeButton Get(Type NewType)
	{
		return HudManager.Instance.m_ModeButtons[(int)NewType];
	}

	public void SetType(Type NewType)
	{
		m_Type = NewType;
		HudManager.Instance.m_ModeButtons[(int)NewType] = this;
		SetRolloverFromID(NewType.ToString() + "Button");
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/NewThing", typeof(GameObject));
		m_NewIcon = Object.Instantiate(original, default(Vector3), Quaternion.identity, base.transform).GetComponent<NewThing>();
		m_NewIcon.GetComponent<RectTransform>().anchoredPosition = new Vector3(-20f, 20f, 0f);
		m_NewIcon.gameObject.SetActive(value: false);
		m_Active = true;
		m_Show = true;
		switch (NewType)
		{
		case Type.BuildingPalette:
			m_Show = false;
			break;
		case Type.Industry:
			if (GameOptionsManager.Instance.m_Options.m_GameMode != 0)
			{
				m_Show = false;
			}
			break;
		case Type.Evolution:
			m_Show = false;
			break;
		case Type.Badges:
			if (GameOptionsManager.Instance.m_Options.m_GameMode != 0)
			{
				m_Show = false;
			}
			break;
		case Type.Creative:
			if (GameOptionsManager.Instance.m_Options.m_GameMode != GameOptions.GameMode.ModeCreative)
			{
				m_Show = false;
			}
			break;
		case Type.Academy:
			m_Show = false;
			break;
		case Type.Research:
			m_Show = false;
			break;
		case Type.Autopedia:
			m_Show = false;
			break;
		}
		UpdateVisible();
	}

	public void Show()
	{
		m_Show = true;
		UpdateVisible();
	}

	private void UpdateVisible()
	{
		if (!m_Active || !m_Show)
		{
			base.gameObject.SetActive(value: false);
		}
		else
		{
			base.gameObject.SetActive(value: true);
		}
	}

	public void SetNew(bool New)
	{
		if (!TutorialPanelController.Instance.GetActive())
		{
			m_NewIcon.gameObject.SetActive(New);
		}
	}

	public void UpdateFolkLevel()
	{
		if (m_Type == Type.Evolution)
		{
			int highestActiveFolk = FolkManager.Instance.m_HighestActiveFolk;
			base.transform.Find("Count").GetComponent<BaseText>().SetText((highestActiveFolk + 1).ToString());
		}
	}

	public void OnClick(BaseGadget NewGadget)
	{
		m_NewIcon.gameObject.SetActive(value: false);
		switch (m_Type)
		{
		case Type.BuildingPalette:
			GameStateManager.Instance.SetState(GameStateManager.State.Edit);
			break;
		case Type.Badges:
			AudioManager.Instance.StartEvent("UIPause");
			GameStateManager.Instance.PushState(GameStateManager.State.Badges);
			break;
		case Type.Stats:
			AudioManager.Instance.StartEvent("UIPause");
			GameStateManager.Instance.PushState(GameStateManager.State.Stats);
			break;
		case Type.Creative:
			GameStateManager.Instance.SetState(GameStateManager.State.CreativeTools);
			break;
		case Type.Industry:
			if (GameStateManager.Instance.GetCurrentState().m_BaseState != GameStateManager.State.Industry)
			{
				GameStateManager.Instance.SetState(GameStateManager.State.Industry);
			}
			else
			{
				GameStateManager.Instance.PopState();
			}
			break;
		case Type.Evolution:
			GameStateManager.Instance.SetState(GameStateManager.State.Evolution);
			break;
		case Type.Academy:
			GameStateManager.Instance.SetState(GameStateManager.State.Academy);
			break;
		case Type.Research:
			GameStateManager.Instance.SetState(GameStateManager.State.Research);
			break;
		case Type.Autopedia:
			GameStateManager.Instance.SetState(GameStateManager.State.Autopedia);
			break;
		}
	}
}
