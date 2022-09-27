using System.Collections.Generic;
using UnityEngine;

public class ObjectTypesPanel : BaseMenu
{
	private BaseScrollView m_Panel;

	private ObjectTypeButton m_DefaultButton;

	private List<ObjectTypeButton> m_ObjectTypeButtons;

	private ObjectTypeButton m_SelectedObjectType;

	private BaseText m_Title;

	protected new void Awake()
	{
		base.Awake();
		m_Panel = base.transform.Find("BasePanel").Find("BaseScrollView").GetComponent<BaseScrollView>();
		m_DefaultButton = m_Panel.GetContent().transform.Find("DefaultButton").GetComponent<ObjectTypeButton>();
		m_DefaultButton.SetActive(Active: false);
		m_Title = base.transform.Find("BasePanel").Find("Title").GetComponent<BaseText>();
		m_SelectedObjectType = null;
		m_ObjectTypeButtons = new List<ObjectTypeButton>();
		base.gameObject.SetActive(value: false);
	}

	protected new void Start()
	{
		base.Start();
		ObjectsPanels.Instance.m_ObjectRecipe.SetParent(base.transform);
	}

	public void SetCurrentIndustrySub(ObjectSubCategory NewObjectSubCategory)
	{
		foreach (ObjectTypeButton objectTypeButton in m_ObjectTypeButtons)
		{
			Object.Destroy(objectTypeButton.gameObject);
		}
		m_ObjectTypeButtons.Clear();
		if (NewObjectSubCategory != ObjectSubCategory.Total)
		{
			base.gameObject.SetActive(value: true);
			CreateButtons(NewObjectSubCategory);
			string subCategoryName = ObjectTypeList.Instance.GetSubCategoryName(NewObjectSubCategory);
			m_Title.SetTextFromID(subCategoryName);
		}
		else
		{
			base.gameObject.SetActive(value: false);
			m_Title.SetText("");
		}
	}

	private void CreateButtons(ObjectSubCategory NewObjectSubCategory)
	{
		Transform parent = m_Panel.GetContent().transform;
		Vector2 anchoredPosition = m_DefaultButton.GetComponent<RectTransform>().anchoredPosition;
		float num = 10f;
		float num2 = m_DefaultButton.GetComponent<RectTransform>().rect.width + num;
		for (int i = 0; i < (int)ObjectTypeList.m_Total; i++)
		{
			if (ObjectTypeList.Instance.GetSubCategoryFromType((ObjectType)i) == NewObjectSubCategory)
			{
				ObjectTypeButton component = Object.Instantiate(m_DefaultButton, parent).GetComponent<ObjectTypeButton>();
				component.gameObject.SetActive(value: true);
				component.Init((ObjectType)i);
				component.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
				RegisterGadget(component);
				AddAction(component, OnObjectTypeClicked);
				m_ObjectTypeButtons.Add(component);
				anchoredPosition.x += num2;
			}
		}
		m_Panel.SetScrollSize(anchoredPosition.x - num2 + num);
	}

	public void SetSelectedObjectType(ObjectType NewObjectType)
	{
		m_SelectedObjectType = null;
		foreach (ObjectTypeButton objectTypeButton in m_ObjectTypeButtons)
		{
			if (objectTypeButton.m_ObjectType == NewObjectType)
			{
				objectTypeButton.SetSelected(Selected: true);
				m_SelectedObjectType = objectTypeButton;
				float width = m_Panel.GetComponent<RectTransform>().rect.width;
				float num = m_Panel.GetScrollSize() - width;
				float num2 = m_Panel.GetScrollValue() * num;
				float num3 = objectTypeButton.GetPosition().x - num2 - objectTypeButton.GetWidth() / 2f;
				if (num3 < 0f || num3 + objectTypeButton.GetWidth() > width)
				{
					float num4 = objectTypeButton.GetPosition().x - objectTypeButton.GetWidth() / 2f;
					float scrollValue = ((!(num3 < 0f)) ? ((num4 - width + objectTypeButton.GetWidth()) / num) : (num4 / num));
					m_Panel.SetScrollValue(scrollValue);
				}
			}
			else
			{
				objectTypeButton.SetSelected(Selected: false);
			}
		}
	}

	public Vector3 GetSelectedObjectPosition()
	{
		Vector3 result = default(Vector3);
		if ((bool)m_SelectedObjectType)
		{
			return m_SelectedObjectType.GetComponent<RectTransform>().anchoredPosition;
		}
		return result;
	}

	public void OnObjectTypeClicked(BaseGadget NewGadget)
	{
		ObjectsPanels.Instance.NavigateDestroy();
		ObjectTypeButton component = NewGadget.GetComponent<ObjectTypeButton>();
		if (m_SelectedObjectType != component)
		{
			ObjectsPanels.Instance.ObjectSeen(component.m_ObjectType);
			SetSelectedObjectType(component.m_ObjectType);
			ObjectsPanels.Instance.SetObjectType(m_SelectedObjectType.m_ObjectType, UpdateMenus: false);
		}
		else
		{
			SetSelectedObjectType(ObjectTypeList.m_Total);
			ObjectsPanels.Instance.SetObjectType(ObjectTypeList.m_Total, UpdateMenus: false);
		}
	}

	public void RemoveNew(ObjectType NewObjectType)
	{
		foreach (ObjectTypeButton objectTypeButton in m_ObjectTypeButtons)
		{
			if (objectTypeButton.m_ObjectType == NewObjectType)
			{
				objectTypeButton.RemoveNew();
			}
		}
	}
}
