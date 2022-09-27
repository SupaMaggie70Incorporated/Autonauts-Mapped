using UnityEngine;

public class ObjectBaseButton : BaseButtonImage
{
	private NewThing m_NewIcon;

	public void AddNew()
	{
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/NewThing", typeof(GameObject));
		m_NewIcon = Object.Instantiate(original, default(Vector3), Quaternion.identity, base.transform).GetComponent<NewThing>();
		m_NewIcon.GetComponent<RectTransform>().localPosition = new Vector3(-20f, 20f, 0f);
		m_NewIcon.UpdateWhilePaused();
	}

	public void RemoveNew()
	{
		if ((bool)m_NewIcon)
		{
			Object.Destroy(m_NewIcon.gameObject);
			m_NewIcon = null;
		}
	}
}
