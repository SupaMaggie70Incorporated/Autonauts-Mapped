using UnityEngine;

public class GameStateOK : GameStateBase
{
	[HideInInspector]
	public OK m_OK;

	protected new void Awake()
	{
		base.Awake();
		Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/OK", typeof(GameObject));
		m_OK = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, menusRootTransform).GetComponent<OK>();
		m_OK.GetComponent<RectTransform>().anchoredPosition = default(Vector2);
	}

	protected new void OnDestroy()
	{
		base.OnDestroy();
		Object.Destroy(m_OK.gameObject);
	}

	public void SetMessage(string Description)
	{
		m_OK.SetMessage(Description);
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
