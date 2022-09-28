using UnityEngine;

public class BuildingCancel : BaseMenu
{
	protected Building m_Building;

	protected BasePanelOptions m_Panel;

	protected BaseButtonImage m_CancelButton;

	protected GameObject m_ObjectArea;

	private GameObject m_ObjectAdded;

	protected new void Awake()
	{
		base.Awake();
		m_Panel = base.transform.Find("BasePanelOptions").GetComponent<BasePanelOptions>();
		m_CancelButton = m_Panel.transform.Find("CancelButton").GetComponent<BaseButtonImage>();
		m_ObjectArea = m_Panel.transform.Find("ObjectArea").gameObject;
	}

	protected new void Start()
	{
		base.Start();
		AddAction(m_CancelButton, OnCancelSelected);
		m_CancelButton.m_OnEnterEvent = OnCancelEnter;
		m_CancelButton.m_OnExitEvent = OnCancelExit;
		SetupObject();
	}

	protected void OnDestroy()
	{
		if ((bool)m_Building.m_Engager)
		{
			m_Building.m_Engager.SendAction(new ActionInfo(ActionType.DisengageObject, m_Building.m_Engager.GetComponent<TileCoordObject>().m_TileCoord, m_Building));
		}
		HudManager.Instance.ActivateUIRollover(Activate: false, "", default(Vector3));
	}

	public virtual void SetBuilding(Building NewBuilding)
	{
		m_Building = NewBuilding;
	}

	protected void AddObjectToPanel(GameObject NewObject)
	{
		m_ObjectAdded = NewObject;
	}

	private void SetupObject()
	{
		Vector2 sizeDelta = m_ObjectAdded.GetComponent<Rollover>().m_Panel.GetComponent<RectTransform>().sizeDelta;
		Vector2 sizeDelta2 = m_Panel.GetComponent<RectTransform>().sizeDelta;
		sizeDelta2 -= m_ObjectArea.GetComponent<RectTransform>().sizeDelta;
		sizeDelta += sizeDelta2;
		GetComponent<RectTransform>().sizeDelta = sizeDelta;
		RectTransform component = m_ObjectAdded.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0.5f, 0.5f);
		component.anchorMax = new Vector2(0.5f, 0.5f);
		component.pivot = new Vector2(0.5f, 0.5f);
		m_ObjectAdded.transform.SetParent(m_ObjectArea.transform);
		m_ObjectAdded.transform.localPosition = new Vector3(0f, 0f);
	}

	protected void Disengage()
	{
		if ((bool)m_Building.m_Engager)
		{
			m_Building.m_Engager.SendAction(new ActionInfo(ActionType.DisengageObject, m_Building.m_Engager.GetComponent<TileCoordObject>().m_TileCoord, m_Building));
		}
	}

	public virtual void OnCancelSelected(BaseGadget NewGadget)
	{
		Disengage();
		GameStateManager.Instance.PopState();
	}

	public virtual void OnCancelEnter(BaseGadget NewGadget)
	{
	}

	public virtual void OnCancelExit(BaseGadget NewGadget)
	{
	}
}
