using UnityEngine;
using UnityEngine.UI;

public class BaseButtonImage : BaseButton
{
	protected Image m_Image;

	private void CheckImage()
	{
		if (m_Image == null)
		{
			m_Image = base.transform.Find("Image").GetComponent<Image>();
		}
	}

	public void SetImageEnabled(bool Enabled)
	{
		CheckImage();
		m_Image.gameObject.SetActive(Enabled);
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

	public void SetImageColour(Color NewColour)
	{
		CheckImage();
		m_Image.color = NewColour;
	}

	private void UpdateImage()
	{
		CheckImage();
		Color oldColour = new Color(1f, 1f, 1f, 1f);
		if (m_Locked)
		{
			oldColour = new Color(0.25f, 0.25f, 0.25f, 1f);
		}
		oldColour = GetInteractableColour(m_Interactable, oldColour);
		m_Image.color = oldColour;
	}

	public override void BaseSetInteractable(bool Interactable)
	{
		base.BaseSetInteractable(Interactable);
		CheckImage();
		UpdateImage();
	}

	public override void BaseSetIndicated(bool Indicated)
	{
		base.BaseSetIndicated(Indicated);
		CheckImage();
		float indicatedScale = GetIndicatedScale();
		m_Image.transform.localScale = new Vector3(indicatedScale, indicatedScale, indicatedScale);
	}

	public override void SetLocked(bool Locked)
	{
		base.SetLocked(Locked);
		UpdateImage();
	}
}
