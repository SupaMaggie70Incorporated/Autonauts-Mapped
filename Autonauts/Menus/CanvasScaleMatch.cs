using UnityEngine;
using UnityEngine.UI;

public class CanvasScaleMatch : MonoBehaviour
{
	private void Awake()
	{
		UpdateMatch();
	}

	public void UpdateMatch()
	{
		float num = (float)Screen.width / (float)Screen.height;
		float num2 = 1.77777779f;
		float matchWidthOrHeight = 0f;
		if (num >= num2)
		{
			matchWidthOrHeight = 1f;
		}
		GameObject.Find("Canvas").GetComponent<CanvasScaler>().matchWidthOrHeight = matchWidthOrHeight;
	}
}
