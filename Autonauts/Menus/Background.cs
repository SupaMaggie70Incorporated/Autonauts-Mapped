using UnityEngine;
using UnityEngine.UI;

public class Background : MonoBehaviour
{
	private Vector2 m_NormalSize;

	private void Awake()
	{
		m_NormalSize.x = GetComponent<Image>().mainTexture.width;
		m_NormalSize.y = GetComponent<Image>().mainTexture.height;
		OnRenderObject();
	}

	private void OnRenderObject()
	{
		Vector2 size = base.transform.parent.GetComponent<RectTransform>().rect.size;
		float num = size.x / size.y;
		float num2 = m_NormalSize.x / m_NormalSize.y;
		Vector2 sizeDelta = size;
		if (num > num2)
		{
			sizeDelta.y = sizeDelta.x * (1f / num2);
		}
		else
		{
			sizeDelta.x = sizeDelta.y * num2;
		}
		base.transform.GetComponent<RectTransform>().sizeDelta = sizeDelta;
	}

	public void SetSprite(string Name)
	{
		Sprite sprite = (Sprite)Resources.Load("Textures/Hud/" + Name, typeof(Sprite));
		GetComponent<Image>().sprite = sprite;
	}

	public void SetColour(Color NewColour)
	{
		GetComponent<Image>().color = NewColour;
	}
}
