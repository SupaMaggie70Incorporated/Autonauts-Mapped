using UnityEngine;

public class GameStateDragInventorySlot : GameStateBase
{
	private InventoryBar m_Parent;

	private BaseButtonImage m_DragImage;

	public InventorySlot m_DragSlot;

	public InventorySlot m_DragTarget;

	public override void ShutDown()
	{
		base.ShutDown();
		Object.Destroy(m_DragImage.gameObject);
	}

	public void SetDragType(InventorySlot DragSlot, InventoryBar BarParent)
	{
		m_DragSlot = DragSlot;
		m_Parent = BarParent;
		Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Inventory/InventoryDrag", typeof(GameObject));
		m_DragImage = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, menusRootTransform).GetComponentInChildren<BaseButtonImage>();
		Sprite icon = IconManager.Instance.GetIcon(m_DragSlot.GetObjectType());
		m_DragImage.SetSprite(icon);
	}

	public void SetDragTarget(InventorySlot NewSlot)
	{
		m_DragTarget = NewSlot;
	}

	public override void UpdateState()
	{
		m_DragImage.transform.localPosition = HudManager.Instance.ScreenToCanvas(Input.mousePosition) + new Vector3(0f, 0f, 0f);
		if (!m_Parent.GetInRange())
		{
			m_Parent.DragEnd(null);
		}
		if (MyInputManager.m_Rewired.GetButtonDown("Quit"))
		{
			m_Parent.DragEnd(null);
			AudioManager.Instance.StartEvent("UIOptionCancelled");
		}
	}

	public void CheckObjectDropped(Holdable NewObject)
	{
		if ((bool)m_DragSlot && NewObject == m_DragSlot.m_Object)
		{
			m_Parent.DragEnd(null);
		}
	}
}
