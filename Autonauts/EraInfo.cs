using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EraInfo : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public enum Type
	{
		Population,
		Happiness,
		Food,
		Housing,
		Clothing,
		Total
	}

	private Type m_Type;

	private Image m_Image;

	private Slider m_Slider;

	private bool m_Active;

	public void SetType(Type NewType)
	{
		m_Type = NewType;
		m_Image = base.transform.Find("Image").GetComponent<Image>();
		m_Slider = base.transform.Find("Slider").GetComponent<Slider>();
		string text = (new string[5] { "Icons/Other/IconFolk", "PopulationStatus/PopulationVeryHappy", "Icons/Food/IconFood", "Icons/Buildings/IconHut", "Icons/Clothes/Tops/IconTop" })[(int)NewType];
		Sprite sprite = (Sprite)Resources.Load("Textures/Hud/" + text, typeof(Sprite));
		m_Image.sprite = sprite;
		m_Active = false;
	}

	private void UpdateRequirementSlider(bool Active, int Population, int Value, float MinValue)
	{
		float num = (float)Value / (float)Population;
		if (num > 1f)
		{
			num = 1f;
		}
		if (Active)
		{
			m_Slider.gameObject.SetActive(value: true);
			m_Image.gameObject.SetActive(value: true);
			m_Slider.value = num;
			Color color = ((num >= MinValue) ? new Color(0f, 1f, 0f) : ((!(num >= MinValue * 0.5f)) ? new Color(1f, 0f, 0f) : new Color(1f, 0.5f, 0f)));
			m_Image = m_Slider.transform.Find("Fill Area").Find("Fill").GetComponent<Image>();
			m_Image.color = color;
		}
		else
		{
			m_Slider.gameObject.SetActive(value: false);
			m_Image.gameObject.SetActive(value: false);
		}
		m_Active = Active;
	}

	private void UpdateValues()
	{
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (m_Active)
		{
			string[] array = new string[5] { "TabEraPopulation", "TabEraHappiness", "TabEraFood", "TabEraHousing", "TabEraClothing" };
			string target = TextManager.Instance.Get(array[(int)m_Type]);
			HudManager.Instance.ActivateUIRollover(Activate: true, target, default(Vector3));
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (m_Active)
		{
			HudManager.Instance.ActivateUIRollover(Activate: false, "", default(Vector3));
		}
	}

	private void Update()
	{
		UpdateValues();
	}
}
