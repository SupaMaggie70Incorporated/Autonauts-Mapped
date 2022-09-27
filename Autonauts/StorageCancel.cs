using System.Collections.Generic;
using UnityEngine;

public class StorageCancel : BuildingCancel
{
	private Storage m_Storage;

	private StorageRollover m_Rollover;

	protected BaseButtonImage m_UpgradeButton;

	private BaseButtonImage m_LockButton;

	private Building m_CollisionBuilding;

	private bool m_Interactable;

	public override void SetBuilding(Building NewBuilding)
	{
		base.SetBuilding(NewBuilding);
		m_Storage = NewBuilding.GetComponent<Storage>();
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Rollovers/StorageRollover", typeof(GameObject));
		m_Rollover = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, base.transform).GetComponent<StorageRollover>();
		m_Rollover.SetStorageTarget(NewBuilding.GetComponent<Storage>());
		m_Rollover.ForceOpen();
		AddObjectToPanel(m_Rollover.gameObject);
		if (m_Storage.m_BuildingToUpgradeTo != ObjectTypeList.m_Total)
		{
			m_CollisionBuilding = ObjectTypeList.Instance.CreateObjectFromIdentifier(m_Storage.m_BuildingToUpgradeTo, m_Storage.transform.position, Quaternion.identity).GetComponent<Building>();
			m_CollisionBuilding.SetRotation(m_Storage.m_Rotation);
			m_CollisionBuilding.HideAccessModel();
			m_CollisionBuilding.gameObject.SetActive(value: false);
		}
		else
		{
			m_CollisionBuilding = null;
		}
		UpdateLockButton();
	}

	protected new void Awake()
	{
		base.Awake();
		m_UpgradeButton = m_Panel.transform.Find("UpgradeButton").GetComponent<BaseButtonImage>();
		m_LockButton = m_Panel.transform.Find("LockButton").GetComponent<BaseButtonImage>();
	}

	protected new void OnDestroy()
	{
		if ((bool)m_CollisionBuilding)
		{
			m_CollisionBuilding.StopUsing();
		}
		base.OnDestroy();
	}

	protected new void Start()
	{
		base.Start();
		m_UpgradeButton.SetAction(OnUpgradeClicked, m_UpgradeButton);
		m_LockButton.SetAction(OnLockClicked, m_LockButton);
	}

	private void UpdateLockButton()
	{
		string text = "LockButton";
		string rolloverFromID = "StorageCancelLock";
		if (!m_Storage.GetIsLocked())
		{
			text = "UnlockButton";
			rolloverFromID = "StorageCancelUnlock";
		}
		m_LockButton.SetSprite("Buttons/" + text);
		m_LockButton.SetRolloverFromID(rolloverFromID);
	}

	public override void OnCancelSelected(BaseGadget NewGadget)
	{
		Disengage();
		m_Building.GetComponent<Storage>().SetObjectType(ObjectTypeList.m_Total);
		GameStateManager.Instance.PopState();
	}

	private void UpdateUpgradeButton()
	{
		bool active = true;
		m_Interactable = true;
		string rolloverFromID = "StorageCancelUpgrade";
		if (m_Storage.m_BuildingToUpgradeTo == ObjectTypeList.m_Total)
		{
			active = false;
		}
		else if (m_Storage.m_TotalLevels != m_Storage.m_MaxLevels)
		{
			m_Interactable = false;
			rolloverFromID = "StorageCancelUpgradeTaller";
		}
		else if (QuestManager.Instance.GetIsBuildingLocked(m_Storage.m_BuildingToUpgradeTo))
		{
			m_Interactable = false;
			rolloverFromID = "StorageCancelUpgradeUnlock";
		}
		else if (GameOptionsManager.Instance.m_Options.m_GameMode == GameOptions.GameMode.ModeCreative || CheatManager.Instance.m_InstantBuild)
		{
			List<BaseClass> list = new List<BaseClass>();
			Building building = TileManager.Instance.GetTile(m_Storage.m_TileCoord).m_Building;
			if ((bool)building)
			{
				list.Add(building);
			}
			if (MapManager.Instance.CheckBuildingIntersection(m_CollisionBuilding, list, out var _) && building.GetComponent<ResearchStation>() == null)
			{
				m_Interactable = false;
				rolloverFromID = "StorageCancelUpgradeSpace";
			}
		}
		m_UpgradeButton.SetActive(active);
		m_UpgradeButton.SetRolloverFromID(rolloverFromID);
	}

	protected new void Update()
	{
		base.Update();
		int storedForDisplay = m_Storage.GetStoredForDisplay();
		m_CancelButton.SetInteractable(storedForDisplay == 0);
		UpdateUpgradeButton();
	}

	public override void OnCancelEnter(BaseGadget NewGadget)
	{
		base.OnCancelEnter(NewGadget);
		HudManager.Instance.ActivateWarningRollover(Activate: true, "StorageCancelButton");
	}

	public override void OnCancelExit(BaseGadget NewGadget)
	{
		base.OnCancelExit(NewGadget);
		HudManager.Instance.ActivateWarningRollover(Activate: false);
	}

	public void OnUpgradeClicked(BaseGadget NewGadget)
	{
		if (m_Interactable)
		{
			m_Storage.m_Engager.SendAction(new ActionInfo(ActionType.DisengageObject, m_Storage.m_Engager.GetComponent<TileCoordObject>().m_TileCoord, m_Storage));
			m_Storage.BeginUpgrade();
			GameStateManager.Instance.PopState();
		}
	}

	public void OnLockClicked(BaseGadget NewGadget)
	{
		m_Storage.ToggleLock();
		UpdateLockButton();
	}
}
