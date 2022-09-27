using UnityEngine;

public class MissionEditorGadget : MonoBehaviour
{
	protected MissionEditor m_Parent;

	public void SetParent(MissionEditor NewParent)
	{
		m_Parent = NewParent;
		BaseButton component = base.transform.Find("Remove").GetComponent<BaseButton>();
		component.SetAction(OnRemoveClicked, component);
		component = base.transform.Find("Up").GetComponent<BaseButton>();
		component.SetAction(OnUpClicked, component);
		component = base.transform.Find("Down").GetComponent<BaseButton>();
		component.SetAction(OnDownClicked, component);
	}

	public void OnRemoveClicked(BaseGadget NewGadget)
	{
		m_Parent.Remove(this);
	}

	public void OnUpClicked(BaseGadget NewGadget)
	{
		m_Parent.Up(this);
	}

	public void OnDownClicked(BaseGadget NewGadget)
	{
		m_Parent.Down(this);
	}
}
