using UnityEngine;

public class BuildingCategoryButton : BaseButtonImage
{
	private BuildingPalette m_Parent;

	public BuildingCategoryInfo m_Info;

	private NewThing m_NewIcon;

	protected new void Awake()
	{
		base.Awake();
		SetAction(OnClick, this);
	}

	public void SetInfo(BuildingPalette Parent, BuildingCategoryInfo NewInfo, bool New)
	{
		m_Parent = Parent;
		m_Info = NewInfo;
		SetRolloverFromID(NewInfo.m_Name);
		string subCategorySprite = ObjectTypeList.Instance.GetSubCategorySprite(NewInfo.m_Category);
		SetSprite(subCategorySprite);
		if (New)
		{
			GameObject original = (GameObject)Resources.Load("Prefabs/Hud/NewThing", typeof(GameObject));
			m_NewIcon = Object.Instantiate(original, default(Vector3), Quaternion.identity, base.transform).GetComponent<NewThing>();
			m_NewIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(-30f, 15f);
		}
		UpdateBackground();
	}

	public void OnClick(BaseGadget NewGadget)
	{
		AudioManager.Instance.StartEvent("UIOptionSelected");
		m_Parent.SetCategory(m_Info.m_Category);
	}

	private void UpdatePosition()
	{
		float y = -10f;
		if (!m_Selected && m_Indicated)
		{
			y = 0f;
		}
		if (m_Selected)
		{
			y = 10f;
		}
		Vector3 localPosition = base.transform.localPosition;
		localPosition.y = y;
		base.transform.localPosition = localPosition;
	}

	private void UpdateBackground()
	{
		if (m_Selected)
		{
			SetBackingSprite("Edit/TabSelected");
		}
		else
		{
			SetBackingSprite("Edit/TabUnselected");
		}
	}

	public override void SetSelected(bool Selected)
	{
		base.SetSelected(Selected);
		if (m_Selected && (bool)m_NewIcon)
		{
			m_NewIcon.gameObject.SetActive(value: false);
		}
		UpdatePosition();
		UpdateBackground();
	}

	public override void SetIndicated(bool Indicated)
	{
		base.SetIndicated(Indicated);
		UpdatePosition();
	}
}
