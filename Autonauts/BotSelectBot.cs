using UnityEngine;
using UnityEngine.EventSystems;

public class BotSelectBot : BaseImage
{
	public Worker m_Worker;

	private BotSelectList m_Parent;

	private BaseText m_Name;

	public void SetWorker(Worker NewBot, BotSelectList NewParent)
	{
		m_Worker = NewBot;
		m_Parent = NewParent;
		m_Name = base.transform.Find("Name").GetComponent<BaseText>();
		m_Name.SetText(m_Worker.GetWorkerName());
		UpdateColour();
	}

	private void UpdateColour()
	{
		float a = 1f;
		if (!m_Interactable)
		{
			a = 0.5f;
		}
		if (!m_Indicated)
		{
			SetColour(new Color(0.8f, 0.8f, 0.8f, a));
		}
		else
		{
			SetColour(new Color(0.6f, 0.6f, 0.6f, a));
		}
		m_Name.SetColour(new Color(0f, 0f, 0f, a));
	}

	public override void OnPointerEnter(PointerEventData eventData)
	{
		if (m_Interactable)
		{
			base.OnPointerEnter(eventData);
			if (GameStateManager.Instance.GetActualState() == GameStateManager.State.SelectWorker)
			{
				GameStateSelectWorker.Instance.SetWorker(m_Worker, Immediate: true);
			}
			UpdateColour();
		}
	}

	public override void OnPointerExit(PointerEventData eventData)
	{
		if (m_Interactable)
		{
			base.OnPointerExit(eventData);
			if (GameStateManager.Instance.GetActualState() == GameStateManager.State.SelectWorker)
			{
				GameStateSelectWorker.Instance.SetWorker(null, Immediate: true);
			}
			UpdateColour();
		}
	}

	public override void OnPointerClick(PointerEventData eventData)
	{
		base.OnPointerClick(eventData);
		if (m_Interactable)
		{
			if (GameStateManager.Instance.GetActualState() == GameStateManager.State.SelectWorker)
			{
				GameStateSelectWorker.Instance.BotSelect(m_Worker);
			}
		}
		else if (GameStateManager.Instance.GetActualState() == GameStateManager.State.SelectWorker)
		{
			GameStateSelectWorker.Instance.SelectList();
		}
	}

	public override void SetInteractable(bool Interactable)
	{
		base.SetInteractable(Interactable);
		UpdateColour();
	}
}
