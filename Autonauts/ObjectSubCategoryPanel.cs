using System.Collections.Generic;
using UnityEngine;

public class ObjectSubCategoryPanel : BaseMenu
{
	private BaseScrollView m_Panel;

	private ObjectSubCategoryButton m_DefaultButton;

	private List<ObjectSubCategoryButton> m_SubCategoryButtons;

	private ObjectSubCategoryButton m_SelectedSubCategory;

	private BaseText m_Title;

	protected new void Awake()
	{
		base.Awake();
		m_Panel = base.transform.Find("BasePanel").Find("BaseScrollView").GetComponent<BaseScrollView>();
		m_DefaultButton = m_Panel.GetContent().transform.Find("DefaultButton").GetComponent<ObjectSubCategoryButton>();
		m_DefaultButton.SetActive(Active: false);
		m_Title = base.transform.Find("BasePanel").Find("Title").GetComponent<BaseText>();
		m_SelectedSubCategory = null;
		m_SubCategoryButtons = new List<ObjectSubCategoryButton>();
		base.gameObject.SetActive(value: false);
	}

	public void SetCurrentCategory(ObjectCategory NewObjectCategory)
	{
		foreach (ObjectSubCategoryButton subCategoryButton in m_SubCategoryButtons)
		{
			Object.Destroy(subCategoryButton.gameObject);
		}
		m_SubCategoryButtons.Clear();
		if (NewObjectCategory != ObjectCategory.Total)
		{
			base.gameObject.SetActive(value: true);
			CreateButtons(NewObjectCategory);
			string categoryName = ObjectTypeList.Instance.GetCategoryName(NewObjectCategory);
			m_Title.SetTextFromID(categoryName);
		}
		else
		{
			base.gameObject.SetActive(value: false);
			m_Title.SetText("");
		}
		ObjectsPanels.Instance.SetObjectSubCategory(ObjectSubCategory.Total);
	}

	public bool GetAnyObjectsUnlocked()
	{
		foreach (ObjectSubCategoryButton subCategoryButton in m_SubCategoryButtons)
		{
			if (!subCategoryButton.GetIsLocked())
			{
				return true;
			}
		}
		return false;
	}

	private void CreateButtons(ObjectCategory NewObjectCategory)
	{
		Transform parent = m_Panel.GetContent().transform;
		Vector2 anchoredPosition = m_DefaultButton.GetComponent<RectTransform>().anchoredPosition;
		float num = m_DefaultButton.GetComponent<RectTransform>().rect.width + 10f;
		for (int i = 0; i < 49; i++)
		{
			ObjectSubCategory objectSubCategory = (ObjectSubCategory)i;
			if (ObjectTypeList.Instance.GetCategoryFromSubCategory(objectSubCategory) == NewObjectCategory)
			{
				ObjectSubCategoryButton component = Object.Instantiate(m_DefaultButton, parent).GetComponent<ObjectSubCategoryButton>();
				component.gameObject.SetActive(value: true);
				component.Init(objectSubCategory);
				component.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
				RegisterGadget(component);
				AddAction(component, OnIndustrySubClicked);
				m_SubCategoryButtons.Add(component);
				anchoredPosition.x += num;
			}
		}
	}

	public void SetSelectedIndustrySub(ObjectSubCategory NewSubCategory)
	{
		m_SelectedSubCategory = null;
		foreach (ObjectSubCategoryButton subCategoryButton in m_SubCategoryButtons)
		{
			if (subCategoryButton.m_ObjectSubCategory == NewSubCategory)
			{
				subCategoryButton.SetSelected(Selected: true);
				m_SelectedSubCategory = subCategoryButton;
			}
			else
			{
				subCategoryButton.SetSelected(Selected: false);
			}
		}
	}

	public void OnIndustrySubClicked(BaseGadget NewGadget)
	{
		ObjectSubCategoryButton component = NewGadget.GetComponent<ObjectSubCategoryButton>();
		if (m_SelectedSubCategory != component)
		{
			SetSelectedIndustrySub(component.m_ObjectSubCategory);
			ObjectsPanels.Instance.SetObjectSubCategory(m_SelectedSubCategory.m_ObjectSubCategory);
		}
		else
		{
			SetSelectedIndustrySub(ObjectSubCategory.Total);
			ObjectsPanels.Instance.SetObjectSubCategory(ObjectSubCategory.Total);
		}
		ObjectsPanels.Instance.HideRecipe();
	}

	public void RemoveNew(ObjectSubCategory NewSubCategory)
	{
		foreach (ObjectSubCategoryButton subCategoryButton in m_SubCategoryButtons)
		{
			if (subCategoryButton.m_ObjectSubCategory == NewSubCategory)
			{
				subCategoryButton.RemoveNew();
			}
		}
	}
}
