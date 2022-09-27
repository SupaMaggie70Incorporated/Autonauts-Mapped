using UnityEngine;
using UnityEngine.UI;

public class BaseImage : BaseGadget
{
	private Image m_Image;

	private void CheckImage()
	{
		if (m_Image == null)
		{
			m_Image = GetComponent<Image>();
		}
	}

	public void SetSprite(string Name)
	{
		CheckImage();
		Sprite sprite = (Sprite)Resources.Load("Textures/Hud/" + Name, typeof(Sprite));
		m_Image.sprite = sprite;
	}

	public void SetSprite(Sprite NewSprite)
	{
		CheckImage();
		m_Image.sprite = NewSprite;
	}

	public void SetColour(Color NewColour)
	{
		CheckImage();
		m_Image.color = NewColour;
	}
}
