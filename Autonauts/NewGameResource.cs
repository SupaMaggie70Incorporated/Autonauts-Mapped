using UnityEngine;

public class NewGameResource : BaseGadget
{
	private PanelBacking m_Border;

	private NewGameOptions m_Parent;

	private Tile.TileType m_TileType;

	public void Set(int Count, int MinCount, Sprite NewSprite, string Rollover, Color NewColour, NewGameOptions Parent, Tile.TileType NewTileType)
	{
		m_Parent = Parent;
		m_TileType = NewTileType;
		BaseImage component = base.transform.Find("Back").GetComponent<BaseImage>();
		component.SetColour(NewColour);
		component.m_OnEnterEvent = OnEnter;
		component.m_OnExitEvent = OnExit;
		BaseText component2 = base.transform.Find("Count").GetComponent<BaseText>();
		component2.SetText(Count.ToString());
		if (Count <= MinCount / 2)
		{
			component2.SetColour(new Color(1f, 0f, 0f));
		}
		else if (Count <= MinCount)
		{
			component2.SetColour(new Color(1f, 0.5f, 0f));
		}
		base.transform.Find("Image").GetComponent<BaseImage>().SetSprite(NewSprite);
		SetRolloverFromID(Rollover);
		m_Border = base.transform.Find("Border").GetComponent<PanelBacking>();
		m_Border.gameObject.SetActive(value: false);
	}

	public void OnEnter(BaseGadget NewGadget)
	{
		m_Border.gameObject.SetActive(value: true);
		m_Parent.ResourceIndicated(m_TileType);
	}

	public void OnExit(BaseGadget NewGadget)
	{
		m_Border.gameObject.SetActive(value: false);
		m_Parent.ResourceIndicated(Tile.TileType.Total);
	}
}
