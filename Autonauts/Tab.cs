using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tab : BaseMenu
{
	private static float m_Spacing = 70f;

	[HideInInspector]
	public TabManager.TabType m_Type;

	public BaseButtonImage m_Tab;

	private GameObject m_TabShadow;

	private Image m_TabImage;

	private Image m_TabPanelImage;

	private Image m_PanelShadow;

	private NewThing m_NewIcon;

	protected bool m_Active;

	private bool m_Open;

	private static float m_Width = -206f;

	private float m_XOffset;

	private Wobbler m_Wobbler;

	private RectTransform m_Transform;

	protected bool m_Visible = true;

	protected new void Awake()
	{
		base.Awake();
	}

	protected new void Start()
	{
		base.Start();
		CheckGadgets();
		m_Active = false;
		m_Open = false;
	}

	private void CheckGadgets()
	{
		if (!m_Tab)
		{
			m_Tab = base.transform.Find("TabPanel").Find("Tab").GetComponent<BaseButtonImage>();
			m_TabShadow = base.transform.Find("TabPanel").Find("TabShadow").gameObject;
			m_PanelShadow = base.transform.Find("TabPanel/BasePanelOptions/Panel/Shadow").GetComponent<Image>();
			AddAction(m_Tab, OnTabClicked);
			m_TabImage = m_Tab.transform.Find("Image").GetComponent<Image>();
			m_NewIcon = m_Tab.transform.Find("NewThing").GetComponent<NewThing>();
			EnableTabNew(Enabled: false);
			m_Transform = GetComponent<RectTransform>();
			m_Wobbler = new Wobbler();
		}
	}

	protected BasePanelOptions GetPanel()
	{
		return base.transform.Find("TabPanel").Find("BasePanelOptions").GetComponent<BasePanelOptions>();
	}

	protected BaseScrollView GetScrollView()
	{
		return base.transform.Find("TabPanel").Find("BasePanelOptions").Find("BaseScrollView")
			.GetComponent<BaseScrollView>();
	}

	public void PostLoad()
	{
		TestTabVisibility(PostLoad: true);
	}

	private void SetTabType(TabManager.TabType NewType)
	{
		string text = "Tab" + NewType;
		m_Tab.SetSprite("Tabs/" + text);
		float y = -43f - m_Spacing * (float)NewType;
		Vector2 vector = new Vector2(5f, y);
		m_Tab.GetComponent<RectTransform>().anchoredPosition = vector;
		m_TabShadow.GetComponent<RectTransform>().anchoredPosition = vector + new Vector2(-5f, -5f);
		if (GameOptionsManager.Instance.m_Options.m_GameMode == GameOptions.GameMode.ModeCampaign)
		{
			m_Tab.SetActive(Active: false);
		}
		else if (NewType == TabManager.TabType.Quests)
		{
			m_Tab.SetActive(Active: false);
		}
		else
		{
			m_Tab.SetActive(Active: true);
		}
		m_Tab.SetRolloverFromID("Tab" + NewType);
	}

	private void EnableTabNew(bool Enabled)
	{
		m_NewIcon.gameObject.SetActive(Enabled);
	}

	private void UpdateTabColour()
	{
		float num = 1f;
		if (!m_Active && m_Open)
		{
			num = 0.8f;
		}
		Color color = new Color(1f, 1f, 1f);
		color *= num;
		color.a = 1f;
	}

	public void TestTabVisibility(bool PostLoad)
	{
		if (LoadJSON.m_Loading && !PostLoad)
		{
			return;
		}
		if (GameOptionsManager.Instance.m_Options.m_GameMode != 0)
		{
			if (m_Type == TabManager.TabType.Workers)
			{
				m_Tab.SetActive(Active: true);
			}
		}
		else if (!m_Tab.GetActive() && m_Type == TabManager.TabType.Workers)
		{
			Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("Worker");
			if (collection != null && collection.Count > 0)
			{
				ActivateTab(PostLoad);
			}
		}
	}

	public void ActivateTab(bool PostLoad)
	{
		m_Tab.SetActive(Active: true);
		if (!PostLoad)
		{
			AddNew();
		}
		else
		{
			EnableTabNew(Enabled: false);
		}
	}

	public void OnTabClicked(BaseGadget NewGadget)
	{
		EnableTabNew(Enabled: false);
		TabManager.Instance.TabClicked(m_Type);
	}

	public void SetType(TabManager.TabType NewType)
	{
		m_Type = NewType;
		SetTabType(NewType);
	}

	public void SetVisible(bool Visible)
	{
		base.gameObject.SetActive(Visible);
	}

	public void AddNew()
	{
		if (!m_Active)
		{
			EnableTabNew(Enabled: true);
		}
	}

	private void PopToTop()
	{
		Transform parent = base.transform.parent;
		base.transform.SetParent(HudManager.Instance.m_RootTransform);
		base.transform.SetParent(parent);
	}

	public virtual void SetActive(bool Active, bool Open)
	{
		CheckGadgets();
		if (Active)
		{
			SetContentsVisible(Visible: true);
		}
		if (!m_Open && Open)
		{
			if (Active)
			{
				m_XOffset = m_Width;
			}
			else
			{
				m_XOffset = m_Width + 10f;
			}
			m_Wobbler.Go(0.125f, 1f, 0f - m_XOffset);
		}
		else if (m_Open)
		{
			if (!Open)
			{
				m_XOffset = 0f;
				m_Wobbler.Go(0.125f, 1f, m_Width);
			}
			else if (!m_Active && Active)
			{
				m_XOffset = m_Width;
				m_Wobbler.Go(0.125f, 1f, 10f);
			}
			else if (m_Active && !Active)
			{
				m_XOffset = m_Width + 10f;
				m_Wobbler.Go(0.125f, 1f, -10f);
			}
		}
		if (!m_Active && Active)
		{
			PopToTop();
		}
		m_Active = Active;
		m_Open = Open;
		string text = "TabSelected";
		if (!m_Active)
		{
			text = "TabUnselected";
		}
		m_Tab.SetBackingSprite("Tabs/" + text);
	}

	protected virtual void UpdateVisibility()
	{
	}

	private void SetContentsVisible(bool Visible)
	{
		if (m_Visible != Visible)
		{
			m_Visible = Visible;
			if (m_Visible)
			{
				GetScrollView().GetContent().transform.localScale = new Vector3(1f, 1f, 1f);
			}
			else
			{
				GetScrollView().GetContent().transform.localScale = new Vector3(0f, 1f, 1f);
			}
			UpdateVisibility();
		}
	}

	private void UpdatePosition()
	{
		if (m_Wobbler.m_Wobbling)
		{
			m_Wobbler.Update();
			_ = m_Transform.anchoredPosition;
			Vector2 offsetMin = m_Transform.offsetMin;
			offsetMin.x = m_Wobbler.m_Height + m_XOffset;
			m_Transform.offsetMin = offsetMin;
			offsetMin = m_Transform.offsetMax;
			offsetMin.x = m_Wobbler.m_Height + m_XOffset;
			m_Transform.offsetMax = offsetMin;
		}
		else
		{
			SetContentsVisible(m_Active);
		}
	}

	protected new void Update()
	{
		base.Update();
		TestTabVisibility(PostLoad: false);
		UpdatePosition();
		UpdateTabColour();
	}
}
