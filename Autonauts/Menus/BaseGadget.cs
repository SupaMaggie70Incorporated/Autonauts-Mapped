using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class BaseGadget : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	[Serializable]
	public class ClickEvent : UnityEvent<BaseGadget>
	{
	}

	private static bool m_Log;

	[SerializeField]
	public string m_OnClickSound = "UIOptionSelected";

	[SerializeField]
	public string m_OnEnterSound = "UIOptionIndicated";

	[SerializeField]
	public string m_OnEnterRolloverID = "";

	private string m_OnEnterRollover = "";

	[SerializeField]
	public bool m_ClickReact = true;

	[SerializeField]
	public ClickEvent m_OnClickEvent;

	[HideInInspector]
	public Action<BaseGadget> m_OnEnterEvent;

	[HideInInspector]
	public Action<BaseGadget> m_OnExitEvent;

	[HideInInspector]
	public Action<BaseGadget> m_OnDownEvent;

	[HideInInspector]
	public Action<BaseGadget> m_OnUpEvent;

	[HideInInspector]
	public Action<BaseGadget> m_OnDragStartEvent;

	[HideInInspector]
	public Action<BaseGadget> m_OnDragEndEvent;

	protected bool m_Interactable = true;

	protected bool m_Selected;

	protected bool m_Indicated;

	protected bool m_Drag;

	private Action<BaseGadget> m_Action;

	private BaseGadget m_ActionGadget;

	[HideInInspector]
	public object m_ExtraData;

	protected void Awake()
	{
		if (m_OnEnterRolloverID != "")
		{
			SetRolloverFromID(m_OnEnterRolloverID);
		}
	}

	protected void Start()
	{
		SetIndicated(Indicated: false);
	}

	protected void OnDestroy()
	{
		if (m_Indicated)
		{
			OnPointerExit(null);
		}
	}

	private void OnDisable()
	{
		if (m_Indicated)
		{
			OnPointerExit(null);
		}
	}

	protected void DoAction()
	{
		if (m_Action != null && !m_Action.Target.Equals(null))
		{
			m_Action(m_ActionGadget);
		}
	}

	private bool IsMouseInsideScreen()
	{
		if (Input.mousePosition.x < 0f || Input.mousePosition.y < 0f || Input.mousePosition.x >= (float)Screen.width || Input.mousePosition.y >= (float)Screen.height)
		{
			return false;
		}
		return true;
	}

	public virtual void OnPointerEnter(PointerEventData eventData)
	{
		if (m_Log)
		{
			Debug.Log("Enter " + this);
		}
		if (m_Interactable && IsMouseInsideScreen())
		{
			if ((bool)CustomStandaloneInputModule.Instance)
			{
				CustomStandaloneInputModule.Instance.SetHoverObject(base.gameObject);
			}
			if ((bool)AudioManager.Instance && m_OnEnterSound != "")
			{
				AudioManager.Instance.StartEvent(m_OnEnterSound);
			}
			if (m_OnEnterRollover != null && m_OnEnterRollover != "")
			{
				HudManager.Instance.ActivateUIRollover(Activate: true, m_OnEnterRollover, default(Vector3));
			}
			if (m_OnEnterEvent != null)
			{
				m_OnEnterEvent(this);
			}
			SetIndicated(Indicated: true);
		}
	}

	public virtual void OnPointerExit(PointerEventData eventData)
	{
		if (m_Log)
		{
			Debug.Log("Exit " + this);
		}
		if (m_Interactable)
		{
			if ((bool)CustomStandaloneInputModule.Instance)
			{
				CustomStandaloneInputModule.Instance.SetHoverObject(null);
			}
			if (m_OnEnterRollover != null && m_OnEnterRollover != "")
			{
				HudManager.Instance.ActivateUIRollover(Activate: false, "", default(Vector3));
			}
			if (m_OnExitEvent != null)
			{
				m_OnExitEvent(this);
			}
			SetIndicated(Indicated: false);
		}
	}

	public virtual void OnPointerDown(PointerEventData eventData)
	{
		if (m_Log)
		{
			Debug.Log("Down " + this);
		}
		if (m_Interactable && eventData.button == PointerEventData.InputButton.Left)
		{
			m_Drag = false;
			if (m_OnDownEvent != null)
			{
				m_OnDownEvent(this);
			}
		}
	}

	public virtual void OnPointerUp(PointerEventData eventData)
	{
		if (m_Log)
		{
			Debug.Log("Up " + this);
		}
		if (m_Interactable && !m_Drag && eventData.button == PointerEventData.InputButton.Left && m_OnUpEvent != null)
		{
			m_OnUpEvent(this);
		}
	}

	protected bool CanReactToClick(PointerEventData eventData)
	{
		if (!m_Interactable)
		{
			return false;
		}
		if (eventData.button != 0)
		{
			return false;
		}
		if (!m_ClickReact)
		{
			return false;
		}
		if (m_Drag && m_OnDragStartEvent != null)
		{
			return false;
		}
		return true;
	}

	public virtual void OnPointerClick(PointerEventData eventData)
	{
		if (m_Log)
		{
			Debug.Log("Click " + this);
		}
		if (CanReactToClick(eventData))
		{
			if ((bool)AudioManager.Instance && m_OnClickSound != null && m_OnClickSound != "")
			{
				AudioManager.Instance.StartEvent(m_OnClickSound);
			}
			DoAction();
		}
	}

	public virtual void OnBeginDrag(PointerEventData eventData)
	{
		if (m_Log)
		{
			Debug.Log("Drag " + this);
		}
		if (m_Interactable && eventData.button == PointerEventData.InputButton.Left)
		{
			m_Drag = true;
			if (m_OnDragStartEvent != null)
			{
				m_OnDragStartEvent(this);
			}
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (m_Interactable && m_Drag)
		{
			_ = eventData.button;
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (m_Interactable && m_Drag && eventData.button == PointerEventData.InputButton.Left)
		{
			m_Drag = false;
			if (m_OnDragEndEvent != null)
			{
				m_OnDragEndEvent(this);
			}
		}
	}

	public void ForceEndDrag()
	{
		if (m_Interactable && m_Drag)
		{
			m_Drag = false;
		}
	}

	public void UpdateRollover()
	{
		if (m_Indicated)
		{
			HudManager.Instance.ActivateUIRollover(Activate: false, "", default(Vector3));
			HudManager.Instance.ActivateUIRollover(Activate: true, m_OnEnterRollover, default(Vector3));
		}
	}

	public void SetRollover(string NewText)
	{
		m_OnEnterRollover = NewText;
		UpdateRollover();
	}

	public void SetRolloverFromID(string ID)
	{
		m_OnEnterRollover = TextManager.Instance.Get(ID);
		UpdateRollover();
	}

	public virtual void SetAction(Action<BaseGadget> NewAction, BaseGadget NewGadget)
	{
		m_Action = NewAction;
		m_ActionGadget = NewGadget;
	}

	public Vector2 GetPosition()
	{
		return GetComponent<RectTransform>().anchoredPosition;
	}

	public void SetPosition(Vector2 Position)
	{
		GetComponent<RectTransform>().anchoredPosition = Position;
	}

	public void SetPosition(float x, float y)
	{
		GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
	}

	public float GetWidth()
	{
		return GetComponent<RectTransform>().sizeDelta.x;
	}

	public float GetHeight()
	{
		return GetComponent<RectTransform>().sizeDelta.y;
	}

	public void SetSize(Vector2 Position)
	{
		GetComponent<RectTransform>().sizeDelta = Position;
	}

	public void SetSize(float Width, float Height)
	{
		GetComponent<RectTransform>().sizeDelta = new Vector2(Width, Height);
	}

	public void SetWidth(float Width)
	{
		GetComponent<RectTransform>().sizeDelta = new Vector2(Width, GetHeight());
	}

	public void SetHeight(float Height)
	{
		GetComponent<RectTransform>().sizeDelta = new Vector2(GetWidth(), Height);
	}

	private void CheckInteractable()
	{
		if (m_Indicated)
		{
			OnPointerExit(null);
		}
	}

	public virtual void SetIndicated(bool Indicated)
	{
		m_Indicated = Indicated;
	}

	public virtual void SetInteractable(bool Interactable)
	{
		if (!Interactable)
		{
			CheckInteractable();
		}
		m_Interactable = Interactable;
	}

	public bool GetInteractable()
	{
		return m_Interactable;
	}

	public virtual void SetSelected(bool Selected)
	{
		m_Selected = Selected;
	}

	public bool GetIsSelected()
	{
		return m_Selected;
	}

	public virtual void SetActive(bool Active)
	{
		if (!Active)
		{
			CheckInteractable();
		}
		if (base.gameObject.activeSelf != Active)
		{
			base.gameObject.SetActive(Active);
		}
	}

	public bool GetActive()
	{
		return base.gameObject.activeSelf;
	}
}
