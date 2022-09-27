using UnityEngine.EventSystems;

public class MainMissionBanner : BaseImage
{
	private MainMissionPanel m_Parent;

	public void SetParent(MainMissionPanel Parent)
	{
		m_Parent = Parent;
		m_OnDragStartEvent = DragEvent;
	}

	public void DragEvent(BaseGadget NewGadget)
	{
	}

	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		m_Parent.OnMouseEnter(eventData);
		IndustryTree.Instance.MouseEnter();
	}

	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		m_Parent.OnMouseExit(eventData);
		IndustryTree.Instance.MouseExit();
	}

	public override void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			base.OnPointerClick(eventData);
			if (!m_Drag)
			{
				m_Parent.OnMouseClick(eventData);
			}
		}
	}

	public override void OnBeginDrag(PointerEventData eventData)
	{
		base.OnBeginDrag(eventData);
	}
}
