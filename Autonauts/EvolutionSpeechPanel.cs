using System.Collections.Generic;
using UnityEngine;

public class EvolutionSpeechPanel : CeremonySpeech
{
	private List<GameObject> m_Requirements;

	private void AddRequirement(string RequirementTierName, GameObject Prefab, int Level, ObjectCategory NewCategory)
	{
		int num = 0;
		if (RequirementTierName != "")
		{
			num = VariableManager.Instance.GetVariableAsInt(RequirementTierName);
		}
		if (Level >= num)
		{
			GameObject gameObject = Object.Instantiate(Prefab, base.transform).gameObject;
			gameObject.SetActive(value: true);
			BaseImage component = gameObject.transform.Find("Icon").GetComponent<BaseImage>();
			string sprite = "ObjectCategories/ObjectCategory" + NewCategory;
			component.SetSprite(sprite);
			gameObject.transform.Find("LevelHeart").GetComponent<LevelHeart>().SetValue(Level);
			m_Requirements.Add(gameObject);
		}
	}

	public void SetFolkLevel(int Level)
	{
		GameObject gameObject = base.transform.Find("DefaultRequirement").gameObject;
		gameObject.SetActive(value: false);
		m_Requirements = new List<GameObject>();
		AddRequirement("", gameObject, Level, ObjectCategory.Food);
		AddRequirement("FirstHousingTier", gameObject, Level, ObjectCategory.Buildings);
		AddRequirement("FirstTopTier", gameObject, Level, ObjectCategory.Clothing);
		AddRequirement("FirstToyTier", gameObject, Level, ObjectCategory.Leisure);
		AddRequirement("FirstMedicineTier", gameObject, Level, ObjectCategory.Medicine);
		AddRequirement("FirstEducationTier", gameObject, Level, ObjectCategory.Education);
		AddRequirement("FirstArtTier", gameObject, Level, ObjectCategory.Art);
		float num = 120f;
		float num2 = 0f - (float)(m_Requirements.Count - 1) * num * 0.5f;
		foreach (GameObject requirement in m_Requirements)
		{
			Vector3 localPosition = requirement.transform.localPosition;
			localPosition.x = num2;
			requirement.transform.localPosition = localPosition;
			num2 += num;
		}
	}
}
