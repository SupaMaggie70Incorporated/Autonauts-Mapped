using UnityEngine;

public class GameStateMissionEditor : GameStateBase
{
	public MissionEditor m_Editor;

	protected new void Awake()
	{
		base.Awake();
		Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Autopedia/MissionEditor/MissionEditor", typeof(GameObject));
		m_Editor = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, menusRootTransform).GetComponent<MissionEditor>();
		m_Editor.transform.localPosition = default(Vector3);
	}

	protected new void OnDestroy()
	{
		base.OnDestroy();
		Object.Destroy(m_Editor.gameObject);
	}

	public override void Popped(GameStateManager.State NewState)
	{
		base.Popped(NewState);
	}

	public void SetLevel(IndustryLevel NewLevel)
	{
		m_Editor.SetLevel(NewLevel);
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
