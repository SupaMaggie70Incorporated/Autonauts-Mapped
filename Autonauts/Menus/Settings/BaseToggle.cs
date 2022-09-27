using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BaseToggle : BaseGadget
{
	[HideInInspector]
	public bool m_On;

	private Image m_Image;

	private bool m_StartOn;

	protected new void Awake()
	{
		base.Awake();
	}

	private void CheckImage()
	{
		if (m_Image == null)
		{
			m_Image = base.transform.Find("Background").Find("Checkmark").GetComponent<Image>();
		}
	}

	protected new void Start()
	{
		base.Start();
		UpdateImage();
	}

	private void UpdateImage()
	{
		CheckImage();
		m_Image.gameObject.SetActive(m_On);
	}

	public override void SetAction(Action<BaseGadget> NewAction, BaseGadget NewGadget)
	{
		base.SetAction(NewAction, NewGadget);
		BaseText component = base.transform.Find("BaseText").GetComponent<BaseText>();
		component.SetAction(ToggleOption, NewGadget);
		component.m_OnClickSound = null;
	}

	public bool GetOn()
	{
		return m_On;
	}

	public void SetOn(bool On)
	{
		m_On = On;
		UpdateImage();
	}

	public void SetStartOn(bool On)
	{
		m_StartOn = On;
		SetOn(On);
	}

	public bool GetStartOn()
	{
		return m_StartOn;
	}

	public void ToggleOption(BaseGadget Target)
	{
		OnPointerClick(null);
	}

	public override void OnPointerClick(PointerEventData eventData)
	{
		if (m_Interactable)
		{
			m_On = !m_On;
			UpdateImage();
			base.OnPointerClick(eventData);
		}
	}

	public override void SetInteractable(bool Interactable)
	{
		base.SetInteractable(Interactable);
		float a = 1f;
		if (!Interactable)
		{
			a = 0.35f;
		}
		Color color = new Color(1f, 1f, 1f, a);
		Image component = base.transform.Find("Background").GetComponent<Image>();
		component.color = color;
		component.transform.Find("Checkmark").GetComponent<Image>().color = color;
	}
}
