using UnityEngine;

public class ObjectSelectScrollView : BaseScrollView
{
	[HideInInspector]
	public ButtonList m_ButtonList;

	protected new void Awake()
	{
		base.Awake();
		m_ContentObject = base.transform.Find("Viewport2").Find("Content").gameObject;
		m_ButtonList = m_ContentObject.transform.Find("ButtonList").GetComponent<ButtonList>();
	}

	public void PopGadgetOnTop(BaseGadget NewGadget)
	{
		Transform parent = NewGadget.transform.parent;
		NewGadget.transform.SetParent(base.transform);
		NewGadget.transform.SetParent(parent);
	}
}
