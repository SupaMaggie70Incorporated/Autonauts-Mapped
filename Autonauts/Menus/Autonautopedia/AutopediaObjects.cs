public class AutopediaObjects : ObjectSelect
{
	public override void OnCategoryClicked(BaseGadget NewGadget)
	{
		base.OnCategoryClicked(NewGadget);
		Objects.Instance.NavigateDestroy();
	}

	public override void OnObjectClicked(BaseGadget NewGadget)
	{
		Objects.Instance.NavigateDestroy();
		int index = m_ObjectButtons.IndexOf(NewGadget.GetComponent<BaseButtonImage>());
		ObjectType objectType = m_ObjectList[index].m_ObjectType;
		Objects.Instance.SetObjectType(objectType);
		QuestManager.Instance.AddEvent(QuestEvent.Type.SelectAutopediaObjectType, Bot: false, objectType, null);
	}
}
