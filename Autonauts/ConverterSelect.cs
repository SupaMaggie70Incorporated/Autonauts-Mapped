using System.Collections.Generic;
using UnityEngine;

public class ConverterSelect : BuildingSelect
{
	private Converter m_Converter;

	private BasePanelOptions m_Panel;

	private ObjectSelectScrollView m_ScrollView;

	private GameObject m_WallsFloors;

	private BaseText m_WallsFloorsText;

	private BaseImage m_DefaultWallsFloorsIcon;

	private List<BaseImage> m_WallsFloorsIcons;

	protected List<ObjectType> m_ObjectTypes;

	private List<bool> m_IsNew;

	private int m_CurrentlySelected;

	protected BaseButtonImage m_UpgradeButton;

	private new void Awake()
	{
		base.Awake();
		m_Panel = base.transform.Find("BasePanelOptions").GetComponent<BasePanelOptions>();
		m_ScrollView = base.transform.Find("ObjectSelectScrollView").GetComponent<ObjectSelectScrollView>();
		m_UpgradeButton = base.transform.Find("UpgradeButton").GetComponent<BaseButtonImage>();
		m_WallsFloors = base.transform.Find("WallsFloors").gameObject;
		m_WallsFloors.gameObject.SetActive(value: false);
		m_WallsFloorsText = m_WallsFloors.transform.Find("WallsFloorsText").GetComponent<BaseText>();
		m_DefaultWallsFloorsIcon = m_WallsFloors.transform.Find("Icon").GetComponent<BaseImage>();
		m_DefaultWallsFloorsIcon.SetActive(Active: false);
		m_WallsFloorsIcons = new List<BaseImage>();
	}

	protected new void Start()
	{
		base.Start();
		m_UpgradeButton.SetAction(OnUpgradeClicked, m_UpgradeButton);
		BaseButtonBack component = m_WallsFloors.transform.Find("Panel/BackButton").GetComponent<BaseButtonBack>();
		component.SetAction(OnBackClicked, component);
	}

	private void OnDestroy()
	{
		if ((bool)m_Building.m_Engager)
		{
			m_Building.m_Engager.SendAction(new ActionInfo(ActionType.DisengageObject, m_Building.m_Engager.GetComponent<TileCoordObject>().m_TileCoord, m_Building));
		}
		HudManager.Instance.ActivateUIRollover(Activate: false, "", default(Vector3));
	}

	protected void SetupBlueprintButtons(List<ObjectType> NewObjectTypes, List<bool> IsNew, int CurrentlySelected)
	{
		m_ObjectTypes = NewObjectTypes;
		m_IsNew = IsNew;
		m_CurrentlySelected = CurrentlySelected;
		int num = 5;
		if (m_ObjectTypes.Count >= 20)
		{
			num = 7;
		}
		if (m_ObjectTypes.Count >= 25)
		{
			num = 8;
		}
		if (m_ObjectTypes.Count >= 30)
		{
			num = 9;
		}
		int num2 = m_ObjectTypes.Count / num + 1;
		if (m_ObjectTypes.Count % num == 0)
		{
			num2--;
		}
		float x = (float)num * m_ScrollView.m_ButtonList.m_HorizontalSpacing + 40f;
		float num3 = (float)num2 * m_ScrollView.m_ButtonList.m_VerticalSpacing;
		float num4 = num3 + 165f;
		if (m_WallsFloors.activeSelf && num4 < 400f)
		{
			num4 = 400f;
		}
		if (num4 > 600f)
		{
			num4 = 600f;
		}
		GetComponent<RectTransform>().sizeDelta = new Vector2(x, num4);
		m_ScrollView.SetScrollSize(num3);
		int count = m_ObjectTypes.Count;
		m_ScrollView.m_ButtonList.m_ObjectCount = count;
		m_ScrollView.m_ButtonList.m_CreateObjectCallback = OnCreateBlueprintButton;
		m_ScrollView.m_ButtonList.m_ButtonsPerRow = num;
		m_ScrollView.m_ButtonList.CreateButtons();
	}

	public virtual void OnCreateBlueprintButton(GameObject NewGadget, int Index)
	{
		StandardButtonBlueprint component = NewGadget.GetComponent<StandardButtonBlueprint>();
		ObjectType newType = ObjectTypeList.m_Total;
		if (Index < m_ObjectTypes.Count)
		{
			newType = m_ObjectTypes[Index];
		}
		component.SetObjectType(newType, m_Converter);
		if (m_CurrentlySelected == Index)
		{
			component.SetSelected(Selected: true);
			m_ScrollView.PopGadgetOnTop(component);
		}
		else
		{
			component.SetSelected(Selected: false);
		}
		if (Index < m_ObjectTypes.Count && m_IsNew[Index])
		{
			component.SetNew(New: true);
		}
		LevelHeart component2 = component.transform.Find("LevelHeart").GetComponent<LevelHeart>();
		component2.SetActive(BaseClass.GetHasTierFromType(newType));
		int tierFromType = BaseClass.GetTierFromType(newType);
		component2.SetValue(tierFromType);
		AddAction(component, OnBlueprintClicked);
	}

	protected void BaseSetBuilding(Building NewBuilding)
	{
		base.SetBuilding(NewBuilding);
		if (NewBuilding.m_TypeIdentifier == ObjectType.ConverterFoundation)
		{
			m_Panel.SetTitleTextFromID("ConverterSelectTitleBlueprint");
		}
		else
		{
			m_Panel.SetTitleText(NewBuilding.GetHumanReadableName());
		}
	}

	private void CreateNeededText(List<string> Names)
	{
		string text = "";
		for (int i = 0; i < Names.Count; i++)
		{
			text += TextManager.Instance.Get("ConverterSelect" + Names[i]);
			if (i != Names.Count - 1)
			{
				text += "/";
			}
		}
		string text2 = TextManager.Instance.Get("ConverterSelectMissing", text);
		m_WallsFloorsText.SetText(text2);
	}

	private void CreateNeededIcons(List<string> Names)
	{
		m_WallsFloorsIcons.Clear();
		float num = 120f;
		float num2 = (0f - (float)(Names.Count - 1) * num) / 2f;
		float y = m_DefaultWallsFloorsIcon.GetComponent<RectTransform>().anchoredPosition.y;
		Transform parent = m_DefaultWallsFloorsIcon.transform.parent;
		foreach (string Name in Names)
		{
			BaseImage baseImage = Object.Instantiate(m_DefaultWallsFloorsIcon, parent);
			baseImage.SetActive(Active: true);
			baseImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(num2, y);
			baseImage.SetSprite(Name);
			m_WallsFloorsIcons.Add(baseImage);
			num2 += num;
		}
	}

	private void CheckWallsFloorsRequired()
	{
		if (m_Converter.m_WallMissing || m_Converter.m_FloorMissing || m_Converter.m_PowerMissing)
		{
			List<string> list = new List<string>();
			if (m_Converter.m_WallMissing)
			{
				list.Add("WallNeeded");
			}
			if (m_Converter.m_FloorMissing)
			{
				list.Add("FloorNeeded");
			}
			if (m_Converter.m_PowerMissing)
			{
				list.Add("PowerNeeded");
			}
			CreateNeededText(list);
			CreateNeededIcons(list);
			m_WallsFloors.gameObject.SetActive(value: true);
			m_ScrollView.m_ButtonList.SetAllInteractable(Interactable: false);
		}
		else
		{
			m_WallsFloors.gameObject.SetActive(value: false);
		}
	}

	public override void SetBuilding(Building NewBuilding)
	{
		BaseSetBuilding(NewBuilding);
		m_Converter = NewBuilding.GetComponent<Converter>();
		CheckWallsFloorsRequired();
		ObjectType type = m_Converter.m_Results[m_Converter.m_ResultsToCreate][0].m_Type;
		List<IngredientRequirement> unlockedResults = m_Converter.GetUnlockedResults();
		List<ObjectType> list = new List<ObjectType>();
		List<bool> list2 = new List<bool>();
		int currentlySelected = 0;
		foreach (IngredientRequirement item in unlockedResults)
		{
			if (item.m_Type == type)
			{
				currentlySelected = list.Count;
			}
			list.Add(item.m_Type);
			list2.Add(NewIconManager.Instance.IsObjectNew(item.m_Type));
		}
		SetupBlueprintButtons(list, list2, currentlySelected);
		NewIconManager.Instance.ConverterSeen(m_Converter);
		ObjectType buildingToUpgradeTo = m_Converter.m_BuildingToUpgradeTo;
		if (buildingToUpgradeTo != ObjectTypeList.m_Total && !QuestManager.Instance.GetIsBuildingLocked(buildingToUpgradeTo))
		{
			m_UpgradeButton.SetActive(Active: true);
			NewIconManager.Instance.IsObjectNew(buildingToUpgradeTo);
		}
		else
		{
			m_UpgradeButton.SetActive(Active: false);
		}
	}

	public virtual void OnBlueprintClicked(BaseGadget NewGadget)
	{
		if (!(m_Converter.m_Engager == null))
		{
			StandardButtonBlueprint component = NewGadget.GetComponent<StandardButtonBlueprint>();
			string saveNameFromIdentifier = ObjectTypeList.Instance.GetSaveNameFromIdentifier(component.m_Type);
			m_Converter.m_Engager.SendAction(new ActionInfo(ActionType.SetValue, m_Converter.m_Engager.GetComponent<TileCoordObject>().m_TileCoord, m_Converter, Converter.m_ResultsToCreateName, saveNameFromIdentifier));
			m_Converter.m_Engager.SendAction(new ActionInfo(ActionType.DisengageObject, m_Converter.m_Engager.GetComponent<TileCoordObject>().m_TileCoord, m_Converter));
			GameStateManager.Instance.PopState();
			ObjectType type = m_Converter.m_Results[m_Converter.m_ResultsToCreate][0].m_Type;
			QuestManager.Instance.AddEvent(QuestEvent.Type.ConverterSelectObject, Bot: false, type, null);
		}
	}

	public virtual void OnUpgradeClicked(BaseGadget NewGadget)
	{
		m_Converter.m_Engager.SendAction(new ActionInfo(ActionType.DisengageObject, m_Converter.m_Engager.GetComponent<TileCoordObject>().m_TileCoord, m_Converter));
		m_Converter.BeginUpgrade();
		GameStateManager.Instance.PopState();
	}

	public GameObject GetButtonFromObjectType(ObjectType NewType)
	{
		if (m_ObjectTypes.Contains(NewType))
		{
			int index = m_ObjectTypes.IndexOf(NewType);
			return m_ScrollView.m_ButtonList.m_Buttons[index].gameObject;
		}
		return null;
	}
}
