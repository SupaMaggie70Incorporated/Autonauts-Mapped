using System.Collections.Generic;
using UnityEngine;

public class SpacePortRollover : Rollover
{
	public static SpacePortRollover Instance;

	[HideInInspector]
	public SpacePort m_Target;

	private OffworldMission m_TargetMission;

	private List<ConverterRequirementIngredient> m_Ingredients;

	private BaseText m_Title;

	private BaseImage m_Image;

	private BaseText m_Name;

	private BaseImage m_TicketImage;

	private BaseText m_Tickets;

	private BaseImage m_Divider;

	private Vector2 m_DividerPosition;

	private BaseImage m_IngredientsPanel;

	protected new void Awake()
	{
		base.Awake();
		Instance = this;
		m_Target = null;
		m_Ingredients = new List<ConverterRequirementIngredient>();
		m_Title = ObjectUtils.FindDeepChild(base.transform, "ConverterName").GetComponent<BaseText>();
		m_Name = m_Panel.transform.Find("Name").GetComponent<BaseText>();
		m_Image = m_Panel.transform.Find("ObjectBox/ObjectImage").GetComponent<BaseImage>();
		m_TicketImage = m_Panel.transform.Find("Tickets").GetComponent<BaseImage>();
		m_Tickets = m_Panel.transform.Find("Value").GetComponent<BaseText>();
		m_Divider = m_Panel.transform.Find("Divider").GetComponent<BaseImage>();
		m_DividerPosition = m_Divider.GetComponent<RectTransform>().anchoredPosition;
		m_IngredientsPanel = m_Panel.transform.Find("IngredientsPanel").GetComponent<BaseImage>();
		Hide();
	}

	protected override void UpdateTarget()
	{
		OffworldMission offworldMission = m_TargetMission;
		if (offworldMission == null)
		{
			offworldMission = OffworldMissionsManager.Instance.m_SelectedMission;
		}
		if (offworldMission != null)
		{
			List<IngredientRequirement> requirements = offworldMission.m_Requirements;
			List<int> progress = offworldMission.m_Progress;
			for (int i = 0; i < m_Ingredients.Count; i++)
			{
				int count = progress[i];
				int count2 = requirements[i].m_Count;
				m_Ingredients[i].SetRequired(count, count2);
			}
		}
	}

	public void SetTarget(SpacePort Target)
	{
		if (Target != m_Target)
		{
			ClearIngredients();
			m_Target = Target;
			m_TargetMission = null;
			if ((bool)m_Target)
			{
				m_Title.SetText(m_Target.GetHumanReadableName());
				SetupTarget(OffworldMissionsManager.Instance.m_SelectedMission);
			}
			else
			{
				m_Title.SetTextFromID("SpacePort");
			}
		}
	}

	public void SetTarget(OffworldMission NewMission)
	{
		if (NewMission != m_TargetMission)
		{
			ClearIngredients();
			m_Target = null;
			m_TargetMission = NewMission;
			m_Title.SetTextFromID("SpacePort");
			if (m_TargetMission != null)
			{
				SetupTarget(m_TargetMission);
			}
		}
	}

	private void SetNoMission()
	{
		m_Name.SetTextFromID("SpacePortRolloverNothing");
		m_TicketImage.SetActive(Active: false);
		m_Tickets.SetActive(Active: false);
		m_Image.SetSprite("SpacePort/MissionNone");
	}

	private void SetupTarget(OffworldMission NewMission)
	{
		if (NewMission == null)
		{
			SetNoMission();
			return;
		}
		m_Name.SetText(NewMission.GetName(Short: true));
		m_Image.SetSprite(NewMission.GetImage());
		List<IngredientRequirement> requirements = NewMission.m_Requirements;
		bool num = "".Length > 40;
		float num2 = 0f;
		if (num)
		{
			num2 = 40f;
		}
		m_Divider.GetComponent<RectTransform>().anchoredPosition = m_DividerPosition - new Vector2(0f, num2);
		m_Tickets.SetText(NewMission.GetTickets().ToString());
		m_Tickets.SetActive(Active: true);
		m_TicketImage.SetActive(Active: true);
		float num3 = 155f;
		float num4 = 27f;
		float num5 = num3 + num4 * (float)requirements.Count;
		num5 += num2;
		m_Panel.SetActive(Active: true);
		m_Panel.SetHeight(num5);
		CreateIngredients(0f - num3 - num2, num4, requirements);
		float height = num4 * (float)m_Ingredients.Count + 15f;
		m_IngredientsPanel.SetHeight(height);
		m_IngredientsPanel.SetActive(Active: true);
		float y = 5f;
		Vector2 anchoredPosition = m_IngredientsPanel.GetComponent<RectTransform>().anchoredPosition;
		anchoredPosition.y = y;
		m_IngredientsPanel.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
		m_Panel.SetActive(Active: false);
		UpdateTarget();
	}

	private void ClearIngredients()
	{
		foreach (ConverterRequirementIngredient ingredient in m_Ingredients)
		{
			Object.Destroy(ingredient.gameObject);
		}
		m_Ingredients.Clear();
	}

	private void CreateIngredients(float y, float Spacing, List<IngredientRequirement> Requirements)
	{
		Transform parent = m_Panel.transform.Find("Contents");
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Rollovers/ConverterRequirementIngredient", typeof(GameObject));
		new List<ObjectType>();
		foreach (IngredientRequirement Requirement in Requirements)
		{
			ConverterRequirementIngredient component = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, parent).GetComponent<ConverterRequirementIngredient>();
			component.SetIngredient(Requirement.m_Type, WhiteText: false);
			component.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, y);
			component.SetRequired(0, Requirement.m_Count, ShowCount: false);
			m_Ingredients.Add(component);
			y -= Spacing;
		}
	}

	protected override bool GetTargettingSomething()
	{
		if (!(m_Target != null))
		{
			return m_TargetMission != null;
		}
		return true;
	}
}
