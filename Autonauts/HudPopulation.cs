using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HudPopulation : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	private Image m_StatusImage;

	private Text m_PopulationCount;

	private void Start()
	{
		m_StatusImage = base.transform.Find("StatusImage").GetComponent<Image>();
		m_PopulationCount = base.transform.Find("PopulationCount").GetComponent<Text>();
		UpdatePopulation();
	}

	private void UpdatePopulation()
	{
		int num = 0;
		float num2 = 0f;
		Dictionary<BaseClass, int> collection = CollectionManager.Instance.GetCollection("Folk");
		if (collection != null)
		{
			foreach (KeyValuePair<BaseClass, int> item in collection)
			{
				Folk component = item.Key.GetComponent<Folk>();
				num++;
				num2 += component.m_Happiness;
			}
		}
		if (num == 0)
		{
			m_StatusImage.gameObject.SetActive(value: false);
			m_PopulationCount.gameObject.SetActive(value: false);
			return;
		}
		m_StatusImage.gameObject.SetActive(value: true);
		m_PopulationCount.gameObject.SetActive(value: true);
		m_PopulationCount.text = num.ToString();
		string text = ((num != 0) ? GetHappinessIconName(num2 / (float)num) : "PopulationNeutral");
		Sprite sprite = (Sprite)Resources.Load("Textures/Hud/PopulationStatus/" + text, typeof(Sprite));
		m_StatusImage.sprite = sprite;
	}

	public static string GetHappinessIconName(float Happiness)
	{
		string result = "PopulationVerySad";
		if (Happiness == 1f)
		{
			result = "PopulationVeryHappy";
		}
		else if (Happiness >= 0.75f)
		{
			result = "PopulationHappy";
		}
		else if (Happiness >= 0.5f)
		{
			result = "PopulationNeutral";
		}
		else if (Happiness >= 0.25f)
		{
			result = "PopulationSad";
		}
		return result;
	}

	private void Update()
	{
		UpdatePopulation();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		HudManager.Instance.ActivateFolkRollover(Activate: true);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		HudManager.Instance.ActivateFolkRollover(Activate: false);
	}
}
