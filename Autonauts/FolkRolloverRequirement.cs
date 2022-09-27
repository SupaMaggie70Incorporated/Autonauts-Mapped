using UnityEngine;
using UnityEngine.UI;

public class FolkRolloverRequirement : MonoBehaviour
{
	private Image m_Image;

	private Slider m_Slider;

	private Text m_Tier;

	private void Awake()
	{
		m_Image = base.transform.Find("Image").GetComponent<Image>();
		m_Slider = base.transform.Find("Slider").GetComponent<Slider>();
		m_Tier = base.transform.Find("Tier").GetComponent<Text>();
	}

	public void SetImage(string ImageName)
	{
		Sprite sprite = (Sprite)Resources.Load("Textures/Hud/" + ImageName, typeof(Sprite));
		m_Image.sprite = sprite;
	}

	public void SetTier(int Tier, bool Valid, bool Lowest)
	{
		m_Tier.text = TextManager.Instance.Get("FolkRolloverTier") + " " + (Tier + 1);
		Color color = new Color(0f, 0f, 0f, 1f);
		if (!Valid)
		{
			color.a = 0.5f;
		}
		else if (Lowest)
		{
			color = new Color(1f, 0f, 0f, 1f);
		}
		m_Tier.color = color;
	}

	public void SetValue(float Value)
	{
		m_Slider.value = Value;
	}
}
