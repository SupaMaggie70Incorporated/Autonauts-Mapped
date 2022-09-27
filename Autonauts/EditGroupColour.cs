using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EditGroupColour : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
{
	private EditGroup m_Parent;

	private int m_Index;

	public void Init(EditGroup Parent, int Index)
	{
		m_Parent = Parent;
		m_Index = Index;
	}

	public void SetSelected(bool Selected)
	{
		string text = "EditGroupColour";
		if (Selected)
		{
			text = "EditGroupColourSelected";
		}
		Sprite sprite = (Sprite)Resources.Load("Textures/Hud/Tabs/" + text, typeof(Sprite));
		GetComponent<Image>().sprite = sprite;
	}

	public void OnPointerDown(PointerEventData Data)
	{
		m_Parent.SetColour(m_Index);
	}
}
