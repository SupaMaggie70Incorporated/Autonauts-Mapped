using UnityEngine;

public class InventoryBar : BasePanel
{
	private static float m_SlotSpacing = 50f;

	private static float m_InventoryGap = 20f;

	private static float m_DefaultHeight = 60f;

	public BaseClass m_Object;

	public Farmer m_Farmer;

	private InventorySlot.Type m_CreateSlotType;

	private float m_TotalWidth;

	private float m_BestHeight;

	private int m_OldCarrySlots;

	private int m_OldInventorySlots;

	public ButtonList m_CarrySlots;

	public ButtonList m_InventorySlots;

	private ButtonList m_UpgradeSlots;

	private InventorySlot[] m_ClothingSlots;

	public ButtonList m_WardrobeSlots;

	private bool m_InRange;

	private BaseGadget m_OutOfRangeText;

	private BaseGadget m_InventoryButton;

	private bool m_Running;

	protected new void Awake()
	{
		base.Awake();
		m_OutOfRangeText = base.transform.Find("OutOfRange").GetComponent<BaseGadget>();
		m_OutOfRangeText.SetActive(Active: false);
		if ((bool)base.transform.Find("InventoryButton"))
		{
			m_InventoryButton = base.transform.Find("InventoryButton").GetComponent<BaseGadget>();
			m_InventoryButton.SetActive(Active: false);
			m_InventoryButton.SetAction(OnInventoryClicked, m_InventoryButton);
		}
	}

	public virtual void SetObject(BaseClass NewObject)
	{
		m_Object = NewObject;
		m_Farmer = null;
		if (m_Object.m_TypeIdentifier == ObjectType.FarmerPlayer || m_Object.m_TypeIdentifier == ObjectType.Worker)
		{
			m_Farmer = m_Object.GetComponent<Farmer>();
		}
		SetupSlots();
		UpdateObjects();
	}

	public void SetInventoryButtonActive(bool Active)
	{
		m_InventoryButton.SetActive(Active);
	}

	private bool GetIsObjectBusy()
	{
		if (m_Farmer != null && m_Farmer.m_State != 0 && m_Farmer.m_State != Farmer.State.Engaged && m_Farmer.m_State != Farmer.State.WorkerSelect)
		{
			return true;
		}
		return false;
	}

	public void OnInventoryClicked(BaseGadget NewGadget)
	{
		GameStateManager.Instance.PushState(GameStateManager.State.Inventory);
		GameStateManager.Instance.GetCurrentState().GetComponent<GameStateInventory>().SetInfo(m_Farmer, null);
	}

	public bool OnClick(InventorySlot NewSlot)
	{
		if (GetIsObjectBusy())
		{
			return false;
		}
		ObjectType objectType = NewSlot.GetObjectType();
		bool result = false;
		if (m_Farmer != null)
		{
			if (NewSlot.m_Type == InventorySlot.Type.Carry)
			{
				int num = m_Farmer.m_FarmerCarry.StowObjects();
				if (num == 1)
				{
					AudioManager.Instance.StartEvent("PlayerInventoryStow");
				}
				else
				{
					AudioManager.Instance.StartEvent("PlayerUpgradeAdded");
				}
				if (num != 0)
				{
					result = true;
				}
			}
			else
			{
				ObjectType topObjectType = m_Farmer.m_FarmerCarry.GetTopObjectType();
				if (topObjectType == ObjectTypeList.m_Total || (objectType == topObjectType && m_Farmer.m_FarmerCarry.CanAddCarry(NewSlot.m_Object)))
				{
					Holdable holdable = null;
					if ((bool)m_InventorySlots)
					{
						if (m_InventorySlots.m_Buttons.Contains(NewSlot))
						{
							int index = m_InventorySlots.m_Buttons.IndexOf(NewSlot);
							holdable = m_Farmer.m_FarmerInventory.ReleaseObject(index);
						}
						else
						{
							int index2 = m_UpgradeSlots.m_Buttons.IndexOf(NewSlot);
							holdable = m_Farmer.m_FarmerUpgrades.ReleaseObject(index2);
						}
					}
					else
					{
						holdable = NewSlot.m_Object;
						m_Farmer.m_FarmerClothes.Remove(holdable);
					}
					m_Farmer.m_FarmerCarry.AddCarry(holdable);
					AudioManager.Instance.StartEvent("PlayerInventoryCycleBackwards");
					result = true;
				}
				else if (objectType != topObjectType && (bool)m_InventorySlots && m_Farmer.m_FarmerCarry.GetCarryCount() == 1 && m_InventorySlots.m_Buttons.Contains(NewSlot))
				{
					int index3 = m_InventorySlots.m_Buttons.IndexOf(NewSlot);
					Holdable carryObject = m_Farmer.m_FarmerInventory.ReleaseObject(index3);
					Holdable carryObject2 = m_Farmer.m_FarmerCarry.RemoveTopObject();
					m_Farmer.m_FarmerCarry.AddCarry(carryObject);
					m_Farmer.m_FarmerInventory.AttemptAdd(carryObject2);
					AudioManager.Instance.StartEvent("PlayerInventoryCycleBackwards");
					result = true;
				}
			}
		}
		return result;
	}

	public void DragStart(InventorySlot NewSlot)
	{
		if (!GetIsObjectBusy())
		{
			GameStateManager.Instance.PushState(GameStateManager.State.DragInventorySlot);
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateDragInventorySlot>().SetDragType(NewSlot, this);
		}
	}

	public void DragEnd(BaseGadget NewGadget)
	{
		GameStateDragInventorySlot component = GameStateManager.Instance.GetCurrentState().GetComponent<GameStateDragInventorySlot>();
		if (component == null)
		{
			return;
		}
		InventorySlot dragSlot = component.m_DragSlot;
		InventorySlot dragTarget = component.m_DragTarget;
		dragSlot.EndDrag();
		GameStateManager.Instance.PopState();
		if (dragTarget != null && (bool)NewGadget && dragTarget.CanAcceptObject(dragSlot))
		{
			Holdable newObject = RemoveObject(dragSlot);
			InventorySlot target = dragTarget;
			if (dragTarget.m_Type == InventorySlot.Type.Carry)
			{
				target = dragTarget.m_Parent.m_CarrySlots.m_Buttons[0].GetComponent<InventorySlot>();
			}
			else if (dragTarget.m_Type == InventorySlot.Type.Inventory)
			{
				target = dragTarget.m_Parent.m_InventorySlots.m_Buttons[0].GetComponent<InventorySlot>();
			}
			else if (dragTarget.m_Type == InventorySlot.Type.Upgrade)
			{
				target = dragTarget.m_Parent.m_UpgradeSlots.m_Buttons[0].GetComponent<InventorySlot>();
			}
			dragTarget.m_Parent.AddObject(newObject, target);
		}
		if (dragTarget != null && (bool)NewGadget && (bool)GameStateManager.Instance.GetCurrentState().GetComponent<GameStateInventory>())
		{
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateInventory>().CheckTargetUpdated(dragSlot.m_Parent.m_Object);
			if (dragSlot.m_Parent.m_Object != dragTarget.m_Parent.m_Object)
			{
				GameStateManager.Instance.GetCurrentState().GetComponent<GameStateInventory>().CheckTargetUpdated(dragTarget.m_Parent.m_Object);
			}
		}
	}

	private Holdable RemoveObject(InventorySlot Drag)
	{
		Holdable holdable = null;
		if ((bool)m_Farmer)
		{
			if (m_CarrySlots != null)
			{
				if (m_CarrySlots.m_Buttons.Contains(Drag))
				{
					int index = m_CarrySlots.m_Buttons.IndexOf(Drag);
					holdable = m_Farmer.m_FarmerCarry.RemoveObject(index);
				}
				else if (m_InventorySlots.m_Buttons.Contains(Drag))
				{
					int index2 = m_InventorySlots.m_Buttons.IndexOf(Drag);
					holdable = m_Farmer.m_FarmerInventory.ReleaseObject(index2);
				}
				else
				{
					int index3 = m_UpgradeSlots.m_Buttons.IndexOf(Drag);
					holdable = m_Farmer.m_FarmerUpgrades.ReleaseObject(index3);
				}
			}
			else
			{
				holdable = Drag.m_Object;
				m_Farmer.m_FarmerClothes.Remove(holdable);
				switch (Clothing.GetTypeFromObjectType(holdable.m_TypeIdentifier))
				{
				case Clothing.Type.Top:
					ModManager.Instance.CheckCustomCallback(ModManager.CallbackTypes.ClothingTopRemoved, holdable.m_TypeIdentifier, holdable.m_TileCoord, holdable.m_UniqueID, m_Farmer.m_UniqueID);
					break;
				case Clothing.Type.Hat:
					ModManager.Instance.CheckCustomCallback(ModManager.CallbackTypes.ClothingHatRemoved, holdable.m_TypeIdentifier, holdable.m_TileCoord, holdable.m_UniqueID, m_Farmer.m_UniqueID);
					break;
				}
			}
		}
		else
		{
			int index4 = m_WardrobeSlots.m_Buttons.IndexOf(Drag);
			holdable = ((m_Object.m_TypeIdentifier != ObjectType.Wardrobe) ? m_Object.GetComponent<Aquarium>().ReleaseObject(index4) : m_Object.GetComponent<Wardrobe>().ReleaseObject(index4));
		}
		return holdable;
	}

	public void AddObject(Holdable NewObject, InventorySlot Target)
	{
		if ((bool)m_Farmer)
		{
			if (m_CarrySlots != null)
			{
				if (m_CarrySlots.m_Buttons.Contains(Target))
				{
					m_Farmer.m_FarmerCarry.AddCarry(NewObject);
				}
				else if (m_InventorySlots.m_Buttons.Contains(Target))
				{
					m_Farmer.m_FarmerInventory.AttemptAdd(NewObject);
				}
				else
				{
					m_Farmer.m_FarmerUpgrades.AttemptAdd(NewObject);
				}
				return;
			}
			m_Farmer.m_FarmerClothes.Add(NewObject);
			switch (Clothing.GetTypeFromObjectType(NewObject.m_TypeIdentifier))
			{
			case Clothing.Type.Top:
				ModManager.Instance.CheckCustomCallback(ModManager.CallbackTypes.ClothingTopAdded, NewObject.m_TypeIdentifier, NewObject.m_TileCoord, NewObject.m_UniqueID, m_Farmer.m_UniqueID);
				break;
			case Clothing.Type.Hat:
				ModManager.Instance.CheckCustomCallback(ModManager.CallbackTypes.ClothingHatAdded, NewObject.m_TypeIdentifier, NewObject.m_TileCoord, NewObject.m_UniqueID, m_Farmer.m_UniqueID);
				break;
			}
		}
		else if (m_Object.m_TypeIdentifier == ObjectType.Wardrobe)
		{
			m_Object.GetComponent<Wardrobe>().AttemptAdd(NewObject);
		}
		else if (Aquarium.GetIsTypeAquiarium(m_Object.m_TypeIdentifier))
		{
			m_Object.GetComponent<Aquarium>().AttemptAdd(NewObject);
		}
	}

	private void SetupSlots()
	{
		if ((bool)base.transform.Find("CarrySlotList"))
		{
			int count = 0;
			int count2 = 0;
			int count3 = 0;
			int count4 = 0;
			if ((bool)m_Farmer)
			{
				count = m_Farmer.m_FarmerCarry.m_TotalCapacity;
				count2 = m_Farmer.m_FarmerInventory.m_TotalCapacity;
				count3 = m_Farmer.m_FarmerUpgrades.m_Capacity;
			}
			else if (m_Object.m_TypeIdentifier == ObjectType.Wardrobe)
			{
				count4 = m_Object.GetComponent<Wardrobe>().GetCapacity();
			}
			else if (Aquarium.GetIsTypeAquiarium(m_Object.m_TypeIdentifier))
			{
				count4 = m_Object.GetComponent<Aquarium>().GetCapacity();
			}
			m_BestHeight = m_DefaultHeight;
			SetHeight(m_DefaultHeight);
			m_TotalWidth = m_DefaultHeight / 2f + 10f;
			if ((bool)m_Farmer)
			{
				m_CarrySlots = CreateSlotCategory("CarrySlotList", count, InventorySlot.Type.Carry);
				m_InventorySlots = CreateSlotCategory("InventorySlotList", count2, InventorySlot.Type.Inventory);
				m_UpgradeSlots = CreateSlotCategory("UpgradeSlotList", count3, InventorySlot.Type.Upgrade);
			}
			else if (m_Object.m_TypeIdentifier == ObjectType.Wardrobe)
			{
				m_WardrobeSlots = CreateSlotCategory("WardrobeSlotList", count4, InventorySlot.Type.Wardrobe);
			}
			else
			{
				m_WardrobeSlots = CreateSlotCategory("WardrobeSlotList", count4, InventorySlot.Type.Aquarium);
			}
			m_TotalWidth += 15f - m_SlotSpacing;
			SetWidth(m_TotalWidth);
			DragEnd(null);
		}
		SetupClothingSlots();
	}

	private void SetBestHeight(float NewHeight)
	{
		if (NewHeight > m_BestHeight)
		{
			m_BestHeight = NewHeight;
			SetHeight(NewHeight);
		}
	}

	private ButtonList CreateSlotCategory(string Name, int Count, InventorySlot.Type NewType)
	{
		m_CreateSlotType = NewType;
		ButtonList component = base.transform.Find(Name).GetComponent<ButtonList>();
		component.GetComponent<RectTransform>().anchoredPosition = new Vector2(m_TotalWidth, component.GetComponent<RectTransform>().anchoredPosition.y);
		component.m_CreateObjectCallback = OnCreateSlot;
		component.m_ObjectCount = Count;
		component.CreateButtons();
		if (Count > component.m_ButtonsPerRow)
		{
			int num = (Count - component.m_ButtonsPerRow) / component.m_ButtonsPerRow;
			if (Count % component.m_ButtonsPerRow != 0)
			{
				num++;
			}
			SetBestHeight(m_DefaultHeight + m_SlotSpacing * (float)num);
		}
		int num2 = Count;
		if (Count > component.m_ButtonsPerRow)
		{
			num2 = component.m_ButtonsPerRow;
		}
		float num3 = (float)num2 * m_SlotSpacing + m_InventoryGap;
		m_TotalWidth += num3;
		return component;
	}

	public void OnCreateSlot(GameObject NewGadget, int Index)
	{
		NewGadget.GetComponent<InventorySlot>().SetType(m_CreateSlotType, this);
	}

	private void SetupClothingSlots()
	{
		int num = 5;
		m_ClothingSlots = new InventorySlot[num];
		for (int i = 0; i < num; i++)
		{
			FarmerClothes.Type type = (FarmerClothes.Type)i;
			string n = type.ToString() + "Slot";
			if ((bool)base.transform.Find(n))
			{
				m_ClothingSlots[i] = base.transform.Find(n).GetComponent<InventorySlot>();
				m_ClothingSlots[i].SetType((InventorySlot.Type)(3 + i), this);
			}
		}
	}

	public void UpdateObjects()
	{
		if ((bool)m_Farmer)
		{
			if ((bool)m_CarrySlots)
			{
				UpdateHeldObjects();
				UpdateInventoryObjects();
				UpdateUpgradeObjects();
			}
			UpdateClothingObjects();
		}
		if ((bool)m_WardrobeSlots && (m_Object.m_TypeIdentifier == ObjectType.Wardrobe || Aquarium.GetIsTypeAquiarium(m_Object.m_TypeIdentifier)))
		{
			UpdateWardrobeSlots();
		}
	}

	private void UpdateHeldObjects()
	{
		int totalCapacity = m_Farmer.m_FarmerCarry.m_TotalCapacity;
		int count = m_Farmer.m_FarmerCarry.m_CarryObject.Count;
		int num = count + m_Farmer.m_FarmerCarry.m_SlotsRemaining;
		ObjectType @object = ObjectTypeList.m_Total;
		for (int i = 0; i < totalCapacity; i++)
		{
			InventorySlot component = m_CarrySlots.m_Buttons[i].GetComponent<InventorySlot>();
			Holdable holdable = null;
			if (i < count)
			{
				holdable = m_Farmer.m_FarmerCarry.m_CarryObject[i];
				if (i == 0)
				{
					@object = holdable.m_TypeIdentifier;
				}
			}
			if (i >= count && count > 0)
			{
				component.SetObject(@object);
			}
			else
			{
				component.SetObject(holdable);
			}
			if (i < num)
			{
				component.SetActive(Active: true);
			}
			else
			{
				component.SetActive(Active: false);
			}
		}
	}

	private void UpdateInventoryObjects()
	{
		int totalCapacity = m_Farmer.m_FarmerInventory.m_TotalCapacity;
		int num = totalCapacity - m_Farmer.m_FarmerInventory.m_SlotsHidden;
		for (int i = 0; i < totalCapacity; i++)
		{
			InventorySlot component = m_InventorySlots.m_Buttons[i].GetComponent<InventorySlot>();
			Holdable holdable = (holdable = m_Farmer.m_FarmerInventory.m_Objects[i]);
			component.SetObject(holdable);
			if (i < num)
			{
				component.gameObject.SetActive(value: true);
			}
			else
			{
				component.gameObject.SetActive(value: false);
			}
		}
	}

	private void UpdateUpgradeObjects()
	{
		int capacity = m_Farmer.m_FarmerUpgrades.m_Capacity;
		for (int i = 0; i < capacity; i++)
		{
			InventorySlot component = m_UpgradeSlots.m_Buttons[i].GetComponent<InventorySlot>();
			Holdable @object = m_Farmer.m_FarmerUpgrades.m_Objects[i];
			component.SetObject(@object);
		}
	}

	private void UpdateWardrobeSlots()
	{
		int num = 0;
		if (m_Object.m_TypeIdentifier == ObjectType.Wardrobe)
		{
			num = m_Object.GetComponent<Wardrobe>().GetCapacity();
		}
		else if (Aquarium.GetIsTypeAquiarium(m_Object.m_TypeIdentifier))
		{
			num = m_Object.GetComponent<Aquarium>().GetCapacity();
		}
		for (int i = 0; i < num; i++)
		{
			InventorySlot component = m_WardrobeSlots.m_Buttons[i].GetComponent<InventorySlot>();
			Holdable @object = null;
			if (m_Object.m_TypeIdentifier == ObjectType.Wardrobe)
			{
				@object = m_Object.GetComponent<Wardrobe>().GetObject(i);
			}
			else if (Aquarium.GetIsTypeAquiarium(m_Object.m_TypeIdentifier))
			{
				@object = m_Object.GetComponent<Aquarium>().GetObject(i);
			}
			component.SetObject(@object);
			component.gameObject.SetActive(value: true);
		}
	}

	private void UpdateClothingObjects()
	{
		if (m_Farmer == null)
		{
			return;
		}
		int num = 5;
		for (int i = 0; i < num; i++)
		{
			if ((bool)m_ClothingSlots[i])
			{
				Holdable @object = m_Farmer.m_FarmerClothes.Get((FarmerClothes.Type)i);
				m_ClothingSlots[i].SetObject(@object);
			}
		}
	}

	public void UpdateHealth()
	{
		if ((bool)m_CarrySlots)
		{
			foreach (InventorySlot button in m_CarrySlots.m_Buttons)
			{
				button.UpdateHealthBar();
			}
			foreach (InventorySlot button2 in m_InventorySlots.m_Buttons)
			{
				button2.UpdateHealthBar();
			}
			foreach (InventorySlot button3 in m_UpgradeSlots.m_Buttons)
			{
				button3.UpdateHealthBar();
			}
		}
		InventorySlot[] clothingSlots = m_ClothingSlots;
		foreach (InventorySlot inventorySlot in clothingSlots)
		{
			if ((bool)inventorySlot)
			{
				inventorySlot.UpdateHealthBar();
			}
		}
	}

	public void SetInRange(bool InRange)
	{
		m_InRange = InRange;
		UpdateInteractable();
	}

	public bool GetInRange()
	{
		if ((bool)m_Farmer && m_Farmer.m_TypeIdentifier == ObjectType.Worker)
		{
			return m_InRange;
		}
		return true;
	}

	private void UpdateInteractable()
	{
		if ((bool)m_CarrySlots)
		{
			m_CarrySlots.SetAllInteractable(m_InRange);
			m_InventorySlots.SetAllInteractable(m_InRange);
			m_UpgradeSlots.SetAllInteractable(m_InRange && !m_Running);
			return;
		}
		InventorySlot[] clothingSlots = m_ClothingSlots;
		foreach (InventorySlot inventorySlot in clothingSlots)
		{
			if ((bool)inventorySlot)
			{
				inventorySlot.SetInteractable(m_InRange);
			}
		}
	}

	private void UpdateRunning()
	{
		if (m_Farmer != null && m_Farmer.m_TypeIdentifier == ObjectType.Worker)
		{
			bool flag = false;
			if (m_Farmer.gameObject.layer != 14)
			{
				flag = true;
			}
			if (flag != m_Running)
			{
				m_Running = flag;
				UpdateInteractable();
			}
		}
	}

	public void CheckSlots()
	{
		if (!m_CarrySlots)
		{
			return;
		}
		int totalCapacity = m_Farmer.m_FarmerCarry.m_TotalCapacity;
		int totalCapacity2 = m_Farmer.m_FarmerInventory.m_TotalCapacity;
		if (totalCapacity != m_OldCarrySlots || totalCapacity2 != m_OldInventorySlots)
		{
			SetupSlots();
			m_OldCarrySlots = totalCapacity;
			m_OldInventorySlots = totalCapacity2;
		}
		bool isUsingHeldObject = m_Farmer.GetIsUsingHeldObject();
		foreach (InventorySlot button in m_CarrySlots.m_Buttons)
		{
			button.SetBeingUsed(isUsingHeldObject);
		}
	}

	private void Update()
	{
		if (!(m_Object == null))
		{
			CheckSlots();
			UpdateObjects();
			UpdateRunning();
		}
	}
}
