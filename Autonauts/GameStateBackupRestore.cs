using UnityEngine;
using UnityEngine.UI;

public class GameStateBackupRestore : GameStateBase
{
	[HideInInspector]
	public GameObject m_Ceremony;

	protected new void Awake()
	{
		base.Awake();
		Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Scripting/BackupRestore", typeof(GameObject));
		m_Ceremony = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, menusRootTransform);
		m_Ceremony.transform.localPosition = new Vector3(HudManager.Instance.m_HalfCanvasWidth, HudManager.Instance.m_HalfCanvasHeight, 0f);
	}

	protected new void OnDestroy()
	{
		base.OnDestroy();
		Object.Destroy(m_Ceremony.gameObject);
	}

	public void SetBackup(bool Backup)
	{
		string text = TextManager.Instance.Get("BackupRestoreBackup");
		if (!Backup)
		{
			text = TextManager.Instance.Get("BackupRestoreRestore");
		}
		Text componentInChildren = m_Ceremony.GetComponentInChildren<Text>();
		componentInChildren.text = text;
		float x = componentInChildren.preferredWidth + 40f;
		m_Ceremony.GetComponent<RectTransform>().sizeDelta = new Vector2(x, 50f);
		componentInChildren.GetComponent<RectTransform>().sizeDelta = new Vector2(x, 50f);
	}

	public override void UpdateState()
	{
	}
}
