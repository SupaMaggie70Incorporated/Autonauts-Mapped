using UnityEngine;

public class GameStateObjectSelect : GameStateBase
{
	private static ObjectCategory m_LastCategory = ObjectCategory.Parts;

	private ObjectSelect m_ObjectSelect;

	protected new void Awake()
	{
		base.Awake();
		Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/ObjectSelect/ObjectSelect", typeof(GameObject));
		m_ObjectSelect = Object.Instantiate(original, menusRootTransform).GetComponent<ObjectSelect>();
		m_ObjectSelect.GetComponent<RectTransform>().anchoredPosition = default(Vector2);
		m_ObjectSelect.transform.localPosition = default(Vector3);
	}

	protected new void OnDestroy()
	{
		m_LastCategory = m_ObjectSelect.m_CurrentCategory;
		base.OnDestroy();
		Object.Destroy(m_ObjectSelect.gameObject);
	}

	public void Init(bool BuildingsOnly, bool Everything)
	{
		m_ObjectSelect.Init(BuildingsOnly, Everything);
		m_ObjectSelect.SetCategory(m_LastCategory);
	}

	public override void UpdateState()
	{
		if (MyInputManager.m_Rewired.GetButtonDown("Quit"))
		{
			AudioManager.Instance.StartEvent("UICloseWindow");
			GameStateManager.Instance.PopState();
		}
	}
}
