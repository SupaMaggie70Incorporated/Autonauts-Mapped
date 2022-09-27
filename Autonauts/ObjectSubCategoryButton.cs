using UnityEngine;

public class ObjectSubCategoryButton : ObjectBaseButton
{
	[HideInInspector]
	public ObjectSubCategory m_ObjectSubCategory;

	public void Init(ObjectSubCategory NewObjectSubCategory)
	{
		m_ObjectSubCategory = NewObjectSubCategory;
		string subCategorySprite = ObjectTypeList.Instance.GetSubCategorySprite(NewObjectSubCategory);
		SetSprite(subCategorySprite);
		string subCategoryName = ObjectTypeList.Instance.GetSubCategoryName(NewObjectSubCategory);
		SetRolloverFromID(subCategoryName);
		if (ObjectsPanels.Instance.GetSubCategoryNew(NewObjectSubCategory))
		{
			AddNew();
		}
		SetLocked(!ObjectTypeButton.GetIsObjectSubCategoryUnlocked(NewObjectSubCategory));
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
