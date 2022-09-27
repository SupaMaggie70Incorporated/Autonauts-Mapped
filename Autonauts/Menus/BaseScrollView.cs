using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BaseScrollView : ScrollRect
{
	[SerializeField]
	public float m_Padding = 20f;

	private Action<BaseScrollView> m_ScrollChangedAction;

	private Scrollbar m_HorizontalBar;

	private Scrollbar m_VerticalBar;

	protected GameObject m_ContentObject;

	protected new void Awake()
	{
		base.Awake();
		GetGadgets();
	}

	private void GetGadgets()
	{
		if (m_HorizontalBar != null)
		{
			return;
		}
		m_HorizontalBar = base.transform.Find("Scrollbar Horizontal").GetComponent<Scrollbar>();
		m_VerticalBar = base.transform.Find("Scrollbar Vertical").GetComponent<Scrollbar>();
		m_VerticalBar.gameObject.SetActive(base.vertical);
		m_HorizontalBar.gameObject.SetActive(!base.vertical);
		m_VerticalBar.onValueChanged.AddListener(delegate
		{
			OnScrollChanged();
		});
		m_HorizontalBar.onValueChanged.AddListener(delegate
		{
			OnScrollChanged();
		});
		GetContent();
		if (base.vertical != base.horizontal)
		{
			if (base.vertical)
			{
				base.verticalScrollbar = m_VerticalBar;
				base.horizontalScrollbar = null;
			}
			else
			{
				base.horizontalScrollbar = m_HorizontalBar;
				base.verticalScrollbar = null;
			}
		}
	}

	public void OnScrollChanged()
	{
		if (m_ScrollChangedAction != null)
		{
			m_ScrollChangedAction(this);
		}
	}

	public void SetScrollChangedAction(Action<BaseScrollView> NewAction)
	{
		m_ScrollChangedAction = NewAction;
	}

	public GameObject GetContent()
	{
		if (m_ContentObject == null)
		{
			m_ContentObject = base.transform.Find("Viewport").Find("Content").gameObject;
		}
		return m_ContentObject;
	}

	public void SetPosition(Vector2 Position)
	{
		base.transform.localPosition = Position;
	}

	public void SetPosition(float x, float y)
	{
		base.transform.localPosition = new Vector3(x, y, 0f);
	}

	public void SetSize(float Width, float Height)
	{
		GetComponent<RectTransform>().sizeDelta = new Vector2(Width, Height);
	}

	public Vector2 GetSize()
	{
		return GetComponent<RectTransform>().sizeDelta;
	}

	public void SetScrollSize(float Size)
	{
		if (base.vertical)
		{
			GetContent().GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Size + m_Padding);
		}
		else
		{
			GetContent().GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Size + m_Padding);
		}
	}

	public float GetScrollSize()
	{
		if (base.vertical)
		{
			return GetContent().GetComponent<RectTransform>().sizeDelta.y;
		}
		return GetContent().GetComponent<RectTransform>().sizeDelta.x;
	}

	public void SetScrollValue(float Value)
	{
		GetGadgets();
		if (base.vertical)
		{
			m_VerticalBar.value = Value;
		}
		else
		{
			m_HorizontalBar.value = Value;
		}
	}

	public float GetScrollValue()
	{
		if (base.vertical)
		{
			return m_VerticalBar.value;
		}
		return m_HorizontalBar.value;
	}

	public float GetScrollPosition()
	{
		if (base.vertical)
		{
			return (1f - m_VerticalBar.value) * (GetScrollSize() - GetHeight());
		}
		return (1f - m_HorizontalBar.value) * (GetScrollSize() - GetWidth());
	}

	public float GetWidth()
	{
		return GetComponent<RectTransform>().rect.width;
	}

	public float GetHeight()
	{
		return GetComponent<RectTransform>().rect.height;
	}

	public void OnScroll(BaseEventData data)
	{
		base.OnScroll((PointerEventData)data);
	}

	public override void OnBeginDrag(PointerEventData eventData)
	{
	}

	public override void OnEndDrag(PointerEventData eventData)
	{
	}

	public override void OnDrag(PointerEventData eventData)
	{
	}
}
