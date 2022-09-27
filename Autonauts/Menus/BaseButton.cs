using UnityEngine;
using UnityEngine.UI;

public class BaseButton : BaseGadget
{
	protected PanelBacking m_Backing;

	protected Image m_Shadow;

	protected Image m_Border;

	protected bool m_Locked;

	protected void CheckGadgets()
	{
		if (m_Backing == null)
		{
			m_Shadow = base.transform.Find("Shadow").GetComponent<Image>();
			m_Backing = base.transform.Find("Back").GetComponent<PanelBacking>();
			m_Border = base.transform.Find("Border").GetComponent<Image>();
			m_Border.gameObject.SetActive(value: false);
		}
	}

	public void SetBorderSprite(string Name)
	{
		Sprite sprite = (Sprite)Resources.Load("Textures/Hud/" + Name, typeof(Sprite));
		CheckGadgets();
		if ((bool)sprite)
		{
			m_Border.sprite = sprite;
		}
		else
		{
			m_Border.sprite = null;
		}
	}

	public void SetShadowSprite(string Name)
	{
		Sprite sprite = (Sprite)Resources.Load("Textures/Hud/" + Name, typeof(Sprite));
		CheckGadgets();
		if ((bool)sprite)
		{
			m_Shadow.sprite = sprite;
		}
		else
		{
			m_Shadow.sprite = null;
		}
	}

	public void SetBackingSprite(string Name)
	{
		Sprite sprite = (Sprite)Resources.Load("Textures/Hud/" + Name, typeof(Sprite));
		CheckGadgets();
		if ((bool)sprite)
		{
			m_Backing.m_Texture = sprite.texture;
		}
		else
		{
			m_Backing.m_Texture = null;
		}
		m_Backing.SetMaterialDirty();
	}

	public void SetBackingSprite(Sprite NewSprite)
	{
		base.transform.Find("Back").GetComponent<PanelBacking>();
		CheckGadgets();
		m_Backing.m_Texture = NewSprite.texture;
		m_Backing.SetMaterialDirty();
	}

	public void SetBackingColour(Color NewColour)
	{
		base.transform.Find("Back").GetComponent<PanelBacking>();
		CheckGadgets();
		m_Backing.SetColour(NewColour);
	}

	protected Color GetInteractableColour(bool Interactable, Color OldColour)
	{
		Color result = OldColour;
		result.a = 1f;
		if (!Interactable)
		{
			result.a = 0.5f;
		}
		return result;
	}

	private void UpdateShadow()
	{
	}

	public virtual void BaseSetInteractable(bool Interactable)
	{
		CheckGadgets();
		Color interactableColour = GetInteractableColour(Interactable, m_Backing.GetColour());
		m_Backing.SetColour(interactableColour);
		UpdateShadow();
	}

	private void UpdateBackground()
	{
		string text = "ColourGradientNormalButton";
		if (m_Selected)
		{
			text = "ColourGradientSelectedButton";
		}
		else if (m_Locked)
		{
			text = "ColourGradientLockedButton";
		}
		m_Backing.SetBackingGradient("Buttons/" + text);
	}

	public virtual void BaseSetSelected(bool Selected)
	{
		CheckGadgets();
		UpdateBackground();
		UpdateShadow();
	}

	protected float GetIndicatedScale()
	{
		float result = 1f;
		if (m_Indicated)
		{
			result = 1.2f;
		}
		return result;
	}

	public virtual void BaseSetIndicated(bool Indicated)
	{
		CheckGadgets();
		float indicatedScale = GetIndicatedScale();
		m_Backing.transform.localScale = new Vector3(indicatedScale, indicatedScale, indicatedScale);
	}

	public virtual void SetLocked(bool Locked)
	{
		CheckGadgets();
		m_Locked = Locked;
		UpdateBackground();
	}

	public bool GetIsLocked()
	{
		return m_Locked;
	}
}
