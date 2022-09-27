using UnityEngine;

public class GameStateInventory : GameStateBase
{
	[HideInInspector]
	private InventoryPanel m_Panel;

	[HideInInspector]
	private InventoryPanel m_SecondPanel;

	private InventoryBar m_Bar;

	private InventoryBar m_SecondBar;

	private GameObject m_NoTrade;

	private bool m_OldHudActive;

	protected new void Awake()
	{
		base.Awake();
		m_OldHudActive = HudManager.Instance.GetHudButtonsActive();
		HudManager.Instance.HideRollovers();
		HudManager.Instance.SetHudButtonsActive(Active: false);
		CameraManager.Instance.SetPausedDOFEffect();
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Inventory/NoTrade", typeof(GameObject));
		m_NoTrade = Object.Instantiate(original, HudManager.Instance.m_MenusRootTransform);
		m_NoTrade.SetActive(value: false);
	}

	protected new void OnDestroy()
	{
		base.OnDestroy();
		Object.Destroy(m_NoTrade.gameObject);
		Object.Destroy(m_Panel.gameObject);
		Object.Destroy(m_Bar.gameObject);
		if ((bool)m_SecondPanel)
		{
			Object.Destroy(m_SecondPanel.gameObject);
		}
		if ((bool)m_SecondBar)
		{
			Object.Destroy(m_SecondBar.gameObject);
		}
		HudManager.Instance.SetHudButtonsActive(m_OldHudActive);
		CameraManager.Instance.RestorePausedDOFEffect();
	}

	public void SetInfo(Farmer NewFarmer, BaseClass SecondObject)
	{
		Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Inventory/InventoryBar", typeof(GameObject));
		GameObject original2 = (GameObject)Resources.Load("Prefabs/Hud/Inventory/InventoryPanel", typeof(GameObject));
		Vector2 vector = new Vector2(10f, 10f);
		m_Panel = Object.Instantiate(original2, new Vector3(0f, 0f, 0f), Quaternion.identity, menusRootTransform).GetComponent<InventoryPanel>();
		m_Panel.GetComponent<RectTransform>().anchoredPosition = vector;
		m_Panel.SetObject(NewFarmer);
		m_Bar = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, menusRootTransform).GetComponent<InventoryBar>();
		RectTransform component = m_Bar.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0f, 0f);
		component.anchorMax = new Vector2(0f, 0f);
		component.pivot = new Vector2(0f, 0f);
		component.anchoredPosition = new Vector2(m_Panel.GetWidth() + 10f, 0f) + vector;
		m_Bar.SetObject(NewFarmer);
		if ((bool)SecondObject)
		{
			vector = new Vector2(-10f, -10f);
			m_SecondPanel = Object.Instantiate(original2, new Vector3(0f, 0f, 0f), Quaternion.identity, menusRootTransform).GetComponent<InventoryPanel>();
			RectTransform component2 = m_SecondPanel.GetComponent<RectTransform>();
			component2.anchorMin = new Vector2(1f, 1f);
			component2.anchorMax = new Vector2(1f, 1f);
			component2.pivot = new Vector2(1f, 1f);
			component2.anchoredPosition = vector;
			m_SecondPanel.SetObject(SecondObject);
			m_SecondBar = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, menusRootTransform).GetComponent<InventoryBar>();
			RectTransform component3 = m_SecondBar.GetComponent<RectTransform>();
			component3.anchorMin = new Vector2(1f, 1f);
			component3.anchorMax = new Vector2(1f, 1f);
			component3.pivot = new Vector2(1f, 1f);
			component3.anchoredPosition = new Vector2(0f - m_SecondPanel.GetWidth() - 10f, 0f) + vector;
			m_SecondBar.SetObject(SecondObject);
		}
	}

	public void FarmerToolUsed(Farmer NewFarmer)
	{
		if (m_Panel.m_Farmer == NewFarmer)
		{
			m_Panel.UpdateHealth();
			m_Bar.UpdateHealth();
		}
		else if ((bool)m_SecondPanel && m_SecondPanel.m_Farmer == NewFarmer)
		{
			m_SecondPanel.UpdateHealth();
			m_SecondBar.UpdateHealth();
		}
	}

	public void Close()
	{
		AudioManager.Instance.StartEvent("UICloseWindow");
		Farmer farmer = null;
		BaseClass baseClass = null;
		if ((bool)m_SecondPanel)
		{
			baseClass = m_SecondPanel.GetComponent<InventoryPanel>().m_Object;
			farmer = m_SecondPanel.GetComponent<InventoryPanel>().m_Farmer;
		}
		GameStateManager.Instance.PopState();
		if ((bool)farmer)
		{
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateNormal>().SetSelectedWorker(farmer.GetComponent<Worker>(), Confirmed: false, Scale: false);
		}
		else if (baseClass != null)
		{
			Actionable component = baseClass.GetComponent<Actionable>();
			if (baseClass.m_TypeIdentifier == ObjectType.Wardrobe || Aquarium.GetIsTypeAquiarium(baseClass.m_TypeIdentifier))
			{
				component.m_Engager.SendAction(new ActionInfo(ActionType.DisengageObject, component.m_Engager.GetComponent<TileCoordObject>().m_TileCoord, component));
			}
		}
	}

	private void UpdateOutOfRange()
	{
		if (!(m_SecondPanel == null) && !(m_SecondPanel.m_Farmer == null))
		{
			Worker component = m_SecondPanel.m_Farmer.GetComponent<Worker>();
			bool flag = m_Panel.m_Farmer.GetComponent<FarmerPlayer>().IsWorkerCloseEnoughToTrade(component);
			m_NoTrade.SetActive(!flag);
			m_SecondPanel.SetInRange(flag);
			m_SecondBar.SetInRange(flag);
		}
	}

	public override void UpdateState()
	{
		UpdateOutOfRange();
		if (MyInputManager.m_Rewired.GetButtonDown("Quit") || MyInputManager.m_Rewired.GetButtonDown("Inventory"))
		{
			Close();
		}
	}

	public void UpdateInventory()
	{
	}

	public void CheckTargetUpdated(BaseClass Target)
	{
		if ((bool)m_Panel)
		{
			m_Panel.GetComponent<InventoryPanel>().CheckTargetUpdated(Target);
		}
		if ((bool)m_SecondPanel)
		{
			m_SecondPanel.GetComponent<InventoryPanel>().CheckTargetUpdated(Target);
		}
	}

	public void CheckTargetPickedUp(Worker TestWorker)
	{
		if ((bool)m_SecondPanel && m_SecondPanel.GetComponent<InventoryPanel>().m_Farmer == TestWorker)
		{
			GameStateManager.Instance.PopState();
			GameStateBase currentState = GameStateManager.Instance.GetCurrentState();
			if ((bool)currentState.GetComponent<GameStateNormal>())
			{
				currentState.GetComponent<GameStateNormal>().CheckTargetPickedUp(TestWorker);
			}
		}
	}
}
