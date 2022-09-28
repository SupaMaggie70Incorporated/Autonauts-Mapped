using UnityEngine;
using UnityEngine.UI;

public class ButtonInteractable : MonoBehaviour
{
	public void SetInteractable(bool Interactable)
	{
		GetComponent<Button>().interactable = Interactable;
		Color color = new Color(1f, 1f, 1f, 1f);
		if (!Interactable)
		{
			color = new Color(1f, 1f, 1f, 0.5f);
		}
		Image[] componentsInChildren = GetComponentsInChildren<Image>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].color = color;
		}
	}
}
