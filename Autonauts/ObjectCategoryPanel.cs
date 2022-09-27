using System.Collections.Generic;
using UnityEngine;

public class ObjectCategoryPanel : BaseMenu
{
	private BaseScrollView m_Panel;

	private ObjectCategoryButton m_DefaultButton;

	private List<ObjectCategoryButton> m_ObjectCategoryButtons;

	private ObjectCategoryButton m_SelectedObjectCategory;

	protected new void Awake()
	{
		base.Awake();
		m_Panel = base.transform.Find("BasePanel").Find("BaseScrollView").GetComponent<BaseScrollView>();
		m_DefaultButton = m_Panel.GetContent().transform.Find("DefaultButton").GetComponent<ObjectCategoryButton>();
		m_DefaultButton.SetActive(Active: false);
		CreateTabs();
		m_SelectedObjectCategory = null;
	}

	protected new void Start()
	{
		base.Start();
		BaseButtonBack component = base.transform.Find("BasePanel/StandardCancelButton").GetComponent<BaseButtonBack>();
		RemoveAction(component);
		component.SetAction(OnMyBackClicked, component);
	}

	public void OnMyBackClicked(BaseGadget NewGadget)
	{
		GameStateManager.Instance.GetCurrentState().GetComponent<GameStateIndustry>().SetMode(GameStateIndustry.Mode.Total);
	}

	private void CreateTabs()
	{
		Transform parent = m_Panel.GetContent().transform;
		Vector2 anchoredPosition = m_DefaultButton.GetComponent<RectTransform>().anchoredPosition;
		float num = m_DefaultButton.GetComponent<RectTransform>().rect.width + 10f;
		m_ObjectCategoryButtons = new List<ObjectCategoryButton>();
		for (int i = 0; i < 17; i++)
		{
			ObjectCategory objectCategory = (ObjectCategory)i;
			if (objectCategory != ObjectCategory.Hidden && objectCategory != ObjectCategory.Any && objectCategory != ObjectCategory.Effects)
			{
				ObjectCategoryButton component = Object.Instantiate(m_DefaultButton, parent).GetComponent<ObjectCategoryButton>();
				component.gameObject.SetActive(value: true);
				component.Init(objectCategory);
				component.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
				RegisterGadget(component);
				AddAction(component, OnIndustryClicked);
				m_ObjectCategoryButtons.Add(component);
				anchoredPosition.x += num;
			}
		}
	}

	public void SetSelectedObjectCategory(ObjectCategory NewObjectCategory)
	{
		m_SelectedObjectCategory = null;
		foreach (ObjectCategoryButton objectCategoryButton in m_ObjectCategoryButtons)
		{
			if (objectCategoryButton.m_ObjectCategory == NewObjectCategory)
			{
				objectCategoryButton.SetSelected(Selected: true);
				m_SelectedObjectCategory = objectCategoryButton;
			}
			else
			{
				objectCategoryButton.SetSelected(Selected: false);
			}
		}
	}

	public void OnIndustryClicked(BaseGadget NewGadget)
	{
		ObjectCategoryButton component = NewGadget.GetComponent<ObjectCategoryButton>();
		if (m_SelectedObjectCategory != component)
		{
			SetSelectedObjectCategory(component.m_ObjectCategory);
			ObjectsPanels.Instance.SetObjectCategory(m_SelectedObjectCategory.m_ObjectCategory);
		}
		else
		{
			SetSelectedObjectCategory(ObjectCategory.Total);
			ObjectsPanels.Instance.SetObjectCategory(ObjectCategory.Total);
		}
		ObjectsPanels.Instance.HideRecipe();
	}

	public void RemoveNew(ObjectCategory NewObjectCategory)
	{
		foreach (ObjectCategoryButton objectCategoryButton in m_ObjectCategoryButtons)
		{
			if (objectCategoryButton.m_ObjectCategory == NewObjectCategory)
			{
				objectCategoryButton.RemoveNew();
			}
		}
	}
}
