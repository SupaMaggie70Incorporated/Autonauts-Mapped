using UnityEngine;
using UnityEngine.UI;

public class BaseRawImage : BaseGadget
{
	private RawImage m_Image;

	private void CheckImage()
	{
		if (m_Image == null)
		{
			m_Image = GetComponent<RawImage>();
		}
	}

	public void SetTexture(Texture NewTexture)
	{
		CheckImage();
		m_Image.texture = NewTexture;
	}

	public Texture GetTexture()
	{
		CheckImage();
		return m_Image.texture;
	}

	public void SetColour(Color NewColour)
	{
		CheckImage();
		m_Image.color = NewColour;
	}
}
