using UnityEngine;
using UnityEngine.UI;

public class GameStateTerraform : GameStateBase
{
	private Text m_Title;

	private TilePalette m_TilePalette;

	public static void Init()
	{
	}

	protected new void Awake()
	{
		base.Awake();
		Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/TestString", typeof(GameObject));
		m_Title = Object.Instantiate(original, new Vector3(350f, 50f, 0f), Quaternion.identity, menusRootTransform).GetComponent<Text>();
		m_Title.transform.localPosition = new Vector3(HudManager.Instance.m_HalfCanvasWidth, HudManager.Instance.m_CanvasHeight - 25f, 0f);
		m_Title.text = TextManager.Instance.Get("TerraformingTitle");
		GameObject original2 = (GameObject)Resources.Load("Prefabs/Hud/Terraform/TilePalette", typeof(GameObject));
		m_TilePalette = Object.Instantiate(original2, new Vector3(0f, 0f, 0f), Quaternion.identity, menusRootTransform).GetComponent<TilePalette>();
		m_TilePalette.transform.localPosition = new Vector3(0f, HudManager.Instance.m_HalfCanvasHeight, 0f);
		HudManager.Instance.RolloversEnabled(Enabled: false);
		HudManager.Instance.SetHudButtonsActive(Active: false);
	}

	protected new void OnDestroy()
	{
		HudManager.Instance.RolloversEnabled(Enabled: true);
		HudManager.Instance.SetHudButtonsActive(Active: true);
		base.OnDestroy();
		Object.Destroy(m_Title.gameObject);
		Object.Destroy(m_TilePalette.gameObject);
	}

	public void SetCurrentTileType(Tile.TileType NewType)
	{
	}

	public override void UpdateState()
	{
		if (!m_EventSystem.IsUIInFocus())
		{
			bool flag = false;
			TileCoord tileCoord = default(TileCoord);
			Vector3 CollisionPoint;
			GameObject gameObject = TestMouseCollision(out CollisionPoint, TestTiles: true, TestBuildings: false, TestWorkers: false, TestMisc: false, TestWorkersDead: false);
			if ((bool)gameObject && (bool)gameObject.GetComponent<PlotRoot>())
			{
				tileCoord = new TileCoord(CollisionPoint);
				Cursor.Instance.TargetTile(tileCoord);
				flag = true;
			}
			if (!flag)
			{
				Cursor.Instance.NoTarget();
			}
			else if (Input.GetMouseButton(0))
			{
				TileManager.Instance.SetTileType(tileCoord, TilePalette.m_CurrentType);
				if (TilePalette.m_CurrentType == Tile.TileType.Dredged)
				{
					WaterManager.Instance.AddDredgedTile(tileCoord);
				}
			}
		}
		else
		{
			Cursor.Instance.NoTarget();
		}
		if (!m_EventSystem.IsUIInUse())
		{
			CameraManager.Instance.UpdateInput();
		}
		if (MyInputManager.m_Rewired.GetButtonDown("Quit") || MyInputManager.m_Rewired.GetButtonDown("Edit"))
		{
			AudioManager.Instance.StartEvent("UICloseWindow");
			GameStateManager.Instance.SetState(GameStateManager.State.Normal);
		}
	}
}
