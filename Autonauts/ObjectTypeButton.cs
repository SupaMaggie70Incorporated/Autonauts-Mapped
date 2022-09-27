using UnityEngine;

public class ObjectTypeButton : ObjectBaseButton
{
	[HideInInspector]
	public ObjectType m_ObjectType;

	private Tile.TileType m_TileType;

	private bool m_Tile;

	private GameObject m_CountPanel;

	private BaseText m_Count;

	public static bool GetIsObjectCategoryUnlocked(ObjectCategory NewObjectCategory)
	{
		foreach (ObjectSubCategory subCategoriesInCategory in ObjectTypeList.Instance.GetSubCategoriesInCategories(NewObjectCategory))
		{
			if (GetIsObjectSubCategoryUnlocked(subCategoriesInCategory))
			{
				return true;
			}
		}
		return false;
	}

	public static bool GetIsObjectSubCategoryUnlocked(ObjectSubCategory NewObjectSubCategory)
	{
		foreach (ObjectType objectsInSubCategory in ObjectTypeList.Instance.GetObjectsInSubCategories(NewObjectSubCategory))
		{
			if (GetIsObjectUnlocked(objectsInSubCategory))
			{
				return true;
			}
		}
		return false;
	}

	public static bool GetIsObjectUnlocked(ObjectType NewObjectType)
	{
		if (NewObjectType < ObjectTypeList.m_Total)
		{
			if (QuestManager.Instance.m_BuildingsLocked.ContainsKey(NewObjectType) || QuestManager.Instance.m_ObjectsLocked.ContainsKey(NewObjectType))
			{
				return false;
			}
			return true;
		}
		return true;
	}

	public void Init(ObjectType NewObjectType, bool CheckNew = true)
	{
		m_ObjectType = NewObjectType;
		if (NewObjectType >= ObjectTypeList.m_Total)
		{
			m_Tile = true;
			m_TileType = (Tile.TileType)(NewObjectType - ObjectTypeList.m_Total);
		}
		else
		{
			m_Tile = false;
		}
		Sprite sprite = (m_Tile ? Tile.GetIcon(m_TileType) : IconManager.Instance.GetIcon(NewObjectType));
		SetSprite(sprite);
		UpdateLocked();
		if (base.transform.Find("CountPanel") != null)
		{
			m_CountPanel = base.transform.Find("CountPanel").gameObject;
			m_Count = base.transform.Find("CountPanel/Count").GetComponent<BaseText>();
			m_CountPanel.SetActive(value: false);
		}
		if (CheckNew && ObjectsPanels.Instance.GetObjectNew(NewObjectType))
		{
			AddNew();
		}
		if (base.transform.Find("Pin") != null)
		{
			base.transform.Find("Pin").gameObject.SetActive(value: false);
		}
	}

	public void SetCount(int Count)
	{
		m_CountPanel.SetActive(Count > 0);
		m_Count.SetText(Count.ToString());
	}

	public override void SetInteractable(bool Interactable)
	{
		base.SetInteractable(Interactable);
		BaseSetInteractable(Interactable);
	}

	public override void SetSelected(bool Selected)
	{
		base.SetSelected(Selected);
		BaseSetSelected(Selected);
	}

	public override void SetIndicated(bool Indicated)
	{
		base.SetIndicated(Indicated);
		BaseSetIndicated(Indicated);
		if (!m_Tile)
		{
			HudManager.Instance.ActivateHoldableRollover(Indicated, m_ObjectType);
		}
		else
		{
			HudManager.Instance.ActivateTileRollover(Indicated, m_TileType, default(TileCoord));
		}
	}

	public void UpdateLocked()
	{
		SetLocked(!GetIsObjectUnlocked(m_ObjectType));
	}
}
