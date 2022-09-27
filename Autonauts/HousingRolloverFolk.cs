using UnityEngine;

public class HousingRolloverFolk : MonoBehaviour
{
	private Folk m_Target;

	private GameObject m_RequirementParent;

	private BaseText m_Name;

	private FolkRolloverRequirement m_Food;

	private FolkRolloverRequirement m_Housing;

	private FolkRolloverRequirement m_Top;

	private FolkRolloverRequirement m_Happiness;

	private float m_Width;

	private void Awake()
	{
		m_Target = null;
		float num = -30f;
		float num2 = -25f;
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Rollovers/FolkRolloverRequirement", typeof(GameObject));
		m_RequirementParent = base.transform.Find("Requirements").gameObject;
		m_Name = base.transform.Find("Name").GetComponent<BaseText>();
		m_Food = Object.Instantiate(original, default(Vector3), Quaternion.identity, m_RequirementParent.transform).GetComponent<FolkRolloverRequirement>();
		m_Food.transform.localPosition = new Vector3(0f, num, 0f);
		num += num2;
		m_Housing = Object.Instantiate(original, default(Vector3), Quaternion.identity, m_RequirementParent.transform).GetComponent<FolkRolloverRequirement>();
		m_Housing.transform.localPosition = new Vector3(0f, num, 0f);
		num += num2;
		m_Top = Object.Instantiate(original, default(Vector3), Quaternion.identity, m_RequirementParent.transform).GetComponent<FolkRolloverRequirement>();
		m_Top.transform.localPosition = new Vector3(0f, num, 0f);
		num += num2;
		m_Happiness = Object.Instantiate(original, default(Vector3), Quaternion.identity, m_RequirementParent.transform).GetComponent<FolkRolloverRequirement>();
		m_Happiness.transform.localPosition = new Vector3(0f, num, 0f);
		SetTarget(null);
		m_Width = GetComponent<RectTransform>().rect.width;
	}

	private void UpdateRequirementSlider(FolkRolloverRequirement Slider, string ImageName, float Value, int Tier, int MinTier, int FinalTier, int BestTier)
	{
		Slider.SetImage(ImageName);
		Slider.SetValue(Value);
		bool valid = false;
		if (BestTier >= MinTier)
		{
			valid = true;
		}
		bool lowest = false;
		if (Tier < FinalTier)
		{
			lowest = true;
		}
		Slider.SetTier(Tier, valid, lowest);
	}

	private void UpdateValues()
	{
		int tier = m_Target.GetTier();
		int bestTier = 0;
		int minTier = 0;
		UpdateRequirementSlider(m_Food, "Icons/IconFood", m_Target.GetFood(), m_Target.GetFoodTier(), minTier, tier, bestTier);
		minTier = VariableManager.Instance.GetVariableAsInt("FirstHousingTier");
		UpdateRequirementSlider(m_Housing, "Icons/IconHousing", m_Target.GetHousing(), m_Target.GetHousingTier(), minTier, tier, bestTier);
		minTier = VariableManager.Instance.GetVariableAsInt("FirstTopTier");
		UpdateRequirementSlider(m_Top, "Icons/IconTop", m_Target.GetClothing(), m_Target.GetClothingTier(), minTier, tier, bestTier);
		minTier = VariableManager.Instance.GetVariableAsInt("FirstMedicineTier");
		UpdateRequirementSlider(m_Top, "Icons/IconMedicine", m_Target.GetMedicine(), m_Target.GetMedicineTier(), minTier, tier, bestTier);
		minTier = VariableManager.Instance.GetVariableAsInt("FirstEducationTier");
		UpdateRequirementSlider(m_Top, "Icons/IconEducation", m_Target.GetEducation(), m_Target.GetEducationTier(), minTier, tier, bestTier);
		minTier = VariableManager.Instance.GetVariableAsInt("FirstArtTier");
		UpdateRequirementSlider(m_Top, "Icons/IconArt", m_Target.GetArt(), m_Target.GetArtTier(), minTier, tier, bestTier);
		float num = 0f;
		if (m_Target.GetIsHappy())
		{
			num = 1f;
		}
		string text = "PopulationNeutral";
		if (num == 1f)
		{
			text = "PopulationVeryHappy";
		}
		UpdateRequirementSlider(m_Happiness, "PopulationStatus/" + text, num, tier, 0, tier, tier);
	}

	public void SetTarget(Folk Target)
	{
		m_Target = Target;
		float num = 0f;
		if ((bool)Target)
		{
			m_Name.SetText(Target.GetHumanReadableName());
			m_RequirementParent.gameObject.SetActive(value: true);
			UpdateValues();
			num = 130f;
		}
		else
		{
			m_Name.SetTextFromID("HousingRolloverEmpty");
			m_RequirementParent.gameObject.SetActive(value: false);
			num = 45f;
		}
		GetComponent<RectTransform>().sizeDelta = new Vector2(m_Width, num);
	}
}
