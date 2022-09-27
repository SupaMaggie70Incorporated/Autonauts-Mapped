using System.Collections.Generic;
using UnityEngine;

public class GameStateSelectObject : GameStateBase
{
	public static GameStateSelectObject Instance;

	private ObjectSelectBar m_Title;

	[HideInInspector]
	public TileCoord m_TilePosition;

	[HideInInspector]
	public Worker m_TargetObject;

	private List<ObjectType> m_SearchTypes;

	private HudInstruction m_TargetInstruction;

	private bool m_MouseButtonHeld;

	private bool m_HudButtonsActive;

	protected new void Awake()
	{
		base.Awake();
		Instance = this;
		Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Standard/ObjectSelectBar", typeof(GameObject));
		m_Title = Object.Instantiate(original, default(Vector3), Quaternion.identity, menusRootTransform).GetComponent<ObjectSelectBar>();
		m_Title.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -30f);
		m_Title.SetTitle("SelectObjectTitle");
		m_TargetInstruction = null;
		m_MouseButtonHeld = TestMouseButtonDown(0);
		MaterialManager.Instance.SetDesaturation(Desaturated: true, BuildingsDesaturated: true);
		PlotManager.Instance.SetDesaturation(Desaturated: true);
		m_HudButtonsActive = HudManager.Instance.GetHudButtonsActive();
		HudManager.Instance.SetHudButtonsActive(Active: false);
	}

	protected new void OnDestroy()
	{
		ModelManager.Instance.SetSearchTypesHighlight(m_SearchTypes, Desaturated: false, AndOccluded: false);
		HudManager.Instance.SetHudButtonsActive(m_HudButtonsActive);
		MaterialManager.Instance.SetDesaturation(Desaturated: false, BuildingsDesaturated: false);
		PlotManager.Instance.SetDesaturation(Desaturated: false);
		base.OnDestroy();
		Object.Destroy(m_Title.gameObject);
	}

	public void SetSearchType(List<ObjectType> SearchTypes)
	{
		if (SearchTypes.Contains(ObjectType.Sign) || SearchTypes.Contains(ObjectType.Sign2) || SearchTypes.Contains(ObjectType.Billboard) || SearchTypes.Contains(ObjectType.Sign3))
		{
			m_Title.SetTitle("SelectObjectSignTitle");
		}
		else if (SearchTypes.Contains(ObjectType.StorageGeneric))
		{
			m_Title.SetTitle("SelectObjectStorageTitle");
		}
		m_SearchTypes = SearchTypes;
		ModelManager.Instance.SetSearchTypesHighlight(m_SearchTypes, Desaturated: true, AndOccluded: false);
	}

	public void SetInstruction(HudInstruction TargetInstruction)
	{
		m_TargetInstruction = TargetInstruction;
	}

	public override void UpdateState()
	{
		bool flag = false;
		Cursor.Instance.SetModel(ObjectTypeList.m_Total);
		TileCoordObject tileCoordObject = null;
		TileCoord tileCoord = default(TileCoord);
		GameObject gameObject = TestMouseCollision(out var CollisionPoint, TestTiles: true, TestBuildings: true, TestWorkers: false, TestMisc: true, TestWorkersDead: false);
		if ((bool)gameObject)
		{
			if ((bool)gameObject.GetComponent<PlotRoot>())
			{
				tileCoord = new TileCoord(CollisionPoint);
				Tile tile = TileManager.Instance.GetTile(tileCoord);
				if ((bool)tile.m_Building)
				{
					gameObject = tile.m_Building.gameObject;
				}
			}
			gameObject = GetRootObject(gameObject);
			if ((bool)gameObject && m_SearchTypes.Contains(gameObject.GetComponent<TileCoordObject>().m_TypeIdentifier))
			{
				if ((bool)gameObject.GetComponent<Building>() && (bool)gameObject.GetComponent<Building>().m_ParentBuilding)
				{
					gameObject = gameObject.GetComponent<Building>().m_ParentBuilding.gameObject;
				}
				tileCoordObject = gameObject.GetComponent<TileCoordObject>();
				tileCoord = tileCoordObject.m_TileCoord;
				flag = true;
				Cursor.Instance.Target(gameObject);
			}
		}
		HighlightObject(tileCoordObject);
		if (!flag)
		{
			Cursor.Instance.NoTarget();
			if (m_MouseButtonHeld)
			{
				m_MouseButtonHeld = TestMouseButtonDown(0);
			}
			else if (TestMouseButtonDown(0))
			{
				m_TargetInstruction.SetAssociatedObject(null);
				GameStateManager.Instance.PopState();
			}
		}
		else if (m_MouseButtonHeld)
		{
			m_MouseButtonHeld = TestMouseButtonDown(0);
		}
		else if (TestMouseButtonDown(0))
		{
			QuestManager.Instance.AddEvent(QuestEvent.Type.ObjectAreaSelect, Bot: false, tileCoordObject.m_TypeIdentifier, null);
			m_TargetInstruction.SetAssociatedObject(tileCoordObject);
			GameStateManager.Instance.PopState();
		}
		CameraManager.Instance.UpdateInput();
		if (MyInputManager.m_Rewired.GetButtonDown("Quit"))
		{
			AudioManager.Instance.StartEvent("UIOptionCancelled");
			GameStateManager.Instance.PopState();
		}
	}
}
