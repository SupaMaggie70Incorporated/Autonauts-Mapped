using UnityEngine;
using UnityEngine.EventSystems;

public class TabWorkerName : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
	public string m_RolloverTag;

	private WorkerInfoPanel m_Parent;

	private bool m_Drag;

	public void SetParent(WorkerInfoPanel Parent)
	{
		m_Parent = Parent;
	}

	public void OnPointerEnter(PointerEventData Data)
	{
		TabWorkers.Instance.SetCurrentHighlighted(m_Parent);
		if (m_Parent.m_Group != null)
		{
			if (m_Parent.m_Group.m_Description != "")
			{
				HudManager.Instance.ActivateUIRollover(Activate: true, m_Parent.m_Group.m_Description, default(Vector3));
			}
		}
		else if (m_RolloverTag != "")
		{
			string target = TextManager.Instance.Get(m_RolloverTag);
			HudManager.Instance.ActivateUIRollover(Activate: true, target, default(Vector3));
		}
	}

	public void OnPointerExit(PointerEventData Data)
	{
		TabWorkers.Instance.SetCurrentHighlighted(null);
		if (m_Parent.m_Group != null || m_RolloverTag != "")
		{
			HudManager.Instance.ActivateUIRollover(Activate: false, "", default(Vector3));
		}
	}

	public void OnPointerDown(PointerEventData Data)
	{
		if (Data.button == PointerEventData.InputButton.Left)
		{
			m_Drag = false;
			m_Parent.OnClick(Release: false);
		}
	}

	public void OnPointerUp(PointerEventData Data)
	{
		if (!m_Drag)
		{
			m_Parent.OnClick(Release: true);
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			m_Drag = true;
			m_Parent.OnDrag();
			TabWorkers.Instance.StartDrag();
		}
	}

	public void OnDrag(PointerEventData data)
	{
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		TabWorkers.Instance.EndDrag();
	}
}
