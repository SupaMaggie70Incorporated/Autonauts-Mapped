using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomStandaloneInputModule : StandaloneInputModule
{
	public static CustomStandaloneInputModule Instance;

	private bool m_IgnoreUI;

	private EventSystem m_OldSystem;

	private GameObject m_HoverObject;

	private GameObject m_HoverObject2;

	private bool m_UIInUse;

	protected override void Awake()
	{
		Instance = this;
		m_UIInUse = false;
	}

	public GameObject GameObjectUnderPointer()
	{
		if (HudManager.Instance == null)
		{
			return null;
		}
		PointerEventData pointerEventData = new PointerEventData(m_OldSystem);
		pointerEventData.position = Input.mousePosition;
		List<RaycastResult> list = new List<RaycastResult>();
		HudManager.Instance.m_CanvasRootTransform.GetComponent<GraphicRaycaster>().Raycast(pointerEventData, list);
		if (list.Count != 0)
		{
			return list[0].gameObject;
		}
		return null;
	}

	public bool IsDragging()
	{
		return GetLastPointerEventData(-1)?.dragging ?? false;
	}

	public void SetIgnoreUI(bool Ignore)
	{
		if (m_IgnoreUI != Ignore)
		{
			m_IgnoreUI = Ignore;
			if (m_OldSystem == null)
			{
				m_OldSystem = EventSystem.current;
			}
			m_OldSystem.enabled = !Ignore;
		}
	}

	public bool IsUIInFocus()
	{
		if (m_HoverObject != null || m_HoverObject2 != null)
		{
			return true;
		}
		if (IsDragging())
		{
			return true;
		}
		if ((bool)EventSystem.current && (bool)EventSystem.current.currentSelectedGameObject)
		{
			if (EventSystem.current.currentSelectedGameObject.GetComponent<InputField>() != null && EventSystem.current.currentSelectedGameObject.GetComponent<InputField>().isFocused)
			{
				return true;
			}
			if (EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>() != null && EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>().isFocused)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsHover()
	{
		if (m_IgnoreUI)
		{
			return false;
		}
		GameObject gameObject = m_HoverObject2;
		if (gameObject == null)
		{
			gameObject = m_HoverObject;
		}
		if ((bool)gameObject)
		{
			Transform parent = gameObject.transform;
			while (parent != null)
			{
				if ((bool)parent.GetComponent<ScrollRect>())
				{
					return true;
				}
				parent = parent.parent;
			}
		}
		return false;
	}

	public bool IsUIInUse()
	{
		if (m_IgnoreUI)
		{
			return false;
		}
		if (IsDragging())
		{
			m_UIInUse = true;
			return true;
		}
		if ((bool)EventSystem.current && (bool)EventSystem.current.currentSelectedGameObject)
		{
			if (EventSystem.current.currentSelectedGameObject.GetComponent<InputField>() != null && EventSystem.current.currentSelectedGameObject.GetComponent<InputField>().isFocused)
			{
				m_UIInUse = true;
				return true;
			}
			if (EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>() != null && EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>().isFocused)
			{
				m_UIInUse = true;
				return true;
			}
		}
		PointerEventData lastPointerEventData = GetLastPointerEventData(-1);
		if (lastPointerEventData != null && (bool)lastPointerEventData.lastPress && lastPointerEventData.lastPress.GetComponent<Dropdown>() != null)
		{
			m_UIInUse = true;
			return true;
		}
		if (m_UIInUse)
		{
			m_UIInUse = false;
			return true;
		}
		return false;
	}

	private void Update()
	{
		m_HoverObject = GameObjectUnderPointer();
	}

	public void SetHoverObject(GameObject NewObject)
	{
		m_HoverObject2 = NewObject;
	}
}
