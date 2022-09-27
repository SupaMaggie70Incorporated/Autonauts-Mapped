using UnityEngine;

public class GameStateMissionList : GameStateBase
{
	private MissionList m_List;

	protected new void Awake()
	{
		base.Awake();
		Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Autopedia/MissionEditor/MissionList", typeof(GameObject));
		m_List = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, menusRootTransform).GetComponent<MissionList>();
	}

	protected new void OnDestroy()
	{
		base.OnDestroy();
		Object.Destroy(m_List.gameObject);
	}

	public void SetMissionGadget(MissionEditorGadget NewGadget, MissionEditor NewEditor)
	{
		m_List.SetMissionGadget(NewGadget, NewEditor);
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
