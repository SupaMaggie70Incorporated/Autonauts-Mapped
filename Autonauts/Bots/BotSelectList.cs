using System.Collections.Generic;
using UnityEngine;

public class BotSelectList : MonoBehaviour
{
	private BasePanelOptions m_Area;

	private BaseScrollView m_ScrollView;

	private Transform m_Content;

	private BotSelectBot m_Prefab;

	private List<BotSelectBot> m_Bots;

	private void CheckGadgets()
	{
		if (!(m_ScrollView != null))
		{
			m_Area = base.transform.Find("Area").GetComponent<BasePanelOptions>();
			m_ScrollView = m_Area.transform.Find("BaseScrollView").GetComponent<BaseScrollView>();
			m_Content = m_ScrollView.GetContent().transform;
			m_Prefab = m_Content.Find("BotPanel").GetComponent<BotSelectBot>();
			m_Prefab.gameObject.SetActive(value: false);
			m_Bots = new List<BotSelectBot>();
		}
	}

	private void DestroyBots()
	{
		foreach (BotSelectBot bot in m_Bots)
		{
			Object.Destroy(bot.gameObject);
		}
		m_Bots.Clear();
	}

	private void CreateBots(List<TileCoordObject> NewBots)
	{
		float num = m_Prefab.GetComponent<RectTransform>().sizeDelta.y + 5f;
		float num2 = num * (float)NewBots.Count + 40f;
		if (num2 > 400f)
		{
			num2 = 400f;
		}
		m_Area.GetComponent<RectTransform>().sizeDelta = new Vector2(m_Area.GetComponent<RectTransform>().sizeDelta.x, num2);
		m_ScrollView.SetScrollSize(num * (float)NewBots.Count - 5f);
		float num3 = 0f;
		foreach (TileCoordObject NewBot in NewBots)
		{
			Worker component = NewBot.GetComponent<Worker>();
			BotSelectBot botSelectBot = Object.Instantiate(m_Prefab, m_Content);
			botSelectBot.gameObject.SetActive(value: true);
			botSelectBot.SetWorker(component, this);
			botSelectBot.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, num3);
			m_Bots.Add(botSelectBot);
			num3 -= num;
		}
	}

	private void UpdatePosition()
	{
		Vector3 vector = m_Bots[0].m_Worker.m_TileCoord.ToWorldPositionTileCentered();
		_ = 1f / (CameraManager.Instance.m_Camera.transform.position - vector).magnitude;
		Vector3 position = CameraManager.Instance.m_Camera.WorldToScreenPoint(vector);
		Vector3 vector2 = HudManager.Instance.ScreenToCanvas(position);
		RectTransform component = m_Area.GetComponent<RectTransform>();
		if (vector2.x < HudManager.Instance.m_CanvasWidth * 0.5f)
		{
			component.anchoredPosition = new Vector2(1f, 0f);
			component.pivot = new Vector2(0f, 0.5f);
			component.anchoredPosition = new Vector2(0f, 0f);
			base.transform.localPosition = new Vector3(vector2.x + 40f, HudManager.Instance.m_CanvasHeight * 0.5f);
		}
		else
		{
			component.anchoredPosition = new Vector2(1f, 0f);
			component.pivot = new Vector2(1f, 0.5f);
			component.anchoredPosition = new Vector2(0f, 0f);
			base.transform.localPosition = new Vector3(vector2.x - 40f, HudManager.Instance.m_CanvasHeight * 0.5f);
		}
	}

	public void SetBots(List<TileCoordObject> NewBots)
	{
		CheckGadgets();
		DestroyBots();
		CreateBots(NewBots);
		UpdatePosition();
	}

	public void SetInteractable(bool Interactable)
	{
		foreach (BotSelectBot bot in m_Bots)
		{
			bot.SetInteractable(Interactable);
		}
		float num = 1f;
		if (!Interactable)
		{
			num = 0.8f;
		}
		m_Area.transform.localScale = new Vector3(num, num, num);
		float a = 1f;
		if (!Interactable)
		{
			a = 0.5f;
		}
		Color backingColour = m_Area.GetPanel().GetBackingColour();
		backingColour.a = a;
		m_Area.GetPanel().SetBackingColour(backingColour);
	}
}
