using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileButton : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
{
	private TilePalette m_Parent;

	[HideInInspector]
	public Tile.TileType m_TileType;

	private bool m_Selected;

	private NewThing m_NewIcon;

	public void SetInfo(TilePalette Parent, Tile.TileType NewType, bool New)
	{
		m_Parent = Parent;
		m_TileType = NewType;
		base.transform.Find("Image").GetComponent<Image>().sprite = Tile.GetIcon(NewType);
		string rolloverTag = Tile.m_TileInfo[(int)NewType].m_Name;
		GetComponent<ButtonRollover>().m_RolloverTag = rolloverTag;
		if (New)
		{
			GameObject original = (GameObject)Resources.Load("Prefabs/Hud/NewThing", typeof(GameObject));
			m_NewIcon = Object.Instantiate(original, default(Vector3), Quaternion.identity, base.transform).GetComponent<NewThing>();
			m_NewIcon.GetComponent<RectTransform>().localPosition = new Vector3(20f, -65f, 0f);
		}
	}

	public void SetSelected(bool Selected)
	{
		m_Selected = Selected;
		if (m_Selected)
		{
			GetComponent<Image>().color = new Color(0.5f, 0.5f, 1f);
		}
		else
		{
			GetComponent<Image>().color = new Color(1f, 1f, 1f);
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		AudioManager.Instance.StartEvent("UIOptionSelected");
		m_Parent.SetTile(m_TileType);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		AudioManager.Instance.StartEvent("UIOptionIndicated");
	}

	public void OnPointerExit(PointerEventData eventData)
	{
	}
}
