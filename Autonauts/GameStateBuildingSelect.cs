using UnityEngine;

public class GameStateBuildingSelect : GameStateBase
{
	[HideInInspector]
	public GameObject m_Menu;

	public Building m_Building;

	protected new void Awake()
	{
		base.Awake();
		HudManager.Instance.HideRollovers();
		HudManager.Instance.SetHudButtonsActive(Active: false);
		CameraManager.Instance.SetPausedDOFEffect();
	}

	protected new void OnDestroy()
	{
		base.OnDestroy();
		Object.Destroy(m_Menu.gameObject);
		HudManager.Instance.SetHudButtonsActive(Active: true);
		CameraManager.Instance.RestorePausedDOFEffect();
	}

	public void SetBuilding(Building NewBuilding)
	{
		m_Building = NewBuilding;
		string text = "GeneralCancelSelect";
		if ((bool)NewBuilding.GetComponent<Converter>())
		{
			text = ((NewBuilding.GetComponent<Converter>().m_Ingredients.Count != 0 || NewBuilding.m_TypeIdentifier == ObjectType.ConverterFoundation) ? "ConverterCancel" : "ConverterSelect");
			QuestManager.Instance.AddEvent(QuestEvent.Type.EngageConverter, Bot: false, NewBuilding.m_TypeIdentifier, null);
		}
		else if ((bool)NewBuilding.GetComponent<ResearchStation>())
		{
			text = (NewBuilding.GetComponent<ResearchStation>().HasResearchStarted() ? "ResearchCancel" : "ResearchSelect");
		}
		else if ((bool)NewBuilding.GetComponent<AnimalStation>())
		{
			text = "AnimalStationCancel";
		}
		else if ((bool)NewBuilding.GetComponent<Storage>())
		{
			text = "StorageCancel";
		}
		else if ((bool)NewBuilding.GetComponent<BotServer>())
		{
			text = "BotServerSelect";
		}
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/BuildingSelect/" + text, typeof(GameObject));
		Transform menusRootTransform = HudManager.Instance.m_MenusRootTransform;
		m_Menu = Object.Instantiate(original, menusRootTransform);
		m_Menu.GetComponent<RectTransform>().anchoredPosition = default(Vector2);
		if (m_Menu.GetComponent<BuildingSelect>() != null)
		{
			m_Menu.GetComponent<BuildingSelect>().SetBuilding(NewBuilding);
		}
		if (m_Menu.GetComponent<BuildingCancel>() != null)
		{
			m_Menu.GetComponent<BuildingCancel>().SetBuilding(NewBuilding);
		}
	}

	public void CancelClicked()
	{
	}

	public override void UpdateState()
	{
		if (MyInputManager.m_Rewired.GetButtonDown("Quit"))
		{
			GameStateManager.Instance.PopState();
		}
	}
}
