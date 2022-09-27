using UnityEngine;

public class StandardButtonBlueprint : BaseButtonImage
{
	[HideInInspector]
	public ObjectType m_Type;

	private Converter m_ParentConverter;

	protected NewThing m_NewIcon;

	public void SetObjectType(ObjectType NewType, Converter ParentConverter = null)
	{
		m_Type = NewType;
		m_ParentConverter = ParentConverter;
		if (m_Type == ObjectTypeList.m_Total)
		{
			SetImageEnabled(Enabled: false);
			SetBackingSprite("Panels/GeneralPanelBack");
			SetInteractable(Interactable: false);
			BaseSetInteractable(Interactable: false);
		}
		else
		{
			SetSprite(IconManager.Instance.GetIcon(NewType));
		}
	}

	public override void SetIndicated(bool Indicated)
	{
		base.SetIndicated(Indicated);
		BaseSetIndicated(Indicated);
		if ((bool)m_ParentConverter)
		{
			HudManager.Instance.ActivateConverterRollover(Indicated, m_Type, m_ParentConverter);
		}
		else
		{
			HudManager.Instance.ActivateConverterRollover(Indicated, m_Type);
		}
	}

	public override void SetSelected(bool Selected)
	{
		base.SetSelected(Selected);
		BaseSetSelected(Selected);
	}

	public void SetNew(bool New)
	{
		if (New)
		{
			GameObject original = (GameObject)Resources.Load("Prefabs/Hud/NewThing", typeof(GameObject));
			m_NewIcon = Object.Instantiate(original, default(Vector3), Quaternion.identity, base.transform).GetComponent<NewThing>();
			m_NewIcon.GetComponent<RectTransform>().localPosition = new Vector3(-20f, 20f, 0f);
		}
		else if (m_NewIcon != null)
		{
			Object.Destroy(m_NewIcon.gameObject);
		}
	}
}
