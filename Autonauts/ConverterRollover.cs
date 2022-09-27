using System.Collections.Generic;
using UnityEngine;

public class ConverterRollover : Rollover
{
	public static ConverterRollover Instance;

	[HideInInspector]
	public Converter m_Target;

	private ObjectType m_TargetType;

	private ObjectType m_Type;

	private int m_ResultsToCreate;

	private List<ConverterRequirementIngredient> m_Ingredients;

	private ConverterRequirementFuel m_Fuel;

	private BaseText m_Converter;

	private BaseImage m_Image;

	private BaseText m_Blueprint;

	private BaseText m_Description;

	private float m_DescriptionHeight;

	private BaseImage m_Divider;

	private Vector2 m_DividerPosition;

	private BaseImage m_StageDivider;

	private Vector2 m_StageDividerPosition;

	private BaseText m_StageText;

	private Vector2 m_StageTextPosition;

	private PanelBacking m_WallFloor;

	private BaseImage m_WallFloorImage;

	private PowerValue m_PowerValue;

	public bool m_MissingFlashOn;

	private float m_MissingTimer;

	protected new void Awake()
	{
		base.Awake();
		Instance = this;
		m_Target = null;
		m_TargetType = ObjectTypeList.m_Total;
		m_Type = ObjectTypeList.m_Total;
		m_Ingredients = new List<ConverterRequirementIngredient>();
		m_Converter = m_Panel.transform.Find("TitleBar/ConverterName").GetComponent<BaseText>();
		m_Blueprint = m_Panel.transform.Find("Title").GetComponent<BaseText>();
		m_Image = m_Panel.transform.Find("ObjectBox").Find("ObjectImage").GetComponent<BaseImage>();
		m_Description = m_Panel.transform.Find("Description").GetComponent<BaseText>();
		m_DescriptionHeight = m_Description.GetHeight();
		m_Divider = m_Panel.transform.Find("Divider").GetComponent<BaseImage>();
		m_DividerPosition = m_Divider.GetComponent<RectTransform>().anchoredPosition;
		m_StageDivider = m_Panel.transform.Find("StageDivider").GetComponent<BaseImage>();
		m_StageDividerPosition = m_StageDivider.GetComponent<RectTransform>().anchoredPosition;
		m_StageText = m_Panel.transform.Find("StageText").GetComponent<BaseText>();
		m_StageTextPosition = m_StageText.GetComponent<RectTransform>().anchoredPosition;
		m_WallFloor = m_Panel.transform.Find("WallFloor").GetComponent<PanelBacking>();
		m_WallFloorImage = m_WallFloor.transform.Find("Image").GetComponent<BaseImage>();
		m_PowerValue = m_Panel.transform.Find("PowerValue").GetComponent<PowerValue>();
		m_Fuel = null;
		Hide();
	}

	private void UpdateWallFloor()
	{
		if ((bool)m_Target.m_WallFloorIcon)
		{
			m_WallFloor.gameObject.SetActive(m_Target.m_WallFloorIcon.gameObject.activeSelf);
			m_WallFloorImage.SetSprite(m_Target.m_WallFloorIcon.GetSprite());
			m_WallFloorImage.SetColour(m_Target.m_WallFloorIcon.GetColour());
		}
		else
		{
			m_WallFloor.gameObject.SetActive(value: false);
		}
	}

	private void UpdatePowerValue()
	{
		if ((bool)m_Target && m_Target.m_LinkedSystem != null)
		{
			m_PowerValue.gameObject.SetActive(value: true);
			int variableAsInt = VariableManager.Instance.GetVariableAsInt(m_Target.m_TypeIdentifier, "LinkedEnergy", CheckValid: false);
			m_PowerValue.SetValue(variableAsInt);
		}
		else
		{
			m_PowerValue.gameObject.SetActive(value: false);
		}
	}

	protected override void UpdateTarget()
	{
		UpdatePowerValue();
		if (m_Target == null)
		{
			m_Converter.SetText("");
			return;
		}
		string humanReadableName = m_Target.GetHumanReadableName();
		m_Converter.SetText(humanReadableName);
		if (GetIsTargetTranscend())
		{
			m_Ingredients[0].SetRequired(FolkManager.Instance.GetTranscended(), FolkManager.Instance.GetTranscendTarget());
		}
		else
		{
			int num = m_Target.m_ResultsToCreate;
			if (m_TargetType != ObjectTypeList.m_Total)
			{
				num = m_Target.GetResultIndex(m_TargetType);
			}
			if (m_Target.m_Requirements.Count > num)
			{
				List<IngredientRequirement> list = m_Target.m_Requirements[num];
				if (list != null)
				{
					for (int i = 0; i < m_Ingredients.Count; i++)
					{
						int ingredientCount = m_Target.GetIngredientCount(list[i].m_Type);
						int count = list[i].m_Count;
						m_Ingredients[i].SetRequired(ingredientCount, count);
					}
				}
			}
		}
		if ((bool)m_Fuel)
		{
			Fueler component = m_Target.GetComponent<Fueler>();
			m_Fuel.UpdateFuel(component.GetFuelPercent(), (int)component.m_Fuel);
		}
		if (m_Target.m_TypeIdentifier == ObjectType.ConverterFoundation && m_StageText.GetActive())
		{
			int num2 = m_Target.GetComponent<ConverterFoundation>().m_Stage + 1;
			int numStages = m_Target.GetComponent<ConverterFoundation>().m_NumStages;
			string text = TextManager.Instance.Get("ConverterRolloverStage", num2.ToString(), numStages.ToString());
			m_StageText.SetText(text);
		}
		UpdateWallFloor();
	}

	public void SetConverterTarget(Converter Target)
	{
		if (!(Target != m_Target) && m_Type == ObjectTypeList.m_Total)
		{
			return;
		}
		TabQuests.Instance.ClearMissingIngredients();
		foreach (ConverterRequirementIngredient ingredient in m_Ingredients)
		{
			Object.Destroy(ingredient.gameObject);
		}
		m_Ingredients.Clear();
		if ((bool)m_Fuel)
		{
			Object.Destroy(m_Fuel.gameObject);
		}
		m_Fuel = null;
		m_Target = Target;
		m_TargetType = ObjectTypeList.m_Total;
		m_Type = ObjectTypeList.m_Total;
		if ((bool)m_Target)
		{
			CreateIngredients();
		}
	}

	public void SetConverterTarget(Converter Target, ObjectType NewType)
	{
		if (!(Target != m_Target) && m_Type == ObjectTypeList.m_Total)
		{
			return;
		}
		TabQuests.Instance.ClearMissingIngredients();
		foreach (ConverterRequirementIngredient ingredient in m_Ingredients)
		{
			Object.Destroy(ingredient.gameObject);
		}
		m_Ingredients.Clear();
		if ((bool)m_Fuel)
		{
			Object.Destroy(m_Fuel.gameObject);
		}
		m_Fuel = null;
		m_Target = Target;
		m_TargetType = NewType;
		m_Type = ObjectTypeList.m_Total;
		if ((bool)m_Target)
		{
			CreateIngredients();
		}
	}

	public void SetConverterTarget(ObjectType NewType)
	{
		if (NewType == m_Type && !(m_Target != null))
		{
			return;
		}
		TabQuests.Instance.ClearMissingIngredients();
		foreach (ConverterRequirementIngredient ingredient in m_Ingredients)
		{
			Object.Destroy(ingredient.gameObject);
		}
		m_Ingredients.Clear();
		if ((bool)m_Fuel)
		{
			Object.Destroy(m_Fuel.gameObject);
		}
		m_Fuel = null;
		m_Type = NewType;
		m_Target = null;
		m_TargetType = ObjectTypeList.m_Total;
		if (m_Type != ObjectTypeList.m_Total)
		{
			CreateIngredients();
		}
		m_WallFloor.gameObject.SetActive(value: false);
	}

	private bool GetShowFueler(ObjectType NewType)
	{
		if (m_Target != null && (bool)m_Target.GetComponent<Fueler>() && NewType != ObjectType.Charcoal)
		{
			return true;
		}
		return false;
	}

	private bool GetShowMultiStage()
	{
		if (m_Target == null)
		{
			return false;
		}
		if (m_Target.m_TypeIdentifier != ObjectType.ConverterFoundation)
		{
			return false;
		}
		if (m_Target.GetComponent<ConverterFoundation>().m_NumStages == 1)
		{
			return false;
		}
		return true;
	}

	private void CreateFuelBar()
	{
		Transform parent = m_Panel.transform.Find("Contents");
		ConverterRequirementFuel component = Object.Instantiate((GameObject)Resources.Load("Prefabs/Hud/Rollovers/ConverterRequirementFuel", typeof(GameObject)), new Vector3(0f, 0f, 0f), Quaternion.identity, parent).GetComponent<ConverterRequirementFuel>();
		component.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 20f);
		Fueler component2 = m_Target.GetComponent<Fueler>();
		component.SetTier(component2.m_Tier);
		m_Fuel = component;
	}

	private bool GetIsTargetTranscend()
	{
		if (m_TargetType == ObjectType.TranscendBuilding || ((bool)m_Target && m_Target.m_TypeIdentifier == ObjectType.TranscendBuilding))
		{
			return true;
		}
		return false;
	}

	private List<IngredientRequirement> GetRequirements()
	{
		List<IngredientRequirement> list;
		if (GetIsTargetTranscend())
		{
			list = new List<IngredientRequirement>();
			IngredientRequirement item = new IngredientRequirement(ObjectType.Folk, FolkManager.Instance.GetTranscendLeft());
			list.Add(item);
		}
		else if (m_Target != null)
		{
			int index = m_Target.m_ResultsToCreate;
			if (m_TargetType != ObjectTypeList.m_Total)
			{
				index = m_Target.GetResultIndex(m_TargetType);
			}
			list = m_Target.m_Requirements[index];
		}
		else
		{
			list = new List<IngredientRequirement>();
			IngredientRequirement[] ingredientsFromIdentifier = ObjectTypeList.Instance.GetIngredientsFromIdentifier(m_Type);
			foreach (IngredientRequirement item2 in ingredientsFromIdentifier)
			{
				list.Add(item2);
			}
		}
		return list;
	}

	private void CreateIngredients()
	{
		ObjectType total = ObjectTypeList.m_Total;
		bool flag = true;
		if (GetIsTargetTranscend())
		{
			total = ObjectType.TranscendBuilding;
		}
		else if (m_Target != null)
		{
			int num = m_Target.m_ResultsToCreate;
			if (m_TargetType != ObjectTypeList.m_Total)
			{
				num = m_Target.GetResultIndex(m_TargetType);
			}
			if (num >= m_Target.m_Results.Count)
			{
				return;
			}
			if ((bool)m_Target.GetComponent<ConverterFoundation>())
			{
				total = m_Target.GetComponent<ConverterFoundation>().m_NewBuilding.m_TypeIdentifier;
			}
			else
			{
				total = m_Target.m_Results[num][0].m_Type;
				if (total == ObjectTypeList.m_Total)
				{
					total = m_Target.m_TypeIdentifier;
					flag = false;
				}
			}
		}
		else
		{
			total = m_Type;
		}
		if (total != ObjectTypeList.m_Total)
		{
			string text = ObjectTypeList.Instance.GetSaveNameFromIdentifier(total);
			if (total == ObjectType.Folk)
			{
				text += "New";
			}
			string text2 = TextManager.Instance.Get(text);
			if (m_Target != null)
			{
				int num2 = m_Target.m_ResultsToCreate;
				if (m_TargetType != ObjectTypeList.m_Total)
				{
					num2 = m_Target.GetResultIndex(m_TargetType);
				}
				int count = m_Target.m_Results[num2][0].m_Count;
				if (count > 1)
				{
					text2 = TextManager.Instance.Get("ConverterRolloverMultiple", count.ToString(), text2);
				}
				m_ResultsToCreate = num2;
			}
			m_Blueprint.SetText(text2);
			string descriptionFromIdentifier = ObjectTypeList.Instance.GetDescriptionFromIdentifier(total);
			m_Description.SetText(descriptionFromIdentifier);
			if (m_Target != null && m_Target.m_TypeIdentifier == ObjectType.ConverterFoundation && m_Target.GetComponent<ConverterFoundation>().m_NewBuilding.m_BuildingToUpgradeFrom != ObjectTypeList.m_Total)
			{
				m_Image.SetSprite("Buttons/UpgradeButton");
			}
			else
			{
				Sprite icon = IconManager.Instance.GetIcon(total);
				m_Image.SetSprite(icon);
			}
			List<IngredientRequirement> requirements = GetRequirements();
			bool showFueler = GetShowFueler(total);
			bool showMultiStage = GetShowMultiStage();
			bool num3 = descriptionFromIdentifier.Length > 40;
			float num4 = 0f;
			if (num3)
			{
				num4 = 40f;
			}
			m_Divider.GetComponent<RectTransform>().anchoredPosition = m_DividerPosition - new Vector2(0f, num4);
			m_StageDivider.GetComponent<RectTransform>().anchoredPosition = m_StageDividerPosition - new Vector2(0f, num4);
			m_StageText.GetComponent<RectTransform>().anchoredPosition = m_StageTextPosition - new Vector2(0f, num4);
			m_Description.SetHeight(m_DescriptionHeight + num4);
			float num5 = 125f;
			float num6 = 27f;
			float num7 = num5 + num6 * (float)requirements.Count;
			float num8 = 40f;
			if (showFueler)
			{
				num7 += num8;
			}
			if (showMultiStage)
			{
				num7 += 40f;
			}
			num7 += num4;
			m_Panel.SetActive(Active: true);
			m_Panel.SetHeight(num7);
			BaseImage component = m_Panel.transform.Find("IngredientsPanel").GetComponent<BaseImage>();
			if (flag)
			{
				Transform parent = m_Panel.transform.Find("Contents");
				GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Rollovers/ConverterRequirementIngredient", typeof(GameObject));
				float num9 = 0f - num5;
				if (showMultiStage)
				{
					num9 -= 40f;
				}
				num9 -= num4;
				List<ObjectType> list = new List<ObjectType>();
				foreach (IngredientRequirement item in requirements)
				{
					ConverterRequirementIngredient component2 = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, parent).GetComponent<ConverterRequirementIngredient>();
					component2.SetIngredient(item.m_Type, WhiteText: false);
					component2.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, num9);
					component2.SetRequired(0, item.m_Count, ShowCount: false);
					m_Ingredients.Add(component2);
					if (QuestManager.Instance.m_ObjectsLocked.ContainsKey(item.m_Type))
					{
						list.Add(item.m_Type);
						component2.SetLocked();
					}
					num9 -= num6;
				}
				TabQuests.Instance.SetMissingIngredients(list);
				float height = num6 * (float)m_Ingredients.Count + 15f;
				component.SetHeight(height);
				component.SetActive(Active: true);
				float num10 = 5f;
				if (showFueler)
				{
					num10 += num8;
				}
				Vector2 anchoredPosition = component.GetComponent<RectTransform>().anchoredPosition;
				anchoredPosition.y = num10;
				component.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
			}
			else
			{
				component.SetActive(Active: false);
			}
			if (showFueler)
			{
				CreateFuelBar();
			}
			m_StageText.SetActive(showMultiStage);
			m_StageDivider.SetActive(showMultiStage);
		}
		m_Panel.SetActive(Active: false);
		UpdateTarget();
	}

	protected override bool GetTargettingSomething()
	{
		if (!(m_Target != null))
		{
			return m_Type != ObjectTypeList.m_Total;
		}
		return true;
	}

	protected new void Update()
	{
		if (m_Target != null && m_TargetType == ObjectTypeList.m_Total && m_ResultsToCreate != m_Target.m_ResultsToCreate)
		{
			Converter target = m_Target;
			SetConverterTarget(null);
			SetConverterTarget(target);
		}
		base.Update();
		m_MissingTimer += TimeManager.Instance.m_NormalDelta;
		m_MissingFlashOn = (int)(m_MissingTimer * 60f) % 10 < 5;
	}
}
