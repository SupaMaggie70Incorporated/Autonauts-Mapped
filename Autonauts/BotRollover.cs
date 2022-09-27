using System.Collections.Generic;
using UnityEngine;

public class BotRollover : GeneralRollover
{
	private Worker m_Target;

	private BaseImage m_Image;

	private InventorySlot m_UpgradeDefault;

	private List<InventorySlot> m_Upgrades;

	protected new void Awake()
	{
		base.Awake();
		m_Target = null;
		m_Image = m_Panel.transform.Find("ObjectImage").GetComponent<BaseImage>();
		m_UpgradeDefault = m_Panel.transform.Find("UpgradeDefault").GetComponent<InventorySlot>();
		m_UpgradeDefault.SetActive(Active: false);
		Hide();
		m_Upgrades = new List<InventorySlot>();
	}

	private void UpdateUpgrades()
	{
		foreach (InventorySlot upgrade in m_Upgrades)
		{
			Object.Destroy(upgrade.gameObject);
		}
		m_Upgrades.Clear();
		int capacity = m_Target.m_FarmerUpgrades.m_Capacity;
		float num = 45f;
		Vector2 anchoredPosition = m_UpgradeDefault.GetComponent<RectTransform>().anchoredPosition;
		anchoredPosition.y += num * 0.5f;
		Transform parent = m_UpgradeDefault.transform.parent;
		for (int i = 0; i < capacity; i++)
		{
			InventorySlot inventorySlot = Object.Instantiate(m_UpgradeDefault, parent);
			inventorySlot.SetActive(Active: true);
			inventorySlot.SetType(InventorySlot.Type.Upgrade, null);
			inventorySlot.SetObject(m_Target.m_FarmerUpgrades.m_Objects[i]);
			Vector2 anchoredPosition2 = anchoredPosition;
			if (i % 2 != 0)
			{
				anchoredPosition2.y -= num;
			}
			if (i / 2 != 0)
			{
				anchoredPosition2.x = 0f - anchoredPosition2.x;
			}
			inventorySlot.GetComponent<RectTransform>().anchoredPosition = anchoredPosition2;
			m_Upgrades.Add(inventorySlot);
		}
	}

	public void SetTarget(Worker Target)
	{
		if (Target != m_Target)
		{
			m_Target = Target;
			if ((bool)Target)
			{
				m_Title.SetText(Target.GetHumanReadableName());
				m_Image.SetActive(Active: true);
				m_Image.SetSprite(Target.GetIcon());
				UpdateUpgrades();
			}
		}
	}

	protected override bool GetTargettingSomething()
	{
		if ((bool)m_Target)
		{
			return true;
		}
		return false;
	}
}
