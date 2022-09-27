using UnityEngine;

public class WorkerInfoBase : MonoBehaviour
{
	public bool m_Error;

	protected BaseImage m_Panel;

	private BaseImage m_StatusImage;

	protected bool m_Selected;

	private float m_ClickTimer;

	protected bool m_JustSelected;

	public int m_StatusIndex;

	public bool m_IsGroup;

	public Worker m_Worker;

	public WorkerGroup m_Group;

	private RectTransform m_RectTransform;

	private float m_StartHeight;

	public virtual void Init()
	{
		m_Error = false;
		m_Selected = false;
		m_ClickTimer = 0f;
		CheckGadgets();
		m_RectTransform = GetComponent<RectTransform>();
		m_StartHeight = m_RectTransform.rect.height;
		m_StatusIndex = -1;
		base.gameObject.SetActive(value: false);
	}

	protected virtual void CheckGadgets()
	{
		if (!m_Panel)
		{
			m_Panel = base.transform.Find("Panel").GetComponent<BaseImage>();
			BaseText component = base.transform.Find("Name").GetComponent<BaseText>();
			ApplyEvents(component);
			m_StatusImage = base.transform.Find("Status").GetComponent<BaseImage>();
		}
	}

	public virtual void UpdateStatusImage()
	{
	}

	protected void ApplyEvents(BaseGadget NewGadget)
	{
		NewGadget.m_OnEnterEvent = OnPointerEnter;
		NewGadget.m_OnExitEvent = OnPointerExit;
		NewGadget.m_OnUpEvent = OnPointerUp;
		NewGadget.m_OnDownEvent = OnPointerDown;
		NewGadget.m_OnDragStartEvent = OnDragStart;
		NewGadget.m_OnDragEndEvent = OnDragEnd;
	}

	protected void SetStatus(int StatusIndex, string RolloverName)
	{
		if (m_StatusIndex != StatusIndex)
		{
			m_StatusIndex = StatusIndex;
			string[] array = new string[5] { "TabWorkerStatusGreen", "TabWorkerStatusIdle", "TabWorkerStatusPaused", "TabWorkerStatusAmber", "TabWorkerStatusRed" };
			m_StatusImage.SetSprite("Tabs/" + array[StatusIndex]);
		}
		m_StatusImage.SetRollover(RolloverName);
	}

	public virtual string GetName()
	{
		return "";
	}

	public void Select(bool Selected)
	{
		m_Selected = Selected;
		UpdateSelectedColour();
	}

	public virtual void UpdateSelectedColour()
	{
	}

	public virtual void OnClick(bool Release)
	{
	}

	public void OnPointerEnter(BaseGadget NewGadget)
	{
		SetHighlight(Highlight: true);
		TabWorkers.Instance.SetCurrentHighlighted(this);
		HudManager.Instance.ActivateBotRollover(Activate: true, m_Worker);
	}

	public void OnPointerExit(BaseGadget NewGadget)
	{
		SetHighlight(Highlight: false);
		TabWorkers.Instance.SetCurrentHighlighted(null);
		HudManager.Instance.ActivateBotRollover(Activate: false, null);
	}

	public void OnPointerDown(BaseGadget NewGadget)
	{
		OnClick(Release: false);
	}

	public void OnPointerUp(BaseGadget NewGadget)
	{
		OnClick(Release: true);
	}

	public virtual void OnDrag()
	{
		m_JustSelected = false;
	}

	public void OnDragStart(BaseGadget NewGadget)
	{
		OnDrag();
		TabWorkers.Instance.StartDrag();
	}

	public void OnDragEnd(BaseGadget NewGadget)
	{
		TabWorkers.Instance.EndDrag();
	}

	public virtual void SetPosition(Vector3 Position)
	{
		m_RectTransform.anchoredPosition = Position;
		Vector2 offsetMax = m_RectTransform.offsetMax;
		m_RectTransform.offsetMax = new Vector2(0.5f, offsetMax.y);
	}

	public virtual void UpdateVisible(float PanelHeight, float PanelY)
	{
		float num = 0f - PanelY - m_RectTransform.anchoredPosition.y;
		if (num > PanelHeight)
		{
			base.gameObject.SetActive(value: false);
		}
		else if (num + GetHeight() < 0f)
		{
			base.gameObject.SetActive(value: false);
		}
		else
		{
			base.gameObject.SetActive(value: true);
		}
	}

	public virtual float GetHeight()
	{
		return m_StartHeight;
	}

	public virtual void SetHighlight(bool Highlight)
	{
	}

	public void Update()
	{
		m_ClickTimer += TimeManager.Instance.m_NormalDelta;
	}
}
