using UnityEngine;

public class BadgeHud : BasePanel
{
	private Badge m_Badge;

	public void SetBadge(Badge NewBadge)
	{
		m_Badge = NewBadge;
		string sprite = "Badges/Badge" + m_Badge.m_ID;
		BaseImage component = base.transform.Find("Image").GetComponent<BaseImage>();
		component.SetSprite(sprite);
		BaseImage component2 = base.transform.Find("Silhouette").GetComponent<BaseImage>();
		component2.transform.Find("Image").GetComponent<BaseImage>().SetSprite(sprite);
		if (!m_Badge.GetIsComplete())
		{
			component2.gameObject.SetActive(value: true);
			component.SetColour(new Color(0f, 0f, 0f, 1f));
			float height = GetComponent<RectTransform>().rect.height;
			float completePercent = m_Badge.GetCompletePercent();
			float num = (height - 10f) * (1f - completePercent);
			Vector2 offsetMax = component2.GetComponent<RectTransform>().offsetMax;
			offsetMax.y = 0f - (5f + num);
			component2.GetComponent<RectTransform>().offsetMax = offsetMax;
		}
		else
		{
			component.SetColour(new Color(1f, 1f, 1f, 1f));
			component2.gameObject.SetActive(value: false);
		}
	}

	public override void SetIndicated(bool Indicated)
	{
		base.SetIndicated(Indicated);
		if (Indicated)
		{
			HudManager.Instance.ActivateBadgeRollover(Activate: true, m_Badge);
		}
		else
		{
			HudManager.Instance.ActivateBadgeRollover(Activate: false, null);
		}
	}
}
