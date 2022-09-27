using UnityEngine;

public class InventorySlot : BaseButtonImage
{
	public enum Type
	{
		Carry,
		Inventory,
		Upgrade,
		Hat,
		Gloves,
		Shoes,
		Top,
		Trousers,
		Wardrobe,
		Aquarium,
		Total
	}

	public Type m_Type;

	public Holdable m_Object;

	private ObjectType m_ObjectType;

	private BaseProgressBar m_HealthBar;

	private float m_LockedFlashTimer;

	public InventoryBar m_Parent;

	protected new void Awake()
	{
		base.Awake();
		GetGadgets();
		m_OnDragStartEvent = OnDragStart;
		m_OnDragEndEvent = OnDragEnd;
		SetAction(OnClick, this);
	}

	private void GetGadgets()
	{
		if (m_HealthBar == null)
		{
			m_HealthBar = base.transform.Find("HealthBar").GetComponent<BaseProgressBar>();
		}
	}

	public void SetType(Type NewType, InventoryBar Parent)
	{
		m_Type = NewType;
		m_Parent = Parent;
		GetGadgets();
		UpdateImage();
		UpdateHealthBar();
		UpdateColour();
	}

	public void OnClick(BaseGadget NewGadget)
	{
		if (m_Object != null && m_Parent.OnClick(this))
		{
			BaseSetIndicated(Indicated: false);
			HudManager.Instance.ActivateHoldableRollover(Activate: false);
		}
	}

	public void OnDragStart(BaseGadget NewGadget)
	{
		BaseSetIndicated(Indicated: false);
		HudManager.Instance.ActivateHoldableRollover(Activate: false);
		if (m_Object != null)
		{
			UpdateColour();
			m_Parent.DragStart(this);
		}
	}

	public void OnDragEnd(BaseGadget NewGadget)
	{
		if (m_Object != null)
		{
			m_Parent.DragEnd(this);
		}
	}

	public void EndDrag()
	{
		UpdateColour();
		ForceEndDrag();
	}

	private void UpdateColour()
	{
		Color imageColour = new Color(1f, 1f, 1f, 1f);
		Color backingColour = new Color(1f, 1f, 1f, 1f);
		if (!m_Interactable)
		{
			imageColour.r *= 0.5f;
			imageColour.g *= 0.5f;
			imageColour.b *= 0.5f;
			backingColour.r *= 0.5f;
			backingColour.g *= 0.5f;
			backingColour.b *= 0.5f;
		}
		if (GetObjectType() != ObjectTypeList.m_Total && m_Object == null)
		{
			imageColour.a *= 0.5f;
		}
		if (m_Drag)
		{
			backingColour.r *= 0.5f;
			backingColour.g *= 0.5f;
			backingColour.b *= 0.5f;
		}
		SetImageColour(imageColour);
		SetBackingColour(backingColour);
	}

	public override void SetInteractable(bool Interactable)
	{
		base.SetInteractable(Interactable);
		UpdateColour();
	}

	public override void SetIndicated(bool Indicated)
	{
		base.SetIndicated(Indicated);
		GameStateDragInventorySlot gameStateDragInventorySlot = null;
		if (GameStateManager.Instance.GetCurrentState() != null)
		{
			gameStateDragInventorySlot = GameStateManager.Instance.GetCurrentState().GetComponent<GameStateDragInventorySlot>();
		}
		if (gameStateDragInventorySlot == null)
		{
			if ((bool)m_Object)
			{
				BaseSetIndicated(Indicated);
				HudManager.Instance.ActivateHoldableRollover(Indicated, m_Object);
			}
			else
			{
				BaseSetIndicated(Indicated: false);
			}
			return;
		}
		gameStateDragInventorySlot.SetDragTarget(null);
		if (Indicated)
		{
			bool flag = CanAcceptObject(gameStateDragInventorySlot.m_DragSlot);
			BaseSetIndicated(flag);
			if (flag)
			{
				gameStateDragInventorySlot.SetDragTarget(this);
			}
		}
		else
		{
			BaseSetIndicated(Indicated: false);
		}
	}

	public bool CanAcceptObject(InventorySlot NewSlot)
	{
		if (NewSlot.m_Type == m_Type && NewSlot.m_Parent.m_Object == m_Parent.m_Object)
		{
			return false;
		}
		ObjectType objectType = NewSlot.GetObjectType();
		if (m_Type == Type.Carry)
		{
			return m_Parent.m_Farmer.m_FarmerCarry.CanAddCarry(NewSlot.m_Object);
		}
		if (m_Type == Type.Inventory)
		{
			if (NewSlot.m_Type == Type.Upgrade && UpgradeInventory.GetIsTypeUpgradeInventory(NewSlot.m_Object.m_TypeIdentifier) && NewSlot.m_Parent.m_Object == m_Parent.m_Object)
			{
				int capacity = NewSlot.m_Object.GetComponent<UpgradeInventory>().m_Capacity;
				if (m_Parent.m_Farmer.m_FarmerInventory.GetFreeSlots() - capacity < 1)
				{
					return false;
				}
			}
			return m_Parent.m_Farmer.m_FarmerInventory.CanAdd(NewSlot.m_Object);
		}
		if (m_Type == Type.Upgrade)
		{
			return m_Parent.m_Farmer.m_FarmerUpgrades.CanAdd(NewSlot.m_Object);
		}
		if (m_Type == Type.Hat)
		{
			return Hat.GetIsTypeHat(objectType);
		}
		if (m_Type == Type.Top)
		{
			return Top.GetIsTypeTop(objectType);
		}
		if (m_Type == Type.Wardrobe)
		{
			if (Hat.GetIsTypeHat(objectType) || Top.GetIsTypeTop(objectType))
			{
				return m_Parent.m_Object.GetComponent<Wardrobe>().CanAdd(NewSlot.m_Object);
			}
		}
		else if (m_Type == Type.Aquarium)
		{
			return m_Parent.m_Object.GetComponent<Aquarium>().CanAdd(NewSlot.m_Object);
		}
		return false;
	}

	public ObjectType GetObjectType()
	{
		ObjectType objectType = m_ObjectType;
		if (objectType == ObjectTypeList.m_Total && m_Object != null)
		{
			objectType = m_Object.m_TypeIdentifier;
		}
		return objectType;
	}

	private void UpdateImage()
	{
		ObjectType objectType = GetObjectType();
		if (objectType == ObjectTypeList.m_Total)
		{
			string sprite = "Inventory/InventorySlot" + m_Type;
			SetSprite(sprite);
			UpdateColour();
			SetRolloverFromID("HotbarRollover" + m_Type);
		}
		else
		{
			SetSprite(IconManager.Instance.GetIcon(objectType));
			UpdateColour();
			SetRollover("");
		}
	}

	public void UpdateHealthBar()
	{
		m_HealthBar.SetActive(Active: false);
		if ((bool)m_Object && m_Object.m_MaxUsageCount != 0)
		{
			m_HealthBar.SetActive(Active: true);
			float value = 1f - m_Object.GetUsed();
			m_HealthBar.SetValue(value);
		}
	}

	public void SetObject(Holdable NewObject)
	{
		if (m_Object != NewObject || m_ObjectType != ObjectTypeList.m_Total)
		{
			m_Object = NewObject;
			m_ObjectType = ObjectTypeList.m_Total;
			UpdateImage();
			UpdateHealthBar();
			UpdateColour();
		}
	}

	public void SetObject(ObjectType NewType)
	{
		if (m_ObjectType != NewType || (bool)m_Object)
		{
			m_ObjectType = NewType;
			m_Object = null;
			UpdateImage();
			UpdateHealthBar();
			UpdateColour();
		}
	}

	public void SetBeingUsed(bool Locked)
	{
		if (m_Locked != Locked && (!Locked || (bool)m_Object))
		{
			m_Locked = Locked;
			m_LockedFlashTimer = 0f;
			SetBackingColour(new Color(1f, 1f, 1f));
		}
	}

	private void UpdateLocked(float Delta)
	{
		if (m_Locked)
		{
			m_LockedFlashTimer += Delta;
			if ((int)(m_LockedFlashTimer * 60f) % 12 < 6)
			{
				SetBackingColour(new Color(0.65f, 1f, 0.65f));
			}
			else
			{
				SetBackingColour(new Color(1f, 1f, 1f));
			}
		}
	}

	private void Update()
	{
		UpdateLocked(TimeManager.Instance.m_NormalDelta);
	}
}
