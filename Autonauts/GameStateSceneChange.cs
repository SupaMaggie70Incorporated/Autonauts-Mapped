using UnityEngine;

public class GameStateSceneChange : GameStateBase
{
	protected new void Awake()
	{
		base.Awake();
		Transform rootTransform = HudManager.Instance.m_RootTransform;
		Object.Instantiate((GameObject)Resources.Load("Prefabs/Hud/SceneChange", typeof(GameObject)), new Vector3(0f, 0f, 0f), Quaternion.identity, rootTransform).GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 0f, 0f);
	}
}
