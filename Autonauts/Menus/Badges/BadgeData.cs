using System.Collections.Generic;

public class BadgeData
{
	public List<Badge> m_Badges;

	public BadgeData()
	{
		m_Badges = new List<Badge>();
		SetUpData();
	}

	public void Save()
	{
		foreach (Badge badge in m_Badges)
		{
			badge.Save();
		}
	}

	public void Load()
	{
		foreach (Badge badge in m_Badges)
		{
			badge.Load();
		}
	}

	public void Clear()
	{
		foreach (Badge badge in m_Badges)
		{
			badge.Clear();
		}
	}

	private Badge AddBadge(Badge.ID NewID, BadgeEvent.Type NewEventType, int EventCount)
	{
		Badge badge = new Badge(NewID, NewEventType, EventCount);
		m_Badges.Add(badge);
		return badge;
	}

	private void SetUpData()
	{
		AddBadge(Badge.ID.Tutorial, BadgeEvent.Type.Tutorial, 1);
		AddBadge(Badge.ID.Berries, BadgeEvent.Type.Berries, 2000);
		AddBadge(Badge.ID.Mushrooms, BadgeEvent.Type.Mushrooms, 2000);
		AddBadge(Badge.ID.Milk, BadgeEvent.Type.Milk, 1000);
		AddBadge(Badge.ID.Wool, BadgeEvent.Type.Wool, 1000);
		AddBadge(Badge.ID.Eggs, BadgeEvent.Type.Eggs, 1000);
		AddBadge(Badge.ID.Fish, BadgeEvent.Type.Fish, 2000);
		AddBadge(Badge.ID.Tools, BadgeEvent.Type.Tools, 1000);
		AddBadge(Badge.ID.Honey, BadgeEvent.Type.Honey, 1000);
		AddBadge(Badge.ID.Colonists, BadgeEvent.Type.Colonists, 50);
		AddBadge(Badge.ID.Mining, BadgeEvent.Type.Mining, 2000);
		AddBadge(Badge.ID.Pottery, BadgeEvent.Type.Pottery, 500);
		AddBadge(Badge.ID.Clothes, BadgeEvent.Type.Clothes, 1000);
		AddBadge(Badge.ID.Plots, BadgeEvent.Type.PlotsUncovered, 200);
		AddBadge(Badge.ID.Trees, BadgeEvent.Type.TreesCut, 2000);
		AddBadge(Badge.ID.Food, BadgeEvent.Type.Food, 1000);
		AddBadge(Badge.ID.Cereal, BadgeEvent.Type.CropsCut, 2000);
		AddBadge(Badge.ID.MobileStorage, BadgeEvent.Type.MobileStorage, 500);
		AddBadge(Badge.ID.Stored, BadgeEvent.Type.AnythingStored, 5000);
		AddBadge(Badge.ID.Bots, BadgeEvent.Type.BotsMade, 200);
		AddBadge(Badge.ID.Complete, BadgeEvent.Type.GameComplete, 1);
	}
}
