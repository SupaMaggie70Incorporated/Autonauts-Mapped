using UnityEngine;

public class ObjectCategoryButton : ObjectBaseButton
{
	[HideInInspector]
	public ObjectCategory m_ObjectCategory;

	public void Init(ObjectCategory NewCategory)
	{
		m_ObjectCategory = NewCategory;
		string categorySprite = ObjectTypeList.Instance.GetCategorySprite(NewCategory);
		SetSprite(categorySprite);
		string categoryName = ObjectTypeList.Instance.GetCategoryName(NewCategory);
		SetRolloverFromID(categoryName);
		if (ObjectsPanels.Instance.GetCategoryNew(NewCategory))
		{
			AddNew();
		}
		SetLocked(!ObjectTypeButton.GetIsObjectCategoryUnlocked(NewCategory));
		if (base.transform.Find("Pin") != null)
		{
			base.transform.Find("Pin").gameObject.SetActive(value: false);
		}
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
	}
}
