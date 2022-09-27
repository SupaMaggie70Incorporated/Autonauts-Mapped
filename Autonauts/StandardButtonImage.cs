using UnityEngine;
using UnityEngine.EventSystems;

public class StandardButtonImage : BaseButtonImage
{
	private float m_ClickTimer;

	public override void OnPointerClick(PointerEventData eventData)
	{
		base.OnPointerClick(eventData);
		if (CanReactToClick(eventData))
		{
			StartClick();
		}
	}

	public override void SetInteractable(bool Interactable)
	{
		base.SetInteractable(Interactable);
		BaseSetInteractable(Interactable);
	}

	public override void SetSelected(bool Selected)
	{
		base.SetSelected(Selected);
		BaseSetSelected(Selected);
	}

	public override void SetIndicated(bool Indicated)
	{
		base.SetIndicated(Indicated);
		BaseSetIndicated(Indicated);
	}

	private void StartClick()
	{
		m_ClickTimer = 0.1f;
		float num = 1f;
		m_Backing.transform.localScale = new Vector3(num, num, num);
		m_Image.transform.localScale = new Vector3(num, num, num);
	}

	private void EndClick()
	{
		m_ClickTimer = 0f;
		float indicatedScale = GetIndicatedScale();
		m_Backing.transform.localScale = new Vector3(indicatedScale, indicatedScale, indicatedScale);
		m_Image.transform.localScale = new Vector3(indicatedScale, indicatedScale, indicatedScale);
	}

	private void Update()
	{
		if (!(TimeManager.Instance == null) && m_ClickTimer > 0f)
		{
			if (TimeManager.Instance.m_PauseTimeEnabled)
			{
				m_ClickTimer -= TimeManager.Instance.m_PauseDelta;
			}
			else
			{
				m_ClickTimer -= TimeManager.Instance.m_NormalDelta;
			}
			if (m_ClickTimer <= 0f)
			{
				EndClick();
			}
		}
	}
}
