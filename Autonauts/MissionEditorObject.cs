public class MissionEditorObject : MissionEditorGadget
{
	public ObjectType m_Type;

	public void SetType(ObjectType NewType)
	{
		m_Type = NewType;
		BaseButtonText component = base.transform.Find("BaseButtonText").GetComponent<BaseButtonText>();
		component.SetText(ObjectTypeList.Instance.GetHumanReadableNameFromIdentifier(NewType));
		component.SetAction(OnClicked, component);
	}

	public void OnClicked(BaseGadget NewGadget)
	{
		m_Parent.OnObjectClicked(this);
	}
}
